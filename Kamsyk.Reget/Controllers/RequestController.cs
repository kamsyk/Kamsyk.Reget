using Kamsyk.Reget.Controllers.Interface;
using Kamsyk.Reget.Model;
using Kamsyk.Reget.Model.Common;
using Kamsyk.Reget.Model.ExtendedModel;
using Kamsyk.Reget.Model.ExtendedModel.Address;
using Kamsyk.Reget.Model.ExtendedModel.Currency;
using Kamsyk.Reget.Model.ExtendedModel.HttpResult;
using Kamsyk.Reget.Model.ExtendedModel.Order;
using Kamsyk.Reget.Model.ExtendedModel.PurchaseGroup;
using Kamsyk.Reget.Model.ExtendedModel.Request;
using Kamsyk.Reget.Model.ExtendedModel.User;
using Kamsyk.Reget.Model.Repositories;
using Kamsyk.Reget.Model.Repositories.Interfaces;
using Kamsyk.Reget.Model.Request.ExtendedModel;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using static Kamsyk.Reget.Model.Repositories.RequestRepository;

namespace Kamsyk.Reget.Controllers
{
    public class RequestController : BaseController {
        #region Constants
        private readonly int VISIBILITY_PRIVATE = 0;
        private readonly int VISIBILITY_CENTRE = 10;
        private readonly int VISIBILITY_PUBLIC = 20;
        #endregion

        #region Virtual Properties
        protected override bool IsGenerateDatePickerLocalization {
            get {
                return true;
            }
        }
        #endregion

        #region Static Properties
        public static string ControllerName {
            get {
                string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

                return GetControllerName(className);
            }
        }

        #endregion

        #region Properties
        private IRequestRepository m_testReqRep = null;
        private IPgRepository m_testPgRep = null;
        #endregion

        #region Constructor
        public RequestController() : base() { }

        public RequestController(
            IRequestRepository testReqRep,
            IPgRepository testPgRep,
            RegetUser currentUser) : base() {
#if TEST
            CurrentUser = currentUser;
#endif
            m_testReqRep = testReqRep;
            m_testPgRep = testPgRep;
        }
        #endregion

        #region Static Methods
        public static bool IsWaitForUserApproval(Request_Event reqEvent, int userId) {
            //if ((reqEvent.request_status == (int)RequestStatus.WaitForAppL1 && reqEvent.manager1 == userId)
            //    || (reqEvent.request_status == (int)RequestStatus.WaitForAppL2 && reqEvent.manager2 == userId)
            //    || (reqEvent.request_status == (int)RequestStatus.WaitForAppL3 && reqEvent.manager3 == userId)
            //    || (reqEvent.request_status == (int)RequestStatus.WaitForAppL4 && reqEvent.manager4 == userId)
            //    || (reqEvent.request_status == (int)RequestStatus.WaitForAppL5 && reqEvent.manager5 == userId)
            //    || (reqEvent.request_status == (int)RequestStatus.WaitForAppL6 && reqEvent.manager6 == userId)) {
            //    return true;
            //}

            if (reqEvent.request_status == (int)RequestStatus.WaitForApproval) {
                return true;
            }

            if (reqEvent.request_status == (int)RequestStatus.WaitForApproval) {
                if (reqEvent.Request_Event_Approval != null && reqEvent.Request_Event_Approval.Count > 0) {
                    var appLevel = (from appLevDb in reqEvent.Request_Event_Approval
                                    where appLevDb.app_man_id == userId
                                    select new { app_man_id = appLevDb.app_man_id }).FirstOrDefault();
                    if (appLevel != null) {
                        return true;
                    }
                }
            }

            return false;
        }
        #endregion
               
        #region Http Get
        [HttpGet]
        public ActionResult GetRequest(int? requestId) {
            //ViewBag.RequestId = requestId;

            HttpRequestResult httpResult = new HttpRequestResult();

            RequestEventExtended requestExtended = null;
            if (requestId == null) {
                requestExtended = new RequestRepository().GetNewRequest(CurrentUser.ParticipantId);
            } else {
                Request_Event re = new RequestRepository().GetRequestEventById((int)requestId);

                if (!IsAuthorizedByPrivacy(re, CurrentUser.ParticipantId)) {
                    httpResult.string_value = MSG_KEY_NOT_AUTHORIZED;
                    httpResult.request_nr = re.request_nr;
                    return GetJson(httpResult);
                }

                requestExtended = new RequestRepository().GetRequestEventByIdJs((int)requestId, CurrentCultureCode, CurrentUser.ParticipantId);
                if (requestExtended != null) {
                    SetCustomFields(requestExtended);
                    SetAttachments(requestExtended);
                    SetPrivacyName(requestExtended);
                    SetApprovalDates(requestExtended.request_event_approval);
                }
            }

            return GetJson(requestExtended);
            
        }

        [HttpGet]
        public ActionResult GetRequestCentres() {
            List<CentreReqDropDownItem> centreList = new List<CentreReqDropDownItem>();

            if (CurrentUser.Participant.Centre_Default != null
                && CurrentUser.Participant.Centre_Default.active == true
                && CurrentUser.Participant.Centre_Default.Centre_Group != null
                && CurrentUser.Participant.Centre_Default.Centre_Group.Count > 0
                && CurrentUser.Participant.Centre_Default.Centre_Group.ElementAt(0).active == true) {
                int centreId = (int)CurrentUser.Participant.Centre_Default.id;
                string centreName = CurrentUser.Participant.Centre_Default.name;
                
                bool isFreeSupplierAllowed = GetIsFreeSupplierAllowed(centreId);
                CentreReqDropDownItem centre = new CentreReqDropDownItem(centreId, centreName, isFreeSupplierAllowed);
                
                centreList.Add(centre);
            }

            if (CurrentUser.Participant.Requestor_Centre != null) {
                var sortCentres = CurrentUser.Participant.Requestor_Centre.OrderBy(x => x.name).ToList();
                foreach (var centre in sortCentres) {
                    if (centre.active == true) {
                        bool isFreeSupplierAllowed = GetIsFreeSupplierAllowed(centre.id);
                        CentreReqDropDownItem reqCentre = new CentreReqDropDownItem(centre.id, centre.name, isFreeSupplierAllowed);
                        centreList.Add(reqCentre);
                    }
                }
            }

            //Thread.Sleep(5000);
            return GetJson(centreList);

        }

        [HttpGet]
        public ActionResult GetPurchaseGroups(int centreId, int requestId) {
            List<PurchaseGroupExtended> pgList = new List<PurchaseGroupExtended>();

            Centre centre = new CentreRepository().GetCentreById(centreId);
            if (centre == null || centre.Centre_Group == null || centre.Centre_Group.Count == 0) {
                return GetJson(pgList);
            }

            Centre_Group cg = centre.Centre_Group.ElementAt(0);
            foreach (var pg in cg.Purchase_Group) {
                if (pg.active == false) {
                    continue;
                }

                var pgReq = (from pgrd in pg.PurchaseGroup_Requestor
                             where pgrd.requestor_id == CurrentUser.ParticipantId
                             select new { id = pgrd.requestor_id }).FirstOrDefault();
                if (pgReq == null) {
                    continue;
                }

                string pgLocName = GetPgLocalName(pg);

                PurchaseGroupExtended tmmpPg = new PurchaseGroupExtended();
                BaseRepository<Purchase_Group>.SetValues(pg, tmmpPg);
                tmmpPg.group_name = pgLocName;

                SetPgLimits(pg, tmmpPg, centreId);
                SetCustomFields(pg, tmmpPg, requestId);
                SetOrderers(pg, tmmpPg);

                pgList.Add(tmmpPg);
            }

            pgList = pgList.OrderBy(x => x.group_name).ToList();

            //PgListCurrency retPgListCurrency =new PgListCurrency();
            //retPgListCurrency.currency_id = 0;
            //retPgListCurrency.pg_list = pgList;

            ////Thread.Sleep(5000);
            return GetJson(pgList);

        }

        [HttpGet]
        public ActionResult GetPurchaseGroup(int pgId, int requestId) {
            PurchaseGroupExtended pg = new PurchaseGroupExtended();

            var tmpPg =  new PgRepository().GetPgById(pgId);
            string pgLocName = GetPgLocalName(tmpPg);

            BaseRepository<Purchase_Group>.SetValues(tmpPg, pg);
            pg.group_name = pgLocName;
            SetCustomFields(tmpPg, pg, requestId);

           
            return GetJson(pg);

        }

        [HttpGet]
        public ActionResult GetCurrencies(int centreId) {
            DropDownCurrencyDefault dropDownCurrencyDefault = new DropDownCurrencyDefault();
            dropDownCurrencyDefault.default_currency_id = -1;

            List<DropDownItem> currList = new List<DropDownItem>();

            Centre centre = new CentreRepository().GetCentreById(centreId);

            if (centre.active == false
                || centre.Centre_Group == null
                || centre.Centre_Group.Count == 0
                || centre.Centre_Group.ElementAt(0).active == false) {
                return null;
            }

            if (centre.Centre_Group.ElementAt(0).Currency != null) {
                DropDownItem agDropDown = new DropDownItem(
                centre.Centre_Group.ElementAt(0).Currency.id,
                centre.Centre_Group.ElementAt(0).Currency.currency_code);
                currList.Add(agDropDown);

                dropDownCurrencyDefault.default_currency_id = centre.Centre_Group.ElementAt(0).Currency.id;
            }

            if (centre.Centre_Group.ElementAt(0).Foreign_Currency != null && centre.Centre_Group.ElementAt(0).Foreign_Currency.Count > 0) {
                foreach (var forCurr in centre.Centre_Group.ElementAt(0).Foreign_Currency) {
                    if (forCurr.active == false) {
                        continue;
                    }

                    var exCurr = (from currDb in currList
                                  where currDb.id == forCurr.id
                                  select currDb).FirstOrDefault();

                    if (exCurr != null) {
                        continue;
                    }

                    DropDownItem agDropDown = new DropDownItem(
                    forCurr.id,
                    forCurr.currency_code);
                    currList.Add(agDropDown);
                }
            }

            currList = currList.OrderBy(x => x.name).ToList();

            dropDownCurrencyDefault.currency_drop_down = currList;

            return GetJson(dropDownCurrencyDefault);

        }

        [HttpGet]
        public ActionResult GetExchangeRates() {
            ExchangeRateRepository exchangeRateRepository = new ExchangeRateRepository();
            var exRates = exchangeRateRepository.GetCrossExchangeRatesJs();

            return GetJson(exRates);

        }

        [HttpGet]
        public ActionResult GetUnitsOfMeasurement() {
            List<DropDownItem> unitList = new List<DropDownItem>();

            var uoms = new UnitRepository().GetActiveUnits();

            if (uoms != null && uoms.Count > 0) {
                foreach (var uom in uoms) {
                    if (uom.active == false) {
                        continue;
                    }

                    DropDownItem agDropDown = new DropDownItem(
                    uom.id,
                    uom.code);
                    unitList.Add(agDropDown);
                }
            }

            unitList = unitList.OrderBy(x => x.name).ToList();

            return GetJson(unitList);

        }

        [HttpGet]
        public ActionResult GetPrivacyItems() {
            List<DropDownItem> privacyList = new List<DropDownItem>();

            var privacies = new PrivacyRepository().GetPrivacies();

            if (privacies != null && privacies.Count > 0) {
                foreach (var privacy in privacies) {

                    string strName = privacy.name;

                    if (privacy.id == VISIBILITY_PRIVATE) {
                        strName = RequestResource.Private;
                    } else if (privacy.id == VISIBILITY_CENTRE) {
                        strName = RequestResource.CentreScope;
                    } else if (privacy.id == VISIBILITY_PUBLIC) {
                        strName = RequestResource.Public;
                    }

                    DropDownItem agDropDown = new DropDownItem(
                    privacy.id,
                    strName);
                    privacyList.Add(agDropDown);
                }
            }

            //unitList = unitList.OrderBy(x => x.name).ToList();

            return GetJson(privacyList);

        }

        [HttpGet]
        public ActionResult GetPgLimits(int pgId) {
            var pgLimits = new PgLimitRepository().GetLimitsByPgIdJs(pgId);

            return GetJson(pgLimits);

        }

        [HttpGet]
        public ActionResult GetCustomFieldsByPgId(int pgId) {
            var custFields = new CustomFieldRepository().GetCustomFieldsByPgIdJs(pgId, CurrentCultureCode);

            JsonResult jsonResult = Json(custFields, JsonRequestBehavior.AllowGet);

            return jsonResult;
        }

        [HttpPost]
        public ActionResult SaveRequestDraft(RequestEventExtended request) {
            HttpRequestResult httpResult = new HttpRequestResult();

            int centreId = -1;
            if (request.request_centre_id != null) {
                centreId = (int)request.request_centre_id;
            }

            RequestRepository requestRepository = new RequestRepository();
            //if (requestRepository.IsRequestorAuthorized(CurrentUser.ParticipantId, centreId)) {
            if (CurrentUser.IsParticipantRequestor) {
                //int requestId;
                //string requestNr;
                new RequestRepository().SaveDraft(request, CurrentUser.ParticipantId, out int requestId, out string requestNr);

                httpResult.request_id = requestId;
                httpResult.request_nr = requestNr;
            } else {
                httpResult.string_value = MSG_KEY_NOT_AUTHORIZED;
            }

            return GetJson(httpResult);
        }

        [HttpPost]
        public ActionResult SendForApproval(RequestEventExtended request) {
            HttpRequestResult httpRequestResult = new HttpRequestResult();

            int userId = DataNulls.INT_NULL;
            if (request.requestor != null) {
                userId = (int)request.requestor;
            }

            int centreId = DataNulls.INT_NULL;
            if (request.request_centre_id != null) {
                centreId = (int)request.request_centre_id;
            }


#if TEST
            if(m_testReqRep != null && m_testReqRep.IsRequestorAuthorized(-1, centreId)) {
#else
            RequestRepository requestRepository = new RequestRepository();
            if (requestRepository.IsRequestorAuthorized(CurrentUser.ParticipantId, centreId)) {
#endif
                if (IsValid(request)) {
                    request.request_status = (int)RequestStatus.WaitForApproval;
                    var firstAppLevel = (from appDb in request.request_event_approval
                                         where appDb.app_level_id == 0
                                         select appDb).FirstOrDefault();
                    if (firstAppLevel != null) {
                        firstAppLevel.approve_status = (int)ApproveStatus.WaitForApproval;
                    }

#if !TEST
                    int requestId = new RequestRepository().Save(request, CurrentUser.ParticipantId);
                    httpRequestResult.request_id = requestId;
                    SendForApprovalMail();
#endif
                } else {
                    httpRequestResult.string_value = MSG_KEY_MISSING_MANDATORY_ITEM;
                }
            } else {
                httpRequestResult.string_value = MSG_KEY_NOT_AUTHORIZED;
            }

            return GetJson(httpRequestResult);
        }

        [HttpGet]
        public ActionResult GetRequestList(string filter, string sort, int pageSize, int pageNr) {
            try {
                string decFilter = GetFilter(filter);
                decFilter = DecodeUrl(decFilter);

                List<int> compIds = CurrentUser.UserCompaniesIds;
                //int rowCount = 0;

                var requestList = new RequestRepository().GetRequestData(
                    compIds,
                    decFilter,
                    sort,
                    pageSize,
                    pageNr,
                    GetRootUrl(),
                    CurrentCultureCode,
                    CurrentUser.ParticipantId,
                    out int rowCount);

                foreach (var request in requestList) {
                    if (request.request_status != null) {
                        request.imgurl = GetRequestStatusImgUrl((int)request.request_status);
                        request.imgstyle = "width:20px;height:20px;border: 1px solid #0288d1;border-radius:10px;";
                        request.request_status_text = GetRequestStatusText((int)request.request_status);
                    }
                }

                PartData<RequestEventExtended> retList = new PartData<RequestEventExtended>();
                retList.db_data = requestList;
                retList.rows_count = rowCount;

                return GetJson(retList);

            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);
            }
        }

        [HttpGet]
        public ActionResult GetRequestorAddress(int centreId) {
            try {
                ShipToAddressRepository shipToAddressRepository = new ShipToAddressRepository();
                List<RequestAddress> addresses = new List<RequestAddress>();

                List<int> lstAddress = new List<int>();

                List<Ship_To_Address> shipToAddresses = shipToAddressRepository.GetShipToAddressByUserCentreId(CurrentUser.ParticipantId, centreId);
                if (shipToAddresses != null && shipToAddresses.Count > 0) {
                    foreach (var shipToAddress in shipToAddresses) {
                        if (lstAddress.Contains(shipToAddress.address_id)) {
                            continue;
                        }
                        lstAddress.Add(shipToAddress.address_id);
                        RequestAddress address = new RequestAddress();
                        BaseRepository<Address>.SetValues(shipToAddress.Address, address);
                        addresses.Add(address);
                    }
                }

                return GetJson(addresses);

            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);
            }
        }

        [HttpGet]
        public ActionResult GetAccessRequest(int requestId) {
            try {
                Request_AccessRequest accessRequest = new RequestAccessRequestRepository().GetAccessRequestByReqIdRequestorIdsJs(requestId, CurrentUser.ParticipantId);
                
                return GetJson(accessRequest);

            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);
            }
        }

        [HttpGet]
        public ActionResult SendAccessRequest(int requestId) {
            HttpResult httpResult = new HttpResult();

            RequestAccessRequestRepository requestAccessRequestRepository = new RequestAccessRequestRepository();
            Request_AccessRequest accessRequest = requestAccessRequestRepository.GetAccessRequestByReqIdRequestorIdsJs(requestId, CurrentUser.ParticipantId);
            if (accessRequest != null && accessRequest.status_id == (int)ApproveStatus.Rejected) {
                httpResult.error_id = HttpResult.NOT_AUTHORIZED_ERROR;
                return GetJson(httpResult);
            }

            if (accessRequest == null) {
                int version = new RequestRepository().GetRequestVersion(requestId);
                requestAccessRequestRepository.AddAccessRequest(requestId, version, CurrentUser.ParticipantId);
            }

            return GetJson(httpResult);
        }

        [HttpGet]
        public ActionResult GetOrder(int requestId) {

            HttpRequestResult httpResult = new HttpRequestResult();
            
            Request_Event re = new RequestRepository().GetRequestEventById((int)requestId);

            if (!IsOrderAuthorized(re, CurrentUser.ParticipantId)) {
                httpResult.string_value = MSG_KEY_NOT_AUTHORIZED;
                httpResult.request_nr = re.request_nr;
                return GetJson(httpResult);
            }

            OrderExtend orderExtend = new OrderRepository().GetOrderByRequestIdJs(requestId);
            orderExtend.requestor_address = ParticipantController.GetUserContact(new UserRepository().GetParticipantById(orderExtend.requestor_id));
            orderExtend.supplier_id = re.supplier_id;

            return GetJson(orderExtend);

        }

        [HttpGet]
        public ActionResult GetOrderLanguages(int requestId) {

            HttpRequestResult httpResult = new HttpRequestResult();
                        
            List<DropDownItem> orderLanguages = new List<DropDownItem>();
            DropDownItem dropDownItem = new DropDownItem(1, RequestResource.LangCZ);
            orderLanguages.Add(dropDownItem);
            dropDownItem = new DropDownItem(2, RequestResource.LangEN);
            orderLanguages.Add(dropDownItem);
            dropDownItem = new DropDownItem(3, RequestResource.LangDE);
            orderLanguages.Add(dropDownItem);


            return GetJson(orderLanguages);

        }

        [HttpPost]
        public ActionResult SetPrivacy(int requestId, int privacyId) {
            HttpRequestResult httpResult = new HttpRequestResult();
                        
            var requestExtended = new RequestRepository().GetRequestEventByIdJs((int)requestId, CurrentCultureCode, CurrentUser.ParticipantId);

            if (requestExtended.requestor != CurrentUser.ParticipantId) {
                httpResult.string_value = MSG_KEY_NOT_AUTHORIZED;
                httpResult.request_nr = requestExtended.request_nr;
                return GetJson(httpResult);
            }

            if (requestExtended.privacy_id != privacyId) {
                requestExtended.privacy_id = privacyId;
                new RequestRepository().Save(requestExtended, CurrentUser.ParticipantId);
            }

            return GetJson(httpResult);
        }

        [HttpPost]
        public ActionResult Revert(int requestId) {
            HttpRequestResult httpResult = new HttpRequestResult();

            RequestRepository requestRepository = new RequestRepository();
            var requestEvent = requestRepository.GetRequestEventById(requestId);
            if (!requestRepository.GetIsRevertable(requestEvent, CurrentUser.ParticipantId)) {
                httpResult.string_value = MSG_KEY_NOT_AUTHORIZED;
                return GetJson(httpResult);
            }

            var prevStatus = requestRepository.GetPreviousStatus(requestEvent, CurrentUser.ParticipantId);
            if (prevStatus == RequestStatus.Unknown) {
                httpResult.string_value = MSG_KEY_REQUEST_CANNOT_BE_REVERTED;
            } else {
                var request = requestRepository.GetRequestEventByIdJs(requestId, CurrentCultureCode, CurrentUser.ParticipantId);
                if (request.request_status == STATUS_CANCELED_ORDERER
                    || request.request_status == STATUS_CANCELED_REQUESTOR
                    || request.request_status == STATUS_CANCELED_SYSTEM) {
                    //no action is needed
                } else {
                    switch (prevStatus) {
                        case RequestStatus.Draft:
                            foreach (var appLevel in request.request_event_approval) {
                                appLevel.approve_status = (int)ApproveStatus.Empty;
                            }
                            break;
                        case RequestStatus.WaitForApproval:
                            var appsDesc = request.request_event_approval.OrderByDescending(x => x.approve_status);
                            bool isRevertingStarted = false;
                            foreach (var appDesc in appsDesc) {
                                if ((appDesc.approve_status == (int)ApproveStatus.Approved
                                    || appDesc.approve_status == (int)ApproveStatus.Rejected)
                                    && appDesc.app_man_id == CurrentUser.ParticipantId) {
                                    appDesc.approve_status = (int)ApproveStatus.WaitForApproval;
                                    isRevertingStarted = true;
                                } else {
                                    if (isRevertingStarted) {
                                        break;
                                    }
                                }
                            }
                            break;
                        case RequestStatus.Approved:
                        case RequestStatus.Rejected:

                            break;
                    }
                }
                request.request_status = (int)prevStatus;
                requestRepository.Save(request, CurrentUser.ParticipantId);
            }

            return GetJson(httpResult);
        }

        [HttpPost]
        public ActionResult CancelRequest(int requestId) {
            HttpRequestResult httpResult = new HttpRequestResult();

            RequestRepository requestRepository = new RequestRepository();
            var requestEvent = requestRepository.GetRequestEventById(requestId);
            if (!requestRepository.GetIsCancelable(requestEvent, CurrentUser.ParticipantId)) {
                httpResult.string_value = MSG_KEY_NOT_AUTHORIZED;
                return GetJson(httpResult);
            }


            var request = requestRepository.GetRequestEventByIdJs(requestId, CurrentCultureCode, CurrentUser.ParticipantId);
            request.request_status = (int)RequestStatus.CanceledRequestor;
            requestRepository.Save(request, CurrentUser.ParticipantId);


            return GetJson(httpResult);
        }

        [HttpPost]
        public ActionResult Approve(int requestId) {
            HttpRequestResult httpResult = new HttpRequestResult();

            RequestRepository requestRepository = new RequestRepository();
            var requestEvent = requestRepository.GetRequestEventById(requestId);
            if (!requestRepository.GetIsApprovable(requestEvent, CurrentUser.ParticipantId)) {
                httpResult.string_value = MSG_KEY_NOT_AUTHORIZED;
                return GetJson(httpResult);
            }

            var requestEventExtended = requestRepository.GetRequestEventByIdJs(requestId, CurrentCultureCode, CurrentUser.ParticipantId);
            requestRepository.Approve(requestEventExtended, CurrentUser.ParticipantId);
                        
            return GetJson(httpResult);
        }

        [HttpPost]
        public ActionResult Reject(int requestId) {
            HttpRequestResult httpResult = new HttpRequestResult();

            RequestRepository requestRepository = new RequestRepository();
            var requestEvent = requestRepository.GetRequestEventById(requestId);
            if (!requestRepository.GetIsApprovable(requestEvent, CurrentUser.ParticipantId)) {
                httpResult.string_value = MSG_KEY_NOT_AUTHORIZED;
                return GetJson(httpResult);
            }

            var requestEventExtended = requestRepository.GetRequestEventByIdJs(requestId, CurrentCultureCode, CurrentUser.ParticipantId);
            requestRepository.Reject(requestEventExtended, CurrentUser.ParticipantId);

            return GetJson(httpResult);
        }
        #endregion

        #region Methods
        private void SetPgLimits(Purchase_Group sourcePg, PurchaseGroupExtended targetPg, int centreId) {
            targetPg.purchase_group_limit = new List<PurchaseGroupLimitExtended>();

            if (sourcePg.Purchase_Group_Limit != null) {
                var pgLimitsSort = sourcePg.Purchase_Group_Limit.OrderBy(x => x.approve_level_id).ToList();
                foreach (var sourcePgLimit in pgLimitsSort) {
                    if (sourcePgLimit.active == false) {
                        continue;
                    }
                    
                    PurchaseGroupLimitExtended targPgLimit = new PurchaseGroupLimitExtended();
                    BaseRepository<Purchase_Group_Limit>.SetValues(sourcePgLimit, targPgLimit);
                    targPgLimit.limit_bottom_text_ro = ConvertData.GetStringValue(sourcePgLimit.limit_bottom, CurrentCultureCode, true);
                    targPgLimit.limit_top_text_ro = ConvertData.GetStringValue(sourcePgLimit.limit_top, CurrentCultureCode, true);
                    targPgLimit.is_top_unlimited = (targPgLimit.limit_top == null);
                    targPgLimit.is_bottom_unlimited = (targPgLimit.limit_bottom == null);
                    

                    targPgLimit.manager_role = new List<ManagerRoleExtended>();
                    if (sourcePgLimit.Manager_Role != null) {
                        foreach (var srcManRole in sourcePgLimit.Manager_Role) {
                            
                            ManagerRoleExtended trgManRole = new ManagerRoleExtended();
                            BaseRepository<Manager_Role>.SetValues(srcManRole, trgManRole);

                            ParticipantsExtended targPart = new ParticipantsExtended();

                            if (srcManRole.Participants != null && srcManRole.Participants.active == true) {
                                var subst = (from partSub in srcManRole.Participants.Participant_Substituted
                                             where partSub.substitute_start_date <= DateTime.Now
                                             && partSub.substitute_end_date > DateTime.Now
                                             && (partSub.approval_status == (int)ApproveStatus.Approved
                                             || partSub.approval_status == (int)ApproveStatus.NotNeeded)
                                             && partSub.active == true
                                             select partSub).FirstOrDefault();

                                if (subst != null) {
                                    BaseRepository<Participants>.SetValues(subst.SubstituteUser, targPart);
                                    trgManRole.participant = targPart;
                                    ParticipantsExtended substPart = new ParticipantsExtended();
                                    BaseRepository<Participants>.SetValues(srcManRole.Participants, substPart);
                                    trgManRole.substituted = substPart;
                                } else {
                                    BaseRepository<Participants>.SetValues(srcManRole.Participants, targPart);
                                    trgManRole.participant = targPart;
                                }
                               
                            }

                            targPgLimit.manager_role.Add(trgManRole);
                        }

                        
                    }

                    targetPg.purchase_group_limit.Add(targPgLimit);
                }

                CheckIfRequestorIsAppMan(targetPg.purchase_group_limit, centreId);
            }
        }

        private void CheckIfRequestorIsAppMan(ICollection<PurchaseGroupLimitExtended> pgLimits, int centreId) {
            //check whether requestor can be app man
            var centre = new CentreRepository().GetCentreById(centreId);
            if (centre.is_approved_by_requestor != true) {
                bool isDeleted = true;
                for (int i = pgLimits.Count- 1; i >= 0; i--) {
                    if (pgLimits.ElementAt(i).manager_role != null) {
                        for (int j = pgLimits.ElementAt(i).manager_role.Count - 1; j >= 0; j--) {
                            if (pgLimits.ElementAt(i).manager_role.ElementAt(j).participant_id == CurrentUser.ParticipantId) {
                                pgLimits.ElementAt(i).manager_role.Remove(pgLimits.ElementAt(i).manager_role.ElementAt(j));
                                
                            }
                        }
                        if (pgLimits.ElementAt(i).manager_role.Count == 0) {
                            pgLimits.Remove(pgLimits.ElementAt(i));
                            isDeleted = true;
                        }
                    }
                    
                }

                if (isDeleted) {
                //    sortAppMen = targPgLimit.manager_role.OrderBy(x => x.approve_level_id).ToList();
                    int iNextAppLevelId = 0;
                //    int iLastAppLevel = -1;
                    for (int i = 0; i < pgLimits.Count; i++) {
                //        if (iLastAppLevel < 0) {
                //            iLastAppLevel = sortAppMen[i].approve_level_id;
                //        }
                        if (pgLimits.ElementAt(i).limit_id > iNextAppLevelId) {
                            pgLimits.ElementAt(i).limit_id = iNextAppLevelId;
                //            sortAppMen[i].approve_level_id = iNextAppLevelId;
                //            if (sortAppMen[i].approve_level_id != iLastAppLevel) {
                //                iLastAppLevel = sortAppMen[i].approve_level_id;
                //                iNextAppLevelId++;
                //            }
                        }
                        iNextAppLevelId++;
                    }
                }
            }
        }

        private bool IsValid(RequestEventExtended request) {
            if (request.requestor == null || request.requestor == DataNulls.INT_NULL) {
                return false;
            }

            if (request.request_centre_id == null) {
                return false;
            }

            if (request.purchase_group_id == null) {
                return false;
            }

            if (request.lead_time == null) {
                return false;
            }

            if (request.estimated_price == null || request.estimated_price == DataNulls.DECIMAL_NULL) {
                return false;
            }

            if (request.currency_id == null || request.currency_id == DataNulls.INT_NULL) {
                return false;
            }

            if (request.use_supplier_list == true) {
                if (request.supplier_id == null || request.supplier_id == DataNulls.INT_NULL) {
                    return false;
                }
            }

#if TEST
            var pg = m_testPgRep.GetPgById((int)request.purchase_group_id);
#else
            var pg = new PgRepository().GetPgById((int)request.purchase_group_id);
#endif
            if (pg.is_approval_needed
                && (request.request_event_approval == null || request.request_event_approval.Count == 0)) {
                return false;
            }

            if (pg.is_order_needed && !pg.is_multi_orderer
                && (request.orderer_id == null || request.orderer_id == DataNulls.INT_NULL)) {
                return false;
            }

            if (request.privacy_id == null) {
                return false;
            }

            return true;
        }

        private void SendForApprovalMail() {

        }

        private void SetCustomFields(Purchase_Group sourcePg, PurchaseGroupExtended targetPg, int requestId) {
            targetPg.custom_field = new List<CustomFieldExtend>();

            CustomFieldRepository customFieldRepository = null;
            if (requestId > -1) {
                customFieldRepository = new CustomFieldRepository();
            }

            if (sourcePg.PurchaseGroup_CustomField != null) {
                foreach (var srcPgCustField in sourcePg.PurchaseGroup_CustomField) {
                   
                    CustomFieldExtend trCustField = new CustomFieldExtend();
                    BaseRepository<CustomFieldExtend>.SetValues(srcPgCustField.Custom_Field, trCustField);

                    //Set Values
                    if (requestId > -1) {
                        //set values for existing custom fields
                        var reCf = customFieldRepository.GetRequestEventCustomFieldsById(requestId, srcPgCustField.custom_field_id);
                        if (reCf != null) {
                            trCustField.string_value = reCf.string_value;
                        }
                    }

                    SetCustomFieldLocal(srcPgCustField, trCustField);
                    targetPg.custom_field.Add(trCustField);
                }
            }
        }

        private void SetCustomFieldLocal(PurchaseGroup_CustomField pgcf, CustomFieldExtend custField) {
            var custLocal = (from locDb in pgcf.Custom_Field.CustomField_Local
                             where locDb.culture == CurrentCultureCode
                             select locDb).FirstOrDefault();

            if (custLocal != null) {
                custField.label = custLocal.local_text;
            }
        }

        private void SetOrderers(Purchase_Group sourcePg, PurchaseGroupExtended targetPg) {
            targetPg.orderer = new List<OrdererExtended>();
            if (sourcePg.PurchaseGroup_Orderer != null) {
                foreach (var purchOrd in sourcePg.PurchaseGroup_Orderer) {
                    OrdererExtended ordererExtended = new OrdererExtended();
                    BaseRepository<Participants>.SetValues(purchOrd, ordererExtended);
                    ordererExtended.participant_id = purchOrd.id;
                    targetPg.orderer.Add(ordererExtended);
                }
            }
        }

        private void SetCustomFields(RequestEventExtended requestExtended) {
            CustomFieldRepository customFieldRepository = new CustomFieldRepository();
            foreach (var custField in requestExtended.custom_fields) {
                var dbCustField = customFieldRepository.GetCustomFieldById(custField.id);
                var custLocal = (from locDb in dbCustField.CustomField_Local
                                 where locDb.culture == CurrentCultureCode
                                 select locDb).FirstOrDefault();
                if (custLocal != null) {
                    custField.label = custLocal.local_text;
                }
            }

        }

        private void SetAttachments(RequestEventExtended requestExtended) {
            foreach (var att in requestExtended.attachments) {
                att.icon_url = AttachmentController.GetFileIconUrl(att.file_name, GetRootUrl());
            }
        }

        private void SetPrivacyName(RequestEventExtended requestExtended) {
            if (requestExtended.privacy_id == null) {
                requestExtended.privacy_name = RequestResource.PrivacyPrivate;
                return;
            }

            var requestPrivacy = PrivacyRepository.GetRequestPrivacy((int)requestExtended.privacy_id);
            switch (requestPrivacy) {
                case PrivacyRepository.RequestPrivacy.Private:
                    requestExtended.privacy_name = RequestResource.PrivacyPrivate;
                    break;
                case PrivacyRepository.RequestPrivacy.Centre:
                    requestExtended.privacy_name = RequestResource.PrivacyCentre;
                    break;
                case PrivacyRepository.RequestPrivacy.Public:
                    requestExtended.privacy_name = RequestResource.PrivacyPublic;
                    break;
                default:
                    requestExtended.privacy_name = RequestResource.PrivacyPrivate;
                    break;
            }
        }

        private void SetApprovalDates(ICollection<RequestEventApprovalExtended> requestEventApprovals) {
            if (requestEventApprovals == null) {
                return;
            }

            foreach (var requestEventApproval in requestEventApprovals) {
                requestEventApproval.modif_date_text = "";
                if (requestEventApproval.modif_date != null) {
                    requestEventApproval.modif_date_text = ((DateTime)requestEventApproval.modif_date).ToString(GetShortDateTimeFormat());
                }
            }
            
        }

        public bool IsAuthorizedByPrivacy(Request_Event request, int participantId) {
            if (request.privacy_id == null) {
                return true;
            }

            if (request.requestor == participantId) {
                return true;
            }

            if (request.Request_Event_Approval != null) {
                foreach (var app in request.Request_Event_Approval) {
                    if (app.app_man_id == participantId) {
                        return true;
                    }
                    
                }
            }
            
            if (request.privacy_id == (int)PrivacyRepository.RequestPrivacy.Public) {
                return true;
            }

            if (request.privacy_id == (int)PrivacyRepository.RequestPrivacy.Centre) {
                var authUser = (from reqAuthDb in request.Request_AuthorizedUsers
                                where reqAuthDb.user_id == participantId
                                select new { user_id = reqAuthDb.user_id }).FirstOrDefault();

                if (authUser != null) {
                    return true;
                }

                if (CurrentUser.Participant.centre_id == request.request_centre_id) {
                    return true;
                }
                var requestor = (from reqCentreDb in CurrentUser.Participant.Requestor_Centre
                                 where reqCentreDb.id == request.request_centre_id
                                 select new { id = reqCentreDb.id }).FirstOrDefault();
                if (requestor != null) {
                    return true;
                }


            }

            if (request.privacy_id == (int)PrivacyRepository.RequestPrivacy.Centre) {
                var authUser = (from reqAuthDb in request.Request_AuthorizedUsers
                                where reqAuthDb.user_id == CurrentUser.Participant.id
                                select new { user_id = reqAuthDb.user_id }).FirstOrDefault();

                if (authUser != null) {
                    return true;
                }
                                
            }

            if (request.Request_AuthorizedUsers != null) {
                foreach (var authUser in request.Request_AuthorizedUsers) {
                    if (authUser.user_id == participantId) {
                        return true;
                    }
                }
            }

            ////substitution
            //request.sub
            //if (request.Request_Event_Approval != null) {
            //    foreach (var app in request.Request_Event_Approval) {
                    

            //    }
            //}

            return false;
        }

        public bool IsOrderAuthorized(Request_Event request, int participantId) {
            if (!IsAuthorizedByPrivacy(request, participantId)) {
                return false;
            }

            if (request.request_status != (int)RequestStatus.Approved
                && request.request_status != (int)RequestStatus.Ordered) {
                return false;
            }

            if (request.orderer_id != participantId) {
                return false;
            }

            return true;
        }

        private string GetRequestStatusImgUrl(int iRequestStatus) {
            switch (iRequestStatus) {
                case (int)RequestStatus.WaitForApproval:
                    return GetRootUrl() + "Content/Images/Request/RequestStatus/WaitForApproval.png";
                case (int)RequestStatus.Approved:
                    return GetRootUrl() + "Content/Images/Request/RequestStatus/Approved.png";
                case (int)RequestStatus.Rejected:
                    return GetRootUrl() + "Content/Images/Request/RequestStatus/Rejected.png";
                case (int)RequestStatus.CanceledRequestor:
                case (int)RequestStatus.CanceledOrderer:
                case (int)RequestStatus.CanceledSystem:
                    return GetRootUrl() + "Content/Images/Request/RequestStatus/Canceled.png";
                default:
                    return GetRootUrl() + "Content/Images/Request/RequestStatus/Draft.png";
            }
        }

        private string GetRequestStatusText(int iRequestStatus) {
            switch (iRequestStatus) {
                case (int)RequestStatus.WaitForApproval:
                    return RequestResource.WaitForApproval;
                case (int)RequestStatus.Approved:
                    return RequestResource.Approved;
                case (int)RequestStatus.CanceledRequestor:
                    return RequestResource.CanceledByRequestor;
                case (int)RequestStatus.CanceledOrderer:
                    return RequestResource.CanceledByOrderer;
                case (int)RequestStatus.CanceledSystem:
                    return RequestResource.CanceledBySystem;
                default:
                    return "";
            }
        }

        public override ActionResult Index(int? id) {
            ViewBag.IsNew = true;

            if (id == null) {
                ViewBag.HeaderImgUrl = GetRootUrl() + "/Content/Images/Menu/New16.png";
                ViewBag.HeaderTitle = RequestResource.NewRequest;
            } else {
                ViewBag.HeaderImgUrl = GetRootUrl() + "/Content/Images/Menu/New16.png";
                ViewBag.HeaderTitle = RequestResource.Request + " " + "{{angCtrl.requestNr}}";
            }

            return View("Index");
        }

        public ActionResult RequestDetail(int id) {
            ViewBag.IsNew = false;

            ViewBag.HeaderImgUrl = GetRootUrl() + "/Content/Images/Controll/New.png";
            ViewBag.HeaderTitle = RequestResource.NewRequest;

            return View("Index");
            //return RedirectToAction("Index");
        }

        //public ActionResult RequestDetail(int requestId) {
        //    ViewBag.IsNew = false;
        //    ViewBag.RequestId = requestId;

        //    //ViewBag.HeaderImgUrl = GetRootUrl() + "/Content/Images/Controll/New.png";
        //    ViewBag.HeaderTitle = RequestResource.Request;

        //    return View("Index");
        //}

        public ActionResult RequestList() {
            ViewBag.IsNew = true;

            ViewBag.HeaderImgUrl = GetRootUrl() + "/Content/Images/Menu/List16.png";
            ViewBag.HeaderTitle = RequestResource.InternalRequestList;

            return View("RequestList");
        }

        public ActionResult Order() {
            ViewBag.IsNew = true;

            ViewBag.HeaderImgUrl = GetRootUrl() + "/Content/Images/Menu/List16.png";
            ViewBag.HeaderTitle = RequestResource.Request + " " + "{{angCtrl.order.request_nr}}" + " - " + RequestResource.Order;

            return View("Order");
        }

        public ActionResult NewRequest(int isError = 0) {
#if TEST
            //Thread.Sleep(5555);
            if (isError == 1) {
                throw new Exception("Dummy Exception for Intergration tests");
            }

#endif

            ViewBag.IsNew = true;
            ViewBag.HeaderImgUrl = GetRootUrl() + "/Content/Images/Controll/New.png";
            ViewBag.HeaderTitle = RequestResource.NewRequest;

            return View("Index");
        }

        private string GetPgLocalName(Purchase_Group pg) {
            var pgl = (from pglDb in pg.Purchase_Group_Local
                       where pglDb.culture.Trim().ToUpper() == CurrentCultureCode.Trim().ToUpper()
                       select pglDb).FirstOrDefault();
            if (pgl != null) {
                return pgl.local_text;
            }

            return pg.group_name;
        }

        private string GetFilter(string filter) {
            if (filter == null) {
                return filter;
            }

            string strFixFilter = "";

            return strFixFilter;
        }

        public RequestStatus GetRequestStatus(int? requestId) {
            if (requestId == null) {
                return RequestStatus.Unknown;
            }
            return new RequestRepository().GetRequestStatus((int)requestId);
        }

        public int GetRequestRequestor(int? requestId, out string requestorName) {
            requestorName = null;
            if (requestId == null) {
                return DataNulls.INT_NULL;
            }
            return new RequestRepository().GetRequestRequestor((int)requestId, out requestorName);
        }

        private bool GetIsFreeSupplierAllowed(int centreId) {
            var dbCentre = new CentreRepository().GetCentreById(centreId);
            if (dbCentre != null && dbCentre.Centre_Group != null && dbCentre.Centre_Group.Count > 0) {
                return (dbCentre.Centre_Group.ElementAt(0).allow_free_supplier == true);
            }

            return false;
        }

        //public bool IsReadOnly
        #endregion
    }
}