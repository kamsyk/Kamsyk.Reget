using Kamsyk.Reget.Model;
using Kamsyk.Reget.Model.Repositories;
using Kamsyk.Reget.TestsIntegration.BaseTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kamsyk.Reget.TestsIntegration.Controllers {
    [TestClass]
    public class SupplierControllerTest : BaseTestIntegration {
        #region Constructor
        public SupplierControllerTest() {
            if (!IsTestDbConnected) {
                throw new Exception("Test DB is not connected");
            }
        }
        #endregion

        #region Test Methods
        [TestMethod]
        public void ZzInt_SupplierMaintDisabled_AllowAndModifysupplier() {

            using (IWebDriver driver = GetWebDriver(0, "cs-CZ", new System.Drawing.Size(2700, 850))) {
                //Arange
                SetCzCompanyMaintSupplier(true);

                DataGridRepository dataGridRepository = new DataGridRepository();
                var userGridSettings = dataGridRepository.GetUserGridSettings(0, "grdSupplier_rg");
                if (userGridSettings != null) {
                    dataGridRepository.DeleteUserGridSettings(userGridSettings);
                }

                SetSupplierAsNonActive(0, "25065700");
                bool isActive = IsSupplierActive(0, "25065700");
                if (isActive) {
                    Assert.Fail();
                    return;
                }

                SetCzCompanyMaintSupplier(false);

                string url = AppRootUrl + "Supplier";
                driver.Url = url;

                WebDriverWait webDriverWait;
                webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                IWebElement cmbCg = webDriverWait.Until(c => c.FindElement(By.Id("cmbSupplierList")));
                Thread.Sleep(1000);

                //Act
                cmbCg.Click();
                Thread.Sleep(1500);

                var options = webDriverWait.Until(c => c.FindElements(By.TagName("md-option")));
                foreach (var option in options) {
                    var optText = option.FindElement(By.ClassName("md-text"));
                    if (optText.Text.Contains("a.s.") || optText.Text.Contains("Prodejní")) {
                        option.Click();
                        break;
                    }
                }

                //webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                //IWebElement select_option_5 = webDriverWait.Until(c => c.FindElement(By.Id("select_option_6")));
                Thread.Sleep(1500);
                //select_option_5.Click();

                //set manual maintenance
                webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                IWebElement ckbManualMaint = webDriverWait.Until(c => c.FindElement(By.Id("ckbManualMaint")));
                var strIsChecked = ckbManualMaint.GetAttribute("aria-checked");
                if (strIsChecked != "false") {
                    Assert.Fail();
                }
                Thread.Sleep(1500);
                ckbManualMaint.Click();
                webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                IWebElement grdSupplier_btnNewRow = webDriverWait.Until(c => c.FindElement(By.Id("grdSupplier_btnNewRow")));
                Thread.Sleep(1000);
                               
                webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                IWebElement grdSupplier = webDriverWait.Until(c => c.FindElement(By.Id("grdSupplier")));
                string gridGenId = GetGridId(grdSupplier);

                webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                IWebElement grdRowCellActive = webDriverWait.Until(c => c.FindElement(By.Id(gridGenId + "-3-uiGrid-000H-cell")));
                grdRowCellActive.Click();

                IWebElement ckbActive = grdRowCellActive.FindElement(By.TagName("md-checkbox"));
                ckbActive.Click();

                webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                IWebElement grdRowCellAction = webDriverWait.Until(c => c.FindElement(By.Id(gridGenId + "-3-uiGrid-0007-cell")));

                var actionButtons = grdRowCellAction.FindElements(By.TagName("button"));
                Thread.Sleep(3000);
                actionButtons[0].Click();

                ////this is neccessary otherwise save button does not work
                //var alert = driver.SwitchTo().Alert();
                //Thread.Sleep(1500);
                //alert.Accept();

                
                Thread.Sleep(1000);

                isActive = IsSupplierActive(0, "25065700");
                

                //Assert
                Assert.IsTrue(isActive);
            }
        }

        [TestMethod]
        public void ZzInt_SupplierFiltered_ModifySupplier() {

            using (IWebDriver driver = GetWebDriver(0, "cs-CZ", new System.Drawing.Size(2700, 850))) {
                try {
                    //Arange

                    SetCzCompanyMaintSupplier(true);

                    SetSupplierAsNonActive(0, "00020478");
                    bool isActive = IsSupplierActive(0, "00020478");
                    if (isActive) {
                        Assert.Fail();
                        return;
                    }



                    string url = AppRootUrl + "Supplier";
                    driver.Url = url;

                    WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));

                    SetSupplierCompany(driver);
                    //IWebElement cmbCg = webDriverWait.Until(c => c.FindElement(By.Id("cmbSupplierList")));
                    //Thread.Sleep(2000);

                    ////Act
                    ////cmbCg.Click();
                    //////webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                    //////IWebElement select_option_5 = webDriverWait.Until(c => c.FindElement(By.Id("select_option_5")));
                    //////Thread.Sleep(1500);
                    //////select_option_5.Click();
                    ////var options = webDriverWait.Until(c => c.FindElements(By.TagName("md-option")));
                    ////foreach (var option in options) {
                    ////    var optText = option.FindElement(By.ClassName("md-text"));
                    ////    if (optText.Text.Contains("a.s.") || optText.Text.Contains("Prodejní")) {
                    ////        option.Click();
                    ////        break;
                    ////    }
                    ////}

                    //cmbCg.Click();
                    //Thread.Sleep(1500);

                    //var options = webDriverWait.Until(c => c.FindElements(By.TagName("md-option")));
                    //foreach (var option in options) {
                    //    var optText = option.FindElement(By.ClassName("md-text"));
                    //    if (optText.Text.Contains("a.s.") || optText.Text.Contains("Prodejní")) {
                    //        option.Click();
                    //        break;
                    //    }
                    //}


                    webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                    IWebElement grdSupplier = webDriverWait.Until(c => c.FindElement(By.Id("grdSupplier")));
                    string gridGenId = GetGridId(grdSupplier);


                    webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                    IWebElement filterHeaderSuppId = webDriverWait.Until(c => c.FindElement(By.ClassName("ui-grid-coluiGrid-0009")));

                    webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                    var filterInputs = webDriverWait.Until(c => filterHeaderSuppId.FindElements(By.TagName("input")));
                    //IWebElement inputFilter = webDriverWait.Until(c => filterHeaderSuppId.FindElement(By.CssSelector("ui-grid-filter-input ui-grid-filter-input-0")));
                    //filterInputs[0].SendKeys("00020478");

                    string[] strKeys = new string[] { "0", "0", "0", "2", "0", "4", "7", "8" };
                    foreach (string str in strKeys) {
                        filterInputs[0].SendKeys(str);
                        Thread.Sleep(500);
                    }



                    Thread.Sleep(2000);

                    webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                    IWebElement grdRowCellActive = webDriverWait.Until(c => c.FindElement(By.Id(gridGenId + "-0-uiGrid-000H-cell")));
                    grdRowCellActive.Click();

                    Thread.Sleep(1000);

                    IWebElement ckbActive = grdRowCellActive.FindElement(By.TagName("md-checkbox"));
                    ckbActive.Click();

                    webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                    IWebElement grdRowCellAction = webDriverWait.Until(c => c.FindElement(By.Id(gridGenId + "-0-uiGrid-0007-cell")));

                    var actionButtons = grdRowCellAction.FindElements(By.TagName("button"));
                    Thread.Sleep(3000);
                    actionButtons[0].Click();


                    Thread.Sleep(1000);

                    isActive = IsSupplierActive(0, "00020478");

                    if (!isActive) {
                        new TestFailRepository().SaveTestFail("ZzInt_SupplierFiltered_ModifySupplier", "NotPassed");
                    }

                    //Assert
                    Assert.IsTrue(isActive);
                } catch (Exception ex) {
                    new TestFailRepository().SaveTestFail("ZzInt_SupplierFiltered_ModifySupplier", ex.ToString());
                    throw ex;
                }
            }
        }

        [TestMethod]
        public void ZzInt_SupplierNotAllowedUserManualEdit_ReadOnly() {

            using (IWebDriver driver = GetWebDriver(11, "cs-CZ")) {
                try {
                    //Arange
                    SetCzCompanyMaintSupplier(true);
                    string url = AppRootUrl + "Supplier";
                    driver.Url = url;

                    //Act
                    WebDriverWait webDriverWait;
                    webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                    string grdId = GetGridId(webDriverWait);

                    //the user should have only 1 company so that company is selected automatically, no drop down is displayed

                    string firstCellId = grdId + "-0-uiGrid-0008-cell";
                    IWebElement firstCell = webDriverWait.Until(c => c.FindElement(By.Id(firstCellId)));
                    //firstCell.Click();

                    Actions actions = new Actions(driver);
                    actions.MoveToElement(firstCell).Click().Perform();
                    Thread.Sleep(1000);

                    //Assert
                    var inputs = firstCell.FindElements(By.TagName("input"));
                    Assert.IsTrue(inputs == null || inputs.Count == 0);
                } catch {
                    Assert.Fail();
                } finally {
                    new UserRepository().RevertDefaultTestUser(DefaultUserName, "pafr");
                }
            }
        }

        [TestMethod]
        public void ZzInt_SupplierDeactivate_Import_Activate() {
            using (IWebDriver driver = GetWebDriver(0, "cs-CZ")) {
                //Arange
                SetCzCompanyMaintSupplier(false);
                
                //Act
                string url = AppRootUrl + "Supplier";
                driver.Url = url;

                SetSupplierCompany(driver);

                Thread.Sleep(2000);

                WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                //IWebElement ckbManualMaint = webDriverWait.Until(c => c.FindElement(By.Id("ckbManualMaint")));
                //IWebElement mdIcon = ckbManualMaint.FindElement(By.ClassName("md-icon"));

                //mdIcon.Click();

                //IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                
                IWebElement ckbManualMaint = webDriverWait.Until(c => c.FindElement(By.Id("ckbManualMaint")));
                Thread.Sleep(1000);
                ckbManualMaint.Click();

                Thread.Sleep(2000);

                //Assert
                bool isSupplierManualMaint = IsSupplierManualMaintenceAllowed();
                Assert.IsTrue(isSupplierManualMaint);
            }
        }

        [TestMethod]
        public void ZzInt_SupplierActive_Import_Deactivate() {

            using (IWebDriver driver = GetWebDriver(0, "cs-CZ")) {
                //Arange
                SetCzCompanyMaintSupplier(true);

                //Act
                string url = AppRootUrl + "Supplier";
                driver.Url = url;

                SetSupplierCompany(driver);

                Thread.Sleep(2000);

                WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                //IWebElement ckbManualMaint = webDriverWait.Until(c => c.FindElement(By.Id("ckbManualMaint")));
                //IWebElement mdIcon = ckbManualMaint.FindElement(By.ClassName("md-icon"));

                //mdIcon.Click();

                //IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

                IWebElement ckbManualMaint = webDriverWait.Until(c => c.FindElement(By.Id("ckbManualMaint")));
                Thread.Sleep(1000);
                ckbManualMaint.Click();

                IWebElement btnCancel = webDriverWait.Until(c => c.FindElement(By.Id("btnPerform")));
                Thread.Sleep(1000);
                btnCancel.Click();
                                
                //var alert = driver.SwitchTo().Alert();
                //Thread.Sleep(1500);
                //alert.Accept();

                Thread.Sleep(2000);

                //Assert
                bool isSupplierManualMaint = IsSupplierManualMaintenceAllowed();
                Assert.IsFalse(isSupplierManualMaint);
            }
        }

        [TestMethod]
        public void ZzInt_Supplier_UserNotAllowdToSee() {

            using (IWebDriver driver = GetWebDriver(0, "cs-CZ")) {
                //Arange
                
                //Act
                
                //Assert
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ZzInt_SupplierDetail_UserNotAllowdToSee() {

            using (IWebDriver driver = GetWebDriver(0, "cs-CZ")) {
                //Arange

                //Act

                //Assert
                Assert.Fail();
            }
        }
        #endregion

        #region Methods
        private void SetCzCompanyMaintSupplier(bool isAllowed) {
            string errMsg;
            new CompanyRepository().SaveSuppMaintenance(0, isAllowed, 0, out errMsg);
        }

        private void SetSupplierAsNonActive(int companyId, string supplierId) {
            SupplierRepository supplierRepository = new SupplierRepository();

            Supplier supplier = supplierRepository.GetSupplierDataBySuppId(companyId, supplierId);
            supplier.active = false;

            List<string> msg = null;
            supplierRepository.SaveSupplier(supplier, 0, companyId, true, out msg);
            
        }

        private bool IsSupplierActive(int companyId, string supplierId) {
            SupplierRepository supplierRepository = new SupplierRepository();

            Supplier supplier = supplierRepository.GetSupplierDataBySuppId(companyId, supplierId);
            return (supplier.active == true);

            
        }

        private void SetSupplierCompany(IWebDriver driver) {
            WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
            IWebElement cmbCg = webDriverWait.Until(c => c.FindElement(By.Id("cmbSupplierList")));
            Thread.Sleep(2000);

            
            cmbCg.Click();
            Thread.Sleep(1500);

            var options = webDriverWait.Until(c => c.FindElements(By.TagName("md-option")));
            foreach (var option in options) {
                var optText = option.FindElement(By.ClassName("md-text"));
                if (optText.Text.Contains("a.s.") || optText.Text.Contains("Prodejní")) {
                    option.Click();
                    break;
                }
            }
        }

        private bool IsSupplierManualMaintenceAllowed() {
            return new CompanyRepository().IsSupplierMaintenanceAllowed(0);
        }

        //private void SetUser0Compa0Amin() {
        //    string errMsg;
        //    new CompanyRepository().SaveSuppMaintenance(0, false, 0, out errMsg);
        //}
        #endregion
    }
}
