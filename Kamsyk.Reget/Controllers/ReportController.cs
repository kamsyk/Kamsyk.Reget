using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Kamsyk.ExcelOpenXml;
using Kamsyk.Reget.Content.Sessions;
using Kamsyk.Reget.Controllers.Interface;
using Kamsyk.Reget.Model;
using Kamsyk.Reget.Model.Common;
using Kamsyk.Reget.Model.ExtendedModel;
using Kamsyk.Reget.Model.ExtendedModel.PurchaseGroup;
using Kamsyk.Reget.Model.Repositories;
using Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Reflection;
using System.Reflection.Emit;
using System.Web;
using System.Web.Mvc;
using static Kamsyk.Reget.Model.Repositories.PgRepository;

namespace Kamsyk.Reget.Controllers {
    public class ReportController : BaseController {

        #region Constants
        private const int STYLE_GREEN_BKG_BOLD = 1;
        private const int STYLE_BOLD = 2;
        private const int STYLE_GREEN_APPROVAL_ONLY_BKG_BOLD = 3;
        #endregion

        #region Enum
       
        #endregion

        #region Properties
        private SpreadsheetDocument _m_xlsDoc;
        private SpreadsheetDocument m_xlsDoc {
            get {
                if (_m_xlsDoc == null) {
                    if (Session[RegetSession.SES_XLS_DOC] == null) {
                        WorkbookPart wbPart;
                        Session[RegetSession.SES_XLS_DOC] = GetAppMatrixExcelDoc(m_Excel, out wbPart);
                        Session[RegetSession.SES_WB_PART] = wbPart;
                    }

                    _m_xlsDoc = (SpreadsheetDocument)Session[RegetSession.SES_XLS_DOC];
                    
                }

                return _m_xlsDoc;
            }
        }

        private WorkbookPart _m_wbPart;
        private WorkbookPart m_wbPart {
            get {
                if (_m_wbPart == null) {
                    if (Session[RegetSession.SES_WB_PART] == null) {
                        WorkbookPart wbPart;
                        Session[RegetSession.SES_XLS_DOC] = GetAppMatrixExcelDoc(m_Excel, out wbPart);
                        Session[RegetSession.SES_WB_PART] = wbPart;
                    }
                    _m_wbPart = (WorkbookPart)Session[RegetSession.SES_WB_PART];

                }

                return _m_wbPart;
            }
        }

        private Excel _m_Excel;
        private Excel m_Excel {
            get {
                if (_m_Excel == null) {
                    if (Session[RegetSession.SES_EXCEL] == null) {
                        Session[RegetSession.SES_EXCEL] = new Excel();
                    }
                    _m_Excel = (Excel)Session[RegetSession.SES_EXCEL];
                }

                return _m_Excel;
            }
        }
        #endregion

        #region Http Methods
        // GET: Report
        [HttpGet]
        public ActionResult GetParticipants(string filter, string sort) {

            try {
                List<int> compIds = CurrentUser.ParticipantAdminCompanyIds;

                List<ParticipantsExtended> participants = new UserRepository().GetParticipantsReport(
                    compIds,
                    filter,
                    sort,
                    UserRepository.UserDisplayType.Users);
                
                var outputStream = GetParticipantExcelStream(participants);

                return File(outputStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Users" + GetTimeStampFileSuffix() + ".xlsx");
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpGet]
        public ActionResult GetNonActiveParticipants(string filter, string sort) {
            List<int> compIds = CurrentUser.ParticipantAdminCompanyIds;

            List<ParticipantsExtended> participants = new UserRepository().GetParticipantsReport(
                compIds,
                filter,
                sort,
                UserRepository.UserDisplayType.NonActiveUsers);

            var outputStream = GetParticipantExcelStream(participants);

            return File(outputStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "NonActiveUsers" + GetTimeStampFileSuffix() + ".xlsx");
        }

        [HttpGet]
        public ActionResult GetCentresReport(string filter, string sort) {
            try {
                List<int> compIds = CurrentUser.ParticipantAdminCompanyIds;

                List<Centre> centres = new CentreRepository().GetCentresReport(
                    compIds,
                    filter,
                    sort,
                    RequestResource.Always,
                    RequestResource.Never,
                    RequestResource.Optional);

                var outputStream = GetCentresExcelStream(centres);

                return File(outputStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Centres" + GetTimeStampFileSuffix() + ".xlsx");
            } catch (Exception ex) {
                ExcelClear();
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpGet]
        public ActionResult GetUsedPgReport(string filter, string sort) {
            try {
                List<int> compIds = CurrentUser.ParticipantAdminCompanyIds;
                var ppg = new PgRepository().GetUsedPgReport(filter, sort, CurrentCultureCode, CurrentUser.ParticipantId, compIds);
                                
                var outputStream = GetUsedPgExcelStream(ppg);

                string xlsName = GetReportName(ppg.ElementAt(0).parent_pg_loc_name + " Purchase Groups");
               
                return File(outputStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", xlsName + GetTimeStampFileSuffix() + ".xlsx");

            } catch (Exception ex) {
                ExcelClear();
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpGet]
        public ActionResult GetParentPgReport(string filter, string sort) {
            try {
                int companyGrouId = CurrentUser.Participant.company_group_id;
                var ppg = new ParentPgRepository().GetParentPgReport(companyGrouId, filter, sort, CurrentCultureCode, CurrentUser.ParticipantId);

                var outputStream = GetParentPgExcelStream(ppg);

                string xlsName = GetReportName("ParentPurchaseGroups");

                return File(outputStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", xlsName + GetTimeStampFileSuffix() + ".xlsx");

            } catch (Exception ex) {
                ExcelClear();
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpGet]
        public ActionResult GetSuppliersReport(int companyId) {
            try {
                List<int> compIds = CurrentUser.ParticipantAdminCompanyIds;

                List<Supplier> suppliers = new SupplierRepository().GetSuppliersReport(companyId);

                var outputStream = GetSuppliersExcelStream(suppliers);

                return File(outputStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Suppliers" + GetTimeStampFileSuffix() + ".xlsx");
            } catch (Exception ex) {
                ExcelClear();
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpGet]
        public ActionResult GetAddressesReport(string filter, string sort) {
            try {
                string decodedFilter = DecodeUrl(filter);
                List<int> compIds = CurrentUser.ParticipantAdminCompanyIds;

                List<AddressAdminExtended> addresses = new AddressRepository().GetAddressAdminData(compIds, decodedFilter, sort);

                var outputStream = GetAddressExcelStream(addresses);

                return File(outputStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ManagerSubst" + GetTimeStampFileSuffix() + ".xlsx");
            } catch (Exception ex) {
                ExcelClear();
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        
        [HttpGet]
        public ActionResult GetCentreGroup(int cgId) {
            try {
                var cg = new CentreGroupRepository().GetCentreGroupFullById(cgId);
                                
                Excel excel = new Excel();
                WorkbookPart wbPart = null; 
                var xlsDoc = GetAppMatrixExcelDoc(excel, out wbPart);

                var wsPart = excel.AddSheet(wbPart, cg.name);
                var sheetdata = wsPart.Worksheet.GetFirstChild<SheetData>();
                wsPart.Worksheet.InsertBefore(GetAppMatrixColumns(), sheetdata);

                int iRow = 1;
                
                MergeCells mergeCells = new MergeCells();
                GetCentreGroupHeader(excel, wsPart, cg, mergeCells, ref iRow);
                GetCentreGroupAppMatrix(excel, wsPart, cg, mergeCells, iRow);

                wsPart.Worksheet.InsertAfter(mergeCells, wsPart.Worksheet.Elements<SheetData>().First());

                xlsDoc.Close();

                var outputStream = excel.GetWorkbookMemoryStream();

                return File(outputStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CentreGroup" + GetTimeStampFileSuffix() + ".xlsx");
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpGet]
        public ActionResult GetCompanyCentreGroup(int cgId, int cgIndex, int cgCount, string companyName) {
            try {
                string decodedCompanyName = DecodeUrl(companyName);

                var cg = new CentreGroupRepository().GetCentreGroupFullById(cgId);

                //if (cgIndex < 30) {
                //int iSheetIndex = cgIndex + 1;
                var wsPart = m_Excel.AddSheet(m_wbPart, cg.name);
                //var ws = wsPart.Worksheet;
                var sheetdata = wsPart.Worksheet.GetFirstChild<SheetData>();
                wsPart.Worksheet.InsertBefore(GetAppMatrixColumns(), sheetdata);

                int iRow = 1;
                MergeCells mergeCells = new MergeCells();
                GetCentreGroupHeader(m_Excel, wsPart, cg, mergeCells, ref iRow);
                GetCentreGroupAppMatrix(m_Excel, wsPart, cg, mergeCells, iRow);

                wsPart.Worksheet.InsertAfter(mergeCells, wsPart.Worksheet.Elements<SheetData>().Last());
                //}

                if ((cgCount - 1) == cgIndex) {
                    m_xlsDoc.Close();
                   
                    var outputStream = m_Excel.GetWorkbookMemoryStream();

                    ExcelClear();

                    string docName = decodedCompanyName.Replace(" ","_").Replace(".", "_") + GetTimeStampFileSuffix() + ".xlsx";

                    return File(outputStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", docName);
                } else {
                    RefreshAppMatrixSessions();
                    return GetJson(true);
                }

            } catch (Exception ex) {
                ExcelClear();
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            } 
        }

        [HttpGet]
        public ActionResult GetUserSubstitutionReport(string filter, string sort) {
            try {
                List<int> compIds = CurrentUser.ParticipantAdminCompanyIds;

                List<UserSubstitutionExtended> substitutions = new SubstitutionRepository().GetSubstitution(
                    CurrentUser.ParticipantId,
                    compIds, 
                    filter, 
                    sort);

                var outputStream = GetSubstitutionExcelStream(substitutions);

                return File(outputStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ManagerSubst" + GetTimeStampFileSuffix() + ".xlsx");
            } catch (Exception ex) {
                ExcelClear();
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpGet]
        public ActionResult GetChart(
            string strDateFrom,
            string strDateTo,
            string companiesList,
            int xItemType,
            int yItemType,
            int periodType,
            string xItemList,
            int currencyId,
            int chartType,
            int iTop,
            bool isActiveOnly) {

            int[] companiesId = StatisticsController.GetCompaniesIds(companiesList);
            List<Exchange_Rate> exchangeRate = null;

            if (periodType != StatisticsController.FILTER_PERIOD_ALL) {
                iTop = 0;
            }

            SortedList<string, List<StatisticsController.Period>> chartSourceDataList = new StatisticsController().GetStatisticsData(
                strDateFrom,
                strDateTo,
                companiesId,
                xItemType,
                yItemType,
                periodType,
                xItemList,
                iTop,
                isActiveOnly,
                ref currencyId,
                out exchangeRate);

            DateTime dateFrom = ConvertUrlStringToDate(strDateFrom);
            DateTime dateTo = ConvertUrlStringToDate(strDateTo);
            List<StatisticsController.Period> allPeriods = new StatisticsController().GetAllPeriods(periodType, dateFrom, dateTo);
            List<string> colHeaders = new StatisticsController().GetChartLabels(periodType, dateFrom, dateTo, allPeriods);

            DataTable chartData = new DataTable("Data");
            foreach (KeyValuePair<string, List<StatisticsController.Period>> pair in chartSourceDataList) {
                if (chartData.Columns == null || chartData.Columns.Count == 0) {
                    DataColumn col = new DataColumn(GetChartFirstColumn(xItemType), typeof(string));
                    chartData.Columns.Add(col);

                    foreach (var colName in colHeaders) {
                        col = new DataColumn(colName, typeof(int));
                        chartData.Columns.Add(col);
                    }
                }

                DataRow newRow = chartData.NewRow();
                newRow[0] = StatisticsController.GetChartDatasetLabel(pair.Key);
                int iCol = 1;
                foreach (var period in allPeriods) {
                    newRow[iCol] = GetChartDataPeriodValue(pair.Value, period.PeriodStart, period.PeriodEnd);
                    iCol++;
                }
                chartData.Rows.Add(newRow);
            }
                        
            string chartTitle = StatisticsController.GetChartTitle(
                xItemType,
                yItemType,
                periodType,
                dateFrom,
                dateTo,
                currencyId,
                null);

            Excel.ChartType exChartType = GetExcelChartType(chartType);
            if (exChartType == Excel.ChartType.xlPie) {
                if (chartData.Columns.Count <= 2) {
                    chartData = TransponeData(chartData);
                } else {
                    exChartType = Excel.ChartType.xlColumnClustered;
                }
            }

            MemoryStream outputStream = new Excel().GenerateExcelWorkbook(chartData, exChartType, chartTitle);
            
            return File(outputStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Suppliers" + GetTimeStampFileSuffix() + ".xlsx");
        }




        #endregion
               
        #region Methods
        private SpreadsheetDocument GetAppMatrixExcelDoc(Excel excel, out WorkbookPart wbPart) {
            var xlsDoc = excel.GetNewXlsDocMemory();
            wbPart = excel.AddWorkbook(xlsDoc);
            
            //string sheetName = cg.name;
            //if (sheetName.Length > 10) {
            //    sheetName = sheetName.Substring(0, 10) + "...";
            //}
            //var wsPart = excel.AddWorkSheet(wbPart, sheetName);

            //var worksheetParts = wbPart.WorksheetParts;
            //var ws = wsPart.Worksheet;

            #region Styles
            var stylesPart = xlsDoc.WorkbookPart.AddNewPart<WorkbookStylesPart>();
            stylesPart.Stylesheet = new Stylesheet();

            // blank font list
            stylesPart.Stylesheet.Fonts = new Fonts();
            stylesPart.Stylesheet.Fonts.Count = 2;
            stylesPart.Stylesheet.Fonts.AppendChild(new Font());
            Font headeFont = new Font(new Bold());
            //headeFont.Bold = new Bold();
            stylesPart.Stylesheet.Fonts.AppendChild(headeFont);

            // create fills
            stylesPart.Stylesheet.Fills = new Fills();

            // create a solid green fill
            var solidGreen = new PatternFill() { PatternType = PatternValues.Solid };
            solidGreen.ForegroundColor = new ForegroundColor { Rgb = HexBinaryValue.FromString("99ff99") }; // red fill
            solidGreen.BackgroundColor = new BackgroundColor { Indexed = 64 };

            // create approval only solid green fill
            var approvalOnlyGreen = new PatternFill() { PatternType = PatternValues.Solid };
            approvalOnlyGreen.ForegroundColor = new ForegroundColor { Rgb = HexBinaryValue.FromString("00b050") }; // red fill
            approvalOnlyGreen.BackgroundColor = new BackgroundColor { Indexed = 64 };

            stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = new PatternFill { PatternType = PatternValues.None } }); // required, reserved by Excel
            stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = new PatternFill { PatternType = PatternValues.Gray125 } }); // required, reserved by Excel
            stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = solidGreen });
            stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = approvalOnlyGreen });
            stylesPart.Stylesheet.Fills.Count = 4;

            // blank border list
            stylesPart.Stylesheet.Borders = new Borders();
            stylesPart.Stylesheet.Borders.Count = 1;
            stylesPart.Stylesheet.Borders.AppendChild(new Border());

            // blank cell format list
            stylesPart.Stylesheet.CellStyleFormats = new CellStyleFormats();
            stylesPart.Stylesheet.CellStyleFormats.Count = 1;
            stylesPart.Stylesheet.CellStyleFormats.AppendChild(new CellFormat());

            // cell format list
            stylesPart.Stylesheet.CellFormats = new CellFormats();
            // empty one for index 0, seems to be required
            stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat());


            //****************************************************************************************************************************************

            //Header Style
            // cell format references style format 0, font 0, border 0, fill 2 and applies the fill
            stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat { FormatId = 0, FontId = 1, BorderId = 0, FillId = 2, ApplyFill = true }).AppendChild(new Alignment { Horizontal = HorizontalAlignmentValues.Left });

            //Bold Style
            stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat { FormatId = 0, FontId = 1, BorderId = 0, FillId = 0, ApplyFill = false }).AppendChild(new Alignment { Horizontal = HorizontalAlignmentValues.Left });
            
            //approval only
            stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat { FormatId = 0, FontId = 1, BorderId = 0, FillId = 3, ApplyFill = false }).AppendChild(new Alignment { Horizontal = HorizontalAlignmentValues.Left });


            stylesPart.Stylesheet.CellFormats.Count = 4;

            //**********************************************************************************************************************************************

            stylesPart.Stylesheet.Save();
            #endregion

            //Columns columns = new Columns();
            //uint iIndex = Convert.ToUInt32(1);
            //columns.Append(Excel.CreateColumnData(iIndex, iIndex, 25));

            //iIndex = Convert.ToUInt32(2);
            //columns.Append(Excel.CreateColumnData(iIndex, iIndex, 25));

            //iIndex = Convert.ToUInt32(3);
            //columns.Append(Excel.CreateColumnData(iIndex, iIndex, 25));

            //var sheetdata = wsPart.Worksheet.GetFirstChild<SheetData>();
            //wsPart.Worksheet.InsertBefore(columns, sheetdata);

            //MergeCells mergeCells = new MergeCells();

            return xlsDoc;
        }

        private void GetCentreGroupHeader(
            Excel excel, 
            //Worksheet ws, 
            WorksheetPart wsPart,
            Centre_Group cg,
            MergeCells mergeCells,
            ref int iRow) {

            var ws = wsPart.Worksheet;

            //MergeCells mergeCells = new MergeCells();

            //append a MergeCell to the mergeCells for each set of merged cells
            mergeCells.Append(new MergeCell() { Reference = new StringValue("A1:C1") });
            //ws.InsertAfter(mergeCells, wsPart.Worksheet.Elements<SheetData>().First());


            string cellAddress = "A1";
            var cell = excel.InsertCellInWorksheet(ws, cellAddress);
            cell.StyleIndex = STYLE_GREEN_BKG_BOLD;
            excel.SetCellValue(cell, cg.name + " (id=" + cg.id + ")");

            cellAddress = "A2";
            cell = excel.InsertCellInWorksheet(ws, cellAddress);
            cell.StyleIndex = STYLE_BOLD;
            excel.SetCellValue(cell, RequestResource.Company + ":");

            cellAddress = "B2";
            cell = excel.InsertCellInWorksheet(ws, cellAddress);
            excel.SetCellValue(cell, cg.Company.country_code);

            cellAddress = "A3";
            cell = excel.InsertCellInWorksheet(ws, cellAddress);
            cell.StyleIndex = STYLE_BOLD;
            excel.SetCellValue(cell, RequestResource.Centres + ":");

            string strCentres = "";
            foreach (var centre in cg.Centre) {
                if (centre.active == false) {
                    continue;
                }

                if (strCentres.Length > 0) {
                    strCentres += ", ";
                }

                strCentres += centre.name + " (id=" + centre.id + ")";
            }


            cellAddress = "B3";
            cell = excel.InsertCellInWorksheet(ws, cellAddress);
            excel.SetCellValue(cell, strCentres);

            //Currency
            cellAddress = "A4";
            cell = excel.InsertCellInWorksheet(ws, cellAddress);
            cell.StyleIndex = STYLE_BOLD;
            excel.SetCellValue(cell, RequestResource.Currency + ":");

            cellAddress = "B4";
            cell = excel.InsertCellInWorksheet(ws, cellAddress);
            excel.SetCellValue(cell, cg.Currency.currency_code);

            iRow = 5;

            //Foreign Currencies
            string strForeignCurrencies = "";
            foreach (var curr in cg.Foreign_Currency) {
                if (curr.active == false || curr.currency_code == cg.Currency.currency_code) {
                    continue;
                }

                if (strForeignCurrencies.Length > 0) {
                    strForeignCurrencies += ", ";
                }

                strForeignCurrencies += curr.currency_code;
            }

            if (!String.IsNullOrEmpty(strForeignCurrencies)) {
                //Foreign Currency
                cellAddress = "A" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = STYLE_BOLD;
                excel.SetCellValue(cell, RequestResource.ForeignCurrency + ":");

                cellAddress = "B" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                excel.SetCellValue(cell, strForeignCurrencies);
                iRow++;
            } 
           
            //Deputy Orderer
            string strDeputyOrderers = "";
            var deputyOrderers = new CentreGroupRepository().GetDeputyOrderers(cg.id); 
            foreach (var depOrd in deputyOrderers) {
                if (depOrd.Participants.active == false) {
                    continue;
                }

                if (strDeputyOrderers.Length > 0) {
                    strDeputyOrderers += ", ";
                }

                strDeputyOrderers += depOrd.Participants.surname + " " + depOrd.Participants.first_name;
            }

            if (!String.IsNullOrEmpty(strDeputyOrderers)) {
                //Deputy Orderers
                cellAddress = "A" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = STYLE_BOLD;
                excel.SetCellValue(cell, RequestResource.DeputyOrderer + ":");

                cellAddress = "B" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                excel.SetCellValue(cell, strDeputyOrderers);
                iRow++;
            }

            //Orderer Supplier
            bool isOrdererSupplier = false;
            foreach (var ordSupp in cg.Orderer_Supplier) {
                if (ordSupp.Participants.active == false || ordSupp.Supplier.active == false) {
                    continue;
                }
                isOrdererSupplier = true;
                break;
            }
                        
            if (isOrdererSupplier) {
                cellAddress = "A" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = STYLE_BOLD;
                excel.SetCellValue(cell, RequestResource.OrdererSupplier + ":");

                
                foreach (var ordSupp in cg.Orderer_Supplier) {
                    if (ordSupp.Participants.active == false || ordSupp.Supplier.active == false) {
                        continue;
                    }

                    cellAddress = "B" + iRow;
                    cell = excel.InsertCellInWorksheet(ws, cellAddress);
                    excel.SetCellValue(cell, ordSupp.Supplier.supp_name + " (" + ordSupp.supplier_id + ")");

                    cellAddress = "C" + iRow;
                    cell = excel.InsertCellInWorksheet(ws, cellAddress);
                    excel.SetCellValue(cell, ordSupp.Participants.surname + " " + ordSupp.Participants.first_name);
                    iRow++;
                }
            }

            //Allow Free Supplier
            cellAddress = "A" + iRow;
            cell = excel.InsertCellInWorksheet(ws, cellAddress);
            cell.StyleIndex = STYLE_BOLD;
            excel.SetCellValue(cell, RequestResource.AllowFreeSupplier + ":");

            string strAllowFreeSupplier = (cg.allow_free_supplier == true) ? RequestResource.Yes : RequestResource.No;
            cellAddress = "B" + iRow;
            cell = excel.InsertCellInWorksheet(ws, cellAddress);
            excel.SetCellValue(cell, strAllowFreeSupplier);
            iRow++;

            //Pg Admins
            cellAddress = "A" + iRow;
            cell = excel.InsertCellInWorksheet(ws, cellAddress);
            cell.StyleIndex = STYLE_BOLD;
            excel.SetCellValue(cell, RequestResource.CentreGroupAdmin + ":");

            Hashtable htAdmins = new Hashtable();
            string strCgAdmins = "";
            var cgAdmins = new CentreGroupRepository().GetCgAdmins(cg.id);
            foreach (var cgAdmin in cgAdmins) {
                if (htAdmins.ContainsKey(cgAdmin.participant_id)) {
                    continue;
                }

                if (cgAdmin.Participants.active == false) {
                    continue;
                }

                if (strCgAdmins.Length > 0) {
                    strCgAdmins += ", ";
                }

                strCgAdmins += cgAdmin.Participants.surname + " " + cgAdmin.Participants.first_name;

                htAdmins.Add(cgAdmin.participant_id, null);
            }

            cellAddress = "B" + iRow;
            cell = excel.InsertCellInWorksheet(ws, cellAddress);
            excel.SetCellValue(cell, strCgAdmins);
            iRow++;
        }

        private void GetCentreGroupAppMatrix(
            Excel excel,
            //Worksheet ws,
            WorksheetPart wsPart,
            Centre_Group cg,
            MergeCells mergeCells,
            int iRow) {

            var ws = wsPart.Worksheet;

            iRow++;
            //se pg loc name
            foreach (var pg in cg.Purchase_Group) {
                string pgName = pg.group_name;
                foreach (var locName in pg.Purchase_Group_Local) {
                    if (CurrentCultureCode.ToLower() == locName.culture.ToLower()) {
                        pgName = locName.local_text;
                        break;
                    }
                }
                pg.group_name = pgName;
            }

            var pgSort = cg.Purchase_Group.OrderBy(x => x.group_name).ToList();
            foreach (var pg in pgSort) {
                if (pg.active == false) {
                    continue;
                }

                //Pg Name
                //MergeCells mergeCells = wsPart.me
                mergeCells.Append(new MergeCell() { Reference = new StringValue("A" + iRow + ":C" + iRow) });
                //ws.InsertAfter(mergeCells, wsPart.Worksheet.Elements<SheetData>().First());

                string pgName = pg.group_name;
                foreach (var locName in pg.Purchase_Group_Local) {
                    if (CurrentCultureCode.ToLower() == locName.culture.ToLower()) {
                        pgName = locName.local_text;
                        break;
                    }
                }

                string cellAddress = "A" + iRow;
                var cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = STYLE_GREEN_BKG_BOLD;
                string strPgHeader = pgName + " (id=" + pg.id + ") - ";
                strPgHeader += RequestResource.Approval + ": " + ((pg.is_approval_needed) ? RequestResource.Yes : RequestResource.No) + ", ";
                strPgHeader += RequestResource.Order + ": " + ((pg.is_order_needed) ? RequestResource.Yes : RequestResource.No);
                //if (pg.purchase_type == (int)PurchaseGroupType.ApprovalOnly) {
                if (!pg.is_approval_needed || !pg.is_order_needed) {
                    strPgHeader += " - " + RequestResource.ApprovalOnly;
                    cell.StyleIndex = STYLE_GREEN_APPROVAL_ONLY_BKG_BOLD;
                }
                excel.SetCellValue(cell, strPgHeader);
                iRow++;

                if (pg.is_approval_needed) {
                    //limits header
                    cellAddress = "A" + iRow;
                    cell = excel.InsertCellInWorksheet(ws, cellAddress);
                    cell.StyleIndex = STYLE_BOLD;
                    excel.SetCellValue(cell, RequestResource.LowerLimit + " (" + cg.Currency.currency_code + ")");

                    cellAddress = "B" + iRow;
                    cell = excel.InsertCellInWorksheet(ws, cellAddress);
                    cell.StyleIndex = STYLE_BOLD;
                    excel.SetCellValue(cell, RequestResource.UpperLimit + " (" + cg.Currency.currency_code + ")");

                    cellAddress = "C" + iRow;
                    cell = excel.InsertCellInWorksheet(ws, cellAddress);
                    cell.StyleIndex = STYLE_BOLD;
                    excel.SetCellValue(cell, RequestResource.ApproveMan);

                    iRow++;


                    #region Limits
                    //limits
                    var pgLimitsSort = pg.Purchase_Group_Limit.OrderBy(x => x.approve_level_id).ToList();
                    foreach (var pgLimit in pgLimitsSort) {
                        if (pgLimit.active == false) {
                            continue;
                        }

                        string strLimitBottom = (pgLimit.limit_bottom == null) ? RequestResource.NoLimit : pgLimit.limit_bottom.ToString();
                        string strLimitTop = (pgLimit.limit_top == null) ? RequestResource.NoLimit : pgLimit.limit_top.ToString();

                        cellAddress = "A" + iRow;
                        cell = excel.InsertCellInWorksheet(ws, cellAddress);
                        excel.SetCellValue(cell, strLimitBottom);

                        cellAddress = "B" + iRow;
                        cell = excel.InsertCellInWorksheet(ws, cellAddress);
                        excel.SetCellValue(cell, strLimitTop);

                        string strAppMen = "";
                        foreach (var manRole in pgLimit.Manager_Role) {
                            if (manRole.active == false || manRole.Participants == null) {
                                continue;
                            }
                            if (strAppMen.Length > 0) {
                                strAppMen += ",";
                            }
                            strAppMen += manRole.Participants.surname + " " + manRole.Participants.first_name;

                            Participant_Substitute partSubst = new UserRepository().GetParticipantActiveSubstitution(manRole.participant_id);
                            if (partSubst != null) {
                                strAppMen += " (" + RequestResource.ManagerSubstitution + " " + partSubst.SubstituteUser.surname + " " + partSubst.SubstituteUser.first_name + " " + RequestResource.To + " " + partSubst.substitute_end_date.ToString("dd.MM.yyyy") + ")";
                            }
                        }

                        cellAddress = "C" + iRow;
                        cell = excel.InsertCellInWorksheet(ws, cellAddress);
                        excel.SetCellValue(cell, strAppMen);

                        iRow++;
                    }
                    #endregion
                }

                #region Requestors
                //requestors
                var sortCentres = cg.Centre.OrderBy(x => x.name);
                foreach (var centre in sortCentres) {
                    if (centre.active == false) {
                        continue;
                    }
                    cellAddress = "A" + iRow;
                    cell = excel.InsertCellInWorksheet(ws, cellAddress);
                    cell.StyleIndex = STYLE_BOLD;
                    excel.SetCellValue(cell, RequestResource.Requestors + " " + centre.name);

                    var purchReqs = (from reqDb in pg.PurchaseGroup_Requestor
                                     where reqDb.centre_id == centre.id
                                     orderby reqDb.Participants.surname, reqDb.Participants.first_name
                                     select reqDb).ToList();

                    string strReqs = "";
                    foreach (var purchReq in purchReqs) {
                        if (purchReq.Participants.active == false) {
                            continue;
                        }

                        if (strReqs.Length > 0) {
                            strReqs += ", ";
                        }

                        strReqs += purchReq.Participants.surname + " " + purchReq.Participants.first_name;
                    }
                    mergeCells.Append(new MergeCell() { Reference = new StringValue("B" + iRow + ":C" + iRow) });
                    cellAddress = "B" + iRow;
                    cell = excel.InsertCellInWorksheet(ws, cellAddress);
                    excel.SetCellValue(cell, strReqs);

                    iRow++;
                }
                #endregion

                #region Orderers
                //orderers
                //if (pg.purchase_type == (int)PgRepository.PurchaseGroupType.Order) {
                if (pg.is_order_needed) {
                    cellAddress = "A" + iRow;
                    cell = excel.InsertCellInWorksheet(ws, cellAddress);
                    cell.StyleIndex = STYLE_BOLD;
                    excel.SetCellValue(cell, RequestResource.Orderers);

                    var purchOrds = (from purchOrdsDb in pg.PurchaseGroup_Orderer
                                     where purchOrdsDb.active == true
                                     orderby purchOrdsDb.surname, purchOrdsDb.first_name
                                     select purchOrdsDb).ToList();

                    string strOrds = "";
                    if (pg.self_ordered == true) {
                        strOrds = RequestResource.SelfOrderedCategory;
                    } else {
                        foreach (var purchOrd in purchOrds) {

                            if (strOrds.Length > 0) {
                                strOrds += ", ";
                            }

                            strOrds += purchOrd.surname + " " + purchOrd.first_name;
                            Participant_Substitute partSubst = new UserRepository().GetParticipantActiveSubstitution(purchOrd.id);
                            if (partSubst != null) {
                                strOrds += " (" + RequestResource.ManagerSubstitution + " " + partSubst.SubstituteUser.surname + " " + partSubst.SubstituteUser.first_name + " " + RequestResource.To + " " + partSubst.substitute_end_date.ToString("dd.MM.yyyy") + ")";
                            }
                        }
                    }

                    mergeCells.Append(new MergeCell() { Reference = new StringValue("B" + iRow + ":C" + iRow) });
                    cellAddress = "B" + iRow;
                    cell = excel.InsertCellInWorksheet(ws, cellAddress);
                    excel.SetCellValue(cell, strOrds);
                    iRow++;
                }
                #endregion

                iRow++;
            }


        }

        private string GetColumnName(string colName,  Hashtable htColName) {
            string retColName = colName;
            int iIndex = 1;
            while (htColName.ContainsKey(retColName)) {
                retColName = colName + iIndex;
                iIndex++;
            }

            htColName.Add(retColName, null);

            return retColName;
        }

        private MemoryStream GetParticipantExcelStream(List<ParticipantsExtended> participants) {
            DataTable participantTable = new DataTable("RegetUsers");

            Hashtable htColName = new Hashtable();

            string strSurname = GetColumnName(RequestResource.Surname, htColName);
            string strFirstname = GetColumnName(RequestResource.Firstname, htColName);
            string strUsername = GetColumnName(RequestResource.Login, htColName);
            string strMail = GetColumnName(RequestResource.Mail, htColName);
            string strPhone = GetColumnName(RequestResource.Phone, htColName);
            string strCompany = GetColumnName(RequestResource.Company, htColName);
            string strActive = GetColumnName(RequestResource.Active, htColName);

            DataColumn col = new DataColumn(strSurname, typeof(string));
            participantTable.Columns.Add(col);

            col = new DataColumn(strFirstname, typeof(string));
            participantTable.Columns.Add(col);

            col = new DataColumn(strUsername, typeof(string));
            participantTable.Columns.Add(col);

            col = new DataColumn(strMail, typeof(string));
            participantTable.Columns.Add(col);

            col = new DataColumn(strPhone, typeof(string));
            participantTable.Columns.Add(col);

            col = new DataColumn(strCompany, typeof(string));
            participantTable.Columns.Add(col);

            col = new DataColumn(strActive, typeof(string));
            participantTable.Columns.Add(col);

            foreach (var participant in participants) {
                DataRow newRow = participantTable.NewRow();
                newRow[strSurname] = participant.surname;
                newRow[strFirstname] = participant.first_name;
                newRow[strUsername] = participant.user_name;
                newRow[strMail] = participant.email;
                newRow[strPhone] = participant.phone;
                newRow[strCompany] = participant.office_name;
                newRow[strActive] = (participant.active == true) ? RequestResource.Yes : RequestResource.No;

                participantTable.Rows.Add(newRow);
            }

            List<double> columnWidths = new List<double> { 20, 20, 15, 40, 20, 35, 15 };
            MemoryStream outputStream = new Excel().GenerateExcelWorkbook(participantTable, columnWidths);

            return outputStream;
        }

        private MemoryStream GetCentresExcelStream(List<Centre> centres) {
            DataTable centresTable = new DataTable("RegetCentres");

            Hashtable htColName = new Hashtable();

            string strName = GetColumnName(RequestResource.Name, htColName);
            string strCompany = GetColumnName(RequestResource.Company, htColName);
            string strExportPrice = GetColumnName(RequestResource.ExportPriceToOrder, htColName);
            string strMultiOrderer = GetColumnName(RequestResource.MultiOrdererInfo, htColName);
            string strCanOtherOredereOrder = GetColumnName(RequestResource.OtherOrderersAllowedToTakeOver, htColName);
            string strRequestorCanApprove = GetColumnName(RequestResource.RequestorCanApprove, htColName);
            string strManagerName = GetColumnName(RequestResource.CentreManager, htColName);
            string strAddress = GetColumnName(RequestResource.ShipToAddress, htColName);
            string strActive = GetColumnName(RequestResource.Active, htColName);

            DataColumn col = new DataColumn(strName, typeof(string));
            centresTable.Columns.Add(col);

            col = new DataColumn(strCompany, typeof(string));
            centresTable.Columns.Add(col);

            col = new DataColumn(strExportPrice, typeof(string));
            centresTable.Columns.Add(col);

            col = new DataColumn(strMultiOrderer, typeof(string));
            centresTable.Columns.Add(col);

            col = new DataColumn(strCanOtherOredereOrder, typeof(string));
            centresTable.Columns.Add(col);

            col = new DataColumn(strRequestorCanApprove, typeof(string));
            centresTable.Columns.Add(col);

            col = new DataColumn(strManagerName, typeof(string));
            centresTable.Columns.Add(col);

            col = new DataColumn(strAddress, typeof(string));
            centresTable.Columns.Add(col);

            col = new DataColumn(strActive, typeof(string));
            centresTable.Columns.Add(col);
                       
            foreach (var fCentre in centres) {
                var centre = new CentreRepository().GetCentreById(fCentre.id);

                DataRow newRow = centresTable.NewRow();
                newRow[strName] = centre.name;
                newRow[strCompany] = centre.Company.country_code;
                newRow[strExportPrice] = CentreController.GetExportToPriceText(centre.export_price_to_order);
                newRow[strMultiOrderer] = (centre.multi_orderer == true) ? RequestResource.Yes : RequestResource.No;
                newRow[strCanOtherOredereOrder] = (centre.other_orderer_can_takeover == true) ? RequestResource.Yes : RequestResource.No;
                newRow[strRequestorCanApprove] = (centre.is_approved_by_requestor == true) ? RequestResource.Yes : RequestResource.No;
                if (centre.Manager != null) {
                    newRow[strManagerName] = centre.Manager.surname + " " + centre.Manager.first_name;
                }
                if (centre.Address != null) {
                    newRow[strAddress] = centre.Address.company_name + ", " + centre.Address.street + ", " + 
                        centre.Address.city + ", " + centre.Address.zip;
                } 
                newRow[strActive] = (centre.active == true) ? RequestResource.Yes : RequestResource.No;

                centresTable.Rows.Add(newRow);
            }

            List<double> columnWidths = new List<double> { 15, 20, 10, 10, 10, 10, 20, 40, 10 };
            MemoryStream outputStream = new Excel().GenerateExcelWorkbook(centresTable, columnWidths);

            return outputStream;
        }

        private MemoryStream GetParentPgExcelStream(List<object> parentPpgs) {
            DataTable ppgTable = new DataTable("ParentPurchaseGroups");

            if (parentPpgs != null && parentPpgs.Count > 0) {
                Hashtable htColName = new Hashtable();

                string strName = GetColumnName(RequestResource.Name, htColName);
                DataColumn col = new DataColumn(strName, typeof(string));
                ppgTable.Columns.Add(col);

                Hashtable companyColumsNames = GetCompanyColumnsNames(parentPpgs[0]);
                if (companyColumsNames != null && companyColumsNames.Count > 0) {
                    IDictionaryEnumerator iEnum = companyColumsNames.GetEnumerator();
                    SortedList<string, int> sortedCompanies = new SortedList<string, int>();
                    while(iEnum.MoveNext()) {
                        sortedCompanies.Add((string)iEnum.Value, (int)iEnum.Key);
                        //col = new DataColumn((string)iEnum.Value, typeof(bool));
                        //ppgTable.Columns.Add(col);
                    }

                    foreach (KeyValuePair<string, int> kvp in sortedCompanies) {
                        col = new DataColumn(kvp.Key, typeof(bool));
                        ppgTable.Columns.Add(col);
                    }
                }

                foreach (var parentPpg in parentPpgs) {
                    DataRow newRow = ppgTable.NewRow();

                    FieldInfo[] fields = parentPpg.GetType().GetFields();
                    foreach (var field in fields) {
                        if (field.Name.Contains(ParentPgRepository.PARENT_PG_NAME)) {
                            string pgName = (string)field.GetValue(parentPpg);
                            newRow[strName] = pgName;
                        }

                        if (field.Name.Contains(ParentPgRepository.COMPANY_SELECTED_PREFIX)) {
                            bool isSelected = (bool)field.GetValue(parentPpg);
                            string strCompId = field.Name.Replace(ParentPgRepository.COMPANY_SELECTED_PREFIX, "");
                            int compId = Convert.ToInt16(strCompId);
                            string compName = companyColumsNames[compId].ToString();
                            newRow[compName] = isSelected;
                        }
                    }

                    ppgTable.Rows.Add(newRow);
                }
            }

            List<double> columnWidths = new List<double> { 25 };
            MemoryStream outputStream = new Excel().GenerateExcelWorkbook(ppgTable, columnWidths);

            return outputStream;
        }

        private Hashtable GetCompanyColumnsNames(object oPpg) {
            Hashtable companies = new Hashtable();
            
            FieldInfo[] fields = oPpg.GetType().GetFields();
            foreach (var field in fields) {
                if (field.Name.Contains(ParentPgRepository.COMPANY_NAME_PREFIX)) {
                    string strCompany = (string)field.GetValue(oPpg);
                    string[] strItems = strCompany.Split('|');
                    string strCompId = strItems[0].Replace(ParentPgRepository.COMPANY_SELECTED_PREFIX, "");
                    int compId = Convert.ToInt16(strCompId);
                    companies.Add(compId, strItems[1]);
                }
            }

            return companies;
        }

        private MemoryStream GetUsedPgExcelStream(List<UsedPg> parentPgCompanyPgs) {
            DataTable pgTable = new DataTable("PurchaseGroups");
                        
            Hashtable htColName = new Hashtable();

            string strPgName = GetColumnName(RequestResource.PurchaseGroup, htColName);
            string strPpgName = GetColumnName(RequestResource.ParentPg, htColName);
            string strCgName = GetColumnName(RequestResource.Area, htColName);
            string strCompany = GetColumnName(RequestResource.Company, htColName);
            string strActive = GetColumnName(RequestResource.Active, htColName);

            DataColumn col = new DataColumn(strPgName, typeof(string));
            pgTable.Columns.Add(col);

            col = new DataColumn(strPpgName, typeof(string));
            pgTable.Columns.Add(col);

            col = new DataColumn(strCgName, typeof(string));
            pgTable.Columns.Add(col);

            col = new DataColumn(strCompany, typeof(string));
            pgTable.Columns.Add(col);
                        
            col = new DataColumn(strActive, typeof(string));
            pgTable.Columns.Add(col);

            foreach (var parentPgCompanyPg in parentPgCompanyPgs) {
                DataRow newRow = pgTable.NewRow();
                
                newRow[strPgName] = parentPgCompanyPg.pg_loc_name;
                newRow[strPpgName] = parentPgCompanyPg.parent_pg_loc_name;
                newRow[strCgName] = parentPgCompanyPg.centre_group_name;
                newRow[strCompany] = parentPgCompanyPg.company_name;
                newRow[strActive] = (parentPgCompanyPg.active == true) ? RequestResource.Yes : RequestResource.No;

                pgTable.Rows.Add(newRow);
            }

            List<double> columnWidths = new List<double> { 25, 25, 20, 20, 10 };
            MemoryStream outputStream = new Excel().GenerateExcelWorkbook(pgTable, columnWidths);

            return outputStream;
        }

        private MemoryStream GetSuppliersExcelStream(List<Supplier> suppliers) {
            DataTable supplierTable = new DataTable("RegetSupplier");

            Hashtable htColName = new Hashtable();

            string strName = GetColumnName(RequestResource.Name, htColName);
            string strIco = GetColumnName(RequestResource.ICO, htColName);
            string strStreet = GetColumnName(RequestResource.Street, htColName);
            string strCity = GetColumnName(RequestResource.City, htColName);
            string strZip = GetColumnName(RequestResource.ZipCode, htColName);
            string strCountry = GetColumnName(RequestResource.Country, htColName);
            string strContactPerson = GetColumnName(RequestResource.ContactPerson, htColName);
            string strPhoneNr = GetColumnName(RequestResource.Phone, htColName);
            string strMail = GetColumnName(RequestResource.Mail, htColName);
            string strActive = GetColumnName(RequestResource.Active, htColName);

            DataColumn col = new DataColumn(strName, typeof(string));
            supplierTable.Columns.Add(col);

            col = new DataColumn(strIco, typeof(string));
            supplierTable.Columns.Add(col);

            col = new DataColumn(strStreet, typeof(string));
            supplierTable.Columns.Add(col);

            col = new DataColumn(strCity, typeof(string));
            supplierTable.Columns.Add(col);

            col = new DataColumn(strZip, typeof(string));
            supplierTable.Columns.Add(col);

            col = new DataColumn(strCountry, typeof(string));
            supplierTable.Columns.Add(col);

            col = new DataColumn(strContactPerson, typeof(string));
            supplierTable.Columns.Add(col);

            col = new DataColumn(strPhoneNr, typeof(string));
            supplierTable.Columns.Add(col);

            col = new DataColumn(strMail, typeof(string));
            supplierTable.Columns.Add(col);

            col = new DataColumn(strActive, typeof(string));
            supplierTable.Columns.Add(col);

            foreach (var supplier in suppliers) {
                DataRow newRow = supplierTable.NewRow();
                newRow[strName] = supplier.supp_name;
                newRow[strIco] = supplier.supplier_id;
                string street = supplier.street_part1;
                if (String.IsNullOrEmpty(street)) {
                    street += " ";
                }
                street += supplier.street_part2;
                newRow[strStreet] = street;
                newRow[strCity] = supplier.city;
                newRow[strZip] = supplier.zip;
                newRow[strCountry] = supplier.country;
                newRow[strContactPerson] = supplier.contact_person;
                newRow[strPhoneNr] = supplier.phone;
                newRow[strMail] = supplier.email;
                newRow[strActive] = (supplier.active == true) ? RequestResource.Yes : RequestResource.No;

                supplierTable.Rows.Add(newRow);
                
            }

            List<double> columnWidths = new List<double> { 30, 10, 40, 25, 10, 20, 20, 15, 15, 10 };
            MemoryStream outputStream = new Excel().GenerateExcelWorkbook(supplierTable, columnWidths);

            return outputStream;
        }

        private MemoryStream GetSubstitutionExcelStream(List<UserSubstitutionExtended> substitutions) {
            DataTable substTable = new DataTable("RegetSubstitution");

            Hashtable htColName = new Hashtable();

            string strSubstituted = GetColumnName(RequestResource.Substituted, htColName);
            string strSubstitutee = GetColumnName(RequestResource.Substitute, htColName);
            string strFrom = GetColumnName(RequestResource.From, htColName);
            string strTo = GetColumnName(RequestResource.To, htColName);
            string strActive = GetColumnName(RequestResource.Active, htColName);

            DataColumn col = new DataColumn(strSubstituted, typeof(string));
            substTable.Columns.Add(col);

            col = new DataColumn(strSubstitutee, typeof(string));
            substTable.Columns.Add(col);

            col = new DataColumn(strFrom, typeof(DateTime));
            substTable.Columns.Add(col);

            col = new DataColumn(strTo, typeof(string));
            substTable.Columns.Add(col);
                        
            col = new DataColumn(strActive, typeof(string));
            substTable.Columns.Add(col);

            foreach (var substitution in substitutions) {
                DataRow newRow = substTable.NewRow();
                newRow[strSubstituted] = substitution.substituted_name_surname_first;
                newRow[strSubstitutee] = substitution.substitutee_name_surname_first;
                newRow[strFrom] = substitution.substitute_start_date;
                newRow[strTo] = substitution.substitute_end_date;

                bool isActive = (substitution.active == true) && substitution.substitute_end_date >= DateTime.Now;
                newRow[strActive] = (isActive) ? RequestResource.Yes : RequestResource.No;

                substTable.Rows.Add(newRow);

            }

            List<double> columnWidths = new List<double> { 30, 30, 20, 20, 10 };
            MemoryStream outputStream = new Excel().GenerateExcelWorkbook(substTable, columnWidths);

            return outputStream;
        }

        private MemoryStream GetAddressExcelStream(List<AddressAdminExtended> addresses) {
            DataTable addressTable = new DataTable("RegetAddreses");

            Hashtable htColName = new Hashtable();

            string strCompany = GetColumnName(RequestResource.Company, htColName);
            string strCompanyName = GetColumnName(RequestResource.Company + " " + RequestResource.Name, htColName);
            string strStreet = GetColumnName(RequestResource.Street, htColName);
            string strCity = GetColumnName(RequestResource.City, htColName);
            string strZip = GetColumnName(RequestResource.ZipCode, htColName);

            DataColumn col = new DataColumn(strCompany, typeof(string));
            addressTable.Columns.Add(col);

            col = new DataColumn(strCompanyName, typeof(string));
            addressTable.Columns.Add(col);

            col = new DataColumn(strStreet, typeof(string));
            addressTable.Columns.Add(col);

            col = new DataColumn(strCity, typeof(string));
            addressTable.Columns.Add(col);

            col = new DataColumn(strZip, typeof(string));
            addressTable.Columns.Add(col);


            foreach (var address in addresses) {
                DataRow newRow = addressTable.NewRow();
                newRow[strCompany] = address.company_name_text;
                newRow[strCompanyName] = address.company_name;
                newRow[strStreet] = address.street;
                newRow[strCity] = address.city;
                newRow[strZip] = address.zip;

                addressTable.Rows.Add(newRow);

            }

            List<double> columnWidths = new List<double> { 30, 30, 20, 20, 10 };
            MemoryStream outputStream = new Excel().GenerateExcelWorkbook(addressTable, columnWidths);

            return outputStream;
        }

        private Columns GetAppMatrixColumns() {
            Columns columns = new Columns();
            uint iIndex = Convert.ToUInt32(1);
            columns.Append(Excel.CreateColumnData(iIndex, iIndex, 25));

            iIndex = Convert.ToUInt32(2);
            columns.Append(Excel.CreateColumnData(iIndex, iIndex, 25));

            iIndex = Convert.ToUInt32(3);
            columns.Append(Excel.CreateColumnData(iIndex, iIndex, 25));

            return columns;
        }

        private void ExcelClear() {
            Session[RegetSession.SES_XLS_DOC] = null;
            Session[RegetSession.SES_WB_PART] = null;
            Session[RegetSession.SES_EXCEL] = null;
            _m_Excel = null;
            _m_wbPart = null;
            _m_xlsDoc = null;
        }

        private string GetTimeStampFileSuffix() {
            return DateTime.Now.ToString("yyyyMMddHHmm");
        }

        private string GetReportName(string rawName) {
            string xlsName = rawName;
            if (xlsName.Length > 40) {
                xlsName = xlsName.Substring(0, 40);
                xlsName = RemoveDiacritics(xlsName).Replace(" ", "_");
            }

            return xlsName;
        }

        private DataTable TransponeData(DataTable chartData) {
            DataTable tranTable = new DataTable("Data");

            DataColumn col = new DataColumn(chartData.Columns[0].ColumnName, typeof(string));
            tranTable.Columns.Add(col);

            for (int i = 0; i < chartData.Rows.Count; i++) {
                //var dsType = (i == 0) ? typeof(string) : typeof(decimal);

                col = new DataColumn(chartData.Rows[i][0].ToString(), typeof(decimal));
                tranTable.Columns.Add(col);
            }

            for (int i = 1; i < chartData.Columns.Count; i++) {
                DataRow newRow = tranTable.NewRow();
                newRow[0] = chartData.Columns[i].ColumnName;
                for (int j = 0; j < chartData.Rows.Count; j++) {
                    newRow[j + 1] = chartData.Rows[j][i];
                }
                tranTable.Rows.Add(newRow);
            }

            return tranTable;
        }

        private Excel.ChartType GetExcelChartType(int iChartType) {
            if (iChartType == StatisticsController.CHART_TYPE_LINE) {
                return Excel.ChartType.xlLine;
            }

            if (iChartType == StatisticsController.CHART_TYPE_PIE) {
                return Excel.ChartType.xlPie;
            }

            if (iChartType == StatisticsController.CHART_TYPE_DOUGHNUT) {
                return Excel.ChartType.xlPie;
            }

            return Excel.ChartType.xlColumnClustered;
        }

        private int GetChartDataPeriodValue(List<StatisticsController.Period> periods, DateTime dateFrom, DateTime dateTo) {
            if (periods == null) {
                return 0;
            }
            foreach (var period in periods) {
                if (period.PeriodStart == dateFrom && period.PeriodEnd == dateTo) {
                    return ConvertData.ToInt32(period.PeriodValue);
                }
            }

            return 0;
        }

        private string GetChartFirstColumn(int xItemType) {
            if (xItemType == StatisticsController.FILTER_APPROVE_MAN) {
                return RequestResource.ApproveMan;
            } else if (xItemType == StatisticsController.FILTER_AREA) {
                return RequestResource.Area;
            } else if (xItemType == StatisticsController.FILTER_CENTER) {
                return RequestResource.Centre;
            } else if (xItemType == StatisticsController.FILTER_COMMODITY) {
                return RequestResource.Commodity;
            } else if (xItemType == StatisticsController.FILTER_COMPANY) {
                return RequestResource.Company;
            } else if (xItemType == StatisticsController.FILTER_ORDERER) {
                return RequestResource.Order;
            } else if (xItemType == StatisticsController.FILTER_REQUESTOR) {
                return RequestResource.Requestor;
            } else if (xItemType == StatisticsController.FILTER_SUPPLIER) {
                return RequestResource.Supplier;
            }

            return "";
        }

        private void RefreshAppMatrixSessions() {
            Session[RegetSession.SES_XLS_DOC] = m_xlsDoc;
            Session[RegetSession.SES_WB_PART] = m_wbPart;
            Session[RegetSession.SES_EXCEL] = m_Excel;
        }

        
        #endregion
    }
}