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
using static Kamsyk.Reget.Model.Repositories.UserRepository;
using Kamsyk.Reget.Model.ExtendedModel.Supplier;
using Kamsyk.Reget.Model.ExtendedModel.HttpResult;

namespace Kamsyk.Reget.Model.Repositories {
    public class SupplierRepository : BaseRepository<Supplier>, ISupplierRepository {
        #region Constant
        public const string DUPLICITY = "DUPLICITY";
        public const string MANUAL_NOT_ALLOWED = "MANUAL_NOT_ALLOWED";
        #endregion

        #region Enums

        #endregion

        #region Methods
        public List<SupplierGridData> GetSupplierData(
            int companyId,
            string filter,
            string sort,
            int pageSize,
            int pageNr,
            out int rowsCount) {

            string strFilterWhere = "";

            if (!String.IsNullOrEmpty(filter)) {
                string[] filterItems = filter.Split(UrlParamDelimiter.ToCharArray());
                foreach (string filterItem in filterItems) {
                    string[] strItemProp = filterItem.Split(UrlParamValueDelimiter.ToCharArray());
                    strFilterWhere += " AND ";

                    string columnName = strItemProp[0].Trim().ToUpper();
                    if (columnName == SupplierData.SUPP_NAME_FIELD.Trim().ToUpper()) {
                        strFilterWhere += "sd." + SupplierData.SUPP_NAME_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'";
                    }
                    if (columnName == SupplierData.SUPPLIER_ID_FIELD.Trim().ToUpper()) {
                        strFilterWhere += "sd." + SupplierData.SUPPLIER_ID_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'";
                    }
                    if (columnName == "street".ToUpper()) {
                        strFilterWhere += "(sd." + SupplierData.STREET_PART1_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'"
                             + " OR sd." + SupplierData.STREET_PART2_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%')";
                    }
                    if (columnName == SupplierData.CITY_FIELD.Trim().ToUpper()) {
                        strFilterWhere += "sd." + SupplierData.CITY_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'";
                    }
                    if (columnName == SupplierData.ZIP_FIELD.Trim().ToUpper()) {
                        strFilterWhere += "sd." + SupplierData.ZIP_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'";
                    }
                    if (columnName == SupplierData.COUNTRY_FIELD.Trim().ToUpper()) {
                        strFilterWhere += "sd." + SupplierData.COUNTRY_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'";
                    }
                    if (columnName == SupplierData.CONTACT_PERSON_FIELD.Trim().ToUpper()) {
                        strFilterWhere += "sd." + SupplierData.CONTACT_PERSON_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'";
                    }
                    if (columnName == SupplierData.PHONE_FIELD.Trim().ToUpper()) {
                        strFilterWhere += "sd." + SupplierData.PHONE_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'";
                    }
                    if (columnName == SupplierData.EMAIL_FIELD.Trim().ToUpper()) {
                        strFilterWhere += "sd." + SupplierData.EMAIL_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'";
                    }
                    if (columnName == SupplierData.ACTIVE_FIELD.Trim().ToUpper()) {
                        if (strItemProp[1].Trim().ToLower() == "true") {
                            strFilterWhere += "sd." + SupplierData.ACTIVE_FIELD + "=1";
                        } else {
                            strFilterWhere += "sd." + SupplierData.ACTIVE_FIELD + "=0";
                        }
                    }
                }
            }

            string strOrder = "ORDER BY sd." + AddressData.ID_FIELD;
            
            if (!String.IsNullOrEmpty(sort)) {
                strOrder = "";
                string[] sortItems = sort.Split(UrlParamDelimiter.ToCharArray());
                foreach (string sortItem in sortItems) {
                    string[] strItemProp = sortItem.Split(UrlParamValueDelimiter.ToCharArray());
                    if (strOrder.Length > 0) {
                        strOrder += ", ";
                    }

                    if (strItemProp[0] == "street") {
                        strOrder = "sd." + SupplierData.STREET_PART1_FIELD + ", sd." + SupplierData.STREET_PART2_FIELD + " " + strItemProp[1]; ;
                    } else {
                        strOrder += "sd." + strItemProp[0] + " " + strItemProp[1];
                    }
                }

                strOrder = " ORDER BY " + strOrder;
            }

            string sqlPure = "SELECT sd.*, ROW_NUMBER() OVER(" + strOrder + ") AS RowNum";

            string sqlPureBody = " FROM " + SupplierData.TABLE_NAME + " sd" +
                 " WHERE sd." + SupplierData.SUPPLIER_GROUP_ID_FIELD + "=" + companyId;
            sqlPureBody += strFilterWhere;

            //Get Row count
            string selectCount = "SELECT COUNT(" + SupplierData.ID_FIELD + ") " + sqlPureBody;
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

            var tmpSuppliers = m_dbContext.Database.SqlQuery<Supplier>(sql).ToList();

            
            List<SupplierGridData> suppliers = new List<SupplierGridData>();
            int rowIndex = (pageNr - 1) * pageSize + 1;
            foreach (var tmpSupplier in tmpSuppliers) {
                SupplierGridData supplier = new SupplierGridData();
                SetValues(tmpSupplier, supplier, new List<string> {
                    SupplierData.ID_FIELD,
                    SupplierData.SUPP_NAME_FIELD,
                    SupplierData.SUPPLIER_ID_FIELD,
                    SupplierData.STREET_PART1_FIELD,
                    SupplierData.CITY_FIELD,
                    SupplierData.ZIP_FIELD,
                    SupplierData.COUNTRY_FIELD,
                    SupplierData.CONTACT_PERSON_FIELD,
                    SupplierData.PHONE_FIELD,
                    SupplierData.EMAIL_FIELD,
                    SupplierData.ACTIVE_FIELD
                });

                supplier.row_index = rowIndex++;
                supplier.company_id = companyId;
                //supplier.street = "";
                //if (!String.IsNullOrEmpty(tmpSupplier.street_part1)) {
                //    supplier.street_p += tmpSupplier.street_part1;
                //}
                //if (!String.IsNullOrEmpty(tmpSupplier.street_part2)) {
                //    if (supplier.street.Length > 0) {
                //        supplier.street += ", ";
                //    }
                //    supplier.street += tmpSupplier.street_part2;
                //}

                suppliers.Add(supplier);

            }

            return suppliers;
            
        }

        public List<Supplier> GetSuppliersReport(int companyId) {
            var company = (from cd in m_dbContext.Company
                           where cd.id == companyId
                           select cd).FirstOrDefault();
            int supGroupId = (int)company.supplier_group_id;

            var suppliers = (from sd in m_dbContext.Supplier
                           where sd.supplier_group_id == supGroupId
                           select sd).ToList();

            return suppliers;
        }

        public List<int> GetActiveSuppliersIds(int companyId) {
            var company = (from cd in m_dbContext.Company
                           where cd.id == companyId
                           select cd).FirstOrDefault();
            int supGroupId = (int)company.supplier_group_id;

            var supplierIds = (from sd in m_dbContext.Supplier
                             where sd.supplier_group_id == supGroupId &&
                             sd.active == true
                             select new { id = sd.id }).ToList();

            List<int> ids = new List<int>();
            foreach (var id in supplierIds) {
                ids.Add(id.id);
            }

            return ids;
        }

        
        

        public int GetCentreGroupId(int companyId) {
            var company = (from cd in m_dbContext.Company
                             where cd.id == companyId
                           select cd).FirstOrDefault();

            return (int)company.supplier_group_id;
        }

        
        public static string GetFullAddress(Address address) {
            string strFullAddress = address.company_name;
            if (!String.IsNullOrEmpty(address.street)) {
                strFullAddress += ", " + address.street;
            }
            if (!String.IsNullOrEmpty(address.city)) {
                strFullAddress += ", " + address.city;
            }
            if (!String.IsNullOrEmpty(address.zip)) {
                strFullAddress += ", " + address.zip;
            }

            return strFullAddress;
        }

        public Supplier GetSupplierByCompanyIdSuppIdName(int companyId, string supplierId, string suppName, out int supplierGroupId) {
            var company = (from cd in m_dbContext.Company
                           where cd.id == companyId
                           select cd).FirstOrDefault();
            int tmpSupplierGroupId = (int)company.supplier_group_id;
            supplierGroupId = tmpSupplierGroupId;

            if (String.IsNullOrEmpty(supplierId) && String.IsNullOrEmpty(suppName)) {
                return null;
            }

            List<Supplier> suppliers = null;

            if (String.IsNullOrEmpty(supplierId)) {
                suppliers = (from sd in m_dbContext.Supplier
                             where sd.supplier_group_id == tmpSupplierGroupId &&
                             (sd.supp_name.Trim().ToLower() == suppName.Trim().ToLower())
                             select sd).ToList();
            } else {
                suppliers = (from sd in m_dbContext.Supplier
                             where sd.supplier_group_id == tmpSupplierGroupId &&
                             (sd.supplier_id.Trim() == supplierId.Trim())
                             select sd).ToList();
            }

            //if (String.IsNullOrEmpty(supplierId)) {
            //    string suppNameWoDiac = RemoveDiacritics(suppName.Trim().ToLower());
            //    suppliers = (from sd in m_dbContext.Supplier
            //                 where sd.supplier_group_id == tmpSupplierGroupId &&
            //                 (sd.supplier_search_key.Trim().ToLower() == suppNameWoDiac)
            //                 select sd).ToList();
            //} else {
            //    string suppIdWodiac = RemoveDiacritics(supplierId.Trim());
            //    suppliers = (from sd in m_dbContext.Supplier
            //                 where sd.supplier_group_id == tmpSupplierGroupId &&
            //                 (sd.supplier_search_key.Trim() == suppIdWodiac)
            //                 select sd).ToList();
            //}
            //} else if (String.IsNullOrEmpty(supplierId) && !String.IsNullOrEmpty(suppName)) {
            //    suppliers = (from sd in m_dbContext.Supplier
            //                     where sd.supplier_group_id == tmpSupplierGroupId &&
            //                     (sd.supp_name == suppName)
            //                     select sd).ToList();
            //}

            //if (!String.IsNullOrEmpty(supplierId) && String.IsNullOrEmpty(suppName)) {
            //    suppliers = (from sd in m_dbContext.Supplier
            //                 where sd.supplier_group_id == tmpSupplierGroupId &&
            //                 (sd.supplier_id == supplierId)
            //                 select sd).ToList();
            //}

            //if (!String.IsNullOrEmpty(supplierId) && !String.IsNullOrEmpty(suppName)) {
            //    suppliers = (from sd in m_dbContext.Supplier
            //                 where sd.supplier_group_id == tmpSupplierGroupId &&
            //                 (sd.supplier_id == supplierId || sd.supp_name == suppName)
            //                 select sd).ToList();
            //}

            if (suppliers == null) {
                return null;
            }

            if (suppliers.Count == 1) {
                return suppliers.ElementAt(0);
            }

            //if there are found more then 1 suplier it returns null and then the duplicity error is thrown
            return null;
        }

        public List<Supplier> GetSupplierByCompany(int companyId) {
            var company = (from cd in m_dbContext.Company
                           where cd.id == companyId
                           select cd).FirstOrDefault();
            int tmpSupplierGroupId = (int)company.supplier_group_id;
            int supplierGroupId = tmpSupplierGroupId;

            var suppliers = (from sd in m_dbContext.Supplier
                            where sd.supplier_group_id == tmpSupplierGroupId 
                            orderby sd.supp_name
                            select sd).ToList();

            return suppliers;
        }

        public int SaveSupplier(
            Supplier modifSupplier,
            int userId,
            int companyId,
            bool isAutoImport,
            out List<string> msg) {
            msg = new List<string>();

            HttpResult httpResult = new HttpResult();

            var compDb = (from cd in m_dbContext.Company
                         where cd.id == companyId 
                         select cd).FirstOrDefault();
            int suppGroupId = (int)compDb.supplier_group_id;

            if (!isAutoImport && compDb.is_supplier_maintenance_allowed != true) {
                msg.Add(MANUAL_NOT_ALLOWED);
                return -1;
            }

            //Check unique key - duplicity
            if (modifSupplier.id < 0) {
                //new 
                var dbSupplier = (from sd in m_dbContext.Supplier
                                 where sd.supp_name == modifSupplier.supp_name &&
                                 sd.supplier_group_id == modifSupplier.supplier_group_id
                                 && (sd.supplier_id == modifSupplier.supplier_id || sd.supplier_id == null || sd.supplier_id == "")
                                  select sd).FirstOrDefault();

                if (dbSupplier != null) {
                    //duplicity
                    msg.Add(DUPLICITY);
                    return -1;
                }
            } else {
                //existing 
                var dbSupplier = (from sd in m_dbContext.Supplier
                                 where sd.supp_name == modifSupplier.supp_name &&
                                 sd.id != modifSupplier.id &&
                                 sd.supplier_group_id == modifSupplier.supplier_group_id
                                  select sd).FirstOrDefault();

                if (dbSupplier != null) {
                    //duplicity
                    
                    msg.Add(DUPLICITY);
                    return -1;
                }
            }


            using (TransactionScope transaction = new TransactionScope()) {
                try {
                    var dbSupplier = new Supplier();

                    if (modifSupplier.id >= 0) {

                        dbSupplier = (from cd in m_dbContext.Supplier
                                    where cd.id == modifSupplier.id
                                    select cd).FirstOrDefault();

                    }

                    dbSupplier.supp_name  = modifSupplier.supp_name;
                    dbSupplier.supplier_id = modifSupplier.supplier_id;
                    dbSupplier.street_part1 = modifSupplier.street_part1;
                    dbSupplier.city = modifSupplier.city;
                    dbSupplier.zip = modifSupplier.zip;
                    dbSupplier.country = modifSupplier.country;
                    dbSupplier.contact_person = modifSupplier.contact_person;
                    dbSupplier.phone = modifSupplier.phone;
                    dbSupplier.email = modifSupplier.email;
                    dbSupplier.active = modifSupplier.active;

                    dbSupplier.supplier_group_id = suppGroupId;

                    dbSupplier.supplier_search_key = GeSupplierSearchKey(dbSupplier);


                    if (modifSupplier.id < 0) {

                        var lastSupp = (from cd in m_dbContext.Supplier
                                         orderby cd.id descending
                                         select cd).Take(1).FirstOrDefault();

                        int lastId = -1;
                        if (lastSupp != null) {
                            lastId = lastSupp.id;
                        }

                        //int lastId = lastAddr.id;
                        lastId++;
                        dbSupplier.id = lastId;
                        
                        m_dbContext.Supplier.Add(dbSupplier);
                    }

                    SaveChanges();
                    transaction.Complete();

                    return dbSupplier.id;
                } catch (Exception ex) {
                    throw ex;
                }
            }
        }

        public void SaveSupplierSearchKey(Supplier modifSupplier) {
            var dbSupplier = (from cd in m_dbContext.Supplier
                          where cd.id == modifSupplier.id
                          select cd).FirstOrDefault();

            dbSupplier.supplier_search_key = GeSupplierSearchKey(dbSupplier);
            //m_dbContext.Entry(dbSupplier).State = EntityState.Modified;
            SaveChanges();
        }

        private string GeSupplierSearchKey(Supplier supplier) {
            string searchKey = "";

            if (!String.IsNullOrEmpty(supplier.supp_name)) {
                searchKey = RemoveDiacriticsWhiteSpace(supplier.supp_name.Replace(" ", "")).Trim().ToLower();
            }

            if (!String.IsNullOrEmpty(supplier.supplier_id)) {
                searchKey += "(" + RemoveDiacriticsWhiteSpace(supplier.supplier_id.Replace(" ", "")).Trim().ToLower() + ")";
            }

            return searchKey;
        }

        public void DeactivateSupplier(int supplierId) {
            //var supplier = (from sd in m_dbContext.Supplier
            //                where sd.id == supplierId
            //                select sd).FirstOrDefault();

            //supplier.active = false;

            //m_dbContext.SaveChanges();
            string sql = "UPDATE " + SupplierData.TABLE_NAME +
                " SET " + SupplierData.ACTIVE_FIELD + "=0" +
                " WHERE " + SupplierData.ID_FIELD + "=" + supplierId;
            m_dbContext.Database.ExecuteSqlCommand(sql);
            //m_dbContext.SaveChanges();
        }

        public void UpdateSupplier(Supplier supplier, int companyId) {
            var dbSupplier = (from suppDb in m_dbContext.Supplier
                              where suppDb.supplier_id == supplier.supplier_id &&
                              suppDb.supplier_group_id == companyId
                              select suppDb).FirstOrDefault();

            if (dbSupplier != null) {

            } else {
                var lastSuppDbId = (from suppDb in m_dbContext.Supplier
                                 orderby suppDb.id descending
                                 select new { suppId = suppDb.id}).Take(1).FirstOrDefault();

                int lastId = -1;
                if (lastSuppDbId != null) {
                    lastId = lastSuppDbId.suppId;
                }
                int newId = ++lastId;
                dbSupplier.id = newId;

                m_dbContext.Supplier.Add(dbSupplier);
            }
        }

        public int UpdateSupplier(Supplier supplier) {
            if (supplier.id == DataNulls.INT_NULL) {
                var lastSuppDbId = (from suppDb in m_dbContext.Supplier
                                    orderby suppDb.id descending
                                    select new { suppId = suppDb.id }).Take(1).FirstOrDefault();

                int lastId = -1;
                if (lastSuppDbId != null) {
                    lastId = lastSuppDbId.suppId;
                }
                int newId = ++lastId;
                supplier.id = newId;

                m_dbContext.Supplier.Add(supplier);
            }

            m_dbContext.SaveChanges();

            return supplier.id;
        }

        public Supplier GetSupplierDataBySuppId(int suppGroupId, string supplierId) {
            var dbSupplier = (from suppDb in m_dbContext.Supplier
                              where suppDb.supplier_id.Trim().ToLower() == supplierId.Trim().ToLower() &&
                              suppDb.supplier_group_id == suppGroupId
                              select suppDb).FirstOrDefault();

            return dbSupplier;
        }

        public Supplier GetSupplierDataById(int id) {
            var dbSupplier = (from suppDb in m_dbContext.Supplier
                              where suppDb.id == id
                              select suppDb).FirstOrDefault();

            return dbSupplier;
        }

        public Supplier GetSupplierDataByLocalAppId(int suppGroupId, string supplierLocalAppId) {
            var dbSupplier = (from suppDb in m_dbContext.Supplier
                              where suppDb.supplier_local_app_id.Trim().ToLower() == supplierLocalAppId.Trim().ToLower() &&
                              suppDb.supplier_group_id == suppGroupId
                              select suppDb).FirstOrDefault();

            return dbSupplier;
        }

        public Supplier GetSupplierDataByName(int suppGroupId, string suppName) {
            var dbSupplier = (from suppDb in m_dbContext.Supplier
                              where suppDb.supp_name.Trim().ToLower() == suppName.Trim().ToLower() &&
                              suppDb.supplier_group_id == suppGroupId
                              select suppDb).FirstOrDefault();

            return dbSupplier;
        }

        public void DeactiveSuppliers(int suppGroupId, Hashtable htActiveIds) {
            var dbSuppliers = (from suppDb in m_dbContext.Supplier
                              where suppDb.supplier_group_id == suppGroupId &&
                              suppDb.active == true
                              select suppDb).ToList();

            foreach (var supp in dbSuppliers) {
                if (!htActiveIds.ContainsKey(supp.id)) {
                    supp.active = false;
                    m_dbContext.SaveChanges();
                }
            }
        }

        public void UpdateLastSuppUpdateDateBySuppGroup(int supplierGroupId) {
            var comp = (from compDb in m_dbContext.Company
                        where compDb.supplier_group_id == supplierGroupId
                        select compDb).FirstOrDefault();

            comp.last_supplier_upload = DateTime.Now;
            m_dbContext.SaveChanges();
        }

        public void UpdateLastSuppUpdateDateByCompany(int companyId) {
            var comp = (from compDb in m_dbContext.Company
                        where compDb.id == companyId
                        select compDb).FirstOrDefault();

            comp.last_supplier_upload = DateTime.Now;
            m_dbContext.SaveChanges();
        }

        public bool DeleteSupplier(int supplierId) {
            bool isSupplierUsed = new RequestRepository().IsSupplierUsed(supplierId);

            using (TransactionScope transaction = new TransactionScope()) {
                try {
                    var supp = (from suppDb in m_dbContext.Supplier
                                where suppDb.id == supplierId
                                select suppDb).FirstOrDefault();
                    if (isSupplierUsed) {
                        supp.active = false;
                    } else {
                        m_dbContext.Supplier.Remove(supp);
                    }

                    SaveChanges();
                    transaction.Complete();

                    return (!isSupplierUsed);
                } catch (Exception ex) {
                    throw ex;
                }
            }
        }

        public void AddNewSuppAdmin(int userId, int companyId) {
            //var participantOffRole = (from partDb in m_dbContext.Participant_Office_Role
            //                   where partDb.participant_id == userId
            //                   select partDb).FirstOrDefault();

            Participant_Office_Role participant_Office_Role = new Participant_Office_Role();
            participant_Office_Role.participant_id = userId;
            participant_Office_Role.office_id = companyId;
            participant_Office_Role.role_id = (int)UserRole.SupplierAdmin;

            m_dbContext.Participant_Office_Role.Add(participant_Office_Role);
            m_dbContext.SaveChanges();
        }

        public void DeleteSuppAdmin(int userId, int companyId) {
            var participantOffRole = (from partDb in m_dbContext.Participant_Office_Role
                                      where partDb.participant_id == userId &&
                                      partDb.role_id == (int)UserRole.SupplierAdmin
                                      select partDb).FirstOrDefault();

            m_dbContext.Participant_Office_Role.Remove(participantOffRole);
            m_dbContext.SaveChanges();
        }

        public List<SupplierSimpleExtended> GetActiveSuppliersByNameId(string searchText, int supplierGroupId) {
            string tmpSearchText = RemoveDiacriticsWhiteSpace(searchText);
            var suppliers = (from sd in m_dbContext.Supplier
                             where sd.supplier_group_id == supplierGroupId 
                            // (sd.supp_name.Contains(searchText) || sd.supplier_id.Contains(searchText)) &&
                            && sd.supplier_search_key.Contains(tmpSearchText)
                            && sd.active == true
                             select new {
                                 id = sd.id,
                                 supp_name = sd.supp_name,
                                 supplier_id = sd.supplier_id
                             }).ToList();

            if (suppliers == null) {
                return null;
            }

            List<SupplierSimpleExtended> retSupplierSimpleList = new List<SupplierSimpleExtended>();
            foreach (var supp in suppliers) {
                SupplierSimpleExtended tmpSupplierSimpleExtended = new SupplierSimpleExtended();
                SetValues(supp, tmpSupplierSimpleExtended, new List<string> { "id", "supp_name" });
                if (!String.IsNullOrEmpty(supp.supplier_id)) {
                    tmpSupplierSimpleExtended.supp_name += " (" + supp.supplier_id + ")";
                }

                retSupplierSimpleList.Add(tmpSupplierSimpleExtended);
            } 

            return retSupplierSimpleList;
        }

        public SupplierSimpleExtended GetActiveSuppliersById(int supplierId) {

            var supplier = (from sd in m_dbContext.Supplier
                            where sd.id == supplierId
                            && sd.active == true
                            select new {
                                id = sd.id,
                                supp_name = sd.supp_name,
                                supplier_id = sd.supplier_id
                            }).FirstOrDefault();

            if (supplier == null) {
                return null;
            }

            SupplierSimpleExtended retSupplierSimple = new SupplierSimpleExtended();
            //SupplierSimpleExtended tmpSupplierSimpleExtended = new SupplierSimpleExtended();
            SetValues(supplier, retSupplierSimple, new List<string> { "id", "supp_name" });
            if (!String.IsNullOrEmpty(supplier.supplier_id)) {
                retSupplierSimple.supp_name += " (" + supplier.supplier_id + ")";
            }

            return retSupplierSimple;
        }

        public Supplier GetSuppliersById(int supplierId) {

            var supplier = (from sd in m_dbContext.Supplier
                            where sd.id == supplierId
                            select sd).FirstOrDefault();

            if (supplier == null) {
                return null;
            }

            Supplier retSupplier = new Supplier();
            SetValues(supplier, retSupplier);
            
            return retSupplier;
        }

        public List<Supplier> GetAllSuppliers() {
            var supps = (from suppDb in m_dbContext.Supplier
                         select suppDb).ToList();

            return supps;
        }

        public List<SupplierContactGridData> GetContactData(
            int supplierId,
            string filter,
            string sort,
            int pageSize,
            int pageNr,
            out int rowsCount) {

            string strFilterWhere = "";

            if (!String.IsNullOrEmpty(filter)) {
                string[] filterItems = filter.Split(UrlParamDelimiter.ToCharArray());
                foreach (string filterItem in filterItems) {
                    string[] strItemProp = filterItem.Split(UrlParamValueDelimiter.ToCharArray());
                    strFilterWhere += " AND ";

                    //string columnName = strItemProp[0].Trim().ToUpper();
                    //if (columnName == SupplierData.SUPP_NAME_FIELD.Trim().ToUpper()) {
                    //    strFilterWhere += "sd." + SupplierData.SUPP_NAME_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'";
                    //}
                    
                }
            }

            string strOrder = "ORDER BY scd." + SupplierContactData.SURNAME_FIELD 
                + "," + "scd." + SupplierContactData.FIRST_NAME_FIELD;

            if (!String.IsNullOrEmpty(sort)) {
                strOrder = "";
                string[] sortItems = sort.Split(UrlParamDelimiter.ToCharArray());
                foreach (string sortItem in sortItems) {
                    //string[] strItemProp = sortItem.Split(UrlParamValueDelimiter.ToCharArray());
                    //if (strOrder.Length > 0) {
                    //    strOrder += ", ";
                    //}

                    //if (strItemProp[0] == "street") {
                    //    strOrder = "sd." + SupplierData.STREET_PART1_FIELD + ", sd." + SupplierData.STREET_PART2_FIELD + " " + strItemProp[1]; ;
                    //} else {
                    //    strOrder += "sd." + strItemProp[0] + " " + strItemProp[1];
                    //}
                }

                strOrder = " ORDER BY " + strOrder;
            }

            string sqlPure = "SELECT scd.*, ROW_NUMBER() OVER(" + strOrder + ") AS RowNum";

            string sqlPureBody = " FROM " + SupplierContactData.TABLE_NAME + " scd" +
                 " WHERE scd." + SupplierContactData.SUPPLIER_ID_FIELD + "=" + supplierId;
            sqlPureBody += strFilterWhere;

            //Get Row count
            string selectCount = "SELECT COUNT(" + SupplierContactData.ID_FIELD + ") " + sqlPureBody;
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

            var tmpSupplierContacts = m_dbContext.Database.SqlQuery<Supplier_Contact>(sql).ToList();


            List<SupplierContactGridData> supplierContacts = new List<SupplierContactGridData>();
            int rowIndex = (pageNr - 1) * pageSize + 1;
            foreach (var tmpSupplierContact in tmpSupplierContacts) {
                SupplierContactGridData supplierContact = new SupplierContactGridData();
                SetValues(tmpSupplierContact, supplierContact);

                supplierContact.row_index = rowIndex++;

                supplierContacts.Add(supplierContact);

            }

            return supplierContacts;

        }
        #endregion
    }
}
