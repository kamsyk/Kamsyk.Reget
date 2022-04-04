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
using System.Configuration;
using System.Web.Configuration;
using System.Reflection;
using System.IO;
using Kamsyk.Reget.Model;
using static Kamsyk.Reget.Model.Repositories.UserRepository;
using System.Threading;

namespace Kamsyk.Reget.TestsIntegration.Controllers {
    [TestClass]
    public class HomeControllerIntegrationTest : BaseTestIntegration {
        #region Constructor
        public HomeControllerIntegrationTest() {
            if (!IsTestDbConnected) {
                throw new Exception("Test DB is not connected");
            }
        }
        #endregion

        #region Methods
        private string SwitchLang(
            IWebDriver driver, 
            string url, 
            string flagName, 
            string langLinkId,
            bool isMobileMode) {

            driver.Url = url;
            WaitUntilLoadDialogIsClosed(driver);

            WebDriverWait webDriverWait;
            if (isMobileMode) {
                webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                //webDriverWait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("btnMenu")));
                //IWebElement btnMenu = driver.FindElement(By.Id("btnMenu"));
                IWebElement btnMenu = webDriverWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("btnMenu")));
                btnMenu.Click();
            }

            webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
            webDriverWait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("imgCurrFlag")));
            IWebElement imgCurrFlag = driver.FindElement(By.Id("imgCurrFlag"));
            
            string strImg = imgCurrFlag.GetAttribute("src").ToLower().Trim();
            if (strImg.IndexOf(flagName) < 0) {
                webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                webDriverWait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("aCurrFlag")));
                IWebElement aCurrFlag = driver.FindElement(By.Id("aCurrFlag"));
                aCurrFlag.Click();

                webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                webDriverWait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id(langLinkId)));
                IWebElement aCul = driver.FindElement(By.Id(langLinkId));
                aCul.Click();

                //webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                //webDriverWait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("aReget")));
                WaitUntilLoadDialogIsClosed(driver);
                driver.FindElement(By.Id("aReget")).Click();
                WaitUntilLoadDialogIsClosed(driver);

                if (isMobileMode) {
                    webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                    webDriverWait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("btnMenu")));
                    IWebElement btnMenu = driver.FindElement(By.Id("btnMenu"));
                    btnMenu.Click();
                }

                webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                webDriverWait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("imgCurrFlag")));

                imgCurrFlag = driver.FindElement(By.Id("imgCurrFlag"));
            }

            strImg = imgCurrFlag.GetAttribute("src").ToLower().Trim();

            return strImg;
        }

        private IWebElement FindElementById(IWebDriver driver, string elementIf) {
            try {
                IWebElement iWebElement = driver.FindElement(By.Id(elementIf));

                return iWebElement;
            } catch (Exception ex) {
                if (ex is OpenQA.Selenium.NoSuchElementException) {
                    return null;
                } else {
                    throw ex;
                }
            }
        }

        private Size m_MobileSize {
            get { return new Size(700, 850); }
        }
        #endregion

        #region Test Methods
        [TestMethod]
        public void ZzInt_HomePage_RequestorOnly() {
            
            using (IWebDriver driver = GetWebDriver(0)) {
                //Arange
                
                string url = AppRootUrl;

                //Act
                driver.Url = url;
                IWebElement aReget = driver.FindElement(By.Id("aReget"));

                //Assert
                Assert.IsNotNull(aReget);
            }
        }

        #region Lang Switch
        [TestMethod]
        public void ZzInt_Switch_Lang_EN() {
            using (IWebDriver driver = GetWebDriver(0, "cs-CZ")) {
                //Arange
                string url = AppRootUrl;

                //Act
                string strImg = SwitchLang(driver, url, "uk.gif", "aCulEn", false);

                //Assert
                Assert.IsTrue((strImg.IndexOf("uk.gif") >= 0));
            }
        }

        [TestMethod]
        public void ZzInt_Switch_Lang_CZ() {
            using (IWebDriver driver = GetWebDriver(0, "en-US")) {
                //Arange
                string url = AppRootUrl;

                //Act
                string strImg = SwitchLang(driver, url, "cz.gif", "aCulCz", false);

                //Assert
                Assert.IsTrue((strImg.IndexOf("cz.gif") >= 0));
            }
        }

        [TestMethod]
        public void ZzInt_Switch_Lang_SK() {
            using (IWebDriver driver = GetWebDriver(0, "en-US")) {
                //Arange
                string url = AppRootUrl;

                //Act
                string strImg = SwitchLang(driver, url, "sk.gif", "aCulSk", false);

                //Assert
                Assert.IsTrue((strImg.IndexOf("sk.gif") >= 0));
            }
        }

        [TestMethod]
        public void ZzInt_Switch_Lang_PL() {
            using (IWebDriver driver = GetWebDriver(0, "en-US")) {
                //Arange
                string url = AppRootUrl;

                //Act
                string strImg = SwitchLang(driver, url, "pl.gif", "aCulPl", false);

                //Assert
                Assert.IsTrue((strImg.IndexOf("pl.gif") >= 0));
            }
        }

        [TestMethod]
        public void ZzInt_Switch_Lang_RO() {
            using (IWebDriver driver = GetWebDriver(0, "en-US")) {
                //Arange
                string url = AppRootUrl;

                //Act
                string strImg = SwitchLang(driver, url, "ro.gif", "aCulRo", false);

                //Assert
                Assert.IsTrue((strImg.IndexOf("ro.gif") >= 0));
            }
        }

        [TestMethod]
        public void ZzInt_Switch_Lang_BG() {
            using (IWebDriver driver = GetWebDriver(0, "en-US")) {
                //Arange
                string url = AppRootUrl;
                
                //Act
                string strImg = SwitchLang(driver, url, "bg.gif", "aCulBg", false);

                //Assert
                Assert.IsTrue((strImg.IndexOf("bg.gif") >= 0));
            }
        }

        [TestMethod]
        public void ZzInt_Switch_Lang_SL() {
            using (IWebDriver driver = GetWebDriver(0, "en-US")) {
                //Arange
                string url = AppRootUrl;

                //Act
                string strImg = SwitchLang(driver, url, "sl.gif", "aCulSl", false);

                //Assert
                Assert.IsTrue((strImg.IndexOf("sl.gif") >= 0));
            }
        }

        [TestMethod]
        public void ZzInt_Switch_Lang_SR() {
            using (IWebDriver driver = GetWebDriver(0, "en-US")) {
                //Arange
                string url = AppRootUrl;

                //Act
                string strImg = SwitchLang(driver, url, "sr.gif", "aCulSr", false);

                //Assert
                Assert.IsTrue((strImg.IndexOf("sr.gif") >= 0));
            }
        }

        [TestMethod]
        public void ZzInt_Switch_Lang_HU() {
            using (IWebDriver driver = GetWebDriver(0, "en-US")) {
                //Arange
                string url = AppRootUrl;

                //Act
                string strImg = SwitchLang(driver, url, "hu.gif", "aCulHu", false);

                //Assert
                Assert.IsTrue((strImg.IndexOf("hu.gif") >= 0));
            }
        }

        [TestMethod]
        public void ZzInt_UserDefaultLangNoUserSettings_CZ() {
            //Arrange
            var participant = GetParticipantNoUserSettingByCompany(0);

            //Act
            bool isOK = IsLangSet(participant.id, "Dodavatel");

            //Assert
            Assert.IsTrue(isOK);
        }

        [TestMethod]
        public void ZzInt_UserDefaultLangNoUserSettings_SK() {
            //Arrange
            var participant = GetParticipantNoUserSettingByCompany(1);

            //Act
            bool isOK = IsLangSet(participant.id, "Dodávateľ");

            //Assert
            Assert.IsTrue(isOK);
        }

        [TestMethod]
        public void ZzInt_UserDefaultLangNoUserSettings_RO() {
            //Arrange
            var participant = GetParticipantNoUserSettingByCompany(3);

            //Act
            bool isOK = IsLangSet(participant.id, "furnizor");

            //Assert
            Assert.IsTrue(isOK);
        }

        [TestMethod]
        public void ZzInt_UserDefaultLangNoUserSettings_PL() {
            //Arrange
            var participant = GetParticipantNoUserSettingByCompany(4);

            //Act
            bool isOK = IsLangSet(participant.id, "Dostawca");

            //Assert
            Assert.IsTrue(isOK);
        }

        [TestMethod]
        public void ZzInt_UserDefaultLangNoUserSettings_BG() {
            //Arrange
            var participant = GetParticipantNoUserSettingByCompany(5);

            //Act
            bool isOK = IsLangSet(participant.id, "Доставчик");

            //Assert
            Assert.IsTrue(isOK);
        }

        [TestMethod]
        public void ZzInt_UserDefaultLangNoUserSettings_SL() {
            //Arrange
            var participant = GetParticipantNoUserSettingByCompany(6);

            //Act
            bool isOK = IsLangSet(participant.id, "Dobavitelj");

            //Assert
            Assert.IsTrue(isOK);
        }

        [TestMethod]
        public void ZzInt_UserDefaultLangNoUserSettings_SR() {
            //Arrange
            var participant = GetParticipantNoUserSettingByCompany(7);

            //Act
            bool isOK = IsLangSet(participant.id, "Dobavljač");

            //Assert
            Assert.IsTrue(isOK);
        }

        [TestMethod]
        public void ZzInt_UserDefaultLangNoUserSettings_HU() {
            //Arrange
            var participant = GetParticipantNoUserSettingByCompany(8);

            //Act
            bool isOK = IsLangSet(participant.id, "Beszállító");

            //Assert
            Assert.IsTrue(isOK);
        }

        [TestMethod]
        public void ZzInt_UserNoUserSettings_Create() {
            //Arrange
            //Delete user settings
            new UserRepository().DeleteUserSettings(0);

            using (IWebDriver driver = GetWebDriver(0, "cs-CZ")) {
                string url = AppRootUrl;
                SwitchLang(driver, url, "uk.gif", "aCulEn", false);
                
            }

            //WebDriver.Quit();
            //WebDriver = null;
            TestCleanup();

            using (IWebDriver driver = GetWebDriver(0)) {
                //Arange
                string url = AppRootUrl;

                //Act
                driver.Url = url;
                IWebElement spNewRequest = driver.FindElement(By.Id("spNewRequest"));

                //Assert
                Assert.IsTrue(spNewRequest.Text == "New Request");
            }
        }

        [TestMethod]
        public void ZzInt_SwitchLangMobile_CZ() {
            using (IWebDriver driver = GetWebDriver(0, "en-US", m_MobileSize)) {
                //Arange
                string url = AppRootUrl;

                //Act
                string strImg = SwitchLang(driver, url, "cz.gif", "aCulCz", true);

                //Assert
                Assert.IsTrue((strImg.IndexOf("cz.gif") >= 0));
            }
        }
        #endregion

        [TestMethod]
        public void ZzInt_AboutHelp_Display() {

            using (IWebDriver driver = GetWebDriver(0)) {
                //Arange
                string url = AppRootUrl;

                //Act
                driver.Url = url;
                WaitUntilLoadDialogIsClosed(driver);

                //IWebElement divAboutDialogHidden = FindElementById(driver, "divAboutDialog");
                //Assert.IsNull(divAboutDialogHidden);

                IWebElement aRegetInfo = driver.FindElement(By.Id("aRegetInfo"));
                aRegetInfo.Click();

                WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                webDriverWait.Until<IWebElement>(d => d.FindElement(By.Id("divAboutHelp")));
                IWebElement divAboutHelp = driver.FindElement(By.Id("divAboutHelp"));

                //Assert
                Assert.IsNotNull(divAboutHelp);
            }
        }

        [TestMethod]
        public void ZzInt_NotAllowedUser_DisplayErrMsg() {
            new UserRepository().ChangeUserName(0, "xxx");

            using (IWebDriver driver = GetWebDriver()) {
                //Arange
                string url = AppRootUrl;

                //Act
                driver.Url = url;
                WaitUntilLoadDialogIsClosed(driver);

                IWebElement h2NotAllowedUser = FindElementById(driver, "h2NotAllowedUser"); 
                
                //Assert
                Assert.IsNotNull(h2NotAllowedUser);
            }
        }

        [TestMethod]
        public void ZzInt_NotActiveUser_DisplayErrMsg() {

            using (IWebDriver driver = GetWebDriver(1)) {
                //Arange
                string url = AppRootUrl;

                //Act
                driver.Url = url;
                WaitUntilLoadDialogIsClosed(driver);
                IWebElement h2NotAllowedUser = FindElementById(driver, "h2NotAllowedUser");

                //Assert
                Assert.IsNotNull(h2NotAllowedUser);
            }
        }


        [TestMethod]
        public void ZzInt_Error_DisplayErrPage() {

            using (IWebDriver driver = GetWebDriver(0)) {
                //Arange
                string url = AppRootUrl + "/Request/NewRequest/?IsError=1";

                //Act
                driver.Url = url;
                WaitUntilLoadDialogIsClosed(driver);
                IWebElement errImg = FindElementById(driver, "errImg");

               
                //Assert
                Assert.IsNotNull(errImg);
            }
        }

        [TestMethod]
        public void ZzInt_ControllerWithoutView_DisplayErrorNoMenu() {
            using (IWebDriver driver = GetWebDriver(0)) {
                //Arange
                string url = AppRootUrl + "/NoViewTest";

                //Act
                driver.Url = url;
                WaitUntilLoadDialogIsClosed(driver);
                IWebElement spNewRequest = FindElementById(driver, "spNewRequest");

                //Assert
                Assert.IsNull(spNewRequest);
            }
        }

        [TestMethod]
        public void ZzInt_UserIsNotAuthorized_DisplayInfo() {
           using (IWebDriver driver = GetWebDriver(0, "en-US")) {
                UserRepository userRepository = new UserRepository();
                try {
                    //Arange
                    var part = userRepository.GetParticipantById(0);
                    part.user_name = "NotExistingUser";
                    userRepository.SaveParticipantData(part);

                    string url = AppRootUrl;//+ "?lang=" + "en-US";

                    //Act
                    driver.Url = url;
                    IWebElement h2NotAllowedUser = driver.FindElement(By.Id("h2NotAllowedUser"));

                    //Assert
                    //Assert.IsTrue(h2NotAllowedUser.Text.IndexOf("is not allowed to use the application.") > -1);
                    Assert.IsTrue(h2NotAllowedUser != null);
                } catch (Exception ex) {
                    throw ex;
                } finally {
                    var part = userRepository.GetParticipantById(0);
                    part.user_name = "syka";
                    userRepository.SaveParticipantData(part);
                }
            }
        }

        [TestMethod]
        public void ZzInt_DbConnection_NotAvailable() {
            using (IWebDriver driver = GetWebDriver(0)) {
                //Arrange
                FileInfo currentFile = new FileInfo(Assembly.GetExecutingAssembly().Location);
                DirectoryInfo directoryInfo = currentFile.Directory.Parent.Parent.Parent;
                string strWebConfigPath = Path.Combine(directoryInfo.FullName, "Kamsyk.Reget");
                strWebConfigPath = Path.Combine(strWebConfigPath, "web.config");

                string strOrigWebConfig = File.ReadAllText(strWebConfigPath);

                try {
                    int pos = strOrigWebConfig.IndexOf("name=\"InternalRequestEntities\"");
                    int posStart = strOrigWebConfig.IndexOf("data source", pos);
                    int posEnd = strOrigWebConfig.IndexOf(";", posStart);
                    string substring = strOrigWebConfig.Substring(posStart, posEnd - posStart);
                    string strModifWebConfig = strOrigWebConfig.Substring(0, posStart);
                    strModifWebConfig += substring + "1";
                    strModifWebConfig += strOrigWebConfig.Substring(posEnd);
                    File.WriteAllText(strWebConfigPath, strModifWebConfig);

                    //Act
                    string url = AppRootUrl;
                                        
                    driver.Url = url;
                    WaitUntilLoadDialogIsClosed(driver);
                    var divDbIsNotAccessible = FindElementById(driver, "divDbIsNotAccessible");
                    string displayStyle = divDbIsNotAccessible.GetCssValue("display");

                    //Assert
                    Assert.IsTrue(displayStyle=="block");
                } catch (Exception ex) {
                    throw ex;
                } finally {
                    File.WriteAllText(strWebConfigPath, strOrigWebConfig);
                }

            }
        }

        #region MenuItems Availability
        [TestMethod]
        public void ZzInt_MenuItem_Availability_Requestor() {
            //Arrange
            var requestorOnly = GetParticipant(UserRole.Requestor);
            List<string> requestMenuItems = GetParticipantMenuItems();

            //Act
            bool isOK = CheckMenuItemsAvailability(requestorOnly.id, requestMenuItems);

            //Assert
            Assert.IsTrue(isOK);
        }

        [TestMethod]
        public void ZzInt_MenuItem_Availability_Orderer() {
            //Arrange
            var ordererOnly = GetParticipant(UserRole.Orderer);
            List<string> ordererMenuItems = GetOrdererMenuItems();

            //Act
            bool isOK = CheckMenuItemsAvailability(ordererOnly.id, ordererMenuItems);

            //Assert
            Assert.IsTrue(isOK);
        }

        [TestMethod]
        public void ZzInt_MenuItem_Availability_ApprovalMan() {
            //Arrange
            var appManOnly = GetParticipant(UserRole.ApprovalManager);
            List<string> appManMenuItems = GetApproveManagerMenuItems();

            //Act
            bool isOK = CheckMenuItemsAvailability(appManOnly.id, appManMenuItems);

            //Assert
            Assert.IsTrue(isOK);
        }

        [TestMethod]
        public void ZzInt_MenuItem_Availability_OfficeAdmin() {
            //Arrange
            var compAdminOnly = GetParticipant(UserRole.OfficeAdministrator);
            List<string> compAdminMenuItems = GetCompanyAdminMenuItems();

            //Act
            bool isOK = CheckMenuItemsAvailability(compAdminOnly.id, compAdminMenuItems);

            //Assert
            Assert.IsTrue(isOK);
        }
        #endregion

        private bool CheckMenuItemsAvailability(int userId, List<string> requestMenuItems) {
            
            try {
                using (IWebDriver driver = GetWebDriver(userId, "en-US")) {
                    string url = AppRootUrl;

                    driver.Url = url;
                    WaitUntilLoadDialogIsClosed(driver);
                    
                    var menuLis = driver.FindElements(By.TagName("li"));
                    foreach (var menuLi in menuLis) {
                        string strId = menuLi.GetAttribute("id");
                        string strClass = menuLi.GetAttribute("class");
                        if (String.IsNullOrEmpty(strId)) {
                            if (strClass.Contains("divider")) {
                                continue;
                            } else {
                                return false;
                            }
                        } else if (requestMenuItems.Contains(strId)) {
                            requestMenuItems.Remove(strId);
                        } else {
                            return false;
                        }
                    }

                    
                    return (requestMenuItems.Count == 0);
                }
            } catch (Exception ex) {
                throw ex;
            } finally {
                SwitchUserToDefaultTestUser();
            }
        }

        private Participants GetParticipant(UserRole role) {
            UserRepository userRepository = new UserRepository();
            Participants partRoleOnly = null;

            List<int> checkedIndexes = new List<int>();
            int maxTryCount = 0;
            if (role == UserRole.OfficeAdministrator && maxTryCount < 100) {
                var participants = userRepository.GetActiveParticipantsByOfficeRole(role);
                while (partRoleOnly == null) {

                    int iIndex = -1;
                    while ((checkedIndexes.Contains(iIndex) || iIndex < 0) && maxTryCount < 100) {
                        iIndex = new Random().Next(0, participants.Count - 1);
                        Thread.Sleep(95);
                        maxTryCount++;
                    }
                    checkedIndexes.Add(iIndex);

                    Participants p = participants.ElementAt(iIndex);
                    if (p.id == 0) {
                        continue;
                    }
                    var appAdmin = (from roleDb in p.Participant_Office_Role
                                    where roleDb.role_id == (int)UserRole.ApplicationAdmin
                                    select roleDb).FirstOrDefault();

                    if (appAdmin != null) {
                        continue;
                    }

                    var participantAreaPropAdmin = (from roleDb in p.ParticipantRole_CentreGroup
                                    where roleDb.role_id == (int)UserRole.CentreGroupPropAdmin
                                    select roleDb).FirstOrDefault();
                    if (participantAreaPropAdmin == null) {
                        partRoleOnly = p;
                        
                    }
                    
                }

            } else { 
                
                var participants = userRepository.GetActiveParticipantsByCgRole(role);
                
                while (partRoleOnly == null) {
                    int iIndex = new Random().Next(0, participants.Count - 1);
                    Participants p = participants.ElementAt(iIndex);
                    if (p.id == 0) {
                        continue;
                    }
                    if (p.ParticipantRole_CentreGroup.Count == 1) {
                        partRoleOnly = p;
                    }
                }
            }

            return partRoleOnly;
        }

        private List<string> GetParticipantMenuItems() {
            return GetBasicUserMenuItems();
        }

        private List<string> GetOrdererMenuItems() {
            return GetBasicUserMenuItems();
        }

        private List<string> GetApproveManagerMenuItems() {
            return GetBasicUserMenuItems();
        }

        private List<string> GetComapnyAdminMenuItems() {
            return GetBasicUserMenuItems();
        }

        private List<string> GetBasicUserMenuItems() {
            List<string> menuItems = new List<string>() {
                "mniStatistics",
                "mniAdmin",
                "mniCg",
                "mniCgSettings",
                "mniAppMatrixExport",
                "mniSupplier",
                "mniAbout",
                "mniLocalization",
                //"mniEN",
                "mniCZ",
                "mniSK",
                "mniPL",
                "mniRO",
                "mniBG",
                "mniSR",
                "mniHU",
                "mniSL",
                "mniUserName",
                "mniParticipantRoot",
                "mniUserSubstitution"
            };

            return menuItems;
        }

        private List<string> GetCompanyAdminMenuItems() {
            List<string> menuItems = GetBasicUserMenuItems();

            //menuItems.Add("mniNewCg");
            menuItems.Add("mniMultiplyAppLevel");
            menuItems.Add("mniAppMatrixCopy");
            menuItems.Add("mniPpgPg");
            menuItems.Add("mniPpg");
            menuItems.Add("mniUsedPg");
            //menuItems.Add("mniParticipantRoot");
            menuItems.Add("mniParticipants");
            menuItems.Add("mniUserInfo");
            menuItems.Add("mniReplaceUser");
            menuItems.Add("mniNonActiveUsers");
            menuItems.Add("mniCentre");
            
            return menuItems;
        }

        private bool IsLangSet(int userId, string locText) {
            try {
                using (IWebDriver driver = GetWebDriver(userId)) {
                    //Arange
                    string url = AppRootUrl;

                    //Act
                    driver.Url = url;
                    IWebElement mniSupplier = driver.FindElement(By.Id("mniSupplier"));
                    string innerHtml = mniSupplier.GetAttribute("innerHTML");

                    //Assert
                    return (innerHtml.Contains(locText));
                }
            } catch (Exception ex) {
                throw ex;
            } finally {
                SwitchUserToDefaultTestUser();
            }
        }

        private Participants GetParticipantNoUserSettingByCompany(int compId) {
            var participants = new UserRepository().GetUserByDefaultLangIdNoUserSettings(compId);

            int iIndex = new Random().Next(0, participants.Count - 1);

            return participants.ElementAt(iIndex);
        }
        #endregion
    }
}
