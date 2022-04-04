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

namespace Kamsyk.Reget.Model.Repositories {
    public class FilterValueRepository : BaseRepository<Filter_Value> {
        #region Constant
        private const int FILTER_VALUE_TYPE_REQUESTOR = 10;
        private const int FILTER_VALUE_TYPE_APP_MAN = 20;
        private const int FILTER_VALUE_TYPE_ORDERER = 30;
        private const int FILTER_VALUE_TYPE_SUPPLIER = 40;
        private const int FILTER_VALUE_TYPE_AREA = 50;
        private const int FILTER_VALUE_TYPE_PARENT_PURCHASE_GROUP = 60;
        #endregion

        #region Enums
        public enum FilterType {
            Requestor = FILTER_VALUE_TYPE_REQUESTOR,
            AppMan = FILTER_VALUE_TYPE_APP_MAN,
            Orderer = FILTER_VALUE_TYPE_ORDERER,
            Supplier = FILTER_VALUE_TYPE_SUPPLIER,
            Area = FILTER_VALUE_TYPE_AREA,
            ParentPurchaseGroup = FILTER_VALUE_TYPE_PARENT_PURCHASE_GROUP
           
        }
        #endregion

        #region Methods
        

        public List<EventGridData> GetEventData(
            int companyId,
            string filter,
            string sort,
            int pageSize,
            int pageNr,
            int currentUserId,
            out int rowsCount) {

            List<int> companies = GetEventAllowedCompanies(currentUserId);
            if (companyId != -1 && !companies.Contains(companyId)) {
                throw new Exception("Events of Company " + companyId + " are not allowed for user " + currentUserId);
            }

            string strFilterWhere = "";

            if (!String.IsNullOrEmpty(filter)) {
                //string[] filterItems = filter.Split(UrlParamDelimiter.ToCharArray());
                //foreach (string filterItem in filterItems) {
                //    string[] strItemProp = filterItem.Split(UrlParamValueDelimiter.ToCharArray());
                //    strFilterWhere += " AND ";

                //    string columnName = strItemProp[0].Trim().ToUpper();
                //    if (columnName == SupplierData.SUPP_NAME_FIELD.Trim().ToUpper()) {
                //        strFilterWhere += "sd." + SupplierData.SUPP_NAME_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'";
                //    }
                //    if (columnName == SupplierData.SUPPLIER_ID_FIELD.Trim().ToUpper()) {
                //        strFilterWhere += "sd." + SupplierData.SUPPLIER_ID_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'";
                //    }
                //    if (columnName == "street".ToUpper()) {
                //        strFilterWhere += "(sd." + SupplierData.STREET_PART1_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'"
                //             + " OR sd." + SupplierData.STREET_PART2_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%')";
                //    }
                //    if (columnName == SupplierData.CITY_FIELD.Trim().ToUpper()) {
                //        strFilterWhere += "sd." + SupplierData.CITY_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'";
                //    }
                //    if (columnName == SupplierData.ZIP_FIELD.Trim().ToUpper()) {
                //        strFilterWhere += "sd." + SupplierData.ZIP_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'";
                //    }
                //    if (columnName == SupplierData.COUNTRY_FIELD.Trim().ToUpper()) {
                //        strFilterWhere += "sd." + SupplierData.COUNTRY_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'";
                //    }
                //    if (columnName == SupplierData.CONTACT_PERSON_FIELD.Trim().ToUpper()) {
                //        strFilterWhere += "sd." + SupplierData.CONTACT_PERSON_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'";
                //    }
                //    if (columnName == SupplierData.PHONE_FIELD.Trim().ToUpper()) {
                //        strFilterWhere += "sd." + SupplierData.PHONE_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'";
                //    }
                //    if (columnName == SupplierData.EMAIL_FIELD.Trim().ToUpper()) {
                //        strFilterWhere += "sd." + SupplierData.EMAIL_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'";
                //    }
                //    if (columnName == SupplierData.ACTIVE_FIELD.Trim().ToUpper()) {
                //        if (strItemProp[1].Trim().ToLower() == "true") {
                //            strFilterWhere += "sd." + SupplierData.ACTIVE_FIELD + "=1";
                //        } else {
                //            strFilterWhere += "sd." + SupplierData.ACTIVE_FIELD + "=0";
                //        }
                //    }
                //}
            }

            string strOrder = "ORDER BY sd." + MaintenanceEventData.ID_FIELD;
            
            if (!String.IsNullOrEmpty(sort)) {
                //strOrder = "";
                //string[] sortItems = sort.Split(UrlParamDelimiter.ToCharArray());
                //foreach (string sortItem in sortItems) {
                //    string[] strItemProp = sortItem.Split(UrlParamValueDelimiter.ToCharArray());
                //    if (strOrder.Length > 0) {
                //        strOrder += ", ";
                //    }

                //    if (strItemProp[0] == "company_name_text") {
                //        //strOrder = "cd." + CompanyData.COUNTRY_CODE_FIELD + " " + strItemProp[1]; ;
                //    } else {
                //        strOrder += "sd." + strItemProp[0] + " " + strItemProp[1];
                //    }
                //}

                //strOrder = " ORDER BY " + strOrder;
            }

            string sqlPure = "SELECT sd.*, ROW_NUMBER() OVER(" + strOrder + ") AS RowNum";

            string sqlPureBody = " FROM " + SupplierData.TABLE_NAME + " sd" +
                 " WHERE sd." + SupplierData.SUPPLIER_GROUP_ID_FIELD + "=" + companyId;
            sqlPureBody += strFilterWhere;

            //Get Row count
            string selectCount = "SELECT COUNT(" + MaintenanceEventData.ID_FIELD + ") " + sqlPureBody;
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

            var tmpEvents = m_dbContext.Database.SqlQuery<Supplier>(sql).ToList();

            
            List<EventGridData> events = new List<EventGridData>();
            int rowIndex = (pageNr - 1) * pageSize + 1;
            foreach (var tmpSupplier in tmpEvents) {
                EventGridData maintEvent = new EventGridData();
                SetValues(tmpEvents, maintEvent, new List<string> {
                    MaintenanceEventData.ID_FIELD,
                    MaintenanceEventData.MODIF_DATE_FIELD
                });

                maintEvent.row_index = rowIndex++;
                //supplier.street = "";
                //if (!String.IsNullOrEmpty(tmpSupplier.street_part1)) {
                //    supplier.street += tmpSupplier.street_part1;
                //}
                //if (!String.IsNullOrEmpty(tmpSupplier.street_part2)) {
                //    if (supplier.street.Length > 0) {
                //        supplier.street += ", ";
                //    }
                //    supplier.street += tmpSupplier.street_part2;
                //}

                events.Add(maintEvent);

            }

            return events;
            
        }

        public List<int> GetEventAllowedCompanies(int userId) {
            List<int> allowedCompanies = new List<int>();

            var rolesCgAdmin = (from dbRole in m_dbContext.ParticipantRole_CentreGroup
                               where dbRole.participant_id == userId && 
                               (dbRole.role_id == (int)UserRepository.UserRole.CentreGroupPropAdmin ||
                               dbRole.role_id == (int)UserRepository.UserRole.ApproveMatrixAdmin ||
                               dbRole.role_id == (int)UserRepository.UserRole.RequestorAdmin ||
                                dbRole.role_id == (int)UserRepository.UserRole.OrdererAdmin)
                               select dbRole).ToList();
            if (rolesCgAdmin != null) {
                foreach (var roleCgAdmin in rolesCgAdmin) {
                    int compId = roleCgAdmin.Centre_Group.company_id;
                    if (!allowedCompanies.Contains(compId)) {
                        allowedCompanies.Add(compId);
                    }
                }
            }

            var rolesCompAdmin = (from dbRole in m_dbContext.Participant_Office_Role
                                where dbRole.participant_id == userId && 
                                dbRole.role_id == (int)UserRepository.UserRole.OfficeAdministrator
                                select dbRole).ToList();
            if (rolesCompAdmin != null) {
                foreach (var roleCompAdmin in rolesCompAdmin) {
                    int compId = roleCompAdmin.office_id;
                    if (!allowedCompanies.Contains(compId)) {
                        allowedCompanies.Add(compId);
                    }
                }
            }

            return allowedCompanies;
        }

        public List<Filter_Value> GetFilterValuesByUserId(int userId) {
            var filterValues = (from filterDb in m_dbContext.Filter_Value
                                where filterDb.item_id == userId &&
                                (filterDb.filed_type == FILTER_VALUE_TYPE_APP_MAN ||
                                filterDb.filed_type == FILTER_VALUE_TYPE_ORDERER ||
                                filterDb.filed_type == FILTER_VALUE_TYPE_REQUESTOR)
                                select filterDb).ToList();

            return filterValues;
        }
        #endregion
    }
}
