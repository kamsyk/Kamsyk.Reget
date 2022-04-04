using Kamsyk.Reget.Content;
using Kamsyk.Reget.Content.Sessions;
using Kamsyk.Reget.Content.UrlParams;
using Kamsyk.Reget.Controllers.Interface;
using Kamsyk.Reget.Controllers.RegetExceptions;
using Kamsyk.Reget.Model;
using Kamsyk.Reget.Model.Common;
using Kamsyk.Reget.Model.Repositories;
using Kamsyk.Reget.Model.Repositories.Interfaces;
using Kamsyk.Reget.Setting;
using Resources;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;
using static Kamsyk.Reget.Model.Repositories.UserRepository;

namespace Kamsyk.Reget.Controllers {
    public class BaseController : Controller, IControllerActivator {
        #region Constants
        public const string CULTURE_EN = "en-US";
        public const string CULTURE_CZ = "cs-CZ";
        public const string CULTURE_SK = "sk-SK";
        public const string CULTURE_PL = "pl-PL";
        public const string CULTURE_RO = "ro-RO";
        public const string CULTURE_BG = "bg-BG";
        public const string CULTURE_SL = "sl-SI";
        public const string CULTURE_SR = "sr-Latn-CS";
        public const string CULTURE_HU = "hu-HU";

        public const string ANG_ERR_MSG_MANDATORY = "_errmandatory";

        //TEST IT in "Get Error Message Are all of the strings covered"
        public string MSG_KEY_NOT_AUTHORIZED = "NotAuthorized";
        public string MSG_KEY_MISSING_MANDATORY_ITEM = "MissingMandatoryItem";
        public string MSG_KEY_REQUEST_CANNOT_BE_REVERTED = "RequestCannotBeReverted";
        #endregion

        #region Struct
        public struct DiscussionColor {
            public string BkgColor;
            public string BorderColor;
            public string UserColor;
        }
        #endregion

        #region Virtual Propertiesw
        public virtual string HeaderTitle {
            get { return null; }
        }
        public virtual string HeaderImgUrl {
            get { return null; }
        }

        //private bool m_IsGenerateDatePickerLocalization = false;
        protected virtual bool IsGenerateDatePickerLocalization {
            get { return false; }
            //set { m_IsGenerateDatePickerLocalization = value; }
        }
        #endregion

        #region Static Properties
        protected static string GetControllerName(string rawControllerName) {

            string className = rawControllerName.Replace("Controller", "");

            return className;

        }

        public static bool IsTestMode {
            get {
#if TEST
                return true;
#else
                return false;
#endif
            }
        }

        public static string UrlParamDelimiter {
            get { return "|"; }
        }
        public static string UrlParamValueDelimiter {
            get { return "~"; }
        }
        #endregion

        #region Properties
        private RegetUser m_CurrentUser = null;
        public RegetUser CurrentUser {
            get {
                if (m_CurrentUser == null) {
                    if (Session[RegetSession.SES_PARTICIPANT] == null) {
                        bool isDbAccessible = false;
                        SetCurrentUser(Request.RequestContext, out isDbAccessible);
                        if (!isDbAccessible) {
                            throw new Exception("DB is not accessible");
                        }
                        Session[RegetSession.SES_PARTICIPANT] = m_CurrentUser;
                    }

                    m_CurrentUser = (RegetUser)Session[RegetSession.SES_PARTICIPANT];
                }

                return m_CurrentUser;
            }
#if TEST
            set { m_CurrentUser = value; }
#endif
        }

        private List<Participants> m_AppAdmins = null;
        public List<Participants> AppAdmins {
            get {
                if (m_AppAdmins == null) {
                    m_AppAdmins = new UserRepository().GetAppAdmins();
                }

                return m_AppAdmins;
            }
        }

        private string m_DefaultLanguage = "en-US";
        protected string DefaultLanguage {
            get { return m_DefaultLanguage; }
        }


        // private string m_CurrentCultureCode = "en-US";
        public string CurrentCultureCode {
            //get { return m_CurrentCultureCode; }
            get {
                if (CurrentUser.Participant.User_Setting == null || CurrentUser.Participant.User_Setting.default_lang == null) {
                    return CurrentUser.Participant.Company.culture_info;
                } else {
                    return CurrentUser.Participant.User_Setting.default_lang;
                }
            }
        }

        public bool IsDefaultLang {
            get { return CurrentCultureCode == DefaultLanguage; }
        }

        public bool IsTest {
            get {
                return RegetSettings.IsTest;
                //string strIsTest = System.Configuration.ConfigurationManager.AppSettings["IsTest"];
                //return (strIsTest.ToLower().Trim() == "true");
            }
        }

        private string _m_DatePickerFormat = null;
        public string DatePickerFormat {
            get {
                if (_m_DatePickerFormat == null) {
                    _m_DatePickerFormat = GetShortDateFormat();
                }

                return _m_DatePickerFormat;

            }
        }

        public string DatePickerFormatMoment {
            get {
                if (DatePickerFormat == null) {
                    return null;
                }
                return DatePickerFormat.ToUpper();
            }
        }

        private string m_DateTimePickerFormat = null;
        public string DateTimePickerFormat {
            get {
                if (m_DateTimePickerFormat == null) {
                    m_DateTimePickerFormat = GetShortDateTimeFormat();
                }

                return m_DateTimePickerFormat;
            }
        }

        private string m_DateTimePickerFormatMoment = null;
        public string DateTimePickerFormatMoment {
            get {
                if (m_DateTimePickerFormatMoment == null) {
                    string dateTimeFormat = GetShortDateFormat().ToUpper();

                    dateTimeFormat += " HH:mm";
                    m_DateTimePickerFormatMoment = dateTimeFormat;
                }

                return m_DateTimePickerFormatMoment;
            }
        }

        public static string DecimalSeparator {
            get {
                return Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;

                ///return ".";
            }
        }

        #endregion

        #region Test Properties
        //protected ICompanyRepository TestCompRep = null;
        private IUserRepository m_testUserRep = null;
        #endregion

        #region Constructor
        public BaseController() : base() { }

        public BaseController(IUserRepository testUserRep) : base() {
            m_testUserRep = testUserRep;
        }
        #endregion

        #region Interface Methods
        public IController Create(RequestContext requestContext, Type controllerType) {

#if TEST
            if (requestContext.ToString().IndexOf("Castle") > -1) {
                //Rhino Mock
                return DependencyResolver.Current.GetService(controllerType) as IController;
            }
#endif

            base.Initialize(requestContext);
            //SetParticipant(requestContext);

            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            //string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            if (controllerType == typeof(HomeController) && actionName == "Error") {
                //Error Page
                if (CurrentUser != null && CurrentUser.Participant != null) {
                    SetLangByUserSettings();
                }
                
                return DependencyResolver.Current.GetService(controllerType) as IController;
            }

            //**********************************************************************************
            //it must be here because m_participant is not null, it remains from previous request and then 
            //e.g. user "skowron" was used for previous chrome session, now the current is "dragos" but 
            //it is still remain here "skowron"
            //the result is that "dragos" is set finally but he has Polish language set
            //this is a workaround

            if (Session[RegetSession.SES_PARTICIPANT] == null) {
                bool isDbAccessible = false;
                SetCurrentUser(requestContext, out isDbAccessible);

                if (!isDbAccessible) {
                    throw new ExNotDbConnection();
                }

                m_CurrentUser = (RegetUser)Session[RegetSession.SES_PARTICIPANT];
            }
            //************************************************************************************

            string lang = null;
            if (requestContext.HttpContext.Request.Params.HasKeys()) {
                foreach (var key in requestContext.HttpContext.Request.Params.Keys) {
                    if (key != null && key.ToString() == UrlParams.LANG) {
                        lang = requestContext.HttpContext.Request.Params[UrlParams.LANG];
                        CurrentUser.Participant.User_Setting.default_lang = lang;
                        break;
                    }
                }
            }

            
            if (CurrentUser == null || CurrentUser.Participant == null) {
                throw new ExNotAuthorizedUser();
            }

            if (lang == null) {
                SetLangByUserSettings();

            } else {
                try {
                    CultureInfo ci = new CultureInfo(lang);

                    //if (Thread.CurrentThread.CurrentCulture != ci) {
                    //    Session[RegetSession.SES_PURCHASE_GROUPS] = null;
                    //}

                    Thread.CurrentThread.CurrentCulture = ci;
                    Thread.CurrentThread.CurrentUICulture = ci;

                    new UserRepository().SetParticipantLang(m_CurrentUser.ParticipantId, ci.Name);
                    m_CurrentUser = null;
                } catch {
                    CultureInfo ci = new CultureInfo(m_DefaultLanguage);

                    //if (Thread.CurrentThread.CurrentCulture != ci) {
                    //    Session[RegetSession.SES_PURCHASE_GROUPS] = null;
                    //}

                    Thread.CurrentThread.CurrentCulture = ci;
                    Thread.CurrentThread.CurrentUICulture = ci;
                }
            }

            //m_CurrentCultureCode = Thread.CurrentThread.CurrentUICulture.Name;

            return DependencyResolver.Current.GetService(controllerType) as IController;
        }

        private void SetLangByUserSettings() {
            string userLang = null;
            if (CurrentUser.Participant.User_Setting != null) {
                userLang = CurrentUser.Participant.User_Setting.default_lang;
            }
            if (userLang == null) {
                userLang = CurrentUser.Participant.Company.culture_info;
            }
            if (userLang == null) {
                userLang = m_DefaultLanguage;
            }

            CultureInfo ci = new CultureInfo(CULTURE_EN);
            try {
                ci = new CultureInfo(userLang);
            } catch {
                ci = new CultureInfo(CULTURE_EN);
            }

            //cannot be here otherwise it is applied all the time, allways Thread.CurrentThread.CurrentCulture != ci
            //if (Thread.CurrentThread.CurrentCulture != ci) {
            //    Session[RegetSession.SES_PURCHASE_GROUPS] = null;
            //}

            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }
        #endregion

        #region Static Methods
        protected static string RemoveDiacritics(string rawText) {
            return ConvertData.RemoveDiacritics(rawText);
        }
        #endregion

        #region Virtual Methods
        public virtual ActionResult Index(int? id) {
            ViewBag.HeaderImgUrl = HeaderImgUrl;
            ViewBag.HeaderTitle = HeaderTitle;
            return View();
        }

        #endregion

        #region Overriden Methods
        protected override void Initialize(RequestContext requestContext) {
            base.Initialize(requestContext);

            if ((Request.Browser.Browser.ToLower() == "internetexplorer" || Request.Browser.Browser.ToLower() == "ie")
                && Request.Browser.MajorVersion < 8) {
                if (!Request.Url.AbsoluteUri.Contains("Home/NotSupportedBrowser")) {
                    Response.Redirect(GetRootUrl() + "Home/NotSupportedBrowser");
                }
            }

            //    //m_Participant = (Participants)Session[RegetSession.SES_PARTICIPANT];
            //    //SetParticipant(requestContext);

            
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext) {
            base.OnActionExecuting(filterContext);


            if (IsGenerateDatePickerLocalization && ViewBag.DatePickerResources == null) {
                StringBuilder sbDatePickerLocalization = new StringBuilder();

                sbDatePickerLocalization.AppendLine("function SetDatePicker($mdDateLocaleProvider) {");

                sbDatePickerLocalization.AppendLine("   $mdDateLocaleProvider.months = [" +
                    "'" + RequestResource.January + "'," +
                    "'" + RequestResource.February + "'," +
                    "'" + RequestResource.March + "'," +
                    "'" + RequestResource.April + "'," +
                    "'" + RequestResource.May + "'," +
                    "'" + RequestResource.Jun + "'," +
                    "'" + RequestResource.July + "'," +
                    "'" + RequestResource.August + "'," +
                    "'" + RequestResource.September + "'," +
                    "'" + RequestResource.October + "'," +
                    "'" + RequestResource.November + "'," +
                    "'" + RequestResource.December + "'" +
                    "];");

                sbDatePickerLocalization.AppendLine("   $mdDateLocaleProvider.shortMonths = [" +
                    "'" + RequestResource.JanuaryShort + "'," +
                    "'" + RequestResource.FebruaryShort + "'," +
                    "'" + RequestResource.MarchShort + "'," +
                    "'" + RequestResource.AprilShort + "'," +
                    "'" + RequestResource.MayShort + "'," +
                    "'" + RequestResource.JuneShort + "'," +
                    "'" + RequestResource.JulyShort + "'," +
                    "'" + RequestResource.AugustShort + "'," +
                    "'" + RequestResource.SeptemberShort + "'," +
                    "'" + RequestResource.OctoberShort + "'," +
                    "'" + RequestResource.NovemberShort + "'," +
                    "'" + RequestResource.DecemberShort + "'" +
                    "];");

                sbDatePickerLocalization.AppendLine("   $mdDateLocaleProvider.days = [" +
                    "'" + RequestResource.Sunday + "'," +
                    "'" + RequestResource.Monday + "'," +
                    "'" + RequestResource.Tuesday + "'," +
                    "'" + RequestResource.Wednesday + "'," +
                    "'" + RequestResource.Thursday + "'," +
                    "'" + RequestResource.Friday + "'," +
                    "'" + RequestResource.Saturday + "'" +
                    "];");

                sbDatePickerLocalization.AppendLine("   $mdDateLocaleProvider.shortDays = ["
                    + "'" + RequestResource.SundayShort + "',"
                    + "'" + RequestResource.MondayShort + "'," 
                    + "'" + RequestResource.TuesdayShort + "'," 
                    + "'" + RequestResource.WednesdayShort + "'," 
                    + "'" + RequestResource.ThursdayShort + "'," 
                    + "'" + RequestResource.FridayShort + "'," 
                    + "'" + RequestResource.SaturdayShort + "'" 
                    + "];");

                CultureInfo ci = new CultureInfo(CurrentCultureCode);
                var firstDayOfWeek = ci.DateTimeFormat.FirstDayOfWeek;
                int iFirstOfWeek = (int)firstDayOfWeek;
                sbDatePickerLocalization.AppendLine("   $mdDateLocaleProvider.firstDayOfWeek = " + iFirstOfWeek + ";");

                sbDatePickerLocalization.AppendLine("   $mdDateLocaleProvider.parseDate = function (dateString) {");
                sbDatePickerLocalization.AppendLine("       var m = null;");
                //sbDatePickerLocalization.AppendLine("       if(isDateTimePicker === true) {");
                //sbDatePickerLocalization.AppendLine("        m = moment(dateString, '" + DateTimePickerFormatMoment + "', true);");
                //sbDatePickerLocalization.AppendLine("       } else {");
                sbDatePickerLocalization.AppendLine("        m = moment(dateString, '" + DatePickerFormatMoment + "', true);");
                //sbDatePickerLocalization.AppendLine("       }");
                //sbDatePickerLocalization.AppendLine("       var m = moment('12.5.2019 12:25', 'D.M.YYYY HH:mm', true);");
                sbDatePickerLocalization.AppendLine("       var retDate = m.isValid() ? m.toDate() : new Date(NaN);");
                //sbDatePickerLocalization.AppendLine("       dateTimeWa = m;");
                sbDatePickerLocalization.AppendLine("       return retDate;");
                sbDatePickerLocalization.AppendLine("   }");

                sbDatePickerLocalization.AppendLine("   $mdDateLocaleProvider.formatDate = function (date) {");
                //sbDatePickerLocalization.AppendLine("       SetDateTimePickerWidth();");
                //sbDatePickerLocalization.AppendLine("       if(isDateTimePicker === true) {");
                //sbDatePickerLocalization.AppendLine("        return date ? moment(date).format('" + DateTimePickerFormatMoment + "') : '';");
                //sbDatePickerLocalization.AppendLine("       } else {");
                sbDatePickerLocalization.AppendLine("        return date ? moment(date).format('" + DatePickerFormatMoment + "') : '';");
                //sbDatePickerLocalization.AppendLine("       }");
                sbDatePickerLocalization.AppendLine("   }");

                //sbDatePickerLocalization.AppendLine(" SetDateTimePickerWidth();");

                sbDatePickerLocalization.AppendLine("}");

                ViewBag.DatePickerResources = sbDatePickerLocalization.ToString();
            }
        }
        /*
         * function SetDatePicker($mdDateLocaleProvider) {
if (IsValueNullOrUndefined(Resources)) {
    return;
}

// Date Picker Localization
$mdDateLocaleProvider.months = [Resources.January, Resources.February, 'Marzo', 'Abril', 'Mayo', 'Junio',
                                'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'];
$mdDateLocaleProvider.shortMonths = [Resources.January, Resources.February, 'Mar', 'Abr', 'May', 'Jun',
                                'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'];
$mdDateLocaleProvider.days = ['Domingo', 'Lunes', 'Martes', 'Miercoles', 'Jueves', 'Viernes', 'Sábado'];
$mdDateLocaleProvider.shortDays = [moLoc, 'Lu', 'Ma', 'Mi', 'Ju', 'Vi', Resources.SaturdayShort];
// Can change week display to start on Monday.
$mdDateLocaleProvider.firstDayOfWeek = 1;
// Optional.
//$mdDateLocaleProvider.dates = [1, 2, 3, 4, 5, 6, 7,8,9,10,11,12,13,14,15,16,17,18,19,
//                               20,21,22,23,24,25,26,27,28,29,30,31];
// In addition to date display, date components also need localized messages
// for aria-labels for screen-reader users.
$mdDateLocaleProvider.weekNumberFormatter = function (weekNumber) {
    return 'Semana ' + weekNumber;
};
$mdDateLocaleProvider.msgCalendar = 'Calendario';
$mdDateLocaleProvider.msgOpenCalendar = 'Abrir calendario';

$mdDateLocaleProvider.formatDate = function (date) {
    return date ? moment(date).format('DD.MM.YYYY HH:mm') : '';
};

$mdDateLocaleProvider.parseDate = function (dateString) {
    var m = moment(dateString, 'DD.MM.YYYY HH:mm', true);
    return m.isValid() ? m.toDate() : new Date(NaN);
};

//$mdDateLocaleProvider.formatDate = function (date) {

//    var day = date.getDate();
//    var monthIndex = date.getMonth();
//    var year = date.getFullYear();

//    return day + '/' + (monthIndex + 1) + '/' + year;

//};

//$mdDateLocaleProvider.parseDate = function (dateString) {
//        var m = moment(dateString, 'DD/MM/YYYY', true);
//        return m.isValid() ? m.toDate() : new Date(NaN);
//    };



        //    StringBuilder sbDatePickerLocalization = new StringBuilder();
        //    sbDatePickerLocalization.AppendLine("var moLoc='Ne';");

        //    sbDatePickerLocalization.AppendLine("var Resources = {");
        //    //Months
        //    sbDatePickerLocalization.AppendLine("January:'" + RequestResource.January + "',");
        //    sbDatePickerLocalization.AppendLine("February:'" + RequestResource.February + "',");


        //    sbDatePickerLocalization.AppendLine("SaturdayShort:'So'");
        //    sbDatePickerLocalization.AppendLine("};");

        //    ViewBag.DatePickerResources = sbDatePickerLocalization.ToString();
        //    //ViewBag.StartupScript = "var moLoc='Ne';";
        }
    }

    //protected override void OnException(ExceptionContext filterContext) {
    //    //base.OnException(filterContext);

    //    Exception ex = filterContext.Exception;
    //    filterContext.ExceptionHandled = true;

    //    string controllenName = MethodBase.GetCurrentMethod().DeclaringType.Name;
    //    var model = new HandleErrorInfo(filterContext.Exception, controllenName, "Action");

    //    filterContext.Result = new ViewResult() {
    //        ViewName = "Error",
    //        ViewData = new ViewDataDictionary(model)
    //    };

    //}

    //protected override void OnActionExecuting(ActionExecutingContext filterContext) {
    //    // call the base method first
    //    base.OnActionExecuting(filterContext);

    //    //return RedirectToAction("HomeController", "Error", new ExNotAuthorizedUser());

    //    // if the user hasn't changed their password yet, force them to the welcome page
    //    if (CurrentParticipant == null) {
    //       filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "HomeController", "Error" }});

    //    }
    //}
    */

        #endregion

        #region Methods
        private Participants GetParticipant(RequestContext requestContext) {
            //if (Session[RegetSession.SES_PARTICIPANT] == null) {
            string httpUserName = requestContext.HttpContext.User.Identity.Name;
            string[] userItems = httpUserName.Split('\\');
            string userName = userItems[1];

#if DEBUG
            //userName = "pafr";
            //userName = "simona";
            //userName = "matm";
            //userName = "rezact";
            //userName = "zavasnik";
            //userName = "vrana";
            //userName = "pesekd";
            //userName = "margulam";
#endif

            UserRepository userRepository = new UserRepository();
            Participants participant = userRepository.GetActiveParticipantByUserName(userName);

            return participant;
            
        }

        private void SetCurrentUser(RequestContext requestContext, out bool isDbAccessible) {
            isDbAccessible = true;
            if (Session[RegetSession.SES_PARTICIPANT] == null) {
                RegetUser regetUser = new RegetUser();
                try {
                    regetUser.Participant = GetParticipant(requestContext);
                } catch (Exception ex) {
                    if (ex is SqlException 
                        || (ex.InnerException != null && ex.InnerException is SqlException) 
                        || (ex.InnerException.InnerException != null && ex.InnerException.InnerException is SqlException)) {
                        //DB cannot be connected
                        //Response.Redirect(GetRootUrl() + "Content/Html/RegetError.html", true);
                        isDbAccessible = false;
                        return;
                    } else {
                        throw ex;
                    }
                   
                }

                isDbAccessible = true;

                //it is better to check it again
                if (Session[RegetSession.SES_PARTICIPANT] == null) {
                    Session[RegetSession.SES_PARTICIPANT] = regetUser;
                }
            }

            m_CurrentUser = (RegetUser)Session[RegetSession.SES_PARTICIPANT];
        }

        public string GetRootUrl() {
           
            string rootUrl = null;
            if (Request == null) {
                rootUrl = "http://localhost/Reget";
            } else {
                rootUrl = Request.Url.AbsoluteUri;
            }

            string[] rootUrlParts = rootUrl.Split('/');
            if (rootUrl.ToLower().IndexOf("localhost") > -1) {
                rootUrl = rootUrlParts[0] + "/" + rootUrlParts[1] + "/" + rootUrlParts[2] + "/";
            } else {
                rootUrl = rootUrlParts[0] + "/" + rootUrlParts[1] + "/" + rootUrlParts[2] + "/" + rootUrlParts[3] + "/";
            }

            return rootUrl;
        }

        protected bool IsUpdateCentreGroupAllowed(UserRepository.UserRole requiredRole, int centreGroupId) {
            CentreGroupRepository centreGroupRepository = new CentreGroupRepository();

            return centreGroupRepository.IsAuthorized(CurrentUser.Participant.id, (int)requiredRole, centreGroupId);

        }

        protected bool IsUserInOfficeRole(UserRepository.UserRole requiredRole, int officeId) {
            string sessKey = RegetSession.SES_IS_USER_IN_OFFICE_ROLE_ALLOWED + "_" + requiredRole + "_" + officeId;
            if (Session != null && ConvertData.ToBoolean(Session[sessKey]) == true) {
                return true;
            }

            bool isAllowed = false;
            UserRepository userRepository = new UserRepository();

            if (m_testUserRep != null) {
                if (m_CurrentUser == null) {
                    m_CurrentUser = new RegetUser();
                    CurrentUser.Participant = new Participants();
                }
                isAllowed = m_testUserRep.IsAuthorized(CurrentUser.Participant.id, (int)requiredRole, officeId);
            } else {
                isAllowed = userRepository.IsAuthorized(CurrentUser.Participant.id, (int)requiredRole, officeId);
            }

            if (Session != null) { //because of TEST reason
                Session[sessKey] = isAllowed;
            }

            return isAllowed;
        }

        protected bool IsUpdateSuppAdminAllowed(UserRepository.UserRole requiredRole, int companyId) {
            string sessKey = RegetSession.SES_IS_SUPP_GROUP_IS_ALLOWED + "_" + requiredRole + "_" + companyId;
            if (ConvertData.ToBoolean(Session[sessKey]) == true) {
                return true;
            }

            bool isAllowed = new UserRepository().IsAuthorized(CurrentUser.Participant.id, (int)requiredRole, companyId);

            Session[sessKey] = isAllowed;

            return isAllowed;
        }

        protected bool IsSuppGroupIsAllowed(int centreId) {
            string sessKey = RegetSession.SES_IS_SUPP_GROUP_IS_ALLOWED + "_" + centreId;
            if (ConvertData.ToBoolean(Session[sessKey]) == true) {
                return true;
            }

            var centre = new CentreRepository().GetCentreById(centreId);
            int companyId = centre.company_id;

            bool isAllowed = new UserRepository().IsAuthorized(CurrentUser.Participant.id, (int)UserRole.Requestor, companyId);
            Session[sessKey] = isAllowed;

            return isAllowed;
        }


        public void HandleError(Exception ex) {
#if TEST
            if (Response == null) {
                return;
            }
#endif

            if (Response.IsRequestBeingRedirected) {
                return;
            }

            string strUser = "";
            if (CurrentUser != null) {
                strUser += CurrentUser.Participant.surname + " " + CurrentUser.Participant.first_name + " (" + CurrentUser.Participant.user_name + ")";
            }
            HandleError(ex, strUser);
        }

        public static void HandleError(Exception ex, string strUser) {
            HandleError(ex, strUser, true);
        }

        public static void HandleError(Exception ex, string strUser, bool isSaveLogToDb) {
            
            if (strUser == null) {
                strUser = "";
            }

            if (isSaveLogToDb) {
                //Save log to DB
            }

#if DEBUG || TEST
            throw ex;
#else
            try {
                WsMail.OtWsMail wsMail = new WsMail.OtWsMail();
                wsMail.SendMail(
                    "reget@otis.com",
                    "kamil.sykora@otis.com",
                    null,
                    "Reget Error " + strUser,
                    ex.ToString(),
                    null,
                    (int)System.Net.Mail.MailPriority.High);
            } catch { }
#endif
            
        }

        public ActionResult GetJson(object data) {
            try {

                JsonResult jsonResult = Json(data, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return jsonResult;

            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        public string GetNavigationText() {
            string navText = "";

            if (Request == null || Request.RawUrl == null) {
                return navText;
            }

            string[] urlParts = Request.RawUrl.Split('/');

            string lastPart = urlParts[urlParts.Length - 1].ToLower();

            switch (lastPart) {
                case "regetadmin":
                    navText = RequestResource.ApprovalMatrix;
                    break;
                case "participant":
                    navText = RequestResource.Users;
                    break;
                case "replaceuser":
                    navText = RequestResource.ReplaceUser;
                    break;
                case "abouthelp":
                    navText = RequestResource.RegetInfo;
                    break;
            }

            if (navText.Length > 0) {
                navText = " > " + navText;
            }

            return navText;
        }


        protected bool CheckUserAutorization(List<UserRole> allowedRoles) {
            foreach (var allowedRole in allowedRoles) {
                switch (allowedRole) {
                    case UserRole.OfficeAdministrator:
                        if (CurrentUser.IsParticipantCompanyAdmin) {
                            return true;
                        }
                        break;
                    case UserRole.ApplicationAdmin:
                        if (CurrentUser.IsParticipantAppAdmin) {
                            return true;
                        }
                        break;
                }
            }

            return false;
        }

        protected string GetErrorMsg(List<string> items) {
            string strMessage = "";

            if (items == null) {
                return strMessage;
            }

            foreach (string item in items) {
                if (strMessage.Length < 100) {
                    if (strMessage.Length > 0) {
                        strMessage += ", ";
                    }

                    strMessage += item;
                } else {
                    strMessage += " ... ";
                    //return strMessage;
                    return strMessage;

                }
            }

            return strMessage;
        }

        protected string DecodeUrl(string url) {
            return HttpUtility.UrlDecode(url);
        }

        public string GetGridLangCode() {
            if (CurrentCultureCode == ParticipantController.CULTURE_BG) {
                return "bg";
            }

            if (CurrentCultureCode == ParticipantController.CULTURE_CZ) {
                return "cz";
            }


            if (CurrentCultureCode == ParticipantController.CULTURE_HU) {
                return "hu";
            }

            if (CurrentCultureCode == ParticipantController.CULTURE_PL) {
                return "pl";
            }

            if (CurrentCultureCode == ParticipantController.CULTURE_RO) {
                return "ro";
            }

            if (CurrentCultureCode == ParticipantController.CULTURE_SK) {
                return "sk";
            }

            if (CurrentCultureCode == ParticipantController.CULTURE_SR) {
                return "sr-lat";
            }

            return "en";
        }

        protected DateTime ConvertUrlStringToDate(string urlDate) {
            if (urlDate == null) {
                return DataNulls.DATETIME_NULL;
            }

            DateTime date;
            DateTime.TryParseExact(
                urlDate,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out date);

            return date;
        }

        public static string GetShortDateFormat() {

            string dateFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern.
                    Replace("MMM", "M").Replace("mmm", "m").
                    Replace("MM", "M").Replace("mm", "m").
                    Replace("DD", "D").Replace("dd", "d").
                    Replace(" ", "");

            //must be fixed e.g. for Bulgaria - following format is retrieved -> d.M.yyyy 'г.'

            string retFormat = "d.M.yyyy";
            try {
                string[] dateParts = dateFormat.Split(Thread.CurrentThread.CurrentCulture.DateTimeFormat.DateSeparator.ToCharArray());
                string strRegexFilter = "(d)*(m)*(y)*(D)*(M)*(Y)*";
                retFormat = "";
                foreach (string datePart in dateParts) {
                    string strFilter = Regex.Match(datePart, strRegexFilter).Value;
                    if (!String.IsNullOrEmpty(strFilter)) {
                        if (retFormat.Length > 0) {
                            retFormat += Thread.CurrentThread.CurrentCulture.DateTimeFormat.DateSeparator.Trim();
                        }
                        retFormat += strFilter;
                    }
                }
                ////var match = Regex.Match(dateParts[1], "(d)*(m)*(y)*(D)*(M)*(Y)*");
                ////string strRes = match.Value;
                //retFormat = Regex.Match(dateParts[0], strRegexFilter).Value + Thread.CurrentThread.CurrentCulture.DateTimeFormat.DateSeparator +
                //   Regex.Match(dateParts[1], strRegexFilter).Value + Thread.CurrentThread.CurrentCulture.DateTimeFormat.DateSeparator +
                //   Regex.Match(dateParts[2], strRegexFilter).Value;

                ////string retFormat = dateFormat;


                DateTime testDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                string strFormatDate = testDate.ToString(retFormat);

                DateTime compDate = DateTime.MinValue;
                DateTime.TryParseExact(
                    strFormatDate,
                    retFormat,
                    Thread.CurrentThread.CurrentCulture,
                    DateTimeStyles.None,
                    out compDate);

                if (testDate != compDate) {
                    retFormat = "d.M.yyyy";
                }
            } catch {
                retFormat = "d.M.yyyy";
            }

            return retFormat;
        }

        public static string GetShortDateMomentFormat() {
            return GetShortDateFormat().ToUpper();
        }

        public bool IsUrlContainsParam(string paramName) {
            if (Request.Params.HasKeys()) {
                foreach (var key in Request.Params.Keys) {
                    if (key != null && key.ToString().ToUpper() == paramName.ToUpper()) {
                        return true;
                    }
                }
            }

            return false;
        }

        public string GetParticipantPhotoUrl() {

            if (!CurrentUser.IsHasParticipantPhoto) {
                return null;
            }

            string strUrl = GetRootUrl() + "Participant/UserPhoto?userId=" + CurrentUser.ParticipantId;

            return strUrl;

        }

        protected string GetShortDateTimeFormat() {
            string dateTimeFormat = GetShortDateFormat();

            return dateTimeFormat + " HH:mm";
        }

        //private string GetShortDateTimeMomentFormat() {
        //    string dateTimeFormat = GetShortDateFormat();

        //    return dateTimeFormat.ToUpper() + " HH:mm";
        //}

        public string GetDateErrorMsg() {
             string strDateErrMsg = String.Format(RequestResource.EnterDateInFormat, 
                 this.DatePickerFormat, 
                 DateTime.Now.ToString(this.DatePickerFormat));

            return strDateErrMsg;
        }

        protected static List<DiscussionColor> GetDiscussionColors() {
            DiscussionColor col1 = new DiscussionColor();
            col1.BkgColor = "#FFCDD2";
            col1.BorderColor = "#EF9A9A";
            col1.UserColor = "#D32F2F";

            DiscussionColor col2 = new DiscussionColor();
            col2.BkgColor = "#F8BBD0";
            col2.BorderColor = "#F48FB1";
            col2.UserColor = "#C2185B";

            DiscussionColor col3 = new DiscussionColor();
            col3.BkgColor = "#E1BEE7";
            col3.BorderColor = "#CE93D8";
            col3.UserColor = "#7B1FA2";

            DiscussionColor col4 = new DiscussionColor();
            col4.BkgColor = "#D1C4E9";
            col4.BorderColor = "#B39DDB";
            col4.UserColor = "#512DA8";

            DiscussionColor col5 = new DiscussionColor();
            col5.BkgColor = "#C5CAE9";
            col5.BorderColor = "#9FA8DA";
            col5.UserColor = "#303F9F";

            DiscussionColor col6 = new DiscussionColor();
            col6.BkgColor = "#BBDEFB";
            col6.BorderColor = "#90CAF9";
            col6.UserColor = "#1976D2";

            DiscussionColor col7 = new DiscussionColor();
            col7.BkgColor = "#B3E5FC";
            col7.BorderColor = "#81D4FA";
            col7.UserColor = "#0277BD";

            DiscussionColor col8 = new DiscussionColor();
            col8.BkgColor = "#B2EBF2";
            col8.BorderColor = "#80DEEA";
            col8.UserColor = "#006064";

            DiscussionColor col9 = new DiscussionColor();
            col9.BkgColor = "#B2DFDB";
            col9.BorderColor = "#80CBC4";
            col9.UserColor = "#00796B";

            DiscussionColor col10 = new DiscussionColor();
            col10.BkgColor = "#C8E6C9";
            col10.BorderColor = "#A5D6A7";
            col10.UserColor = "#2E7D32";

            DiscussionColor col11 = new DiscussionColor();
            col11.BkgColor = "#DCEDC8";
            col11.BorderColor = "#C5E1A5";
            col11.UserColor = "#33691E";

            DiscussionColor col12 = new DiscussionColor();
            col12.BkgColor = "#F0F4C3";
            col12.BorderColor = "#E6EE9C";
            col12.UserColor = "#827717";

            DiscussionColor col13 = new DiscussionColor();
            col13.BkgColor = "#FFF9C4";
            col13.BorderColor = "#FFF59D";
            col13.UserColor = "#F57F17";

            DiscussionColor col14 = new DiscussionColor();
            col14.BkgColor = "#D7CCC8";
            col14.BorderColor = "#BCAAA4";
            col14.UserColor = "#5D4037";

            List<DiscussionColor> discColors = new List<DiscussionColor>();
            discColors.Add(col1);
            discColors.Add(col2);
            discColors.Add(col3);
            discColors.Add(col4);
            discColors.Add(col5);
            discColors.Add(col6);
            discColors.Add(col7);
            discColors.Add(col8);
            discColors.Add(col9);
            discColors.Add(col10);
            discColors.Add(col11);
            discColors.Add(col12);
            discColors.Add(col13);
            discColors.Add(col14);

            List<DiscussionColor> retDiscColors = new List<DiscussionColor>();
            int tmpIndex = new Random().Next(0, discColors.Count - 1);
            bool isFirstHalf = (tmpIndex < discColors.Count / 2);
            
            while (discColors.Count > 0) {
                
                int iHalf = discColors.Count / 2;
                int iStart = (isFirstHalf) ? 0 : iHalf - 1;
                int iEnd = (isFirstHalf) ? iHalf : discColors.Count - 1;
                int iIndex = new Random().Next(iStart, iEnd);

                if ((retDiscColors.Count / 2) % 2 == 1) {
                    iIndex = iIndex + 2;
                    if (iIndex >= discColors.Count) {
                        iIndex = 0;
                    }
                }


                int iSecondIndex = (iIndex >= iHalf) ? (iIndex - iHalf) : (iIndex + iHalf);
                
                //if (iLastIndex != -1) {
                //    int iMin = iIndex - 2;
                //    int iMax = iIndex + 2;
                //    if (iLastIndex >= iMin && iLastIndex <= iMax) {
                //        int iTmp = iSecondIndex;
                //        iSecondIndex = iIndex;
                //        iIndex = iTmp;
                //    }
                //}

                retDiscColors.Add(discColors.ElementAt(iIndex));
                retDiscColors.Add(discColors.ElementAt(iSecondIndex));
                //iLastIndex = (iSecondIndex - 1);
                //if (iIndex > iSecondIndex) {
                //    iLastIndex--;
                //}

                if (iIndex < iSecondIndex) {
                    discColors.RemoveAt(iIndex);
                    discColors.RemoveAt(iSecondIndex - 1);
                } else {
                    discColors.RemoveAt(iSecondIndex);
                    discColors.RemoveAt(iIndex - 1);
                }
            }

            return retDiscColors;
        }

        //protected static void SendMail(
        //    string strRecipients,
        //    string strCc,
        //    string strBcc,
        //    string strSubject,
        //    string strBody) {

        //    string bcc = strBcc;
        //    if (bcc == null) {
        //        bcc = "";
        //    }
        //    if (!String.IsNullOrWhiteSpace(bcc)) {
        //        bcc += ";";
        //    }
        //    bcc += RegetSettings.MailBcc;

        //    WsMail.OtWsMail wsMail = new WsMail.OtWsMail();
        //    wsMail.SendMailBcc(
        //        RegetSettings.MailSender,
        //        strRecipients,
        //        strCc,
        //        bcc,
        //        strSubject,
        //        strBody,
        //        null,
        //        (int)MailPriority.Normal);
        //}
#endregion

        #region Angular Controls

#region AngTextBox
        //public string GetAngularTextBox(
        //    string formName,
        //    string ngModel,
        //    string agIsReadOnly,
        //    string strLabelLeft,
        //    string strLabelTop,
        //    string strRootTagId,
        //    bool isReadOnly,
        //    bool isMandatory,
        //    int iWidth,
        //    string inputCssClass,
        //    string labelLeftCssClass,
        //    TextBoxType type) {

        //    return GetAngularTextBox(
        //        formName,
        //        ngModel,
        //        agIsReadOnly,
        //        strLabelLeft,
        //        strLabelTop,
        //        strRootTagId,
        //        isReadOnly,
        //        isMandatory,
        //        iWidth,
        //        inputCssClass,
        //        labelLeftCssClass,
        //        type,
        //        null,
        //        null);

        //}

//        public string GetAngularTextBox(
//            string formName,
//            string ngModel,
//            string agIsReadOnly,
//            string strLabelLeft,
//            string strLabelTop,
//            string strRootTagId,
//            bool isReadOnly,
//            bool isMandatory,
//            int iWidth,
//            string inputCssClass,
//            string labelLeftCssClass,
//            TextBoxType type,
//            string ngChange,
//            string placeHolder) {

//            string strWidth = "";

//            if (iWidth > 0) {
//                strWidth = "style=\"width:" + iWidth + "px;max-width:" + iWidth + "px;\"";
//            }

//            string cssHasValue = "";
//            if (isReadOnly) {
//                cssHasValue = " md-input-has-value";
//            }

//            string cssInputContainer = (isReadOnly) ? "reget-ang-md-input-container-readonly" : "reget-ang-md-input-container";
//            string cssLblLeft = null;
//            if (String.IsNullOrEmpty(labelLeftCssClass)) {
//                cssLblLeft = (isReadOnly) ? "reget-ang-lbl-control-left-readonly" : "reget-ang-lbl-control-left";
//            } else {
//                cssLblLeft = labelLeftCssClass;
//            }

//            string strRequired = (isMandatory) ? "required" : "";
//            if (isMandatory) {
//                strLabelLeft += " *";
//            }

//            string angPart = "";
//            if (strRootTagId.Contains("{{")) {
//                int iAngPartStart = strRootTagId.IndexOf("{");
//                angPart = strRootTagId.Substring(iAngPartStart);

//                strRootTagId = strRootTagId.Split('{')[0];

//            }

//            string ngShowRO = "";
//            string ngHideEdit = "";
//            if (!String.IsNullOrEmpty(agIsReadOnly)) {
//                ngShowRO = "ng-show=\"" + agIsReadOnly + "\"";
//                ngHideEdit = "ng-hide=\"" + agIsReadOnly + "\"";

//                //ngShowRO = "ng-if=\"" + agIsReadOnly + "==true\"";
//                //ngHideEdit = "ng-if=\"" + agIsReadOnly + "==false\"";
//            } else {
//                if (isReadOnly) {
//                    ngShowRO = "ng-show=\"true\"";
//                    ngHideEdit = "ng-hide=\"true\"";
//                } else {
//                    ngShowRO = "ng-show=\"false\"";
//                    ngHideEdit = "ng-hide=\"false\"";
//                }
//            }

//            string strInputClass = "reget-ang-data-control";
//            if (isMandatory) {
//                strInputClass += " reget-ang-mandatory-field";
//            }

//            if (!String.IsNullOrEmpty(inputCssClass)) {
//                strInputClass += " " + inputCssClass;
//            }

//            string strType = "";
//            if (type == TextBoxType.DecimalNumber) {
//                strType = " type=\"text\" onkeydown=\"validateDecimalNumber(event,'" + DecimalSeparator + "')\"";
//                strInputClass += " " + "reget-ang-data-control-number";
//            } else if (type == TextBoxType.IntNumber) {
//                strType = " type=\"text\" onkeydown=\"validateIntNumber(event)\"";
//                strInputClass += " " + "reget-ang-data-control-number";
//            }

//            StringBuilder sbTextBox = new StringBuilder();

//            sbTextBox.AppendLine("<div id=\"" + ANG_WRAPPER_PREFIX + strRootTagId + "\"" + angPart + " class=\"reget-ang-div-container\">");
//            sbTextBox.AppendLine("    <label id=\"" + ANG_LABEL_LEFT_PREFIX + strRootTagId + "\" class=\"control-label hidden-xs " + cssLblLeft + "\">" + strLabelLeft + " :" + "</label>");
//            sbTextBox.AppendLine("    <md-input-container id=\"" + ANG_CONTAINER_PREFIX + strRootTagId + "\" class=\"" + cssInputContainer + " " + cssHasValue + "\">");
//            sbTextBox.AppendLine("        <label class=\"hidden-sm hidden-md hidden-lg reget-ang-lbl-control-top\" style=\"min-width:150px;\">" + strLabelTop + "</label>");
//            sbTextBox.AppendLine("        <div " + ngShowRO + ">" + "{{" + ngModel + "}}" + "</div>");
//            if (!isReadOnly) {
//                string ngChangeFce = (String.IsNullOrEmpty(ngChange)) ? "" : " ng-change=\"" + ngChange + "\" ";
//                string inputPlaceholder = (String.IsNullOrEmpty(placeHolder)) ? "" : " placeholder=\"" + placeHolder + "\" ";
//                sbTextBox.AppendLine("        <input " + strRequired + " id=\"" + "txt" + strRootTagId + "\" name=\"" + "txt" + strRootTagId + "\" ng-model=\"" + ngModel + "\"" 
//                    + ngChangeFce + strType +  " class=\"" + strInputClass + "\" " + strWidth + " " + ngHideEdit + inputPlaceholder + "/>");
//            }
//            sbTextBox.AppendLine("        <div ng-messages=\"" + formName + "." + "txt" + strRootTagId + ".$error\" >");
//#if !TEST
//            sbTextBox.AppendLine("              <div ng-message=\"required\" class=\"reget-ang-controll-invalid-msg\">" + RequestResource.EnterDecimalNumber + "</div>");
//#endif
//            sbTextBox.AppendLine("        </div>");
//            sbTextBox.AppendLine("    </md-input-container>");
//            sbTextBox.AppendLine("</div>");

           
//            //if (!String.IsNullOrEmpty(agIsReadOnly) || isReadOnly) {
//            //    sbTextBox.AppendLine("<div class=\"reget-ang-div-container\" style=\"margin-right:20px;float:left;\" " + ngShowRO + " >");
//            //    sbTextBox.AppendLine("    <label id=\"" + ANG_LABEL_LEFT_PREFIX + strRootTagId + "\" class=\"control-label hidden-xs " + cssLblLeft + "\">" + strLabelLeft + " :" + "</label>");
//            //    sbTextBox.AppendLine("    <md-input-container class=\"reget-ang-md-input-container\" style=\"min-width:300px;\">");
//            //    sbTextBox.AppendLine("        <label class=\"hidden-sm hidden-md hidden-lg reget-ang-lbl-control-top\" style=\"min-width:150px;\">" + strLabelTop + "</label>");
//            //    sbTextBox.AppendLine("          <input value=\"{{" + ngModel + "}}\" ng-show=\"1==0\" />");
//            //    sbTextBox.AppendLine("          <div>" + "{{" + ngModel + "}}" + "</div>");
//            //    sbTextBox.AppendLine("    </md-input-container>");
//            //    sbTextBox.AppendLine("</div>");
//            //}

//            //if (!String.IsNullOrEmpty(agIsReadOnly) || !isReadOnly) {
//            //    sbTextBox.AppendLine("<div id=\"" + ANG_WRAPPER_PREFIX + strRootTagId + "\"" + angPart + " class=\"reget-ang-div-container\" " + ngHideEdit + " >");
//            //    sbTextBox.AppendLine("    <label id=\"" + ANG_LABEL_LEFT_PREFIX + strRootTagId + "\" class=\"control-label hidden-xs " + cssLblLeft + "\">" + strLabelLeft + " :" + "</label>");
//            //    sbTextBox.AppendLine("    <md-input-container id=\"" + ANG_CONTAINER_PREFIX + strRootTagId + "\" class=\"" + cssInputContainer + " " + cssHasValue + "\">");
//            //    sbTextBox.AppendLine("        <label class=\"hidden-sm hidden-md hidden-lg reget-ang-lbl-control-top\" style=\"min-width:150px;\">" + strLabelTop + "</label>");
//            //    sbTextBox.AppendLine("        <input " + strRequired + " id=\"" + "txt" + strRootTagId + "\" name=\"" + "txt" + strRootTagId + "\" ng-model=\"" + ngModel + "\"" + " class=\"" + strClass + "\" " + strWidth + " />");
//            //    sbTextBox.AppendLine("        <div ng-messages=\"" + formName + "." + "txt" + strRootTagId + ".$error\" >");
//            //    sbTextBox.AppendLine("              <div ng-message=\"required\" class=\"reget-ang-controll-invalid-msg\">" + RequestResource.MandatoryTextField + "</div>");
//            //    sbTextBox.AppendLine("        </div>");
//            //    sbTextBox.AppendLine("    </md-input-container>");
//            //    sbTextBox.AppendLine("</div>");
//            //}



//            return sbTextBox.ToString();
//        }

        //public string GetAngularTextBox(
        //    string formName,
        //    string ngModel,
        //    string strLabelLeft,
        //    string strLabelTop,
        //    string strRootTagId,
        //    bool isReadOnly,
        //    bool isMandatory) {

        //    return GetAngularTextBox(
        //        formName,
        //        ngModel,
        //        null,
        //        strLabelLeft,
        //        strLabelTop,
        //        strRootTagId,
        //        isReadOnly,
        //        isMandatory,
        //        -1,
        //        null,
        //        null,
        //        TextBoxType.Text);
        //}

        //public string GetAngularTextBox(
        //    string formName,
        //    string ngModel,
        //    string strLabelLeft,
        //    string strLabelTop,
        //    string strRootTagId,
        //    bool isReadOnly,
        //    bool isMandatory,
        //    int iWidth) {

        //    return GetAngularTextBox(
        //        formName,
        //        ngModel,
        //        null,
        //        strLabelLeft,
        //        strLabelTop,
        //        strRootTagId,
        //        isReadOnly,
        //        isMandatory,
        //        iWidth,
        //        null,
        //        null,
        //        TextBoxType.Text);
        //}

        //public string GetAngularTextBox(
        //    string formName,
        //    string ngModel,
        //    string agIsReadOnly,
        //    string strLabelLeft,
        //    string strLabelTop,
        //    string strRootTagId,
        //    bool isMandatory) {

        //    return GetAngularTextBox(
        //        formName,
        //        ngModel,
        //        agIsReadOnly,
        //        strLabelLeft,
        //        strLabelTop,
        //        strRootTagId,
        //        false,
        //        isMandatory,
        //        -1,
        //        null,
        //        null,
        //        TextBoxType.Text);
        //}

        //public string GetAngularTextBox(
        //    string formName,
        //    string ngModel,
        //    string agIsReadOnly,
        //    string strLabelLeft,
        //    string strLabelTop,
        //    string strRootTagId,
        //    bool isMandatory,
        //    int iWidth) {

        //    return GetAngularTextBox(
        //        formName,
        //        ngModel,
        //        agIsReadOnly,
        //        strLabelLeft,
        //        strLabelTop,
        //        strRootTagId,
        //        false,
        //        isMandatory,
        //        iWidth,
        //        null,
        //        null,
        //        TextBoxType.Text);
        //}

        //public string GetAngularTextBox(
        //    string formName,
        //    string ngModel,
        //    string agIsReadOnly,
        //    string strLabelLeft,
        //    string strLabelTop,
        //    string strRootTagId,
        //    bool isMandatory,
        //    string inputCssClass) {

        //    return GetAngularTextBox(
        //        formName,
        //        ngModel,
        //        agIsReadOnly,
        //        strLabelLeft,
        //        strLabelTop,
        //        strRootTagId,
        //        false,
        //        isMandatory,
        //        -1,
        //        inputCssClass,
        //        null,
        //        TextBoxType.Text);
        //}
#endregion

#region AngTextArea
//        public string GetAngularTextArea(
//            string formName,
//            string ngModel,
//            string agIsReadOnly,
//            string strLabelLeft,
//            string strLabelTop,
//            string strRootTagId,
//            bool isReadOnly,
//            bool isMandatory,
//            string inputCssClass,
//            string labelLeftCss) {

//            //string strWidth = "";

            
//            string cssHasValue = "";
//            if (isReadOnly) {
//                cssHasValue = " md-input-has-value";
//            }

//            string cssInputContainer = (isReadOnly) ? "reget-ang-md-input-container-readonly" : "reget-ang-md-input-container";
//            //string cssLblLeft = (isReadOnly) ? "reget-ang-lbl-control-left-readonly" : "reget-ang-lbl-control-left";
//            string cssLblLeft = "";
//            if (!String.IsNullOrEmpty(labelLeftCss)) {
//                cssLblLeft = labelLeftCss;
//            }

//            string strRequired = (isMandatory) ? "required" : "";
//            if (isMandatory) {
//                strLabelLeft += " *";
//            }

//            string angPart = "";
//            if (strRootTagId.Contains("{{")) {
//                int iAngPartStart = strRootTagId.IndexOf("{");
//                angPart = strRootTagId.Substring(iAngPartStart);

//                strRootTagId = strRootTagId.Split('{')[0];

//            }

//            string ngShowRO = "";
//            string ngHideEdit = "";
           
//            if (!String.IsNullOrEmpty(agIsReadOnly)) {
//                ngShowRO = "ng-show=\"" + agIsReadOnly + "\"";
//                ngHideEdit = "ng-hide=\"" + agIsReadOnly + "\"";
                
//            }

//            string strClass = "reget-ang-data-control";
//            if (isMandatory) {
//                strClass += " reget-ang-mandatory-field";
//            }

//            if (!String.IsNullOrEmpty(inputCssClass)) {
//                strClass += " " + inputCssClass;
//            }

//            //string strLeftLabelStyle = "";
//            //if (labelMinWidth > 0) {
//            //    strLeftLabelStyle = "style=\"min-width:" + labelMinWidth + "px;\"";
//            //}

//            StringBuilder sbTextBox = new StringBuilder();

//            sbTextBox.AppendLine("<div id=\"" + ANG_WRAPPER_PREFIX + strRootTagId + "\"" + angPart + " class=\"reget-ang-div-container\">");
//            sbTextBox.AppendLine("  <table style=\"width:100%\"><tr>");
//            sbTextBox.AppendLine("    <td class=\"class=\"hidden-xs\" style=\"vertical-align:top;\"><label id=\"" + ANG_LABEL_LEFT_PREFIX + strRootTagId + "\" class=\"control-label hidden-xs " + cssLblLeft + "\">" + strLabelLeft + " :" + "</label></td>");
//            sbTextBox.AppendLine("    <td style=\"width:100%;\"><md-input-container id=\"" + ANG_CONTAINER_PREFIX + strRootTagId + "\" class=\"" + cssInputContainer + " " + cssHasValue + "\" style=\"width:100%;\">");
//            sbTextBox.AppendLine("        <label class=\"hidden-sm hidden-md hidden-lg reget-ang-lbl-control-top\" style=\"min-width:150px;\">" + strLabelTop + "</label>");
//            if (!String.IsNullOrEmpty(agIsReadOnly) && !isReadOnly) {
//                sbTextBox.AppendLine("        <div " + ngShowRO + " style=\"margin-top:6px;\">" + "{{" + ngModel + "}}" + "</div>");
//            }
//            sbTextBox.AppendLine("        <textarea " + strRequired + " id=\"" + "txt" + strRootTagId + "\" name=\"" + "txt" + strRootTagId + "\" ng-model=\"" + ngModel + "\"" + " class=\"" + strClass + "\" " + " " + ngHideEdit + "></textarea>");
//            sbTextBox.AppendLine("        <div ng-messages=\"" + formName + "." + "txt" + strRootTagId + ".$error\" >");
//#if !TEST
//            sbTextBox.AppendLine("              <div ng-message=\"required\" class=\"reget-ang-controll-invalid-msg\">" + RequestResource.MandatoryTextField + "</div>");
//#endif
//            sbTextBox.AppendLine("        </div>");
//            sbTextBox.AppendLine("    </md-input-container></td>");
//            sbTextBox.AppendLine("  </tr></table>");
//            sbTextBox.AppendLine("</div>");


//            return sbTextBox.ToString();
//        }
#endregion

#region AngDatePicker
        //public string GetAngularDatePicker(
        //    string formName,
        //    string ngModel,
        //    //bool isTime,
        //    string strLabelLeft,
        //    string strLabelTop,
        //    string strRootTagId,
        //    bool isReadOnly,
        //    bool isMandatory,
        //    string leftLabelCss) {

        //    return GetAngularDatePicker(
        //        formName,
        //        null,
        //        ngModel,
        //        //isTime,
        //        strLabelLeft,
        //        strLabelTop,
        //        strRootTagId,
        //        isReadOnly,
        //        isMandatory,
        //        leftLabelCss);
        //}

        //public string GetAngularDatePicker(
        //    string formName,
        //    string agControlerName,
        //    string ngModel,
        //    //bool isTime,
        //    string strLabelLeft,
        //    string strLabelTop,
        //    string strRootTagId,
        //    bool isReadOnly,
        //    bool isMandatory,
        //    string leftLabelCss) {

        //    string agControlerNameDot = (String.IsNullOrEmpty(agControlerName)) ? "" : agControlerName + ".";


        //    string cssHasValue = "";

        //    string cssInputContainer = (isReadOnly) ? "reget-ang-md-input-container-readonly" : "reget-ang-md-input-container";
        //    string cssLblLeft = (isReadOnly) ? "reget-ang-lbl-control-left-readonly" : leftLabelCss;

        //    string strRequired = (isMandatory) ? "required" : "";
        //    if (isMandatory && !String.IsNullOrWhiteSpace(strLabelLeft)) {
        //        strLabelLeft += " *";
        //    }

        //    if (!String.IsNullOrWhiteSpace(strLabelLeft)) {
        //        strLabelLeft += " :";
        //    }

        //    string angPart = "";
        //    if (strRootTagId.Contains("{{")) {
        //        int iAngPartStart = strRootTagId.IndexOf("{");
        //        angPart = strRootTagId.Substring(iAngPartStart);

        //        strRootTagId = strRootTagId.Split('{')[0];

        //    }

        //    //string strWidth = (isTime) ? "max-width:180px;min-width:180px;" : "max-width:150px;min-width:150px;";
        //    string strWidth = "max-width:150px;min-width:150px;";

        //    StringBuilder sbDatePicker = new StringBuilder();
        //    sbDatePicker.AppendLine("<div id=\"" + ANG_WRAPPER_PREFIX + strRootTagId + "\"" + angPart + " class=\"reget-ang-div-container\">");
        //    sbDatePicker.AppendLine("    <label id=\"" + ANG_LABEL_LEFT_PREFIX + strRootTagId + "\" class=\"control-label hidden-xs " + cssLblLeft + "\" style=\"margin-top:3px;\">" + strLabelLeft + "</label>");
        //    sbDatePicker.AppendLine("    <md-input-container id=\"" + ANG_CONTAINER_PREFIX + strRootTagId + "\" class=\"" + cssInputContainer + " " + cssHasValue + "\" style=\"" + strWidth + "\">");
        //    sbDatePicker.AppendLine("        <label class=\"hidden-sm hidden-md hidden-lg reget-ang-lbl-control-top\" style=\"min-width:150px;margin-right:0px;\">" + strLabelTop + "</label>");
        //    if (isReadOnly) {
        //        //sbDatePicker.AppendLine("<div>" + "{{" + ngModel + "}}" + "</div>");
        //    } else {
        //        string strClass = "";
        //        if (isMandatory) {
        //            //strClass += "class=\"reget-ang-mandatory-datepicker-field\"";
        //            //if (isTime) {
        //            //    strClass = "class=\"reget-ang-mandatory-datetimepicker-field reget-ang-datetimepicker\"";
        //            //} else {
        //                strClass = "class=\"reget-ang-mandatory-datepicker-field\"";
        //            //}
        //        }
                                                                                
        //        //string strIsTime = (isTime) ? "1" : "0";
        //        sbDatePicker.AppendLine("        <md-datepicker name=\"" + "dtp" + strRootTagId + "\" ng-model=\"" + agControlerNameDot + ngModel + "\"" + 
        //            " ng-change=\"" + agControlerNameDot + "datePickerChange()\" " +
        //            //" ng-blur=\"" + agControlerNameDot + "datePickerBlur(" + strIsTime + "," + "'" + formName + "'," + "'" + strRootTagId + "')\" " +
        //            strRequired +
        //            //" md-placeholder=\"" + ((isTime) ? RequestResource.EnterDateTime : RequestResource.EnterDate) + "\"" +
        //            " md-placeholder=\"" + RequestResource.EnterDate + "\"" +
        //            "  md-hide-icons=\"calendar\"" + strClass + " style=\"float:left;\">" + 
        //            "</md-datepicker>");
                
        //        //string strRequiredMsg = (isTime) ? String.Format(RequestResource.EnterDateInFormat, DateTimePickerFormat, DateTime.Now.ToString(DateTimePickerFormat)) :
        //        //    String.Format(RequestResource.EnterDateInFormat, DatePickerFormat, DateTime.Now.ToString(DatePickerFormat));
        //        string strRequiredMsg = String.Format(RequestResource.EnterDateInFormat, DatePickerFormat, DateTime.Now.ToString(DatePickerFormat));
        //        sbDatePicker.AppendLine("        <div id=\"" + "msg" + strRootTagId + "\" ng-messages=\"" + formName + "." + "dtp" + strRootTagId + ".$error\" style=\"color:maroon\" role=\"alert\">");
        //        sbDatePicker.AppendLine("              <div ng-message=\"required\" class=\"reget-ang-controll-invalid-msg\">" + strRequiredMsg + "</div>");
        //        sbDatePicker.AppendLine("              <div ng-message=\"valid\" class=\"reget-ang-controll-invalid-msg\">" + strRequiredMsg + "</div>");
        //        sbDatePicker.AppendLine("        </div>");

                
        //    }
        //    sbDatePicker.AppendLine("    </md-input-container>");
        //    sbDatePicker.AppendLine("</div>");

        //    //if (isTime) {
        //    //    sbDatePicker.AppendLine("<div>");
        //    //    sbDatePicker.AppendLine("<input id=\"dtm" + strRootTagId + "\" type=\"text\">");
        //    //    sbDatePicker.AppendLine("</div>");
        //    //}

        //    return sbDatePicker.ToString();
        //}



        //private string GetDatePickerButton() {
        //    StringBuilder sbDatePickerBtn = new StringBuilder();
        //    sbDatePickerBtn.Append("<button tabindex=\"-1\" class=\"\" aria-hidden=\"true\" type=\"button\" ng-click=\"ctrl.openCalendarPane($event)\" ng-transclude=\"\">");
        //    sbDatePickerBtn.Append("<md-icon class=\"md-datepicker-calendar-icon ng-scope\" role=\"img\" aria-label=\"md-calendar\" md-svg-src=\"" + GetRootUrl() + "Content/Images/Error.png" + "\">");
        //    sbDatePickerBtn.Append("</md-icon>");
        //    sbDatePickerBtn.Append("</button>");

        //    return sbDatePickerBtn.ToString();
        //}



#endregion

#region AngDropDown
//        public string GetAngularDropdownBox(
//            string formName,
//            string agSourceList,
//            string agModel,
//            string agIdItem,
//            string agTextItem,
//            string agSelectedText,
//            string agIsReadOnly,
//            string agItemDisable,
//            string strRootTagId,
//            string onSelectFunct,
//            string strLabelLeft,
//            string strLabelTop,
//            string leftLabelCss,
//            bool isReadOnly,
//            bool isMandatory,
//            bool isDisplayNoneSelect,
//            bool isLeftLabelDisplayed,
//            bool isBold,
//            int iLabelWidth,
//            string strReadOnlyItemHtml,
//            string strItemHtml) {

//            StringBuilder sbTextBox = new StringBuilder();

//            //string strRequired = (isMandatory) ? "ng-required=\"true\" required" : "";
//            string strRequired = (isMandatory) ? "required" : "";
//            if (isMandatory) {
//                strLabelLeft += " *";
//                strLabelTop += " *";
//            }


//            string strCssBold = (isBold) ? " reget-bold" : "";

//            string strWidth = "";

//            if (iLabelWidth > 0) {
//                strWidth = "style=\"min-width:" + iLabelWidth + "px;text-align:right;\"";
//            }

//            string ngShowRO = "";
//            string ngHideEdit = "";
//            if (!String.IsNullOrEmpty(agIsReadOnly)) {
//                ngShowRO = "ng-show=\"" + agIsReadOnly + "\"";
//                ngHideEdit = "ng-hide=\"" + agIsReadOnly + "\"";
//                //ngShowRO = "ng-if=\"" + agIsReadOnly + "==true\"";
//                //ngHideEdit = "ng-if=\"" + agIsReadOnly + "==false\"";
//            }

//            #region ReadOnly
//            if (!String.IsNullOrEmpty(agIsReadOnly) || isReadOnly) {
//                string cssInputContainer = (isReadOnly) ? "reget-ang-md-input-container-readonly" : "reget-ang-md-input-container";

//                sbTextBox.AppendLine("<div id=\"" + ANG_WRAPPER_PREFIX + strRootTagId + "\" " + ngShowRO + " class=\"reget-ang-div-container-readonly\" >");
//                if (isLeftLabelDisplayed) {
//                    sbTextBox.AppendLine("    <label id=\"" + ANG_LABEL_LEFT_PREFIX + strRootTagId + "\" class=\"control-label hidden-xs " + leftLabelCss + "\" " + strWidth + ">" + strLabelLeft + " :" + "</label>");
//                } else {
//                    sbTextBox.AppendLine("<label class=\"hidden-xs reget-ang-lbl-control-left\"></label>");
//                }
//                sbTextBox.AppendLine("    <md-input-container id=\"" + ANG_CONTAINER_PREFIX + strRootTagId + "\" class=\"" + cssInputContainer + strCssBold + " md-input-has-value" + "\" style=\"margin-top:2px;\">");
//                sbTextBox.AppendLine("          <label class=\"hidden-sm hidden-md hidden-lg reget-ang-lbl-control-top\"" + strWidth + " >" + strLabelTop + "</label>");
//                sbTextBox.AppendLine("          <div>" + "{{" + agSelectedText + "}}" + "</div>");
//                sbTextBox.AppendLine("    </md-input-container>");
//                sbTextBox.AppendLine("</div>");

//                //return sbTextBox.ToString();
//            }
//            #endregion

//            if (!isReadOnly) {
//                sbTextBox.AppendLine("<div id=\"" + ANG_WRAPPER_PREFIX + strRootTagId + "\" class=\"reget-ang-div-container\" " + ngHideEdit + " >");
//                if (isLeftLabelDisplayed) {
//                    sbTextBox.AppendLine("<label id=\"" + ANG_LABEL_LEFT_PREFIX + strRootTagId + "\" class=\"control-label hidden-xs " + leftLabelCss + "\" " + strWidth + ">" + strLabelLeft + " :</label>");
//                }

//                sbTextBox.AppendLine("<md-input-container id=\"" + ANG_CONTAINER_PREFIX + strRootTagId + "\" class=\"reget-ang-md-input-container\" >");
//                sbTextBox.AppendLine("    <label class=\"hidden-sm hidden-md hidden-lg reget-ang-lbl-control-top\"> " + strLabelTop + "</label>");

//                string strClass = "reget-ang-data-control";
//                if (isMandatory) {
//                    strClass += " reget-ang-mandatory-cmb-field";
//                }

//                string strNgChange = "";
//                if (!String.IsNullOrEmpty(onSelectFunct)) {
//                    strNgChange = "ng-change=\"" + onSelectFunct + ";\"";
//                }
//                sbTextBox.AppendLine("    <md-select " + strRequired + " name=\"" + "cmb" + strRootTagId + "\" id=\"" + "cmb" + strRootTagId + "\" ng-model=\"" + agModel + "\" class=\"" + strClass + "" + strCssBold + "\" " + strNgChange + " md-no-asterisk>");
//                if (!isMandatory || isDisplayNoneSelect) {
//#if !TEST //because of missing resources in Test Project
//                    sbTextBox.AppendLine("        <md-option><em>" + RequestResource.CancelSelect + "</em></md-option>");
//#endif
//                }
//                string ngDisabled = "";
//                if (!String.IsNullOrEmpty(agItemDisable)) {
//                    ngDisabled = " ng-disabled=\"" + agItemDisable + "\"";
//                }
//                sbTextBox.AppendLine("        <md-option ng-repeat=\"liElement in " + agSourceList + "\" ng-value=\"liElement." + agIdItem + "\"" + ngDisabled + " > ");
//                if (strItemHtml == null) {
//                    sbTextBox.AppendLine("            {{liElement." + agTextItem + "}}");
//                } else {
//                    sbTextBox.AppendLine("            " + strItemHtml);
//                }
//                sbTextBox.AppendLine("        </md-option>");
//                sbTextBox.AppendLine("    </md-select>");

//                sbTextBox.AppendLine("        <div ng-messages=\"" + formName + "." + "cmb" + strRootTagId + ".$error\" style=\"color:maroon\" role=\"alert\">");
//#if !TEST
//                sbTextBox.AppendLine("              <div ng-message=\"required\" class=\"reget-ang-controll-invalid-msg\">" + RequestResource.MandatoryTextField + "</div>");
//#endif
//                sbTextBox.AppendLine("        </div>");
//                //sbTextBox.AppendLine("              <div class=\"reget-ang-controll-invalid-msg\">" + RequestResource.MandatoryTextField + "</div>");

//                sbTextBox.AppendLine("</md-input-container>");
//                sbTextBox.AppendLine("</div>");
//            }

//            return sbTextBox.ToString();
//        }

        //public string GetAngularDropdownBox(
        //    string formName,
        //    string agSourceList,
        //    string agModel,
        //    string agIdItem,
        //    string agTextItem,
        //    string agSelectedText,
        //    string strRootTagId,
        //    string onSelectFunct,
        //    string strLabelLeft,
        //    string strLabelTop,
        //    string leftLabelCss,
        //    bool isReadOnly,
        //    bool isMandatory,
        //    bool isDisplayNoneSelect,
        //    bool isLeftLabelDisplayed,
        //    bool isBold,
        //    int iLabelWidth,
        //    string strReadOnlyItemHtml,
        //    string strItemHtml) {

        //    return GetAngularDropdownBox(
        //            formName,
        //            agSourceList,
        //            agModel,
        //            agIdItem,
        //            agTextItem,
        //            agSelectedText,
        //            null,
        //            null,
        //            strRootTagId,
        //            onSelectFunct,
        //            strLabelLeft,
        //            strLabelTop,
        //            leftLabelCss,
        //            isReadOnly,
        //            isMandatory,
        //            isDisplayNoneSelect,
        //            isLeftLabelDisplayed,
        //            isBold,
        //            iLabelWidth,
        //            strReadOnlyItemHtml,
        //            strItemHtml);
        //}

        //public string GetAngularDropdownBox(
        //    string formName,
        //    string agSourceList,
        //    string agModel,
        //    string agIdItem,
        //    string agTextItem,
        //    string agSelectedText,
        //    string agItemDisabled,
        //    string strRootTagId,
        //    string onSelectFunct,
        //    string strLabelLeft,
        //    string strLabelTop,
        //    string leftLabelCss,
        //    bool isReadOnly,
        //    bool isMandatory,
        //    bool isDisplayNoneSelect,
        //    bool isLeftLabelDisplayed,
        //    bool isBold,
        //    int iLabelWidth,
        //    string strReadOnlyItemHtml,
        //    string strItemHtml) {

        //    return GetAngularDropdownBox(
        //            formName,
        //            agSourceList,
        //            agModel,
        //            agIdItem,
        //            agTextItem,
        //            agSelectedText,
        //            null,
        //            agItemDisabled,
        //            strRootTagId,
        //            onSelectFunct,
        //            strLabelLeft,
        //            strLabelTop,
        //            leftLabelCss,
        //            isReadOnly,
        //            isMandatory,
        //            isDisplayNoneSelect,
        //            isLeftLabelDisplayed,
        //            isBold,
        //            iLabelWidth,
        //            strReadOnlyItemHtml,
        //            strItemHtml);
        //}

        //public string GetAngularDropdownBox(
        //    string formName,
        //    string agSourceList,
        //    string agModel,
        //    string agIdItem,
        //    string agTextItem,
        //    string agSelectedText,
        //    string agIsReadOnly,
        //    string strRootTagId,
        //    string onSelectFunct,
        //    string strLabelLeft,
        //    string strLabelTop,
        //    string leftLabelCss,
        //    bool isMandatory,
        //    bool isDisplayNoneSelect,
        //    bool isLeftLabelDisplayed,
        //    bool isBold,
        //    int iLabelWidth,
        //    string strReadOnlyItemHtml,
        //    string strItemHtml) {

        //    return GetAngularDropdownBox(
        //            formName,
        //            agSourceList,
        //            agModel,
        //            agIdItem,
        //            agTextItem,
        //            agSelectedText,
        //            agIsReadOnly,
        //            null,
        //            strRootTagId,
        //            onSelectFunct,
        //            strLabelLeft,
        //            strLabelTop,
        //            leftLabelCss,
        //            false,
        //            isMandatory,
        //            isDisplayNoneSelect,
        //            isLeftLabelDisplayed,
        //            isBold,
        //            iLabelWidth,
        //            strReadOnlyItemHtml,
        //            strItemHtml);
        //}



        //public string GetAngularDropdownBox(
        //                string formName,
        //                string agSourceList,
        //                string agModel,
        //                string agIdItem,
        //                string agTextItem,
        //                string agSelectedText,
        //                string agIsReadOnly,
        //                string strRootTagId,
        //                string onSelectFunct,
        //                string strLabelLeft,
        //                string strLabelTop,
        //                string leftLabelCss,
        //                bool isMandatory) {

        //    return GetAngularDropdownBox(
        //        formName,
        //        agSourceList,
        //        agModel,
        //        agIdItem,
        //        agTextItem,
        //        agSelectedText,
        //        agIsReadOnly,
        //        null,
        //        strRootTagId,
        //        onSelectFunct,
        //        strLabelLeft,
        //        strLabelTop,
        //        leftLabelCss,
        //        false,
        //        isMandatory,
        //        false,
        //        true,
        //        false,
        //        -1,
        //        null,
        //        null);
        //}

        //public string GetAngularDropdownBox(
        //                string formName,
        //                string agSourceList,
        //                string agModel,
        //                string agIdItem,
        //                string agTextItem,
        //                string agSelectedText,
        //                string strRootTagId,
        //                string onSelectFunct,
        //                string strLabelLeft,
        //                string strLabelTop,
        //                string leftLabelCss,
        //                bool isReadOnly,
        //                bool isMandatory) {

        //    return GetAngularDropdownBox(
        //        formName,
        //        agSourceList,
        //        agModel,
        //        agIdItem,
        //        agTextItem,
        //        agSelectedText,
        //        strRootTagId,
        //        onSelectFunct,
        //        strLabelLeft,
        //        strLabelTop,
        //        leftLabelCss,
        //        isReadOnly,
        //        isMandatory,
        //        false,
        //        true,
        //        false,
        //        -1,
        //        null,
        //        null);
        //}
#endregion

#region AngAutoComplete
        //public string GetAngularAutocomplete(
        //        string strElementId,
        //        //string strInputId,
        //        bool isRequired,
        //        string strLabelLeft,
        //        string strLabelTop,
        //        bool isLeftLabelDisplayed,
        //        string leftLabelCss,
        //        string mdSelectItem,
        //        string mdSearchText,
        //        string mdSelectedItemChange,
        //        string mdItems,
        //        string mdItemText,
        //        string placeholder,
        //        string mdInputId,
        //        string mdItemTemplate,
        //        string onBlur,
        //        string strClass,
        //        int minLength) {
        //    StringBuilder sbAuto = new StringBuilder();

        //    //sbAuto.AppendLine("<div style=\"clear:both;\"></div>");
        //    if (isRequired) {
        //        strLabelLeft += " *";
        //    }
        //    sbAuto.AppendLine("<div id=\"" + ANG_WRAPPER_PREFIX + strElementId + "\">");
        //    if (isLeftLabelDisplayed) {
        //        sbAuto.AppendLine("<label id=\"" + ANG_LABEL_LEFT_PREFIX + strElementId + "\" class=\"control-label hidden-xs " + leftLabelCss + "\" style=\"float:left;margin-top:10px;\">" + strLabelLeft + " :</label>");
        //    }

        //    if (isRequired) {
        //        strLabelTop += " *";
        //    }


        //    if (isRequired) {
        //        //strStyle = "style=\"border-bottom:3px solid #777\"";
        //        if (String.IsNullOrEmpty(strClass)) {
        //            strClass = "reget-ang-mandatory-autocomplete";
        //        } else {
        //            strClass += " " + "reget-ang-mandatory-autocomplete";
        //        }
        //    }

        //    sbAuto.AppendLine(" <md-input-container class=\"reget-ang-md-input-container md-input-has-value\" style=\"float:left;\">");
        //    sbAuto.AppendLine("     <label class=\"hidden-sm hidden-md hidden-lg reget-ang-lbl-control-top\" style=\"margin-bottom:8px;\">" + strLabelTop + "</label>");
        //    sbAuto.AppendLine(" </md-input-container>");
        //    sbAuto.AppendLine(" <div style=\"float:left;\">");
        //    sbAuto.AppendLine("     <md-autocomplete flex id=\"" + strElementId + "\"");
        //    sbAuto.AppendLine("                          name=\"" + strElementId + "\"");
        //    if (isRequired) {
        //        sbAuto.AppendLine("                          required");
        //    }
        //    sbAuto.AppendLine("                          md-input-minlength=\"" + minLength + "\"");
        //    sbAuto.AppendLine("                          md-min-length=\"" + minLength + "\"");
        //    sbAuto.AppendLine("                          ng-disabled=\"false\"");
        //    sbAuto.AppendLine("                          md-no-cache=\"true\"");
        //    sbAuto.AppendLine("                          md-delay=\"300\"");
        //    sbAuto.AppendLine("                          md-selected-item=\"" + mdSelectItem + "\"");
        //    sbAuto.AppendLine("                          md-search-text=\"" + mdSearchText + "\"");
        //    if (!String.IsNullOrEmpty(mdSelectedItemChange)) {
        //        sbAuto.AppendLine("                          md-selected-item-change=\"" + mdSelectedItemChange + "\"");
        //    }
        //    sbAuto.AppendLine("                          md-items=\"" + mdItems + "\"");
        //    sbAuto.AppendLine("                          md-item-text=\"" + mdItemText + "\"");
        //    sbAuto.AppendLine("                          placeholder=\"" + placeholder + "\"");
        //    sbAuto.AppendLine("                          md-input-id=\"" + mdInputId + "\"");
        //    sbAuto.AppendLine("                          md-input-name=\"" + mdInputId + "\"");
        //    if (!String.IsNullOrEmpty(onBlur)) {
        //        sbAuto.AppendLine("                          ng-blur=\"" + onBlur + "\"");
        //    }
        //    string autClass = "reget-autocomplete-white";
        //    if (!String.IsNullOrEmpty(strClass)) {
        //        autClass += " " + strClass;
        //    } 
        //    sbAuto.AppendLine("                          class=\"" + strClass + "\">");

        //    sbAuto.AppendLine(" <md-item-template>");
        //    sbAuto.AppendLine(mdItemTemplate);
        //    sbAuto.AppendLine(" </md-item-template>");

        //    sbAuto.AppendLine(" <md-not-found>");
        //    sbAuto.AppendLine("     \"{{" + mdSearchText + "}}\" " + @RequestResource.NotFound + ".");
        //    sbAuto.AppendLine(" </md-not-found>");



        //    sbAuto.AppendLine(" </md-autocomplete>");

        //    sbAuto.AppendLine(" <div style=\"color:maroon;\" role=\"alert\">");
        //    sbAuto.AppendLine("     <div id=\"" + strElementId + ANG_ERR_MSG_MANDATORY + "\" class=\"reget-ang-controll-invalid-msg\" style=\"display:none;line-height:14px;color:rgb(221,24,0);padding-top:5px;font-size:12px;\">" + @RequestResource.MandatoryTextField + "</div>");
        //    sbAuto.AppendLine(" </div>");

        //    sbAuto.AppendLine(" </div>");

        //    sbAuto.AppendLine("</div>");

        //    sbAuto.AppendLine("<div style=\"clear:both;\"></div>");

        //    if (isRequired) {
        //        sbAuto.AppendLine("<input id=\"" + "custLocV_" + mdInputId + "\" class=\"reget-custom-loc-valid-text\" value=\"" + RequestResource.MandatoryTextField + "\" />");
        //    }

        //    return sbAuto.ToString();
        //}
#endregion

#region AngCheckbox
        //public string GetAngularCheckbox(
        //    string strRootTagId,
        //    string ckbText,
        //    string agIsChecked,
        //    string agIsReadOnly,
        //    string agClick,
        //    bool isReadOnly) {

        //    StringBuilder sbCkb = new StringBuilder();

        //    string ngShowRO = "";
        //    string ngHideEdit = "";

        //    if (!String.IsNullOrEmpty(agIsReadOnly)) {
        //        ngShowRO = "ng-show=\"" + agIsReadOnly + "\"";
        //        ngHideEdit = "ng-hide=\"" + agIsReadOnly + "\"";
        //        //ngShowRO = "ng-if=\"" + agIsReadOnly + "==true\"";
        //        //ngHideEdit = "ng-if=\"" + agIsReadOnly + "==false\"";
        //    }

        //    //Enabled
        //    if (!String.IsNullOrEmpty(agIsReadOnly) || !isReadOnly) {
        //        sbCkb.AppendLine("<div " + ngHideEdit + " style=\"float:left;\">");
        //        sbCkb.AppendLine("  <md-checkbox id=\"" + strRootTagId + "\" aria-label=\"" +
        //            ckbText + "\" class=\"reget-blue\" ng-checked=\"" + agIsChecked + "\" ng-click=\"" +
        //            agClick + "\" style=\"margin-bottom:0px;\"> ");
        //        sbCkb.AppendLine(ckbText);
        //        sbCkb.AppendLine("  </md-checkbox>");
        //        sbCkb.AppendLine("</div>");
        //    }

        //    //Read Only
        //    if (!String.IsNullOrEmpty(agIsReadOnly) || isReadOnly) {
        //        sbCkb.AppendLine("<div " + ngShowRO + " style=\"float:left;\">");

        //        //checked
        //        sbCkb.AppendLine("  <div ng-show=\"" + agIsChecked + "\">");
        //        sbCkb.AppendLine("  <img ng-src=\"" + GetRootUrl() + "Content/Images/Controll/Check20.png\" class=\"reget-check-readonly\" style=\"float:left;\" />");
        //        sbCkb.AppendLine("  <div style=\"float:left;\" >" + ckbText + "</div>");
        //        sbCkb.AppendLine("  <md-tooltip>" + ckbText + " " + RequestResource.Yes + "</md-tooltip>");
        //        sbCkb.AppendLine("  </div>");
        //        //not checked
        //        sbCkb.AppendLine("  <div ng-hide=\"" + agIsChecked + "\">");
        //        sbCkb.AppendLine("  <img ng-src=\"" + GetRootUrl() + "Content/Images/Controll/UnCheck20.png\" class=\"reget-check-readonly\" style=\"float:left;\" />");
        //        sbCkb.AppendLine("  <div class=\"reget-check-readonly-text\" >" + ckbText + "</div>");
        //        sbCkb.AppendLine("  <md-tooltip>" + ckbText + " " + RequestResource.No + "</md-tooltip>");
        //        sbCkb.AppendLine("  </div>");

        //        sbCkb.AppendLine("</div>");
        //    }


        //    return sbCkb.ToString();
        //}

        //public string GetAngularCheckbox(
        //    string strRootTagId,
        //    string ckbText,
        //    string agIsChecked,
        //    string agIsReadOnly,
        //    string agClick) {

        //    return GetAngularCheckbox(
        //        strRootTagId,
        //        ckbText,
        //        agIsChecked,
        //        agIsReadOnly,
        //        agClick,
        //        false);
        //}

        //public string GetAngularCheckbox(
        //    string strRootTagId,
        //    string ckbText,
        //    string agIsChecked,
        //    string agClick,
        //    bool isReadOnly) {

        //    return GetAngularCheckbox(
        //        strRootTagId,
        //        ckbText,
        //        agIsChecked,
        //        null,
        //        agClick,
        //        isReadOnly);
        //}
#endregion

#region AngDataGrid
        //public string GetDataGrid(
        //    string strId,
        //    string strGridOptions,
        //    string strGridApi,
        //    string strTitle,
        //    string fcePreviousPage,
        //    string fceNextPage,
        //    string fceFirstPage,
        //    string fceLastPage,
        //    string fceExportToExcel,
        //    string fceRefresh,
        //    string fceNewRow,
        //    string currentPage,
        //    string fceGotoPage,
        //    string fcePageSizeChanged,
        //    string fceGetRowsCount,
        //    string fceGetDisplayInfoTo,
        //    string fceGetCurrentPage,
        //    string fceGetLastPageIndex,
        //    string fceSaveGridSetting,
        //    string fceResetGridSettings,
        //    string userGridSettings,
        //    string PropPageSize,
        //    string strAddNewText,
        //    bool isHiddenAtStart) {
        //    StringBuilder sb = new StringBuilder();


        //    string strButtonStyle = "float:right;width:20px;min-height:22px;line-height:22px;min-width:36px;margin-right:0px;margin-left:0px;margin-top:0px;margin-bottom:0px;padding:4px;";

        //    sb.AppendLine("<div id=\"grdHeader_" + strId + "\" class=\"reget-grid-header\">");

            
            

        //    if (!String.IsNullOrEmpty(strTitle)) {
        //        sb.AppendLine(" <div id=\"grdHeaderTitle_" + strId + "\" style=\"float:left;margin-right:30px;padding:4px;\">" + strTitle + "</div>");
        //    }

        //    sb.AppendLine("     <md-button style=\"" + strButtonStyle + "\" ng-hide=\"" + userGridSettings + "==null || " + userGridSettings + "==''\"");
        //    sb.AppendLine("           aria-label=\"ResetGridSettings\"");
        //    sb.AppendLine("           ng-click=\"" + fceResetGridSettings + "\">");
        //    sb.AppendLine("         <img ng-src=\"" + GetRootUrl() + "Content\\Images\\Controll\\Grid\\reset.png\" arial-label=\"Reset\" style=\"width:18px;\" />");
        //    sb.AppendLine("         <md-tooltip>" + RequestResource.ResetDataGridSettings + "</md-tooltip>");
        //    sb.AppendLine("     </md-button>");

        //    sb.AppendLine("     <md-button style=\"" + strButtonStyle + "\"");
        //    sb.AppendLine("           aria-label=\"SaveGridSettings\"");
        //    sb.AppendLine("           ng-click=\"" + fceSaveGridSetting + ";\">");
        //    sb.AppendLine("         <img ng-src=\"" + GetRootUrl() + "Content\\Images\\Controll\\Grid\\save.png\" style=\"width:18px;\" />");
        //    sb.AppendLine("         <md-tooltip>" + RequestResource.SaveDataGridSettings + "</md-tooltip>");
        //    sb.AppendLine("     </md-button>");


        //    //sb.AppendLine("<div style=\"float:left;margin-right:15px;\"><img src=\"" + GetRootUrl() + "Content\\Images\\Controll\\Grid\\save.png\" width=\"18px\"></div>");
        //    //sb.AppendLine("<div style=\"float:left;margin-right:15px;\"><img src=\"" + GetRootUrl() + "Content\\Images\\Controll\\Grid\\reset.png\" width=\"18px\"></div>");

        //    sb.AppendLine(" <div style=\"clear:both\"></div>");
        //    sb.AppendLine("</div>");


        //    sb.AppendLine("<div class=\"reget-grid-border\" ui-i18n=\"{{lang}}\" >");
        //    sb.AppendLine("<div id =\"" + strId + "\" ui-grid=\"" + strGridOptions + "\" ui-grid-resize-columns ui-grid-move-columns ui-grid-edit ui-grid-pagination class=\"grid\"></div>");
        //    sb.AppendLine(GetDataGridFootPanel(
        //        strId,
        //        strGridOptions,
        //        strGridApi,
        //        fcePreviousPage,
        //        fceNextPage,
        //        fceFirstPage,
        //        fceLastPage,
        //        fceExportToExcel,
        //        fceRefresh,
        //        fceNewRow,
        //        currentPage,
        //        fceGotoPage,
        //        fcePageSizeChanged,
        //        fceGetRowsCount,
        //        fceGetDisplayInfoTo,
        //        fceGetCurrentPage,
        //        fceGetLastPageIndex,
        //        PropPageSize,
        //        strAddNewText,
        //        strId,
        //        isHiddenAtStart));
        //    sb.AppendLine("</div>");

        //    return sb.ToString();
        //}

        //public string GetDataGrid(
        //    string strId,
        //    string strGridOptions,
        //    string strGridApi,
        //    string strTitle,
        //    string fcePreviousPage,
        //    string fceNextPage,
        //    string fceFirstPage,
        //    string fceLastPage,
        //    string fceExportToExcel,
        //    string fceRefresh,
        //    string fceNewRow,
        //    string currentPage,
        //    string fceGotoPage,
        //    string fcePageSizeChanged,
        //    string fceGetRowsCount,
        //    string fceGetDisplayInfoTo,
        //    string fceGetCurrentPage,
        //    string fceGetLastPageIndex,
        //    string fceSaveGridSetting,
        //    string fceResetGridSettings,
        //    string userGridSettings,
        //    string PropPageSize,
        //    string strAddNewText) {

        //    return GetDataGrid(
        //        strId,
        //        strGridOptions,
        //        strGridApi,
        //        strTitle,
        //        fcePreviousPage,
        //        fceNextPage,
        //        fceFirstPage,
        //        fceLastPage,
        //        fceExportToExcel,
        //        fceRefresh,
        //        fceNewRow,
        //        currentPage,
        //        fceGotoPage,
        //        fcePageSizeChanged,
        //        fceGetRowsCount,
        //        fceGetDisplayInfoTo,
        //        fceGetCurrentPage,
        //        fceGetLastPageIndex,
        //        fceSaveGridSetting,
        //        fceResetGridSettings,
        //        userGridSettings,
        //        PropPageSize,
        //        strAddNewText,
        //        false);
        //}



        //public string GetDataGridFootPanel(
        //    string strId,
        //    string strGridOption,
        //    string strGridApi,
        //    string fcePreviousPage,
        //    string fceNextPage,
        //    string fceFirstPage,
        //    string fceLastPage,
        //    string fceExportToExcel,
        //    string fceRefresh,
        //    string fceNewRow,
        //    string currentPage,
        //    string fceGotoPage,
        //    string fcePageSizeChanged,
        //    string fceGetRowsCount,
        //    string fceGetDisplayInfoTo,
        //    string fceGetCurrentPage,
        //    string fceGetLastPageIndex,
        //    string PropPageSize,
        //    string strAddNewText,
        //    string idPrefix,
        //    bool isHiddenAtStart) {
        //    //string fceSetPageSize,
        //    //int[] pageSize) {
        //    StringBuilder sb = new StringBuilder();

        //    //*********************** Throws error in F12 mode ***************************************************
        //    //Syntax error, unrecognized expression: td[ng-hide='IsValueNullOrUndefined(gridOptions.data) || gridOptions.data.length ='= 0 || (gridOptions.data.length == 1 && gridOptions.data[0'].id < 0)']
        //    string strHideControls = "IsValueNullOrUndefined(" + strGridOption + ".data) || " + strGridOption + ".data.length === 0 || (" + strGridOption + ".data.length === 1 && " + strGridOption + ".data[0].id < 0)";
        //    //string strHideControls = "regetgridservice.IsHideGridControls(" + strGridOption + ")";
        //    //*******************************************************************************************************

        //    sb.AppendLine(" <div style=\"overflow-x:auto;overflow-y:auto\">");
        //    sb.AppendLine("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"100%\" >");
        //    sb.AppendLine(" <tbody>");
        //    sb.AppendLine("     <tr>");
        //    //if (pageSize != null && pageSize.Length > 0) {
        //    //    string cmbPageSizeId = strGridOption + "_cmbPageSize";
        //    //    sb.AppendLine("         <td>");
        //    //    sb.AppendLine("             <select id=\"" + cmbPageSizeId + "\" ng-click=\"" + fceSetPageSize + "('" + cmbPageSizeId + "')" + "\">");
        //    //    for (int i = 0; i < pageSize.Length; i++) {
        //    //        sb.AppendLine("             <option value=\"" + pageSize[i] + "\" >" + pageSize[i] + "</option>");
        //    //    }
        //    //    sb.AppendLine("             </select>");
        //    //    sb.AppendLine("         </td>");
        //    //}
        //    sb.AppendLine("         <td ng-hide=\"" + strHideControls + "\">");
        //    sb.AppendLine("             <select id=\"" + idPrefix + "_PageSize\" ng-model=\"" + (isHiddenAtStart ? "$parent." : "") + PropPageSize + "\" ng-change=\"" + fcePageSizeChanged + "\" ng-options=\"o as o for o in gridOptions.paginationPageSizes\" style=\"border-color:#95B8E7;margin-left:8px;margin-right:8px;height:25px;\"></select>");
        //    sb.AppendLine("         </td>");
        //    sb.AppendLine("         <td ng-hide=\"" + strHideControls + "\"><div class=\"reget-grid-pagination-btn-separator\"></div></td>");
        //    sb.AppendLine("         <td ng-hide=\"" + strHideControls + "\">");
        //    sb.AppendLine("             <md-button arial-label=\"First Page\" ng-class=\"" + currentPage + "==1 ? 'reget-grid-footer-button reget-grid-footer-button-first-disabled':'reget-grid-footer-button reget-grid-footer-button-first'\" ng-click=\"" + fceFirstPage + "\" ng-disabled=\"" + currentPage + "==1\">");
        //    sb.AppendLine("                 <md-tooltip ng-if=\"" + currentPage + "!=1\">" + RequestResource.FirstPage + "</md-tooltip>");
        //    sb.AppendLine("             </md-button>");
        //    sb.AppendLine("         </td>");
        //    sb.AppendLine("         <td ng-hide=\"" + strHideControls + "\">");
        //    sb.AppendLine("             <md-button arial-label=\"Previous Page\" ng-class=\"" + currentPage + "==1 ? 'reget-grid-footer-button reget-grid-footer-button-previous-disabled':'reget-grid-footer-button reget-grid-footer-button-previous'\" ng-click=\"" + fcePreviousPage + "\" ng-disabled=\"" + currentPage + "==1\">");
        //    sb.AppendLine("                 <md-tooltip ng-if=\"" + currentPage + "!=1\">" + RequestResource.PreviousPage + "</md-tooltip>");
        //    sb.AppendLine("             </md-button>");
        //    sb.AppendLine("         </td>");
        //    sb.AppendLine("         <td ng-hide=\"" + strHideControls + "\"><div class=\"reget-grid-pagination-btn-separator\"></div></td>");
        //    sb.AppendLine("         <td style=\"padding-left:8px;padding-right:8px;\" ng-hide='" + strHideControls + "'>");
        //    sb.AppendLine("             <table style=\"width:100%;\"><tr><td><input id=\"" + idPrefix + "_PageIndex\" type =\"number\" ng-model=\"" + (isHiddenAtStart ? "$parent." : "") + currentPage + "\" ng-change=\"" + fceGotoPage + "\" onkeypress='return event.charCode >= 48 && event.charCode <= 57' ng-class=\"{'reget-grid-current-page-2': " + fceGetLastPageIndex + "<100, 'reget-grid-current-page-3': " + fceGetLastPageIndex + ">=100, 'reget-grid-current-page-4': " + fceGetLastPageIndex + ">=1000, 'reget-grid-current-page-5': " + fceGetLastPageIndex + ">=10000, 'reget-grid-current-page-6': " + fceGetLastPageIndex + ">=100000}\" style=\"padding-left:5px;border:1px solid #95B8E7;height:25px;\" /></td>");
        //    sb.AppendLine("             <td style=\"white-space:nowrap;padding-left:5px;\"> of {{ " + fceGetLastPageIndex + "}}</td></tr></table>");
        //    sb.AppendLine("         </td>");
        //    sb.AppendLine("         <td ng-hide=\"" + strHideControls + "\"><div class=\"reget-grid-pagination-btn-separator\"></div></td>");
        //    sb.AppendLine("         <td ng-hide=\"" + strHideControls + "\" >");
        //    sb.AppendLine("             <md-button arial-label=\"Next Page\" ng-class=\"" + currentPage + "==" + fceGetLastPageIndex + "?'reget-grid-footer-button reget-grid-footer-button-next-disabled':'reget-grid-footer-button reget-grid-footer-button-next'\" ng-click=\"" + fceNextPage + "\" ng-disabled=\"" + currentPage + "==" + fceGetLastPageIndex + "\">");
        //    sb.AppendLine("                 <md-tooltip ng-if=\"" + currentPage + "!=" + fceGetLastPageIndex + "\">" + RequestResource.NextPage + "</md-tooltip>");
        //    sb.AppendLine("             </md-button>");
        //    sb.AppendLine("         </td>");
        //    sb.AppendLine("         <td ng-hide=\"" + strHideControls + "\">");
        //    sb.AppendLine("             <md-button arial-label=\"Last Page\" ng-class=\"" + currentPage + "==" + fceGetLastPageIndex + "?'reget-grid-footer-button reget-grid-footer-button-last-disabled':'reget-grid-footer-button reget-grid-footer-button-last'\" ng-click=\"" + fceLastPage + "\" ng-disabled=\"" + currentPage + "==" + fceGetLastPageIndex + "\">");
        //    sb.AppendLine("                 <md-tooltip ng-if=\"" + currentPage + "!=" + fceGetLastPageIndex + "\">" + RequestResource.LastPage + "</md-tooltip>");
        //    sb.AppendLine("             </md-button>");
        //    sb.AppendLine("         </td>");
        //    sb.AppendLine("         <td ng-hide=\"" + strHideControls + "\"><div class=\"reget-grid-pagination-btn-separator\"></div></td>");
        //    sb.AppendLine("         <td>");
        //    sb.AppendLine("             <md-button class=\"btn reget-grid-footer-button reget-grid-footer-button-refresh\" ng-click=\"" + fceRefresh + "\">");
        //    sb.AppendLine("                     <md-tooltip>" + RequestResource.Refresh + "</md-tooltip>");
        //    sb.AppendLine("             </md-button>");
        //    sb.AppendLine("         </td>");
        //    sb.AppendLine("         <td><div class=\"reget-grid-pagination-btn-separator\"></div></td>");
        //    if (!String.IsNullOrEmpty(fceNewRow)) {
        //        sb.AppendLine("         <td id=\"" + strId + "_btnNewRow" + "\">");
        //        sb.AppendLine("             <md-button class=\"btn reget-grid-footer-button reget-grid-footer-button-add\" ng-click=\"" + fceNewRow + "\">");
        //        sb.AppendLine("                 " + strAddNewText + "<md-tooltip>" + strAddNewText + "</md-tooltip>");
        //        sb.AppendLine("             </md-button>");
        //        sb.AppendLine("         </td>");

        //        sb.AppendLine("         <td id=\"" + strId + "_btnNewRowSeparator" + "\"><div class=\"reget-grid-pagination-btn-separator\"></div></td>");
        //    }
        //    sb.AppendLine("         <td ng-hide=\"" + strHideControls + "\">");
        //    sb.AppendLine("             <md-button class=\"btn reget-grid-footer-button reget-grid-footer-button-excel\" ng-click=\"" + fceExportToExcel + "\">");
        //    sb.AppendLine("                 " + RequestResource.ExportToExcel + "<md-tooltip>" + RequestResource.ExportToExcel + "</md-tooltip>");
        //    sb.AppendLine("             </md-button>");
        //    sb.AppendLine("         </td>");
        //    sb.AppendLine("         <td ng-hide=\"" + strHideControls + "\"><div class=\"reget-grid-pagination-btn-separator\"></div></td>");
        //    sb.AppendLine("         <td width=\"100%\" align=\"right\" style=\"padding-right:8px;\" >");
        //    //string itemsInfo = RequestResource.DisplayingFromToOf
        //    //    .Replace("{0}", "{{(" + strGridApi + ".pagination.getPage() - 1) * " + strGridOption + ".paginationPageSize + 1}}")
        //    //    .Replace("{1}", "{{GetDisplayItemsToInfo()}}")
        //    //    .Replace("{2}", "{{" + strGridApi + ".grid.rows.length}}");
        //    string itemsInfo = RequestResource.DisplayingFromToOf
        //        .Replace("{0}", "{{(" + fceGetCurrentPage + " - 1) * " + PropPageSize + " + 1}}")
        //        .Replace("{1}", "{{" + fceGetDisplayInfoTo + "}}")
        //        .Replace("{2}", "{{" + fceGetRowsCount + "}}");
        //    sb.AppendLine("             <span style=\"font-size:12px;\" ng-hide=\"" + strHideControls + "\"> " + itemsInfo + "</span>");
        //    sb.AppendLine("         </td>");
        //    sb.AppendLine(" </tr>");
        //    sb.AppendLine("</tbody>");
        //    sb.AppendLine("</table>");
        //    sb.AppendLine("</div>");

        //    return sb.ToString();
        //}
#endregion

#region AngDataGridTypeScript
        //public string GetDataGridTs(
        //    string strId,
        //    string agControlerName,
        //    string strTitle,
        //    string strAddNewText,
        //    bool isHiddenAtStart) {

        //    StringBuilder sb = new StringBuilder();

        //    string agControlerNameDot = (String.IsNullOrEmpty(agControlerName)) ? "" : agControlerName + ".";

        //    string strButtonStyle = "float:right;width:20px;min-height:22px;line-height:22px;min-width:36px;margin-right:0px;margin-left:0px;margin-top:0px;margin-bottom:0px;padding:4px;";

        //    sb.AppendLine("<div id=\"grdHeader_" + strId + "\" class=\"reget-grid-header\">");

        //    //sb.AppendLine("     <div "
        //    //    //+ " ng-hide=\"" + agControlerNameDot + "isValueNullOrUndefined(" + agControlerNameDot + "userGridSettings.filter)\""
        //    //    + " ng-hide=\"" + agControlerNameDot + "isValueNullOrUndefined(" + agControlerNameDot + "getFilterUrl())\""
        //    //    + " style=\"float:left;margin-top:2px;margin-right:5px;\">" 
        //    //    + "<img ng-src=\"" + GetRootUrl() + "Content\\Images\\Controll\\Grid\\Filter.png\" arial-label=\"Filter\" style=\"width:18px;\" />" 
        //    //    + "<md-tooltip>" + RequestResource.Filtered + "</md-tooltip>"
        //    //    + "</div>");

        //    if (!String.IsNullOrEmpty(strTitle)) {
        //        sb.AppendLine(" <div id=\"grdHeaderTitle_" + strId + "\" style=\"float:left;margin-right:30px;padding:4px;\">" + strTitle + "</div>");
        //    }
                        
        //    sb.AppendLine("     <md-button id=\"" + "clearGridSettings_" + strId + "\" style=\"" + strButtonStyle + "\" ng-hide=\"" + agControlerNameDot + "isValueNullOrUndefined(" + agControlerNameDot + "userGridSettings)\"");
        //    sb.AppendLine("           aria-label=\"ResetGridSettings\"");
        //    sb.AppendLine("           ng-click=\"" + agControlerNameDot + "resetGridSettings()" + "\">");
        //    sb.AppendLine("         <img ng-src=\"" + GetRootUrl() + "Content\\Images\\Controll\\Grid\\reset.png\" arial-label=\"Reset\" style=\"width:18px;\" />");
        //    sb.AppendLine("         <md-tooltip>" + RequestResource.ResetDataGridSettings + "</md-tooltip>");
        //    sb.AppendLine("     </md-button>");

        //    sb.AppendLine("     <md-button id=\"" + "saveGridSettings_" + strId + "\" style=\"" + strButtonStyle + "\"");
        //    sb.AppendLine("           aria-label=\"SaveGridSettings\"");
        //    sb.AppendLine("           ng-click=\"" + agControlerNameDot + "saveGridSettings()\">");
        //    sb.AppendLine("         <img ng-src=\"" + GetRootUrl() + "Content\\Images\\Controll\\Grid\\save.png\" style=\"width:18px;\" />");
        //    sb.AppendLine("         <md-tooltip>" + RequestResource.SaveDataGridSettings + "</md-tooltip>");
        //    sb.AppendLine("     </md-button>");

        //    sb.AppendLine("<div style=\"border-right:solid 1px #95B8E7;float:right;padding-right:10px;margin-right:10px;\">");
        //    sb.AppendLine("     <md-button id=\"" + "showGridColumns_" + strId + "\" style=\"" + strButtonStyle + "\"");
        //    sb.AppendLine("           aria-label=\"ShowHiddenColumns\"");
        //    sb.AppendLine("           ng-click=\"" + agControlerNameDot + "showAllColumnsReload()\" ");
        //    sb.AppendLine("           ng-hide=\"" + agControlerNameDot + "isColumnHidden == false\" />");
        //    sb.AppendLine("         <img ng-src=\"" + GetRootUrl() + "Content\\Images\\Controll\\Grid\\ShowHideCol.png\" style=\"height:12px;\" />");
        //    sb.AppendLine("         <md-tooltip>" + RequestResource.ShowHiddenColumns + "</md-tooltip>");
        //    sb.AppendLine("     </md-button>");

        //    sb.AppendLine("     <md-button id=\"" + "clearGridFilter_" + strId + "\" style=\"" + strButtonStyle + "\"");
        //    sb.AppendLine("           aria-label=\"RemoveFilter\"");
        //    sb.AppendLine("           ng-click=\"" + agControlerNameDot + "clearFiltersReload()\" ");
        //    sb.AppendLine("           ng-hide=\"" + agControlerNameDot + "isFilterApplied == false\" />");
        //    sb.AppendLine("         <img ng-src=\"" + GetRootUrl() + "Content\\Images\\Controll\\Grid\\RemoveFilter.png\" style=\"height:20px;\" />");
        //    sb.AppendLine("         <md-tooltip>" + RequestResource.ClearFilters + "</md-tooltip>");
        //    sb.AppendLine("     </md-button>");
        //    sb.AppendLine("</div>");

        //    sb.AppendLine(" <div style=\"clear:both\"></div>");
        //    sb.AppendLine("</div>");


        //    sb.AppendLine("<div class=\"reget-grid-border\" ui-i18n=\"{{" + agControlerNameDot + "lang}}\" >");
        //    sb.AppendLine("<div id =\"" + strId + "\" ui-grid=\"" + agControlerNameDot + "gridOptions" + "\" ui-grid-resize-columns ui-grid-move-columns ui-grid-edit ui-grid-pagination class=\"grid\"></div>");
        //    sb.AppendLine(GetDataGridFootPanelTs(
        //        strId,
        //        agControlerName,
        //        strAddNewText,
        //        strId,
        //        isHiddenAtStart));
        //    sb.AppendLine("</div>");

        //    if (IsTestMode) {
        //        sb.AppendLine("<div><span>DataGrid Load Data Count:</span><span id=\"testSpGridLoadDataCount\">{{angCtrl.testLoadDataCount}}</span></div>");
        //    }

        //    return sb.ToString();
        //}

        //public string GetDataGridTs(
        //    string strId,
        //    string agControlerName,
        //    string strTitle,
        //    string strAddNewText) {

        //    return GetDataGridTs(
        //        strId,
        //        agControlerName,
        //        strTitle,
        //        strAddNewText,
        //        false);
        //}


        //public string GetDataGridFootPanelTs(
        //    string strId,
        //    string agControlerName,
        //    string strAddNewText,
        //    string idPrefix,
        //    bool isHiddenAtStart) {

        //    StringBuilder sb = new StringBuilder();

        //    //*********************** Throws error in F12 mode ***************************************************
        //    //Syntax error, unrecognized expression: td[ng-hide='IsValueNullOrUndefined(gridOptions.data) || gridOptions.data.length ='= 0 || (gridOptions.data.length == 1 && gridOptions.data[0'].id < 0)']
        //    string strHideControls = "IsValueNullOrUndefined(" + agControlerName + ".gridOption" + ".data) || " + agControlerName + ".gridOption" + ".data.length === 0 || (" + agControlerName + ".gridOption" + ".data.length === 1 && " + agControlerName + ".gridOption" + ".data[0].id < 0)";
        //    //string strHideControls = "regetgridservice.IsHideGridControls(" + strGridOption + ")";
        //    //*******************************************************************************************************

        //    sb.AppendLine(" <div style=\"overflow-x:auto;overflow-y:auto\">");
        //    sb.AppendLine("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"100%\" ng-hide=\"" + agControlerName + ".rowsCount==0" + "\">");
        //    sb.AppendLine(" <tbody>");
        //    sb.AppendLine("     <tr>");

        //    sb.AppendLine("         <td ng-hide=\"" + strHideControls + "\">");
        //    sb.AppendLine("             <select id=\"" + idPrefix + "_PageSize\" ng-model=\"" + (isHiddenAtStart ? "$parent." : "") + agControlerName + ".pageSize" + "\" ng-change=\"" + agControlerName + ".pageSizeChanged()" + "\" ng-options=\"o as o for o in " + agControlerName + ".gridOptions.paginationPageSizes\" style=\"border-color:#95B8E7;margin-left:8px;margin-right:8px;height:25px;\"></select>");
        //    sb.AppendLine("         </td>");
        //    sb.AppendLine("         <td ng-hide=\"" + strHideControls + "\"><div class=\"reget-grid-pagination-btn-separator\"></div></td>");
        //    sb.AppendLine("         <td ng-hide=\"" + strHideControls + "\">");
        //    sb.AppendLine("             <md-button arial-label=\"First Page\" ng-class=\"" + agControlerName + ".currentPage" + "==1 ? 'reget-grid-footer-button reget-grid-footer-button-first-disabled':'reget-grid-footer-button reget-grid-footer-button-first'\" ng-click=\"" + agControlerName + ".firstPage()" + "\" ng-disabled=\"" + agControlerName + ".currentPage" + "==1\">");
        //    sb.AppendLine("                 <md-tooltip ng-if=\"" + agControlerName + ".currentPage" + "!=1\">" + RequestResource.FirstPage + "</md-tooltip>");
        //    sb.AppendLine("             </md-button>");
        //    sb.AppendLine("         </td>");
        //    sb.AppendLine("         <td ng-hide=\"" + strHideControls + "\">");
        //    sb.AppendLine("             <md-button arial-label=\"Previous Page\" ng-class=\"" + agControlerName + ".currentPage" + "==1 ? 'reget-grid-footer-button reget-grid-footer-button-previous-disabled':'reget-grid-footer-button reget-grid-footer-button-previous'\" ng-click=\"" + agControlerName + ".previousPage()" + "\" ng-disabled=\"" + agControlerName + ".currentPage" + "==1\">");
        //    sb.AppendLine("                 <md-tooltip ng-if=\"" + agControlerName + ".currentPage" + "!=1\">" + RequestResource.PreviousPage + "</md-tooltip>");
        //    sb.AppendLine("             </md-button>");
        //    sb.AppendLine("         </td>");
        //    sb.AppendLine("         <td ng-hide=\"" + strHideControls + "\"><div class=\"reget-grid-pagination-btn-separator\"></div></td>");
        //    sb.AppendLine("         <td style=\"padding-left:8px;padding-right:8px;\" ng-hide='" + strHideControls + "'>");
        //    sb.AppendLine("             <table style=\"width:100%;\"><tr><td><input id=\"" + idPrefix + "_PageIndex\" type =\"number\" ng-model=\"" + (isHiddenAtStart ? "$parent." : "") +
        //        agControlerName + ".currentPage" + "\" ng-change=\"" + agControlerName + ".gotoPage()" + "\" onkeypress='return event.charCode >= 48 && event.charCode <= 57' ng-class=\"{'reget-grid-current-page-2': " +
        //        agControlerName + ".getLastPageIndex()" + "<100, 'reget-grid-current-page-3': " + agControlerName + ".getLastPageIndex()" + ">=100, 'reget-grid-current-page-4': " + agControlerName + ".getLastPageIndex()" +
        //        ">=1000, 'reget-grid-current-page-5': " + agControlerName + ".getLastPageIndex()" + ">=10000, 'reget-grid-current-page-6': " + agControlerName + ".getLastPageIndex()" + ">=100000}\" style=\"padding-left:5px;border:1px solid #95B8E7;height:25px;\" /></td>");
        //    sb.AppendLine("             <td id=" + strId + "_PagesCount" + " style=\"white-space:nowrap;padding-left:5px;\"> of {{ " + agControlerName + ".getLastPageIndex()" + "}}</td></tr></table>");
        //    sb.AppendLine("         </td>");
        //    sb.AppendLine("         <td ng-hide=\"" + strHideControls + "\"><div class=\"reget-grid-pagination-btn-separator\"></div></td>");
        //    sb.AppendLine("         <td ng-hide=\"" + strHideControls + "\" >");
        //    sb.AppendLine("             <md-button arial-label=\"Next Page\" ng-class=\"" + agControlerName + ".currentPage" + "==" + agControlerName + ".getLastPageIndex()" + "?'reget-grid-footer-button reget-grid-footer-button-next-disabled':'reget-grid-footer-button reget-grid-footer-button-next'\" ng-click=\"" +
        //        agControlerName + ".nextPage()" + "\" ng-disabled=\"" + agControlerName + ".currentPage" + "==" + agControlerName + ".getLastPageIndex()" + "\">");
        //    sb.AppendLine("                 <md-tooltip ng-if=\"" + agControlerName + ".currentPage" + "!=" + agControlerName + ".getLastPageIndex()" + "\">" + RequestResource.NextPage + "</md-tooltip>");
        //    sb.AppendLine("             </md-button>");
        //    sb.AppendLine("         </td>");
        //    sb.AppendLine("         <td ng-hide=\"" + strHideControls + "\">");
        //    sb.AppendLine("             <md-button arial-label=\"Last Page\" ng-class=\"" + agControlerName + ".currentPage" + "==" + agControlerName + ".getLastPageIndex()" + "?'reget-grid-footer-button reget-grid-footer-button-last-disabled':'reget-grid-footer-button reget-grid-footer-button-last'\" ng-click=\"" +
        //        agControlerName + ".lastPage()" + "\" ng-disabled=\"" + agControlerName + ".currentPage" + "==" + agControlerName + ".getLastPageIndex()" + "\">");
        //    sb.AppendLine("                 <md-tooltip ng-if=\"" + agControlerName + ".currentPage" + "!=" + agControlerName + ".getLastPageIndex()" + "\">" + RequestResource.LastPage + "</md-tooltip>");
        //    sb.AppendLine("             </md-button>");
        //    sb.AppendLine("         </td>");
        //    sb.AppendLine("         <td ng-hide=\"" + strHideControls + "\"><div class=\"reget-grid-pagination-btn-separator\"></div></td>");
        //    sb.AppendLine("         <td>");
        //    sb.AppendLine("             <md-button class=\"btn reget-grid-footer-button reget-grid-footer-button-refresh\" ng-click=\"" + agControlerName + ".refresh()" + "\">");
        //    sb.AppendLine("                     <md-tooltip>" + RequestResource.Refresh + "</md-tooltip>");
        //    sb.AppendLine("             </md-button>");
        //    sb.AppendLine("         </td>");
        //    sb.AppendLine("         <td><div class=\"reget-grid-pagination-btn-separator\"></div></td>");
        //    if (!String.IsNullOrEmpty(strAddNewText)) {
        //        sb.AppendLine("         <td id=\"" + strId + "_tdNewRow" + "\">");
        //        sb.AppendLine("             <md-button id=\"" + strId + "_btnNewRow" + "\" class=\"btn reget-grid-footer-button reget-grid-footer-button-add\" ng-click=\"" + agControlerName + ".addNewRow()" + "\">");
        //        sb.AppendLine("                 <span class=\"hidden-xs hidden-sm\">" + strAddNewText + "<span><md-tooltip>" + strAddNewText + "</md-tooltip>");
        //        sb.AppendLine("             </md-button>");
        //        sb.AppendLine("         </td>");

        //        sb.AppendLine("         <td id=\"" + strId + "_btnNewRowSeparator" + "\"><div class=\"reget-grid-pagination-btn-separator\"></div></td>");
        //    }
        //    sb.AppendLine("         <td ng-hide=\"" + strHideControls + "\">");
        //    sb.AppendLine("             <md-button class=\"btn reget-grid-footer-button reget-grid-footer-button-excel\" ng-click=\"" + agControlerName + ".exportToXls()" + "\">");
        //    sb.AppendLine("                 <span class=\"hidden-xs hidden-sm\">" + RequestResource.ExportToExcel + "</span><md-tooltip>" + RequestResource.ExportToExcel + "</md-tooltip>");
        //    sb.AppendLine("             </md-button>");
        //    sb.AppendLine("         </td>");
        //    sb.AppendLine("         <td ng-hide=\"" + strHideControls + "\"><div class=\"reget-grid-pagination-btn-separator\"></div></td>");
        //    sb.AppendLine("         <td width=\"100%\" align=\"right\" style=\"padding-right:8px;\" >");
        //    string itemsInfo = RequestResource.DisplayingFromToOf
        //        .Replace("{0}", "{{" + agControlerName + ".getDisplayItemsFromInfo()" + "}}")
        //        .Replace("{1}", "{{" + agControlerName + ".getDisplayItemsToInfo()" + "}}")
        //        .Replace("{2}", "{{" + agControlerName + ".rowsCount" + "}}");
        //    string itemsInfoMp = "{{" + agControlerName + ".getDisplayItemsFromInfo()" + "}}" + " - "
        //        + "{{" + agControlerName + ".getDisplayItemsToInfo()" + "}}" + " / " 
        //        + "{{" + agControlerName + ".rowsCount" + "}}";

        //    sb.AppendLine("             <span class=\"hidden-xs hidden-sm\" style=\"font-size:12px;\" ng-hide=\"" + strHideControls + "\"> " + itemsInfo + "</span>");
        //    sb.AppendLine("             <span class=\"hidden-md hidden-lg\" style=\"font-size:12px;\" ng-hide=\"" + strHideControls + "\"> " + itemsInfoMp + "</span>");
        //    sb.AppendLine("         </td>");
        //    sb.AppendLine(" </tr>");
        //    sb.AppendLine("</tbody>");
        //    sb.AppendLine("</table>");
        //    sb.AppendLine("<div id=\"" + strId + "_DivNoRecord" + "\" style=\"margin-top:25px;margin-bottom:25px;text-align:center;color:#888;\" ng-show=\"" + agControlerName + ".rowsCount==0\">");
        //    sb.AppendLine(RequestResource.NoRecordFound);
        //    sb.AppendLine("</div>");
        //    sb.AppendLine("</div>");

        //    return sb.ToString();
        //}

        //public string GetAngularAutocompleteTs(
        //       string strRootTagId,
        //       string agControlerName,
        //       bool isRequired,
        //       string strLabelLeft,
        //       string strLabelTop,
        //       bool isLeftLabelDisplayed,
        //       string leftLabelCss,
        //       string mdSelectItem,
        //       string mdSearchText,
        //       string mdSelectedItemChange,
        //       string mdItems,
        //       string mdItemText,
        //       string placeholder,
        //       string mdInputId,
        //       string mdItemTemplate,
        //       string onBlur,
        //       string strClass) {
        //    StringBuilder sbAuto = new StringBuilder();

        //    //sbAuto.AppendLine("<div style=\"clear:both;\"></div>");
        //    if (isRequired) {
        //        strLabelLeft += " *";
        //    }
        //    sbAuto.AppendLine("<div id=\"" + ANG_WRAPPER_PREFIX + strRootTagId + "\">");
        //    if (isLeftLabelDisplayed) {
        //        sbAuto.AppendLine("<label id=\"" + ANG_LABEL_LEFT_PREFIX + strRootTagId + "\" class=\"control-label hidden-xs " + leftLabelCss + "\" style=\"float:left;\">" + strLabelLeft + " :</label>");
        //    }

        //    if (isRequired) {
        //        strLabelTop += " *";
        //    }


        //    if (isRequired) {
        //        //strStyle = "style=\"border-bottom:3px solid #777\"";
        //        if (String.IsNullOrEmpty(strClass)) {
        //            strClass = "reget-ang-mandatory-autocomplete";
        //        } else {
        //            strClass += " " + "reget-ang-mandatory-autocomplete";
        //        }
        //    }

        //    sbAuto.AppendLine(" <md-input-container class=\"reget-ang-md-input-container md-input-has-value\" style=\"float:left;\">");
        //    sbAuto.AppendLine("     <label class=\"hidden-sm hidden-md hidden-lg reget-ang-lbl-control-top\" style=\"margin-bottom:8px;\">" + strLabelTop + "</label>");
        //    sbAuto.AppendLine(" </md-input-container>");
        //    sbAuto.AppendLine(" <div style=\"float:left;\">");
        //    sbAuto.AppendLine("     <md-autocomplete flex id=\"agAutoCompl\"");
        //    sbAuto.AppendLine("                          name=\"agAutoCompl\"");
        //    if (isRequired) {
        //        sbAuto.AppendLine("                          required");
        //    }
        //    sbAuto.AppendLine("                          md-input-minlength=\"0\"");
        //    sbAuto.AppendLine("                          md-min-length=\"0\"");
        //    sbAuto.AppendLine("                          ng-disabled=\"false\"");
        //    sbAuto.AppendLine("                          md-no-cache=\"true\"");
        //    sbAuto.AppendLine("                          md-selected-item=\"" + agControlerName + "." + mdSelectItem + "\"");
        //    sbAuto.AppendLine("                          md-search-text=\"" + agControlerName + "." + mdSearchText + "\"");
        //    sbAuto.AppendLine("                          md-selected-item-change=\"" + agControlerName + "." + mdSelectedItemChange + "\"");
        //    sbAuto.AppendLine("                          md-items=\"" + mdItems + "\"");
        //    sbAuto.AppendLine("                          md-item-text=\"" + mdItemText + "\"");
        //    sbAuto.AppendLine("                          placeholder=\"" + placeholder + "\"");
        //    sbAuto.AppendLine("                          md-input-id=\"" + mdInputId + "\"");
        //    sbAuto.AppendLine("                          md-input-name=\"" + mdInputId + "\"");
        //    if (!String.IsNullOrEmpty(onBlur)) {
        //        sbAuto.AppendLine("                          ng-blur=\"" + agControlerName + "." + onBlur + "\"");
        //    }
        //    string autClass = "reget-autocomplete-white";
        //    if (!String.IsNullOrEmpty(strClass)) {
        //        autClass += " " + strClass;
        //    }
        //    sbAuto.AppendLine("                          class=\"" + strClass + "\">");

        //    sbAuto.AppendLine(" <md-item-template>");
        //    sbAuto.AppendLine(mdItemTemplate);
        //    sbAuto.AppendLine(" </md-item-template>");

        //    sbAuto.AppendLine(" <md-not-found>");
        //    sbAuto.AppendLine("     \"{{" + mdSearchText + "}}\" " + @RequestResource.NotFound + ".");
        //    sbAuto.AppendLine(" </md-not-found>");



        //    sbAuto.AppendLine(" </md-autocomplete>");

        //    sbAuto.AppendLine(" <div style=\"color:maroon;\" role=\"alert\">");
        //    sbAuto.AppendLine("     <div id=\"" + strRootTagId + ANG_ERR_MSG_MANDATORY + "\" class=\"reget-ang-controll-invalid-msg\" style=\"display:none;line-height:14px;color:rgb(221,24,0);padding-top:5px;\">" + @RequestResource.MandatoryTextField + "</div>");
        //    sbAuto.AppendLine(" </div>");

        //    sbAuto.AppendLine(" </div>");

        //    sbAuto.AppendLine("</div>");

        //    sbAuto.AppendLine("<div style=\"clear:both;\"></div>");
        //    return sbAuto.ToString();
        //}
#endregion

#endregion
    }
}
 