using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Kamsyk.Reget.AgControls {
    public class MdSelect : BaseAgControl {
        #region Properties
        //private string m_formName = null;
        //public string FormName {
        //    get { return m_formName; }
        //    set { m_formName = value; }
        //}

        private string m_agSourceList = null;
        public string AgSourceList {
            get { return m_agSourceList; }
            set { m_agSourceList = value; }
        }

        private string m_agModel = null;
        public string AgModel {
            get { return m_agModel; }
            set { m_agModel = value; }
        }

        private string m_agIdItem = null;
        public string AgIdItem {
            get { return m_agIdItem; }
            set { m_agIdItem = value; }
        }

        private string m_agTextItem = null;
        public string AgTextItem {
            get { return m_agTextItem; }
            set { m_agTextItem = value; }
        }

        private string m_agSelectedText = null;
        public string AgSelectedText {
            get { return m_agSelectedText; }
            set { m_agSelectedText = value; }
        }

        //private string m_agIsReadOnly = null;
        //public string AgIsReadOnly {
        //    get { return m_agIsReadOnly; }
        //    set { m_agIsReadOnly = value; }
        //}

        private string m_agItemDisable = null;
        public string AgItemDisable {
            get { return m_agItemDisable; }
            set { m_agItemDisable = value; }
        }

        //private string m_strRootTagId = null;
        //public string RootTagId {
        //    get { return m_strRootTagId; }
        //    set { m_strRootTagId = value; }
        //}

        private string m_onSelectFunct = null;
        public string OnSelectFunct {
            get { return m_onSelectFunct; }
            set { m_onSelectFunct = value; }
        }

        
        private bool m_isDisplayNoneSelect = false;
        public bool IsDisplayNoneSelect {
            get { return m_isDisplayNoneSelect; }
            set { m_isDisplayNoneSelect = value; }
        }
                
        private string m_strReadOnlyItemHtml = null;
        public string ReadOnlyItemHtml {
            get { return m_strReadOnlyItemHtml; }
            set { m_strReadOnlyItemHtml = value; }
        }

        private string m_strItemHtml = null;
        public string ItemHtml {
            get { return m_strItemHtml; }
            set { m_strItemHtml = value; }
        }

        private int m_width = 0;
        public int Width {
            get { return m_width; }
            set { m_width = value; }
        }

        private string m_placeHolder = null;
        public string Placeholder {
            get { return m_placeHolder; }
            set { m_placeHolder = value; }
        }
        #endregion

        #region Constructor
        
        #endregion

        #region Abstract Methods
        public override string RenderControlHtml() {
            StringBuilder sbControl = new StringBuilder();

            string strRequired = GetMandatoryJs();
            string strKeepErrMsgHidden = GetKeepErrMsgHiddenJs();
            
            string strCssBold = (IsBold) ? " reget-bold" : "";

            string strWidth = "";

            if (LabelWidth > 0) {
                strWidth = "style=\"min-width:" + LabelWidth + "px;text-align:right;\"";
            }

                        
            string ngHideErrMsg = "";
            if (!String.IsNullOrEmpty(AgIsMandatory)) {
                ngHideErrMsg = "ng-show=\"" + AgIsMandatory + "\"";
            }

            string strLeftLabelCss = "";
            if (!String.IsNullOrWhiteSpace(LabelLeftCssClass)) {
                strLeftLabelCss = LabelLeftCssClass;
            }

            string strCmbWidth = "";
            if (m_width > 0) {
                strCmbWidth = "style=\"min-width:150px;\"";
            }

            #region ReadOnly
            if (!String.IsNullOrEmpty(AgIsReadOnly) || IsReadOnly) {
                sbControl.AppendLine(GetReadOnlyHtml("{{" + m_agSelectedText + "}}"));

            }
            #endregion

            if (!IsReadOnly) {
                sbControl.AppendLine("<div id=\"" + ANG_WRAPPER_PREFIX + RootTagId + "\" class=\"" + GetContainerClass() + "\" " + NgHideEdit + " >");
                if (IsLeftLabelDisplayed) {
                    sbControl.AppendLine("<label id=\"" + ANG_LABEL_LEFT_PREFIX + RootTagId + "\" class=\"control-label hidden-xs " + strLeftLabelCss + "\" " + strWidth + ">" + LabelLeft + " :</label>");
                }

                string strClassContainer = "reget-ang-md-input-container";
                
                sbControl.AppendLine("<md-input-container id=\"" + ANG_CONTAINER_PREFIX + RootTagId + "\" class=\"" + strClassContainer + "\" " + strCmbWidth + " >");
                sbControl.AppendLine("    <label class=\"hidden-sm hidden-md hidden-lg reget-ang-lbl-control-top\"> " + LabelTop + "</label>");

                string strClass = "reget-ang-data-control";
                
                string strNgChange = "";
                if (!String.IsNullOrEmpty(m_onSelectFunct)) {
                    strNgChange = "ng-change=\"" + m_onSelectFunct + ";\"";
                }

                if (m_placeHolder == null) {
                    m_placeHolder = m_strLabelLeft;
                }
                string cmbPlaceholder = (String.IsNullOrEmpty(m_placeHolder)) ? "" : " placeholder=\"" + m_placeHolder + "\" ";


                sbControl.AppendLine("    <md-select " + strRequired + " name=\"" + "cmb" + RootTagId + "\" id=\"" + "cmb" + RootTagId + "\" ng-model=\"" + m_agModel
                    + "\" class=\"" + strClass + "" + strCssBold + "\" " + strNgChange + cmbPlaceholder + " md-no-asterisk>");
                if (!IsMandatory || m_isDisplayNoneSelect) {
#if TEST
                    sbControl.AppendLine("        <md-option><em>" + "CancelSelectText" + "</em></md-option>");
#else
                    //because of missing resources in Test Project
                    sbControl.AppendLine("        <md-option><em>" + RequestResource.CancelSelect + "</em></md-option>");
#endif
                }
                string ngDisabled = "";
                if (!String.IsNullOrEmpty(m_agItemDisable)) {
                    ngDisabled = " ng-disabled=\"" + m_agItemDisable + "\"";
                }
                sbControl.AppendLine("        <md-option ng-repeat=\"liElement in " + m_agSourceList + "\" ng-value=\"liElement." + m_agIdItem + "\"" + ngDisabled + " > ");
                if (m_strItemHtml == null) {
                    sbControl.AppendLine("            {{liElement." + m_agTextItem + "}}");
                } else {
                    sbControl.AppendLine("            " + m_strItemHtml);
                }
                sbControl.AppendLine("        </md-option>");
                sbControl.AppendLine("    </md-select>");

                if (IsMandatory) {
                    sbControl.AppendLine("    <div class=\"reget-ang-mandatory-field\"></div>");
                }

                sbControl.AppendLine("        <div ng-messages=\"" + FormName + "." + "cmb" + RootTagId + ".$error\" " + strKeepErrMsgHidden + " style=\"color:maroon\" role=\"alert\">");
#if TEST
                sbControl.AppendLine("              <div ng-message=\"required\"><div class=\"reget-ang-controll-invalid-msg\">" + "MandatoryTextFieldTest" + "</div></div>");
#else
                string strErrMsg = "";

                if (String.IsNullOrWhiteSpace(MandatoryErrMsg)) {
                    strErrMsg = RequestResource.MandatoryTextField;
                } else {
                    strErrMsg = MandatoryErrMsg;
                }

                sbControl.AppendLine("              <div ng-message=\"required\"><div class=\"reget-ang-controll-invalid-msg\">" + strErrMsg + "</div></div>");
#endif
                sbControl.AppendLine("        </div>");

                sbControl.AppendLine("</md-input-container>");
                sbControl.AppendLine("</div>");
            }

            return sbControl.ToString();
        }
        #endregion
    }
}