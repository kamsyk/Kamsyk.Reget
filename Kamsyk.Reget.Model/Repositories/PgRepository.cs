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
using System.Dynamic;
using System.Reflection.Emit;
using System.Reflection;
using static Kamsyk.Reget.Model.Repositories.UserRepository;
using Kamsyk.Reget.Model.ExtendedModel.PurchaseGroup;

namespace Kamsyk.Reget.Model.Repositories {
    public class PgRepository : BaseRepository<Purchase_Group> {
        #region Constant
        #endregion

        #region Enums
        public enum PurchaseGroupType {
            Standard = 0,
            //Order = 0,
            //ApprovalOnly = 1,
            Items = 20
        }
        #endregion

        #region Methods
        public Purchase_Group GetPgById(int pgId) {

            var pg = (from pgDb in m_dbContext.Purchase_Group
                           where pgDb.id == pgId
                           select pgDb).FirstOrDefault();


            return pg;

        }

        public List<Purchase_Group> GetPgsByName(string pgName) {
            var pgs = (from pgDb in m_dbContext.Purchase_Group
                      where pgDb.group_name == pgName
                      select pgDb).ToList();

            return pgs;
        }

        public string GetLocalName(int pgId, string cultureName) {
            var pgLoc = (from pgLocDb in m_dbContext.Purchase_Group_Local
                      where pgLocDb.purchase_group_id == pgId &&
                      pgLocDb.culture == cultureName
                         select pgLocDb).FirstOrDefault();

            if (pgLoc != null) {
                return pgLoc.local_text;
            }


            var pg = (from pgDb in m_dbContext.Purchase_Group
                      where pgDb.id == pgId
                      select pgDb).FirstOrDefault();

            return pg.group_name;
        }

        public int GetMissingLocalTextCount(string culture, out List<Purchase_Group> pgs) {
            string sql = "SELECT pgd.*" +
                " FROM " + PurchaseGroupData.TABLE_NAME + " pgd" +
                " LEFT OUTER JOIN " + PurchaseGroupLocalData.TABLE_NAME + " pgld" +
                " ON pgd." + PurchaseGroupData.ID_FIELD + "=pgld." + PurchaseGroupLocalData.PURCHASE_GROUP_ID_FIELD +
                " AND pgld." + PurchaseGroupLocalData.CULTURE_FIELD + " = '" + culture + "'" +
                " WHERE pgld." + PurchaseGroupLocalData.LOCAL_TEXT_FIELD + " IS NULL";

            pgs = m_dbContext.Database.SqlQuery<Purchase_Group>(sql).ToList();

            if (pgs == null) {
                return 0;
            }

            return pgs.Count;
        }

        public void SetMissingLocalText(Purchase_Group pg, string culture) {
            //string sql = "SELECT pgd.*" +
            //    " FROM " + PurchaseGroupData.TABLE_NAME + " pgd" +
            //    " LEFT OUTER JOIN " + PurchaseGroupLocalData.TABLE_NAME + " pgld" +
            //    " ON pgd." + PurchaseGroupData.ID_FIELD + "=pgld." + PurchaseGroupLocalData.PURCHASE_GROUP_ID_FIELD +
            //    " AND pgld." + PurchaseGroupLocalData.CULTURE_FIELD + " = '" + culture + "'" +
            //    " WHERE pgld." + PurchaseGroupLocalData.LOCAL_TEXT_FIELD + " IS NULL";

            //var pgs = m_dbContext.Database.SqlQuery<Purchase_Group>(sql).ToList();

            //if (pgs != null && pgs.Count > 0) {

            //    foreach (var pg in pgs) {
            //        Console.WriteLine(culture + ", PG " + pg.group_name);
                    var mPpgl = (from pglDb in m_dbContext.Purchase_Group_Local
                                 where pglDb.purchase_group_id == pg.id &&
                                 pglDb.culture == culture
                                 select pglDb).FirstOrDefault();

                    if (mPpgl == null) {
                        Purchase_Group_Local pgl = new Purchase_Group_Local();
                        pgl.purchase_group_id = pg.id;
                        pgl.culture = culture;
                        pgl.local_text = pg.group_name;
                        m_dbContext.Purchase_Group_Local.Add(pgl);
                    } else {
                        mPpgl.local_text = pg.group_name;
                    }

                    SaveChanges();
                //}

                
            //}

            ////Default lang
            //sql = "SELECT pgd.*" +
            //    " FROM " + PurchaseGroupData.TABLE_NAME + " pgd" +
            //    " LEFT OUTER JOIN " + PurchaseGroupLocalData.TABLE_NAME + " pgld" +
            //    " ON pgd." + PurchaseGroupData.ID_FIELD + "=pgld." + PurchaseGroupLocalData.PURCHASE_GROUP_ID_FIELD +
            //    " AND pgld." + PurchaseGroupLocalData.CULTURE_FIELD + " = '" + DefaultLanguage + "'" +
            //    " WHERE pgld." + PurchaseGroupLocalData.LOCAL_TEXT_FIELD + " IS NULL";

            //pgs = m_dbContext.Database.SqlQuery<Purchase_Group>(sql).ToList();

            //if (pgs != null && pgs.Count > 0) {

            //    foreach (var pg in pgs) {
            //        Console.WriteLine(culture + ", PG " + pg.group_name);
            //        var mPgl = (from pglDb in m_dbContext.Purchase_Group_Local
            //                     where pglDb.purchase_group_id == pg.id &&
            //                     pglDb.culture == DefaultLanguage
            //                     select pglDb).FirstOrDefault();

            //        if (mPgl == null) {
            //            Purchase_Group_Local pgl = new Purchase_Group_Local();
            //            pgl.purchase_group_id = pg.id;
            //            pgl.culture = DefaultLanguage;
            //            pgl.local_text = pg.group_name;
            //            m_dbContext.Purchase_Group_Local.Add(pgl);
            //        } else {
            //            mPgl.local_text = pg.group_name;
            //        }

            //        SaveChanges();
            //    }

                
            //}
        }

        public List<string> SavePgTranslation(int pgId, List<LocalText> localTexts) {
            if (localTexts == null) {
                return null;
            }

            List<string> errMsgs = null;

            foreach (var localText in localTexts) {
                if (String.IsNullOrWhiteSpace(localText.culture)) {
                    if (errMsgs == null) {
                        errMsgs = new List<string>();
                    }
                    errMsgs.Add("Culture cannot be blank");
                    return errMsgs;
                }
                if (String.IsNullOrWhiteSpace(localText.text)) {
                    if (errMsgs == null) {
                        errMsgs = new List<string>();
                    }
                    errMsgs.Add("Text cannot be blank");
                    return errMsgs;
                }
            }

            ////duplicity check
            //List<string> errMsg = GetDuplicityCheck(pgId, localTexts);
            //if (errMsg != null) {
            //    return errMsg;
            //}

            
            using (TransactionScope transaction = new TransactionScope()) {
                try {

                    foreach (var localText in localTexts) {
                        string strCulture = localText.culture;
                        var pgLoc = (from pgLocDb in m_dbContext.Purchase_Group_Local
                                           where pgLocDb.purchase_group_id == pgId &&
                                           pgLocDb.culture == strCulture
                                           select pgLocDb).FirstOrDefault();
                        if (pgLoc == null) {
                            Purchase_Group_Local pgl = new Purchase_Group_Local();
                            pgl.purchase_group_id = pgId;
                            pgl.culture = strCulture;
                            pgl.local_text = localText.text;
                            m_dbContext.Purchase_Group_Local.Add(pgl);
                            if (strCulture == DefaultLanguage) {
                                SetDefaultPgName(pgId, localText.text);
                            }
                            SaveChanges();
                        } else {
                            if (pgLoc.local_text != localText.text) {
                                pgLoc.local_text = localText.text;
                                if (strCulture == DefaultLanguage) {
                                    SetDefaultPgName(pgId, localText.text);
                                }
                                SaveChanges();
                            }
                        }
                    }

                    transaction.Complete();
                } catch (Exception ex) {
                    throw ex;
                }
            }

            return errMsgs;
        }

        public List<UsedPg> GetUsedPgJs(
            List<int> companyIds,
            string cultureName,
            string filter,
            string sort,
            int pageSize,
            int pageNr,
            string rootUrl,
            int? parentPgId,
            out int rowsCount) {

            rowsCount = 0;

            string strFilterWhere = GetFilter(filter, cultureName);

            string strOrder = GetOrder(sort);
                        
            string sqlPure = "SELECT " +
                " pgd." + PurchaseGroupData.ID_FIELD + " as 'id'," +
                " pgd." + PurchaseGroupData.GROUP_NAME_FIELD + " as 'pg_name'," +
                " pgd." + PurchaseGroupData.ACTIVE_FIELD + " as 'active'," +
                " ppgd." + ParentPurchaseGroupData.ID_FIELD + " as 'parent_pg_id'," +
                " ppgd." + ParentPurchaseGroupData.NAME_FIELD + " as 'parent_pg_name'," +
                " ppgld." + ParentPurchaseGroupLocalData.LOCAL_TEXT_FIELD + " as 'parent_pg_loc_name'," +
                " pgld." + PurchaseGroupLocalData.LOCAL_TEXT_FIELD + " as 'pg_loc_name'," +
                " cgd." + CentreGroupData.ID_FIELD + " as 'centre_group_id'," +
                " cgd." + CentreGroupData.NAME_FIELD + " as 'centre_group_name'," +
                " compd." + CompanyData.ID_FIELD + " as 'company_id'," +
                " compd." + CompanyData.COUNTRY_CODE_FIELD + " as 'company_name'," +
                " ROW_NUMBER() OVER(" + strOrder + ") AS RowNum";

            string sqlPureBody = GetPureBody(companyIds, cultureName, parentPgId);
            sqlPureBody += strFilterWhere;

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

            var tmpPgs = m_dbContext.Database.SqlQuery<UsedPg>(sql).ToList();

            if (tmpPgs == null && tmpPgs.Count == 0) {
                return null;
            }

            int rowIndex = (pageNr - 1) * pageSize + 1;

            //List<ParentPgCompanyPg> parentPgCompanyPgs = new List<ParentPgCompanyPg>();
            foreach (var tmpPg in tmpPgs) {

                if (String.IsNullOrEmpty(tmpPg.pg_loc_name)) {
                    tmpPg.pg_loc_name = tmpPg.pg_name;
                }

                if (String.IsNullOrEmpty(tmpPg.parent_pg_loc_name)) {
                    tmpPg.parent_pg_loc_name = tmpPg.parent_pg_name;
                }

                tmpPg.local_text = new List<LocalText>();
                LocalText localText = new LocalText();
                localText.text_id = tmpPg.id;
                localText.culture = cultureName;
                localText.label = GetPgLabel(cultureName);
                localText.text = tmpPg.pg_loc_name;
                localText.flag_url = GetPgFlagUrl(cultureName, rootUrl);
                tmpPg.local_text.Add(localText);

                if (DefaultLanguage != cultureName) {
                    localText = new LocalText();
                    localText.text_id = tmpPg.id;
                    localText.culture = DefaultLanguage;
                    localText.label = GetPgLabel(DefaultLanguage);
                    localText.text = tmpPg.pg_name;
                    localText.flag_url = GetPgFlagUrl(DefaultLanguage, rootUrl);
                    tmpPg.local_text.Add(localText);
                }

                tmpPg.row_index = rowIndex;

                rowIndex++;
            }

            return tmpPgs;
        }

        public List<UsedPg> GetUsedPgReport(
            string filter,
            string sort,
            string cultureName,
            int currentUserId,
            List<int> companyIds) {

            string strFilterWhere = GetFilter(filter, cultureName);

            string strOrder = GetOrder(sort);

            string sqlPure = "SELECT " +
                " pgd." + PurchaseGroupData.ID_FIELD + " as 'id'," +
                " pgd." + PurchaseGroupData.GROUP_NAME_FIELD + " as 'pg_name'," +
                " pgd." + PurchaseGroupData.ACTIVE_FIELD + " as 'active'," +
                " ppgd." + ParentPurchaseGroupData.ID_FIELD + " as 'parent_pg_id'," +
                " ppgd." + ParentPurchaseGroupData.NAME_FIELD + " as 'parent_pg_name'," +
                " ppgld." + ParentPurchaseGroupLocalData.LOCAL_TEXT_FIELD + " as 'parent_pg_loc_name'," +
                " pgld." + PurchaseGroupLocalData.LOCAL_TEXT_FIELD + " as 'pg_loc_name'," +
                " cgd." + CentreGroupData.ID_FIELD + " as 'centre_group_id'," +
                " cgd." + CentreGroupData.NAME_FIELD + " as 'centre_group_name'," +
                " compd." + CompanyData.ID_FIELD + " as 'company_id'," +
                " compd." + CompanyData.COUNTRY_CODE_FIELD + " as 'company_name'";

            string sqlPureBody = GetPureBody(companyIds, cultureName, null);
            sqlPureBody += strFilterWhere;

            string sql = sqlPure + sqlPureBody;

            var tmpPgs = m_dbContext.Database.SqlQuery<UsedPg>(sql).ToList();

            foreach (var tmpPg in tmpPgs) {

                if (String.IsNullOrEmpty(tmpPg.pg_loc_name)) {
                    tmpPg.pg_loc_name = tmpPg.pg_name;
                }

                if (String.IsNullOrEmpty(tmpPg.parent_pg_loc_name)) {
                    tmpPg.parent_pg_loc_name = tmpPg.parent_pg_name;
                }
            }

            return tmpPgs;
        }

        private string GetFilter(string filter, string cultureName) {
            string strFilterWhere = "";
            if (!String.IsNullOrEmpty(filter)) {
                string[] filterItems = filter.Split(UrlParamDelimiter.ToCharArray());

                foreach (string filterItem in filterItems) {

                    string[] strItemProp = filterItem.Split(UrlParamValueDelimiter.ToCharArray());
                    strFilterWhere += " AND ";

                    string columnName = strItemProp[0].Trim().ToUpper();

                    if (columnName == "pg_loc_name".Trim().ToUpper()) {
                        strFilterWhere += "((pgld." + PurchaseGroupLocalData.LOCAL_TEXT_FIELD + " IS NOT NULL" + " AND " +
                            "pgld." + PurchaseGroupLocalData.CULTURE_FIELD + "='" + cultureName + "'" + " AND " +
                            "pgld." + PurchaseGroupLocalData.LOCAL_TEXT_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%')" + " OR " +
                            "(pgld." + PurchaseGroupLocalData.LOCAL_TEXT_FIELD + " IS NULL" + " AND " +
                            "pgd." + PurchaseGroupData.GROUP_NAME_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'))";
                    }

                    if (columnName == "parent_pg_loc_name".Trim().ToUpper()) {
                        strFilterWhere += "((ppgld." + ParentPurchaseGroupLocalData.LOCAL_TEXT_FIELD + " IS NOT NULL" + " AND " +
                            "ppgld." + ParentPurchaseGroupLocalData.CULTURE_FIELD + "='" + cultureName + "'" + " AND " +
                            "ppgld." + ParentPurchaseGroupLocalData.LOCAL_TEXT_FIELD + "='" + strItemProp[1].Trim() + "')" + " OR " +
                            "(ppgld." + ParentPurchaseGroupLocalData.LOCAL_TEXT_FIELD + " IS NULL" + " AND " +
                            "ppgd." + ParentPurchaseGroupData.NAME_FIELD + "='" + strItemProp[1].Trim() + "'))";
                    }

                    if (columnName == "centre_group_name".Trim().ToUpper()) {
                        strFilterWhere += "cgd." + CentreGroupData.NAME_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'";
                    }

                    if (columnName == "company_name".Trim().ToUpper()) {
                        strFilterWhere += "compd." + CompanyData.COUNTRY_CODE_FIELD + "='" + strItemProp[1].Trim() + "'";
                    }

                    if (columnName == PurchaseGroupData.ACTIVE_FIELD.Trim().ToUpper()) {
                        if (strItemProp[1].Trim().ToLower() == "true") {
                            strFilterWhere += "pgd." + PurchaseGroupData.ACTIVE_FIELD + "=1";
                        } else {
                            strFilterWhere += "(pgd." + PurchaseGroupData.ACTIVE_FIELD + "=0" +
                                " OR pgd." + PurchaseGroupData.ACTIVE_FIELD + " IS NULL)";
                        }
                    }
                }

            }

            return strFilterWhere;
        }

        private string GetOrder(string sort) {
            string strOrder = "ORDER BY pgd." + PurchaseGroupData.ID_FIELD;

            if (!String.IsNullOrEmpty(sort)) {
                strOrder = "";
                string[] sortItems = sort.Split(UrlParamDelimiter.ToCharArray());
                foreach (string sortItem in sortItems) {
                    string[] strItemProp = sortItem.Split(UrlParamValueDelimiter.ToCharArray());
                    if (strOrder.Length > 0) {
                        strOrder += ", ";
                    }

                    if (strItemProp[0] == "pg_loc_name") {
                        strOrder = "pgld." + PurchaseGroupLocalData.LOCAL_TEXT_FIELD + "," +
                            "pgd." + PurchaseGroupData.GROUP_NAME_FIELD + " " + strItemProp[1];
                    } else if (strItemProp[0] == "parent_pg_loc_name") {
                        strOrder = "ppgld." + ParentPurchaseGroupLocalData.LOCAL_TEXT_FIELD + "," +
                             "ppgd." + ParentPurchaseGroupData.NAME_FIELD + " " + strItemProp[1];
                    } else if (strItemProp[0] == "centre_group_name") {
                        strOrder = "cgd." + CentreGroupData.NAME_FIELD + " " + strItemProp[1];
                    } else if (strItemProp[0] == "company_name") {
                        strOrder = "compd." + CompanyData.COUNTRY_CODE_FIELD + " " + strItemProp[1];
                    } else if (strItemProp[0] == "active") {
                        strOrder = "pgd." + PurchaseGroupData.ACTIVE_FIELD + " " + strItemProp[1];
                    }
                }

                strOrder = " ORDER BY " + strOrder;
            }

            return strOrder;
        }

        private string GetPureBody(List<int> companyIds, string cultureName, int? parentPgId) {
            string sqlPureBody = " FROM " + PurchaseGroupData.TABLE_NAME + " pgd" +
                " INNER JOIN " + ParentPurchasePurchaseGroupData.TABLE_NAME + " pppgd" +
                " ON pgd." + PurchaseGroupData.ID_FIELD + "=pppgd." + ParentPurchasePurchaseGroupData.PURCHASE_GROUP_ID_FIELD +
                " INNER JOIN " + CentregroupPurchasegroupData.TABLE_NAME + " cgpgd" +
                " ON pgd." + PurchaseGroupData.ID_FIELD + "=cgpgd." + CentregroupPurchasegroupData.PURCHASE_GROUP_ID_FIELD +
                " INNER JOIN " + CentreGroupData.TABLE_NAME + " cgd" +
                " ON cgpgd." + CentregroupPurchasegroupData.CENTRE_GROUP_ID_FIELD + "=cgd." + CentreGroupData.ID_FIELD +
                " INNER JOIN " + ParentPurchaseGroupData.TABLE_NAME + " ppgd" +
                " ON ppgd." + ParentPurchaseGroupData.ID_FIELD + "=pppgd." + ParentPurchasePurchaseGroupData.PARENT_PURCHASE_GROUP_ID_FIELD +
                " INNER JOIN " + CompanyData.TABLE_NAME + " compd" +
                " ON compd." + CompanyData.ID_FIELD + "=cgd." + CentreGroupData.COMPANY_ID_FIELD +
                " LEFT OUTER JOIN " + PurchaseGroupLocalData.TABLE_NAME + " pgld" +
                " ON pgld." + PurchaseGroupLocalData.PURCHASE_GROUP_ID_FIELD + "=pgd." + PurchaseGroupData.ID_FIELD +
                " LEFT OUTER JOIN " + ParentPurchaseGroupLocalData.TABLE_NAME + " ppgld" +
                " ON ppgld." + ParentPurchaseGroupLocalData.PARENT_PURCHASE_GROUP_ID_FIELD + "=pppgd." + ParentPurchasePurchaseGroupData.PARENT_PURCHASE_GROUP_ID_FIELD +
                " WHERE " +
                " cgd." + CentreGroupData.COMPANY_ID_FIELD + " IN" + GetSqlIn(companyIds) +
                " AND pgld." + PurchaseGroupLocalData.CULTURE_FIELD + "='" + cultureName + "'" +
                " AND ppgld." + ParentPurchaseGroupLocalData.CULTURE_FIELD + "='" + cultureName + "'";
            if (parentPgId != null) {
                sqlPureBody += " AND pppgd." + ParentPurchasePurchaseGroupData.PARENT_PURCHASE_GROUP_ID_FIELD + "=" + parentPgId;
            }

            return sqlPureBody;
        }

        public void DeletePgLocData(string culture) {
            List<Purchase_Group_Local> pgls = (from pglDb in m_dbContext.Purchase_Group_Local
                                                       where pglDb.culture == culture
                                                       select pglDb).ToList();

            if (pgls == null) {
                return;
            }

            foreach (var pgl in pgls) {
                m_dbContext.Purchase_Group_Local.Remove(pgl);
            }

            m_dbContext.SaveChanges();
        }

        public List<Purchase_Group_Local> GetPgLocData(string culture) {
            List<Purchase_Group_Local> pgls = (from pglDb in m_dbContext.Purchase_Group_Local
                                                       where pglDb.culture == culture
                                                       select pglDb).ToList();

            return pgls;
        }

        public List<Purchase_Group> GetPgData() {
            List<Purchase_Group> pgs = (from pgDb in m_dbContext.Purchase_Group
                                                select pgDb).ToList();

            return pgs;
        }

        ///// <summary>
        ///// duplicity check - not needed there is possible to have 2 PG with the same name within the CG
        ///// </summary>
        ///// <param name="pgId"></param>
        ///// <param name="localTexts"></param>
        ///// <returns></returns>
        //public List<string> GetDuplicityCheck(int pgId, List<LocalText> localTexts) {
        //    //duplicity check - not needed there is possible to have 2 PG with the same name within the CG
        //    //List<string> errMsgs = null;
        //    //foreach (var localText in localTexts) {
        //    //    string strCulture = localText.culture;

        //    //    var pg = (from pgDb in m_dbContext.Purchase_Group
        //    //              where pgDb.id == pgId
        //    //              select pgDb).FirstOrDefault();

        //    //    int cgId = pg.Centre_Group.ElementAt(0).id;

        //    //    string strSql = "SELECT pgloc.*" +
        //    //        " FROM " + PurchaseGroupLocalData.TABLE_NAME + " pgloc" +
        //    //        " INNER JOIN " + CentregroupPurchasegroupData.TABLE_NAME + " cppg" +
        //    //        " ON pgloc." + PurchaseGroupLocalData.PURCHASE_GROUP_ID_FIELD +
        //    //        "=cppg." + CentregroupPurchasegroupData.PURCHASE_GROUP_ID_FIELD +
        //    //        " WHERE cppg." + CentregroupPurchasegroupData.CENTRE_GROUP_ID_FIELD + "=" + cgId +
        //    //        " AND pgloc." + PurchaseGroupLocalData.PURCHASE_GROUP_ID_FIELD + "<>" + pgId +
        //    //        " AND pgloc." + PurchaseGroupLocalData.CULTURE_FIELD + "='" + strCulture + "'" +
        //    //        " AND pgloc." + PurchaseGroupLocalData.LOCAL_TEXT_FIELD + "='" + localText.text + "'";

        //    //    var pgLoc = m_dbContext.Database.SqlQuery<Purchase_Group_Local>(strSql).FirstOrDefault();


        //    //    if (pgLoc != null) {
        //    //        if (errMsgs == null) {
        //    //            errMsgs = new List<string>();
        //    //        }
        //    //        errMsgs.Add(DUPLICITY);
        //    //        errMsgs.Add(localText.text);
        //    //        return errMsgs;
        //    //    }
        //    //}

        //    return null;
        //}

        private void SetDefaultPgName(int pgId, string name) {
            Purchase_Group pg = (from ppDb in m_dbContext.Purchase_Group
                                         where ppDb.id == pgId
                                         select ppDb).FirstOrDefault();

            if (pg != null) {
                pg.group_name = name;
            }
        }

        public Purchase_Group GetRandomDeactivatedPgwithCgAppMan() {
            var pgs = (from pgDb in m_dbContext.Purchase_Group
                      join limitDb in m_dbContext.Purchase_Group_Limit
                      on pgDb.id equals limitDb.purchase_group_id
                      join manRoleDb in m_dbContext.Manager_Role
                      on limitDb.purchase_group_id equals manRoleDb.purchase_group_limit_id
                      where pgDb.active == false
                       select pgDb).ToList();

            
                int iIndex = new Random().Next(0, pgs.Count - 1);
                //var pg = pgs.ElementAt(iIndex);

            for (int i = iIndex; i >= 0; i--) {
                var pg = pgs.ElementAt(i);
                if (pg.Centre_Group != null && pg.Purchase_Group_Limit != null) {
                    foreach (var pgLimit in pg.Purchase_Group_Limit) {
                        if (pgLimit.Manager_Role != null && pgLimit.Manager_Role.Count > 0) {
                            if (pg.Centre_Group.Count > 0) {
                                return pg;
                            }
                        }
                    }
                }
            }

            return null;
        }

        public int DeactivatePg() {
            var pg = (from pgDb in m_dbContext.Purchase_Group
                      select pgDb).FirstOrDefault();

            pg.active = false;

            SaveChanges();

            return pg.id;
        }

        public void DeactivatePg(InternalRequestEntities dbContext, int pgId) {
            var pg = (from pgDb in dbContext.Purchase_Group
                      where pgDb.id == pgId
                      select pgDb).FirstOrDefault();

            pg.active = false;

            SaveChanges();


        }

        public void ActivatePg(int pgId) {
            var pg = (from pgDb in m_dbContext.Purchase_Group
                      where pgDb.id == pgId
                      select pgDb).FirstOrDefault();

            pg.active = true;

            SaveChanges();

            
        }

        public Purchase_Group GetActivePgInDeactivatedCg() {
            var cgs = (from cgDb in m_dbContext.Centre_Group
                      where cgDb.active == false
                      select cgDb).ToList();

            foreach (var cg in cgs) {
                var pgs = (from pgDb in cg.Purchase_Group
                          where pgDb.active == true
                          select pgDb).ToList();

                if (pgs != null) {
                    foreach (var pg in pgs) {
                        foreach (var limit in pg.Purchase_Group_Limit) {
                            if (limit.Manager_Role != null && limit.Manager_Role.Count > 0) {
                                return pg;
                            }
                        }
                        
                    }
                }
            }

            return null;
        }

        public int GetLastId() {
            var lastPg = (from pgDb in m_dbContext.Purchase_Group
                            orderby pgDb.id descending
                            select pgDb).Take(1).FirstOrDefault();

            int lastId = -1;
            if (lastPg != null) {
                lastId = lastPg.id;
            }

            return lastId;
        }

        public void ReplacePg(int sourcePgId, int targetParentPgId, string strNewPgName, string rootUrl) {
            using (TransactionScope transaction = new TransactionScope()) {
                try {
                    var sourcePg = (from pgDb in m_dbContext.Purchase_Group
                                    where pgDb.id == sourcePgId
                                    select pgDb).FirstOrDefault();

                    //Limits
                    int newPgId = new CentreGroupRepository().CopyAppMatrixAddNew(
                        sourcePg,
                        sourcePg.Centre_Group.ElementAt(0).id,
                        -1,
                        "pl-PL",
                        rootUrl);

                    var newPg = (from pgDb in m_dbContext.Purchase_Group
                                    where pgDb.id == newPgId
                                 select pgDb).FirstOrDefault();

                    //Parent PG
                    newPg.Parent_Purchase_Group.Remove(newPg.Parent_Purchase_Group.ElementAt(0));

                    var ppg = new ParentPgRepository().GetParentPgById(targetParentPgId);
                    newPg.Parent_Purchase_Group.Add(ppg);

                    //Requestors
                    foreach (var requestor in sourcePg.PurchaseGroup_Requestor) {
                        PurchaseGroup_Requestor pgr = new PurchaseGroup_Requestor();
                        SetValues(requestor, pgr);
                        pgr.purchase_group_id = newPg.id;
                        newPg.PurchaseGroup_Requestor.Add(pgr);
                    }

                    //Orderers
                    foreach (var orderer in sourcePg.PurchaseGroup_Orderer) {
                        sourcePg.PurchaseGroup_Orderer.Add(orderer);
                    }

                    transaction.Complete();
                } catch (Exception ex) {
                    throw ex;
                }
            }
        }
        #endregion
    }
}
