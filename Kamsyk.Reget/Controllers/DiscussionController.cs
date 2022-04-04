using Kamsyk.Reget.Model;
using Kamsyk.Reget.Model.ExtendedModel;
using Kamsyk.Reget.Model.ExtendedModel.Discussion;
using Kamsyk.Reget.Model.ExtendedModel.HttpResult;
using Kamsyk.Reget.Model.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;

namespace Kamsyk.Reget.Controllers {
    public class DiscussionController : BaseController {
        #region Http Methods
        [HttpGet]
        public ActionResult GetSubstitutionDiscussion(int substId) {
            try {
                HttpResult httpResult = new HttpResult();
                //Check whether user is authorized to see it
                if (CurrentUser.UserCompaniesIds == null) {
                    httpResult.string_value = MSG_KEY_NOT_AUTHORIZED;
                    return GetJson(httpResult);
                }

                var subst = new SubstitutionRepository().GetSubstitutionById(substId);
                
                bool isAuthorized = false;
                foreach (var compId in CurrentUser.UserCompaniesIds) {
                    if (subst.companies_ids.Contains("," + compId + ",")) {
                        isAuthorized = true;
                        break;
                    }
                }
                if (!isAuthorized) {
                    httpResult.string_value = MSG_KEY_NOT_AUTHORIZED;
                    return GetJson(httpResult);
                }

                var discColors = GetDiscussionColors();

                var discs = new DiscussionRepository().GetDiscussionBySubstitutionId(subst.id);
                DiscussionExtended discussionExtended = GetDiscussion(discs);
                                
                return GetJson(discussionExtended);

            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);
            }
        }

        [HttpPost]
        public ActionResult AddSubstitutiuonRemark(int substId, string remark) {
            try {
                HttpResult httpResult = new HttpResult();
                var subst = new SubstitutionRepository().GetSubstitutionByIdJs(substId, CurrentUser.Participant);
                //Check whether user is authorized to see it
                if (CurrentUser.UserCompaniesIds == null) {
                    httpResult.string_value = MSG_KEY_NOT_AUTHORIZED;
                    return GetJson(httpResult);
                }

                bool isAuthorized = false;
                foreach (var compId in CurrentUser.UserCompaniesIds) {
                    if (subst.companies_ids.Contains("," + compId + ",")) {
                        isAuthorized = true;
                        break;
                    }
                }
                if (!isAuthorized) {
                    httpResult.string_value = MSG_KEY_NOT_AUTHORIZED;
                    return GetJson(httpResult);
                }

                new DiscussionRepository().AddSubstitutionDiscussion(substId, remark, CurrentUser.ParticipantId);
                
                return GetJson(httpResult);
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpPost]
        public ActionResult AddRequestRemark(int requestId, string remark) {
            try {
                HttpResult httpResult = new HttpResult();
               
                //Check whether user is authorized to see it
                if (CurrentUser.UserCompaniesIds == null) {
                    httpResult.string_value = MSG_KEY_NOT_AUTHORIZED;
                    return GetJson(httpResult);
                }

                var request = new RequestRepository().GetRequestEventById(requestId);

                bool isAuthorized = IsRequestAuthorized(request);
                //foreach (var compId in CurrentUser.UserCompaniesIds) {
                //    if (request.Company != null && request.Company.id == compId) {
                //        if (new RequestController().IsAuthorizedByPrivacy(request, CurrentUser.ParticipantId)) {
                //            isAuthorized = true;
                //            break;
                //        }
                //    }
                //}

                if (!isAuthorized) {
                    httpResult.string_value = MSG_KEY_NOT_AUTHORIZED;
                    return GetJson(httpResult);
                }

                new DiscussionRepository().AddRequestDiscussion(requestId, remark, CurrentUser.ParticipantId);

                return GetJson(httpResult);
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }


        [HttpGet]
        public ActionResult GetRequestDiscussion(int requestId) {
            try {
                HttpResult httpResult = new HttpResult();
                //Check whether user is authorized to see it
                if (CurrentUser.UserCompaniesIds == null) {
                    httpResult.string_value = MSG_KEY_NOT_AUTHORIZED;
                    return GetJson(httpResult);
                }
                                
                var request = new RequestRepository().GetRequestEventById(requestId);

                bool isAuthorized = IsRequestAuthorized(request);
                //foreach (var compId in CurrentUser.UserCompaniesIds) {
                //    if (request.Company != null && request.Company.id == compId) {
                //        if (new RequestController().IsAuthorizedByPrivacy(request, CurrentUser.ParticipantId)) {
                //            isAuthorized = true;
                //            break;
                //        }
                //    }
                //}
                if (!isAuthorized) {
                    httpResult.string_value = MSG_KEY_NOT_AUTHORIZED;
                    return GetJson(httpResult);
                }

                List<Discussion> reqDiscussions = new DiscussionRepository().GetDiscussionByRequestId(request.id);
                DiscussionExtended discussionExtended = GetDiscussion(reqDiscussions);

                return GetJson(discussionExtended);

            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);
            }
        }
        #endregion

        #region Methods
        private void SetDiscussionColor(
            DiscussionItemExtended discussionItemExtended, 
            List<DiscussionColor> discColors,
            Hashtable htColor) {

            if (htColor.ContainsKey(discussionItemExtended.author_id)) {
                DiscussionColor col = (DiscussionColor)htColor[discussionItemExtended.author_id];
                discussionItemExtended.bkg_color = col.BkgColor;
                discussionItemExtended.border_color = col.BorderColor;
                discussionItemExtended.user_color = col.UserColor;
            } else {
                int iColorIndex = htColor.Count;
                if (iColorIndex >= discColors.Count) {
                    iColorIndex = 0;
                }

                DiscussionColor col = discColors.ElementAt(iColorIndex);
                discussionItemExtended.bkg_color = col.BkgColor;
                discussionItemExtended.border_color = col.BorderColor;
                discussionItemExtended.user_color = col.UserColor;

                htColor.Add(discussionItemExtended.author_id, col);
            }

          
        }

        private DiscussionExtended GetDiscussion(ICollection<Discussion> discussionItems) {
            var discColors = GetDiscussionColors();

            DiscussionExtended discussionExtended = new DiscussionExtended();

            Hashtable htColor = new Hashtable();
            if (discussionItems != null && discussionItems.Count > 0) {
                discussionExtended.discussion_items = new List<DiscussionItemExtended>();

                foreach (var discItem in discussionItems) {
                    DiscussionItemExtended tmpDiscItem = new DiscussionItemExtended();
                    tmpDiscItem.author_id = discItem.author_id;
                    tmpDiscItem.author_name = discItem.Participants.surname + " " + discItem.Participants.first_name;
                    if (discItem.Participants.ParticipantPhoto != null) {
                        //@participantController.GetRootUrl()Participant/UserPhoto?userId={{angCtrl.userInfo.photo240_url}}
                        tmpDiscItem.author_photo_url = GetRootUrl() + "Participant/UserPhoto?userId=" + discItem.Participants.id.ToString();
                    }
                    tmpDiscItem.disc_text = discItem.App_Text_Store.text_content;
                    tmpDiscItem.author_initials = discItem.Participants.first_name.Substring(0, 1).ToUpper()
                        + discItem.Participants.surname.Substring(0, 1).ToUpper();
                    tmpDiscItem.modif_date_text = discItem.modif_date.ToString(GetShortDateTimeFormat());
                    SetDiscussionColor(tmpDiscItem, discColors, htColor);
                    discussionExtended.discussion_items.Add(tmpDiscItem);
                }
            }

            if (htColor.ContainsKey(CurrentUser.ParticipantId)) {
                DiscussionColor col = (DiscussionColor)htColor[CurrentUser.ParticipantId];
                discussionExtended.discussion_bkg_color = col.BkgColor;
                discussionExtended.discussion_border_color = col.BorderColor;
                discussionExtended.discussion_user_color = col.UserColor;
            } else {
                int colIndex = htColor.Count;
                if (colIndex >= discColors.Count) {
                    colIndex = 0;
                }
                DiscussionColor col = discColors.ElementAt(colIndex);
                discussionExtended.discussion_bkg_color = col.BkgColor;
                discussionExtended.discussion_border_color = col.BorderColor;
                discussionExtended.discussion_user_color = col.UserColor;
            }

            return discussionExtended;
        }

        private bool IsRequestAuthorized(Request_Event request) {
            if (request.requestor == CurrentUser.ParticipantId) {
                return true;
            }
            
            foreach (var compId in CurrentUser.UserCompaniesIds) {
                if (request.Company != null && request.Company.id == compId) {
                    if (new RequestController().IsAuthorizedByPrivacy(request, CurrentUser.ParticipantId)) {
                        return true;
                    }
                }
            }

            return false;
        }
        #endregion
    }
}