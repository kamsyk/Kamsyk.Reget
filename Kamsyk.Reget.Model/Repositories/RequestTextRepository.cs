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
using static Kamsyk.Reget.Model.Repositories.AppTextStoreRepository;

namespace Kamsyk.Reget.Model.Repositories {
    public class RequestTextRepository : BaseRepository<Request_Text>, IRequestTextRepository {
        //#region Constant
        //public const int TEXT_TYPE_REQUEST = 10;
        //#endregion

        //#region Enums
        //public enum TextType {
        //    RequestText = TEXT_TYPE_REQUEST
        //}
        //#endregion

        #region Methods
        //private void AddRequestText(int requestId, int requestVersion) {

        //}

        public Request_Text GetLastVersion(int requestId, AppTextStoreRepository.TextType textType, InternalRequestEntities dbContext) {
            var requestTexts = (from requestTextDb in dbContext.Request_Text
                                where requestTextDb.request_id == requestId
                                //&& requestTextDb.text_type == (int)textType
                                && requestTextDb.is_last_version == true
                                select requestTextDb).ToList();

            if (requestTexts == null) {
                return null;
            }

            foreach (var requestText in requestTexts) {
                if (requestText.App_Text_Store.text_type == (int)textType) {
                    return requestText;
                }
            }

            return null;
        }

        //public List<Request_Text> GetRequestDiscussion(int requestId) {
        //    var requestTexts = (from requestTextDb in m_dbContext.Request_Text
        //                        where requestTextDb.request_id == requestId
        //                        && (requestTextDb.app_text_store_id == (int)AppTextStoreRepository.TextType.RequestRemark
        //                        || requestTextDb.app_text_store_id == (int)AppTextStoreRepository.TextType.RequestDisc)
        //                        && requestTextDb.is_last_version == true
        //                        select requestTextDb).ToList();

        //    return requestTexts;
        //}

        public Request_Text GetRequestTextByAppTextId(int appTextId) {
            var requestTexts = (from requestTextDb in m_dbContext.Request_Text
                                where requestTextDb.app_text_store_id == appTextId
                                select requestTextDb).FirstOrDefault();

            return requestTexts;
        }

        public Discussion GetDiscussionTextByAppTextId(int appTextId) {
            var discTexts = (from discDb in m_dbContext.Discussion
                                where discDb.app_text_store_id == appTextId
                                select discDb).FirstOrDefault();

            return discTexts;
        }

        public void DeactivateRequestRemark(int requestId, InternalRequestEntities m_dbContext) {
            var requestTexts = (from requestTextDb in m_dbContext.Request_Text
                               where requestTextDb.request_id == requestId
                               && requestTextDb.is_last_version == true
                               select requestTextDb).ToList();
            if (requestTexts != null && requestTexts.Count > 0) {
                foreach (var requestText in requestTexts) {
                    if (requestText.App_Text_Store.text_type == (int)TextType.RequestRemark) {
                        requestText.is_last_version = false;
                    }
                }
            }
        }
        #endregion
    }
}
