using Kamsyk.Reget.Controllers;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Kamsyk.Reget.AgControls {
    public class MdAutocomplete : BaseAgControl {
        #region Properties
        private string m_strClass = null;
        public string CssClass {
            get { return m_strClass; }
            set { m_strClass = value; }
        }

        private string m_mdInputId = null;
        public string MdInputId {
            get { return m_mdInputId; }
            set { m_mdInputId = value; }
        }

        private string m_mdItemTemplate = null;
        public string MdItemTemplate {
            get { return m_mdItemTemplate; }
            set { m_mdItemTemplate = value; }
        }

        private string m_mdSelectItem = null;
        public string MdSelectItem {
            get { return m_mdSelectItem; }
            set { m_mdSelectItem = value; }
        }

        private string m_mdSearchText = null;
        public string MdSearchText {
            get { return m_mdSearchText; }
            set { m_mdSearchText = value; }
        }

        private string m_mdItems = null;
        public string MdItems {
            get { return m_mdItems; }
            set { m_mdItems = value; }
        }

        private string m_mdItemText = null;
        public string MdItemText {
            get { return m_mdItemText; }
            set { m_mdItemText = value; }
        }

        private string m_placeholder = null;
        public string Placeholder {
            get { return m_placeholder; }
            set { m_placeholder = value; }
        }

        private string m_OnSelectedItemChange = null;
        public string OnSelectedItemChange {
            get { return m_OnSelectedItemChange; }
            set { m_OnSelectedItemChange = value; }
        }

        private string m_OnBlur = null;
        public string OnBlur {
            get { return m_OnBlur; }
            set { m_OnBlur = value; }
        }

        private int m_minLength = 0;
        public int MinLength {
            get { return m_minLength; }
            set { m_minLength = value; }
        }

        private int m_width = 0;
        public int Width {
            get { return m_width; }
            set { m_width = value; }
        }

        //private int m_iLabelWidth = 0;
        //public int LabelWidth {
        //    get { return m_iLabelWidth; }
        //    set { m_iLabelWidth = value; }
        //}

        //private bool m_isBold = false;
        //public bool IsBold {
        //    get { return m_isBold; }
        //    set { m_isBold = value; }
        //}

        //private string m_agIsReadOnly = null;
        //public string AgIsReadOnly {
        //    get { return m_agIsReadOnly; }
        //    set { m_agIsReadOnly = value; }
        //}

        //private bool m_isReadOnly = false;

        #endregion

        #region Abstract Methods
        public override string RenderControlHtml() {
            StringBuilder sbAuto = new StringBuilder();

            if (String.IsNullOrEmpty(LabelTop) || LabelTop.Trim() == "*") {
                LabelTop = LabelLeft;
                if (IsMandatory && LabelTop != null) {
                    LabelTop += "*";
                }
            }

            string angPart = "";
            if (RootTagId != null && RootTagId.Contains("{{")) {
                int iAngPartStart = RootTagId.IndexOf("{");
                angPart = RootTagId.Substring(iAngPartStart);

                RootTagId = RootTagId.Split('{')[0];

            }

            string strMandatory = GetMandatoryJs();
            string strKeepErrMsgHidden = GetKeepErrMsgHiddenJs();

            string strLlStyleTop = "";
            //if (AgIsReadOnly != null) {
            //    strLlStyleTop = "ng-style=\"{'margin-bottom':(" + AgIsReadOnly + ") ? '' : ''}\"";
            //}

            string strLeftLabelCss = "";
            if (!String.IsNullOrWhiteSpace(LabelLeftCssClass)) {
                strLeftLabelCss = LabelLeftCssClass;
            }

            string strWidth = "";

            string strCssBold = (IsBold) ? " reget-bold" : "";

            if (LabelWidth > 0) {
                strWidth = "style=\"min-width:" + LabelWidth + "px;text-align:right;\"";
            }

            #region ReadOnly
            if (!String.IsNullOrEmpty(AgIsReadOnly) || IsReadOnly) {
                sbAuto.AppendLine(GetReadOnlyHtml("{{" + m_mdSelectItem + "}}"));

            }
            #endregion

            if (!IsReadOnly) {

                /*1*/
                sbAuto.AppendLine("<div id=\"" + ANG_WRAPPER_PREFIX + RootTagId + "\" class=\"" + GetContainerClass() + "\"" + NgHideEdit  + ">");
                if (IsLeftLabelDisplayed) {
                    string strLlStyle = "";
                    if (AgIsReadOnly != null) {
                        strLlStyle = "ng-style=\"{'margin-top':(" + AgIsReadOnly + ") ? '' : '10px','margin-right':(" + AgIsReadOnly + ") ? '10px' : ''}\"";
                    } else {
                        strLlStyle = "ng-style=\"{'margin-top': '10px','margin-right': '10px'}\"";
                    }
                    if (!String.IsNullOrWhiteSpace(LabelLeft)) {
                        sbAuto.AppendLine("<label id=\"" + ANG_LABEL_LEFT_PREFIX + RootTagId + "\" class=\"control-label hidden-xs " + LabelLeftCssClass + "\" style=\"float:left;\" " + strLlStyle + " >" + LabelLeft + " :</label>");
                    }
                }
                    /*1.1*/
                    sbAuto.AppendLine("<div id=\"" + "div" + ANG_WRAPPER_PREFIX + RootTagId + angPart + "\">");
                if (!String.IsNullOrWhiteSpace(LabelTop)) {
                    sbAuto.AppendLine(" <md-input-container class=\"reget-ang-md-input-container md-input-has-value\" style=\"float:left;\">");
                    sbAuto.AppendLine("     <label class=\"hidden-sm hidden-md hidden-lg reget-ang-lbl-control-top\" " + strLlStyleTop + ">" + LabelTop + "</label>");
                    sbAuto.AppendLine(" </md-input-container>");
                }
                        /*1.1.1*/
                        sbAuto.AppendLine(" <div style=\"float:left;\" class=\"reget-dropdown-auto-wrap\" " + NgHideEdit + ">");
                        //sbAuto.AppendLine(" <div style=\"min-height:8px;\" class=\"hidden-sm hidden-md hidden-lg\"></div>");
                        sbAuto.AppendLine("     <md-autocomplete flex id=\"" + RootTagId + "\"");
                        sbAuto.AppendLine("                          name=\"" + RootTagId + "\"");
                        sbAuto.AppendLine("                         " + strMandatory);
                        sbAuto.AppendLine("                          md-input-minlength=\"" + m_minLength + "\"");
                        sbAuto.AppendLine("                          md-min-length=\"" + m_minLength + "\"");
                        sbAuto.AppendLine("                          ng-disabled=\"false\"");
                        sbAuto.AppendLine("                          md-no-cache=\"true\"");
                        sbAuto.AppendLine("                          md-delay=\"300\"");
                        sbAuto.AppendLine("                          md-selected-item=\"" + m_mdSelectItem + "\"");
                        sbAuto.AppendLine("                          md-search-text=\"" + m_mdSearchText + "\"");
                        if (!String.IsNullOrEmpty(m_OnSelectedItemChange)) {
                            sbAuto.AppendLine("                          md-selected-item-change=\"" + m_OnSelectedItemChange + "\"");
                        }
                        sbAuto.AppendLine("                          md-items=\"" + m_mdItems + "\"");
                        sbAuto.AppendLine("                          md-item-text=\"" + m_mdItemText + "\"");
                        sbAuto.AppendLine("                          placeholder=\"" + m_placeholder + "\"");
                        sbAuto.AppendLine("                          md-input-id=\"" + m_mdInputId + "\"");
                        sbAuto.AppendLine("                          md-input-name=\"" + m_mdInputId + "\"");
                        if (!String.IsNullOrEmpty(m_OnBlur)) {
                            sbAuto.AppendLine("                          ng-blur=\"" + m_OnBlur + "\"");
                        }
                        string autClass = "reget-autocomplete-white";
                        if (!String.IsNullOrEmpty(m_strClass)) {
                            autClass += " " + m_strClass;
                        }
                        sbAuto.AppendLine("                          class=\"" + m_strClass + "\">");

                        sbAuto.AppendLine(" <md-item-template>");
                        sbAuto.AppendLine(m_mdItemTemplate);
                        sbAuto.AppendLine(" </md-item-template>");

                        sbAuto.AppendLine(" <md-not-found>");
                        sbAuto.AppendLine("     \"{{" + m_mdSearchText + "}}\" " + @RequestResource.NotFound + ".");
                        sbAuto.AppendLine(" </md-not-found>");
                
                        sbAuto.AppendLine(" </md-autocomplete>");

                        if (IsMandatory) {
                            sbAuto.AppendLine("    <div class=\"reget-ang-mandatory-field\"></div>");
                        }

                        string strErrMsg = @RequestResource.MandatoryTextField;
                        if (String.IsNullOrWhiteSpace(MandatoryErrMsg)) {
                            strErrMsg = RequestResource.MandatoryTextField;
                        } else {
                            strErrMsg = MandatoryErrMsg;
                        }

                        sbAuto.AppendLine(" <div " + strKeepErrMsgHidden + " class=\"md-input-message-animation ng-scope ng-enter ng-enter-active\" style=\"position:relative;color:maroon;\" role=\"alert\">");
                                sbAuto.AppendLine("     <div id=\"" + RootTagId + BaseController.ANG_ERR_MSG_MANDATORY + "\" style=\"display:none;line-height:14px;color:rgb(221,24,0);padding-top:5px;font-size:12px;\"><div class=\"reget-ang-controll-invalid-msg\">" + strErrMsg + "</div></div>");
                                sbAuto.AppendLine(" </div>");

                        sbAuto.AppendLine(" </div>");
                        /*1.1.1*/

                    sbAuto.AppendLine("</div>");
                    /*1.1*/

                    sbAuto.AppendLine("<div style=\"clear:both;\"></div>");
                sbAuto.AppendLine("</div>");
                /*1*/
            }

            sbAuto.AppendLine("<div style=\"clear:both;\"></div>");

            if (IsMandatory) {
                sbAuto.AppendLine("<input id=\"" + "custLocV_" + m_mdInputId + "\" class=\"reget-custom-loc-valid-text\" value=\"" + RequestResource.MandatoryTextField + "\" />");
            }

            return sbAuto.ToString();
        }
        #endregion
    }
}