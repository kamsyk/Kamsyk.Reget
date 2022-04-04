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
using static Kamsyk.Reget.Model.Repositories.RequestRepository;

namespace Kamsyk.Reget.Model.Repositories {
    public class RequestEventApprovalRepository : BaseRepository<Request_Event_Approval> {
        #region Methods
        public int GetLastId() {
            var rea = (from reaDb in m_dbContext.Request_Event_Approval
                           orderby reaDb.id descending
                           select new { id = reaDb.id }).Take(1).FirstOrDefault();
            int lastId = -1;
            if (rea != null) {
                lastId = rea.id;
            }

            return lastId;
        }

        public void AddRequestEventApproval(
            InternalRequestEntities dbContext,
            int requestId,
            int requestVersion,
            int approveLevelId,
            int appManId,
            int? substitutedUserId,
            ApproveStatus approveStatus) {

            AddRequestEventApproval(
                dbContext,
                requestId,
                requestVersion,
                approveLevelId,
                appManId,
                substitutedUserId,
                (int)approveStatus);
        }

        public void AddRequestEventApproval(
            InternalRequestEntities dbContext,
            int requestId,
            int requestVersion,
            int approveLevelId,
            int appManId,
            int? substitutedUserId,
            int iApproveStatus) {

            int lastId = GetLastId();
            int iNewId = ++lastId;

            Request_Event_Approval rea = new Request_Event_Approval();
            rea.id = iNewId;
            rea.request_event_id = requestId;
            rea.request_event_version = requestVersion;
            rea.app_level_id = approveLevelId;
            rea.app_man_id = appManId;
            rea.approve_status = iApproveStatus;
            rea.substituted_user_id = substitutedUserId;
            rea.is_last_version = true;

            dbContext.Request_Event_Approval.Add(rea);
        }
        #endregion
    }
}
