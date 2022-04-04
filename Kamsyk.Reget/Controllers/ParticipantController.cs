using Kamsyk.Reget.Controllers.Interface;
using Kamsyk.Reget.Controllers.RegetExceptions;
using Kamsyk.Reget.Mail;
using Kamsyk.Reget.Model;
using Kamsyk.Reget.Model.Common;
using Kamsyk.Reget.Model.ExtendedModel;
using Kamsyk.Reget.Model.ExtendedModel.HttpResult;
using Kamsyk.Reget.Model.ExtendedModel.User;
using Kamsyk.Reget.Model.Repositories;
using Kamsyk.Reget.Model.Repositories.Interfaces;
using OTISCZ.ActiveDirectory;
using Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Resources;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using static Kamsyk.Reget.Model.Repositories.RequestRepository;
using static Kamsyk.Reget.Model.Repositories.UserRepository;

namespace Kamsyk.Reget.Controllers
{
    public class ParticipantController : BaseController, IParticipantController {
        #region Properties
        private ICompanyRepository m_testCompRep = null;
        //private IUserRepository m_testUserRep = null;
        #endregion

        #region Constructor
        public ParticipantController() : base() { }

        public ParticipantController(ICompanyRepository testCompRep, IUserRepository testUserRep) : base(testUserRep) {
            m_testCompRep = testCompRep;
            //m_testUserRep = testUserRep;
        }
        #endregion

        #region Virtual Properties
        public override string HeaderImgUrl {
            get {
                return GetRootUrl() + "/Content/Images/Controll/Users.png";
            }
        }

        public override string HeaderTitle {
            get {
                return RequestResource.Users;
            }
        }

        //private bool m_IsContainsDatePicker = false;
        protected override bool IsGenerateDatePickerLocalization {
            get {
                return true;
            }
            
        }
        #endregion

        #region Views
        public override ActionResult Index(int? id) {
            if (!(CheckUserAutorization(new List<UserRole> { UserRole.OfficeAdministrator }))) {
                return RedirectToAction("Error", "Home", new { isNotAthorizedPage = 1 });
            }

            ViewBag.HeaderImgUrl = GetRootUrl() + "/Content/Images/Controll/Users.png";
            ViewBag.HeaderTitle = RequestResource.Users;

            return View();

        }

        public ActionResult ReplaceUser(int? userToBeReplacedId) {
            if (!(CheckUserAutorization(new List<UserRole> { UserRole.OfficeAdministrator }))) {
                return RedirectToAction("Error", "Home", new { isNotAthorizedPage = 1 });
            }

            ViewBag.HeaderImgUrl = GetRootUrl() + "/Content/Images/Controll/ReplaceUser.png";
            ViewBag.HeaderTitle = RequestResource.ReplaceUser;

            if (userToBeReplacedId != null) {
                Participants partToBeReplaced = new UserRepository().GetParticipantById((int)userToBeReplacedId);
                ViewBag.UserToBeReplacedId = userToBeReplacedId;
                ViewBag.UserToBeReplacedName = partToBeReplaced.surname + " " + partToBeReplaced.first_name;
            }

            return View();
        }

        public ActionResult CopyUser() {
            ViewBag.HeaderImgUrl = GetRootUrl() + "/Content/Images/Controll/UserSubstitution.png";
            ViewBag.HeaderTitle = RequestResource.ManagerSubstitution;

            if (!(CheckUserAutorization(new List<UserRole> { UserRole.OfficeAdministrator }))) {
                return RedirectToAction("Error", "Home", new { isNotAthorizedPage = 1 });
            }


            return View();
        }

        public ActionResult UserInfo(int? userId) {
            ViewBag.HeaderImgUrl = GetRootUrl() + "/Content/Images/Controll/Roles.png";
            ViewBag.HeaderTitle = RequestResource.UserRoles;

            if (userId != null) {
                Participants participant = new UserRepository().GetParticipantById((int)userId);
                ViewBag.ParticipantId = userId;
                ViewBag.ParticipantName = participant.surname + " " + participant.first_name;
                
            }

            return View();
        }

        public ActionResult CurrentUserInfo() {
            ViewBag.HeaderImgUrl = GetRootUrl() + "/Content/Images/Controll/Roles.png";
            ViewBag.HeaderTitle = RequestResource.UserRoles;
            ViewBag.ActionDesc = "CurrentUserInfo";

            int userId = CurrentUser.ParticipantId;
            Participants participant = new UserRepository().GetParticipantById(userId);
            ViewBag.ParticipantId = userId;
            ViewBag.ParticipantName = participant.surname + " " + participant.first_name;


            return View("UserInfo");
        }

        public ActionResult NonActiveUser() {
            ViewBag.HeaderImgUrl = GetRootUrl() + "/Content/Images/Controll/NonActiveUser.png";
            ViewBag.HeaderTitle = RequestResource.NonActiveUser;

            if (!(CheckUserAutorization(new List<UserRole> { UserRole.OfficeAdministrator }))) {
                return RedirectToAction("Error", "Home", new { isNotAthorizedPage = 1 });
            }

            return View();
        }

        public ActionResult UserSubstitution() {
            ViewBag.HeaderImgUrl = GetRootUrl() + "/Content/Images/Controll/UserSubstitution.png";
            ViewBag.HeaderTitle = RequestResource.ManagerSubstitution;

            //if (!(CheckUserAutorization(new List<UserRole> { UserRole.OfficeAdministrator, UserRole.Orderer, UserRole.ApprovalManager }))) {
            //    return RedirectToAction("Error", "Home", new { isNotAthorizedPage = 1 });
            //}

            
            return View();
        }

        public ActionResult UserSubstitutionDetail() {
            ViewBag.HeaderImgUrl = GetRootUrl() + "/Content/Images/Controll/UserSubstitution.png";
            ViewBag.HeaderTitle = RequestResource.ManagerSubstitutionDetail;

            return View();
        }

        public ActionResult UserPhoto(int? userId) {
            //var dir = Server.MapPath("/Images");
            //var path = Path.Combine(dir, id + ".jpg"); //validate the path for security or use other means to generate the path.

            if (userId == null) {
                return null;
            }

            byte[] imgByte = null;
            var userPhotoRepository = new UserPhotoRepository().GetParticipantPhoto((int)userId);
            if (userPhotoRepository != null) {
                imgByte = userPhotoRepository.user_picture_240;
            }
            
            return base.File(imgByte, "image/jpeg");
        }
        #endregion

        #region Static Methods
        public static string GetUserContact(Participants p) {
            string strUseContact = "";

            strUseContact += GetUserNameFirstnameFirst(p);
            if (!String.IsNullOrEmpty(p.email)) {
                strUseContact += Environment.NewLine + RequestResource.Mail + ": " + p.email;
            }

            return strUseContact;
        }
        #endregion

        #region HttpGet
        [HttpGet]
        public ActionResult GetParticipants(string filter, string sort, int pageSize, int currentPage) {
            try {
                string decFilter = DecodeUrl(filter);
                List<int> compIds = CurrentUser.ParticipantAdminCompanyIds;
                
                //Not Needed
                //int companyGroupId = CurrentUser.Participant.company_group_id;
                
                //List<ParticipantsExtended> participants = new UserRepository().GetParticipants(compIds);
                int rowCount;
                List<ParticipantsExtended> participants = new UserRepository().GetParticipantsByFilterPaged(
                    compIds,
                    decFilter,
                    sort,
                    pageSize,
                    currentPage,
                    UserRepository.UserDisplayType.Users,
                    out rowCount);

                PartData<ParticipantsExtended> pd = new PartData<ParticipantsExtended>();
                pd.db_data = participants;
                pd.rows_count = rowCount;

                //return GetJson(participants.ToList());
                return GetJson(pd);

            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpGet]
        public ActionResult GetNonActiveParticipants(string filter, string sort, int pageSize, int currentPage) {
            try {
                string decFilter = DecodeUrl(filter);
                
                List<int> compIds = CurrentUser.ParticipantAdminCompanyIds;
                //not needed
                //int companyGroupId = CurrentUser.Participant.company_group_id;

                //List<ParticipantsExtended> participants = new UserRepository().GetParticipants(compIds);
                int rowCount;
                List<ParticipantsExtended> participants = new UserRepository().GetParticipantsByFilterPaged(
                    compIds,
                    decFilter,
                    sort,
                    pageSize,
                    currentPage,
                    UserRepository.UserDisplayType.NonActiveUsers,
                    out rowCount);

                PartData<ParticipantsExtended> pd = new PartData<ParticipantsExtended>();
                pd.db_data = participants;
                pd.rows_count = rowCount;

                //return GetJson(participants.ToList());
                return GetJson(pd);

            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpGet]
        public ActionResult GetParticipantCompanyByUserName(string userName) {
            try {
                
                Company company = new UserRepository().GetParticipantCompanyByUserName(userName);

                return GetJson(company);
                                
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpGet]
        public ActionResult GetParticipantById(int userId) {
            try {

                ParticipantsExtended participant = new UserRepository().GetParticipantByIdJs(userId, GetRootUrl());

                return GetJson(participant);

            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpGet]
        public ActionResult GetParticipantFromActiveDirectory(string userName, int companyId) {
            try {
#if TEST
                Participants user = new Participants();
                user.first_name = "New";
                user.surname = "Item";
                user.email = "New@Item";
#else

                Participants user = null;

                Hashtable htPerformeCheck = new Hashtable();
                List<Company> companies = new CompanyRepository().GetActiveCompanies();

                if (companyId >= 0) {
                    Company comp = companies.Where(c => c.id == companyId).FirstOrDefault();
                    if (comp != null) {
                        OtActiveDirectory otActiveDirectory = new OtActiveDirectory(comp.active_directory_root);
                        SearchResult sr = otActiveDirectory.GetUserData(userName);

                        if (sr != null) {
                            user = new Participants();

                            user.first_name = OtActiveDirectory.GetResultPropertyValue(sr, "givenName");
                            user.surname = OtActiveDirectory.GetResultPropertyValue(sr, "sn");
                            user.email = OtActiveDirectory.GetResultPropertyValue(sr, "mail");
                        }
                    }
                }
#endif

                return GetJson(user);

            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpGet]
        public ActionResult GetReplaceEntities(int userToBeReplacedId) {
            try {
                ReplaceEntity replaceEntity = new UserRepository().GetReplaceEntity(userToBeReplacedId);

                return GetJson(replaceEntity);

            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);
            }
        }

        [HttpGet]
        public ActionResult GetCompanyAdminActiveParticipantsData() {
            try {
                //CurrentUser.ParticipantAdminCompanyIds
                List<ParticipantsExtended> participants = new UserRepository().GetActiveParticipantsData(
                    GetRootUrl(), 
                    CurrentUser.ParticipantAdminCompanyIds, 
                    false,
                    CurrentUser.Participant.Company_Group.id);
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
        public ActionResult GetUserSubstitution(string filter, string sort, int pageSize, int currentPage) {
            try {
                string decFilter = GetFilter(filter);
                decFilter = DecodeUrl(decFilter);

                List<int> compIds = CurrentUser.UserCompaniesIds;
                //List<int> compIds = CurrentUser.ParticipantCompanyIds;

                if (compIds == null || compIds.Count == 0) {
                    PartData<UserSubstitutionExtended> emptPd = new PartData<UserSubstitutionExtended>();
                    emptPd.db_data = null;
                    emptPd.rows_count = 0;

                    Substitution emptSubst = new Substitution();
                    emptSubst.user_substitution = emptPd;
                    emptSubst.is_approve_visible = false;
                    emptSubst.is_edit_visible = false;

                    return GetJson(emptSubst);
                }

                int rowCount;
                bool isApprovable = false;
                bool isEditable = false;
                List<UserSubstitutionExtended> userSubstitution = new SubstitutionRepository().GetUserSubstitutionData(
                    CurrentUser.ParticipantId,
                    compIds,
                    decFilter,
                    sort,
                    pageSize,
                    currentPage,
                    CurrentUser.ParticipantId,
                    out rowCount,
                    out isApprovable,
                    out isEditable);

                if (userSubstitution != null) {
                    SetApprovalTextStatus(userSubstitution);
                }

                PartData<UserSubstitutionExtended> pd = new PartData<UserSubstitutionExtended>();
                pd.db_data = userSubstitution;
                pd.rows_count = rowCount;

                Substitution subst = new Substitution();
                subst.user_substitution = pd;
                subst.is_approve_visible = isApprovable;
                subst.is_edit_visible = isEditable;

                return GetJson(subst);

            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);
            }
        }

        [HttpGet]
        public ActionResult GetSubstitutedParticipantsData() {
            try {
                List<int> compIds = CurrentUser.ParticipantAdminCompanyIds;
                                
                List<ParticipantsExtended> userSubstitution = new SubstitutionRepository().GetSubstitutedParticipantsData(
                    GetRootUrl(),
                    compIds,
                    CurrentUser.ParticipantId);
                                
                return GetJson(userSubstitution);

            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);
            }
        }

        [HttpGet]
        public ActionResult GetSubstitutedMen(string name) {
            try {
                List<int> adminCompIds = CurrentUser.ParticipantAdminCompanyIds;
                List<int> substManCompIds = CurrentUser.ParticipantSubstManCompanyIds;
                List<int> compIds = new List<int>();
                foreach (int compId in adminCompIds) {
                    if (!compIds.Contains(compId)) {
                        compIds.Add(compId);
                    }
                }
                foreach (int compId in substManCompIds) {
                    if (!compIds.Contains(compId)) {
                        compIds.Add(compId);
                    }
                }

                List<ParticipantsExtended> userSubstitution = new SubstitutionRepository().GetSubstitutedMen(
                    name,
                    GetRootUrl(),
                    compIds);

                return GetJson(userSubstitution);

            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);
            }
        }

        [HttpGet]
        public ActionResult GetSubsitutedApprovers(int substitutedId) {
            try {
               
                List<Participants> participants = new UserRepository().GetSubsitutedApproversJs(substitutedId, CurrentUser.ParticipantId);
                return GetJson(participants);

            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpGet]
        public ActionResult GetUserSubstitutionDetail(int id, string filter=null, string sort=null, string pageSize=null, string currPage = null) {
            try {
                HttpResult httpResult = new HttpResult();
                var subst = new SubstitutionRepository().GetSubstitutionByIdJs(id, CurrentUser.Participant);
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

                //subst.substitute_start_date_text = ConvertData.ToTextFromDate(subst.substitute_start_date);
                subst.substitute_start_date_text = "";
                if (subst.substitute_start_date != null) {
                    subst.substitute_start_date_text = subst.substitute_start_date.ToString(GetShortDateTimeFormat());
                }

                subst.substitute_end_date_text = "";
                if (subst.substitute_end_date != null) {
                    subst.substitute_end_date_text = subst.substitute_end_date.ToString(GetShortDateTimeFormat());
                }

                subst.modified_date_text = "";
                if (subst.modified_date_text != null) {
                    subst.modified_date_text = subst.modified_date.ToString(GetShortDateTimeFormat());
                }

                subst.approval_status_text = GetSubstAppStatusText(subst.approval_status);

                if (subst.active) {
                    if (subst.approval_status == (int)ApproveStatus.Rejected) {
                        subst.active_status_text = RequestResource.Rejected;
                    } else if (subst.approval_status == (int)ApproveStatus.WaitForApproval) {
                        subst.active_status_text = RequestResource.WaitForApproval;
                    } else {
                        subst.active_status_text = RequestResource.Active;
                    }
                } else {
                    subst.active_status_text = RequestResource.NonActive;
                }

                                
                return GetJson(subst);

            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);
            }
        }
       
        #endregion

        #region HttpPost
        [HttpPost]
        public ActionResult  SaveUserData(ParticipantsExtended user) {
            try {
                UserRepository userRepository = new UserRepository();
                int officeId = new CompanyRepository().GetCompanyIdByName(user.office_name);
                if (officeId == DataNulls.INT_NULL) {
                    throw new ExNotAuthorizedUpdateUser("Unknown Company");
                }

                if (!IsUserInOfficeRole(UserRole.OfficeAdministrator, officeId)) {
                    throw new ExNotAuthorizedUpdateUser("Not authorized to update Users");
                }

                if (!IsParticipantValid(user)) {
                    throw new ExMissingMandatoryFields("Missing Mandatory Fields");
                }

                HttpResult httpResult = new HttpResult();

                List<string> msgItems;
                int errorId = 0;
                //bool isNewUser = (user.id < 0);
                int userId = new UserRepository().SaveParticipantData(user, (int)CurrentUser.Participant.company_group_id, out msgItems, out errorId);

                //if (isNewUser) {
                //    Thread t = new Thread(LoadUserPhoto);
                //    t.Start();
                //}

                httpResult.int_value = userId;
                httpResult.string_value = GetErrorMsg(msgItems);

                if (errorId < 0) {
                    httpResult.error_id = errorId;
                }
                                
                return GetJson(httpResult);
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpPost]
        public ActionResult DeleteUser(ParticipantsExtended user) {
            try {
                UserRepository userRepository = new UserRepository();
                int officeId = -1;
                if (m_testCompRep != null) {
                    officeId = m_testCompRep.GetCompanyIdByName(user.office_name);
                } else {
                    officeId = new CompanyRepository().GetCompanyIdByName(user.office_name);
                }

                if (officeId == DataNulls.INT_NULL) {
                    throw new ExNotAuthorizedUpdateUser("Unknown Company");
                }
                
                if (!IsUserInOfficeRole(UserRole.OfficeAdministrator, officeId)) {
                    throw new ExNotAuthorizedUpdateUser("Not authorized to update Users");
                }

                HttpResult httpResult = new HttpResult();

                bool isActiveAppMan = false;
                bool isActiveOrderer = false;
                CheckAppMatrixBeforeDeleteUser(user.id, out isActiveAppMan, out isActiveOrderer);
                              
                if (isActiveAppMan) {
                    httpResult.string_value = user.first_name + " " + user.surname;
                    httpResult.error_id  = USER_CANNOT_BE_DELETED_APPMAN;
                    return GetJson(httpResult);
                }

                if (isActiveOrderer) {
                    httpResult.string_value = user.first_name + " " + user.surname;
                    httpResult.error_id = USER_CANNOT_BE_DELETED_ORDERER;
                    return GetJson(httpResult);
                }

                //if (cgNames == null || cgNames.Count == 0) {
                    if (new UserRepository().DeleteUser(user.id)) {
                        return null;
                    } else {
                        //return "disabled";
                        httpResult.string_value = "disabled";
                        return GetJson(httpResult);
                    }

                //} else {
                    //string strMessage = "";

                    //foreach (string cgName in cgNames) {
                    //    if (strMessage.Length < 100) {
                    //        if (strMessage.Length > 0) {
                    //            strMessage += ", ";
                    //        }

                    //        strMessage += cgName;
                    //    } else {
                    //        strMessage += " ... ";
                    //        //return strMessage;
                    //        httpResult.string_value = strMessage;
                    //        return GetJson(httpResult);
                    //    }
                    //}

                   
                    //httpResult.string_value = GetErrorMsg(cgNames); 
                    //return GetJson(httpResult);
                    //return strMessage;
                //}
            } catch (Exception ex) {
#if TEST
                throw ex;
#endif
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpPost]
        public ActionResult ReplaceAppMan(List<CentreGroupReplace> cge) {
            try {
                CheckReplaceAuthorization(cge);

                new UserRepository().ReplaceAppMan(cge, CurrentUser.ParticipantId);

                return GetJson(new HttpResult());
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpPost]
        public ActionResult SaveUserSubstitution(UserSubstitutionExtended substData) {
            try {
                if (substData.substituted_user_id != CurrentUser.ParticipantId 
                    && substData.author_id != CurrentUser.ParticipantId) {
                    throw new ExNotAuthorizedUpdateUser("Not authorized to update user substitution");
                }

                if (substData.approval_status == (int)ApproveStatus.Approved) {
                    throw new ExNotAuthorizedUpdateUser("Substitution cannot be modified, it was approved");
                }

                if (substData.approval_status == (int)ApproveStatus.Rejected) {
                    throw new ExNotAuthorizedUpdateUser("Substitution cannot be modified, it was rejected");
                }

                SubstitutionRepository substitutionRepository = new SubstitutionRepository();
                if (!IsSubstitutionValid(substData)) {
                    throw new ExMissingMandatoryFields("Missing Mandatory Fields");
                }

                bool isNew = (substData.id < 0);

                List<string> msgItems = null;
                int substId = new SubstitutionRepository().SaveSubstitutionData(
                    substData, 
                    CurrentUser.Participant.id,
                    out msgItems);

                //Send approval request
                if (isNew) {
                    if (substData.approval_status == (int)ApproveStatus.WaitForApproval) {
                        var part = new UserRepository().GetParticipantById(substData.substituted_user_id);
                        var substAppMen = (from compRoleDb in part.Company.Participant_Office_Role
                                           where compRoleDb.role_id == (int)UserRole.SubstitutionApproveManager
                                           select compRoleDb).ToList();
                        List<Participants> recipients = new List<Participants>();
                        foreach (var appMan in substAppMen) {
                            recipients.Add(appMan.Participants);
                        }
                        var htRecipient = AppMail.GetRecipientsLangs(recipients);

                        IDictionaryEnumerator iEnum = htRecipient.GetEnumerator();
                        
                        try {
                            ResourceManager rm = new ResourceManager("Resources.RequestResource", global::System.Reflection.Assembly.Load("App_GlobalResources"));
                            while (iEnum.MoveNext()) {
                                string strCulture = iEnum.Key.ToString();
                                CultureInfo culture = CultureInfo.CreateSpecificCulture(strCulture);
                                
                                string strTo = iEnum.Value.ToString();

                                string strSubject = rm.GetString("MailSubstAppRequestSubject", culture); 
                                string strSubst = substData.substituted_name_surname_first + " -> " + substData.substitutee_name_surname_first
                                    + " " + substData.substitute_start_date.ToString() + " - " + substData.substitute_end_date.ToString();
                                string strBody = "<p>" + String.Format(
                                    rm.GetString("MailSubstAppRequestBody", culture), 
                                    CurrentUser.ParticipantNameFirstNameFirst,
                                    strSubst) + "</p>";
                                strBody += "<p><a href=\"" + GetRootUrl() + "Participant/UserSubstitutionDetail?id=" + substId + "\">" 
                                    + GetRootUrl() + "Participant/UserSubstitutionDetail?id=" + substId + "</a></p>";
                                
                                AppMail.SendMail(
                                    strTo,
                                    null,
                                    null,
                                    strSubject,
                                    strBody);
                            }
                        } catch (Exception ex) {
                            throw ex;
                        } 
                    }
                }
                
                HttpResult httpResult = new HttpResult();
                httpResult.int_value = substId;
                httpResult.string_value = GetErrorMsg(msgItems);

                return GetJson(httpResult);
            } catch (Exception ex) {
                HandleError(ex);
                if (Response != null) {
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                } else {
                    throw ex;
                }
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpPost]
        public ActionResult DeactivateUserSubstitution(UserSubstitutionExtended substData) {
            try {
                HttpResult httpResult = new HttpResult();

                if (substData.substituted_user_id != CurrentUser.ParticipantId) {
                    var partSubst = new UserRepository().GetParticipantById(substData.substituted_user_id);
                    int substCompId = partSubst.company_id;
                    bool isParticipantSubstAdmin = CurrentUser.IsParticipantSubstituteManagerOfCompany(partSubst.company_id);

                    if (isParticipantSubstAdmin == false) {
                        if (!CurrentUser.ParticipantCompanyIds.Contains(substCompId)
                            || (substData.substituted_user_id != CurrentUser.ParticipantId) 
                            && (substData.author_id != CurrentUser.ParticipantId)) {
                            //throw new ExNotAuthorizedUpdateUser("Not authorized to update user substitution");
                            httpResult.error_id = HttpResult.NOT_AUTHORIZED_ERROR;

                            return GetJson(httpResult);
                        }
                    }
                }

                SubstitutionRepository substitutionRepository = new SubstitutionRepository();
                substitutionRepository.DeactiveUserSubstitutionById(substData.id);
                
                
                return GetJson(httpResult);
            } catch (Exception ex) {
                HandleError(ex);
#if !TEST
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
#endif
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        

        [HttpPost]
        public ActionResult ReplaceRequestor(List<CentreGroupReplace> cge) {
            try {
                CheckReplaceAuthorization(cge);

                new UserRepository().ReplaceRequestor(cge);
                                
                return GetJson(new HttpResult());
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpPost]
        public ActionResult ReplaceOrderer(List<CentreGroupReplace> cge) {
            try {
                CheckReplaceAuthorization(cge);

                new UserRepository().ReplaceOrderer(cge);

                return GetJson(new HttpResult());
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpPost]
        public ActionResult ReplaceCentreMan(List<CentreGroupReplace> cge) {
            try {
                CheckReplaceAuthorization(cge);

                new UserRepository().ReplaceCentreMan(cge);

                return GetJson(new HttpResult());
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        //[HttpPost]
        //public ActionResult SaveSubstitution(UserSubstitutionExtended subst) {
        //    try {
        //        SubstitutionRepository substitutionRepository = new SubstitutionRepository();

        //        UserRepository userRepository = new UserRepository();
        //        if (subst.substituted_user_id != CurrentUser.ParticipantId && subst.author_id != CurrentUser.ParticipantId) {
        //            var substedPart =  userRepository.GetParticipantById(subst.substituted_user_id);
        //            int substedCompId = substedPart.company_id;
        //            //if (!IsUserInOfficeRole(UserRole.OfficeAdministrator, substedCompId)) {
        //                throw new ExNotAuthorizedUpdateUser("Not authorized to update Substitution");
        //            //}
        //        }

        //        //var substeePart = userRepository.GetParticipantById(subst.substitute_user_id);
        //        //int substeeCompId = substeePart.company_id;
        //        //if (!IsUserInOfficeRole(UserRole.OfficeAdministrator, substeeCompId)) {
        //        //    throw new ExNotAuthorizedUpdateUser("Not authorized to update Users");
        //        //}
                                
        //        if (!IsSubstitutionValid(subst)) {
        //            throw new ExMissingMandatoryFields("Subtitution Data is not Valid");
        //        }

        //        HttpResult httpResult = new HttpResult();

        //        List<string> msgItems;
        //        int subsId = substitutionRepository.SaveSubstitutionData(subst, CurrentUser.ParticipantId, out msgItems);

        //        httpResult.int_value = subsId;
        //        httpResult.string_value = GetErrorMsg(msgItems);

        //        return GetJson(httpResult);
        //    } catch (Exception ex) {
        //        HandleError(ex);
        //        Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        //        return Content(ex.Message, MediaTypeNames.Text.Plain);

        //    }
        //}

        [HttpPost]
        public ActionResult ApproveSubstitution(int substId) {
            try {
                HttpResult httpResult = new HttpResult();

                if (IsUserAuthorizedToApproveSubstitution(substId)) {
                    new SubstitutionRepository().ApproveSubstitution(substId, CurrentUser.ParticipantId);
                } else {
                    httpResult.string_value = MSG_KEY_NOT_AUTHORIZED;
                }
                               
                return GetJson(httpResult);
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpPost]
        public ActionResult RejectSubstitution(int substId) {
            try {
                HttpResult httpResult = new HttpResult();

                if (IsUserAuthorizedToApproveSubstitution(substId)) {
                    new SubstitutionRepository().RejectSubstitution(substId, CurrentUser.ParticipantId);
                } else {
                    httpResult.string_value = MSG_KEY_NOT_AUTHORIZED;
                }

                //SendMail();

                return GetJson(httpResult);
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }
#endregion

        #region Methods
        //private string GetErrorMsg(List<string> items) {
        //    string strMessage = "";

        //    foreach (string item in items) {
        //        if (strMessage.Length < 100) {
        //            if (strMessage.Length > 0) {
        //                strMessage += ", ";
        //            }

        //            strMessage += item;
        //        } else {
        //            strMessage += " ... ";
        //            //return strMessage;
        //            return strMessage;

        //        }
        //    }

        //    return strMessage;
        //}

        public ActionResult GetParticipantCompanies() {
            List<AgDropDown> companies = new UserRepository().GetCompaniesByParticipantId(CurrentUser.ParticipantAdminCompanyIds);

            return GetJson(companies.ToList());
        }

        private bool IsSubstitutionValid(UserSubstitutionExtended subt) {
            if (subt.substituted_user_id == DataNulls.INT_NULL) {
                return false;
            }

            if (subt.substitute_user_id == DataNulls.INT_NULL) {
                return false;
            }

            if (subt.substituted_user_id == subt.substitute_user_id) {
                return false;
            }

            if (subt.substitute_start_date == DataNulls.DATETIME_NULL) {
                return false;
            }

            if (subt.substitute_end_date == DataNulls.DATETIME_NULL) {
                return false;
            }

            return true;
        }

        private bool IsParticipantValid(ParticipantsExtended user) {
            if (String.IsNullOrEmpty(user.first_name)) {
                return false;
            }

            if (String.IsNullOrEmpty(user.surname)) {
                return false;
            }

            if (String.IsNullOrEmpty(user.user_name)) {
                return false;
            }

            if (user.company_id == DataNulls.INT_NULL) {
                return false;
            }

            return true;
        }

        private void CheckAppMatrixBeforeDeleteUser(
            int participantId, 
            out bool isActiveAppMan, 
            out bool isActiveOrderer) {

            new UserRepository().GetActiveManagerCg(participantId, out isActiveAppMan, out isActiveOrderer);

        }

        private void CheckReplaceAuthorization(List<CentreGroupReplace> cge) {
            UserRepository userRepository = new UserRepository();

            foreach (var cgReplace in cge) {
                if (!cgReplace.is_selected) {
                    continue;
                }


                var origParticipant = userRepository.GetParticipantById(cgReplace.orig_user_id);

                CompanyRepository companyRepository = new CompanyRepository();

                int origOfficeId = companyRepository.GetCompanyIdByName(origParticipant.Company.country_code);
                if (origOfficeId == DataNulls.INT_NULL) {
                    throw new ExNotAuthorizedUpdateUser("Unknown Company");
                }

                if (!IsUserInOfficeRole(UserRole.OfficeAdministrator, origOfficeId)) {
                    throw new ExNotAuthorizedUpdateUser("Not authorized to update Users");
                }

                var replaceParticipant = userRepository.GetParticipantById(cgReplace.replace_user_id);

                int replaceOfficeId = companyRepository.GetCompanyIdByName(replaceParticipant.Company.country_code);
                if (replaceOfficeId == DataNulls.INT_NULL) {
                    throw new ExNotAuthorizedUpdateUser("Unknown Company");
                }

                if (!IsUserInOfficeRole(UserRole.OfficeAdministrator, replaceOfficeId)) {
                    throw new ExNotAuthorizedUpdateUser("Not authorized to update Users");
                }
            }
        }

        private string GetSubstAppStatusText(int iAppStatus) {
            switch (iAppStatus) {
                case (int)RequestRepository.ApproveStatus.Rejected:
                    return RequestResource.Rejected;
                case (int)RequestRepository.ApproveStatus.Approved:
                    return RequestResource.Approved;
                case (int)RequestRepository.ApproveStatus.NotNeeded:
                    return RequestResource.NotNeeded;
                case (int)RequestRepository.ApproveStatus.WaitForApproval:
                    return RequestResource.WaitForApproval;
            }

            return null;
        }

        private int GetSubstAppStatus(string strAppStatus) {
            if (strAppStatus == RequestResource.Rejected) {
                return (int)RequestRepository.ApproveStatus.Rejected;
            } else if (strAppStatus == RequestResource.Approved) {
                return (int)RequestRepository.ApproveStatus.Approved;
            } else if (strAppStatus == RequestResource.NotNeeded) {
                return (int)RequestRepository.ApproveStatus.NotNeeded;
            } else if (strAppStatus == RequestResource.WaitForApproval) {
                return (int)RequestRepository.ApproveStatus.WaitForApproval;
            }

            return (int)RequestRepository.ApproveStatus.Empty;
        }

        private void SetApprovalTextStatus(List<UserSubstitutionExtended> userSubstitution) {
            if (userSubstitution == null) {
                return;
            }

            foreach (var userSubst in userSubstitution) {
                userSubst.approval_status_text = GetSubstAppStatusText(userSubst.approval_status);
            }
        }

        private string GetFilter(string filter) {
            if (filter == null) {
                return filter;
            }

            string strFixFilter = "";
            string[] filterItems = filter.Split(BaseController.UrlParamDelimiter.ToCharArray());
            foreach (var filterItem in filterItems) {
                string[] itemItems = filterItem.Split(BaseController.UrlParamValueDelimiter.ToCharArray());
                if (strFixFilter.Length > 0) {
                    strFixFilter += BaseController.UrlParamDelimiter;
                }
                if (itemItems[0] == "approval_status_text") {
                    strFixFilter += "approval_status" + BaseController.UrlParamValueDelimiter + GetSubstAppStatus(itemItems[1]);
                } else {
                    strFixFilter += filterItem;
                }
            }

            return strFixFilter;
        }

        private void LoadUserPhoto() {

        }

        private bool IsUserAuthorizedToApproveSubstitution(int substId) {
            return new SubstitutionRepository().IsApprovalAllowed(substId, CurrentUser.ParticipantId);
        }
#endregion
    }
}