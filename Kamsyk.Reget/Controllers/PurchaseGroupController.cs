using Kamsyk.Reget.Controllers.RegetExceptions;
using Kamsyk.Reget.Model.ExtendedModel;
using Kamsyk.Reget.Model.ExtendedModel.HttpResult;
using Kamsyk.Reget.Model.ExtendedModel.PurchaseGroup;
using Kamsyk.Reget.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using static Kamsyk.Reget.Model.Repositories.UserRepository;

namespace Kamsyk.Reget.Controllers {
    public class PurchaseGroupController : BaseController {
        public ActionResult SavePgTranslation(int pgId, List<LocalText> localTexts) {
            try {
                if (!CurrentUser.IsParticipantAppAdmin && !CurrentUser.IsParticipantCompanyAdmin) {
                    throw new ExNotAuthorizedUpdateUser("Not authorized to edit Parent Purchases");
                }

                var msgItems = new PgRepository().SavePgTranslation(pgId, localTexts);

                HttpResult httpResult = new HttpResult();
                httpResult.string_value = GetErrorMsg(msgItems);

                return GetJson(httpResult);
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpPost]
        public ActionResult DeleteUsedPg(UsedPg usedPg) {

            try {
                if (!IsUpdateCentreGroupAllowed(UserRole.ApproveMatrixAdmin, usedPg.centre_group_id)) {
                    throw new ExNotAuthorizedUpdateUser("Not authorized to update Purchase Group");
                }

                bool isDeleted = new CentreGroupRepository().DeletePurchaseGroup(usedPg.id, usedPg.centre_group_id);

                HttpResult httpResult = new HttpResult();
                if (isDeleted) {
                    httpResult.string_value = "";
                } else {
                    httpResult.string_value = "disabled";
                }
                return GetJson(httpResult);
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);
            }
        }

    }
}