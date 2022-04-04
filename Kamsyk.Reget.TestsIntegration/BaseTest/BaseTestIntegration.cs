using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Kamsyk.Reget.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using OpenQA.Selenium.Interactions;
using System.Collections.ObjectModel;
using Kamsyk.Reget.TestsIntegration.Controllers;
using OpenQA.Selenium.Chrome;

namespace Kamsyk.Reget.TestsIntegration.BaseTest {
    [TestClass]
    public class BaseTestIntegration {
        #region Constants
        private const int WEB_DRIVER_REPEAT_TIME_IN_MILISECONDS = 500;
        private const int WEB_DRIVER_WAIT_REPEAT = 40;
        protected const int WAIT_IN_SECONDS = 10;
        #endregion

        #region Static Properties
        private static bool m_IsTestDbConnected = false;
        public static bool IsTestDbConnected {
#if TEST
            get {
                if (!m_IsTestDbConnected) {
                    new InitialTest().CheckSettings();
                }

                return m_IsTestDbConnected;
            }
#else
            get { return false; }
#endif
            set { m_IsTestDbConnected = value; }
        }
        #endregion

        #region Properties
        private IWebDriver m_webDriver = null;
        public IWebDriver WebDriver {
            get {
                if (m_webDriver == null) {
                    ChromeOptions options = new ChromeOptions();
                    options.AddArguments(
                        "browser.helperApps.neverAsk.saveToDisk",
                        "application/octet-stream;application/csv;text/csv;application/vnd.ms-excel;");
                    options.AddArgument("no-sandbox");
                    options.AddAdditionalCapability("useAutomationExtension", false);

                    m_webDriver = new ChromeDriver(options);
                    WebDriver.Manage().Window.Size = new Size(1200, 800);
                }

                return m_webDriver;
            }
            set {
                m_webDriver = value;
            }
        }

        protected string AppRootUrl {
            get {
                string strCred = DefaultUserName + ":" + TestUserPwd + "@";
                return "http://" + strCred + "localhost:26077/";
            }
        }

        protected string DefaultUserName {
            get { return GetConfigValue("DefaultUserName"); }
        }

        protected string TestUserPwd {
            get { return GetConfigValue("TestUserPwd"); }
        }

        protected int WaitInSeconds {
            get { return 10; }
        }
        #endregion

        #region Constructor
        //public BaseTestIntegration(int userId) : base() {
        //    SetCurrentUser(userId);
        //}
        #endregion

        #region Methods
        [TestCleanup]
        protected void TestCleanup() {
            WebDriver.Quit();
            m_webDriver = null;
        }

        protected IWebDriver GetWebDriver() {

            return WebDriver;
        }

        protected IWebDriver GetWebDriver(int userId) {
            SetCurrentUser(userId, null);
            return WebDriver;
        }

        protected IWebDriver GetWebDriver(int userId, string cultureName) {
            SetCurrentUser(userId, cultureName);
            return WebDriver;
        }

        protected IWebDriver GetWebDriver(int userId, string cultureName, Size windowSize) {
            SetCurrentUser(userId, cultureName);
            WebDriver.Manage().Window.Size = windowSize;
            return WebDriver;
        }

        protected void WaitUntilLoadDialogIsClosed(IWebDriver driver) {
            WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            webDriverWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("divLoadImg")));
        }

        private void SetCurrentUser(int userId, string cultureName) {
            UserRepository userRepository = new UserRepository();
            userRepository.SwitchUserToDefaultTestUser(userId, DefaultUserName);
            if (cultureName != null) {
                userRepository.SetParticipantLang(userId, cultureName);
            }
        }

        private string GetConfigValue(string keyName) {
            return ConfigurationManager.AppSettings[keyName];

        }

        protected string GetGridId(IWebElement grdSupplier) {
            string cssClass = grdSupplier.GetAttribute("class");

            string[] strItems = cssClass.Split(' ');
            string grdClass = strItems[strItems.Length - 1];
            string grdId = grdClass.Replace("grid", "");

            return grdId;
        }

        protected bool IsAutoCompleteNotFoundTestDisplayed(
            IWebDriver driver,
            WebDriverWait webDriverWait,
            string btnIdToDisplayeAutocomplete,
            string autoCompleteId,
            string notExistingItem,
            string warningMessage) {

            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("var el = document.getElementById('" + btnIdToDisplayeAutocomplete + "');el.scrollIntoView();");
            //Thread.Sleep(3000);
            IWebElement btnDisplayAutoComplete = webDriverWait.Until(c => c.FindElement(By.Id(btnIdToDisplayeAutocomplete)));
            //Thread.Sleep(1000);
            btnDisplayAutoComplete.Click();
            //Thread.Sleep(2000);

            IWebElement autoDeputyOrderer = webDriverWait.Until(c => c.FindElement(By.Id(autoCompleteId)));
            var inputs = webDriverWait.Until(c => autoDeputyOrderer.FindElements(By.TagName("input")));
            string ulId = inputs[0].GetAttribute("aria-owns");
            //string strKey = inpId.Replace("input-", "");
            //string strNotExistUser = "asdaswejwe wejwej";
            inputs[0].SendKeys(notExistingItem);

            bool isOK = false;
            int iRepeatRound = 0;
            ReadOnlyCollection<IWebElement> lis = null;
            while ((lis == null || lis.Count == 0) && iRepeatRound < 3) {
                Thread.Sleep(6000);

                //string ulId = "ul-" + strKey;
                IWebElement ul = webDriverWait.Until(c => c.FindElement(By.Id(ulId)));
                lis = webDriverWait.Until(c => ul.FindElements(By.TagName("li")));
                if (lis != null && lis.Count > 0) {
                    try {
                        var strWebWarning = lis[0].Text;
                        isOK = (warningMessage == strWebWarning);
                    } catch (Exception ex) {
                        throw ex;
                    }
                }
                iRepeatRound++;
            }

            //inputs[0].Click();
            btnDisplayAutoComplete = webDriverWait.Until(c => c.FindElement(By.Id(btnIdToDisplayeAutocomplete)));
            //Thread.Sleep(3000);
            Actions actions = new Actions(driver);

            actions.MoveToElement(btnDisplayAutoComplete).Click().Perform();
            //btnDisplayAutoComplete.Click();
            //Thread.Sleep(3000);

            return isOK;
        }

        public string GetGridId(WebDriverWait webDriverWait) {
            IWebElement grid = webDriverWait.Until(c => c.FindElement(By.ClassName("grid")));
            string grdClass = grid.GetAttribute("class");
            string[] idITems = grdClass.Split(' ');
            string grdId = idITems[idITems.Length - 1].Replace("grid", "");

            return grdId;
        }

        protected ReadOnlyCollection<IWebElement> FindElementsByTagName(IWebDriver driver, string tagName) {
            ReadOnlyCollection<IWebElement> elements = null;
            int iCount = 0;
            while (elements == null || elements.Count == 0) {
                Thread.Sleep(WEB_DRIVER_REPEAT_TIME_IN_MILISECONDS);
                elements = driver.FindElements(By.TagName(tagName));
                iCount++;
                if (iCount > WEB_DRIVER_WAIT_REPEAT) {
                    return elements;
                }
            }

            return elements;
        }

        protected ReadOnlyCollection<IWebElement> FindCmbOptionsElements(
            IWebDriver driver,
            WebDriverWait webDriverWait,
            string selectId) {

            IWebElement cmb = FindElementById(webDriverWait, selectId);
            string ariOwns = cmb.GetAttribute("aria-owns");
            IWebElement divOptions = FindElementById(webDriverWait, ariOwns);

            ReadOnlyCollection<IWebElement> elements = null;
            int iCount = 0;
            while (elements == null || elements.Count == 0) {
                Thread.Sleep(WEB_DRIVER_REPEAT_TIME_IN_MILISECONDS);
                elements = divOptions.FindElements(By.TagName("md-option"));
                iCount++;
                if (iCount > WEB_DRIVER_WAIT_REPEAT) {
                    return elements;
                }
            }

            return elements;
        }

        protected ReadOnlyCollection<IWebElement> FindCmbOptionsElements(IWebElement cmb, WebDriverWait webDriverWait) {

            string ariOwns = cmb.GetAttribute("aria-owns");
            IWebElement divOptions = FindElementById(webDriverWait, ariOwns);

            ReadOnlyCollection<IWebElement> elements = null;
            int iCount = 0;
            while (elements == null || elements.Count == 0) {
                Thread.Sleep(WEB_DRIVER_REPEAT_TIME_IN_MILISECONDS);
                elements = divOptions.FindElements(By.TagName("md-option"));
                iCount++;
                if (iCount > WEB_DRIVER_WAIT_REPEAT) {
                    return elements;
                }
            }

            return elements;
        }

        protected ReadOnlyCollection<IWebElement> FindautoCompleteOptionsElements(
            IWebDriver driver,
            WebDriverWait webDriverWait,
            string txtId) {

            IWebElement txt = FindElementById(webDriverWait, txtId);
            string ariOwns = txt.GetAttribute("aria-owns");
            //IWebElement ul = FindElementById(webDriverWait, ariOwns);
            //IWebElement ul = webDriverWait.Until(c => c.);
            var ul = webDriverWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id(ariOwns)));

            ReadOnlyCollection<IWebElement> elements = null;
            int iCount = 0;
            while (elements == null || elements.Count == 0) {
                Thread.Sleep(WEB_DRIVER_REPEAT_TIME_IN_MILISECONDS);
                elements = ul.FindElements(By.TagName("li"));
                iCount++;
                if (iCount > WEB_DRIVER_WAIT_REPEAT) {
                    return elements;
                }
            }


            return elements;
        }

        protected IWebElement FindElementById(WebDriverWait webDriverWait, string strId) {
            IWebElement element = null;
            int iCount = 0;
            while (element == null) {
                Thread.Sleep(WEB_DRIVER_REPEAT_TIME_IN_MILISECONDS);
                try {
                    element = webDriverWait.Until(c => c.FindElement(By.Id(strId)));
                } catch { }
                iCount++;
                if (iCount > WEB_DRIVER_WAIT_REPEAT) {
                    return element;
                }
            }

            return element;
        }

        protected IWebElement FindElementByClassName(WebDriverWait webDriverWait, string className) {
            IWebElement element = null;
            int iCount = 0;
            while (element == null) {
                Thread.Sleep(WEB_DRIVER_REPEAT_TIME_IN_MILISECONDS);
                try {
                    element = webDriverWait.Until(c => c.FindElement(By.ClassName(className)));
                } catch { }
                iCount++;
                if (iCount > WEB_DRIVER_WAIT_REPEAT) {
                    return element;
                }
            }

            return element;
        }

        protected IWebElement FindElementByName(WebDriverWait webDriverWait, string strName) {
            IWebElement element = null;
            int iCount = 0;
            while (element == null) {
                Thread.Sleep(WEB_DRIVER_REPEAT_TIME_IN_MILISECONDS);
                try {
                    element = webDriverWait.Until(c => c.FindElement(By.Name(strName)));
                } catch { }
                iCount++;
                if (iCount > WEB_DRIVER_WAIT_REPEAT) {
                    return element;
                }
            }

            return element;
        }

        protected void SwitchUserToDefaultTestUser() {
            new UserRepository().SwitchUserToDefaultTestUser(0, DefaultUserName);
        }

        protected bool IsErrorDisplayed(IWebDriver driver) {
            //WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_IN_SECONDS));
            try {
                IWebElement imgError = driver.FindElement(By.ClassName("reget-error-img"));
                return (imgError != null);
            } catch (Exception ex) {
                if (ex is NoSuchElementException) {
                    return false;
                }
            }


            return false;

        }

        protected bool IsElementExistByClassName(IWebDriver driver, string className, IWebElement parentElement) {
            if (parentElement == null) {
                if (driver.FindElements(By.ClassName(className)).Count() > 0) {
                    return true;
                }
            } else {
                if (parentElement.FindElements(By.ClassName(className)).Count() > 0) {
                    return true;
                }
            }

            return false;
        }

        protected bool IsElementExistByClassName(IWebDriver driver, string className) {
            return IsElementExistByClassName(driver, className, null);
        }
        #endregion


    }
}
