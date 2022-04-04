using Kamsyk.Reget.Model;
using Kamsyk.Reget.Model.ExtendedModel;
using Kamsyk.Reget.Model.ExtendedModel.HttpResult;
using Kamsyk.Reget.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;

namespace Kamsyk.Reget.Controllers
{
    public class DataGridController : BaseController {
        #region HttpGet
        [HttpGet]
        public ActionResult GetUserGridSettings(string gridId) {
            try {
                User_GridSetting userGridSetting = new DataGridRepository().GetUserGridSettings(CurrentUser.ParticipantId, gridId);
                
                return GetJson(userGridSetting);

            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }
        #endregion

        #region HttpPost
        [HttpPost]
        public ActionResult SetUserGridSettings(User_GridSetting userGridSettings) {
            try {
                userGridSettings.user_id = CurrentUser.ParticipantId;
                new DataGridRepository().SetUserGridSettings(userGridSettings);

                HttpResult httpResult = new HttpResult();
                                
                httpResult.int_value = 0;
                httpResult.string_value = null;

                return GetJson(httpResult);
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpPost]
        public ActionResult DeleteUserGridSettings(User_GridSetting userGridSettings) {
            try {
                userGridSettings.user_id = CurrentUser.ParticipantId;
                new DataGridRepository().DeleteUserGridSettings(userGridSettings);

                HttpResult httpResult = new HttpResult();

                httpResult.int_value = 0;
                httpResult.string_value = null;

                return GetJson(httpResult);
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }
        #endregion
    }
}