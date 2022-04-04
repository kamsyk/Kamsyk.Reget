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
using Kamsyk.Reget.Model.ExtendedModel.Discussion;

namespace Kamsyk.Reget.Model.Repositories {
    public class DiscussionRepository : BaseRepository<Discussion> {
        #region Methods
        //public DiscussionExtended GetSubstitutionDiscussion(int substId) {
        //    var subst = (from substDb in m_dbContext.Participant_Substitute
        //                       where substDb.id == substId
        //                       select substDb).FirstOrDefault();

        //    if (subst.Discussion == null || subst.Discussion.Count == 0) {
        //        return null;
        //    }

        //    DiscussionExtended discussionExtended = new DiscussionExtended();


        //    return discussionExtended;
        //}

        public void AddSubstitutionDiscussion(int substId, string remark, int userId) {
            var subst = (from substDb in m_dbContext.Participant_Substitute
                         where substDb.id == substId
                         select substDb).FirstOrDefault();

            AddSubstitutionDiscussion(subst, remark, AppTextStoreRepository.TextType.SubstDisc, userId, m_dbContext);
        }

        public void AddRequestDiscussion(int requestId, string remark, int userId) {
            var reqEvent = (from reqDb in m_dbContext.Request_Event
                         where reqDb.id == requestId && reqDb.last_event == true
                            select reqDb).FirstOrDefault();

            AddRequestDiscussion(reqEvent, remark, AppTextStoreRepository.TextType.RequestDisc, userId, m_dbContext);
        }
                
        public int AddSubstitutionDiscussion(
            Participant_Substitute subst, 
            string remark, 
            AppTextStoreRepository.TextType substTextType, 
            int userId,
            //int? lastAppTextId,
            InternalRequestEntities dbContext) {

            using (TransactionScope transaction = new TransactionScope()) {
                try {

                    //App_Text_Store appText = new App_Text_Store();
                    //int newTextId = new AppTextStoreRepository().GetLastId();
                    //newTextId++;
                    //appText.id = newTextId;
                    //appText.text_content = remark;
                    //appText.text_type = (int)substTextType;
                    //m_dbContext.App_Text_Store.Add(appText);

                    //int lastId = GetLastId();
                    //int newId = ++lastId;

                    //Discussion disc = new Discussion();
                    //disc.id = newId;
                    //disc.text_id = newTextId;
                    //disc.author_id = userId;
                    //disc.modif_date = DateTime.Now;

                    int appTextId = -1;
                    var disc = SaveDiscussion(
                        remark,
                        substTextType,
                        userId,
                        dbContext,
                        null,
                        out appTextId);

                    dbContext.Discussion.Add(disc);

                    Substitute_Discussion substituteDiscussion = new Substitute_Discussion();
                    substituteDiscussion.substitute_id = subst.id;
                    substituteDiscussion.discussion_id = disc.id;
                    substituteDiscussion.is_active = true;

                    subst.Substitute_Discussion.Add(substituteDiscussion);
                                        
                    SaveChanges();
                    transaction.Complete();

                    return appTextId;
                } catch (Exception ex) {
                    throw ex;
                }
            }
        }

        public int AddRequestDiscussion(
            Request_Event requestEvent,
            string remark,
            AppTextStoreRepository.TextType substTextType,
            int userId,
            InternalRequestEntities dbContext) {

            return AddRequestDiscussion(
                requestEvent,
                remark,
                substTextType,
                userId,
                dbContext,
                null);
        }

        public int AddRequestDiscussion(
            Request_Event requestEvent,
            string remark,
            AppTextStoreRepository.TextType substTextType,
            int userId,
            InternalRequestEntities dbContext,
            int? lastAppTextId) {

            using (TransactionScope transaction = new TransactionScope()) {
                try {
                    int appTextId = -1;
                    var disc = SaveDiscussion(
                        remark,
                        substTextType,
                        userId,
                        dbContext,
                        lastAppTextId,
                        out appTextId);

                    Request_Discussion requestDiscussion = new Request_Discussion();
                    requestDiscussion.request_id = requestEvent.id;
                    requestDiscussion.request_version = requestEvent.version;
                    requestDiscussion.discussion_id = disc.id;
                    requestDiscussion.is_active = true;

                    requestEvent.Request_Discussion.Add(requestDiscussion);

                    SaveChanges();
                    transaction.Complete();

                    return appTextId;
                } catch (Exception ex) {
                    throw ex;
                }
            }
        }

        //private Discussion SaveDiscussion(
        //    string remark,
        //    AppTextStoreRepository.TextType substTextType,
        //    int userId,
        //    InternalRequestEntities dbContext,
        //    out int appTextId) {

        //    return SaveDiscussion(
        //        remark,
        //        substTextType,
        //        userId,
        //        dbContext,
        //        null,
        //        out appTextId);

        //}

        private Discussion SaveDiscussion(
            string remark,
            AppTextStoreRepository.TextType substTextType,
            int userId,
            InternalRequestEntities dbContext,
            int? lastAppTextId,
            out int appTextId) {

            App_Text_Store appText = new App_Text_Store();
            int newTextId = DataNulls.INT_NULL;
            if (lastAppTextId != null) {
                newTextId = (int)lastAppTextId + 1;
            } else { 
                newTextId = new AppTextStoreRepository().GetLastId();
                newTextId++;
            }
            appText.id = newTextId;
            appText.text_content = remark;
            appText.text_type = (int)substTextType;
            dbContext.App_Text_Store.Add(appText);

            int lastId = GetLastId();
            int newId = ++lastId;

            Discussion disc = new Discussion();
            disc.id = newId;
            disc.app_text_store_id = newTextId;
            disc.author_id = userId;
            disc.modif_date = DateTime.Now;

            dbContext.Discussion.Add(disc);

            appTextId = appText.id;

            return disc;
        }

        public int GetLastId() {
            var lastDisc = (from discDb in m_dbContext.Discussion
                          orderby discDb.id descending
                          select discDb).Take(1).FirstOrDefault();

            int lastId = -1;
            if (lastDisc != null) {
                lastId = lastDisc.id;
            }

            return lastId;
        }

        public Discussion GetLastDiscussion() {
            var disc = (from discDb in m_dbContext.Discussion
                        orderby discDb.id descending
                        select discDb).Take(1).FirstOrDefault();

            return disc;
        }

        public List<Discussion> GetDiscussionByRequestId(int requestId) {
            var reqDiscs = (from discDb in m_dbContext.Discussion
                            join reqDiscDb in m_dbContext.Request_Discussion
                            on discDb.id equals reqDiscDb.discussion_id
                            where reqDiscDb.request_id == requestId
                            && reqDiscDb.is_active == true
                        select discDb).ToList();

            return reqDiscs;
        }

        public List<Discussion> GetDiscussionBySubstitutionId(int substId) {
            var reqDiscs = (from discDb in m_dbContext.Discussion
                            join subDiscDb in m_dbContext.Substitute_Discussion
                            on discDb.id equals subDiscDb.discussion_id
                            where subDiscDb.substitute_id == substId
                            && subDiscDb.is_active == true
                            select discDb).ToList();

            return reqDiscs;
        }
        #endregion
    }
}
