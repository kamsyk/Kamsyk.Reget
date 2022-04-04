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
    public class RequestAuthUserRepository : BaseRepository<Request_AuthorizedUsers>, IRequestAuthorizedUsers {
        #region Constant
        
        #endregion

        #region Enums
       
        #endregion

        #region Methods
        public List<Request_AuthorizedUsers> GetRequestAuthUsersByReqId(int reqId) {
            var authUsers = (from requAuthDb in m_dbContext.Request_AuthorizedUsers
                             where requAuthDb.request_id == reqId
                             select requAuthDb).ToList();

            return authUsers;
        }

        
        #endregion
    }
}
