using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Kamsyk.Reget.Setting {
    public static class RegetSettings {
        public static bool IsTest {
            get {
                string strIsTest = ConfigurationManager.AppSettings["IsTest"];
                return (strIsTest.ToLower().Trim() == "true");
            }
        }

        public static string MailSender {
            get {
                string strValue = ConfigurationManager.AppSettings["MailSender"];
                return strValue;
            }
        }

        public static string MailBcc {
            get {
                string strValue = ConfigurationManager.AppSettings["MailBcc"];
                return strValue;
            }
        }

        public static string AppName {
            get {
                string strValue = ConfigurationManager.AppSettings["AppName"];
                return strValue;
            }
        }
    }
}