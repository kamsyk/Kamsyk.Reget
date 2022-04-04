using Kamsyk.Reget.Content.Sessions;
using Kamsyk.Reget.Controllers.Interface;
using Kamsyk.Reget.Controllers.RegetExceptions;
using Kamsyk.Reget.Model;
using Kamsyk.Reget.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kamsyk.Reget.Controllers {
    public class HomeController : BaseController {
        
        #region Properties
        private List<Company> m_Companies = null;
        public List<Company> Companies {
            get {
                if (m_Companies == null) {

                    m_Companies = new UserRepository().GetCompanies();
                }

                return m_Companies;
            }
        }
        #endregion

        public override ActionResult Index(int? id) {
            return View();
            //return RedirectToAction("NewRequest", RequestController.ControllerName);
        }


        //public ActionResult About() {
        //    //ViewBag.Message = "Your application description page.";
        //   // ViewBag.NoHeader = true;

        //    return View();
        //}

        public ActionResult AboutHelp() {
            //ViewBag.Message = "Your application description page.";
            // ViewBag.NoHeader = true;

            return View();
        }

        public ActionResult Error(int? isNotAthorizedPage) {
            //ViewBag.IsDebug = (isDebug != null);
            ViewBag.IsError = true;

            if (isNotAthorizedPage != null) {
                ViewBag.Exception = new ExNotAuthorizedUserPage();
            } else {
                ViewBag.Exception = HttpContext.Application[RegetSession.SES_EXCEPTION]; // HttpContext.Session[RegetSession.SES_EXCEPTION];
            }


            //return View("~/Views/Shared/Error.cshtml");

            return View("Error");
            //if (isNotAthorizedPage == null) {
            //    return RedirectToAction("Error", "Home");
            //} else {
            //    return RedirectToAction("Error", "Home", new { isNotAthorizedPage = 1 });
            //}
        }

        public ActionResult NotSupportedBrowser() {
            //return View("Error");
            return View("~/Views/Shared/NotSupportedBrowser.cshtml");
        }
    }
}