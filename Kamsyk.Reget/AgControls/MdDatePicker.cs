using Kamsyk.Reget.Controllers;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Kamsyk.Reget.AgControls {
    public class MdDatePicker : BaseAgControl {
        #region Properties
        private string m_agControlerName = null;
        public string AgControlerName {
            get { return m_agControlerName; }
            set { m_agControlerName = value; }
        }

        private string _m_DatePickerFormat = null;
        public string DatePickerFormat {
            get {
                if (_m_DatePickerFormat == null) {
                    _m_DatePickerFormat = BaseController.GetShortDateFormat();
                }

                return _m_DatePickerFormat;

            }
        }

        //private string m_agIsReadOnly = null;
        //public string AgIsReadOnly {
        //    get { return m_agIsReadOnly; }
        //    set { m_agIsReadOnly = value; }
        //}

        private string m_ngModel = null;
        public string NgModel {
            get { return m_ngModel; }
            set { m_ngModel = value; }
        }

        private string m_RootUrl = null;

        //private HttpWebRequest m_request;
        private string m_requestUserAgent;

        //private bool m_isBold = false;
        //public bool IsBold {
        //    get { return m_isBold; }
        //    set { m_isBold = value; }
        //}

        #endregion

        #region Constructor
        public MdDatePicker(string rootUrl, string requestUserAgent) {
            m_RootUrl = rootUrl;
            m_requestUserAgent = requestUserAgent;
        }
        #endregion

        #region Abstract Methods
        public override string RenderControlHtml() {
            string agControlerNameDot = (String.IsNullOrEmpty(m_agControlerName)) ? "" : m_agControlerName + ".";


            string cssHasValue = "";

            string cssInputContainer = (IsReadOnly) ? "reget-ang-md-input-container-readonly" : "reget-ang-md-input-container";
            string cssLblLeft = (IsReadOnly) ? "reget-ang-lbl-control-left" : LabelLeftCssClass;

            //string strRequired = (IsMandatory) ? "required" : "";
            string strRequired = GetMandatoryJs();
            string strKeepErrMsgHidden = GetKeepErrMsgHiddenJs();

            if (!String.IsNullOrWhiteSpace(LabelLeft)) {
                LabelLeft += " :";
            }

            string angPart = "";
            if (RootTagId.Contains("{{")) {
                int iAngPartStart = RootTagId.IndexOf("{");
                angPart = RootTagId.Substring(iAngPartStart);

                RootTagId = RootTagId.Split('{')[0];
            }

            string strWidth = "max-width:150px;min-width:150px;";

            StringBuilder sbDatePicker = new StringBuilder();
            
            string strLeftLabelCss = "";
            if (!String.IsNullOrWhiteSpace(LabelLeftCssClass)) {
                strLeftLabelCss = LabelLeftCssClass;
            }

            string strCssBold = (IsBold) ? " reget-bold" : "";

            #region ReadOnly
            if (!String.IsNullOrEmpty(AgIsReadOnly) || IsReadOnly) {
                sbDatePicker.AppendLine(GetReadOnlyHtml("{{" + agControlerNameDot + "convertMomentDateToString(" + m_ngModel + ",'" + BaseController.GetShortDateMomentFormat() + "')" + "}}"));
            }
            #endregion

            if (!IsReadOnly) {
                string strClass = "";
                string strStyle = "float:left;";
                if (m_requestUserAgent != null && m_requestUserAgent.ToLower() == "internetexplorer") {
                    strStyle += ";margin-bottom:-6px;";
                }

                sbDatePicker.AppendLine("<div id=\"" + ANG_WRAPPER_PREFIX + RootTagId + angPart + "\"" + " class=\"" + GetContainerClass() + "\" " + NgHideEdit  + ">");
                sbDatePicker.AppendLine("    <label id=\"" + ANG_LABEL_LEFT_PREFIX + RootTagId + "\" class=\"control-label hidden-xs " + cssLblLeft + "\" style=\"margin-top:3px;\">" + LabelLeft + "</label>");
                sbDatePicker.AppendLine("    <md-input-container id=\"" + ANG_CONTAINER_PREFIX + RootTagId + "\" class=\"" + cssInputContainer + " " + cssHasValue + "\" style=\"" + strWidth + "\">");
                sbDatePicker.AppendLine("        <label class=\"hidden-sm hidden-md hidden-lg reget-ang-lbl-control-top\" style=\"min-width:150px;margin-right:0px;\">" + LabelTop + "</label>");


                sbDatePicker.AppendLine("        <md-datepicker name=\"" + "dtp" + RootTagId + "\" ng-model=\"" + m_ngModel + "\"" +
                " ng-change=\"" + agControlerNameDot + "datePickerChange()\" " +
                strRequired +
                " md-placeholder=\"" + RequestResource.EnterDate + "\"" +
                "  md-hide-icons=\"calendar\"" + strClass + " style=\"" + strStyle + "\" " + NgHideEdit + ">" +
                "</md-datepicker>");

                //sbDatePicker.AppendLine("        <div class=\"reget-mdinput-ro\" " + NgShowRO + ">" 
                //    + "{{" + agControlerNameDot + "convertMomentDateToString(" + m_ngModel + ",'" + BaseController.GetShortDateMomentFormat() + "')" + "}}" 
                //    + "</div>");

                if (IsMandatory) {
                    sbDatePicker.AppendLine("    <div class=\"reget-ang-mandatory-field\" " + NgHideEdit + " style=\"max-width:120px;\"></div>");
                }

                string strRequiredMsg = String.Format(RequestResource.EnterDateInFormat, DatePickerFormat, DateTime.Now.ToString(DatePickerFormat));
                //sbDatePicker.AppendLine("        <div id=\"" + "msg" + RootTagId + "\" ng-messages=\"" + FormName + "." + "dtp" + RootTagId + ".$error\" ng-show=\"" + FormName + "." + "dtp" + RootTagId + ".$invalid==true\" style=\"color:maroon\" role=\"alert\">");
                sbDatePicker.AppendLine("        <div id=\"" + "msg" + RootTagId + "\" ng-messages=\"" + FormName + "." + "dtp" + RootTagId + ".$error\" " + strKeepErrMsgHidden + " role=\"alert\">");
                sbDatePicker.AppendLine("              <div ng-message=\"required\"><div class=\"reget-ang-controll-invalid-msg\">" + strRequiredMsg + "</div></div>");
                sbDatePicker.AppendLine("              <div ng-message=\"valid\"><div class=\"reget-ang-controll-invalid-msg\">" + strRequiredMsg + "</div></div>");
                sbDatePicker.AppendLine("        </div>");


                sbDatePicker.AppendLine("    </md-input-container>");
                sbDatePicker.AppendLine("</div>");
                //sbDatePicker.Append("{{" + FormName + "." + "dtp" + RootTagId + ".$error" + "}}");
            }

            return sbDatePicker.ToString();

        }
        #endregion

        #region Virtual Methods
        protected override void SetManadatoryLabel() {
            if (m_isMandatory && !String.IsNullOrWhiteSpace(m_strLabelLeft)
                && !m_strLabelLeft.EndsWith("*")
                && !m_strLabelLeft.EndsWith("* :")) {
                m_strLabelLeft += " *";
            }
        }
        #endregion

        #region Methods
        private string GetDatePickerButton() {
            StringBuilder sbDatePickerBtn = new StringBuilder();
            sbDatePickerBtn.Append("<button tabindex=\"-1\" class=\"\" aria-hidden=\"true\" type=\"button\" ng-click=\"ctrl.openCalendarPane($event)\" ng-transclude=\"\">");
            sbDatePickerBtn.Append("<md-icon class=\"md-datepicker-calendar-icon ng-scope\" role=\"img\" aria-label=\"md-calendar\" md-svg-src=\"" + m_RootUrl + "Content/Images/Error.png" + "\">");
            sbDatePickerBtn.Append("</md-icon>");
            sbDatePickerBtn.Append("</button>");

            return sbDatePickerBtn.ToString();
        }
        #endregion
    }
}