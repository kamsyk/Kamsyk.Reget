using OTISCZ.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WcfReget.Authorization {
    public class BaseWcf {
        #region Constants
        private const string USER_NAME = "RegetWcf";
        private const string PASSWORD = "d4gh>e%j6,87'";
        private const string ENCRYPT_PASSWORD = "fg.g.;r5er8ee.-";
        #endregion

        #region Properties
        public AuthHeader Authentication;
        #endregion

        protected void Authenticate(AuthHeader authentication) {
            if (!(authentication != null &&
                Authentication.Username == USER_NAME &&
                Des.Decrypt(Authentication.Password, ENCRYPT_PASSWORD) == PASSWORD)) {

                throw new Exception("Non authorized request");
            }
        }
    }
}