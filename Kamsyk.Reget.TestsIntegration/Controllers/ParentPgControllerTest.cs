using Kamsyk.Reget.Model;
using Kamsyk.Reget.Model.Repositories;
using Kamsyk.Reget.TestsIntegration.BaseTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Kamsyk.Reget.Model.Repositories.UserRepository;

namespace Kamsyk.Reget.TestsIntegration.Controllers {
    [TestClass]
    public class ParentPgControllerTest : BaseTestIntegration{
        #region Constants
        private const string NEW_ITEM_TEXT = "NewItem";
        #endregion

        #region Test Methods
        [TestMethod]
        public void ZzInt_ParentPg_AddDelete() {
            try {
                //Arrange
                ParentPgRepository parentPgRepository = new ParentPgRepository();
                parentPgRepository.DeleteParentPgByName(NEW_ITEM_TEXT);
                var ppg = parentPgRepository.GetParentPgByName(NEW_ITEM_TEXT);
                if (ppg != null) {
                    throw new Exception("Pg was not cleared");
                }
                               
                //Act
                using (IWebDriver driver = GetWebDriver(0, "en-US")) {
                    string url = AppRootUrl + "ParentPg";
                    driver.Url = url;
                    var webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                    new BaseDbGridTestIntegration().AddNewRecord(driver, BaseDbGridTestIntegration.NewRowCheckboxValueType.All);
                    ppg = parentPgRepository.GetParentPgByName(NEW_ITEM_TEXT);
                    if (ppg == null) {
                        throw new Exception("Pg was not created");
                    }

                    DeleteParentPg(driver, false);
                    var pErrMsg = FindElementById(webDriverWait, "pErrMsg");
                    if (pErrMsg.GetAttribute("innerHTML").IndexOf("Parent Purchase Group is used and cannot be deactivated") > -1) {
                        var btnErrClose = FindElementById(webDriverWait, "btnErrClose");
                        btnErrClose.Click();
                        Thread.Sleep(1000);

                        ReadOnlyCollection<IWebElement> rows = driver.FindElements(By.ClassName("ui-grid-row"));
                        IWebElement lastRow = rows[rows.Count - 1];
                        IWebElement btnEdit = lastRow.FindElement(By.ClassName("reget-btn-grid-edit"));
                        //btnEdit.Click();
                        IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                        js = (IJavaScriptExecutor)driver;
                        js.ExecuteScript("arguments[0].click();", btnEdit);
                        Thread.Sleep(1000);

                        SetEditRowCkb(driver, webDriverWait, lastRow);
                        IWebElement btnSave = driver.FindElement(By.ClassName("reget-btn-save"));
                        js = (IJavaScriptExecutor)driver;
                        js = (IJavaScriptExecutor)driver;
                        js.ExecuteScript("arguments[0].click();", btnSave);
                        Thread.Sleep(2000);
                        
                        DeleteParentPg(driver, true);
                    }
                }

                //Assert
                ppg = parentPgRepository.GetParentPgByName(NEW_ITEM_TEXT);
                Assert.IsTrue(ppg == null);

            } catch (Exception ex) {
                throw ex;
            }
        }

        [TestMethod]
        public void ZzInt_ParentPg_NotDisplayedCompany_AddDelete() {

            //Arrange
            ParentPgRepository parentPgRepository = new ParentPgRepository();
            parentPgRepository.DeleteParentPgByName(NEW_ITEM_TEXT);

            int userId = 0;
            int testCompId = -1;
            UserRepository userRepository = new UserRepository();
            var user = userRepository.GetParticipantById(userId);
            CompanyRepository companyRepository = new CompanyRepository();
            var companies = companyRepository.GetActiveCompanies();
            foreach (var comp in companies) {
                var adminRole = (from useRoleDb in user.Participant_Office_Role
                             where useRoleDb.office_id == comp.id &&
                             useRoleDb.role_id == (int)UserRole.OfficeAdministrator
                             select useRoleDb).FirstOrDefault();

                if (adminRole == null) {
                    testCompId = comp.id;
                    userRepository.AddUserOfficeRole(userId, testCompId, UserRole.OfficeAdministrator);
                    break;
                }
            }
            if (testCompId == -1) {
                testCompId = companies.ElementAt(companies.Count - 1).id;
            }

            //Act
            using (IWebDriver driver = GetWebDriver(userId, "en-US")) {
                string url = AppRootUrl + "ParentPg";
                driver.Url = url;
                var webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                new BaseDbGridTestIntegration().AddNewRecord(driver, BaseDbGridTestIntegration.NewRowCheckboxValueType.All);
                var ppg = parentPgRepository.GetParentPgByName(NEW_ITEM_TEXT);
                if (ppg == null) {
                    throw new Exception("Pg was not created");
                }
                userRepository.RemoveUserOfficeRole(userId, testCompId, UserRole.OfficeAdministrator);

                driver.Navigate().Refresh();
                Thread.Sleep(2000);

                var filterName = driver.FindElement(By.ClassName("ui-grid-filter-input-0"));
                Thread.Sleep(1000);
                filterName.SendKeys(NEW_ITEM_TEXT);
                Thread.Sleep(1000);

                ReadOnlyCollection<IWebElement> rows = driver.FindElements(By.ClassName("ui-grid-row"));
                if (rows == null || rows.Count == 0) {
                    throw new Exception("No Rows");
                }
                IWebElement lastRow = rows[rows.Count - 1];

                IWebElement btnEdit = lastRow.FindElement(By.ClassName("reget-btn-grid-edit"));
                btnEdit.Click();
                Thread.Sleep(1000);

                new BaseDbGridTestIntegration().SetEditRow(driver, webDriverWait, lastRow, BaseDbGridTestIntegration.NewRowCheckboxValueType.None);
                IWebElement btnSave = lastRow.FindElement(By.ClassName("reget-btn-save"));
                btnSave.Click();
                Thread.Sleep(1000);

                IWebElement btnDel = lastRow.FindElement(By.ClassName("reget-btn-grid-delete"));
                btnDel.Click();
                Thread.Sleep(1000);

                IWebElement errImg = driver.FindElement(By.ClassName("reget-error-img"));
                if (errImg == null) {
                    Assert.Fail();
                }

                var btnErrClose = FindElementById(webDriverWait, "btnErrClose");
                btnErrClose.Click();
                Thread.Sleep(1000);

                userRepository.AddUserOfficeRole(userId, testCompId, UserRole.OfficeAdministrator);
                driver.Navigate().Refresh();
                Thread.Sleep(1000);

                filterName = driver.FindElement(By.ClassName("ui-grid-filter-input-0"));
                Thread.Sleep(1000);
                filterName.SendKeys(NEW_ITEM_TEXT);
                Thread.Sleep(1000);

                rows = driver.FindElements(By.ClassName("ui-grid-row"));
                if (rows == null || rows.Count == 0) {
                    throw new Exception("No Rows");
                }
                lastRow = rows[rows.Count - 1];

                btnEdit = lastRow.FindElement(By.ClassName("reget-btn-grid-edit"));
                btnEdit.Click();
                Thread.Sleep(1000);

                new BaseDbGridTestIntegration().SetEditRow(driver, webDriverWait, lastRow, BaseDbGridTestIntegration.NewRowCheckboxValueType.None);
                btnSave = lastRow.FindElement(By.ClassName("reget-btn-save"));
                btnSave.Click();
                Thread.Sleep(1000);

                btnDel = lastRow.FindElement(By.ClassName("reget-btn-grid-delete"));
                btnDel.Click();
                Thread.Sleep(1000);

                var btnYes = driver.FindElement(By.Id("btnPerform"));
                btnYes.Click();
                Thread.Sleep(1000);

                //Thread.Sleep(10000);

                //Assert
                ppg = parentPgRepository.GetParentPgByName(NEW_ITEM_TEXT);
                Assert.IsTrue(ppg == null);

            }
        }

        [TestMethod]
        public void ZzInt_ParentPg_NotCompanyAdmin_AccessCheck() {
            try {
                //Arrange
                UserRepository userRepository = new UserRepository();
                var participants = userRepository.GetActiveParticipantByCompanyId(0);
                int iIndex = new Random().Next(0, participants.Count - 1);
                Participants nonCompAdminPart = null;
                while (nonCompAdminPart == null) {
                    var part = participants.ElementAt(iIndex);
                    var adminRole = (from adminRoleDb in part.Participant_Office_Role
                                     where adminRoleDb.role_id == (int)UserRole.OfficeAdministrator
                                     select adminRoleDb).FirstOrDefault();
                    if (adminRole == null) {
                        nonCompAdminPart = part;
                        break;
                    }
                }

                //Act
                using (IWebDriver driver = GetWebDriver(nonCompAdminPart.id, "en-US")) {
                    string url = AppRootUrl;
                    driver.Url = url;
                    var webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));

                    bool isCgAdminAvailable = false;
                    bool isParentPgAdminAvailable = false;
                    bool isPgAdminAvailable = false;

                    var ahrefs = driver.FindElements(By.TagName("a"));
                    foreach (var ahref in ahrefs) {
                        string strHref = ahref.GetAttribute("href");
                        if (strHref.Contains("/RegetAdmin")) {
                            isCgAdminAvailable = true;
                        }
                        if (strHref.Contains("/ParentPg")) {
                            isParentPgAdminAvailable = true;
                        }
                        if (strHref.Contains("/ParentPg/UsedPg")) {
                            isPgAdminAvailable = true;
                        }
                    }

                    //Assert
                    Assert.IsTrue(isCgAdminAvailable && !isParentPgAdminAvailable && !isPgAdminAvailable);
                }
            } catch (Exception ex) {
                throw ex;
            } finally {
                SwitchUserToDefaultTestUser();
            }
        }

        [TestMethod]
        public void ZzInt_ParentPg_AddMissingLocalization() {
            try {
                //Arrange
                string strCulture = "ro-RO";
                ParentPgRepository parentPgRepository = new ParentPgRepository();
                parentPgRepository.DeletePpgLocData(strCulture);
                var ppgls = parentPgRepository.GetPpgLocData(strCulture);
                if (ppgls != null && ppgls.Count > 0) {
                    Assert.Fail();
                }

                //Act
                using (IWebDriver driver = GetWebDriver(0, strCulture)) {
                   
                    string url = AppRootUrl + "ParentPg";
                    driver.Url = url;
                    var webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                    
                    bool isLoadDialogDisplayed = true;
                    while (isLoadDialogDisplayed) {
                        try {
                            var loadDialog = driver.FindElement(By.ClassName("reget-se-pre-con"));
                            isLoadDialogDisplayed = loadDialog.Displayed;
                            Thread.Sleep(2000);
                        } catch (Exception ex) {
                            if (ex is NoSuchElementException) {
                                isLoadDialogDisplayed = false;
                            } else {
                                Assert.Fail();
                            }
                        }
                    }

                    //Assert
                    bool isErrorDisplayed = IsErrorDisplayed(driver);
                    if (IsErrorDisplayed(driver)) {
                        Assert.Fail();
                    } 

                }

                //Assert
                var ppgs = parentPgRepository.GetPpgData();
                ppgls = parentPgRepository.GetPpgLocData(strCulture);
                Assert.IsTrue(ppgs.Count == ppgls.Count);

            } catch (Exception ex) {
                throw ex;
            } finally {
                SwitchUserToDefaultTestUser();
            }
        }

        [TestMethod]
        public void ZzInt_Pg_AddMissingLocalization() {
            try {
                //Arrange
                string strCulture = "ro-RO";
                ParentPgRepository parentPgRepository = new ParentPgRepository();
                PgRepository pgRepository = new PgRepository();

                parentPgRepository.DeletePpgLocData(strCulture);
                var ppgls = parentPgRepository.GetPpgLocData(strCulture);
                if (ppgls != null && ppgls.Count > 0) {
                    Assert.Fail();
                }

                pgRepository.DeletePgLocData(strCulture);
                var pgls = pgRepository.GetPgLocData(strCulture);
                if (pgls != null && pgls.Count > 0) {
                    Assert.Fail();
                }

                //Act
                using (IWebDriver driver = GetWebDriver(0, strCulture)) {

                    string url = AppRootUrl + "ParentPg/UsedPg";
                    driver.Url = url;
                    var webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                    bool isLoadDialogDisplayed = true;
                    while (isLoadDialogDisplayed) {
                        try {
                            var loadDialog = driver.FindElement(By.ClassName("reget-se-pre-con"));
                            isLoadDialogDisplayed = loadDialog.Displayed;
                            Thread.Sleep(2000);
                        } catch (Exception ex) {
                            if (ex is NoSuchElementException) {
                                isLoadDialogDisplayed = false;
                            } else {
                                Assert.Fail();
                            }
                        }
                    }

                    //Assert
                    bool isErrorDisplayed = IsErrorDisplayed(driver);
                    if (IsErrorDisplayed(driver)) {
                        Assert.Fail();
                    }

                }

                //Assert
                var ppgs = parentPgRepository.GetPpgData();
                ppgls = parentPgRepository.GetPpgLocData(strCulture);

                var pgs = pgRepository.GetPgData();
                pgls = pgRepository.GetPgLocData(strCulture);

                Assert.IsTrue(ppgs.Count == ppgls.Count && pgs.Count == pgls.Count);

            } catch (Exception ex) {
                throw ex;
            } finally {
                SwitchUserToDefaultTestUser();
            }
        }
        #endregion

        #region Methods
        private void DeleteParentPg(IWebDriver driver, bool isConfirm) {
            //Delete Row
            ReadOnlyCollection<IWebElement> rows = driver.FindElements(By.ClassName("ui-grid-row"));
            if (rows == null || rows.Count == 0) {
                throw new Exception("No Rows");
            }
            IWebElement lastRow = rows[rows.Count - 1];
            IWebElement btnDel = lastRow.FindElement(By.ClassName("reget-btn-grid-delete"));
            btnDel.Click();
            Thread.Sleep(1000);

            //Check confirm text
            if (isConfirm) {
                IWebElement btnYes = driver.FindElement(By.Id("btnPerform"));
                btnYes.Click();
                Thread.Sleep(1000);
            } else {
                var mdDialogContent = driver.FindElement(By.ClassName("md-dialog-content"));
                var ps = mdDialogContent.FindElements(By.TagName("p"));
            }
            
            Thread.Sleep(2000);
        }

        private void SetEditRowCkb(
            IWebDriver driver,
            WebDriverWait webDriverWait,
            IWebElement editRow) {


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
                    case "div":
                        //Checkbox
                        var mdCheckboxs = editElement.FindElements(By.TagName("md-checkbox"));
                        if (mdCheckboxs != null && mdCheckboxs.Count > 0) {
                            if (mdCheckboxs[0].GetAttribute("checked") == "true") {
                                mdCheckboxs[0].Click();
                            }
                           
                        }
                                                
                        break;
                }

            }
        }
        #endregion
    }
}
