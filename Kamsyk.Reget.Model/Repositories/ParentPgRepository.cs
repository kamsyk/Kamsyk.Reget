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
using System.Data.Entity.Infrastructure;

namespace Kamsyk.Reget.Model.Repositories {
    public class ParentPgRepository : BaseRepository<Parent_Purchase_Group> {
        #region Constant
        public const string COMPANY_SELECTED_PREFIX = "companySelected_";
        public const string COMPANY_NAME_PREFIX = "companyName_";
        public const string PARENT_PG_NAME = "name";
        #endregion

        #region Enums
        //private enum CompanyFilterYn {
        //    No,
        //    Yes
        //}
        #endregion

        #region Struct
        private struct CompanyFilter {
            public int CompanyId;
            public bool IsYes;

            public CompanyFilter(int companyId, bool isYes) {
                CompanyId = companyId;
                IsYes = isYes;
            }
        }
        #endregion

        #region Methods


        public List<object> GetParentPgAdminData(
            //List<int> companyIds,
            int purchaseGroupId,
            string filter,
            string sort,
            int pageSize,
            int pageNr,
            string cultureName,
            int currentUserId,
            string rootUrl,
            out int rowsCount) {

            string ppgNameFilter = null;
            List<CompanyFilter> companyFilters = null;
            string strFilterWhere = GetFilter(filter, purchaseGroupId,  out ppgNameFilter, out companyFilters);
            string strOrder = GetOrder(sort);

            string sqlPure = "SELECT ppgd." + ParentPurchaseGroupData.ID_FIELD + "," +
                "ppgld." + ParentPurchaseGroupLocalData.LOCAL_TEXT_FIELD + " AS " + ParentPurchaseGroupData.NAME_FIELD +
                "," + ParentPurchaseGroupData.ICON_NAME_FIELD +
                "," + ParentPurchaseGroupData.COMPANY_GROUP_ID_FIELD +
                ", ROW_NUMBER() OVER(" + strOrder + ") AS RowNum";

            string sqlPureBody = GetPureBody(cultureName);
            
            sqlPureBody += strFilterWhere;


            //Get Row count
            rowsCount = 0;
            List<Parent_Purchase_Group> tmpParentPgs = null;

            //Get Part Data
            string sqlPart = sqlPure + sqlPureBody;
            int partStart = pageSize * (pageNr - 1) + 1;
            int partStop = partStart + pageSize - 1;
            if (companyFilters == null || companyFilters.Count == 0) {
                string selectCount = "SELECT COUNT(*) " + sqlPureBody;
                rowsCount = m_dbContext.Database.SqlQuery<int>(selectCount).Single();
                                
                while (partStart > rowsCount) {
                    partStart -= pageSize;
                    partStart = partStart + pageSize - 1;
                }

                string sql = "SELECT * FROM(" + sqlPart + ") AS RegetPartData" +
                    " WHERE RegetPartData.RowNum BETWEEN " + partStart + " AND " + partStop;
               
                tmpParentPgs = m_dbContext.Database.SqlQuery<Parent_Purchase_Group>(sql).ToList();
            } else {
                tmpParentPgs = FilterParentPgsByCompany(
                    cultureName, 
                    ppgNameFilter, 
                    companyFilters, 
                    partStart - 1, 
                    partStop - 1, 
                    out rowsCount);
                
                //while (partStart > rowsCount) {
                //    partStart -= pageSize;
                //    partStart = partStart + pageSize - 1;
                //}
            }

            var partCompAdmin = (from partCompDb in m_dbContext.Participant_Office_Role
                                 join compDb in m_dbContext.Company
                                 on partCompDb.office_id equals compDb.id
                                 where partCompDb.participant_id == currentUserId &&
                                 partCompDb.role_id == (int)UserRole.OfficeAdministrator
                                 orderby compDb.country_code
                                 select partCompDb).ToList();


            if (partCompAdmin == null) {
                return null;
            }

            List<object> parentPgs = new List<object>();
            int rowIndex = (pageNr - 1) * pageSize + 1;
            if (tmpParentPgs != null) {
                foreach (var parentPg in tmpParentPgs) {
                    object oParentPg = GetParentPgDynamic(parentPg, partCompAdmin, rowIndex, cultureName, rootUrl);
                    parentPgs.Add(oParentPg);
                    rowIndex++;
                }
            }


            return parentPgs;

        }

        private string GetPureBody(string cultureName) {
            string sqlPureBody = " FROM " + ParentPurchaseGroupData.TABLE_NAME + " ppgd" +
                " LEFT OUTER JOIN " + ParentPurchaseGroupLocalData.TABLE_NAME + " ppgld" +
                " ON ppgd." + ParentPurchaseGroupData.ID_FIELD + "=" + "ppgld." + ParentPurchaseGroupLocalData.PARENT_PURCHASE_GROUP_ID_FIELD +
                " AND ppgld." + ParentPurchaseGroupLocalData.CULTURE_FIELD + "='" + cultureName + "'";

            return sqlPureBody;

        }

        private List<Parent_Purchase_Group> FilterParentPgsByCompany(
            string cultureName,
            string ppgNameFilter, 
            List<CompanyFilter> companyFilters, 
            int partStart, 
            int partStop,
            out int rowsCount) {

            List<Parent_Purchase_Group> tmpParentPgsRaw = null;
            if (String.IsNullOrEmpty(ppgNameFilter)) {
                tmpParentPgsRaw = (from ppgDb in m_dbContext.Parent_Purchase_Group
                                   select ppgDb).ToList();
            } else {
                tmpParentPgsRaw = (from ppgDb in m_dbContext.Parent_Purchase_Group
                                   join ppglDb in m_dbContext.Parent_Purchase_Group_Local
                                   on ppgDb.id equals ppglDb.parent_purchase_group_id
                                   where ppglDb.culture.ToLower() == cultureName.ToLower() 
                                   && ppglDb.local_text.ToLower().Contains(ppgNameFilter.ToLower())
                                   select ppgDb).ToList();
            }
                        
            List<Parent_Purchase_Group> tmpParentPgs = new List<Parent_Purchase_Group>();

            
            //int rowPos = 0;
            rowsCount = 0;
            foreach (Parent_Purchase_Group tmpParentPgRaw in tmpParentPgsRaw) {
                bool isVisible = true;
                foreach (CompanyFilter companyFilter in companyFilters) {
                    
                    var ppgComp = (from ppgCDb in tmpParentPgRaw.Company
                                   where ppgCDb.id == companyFilter.CompanyId
                                   select ppgCDb).FirstOrDefault();
                    if (ppgComp != null && !companyFilter.IsYes) {
                        isVisible = false;
                        break;
                    }
                    if (ppgComp == null && companyFilter.IsYes) {
                        isVisible = false;
                        break;
                    }
                }

                if (isVisible) {
                    if (partStop == 0 || (rowsCount >= partStart && rowsCount <= partStop)) {
                        tmpParentPgs.Add(tmpParentPgRaw);
                    }
                    rowsCount++;
                    
                }
            }

            return tmpParentPgs;
        }

        private string GetFilter(string filter, int companyGroupId, out string ppgNameFilter, out List<CompanyFilter> companyFilter) {
            ppgNameFilter = null;
            companyFilter = null;
            
            string strFilterWhere = " WHERE ppgd." + ParentPurchaseGroupData.COMPANY_GROUP_ID_FIELD + "=" + companyGroupId;
            if (!String.IsNullOrEmpty(filter)) {
                string[] filterItems = filter.Split(UrlParamDelimiter.ToCharArray());
                foreach (string filterItem in filterItems) {
                    string[] strItemProp = filterItem.Split(UrlParamValueDelimiter.ToCharArray());
                    
                    string columnName = strItemProp[0].Trim().ToUpper();
                    string filterValue = strItemProp[1].Trim().ToUpper();
                    if (columnName == ParentPurchaseGroupData.NAME_FIELD.Trim().ToUpper()) {
                        strFilterWhere += " AND ";
                        strFilterWhere += "ppgld." + ParentPurchaseGroupLocalData.LOCAL_TEXT_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'";
                        ppgNameFilter = strItemProp[1].Trim();
                    } else if (columnName.ToLower().StartsWith(COMPANY_SELECTED_PREFIX.ToLower())) {
                        string strCompId = columnName.Replace(COMPANY_SELECTED_PREFIX.ToUpper(), "");
                        int companyId = Convert.ToInt16(strCompId);
                        if (companyFilter == null) {
                            companyFilter = new List<CompanyFilter>();
                        }
                        if (filterValue == "TRUE") {
                            companyFilter.Add(new CompanyFilter(companyId, true));
                        } else {
                            companyFilter.Add(new CompanyFilter(companyId, false));
                        }
                    }
                }
            }

            return strFilterWhere;
        }

        private string GetOrder(string sort) {
            string strOrder = "ORDER BY ppgd." + ParentPurchaseGroupData.ID_FIELD;

            if (!String.IsNullOrEmpty(sort)) {
                strOrder = "";
                string[] sortItems = sort.Split(UrlParamDelimiter.ToCharArray());
                foreach (string sortItem in sortItems) {
                    string[] strItemProp = sortItem.Split(UrlParamValueDelimiter.ToCharArray());
                    if (strOrder.Length > 0) {
                        strOrder += ", ";
                    }
                    
                    if (strItemProp[0].ToLower() == ParentPurchaseGroupData.NAME_FIELD.ToLower()) {
                        strOrder = "ppgld." + ParentPurchaseGroupLocalData.LOCAL_TEXT_FIELD + " " + strItemProp[1];
                    } 
                }

                strOrder = " ORDER BY " + strOrder;
            }

            return strOrder;
        }

        public List<UsedPgList> GetParentPgAdminData(int parentPgId, List<int> companies) {
            List<UsedPgList> compParentPpgList = new List<UsedPgList>();

            var ppg = (from ppgDb in m_dbContext.Parent_Purchase_Group
                        where ppgDb.id == parentPgId
                        select ppgDb).FirstOrDefault();

            if (ppg == null) {
                return null;
            }


            var comp = (from comDb in ppg.Company
                        where companies.Contains(comDb.id)
                        select ppg).FirstOrDefault();
            if (comp == null) {
                return null;
            }

            int comId = comp.id;

            var cppgs = (from cppgsDb in compParentPpgList
                         select cppgsDb).FirstOrDefault();

            if (cppgs == null) {
                UsedPgList tmpParentPgCompanyPgList = new UsedPgList();
                tmpParentPgCompanyPgList.company_id = comId;
                List<UsedPg> tmpParentPgCompanyPg = new List<UsedPg>();
                UsedPg parentPgCompanyPg = new UsedPg();
                //parentPgCompanyPg.id = ppg
                //tmpParentPgCompanyPgList.purchase_groups
            }

            UsedPgList parentPgCompanyPgList = new UsedPgList();
            //parentPgCompanyPgList.company_id = ppg.Company.i


            return null;
        }

        public List<ParentPgExtended> GetParentPgAdminData(List<int> companies, string strCultureName) {
            var tmpCompanies = (from compDb in m_dbContext.Company
                                where companies.Contains(compDb.id)
                                select compDb).ToList();

            if (tmpCompanies == null) {
                return null;
            }

            List<ParentPgExtended> ppgs = new List<ParentPgExtended>();

            foreach (var comp in tmpCompanies) {
                if (comp.Parent_Purchase_Group == null) {
                    continue;
                }

                foreach (var ppg in comp.Parent_Purchase_Group) {
                    var exPpg = (from ppgsDb in ppgs
                                 where ppgsDb.id == ppg.id
                                 select ppgsDb).FirstOrDefault();
                    if (exPpg != null) {
                        continue;
                    }

                    ParentPgExtended ppgEx = new ParentPgExtended();

                    //var locName = (from ppgLocDb in m_dbContext.Parent_Purchase_Group_Local
                    //               where ppgLocDb.parent_purchase_group_id == ppg.id &&
                    //               ppgLocDb.culture == strCultureName
                    //               select new { local_text = ppgLocDb.local_text }).FirstOrDefault();
                    //string parentPgName = (locName == null) ? ppg.name : locName.local_text;

                    string parentPgName = GetLocalName(ppg.id, strCultureName);

                    ppgEx.id = ppg.id;
                    ppgEx.name = parentPgName;
                    ppgEx.name_wo_diacritics = RemoveDiacritics(parentPgName);
                    ppgs.Add(ppgEx);
                }
            }

            return ppgs.OrderBy(x => x.name).ToList();
        }

        private object GetParentPgDynamic(
            Parent_Purchase_Group ppg, 
            List<Participant_Office_Role> por, 
            int rowIndex, 
            string strCultureName,
            string rootUrl) {
            
            AssemblyName assemblyName = new AssemblyName("Kamsyk.RegetModel");
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("ParentPgCompany");
            TypeBuilder typeBuilder = moduleBuilder.DefineType("ParentPgCompany"
                                , TypeAttributes.Public |
                                TypeAttributes.Class |
                                TypeAttributes.AutoClass |
                                TypeAttributes.AnsiClass |
                                TypeAttributes.BeforeFieldInit |
                                TypeAttributes.AutoLayout
                                , null);

            typeBuilder.DefineField("id", typeof(int), FieldAttributes.Public);
            typeBuilder.DefineField("row_index", typeof(int), FieldAttributes.Public);
            typeBuilder.DefineField(PARENT_PG_NAME, typeof(string), FieldAttributes.Public);
            typeBuilder.DefineField("selected_companies", typeof(List<int>), FieldAttributes.Public);
            typeBuilder.DefineField("local_text", typeof(List<LocalText>), FieldAttributes.Public);
            string compNamePrefix = COMPANY_NAME_PREFIX;
            string compSelectedPrefix = COMPANY_SELECTED_PREFIX;
            string compIsUsedPrefix = "companyIsUsed_";
            foreach (var compAdmin in por) {
                if (compAdmin.Company.active == false) {
                    continue;
                }

                string filedName = compNamePrefix + compAdmin.Company.id;
                typeBuilder.DefineField(filedName, typeof(string), FieldAttributes.Public);
                filedName = compSelectedPrefix + compAdmin.Company.id;
                typeBuilder.DefineField(filedName, typeof(bool), FieldAttributes.Public);
                filedName = compIsUsedPrefix + compAdmin.Company.id;
                typeBuilder.DefineField(filedName, typeof(bool), FieldAttributes.Public);
            }

            Type dynamicType = typeBuilder.CreateType();

            // create an instance of the class
            object destObject = Activator.CreateInstance(dynamicType);

            //set values
            FieldInfo destField = destObject.GetType().GetField("id");
            destField.SetValue(destObject, ppg.id);

            destField = destObject.GetType().GetField("row_index");
            destField.SetValue(destObject, rowIndex);

            var locName = (from ppgLocDb in m_dbContext.Parent_Purchase_Group_Local
                           where ppgLocDb.parent_purchase_group_id == ppg.id && 
                           ppgLocDb.culture == strCultureName
                           select new { local_text = ppgLocDb .local_text}).FirstOrDefault();
            string parentPgName = (locName == null) ? ppg.name : locName.local_text;
            destField = destObject.GetType().GetField("name");
            destField.SetValue(destObject, parentPgName);

            if (rootUrl != null) {
                List<LocalText> localTexts = GetParentPgLocalNames(ppg.id, strCultureName, rootUrl);
                destField = destObject.GetType().GetField("local_text");
                destField.SetValue(destObject, localTexts);
            }

            List<int> selectedCompanies = new List<int>();
            
            foreach (var compAdmin in por) {
                if (compAdmin.Company.active == false) {
                    continue;
                }

                string strColValue = compSelectedPrefix + compAdmin.Company.id + "|" + compAdmin.Company.country_code.Replace("|", "_");
                string filedName = compNamePrefix + compAdmin.Company.id;
                destField = destObject.GetType().GetField(filedName);
                destField.SetValue(destObject, strColValue);

                var ppgCom = (from ppgComDb in compAdmin.Company.Parent_Purchase_Group
                              where ppgComDb.id == ppg.id
                              select ppgComDb).FirstOrDefault();
                bool isSelected = (ppgCom != null);
                bool isUsed = false;
                if (isSelected) {
                    if(ppgCom.Purchase_Group != null || ppgCom.Purchase_Group.Count > 0) {
                        foreach (var pg in ppgCom.Purchase_Group) {
                            if (pg.Centre_Group != null) {
                                foreach (var cg in pg.Centre_Group) {
                                    if (cg.company_id == compAdmin.Company.id && pg.active == true) {
                                        isUsed = true;
                                        break;
                                    }
                                }
                            }
                            if (isUsed) {
                                break;
                            }
                        }
                    }
                             
                }
                filedName = compSelectedPrefix + compAdmin.Company.id;
                destField = destObject.GetType().GetField(filedName);
                destField.SetValue(destObject, isSelected);

                if (isSelected) {
                    if (!selectedCompanies.Contains(compAdmin.Company.id)) {
                        selectedCompanies.Add(compAdmin.Company.id);
                    }
                }

                filedName = compIsUsedPrefix + compAdmin.Company.id;
                destField = destObject.GetType().GetField(filedName);
                destField.SetValue(destObject, isUsed);
            }
                                    
            var selCompaniesField = destObject.GetType().GetField("selected_companies");
            selCompaniesField.SetValue(destObject, selectedCompanies);

            return destObject;
        }

        private List<LocalText> GetParentPgLocalNames(int parentPgId, string strCurrCultureName, string rootUrl) {
            Parent_Purchase_Group parentPg = (from parentPgDb in m_dbContext.Parent_Purchase_Group
                                              where parentPgDb.id == parentPgId
                                              select parentPgDb).FirstOrDefault();

            List<LocalText> localTexts = new List<LocalText>();
            var locCurrText = (from pgLocName in parentPg.Parent_Purchase_Group_Local
                               where pgLocName.culture.ToLower() == strCurrCultureName.ToLower()
                               select pgLocName).FirstOrDefault();

            bool isDefaultLang = (strCurrCultureName == DefaultLanguage);
            Parent_Purchase_Group_Local locDefaultText = null;
            if (!isDefaultLang) {
                locDefaultText = (from pgLocName in parentPg.Parent_Purchase_Group_Local
                                  where pgLocName.culture == DefaultLanguage
                                  select pgLocName).FirstOrDefault();
            }

            //first text
            if (locCurrText != null) {
                LocalText locTxt = new LocalText();
                locTxt.text_id = locCurrText.parent_purchase_group_id;
                locTxt.culture = locCurrText.culture;
                locTxt.label = GetPgLabel(locCurrText.culture);
                locTxt.text = locCurrText.local_text;
                locTxt.flag_url = GetPgFlagUrl(strCurrCultureName, rootUrl);

                localTexts.Add(locTxt);
            } else {
                if (!isDefaultLang && locDefaultText != null) {
                    LocalText locTxt = new LocalText();
                    locTxt.text_id = locDefaultText.parent_purchase_group_id;
                    locTxt.culture = strCurrCultureName;// locDefaultText.culture;
                    locTxt.label = GetPgLabel(strCurrCultureName);
                    locTxt.text = locDefaultText.local_text;
                    locTxt.flag_url = GetPgFlagUrl(strCurrCultureName, rootUrl);

                    localTexts.Add(locTxt);
                } else {
                    LocalText locTxt = new LocalText();
                    locTxt.text_id = parentPg.id;
                    locTxt.culture = strCurrCultureName;
                    locTxt.label = GetPgLabel(strCurrCultureName);
                    locTxt.text = parentPg.name;
                    locTxt.flag_url = GetPgFlagUrl(strCurrCultureName, rootUrl);

                    localTexts.Add(locTxt);
                }
            }

            //second default
            if (!isDefaultLang && locDefaultText != null) {
                LocalText locTxt = new LocalText();
                locTxt.text_id = locDefaultText.parent_purchase_group_id;
                locTxt.culture = locDefaultText.culture;
                locTxt.label = GetPgLabel(locDefaultText.culture);
                locTxt.text = locDefaultText.local_text;
                locTxt.flag_url = GetPgFlagUrl(DefaultLanguage, rootUrl);

                localTexts.Add(locTxt);
            } else if (!isDefaultLang) {
                LocalText locTxt = new LocalText();
                locTxt.text_id = parentPg.id;
                locTxt.culture = DefaultLanguage;
                locTxt.label = GetPgLabel(DefaultLanguage);
                locTxt.text = parentPg.name;
                locTxt.flag_url = GetPgFlagUrl(DefaultLanguage, rootUrl);

                localTexts.Add(locTxt);
            }

                        
            return localTexts;
        }

        //private void AddOtherParentPurchaseGroupLocName(Parent_Purchase_Group parentPg, List<LocalText> localTexts, string strCurrCultureName, string rootUrl) {
        //    var locCurrTextOther = (from pgLocName in parentPg.Parent_Purchase_Group_Local
        //                            where pgLocName.culture == strCurrCultureName
        //                            select pgLocName).FirstOrDefault();

        //    LocalText locTxt = new LocalText();
        //    locTxt.text_id = parentPg.id;
        //    locTxt.culture = strCurrCultureName;
        //    locTxt.label = GetPgLabel(strCurrCultureName);
        //    if (locCurrTextOther != null) {
        //        locTxt.text = locCurrTextOther.local_text;
        //    }
        //    locTxt.flag_url = GetPgFlagUrl(strCurrCultureName, rootUrl);

        //    localTexts.Add(locTxt);
        //}

        public Parent_Purchase_Group GetParentPgByPgId(int pgId) {

            var pg = (from ppgDb in m_dbContext.Purchase_Group
                      where ppgDb.id == pgId
                      select ppgDb).FirstOrDefault();
            if (pg.Parent_Purchase_Group != null && pg.Parent_Purchase_Group.Count > 0) {
                return pg.Parent_Purchase_Group.ElementAt(0);
            }

            return null;
        }

        
        public Parent_Purchase_Group GetParentPgById(int ppgId) {

            var pg = (from ppgDb in m_dbContext.Parent_Purchase_Group
                      where ppgDb.id == ppgId
                      select ppgDb).FirstOrDefault();
            
            return pg;
        }

        public Parent_Purchase_Group GetParentPgByName(string ppgName) { 

            var pg = (from ppgDb in m_dbContext.Parent_Purchase_Group
                      where ppgDb.name == ppgName
                      select ppgDb).FirstOrDefault();

            return pg;
        }

        //public List<UsedPg> GetUsedPgJs(
        //    List<int> companyIds,
        //    string cultureName,
        //    string filter,
        //    string sort,
        //    int pageSize,
        //    int pageNr,
        //    string rootUrl,
        //    int? parentPgId,
        //    out int rowsCount) {

        //    rowsCount = 0;

        //    string strFilterWhere = "";


        //    if (!String.IsNullOrEmpty(filter)) {
        //        string[] filterItems = filter.Split(UrlParamDelimiter.ToCharArray());

        //        foreach (string filterItem in filterItems) {

        //            string[] strItemProp = filterItem.Split(UrlParamValueDelimiter.ToCharArray());
        //            strFilterWhere += " AND ";

        //            string columnName = strItemProp[0].Trim().ToUpper();

        //            if (columnName == "pg_loc_name".Trim().ToUpper()) {
        //                strFilterWhere += "((pgld." + PurchaseGroupLocalData.LOCAL_TEXT_FIELD + " IS NOT NULL" + " AND " +
        //                    "pgld." + PurchaseGroupLocalData.CULTURE_FIELD + "='" + cultureName + "'" + " AND " +
        //                    "pgld." + PurchaseGroupLocalData.LOCAL_TEXT_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%')" + " OR " +
        //                    "(pgld." + PurchaseGroupLocalData.LOCAL_TEXT_FIELD + " IS NULL" + " AND " +
        //                    "pgd." + PurchaseGroupData.GROUP_NAME_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'))";
        //            }

        //            if (columnName == "parent_pg_loc_name".Trim().ToUpper()) {
        //                strFilterWhere += "((ppgld." + ParentPurchaseGroupLocalData.LOCAL_TEXT_FIELD + " IS NOT NULL" + " AND " +
        //                    "ppgld." + ParentPurchaseGroupLocalData.CULTURE_FIELD + "='" + cultureName + "'" + " AND " +
        //                    "ppgld." + ParentPurchaseGroupLocalData.LOCAL_TEXT_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%')" + " OR " +
        //                    "(ppgld." + ParentPurchaseGroupLocalData.LOCAL_TEXT_FIELD + " IS NULL" + " AND " +
        //                    "ppgd." + ParentPurchaseGroupData.NAME_FIELD + "='" + strItemProp[1].Trim() + "'))";
        //            }

        //            if (columnName == "centre_group_name".Trim().ToUpper()) {
        //                strFilterWhere += "cgd." + CentreGroupData.NAME_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'";
        //            }

        //            if (columnName == "company_name".Trim().ToUpper()) {
        //                strFilterWhere += "compd." + CompanyData.COUNTRY_CODE_FIELD + "='" + strItemProp[1].Trim() + "'";
        //            }

        //            if (columnName == PurchaseGroupData.ACTIVE_FIELD.Trim().ToUpper()) {
        //                if (strItemProp[1].Trim().ToLower() == "true") {
        //                    strFilterWhere += "pgd." + PurchaseGroupData.ACTIVE_FIELD + "=1";
        //                } else {
        //                    strFilterWhere += "(pgd." + PurchaseGroupData.ACTIVE_FIELD + "=0" +
        //                        " OR pgd." + PurchaseGroupData.ACTIVE_FIELD + " IS NULL)";
        //                }
        //            }
        //        }

        //    }

        //    string strOrder = "ORDER BY pgd." + PurchaseGroupData.ID_FIELD;

        //    if (!String.IsNullOrEmpty(sort)) {
        //        strOrder = "";
        //        string[] sortItems = sort.Split(UrlParamDelimiter.ToCharArray());
        //        foreach (string sortItem in sortItems) {
        //            string[] strItemProp = sortItem.Split(UrlParamValueDelimiter.ToCharArray());
        //            if (strOrder.Length > 0) {
        //                strOrder += ", ";
        //            }

        //            if (strItemProp[0] == "pg_loc_name") {
        //                strOrder = "pgld." + PurchaseGroupLocalData.LOCAL_TEXT_FIELD + "," + 
        //                    "pgd." + PurchaseGroupData.GROUP_NAME_FIELD + " " + strItemProp[1];
        //            } else if (strItemProp[0] == "parent_pg_loc_name") {
        //                strOrder = "ppgld." + ParentPurchaseGroupLocalData.LOCAL_TEXT_FIELD + "," +
        //                     "ppgd." + ParentPurchaseGroupData.NAME_FIELD + " " + strItemProp[1];
        //            } else if (strItemProp[0] == "centre_group_name") {
        //                strOrder = "cgd." + CentreGroupData.NAME_FIELD + " " + strItemProp[1];
        //            } else if (strItemProp[0] == "company_name") {
        //                strOrder = "compd." + CompanyData.COUNTRY_CODE_FIELD + " " + strItemProp[1];
        //            } else if (strItemProp[0] == "active") {
        //                strOrder = "pgd." + PurchaseGroupData.ACTIVE_FIELD + " " + strItemProp[1];
        //            }
        //        }

        //        strOrder = " ORDER BY " + strOrder;
        //    }

        //    string sqlPure = "SELECT " + 
        //        " pgd." + PurchaseGroupData.ID_FIELD + " as 'id'," +
        //        " pgd." + PurchaseGroupData.GROUP_NAME_FIELD + " as 'pg_name'," +
        //        " pgd." + PurchaseGroupData.ACTIVE_FIELD + " as 'active'," +
        //        " ppgd." + ParentPurchaseGroupData.ID_FIELD + " as 'parent_pg_id'," +
        //        " ppgd." + ParentPurchaseGroupData.NAME_FIELD + " as 'parent_pg_name'," +
        //        " ppgld." + ParentPurchaseGroupLocalData.LOCAL_TEXT_FIELD + " as 'parent_pg_loc_name'," +
        //        " pgld." + PurchaseGroupLocalData.LOCAL_TEXT_FIELD + " as 'pg_loc_name'," +
        //        " cgd." + CentreGroupData.ID_FIELD + " as 'centre_group_id'," +
        //        " cgd." + CentreGroupData.NAME_FIELD + " as 'centre_group_name'," +
        //        " compd." + CompanyData.ID_FIELD + " as 'company_id'," +
        //        " compd." + CompanyData.COUNTRY_CODE_FIELD + " as 'company_name'," +
        //        " ROW_NUMBER() OVER(" + strOrder + ") AS RowNum";

        //    string sqlPureBody = " FROM " + PurchaseGroupData.TABLE_NAME + " pgd" +
        //        " INNER JOIN " + ParentPurchasePurchaseGroupData.TABLE_NAME + " pppgd" +
        //        " ON pgd." + PurchaseGroupData.ID_FIELD + "=pppgd." + ParentPurchasePurchaseGroupData.PURCHASE_GROUP_ID_FIELD +
        //        " INNER JOIN " + CentregroupPurchasegroupData.TABLE_NAME + " cgpgd" +
        //        " ON pgd." + PurchaseGroupData.ID_FIELD + "=cgpgd." + CentregroupPurchasegroupData.PURCHASE_GROUP_ID_FIELD +
        //        " INNER JOIN " + CentreGroupData.TABLE_NAME + " cgd" +
        //        " ON cgpgd." + CentregroupPurchasegroupData.CENTRE_GROUP_ID_FIELD + "=cgd." + CentreGroupData.ID_FIELD +
        //        " INNER JOIN " + ParentPurchaseGroupData.TABLE_NAME + " ppgd" +
        //        " ON ppgd." + ParentPurchaseGroupData.ID_FIELD + "=pppgd." + ParentPurchasePurchaseGroupData.PARENT_PURCHASE_GROUP_ID_FIELD +
        //        " INNER JOIN " + CompanyData.TABLE_NAME + " compd" +
        //        " ON compd." + CompanyData.ID_FIELD + "=cgd." + CentreGroupData.COMPANY_ID_FIELD +
        //        " LEFT OUTER JOIN " + PurchaseGroupLocalData.TABLE_NAME + " pgld" +
        //        " ON pgld." + PurchaseGroupLocalData.PURCHASE_GROUP_ID_FIELD + "=pgd." + PurchaseGroupData.ID_FIELD +
        //        " LEFT OUTER JOIN " + ParentPurchaseGroupLocalData.TABLE_NAME + " ppgld" +
        //        " ON ppgld." + ParentPurchaseGroupLocalData.PARENT_PURCHASE_GROUP_ID_FIELD + "=pppgd." + ParentPurchasePurchaseGroupData.PARENT_PURCHASE_GROUP_ID_FIELD +
        //        " WHERE " +
        //        " cgd." + CentreGroupData.COMPANY_ID_FIELD + " IN" + GetSqlIn(companyIds) +
        //        " AND pgld." + PurchaseGroupLocalData.CULTURE_FIELD + "='" + cultureName + "'" +
        //        " AND ppgld." + ParentPurchaseGroupLocalData.CULTURE_FIELD + "='" + cultureName + "'";
        //    if (parentPgId != null) {
        //        sqlPureBody += " AND pppgd." + ParentPurchasePurchaseGroupData.PARENT_PURCHASE_GROUP_ID_FIELD + "=" + parentPgId;
        //    }
        //    //" AND pgd." + PurchaseGroupData.ACTIVE_FIELD + "=1";
        //    sqlPureBody += strFilterWhere;

        //    //Get Row count
        //    string selectCount = "SELECT COUNT(*) " + sqlPureBody;
        //    rowsCount = m_dbContext.Database.SqlQuery<int>(selectCount).Single();

        //    //Get Part Data
        //    string sqlPart = sqlPure + sqlPureBody;
        //    int partStart = pageSize * (pageNr - 1) + 1;
        //    int partStop = partStart + pageSize - 1;

        //    while (partStart > rowsCount) {
        //        partStart -= pageSize;
        //        partStart = partStart + pageSize - 1;
        //    }

        //    string sql = "SELECT * FROM(" + sqlPart + ") AS RegetPartData" +
        //        " WHERE RegetPartData.RowNum BETWEEN " + partStart + " AND " + partStop;

        //    var tmpPgs = m_dbContext.Database.SqlQuery<UsedPg>(sql).ToList();

        //    if (tmpPgs == null && tmpPgs.Count == 0) {
        //        return null;
        //    }

        //    int rowIndex = (pageNr - 1) * pageSize + 1;

        //    //List<ParentPgCompanyPg> parentPgCompanyPgs = new List<ParentPgCompanyPg>();
        //    foreach (var tmpPg in tmpPgs) {

        //        if (String.IsNullOrEmpty(tmpPg.pg_loc_name)) {
        //            tmpPg.pg_loc_name = tmpPg.pg_name;
        //        }

        //        if (String.IsNullOrEmpty(tmpPg.parent_pg_loc_name)) {
        //            tmpPg.parent_pg_loc_name = tmpPg.parent_pg_name;
        //        }

        //        tmpPg.local_text = new List<LocalText>();
        //        LocalText localText = new LocalText();
        //        localText.text_id = tmpPg.id;
        //        localText.culture = cultureName;
        //        localText.label = GetPgLabel(cultureName);
        //        localText.text = tmpPg.pg_loc_name;
        //        localText.flag_url = GetPgFlagUrl(cultureName, rootUrl);
        //        tmpPg.local_text.Add(localText);

        //        if (DefaultLanguage != cultureName) {
        //            localText = new LocalText();
        //            localText.text_id = tmpPg.id;
        //            localText.culture = DefaultLanguage;
        //            localText.label = GetPgLabel(DefaultLanguage);
        //            localText.text = tmpPg.pg_name;
        //            localText.flag_url = GetPgFlagUrl(DefaultLanguage, rootUrl);
        //            tmpPg.local_text.Add(localText);
        //        }

        //        tmpPg.row_index = rowIndex;

        //        rowIndex++;
        //    }

        //    return tmpPgs;
        //}

        public List<object> GetParentPgReport(
            int purchaseGroupId,
            string filter,
            string sort,
            string cultureName,
            int currentUserId) {

            string sqlPure = "SELECT ppgd." + ParentPurchaseGroupData.ID_FIELD +
                "," + "ppgld." + ParentPurchaseGroupLocalData.LOCAL_TEXT_FIELD + " AS " + ParentPurchaseGroupData.NAME_FIELD +
                "," + "ppgd." + ParentPurchaseGroupData.COMPANY_GROUP_ID_FIELD +
                "," + "ppgd." + ParentPurchaseGroupData.ICON_NAME_FIELD;

            string ppgNameFilter = null;
            List<CompanyFilter> companyFilters = null;
            string strFilterWhere = GetFilter(filter, purchaseGroupId, out ppgNameFilter, out companyFilters);
            string strOrder = GetOrder(sort);

            string sqlPureBody = GetPureBody(cultureName);
            sqlPureBody += strFilterWhere;
            sqlPureBody += strOrder;

            string sql = sqlPure + sqlPureBody;

            List<Parent_Purchase_Group> tmpParentPgs = null;
            if (companyFilters == null || companyFilters.Count == 0) {
                tmpParentPgs = m_dbContext.Database.SqlQuery<Parent_Purchase_Group>(sql).ToList();
            } else {
                int rowsCount = 0;
                tmpParentPgs = FilterParentPgsByCompany(
                    cultureName,
                    ppgNameFilter,
                    companyFilters,
                    0,
                    0,
                    out rowsCount);
            }

            var partCompAdmin = (from partCompDb in m_dbContext.Participant_Office_Role
                                 join compDb in m_dbContext.Company 
                                 on partCompDb.office_id equals compDb.id
                                 where partCompDb.participant_id == currentUserId &&
                                 partCompDb.role_id == (int)UserRole.OfficeAdministrator
                                 orderby compDb.country_code
                                 select partCompDb).ToList();


            if (partCompAdmin == null) {
                return null;
            }

            List<object> parentPgs = new List<object>();
            if (tmpParentPgs != null) {
                foreach (var parentPg in tmpParentPgs) {
                    object oParentPg = GetParentPgDynamic(
                        parentPg, partCompAdmin, 0, cultureName, null);
                    parentPgs.Add(oParentPg);
                }
            }

            return parentPgs;
        }


        
        //public List<UsedPg> GetPgsByCompIdParentPgId(
        //    int parentPgId,
        //    List<int> companyIds,
        //    string cultureName) {


        //    string sqlPure = "SELECT " +
        //        " pgd." + PurchaseGroupData.ID_FIELD + " as 'pg_id'," +
        //        " pgd." + PurchaseGroupData.GROUP_NAME_FIELD + " as 'pg_name'," +
        //        " pgd." + PurchaseGroupData.ACTIVE_FIELD + " as 'active'," +
        //        " ppgd." + ParentPurchaseGroupData.ID_FIELD + " as 'parent_pg_id'," +
        //        " ppgd." + ParentPurchaseGroupData.NAME_FIELD + " as 'parent_pg_name'," +
        //        " ppgld." + ParentPurchaseGroupLocalData.LOCAL_TEXT_FIELD + " as 'parent_pg_loc_name'," +
        //        " pgld." + PurchaseGroupLocalData.LOCAL_TEXT_FIELD + " as 'pg_loc_name'," +
        //        " cgd." + CentreGroupData.ID_FIELD + " as 'centre_group_id'," +
        //        " cgd." + CentreGroupData.NAME_FIELD + " as 'centre_group_name'," +
        //        " compd." + CompanyData.ID_FIELD + " as 'company_id'," +
        //        " compd." + CompanyData.COUNTRY_CODE_FIELD + " as 'company_name'";

        //    string sqlPureBody = " FROM " + PurchaseGroupData.TABLE_NAME + " pgd" +
        //        " INNER JOIN " + ParentPurchasePurchaseGroupData.TABLE_NAME + " pppgd" +
        //        " ON pgd." + PurchaseGroupData.ID_FIELD + "=pppgd." + ParentPurchasePurchaseGroupData.PURCHASE_GROUP_ID_FIELD +
        //        " INNER JOIN " + CentregroupPurchasegroupData.TABLE_NAME + " cgpgd" +
        //        " ON pgd." + PurchaseGroupData.ID_FIELD + "=cgpgd." + CentregroupPurchasegroupData.PURCHASE_GROUP_ID_FIELD +
        //        " INNER JOIN " + CentreGroupData.TABLE_NAME + " cgd" +
        //        " ON cgpgd." + CentregroupPurchasegroupData.CENTRE_GROUP_ID_FIELD + "=cgd." + CentreGroupData.ID_FIELD +
        //        " INNER JOIN " + ParentPurchaseGroupData.TABLE_NAME + " ppgd" +
        //        " ON ppgd." + ParentPurchaseGroupData.ID_FIELD + "=pppgd." + ParentPurchasePurchaseGroupData.PARENT_PURCHASE_GROUP_ID_FIELD +
        //        " INNER JOIN " + CompanyData.TABLE_NAME + " compd" +
        //        " ON compd." + CompanyData.ID_FIELD + "=cgd." + CentreGroupData.COMPANY_ID_FIELD +
        //        " LEFT OUTER JOIN " + PurchaseGroupLocalData.TABLE_NAME + " pgld" +
        //        " ON pgld." + PurchaseGroupLocalData.PURCHASE_GROUP_ID_FIELD + "=pgd." + PurchaseGroupData.ID_FIELD +
        //        " LEFT OUTER JOIN " + ParentPurchaseGroupLocalData.TABLE_NAME + " ppgld" +
        //        " ON ppgld." + ParentPurchaseGroupLocalData.PARENT_PURCHASE_GROUP_ID_FIELD + "=pppgd." + ParentPurchasePurchaseGroupData.PARENT_PURCHASE_GROUP_ID_FIELD +
        //        " WHERE " +
        //        " pppgd." + ParentPurchasePurchaseGroupData.PARENT_PURCHASE_GROUP_ID_FIELD + "=" + parentPgId +
        //        " AND cgd." + CentreGroupData.COMPANY_ID_FIELD + " IN" + GetSqlIn(companyIds) +
        //        //" AND cgd." + CentreGroupData.ACTIVE_FIELD + "=1" +
        //        " AND pgld." + PurchaseGroupLocalData.CULTURE_FIELD + "='" + cultureName + "'" +
        //        " AND ppgld." + ParentPurchaseGroupLocalData.CULTURE_FIELD + "='" + cultureName + "'";

        //    string sql = sqlPure + sqlPureBody;

        //    var tmpPgs = m_dbContext.Database.SqlQuery<UsedPg>(sql).ToList();

        //    if (tmpPgs == null && tmpPgs.Count == 0) {
        //        return null;
        //    }


        //    foreach (var tmpPg in tmpPgs) {

        //        if (String.IsNullOrEmpty(tmpPg.pg_loc_name)) {
        //            tmpPg.pg_loc_name = tmpPg.pg_name;
        //        }

        //        if (String.IsNullOrEmpty(tmpPg.parent_pg_loc_name)) {
        //            tmpPg.parent_pg_loc_name = tmpPg.parent_pg_name;
        //        }

        //    }

        //    return tmpPgs;
        //}

        private string GetLocalName(int ppgId, string strCultureName) {
            var locName = (from ppgLocDb in m_dbContext.Parent_Purchase_Group_Local
                           where ppgLocDb.parent_purchase_group_id == ppgId &&
                           ppgLocDb.culture == strCultureName
                           select new { local_text = ppgLocDb.local_text }).FirstOrDefault();

            if (locName != null) {
                return locName.local_text;
            }

            var pg = (from ppgDb in m_dbContext.Parent_Purchase_Group
                      where ppgDb.id == ppgId
                      select ppgDb).FirstOrDefault();

            return pg.name;
        }

        //public List<ParentPgCompanyPgList> GetPgsByCompIdParentPgIdJs(int parentPgId, List<int> companyIds, string cultureName) {
        //    var ppgs = (from ppgDb in m_dbContext.Parent_Purchase_Group
        //                where ppgDb.id == parentPgId
        //                select ppgDb).ToList();

        //    List<ParentPgCompanyPgList> ppgList = new List<ParentPgCompanyPgList>();

        //    foreach (var ppg in ppgs) {
        //        if (ppg.Purchase_Group == null) {
        //            continue;
        //        }

        //        Hashtable htcompParentPgs = new Hashtable();

        //        foreach (var pg in ppg.Purchase_Group) {
        //            var cg = (from ppgCgDb in pg.Centre_Group
        //                      where companyIds.Contains(ppgCgDb.company_id)
        //                      select ppgCgDb).FirstOrDefault();

        //            if (cg == null) {
        //                continue;
        //            }

        //            if (cg.Company.active == false) {
        //                continue;
        //            }

        //            int compId = cg.company_id;

        //            var ppgcompList = (from ppgListDb in ppgList
        //                               where ppgListDb.company_id == compId
        //                               select ppgListDb).FirstOrDefault();

        //            if (ppgcompList == null) {
        //                ppgcompList = new ParentPgCompanyPgList();
        //                ppgcompList.company_id = compId;
        //                ppgcompList.company_name = cg.Company.country_code;
        //                ppgcompList.purchase_groups = new List<ParentPgCompanyPg>();
        //                ppgList.Add(ppgcompList);
        //            }

        //            ParentPgCompanyPg parentPgCompanyPg = new ParentPgCompanyPg();
        //            parentPgCompanyPg.id = pg.id;
        //            parentPgCompanyPg.parent_pg_id = parentPgId;
        //            parentPgCompanyPg.pg_name = pg.group_name;
        //            parentPgCompanyPg.pg_loc_name = new PgRepository().GetLocalName(pg.id, cultureName);

        //            int cgId = DataNulls.INT_NULL;
        //            if (pg.Centre_Group != null && pg.Centre_Group.Count > 0) {
        //                cgId = pg.Centre_Group.ElementAt(0).id;
        //                parentPgCompanyPg.centre_group_name = pg.Centre_Group.ElementAt(0).name;
        //            }
        //            parentPgCompanyPg.centre_group_id = cgId;

        //            //parent pgs list
        //            if (htcompParentPgs.ContainsKey(cg.company_id)) {
        //                parentPgCompanyPg.parent_pgs = (List<AgDropDown>)htcompParentPgs[cg.company_id];
        //            } else { 
        //                if (cg.Company.Parent_Purchase_Group == null) {
        //                    parentPgCompanyPg.parent_pgs = null;
        //                    htcompParentPgs.Add(cg.company_id, null);
        //                } else {
        //                    List<AgDropDown> agDropDowns = new List<AgDropDown>();
        //                    foreach (var cppg in cg.Company.Parent_Purchase_Group) {
        //                        AgDropDown agDropDown = new AgDropDown();
        //                        agDropDown.value = cppg.id.ToString();
        //                        agDropDown.label = cppg.name;
        //                        agDropDowns.Add(agDropDown);
        //                    }

        //                    htcompParentPgs.Add(cg.company_id, agDropDowns);
        //                    parentPgCompanyPg.parent_pgs = agDropDowns;
        //                }
        //            }


        //            ppgcompList.purchase_groups.Add(parentPgCompanyPg);
        //        }
        //    }

        //    return ppgList;
        //}

        public int SaveParentPg(ParentPgExtended modifParentPg, int userId, string cultureCode, out List<string> errMsg) {
            errMsg = null;

            //Duplicity Check
            var ppglDup = (from ppglDb in m_dbContext.Parent_Purchase_Group_Local
                        where ppglDb.parent_purchase_group_id != modifParentPg.id &&
                        ppglDb.culture == cultureCode &&
                        ppglDb.local_text == modifParentPg.name
                        select ppglDb).FirstOrDefault();
            if (ppglDup != null) {
                errMsg = new List<string>();
                errMsg.Add(DUPLICITY);
                return DataNulls.INT_NULL;
            }

            Parent_Purchase_Group ppgDb = null;
            using (TransactionScope transaction = new TransactionScope()) {
                try {
                    if (modifParentPg.id < 0) {
                        Parent_Purchase_Group newPpg = new Parent_Purchase_Group();
                        newPpg.name = modifParentPg.name;
                        var lastParentPg = (from ppgd in m_dbContext.Parent_Purchase_Group
                                         orderby ppgd.id descending
                                         select ppgd).Take(1).FirstOrDefault();
                        int lastId = -1;
                        if (lastParentPg != null) {
                            lastId = lastParentPg.id;
                        }
                        lastId++;
                        newPpg.id = lastId;

                        //Parent pg Local
                        Parent_Purchase_Group_Local newPpgl = new Parent_Purchase_Group_Local();
                        newPpgl.parent_purchase_group_id = newPpg.id;
                        newPpgl.culture = cultureCode;
                        newPpgl.local_text = modifParentPg.name;
                        newPpg.Parent_Purchase_Group_Local.Add(newPpgl);

                        //Add Companies
                        if (modifParentPg.selected_companies != null) {
                            foreach (int compId in modifParentPg.selected_companies) {
                                Company company = (from compDb in m_dbContext.Company
                                                       where compDb.id == compId
                                                       select compDb).FirstOrDefault();
                                newPpg.Company.Add(company);
                                
                            }
                        }

                        m_dbContext.Parent_Purchase_Group.Add(newPpg);

                        ppgDb = newPpg;

                    } else {
                        ppgDb = GetParentPgById(modifParentPg.id);
                        //Local Texts
                        var ppgl = (from ppglDb in ppgDb.Parent_Purchase_Group_Local
                                    where ppglDb.culture == cultureCode
                                    select ppglDb).FirstOrDefault();

                        if (ppgl == null) {
                            Parent_Purchase_Group_Local newPpgl = new Parent_Purchase_Group_Local();
                            newPpgl.parent_purchase_group_id = modifParentPg.id;
                            newPpgl.local_text = modifParentPg.name;
                            m_dbContext.Parent_Purchase_Group_Local.Add(newPpgl);
                        } else {
                            if (ppgl.local_text != modifParentPg.name) {
                                ppgl.local_text = modifParentPg.name;
                            }
                        }
                        if (cultureCode == DefaultLanguage) {
                            ppgDb.name = modifParentPg.name;
                        }

                        //Delete Company
                        if (ppgDb.Company != null) {
                            for (int i = ppgDb.Company.Count - 1; i >= 0; i--) {
                                var compAdmin = (from compAdminDb in ppgDb.Company.ElementAt(i).Participant_Office_Role
                                                 where compAdminDb.participant_id == userId && compAdminDb.role_id == (int)UserRole.OfficeAdministrator
                                                 select compAdminDb).FirstOrDefault();

                                if (compAdmin == null) {
                                    continue;
                                }

                                if (modifParentPg.selected_companies == null || !modifParentPg.selected_companies.Contains(ppgDb.Company.ElementAt(i).id)) {
                                    DeleteParentPgCompany(modifParentPg.id, ppgDb.Company.ElementAt(i).id);
                                }
                            }
                        }

                        //Add Companies
                        if (modifParentPg.selected_companies != null) {
                            foreach (int compId in modifParentPg.selected_companies) {
                                bool isCompExist = false;
                                if (ppgDb.Company != null) {
                                    var dbcomp = (from tmpComp in ppgDb.Company
                                                  where tmpComp.id == compId
                                                  select tmpComp).FirstOrDefault();
                                    if (dbcomp != null) {
                                        isCompExist = true;
                                    }
                                }

                                if (!isCompExist) {
                                    Company company = (from compDb in m_dbContext.Company
                                                       where compDb.id == compId
                                                       select compDb).FirstOrDefault();
                                    ppgDb.Company.Add(company);
                                }
                            }
                        }
                        
                    }

                    SaveChanges();
                    transaction.Complete();

                    
                } catch (Exception ex) {
                    throw ex;

                }
            }

            return ppgDb.id;
        }

        public void DeleteParentPgCompany(int ppgId, int companyId) {

            var pg = (from ppgDb in m_dbContext.Parent_Purchase_Group
                      where ppgDb.id == ppgId
                      select ppgDb).FirstOrDefault();

            var comp = (from compDb in pg.Company
                        where compDb.id == companyId
                        select compDb).FirstOrDefault();

            pg.Company.Remove(comp);

            m_dbContext.SaveChanges();
            
        }

        public List<string> SaveUsedPgData(UsedPg usedPg, string cultureCode) {
            using (TransactionScope transaction = new TransactionScope()) {
                try {
                    
                    if (usedPg.parent_pg_id < 0) {
                        throw new Exception("Purchase Group Id must not be a negative number");
                    } else {
                        //duplicity check
                        List<LocalText> localTexts = new List<LocalText>();
                        LocalText localText = new LocalText();
                        localText.text = usedPg.pg_loc_name;
                        localText.culture = cultureCode;
                        localTexts.Add(localText);
                        //List<string> errMsg = new PgRepository().GetDuplicityCheck(usedPg.id, localTexts);
                        //if (errMsg != null) {
                        //    return errMsg;
                        //}

                        Purchase_Group pg = (from pgd in m_dbContext.Purchase_Group
                                               where pgd.id == usedPg.id
                                             select pgd).FirstOrDefault(); 

                        var pgLocal = (from pgld in pg.Purchase_Group_Local
                                       where pgld.culture == cultureCode
                                       select pgld).FirstOrDefault();

                        if (pgLocal != null) {
                            if (pgLocal.local_text != usedPg.pg_loc_name) {
                                pgLocal.local_text = usedPg.pg_loc_name;
                                //m_dbContext.Entry(pg).State = EntityState.Modified;
                                
                            }
                        } else {
                            if (pg.group_name != usedPg.pg_loc_name) {
                                pg.group_name = usedPg.pg_loc_name;
                            }
                        }

                        if (pg.active != usedPg.active) {
                            pg.active = usedPg.active;
                        }

                        Parent_Purchase_Group ppg = pg.Parent_Purchase_Group.ElementAt(0);
                    }

                    SaveChanges();
                    transaction.Complete();

                    return null;
                } catch (Exception ex) {
                    throw ex;

                }
            }
        }

        public List<string> SaveParentPgTranslation(int parentPgId, List<LocalText> localTexts) {
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

            //duplicity check
            foreach (var localText in localTexts) {
                string strCulture = localText.culture;
                var parentPgLoc = (from parentPgLocDb in m_dbContext.Parent_Purchase_Group_Local
                                   where parentPgLocDb.parent_purchase_group_id != parentPgId &&
                                   parentPgLocDb.culture == strCulture &&
                                   parentPgLocDb.local_text == localText.text
                                   select parentPgLocDb).FirstOrDefault();
                if (parentPgLoc != null) {
                    if (errMsgs == null) {
                        errMsgs = new List<string>();
                    }
                    errMsgs.Add(DUPLICITY);
                    errMsgs.Add(localText.text);
                    return errMsgs;
                }
            }

            using (TransactionScope transaction = new TransactionScope()) {
                try {

                    foreach (var localText in localTexts) {
                        string strCulture = localText.culture;
                        var parentPgLoc = (from parentPgLocDb in m_dbContext.Parent_Purchase_Group_Local
                                           where parentPgLocDb.parent_purchase_group_id == parentPgId &&
                                           parentPgLocDb.culture == strCulture
                                           select parentPgLocDb).FirstOrDefault();
                        if (parentPgLoc == null) {
                            Parent_Purchase_Group_Local ppgl = new Parent_Purchase_Group_Local();
                            ppgl.parent_purchase_group_id = parentPgId;
                            ppgl.culture = strCulture;
                            ppgl.local_text = localText.text;
                            m_dbContext.Parent_Purchase_Group_Local.Add(ppgl);
                            if (strCulture == DefaultLanguage) {
                                SetDefaultPpgName(parentPgId, localText.text);
                            }
                            SaveChanges();
                        } else {
                            if (parentPgLoc.local_text != localText.text) {
                                parentPgLoc.local_text = localText.text;
                                if (strCulture == DefaultLanguage) {
                                    SetDefaultPpgName(parentPgId, localText.text);
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

        private void SetDefaultPpgName(int parentPgId, string name) {
            Parent_Purchase_Group ppg = (from ppgDb in m_dbContext.Parent_Purchase_Group
                                         where ppgDb.id == parentPgId
                                         select ppgDb).FirstOrDefault();

            if (ppg != null) {
                ppg.name = name;
            }
        }
                
        //public void SetMissingLocalText(string culture) {
        //    string sql = "SELECT ppgd.*" +
        //        " FROM " + ParentPurchaseGroupData.TABLE_NAME + " ppgd" +
        //        " LEFT OUTER JOIN " + ParentPurchaseGroupLocalData.TABLE_NAME + " ppgld" +
        //        " ON ppgd." + ParentPurchaseGroupData.ID_FIELD + "=ppgld." + ParentPurchaseGroupLocalData.PARENT_PURCHASE_GROUP_ID_FIELD +
        //        " AND ppgld." + ParentPurchaseGroupLocalData.CULTURE_FIELD + " = '" + culture + "'" +
        //        " WHERE ppgld." + ParentPurchaseGroupLocalData.LOCAL_TEXT_FIELD + " IS NULL";

        //    var ppgs = m_dbContext.Database.SqlQuery<Parent_Purchase_Group>(sql).ToList();

        //    if (ppgs != null && ppgs.Count > 0) {
        //        foreach (var ppg in ppgs) {
        //            Console.WriteLine(culture + ", PPG " + ppg.name);
        //            var mPpgl = (from ppglDb in m_dbContext.Parent_Purchase_Group_Local
        //                         where ppglDb.parent_purchase_group_id == ppg.id &&
        //                         ppglDb.culture == culture
        //                         select ppglDb).FirstOrDefault();

        //            if (mPpgl == null) {
        //                Parent_Purchase_Group_Local ppgl = new Parent_Purchase_Group_Local();
        //                ppgl.parent_purchase_group_id = ppg.id;
        //                ppgl.culture = culture;
        //                ppgl.local_text = ppg.name;
        //                m_dbContext.Parent_Purchase_Group_Local.Add(ppgl);
        //            } else {
        //                mPpgl.local_text = ppg.name;
        //            }
        //        }

        //        m_dbContext.SaveChanges();
        //    }

        //    //Default lang
        //    sql = "SELECT ppgd.*" +
        //        " FROM " + ParentPurchaseGroupData.TABLE_NAME + " ppgd" +
        //        " LEFT OUTER JOIN " + ParentPurchaseGroupLocalData.TABLE_NAME + " ppgld" +
        //        " ON ppgd." + ParentPurchaseGroupData.ID_FIELD + "=ppgld." + ParentPurchaseGroupLocalData.PARENT_PURCHASE_GROUP_ID_FIELD +
        //        " AND ppgld." + ParentPurchaseGroupLocalData.CULTURE_FIELD + " = '" + DefaultLanguage + "'" +
        //        " WHERE ppgld." + ParentPurchaseGroupLocalData.LOCAL_TEXT_FIELD + " IS NULL";

        //    ppgs = m_dbContext.Database.SqlQuery<Parent_Purchase_Group>(sql).ToList();

        //    if (ppgs != null && ppgs.Count > 0) {
               
        //        foreach (var ppg in ppgs) {
        //            Console.WriteLine(culture + ", PPG " + ppg.name);
        //            var mPpgl = (from ppglDb in m_dbContext.Parent_Purchase_Group_Local
        //                        where ppglDb.parent_purchase_group_id == ppg.id &&
        //                        ppglDb.culture == DefaultLanguage
        //                        select ppglDb).FirstOrDefault();

        //            if (mPpgl == null) {
        //                Parent_Purchase_Group_Local ppgl = new Parent_Purchase_Group_Local();
        //                ppgl.parent_purchase_group_id = ppg.id;
        //                ppgl.culture = DefaultLanguage;
        //                ppgl.local_text = ppg.name;
        //                m_dbContext.Parent_Purchase_Group_Local.Add(ppgl);
        //            } else {
        //                mPpgl.local_text = ppg.name;
        //            }
        //        }

        //        SaveChanges();
        //    }
        //}

        public int GetMissingLocalTextCount(string culture, out List<Parent_Purchase_Group> ppgs) {
            string sql = "SELECT ppgd.*" +
                " FROM " + ParentPurchaseGroupData.TABLE_NAME + " ppgd" +
                " LEFT OUTER JOIN " + ParentPurchaseGroupLocalData.TABLE_NAME + " ppgld" +
                " ON ppgd." + ParentPurchaseGroupData.ID_FIELD + "=ppgld." + ParentPurchaseGroupLocalData.PARENT_PURCHASE_GROUP_ID_FIELD +
                " AND ppgld." + ParentPurchaseGroupLocalData.CULTURE_FIELD + " = '" + culture + "'" +
                " WHERE ppgld." + ParentPurchaseGroupLocalData.LOCAL_TEXT_FIELD + " IS NULL";

            ppgs = m_dbContext.Database.SqlQuery<Parent_Purchase_Group>(sql).ToList();

            if (ppgs == null) {
                return 0;
            }

            return ppgs.Count;

            ////Default lang
            //sql = "SELECT ppgd.*" +
            //    " FROM " + ParentPurchaseGroupData.TABLE_NAME + " ppgd" +
            //    " LEFT OUTER JOIN " + ParentPurchaseGroupLocalData.TABLE_NAME + " ppgld" +
            //    " ON ppgd." + ParentPurchaseGroupData.ID_FIELD + "=ppgld." + ParentPurchaseGroupLocalData.PARENT_PURCHASE_GROUP_ID_FIELD +
            //    " AND ppgld." + ParentPurchaseGroupLocalData.CULTURE_FIELD + " = '" + DefaultLanguage + "'" +
            //    " WHERE ppgld." + ParentPurchaseGroupLocalData.LOCAL_TEXT_FIELD + " IS NULL";

            //ppgs = m_dbContext.Database.SqlQuery<Parent_Purchase_Group>(sql).ToList();

            //if (ppgs != null && ppgs.Count > 0) {

            //    foreach (var ppg in ppgs) {
            //        Console.WriteLine(culture + ", PPG " + ppg.name);
            //        var mPpgl = (from ppglDb in m_dbContext.Parent_Purchase_Group_Local
            //                     where ppglDb.parent_purchase_group_id == ppg.id &&
            //                     ppglDb.culture == DefaultLanguage
            //                     select ppglDb).FirstOrDefault();

            //        if (mPpgl == null) {
            //            Parent_Purchase_Group_Local ppgl = new Parent_Purchase_Group_Local();
            //            ppgl.parent_purchase_group_id = ppg.id;
            //            ppgl.culture = DefaultLanguage;
            //            ppgl.local_text = ppg.name;
            //            m_dbContext.Parent_Purchase_Group_Local.Add(ppgl);
            //        } else {
            //            mPpgl.local_text = ppg.name;
            //        }
            //    }

            //    SaveChanges();
            //}
        }

        public void SetMissingLocalText(Parent_Purchase_Group ppg, string culture) {
            var mPpgl = (from ppglDb in m_dbContext.Parent_Purchase_Group_Local
                         where ppglDb.parent_purchase_group_id == ppg.id &&
                         ppglDb.culture == culture
                         select ppglDb).FirstOrDefault();

            if (mPpgl == null) {
                Parent_Purchase_Group_Local ppgl = new Parent_Purchase_Group_Local();
                ppgl.parent_purchase_group_id = ppg.id;
                ppgl.culture = culture;
                ppgl.local_text = ppg.name;
                m_dbContext.Parent_Purchase_Group_Local.Add(ppgl);
            } else {
                mPpgl.local_text = ppg.name;
            }

            m_dbContext.SaveChanges();
        }

        public void DeleteParentPgByName(string strName) {
            List<Parent_Purchase_Group> parentPgs = (from parentPgDb in m_dbContext.Parent_Purchase_Group
                                       where parentPgDb.name == strName
                                       select parentPgDb).ToList();
            foreach (var parentPg in parentPgs) {
                DeleteParentPg(parentPg.id);
            }
        }

        public void DeleteParentPg(int ppgId) {

            using (TransactionScope transaction = new TransactionScope()) {
                try {

                    var ppg = (from ppgDb in m_dbContext.Parent_Purchase_Group
                                   where ppgDb.id == ppgId
                               select ppgDb).FirstOrDefault();

                    if (ppg.Company != null) {
                        for (int i = ppg.Company.Count - 1; i >= 0; i--) {
                            ppg.Company.Remove(ppg.Company.ElementAt(i));
                        }
                    }

                    if (ppg.Parent_Purchase_Group_Local != null) {
                        for (int i = ppg.Parent_Purchase_Group_Local.Count - 1; i >= 0; i--) {
                            ppg.Parent_Purchase_Group_Local.Remove(ppg.Parent_Purchase_Group_Local.ElementAt(i));
                        }
                    }

                    m_dbContext.Parent_Purchase_Group.Remove(ppg);

                    SaveChanges();
                    transaction.Complete();

                } catch (Exception ex) {
                    throw ex;
                }
            }
        }

        public void DeletePpgLocData(string culture) {
            List<Parent_Purchase_Group_Local> ppgls = (from ppglDb in m_dbContext.Parent_Purchase_Group_Local
                                                      where ppglDb.culture == culture
                                                      select ppglDb).ToList();

            if (ppgls == null) {
                return;
            }

            foreach (var ppgl in ppgls) {
                m_dbContext.Parent_Purchase_Group_Local.Remove(ppgl);
            }

            m_dbContext.SaveChanges();
        }

        public List<Parent_Purchase_Group_Local> GetPpgLocData(string culture) {
            List<Parent_Purchase_Group_Local> ppgls = (from ppglDb in m_dbContext.Parent_Purchase_Group_Local
                                                       where ppglDb.culture == culture
                                                       select ppglDb).ToList();

            return ppgls;
        }

        public List<Parent_Purchase_Group> GetPpgData() {
            List<Parent_Purchase_Group> ppgs = (from ppgDb in m_dbContext.Parent_Purchase_Group
                                                       select ppgDb).ToList();

            return ppgs;
        }

        //public Parent_Purchase_Group GetParentPgById(int ppgId) {
        //    var ppg = (from ppgDb in m_dbContext.Parent_Purchase_Group
        //              where ppgDb.id == ppgId
        //              select ppgDb).FirstOrDefault();

        //    return ppg;
        //}
        #endregion
    }
}
