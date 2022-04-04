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
using static Kamsyk.Reget.Model.Repositories.RequestRepository;

namespace Kamsyk.Reget.Model.Repositories {
    public class AppTextStoreRepository : BaseRepository<App_Text_Store>, IRequestTextRepository {
        #region Constant
        private const int TEXT_TYPE_REQUEST_TEXT = 10;
        private const int TEXT_TYPE_REQUEST_REMARK = 11;
        private const int TEXT_TYPE_REQUEST_DISC = 20;
        private const int TEXT_TYPE_REQUEST_DISC_DELETED = 21;
        private const int TEXT_TYPE_REQUEST_NR = 30;
        private const int TEXT_TYPE_REQUEST_SUPPLIER = 40;
        private const int TEXT_TYPE_SUBT_DISC = 100;
        private const int TEXT_TYPE_SUBT_DISC_DELETED = 101;
        private const int TEXT_TYPE_SUBT_REMARK = 110;
        #endregion

        #region Enums
        public enum TextType {
            RequestText = TEXT_TYPE_REQUEST_TEXT,
            RequestRemark = TEXT_TYPE_REQUEST_REMARK,
            RequestDisc = TEXT_TYPE_REQUEST_DISC,
            RequestNr = TEXT_TYPE_REQUEST_NR,
            RequestSupplier = TEXT_TYPE_REQUEST_SUPPLIER,
            SubstDisc = TEXT_TYPE_SUBT_DISC,
            SubstRemark = TEXT_TYPE_SUBT_REMARK
        }
        #endregion

        #region Methods
        public int GetLastId() {
            var lastTextId = (from textDb in m_dbContext.App_Text_Store
                              orderby textDb.id descending
                              select textDb).Take(1).FirstOrDefault();

            int lastId = -1;
            if (lastTextId != null) {
                lastId = lastTextId.id;
            }

            return lastId;
        }

        public List<App_Text_Store> SearchText(
            string searchText, 
            List<int> companyIds, 
            int pageSize,
            int pageNr,
            bool isMyRequestsOnly,
            int currentUserId,
            out int rowsCount) {

            string strFilterWhere = GetStoreFilter(searchText, isMyRequestsOnly, currentUserId);
            var strOrder = GetOrder();
            string sqlPureBody = GetStorePureBody(companyIds, strFilterWhere, isMyRequestsOnly);
            string sqlPure = "";
            if (isMyRequestsOnly) {
                sqlPure = "SELECT apptsd.*, ROW_NUMBER() OVER(" + strOrder + ") AS RowNum";
            } else {
                sqlPure = "SELECT apptsd.*, ROW_NUMBER() OVER(" + strOrder + ") AS RowNum";
            }

            string selectCount = "SELECT COUNT(*) " + sqlPureBody;
            rowsCount = m_dbContext.Database.SqlQuery<int>(selectCount).Single();

            string sqlPart = sqlPure + sqlPureBody;
            int partStart = pageSize * (pageNr - 1) + 1;
            int partStop = partStart + pageSize - 1;

            while (partStart > rowsCount) {
                partStart -= pageSize;
                partStart = partStart + pageSize - 1;
            }
            
            string sql = "SELECT * FROM(" + sqlPart + ") AS SearchPartData" +
                " WHERE SearchPartData.RowNum BETWEEN " + partStart + " AND " + partStop;

            var appStoreTexts = m_dbContext.Database.SqlQuery<App_Text_Store>(sql).ToList();
                       
            return appStoreTexts;

            
        }

        private string GetOrder() {
            string strOrder = "ORDER BY apptsd." + AppTextStoreData.ID_FIELD;

            return strOrder;
        }

        private string GetStoreFilter(string searchText, bool isMyRequestsOnly, int currentUserId) {
            string strFilterWhere = "";
                       
            if (isMyRequestsOnly) {
                strFilterWhere += " AND " + RequestEventData.REQUESTOR_FIELD + "=" + currentUserId;
            }

            if (searchText.Contains(" ")) {
                string[] searchParts = searchText.Split(' ');
                int iIndex = 0;
                while (iIndex < 10 && iIndex < searchParts.Length) {
                    strFilterWhere += " AND CONTAINS(" + AppTextStoreData.TEXT_CONTENT_FIELD + "," + "'\"" + searchParts[iIndex] + "*\"')";

                    iIndex++;
                }
            } else {
                strFilterWhere += " AND CONTAINS(" + AppTextStoreData.TEXT_CONTENT_FIELD + "," + "'\"" + searchText + "*\"')";
            }

            return strFilterWhere;
        }

        private string GetStorePureBody(List<int> companyIds, string strFilterWhere, bool isMyRequestsOnly) {
            string sqlPureBody = " FROM " + AppTextStoreData.TABLE_NAME + " apptsd "
                    + " INNER JOIN " + RequestTextData.TABLE_NAME + " rtd "
                    + " ON apptsd." + AppTextStoreData.ID_FIELD + "=rtd." + RequestTextData.APP_TEXT_STORE_ID_FIELD
                    + " INNER JOIN " + RequestEventData.TABLE_NAME + " red "
                    + " ON rtd." + RequestTextData.REQUEST_ID_FIELD + "=red." + RequestEventData.ID_FIELD
                    + " AND rtd." + RequestTextData.REQUEST_VERSION_FIELD + "=red." + RequestEventData.VERSION_FIELD
                    + " WHERE apptsd." + CentreData.COMPANY_ID_FIELD + " IN" + GetSqlIn(companyIds)
                    + " AND rtd." + RequestTextData.IS_LAST_VERSION_FIELD + "=1";

            //string sqlPureBody = null;
            //if (isMyRequestsOnly) {
            //    sqlPureBody = " FROM " + AppTextStoreData.TABLE_NAME + " apptsd "
            //        + " INNER JOIN " + RequestTextData.TABLE_NAME + " rtd "
            //        + " ON apptsd." + AppTextStoreData.ID_FIELD + "=rtd." + RequestTextData.APP_TEXT_STORE_ID_FIELD
            //        + " INNER JOIN " + RequestEventData.TABLE_NAME + " red "
            //        + " ON rtd." + RequestTextData.REQUEST_ID_FIELD + "=red." + RequestEventData.ID_FIELD
            //        + " WHERE apptsd." + CentreData.COMPANY_ID_FIELD + " IN" + GetSqlIn(companyIds)
            //        + " AND rtd." + RequestTextData.IS_LAST_VERSION_FIELD + "=1";
            //} else {
            //    //sqlPureBody = " FROM " + AppTextStoreData.TABLE_NAME + " apptsd "
            //    //    + " WHERE " + AppTextStoreData.COMPANY_ID_FIELD + " IN" + GetSqlIn(companyIds);
            //    //    //+ " AND rtd." + RequestTextData.IS_LAST_VERSION_FIELD + "=1";

            //}
            sqlPureBody += strFilterWhere;

            return sqlPureBody;
        }

        public App_Text_Store GetAppTextStoreByIdVersionType(int requestId, int version, TextType textType) {
            var appTextStore = (from appTextDb in m_dbContext.App_Text_Store
                                join requestTextDb in m_dbContext.Request_Text
                                on appTextDb.id equals requestTextDb.app_text_store_id
                                where appTextDb.text_type == (int)textType
                                && requestTextDb.request_id == requestId
                                && requestTextDb.request_version == version
                                select appTextDb).FirstOrDefault();

            return appTextStore;
        }

        public App_Text_Store GetActiveAppTextStoreByIdType(int requestId, TextType textType) {
            var appTextStore = (from appTextDb in m_dbContext.App_Text_Store
                                join requestTextDb in m_dbContext.Request_Text
                                on appTextDb.id equals requestTextDb.app_text_store_id
                                where appTextDb.text_type == (int)textType
                                && requestTextDb.request_id == requestId
                                && requestTextDb.is_last_version == true
                                select appTextDb).FirstOrDefault();

            return appTextStore;
        }

        public void AddRequestAppTest(
            int requestId,
            int requestVersion,
            string requestText,
            TextType textType,
            int companyId,
            DateTime modifDate) {

            int lastId = GetLastId();
            int newId = lastId + 1;
            App_Text_Store newAppTextStore = new App_Text_Store();
            newAppTextStore.id = newId;
            newAppTextStore.text_content = requestText;
            newAppTextStore.text_type = (int)textType;
            newAppTextStore.company_id = companyId;

            List<Request_Text> newRequestTexts = new List<Request_Text>();
            Request_Text newRequestText = new Request_Text();
            newRequestText.app_text_store_id = newId;
            newRequestText.request_id = requestId;
            newRequestText.request_version = requestVersion;
            newRequestText.is_last_version = true;
            newRequestText.modify_date = modifDate;
            newRequestTexts.Add(newRequestText);

            newAppTextStore.Request_Text = newRequestTexts;
            m_dbContext.App_Text_Store.Add(newAppTextStore);
            SaveChanges();
        }
        #endregion
    }
}
