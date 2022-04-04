using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Kamsyk.Reget.AgControls {
    public class MdCheckBox : BaseAgControl {
        #region Properties
        private string m_agIsReadOnly = null;
        //public string AgIsReadOnly {
        //    get { return m_agIsReadOnly; }
        //    set { m_agIsReadOnly = value; }
        //}

        private string m_ckbText = null;
        public string CkbText {
            get { return m_ckbText; }
            set { m_ckbText = value; }
        }

        //private string m_leftLabelText = null;
        //public string LeftLabelText {
        //    get { return m_leftLabelText; }
        //    set { m_leftLabelText = value; }
        //}

        private string m_agIsChecked = null;
        public string AgIsChecked {
            get { return m_agIsChecked; }
            set { m_agIsChecked = value; }
        }

        private string m_yesText = null;
        public string YesText {
            get { return m_yesText; }
            set { m_yesText = value; }
        }

        private string m_noText = null;
        public string NoText {
            get { return m_noText; }
            set { m_noText = value; }
        }

        private string m_OnClick = null;
        public string OnClick {
            get { return m_OnClick; }
            set { m_OnClick = value; }
        }

       
        private string m_RootUrl = null;

        string cssInputContainer = "reget-ang-md-input-container-label";
        string cssHasValue = " md-input-has-value";


        #endregion

        #region Constructor
        public MdCheckBox(string rootUrl) {
            m_RootUrl = rootUrl;
        }
        #endregion

        #region Abstract Methods
        public override string RenderControlHtml() {
            StringBuilder sbCkb = new StringBuilder();

            //string ngShowRO = "";
            //string ngHideEdit = "";

            //if (!String.IsNullOrEmpty(m_agIsReadOnly)) {
            //    ngShowRO = "ng-show=\"" + m_agIsReadOnly + "\"";
            //    ngHideEdit = "ng-hide=\"" + m_agIsReadOnly + "\"";
                
            //}

            string cssInputContainer = "reget-ang-md-input-container-label";
            string cssLblLeft = null;
            if (String.IsNullOrEmpty(LabelLeftCssClass)) {
                cssLblLeft = "reget-ang-lbl-control-left";
            } else {
                cssLblLeft = LabelLeftCssClass;
            }

            bool isLeftLabel = (!String.IsNullOrWhiteSpace(m_strLabelLeft));

            //Enabled
            if (String.IsNullOrEmpty(m_agIsReadOnly) || !IsReadOnly) {
                if (isLeftLabel) {
                    string angPart = "";
                    if (RootTagId != null && RootTagId.Contains("{{")) {
                        int iAngPartStart = RootTagId.IndexOf("{");
                        angPart = RootTagId.Substring(iAngPartStart);

                        RootTagId = RootTagId.Split('{')[0];

                    }

                    sbCkb.AppendLine("<div id=\"" + ANG_WRAPPER_PREFIX + RootTagId + angPart + "\"" + " class=\"" + GetContainerClass() + "\" " + NgHideEdit + ">");
                    sbCkb.AppendLine("    <label id=\"" + ANG_LABEL_LEFT_PREFIX + RootTagId + "\" class=\"control-label hidden-xs " + cssLblLeft + "\">" + LabelLeft + " :" + "</label>");
                    sbCkb.AppendLine("    <md-input-container id=\"" + ANG_CONTAINER_PREFIX + RootTagId + "\" class=\"" + cssInputContainer + " " + cssHasValue + "\">");
                    sbCkb.AppendLine("        <label class=\"hidden-sm hidden-md hidden-lg reget-ang-lbl-control-top\" style=\"min-width:150px;\">" + LabelTop + "</label>");

                    sbCkb.AppendLine("  <md-checkbox id=\"" + RootTagId + "\" aria-label=\"" 
                        + m_ckbText + "\" class=\"reget-blue\" ng-checked=\"" + m_agIsChecked + "\" ng-click=\"" 
                        + m_OnClick + "\" style=\"margin-bottom:0px;\"> ");
                    sbCkb.AppendLine(m_ckbText);
                    sbCkb.AppendLine("  </md-checkbox>");

                    sbCkb.AppendLine("    </md-input-container>");
                    sbCkb.AppendLine("</div>");
                } else {
                    sbCkb.AppendLine("<div " + NgHideEdit + " style=\"float:left;margin-bottom:5px;\">");
                    sbCkb.AppendLine("  <md-checkbox id=\"" + RootTagId + "\" aria-label=\"" +
                        m_ckbText + "\" class=\"reget-blue\" ng-checked=\"" + m_agIsChecked + "\" ng-click=\"" +
                        m_OnClick + "\" style=\"margin-bottom:0px;\"> ");
                    sbCkb.AppendLine(m_ckbText);
                    sbCkb.AppendLine("  </md-checkbox>");
                    sbCkb.AppendLine("</div>");
                }
            }

            //Read Only
            //#region ReadOnly
            //if (!String.IsNullOrEmpty(AgIsReadOnly) || IsReadOnly) {
            //    sbCkb.AppendLine(GetReadOnlyHtml("{{" + m_agSelectedText + "}}"));

            //}
            //#endregion
            if (!String.IsNullOrEmpty(m_agIsReadOnly) || IsReadOnly) {

                string strYes = (String.IsNullOrWhiteSpace(m_yesText)) ? "Yes" : m_yesText;
                string strNo = (String.IsNullOrWhiteSpace(m_noText)) ? "No" : m_noText;

                sbCkb.AppendLine("<div " + NgShowRO + " class=\"" + GetContainerRoClass() + "\">");
                sbCkb.AppendLine("    <label id=\"" + ANG_LABEL_LEFT_PREFIX + RootTagId + "\" class=\"control-label hidden-xs " + cssLblLeft + "\">" + m_ckbText + " :" + "</label>");
                sbCkb.AppendLine("    <md-input-container id=\"" + ANG_CONTAINER_PREFIX + RootTagId + "\" class=\"" + cssInputContainer + " " + cssHasValue + "\">");
                sbCkb.AppendLine("        <label class=\"hidden-sm hidden-md hidden-lg reget-ang-lbl-control-top\" style=\"min-width:150px;\">" + m_ckbText + "</label>");

                sbCkb.AppendLine("        <div ng-if=\"" + m_agIsChecked + "==true\">" + strYes + "</div>");
                sbCkb.AppendLine("        <div ng-if=\"" + m_agIsChecked + "!=true\">" + strNo + "</div>");

                sbCkb.AppendLine("    </md-input-container>");
                sbCkb.AppendLine("</div>");
                //sbCkb.AppendLine("<div style=\"clear:both;\"></div>");
            }


            return sbCkb.ToString();
        }
        #endregion
    }
}