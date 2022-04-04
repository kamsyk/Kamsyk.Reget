using OTISCZ.ActiveDirectory;
using Kamsyk.Reget.Model;
using Kamsyk.Reget.Model.Repositories;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kamsyk.Reget.Model.Repositories.UserRepository;
using System.Net;
using System.IO;
using System.Threading;
using System.Collections;
using static Kamsyk.Reget.ScheduledTasks.RegetWebRequest;
using System.Reflection;

namespace Kamsyk.Reget.ScheduledTasks {
    public class User {
        public void CheckNonActiveUsers() {
            Console.WriteLine("Checking non active users ...");

            UserRepository userRepository = new UserRepository();

            List<Participants> activeParticipants = userRepository.GetActiveParticipants();

            List<Company> companies = new CompanyRepository().GetActiveCompanies();
            int iPartCount = activeParticipants.Count;
            int iIndexd = 0;
            foreach (var participant in activeParticipants) {
                try {
                    Company comp = companies.Where(c => c.id == participant.company_id).FirstOrDefault();

                    string userName = participant.user_name;

                    OtActiveDirectory otActiveDirectory = new OtActiveDirectory(comp.active_directory_root);
                    SearchResult sr = otActiveDirectory.GetUserData(userName);

                    if (sr == null) {

                        if (participant.is_non_active == false) {
                            Console.WriteLine(iIndexd + "/" + iPartCount + " Deactivating " + userName + " ...");
                            userRepository.SetNonActiveUser(participant.id);
                        } else {
                            Console.WriteLine(iIndexd + "/" + iPartCount + " " + userName + " OK - No action is needed");
                        }
                    } else {
                        if (participant.is_non_active == true) {
                            Console.WriteLine(iIndexd + "/" + iPartCount + " Activate user " + userName + " ...");
                            userRepository.SetRevertNonActiveUser(participant.id);
                        } else {
                            Console.WriteLine(iIndexd + "/" + iPartCount + " " + userName + " OK - No action is needed");
                        }
                    }
                    iIndexd++;
                } catch (Exception ex) {
                    HandleError(ex);
                }
            }

            //clear non active flag for disabled users
            List<Participants> disabledParticipants = userRepository.GetDisabledParticipantsNonActive();
            foreach (var participant in disabledParticipants) {
                try {
                    userRepository.SetRevertNonActiveUser(participant.id);
                } catch (Exception ex) {
                    HandleError(ex);
                }
            }
        }

        private void HandleError(Exception ex) {
            string folder = Path.Combine(new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName, "ErrorLog");
            if (!Directory.Exists(folder)) {
                Directory.CreateDirectory(folder);
            }
            string errFileName = Path.Combine(folder, "ErrorLog_" + DateTime.Now.ToString("yyyyMM") + ".log");
            using (StreamWriter sw = new StreamWriter(errFileName, true)) {
                string strMsg = DateTime.Now.ToString() + " " + ex.ToString();
                sw.WriteLine(strMsg);
            }
        }

        public void CheckEmptyRoles() {
            Console.WriteLine("Checking empty roles ...");

            UserRepository userRepository = new UserRepository();

            List<Participants> activeParticipants = userRepository.GetActiveParticipants();

            foreach (var participant in activeParticipants) {
                if (participant.ParticipantRole_CentreGroup != null) {
                    foreach (var userRole in participant.ParticipantRole_CentreGroup) {
                        if (userRole.role_id == (int)UserRole.ApprovalManager) {
                            userRepository.DeleteEmptyAppManRole(userRole.participant_id, userRole.centre_group_id);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Stopped working
        /// </summary>
        public void LoadUserPhotos() {
            UserRepository userRepository = new UserRepository();
            UserPhotoRepository userPhotoRepository = new UserPhotoRepository();
            List<Participants> participants = userRepository.GetPhotoActiveParticipants();
            List<Participants> partialParticipants = GetPartialParticipants(participants);
            
            CookieCollection cc = null;

            //cc = LoginToOwa();

            int i = 0;
            int iCount = partialParticipants.Count;
            //foreach (var participant in partialParticipants) {
            while (partialParticipants.Count > 0) { 
                int iPartIndex = new Random().Next(0, partialParticipants.Count - 1);
                Participants participant = partialParticipants.ElementAt(iPartIndex);

                if (i % 12 == 0) {
                    try {
                        cc = LoginToOwa();
                    } catch (Exception ex) {
                        throw ex;
                    }
                }


                string eMail = participant.email;
                HttpStatusCode returnCode = HttpStatusCode.Unused;
                int iRepeatCount = 0;
                
                do {

                    byte[] imgByte = GetUserPhoto(eMail, cc, out returnCode);

                    string consMsg = "Loading Owa Photo " + participant.surname;

                    if (returnCode == HttpStatusCode.OK) {
                        consMsg += " Loaded";
                        ParticipantPhoto participantPhoto = userPhotoRepository.GetParticipantPhoto(participant.id);
                        if (participantPhoto == null) {
                            participantPhoto = new ParticipantPhoto();
                            participantPhoto.participant_id = participant.id;
                            consMsg += " Photo was not Found";
                        } else {
                            consMsg += " -> PHOTO FOUND  :-):-):-):-):-):-)";
                        }

                        participantPhoto.user_picture_240 = imgByte;
                        participantPhoto.modif_date = DateTime.Now;
                        userPhotoRepository.SaveParticipantPhotoData(participantPhoto);

                        //using (Stream file = File.OpenWrite(@"c:\temp\UserPhotos\" + participant.surname + participant.id + ".png")) {
                        //    file.Write(imgByte, 0, imgByte.Length);
                        //}
                    } else {
                        iRepeatCount++;

                        consMsg = "Loading Owa Photo " + participant.surname + " ERROR !!! " + returnCode;
                        Thread.Sleep(10000);
                        try {
                            cc = LoginToOwa();
                        } catch (Exception ex) {
                            throw ex;
                        }

                    }

                    Console.WriteLine(i + "/" + iCount + ". " + consMsg);
                   
                    i++;

                    Thread.Sleep(10000);
                } while (returnCode != HttpStatusCode.OK && iRepeatCount++ < 2);

                partialParticipants.RemoveAt(iPartIndex);
            }

        }

        public void LoadUserPhotosSelenium() {
            UserRepository userRepository = new UserRepository();
            UserPhotoRepository userPhotoRepository = new UserPhotoRepository();
            List<Participants> participants = userRepository.GetPhotoActiveParticipants();
            List<Participants> partialParticipants = GetPartialParticipants(participants);
#if DEBUG
            partialParticipants = participants;
#endif

            int i = 0;
            int iCount = partialParticipants.Count;

            SeleniumPhoto seleniumPhoto = new SeleniumPhoto();
            try {
                while (partialParticipants.Count > 0) {
                    int iPartIndex = new Random().Next(0, partialParticipants.Count - 1);
                    Participants participant = partialParticipants.ElementAt(iPartIndex);

#if DEBUG
                    //participant = new UserRepository().GetParticipantById(1444);
#endif

                    string eMail = participant.email;
                    int iRepeatCount = 0;

                    string consMsg = "";
                    do {

                        try {
                            byte[] imgByte = seleniumPhoto.GetUserPhoto(participant.email);

                            consMsg = "Loading Owa Photo " + participant.surname + " " + participant.first_name;

                            if (imgByte != null) {
                                consMsg += " *** Loaded :-) ***";
                            } else {
                                consMsg += " Not found";
                            }
                            
                            ParticipantPhoto participantPhoto = userPhotoRepository.GetParticipantPhoto(participant.id);
                            if (participantPhoto == null) {
                                participantPhoto = new ParticipantPhoto();
                                participantPhoto.participant_id = participant.id;
                            }

                            participantPhoto.user_picture_240 = imgByte;
                            participantPhoto.modif_date = DateTime.Now;
                            userPhotoRepository.SaveParticipantPhotoData(participantPhoto);
                                                       
                            break;
                        } catch (Exception ex) {
                            if (ex is TooMuchErrorsEx) {
                                Console.WriteLine("*************************************************");
                                Console.WriteLine(" TOO MUCH ERRORS OCCURED ");
                                Console.WriteLine("*************************************************");
                                Environment.Exit(0);
                            }
                            if (iRepeatCount == 2) {
                                consMsg += " ERROR " + ex.ToString();
                                break;
                            }
                        }


                    } while (iRepeatCount++ < 2);

                    Console.WriteLine(i + "/" + iCount + ". " + consMsg);
                    i++;
                    Thread.Sleep(2000);

                    partialParticipants.RemoveAt(iPartIndex);
                }
            } catch (Exception ex) {
                throw ex;
            } finally {
                seleniumPhoto.DispatchSelenium();
            }

        }

        public void LoadUserPhotosGraph() {
            
            UserRepository userRepository = new UserRepository();
            UserPhotoRepository userPhotoRepository = new UserPhotoRepository();
            List<Participants> participants = userRepository.GetPhotoActiveParticipants();
            //List<Participants> partialParticipants = GetPartialParticipants(participants);

//#if DEBUG
            var partialParticipants = participants.OrderByDescending(x => x.id).ToList();
//#endif


            int i = 0;
            int iCount = partialParticipants.Count;
            SeleniumPhoto seleniumPhoto = new SeleniumPhoto();
            try {
                while (partialParticipants.Count > 0) {
                    //int iPartIndex = new Random().Next(0, partialParticipants.Count - 1);
//#if DEBUG
                    int iPartIndex = 0;
//#endif
                    Participants participant = partialParticipants.ElementAt(iPartIndex);

#if DEBUG
                    //participant = new UserRepository().GetParticipantById(1444);
#endif

                    string eMail = participant.email;
#if DEBUG
                    //eMail = "kamil.sykora@otis.com";
#endif
                    int iRepeatCount = 0;

                    string consMsg = "";
                    do {
                        
                        try {
                            byte[] imgByte = seleniumPhoto.GetUserPhotoGraph(eMail);

                            consMsg = "Loading Graph Photo " + participant.surname + " " + participant.first_name;

                            if (imgByte != null) {
                                consMsg += " *** Loaded :-) ***";
                            } else {
                                consMsg += " Not found";
                            }

                            ParticipantPhoto participantPhoto = userPhotoRepository.GetParticipantPhoto(participant.id);
                            if (participantPhoto == null) {
                                participantPhoto = new ParticipantPhoto();
                                participantPhoto.participant_id = participant.id;
                            }

                            participantPhoto.user_picture_240 = imgByte;
                            participantPhoto.modif_date = DateTime.Now;
                            userPhotoRepository.SaveParticipantPhotoData(participantPhoto);

                            break;
                        } catch (Exception ex) {
                            if (ex is TooMuchErrorsEx) {
                                Console.WriteLine("*************************************************");
                                Console.WriteLine(" TOO MUCH ERRORS OCCURED ");
                                Console.WriteLine("*************************************************");
                                Environment.Exit(0);
                            }
                            if (iRepeatCount == 2) {
                                consMsg += " ERROR " + ex.ToString();
                                break;
                            }
                        }


                    } while (iRepeatCount++ < 2);

                    Console.WriteLine(i + "/" + iCount + ". " + consMsg);
                    i++;
                    Thread.Sleep(2000);

                    partialParticipants.RemoveAt(iPartIndex);

                }
            } catch (Exception ex) {
                throw ex;
            } finally {
                seleniumPhoto.DispatchSelenium();
            }

        }

        //private List<Participants> GetRandomParticipants(List<Participants> participants) {
        //    List<Participants> randomParticipants = new List<Participants>();

        //    //load only 100 photos - it takes quite long so it is loaded only partially each day a part - 100 users
        //    while (participants.Count > 0 && randomParticipants.Count < 100) {
        //        Random rnd = new Random();
        //        int iIndex = rnd.Next(participants.Count - 1);
        //        var part = participants.ElementAt(iIndex);

        //        randomParticipants.Add(part);
        //        participants.RemoveAt(iIndex);
        //    }

        //    return randomParticipants;
        //}

        private List<Participants> GetPartialParticipants(List<Participants> participants) {
            if (participants.Count < 15) {
                return participants;
            }

            var dayPart = participants.Count / 7;
            int iDayPartCount = Convert.ToInt32(dayPart) + 1;
            int iDayOfWeek = (int)DateTime.Now.DayOfWeek;
#if DEBUG
            //iDayOfWeek = (int) new DateTime(2020, 3, 16).DayOfWeek;
#endif
            int iStart = iDayOfWeek * iDayPartCount;
            int iStop = iStart + iDayPartCount;
            if (iStart > 0) iStart--;
            if (iStop > participants.Count) iStop = participants.Count;
            if (iStop < participants.Count) iStop++;

            List<Participants> partialParticipants = new List<Participants>();
            for (int i = iStart; i < iStop; i++) {
                var part = participants.ElementAt(i);
                partialParticipants.Add(part);
            }
                        
            return partialParticipants;
        }

        private byte[] GetUserPhoto(string email, CookieCollection cc, out HttpStatusCode returnCode) {
            try {
                List<RequestHeader> responseHeaders = null;
                RegetWebRequest wr = new RegetWebRequest();

                string urlMail = email.Replace("@", "%40");

                //UTC OWa
                //string strUrl = "https://owa.utc.com/owa/service.svc/s/GetPersonaPhoto?email=" + urlMail + "&UA=0&size=HR240x240&t=" + DateTime.Now.ToString();

                //OTIS
                string strUrl = "https://outlook.office.com/owa/service.svc/s/GetPersonaPhoto?email=" + urlMail + "&UA=0&size=HR240x240&t=" + DateTime.Now.ToString();

#region Photo
                CookieCollection tmpCc1000;
                List<RequestHeader>  requestHeaders = new List<RequestHeader>();
                requestHeaders.Add(new RequestHeader("Host", "outlook.office.com"));
                requestHeaders.Add(new RequestHeader("Connection", "keep-alive"));
                requestHeaders.Add(new RequestHeader("sec-ch-ua", "\"Google Chrome\";v=\"87\", \"\\\"Not; A\\\\Brand\";v=\"99\", \"Chromium\";v=\"87\""));
                requestHeaders.Add(new RequestHeader("sec-ch-ua-mobile", "?0"));
                requestHeaders.Add(new RequestHeader("Upgrade-Insecure-Requests", "1"));
                requestHeaders.Add(new RequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36"));
                requestHeaders.Add(new RequestHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9"));
                requestHeaders.Add(new RequestHeader("Sec-Fetch-Site", "none"));
                requestHeaders.Add(new RequestHeader("Sec-Fetch-Mode", "navigate"));
                requestHeaders.Add(new RequestHeader("Sec-Fetch-Dest", "document"));
                requestHeaders.Add(new RequestHeader("Accept-Encoding", "gzip, deflate, br"));
                requestHeaders.Add(new RequestHeader("Accept-Language", "cs-CZ,cs;q=0.9,en;q=0.8"));

                byte[] responseBytes = null;
                wr.HttpGet(strUrl, cc, requestHeaders, out tmpCc1000, out responseHeaders, out responseBytes, out returnCode);
                cc = wr.JoinCookies(cc, tmpCc1000);
                
                byte[] imgByte = null;
                if (returnCode == HttpStatusCode.OK) {
                    imgByte = responseBytes;
                }

                if (imgByte != null && imgByte.Length < 64) {
                    imgByte = null;
                }
#endregion

                return imgByte;
            } catch(Exception ex) {
                returnCode = HttpStatusCode.InternalServerError;
                return null;
                //throw ex;
            }
        }

        private CookieCollection LoginToOwa() {
            RegetWebRequest wr = new RegetWebRequest();
            CookieCollection cc = new CookieCollection();
            //byte[] byteResponse;
            string strResponse = null;
            HttpStatusCode returnCode;

            string owaUserName = System.Configuration.ConfigurationManager.AppSettings["owaUserName"];
            string owaUserPwd = System.Configuration.ConfigurationManager.AppSettings["owaUserPwd"];
            List<RequestHeader> responseHeaders = null;

            //10 - OK
#region login.microsoftonline.com
            CookieCollection tmpCc10;
            List<RequestHeader> requestHeaders = new List<RequestHeader>();
            requestHeaders.Add(new RequestHeader("Host", "login.microsoftonline.com"));
            requestHeaders.Add(new RequestHeader("Connection", "keep-alive"));
            requestHeaders.Add(new RequestHeader("sec-ch-ua", "\"Google Chrome\";v=\"87\", \"\\\"Not; A\\\\Brand\";v=\"99\", \"Chromium\";v=\"87\""));
            requestHeaders.Add(new RequestHeader("sec-ch-ua-mobile", "?0"));
            requestHeaders.Add(new RequestHeader("Upgrade-Insecure-Requests", "1"));
            requestHeaders.Add(new RequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36"));
            requestHeaders.Add(new RequestHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9"));
            requestHeaders.Add(new RequestHeader("Sec-Fetch-Site", "none"));
            requestHeaders.Add(new RequestHeader("Sec-Fetch-Mode", "navigate"));
            requestHeaders.Add(new RequestHeader("Sec-Fetch-User", "?1"));
            requestHeaders.Add(new RequestHeader("Sec-Fetch-Dest", "document"));
            requestHeaders.Add(new RequestHeader("Accept-Encoding", "gzip, deflate, br"));
            requestHeaders.Add(new RequestHeader("Accept-Language", "cs-CZ,cs;q=0.9,en;q=0.8"));

            wr.HttpGet("https://login.microsoftonline.com", cc, requestHeaders, out tmpCc10, out responseHeaders, out strResponse, out returnCode);
            cc = wr.JoinCookies(cc, tmpCc10);
#endregion

            //20 - OK
#region https://www.office.com/login
            CookieCollection tmpCc20;

            requestHeaders = new List<RequestHeader>();
            requestHeaders.Add(new RequestHeader("Host", "www.office.com"));
            requestHeaders.Add(new RequestHeader("Connection", "keep-alive"));
            requestHeaders.Add(new RequestHeader("Upgrade-Insecure-Requests", "1"));
            requestHeaders.Add(new RequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36"));
            requestHeaders.Add(new RequestHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9"));
            requestHeaders.Add(new RequestHeader("Sec-Fetch-Site", "none"));
            requestHeaders.Add(new RequestHeader("Sec-Fetch-Mode", "navigate"));
            requestHeaders.Add(new RequestHeader("Sec-Fetch-User", "?1"));
            requestHeaders.Add(new RequestHeader("Sec-Fetch-Dest", "document"));
            requestHeaders.Add(new RequestHeader("sec-ch-ua", "\"Google Chrome\";v=\"87\", \"\\\"Not; A\\\\Brand\";v=\"99\", \"Chromium\";v=\"87\""));
            requestHeaders.Add(new RequestHeader("sec-ch-ua-mobile", "?0"));
            requestHeaders.Add(new RequestHeader("Accept-Encoding", "gzip, deflate, br"));
            requestHeaders.Add(new RequestHeader("Accept-Language", "cs-CZ,cs;q=0.9,en;q=0.8"));

            wr.HttpGet("https://www.office.com/login", cc, requestHeaders, out tmpCc20, out responseHeaders, out strResponse, out returnCode);
            cc = wr.JoinCookies(cc, tmpCc20);
            string location = GetHeaderValue(responseHeaders, "Location");
            int iPosStart = location.IndexOf("client-request-id=") + "client-request-id=".Length;
            int iPosEnd = location.IndexOf("&", iPosStart);
            string clientRequestId = location.Substring(iPosStart, iPosEnd - iPosStart);
#endregion

            //30 - OK
#region Location
            CookieCollection tmpCc30;
            requestHeaders = new List<RequestHeader>();
            requestHeaders.Add(new RequestHeader("Host", "login.microsoftonline.com"));
            requestHeaders.Add(new RequestHeader("Connection", "keep-alive"));
            requestHeaders.Add(new RequestHeader("Upgrade-Insecure-Requests", "1"));
            requestHeaders.Add(new RequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36"));
            requestHeaders.Add(new RequestHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9"));
            requestHeaders.Add(new RequestHeader("Sec-Fetch-Site", "none"));
            requestHeaders.Add(new RequestHeader("Sec-Fetch-Mode", "navigate"));
            requestHeaders.Add(new RequestHeader("Sec-Fetch-User", "?1"));
            requestHeaders.Add(new RequestHeader("Sec-Fetch-Dest", "document"));
            requestHeaders.Add(new RequestHeader("sec-ch-ua", "\"Google Chrome\";v=\"87\", \"\\\"Not; A\\\\Brand\";v=\"99\", \"Chromium\";v=\"87\""));
            requestHeaders.Add(new RequestHeader("sec-ch-ua-mobile", "?0"));
            requestHeaders.Add(new RequestHeader("Accept-Encoding", "gzip, deflate, br"));
            requestHeaders.Add(new RequestHeader("Accept-Language", "cs-CZ,cs;q=0.9,en;q=0.8"));

            wr.HttpGet(location, cc, requestHeaders, out tmpCc30, out responseHeaders, out strResponse, out returnCode);
            cc = wr.JoinCookies(cc, tmpCc30);
            //string result = System.Text.Encoding.UTF8.GetString(byteResponse);
            iPosStart = strResponse.IndexOf("canary\"");
            iPosStart = strResponse.IndexOf(":\"", iPosStart) + 2;
            iPosEnd = strResponse.IndexOf("\",", iPosStart);
            string canary = strResponse.Substring(iPosStart, iPosEnd - iPosStart);
#endregion

            //40 - OK
#region Location &sso_reload=true
            CookieCollection tmpCc40;

            requestHeaders = new List<RequestHeader>();
            requestHeaders.Add(new RequestHeader("Host", "login.microsoftonline.com"));
            requestHeaders.Add(new RequestHeader("Connection", "keep-alive"));
            requestHeaders.Add(new RequestHeader("sec-ch-ua", "\"Google Chrome\";v=\"87\", \"\\\"Not; A\\\\Brand\";v=\"99\", \"Chromium\";v=\"87\""));
            requestHeaders.Add(new RequestHeader("sec-ch-ua-mobile", "?0"));
            requestHeaders.Add(new RequestHeader("Upgrade-Insecure-Requests", "1"));
            requestHeaders.Add(new RequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36"));
            requestHeaders.Add(new RequestHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9"));
            requestHeaders.Add(new RequestHeader("Sec-Fetch-Site", "same-origin"));
            requestHeaders.Add(new RequestHeader("Sec-Fetch-Mode", "navigate"));
            requestHeaders.Add(new RequestHeader("Sec-Fetch-Dest", "document"));
            requestHeaders.Add(new RequestHeader("Referer", "https://login.microsoftonline.com/common/oauth2/authorize?client_id=4345a7b9-9a63-4910-a426-35363201d503&redirect_uri=https%3A%2F%2Fwww.office.com%2Flanding&response_type=code%20id_token&scope=openid%20profile&response_mode=form_post&nonce=637421798753086875.YzU1Mzg3NmQtZTNkMy00ZmViLTgzMjYtM2Y1OTA5N2MzNWM2M2Y1OTc3MjgtY2Y0Mi00YjEyLTkzNWUtZTI4YTgwMmZlYjhj&ui_locales=cs-CZ&mkt=cs-CZ&client-request-id=6961a529-3701-49c3-8c5f-8314b2b01e66&state=52MpVHTek26IhZeu-iK0pIGIJeptKu_64m90l7qQF13pIt-SVmcXYNn6UAnRNMH_A5uWyGoJ4zoK61J_zEpjGp9oS_HBvybtsugzJZAXEIQG1NAHza_t5h_criv5VIINzyKnC67qSaIHAE6s7HPFraNDfr4_MKXAoBvPQQRoYwS-3S9CRxVB557hN9jTmllsQfz9pWXwUd-SopIipFijyrF_5al1NDugOtJ9kI5ZFiBb8wntjgSM7ZwfE4Y1vtD5kH85IHdoRFK3MpYT3gwfrA&x-client-SKU=ID_NETSTANDARD2_0&x-client-ver=6.6.0.0"));
            requestHeaders.Add(new RequestHeader("Accept-Encoding", "gzip, deflate, br"));
            requestHeaders.Add(new RequestHeader("Accept-Language", "cs-CZ,cs;q=0.9,en;q=0.8"));

            wr.HttpGet(location + "&sso_reload=true", cc, requestHeaders, out tmpCc40, out responseHeaders, out strResponse, out returnCode);
            cc = wr.JoinCookies(cc, tmpCc40);
            string xmsrequestid = GetHeaderValue(responseHeaders, "x-ms-request-id");
            //result = System.Text.Encoding.UTF8.GetString(byteResponse);

            iPosStart = strResponse.IndexOf("ctx%3d") + "ctx%3d".Length;
            iPosEnd = strResponse.IndexOf("\\u0026", iPosStart);
            string ctx = strResponse.Substring(iPosStart, iPosEnd - iPosStart);

            iPosStart = strResponse.IndexOf("sFT\":\"") + "sFT\":\"".Length;
            iPosEnd = strResponse.IndexOf("\",", iPosStart);
            string flowToken = strResponse.Substring(iPosStart, iPosEnd - iPosStart);

            iPosStart = strResponse.IndexOf("Trace ID:") + "Trace ID:".Length;
            iPosEnd = strResponse.IndexOf("\r\n", iPosStart);
            string traceId = strResponse.Substring(iPosStart, iPosEnd - iPosStart).Trim();

            iPosStart = strResponse.IndexOf("hpgid\":") + "hpgid\":".Length;
            iPosEnd = strResponse.IndexOf(",", iPosStart);
            string hpgId = strResponse.Substring(iPosStart, iPosEnd - iPosStart).Trim();
#endregion

#region https://login.microsoftonline.com/common/GetCredentialType?mkt=en-US NOT USED
            ////CookieCollection tmpCc50;
            ////string strResponse;
            ////HttpWebResponse httpWebResponse;

            ////            string json = "{\"username\":\"kamil.sykora@otis.com\"," + 
            ////                "\"isOtherIdpSupported\":true," + 
            ////                "\"checkPhones\":false," + 
            ////                "\"isRemoteNGCSupported\":true," + 
            ////                "\"isCookieBannerShown\":false," + 
            ////                "\"isFidoSupported\":true," +
            ////                "\"originalRequest\":" + ctx +
            ////                "\"country\":\"FR\"," + 
            ////                "\"forceotclogin\":false," +
            ////                "\"isExternalFederationDisallowed\":false," + 
            ////                "\"isRemoteConnectSupported\":false," + 
            ////                "\"federationFlags\":0," + 
            ////                "\"isSignup\":false," + 
            ////                "\"flowToken\":" + flowToken +
            ////                "\"isAccessPassSupported\":true}";



            ////            wr.PostForm(
            ////                "https://login.microsoftonline.com/common/GetCredentialType?mkt=en-US",
            ////                null,
            ////                cc,
            ////                requestHeaders,
            ////                json,
            ////                false,
            ////                out tmpCc50,
            ////                out strResponse,
            ////                out httpWebResponse);
            ////            cc = wr.JoinCookies(cc, tmpCc50);
            ////            //string flowToken = GetHeaderValue(responseHeaders, "FlowToken");

#endregion

            //60 - OK
#region https://login.microsoftonline.com/common/login
            
            string url = "https://login.microsoftonline.com/common/login";
            string strFormFields =
                "i13=0" +
                "&" + "login=" + owaUserName.Replace("@", "%40") +
                "&" + "loginfmt=" + owaUserName.Replace("@", "%40") +
                "&" + "type=" + "11" +
                "&" + "LoginOptions=" + "3" +
                "&" + "irt=" + "" +
                "&" + "lrtPartition=" + "" +
                "&" + "hisRegion=" + "" +
                "&" + "hisScaleUnit=" + "" +
                "&" + "passwd=" + owaUserPwd +
                "&" + "ps=" + "2" +
                "&" + "psRNGCDefaultType=" + "" +
                "&" + "psRNGCEntropy=" + "" +
                "&" + "psRNGCSLK=" + "" +
                "&" + "canary=" + canary +
                "&" + "ctx=" + ctx +
                "&" + "hpgrequestid=" + xmsrequestid +
                "&" + "flowToken=" + flowToken +
                "&" + "PPSX=" + "" +
                "&" + "NewUser=" + "1" +
                "&" + "FoundMSAs=" + "" +
                "&" + "fspost=" + "0" +
                "&" + "i21=" + "0" +
                "&" + "CookieDisclosure=" + "0" +
                "&" + "IsFidoSupported=" + "1" +
                "&" + "isSignupPost=" + "0" +
                "&" + "i2=" + "1" +
                "&" + "i17=" + "" +
                "&" + "i18=" + "" +
                "&" + "i19=" + "22016";

            Cookie cookie = new Cookie();
            cookie.Name = "AADSSO";
            cookie.Value = "NA|NoExtension";
            cc.Add(cookie);

            cookie = new Cookie();
            cookie.Name = "SSOCOOKIEPULLED";
            cookie.Value = "1";
            cc.Add(cookie);

            cookie = new Cookie();
            cookie.Name = "brcap";
            cookie.Value = "0";

            cookie = new Cookie();
            cookie.Name = "wlidperf";
            cookie.Value = "FR=L&ST=1606650098555";

            cc.Add(cookie);

            CookieCollection tmpCc60;

            requestHeaders = new List<RequestHeader>();
            requestHeaders.Add(new RequestHeader("Host", "login.microsoftonline.com"));
            requestHeaders.Add(new RequestHeader("Connection", "keep-alive"));
            requestHeaders.Add(new RequestHeader("Content-Length", "1872"));
            requestHeaders.Add(new RequestHeader("Cache-Control", "max-age=0"));
            requestHeaders.Add(new RequestHeader("sec-ch-ua", "\"Google Chrome\";v=\"87\", \"\\\"Not; A\\\\Brand\";v=\"99\", \"Chromium\";v=\"87\""));
            requestHeaders.Add(new RequestHeader("sec-ch-ua-mobile", "?0"));
            requestHeaders.Add(new RequestHeader("Upgrade-Insecure-Requests", "1"));
            requestHeaders.Add(new RequestHeader("Origin", "https://login.microsoftonline.com"));
            requestHeaders.Add(new RequestHeader("Content-Type", "application/x-www-form-urlencoded"));
            requestHeaders.Add(new RequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36"));
            requestHeaders.Add(new RequestHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9"));
            requestHeaders.Add(new RequestHeader("Sec-Fetch-Site", "same-origin"));
            requestHeaders.Add(new RequestHeader("Sec-Fetch-Mode", "navigate"));
            requestHeaders.Add(new RequestHeader("Sec-Fetch-User", "?1"));
            requestHeaders.Add(new RequestHeader("Sec-Fetch-Dest", "document"));
            requestHeaders.Add(new RequestHeader("Referer", location + "&sso_reload=true"));
            requestHeaders.Add(new RequestHeader("Accept-Encoding", "gzip, deflate, br"));
            requestHeaders.Add(new RequestHeader("Accept-Language", "cs-CZ,cs;q=0.9,en;q=0.8"));

            //string strResponse;
            HttpWebResponse httpWebResponse;

            wr.HttpPost(
                url,
                strFormFields,
                cc,
                requestHeaders,
                null,
                false,
                out tmpCc60,
                out strResponse,
                out httpWebResponse);
            cc = wr.JoinCookies(cc, tmpCc60);
#endregion

#region https://login.microsoftonline.com/kmsi
            CookieCollection tmpCc70;

            requestHeaders = new List<RequestHeader>();
            requestHeaders.Add(new RequestHeader("Host", "login.microsoftonline.com"));
            requestHeaders.Add(new RequestHeader("Connection", "keep-alive"));
            requestHeaders.Add(new RequestHeader("Content-Length", "1798"));
            requestHeaders.Add(new RequestHeader("Cache-Control", "max-age=0"));
            requestHeaders.Add(new RequestHeader("sec-ch-ua", "\"Google Chrome\";v=\"87\", \"\\\"Not; A\\\\Brand\";v=\"99\", \"Chromium\";v=\"87\""));
            requestHeaders.Add(new RequestHeader("sec-ch-ua-mobile", "?0"));
            requestHeaders.Add(new RequestHeader("Upgrade-Insecure-Requests", "1"));
            requestHeaders.Add(new RequestHeader("Origin", "https://login.microsoftonline.com"));
            requestHeaders.Add(new RequestHeader("Content-Type", "application/x-www-form-urlencoded"));
            requestHeaders.Add(new RequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36"));
            requestHeaders.Add(new RequestHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9"));
            requestHeaders.Add(new RequestHeader("Sec-Fetch-Site", "same-origin"));
            requestHeaders.Add(new RequestHeader("Sec-Fetch-Mode", "navigate"));
            requestHeaders.Add(new RequestHeader("Sec-Fetch-User", "?1"));
            requestHeaders.Add(new RequestHeader("Sec-Fetch-Dest", "document"));
            requestHeaders.Add(new RequestHeader("Referer", "https://login.microsoftonline.com/common/login"));
            requestHeaders.Add(new RequestHeader("Accept-Encoding", "gzip, deflate, br"));
            requestHeaders.Add(new RequestHeader("Accept-Language", "cs-CZ,cs;q=0.9,en;q=0.8"));

            strFormFields =
                "i13=0" +
                "&" + "LoginOptions=" + "3" +
                "&" + "type=" + "28" +
                "&" + "ctx=" + ctx +
                "&" + "hpgrequestid=" + xmsrequestid +
                "&" + "flowToken=" + flowToken +
                "&" + "canary=" + canary;

            wr.HttpPost(
                "https://login.microsoftonline.com/kmsi",
                strFormFields,
                cc,
                requestHeaders,
                null,
                false,
                out tmpCc70,
                out strResponse,
                out httpWebResponse);
            cc = wr.JoinCookies(cc, tmpCc70);
#endregion

#region https://outlook.office.com/owa/SuiteServiceProxy.aspx?suiteServiceUserName=Kamil.Sykora%40otis.com&suiteServiceReturnUrl=https%3A%2F%2Fwww.office.com%2F%3Fauth%3D2&apiver=1
            CookieCollection tmpCc80;

            requestHeaders = new List<RequestHeader>();
            requestHeaders.Add(new RequestHeader("Host", "outlook.office.com"));
            requestHeaders.Add(new RequestHeader("Connection", "keep-alive"));
            requestHeaders.Add(new RequestHeader("sec-ch-ua", "\"Google Chrome\";v=\"87\", \"\\\"Not; A\\\\Brand\";v=\"99\", \"Chromium\";v=\"87\""));
            requestHeaders.Add(new RequestHeader("sec-ch-ua-mobile", "?0"));
            requestHeaders.Add(new RequestHeader("Upgrade-Insecure-Requests", "1"));
            requestHeaders.Add(new RequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36"));
            requestHeaders.Add(new RequestHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9"));
            requestHeaders.Add(new RequestHeader("Sec-Fetch-Site", "same-site"));
            requestHeaders.Add(new RequestHeader("Sec-Fetch-Mode", "navigate"));
            requestHeaders.Add(new RequestHeader("Sec-Fetch-Dest", "iframe"));
            requestHeaders.Add(new RequestHeader("Referer", "https://www.office.com/"));
            requestHeaders.Add(new RequestHeader("Accept-Encoding", "gzip, deflate, br"));
            requestHeaders.Add(new RequestHeader("Accept-Language", "cs-CZ,cs;q=0.9,en;q=0.8"));

            url = "https://outlook.office.com/owa/SuiteServiceProxy.aspx?suiteServiceUserName=Kamil.Sykora%40otis.com&suiteServiceReturnUrl=https%3A%2F%2Fwww.office.com%2F%3Fauth%3D2&apiver=1";
            wr.HttpGet(url, cc, requestHeaders, out tmpCc80, out responseHeaders, out strResponse, out returnCode);
            cc = wr.JoinCookies(cc, tmpCc80);

            //result = System.Text.Encoding.UTF8.GetString(byteResponse);

            string location1 = GetHeaderValue(responseHeaders, "Location");

            iPosStart = location1.IndexOf("&state=") + "&state=".Length;
            iPosEnd = location1.IndexOf("&", iPosStart);
            string state = null;
            if (iPosEnd > 0) {
                state = location1.Substring(iPosStart, iPosEnd - iPosStart);
            } else {
                state = location1.Substring(iPosStart);
            }

            iPosStart = location1.IndexOf("&client-request-id=") + "&state=".Length;
            iPosEnd = location1.IndexOf("&", iPosStart);
            string correlactionId = null;
            if (iPosEnd > 0) {
                correlactionId = location1.Substring(iPosStart, iPosEnd - iPosStart);
            } else {
                correlactionId = location1.Substring(iPosStart);
            }
#endregion

#region Location
            CookieCollection tmpCc90;

            requestHeaders = new List<RequestHeader>();
            requestHeaders.Add(new RequestHeader("Host", "login.microsoftonline.com"));
            requestHeaders.Add(new RequestHeader("Connection", "keep-alive"));
            requestHeaders.Add(new RequestHeader("Upgrade-Insecure-Requests", "1"));
            requestHeaders.Add(new RequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36"));
            requestHeaders.Add(new RequestHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9"));
            requestHeaders.Add(new RequestHeader("Sec-Fetch-Site", "cross-site"));
            requestHeaders.Add(new RequestHeader("Sec-Fetch-Mode", "navigate"));
            requestHeaders.Add(new RequestHeader("Sec-Fetch-Dest", "iframe"));
            requestHeaders.Add(new RequestHeader("sec-ch-ua", "\"Google Chrome\";v=\"87\", \"\\\"Not; A\\\\Brand\";v=\"99\", \"Chromium\";v=\"87\""));
            requestHeaders.Add(new RequestHeader("sec-ch-ua-mobile", "?0"));
            requestHeaders.Add(new RequestHeader("Referer", "https://www.office.com/"));
            requestHeaders.Add(new RequestHeader("Accept-Encoding", "gzip, deflate, br"));
            requestHeaders.Add(new RequestHeader("Accept-Language", "cs-CZ,cs;q=0.9,en;q=0.8"));

            wr.HttpGet(location1, cc, requestHeaders, out tmpCc90, out responseHeaders, out strResponse, out returnCode);
            cc = wr.JoinCookies(cc, tmpCc90);

            //result = System.Text.Encoding.UTF8.GetString(byteResponse);

            iPosStart = strResponse.IndexOf("name=\"code\" value=\"") + "name=\"code\" value=\"".Length;
            iPosEnd = strResponse.IndexOf("\"", iPosStart);
            string code = strResponse.Substring(iPosStart, iPosEnd - iPosStart);

            iPosStart = strResponse.IndexOf("name=\"id_token\" value=\"") + "name=\"id_token\" value=\"".Length;
            iPosEnd = strResponse.IndexOf("\"", iPosStart);
            string idToken = strResponse.Substring(iPosStart, iPosEnd - iPosStart);
#endregion

#region https://outlook.office.com/owa/
            string sessState = null;
            foreach (var c in cc) {
                Cookie cook = (Cookie)c;
                if(cook.Name.Trim().ToUpper() == "ESTSAUTHLIGHT") {
                    sessState = cook.Value.Substring(1).Trim();
                }
            }

            url = "https://outlook.office.com/owa/";
            strFormFields =
                "code=" + code +
                "&" + "id_token=" + idToken +
                "&" + "state=" + state +
                "&" + "session_state=" + sessState +
                "&" + "correlation_id=" + correlactionId;
                        
            CookieCollection tmpCc100;

            requestHeaders = new List<RequestHeader>();
            requestHeaders.Add(new RequestHeader("Host", "outlook.office.com"));
            requestHeaders.Add(new RequestHeader("Connection", "keep-alive"));
            requestHeaders.Add(new RequestHeader("Content-Length", "2907"));
            requestHeaders.Add(new RequestHeader("Cache-Control", "max-age=0"));
            requestHeaders.Add(new RequestHeader("sec-ch-ua", "\"Google Chrome\";v=\"87\", \"\\\"Not; A\\\\Brand\";v=\"99\", \"Chromium\";v=\"87\""));
            requestHeaders.Add(new RequestHeader("sec-ch-ua-mobile", "?0"));
            requestHeaders.Add(new RequestHeader("Upgrade-Insecure-Requests", "1"));
            requestHeaders.Add(new RequestHeader("Origin", "https://login.microsoftonline.com"));
            requestHeaders.Add(new RequestHeader("Content-Type", "application/x-www-form-urlencoded"));
            requestHeaders.Add(new RequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36"));
            requestHeaders.Add(new RequestHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9"));
            requestHeaders.Add(new RequestHeader("Sec-Fetch-Site", "cross-site"));
            requestHeaders.Add(new RequestHeader("Sec-Fetch-Mode", "navigate"));
            requestHeaders.Add(new RequestHeader("Sec-Fetch-Dest", "iframe"));
            requestHeaders.Add(new RequestHeader("Referer", "https://login.microsoftonline.com/"));
            requestHeaders.Add(new RequestHeader("Accept-Encoding", "gzip, deflate, br"));
            requestHeaders.Add(new RequestHeader("Accept-Language", "cs-CZ,cs;q=0.9,en;q=0.8"));

            wr.HttpPost(
                url,
                strFormFields,
                cc,
                requestHeaders,
                null,
                false,
                out tmpCc100,
                out strResponse,
                out httpWebResponse);
            cc = wr.JoinCookies(cc, tmpCc100);
#endregion

            cookie = new Cookie();
            cookie.Name = "domainName";
            cookie.Value = "otis.com";
            cc.Add(cookie);

            cookie = new Cookie();
            cookie.Name = "OIDC";
            cookie.Value = "1";
            cc.Add(cookie);

            //#region Photo
            //CookieCollection tmpCc1000;
            //requestHeaders = new List<RequestHeader>();
            //requestHeaders.Add(new RequestHeader("Host", "outlook.office.com"));
            //requestHeaders.Add(new RequestHeader("Connection", "keep-alive"));
            //requestHeaders.Add(new RequestHeader("sec-ch-ua", "\"Google Chrome\";v=\"87\", \"\\\"Not; A\\\\Brand\";v=\"99\", \"Chromium\";v=\"87\""));
            //requestHeaders.Add(new RequestHeader("sec-ch-ua-mobile", "?0"));
            //requestHeaders.Add(new RequestHeader("Upgrade-Insecure-Requests", "1"));
            //requestHeaders.Add(new RequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36"));
            //requestHeaders.Add(new RequestHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9"));
            //requestHeaders.Add(new RequestHeader("Sec-Fetch-Site", "none"));
            //requestHeaders.Add(new RequestHeader("Sec-Fetch-Mode", "navigate"));
            //requestHeaders.Add(new RequestHeader("Sec-Fetch-Dest", "document"));
            //requestHeaders.Add(new RequestHeader("Accept-Encoding", "gzip, deflate, br"));
            //requestHeaders.Add(new RequestHeader("Accept-Language", "cs-CZ,cs;q=0.9,en;q=0.8"));

            //byte[] responseBytes = null;
            ////wr.HttpGet("https://outlook.office.com/owa/service.svc/s/GetPersonaPhoto?email=kamil.sykora%40otis.com&UA=0&size=HR240x240", cc, requestHeaders, out tmpCc1000, out responseHeaders, out strResponse, out returnCode);
            //wr.HttpGet("https://outlook.office.com/owa/service.svc/s/GetPersonaPhoto?email=dusan.ondirko%40otis.com&UA=0&size=HR240x240", cc, requestHeaders, out tmpCc1000, out responseHeaders, out responseBytes, out returnCode);
            //cc = wr.JoinCookies(cc, tmpCc1000);

            //byte[] imgByte = null;
            //if (returnCode == HttpStatusCode.OK) {
            //    imgByte = responseBytes;
            //}

            //File.WriteAllBytes(@"c:\temp\outlookphoto.bmp", imgByte);

            //#endregion

            return cc;
        }

        private string GetHeaderValue(List<RequestHeader> headers, string strKey) {
            if (headers == null) {
                return null;
            }

            foreach (var header in headers) {
                if (header.Name.Trim().ToLower() == strKey.Trim().ToLower()) {
                    return header.Value;
                }
            }

            return null;
        }

        //private CookieCollection LoginToOwa() {
        //    RegetWebRequest wr = new RegetWebRequest();
        //    CookieCollection cc = new CookieCollection();
        //    //string strRet;
        //    byte[] byteResponse;
        //    HttpStatusCode returnCode;

        //    //Load Logon Page
        //    CookieCollection tmpCc10;
        //    wr.LoadPage("https://owa.utc.com/owa", cc, out tmpCc10, out byteResponse, out returnCode);
        //    cc = wr.JoinCookies(cc, tmpCc10);

        //    CookieCollection tmpCc20;
        //    wr.LoadPage("https://owa.utc.com/owa/auth/logon.aspx?url=https%3a%2f%2fowa.utc.com%2fowa", cc, out tmpCc20, out byteResponse, out returnCode);
        //    //wr.LoadPage("https://owa.utc.com/owa/auth/logon.aspx?url=https%3a%2f%2fowa.utc.com%2fowa%2fservice.svc%2fs%2fGetPersonaPhoto&amp;reason=0", cc, out tmpCc20, out strRet);
        //    cc = wr.JoinCookies(cc, tmpCc20);

        //    cc.Add(new Cookie("PrivateComputer", "1"));
        //    cc.Add(new Cookie("PBack", "0"));

        //    CookieCollection tmpCc30;
        //    wr.LoadPage("https://owa.utc.com/owa/auth/logon.aspx?replaceCurrent=1&url=https%3a%2f%2fowa.utc.com%2fowa", cc, out tmpCc30, out byteResponse, out returnCode);
        //    //wr.LoadPage("https://owa.utc.com/owa/auth/logon.aspx?replaceCurrent=1&url=https%3a%2f%2fowa.utc.com%2fowa%2fservice.svc%2fs%2fGetPersonaPhoto", cc, out tmpCc30, out strRet);
        //    cc = wr.JoinCookies(cc, tmpCc30);

        //    //Send Credentials

        //    string owaUserName = System.Configuration.ConfigurationManager.AppSettings["owaUserName"];
        //    string owaUserPwd = System.Configuration.ConfigurationManager.AppSettings["owaUserPwd"];

        //    string url = "https://owa.utc.com/owa/auth.owa";
        //    string strFormFields =
        //        "destination=https://owa.utc.com/owa" +
        //        //"destination=https://owa.utc.com/owa/service.svc/s/GetPersonaPhoto" +
        //        "&" + "flags=4" +
        //        "&" + "forcedownlevel=0" +
        //        "&" + "username=" + owaUserName +
        //        "&" + "password=" + owaUserPwd +
        //        "&" + "passwordText=" +
        //        "&" + "trusted=4" +
        //        "&" + "isUtf8=1";

        //    CookieCollection tmpCc40;
        //    string strResponse;
        //    HttpWebResponse httpWebResponse;

        //    wr.PostForm(
        //        url,
        //        strFormFields,
        //        cc,
        //        null,
        //        false,
        //        out tmpCc40,
        //        out strResponse,
        //        out httpWebResponse);
        //    cc = wr.JoinCookies(cc, tmpCc40);
        //    if (strResponse.Contains("https://owa.utc.com/owa/auth/logon.aspx")) {
        //        //logon failed
        //        throw new Exception("Logon to " + url + " failed");
        //    }

        //    return cc;
        //}

    }
}
