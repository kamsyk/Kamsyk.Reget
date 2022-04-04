using Kamsyk.Reget.Model.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kamsyk.Reget.TestsIntegration.BaseTest {
    [TestClass]
    public class BaseDbGridTestIntegration : BaseTestIntegration {
        #region Constants
        private const int WAIT_IN_SECONDS = 10;
        private const string PAGE_INDEX = "_PageIndex";
        private const string PAGES_COUNT = "_PagesCount";
        private const string PAGE_SIZE = "_PageSize";
        protected const string NEW_ITEM_TEXT = "NewItem";
        #endregion

        #region Enums
        public enum NewRowCheckboxValueType {
            All,
            None,
            Random
        }
        #endregion

        #region Delegates
        public delegate void DlgClear();
        #endregion

        #region Abstract Methods
        //public abstract void AddNewRecord();
        #endregion

        #region Methods
        protected bool TestDataGrid(
            IWebDriver driver,
            string grdId,
            bool isRowUnique,
            DlgClear dlgClear,
            NewRowCheckboxValueType ckbValueType,
            int maxAllowedLoadCount) {

            try { 
                dlgClear();

                if (!ClearFilter(driver)) {
                    new TestFailRepository().SaveTestFail(grdId, "ClearFilter");
                    return false;
                }

                SetPageSize(driver, grdId, 10);

                if (!NextPage(driver, grdId)) {
                    new TestFailRepository().SaveTestFail(grdId, "Next Page");
                    return false;
                }

                if (!PreviousPage(driver, grdId)) {
                    new TestFailRepository().SaveTestFail(grdId, "Previous Page");
                    return false;
                }

                if (!LastPage(driver, grdId)) {
                    new TestFailRepository().SaveTestFail(grdId, "Last Page");
                    return false;
                }

                if (!FirstPage(driver, grdId)) {
                    new TestFailRepository().SaveTestFail(grdId, "First Page");
                    return false;
                }

                if (!GoToPage(driver, grdId)) {
                    new TestFailRepository().SaveTestFail(grdId, "GoTo Page");
                    return false;
                }

                if (!Refresh(driver)) {
                    new TestFailRepository().SaveTestFail(grdId, "Refresh");
                    return false;
                }

                if (!SetPageSize(driver, grdId)) {
                    new TestFailRepository().SaveTestFail(grdId, "Set Page Size");
                    return false;
                }

                SetPageSize(driver, grdId, 10);

                //click twice on AddNewRow
                if (!ClickAddNewTwice(driver)) {
                    new TestFailRepository().SaveTestFail(grdId, "Add New Twice");
                    return false;
                }

                if (!AddNewRecord(driver, ckbValueType)) {
                    new TestFailRepository().SaveTestFail(grdId,"Add New Record");
                    return false;
                }

                if (!DeleteLastRow(driver)) {
                    new TestFailRepository().SaveTestFail(grdId,"Delete Last Row");
                    return false;
                }

                if (!SetFilter(driver)) {
                    new TestFailRepository().SaveTestFail(grdId, "Set Filter");
                    return false;
                }

                if (!SaveFilter(driver, grdId)) {
                    new TestFailRepository().SaveTestFail(grdId, "Save Filter");
                    return false;
                }

                if (!ResetFilter(driver, grdId)) {
                    new TestFailRepository().SaveTestFail(grdId, "Reset Filter");
                    return false;
                }

                if (!LoadIsNotDuplicated(driver, maxAllowedLoadCount)) {
                    new TestFailRepository().SaveTestFail(grdId, "Load Db Data is Doubled");
                    return false;
                }

                if (!LoadFilterIsNotDuplicated(driver, grdId, maxAllowedLoadCount)) {
                    new TestFailRepository().SaveTestFail(grdId, "Filter Load Db Data is Doubled");
                    return false;
                }

                
                if (isRowUnique) {
                    if (!DuplicityRow(driver, grdId)) {
                        new TestFailRepository().SaveTestFail(grdId, "Duplicity Row");
                        return false;
                    }
                    dlgClear();

                    Refresh(driver);
                }

                if (!ExportToExcel()) {
                    new TestFailRepository().SaveTestFail(grdId, "Export to Excel");
                    return false;
                }

                return true;
            } catch (Exception ex) {
                new TestFailRepository().SaveTestFail(grdId, ex.ToString());
                return false;
            }
        }

        protected bool TestDataGrid(
            IWebDriver driver,
            string grdId,
            bool isRowUnique,
            DlgClear dlgClear) {

            return TestDataGrid(
                driver,
                grdId,
                isRowUnique,
                dlgClear,
                NewRowCheckboxValueType.None,
                1);
        }

        protected bool TestDataGrid(
            IWebDriver driver,
            string grdId,
            bool isRowUnique,
            DlgClear dlgClear,
            NewRowCheckboxValueType nrckbv) {

            return TestDataGrid(
                driver,
                grdId,
                isRowUnique,
                dlgClear,
                nrckbv,
                1);
        }

        private bool NextPage(IWebDriver driver, string grdId) {
            Thread.Sleep(2000);
            int pagesCount = GetPagesCount(driver, grdId);
            if (pagesCount <= 1) {
                return true;
            }

            int currPageIndexBefore = GetPageIndex(driver, grdId);

            WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_IN_SECONDS));
            IWebElement btnNext = webDriverWait.Until(c => c.FindElement(By.ClassName("reget-grid-footer-button-next")));
            btnNext.Click();

            Thread.Sleep(2000);
            int currPageIndexAfter = GetPageIndex(driver, grdId);

            if (IsErrorDisplayed(driver)) {
                return false;
            }

            return (currPageIndexBefore + 1 == currPageIndexAfter);
        }

        private bool PreviousPage(IWebDriver driver, string grdId) {
            Thread.Sleep(2000);
            int pagesCount = GetPagesCount(driver, grdId);
            if (pagesCount <= 1) {
                return true;
            }

            int currPageIndexBefore = GetPageIndex(driver, grdId);

            WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_IN_SECONDS));
            IWebElement btnPrevious = webDriverWait.Until(c => c.FindElement(By.ClassName("reget-grid-footer-button-previous")));
            btnPrevious.Click();

            Thread.Sleep(2000);
            int currPageIndexAfter = GetPageIndex(driver, grdId);

            if (IsErrorDisplayed(driver)) {
                return false;
            }

            return (currPageIndexBefore == currPageIndexAfter + 1);
        }

        private bool LastPage(IWebDriver driver, string grdId) {
            Thread.Sleep(2000);
            int pagesCount = GetPagesCount(driver, grdId);
            if (pagesCount <= 1) {
                return true;
            }

            WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_IN_SECONDS));
            IWebElement btnLast = webDriverWait.Until(c => c.FindElement(By.ClassName("reget-grid-footer-button-last")));
            btnLast.Click();

            Thread.Sleep(2000);
            pagesCount = GetPagesCount(driver, grdId);
            int currPageIndex = GetPageIndex(driver, grdId);

            if (IsErrorDisplayed(driver)) {
                return false;
            }

            return (pagesCount == currPageIndex);
        }

        private bool FirstPage(IWebDriver driver, string grdId) {
            Thread.Sleep(2000);
            int pagesCount = GetPagesCount(driver, grdId);
            if (pagesCount <= 1) {
                return true;
            }

            WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_IN_SECONDS));
            IWebElement btnFirst = webDriverWait.Until(c => c.FindElement(By.ClassName("reget-grid-footer-button-first")));
            btnFirst.Click();

            Thread.Sleep(2000);
            int currPageIndex = GetPageIndex(driver, grdId);

            if (IsErrorDisplayed(driver)) {
                return false;
            }

            return (currPageIndex == 1);
        }

        private bool GoToPage(IWebDriver driver, string grdId) {
            Thread.Sleep(2000);
            int pagesCount = GetPagesCount(driver, grdId);
            if (pagesCount <= 1) {
                return true;
            }

            WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_IN_SECONDS));
            string strPageIndex = grdId + PAGE_INDEX;
            IWebElement txtPageIndex = webDriverWait.Until(c => c.FindElement(By.Id(strPageIndex)));
            txtPageIndex.Clear();

            pagesCount = GetPagesCount(driver, grdId);
            int goToPage = new Random().Next(2, pagesCount + 1);
            txtPageIndex.Clear();
            string strGoToPage = goToPage.ToString();
            txtPageIndex.SendKeys(strGoToPage);
            //for (int i = 0; i < strGoToPage.Length; i++) {
            //    txtPageIndex.SendKeys(strGoToPage.Substring(i,1));
            //}

            Thread.Sleep(2000);
            int currPageIndex = GetPageIndex(driver, grdId);

            if (IsErrorDisplayed(driver)) {
                return false;
            }

            return (currPageIndex == goToPage);
        }

        private bool Refresh(IWebDriver driver) {
            Thread.Sleep(2000);

            WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_IN_SECONDS));
            IWebElement btnRefresh = webDriverWait.Until(c => c.FindElement(By.ClassName("reget-grid-footer-button-refresh")));
            btnRefresh.Click();

            Thread.Sleep(2000);

            if (IsErrorDisplayed(driver)) {
                return false;
            }

            return true;
        }

        private bool SetPageSize(IWebDriver driver, string grdId) {
            Thread.Sleep(2000);

            WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_IN_SECONDS));

            int pagesCountBefore = GetPagesCount(driver, grdId);

            string strPageSize = grdId + PAGE_SIZE;
            SelectElement cmbPageSize = new SelectElement(webDriverWait.Until(c => c.FindElement(By.Id(strPageSize))));
            cmbPageSize.SelectByText("50");

            Thread.Sleep(2000);

            int pagesCountAfter = GetPagesCount(driver, grdId);

            if (IsErrorDisplayed(driver)) {
                return false;
            }

            double dRation = pagesCountBefore / 5.0;
            dRation = Math.Ceiling(dRation);
            int iPageCount = Convert.ToInt32(dRation);

            return (pagesCountAfter == iPageCount);
        }

        private void SetPageSize(IWebDriver driver, string grdId, int pageSize) {
            Thread.Sleep(2000);

            WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_IN_SECONDS));

            int pagesCountBefore = GetPagesCount(driver, grdId);

            string strPageSize = grdId + PAGE_SIZE;
            SelectElement cmbPageSize = new SelectElement(webDriverWait.Until(c => c.FindElement(By.Id(strPageSize))));
            cmbPageSize.SelectByText(pageSize.ToString());

            Thread.Sleep(2000);

        }


        private int GetPageIndex(IWebDriver driver, string grdId) {
            WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_IN_SECONDS));
            IWebElement intputPageIndex = webDriverWait.Until(c => c.FindElement(By.Id(grdId + PAGE_INDEX)));
            string strPageIndex = intputPageIndex.GetAttribute("value");
            int iPageIndex = Convert.ToInt32(strPageIndex);
            return iPageIndex;

            //ReadOnlyCollection<IWebElement> inputs = webDriverWait.Until(c => c.FindElements(By.TagName("input")));
            //foreach (var input in inputs) {
            //    if (input.GetAttribute("id") != null && input.GetAttribute("id").EndsWith(PAGE_INDEX)) {
            //        string strPageIndex = input.GetAttribute("value");
            //        int iPageIndex = Convert.ToInt32(strPageIndex);
            //        return iPageIndex;

            //    }
            //}

            //return -1;
        }

        private int GetPagesCount(IWebDriver driver, string grdId) {
            WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_IN_SECONDS));
            IWebElement tdPagesCount = webDriverWait.Until(c => c.FindElement(By.Id(grdId + PAGES_COUNT)));
            string strPagesCount = tdPagesCount.GetAttribute("innerHTML");
            string[] pgCountItems = strPagesCount.Trim().Split(' ');
            int iPagesCount = Convert.ToInt32(pgCountItems[1]);
            return iPagesCount;
        }


        public bool ExportToExcel() {
            string downloadedFile = null;
            try {
                using (IWebDriver driver = GetWebDriver(0)) {
                    //Arange
                    string strDownloadFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                    strDownloadFolder = Path.Combine(strDownloadFolder, "Downloads");
                    var lastFileWriteDate = DateTime.MinValue;
                    var sortedFiles = new DirectoryInfo(strDownloadFolder).GetFiles()
                                                      .OrderByDescending(f => f.LastWriteTime)
                                                      .ToList();
                    if (sortedFiles.Count > 0) {
                        lastFileWriteDate = (sortedFiles.ElementAt(0).LastWriteTime);
                    }

                    WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_IN_SECONDS));
                    IWebElement btnExportExcel = webDriverWait.Until(c => c.FindElement(By.ClassName("reget-grid-footer-button-excel")));

                    //btnExportExcel.Click();
                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                    js = (IJavaScriptExecutor)driver;
                    js.ExecuteScript("arguments[0].click();", btnExportExcel);
                    Thread.Sleep(3000);

                    if (IsErrorDisplayed(driver)) {
                        return false;
                    }

                    int iStep = 0;
                    bool isOk = false;
                    while (iStep < 10 && !isOk) {
                        sortedFiles = new DirectoryInfo(strDownloadFolder).GetFiles()
                                                          .OrderByDescending(f => f.LastWriteTime)
                                                          .ToList();
                        if (sortedFiles.Count == 0) {
                            Thread.Sleep(3000);
                            iStep++;
                        }

                        var newLastFileWriteDate = (sortedFiles.ElementAt(0).LastWriteTime);
                        if (newLastFileWriteDate > lastFileWriteDate &&
                            sortedFiles.ElementAt(0).Extension.ToLower() == ".xlsx") {
                            downloadedFile = sortedFiles.ElementAt(0).FullName;
                            isOk = true;
                        }
                        else {
                            Thread.Sleep(3000);
                            iStep++;
                        }
                    }

                    return isOk;
                }
            }
            catch (Exception ex) {
                return false;
            } finally {
                if (!String.IsNullOrEmpty(downloadedFile)) {
                    File.Delete(downloadedFile);
                }
            }
        }

        public bool AddNewRecord(IWebDriver driver, NewRowCheckboxValueType ckbValueType) {
            Thread.Sleep(2000);
                       
            IWebElement btnAdd = null;
            try {
                btnAdd = driver.FindElement(By.ClassName("reget-grid-footer-button-add"));

            } catch (Exception ex) {
                if (ex is NoSuchElementException) {
                    return true;
                }
            }

            if (btnAdd != null) {
                btnAdd.Click();
                Thread.Sleep(1000);

                IWebElement editRow = GetEditRowWebElement(driver);
                if (editRow == null) {
                    return false;
                }

                WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_IN_SECONDS));
                SetEditRow(driver, webDriverWait, editRow, ckbValueType);

                Thread.Sleep(1000);
                IWebElement btnSave = driver.FindElement(By.ClassName("reget-btn-save"));
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("arguments[0].click();", btnSave);
                //btnSave.Click();
                Thread.Sleep(2000);

                if (IsErrorDisplayed(driver)) {
                    return false;
                }

                return true;
            }

            return false;
        }

        private bool ClickAddNewTwice(IWebDriver driver) {
            Thread.Sleep(2000);

            IWebElement btnAdd = null;
            try {
                btnAdd = driver.FindElement(By.ClassName("reget-grid-footer-button-add"));

            } catch (Exception ex) {
                if (ex is NoSuchElementException) {
                    return true;
                }
            }

            if (btnAdd != null) {
                btnAdd.Click();
                Thread.Sleep(1000);
            }

            btnAdd = driver.FindElement(By.ClassName("reget-grid-footer-button-add"));
            btnAdd.Click();
            Thread.Sleep(1000);

            if (IsErrorDisplayed(driver)) {
                return false;
            }

            return true;
        }

        private bool DeleteLastRow(IWebDriver driver) {
           
            if (!IsAddNewRowEnabled(driver)) {
                return true;
            }

            //Delete Row
            ReadOnlyCollection<IWebElement> rows = driver.FindElements(By.ClassName("ui-grid-row"));
            if (rows == null || rows.Count == 0) {
                return false;
            }
            IWebElement lastRow = rows[rows.Count - 1];
            IWebElement btnDel = lastRow.FindElement(By.ClassName("reget-btn-grid-delete"));
            btnDel.Click();
            Thread.Sleep(1000);

            //Check confirm text
            var mdDialogContent = driver.FindElement(By.ClassName("md-dialog-content"));
            var ps = mdDialogContent.FindElements(By.TagName("p"));
            if (String.IsNullOrWhiteSpace(ps[0].GetAttribute("innerHTML"))) {
                return false;
            }

            IWebElement btnYes = driver.FindElement(By.Id("btnPerform"));
            btnYes.Click();
            Thread.Sleep(1000);

            if (IsErrorDisplayed(driver)) {
                return false;
            }

            return true;
        }

        private IWebElement GetEditRowWebElement(IWebDriver driver) {
            ReadOnlyCollection<IWebElement> rows = driver.FindElements(By.ClassName("ui-grid-row"));
            if (rows == null || rows.Count == 0) {
                return null;
            }

            foreach (var row in rows) {
                try {
                    IWebElement btnSuccess = row.FindElement(By.ClassName("reget-btn-grid-save"));
                    return row;
                } catch (Exception ex) {
                    if (!(ex is NoSuchElementException)) {
                        throw ex;
                    }
                }
            }

            return null;
        }

        public void SetEditRow(
            IWebDriver driver,
            WebDriverWait webDriverWait,
            IWebElement editRow,
            NewRowCheckboxValueType ckbValueType) {


            //IWebElement editRowArea = editRow.FindElement(By.ClassName("ng-isolate-scope"));
            ReadOnlyCollection<IWebElement> rowChildrenElements = editRow.FindElements(By.XPath("*"));
            var editRowArea = rowChildrenElements[0];

            ReadOnlyCollection<IWebElement> cells = editRowArea.FindElements(By.ClassName("ui-grid-cell"));
            foreach (var cell in cells) {
                ReadOnlyCollection<IWebElement> childrenElements = cell.FindElements(By.XPath("*"));
                var divNgScope = childrenElements[0];

                childrenElements = divNgScope.FindElements(By.XPath("*"));
                if (childrenElements == null || childrenElements.Count == 0) {
                    continue;
                }
                var divNgScope2 = childrenElements[0];

                childrenElements = divNgScope2.FindElements(By.XPath("*"));
                if (childrenElements == null || childrenElements.Count == 0) {
                    continue;
                }

                var editElement = childrenElements[0];
                string strTagName = editElement.TagName;
                switch (strTagName) {
                    case "md-select":
                        SetMdSelect(editElement, webDriverWait);
                        break;
                    case "md-autocomplete":
                        Thread.Sleep(2000);
                        editElement.Click();
                        IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                        string txtAutoId = null;
                        if (editElement.GetAttribute("id") == "gridFieldAutoUser") {
                            txtAutoId = "intGridUserAutocomplete";
                        } else if (editElement.GetAttribute("id") == "gridFieldAutoAddress") {
                            txtAutoId = "intGridAddressAutocomplete";
                        }

                        var lis = FindautoCompleteOptionsElements(driver, webDriverWait, txtAutoId);
                        if (lis.Count > 0) {
                            js = (IJavaScriptExecutor)driver;
                            js.ExecuteScript("arguments[0].click();", lis[0]);
                            Thread.Sleep(2000);
                        }
                        break;
                    case "input":
                        SetNewItemInput(editElement);
                        break;
                    case "div":
                        //Checkbox
                        var mdCheckboxs = editElement.FindElements(By.TagName("md-checkbox"));
                        if (mdCheckboxs != null && mdCheckboxs.Count > 0) {
                            if (ckbValueType == NewRowCheckboxValueType.All) {
                                if (mdCheckboxs[0].GetAttribute("checked") != "true") {
                                    mdCheckboxs[0].Click();
                                }
                            } else if (ckbValueType == NewRowCheckboxValueType.None) {
                                if (mdCheckboxs[0].GetAttribute("checked") == "true") {
                                    mdCheckboxs[0].Click();
                                }
                            }
                            break;
                        } 

                        var buttons = editElement.FindElements(By.TagName("button"));
                        if (buttons != null && buttons.Count > 0) {
                            if (buttons[0].GetAttribute("id") == "btnUserLookup") {

                                var inputs = divNgScope.FindElements(By.TagName("input"));
                                SetNewItemInput(inputs[0]);
                                buttons[0].Click();
                                Thread.Sleep(2000);
                                break;
                            }

                            if (buttons[0].GetAttribute("id") == "btnTranslate") {
                                var inputs = divNgScope.FindElements(By.TagName("input"));
                                SetNewItemInput(inputs[0]);
                            }
                        }
                        break;
                }

            }


        }

        private void SetMdSelect(IWebElement editElement, WebDriverWait webDriverWait) {
            editElement.Click();
            Thread.Sleep(1000);
            var elements = FindCmbOptionsElements(editElement, webDriverWait);
            elements[0].Click();
        }

        private void SetNewItemInput(IWebElement editElement) {
            editElement.Clear();
            for(int i=0; i< NEW_ITEM_TEXT.Length; i++) {
                editElement.SendKeys(NEW_ITEM_TEXT.Substring(i,1));
                Thread.Sleep(200);
            }
            //editElement.SendKeys(NEW_ITEM_TEXT);
        }

        private bool ClearFilter(IWebDriver driver) {
            Thread.Sleep(1000);
            ReadOnlyCollection<IWebElement> filterContainers = driver.FindElements(By.ClassName("ui-grid-filter-container"));
            foreach (var filterContainer in filterContainers) {
                //IWebElement filterContainer = gridFilter.FindElement(By.ClassName("ui-grid-filter-container"));
                ReadOnlyCollection<IWebElement> filterDivs = filterContainer.FindElements(By.XPath("*"));
                IWebElement filterDiv = filterDivs[0];
                ReadOnlyCollection<IWebElement> filterElements = filterDiv.FindElements(By.XPath("*"));
                foreach (var filterElement in filterElements) {
                    try {

                        string strTagName = filterElement.TagName;
                        switch (strTagName) {
                            case "select":
                                SelectElement selElement = new SelectElement(filterElement);
                                selElement.SelectByValue("");
                                break;
                            case "input":
                                filterElement.Clear();
                                break;
                        }

                    } catch (Exception ex) {
                        if (ex is ElementNotInteractableException) {
                            continue;
                        } else {
                            throw ex;
                        }
                    }
                }
            }

            if (IsErrorDisplayed(driver)) {
                return false;
            }

            return true;
        }

        private bool SetFilter(IWebDriver driver) {
            Thread.Sleep(1000);

            ReadOnlyCollection<IWebElement> filterContainers = driver.FindElements(By.ClassName("ui-grid-filter-container"));
            foreach (var filterContainer in filterContainers) {
                //IWebElement filterContainer = gridFilter.FindElement(By.ClassName("ui-grid-filter-container"));
                ReadOnlyCollection<IWebElement> filterDivs = filterContainer.FindElements(By.XPath("*"));
                IWebElement filterDiv = filterDivs[0];
                ReadOnlyCollection<IWebElement> filterElements = filterDiv.FindElements(By.XPath("*"));
                foreach (var filterElement in filterElements) {
                    try {
                        string strTagName = filterElement.TagName;
                        switch (strTagName) {
                            case "select":
                                SelectElement selElement = new SelectElement(filterElement);
                                selElement.SelectByIndex(1);
                                break;
                            case "input":
                                filterElement.Clear();
                                filterElement.SendKeys("filter");
                                break;
                        }
                    } catch (Exception ex) {
                        if (ex is ElementNotInteractableException) {
                            continue;
                        } else {
                            throw ex;
                        }
                    }
                }
            }

            Thread.Sleep(1000);

            if (IsErrorDisplayed(driver)) {
                return false;
            }

            return true;
        }

        private bool SaveFilter(IWebDriver driver, string grdId) {
            Thread.Sleep(1000);

            WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_IN_SECONDS));
            Thread.Sleep(1000);
            IWebElement btnSave = FindElementById(webDriverWait, "saveGridSettings_" + grdId);
            btnSave.Click();
            Thread.Sleep(2000);

            driver.Navigate().Refresh();
            Thread.Sleep(3000);

            //No Rows
            if (IsElementExistByClassName(driver, "ui-grid-row")) {
                return false;
            }

            //IWebElement divNoRecord = FindElementById(webDriverWait, grdId + "_DivNoRecord");
            //if (divNoRecord == null) {
            //    return false;
            //}

            if (IsErrorDisplayed(driver)) {
                return false;
            }

            return true;
        }

        private bool ResetFilter(IWebDriver driver, string grdId) {
            Thread.Sleep(1000);

            WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_IN_SECONDS));
                        
            IWebElement btnReset = FindElementById(webDriverWait, "clearGridSettings_" + grdId);
            btnReset.Click();
            Thread.Sleep(2000);

            if (!IsElementExistByClassName(driver, "ui-grid-row")) {
                return false;
            }
            //try {
            //    IWebElement divNoRecord = FindElementById(webDriverWait, grdId + "_DivNoRecord");

            //} catch (Exception ex) {
            //    if (!(ex is NoSuchElementException)) {
            //        throw ex;
            //    }
            //}

            if (IsErrorDisplayed(driver)) {
                return false;
            }

            return true;
        }

        private bool LoadIsNotDuplicated(IWebDriver driver, int maxAllowedCount) {
            WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_IN_SECONDS));

            driver.Navigate().Refresh();

            Thread.Sleep(1000);

            int loadCount = GetLoadDataCount(webDriverWait);
            if (loadCount == 0) {
                return false;
            }

            if (loadCount > maxAllowedCount) {
                return false;
            }

            return true;
        }

        private bool LoadFilterIsNotDuplicated(IWebDriver driver, string grdId, int maxAllowedCount) {
            try {
                WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_IN_SECONDS));

                SetFilter(driver);
                SaveFilter(driver, grdId);

                driver.Navigate().Refresh();

                Thread.Sleep(1000);

                int loadCount = GetLoadDataCount(webDriverWait);
                if (loadCount == 0) {
                    return false;
                }

                if (loadCount > maxAllowedCount) {
                    return false;
                }

                return true;
            } catch (Exception ex) {
                throw ex;
            } finally {
                ResetFilter(driver, grdId);
            }
        }

        private int GetLoadDataCount(WebDriverWait webDriverWait) {
            //How Many Time are data loaded during displaying grid and loading filter
            var testSpGridLoadDataCount = FindElementById(webDriverWait, "testSpGridLoadDataCount");
            string strCount = testSpGridLoadDataCount.GetAttribute("innerHTML");
            int iCount = Convert.ToInt16(strCount);

            return iCount;
        }

        private bool DuplicityRow(IWebDriver driver, string grdId) {
            Thread.Sleep(2000);
            NewRowCheckboxValueType ckbValueType = NewRowCheckboxValueType.None;
            if (!AddNewRecord(driver, ckbValueType)) {
                return false;
            }

            Thread.Sleep(2000);
            if (AddNewRecord(driver, ckbValueType)) {
                return false;
            }

            Thread.Sleep(1000);
            WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_IN_SECONDS));
            IWebElement btnErrClose = FindElementById(webDriverWait, "btnErrClose");
            btnErrClose.Click();

            return true;
        }

        private bool IsAddNewRowEnabled(IWebDriver driver) {
            IWebElement btnAdd = null;
            try {
                btnAdd = driver.FindElement(By.ClassName("reget-grid-footer-button-add"));

            } catch (Exception ex) {
                if (ex is NoSuchElementException) {
                    return false;
                }
            }

            return (btnAdd != null);
        }

        protected void SaveFailTestLog() {
        }
        #endregion
    }
}
