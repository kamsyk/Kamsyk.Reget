using Kamsyk.Reget.Controllers;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Kamsyk.Reget.AgControls {
    public class UiGrid : BaseAgControl {
        #region Enum
        public enum Gridtype {
            Standard,
            Simple
        }
        #endregion

        #region Properties
        private string m_RootUrl = null;

        private string m_agControlerName = null;
        public string AgControlerName {
            get { return m_agControlerName; }
            set { m_agControlerName = value; }
        }

        private string m_strTitle = null;
        public string Title {
            get { return m_strTitle; }
            set { m_strTitle = value; }
        }

        private string m_strAddNewText = null;
        public string AddNewText {
            get { return m_strAddNewText; }
            set { m_strAddNewText = value; }
        }

        private bool m_isHiddenAtStart = false;
        public bool IsHiddenAtStart {
            get { return m_isHiddenAtStart; }
            set { m_isHiddenAtStart = value; }
        }

        private Gridtype m_GridType = Gridtype.Standard;
        public Gridtype GridType {
            get { return m_GridType; }
            set { m_GridType = value; }
        }
        #endregion

        #region Constructor
        public UiGrid(string rootUrl) {
            m_RootUrl = rootUrl;
        }
        #endregion

        #region Abstract Methods
        public override string RenderControlHtml() {
            StringBuilder sb = new StringBuilder();

            string agControlerNameDot = (String.IsNullOrEmpty(m_agControlerName)) ? "" : m_agControlerName + ".";
            
            string strButtonStyle = "float:right;width:20px;min-height:22px;line-height:22px;min-width:36px;margin-right:0px;margin-left:0px;margin-top:0px;margin-bottom:0px;padding:4px;";

            sb.AppendLine("<div id=\"grdHeader_" + RootTagId + "\" class=\"reget-grid-header\">");

            
            if (!String.IsNullOrEmpty(m_strTitle)) {
                sb.AppendLine(" <div id=\"grdHeaderTitle_" + RootTagId + "\" style=\"float:left;margin-right:30px;padding:4px;\">" + m_strTitle + "</div>");
            }

            sb.AppendLine("     <div ng-dropdown-multiselect=\"\" options=\"angCtrl.columnsWhichCanBeHidden\" selected-model=\"angCtrl.columnsWhichCanBeHiddenModel\" checkboxes=\"true\""
                + " extra-settings=\"dropDownMultiSelectStyle\""
                + " events=\"onMultiSelect();\""
                //+ " buttonClasses=\"btn1\""
                + " translation-texts=\"{"
                + "  checkAll: '" + RequestResource.SelectAll + "',"
                + "  uncheckAll: '" + RequestResource.DeSelectAll + "',"
                + "  buttonDefaultText: '" + RequestResource.ColumnsDisplaying + "',"
                //+ "  dynamicButtonTextSuffix: 'item selectedq',"
                + " }\""
                + " ng-if=\"angCtrl.isColumnDisplayHideBtnVisible==true\" style=\"float:left;\"></div>");
            

            if (m_GridType == Gridtype.Standard) {
                sb.AppendLine("     <md-button id=\"" + "clearGridSettings_" + RootTagId + "\" style=\"" + strButtonStyle + "\" ng-hide=\"" + agControlerNameDot + "isValueNullOrUndefined(" + agControlerNameDot + "userGridSettings)\"");
                sb.AppendLine("           aria-label=\"ResetGridSettings\"");
                sb.AppendLine("           ng-click=\"" + agControlerNameDot + "resetGridSettings()" + "\">");
                sb.AppendLine("         <img ng-src=\"" + m_RootUrl + "Content\\Images\\Controll\\Grid\\reset.png\" arial-label=\"Reset\" style=\"width:18px;\" />");
                sb.AppendLine("         <md-tooltip>" + RequestResource.ResetDataGridSettings + "</md-tooltip>");
                sb.AppendLine("     </md-button>");

                sb.AppendLine("     <md-button id=\"" + "saveGridSettings_" + RootTagId + "\" style=\"" + strButtonStyle + "\"");
                sb.AppendLine("           aria-label=\"SaveGridSettings\"");
                sb.AppendLine("           ng-click=\"" + agControlerNameDot + "saveGridSettings()\">");
                sb.AppendLine("         <img ng-src=\"" + m_RootUrl + "Content\\Images\\Controll\\Grid\\save.png\" style=\"width:18px;\" />");
                sb.AppendLine("         <md-tooltip>" + RequestResource.SaveDataGridSettings + "</md-tooltip>");
                sb.AppendLine("     </md-button>");

                sb.AppendLine("<div style=\"border-right:solid 1px #95B8E7;float:right;padding-right:10px;margin-right:10px;\">");
                //sb.AppendLine("     <md-button id=\"" + "showGridColumns_" + RootTagId + "\" style=\"" + strButtonStyle + "\"");
                //sb.AppendLine("           aria-label=\"ShowHiddenColumns\"");
                //sb.AppendLine("           ng-click=\"" + agControlerNameDot + "showAllColumnsReload()\" ");
                //sb.AppendLine("           ng-hide=\"" + agControlerNameDot + "isColumnHidden == false\" />");
                //sb.AppendLine("         <img ng-src=\"" + m_RootUrl + "Content\\Images\\Controll\\Grid\\ShowHideCol.png\" style=\"height:12px;\" />");
                //sb.AppendLine("         <md-tooltip>" + RequestResource.ShowHiddenColumns + "</md-tooltip>");
                //sb.AppendLine("     </md-button>");

                
                sb.AppendLine("     <md-button id=\"" + "clearGridFilter_" + RootTagId + "\" style=\"" + strButtonStyle + "\"");
                sb.AppendLine("           aria-label=\"RemoveFilter\"");
                sb.AppendLine("           ng-click=\"" + agControlerNameDot + "clearFiltersReload()\" ");
                sb.AppendLine("           ng-hide=\"" + agControlerNameDot + "isFilterApplied == false\" />");
                sb.AppendLine("         <img ng-src=\"" + m_RootUrl + "Content\\Images\\Controll\\Grid\\RemoveFilter.png\" style=\"height:20px;\" />");
                sb.AppendLine("         <md-tooltip>" + RequestResource.ClearFilters + "</md-tooltip>");
                sb.AppendLine("     </md-button>");

                sb.AppendLine("</div>");
            }

            sb.AppendLine(" <div style=\"clear:both\"></div>");
            sb.AppendLine("</div>");


            sb.AppendLine("<div class=\"reget-grid-border\" ui-i18n=\"{{" + agControlerNameDot + "lang}}\" style=\"background-color:#fff;\" >");
            //*** Grid Tag **//
            sb.AppendLine("<div id =\"" + RootTagId + "\" ui-grid=\"" + agControlerNameDot + "gridOptions" + "\" ui-grid-resize-columns ui-grid-move-columns ui-grid-edit ui-grid-pagination ui-grid-auto-resize class=\"grid\" ></div>");
            if (m_GridType == Gridtype.Standard) {
                sb.AppendLine(GetDataGridFootPanelTs(
                    RootTagId,
                    m_agControlerName,
                    m_strAddNewText,
                    m_isHiddenAtStart));
            }
            sb.AppendLine("</div>");

            if (BaseController.IsTestMode) {
                sb.AppendLine("<div><span>DataGrid Load Data Count:</span><span id=\"testSpGridLoadDataCount\">{{angCtrl.testLoadDataCount}}</span></div>");
            }

            return sb.ToString();
        }
        #endregion

        #region Methods
        public string GetDataGridFootPanelTs(
            string strId,
            string agControlerName,
            string strAddNewText,
            //string idPrefix,
            bool isHiddenAtStart) {

            StringBuilder sb = new StringBuilder();

            //*********************** Throws error in F12 mode ***************************************************
            //Syntax error, unrecognized expression: td[ng-hide='IsValueNullOrUndefined(gridOptions.data) || gridOptions.data.length ='= 0 || (gridOptions.data.length == 1 && gridOptions.data[0'].id < 0)']
            string strHideControls = "IsValueNullOrUndefined(" + agControlerName + ".gridOption" + ".data) || " + agControlerName + ".gridOption" + ".data.length === 0 || (" + agControlerName + ".gridOption" + ".data.length === 1 && " + agControlerName + ".gridOption" + ".data[0].id < 0)";
            //string strHideControls = "regetgridservice.IsHideGridControls(" + strGridOption + ")";
            //*******************************************************************************************************

            sb.AppendLine(" <div style=\"overflow-x:auto;overflow-y:auto;background-color:#fff;\">");
            //sb.AppendLine("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"100%\" ng-hide=\"" + agControlerName + ".rowsCount==0" + "\">");
            sb.AppendLine("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"100%\" >");
            sb.AppendLine(" <tbody>");
            sb.AppendLine("     <tr>");

            sb.AppendLine("         <td ng-hide=\"" + strHideControls + "\">");
            sb.AppendLine("             <select id=\"" + strId + "_PageSize\" ng-model=\"" + (isHiddenAtStart ? "$parent." : "") + agControlerName + ".pageSize" + "\" ng-change=\"" + agControlerName + ".pageSizeChanged()" + "\" ng-options=\"o as o for o in " + agControlerName + ".gridOptions.paginationPageSizes\" style=\"border-color:#95B8E7;margin-left:8px;margin-right:8px;height:25px;\"></select>");
            sb.AppendLine("         </td>");
            sb.AppendLine("         <td ng-hide=\"" + strHideControls + "\"><div class=\"reget-grid-pagination-btn-separator\"></div></td>");
            sb.AppendLine("         <td ng-hide=\"" + strHideControls + "\">");
            sb.AppendLine("             <md-button arial-label=\"First Page\" ng-class=\"" + agControlerName + ".currentPage" + "==1 ? 'reget-grid-footer-button reget-grid-footer-button-first-disabled':'reget-grid-footer-button reget-grid-footer-button-first'\" ng-click=\"" + agControlerName + ".firstPage()" + "\" ng-disabled=\"" + agControlerName + ".currentPage" + "==1\">");
            sb.AppendLine("                 <md-tooltip ng-if=\"" + agControlerName + ".currentPage" + "!=1\">" + RequestResource.FirstPage + "</md-tooltip>");
            sb.AppendLine("             </md-button>");
            sb.AppendLine("         </td>");
            sb.AppendLine("         <td ng-hide=\"" + strHideControls + "\">");
            sb.AppendLine("             <md-button arial-label=\"Previous Page\" ng-class=\"" + agControlerName + ".currentPage" + "==1 ? 'reget-grid-footer-button reget-grid-footer-button-previous-disabled':'reget-grid-footer-button reget-grid-footer-button-previous'\" ng-click=\"" + agControlerName + ".previousPage()" + "\" ng-disabled=\"" + agControlerName + ".currentPage" + "==1\">");
            sb.AppendLine("                 <md-tooltip ng-if=\"" + agControlerName + ".currentPage" + "!=1\">" + RequestResource.PreviousPage + "</md-tooltip>");
            sb.AppendLine("             </md-button>");
            sb.AppendLine("         </td>");
            sb.AppendLine("         <td ng-hide=\"" + strHideControls + "\"><div class=\"reget-grid-pagination-btn-separator\"></div></td>");
            sb.AppendLine("         <td style=\"padding-left:8px;padding-right:8px;\" ng-hide='" + strHideControls + "'>");
            sb.AppendLine("             <table style=\"width:100%;\"><tr><td><input id=\"" + strId + "_PageIndex\" type =\"number\" ng-model=\"" + (isHiddenAtStart ? "$parent." : "") +
                agControlerName + ".currentPage" + "\" ng-change=\"" + agControlerName + ".gotoPage()" + "\" onkeypress='return event.charCode >= 48 && event.charCode <= 57' ng-class=\"{'reget-grid-current-page-2': " +
                agControlerName + ".getLastPageIndex()" + "<100, 'reget-grid-current-page-3': " + agControlerName + ".getLastPageIndex()" + ">=100, 'reget-grid-current-page-4': " + agControlerName + ".getLastPageIndex()" +
                ">=1000, 'reget-grid-current-page-5': " + agControlerName + ".getLastPageIndex()" + ">=10000, 'reget-grid-current-page-6': " + agControlerName + ".getLastPageIndex()" + ">=100000}\" style=\"padding-left:5px;border:1px solid #95B8E7;height:25px;\" /></td>");
            sb.AppendLine("             <td id=" + strId + "_PagesCount" + " style=\"white-space:nowrap;padding-left:5px;\"> of {{ " + agControlerName + ".getLastPageIndex()" + "}}</td></tr></table>");
            sb.AppendLine("         </td>");
            sb.AppendLine("         <td ng-hide=\"" + strHideControls + "\"><div class=\"reget-grid-pagination-btn-separator\"></div></td>");
            sb.AppendLine("         <td ng-hide=\"" + strHideControls + "\" >");
            sb.AppendLine("             <md-button arial-label=\"Next Page\" ng-class=\"" + agControlerName + ".currentPage" + "==" + agControlerName + ".getLastPageIndex()" + "?'reget-grid-footer-button reget-grid-footer-button-next-disabled':'reget-grid-footer-button reget-grid-footer-button-next'\" ng-click=\"" +
                agControlerName + ".nextPage()" + "\" ng-disabled=\"" + agControlerName + ".currentPage" + "==" + agControlerName + ".getLastPageIndex()" + "\">");
            sb.AppendLine("                 <md-tooltip ng-if=\"" + agControlerName + ".currentPage" + "!=" + agControlerName + ".getLastPageIndex()" + "\">" + RequestResource.NextPage + "</md-tooltip>");
            sb.AppendLine("             </md-button>");
            sb.AppendLine("         </td>");
            sb.AppendLine("         <td ng-hide=\"" + strHideControls + "\">");
            sb.AppendLine("             <md-button arial-label=\"Last Page\" ng-class=\"" + agControlerName + ".currentPage" + "==" + agControlerName + ".getLastPageIndex()" + "?'reget-grid-footer-button reget-grid-footer-button-last-disabled':'reget-grid-footer-button reget-grid-footer-button-last'\" ng-click=\"" +
                agControlerName + ".lastPage()" + "\" ng-disabled=\"" + agControlerName + ".currentPage" + "==" + agControlerName + ".getLastPageIndex()" + "\">");
            sb.AppendLine("                 <md-tooltip ng-if=\"" + agControlerName + ".currentPage" + "!=" + agControlerName + ".getLastPageIndex()" + "\">" + RequestResource.LastPage + "</md-tooltip>");
            sb.AppendLine("             </md-button>");
            sb.AppendLine("         </td>");
            sb.AppendLine("         <td ng-hide=\"" + strHideControls + "\"><div class=\"reget-grid-pagination-btn-separator\"></div></td>");
            sb.AppendLine("         <td>");
            sb.AppendLine("             <md-button class=\"btn reget-grid-footer-button reget-grid-footer-button-refresh\" ng-click=\"" + agControlerName + ".refresh()" + "\">");
            sb.AppendLine("                     <md-tooltip>" + RequestResource.Refresh + "</md-tooltip>");
            sb.AppendLine("             </md-button>");
            sb.AppendLine("         </td>");
            sb.AppendLine("         <td><div class=\"reget-grid-pagination-btn-separator\"></div></td>");
            if (!String.IsNullOrEmpty(strAddNewText)) {
                sb.AppendLine("         <td id=\"" + strId + "_tdNewRow" + "\">");
                sb.AppendLine("             <md-button id=\"" + strId + "_btnNewRow" + "\" class=\"btn reget-grid-footer-button reget-grid-footer-button-add\" ng-click=\"" + agControlerName + ".addNewRow()" + "\">");
                sb.AppendLine("                 <span class=\"hidden-xs hidden-sm\">" + strAddNewText + "<span><md-tooltip>" + strAddNewText + "</md-tooltip>");
                sb.AppendLine("             </md-button>");
                sb.AppendLine("         </td>");

                sb.AppendLine("         <td id=\"" + strId + "_btnNewRowSeparator" + "\"><div class=\"reget-grid-pagination-btn-separator\"></div></td>");
            }
            sb.AppendLine("         <td ng-hide=\"" + strHideControls + "\">");
            sb.AppendLine("             <md-button class=\"btn reget-grid-footer-button reget-grid-footer-button-excel\" ng-click=\"" + agControlerName + ".exportToXls()" + "\">");
            sb.AppendLine("                 <span class=\"hidden-xs hidden-sm\">" + RequestResource.ExportToExcel + "</span><md-tooltip>" + RequestResource.ExportToExcel + "</md-tooltip>");
            sb.AppendLine("             </md-button>");
            sb.AppendLine("         </td>");
            sb.AppendLine("         <td ng-hide=\"" + strHideControls + "\"><div class=\"reget-grid-pagination-btn-separator\"></div></td>");
            sb.AppendLine("         <td width=\"100%\" align=\"right\" style=\"padding-right:8px;\" >");
            string itemsInfo = RequestResource.DisplayingFromToOf
                .Replace("{0}", "{{" + agControlerName + ".getDisplayItemsFromInfo()" + "}}")
                .Replace("{1}", "{{" + agControlerName + ".getDisplayItemsToInfo()" + "}}")
                .Replace("{2}", "{{" + agControlerName + ".rowsCount" + "}}");
            string itemsInfoMp = "{{" + agControlerName + ".getDisplayItemsFromInfo()" + "}}" + " - "
                + "{{" + agControlerName + ".getDisplayItemsToInfo()" + "}}" + " / "
                + "{{" + agControlerName + ".rowsCount" + "}}";

            sb.AppendLine("             <span class=\"hidden-xs hidden-sm\" style=\"font-size:12px;\" ng-hide=\"" + strHideControls + "\"> " + itemsInfo + "</span>");
            sb.AppendLine("             <span class=\"hidden-md hidden-lg\" style=\"font-size:12px;\" ng-hide=\"" + strHideControls + "\"> " + itemsInfoMp + "</span>");
            sb.AppendLine("         </td>");
            sb.AppendLine(" </tr>");
            sb.AppendLine("</tbody>");
            sb.AppendLine("</table>");
            //sb.AppendLine("<div id=\"" + strId + "_DivNoRecord" + "\" style=\"margin-top:25px;margin-bottom:25px;text-align:center;color:#888;\" ng-show=\"" + agControlerName + ".rowsCount==0\">");
            //sb.AppendLine(RequestResource.NoRecordFound);
            //sb.AppendLine("</div>");
            sb.AppendLine("</div>");

            return sb.ToString();
        }
        #endregion
    }
}