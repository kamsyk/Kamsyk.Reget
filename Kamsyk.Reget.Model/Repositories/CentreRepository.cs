using Kamsyk.Reget.Model.Repositories.Interfaces;
using System.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Kamsyk.Reget.Model.ExtendedModel;
using Kamsyk.Reget.Model.Common;
using Kamsyk.Reget.Model.DataDictionary;
using System.Collections;
using Kamsyk.Reget.Model.ExtendedModel.HttpResult;

namespace Kamsyk.Reget.Model.Repositories {
    public class CentreRepository : BaseRepository<Centre>, ICentreRepository {
        #region Constant
        public const string DRAFT = "DRAFT";
        #endregion

        #region Enums
        public enum ExportPrice {
            Never = 0,
            Always = 1,
            Optional = 2
        }
        #endregion

        #region Methods
        public void SetCentreCompany() {
            var centres = (from centreDb in m_dbContext.Centre
                           select centreDb).ToList();

            foreach (Centre centre in centres) {
                if (centre.id < 0) {
                    continue;
                }

                var cg = centre.Centre_Group.ToList();
                if (cg == null || cg.Count == 0) {
                    Console.WriteLine(centre.name + " company was not found");
                    continue;
                }

                int compId = cg.ElementAt(0).company_id;
                centre.company_id = compId;
            }

            m_dbContext.SaveChanges();
        }

        public List<CentreAdminExtended> GetCentresAdminData(
            List<int> companyIds,
            string filter,
            string sort,
            int pageSize,
            int pageNr,
            string strAlwaysText,
            string strNeverText,
            string strOptionalText,
            out int rowsCount) {

            string strFilterWhere = GetFilter(
                filter,
                strAlwaysText,
                strNeverText,
                strOptionalText);
            

            string strOrder = GetOrder(sort);
            

            string sqlPure = "SELECT cd.*, ROW_NUMBER() OVER(" + strOrder + ") AS RowNum";

            string sqlPureBody = GetPureBody(companyIds, strFilterWhere);

            //Get Row count
            string selectCount = "SELECT COUNT(*) " + sqlPureBody;
            rowsCount = m_dbContext.Database.SqlQuery<int>(selectCount).Single();

            //Get Part Data
            string sqlPart = sqlPure + sqlPureBody;
            int partStart = pageSize * (pageNr - 1) + 1;
            int partStop = partStart + pageSize - 1;

            while (partStart > rowsCount) {
                partStart -= pageSize;
                partStart = partStart + pageSize - 1;
            }

            string sql = "SELECT * FROM(" + sqlPart + ") AS RegetPartData" +
                " WHERE RegetPartData.RowNum BETWEEN " + partStart + " AND " + partStop;

            var tmpCentres = m_dbContext.Database.SqlQuery<Centre>(sql).ToList();

            Hashtable htCompany = new Hashtable();
            List<CentreAdminExtended> centres = new List<CentreAdminExtended>();
            int rowIndex = (pageNr - 1) * pageSize + 1;
            foreach (var tmpCentre in tmpCentres) {
                CentreAdminExtended centre = new CentreAdminExtended();
                SetValues(tmpCentre, centre, new List<string> {
                    CentreData.ID_FIELD, CentreData.NAME_FIELD,
                    CentreData.COMPANY_ID_FIELD,
                    CentreData.EXPORT_PRICE_TO_ORDER_FIELD,
                    CentreData.MULTI_ORDERER_FIELD,
                    CentreData.OTHER_ORDERER_CAN_TAKEOVER_FIELD,
                    CentreData.IS_APPROVED_BY_REQUESTOR_FIELD,
                    CentreData.MANAGER_ID_FIELD,
                    CentreData.ADDRESS_ID_FIELD,
                    CentreData.ACTIVE_FIELD });


                //manager
                //centre.manager_name = " ";
                if (tmpCentre.manager_id != null) {
                    var participant = (from partDb in m_dbContext.Participants
                                       where partDb.id == tmpCentre.manager_id
                                       select partDb).FirstOrDefault();
                    centre.manager_name = participant.surname + " " + participant.first_name;// + " (" + participant.user_name + ")";
                }

                centre.row_index = rowIndex++;
                
                //company                 
                if (htCompany.ContainsKey(tmpCentre.company_id)) {
                    centre.company_name = htCompany[tmpCentre.company_id].ToString();
                } else {
                    var company = (from compDb in m_dbContext.Company
                                   where compDb.id == tmpCentre.company_id
                                   select compDb).FirstOrDefault();
                    centre.company_name = company.country_code;
                    htCompany.Add(company.id, company.country_code);
                }

                //Address
               // centre.address_text = " ";
                if (tmpCentre.address_id != null) {
                    var address = new AddressRepository().GetAddressDataById((int)tmpCentre.address_id);
                    if (address != null) {
                        centre.address_text = address.address_text;
                    }
                } 
                //if (tmpCentre.Address != null) {
                //    centre.address_text = AddressRepository.GetFullAddress(tmpCentre.Address);
                //}

                //export price tex
                centre.export_price_text = GetExportToPriceText(
                    tmpCentre.export_price_to_order,
                    strAlwaysText,
                    strNeverText,
                    strOptionalText);
                
                //multi orderer
                if(tmpCentre.multi_orderer == null) {
                    centre.multi_orderer = false;
                }

                //can take over
                if (tmpCentre.other_orderer_can_takeover == null) {
                    centre.other_orderer_can_takeover = false;
                }

                //can participant approve
                if (tmpCentre.is_approved_by_requestor == null) {
                    centre.is_approved_by_requestor = false;
                }

                centres.Add(centre);


            }

            return centres;

            
        }

        public List<Centre> GetCentresReport(
            List<int> companyIds,
            string filter,
            string sort,
            string strAlwaysText,
            string strNeverText,
            string strOptionalText) {

            string strFilterWhere = GetFilter(
                filter,
                strAlwaysText,
                strNeverText,
                strOptionalText);

            string strOrder = GetOrder(sort);

            string sqlPure = "SELECT cd.* ";
            string sqlPureBody = GetPureBody(companyIds, strFilterWhere);
            sqlPureBody += strOrder;

            string sql = sqlPure + sqlPureBody;

            var centres = m_dbContext.Database.SqlQuery<Centre>(sql).ToList();

            //var centres = (from cd in m_dbContext.Centre
            //                    where companyIds.Contains(cd.company_id) &&
            //                    cd.name != (CentreRepository.DRAFT) 
            //                    select cd).ToList();

            return centres;
        }

        private string GetPureBody(List<int> companyIds, string strFilterWhere) {
            string sqlPureBody = " FROM " + CentreData.TABLE_NAME + " cd" +
                " INNER JOIN " + CompanyData.TABLE_NAME + " cmpd" +
                " ON cd." + CentreData.COMPANY_ID_FIELD + "=cmpd." + CompanyData.ID_FIELD +
                " LEFT OUTER JOIN " + ParticipantsData.TABLE_NAME + " pd" +
                " ON cd." + CentreData.MANAGER_ID_FIELD + "=pd." + ParticipantsData.ID_FIELD +
                " LEFT OUTER JOIN " + AddressData.TABLE_NAME + " ad" +
                " ON cd." + CentreData.ADDRESS_ID_FIELD + "=ad." + AddressData.ID_FIELD +
                " WHERE cd." + CentreData.COMPANY_ID_FIELD + " IN" + GetSqlIn(companyIds) +
                " AND cd." + CentreData.NAME_FIELD + "<>'" + CentreRepository.DRAFT + "'";
            sqlPureBody += strFilterWhere;

            return sqlPureBody;
        }


        private string GetFilter(
            string filter,
            string strAlwaysText,
            string strNeverText,
            string strOptionalText) {

            string strFilterWhere = "";
            if (!String.IsNullOrEmpty(filter)) {
                string[] filterItems = filter.Split(UrlParamDelimiter.ToCharArray());
                foreach (string filterItem in filterItems) {
                    string[] strItemProp = filterItem.Split(UrlParamValueDelimiter.ToCharArray());
                    strFilterWhere += " AND ";

                    string columnName = strItemProp[0].Trim().ToUpper();
                    if (columnName == CentreData.NAME_FIELD.Trim().ToUpper()) {
                        strFilterWhere += "cd." + CentreData.NAME_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'";
                    } else if (columnName == "company_name".Trim().ToUpper()) {
                        int companyId = new CompanyRepository().GetCompanyIdByName(strItemProp[1]);
                        strFilterWhere += "cd." + CentreData.COMPANY_ID_FIELD + "=" + companyId;
                    } else if (columnName == "export_price_text".Trim().ToUpper()) {
                        if (strItemProp[1].Trim() == strAlwaysText) {
                            strFilterWhere += "cd." + CentreData.EXPORT_PRICE_TO_ORDER_FIELD + "=" + (int)ExportPrice.Always;
                        } else if (strItemProp[1].Trim() == strOptionalText) {
                            strFilterWhere += "cd." + CentreData.EXPORT_PRICE_TO_ORDER_FIELD + "=" + (int)ExportPrice.Optional;
                        } else if (strItemProp[1].Trim() == strNeverText) {
                            strFilterWhere += "(cd." + CentreData.EXPORT_PRICE_TO_ORDER_FIELD + "=" + (int)ExportPrice.Never +
                                " OR " + "cd." + CentreData.EXPORT_PRICE_TO_ORDER_FIELD + " IS NULL" + ")";
                        }
                    } else if (columnName == "manager_name".Trim().ToUpper()) {
                        strFilterWhere += "(pd." + ParticipantsData.SURNAME_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'" +
                            " OR pd." + ParticipantsData.FIRST_NAME_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'" +
                            " OR pd." + ParticipantsData.USER_SEARCH_KEY_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%')";
                    } else if (columnName == AddressData.ADDRESS_TEXT_FIELD.Trim().ToUpper()) {
                        strFilterWhere += "ad." + AddressData.ADDRESS_TEXT_FIELD + " LIKE '%" + strItemProp[1] + "%'";
                    } else if (columnName == CentreData.MULTI_ORDERER_FIELD.Trim().ToUpper()) {
                        if (strItemProp[1].Trim().ToLower() == "true") {
                            strFilterWhere += "cd." + CentreData.MULTI_ORDERER_FIELD + "=1";
                        } else {
                            strFilterWhere += "(cd." + CentreData.MULTI_ORDERER_FIELD + "=0" +
                                " OR cd." + CentreData.MULTI_ORDERER_FIELD + " IS NULL)";
                        }
                    } else if (columnName == CentreData.OTHER_ORDERER_CAN_TAKEOVER_FIELD.Trim().ToUpper()) {
                        if (strItemProp[1].Trim().ToLower() == "true") {
                            strFilterWhere += "cd." + CentreData.OTHER_ORDERER_CAN_TAKEOVER_FIELD + "=1";
                        } else {
                            strFilterWhere += "(cd." + CentreData.OTHER_ORDERER_CAN_TAKEOVER_FIELD + "=0" +
                                " OR cd." + CentreData.OTHER_ORDERER_CAN_TAKEOVER_FIELD + " IS NULL)";
                        }
                    } else if (columnName == CentreData.IS_APPROVED_BY_REQUESTOR_FIELD.Trim().ToUpper()) {
                        if (strItemProp[1].Trim().ToLower() == "true") {
                            strFilterWhere += "cd." + CentreData.IS_APPROVED_BY_REQUESTOR_FIELD + "=1";
                        } else {
                            strFilterWhere += "(cd." + CentreData.IS_APPROVED_BY_REQUESTOR_FIELD + "=0" +
                                " OR cd." + CentreData.IS_APPROVED_BY_REQUESTOR_FIELD + " IS NULL)";
                        }
                    } else if (columnName == CentreData.ACTIVE_FIELD.Trim().ToUpper()) {
                        if (strItemProp[1].Trim().ToLower() == "true") {
                            strFilterWhere += "cd." + CentreData.ACTIVE_FIELD + "=1";
                        } else {
                            strFilterWhere += "(cd." + CentreData.ACTIVE_FIELD + "=0" +
                                " OR cd." + CentreData.ACTIVE_FIELD + " IS NULL)";
                        }
                    }
                    //if (columnName == ParticipantsData.USER_NAME_FIELD.Trim().ToUpper()) {
                    //    strFilterWhere += "pd." + ParticipantsData.USER_NAME_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'";
                    //}
                    //if (columnName == ParticipantsData.EMAIL_FIELD.Trim().ToUpper()) {
                    //    strFilterWhere += "pd." + ParticipantsData.EMAIL_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'";
                    //}
                    //if (columnName == ParticipantsData.PHONE_FIELD.Trim().ToUpper()) {
                    //    strFilterWhere += "pd." + ParticipantsData.PHONE_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'";
                    //}
                    //if (columnName == ParticipantsData.ACTIVE_FIELD.Trim().ToUpper()) {
                    //    if (strItemProp[1].Trim().ToLower() == "true") {
                    //        strFilterWhere += "pd." + ParticipantsData.ACTIVE_FIELD + "=1";
                    //    } else {
                    //        strFilterWhere += "pd." + ParticipantsData.ACTIVE_FIELD + "=0";
                    //    }
                    //}

                }
            }

            return strFilterWhere;
        }

        private string GetOrder(string sort) {
            string strOrder = "ORDER BY cd." + CentreData.ID_FIELD;

            if (!String.IsNullOrEmpty(sort)) {
                strOrder = "";
                string[] sortItems = sort.Split(UrlParamDelimiter.ToCharArray());
                foreach (string sortItem in sortItems) {
                    string[] strItemProp = sortItem.Split(UrlParamValueDelimiter.ToCharArray());
                    if (strOrder.Length > 0) {
                        strOrder += ", ";
                    }
                    //strOrder += strItemProp[0] + " " + strItemProp[1];
                    if (strItemProp[0] == "company_name") {
                        strOrder = "cmpd." + CompanyData.COUNTRY_CODE_FIELD + " " + strItemProp[1];
                    } else if (strItemProp[0] == "export_price_text") {
                        strOrder += "cd." + CentreData.EXPORT_PRICE_TO_ORDER_FIELD + " " + strItemProp[1];
                    } else if (strItemProp[0] == "manager_name") {
                        strOrder += "pd." + ParticipantsData.SURNAME_FIELD + " " + strItemProp[1] +
                            ",pd." + ParticipantsData.FIRST_NAME_FIELD + " " + strItemProp[1];
                    } else if (strItemProp[0] == "address_text") {
                        strOrder += "ad." + AddressData.COMPANY_NAME_FIELD + " " + strItemProp[1] +
                           ",ad." + AddressData.STREET_FIELD + " " + strItemProp[1] +
                           ",ad." + AddressData.CITY_FIELD + " " + strItemProp[1] +
                           ",ad." + AddressData.ZIP_FIELD + " " + strItemProp[1];
                    } else {
                        strOrder += "cd." + strItemProp[0] + " " + strItemProp[1];
                    }
                }

                strOrder = " ORDER BY " + strOrder;
            }

            return strOrder;
        }

        private string GetExportToPriceText(
            int? exportPriceToOrder, 
            string strAlwaysText,
            string strNeverText,
            string strOptionalText) {

            if (exportPriceToOrder == (int)ExportPrice.Always) {
                return strAlwaysText;
            } else if (exportPriceToOrder == (int)ExportPrice.Optional) {
                return strOptionalText;
            } else if (exportPriceToOrder == (int)ExportPrice.Never || exportPriceToOrder == null) {
                return strNeverText;
            } else {
                throw new Exception("Export To Price was not found");
            }
        }

        private ExportPrice GetExportToPriceFromText(
            string exportPriceToOrderText,
            string strAlwaysText,
            string strNeverText,
            string strOptionalText) {

            if (exportPriceToOrderText == strAlwaysText) {
                return ExportPrice.Always;
            } else if (exportPriceToOrderText == strNeverText) {
                return ExportPrice.Never;
            } else if (exportPriceToOrderText == strOptionalText) {
                return ExportPrice.Optional;
            } else {
                throw new Exception("Export To Price was not found");
            }
        }
                
        public int SaveCentreData(
            CentreAdminExtended modifCentre, 
            int userId,
            string strAlwaysText,
            string strNeverText,
            string strOptionalText,
            out List<string> msg) {
            msg = new List<string>();

            HttpResult httpResult = new HttpResult();

            var companies = (from compDb in m_dbContext.Company
                                     where compDb.country_code == modifCentre.company_name
                                     select compDb).FirstOrDefault();
            int compId = companies.id;

            //Check unique key - duplicity
            if (modifCentre.id < 0) {
                //new 
                var dbUCentre = (from cd in m_dbContext.Centre
                                 where cd.name == modifCentre.name &&
                                 cd.company_id == compId
                                 select cd).FirstOrDefault();

                if (dbUCentre != null) {
                    //duplicity
                    msg.Add(DUPLICITY);
                    return -1;
                }
            } else {
                //existing 
                var dbUCentre = (from cd in m_dbContext.Centre
                                 where cd.name == modifCentre.name &&
                                 cd.id != modifCentre.id
                                 select cd).FirstOrDefault();

                if (dbUCentre != null) {
                    //duplicity
                    msg.Add(DUPLICITY);
                    return -1;
                }
            }


            using (TransactionScope transaction = new TransactionScope()) {
                try {
                    var dbCentre = new Centre();

                    if (modifCentre.id >= 0) {

                        dbCentre = (from cd in m_dbContext.Centre
                                     where cd.id == modifCentre.id
                                     select cd).FirstOrDefault();

                    }

                    dbCentre.name = modifCentre.name;
                    dbCentre.export_price_to_order = (int)GetExportToPriceFromText(
                        modifCentre.export_price_text,
                        strAlwaysText,
                        strNeverText,
                        strOptionalText);
                    dbCentre.multi_orderer = modifCentre.multi_orderer;
                    dbCentre.other_orderer_can_takeover = modifCentre.other_orderer_can_takeover;
                    dbCentre.is_approved_by_requestor = modifCentre.is_approved_by_requestor;

                    
                    dbCentre.company_id = compId;

                    dbCentre.manager_id = modifCentre.manager_id;

                    if (String.IsNullOrEmpty(modifCentre.address_text)) {
                        dbCentre.address_id = null;
                    } else {
                        string[] addItems = modifCentre.address_text.Split(',');
                    }

                    var address = (from addDb in m_dbContext.Address
                                     where addDb.address_text == modifCentre.address_text
                                     && addDb.company_id == compId
                                   select addDb).FirstOrDefault();
                    if (address == null) {
                        dbCentre.address_id = null;
                    } else {
                        int addressId = address.id;
                        dbCentre.address_id = addressId;
                    }

                    dbCentre.active = modifCentre.active;
                    dbCentre.modify_user = userId;
                    dbCentre.modify_date = DateTime.Now;

                    if (modifCentre.id < 0) {

                        var lastCentr = (from cd in m_dbContext.Centre
                                        orderby cd.id descending
                                        select cd).Take(1).FirstOrDefault();

                        int lastId = -1;
                        if (lastCentr != null) {
                            lastId = lastCentr.id;
                        }

                        //int lastId = lastAddr.id;
                        lastId++;
                        dbCentre.id = lastId;
                        //dbParticipant.user_name = modifParticipant.user_name;


                        m_dbContext.Centre.Add(dbCentre);
                    }

                    SaveChanges();
                    transaction.Complete();

                    return dbCentre.id;
                } catch (Exception ex) {
                    throw ex;
                }
            }
        }

        public void DeleteCentreByName(string strName) {
            List<Centre> centres = (from centreDb in m_dbContext.Centre
                                    where centreDb.name == strName
                                    select centreDb).ToList();
            foreach (var centre in centres) {
                DeleteCentre(centre.id);
            }
        }

        public bool DeleteCentre(int centreId) {
            bool isCentreUsed = IsCentreUsed(centreId);

            using (TransactionScope transaction = new TransactionScope()) {
                try {
                    var centre = (from suppDb in m_dbContext.Centre
                                where suppDb.id == centreId
                                select suppDb).FirstOrDefault();
                    if (isCentreUsed) {
                        centre.active = false;
                    } else {
                        
                        //Participants + Requestor role
                        var participants = (from partDb in m_dbContext.Participants
                                            where partDb.centre_id == centreId
                                            select partDb).ToList();
                        if (participants != null) {
                            Centre_Group cg = null;
                            if (centre.Centre_Group != null && centre.Centre_Group.Count > 0) {
                                cg = centre.Centre_Group.ElementAt(0);
                            }
                            foreach (var participant in participants) {
                                participant.centre_id = null;
                                if (cg != null) {
                                    new CentreGroupRepository().DeleteParticipantRequestorRole(cg, participant.id, DataNulls.INT_NULL, centre.id);
                                }
                            }
                        }

                        //delete requestor centre
                        if (centre.Requestor_Centre != null) {
                            for (int i = centre.Requestor_Centre.Count - 1; i >= 0; i--) {
                                Centre_Group cg = null;
                                if (centre.Centre_Group != null && centre.Centre_Group.Count > 0) {
                                    cg = centre.Centre_Group.ElementAt(0);
                                    new CentreGroupRepository().DeleteParticipantRequestorRole(cg, centre.Requestor_Centre.ElementAt(i).id, DataNulls.INT_NULL, centre.id);
                                }
                                centre.Requestor_Centre.Remove(centre.Requestor_Centre.ElementAt(i));
                            }
                        }

                        //delete asset requestor centre
                        if (centre.Asset_Requestor_Centre != null) {
                            for (int i = centre.Asset_Requestor_Centre.Count - 1; i >= 0; i--) {
                                centre.Asset_Requestor_Centre.Remove(centre.Asset_Requestor_Centre.ElementAt(i));
                            }
                        }

                        //ship to address - will be eliminated
                        if (centre.Ship_To_Address != null) {
                            for (int i = centre.Ship_To_Address.Count - 1; i >= 0; i--) {
                                centre.Ship_To_Address.Remove(centre.Ship_To_Address.ElementAt(i));
                            }
                        }

                        //purchase group requestor
                        if (centre.PurchaseGroup_Requestor != null) {
                            for (int i = centre.PurchaseGroup_Requestor.Count - 1; i >= 0; i--) {
                                centre.PurchaseGroup_Requestor.Remove(centre.PurchaseGroup_Requestor.ElementAt(i));
                            }
                        }

                        //delete centre_centregroup
                        if (centre.Centre_Group != null) {
                            for (int i = centre.Centre_Group.Count - 1; i >= 0; i--) {
                                centre.Centre_Group.Remove(centre.Centre_Group.ElementAt(i));
                            }
                        }

                        //delete centre_assetcentregroup
                        if (centre.Asset_Centre_Group != null) {
                            for (int i = centre.Asset_Centre_Group.Count - 1; i >= 0; i--) {
                                centre.Asset_Centre_Group.Remove(centre.Asset_Centre_Group.ElementAt(i));
                            }
                        }

                        m_dbContext.Centre.Remove(centre);
                    }

                    SaveChanges();
                    transaction.Complete();

                    return (!isCentreUsed);
                } catch (Exception ex) {
                    throw ex;
                }
            }
        }

        private bool IsCentreUsed(int centreId) {
            var request = (from requestDb in m_dbContext.Request_Event
                           where requestDb.request_centre_id == centreId
                           select new { id = requestDb.id }).FirstOrDefault();

            if (request != null) {
                return true;
            }

            var ass_request = (from requestDb in m_dbContext.Asset_Request_Event
                           where requestDb.request_centre_id == centreId
                           select new { id = requestDb.id }).FirstOrDefault();

            if (ass_request != null) {
                return true;
            }

            return false;
        }

        public Centre GetCentreById(int id) {
            if (id == DataNulls.INT_NULL) {
                return null;
            }

            var centre = GetFirstOrDefault(p => p.id == id);
            
            if (centre == null) {
                return null;
            }

            return (Centre)centre;
        }

        public Centre GetCentreByName(string strName) {
            if (String.IsNullOrEmpty(strName)) {
                return null;
            }

            //var centre = GetFirstOrDefault(p => p.name == strName);
            var centre = (from cd in m_dbContext.Centre
                          where cd.name == strName
                          select cd).FirstOrDefault();

            if (centre == null) {
                return null;
            }

            return centre;
        }

        public void RemoveCentreFromCg(int centreId) {
            var centre = GetCentreById(centreId);
            for (int i = centre.Centre_Group.Count - 1; i >= 0; i--) {
                var cg = centre.Centre_Group.ElementAt(i);


                centre.Centre_Group.Remove(cg);
                m_dbContext.Entry(centre).State = EntityState.Modified;
            }

            m_dbContext.SaveChanges();
        }

        public string GetCentreNameById(int id) {
            if (id == DataNulls.INT_NULL) {
                return null;
            }

            CentreName centreName = (from cDb in m_dbContext.Centre
                              where cDb.id == id
                              select new CentreName { name = cDb.name }).FirstOrDefault();

            if (centreName == null) {
                return null;
            }

            return centreName.name;
        }
        #endregion
    }

    public class CentreName {
        public string name { get; set; }
    }
}

