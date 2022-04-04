using Kamsyk.Reget.Model.Common;
using Kamsyk.Reget.Model.ExtendedModel;
using Kamsyk.Reget.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.Repositories {
    public class RequestMailRepository : BaseRepository<Request_Mail> {
        #region Methods
        public RequestMailLight GetLastMail(int requestId) {
            RequestMailLight requestMail = (from reqMailDb in m_dbContext.Request_Mail
                                        where reqMailDb.request_id == requestId
                                        orderby reqMailDb.sent_date descending
                                        select new RequestMailLight(){
                                            request_id = reqMailDb.request_id,
                                            recipients = reqMailDb.recipients,
                                            sender = reqMailDb.sender
                                        }).Take(1).FirstOrDefault();

            return requestMail;
        }

       
        #endregion
    }
}
