
using Newtonsoft.Json;
using Kamsyk.Reget.Content.Sessions;
using Kamsyk.Reget.Controllers;
using System;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Kamsyk.Reget.Controllers.RegetExceptions;

namespace Kamsyk.Reget
{
    public class MvcApplication : System.Web.HttpApplication
    {
        #region Properties
        //private Participants m_Participant = null;
        //private Participants Participant {
        //    get {
        //        if (m_Participant == null) {
        //            if (Session[RegetSession.SES_USER_MODEL] == null) {
        //                string httpUserName = HttpContext.Current.User.Identity.Name;
        //                string[] userItems = httpUserName.Split('\\');
        //                string userName = userItems[1];
        //                UserRepository userRepository = new UserRepository();
        //                Participants participant = userRepository.GetParticipantByUserName(userName);
        //                Session[RegetSession.SES_USER_MODEL] = participant;
        //            }

        //            m_Participant = (Participants)Session[RegetSession.SES_USER_MODEL];
        //        }

        //        return m_Participant;
        //    }
        //}
        #endregion

        #region Methods
        //private void SetUser() {
        //    //if (m_Participant == null) {
        //    var context = HttpContext.Current;
        //        if (context.Session[RegetSession.SES_USER_MODEL] == null) {
        //            string httpUserName = HttpContext.Current.User.Identity.Name;
        //            string[] userItems = httpUserName.Split('\\');
        //            string userName = userItems[1];
        //            UserRepository userRepository = new UserRepository();
        //            Participants participant = userRepository.GetParticipantByUserName(userName);
        //            Context.Session[RegetSession.SES_USER_MODEL] = participant;
        //        }

        //        //m_Participant = (Participants)Session[RegetSession.SES_USER_MODEL];
        //    //}
        //}

        private void LogError(Exception ex) {
            try {
                var userName = System.Web.HttpContext.Current.User.Identity.Name;
                BaseController.HandleError(ex, userName);
            } catch { }
        }
        #endregion

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

                        
            ControllerBuilder.Current.SetControllerFactory(new DefaultControllerFactory(new BaseController()));


            //var jsonSerializerSettings = new JsonSerializerSettings {
            //    PreserveReferencesHandling = PreserveReferencesHandling.Objects
            //};

            //GlobalConfiguration.Configuration.Formatters.Clear();
            //GlobalConfiguration.Configuration.Formatters.Add(new JsonNetFormatter(jsonSerializerSettings));
        }



        protected void Application_BeginRequest() {
           
        }

        protected void Application_EndRequest(object sender, EventArgs e) {
            
        }

        void Application_Error(object sender, EventArgs e) {
                        
            // Get last error from the server
            Exception exc = Server.GetLastError();

#if !DEBUG 
            HttpApplication application = (HttpApplication)sender;
            HttpContext context = application.Context;
            //context.Response.Write(exc.ToString());
            HttpContext.Current.Application[RegetSession.SES_EXCEPTION] = exc;
            try {
                //LogError(exc);
                //Response.Redirect(GetRootUrl() + "Home/Error", true);
                //Server.ClearError();
                //Server.Transfer(GetRootUrl() + "Home/Error", true);
                if (exc is ExNotDbConnection) {
#if !TEST
                    BaseController.HandleError(exc, System.Web.HttpContext.Current.User.Identity.Name, false);
#endif
                    Response.Redirect(GetRootUrl() + "Content/Html/RegetError.html?dbconnectionerror=1", true);
                } else if(exc is CultureNotFoundException) {
                    BaseController.HandleError(exc, System.Web.HttpContext.Current.User.Identity.Name, false);
                    Response.Redirect(GetRootUrl() + "Content/Html/RegetError.html", true);
                } else {
                    //LogError(exc);
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    Response.Redirect(String.Format("~/Home/Error"));
                }

            } catch (Exception ex) {
                LogError(ex);
                Server.Transfer(GetRootUrl() + "Content/Html/RegetError.html", true);
            }
#else

                    if (exc != null && exc.Message.ToLower().IndexOf("json") > -1) {
                try { LogError(exc); } catch { };
                HttpApplication application = (HttpApplication)sender;
                HttpContext context = application.Context;
                context.Response.Write(exc.ToString());
                return;
            }

                        
            //context.Session[RegetSession.SES_EXCEPTION] = exc;
            //HttpContext.Current.Session[RegetSession.SES_EXCEPTION] = exc;
            HttpContext.Current.Application[RegetSession.SES_EXCEPTION] = exc;
            try {
                if (exc is ExNotDbConnection) {

                    BaseController.HandleError(exc, System.Web.HttpContext.Current.User.Identity.Name, false);
                    Response.Redirect(GetRootUrl() + "Content/Html/RegetError.html?dbconnectionerror=1", true);
                } else {
                    LogError(exc);
                    //Response.Redirect(GetRootUrl() + "Home/Error", true);
                    //Server.Transfer(GetRootUrl() + "Home/Error", true);
                    //var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    //var url = urlHelper.RouteUrl("Error", "Home");
                    //Server.TransferRequest(url, true);
                    if (!Response.IsRequestBeingRedirected) {
                        Response.Redirect(GetRootUrl() + "Home/Error/");
                    }
                }               
            } catch (Exception ex) {
                LogError(ex);
                if (!Response.IsRequestBeingRedirected) {
                    Response.Redirect(GetRootUrl() + "Content/Html/RegetError.html");
                }
                //Server.Transfer(GetRootUrl() + "Content/Html/RegetError.html", true);
            }

#endif
                }

        private string GetRootUrl() {
            string rootUrl = Request.Url.AbsoluteUri;
            string[] rootUrlParts = rootUrl.Split('/');
            if (rootUrl.ToLower().IndexOf("localhost") > -1) {
                rootUrl = rootUrlParts[0] + "/" + rootUrlParts[1] + "/" + rootUrlParts[2] + "/";
            } else {
                rootUrl = rootUrlParts[0] + "/" + rootUrlParts[1] + "/" + rootUrlParts[2] + "/" + rootUrlParts[3] + "/";
            }

            return rootUrl;
        }

        
    }
}
