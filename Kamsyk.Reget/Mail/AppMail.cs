using Kamsyk.Reget.Model;
using Kamsyk.Reget.Setting;
using Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;

namespace Kamsyk.Reget.Mail {
    public class AppMail {
        #region Static Methods
        public static void SendMail(
            string strRecipients,
            string strCc,
            string strBcc,
            string strSubject,
            string strBody) {

            string bcc = strBcc;

            if (!String.IsNullOrWhiteSpace(RegetSettings.MailBcc)) {
                if (bcc == null) {
                    bcc = "";
                }

                if (!String.IsNullOrWhiteSpace(bcc)) {
                    bcc += ";";
                }
                bcc += RegetSettings.MailBcc;
            }

            string subject = strSubject;
            if (!String.IsNullOrWhiteSpace(RegetSettings.AppName)) {
                subject = RegetSettings.AppName + " - " + subject;
            }

            if (strBody == null) {
                strBody = "";
            }

            strBody += "<p>" + RequestResource.DoNotReplyMail + "</p>";

#if !RELEASE
            strRecipients = "kamil.sykora@otis.com";
            strCc = null;
            bcc = null;
#endif

            WsMail.OtWsMail wsMail = new WsMail.OtWsMail();

#if RELEASE
            wsMail.SendMailBcc(
                RegetSettings.MailSender,
                strRecipients,
                strCc,
                bcc,
                subject,
                strBody,
                null,
                (int)MailPriority.Normal);
#endif
        }

        public static Hashtable GetRecipientsLangs(List<Participants> parts) {
            if (parts == null) {
                return null;
            }

            Hashtable htMailLang = new Hashtable();
            foreach (var part in parts) {
                string strLang = part.Company.culture_info;
                if (part.User_Setting != null) {
                    if (!String.IsNullOrEmpty(part.User_Setting.default_lang)) {
                        strLang = part.User_Setting.default_lang;
                    }
                }

                if (htMailLang.ContainsKey(strLang)) {
                    string mailRec = htMailLang[strLang].ToString();
                    mailRec += ";" + part.email;
                } else {
                    htMailLang.Add(strLang, part.email);
                }
                
            }

            return htMailLang;
        }

        public static bool IsValidEmail(string email) {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match) {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            } catch (RegexMatchTimeoutException e) {
                return false;
            } catch (ArgumentException e) {
                return false;
            }

            try {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            } catch (RegexMatchTimeoutException) {
                return false;
            }
        }
        #endregion
    }
}