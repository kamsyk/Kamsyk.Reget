using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.ScheduledTasks {
    public class RegetWebRequest {
        #region Stuct 
        public struct RequestHeader {
            public string Name;
            public string Value;

            public RequestHeader(string name, string value) {
                Name = name;
                Value = value;
            }
        }
        #endregion

        #region Methods
        //public void LoadPage(string url, CookieCollection cc, out CookieCollection ccOut, out List<RequestHeader> responseHeaders, out byte[] byteResponse, out HttpStatusCode returnCode) {
        //    LoadPage(url, cc, false, out ccOut, out responseHeaders, out byteResponse, out returnCode);
        //}

        public void HttpGet(
            string url, 
            CookieCollection cc,
            List<RequestHeader> requestHeaders,
            out CookieCollection ccOut, 
            out List<RequestHeader> headers,
            //out byte[] byteResponse,
            out string strResponse,
            out HttpStatusCode returnCode) {

            returnCode = HttpStatusCode.InternalServerError;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            SetHeaders(requestHeaders, request);

            request.AllowAutoRedirect = false;

            string strCookies = "";
            if (cc != null) {
                foreach (Cookie c in cc) {
                    if (strCookies.Length > 0) {
                        strCookies += ";";
                    }
                    strCookies += c.Name + "=" + c.Value;
                }
            }
            if (strCookies != null && strCookies.Trim().Length > 0) {
                request.Headers.Add("Cookie", strCookies);
            }
           
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            //string ret = "";
            //byteResponse = null;
            if (response.ContentEncoding == "gzip") {

                Stream responseStream = response.GetResponseStream();
                responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
                StreamReader sr = new StreamReader(responseStream, Encoding.Default);
                strResponse = sr.ReadToEnd();
                
                //BinaryReader breader = new BinaryReader(responseStream);
                //byteResponse = breader.ReadBytes((int)response.ContentLength);

            } else {
                //ret = (new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd());
                BinaryReader breader = new BinaryReader(response.GetResponseStream());
                var byteResponse = breader.ReadBytes((int)response.ContentLength);
                strResponse = System.Text.Encoding.UTF8.GetString(byteResponse);
            }

            returnCode = response.StatusCode;
            //string res = (response.StatusCode.ToString());

            ccOut = GetCookiesFromResponse(response);
            //localition = GetLocationResponse(response);
            headers = GetResponseHeaders(response);

            response.Close();

        }

        public void HttpGet(
            string url,
            CookieCollection cc,
            List<RequestHeader> requestHeaders,
            out CookieCollection ccOut,
            out List<RequestHeader> headers,
            out byte[] byteResponse,
            out HttpStatusCode returnCode) {

            returnCode = HttpStatusCode.InternalServerError;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            SetHeaders(requestHeaders, request);

            request.AllowAutoRedirect = false;

            string strCookies = "";
            if (cc != null) {
                foreach (Cookie c in cc) {
                    if (strCookies.Length > 0) {
                        strCookies += ";";
                    }
                    strCookies += c.Name + "=" + c.Value;
                }
            }
            if (strCookies != null && strCookies.Trim().Length > 0) {
                request.Headers.Add("Cookie", strCookies);
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {

                if (response.ContentEncoding == "gzip") {

                    Stream responseStream = response.GetResponseStream();
                    responseStream = new GZipStream(responseStream, CompressionMode.Decompress);

                    BinaryReader breader = new BinaryReader(responseStream);
                    byteResponse = breader.ReadBytes((int)responseStream.Length);

                } else {
                    //BinaryReader breader = new BinaryReader(response.GetResponseStream());
                    //byteResponse = breader.ReadBytes((int)response.ContentLength);

                    //using (BinaryReader reader = new BinaryReader(response.GetResponseStream())) {
                    //    Byte[] lnByte = reader.ReadBytes(1 * 1024 * 1024 * 10);
                    //    using (FileStream lxFS = new FileStream(@"c:\temp\34891.jpg", FileMode.Create)) {
                    //        lxFS.Write(lnByte, 0, lnByte.Length);
                    //    }
                    //}

                    byte[] lnBuffer;
                    using (BinaryReader lxBR = new BinaryReader(response.GetResponseStream())) {
                        using (MemoryStream lxMS = new MemoryStream()) {
                            lnBuffer = lxBR.ReadBytes(1024);
                            while (lnBuffer.Length > 0) {
                                lxMS.Write(lnBuffer, 0, lnBuffer.Length);
                                lnBuffer = lxBR.ReadBytes(1024);
                            }
                            byteResponse = new byte[(int)lxMS.Length];
                            lxMS.Position = 0;
                            lxMS.Read(byteResponse, 0, byteResponse.Length);
                        }
                    }

                }

                returnCode = response.StatusCode;


                ccOut = GetCookiesFromResponse(response);
                headers = GetResponseHeaders(response);

                response.Close();
            }

        }

        public void HttpPost(
            string url,
            string strFormParams,
            CookieCollection cc,
            List<RequestHeader> requestHeaders,
            string strJson,
            bool isAutoRedirectEnabled,
            out CookieCollection outCc,
            out string strResponse,
            out HttpWebResponse outResponse) {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);


            //request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Win64; x64; Trident/5.0)";
            //request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36";
            //request.ProtocolVersion = new Version(1, 1);

            //if (requestHeaders == null) {
            //    request.ContentType = "application/x-www-form-urlencoded";
            //    request.Accept = "text/html, application/xhtml+xml, */*";
            //    //request.Host = "outlook.office.com";
            //    request.Headers.Add("Accept-Language", "en-US");
            //    request.Headers.Add("Accept-Encoding", "gzip, deflate");
            //    //request.Headers.Add("DNT", "1");
            //    request.Headers.Add("Cache-Control", "no-cache");
            //    ////request.Headers.Add("Connection","Keep-Alive");


            //    request.KeepAlive = true;
            //    request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36";
            //} else { 
            //foreach (RequestHeader requestHeader in requestHeaders) {
            //    if (requestHeader.Name.ToLower() == "content-type") {
            //        request.ContentType = requestHeader.Value;
            //    } else if (requestHeader.Name.ToLower() == "accept") {
            //        request.Accept = requestHeader.Value;
            //    } else if (requestHeader.Name.ToLower() == "host") {
            //        request.Host = requestHeader.Value;
            //    } else if (requestHeader.Name.ToLower() == "connection") {
            //        if (requestHeader.Value.ToLower() == "keep-alive") {
            //            request.KeepAlive = true;
            //        }
            //    } else if (requestHeader.Name.ToLower() == "user-agent") {
            //        request.UserAgent = requestHeader.Value;
            //    } else if (requestHeader.Name.ToLower() == "referer") {
            //        request.Referer = requestHeader.Value;
            //    } else {
            //        request.Headers.Add(requestHeader.Name, requestHeader.Value);
            //    }
            //}

            SetHeaders(requestHeaders, request);

            request.Method = "POST";
            request.AllowAutoRedirect = isAutoRedirectEnabled;
            //}
            
            string strCookies = "";
            if (cc != null) {
                foreach (Cookie c in cc) {
                    if (strCookies.Length > 0) {
                        strCookies += ";";
                    }
                    strCookies += c.Name + "=" + c.Value;
                }
            }
            request.Headers.Add("Cookie", strCookies);

            Stream requestStream = request.GetRequestStream();

            if (!String.IsNullOrWhiteSpace(strFormParams)) {
                byte[] postBytes = Encoding.UTF8.GetBytes(strFormParams);
                requestStream.Write(postBytes, 0, postBytes.Length);
            }

            if (!String.IsNullOrWhiteSpace(strJson)) {
                byte[] postBytes = Encoding.UTF8.GetBytes(strJson);
                requestStream.Write(postBytes, 0, postBytes.Length);
            }

            requestStream.Close();


            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string ret = (new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd());
            string res = (response.StatusCode.ToString());
            outCc = GetCookiesFromResponse(response);
            strResponse = ret;

            outResponse = response;

            //using (StreamWriter writer = new StreamWriter(@"c:\temp\simplesmsfull.txt", true)) {
            //    writer.WriteLine("----------------------------------------------------------------------");
            //    writer.WriteLine("url:" + url);
            //    writer.WriteLine("Form Params:" + strFormParams);
            //    writer.WriteLine("cookies in:");
            //    if (cc != null) {
            //        foreach (Cookie c in cc) {
            //            writer.WriteLine(c.Name + "=" + c.Value + " domain:" + c.Domain);
            //        }
            //    }
            //    writer.WriteLine("cookies out:");
            //    if (outCc != null) {
            //        foreach (Cookie c in outCc) {
            //            writer.WriteLine(c.Name + "=" + c.Value + " domain:" + c.Domain);
            //        }
            //    }

            //    writer.WriteLine("ReturnedText:");
            //    writer.WriteLine(strResponse);

            //    writer.WriteLine("----------------------------------------------------------------------");
            //}
        }

        private CookieCollection GetCookiesFromResponse(HttpWebResponse response) {

            CookieCollection ccOut = new CookieCollection();
            for (int i = 0; i < response.Headers.Keys.Count; i++) {
                if (response.Headers.Keys[i].ToLower() == "set-cookie") {
                    string strOutCookies = response.Headers[i].ToString();
                    //string[] outCookiesArray = strOutCookies.Split(',');
                    //foreach (string outCookie in outCookiesArray) {
                    //    string[] cookiesItems = outCookie.Split(';');
                    //    string[] cookieValues = cookiesItems[0].Split('=');
                    //    if (cookieValues.Length > 1) {
                    //        ccOut.Add(new Cookie(cookieValues[0], cookieValues[1]));
                    //    }
                    //    //Cookie cookie = GetCookieValues(outCookie);
                    //    //ccOut.Add(cookie);
                    //}

                    CookieCollection cookieCollection = GetCookiesFromResponse(strOutCookies);
                    foreach (var cookie in cookieCollection) {
                        ccOut.Add((Cookie)cookie);
                    }
                }
            }

            return ccOut;
        }

        private List<RequestHeader> GetResponseHeaders(HttpWebResponse response) {

            List<RequestHeader> headers = new List<RequestHeader>();

            for (int i = 0; i < response.Headers.Keys.Count; i++) {
                RequestHeader requestHeader = new RequestHeader();
                requestHeader.Name = response.Headers.Keys[i];
                requestHeader.Value = response.Headers[i].ToString();
                headers.Add(requestHeader);
            }

            return headers;
        }

        private CookieCollection GetCookiesFromResponse(string strCookies) {
            CookieCollection cookieCollection = new CookieCollection();

            int pos = 0;
            int iLastEnd = 0;
            while (pos >= 0) {
                pos = strCookies.IndexOf(',', pos);
                if (pos > -1) {
                    if (IsComaCookieDelimiter(strCookies, pos)) {
                        string strCookie = strCookies.Substring(iLastEnd, pos - iLastEnd);
                        iLastEnd = pos + 1;
                        Cookie cookie = GetCookieValues(strCookie);
                        cookieCollection.Add(cookie);
                                     
                    }
                    pos++;
                } else {
                    string strCookie = strCookies.Substring(iLastEnd);
                    Cookie cookie = GetCookieValues(strCookie);
                    cookieCollection.Add(cookie);
                }
                
            }
                        
           
            return cookieCollection;
        }

        private bool IsComaCookieDelimiter(string strCookies, int commaIndex) {
            int posNextSemicolon = strCookies.IndexOf(';', commaIndex + 1);
            int posNextEqual = strCookies.IndexOf('=', commaIndex + 1);

            if (posNextSemicolon < posNextEqual) {
                return false;
            }

            return true;
        }

        private Cookie GetCookieValues(string strSourceCookie) {
            string[] cookiesItems = strSourceCookie.Split(';');

            //name
            string strNamePart = cookiesItems[0];
            int pos = strNamePart.IndexOf("=");
            string strName = strNamePart.Substring(0, pos);
            string strValue = strNamePart.Substring(pos + 1);
                        
            Cookie cookie = new Cookie(strName, strValue);

            ////path
            //pos = cookiesItems[startIndex + 1].IndexOf("=");
            //string strPathName = cookiesItems[startIndex + 1].Substring(0, pos);
            //string strPathValue = cookiesItems[startIndex + 1].Substring(pos);

            //cookie.Path = strPathValue;

            return cookie;
        }

        public CookieCollection JoinCookies(CookieCollection cc1, CookieCollection cc2) {
            CookieCollection cc = new CookieCollection();

            if (cc2 != null) {
                for (int i = 0; i < cc2.Count; i++) {
                    Cookie c = new Cookie(cc2[i].Name, cc2[i].Value);
                    c.Domain = cc2[i].Domain;
                    //if (c.Name == "gPcookie") {
                    //    continue;
                    //}
                    cc.Add(c);
                }
            }

            if (cc1 != null) {
                for (int i = 0; i < cc1.Count; i++) {
                    bool bFound = false;
                    for (int j = 0; j < cc.Count; j++) {
                        if (cc1[i].Name == cc[j].Name) {
                            bFound = true;
                            break;
                        }
                    }

                    if (!bFound) {
                        Cookie c = new Cookie(cc1[i].Name, cc1[i].Value);
                        c.Domain = cc1[i].Domain;
                        //if (c.Name == "gPcookie") {
                        //    continue;
                        //}
                        cc.Add(c);
                    }
                }
            }

            return cc;

        }

        private void SetHeaders(List<RequestHeader> requestHeaders, HttpWebRequest request) {
            foreach (RequestHeader requestHeader in requestHeaders) {
                if (requestHeader.Name.ToLower() == "content-type") {
                    request.ContentType = requestHeader.Value;
                } else if (requestHeader.Name.ToLower() == "accept") {
                    request.Accept = requestHeader.Value;
                } else if (requestHeader.Name.ToLower() == "host") {
                    request.Host = requestHeader.Value;
                } else if (requestHeader.Name.ToLower() == "connection") {
                    if (requestHeader.Value.ToLower() == "keep-alive") {
                        request.KeepAlive = true;
                    }
                } else if (requestHeader.Name.ToLower() == "user-agent") {
                    request.UserAgent = requestHeader.Value;
                } else if (requestHeader.Name.ToLower() == "referer") {
                    request.Referer = requestHeader.Value;
                } else if (requestHeader.Name.ToLower() == "content-length") {
                    continue;
                } else {
                    request.Headers.Add(requestHeader.Name, requestHeader.Value);
                }
            }
        }
        #endregion
    }
}
