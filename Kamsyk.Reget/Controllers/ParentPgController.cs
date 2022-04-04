using Kamsyk.Reget.Content.Sessions;
using Kamsyk.Reget.Controllers.RegetExceptions;
using Kamsyk.Reget.Model;
using Kamsyk.Reget.Model.Common;
using Kamsyk.Reget.Model.ExtendedModel;
using Kamsyk.Reget.Model.ExtendedModel.HttpResult;
using Kamsyk.Reget.Model.ExtendedModel.PurchaseGroup;
using Kamsyk.Reget.Model.Repositories;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;

namespace Kamsyk.Reget.Controllers {
    public class ParentPgController : BaseController {
        #region Properties
        //private const string PPG_IS_USED = "ppg_is_used";
        #endregion

        #region Virtual Properties
        public override string HeaderImgUrl {
            get {
                return GetRootUrl() + "/Content/Images/Controll/Commodity.png";
            }
        }

        public override string HeaderTitle {
            get {
                return RequestResource.ParentPg;
            }
        }
        #endregion

        #region Views
        public ActionResult UsedPg(int? parentPgIg) {
            ViewBag.AdminCompaniesIds = CurrentUser.ParticipantAdminCompanyIds;
            ViewBag.HeaderImgUrl = GetRootUrl() + "/Content/Images/Controll/Commodity.png";
            ViewBag.HeaderTitle = RequestResource.PurchaseGroup;
            if (parentPgIg != null) {
                ViewBag.ParentPgId = parentPgIg;
            }
            return View();
        }
        #endregion

        #region Http Get
        [HttpGet]
        public ActionResult SetMissingParentPgLocalText(int ppgIndex) {
            try {
                HttpResult httpResult = new HttpResult();
                if (Session[RegetSession.SES_PARENT_PG_LIST] == null) {
                    return GetJson(httpResult); 
                }
                List<Parent_Purchase_Group> ppgs = (List<Parent_Purchase_Group>)Session[RegetSession.SES_PARENT_PG_LIST];
                if (ppgs.Count - 1 < ppgIndex) {
                    return GetJson(httpResult);
                }

                new ParentPgRepository().SetMissingLocalText(ppgs.ElementAt(ppgIndex), CurrentCultureCode);
                
                return GetJson(httpResult);
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);
            }
        }

        [HttpGet]
        public ActionResult GetMissingParentPgLocalTextCount() {
            try {
                List<Parent_Purchase_Group> ppgs;
                int iCount = new ParentPgRepository().GetMissingLocalTextCount(CurrentCultureCode, out ppgs);
                Session[RegetSession.SES_PARENT_PG_LIST] = ppgs;

                HttpResult httpResult = new HttpResult();
                httpResult.int_value = iCount;
                return GetJson(httpResult);
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);
            }
        }

        [HttpGet]
        public ActionResult GetMissingPgLocalTextCount() {
            try {
                List<Purchase_Group> ppgs;
                int iCount = new PgRepository().GetMissingLocalTextCount(CurrentCultureCode, out ppgs);
                Session[RegetSession.SES_PG_LIST] = ppgs;

                HttpResult httpResult = new HttpResult();
                httpResult.int_value = iCount;
                return GetJson(httpResult);
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);
            }
        }

        [HttpGet]
        public ActionResult SetMissingPgLocalText(int ppgIndex) {
            try {
                HttpResult httpResult = new HttpResult();
                if (Session[RegetSession.SES_PG_LIST] == null) {
                    return GetJson(httpResult);
                }
                List<Purchase_Group> pgs = (List<Purchase_Group>)Session[RegetSession.SES_PG_LIST];
                if (pgs.Count - 1 < ppgIndex) {
                    return GetJson(httpResult);
                }

                new PgRepository().SetMissingLocalText(pgs.ElementAt(ppgIndex), CurrentCultureCode);
                return GetJson(httpResult);
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);
            }
        }

        //new PgRepository().SetMissingLocalText(CurrentCultureCode);


        [HttpGet]
        public ActionResult GetParentPgsAdmin(string filter, string sort, int pageSize, int currentPage) {
            if (!CurrentUser.IsParticipantAppAdmin && !CurrentUser.IsParticipantCompanyAdmin) {
                throw new ExNotAuthorizedUpdateUser("Not authorized to edit Parent Purchases");
            }

            try {
                //new ParentPgRepository().SetMissingLocalText(CurrentCultureCode);

                string decFilter = DecodeUrl(filter);

                //List<int> compIds = CurrentUser.ParticipantAdminCompanyIds;
                int companyGrouId = CurrentUser.Participant.company_group_id;

                int rowCount;
                var parentPgs = new ParentPgRepository().GetParentPgAdminData(
                    companyGrouId,
                    decFilter,
                    sort,
                    pageSize,
                    currentPage,
                    CurrentCultureCode,
                    CurrentUser.ParticipantId,
                    GetRootUrl(),
                    out rowCount);

                PartData<Object> ppg = new PartData<object>();
                ppg.db_data = parentPgs;
                ppg.rows_count = rowCount;

                return GetJson(ppg);
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);
            }
        }

        [HttpGet]
        public ActionResult GetParentPgs() {
            if (!CurrentUser.IsParticipantAppAdmin && !CurrentUser.IsParticipantCompanyAdmin) {
                throw new ExNotAuthorizedUpdateUser("Not authorized to edit Parent Purchases");
            }

            try {
                List<int> compIds = CurrentUser.ParticipantAdminCompanyIds;

                List<ParentPgExtended> parentPgs = new ParentPgRepository().GetParentPgAdminData(compIds, CurrentCultureCode);

                return GetJson(parentPgs);
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);
            }
        }

        //[HttpGet]
        //public ActionResult GetParentPgCommodities(int parentPgId) {
        //    if (!CurrentUser.IsParticipantAppAdmin && !CurrentUser.IsParticipantCompanyAdmin) {
        //        throw new ExNotAuthorizedUpdateUser("Not authorized to edit Parent Purchases");
        //    }

        //    try {
        //        List<int> compIds = CurrentUser.ParticipantAdminCompanyIds;

        //        List<ParentPgCompanyPgList> companiesPgList = new ParentPgRepository().GetPgsByCompIdParentPgIdJs(parentPgId, compIds, CurrentCultureCode);


        //        return GetJson(companiesPgList);
        //    } catch (Exception ex) {
        //        HandleError(ex);
        //        Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        //        return Content(ex.Message, MediaTypeNames.Text.Plain);
        //    }
        //}

        [HttpGet]
        public ActionResult GetUsedParentPgs(string filter, string sort, int pageSize, int currentPage, int? parentPgId) {
            if (!CurrentUser.IsParticipantAppAdmin && !CurrentUser.IsParticipantCompanyAdmin) {
                throw new ExNotAuthorizedUpdateUser("Not authorized to edit Parent Purchases");
            }

            try {
                //new PgRepository().SetMissingLocalText(CurrentCultureCode);

                List<int> compIds = CurrentUser.ParticipantAdminCompanyIds;

                int rowCount;
                List<UsedPg> companiesPgList = new PgRepository().GetUsedPgJs(
                    compIds,
                    CurrentCultureCode,
                    filter,
                    sort,
                    pageSize,
                    currentPage,
                    GetRootUrl(),
                    parentPgId,
                    out rowCount);

                PartData<UsedPg> pgd = new PartData<UsedPg>();
                pgd.db_data = companiesPgList;
                pgd.rows_count = rowCount;

                return GetJson(pgd);


            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);
            }
        }

        [HttpGet]
        public ActionResult GetMaxCompanyId() {
            try {
                int compId = new CompanyRepository().GetMaxCompanyId();

                return GetJson(compId);
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);
            }
        }

        [HttpGet]
        public ActionResult IsCanParentPgCanBeDeleted(int ppgId) {
            try {
                var userCompanies = GetParentgUsedInCompanies(ppgId);
                HttpResult httpResult = new HttpResult();
                //httpResult.string_list_value = userCompanies;

                return GetJson(userCompanies);

            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);
            }
        }
        #endregion

        #region HttpPost
        [HttpPost]
        public ActionResult SaveParentPgData(ParentPgExtended parentPg) {

            try {
                if (!CurrentUser.IsParticipantAppAdmin && !CurrentUser.IsParticipantCompanyAdmin) {
                    throw new ExNotAuthorizedUpdateUser("Not authorized to edit Parent Purchases");
                }

                List<string> msgItems;
                int ppgId = new ParentPgRepository().SaveParentPg(
                    parentPg, 
                    CurrentUser.ParticipantId,
                    CurrentCultureCode, 
                    out msgItems);

                HttpResult httpResult = new HttpResult();
                httpResult.int_value = ppgId;
                httpResult.string_value = GetErrorMsg(msgItems);

                return GetJson(httpResult);
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpPost]
        public ActionResult DeleteParentPg(Parent_Purchase_Group ppg) {
            try {
                if (!CurrentUser.IsParticipantAppAdmin && !CurrentUser.IsParticipantCompanyAdmin) {
                    throw new ExNotAuthorizedUpdateUser("Not authorized to edit Parent Purchases");
                }

                var userCompanies = GetParentgUsedInCompanies(ppg.id);
                if (userCompanies != null && userCompanies.Count > 0) {
                    throw new Exception("Parent Purchase Group cannot be deleted, it is used");
                }

                ParentPgRepository ppgRepository = new ParentPgRepository();

                ppgRepository.DeleteParentPg(ppg.id);

                HttpResult httpResult = new HttpResult();
                httpResult.string_value = null;

                return GetJson(httpResult);

            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }


        #endregion

        #region Methods
        public ActionResult SaveUsedPgData(UsedPg usedPg) {

            try {
                if (!CurrentUser.IsParticipantAppAdmin && !CurrentUser.IsParticipantCompanyAdmin) {
                    throw new ExNotAuthorizedUpdateUser("Not authorized to edit Parent Purchases");
                }

                var msgItems = new ParentPgRepository().SaveUsedPgData(usedPg, CurrentCultureCode);

                HttpResult httpResult = new HttpResult();
                httpResult.string_value = GetErrorMsg(msgItems);

                return GetJson(httpResult);
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }

        }

        public ActionResult SaveParentPgTranslation(int parentPgId, List<LocalText> localTexts) {
            try {
                if (!CurrentUser.IsParticipantAppAdmin && !CurrentUser.IsParticipantCompanyAdmin) {
                    throw new ExNotAuthorizedUpdateUser("Not authorized to edit Parent Purchases");
                }

                var msgItems = new ParentPgRepository().SaveParentPgTranslation(parentPgId, localTexts);

                HttpResult httpResult = new HttpResult();
                httpResult.string_value = GetErrorMsg(msgItems);

                return GetJson(httpResult);
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        private List<string> GetParentgUsedInCompanies(int ppgId) {

            ParentPgRepository ppgRepository = new ParentPgRepository();
            var tPpg = ppgRepository.GetParentPgById(ppgId);

            List<string> userCompanies = null;
            if (tPpg.Company != null && tPpg.Company.Count > 0) {
                userCompanies = new List<string>();
                foreach (var comp in tPpg.Company) {
                    string compName = comp.country_code;
                    if (!userCompanies.Contains(compName)) {
                        userCompanies.Add(compName);
                    }
                }
            }

            return userCompanies;
        }
        #endregion
    }
}