using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Kamsyk.Reget.AgControls {
    public class MdLabel : BaseAgControl {
        
        #region Properties
        //private string m_formName = null;
        //public string FormName {
        //    get { return m_formName; }
        //    set { m_formName = value; }
        //}

        private string m_ngModel = null;
        public string NgModel {
            get { return m_ngModel; }
            set { m_ngModel = value; }
        }

        
        //private string m_strLabelLeft = null;
        //public string LabelLeft {
        //    get { return m_strLabelLeft; }
        //    set { m_strLabelLeft = value; }
        //}

        //private string m_strLabelTop = null;
        //public string LabelTop {
        //    get { return m_strLabelTop; }
        //    set { m_strLabelTop = value; }
        //}

        //private string m_strRootTagId = null;
        //public string RootTagId {
        //    get { return m_strRootTagId; }
        //    set { m_strRootTagId = value; }
        //}

        
        //private bool m_isMandatory = false;
        //public bool IsMandatory {
        //    get { return m_isMandatory; }
        //    set { m_isMandatory = value; }
        //}

        private int m_iWidth = 0;
        public int Width {
            get { return m_iWidth; }
            set { m_iWidth = value; }
        }

        private string m_inputCssClass = null;
        public string InputCssClass {
            get { return m_inputCssClass; }
            set { m_inputCssClass = value; }
        }

        //private string m_labelLeftCssClass = null;
        //public string LabelLeftCssClass {
        //    get { return m_labelLeftCssClass; }
        //    set { m_labelLeftCssClass = value; }
        //}
                
        #endregion

        #region Abstract Methods
        public override string RenderControlHtml() {
            //IsMandatory = false;

            //string strWidth = "";

            //if (m_iWidth > 0) {
            //    strWidth = "style=\"width:" + m_iWidth + "px;max-width:" + m_iWidth + "px;\"";
            //}

            //string cssHasValue = " md-input-has-value"; 

            //string cssInputContainer = "reget-ang-md-input-container-label";
            //string cssLblLeft = null;
            //if (String.IsNullOrEmpty(LabelLeftCssClass)) {
            //    cssLblLeft = "reget-ang-lbl-control-left";
            //} else {
            //    cssLblLeft = LabelLeftCssClass;
            //}

            //string strRequired = (m_isMandatory) ? "required" : "";

            //string angPart = "";
            //if (RootTagId != null && RootTagId.Contains("{{")) {
            //    int iAngPartStart = RootTagId.IndexOf("{");
            //    angPart = RootTagId.Substring(iAngPartStart);

            //    RootTagId = RootTagId.Split('{')[0];

            //}

            //string strInputClass = "reget-ang-data-control";
            //if (m_isMandatory) {
            //    strInputClass += " reget-ang-mandatory-field";
            //}

            //if (!String.IsNullOrEmpty(m_inputCssClass)) {
            //    strInputClass += " " + m_inputCssClass;
            //}

            //            StringBuilder sbTextBox = new StringBuilder();

            //            sbTextBox.AppendLine("<div id=\"" + ANG_WRAPPER_PREFIX + RootTagId + angPart + "\"" + " class=\"" + GetContainerClass() + "\">");
            //            sbTextBox.AppendLine("    <label id=\"" + ANG_LABEL_LEFT_PREFIX + RootTagId + "\" class=\"control-label hidden-xs " + cssLblLeft + "\">" + m_strLabelLeft + " :" + "</label>");
            //            sbTextBox.AppendLine("    <md-input-container id=\"" + ANG_CONTAINER_PREFIX + RootTagId + "\" class=\"" + cssInputContainer + " " + cssHasValue + "\">");
            //            sbTextBox.AppendLine("        <label class=\"hidden-sm hidden-md hidden-lg reget-ang-lbl-control-top\" style=\"min-width:150px;\">" + m_strLabelTop + "</label>");
            //            sbTextBox.AppendLine("        <div>" + "{{" + m_ngModel + "}}" + "</div>");

            //            sbTextBox.AppendLine("        <div ng-messages=\"" + FormName + "." + "txt" + RootTagId + ".$error\" >");
            //#if !TEST
            //            sbTextBox.AppendLine("              <div ng-message=\"required\" class=\"reget-ang-controll-invalid-msg\">" + RequestResource.MissingMandatoryValue + "</div>");
            //#endif
            //            sbTextBox.AppendLine("        </div>");
            //            sbTextBox.AppendLine("    </md-input-container>");
            //            sbTextBox.AppendLine("</div>");

            //            return sbTextBox.ToString();

            IsReadOnly = true;

            return GetReadOnlyHtml("{{" + m_ngModel + "}}");
        }
        #endregion
    }
}