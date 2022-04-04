using Kamsyk.Reget.Controllers.RegetExceptions;
using Kamsyk.Reget.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kamsyk.Reget.Controllers
{
    public class TestIndicatorController : BaseController
    {
        // GET: TestIndicator
        public override ActionResult Index(int? id)
        {
            try {
                ViewBag.TestIndicatorText = new TestIndicatorRepository().GetTestIndicatorText();
            } catch {
                throw new ExTestModeProdDb();
            }

            return View();
        }
    }
}