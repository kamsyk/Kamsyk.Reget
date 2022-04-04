using Kamsyk.Reget.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Kamsyk.Reget.AgControls {
    public abstract class BaseAgControl {
        #region Constants
        protected string ANG_WRAPPER_PREFIX = "wrap";
        protected string ANG_CONTAINER_PREFIX = "ct";
        protected string ANG_LABEL_LEFT_PREFIX = "lblL";
        //public const string ANG_ERR_MSG_MANDATORY = "_errmandatory";
        #endregion
       
        #region Properties
        public string DecimalSeparator {
            get {
                return BaseController.DecimalSeparator;
            }
        }

        private string m_formName = null;
        public string FormName {
            get { return m_formName; }
            set { m_formName = value; }
        }

        
        protected bool m_isMandatory = false;
        public virtual bool IsMandatory {
            get { return m_isMandatory; }
            set {
                m_isMandatory = value;
                SetManadatoryLabel();
            }
        }

        private bool m_isReadOnly = false;
        public bool IsReadOnly {
            get { return m_isReadOnly; }
            set { m_isReadOnly = value; }
        }

        private string m_agIsMandatory = null;
        public string AgIsMandatory {
            get { return m_agIsMandatory; }
            set { m_agIsMandatory = value; }
        }

        private string m_agIsKeepErrMsgHidden = null;
        public string AgIsKeepErrMsgHidden {
            get { return m_agIsKeepErrMsgHidden; }
            set { m_agIsKeepErrMsgHidden = value; }
        }

        private string m_agIsReadOnly = null;
        public string AgIsReadOnly {
            get { return m_agIsReadOnly; }
            set { m_agIsReadOnly = value; }
        }

        protected string m_strLabelLeft = null;
        public virtual string LabelLeft {
            get { return m_strLabelLeft; }
            set {
                m_strLabelLeft = value;
                if (m_strLabelLeft != null && m_strLabelTop == null) {
                    m_strLabelTop = m_strLabelLeft;
                }
                SetManadatoryLabel();
            }
        }

        protected string m_strLabelTop = null;
        public string LabelTop {
            get { return m_strLabelTop; }
            set {
                m_strLabelTop = value;
                SetManadatoryLabel();
            }
        }

        private string m_strRootTagId = null;
        public string RootTagId {
            get { return m_strRootTagId; }
            set { m_strRootTagId = value; }
        }

        private string m_labelLeftCssClass = null;
        public string LabelLeftCssClass {
            get { return m_labelLeftCssClass; }
            set { m_labelLeftCssClass = value; }
        }

        private bool m_isLeftLabelDisplayed = true;
        public bool IsLeftLabelDisplayed {
            get { return m_isLeftLabelDisplayed; }
            set { m_isLeftLabelDisplayed = value; }
        }

        private bool m_isTopLabelDisplayed = true;
        public bool IsTopLabelDisplayed {
            get { return m_isTopLabelDisplayed; }
            set { m_isTopLabelDisplayed = value; }
        }

        protected string NgShowRO {
            get {
                if (IsReadOnly) {
                    return "ng-show=\"true\"";
                } else {
                    if (!String.IsNullOrEmpty(m_agIsReadOnly)) {
                        return "ng-show=\"" + m_agIsReadOnly + "\"";
                    } else {
                        return "ng-show=\"false\"";
                    }
                }

                //if (!String.IsNullOrEmpty(m_agIsReadOnly)) {
                //    return "ng-show=\"" + m_agIsReadOnly + "\"";
                //} else {
                //    if (IsReadOnly) {
                //        return "ng-show=\"true\"";
                //    } else {
                //        return "ng-show=\"false\"";
                //    }
                //}

                //return "";
            }
        }

        private string m_containerClass = null;
        public string ContainerClass {
            get { return m_containerClass; }
            set { m_containerClass = value; }
        }

        protected string NgHideEdit {
            get {
                if (!String.IsNullOrEmpty(m_agIsReadOnly)) {
                    return "ng-hide=\"" + m_agIsReadOnly + "\"";
                } else {
                    if (IsReadOnly) {
                        return "ng-hide=\"true\"";
                    } else {
                        return "ng-hide=\"false\"";
                    }
                }

            }
        }

        private bool m_isBold = false;
        public bool IsBold {
            get { return m_isBold; }
            set { m_isBold = value; }
        }

        private int m_iLabelWidth = 0;
        public int LabelWidth {
            get { return m_iLabelWidth; }
            set { m_iLabelWidth = value; }
        }

        private string m_mandatoryErrMsg = null;
        public string MandatoryErrMsg {
            get { return m_mandatoryErrMsg; }
            set { m_mandatoryErrMsg = value; }
        }
        #endregion

        #region Abstract Methods
        public abstract string RenderControlHtml();
        #endregion

        #region Methods
        protected virtual void SetManadatoryLabel() {
            if (m_isMandatory && !String.IsNullOrWhiteSpace(m_strLabelLeft) 
                && !m_strLabelLeft.EndsWith("*")
                && !m_strLabelLeft.EndsWith("* :")) {
                m_strLabelLeft += " *";
            }
            if (m_isMandatory && !String.IsNullOrWhiteSpace(m_strLabelTop) && !m_strLabelTop.EndsWith("*")) {
                m_strLabelTop += " *";
            }

            //if (m_isMandatory && !String.IsNullOrWhiteSpace(m_strLabelLeft) && m_strLabelLeft.EndsWith("*")) {
            //    m_strLabelLeft = m_strLabelLeft.Substring(0, m_strLabelLeft.Length - 1).Trim();
            //}
            //if (m_isMandatory && !String.IsNullOrWhiteSpace(m_strLabelTop) && m_strLabelTop.EndsWith("*")) {
            //    m_strLabelTop = m_strLabelTop.Substring(0, m_strLabelTop.Length - 1).Trim();
            //}
        }

        protected virtual string GetMandatoryJs() {
            string strRequired = "";
            if (!String.IsNullOrWhiteSpace(AgIsMandatory)) {
                strRequired = "ng-required=\"" + AgIsMandatory + "\"";
            } else if (IsMandatory) {
                strRequired = "ng-required=\"true\"";
            }

            return strRequired;
        }

        protected virtual string GetKeepErrMsgHiddenJs() {
            string strKeepErrMsgHidden = "";
            if (!String.IsNullOrWhiteSpace(AgIsKeepErrMsgHidden)) {
                strKeepErrMsgHidden = "ng-hide=\"" + AgIsKeepErrMsgHidden + "\"";
            } 

            return strKeepErrMsgHidden;
        }

        protected string GetContainerClass() {
            if (m_containerClass != null) {
                return m_containerClass;
            }
            return "reget-ang-div-container";
        }

        protected string GetContainerRoClass() {
            return GetContainerClass() + "-readonly";
            
        }

        protected string GetReadOnlyHtml(string roText) {
            StringBuilder sbHtml = new StringBuilder();

            string strLeftLabelCss = "";
            if (!String.IsNullOrWhiteSpace(LabelLeftCssClass)) {
                strLeftLabelCss = LabelLeftCssClass;
            }

            string strWidth = "";

            string strCssBold = (m_isBold) ? " reget-bold" : "";

            if (m_iLabelWidth > 0) {
                strWidth = "style=\"min-width:" + m_iLabelWidth + "px;text-align:right;\"";
            }

            string strLabelLeft = LabelLeft;
            if (strLabelLeft != null && !strLabelLeft.EndsWith(":")) {
                strLabelLeft += " :";
            }

            sbHtml.AppendLine("<div id=\"" + ANG_WRAPPER_PREFIX + RootTagId + "\" " + NgShowRO + " class=\"" + GetContainerRoClass() + "\" >");
            sbHtml.AppendLine(" <table><tr>");
            if (IsLeftLabelDisplayed) {
                sbHtml.AppendLine(" <td class=\"hidden-xs\" style=\"vertical-align:top;\">");
                sbHtml.AppendLine("    <label id=\"" + ANG_LABEL_LEFT_PREFIX + RootTagId + "\" class=\"control-label hidden-xs " + strLeftLabelCss + "\" " + strWidth + ">" + strLabelLeft + "</label>");
                sbHtml.AppendLine(" </td>");
            }
            sbHtml.AppendLine(" <td style=\"padding-bottom:4px;\">");
            sbHtml.AppendLine("    <md-input-container id=\"" + ANG_CONTAINER_PREFIX + RootTagId + "\" class=\"" + "reget-ang-md-input-container-label" + strCssBold + " md-input-has-value" + "\" >");
            if (IsTopLabelDisplayed) {
                sbHtml.AppendLine("          <label class=\"hidden-sm hidden-md hidden-lg reget-ang-lbl-control-top\"" + strWidth + " >" + LabelTop + "</label>");
            }
            sbHtml.AppendLine("          <div style=\"white-space:pre-line;\">" + roText + "</div>");
            sbHtml.AppendLine("    </md-input-container>");
            sbHtml.AppendLine(" </td>");
            sbHtml.AppendLine(" </tr></table>");
            sbHtml.AppendLine("</div>");

            return sbHtml.ToString();
        }
        #endregion
    }
}