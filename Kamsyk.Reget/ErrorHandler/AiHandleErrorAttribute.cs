using System;
using System.Web.Mvc;
using Microsoft.ApplicationInsights;
using Kamsyk.Reget.Controllers;

namespace Kamsyk.Reget.ErrorHandler
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)] 
    public class AiHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext != null && filterContext.HttpContext != null && filterContext.Exception != null)
            {
                //If customError is Off, then AI HTTPModule will report the exception
                if (filterContext.HttpContext.IsCustomErrorEnabled)
                {   
                    var ai = new TelemetryClient();
                    ai.TrackException(filterContext.Exception);
                    
                    LogError(filterContext.Exception);
                    
                }
            }
            base.OnException(filterContext);
        }

        private void LogError(Exception ex) {
            var userName = System.Web.HttpContext.Current.User.Identity.Name;

            BaseController.HandleError(ex, userName);
        }
    }
}