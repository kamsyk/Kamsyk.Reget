using Kamsyk.Reget.Mail;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Kamsyk.Reget.AgControls {
    public class MdInputContainer : BaseAgControl {
        #region Enums
        public enum TextBoxType {
            Text,
            IntNumber,
            DecimalNumber,
            PhoneNumber,
            EMailAddress,
            MultiEMailAddresses
        }
        #endregion

        #region Properties
        
        private string m_ngModel = null;
        public string NgModel {
            get { return m_ngModel; }
            set { m_ngModel = value; }
        }
                
        public override bool IsMandatory {
            get { return base.IsMandatory; }
            set {
                m_isMandatory = value;
                //if (m_isMandatory) {
                //    m_strLabelLeft += " *";
                //    //m_strLabelTop += " *";
                //}
            }
        }

        public override string LabelLeft {
            get { return m_strLabelLeft; }
            set {
                m_strLabelLeft = value;
                if (m_strLabelLeft != null && m_strLabelTop == null) {
                    m_strLabelTop = m_strLabelLeft;
                    if (m_strLabelTop != null && m_strLabelTop.EndsWith("*")) {
                        m_strLabelTop = m_strLabelTop.Substring(0, m_strLabelTop.Length - 1);
                    }
                }
            }
        }

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

       
        private TextBoxType m_dataType = TextBoxType.Text;
        public TextBoxType DataType {
            get { return m_dataType; }
            set { m_dataType = value; }
        }

        private string m_leftImgUrl = null;
        public string LeftImgUrl {
            get { return m_leftImgUrl; }
            set { m_leftImgUrl = value; }
        }

        private string m_ngChange = null;
        public string OnChange {
            get { return m_ngChange; }
            set { m_ngChange = value; }
        }

        private string m_placeHolder = null;
        public string Placeholder {
            get { return m_placeHolder; }
            set { m_placeHolder = value; }
        }

        private int? m_maxLength = null;
        public int? MaxLength {
            get { return m_maxLength; }
            set { m_maxLength = value; }
        }

        #endregion

        #region Abstract Methods
        public override string RenderControlHtml() {
            string strWidth = "";
            
            if (m_iWidth > 0) {
                strWidth = "style=\"width:" + m_iWidth + "px;max-width:" + m_iWidth + "px;\"";
            }

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

            if (IsMandatory) {
                LabelLeft += " *";
            }

            string angPart = "";
            if (RootTagId != null && RootTagId.Contains("{{")) {
                int iAngPartStart = RootTagId.IndexOf("{");
                angPart = RootTagId.Substring(iAngPartStart);

                RootTagId = RootTagId.Split('{')[0];

            }

            string strInputClass = "reget-ang-data-control";
            
            if (!String.IsNullOrEmpty(m_inputCssClass)) {
                strInputClass += " " + m_inputCssClass;
            }

            string strInputId = "txt" + RootTagId;

            string strType = "";
            if (m_dataType == TextBoxType.DecimalNumber) {
                strType = " type=\"text\" onkeydown=\"validateDecimalNumber(event,'" + DecimalSeparator + "')\""
                     + " ng-change=\"angCtrl.priceWasChanged(" +
                                    m_ngModel + ", angCtrl.angScopeAny." + FormName + "['" + strInputId + "'])\"";
                strInputClass += " " + "reget-ang-data-control-number";
            } else if (m_dataType == TextBoxType.IntNumber) {
                strType = " type=\"text\" onkeydown=\"validateIntNumber(event)\"";
                strInputClass += " " + "reget-ang-data-control-number";
            } else if (m_dataType == TextBoxType.PhoneNumber) {
                strType = " type=\"text\" onkeydown=\"validatePhoneNumber(event)\"";
                //strInputClass += " " + "reget-ang-data-control-number";
            } else if (m_dataType == TextBoxType.EMailAddress) {
                strType = " type=\"email\" ";

            } else if (m_dataType == TextBoxType.MultiEMailAddresses) {
                strType = " type=\"text\" ng-change=\"angCtrl.validateMultiEmails(" +
                                    m_ngModel + ", angCtrl.angScopeAny." + FormName + "['" + strInputId + "'])\"";

            } 
            
            StringBuilder sbTextBox = new StringBuilder();

            if (!String.IsNullOrEmpty(AgIsReadOnly) || IsReadOnly) {
                sbTextBox.AppendLine(GetReadOnlyHtml("{{" + m_ngModel + "}}"));

            }

            bool isImgLeft = !String.IsNullOrEmpty(m_leftImgUrl);

            if (!IsReadOnly) {
                sbTextBox.AppendLine("<div id=\"" + ANG_WRAPPER_PREFIX + RootTagId + angPart + "\"" + " class=\"" + GetContainerClass() + "\" " + NgHideEdit + ">");
                if (isImgLeft) {
                    sbTextBox.AppendLine("    <div class=\"" + cssLblLeft + "\" style=\"float:left;\"><img src=\"" + m_leftImgUrl + "\" style=\"height:20px;\"></div>");
                } else { 
                    sbTextBox.AppendLine("    <label id=\"" + ANG_LABEL_LEFT_PREFIX + RootTagId + "\" class=\"control-label hidden-xs " + cssLblLeft + "\">" + LabelLeft + " :" + "</label>");
                }
                sbTextBox.AppendLine("    <md-input-container id=\"" + ANG_CONTAINER_PREFIX + RootTagId + "\" class=\"" + cssInputContainer + " " + cssHasValue + "\">");
                if (isImgLeft) {
                    sbTextBox.AppendLine("        <label class=\"hidden-sm hidden-md hidden-lg reget-ang-lbl-control-top\" style=\"min-width:150px;\">" + "" + "</label>");
                } else {
                    sbTextBox.AppendLine("        <label class=\"hidden-sm hidden-md hidden-lg reget-ang-lbl-control-top\" style=\"min-width:150px;\">" + LabelTop + "</label>");
                }

                string ngChangeFce = (String.IsNullOrEmpty(m_ngChange)) ? "" : " ng-change=\"" + m_ngChange + "\" ";
                string inputPlaceholder = (String.IsNullOrEmpty(m_placeHolder)) ? "" : " placeholder=\"" + m_placeHolder + "\" ";
                string strMaxLength = "";
                if (m_maxLength != null && m_maxLength > 0) {
                    strMaxLength = " maxlength=\"" + m_maxLength + "\"";
                }
                sbTextBox.AppendLine("        <input " + strRequired + " id=\"" + strInputId + "\" name=\"" + "txt" + RootTagId + "\" ng-model=\"" + m_ngModel + "\""
                    + strMaxLength
                    + ngChangeFce + strType + " class=\"" + strInputClass + "\" " + strWidth + " " + NgHideEdit + inputPlaceholder + "/>");

                if (IsMandatory) {
                    sbTextBox.AppendLine("    <div class=\"reget-ang-mandatory-field\" " + NgHideEdit + "></div>");

                }
                
                string strErrMsgStyle = "";
                if (m_iWidth > 0) {
                    strErrMsgStyle = "style=\"max-width:" + m_iWidth + "px;\"";
                }
                sbTextBox.AppendLine("        <div ng-messages=\"angCtrl.angScopeAny." + FormName + "." + "txt" + RootTagId + ".$error\" " + strKeepErrMsgHidden + " " + strErrMsgStyle + " >");
#if TEST
            if (m_dataType == TextBoxType.DecimalNumber) {
                sbTextBox.AppendLine("              <div ng-message=\"required\"><div class=\"reget-ang-controll-invalid-msg\">" + "Enter Decimal Test" + "</div></div>");
            } else {
                sbTextBox.AppendLine("              <div ng-message=\"required\"><div class=\"reget-ang-controll-invalid-msg\">" + "Enter Decimal Test" + "</div></div>");
            } 
#else
                //if(IsMandatory) { 
                string strErrMsg = "";
                if (m_dataType == TextBoxType.DecimalNumber) {
                    if (String.IsNullOrWhiteSpace(MandatoryErrMsg)) {
                        strErrMsg = RequestResource.EnterDecimalNumber;
                    } else {
                        strErrMsg = MandatoryErrMsg;
                    }
                } else if (m_dataType == TextBoxType.EMailAddress
                    || m_dataType == TextBoxType.MultiEMailAddresses) {
                    if (String.IsNullOrWhiteSpace(MandatoryErrMsg)) {
                        strErrMsg = RequestResource.EnterEMail;
                    } else {
                        strErrMsg = MandatoryErrMsg;
                    }
                } else {
                    if (String.IsNullOrWhiteSpace(MandatoryErrMsg)) {
                        strErrMsg = RequestResource.MissingMandatoryValue;
                    } else {
                        strErrMsg = MandatoryErrMsg;
                    }
                    
                }
                sbTextBox.AppendLine("              <div ng-message=\"required\"><div class=\"reget-ang-controll-invalid-msg\">" + strErrMsg + "</div></div>");
                if(m_dataType == TextBoxType.EMailAddress || m_dataType == TextBoxType.MultiEMailAddresses) { 
                    sbTextBox.AppendLine("              <div ng-message=\"email\"><div class=\"reget-ang-controll-invalid-msg\">" + strErrMsg + "</div></div>");
                }
#endif
                sbTextBox.AppendLine("        </div>");
                sbTextBox.AppendLine("    </md-input-container>");
                sbTextBox.AppendLine("</div>");
            }

            if (IsMandatory) {
                sbTextBox.AppendLine("<input id=\"" + "custLocV_" + strInputId + "\" class=\"reget-custom-loc-valid-text\" value=\"" + RequestResource.MandatoryTextField + "\" />");
            }

            return sbTextBox.ToString();
        }
        #endregion
    }
}