using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Kamsyk.Reget.AgControls {
    public class MdTextArea : BaseAgControl {
        #region Properties
        //private string m_agIsReadOnly = null;
        //public string AgIsReadOnly {
        //    get { return m_agIsReadOnly; }
        //    set { m_agIsReadOnly = value; }
        //}

        private string m_inputCssClass = null;
        public string InputCssClass {
            get { return m_inputCssClass; }
            set { m_inputCssClass = value; }
        }

        private string m_ngModel = null;
        public string NgModel {
            get { return m_ngModel; }
            set { m_ngModel = value; }
        }

        private int m_Height = 0;
        public int Height {
            get { return m_Height; }
            set { m_Height = value; }
        }

        private string _prevControlHtml = null;
        public string PrevControlHtml {
            get { return _prevControlHtml; }
            set { _prevControlHtml = value; }
        }
        #endregion

        #region Abstract Methods
        public override string RenderControlHtml() {
            string cssHasValue = "";
            if (IsReadOnly) {
                cssHasValue = " md-input-has-value";
            }

            string cssInputContainer = (IsReadOnly) ? "reget-ang-md-input-container-readonly" : "reget-ang-md-input-container";
            
            string cssLblLeft = null;
            if (String.IsNullOrEmpty(LabelLeftCssClass)) {
                cssLblLeft = (IsReadOnly) ? "reget-ang-lbl-control-left" : "reget-ang-lbl-control-left";
            } else {
                cssLblLeft = LabelLeftCssClass;
            }

            //string strRequired = (IsMandatory) ? "required" : "";
            string strRequired = GetMandatoryJs();
            string strKeepErrMsgHidden = GetKeepErrMsgHiddenJs();

            string angPart = "";
            if (RootTagId !=null && RootTagId.Contains("{{")) {
                int iAngPartStart = RootTagId.IndexOf("{");
                angPart = RootTagId.Substring(iAngPartStart);

                RootTagId = RootTagId.Split('{')[0];

            }

            string strClass = "reget-ang-data-control";
           
            if (!String.IsNullOrEmpty(m_inputCssClass)) {
                strClass += " " + m_inputCssClass;
            }

            string strHeight = "";
            if (m_Height > 0) {
                strHeight = " style=\"height:" + m_Height + "px;min-height:" + m_Height + "px!important\"";
            }

            string textAreaId = "txt" + RootTagId;

            StringBuilder sbTextArea = new StringBuilder();

            string strLlClass = "";
            if (IsLeftLabelDisplayed) {
                
                if (AgIsReadOnly != null) {
                    strLlClass = "ng-class=\"" + AgIsReadOnly + " ? 'reget-textarea-leftlabel':''\"";
                }

            }

            if (!String.IsNullOrEmpty(AgIsReadOnly) || IsReadOnly) {
                sbTextArea.AppendLine(GetReadOnlyHtml("{{" + m_ngModel + "}}"));
            }

            if (!IsReadOnly) {
                sbTextArea.AppendLine("<div id=\"" + ANG_WRAPPER_PREFIX + RootTagId + angPart + "\" " + NgHideEdit + " class=\"reget-ang-div-container\">");
                sbTextArea.AppendLine("  <table style=\"width:100%\"><tr>");
                sbTextArea.AppendLine("    <td class=\"hidden-xs\" style=\"vertical-align:top;\" " + strLlClass + "><label id=\"" + ANG_LABEL_LEFT_PREFIX + RootTagId + "\" class=\"control-label hidden-xs " + cssLblLeft + "\">" + LabelLeft + " :" + "</label></td>");
                sbTextArea.AppendLine("    <td style=\"width:100%;vertical-align:top;\"><md-input-container id=\"" + ANG_CONTAINER_PREFIX + RootTagId + "\" class=\"" + cssInputContainer + " " + cssHasValue + "\" style=\"width:100%;\">");
                sbTextArea.AppendLine("        <label class=\"hidden-sm hidden-md hidden-lg reget-ang-lbl-control-top\" style=\"min-width:150px;\">" + LabelTop + "</label>");
                
                if(!String.IsNullOrEmpty(_prevControlHtml)) {
                    sbTextArea.AppendLine(_prevControlHtml);
                }

                sbTextArea.AppendLine("        <textarea " + strRequired + " id=\"" + textAreaId + "\" name=\"" + "txt" + RootTagId + "\" ng-model=\"" + m_ngModel + "\""
                    + " class=\"" + strClass + "\" " + strHeight + " " + NgHideEdit + "></textarea>");
                if (IsMandatory) {
                    sbTextArea.AppendLine("    <div class=\"reget-ang-mandatory-field\" " + NgHideEdit + "></div>");
                }
                sbTextArea.AppendLine("        <div ng-messages=\"" + FormName + "." + "txt" + RootTagId + ".$error\" " + strKeepErrMsgHidden + " >");
#if TEST
            sbTextArea.AppendLine("              <div ng-message=\"required\"><div class=\"reget-ang-controll-invalid-msg\">" + "Mandatory Text Field Text" + "</div></div>");
#else
                //sbTextArea.AppendLine("              <div ng-message=\"required\" class=\"reget-ang-controll-invalid-msg\">" + RequestResource.MandatoryTextField + "</div>");
                sbTextArea.AppendLine("              <div ng-message=\"required\"><div class=\"reget-ang-controll-invalid-msg\">" + RequestResource.MandatoryTextField + "</div></div>");
#endif
                sbTextArea.AppendLine("        </div>");
                sbTextArea.AppendLine("    </md-input-container></td>");
                sbTextArea.AppendLine("  </tr></table>");
                sbTextArea.AppendLine("</div>");
            }

            if (IsMandatory) {
                sbTextArea.AppendLine("<input id=\"" + "custLocV_" + textAreaId + "\" class=\"reget-custom-loc-valid-text\" value=\"" + RequestResource.MandatoryTextField + "\" />");
            }

            return sbTextArea.ToString();
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
    }
}