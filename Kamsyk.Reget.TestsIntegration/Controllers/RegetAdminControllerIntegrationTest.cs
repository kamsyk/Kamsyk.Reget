using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Kamsyk.Reget.Model.Repositories;
using Kamsyk.Reget.TestsIntegration.BaseTest;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.ObjectModel;
using Kamsyk.Reget.Model.ExtendedModel;
using static Kamsyk.Reget.Model.Repositories.UserRepository;
using Kamsyk.Reget.Model;
using Kamsyk.Reget.Model.Common;

namespace Kamsyk.Reget.TestsIntegration.Controllers {
    [TestClass]
    public class RegetAdminControllerIntegrationTest : BaseTestIntegration {
        #region Constructor
        public RegetAdminControllerIntegrationTest() {
            if (!IsTestDbConnected) {
                throw new Exception("Test DB is not connected");
            }
        }
        #endregion

        #region Test Methods
        [TestMethod]
        public void ZzInt_CentreGroup_DetailsSaveChanges() {
            try {
                using (IWebDriver driver = GetWebDriver(0)) {
                    //Arange
                    string url = AppRootUrl + "RegetAdmin";

                    //Act
                    driver.Url = url;
                    WaitUntilLoadDialogIsClosed(driver);

                    Thread.Sleep(1000);

                    WebDriverWait webDriverWait;
                    webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                    IWebElement cmbCg = FindElementById(webDriverWait, "cmbCgList"); //webDriverWait.Until(c => c.FindElement(By.Id("cmbCgList")));
                    cmbCg.Click();
                    Thread.Sleep(500);

                    var options = webDriverWait.Until(c => c.FindElements(By.TagName("md-option")));

                    int optIndex = 0;
                    string strCgId = null;
                    while (strCgId == null) {
                        optIndex = new Random().Next(0, options.Count - 1);
                        strCgId = options[optIndex].GetAttribute("value");
                    }

                    options[optIndex].Click();
                    Thread.Sleep(3000);

                    Thread.Sleep(1000);
                    IWebElement txtCgName = driver.FindElement(By.Id("txtCgName"));
                    string newCgName = (txtCgName.GetAttribute("value").Length > 25) ? txtCgName.GetAttribute("value").Substring(0, 24) : txtCgName.GetAttribute("value") + "1";
                    txtCgName.Clear();
                    txtCgName.SendKeys(newCgName);

                    webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                    //webDriverWait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("btnSaveCgDetails")));
                    IWebElement btnSaveCgDetails = webDriverWait.Until(c => c.FindElement(By.Id("btnSaveCgDetails")));
                    //IWebElement btnSaveCgDetails = driver.FindElement(By.Id("btnSaveCgDetails"));
                    Thread.Sleep(2000);
                    btnSaveCgDetails.Click();

                    webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                    //webDriverWait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("btnRefresh")));
                    IWebElement btnRefresh = webDriverWait.Until(c => c.FindElement(By.Id("btnRefresh")));
                    Thread.Sleep(2000);
                    btnRefresh.Click();

                    //txtCgName = driver.FindElement(By.Id("txtCgName"));
                    txtCgName = webDriverWait.Until(c => c.FindElement(By.Id("txtCgName")));

                    //Assert
                    Assert.IsTrue(txtCgName.GetAttribute("value") == newCgName);
                }
            } catch (Exception ex) {
                new TestFailRepository().SaveTestFail("ZzInt_CentreGroup_DetailsSaveChanges", ex.ToString());
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ZzInt_CgAdmin_Autocomplete_NotFoud_TextInfoDisplayed() {

            using (IWebDriver driver = GetWebDriver(0, "en-US")) {
                //Arrange
                string url = AppRootUrl + "RegetAdmin";
                driver.Url = url;
                WaitUntilLoadDialogIsClosed(driver);

                WebDriverWait webDriverWait;
                webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                IWebElement cmbCg = webDriverWait.Until(c => c.FindElement(By.Id("cmbCgList")));
                cmbCg.Click();
                Thread.Sleep(1000);

                //webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                //var cmbSelect_option_5 = webDriverWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("select_option_20")));

                var options = webDriverWait.Until(c => c.FindElements(By.TagName("md-option")));

                int optIndex = 0;
                string strCgId = null;
                while (strCgId == null) {
                    optIndex = new Random().Next(0, options.Count - 1);
                    strCgId = options[optIndex].GetAttribute("value");
                }

                options[optIndex].Click();
                Thread.Sleep(3000);


                //Act
                bool isOk = true;

                //Deputy Orderer
                bool isDeputyOrdererOk = IsAutoCompleteNotFoundTestDisplayed(
                    driver,
                    webDriverWait,
                    "btnAddDeputyOrderer",
                    "autoDeputyOrderer",
                    "asdaswejwe wejwej",
                    "\"" + "asdaswejwe wejwej" + "\"" + " was not found.");

                //Thread.Sleep(3000);

                //Orderer Supplier
                bool isOkOrdSupSup = IsAutoCompleteNotFoundTestDisplayed(
                    driver,
                    webDriverWait,
                    "btnOrdererSupplier",
                    "agOrdererSupplierSupplier",
                    "asdaswejwe wejwej",
                    "\"" + "asdaswejwe wejwej" + "\"" + " was not found.");

                isOk = isDeputyOrdererOk && isOkOrdSupSup;

                //Assert
                Assert.IsTrue(isOk);
            }
        }

        [TestMethod]
        public void ZzInt_CentreGroup_AddNew() {
            try
            {
                string cgName = "Integration Test Cg";
                using (IWebDriver driver = GetWebDriver(0))
                {
                    //Arrange
                    CentreGroupRepository cgRepository = new CentreGroupRepository();
                    cgRepository.DeleteCentreGroup(cgName);

                    //Act
                    AddNewCg(driver, cgName);

                    //Assert    
                    Assert.IsTrue(cgRepository.IsCentreGroupExists(cgName));
                }
            }
            catch (Exception ex) {
                new TestFailRepository().SaveTestFail("ZzInt_CentreGroup_AddNew", ex.ToString());
                Assert.Fail();
            }
        }


        [TestMethod]
        public void ZzInt_CentreGroupAddNew_CgAlreadyExists() {
            //syka - Check Test
            string cgName = "Integration Test Cg";
            using (IWebDriver driver = GetWebDriver(0, "en-US")) {
                //Arrange
                CentreGroupRepository cgRepository = new CentreGroupRepository();
                try {
                    cgRepository.DeleteCentreGroup(cgName);

                    //Act
                    //cgRepository.AddCentreGroup(cgName);
                    AddNewCg(driver, cgName);

                    string url = AppRootUrl;
                    driver.Url = url;

                    Thread.Sleep(3000);

                    AddNewCg(driver, cgName);

                    //Assert   
                    WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                    IWebElement dialog = webDriverWait.Until(c => c.FindElement(By.Id("pErrMsg")));
                    Thread.Sleep(3000);
                    if (dialog == null) {
                        Assert.Fail();
                    }

                    string dialogHtml = dialog.GetAttribute("innerHTML");

                    Assert.IsTrue(dialogHtml.IndexOf("Area '" + cgName + "' already exists") > -1);
                } catch (Exception ex) {
                    new TestFailRepository().SaveTestFail("ZzInt_CentreGroupAddNew_CgAlreadyExists", ex.ToString());
                    Assert.Fail();
                } finally {
                    cgRepository.DeleteCentreGroup(cgName);
                }

            }
        }

        [TestMethod]
        public void ZzInt_CentreGroup_AddPurchaseGroup() {
            try {
                using (IWebDriver driver = GetWebDriver(0, "en-US")) {
                    //Arange
                    string url = AppRootUrl + "RegetAdmin";
                    string strPgName = "Purchase Group Name Test";

                    //Act
                    #region Act
                    driver.Url = url;
                    WaitUntilLoadDialogIsClosed(driver);


                    WebDriverWait webDriverWait;
                    webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                    IWebElement cmbCg = webDriverWait.Until(c => c.FindElement(By.Id("cmbCgList")));
                    cmbCg.Click();
                    Thread.Sleep(1000);

                    var options = webDriverWait.Until(c => c.FindElements(By.TagName("md-option")));

                    int optIndex = 0;
                    string strCgId = null;
                    while (strCgId == null) {
                        optIndex = new Random().Next(0, options.Count - 1);
                        strCgId = options[optIndex].GetAttribute("value");
                    }

                    //string strCgId = options[0].GetAttribute("value");
                    int cgId = Convert.ToInt32(strCgId);
                    options[optIndex].Click();
                    Thread.Sleep(3000);

                    CentreGroupExtended cg = new CentreGroupRepository().GetCentreGroupDataById(cgId, 0);
                    int companyId = cg.company_id;
                    //Delete PG
                    List<Purchase_Group> pgs = new PgRepository().GetPgsByName(strPgName);
                    if (pgs != null) {
                        foreach (var pg in pgs) {
                            new CentreGroupRepository().DeletePurchaseGroup(pg.id, cgId);
                        }
                    }
                    pgs = new PgRepository().GetPgsByName(strPgName);
                    if (pgs != null && pgs.Count > 0) {
                        throw new Exception("Test PG is not deleted");
                    }

                    //refresh
                    var btnRefresh = FindElementById(webDriverWait, "btnRefresh");
                    Thread.Sleep(5000);

                    CreatePg(driver, webDriverWait, companyId, strPgName, "0", false, "126.56", false);
                    #endregion

                    //Assert
                    pgs = new PgRepository().GetPgsByName(strPgName);
                    if (pgs == null || pgs.Count == 0) {
                        Assert.Fail();
                    }

                    if (pgs.ElementAt(0).Purchase_Group_Limit.Count == 0) {
                        Assert.Fail();
                    }

                    if (pgs.ElementAt(0).PurchaseGroup_Orderer.Count == 0) {
                        Assert.Fail();
                    }

                    Assert.IsTrue(true);
                }
            } catch (Exception ex) {
                new TestFailRepository().SaveTestFail("ZzInt_CentreGroup_AddPurchaseGroup", ex.ToString());
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ZzInt_MultiplyLimit_NotAuthorized() {
            //Arrange
            DeletePgMultiAppLevel();
            var participant = new UserRepository().GetNotCompanyAdmin();

            try {
                using (IWebDriver driver = GetWebDriver(participant.id, "cs-CZ")) {
                    //Act
                    string url = AppRootUrl + "RegetAdmin/MultiplyAppLevel";

                    //Act
                    driver.Url = url;
                    WaitUntilLoadDialogIsClosed(driver);

                    WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                    IWebElement h2NotAllowedPage = FindElementById(webDriverWait, "h2NotAllowedPage");


                    //Assert
                    bool isOK = h2NotAllowedPage.Text == "Nejste oprávněn(a) zobrazit tuto stánku.";
                    Assert.IsTrue(isOK);
                }
            } catch (Exception ex) {
                throw ex;
            } finally {
                SwitchUserToDefaultTestUser();
                DeletePgMultiAppLevel();
            }
        }

        [TestMethod]
        public void ZzInt_MultiplyLimit_SetSuccess() {
            //TEST FAILS REGULARLY
            try {
                string cultureName = "cs-CZ";
                using (IWebDriver driver = GetWebDriver(0, cultureName)) {
                    //Arange
                    DeletePgMultiAppLevel();
                    new PgLimitRepository().SetMultiLimitBoth();


                    string url = AppRootUrl + "RegetAdmin";
                    string pgName = "Pg Multi App Level";
                    decimal dOrigBottomLimit = 10.4M;
                    decimal dOrigTopLimit = 100.6M;
                    decimal dMultipl = 2.5M;
                    Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
                    string strBottomLimit = ConvertData.ToString(dOrigBottomLimit);
                    string strTopLimit = ConvertData.ToString(dOrigTopLimit);
                    string strMultipl = ConvertData.ToString(dMultipl);

                    //Act
                    #region Act
                    int cgId = DisplayCgSettings(driver);
                    WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));

                    CentreGroupExtended cg = new CentreGroupRepository().GetCentreGroupDataById(cgId, 0);
                    int companyId = cg.company_id;

                    int pgId = DataNulls.INT_NULL;
                    IWebElement pgDiv = GetPgDiv(driver, pgName, out pgId);

                    if (pgDiv != null) {
                        decimal currBottomLimit;
                        decimal currTopLimit;
                        bool isCurrBottomMulti;
                        bool isCurrTopMulti;


                        GetAppLimits(
                            driver,
                            webDriverWait,
                            pgName,
                            out currBottomLimit,
                            out currTopLimit,
                            out isCurrBottomMulti,
                            out isCurrTopMulti);

                        if (currBottomLimit != dOrigBottomLimit || currTopLimit != dOrigTopLimit ||
                            isCurrBottomMulti || !isCurrTopMulti) {

                            //set (fix) limits
                            SetPgLimitProperties(
                                driver,
                                webDriverWait,
                                pgName,
                                strBottomLimit,
                                strTopLimit,
                                false,
                                true);
                        }


                    } else {
                        CreatePg(driver, webDriverWait, companyId, pgName, strBottomLimit, false, strTopLimit, true);
                    }


                    MultiplyLimits(driver, webDriverWait, strMultipl);
                    DisplayCgSettings(driver, cgId);

                    decimal mulBottomLimit;
                    decimal mulTopLimit;
                    bool isBottomMulti;
                    bool isTopMulti;
                    GetAppLimits(
                        driver,
                        webDriverWait,
                        pgName,
                        out mulBottomLimit,
                        out mulTopLimit,
                        out isBottomMulti,
                        out isTopMulti);
                    decimal dCalcBottomLimit = dOrigBottomLimit;
                    decimal dCalcTopLimit = dOrigTopLimit * dMultipl;

                    if (mulBottomLimit != dCalcBottomLimit) {
                        Assert.Fail();
                        
                    }

                    if (mulTopLimit != dCalcTopLimit) {
                        Assert.Fail();
                        
                    }

                    //Revert changes
                    decimal dRevMultipl = 1 / dMultipl;
                    string strRevMultipl = ConvertData.ToString(dRevMultipl);
                    MultiplyLimits(driver, webDriverWait, strRevMultipl);
                    DisplayCgSettings(driver, cgId);

                    Thread.Sleep(3000);

                    decimal mulRevBottomLimit;
                    decimal mulRevTopLimit;
                    bool isMulBottomMulti;
                    bool isMulTopMulti;
                    GetAppLimits(
                        driver,
                        webDriverWait,
                        pgName,
                        out mulRevBottomLimit,
                        out mulRevTopLimit,
                        out isMulBottomMulti,
                        out isMulTopMulti);
                    if (mulRevBottomLimit != dOrigBottomLimit) {
                        Assert.Fail();
                        
                    }

                    if (mulRevTopLimit != dOrigTopLimit) {
                        Assert.Fail();
                        
                    }
                    #endregion


                    //Assert
                    Assert.IsTrue(true);

                }
            } catch (Exception ex) {
                new TestFailRepository().SaveTestFail("ZzInt_MultiplyLimit_SetSuccess", ex.ToString());
                throw ex;
            } finally {
                SwitchUserToDefaultTestUser();
                DeletePgMultiAppLevel();
            }
        }

        [TestMethod]
        public void ZzInt_MultiplyLimit_Fail() {
            //TEST FAILS REGULARLY
            try {
                string cultureName = "cs-CZ";
                using (IWebDriver driver = GetWebDriver(0, cultureName)) {
                    //Arange
                    DeletePgMultiAppLevel();

                    string url = AppRootUrl + "RegetAdmin";
                    string pgName = "Pg Multi App Level";
                    decimal dOrigBottomLimit = 99;
                    decimal dOrigTopLimit = 100;
                    decimal dMultipl = 1.5M;
                    Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
                    string strBottomLimit = ConvertData.ToString(dOrigBottomLimit);
                    string strTopLimit = ConvertData.ToString(dOrigTopLimit);
                    string strMultipl = ConvertData.ToString(dMultipl);

                    //Act
                    #region Act
                    int cgId = DisplayCgSettings(driver);
                    WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));

                    CentreGroupExtended cg = new CentreGroupRepository().GetCentreGroupDataById(cgId, 0);
                    int companyId = cg.company_id;

                    int pgId = DataNulls.INT_NULL;
                    IWebElement pgDiv = GetPgDiv(driver, pgName, out pgId);

                    if (pgDiv != null) {
                        decimal currBottomLimit;
                        decimal currTopLimit;
                        bool isCurrBottomMulti;
                        bool isCurrTopMulti;
                        
                        GetAppLimits(
                            driver,
                            webDriverWait,
                            pgName,
                            out currBottomLimit,
                            out currTopLimit,
                            out isCurrBottomMulti,
                            out isCurrTopMulti);

                        if (currBottomLimit != dOrigBottomLimit || currTopLimit != dOrigTopLimit ||
                            !isCurrBottomMulti || isCurrTopMulti) {

                            //set (fix) limits
                            SetPgLimitProperties(
                                driver,
                                webDriverWait,
                                pgName,
                                strBottomLimit,
                                strTopLimit,
                                true,
                                false);
                        }


                    } else {
                        CreatePg(driver, webDriverWait, companyId, pgName, strBottomLimit, true, strTopLimit, false);
                    }

                    MultiplyLimits(driver, webDriverWait, strMultipl);
                    var pErrMsg = FindElementById(webDriverWait, "pErrMsg");
                    if (pErrMsg.Text != "Schvalovací limity nemohly být vynásobeny. Další informace viz Seznam chyb níže") {
                        Assert.Fail();
                        return;
                    }

                    //try {
                    //    var pErrMsg = driver.FindElement(By.Id("pErrMsg"));

                    //    if (pErrMsg.Text != "Schvalovací limity nemohly být vynásobeny. Další informace viz Seznam chyb níže") {
                    //        Assert.Fail();
                    //        return;
                    //    }
                    //} catch (Exception ex) {
                    //    if (!(ex is NoSuchElementException)) {
                    //        throw ex;
                    //    }
                    //}


                    var btnErrClose = FindElementById(webDriverWait, "btnErrClose");
                    btnErrClose.Click();

                    bool isErrorDisplayed = false;
                    var divPgLimitMultiplyErr = FindElementById(webDriverWait, "divPgLimitMultiplyErr");
                    IReadOnlyCollection<IWebElement> tds = divPgLimitMultiplyErr.FindElements(By.TagName("td"));
                    foreach (var td in tds) {
                        if (isErrorDisplayed) {
                            break;
                        }
                        IReadOnlyCollection<IWebElement> hrefs = td.FindElements(By.TagName("a"));

                        foreach (var href in hrefs) {
                            string hrefUrl = href.GetAttribute("href");
                            if (hrefUrl.Contains("cgId=" + cgId)) {
                                isErrorDisplayed = true;
                                break;
                            }
                        }
                    }

                    //DisplayCgSettings(driver);
                    #endregion

                    //Assert
                    Assert.IsTrue(isErrorDisplayed);
                }
            } catch (Exception ex) {
                throw ex;
            } finally {
                SwitchUserToDefaultTestUser();
                DeletePgMultiAppLevel();
            }
        }

        [TestMethod]
        public void ZzInt_CentreGroup_AddDeleteCentre() {
            //Arrange
            string testCentreName = "It Test Centre";
            int companyId = 0;
            int userId = 0;

            CompanyRepository companyRepository = new CompanyRepository();
            var company = companyRepository.GetCompanyById(companyId);

            CentreRepository centreRepository = new CentreRepository();
            Centre centre = centreRepository.GetCentreByName(testCentreName);
            if (centre == null) {
                CentreAdminExtended centreAdminExtended = new CentreAdminExtended();
                centreAdminExtended.id = -1;
                centreAdminExtended.manager_id = null;
                centreAdminExtended.name = testCentreName;
                centreAdminExtended.company_id = companyId;
                centreAdminExtended.company_name = company.country_code;
                centreAdminExtended.export_price_text = "Always";
                centreAdminExtended.active = true;
                centreAdminExtended.modify_user = UserRepository.REGET_SYSTEM_USER;
                centreAdminExtended.modify_date = DateTime.Now;
                List<string> errMsg;
                centreRepository.SaveCentreData(
                    centreAdminExtended,
                    userId,
                    "Always",
                    "Never",
                    "Optional",
                    out errMsg);

                centre = centreRepository.GetCentreByName(testCentreName);
            }

                        
            
            if (centre.Centre_Group != null && centre.Centre_Group.Count > 0) {
                centreRepository.RemoveCentreFromCg(centre.id);
            }

            CentreGroupRepository centreGroupRepository = new CentreGroupRepository();
            List<Centre_Group> aCgs = centreGroupRepository.GetActiveCentreGroupDataByCompanyId(companyId);
            Centre_Group cg = aCgs.ElementAt(0);
            string cgName = cg.name;

            //Act
            using (IWebDriver driver = GetWebDriver(userId, "en-US")) {
                
                string url = AppRootUrl + "RegetAdmin";
                                
                driver.Url = url;
                WaitUntilLoadDialogIsClosed(driver);


                WebDriverWait webDriverWait;
                webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                IWebElement cmbCg = webDriverWait.Until(c => c.FindElement(By.Id("cmbCgList")));
                cmbCg.Click();

                Thread.Sleep(1000);

                var options = webDriverWait.Until(c => c.FindElements(By.TagName("md-option")));
                                
                foreach (IWebElement option in options) {
                    ReadOnlyCollection<IWebElement> children = option.FindElements(By.XPath("*"));
                    IWebElement div = children[0];

                    //var mdTextDiv = FindElementByClassName(webDriverWait, "md-text");

                    ReadOnlyCollection<IWebElement> spans = div.FindElements(By.TagName("span"));
                    if (spans.Count == 0) {
                        continue;
                    }
                    IWebElement span = spans[0];

                    string strItemCgNAme = span.GetAttribute("innerHTML");
                    if (strItemCgNAme == cgName) {
                        option.Click();
                        Thread.Sleep(3000);
                        break;
                    }
                }

                var btnAddCentre = FindElementById(webDriverWait, "btnAddCentre");
                btnAddCentre.Click();

                var autoCentre = FindElementById(webDriverWait, "txtAutoCgCentre");
                autoCentre.SendKeys(testCentreName);
                var centres = FindautoCompleteOptionsElements(driver, webDriverWait, "txtAutoCgCentre");
                centres[0].Click();
                Thread.Sleep(2000);

                var btnSaveCgDetails = FindElementById(webDriverWait, "btnSaveCgDetails");
                btnSaveCgDetails.Click();
                Thread.Sleep(2000);

                var cgCheck = centreGroupRepository.GetCentreGroupDataById(cg.id, userId);
                bool isFound = false;
                foreach (var c in cgCheck.centre) {
                    if (c.name == testCentreName) {
                        isFound = true;
                        break;
                    }
                }
                if (!isFound) {
                    Assert.Fail();
                }

                //centre = centreRepository.GetCentreByName(testCentreName);
                //if(centre.Centre_Group == null || centre.Centre_Group.Count == 0) {
                //    Assert.Fail();
                //}

                //Delete Centre from Cg
                var centresChips = driver.FindElements(By.TagName("centre"));
                foreach (var centresChip in centresChips) {
                    var spans = centresChip.FindElements(By.TagName("span"));
                    foreach (var span in spans) {
                        string spanText = span.GetAttribute("innerHTML");
                        if (spanText == testCentreName) {
                            var btns = centresChip.FindElements(By.TagName("button"));
                            btns[0].Click();
                            Thread.Sleep(2000);
                            btnSaveCgDetails = FindElementById(webDriverWait, "btnSaveCgDetails");
                            btnSaveCgDetails.Click();
                            Thread.Sleep(2000);
                            break;
                        }
                    }
                }
            }

            //Assert
            var cgCheckF = centreGroupRepository.GetCentreGroupDataById(cg.id, userId);
            bool isFoundF = false;
            foreach (var c in cgCheckF.centre) {
                if (c.name == testCentreName) {
                    isFoundF = true;
                    break;
                }
            }
            Assert.IsFalse(isFoundF);
        }

        //[TestMethod]
        //public void ZzInt_CentreGroup_ChangeCurrency() {

        //    using (IWebDriver driver = GetWebDriver(0)) {

        //        Assert.Fail();
        //    }
        //}

        //[TestMethod]
        //public void ZzInt_CentreGroup_ReadOnly() {

        //    using (IWebDriver driver = GetWebDriver(0)) {

        //        Assert.Fail();
        //    }
        //}

        //[TestMethod]
        //public void ZzInt_CentreGroup_Editable() {

        //    using (IWebDriver driver = GetWebDriver(0)) {

        //        Assert.Fail();
        //    }
        //}

        //[TestMethod]
        //public void ZzInt_CentreGroup_NotAllCgAvailable() {

        //    using (IWebDriver driver = GetWebDriver(0)) {

        //        Assert.Fail();
        //    }
        //}

        //[TestMethod]
        //public void ZzInt_CentreGroupAddCentre_AvailableForAdmin() {

        //    using (IWebDriver driver = GetWebDriver(0)) {

        //        Assert.Fail();
        //    }
        //}

        //[TestMethod]
        //public void ZzInt_CentreGroupAddCentre_NotAvailableForNonAdminUsers() {

        //    using (IWebDriver driver = GetWebDriver(0)) {

        //        Assert.Fail();
        //    }
        //}

        //[TestMethod]
        //public void ZzInt_CentreGroup_AppMatrix_EmptyPurchaseGroups() {

        //    using (IWebDriver driver = GetWebDriver(0)) {

        //        Assert.Fail();
        //    }
        //}

        //[TestMethod]
        //public void ZzInt_CentreGroup_AppMatrix_EmptyPurchaseGroupLimits() {

        //    using (IWebDriver driver = GetWebDriver(0)) {

        //        Assert.Fail();
        //    }
        //}

        //[TestMethod]
        //public void ZzInt_CentreGroup_AppMatrix_EmptyApproveManagers() {

        //    using (IWebDriver driver = GetWebDriver(0)) {

        //        Assert.Fail();
        //    }
        //}

        //[TestMethod]
        //public void ZzInt_CentreGroup_AppMatrixChangeCurrency_AppMatrixLimitRecalculated() {

        //    using (IWebDriver driver = GetWebDriver(0)) {

        //        Assert.Fail();
        //    }
        //}

        //[TestMethod]
        //public void ZzInt_CentreGroup_CZ_AppMatrixPurchaseGroupInCzech() {

        //    using (IWebDriver driver = GetWebDriver(0)) {

        //        Assert.Fail();
        //    }
        //}

        //[TestMethod]
        //public void ZzInt_PurchaseGroup_6limits_addlimitbutton_is_hidden() {

        //    using (IWebDriver driver = GetWebDriver(0)) {

        //        Assert.Fail();
        //    }
        //}

        //[TestMethod]
        //public void ZzInt_CentreGroup_DisplyAllPgMaintenanceOneByOne() {

        //    using (IWebDriver driver = GetWebDriver(0)) {

        //        Assert.Fail();
        //    }
        //}

        //[TestMethod]
        //public void ZzInt_PurchaseGroup_NameChange() {

        //    using (IWebDriver driver = GetWebDriver(0)) {

        //        Assert.Fail();
        //    }
        //}

        //[TestMethod]
        //public void ZzInt_DeleteMoreRequestors_Saved() {

        //    using (IWebDriver driver = GetWebDriver(0)) {

        //        Assert.Fail();
        //    }
        //}

        //[TestMethod]
        //public void ZzInt_DeleteMoreOrderers_Saved() {

        //    using (IWebDriver driver = GetWebDriver(0)) {

        //        Assert.Fail();
        //    }
        //}

        //[TestMethod]
        //public void ZzInt_SavePurchaseGroupData_Saved() {

        //    using (IWebDriver driver = GetWebDriver(0)) {

        //        Assert.Fail();
        //    }
        //}

        //[TestMethod]
        //public void ZzInt_DeleteNonActiveUserFromPgSettings_Saved() {

        //    using (IWebDriver driver = GetWebDriver(0)) {

        //        Assert.Fail();
        //    }
        //}

        //[TestMethod]
        //public void ZzInt_ReplaceAppMan_SaveddisplayedInRaedOnlyList() {

        //    using (IWebDriver driver = GetWebDriver(0)) {

        //        Assert.Fail();
        //    }
        //}

        //[TestMethod]
        //public void ZzInt_AddApproveLimit2AppMenMoveUp_SaveddisplayedInRaedOnlyList() {

        //    using (IWebDriver driver = GetWebDriver(0)) {

        //        Assert.Fail();
        //    }
        //}

        //[TestMethod]
        //public void ZzInt_PurchaseGroup_DeleteLastImpliciteOrderer_OrderersEmpty() {

        //    using (IWebDriver driver = GetWebDriver(0)) {

        //        Assert.Fail();
        //    }
        //}

        //[TestMethod]
        //public void ZzInt_PurchaseGroup_DeleteLastImpliciteRequestor_OrderersEmpty() {

        //    using (IWebDriver driver = GetWebDriver(0)) {

        //        Assert.Fail();
        //    }
        //}

        //[TestMethod]
        //public void ZzInt_DeletePurchaseGroup_LastImpliciteOrderer_ImpliciteRequestorNotDefault() {

        //    using (IWebDriver driver = GetWebDriver(0)) {

        //        Assert.Fail();
        //    }
        //}

        //[TestMethod]
        //public void ZzInt_DeletePurchaseGroup_LastImpliciteOrderer_ImpliciteOrdererNotDefault() {

        //    using (IWebDriver driver = GetWebDriver(0)) {

        //        Assert.Fail();
        //    }
        //}

        #region Participants
        //[TestMethod]
        //public void ZzInt_DeleteParticipant_Implicite_1Centre_noother_implicite() {

        //    using (IWebDriver driver = GetWebDriver(0)) {

        //        Assert.Fail();
        //    }
        //}

        //[TestMethod]
        //public void ZzInt_DeleteParticipant_Implicite_1Centre_other_implicite() {

        //    using (IWebDriver driver = GetWebDriver(0)) {

        //        Assert.Fail();
        //    }
        //}

        //[TestMethod]
        //public void ZzInt_DeleteParticipant_Implicite_MoreCentre_noother_implicite() {

        //    using (IWebDriver driver = GetWebDriver(0)) {

        //        Assert.Fail();
        //    }
        //}

        //[TestMethod]
        //public void ZzInt_DeleteParticipant_Implicite_MoreCentre_other_implicite() {

        //    using (IWebDriver driver = GetWebDriver(0)) {

        //        Assert.Fail();
        //    }
        //}

        //[TestMethod]
        //public void ZzInt_AllRequestorsAreForAllPginCg_DeleteAllImpliciteReq() {

        //    using (IWebDriver driver = GetWebDriver(0)) {

        //        Assert.Fail();
        //    }
        //}
        #endregion

        #endregion

        #region Methods
        private void AddNewCg(IWebDriver driver, string cgName) {
            //there are some troubles with NewCg, sometimes Error message box is diplayed - displayErrorMsg from reget-base.ts
            //this is kind of workaround
            string url = AppRootUrl;
            driver.Url = url;
            WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
            Thread.Sleep(2000);

            url = AppRootUrl + "RegetAdmin/NewCentreGroup";
            driver.Url = url;
            
            //Act
            webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
            Thread.Sleep(1000);

            //IWebElement txtCgName = webDriverWait.Until(c => c.FindElement(By.Id("txtCgName")));
            IWebElement txtCgName = webDriverWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtCgName")));
            txtCgName.SendKeys(cgName);
            Thread.Sleep(2000);

            //office
            IWebElement cmbCgOffice = webDriverWait.Until(c => c.FindElement(By.Id("cmbCgOffice")));
            cmbCgOffice.Click();
            cmbCgOffice = webDriverWait.Until(c => c.FindElement(By.Id("cmbCgOffice")));
            string ariaOwnsOffice = cmbCgOffice.GetAttribute("aria-owns");

            //IWebElement scOffice = webDriverWait.Until(c => c.FindElement(By.Id(ariaOwnsOffice)));
            IWebElement scOffice = webDriverWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id(ariaOwnsOffice)));
            Thread.Sleep(2000);
            ReadOnlyCollection<IWebElement> officeOptions = webDriverWait.Until(c => scOffice.FindElements(By.TagName("md-option")));
            //ReadOnlyCollection<IWebElement> officeOptions = FindElementsByTagName(driver, "md-option");
            IWebElement firstOption = webDriverWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id(officeOptions[0].GetAttribute("id"))));
            firstOption.Click();
            Thread.Sleep(2000);

            //currency
            IWebElement cmbCgCurrency = webDriverWait.Until(c => c.FindElement(By.Id("cmbCgCurrency")));
            cmbCgCurrency.Click();
            cmbCgCurrency = webDriverWait.Until(c => c.FindElement(By.Id("cmbCgCurrency")));
            string ariaOwnsCurrency = cmbCgCurrency.GetAttribute("aria-owns");

            //IWebElement scCurrency = webDriverWait.Until(c => c.FindElement(By.Id(ariaOwnsCurrency)));
            IWebElement scCurrency = webDriverWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id(ariaOwnsCurrency)));
            Thread.Sleep(2000);
            ReadOnlyCollection<IWebElement> currencyOptions = webDriverWait.Until(c => scCurrency.FindElements(By.TagName("md-option")));
            firstOption = webDriverWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id(currencyOptions[0].GetAttribute("id"))));
            firstOption.Click();
            Thread.Sleep(1500);

            IWebElement btnSaveCgDetails = webDriverWait.Until(c => c.FindElement(By.Id("btnSaveCgDetails")));
            btnSaveCgDetails.Click();
            Thread.Sleep(5000);
        }

        private void CreatePg(
            IWebDriver driver,
            WebDriverWait webDriverWait,
            int companyId,
            string strPgName,
            string strBottomLimit,
            bool isBottomMultiply,
            string strTopLimit,
            bool isTopMultiply) {

            IWebElement imgCgExpand = FindElementById(webDriverWait, "imgCgExpand");
            //imgCgExpand.Click();
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].click();", imgCgExpand);

            //WebElement element = driver.findElement(By.xpath(Login_Btn));
            //JavascriptExecutor executor = (JavascriptExecutor)driver;
            //executor.executeScript("arguments[0].click();", element);

            Thread.Sleep(2000);

            var btnAddNewPg = webDriverWait.Until(c => c.FindElement(By.Id("btnAddNewPg")));
            btnAddNewPg.Click();

            var cmbParentPg = FindElementById(webDriverWait, "cmbParentPg");
            cmbParentPg.Click();
            Thread.Sleep(3000);

            ReadOnlyCollection<IWebElement> options = FindCmbOptionsElements(driver, webDriverWait, "cmbParentPg");
            //options = FindElementsByTagName(driver, "md-option");
            options[0].Click();
            Thread.Sleep(1000);

            var txtPgName = FindElementById(webDriverWait, "txtPgName");
            txtPgName.Clear();
            txtPgName.SendKeys(strPgName);

            var btnAddLimit = FindElementById(webDriverWait, "btnAddLimit");
            btnAddLimit.Click();

            string strIndex = null;
            var inputs = FindElementsByTagName(driver, "input");
            foreach (var input in inputs) {
                if (input.GetAttribute("id").StartsWith("txtLimitBottom_-")) {
                    strIndex = input.GetAttribute("id").Substring("txtLimitBottom_-".Length);
                    break;
                }
            }

            var txtLimitBottom = FindElementById(webDriverWait, "txtLimitBottom_-" + strIndex);
            txtLimitBottom.SendKeys(strBottomLimit);

            var txtLimitTop = FindElementById(webDriverWait, "txtLimitTop_-" + strIndex);
            txtLimitTop.SendKeys(strTopLimit);

            var btnAddManager = FindElementById(webDriverWait, "btnAddManager_-" + strIndex);
            btnAddManager.Click();
            Thread.Sleep(1000);

            if (isBottomMultiply) {
                ReadOnlyCollection<IWebElement> mdCheckboxes = driver.FindElements(By.TagName("md-checkbox"));
                foreach (var mdCheckbox in mdCheckboxes) {
                    if (mdCheckbox.GetAttribute("id").StartsWith("ckbLimitBottomMulti_")) {
                        mdCheckbox.Click();
                        break;
                    }
                }

            }

            if (isTopMultiply) {
                ReadOnlyCollection<IWebElement> mdCheckboxes = driver.FindElements(By.TagName("md-checkbox"));
                foreach (var mdCheckbox in mdCheckboxes) {
                    if (mdCheckbox.GetAttribute("id").StartsWith("ckbLimitTopMulti_")) {
                        mdCheckbox.Click();
                        break;
                    }
                }

            }

            List<Participants> appMen = new UserRepository().GetActiveParticipantInRoleByCompanyId(companyId, (int)UserRole.ApprovalManager);

            var txtAppManAutoCompl = FindElementById(webDriverWait, "txtAppManAutoCompl_-" + strIndex);
            txtAppManAutoCompl.SendKeys(appMen.ElementAt(0).surname);
            Thread.Sleep(3000);

            var li = FindautoCompleteOptionsElements(driver, webDriverWait, "txtAppManAutoCompl_-" + strIndex);
            js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].click();", li[0]);
            Thread.Sleep(3000);

            var btnAddRequestor = FindElementById(webDriverWait, "btnAddRequestor");
            btnAddRequestor.Click();

            string strId = null;
            var autocompletes = FindElementsByTagName(driver, "input");
            foreach (var reqAutoCompl in autocompletes) {
                strId = reqAutoCompl.GetAttribute("id");
                if (strId.StartsWith("txtAutoRequestor_")) {
                    break;
                }
            }

            List<Participants> requestors = new UserRepository().GetActiveParticipantInRoleByCompanyId(companyId, (int)UserRole.Requestor);
            var txtRequestor = FindElementById(webDriverWait, strId);
            if (appMen.ElementAt(0).id == requestors.ElementAt(0).id) {
                txtRequestor.SendKeys(requestors.ElementAt(4).surname);
            } else {
                txtRequestor.SendKeys(requestors.ElementAt(0).surname);
            }
            Thread.Sleep(3000);
            li = FindautoCompleteOptionsElements(driver, webDriverWait, strId);
            js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].click();", li[0]);
            Thread.Sleep(3000);

            //orderers
            var btnAddOrderer = FindElementById(webDriverWait, "btnAddOrderer");
            btnAddOrderer.Click();
            List<Participants> orderers = new UserRepository().GetActiveParticipantInRoleByCompanyId(companyId, (int)UserRole.Orderer);
            var txtOrderer = FindElementById(webDriverWait, "txtAutoOrderer");
            int ordId = -1;
            if (appMen.ElementAt(0).id == orderers.ElementAt(0).id) {
                txtOrderer.SendKeys(orderers.ElementAt(1).surname);
                ordId = orderers.ElementAt(1).id;
            } else {
                txtOrderer.SendKeys(orderers.ElementAt(0).surname);
                ordId = orderers.ElementAt(0).id;
            }
            Thread.Sleep(3000);
            li = FindautoCompleteOptionsElements(driver, webDriverWait, "txtAutoOrderer");
            js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].click();", li[0]);

            IWebElement divOrdChip = null;
            try {
                divOrdChip = FindElementById(webDriverWait, "divOrd_" + ordId);
            } catch { }

            if (divOrdChip == null) {
                txtOrderer = FindElementById(webDriverWait, "txtAutoOrderer");
                ordId = -1;
                if (appMen.ElementAt(0).id == orderers.ElementAt(0).id) {
                    txtOrderer.SendKeys(orderers.ElementAt(1).surname);
                    ordId = orderers.ElementAt(1).id;
                } else {
                    txtOrderer.SendKeys(orderers.ElementAt(0).surname);
                    ordId = orderers.ElementAt(0).id;
                }
                Thread.Sleep(3000);
                li = FindautoCompleteOptionsElements(driver, webDriverWait, "txtAutoOrderer");
                js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("arguments[0].click();", li[0]);
            }

            var btnSave = FindElementById(webDriverWait, "btnSave");
            btnSave.Click();
            Thread.Sleep(10000);
        }

        private int DisplayCgSettings(IWebDriver driver) {
            string url = AppRootUrl + "RegetAdmin";
            driver.Url = url;
            WaitUntilLoadDialogIsClosed(driver);

            Thread.Sleep(1000);
            WebDriverWait webDriverWait;
            webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
            IWebElement cmbCg = webDriverWait.Until(c => c.FindElement(By.Id("cmbCgList")));
            cmbCg.Click();
            Thread.Sleep(500);

            var options = webDriverWait.Until(c => c.FindElements(By.TagName("md-option")));

            int optIndex = 0;
            string strCgId = null;
            while (strCgId == null) {
                optIndex = new Random().Next(0, options.Count - 1);
                strCgId = options[optIndex].GetAttribute("value");
            }

            //string strCgId = options[0].GetAttribute("value");
            int cgId = Convert.ToInt32(strCgId);
            options[optIndex].Click();
            Thread.Sleep(3000);

            return cgId;
        }

        private void DisplayCgSettings(IWebDriver driver, int selCgId) {
            string url = AppRootUrl + "RegetAdmin";
            driver.Url = url;
            WaitUntilLoadDialogIsClosed(driver);

            Thread.Sleep(1000);
            WebDriverWait webDriverWait;
            webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
            IWebElement cmbCg = webDriverWait.Until(c => c.FindElement(By.Id("cmbCgList")));
            cmbCg.Click();
            Thread.Sleep(500);

            var options = webDriverWait.Until(c => c.FindElements(By.TagName("md-option")));
            foreach (var option in options) {
                string strCgId = option.GetAttribute("value");
                if (strCgId != null) {
                    int iCgId = Convert.ToInt32(strCgId);
                    if (selCgId == iCgId) {
                        option.Click();
                        Thread.Sleep(3000);
                        return;
                    }
                }
            }

            
            

            
        }

        private void GetAppLimits(
            IWebDriver driver,
            WebDriverWait webDriverWait,
            string pgName,
            out decimal bottomLimit,
            out decimal topLimit,
            out bool isBottomMulti,
            out bool isTopMulti) {

            bottomLimit = DataNulls.DECIMAL_NULL;
            topLimit = DataNulls.DECIMAL_NULL;
            isBottomMulti = false;
            isTopMulti = false;

            bool isPgExist = false;
            ReadOnlyCollection<IWebElement> pgHeaders = driver.FindElements(By.ClassName("reget-box-header"));
            IWebElement pgParentHeader = null;
            IWebElement pgLabelHeader = null;
            if (pgHeaders != null) {
                foreach (var pgHeader in pgHeaders) {
                    ReadOnlyCollection<IWebElement> divs = pgHeader.FindElements(By.TagName("div"));
                    if (divs != null) {
                        foreach (var div in divs) {
                            string divHtml = div.GetAttribute("innerHTML");
                            if (divHtml == pgName) {
                                isPgExist = true;

                                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                                pgLabelHeader = pgHeader;
                                pgParentHeader = pgHeader.FindElement(By.XPath(".."));

                                break;
                            }
                        }

                        if (isPgExist) {
                            break;
                        }
                    }
                }

                ReadOnlyCollection<IWebElement> imgs = pgLabelHeader.FindElements(By.TagName("img"));
                if (imgs != null) {
                    foreach (var img in imgs) {
                        if (img.GetProperty("id").StartsWith("impAppMatrixCollExp")) {
                            if (img.GetProperty("src").Contains("Expand.png")) {
                                img.Click();
                                Thread.Sleep(2000);
                                break;
                            }
                            break;
                        }
                    }
                }

                ReadOnlyCollection<IWebElement> tables = pgParentHeader.FindElements(By.TagName("table"));
                ReadOnlyCollection<IWebElement> tbodys = tables[1].FindElements(By.TagName("tbody"));
                ReadOnlyCollection<IWebElement> trs = tbodys[0].FindElements(By.TagName("tr"));
                ReadOnlyCollection<IWebElement> tds = trs[0].FindElements(By.TagName("td"));
                string strModBottomLimit = tds[0].Text.Replace("\r\n", "").Replace("*", "");
                string strModTopLimit = tds[1].Text.Replace("\r\n", "").Replace("*", "");

                isBottomMulti = tds[0].Text.Contains("*");
                isTopMulti = tds[1].Text.Contains("*");

                bottomLimit = ConvertData.ToDecimal(strModBottomLimit);
                topLimit = ConvertData.ToDecimal(strModTopLimit);
            }
        }

        private void MultiplyLimits(
            IWebDriver driver,
            WebDriverWait webDriverWait,
            string strMultipl) {
            string url = AppRootUrl + "RegetAdmin/MultiplyAppLevel";
            driver.Url = url;
            WaitUntilLoadDialogIsClosed(driver);

            IWebElement txtMultiFactor = FindElementById(webDriverWait, "txtMultiFactor");
            txtMultiFactor.SendKeys(strMultipl);
            IWebElement ckbSelectAll = FindElementById(webDriverWait, "ckbSelectAll");
            ckbSelectAll.Click();

            IWebElement btnCalculate = FindElementById(webDriverWait, "btnCalculate");
            btnCalculate.Click();

            Thread.Sleep(2000);

            IWebElement btnOk = webDriverWait.Until(c => c.FindElement(By.Id("btnPerform")));
            btnOk.Click();

            Thread.Sleep(3000);
        }

        private IWebElement GetPgDiv(
            IWebDriver driver,
            string pgName,
            out int pgId) {

            bool isPgExist = false;
            pgId = DataNulls.INT_NULL;
            IWebElement pgParentHeader = null;
            ReadOnlyCollection<IWebElement> pgHeaders = driver.FindElements(By.ClassName("reget-box-header"));
            if (pgHeaders != null) {
                foreach (var pgHeader in pgHeaders) {
                    ReadOnlyCollection<IWebElement> divs = pgHeader.FindElements(By.TagName("div"));
                    if (divs != null) {
                        foreach (var div in divs) {
                            if (div.Text == pgName) {
                                isPgExist = true;
                                pgParentHeader = pgHeader.FindElement(By.XPath(".."));
                                string divParId = pgParentHeader.GetProperty("id");
                                string[] divParIdParts = divParId.Split('_');
                                pgId = Convert.ToInt16(divParIdParts[1]);
                                break;
                            }
                        }

                        if (isPgExist) {
                            break;
                        }
                    }
                }
            }

            return pgParentHeader;
        }

        private void SetPgLimitProperties(
            IWebDriver driver,
            WebDriverWait webDriverWait,
            string pgName,
            string strBottomLimit,
            string strTopLimit,
            bool isBottomMulti,
            bool isTopMulti) {

            int pgId = DataNulls.INT_NULL;
            IWebElement fixHeader = GetPgDiv(driver, pgName, out pgId);

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", fixHeader);
            Thread.Sleep(1000);

            //var pgParentFixHeader = fixHeader.FindElement(By.XPath(".."));
            ReadOnlyCollection<IWebElement> imgs = fixHeader.FindElements(By.TagName("img"));
            if (imgs != null) {
                foreach (var img in imgs) {
                    if (img.GetProperty("id").StartsWith("impAppMatrixCollExp")) {
                        if (img.GetProperty("src").Contains("Expand.png")) {
                            img.Click();
                            Thread.Sleep(2000);
                            break;
                        }
                    }
                }
            }
            IReadOnlyCollection<IWebElement> btnEdits = fixHeader.FindElements(By.ClassName("reget-btn-edit"));
            btnEdits.ElementAt(0).Click();
            Thread.Sleep(3000);

            IReadOnlyCollection<IWebElement> inputs = FindElementsByTagName(driver, "input");
            bool isBottomSet = false;
            bool isTopSet = false;
            foreach (var input in inputs) {
                if (input.GetProperty("id").StartsWith("txtLimitBottom_")) {
                    input.Clear();
                    input.SendKeys(strBottomLimit);
                    isBottomSet = true;
                }
                if (input.GetProperty("id").StartsWith("txtLimitTop_")) {
                    input.Clear();
                    input.SendKeys(strTopLimit);
                    isTopSet = true;
                }

                if (isBottomSet && isTopSet) {
                    break;
                }
            }

            IReadOnlyCollection<IWebElement> ckbs = FindElementsByTagName(driver, "md-checkbox");
            bool isBottomMultiSet = false;
            bool isTopMultiSet = false;
            foreach (var ckb in ckbs) {
                if (ckb.GetProperty("id").StartsWith("ckbLimitBottomMulti_")) {
                    var strIsChecked = ckb.GetAttribute("aria-checked");
                    if (isBottomMulti && strIsChecked == "false") {
                        ckb.Click();
                    }
                    if (!isBottomMulti && strIsChecked == "true") {
                        ckb.Click();
                    }
                    isBottomMultiSet = true;
                }
                if (ckb.GetProperty("id").StartsWith("ckbLimitTopMulti_")) {
                    var strIsChecked = ckb.GetAttribute("aria-checked");
                    if (isTopMulti && strIsChecked == "false") {
                        ckb.Click();
                    }
                    if (!isTopMulti && strIsChecked == "true") {
                        ckb.Click();
                    }
                    isTopMultiSet = true;
                }

                if (isBottomMultiSet && isTopMultiSet) {
                    break;
                }
            }


            var btnSave = FindElementById(webDriverWait, "btnSave");
            btnSave.Click();
            Thread.Sleep(3000);
        }

        private void DeletePgMultiAppLevel() {
            PgRepository pgRep = new PgRepository();
            var pgs = pgRep.GetPgsByName("Pg Multi App Level");
            CentreGroupRepository cgr = new CentreGroupRepository();
            foreach (var pg in pgs) {
                cgr.DeletePurchaseGroup(pg.id, pg.Centre_Group.ElementAt(0).id);
            }
        }
        #endregion

    }
}
