using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services.Protocols;

namespace WcfReget.Authorization {
    public class AuthHeader : SoapHeader {
        public string Username;
        public string Password;

        public AuthHeader() { }
    }
}