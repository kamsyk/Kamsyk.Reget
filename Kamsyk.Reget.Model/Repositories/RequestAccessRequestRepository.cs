using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.Repositories {
    public class RequestAccessRequestRepository : BaseRepository<Request_AccessRequest> {
        public Request_AccessRequest GetAccessRequestByReqIdRequestorIdsJs(int requestId, int requestorId) {
            var accessRequest = (from accessRequestDb in m_dbContext.Request_AccessRequest
                                where accessRequestDb.request_id == requestId && accessRequestDb.requestor_id == requestorId
                                select accessRequestDb).FirstOrDefault();

            if(accessRequest == null) {
                return null;
            }

            Request_AccessRequest retAccessRequest = new Request_AccessRequest();
            SetValues(accessRequest, retAccessRequest);

            return retAccessRequest;
        }

        public void AddAccessRequest(int requestId, int requestVersion, int requestorId) {
            Request_AccessRequest accessRequest = new Request_AccessRequest();
            accessRequest.request_id = requestId;
            accessRequest.request_version = requestVersion;
            accessRequest.requestor_id = requestorId;
            accessRequest.status_id = (int)RequestRepository.ApproveStatus.WaitForApproval;
            accessRequest.request_date = DateTime.Now;

            m_dbContext.Request_AccessRequest.Add(accessRequest);

            m_dbContext.SaveChanges();
        }
    }
}
