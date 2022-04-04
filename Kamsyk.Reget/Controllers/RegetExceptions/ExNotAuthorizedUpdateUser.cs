using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kamsyk.Reget.Controllers.RegetExceptions {
    public class ExNotAuthorizedUpdateUser : Exception{
        public ExNotAuthorizedUpdateUser(string strMsg) : base(strMsg) {
        }
    }
}