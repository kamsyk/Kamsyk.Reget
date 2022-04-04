using Newtonsoft.Json;
using Kamsyk.Reget.Content.Sessions;
using Kamsyk.Reget.Controllers.Interface;
using Kamsyk.Reget.Controllers.RegetExceptions;
using Kamsyk.Reget.Model;
using Kamsyk.Reget.Model.Common;
using Kamsyk.Reget.Model.ExtendedModel;
using Kamsyk.Reget.Model.Repositories;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using static Kamsyk.Reget.Model.Repositories.CentreGroupRepository;
using static Kamsyk.Reget.Model.Repositories.UserRepository;
using Kamsyk.Reget.Model.ExtendedModel.Supplier;
using Kamsyk.Reget.Model.ExtendedModel.HttpResult;

namespace Kamsyk.Reget.Controllers
{
    public class RegetAdminController : BaseController, IRegetAdminController {
        #region Virtual Properties
        public override string HeaderImgUrl {
            get {
                return GetRootUrl() + "/Content/Images/Controll/settings.png";
            }
        }

        public override string HeaderTitle {
            get {
#if TEST
                return "Area Settings";
#else
                return RequestResource.AreaSettings;
#endif
            }
        }
        #endregion

        #region Methods
        public ActionResult CentreGroup(int? cgId) {
            
            if (cgId != null) {
                CentreGroupExtended cg = new CentreGroupRepository().GetCentreGroupDataById((int)cgId, CurrentUser.ParticipantId);
                ViewBag.CgId = cgId;
                ViewBag.CgName = cg.name;

            }

            ViewBag.HeaderTitle = RequestResource.AreaSettings;

            return View("Index");
        }

        public ActionResult NewCentreGroup() {
            if (!(CheckUserAutorization(new List<UserRole> { UserRole.OfficeAdministrator }))) {
                return RedirectToAction("Error", "Home", new { isNotAthorizedPage = 1 });
            }

            ViewBag.IsNew = true;
            ViewBag.HeaderImgUrl = GetRootUrl() + "/Content/Images/Controll/New.png";
            ViewBag.HeaderTitle = RequestResource.NewArea;

            return View("Index");
        }

        public ActionResult AppMatrixExport() {

            return View("AppMatrixExport");
        }

        public ActionResult AppMatrixCopy() {
            ViewBag.HeaderImgUrl = GetRootUrl() + "/Content/Images/Controll/Copy.png";
            ViewBag.HeaderTitle = RequestResource.CopyAppMatrix;

            return View("AppMatrixCopy");
        }

        public ActionResult ApprovalMatrixExport() {
            ViewBag.HeaderImgUrl = GetRootUrl() + "/Content/Images/Controll/ExcelWhite.png";
            ViewBag.HeaderTitle = RequestResource.ApprovalMatrixExport;

            return View("ApprovalMatrixExport");
        }

        public ActionResult MultiplyAppLevel() {
            if (!CurrentUser.IsParticipantCompanyAdmin) {
                throw new ExNotAuthorizedUserPage();
            }

            ViewBag.HeaderImgUrl = GetRootUrl() + "/Content/Images/Controll/Calculator20.png";
            ViewBag.HeaderTitle = RequestResource.MultiplyAppLimit;

            return View("MultiplyAppLevel");
        }

        private bool IsPgValid(PurchaseGroupExtended pg) {
            if (pg.is_active == false) {
                return true;
            }

            if (String.IsNullOrEmpty(pg.group_name)) {
                return false;
            }

            if (pg.is_approval_needed) {
                if (pg.purchase_group_limit == null || pg.purchase_group_limit.Count == 0) {
                    return false;
                }
            }

            //if (pg.requestor == null || pg.requestor.Count == 0) {
            //    return false;
            //}

            // if (!pg.is_approval_only && !pg.self_ordered) {
            if (pg.is_order_needed && !pg.self_ordered) {
                if (pg.orderer == null || pg.orderer.Count == 0) {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region HttpGet, Post
        //***************************************
        //it is important to use try catch, otherwise in release version the error message is not retrieved and it is retrieved user error page
        //****************************************
        [HttpGet]
        public ActionResult GetCentreGroups(bool? isDeactivatedLoaded) {
            try {

                List<CentreGroupAdminList> centreGroups = null;
                if (CurrentUser.IsParticipantAreaAdmin) {
                    List<int> companies = new List<int>();
                    if (CurrentUser.ParticipantAdminCompanyIds != null) {
                        foreach (var compId in CurrentUser.ParticipantAdminCompanyIds) {
                            companies.Add(compId);
                        }
                    }

                    bool isDeactivatedCgLoaded = (isDeactivatedLoaded == true);
                    centreGroups = new CentreGroupRepository().GetCentreGroupsByUserCompanies(CurrentUser.Participant.id, companies, isDeactivatedCgLoaded, GetRootUrl());
                } else {
                    centreGroups = new CentreGroupRepository().GetActiveCentreGroupsByUserId(CurrentUser.Participant.id, GetRootUrl());
                }

                return GetJson(centreGroups.ToList());


            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        //***************************************
        //it is important to use try catch, otherwise in release version the error message is not retrieved and it is retrieved user error page
        //****************************************
        [HttpGet]
        public ActionResult GetAdministratedCentreGroups(bool? isDeactivatedLoaded) {
            try {
                List<int> companies = new List<int>();
                if (CurrentUser.IsParticipantAreaPropAdmin ||
                    CurrentUser.IsParticipantMatrixAdmin ||
                    CurrentUser.IsParticipantOrderAdmin ||
                    CurrentUser.IsParticipantRequestorAdmin) {
                    if (CurrentUser.ParticipantAdminCompanyIds != null) {
                        foreach (var compId in CurrentUser.ParticipantAdminCompanyIds) {
                            companies.Add(compId);
                        }
                    }
                }

                bool isDeactivatedCgLoaded = (isDeactivatedLoaded == true);

                var adminCentreGroups = new CentreGroupRepository().GetAdministaredCentreGroupsByUserId(
                    CurrentUser.Participant.id,
                    companies,
                    isDeactivatedCgLoaded,
                    GetRootUrl());


                return GetJson(adminCentreGroups.ToList());


            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }


        [HttpGet]
        public ActionResult GetCurrencies(int cgId, int currentCurrencyId, int? companyId) {
            try {
                List<CurrencyExtended> currencies = new CentreGroupRepository().GetCurrencyData(cgId, currentCurrencyId, companyId);

                return GetJson(currencies.ToList());
                //JsonResult jsonResult = Json(currencies.ToList(), JsonRequestBehavior.AllowGet);

                //return jsonResult;
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpGet]
        public JsonResult GetActiveCurrencies() {
            List<CurrencyExtended> currencies = new CentreGroupRepository().GetActiveCurrencyData();

            JsonResult jsonResult = Json(currencies.ToList(), JsonRequestBehavior.AllowGet);

            return jsonResult;
        }

        [HttpGet]
        public ActionResult GetActiveCentres() {
            try {
                List<CentreExtended> centres = new CentreGroupRepository().GetActiveCentreData();
                return GetJson(centres.ToList());
                //JsonResult jsonResult = Json(centres.ToList(), JsonRequestBehavior.AllowGet);

                //return jsonResult;
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpGet]
        public ActionResult GetActiveNotAssignedCentresData(int cgId) {
            try {
                List<CentreExtended> centres = new CentreGroupRepository().GetActiveNotAssignedCentresData(cgId);
                return GetJson(centres.ToList());

            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpGet]
        public ActionResult GetActiveCompanyCentresData(int cgId, int officeId) {
            try {

                List<CentreExtended> centres = new CentreGroupRepository().GetActiveCompanyCentresData(cgId, officeId);
                return GetJson(centres.ToList());

            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpGet]
        public ActionResult GetParentPurchaseGroups() {
            try {
                int companyGroupId = CurrentUser.Participant.company_group_id;
                List<Parent_Purchase_Group> centres = new CentreGroupRepository().GetParentPurchaseGroups(companyGroupId, CurrentCultureCode);
                return GetJson(centres.ToList());
                
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpGet]
        public ActionResult GetCgActiveCentres(int cgId) {
            try {
                List<Centre> centres = new CentreGroupRepository().GetCgActiveCentreData(cgId);
                return GetJson(centres.ToList());
                //JsonResult jsonResult = Json(centres.ToList(), JsonRequestBehavior.AllowGet);

                //return jsonResult;
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpGet]
        public ActionResult GetCentreGroupDataById(int cgId) {
            try {
                CentreGroupExtended centreGroup = new CentreGroupRepository().GetCentreGroupDataById(cgId, CurrentUser.ParticipantId);

                if (cgId < 0) {
                    CgAdminsExtended cgAdminsExtended = new CgAdminsExtended();
                    cgAdminsExtended.id = CurrentUser.ParticipantId;
                    cgAdminsExtended.first_name = CurrentUser.Participant.first_name;
                    cgAdminsExtended.surname = CurrentUser.Participant.surname;
                    cgAdminsExtended.is_appmatrix_admin = true;
                    cgAdminsExtended.is_cg_prop_admin = true;
                    cgAdminsExtended.is_orderer_admin = true;
                    cgAdminsExtended.is_requestor_admin = true;
                    cgAdminsExtended.is_company_admin = true;

                    centreGroup.cg_admin = new List<CgAdminsExtended>();
                    centreGroup.cg_admin.Add(cgAdminsExtended);

                }
                
                return GetJson(centreGroup);
                //JsonResult jsonResult = Json(centreGroup, JsonRequestBehavior.AllowGet);

                //return jsonResult;
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpGet]
        public ActionResult GetPurchaseGroupsByCgId(int cgId, int indexFrom, bool? isDeativatedLoaded, int? pgRequestor) {
            try {
                List<PurchaseGroupExtended> retPurchaseGroups = new List<PurchaseGroupExtended>();

                bool isDeactivatedLoad = (isDeativatedLoaded == true);

                CentreGroupRepository.PurchaseGroupRequestor pgReq = PurchaseGroupRequestor.CentreGroupAdmin;
                if (pgRequestor != null) {
                    int iPgReq = (int)pgRequestor;

                    if (iPgReq == (int)CentreGroupRepository.PurchaseGroupRequestor.AppMatrixCopy) {
                        pgReq = PurchaseGroupRequestor.AppMatrixCopy;
                    }
                }

                if (indexFrom > 0) {
                    //Thread.Sleep(100);
                    if (Session[RegetSession.SES_PURCHASE_GROUPS] == null) {
                        Session[RegetSession.SES_PURCHASE_GROUPS] = new CentreGroupRepository().GetCgPurchaseGroupData(
                            cgId,
                            CurrentCultureCode,
                            CurrentUser.IsParticipantAreaAdmin,
                            isDeactivatedLoad,
                            GetRootUrl(),
                            pgReq);
                    }

                    List<PurchaseGroupExtended> purchaseGroups = (List<PurchaseGroupExtended>)Session[RegetSession.SES_PURCHASE_GROUPS];
                    if (purchaseGroups.Count > indexFrom) {
                        retPurchaseGroups.Add(purchaseGroups[indexFrom]);
                       
                    } else {
                        Session[RegetSession.SES_PURCHASE_GROUPS] = null;
                    }
                } else {
                    Session[RegetSession.SES_PURCHASE_GROUPS] = null;
                    //CurrentParticipant.l
                    List<PurchaseGroupExtended> purchaseGroups = new CentreGroupRepository().GetCgPurchaseGroupData(
                        cgId,
                        CurrentCultureCode,
                        CurrentUser.IsParticipantAreaAdmin,
                        isDeactivatedLoad,
                        GetRootUrl(),
                        pgReq);

                    if (purchaseGroups == null) {
                        retPurchaseGroups = new List<PurchaseGroupExtended>();
                    } else {

                        if (purchaseGroups.Count > 25) {
                            Session[RegetSession.SES_PURCHASE_GROUPS] = purchaseGroups;
                            //Must be 26 to oversize the limit 25, it launches following code in regent-angular.js
                            /*
                             if ($scope.PurchaseGroupList.length > 25) {
                                //*** do not delete ****************
                                $scope.PgItemCount = 25;
                                //$scope.LoadTextBkp = $("#spanLoading").html();
                                displayElement('divPgLoading');
                                $scope.getPurchaseGroupsCount();
                                //************************************
                            }
                             */
                            for (int i = 0; i < 26; i++) {
                                retPurchaseGroups.Add(purchaseGroups[i]);
                            }
                        } else {
                            retPurchaseGroups = purchaseGroups;
                        }
                    }
                }

                return GetJson(retPurchaseGroups);
                //JsonResult jsonResult = Json(retPurchaseGroups, JsonRequestBehavior.AllowGet);
                //jsonResult.MaxJsonLength = int.MaxValue;

                //return jsonResult;
            } catch (Exception ex) {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpGet]
        public ActionResult GetPurchaseGroupsCount(int cgId, bool? isDeactivatedLoaded) {
            bool isDeactivatedLoad = (isDeactivatedLoaded == true);
            try {
                if (Session[RegetSession.SES_PURCHASE_GROUPS] == null) {
                    Session[RegetSession.SES_PURCHASE_GROUPS] = new CentreGroupRepository().GetCgPurchaseGroupData(
                        cgId,
                        CurrentCultureCode,
                        CurrentUser.IsParticipantAreaAdmin,
                        isDeactivatedLoad, GetRootUrl(),
                        PurchaseGroupRequestor.AppMatrixCopy);
                }

                List<PurchaseGroupExtended> purchaseGroups = (List<PurchaseGroupExtended>)Session[RegetSession.SES_PURCHASE_GROUPS];

                JsonResult jsonResult = Json(purchaseGroups.Count, JsonRequestBehavior.AllowGet);

                return jsonResult;
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        //[HttpGet]
        ////Not used now
        //public ActionResult GetPurchaseGroupsByCgId(int cgId, int indexFrom) {
        //    try {

        //        List<PurchaseGroupExtended> purchaseGroups = new CentreGroupRepository().GetCgPurchaseGroupData(cgId, CurrentCultureCode);


        //        JsonResult jsonResult = Json(purchaseGroups, JsonRequestBehavior.AllowGet);
        //        jsonResult.MaxJsonLength = int.MaxValue;

        //        return jsonResult;
        //    } catch (Exception ex) {
        //        Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        //        return Content(ex.Message, MediaTypeNames.Text.Plain);

        //    }
        //}

        [HttpGet]
        public ActionResult GetCgAdminRoles(int cgId, int userId) {
            CgAdminRoles cgAdminRoles = new CentreGroupRepository().GetCgAdminRoles(cgId, userId);

            return GetJson(cgAdminRoles);

        }

        [HttpGet]
        public ActionResult IsCentreAssigned(int centreId, int cgId) {
            string isCentrAassigned = new CentreGroupRepository().IsCentreAssigned(centreId, cgId);

            return GetJson(isCentrAassigned);
        }

        [HttpGet]
        public ActionResult GetActiveParticipantsData() {
            try {

                List<ParticipantsExtended> participants = new UserRepository().GetActiveParticipantsData(GetRootUrl(), false, CurrentUser.Participant.Company_Group.id);
                return GetJson(participants.ToList());
                //JsonResult jsonResult = Json(participants.ToList(), JsonRequestBehavior.AllowGet);

                //return jsonResult;
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpGet]
        public ActionResult GetActiveParticipantsDataByName(string name) {
            try {
                var decName = DecodeUrl(name);

                //if (decName == "null" || decName == "undefined") {
                //    decName = null;
                //}

                List<ParticipantsExtended> participants = new UserRepository().GetParticipantByName(
                    decName,
                    GetRootUrl(),
                    CurrentUser.ParticipantAdminCompanyIds,
                    CurrentUser.Participant.Company_Group.id);

                //Thread.Sleep(3000);

                return GetJson(participants.ToList());
                
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpGet]
        public ActionResult GetActiveOfficeData() {
            try {

                List<CompanyCgDropDown> participants = new CentreGroupRepository().GetActiveOfficeCgData(CurrentUser.ParticipantId);
                return GetJson(participants.ToList());

            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpGet]
        public ActionResult GetActiveSuppliers(int supplierGroupId, string searchText) {
            try {
                List<SupplierSimpleExtended> currencies = new CentreGroupRepository().GetActiveSupplierData(supplierGroupId, searchText);
                return GetJson(currencies.ToList());
                //JsonResult jsonResult = Json(currencies.ToList(), JsonRequestBehavior.AllowGet);

                //return jsonResult;
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpGet]
        public JsonResult GetCgCentres(string searchText, bool? isDisabledCgCentresLoaded) {
            bool isDisabled = (isDisabledCgCentresLoaded == true);
            List<CentreCgSimple> centres = new CentreGroupRepository().GetCentres(searchText, CurrentUser.ParticipantAdminCompanyIds, isDisabled);
            //List<CentreCgSimple> centres = new CentreGroupRepository().GetCentres(CurrentUser.ParticipantId, searchText);

            JsonResult jsonResult = Json(centres, JsonRequestBehavior.AllowGet);

            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetCgCentresByCgId(int cgId) {

            List<Centre> centres = new CentreGroupRepository().GetCgActiveCentreData(cgId);

            JsonResult jsonResult = Json(centres, JsonRequestBehavior.AllowGet);

            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetActiveCompanies() {
            List<CompanyDropDown> companies = new CompanyRepository().GetActiveCompaniesByUserId(CurrentUser.ParticipantId, CurrentUser.ParticipantAdminCompanyIds);

            JsonResult jsonResult = Json(companies, JsonRequestBehavior.AllowGet);

            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetCompanyCentreGroups(int companyId) {

            List<CentreGroupSimple> cgs = new CentreGroupRepository().GetCentreGroupsByCompany(companyId);
            JsonResult jsonResult = Json(cgs, JsonRequestBehavior.AllowGet);

            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetCompanyCentreGroupsActiveOnly(int companyId) {

            List<CentreGroupSimple> cgs = new CentreGroupRepository().GetCentreGroupsByCompanyActiveOnly(companyId);
            JsonResult jsonResult = Json(cgs, JsonRequestBehavior.AllowGet);

            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetAppMatrixCetreGroups(int companyId) {
            List<CentreGroupSimple> cgs = new List<CentreGroupSimple>();
            if (CurrentUser.ParticipantAdminCompanyIds != null && CurrentUser.ParticipantAdminCompanyIds.Contains(companyId)) {
                cgs = new CentreGroupRepository().GetCentreGroupsByCompany(companyId);
            } else {
                cgs = new CentreGroupRepository().GetCentreGroupsByCompany(companyId, CurrentUser.ParticipantId);
            }

            JsonResult jsonResult = Json(cgs, JsonRequestBehavior.AllowGet);

            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetUserAdminCompanies() {
            List<CompanyCheckbox> userAdminCompanies = new List<CompanyCheckbox>();
            if (CurrentUser.ParticipantAdminCompanyIds != null) {
                CompanyRepository companyRepository = new CompanyRepository();
                foreach (var compId in CurrentUser.ParticipantAdminCompanyIds) {
                    var company = companyRepository.GetCompanyById(compId);
                    if (company.active == true) {
                        CompanyCheckbox tmp = new CompanyCheckbox();
                        tmp.company_id = company.id;
                        tmp.country_code = company.country_code;

                        userAdminCompanies.Add(tmp);
                    }
                }
            }

            userAdminCompanies = userAdminCompanies.OrderBy(x => x.country_code).ToList();

            JsonResult jsonResult = Json(userAdminCompanies, JsonRequestBehavior.AllowGet);

            return jsonResult;
        }

        [HttpPost]
        public ActionResult SaveCentreGroupData(CentreGroupExtended cg) {
            try {
                ViewBag.IsError = false;
                if (cg.id < 0) {
                    //new CG
                    if (!CurrentUser.IsParticipantAreaPropAdmin) {
                        throw new ExNotAuthorizedUpdateUser("Not authorized to update Centre Group");
                    }
                } else {
                    if (!IsUpdateCentreGroupAllowed(UserRole.CentreGroupPropAdmin, cg.id) &&
                        !CurrentUser.ParticipantAdminCompanyIds.Contains(cg.company_id)) {
                        throw new ExNotAuthorizedUpdateUser("Not authorized to update Centre Group");
                    }
                }

                if (cg == null) {
                    throw new Exception("Centre Group Data Detail is null");
                }


                HttpResult httpResult = new HttpResult();
                CentreGroupRepository centreGroupRepository = new CentreGroupRepository();
                //check the cg name is unique witin the company
                var duplCg = centreGroupRepository.GetCentreGroupsByCompanyCgId(cg.id, cg.name, cg.company_id);
                if (duplCg == null) {
                    httpResult.string_value = null;
                    httpResult.int_value = centreGroupRepository.SaveCentreGroupData(cg);

                } else {
                    httpResult.string_value = String.Format(RequestResource.CentreGroupExist, cg.name);
                    httpResult.int_value = DataNulls.INT_NULL;
                }
                return GetJson(httpResult);
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);
            }
        }

        [HttpPost]
        public ActionResult SavePurchaseGroupData(PurchaseGroupExtended pg) {
            try {
                if (!IsUpdateCentreGroupAllowed(UserRole.ApproveMatrixAdmin, pg.centre_group_id) &&
                    !IsUpdateCentreGroupAllowed(UserRole.OrdererAdmin, pg.centre_group_id) &&
                    !IsUpdateCentreGroupAllowed(UserRole.RequestorAdmin, pg.centre_group_id) &&
                    !IsUpdateCentreGroupAllowed(UserRole.ApproveMatrixAdmin, pg.centre_group_id)) {
                    throw new ExNotAuthorizedUpdateUser("Not authorized to update Purchase Group");
                }

                if (pg == null) {
                    throw new Exception("Purchase Group Data Detail is null");
                }

                if (!IsPgValid(pg)) {
                    throw new Exception("Purchase Group Data is not valid");
                }

                string eventErrMsg;
                new CentreGroupRepository().SavePurchaseGroupData(
                    pg,
                    CurrentUser.Participant.id,
                    CurrentCultureCode,
                    out eventErrMsg);

                if (!String.IsNullOrEmpty(eventErrMsg)) {
                    HandleError(new Exception(eventErrMsg));
                }

                HttpResult httpResult = new HttpResult();
                return GetJson(httpResult);
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);
            }
        }

        [HttpPost]
        public ActionResult AddNewPurchaseGroupData(PurchaseGroupExtended pg) {
            try {
                if (!IsUpdateCentreGroupAllowed(UserRole.ApproveMatrixAdmin, pg.centre_group_id)) {
                    throw new ExNotAuthorizedUpdateUser("Not authorized to update Purchase Group");
                }

                if (pg == null) {
                    throw new Exception("Purchase Group Data Detail is null");
                }

                PurchaseGroupExtended pgNew = new CentreGroupRepository().AddPurchaseGroupData(
                    pg,
                    CurrentUser.Participant.id, 
                    CurrentCultureCode,
                     GetRootUrl());

                return GetJson(pgNew);
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);
            }
        }

        [HttpPost]
        public ActionResult MultiplyAppLevels(List<CompanyCheckbox> companies, decimal multipl, bool isAll) {
            MultiplyLimitResult multiResult = new MultiplyLimitResult();
            try {
                if (companies != null) {
                    foreach (var company in companies) {
                        if (!company.is_selected) {
                            continue;
                        }
                        if (!IsUpdateCentreGroupAllowed(UserRole.OfficeAdministrator, company.company_id)) {
                            throw new ExNotAuthorizedUpdateUser("Not authorized to update Centre Group");
                        }
                    }
                    multiResult = new PgLimitRepository().MultiplyAppLevels(companies, multipl, isAll, CurrentCultureCode);
                    if (multiResult.err_msg != null) {
                        foreach (var err_msg in multiResult.err_msg) {
                            if (err_msg.reason == (int)PgLimitRepository.PgLimitError.BottomGreaterTop) {
                                err_msg.err_msg = RequestResource.LowerLimit + " > " + RequestResource.UpperLimit;
                            } else {
                                err_msg.err_msg = "error";
                            }
                        }
                    }
                        //multiResult.affected_limits_count += tmpMultiResult.affected_limits_count;
                        //if (tmpMultiResult.err_msg != null) {
                        //    foreach (var err_msg in tmpMultiResult.err_msg) {
                        //        MultiplyLimitMsg msg = new MultiplyLimitMsg();
                        //        msg.cgId = err_msg.cgId;
                        //        msg.cgName = err_msg.cgName;
                        //        msg.pgId = err_msg.pgId;
                        //        msg.pgName = err_msg.pgName;
                        //        msg.reason = err_msg.reason;
                        //        multiResult.err_msg.Add(msg);
                        //    }
                        //}
                    }

                //HttpResult httpResult = new HttpResult();
                //httpResult.int_value = limitAffectedCount;
                return GetJson(multiResult);
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);
            }
        }

        //[HttpGet]
        //public ActionResult GetCentresAdminData() {
        //    List<int> compIds = CurrentUser.ParticipantAdminCompanyIds;
        //    var centres = new CentreRepository().GetCentresAdminData(compIds);

        //    return GetJson(centres.ToList());

        //    //JsonResult jsonResult = Json(centres, JsonRequestBehavior.AllowGet);

        //    //return jsonResult;
        //}

        [HttpPost]
        public ActionResult DeletePurchaseGroup(int pgId, int cgId) {
            try {
                if (!IsUpdateCentreGroupAllowed(UserRole.ApproveMatrixAdmin, cgId)) {
                    throw new ExNotAuthorizedUpdateUser("Not authorized to update Purchase Group");
                }

                bool isDeleted = new CentreGroupRepository().DeletePurchaseGroup(pgId, cgId);

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

        [HttpPost]
        public ActionResult DeleteCentreGroup(int cgId) {
            try {
                if (!IsUpdateCentreGroupAllowed(UserRole.CentreGroupPropAdmin, cgId)) {
                    throw new ExNotAuthorizedUpdateUser("Not authorized to delete Centre Group");
                }

                bool isDeleted = new CentreGroupRepository().DeleteCentreGroup(cgId);

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

        [HttpPost]
        public ActionResult CopyAppMatrix(int sourcePgId, bool isNew, int targetCgId) {
            try {
                if (!IsUpdateCentreGroupAllowed(UserRole.ApproveMatrixAdmin, targetCgId)) {
                    throw new ExNotAuthorizedUpdateUser("Not authorized to update Centre Group");
                }

                string eventErrMsg;
                new CentreGroupRepository().CopyAppMatrix(
                    sourcePgId,
                    isNew,
                    targetCgId,
                    CurrentUser.ParticipantId,
                    CurrentCultureCode,
                    GetRootUrl(),
                    out eventErrMsg);

                if (!String.IsNullOrEmpty(eventErrMsg)) {
                    HandleError(new Exception(eventErrMsg));
                }


                HttpResult httpResult = new HttpResult();
                httpResult.int_value = 0;
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