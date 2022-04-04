using Kamsyk.Reget.Model.Repositories;
using Kamsyk.Reget.TestsIntegration.BaseTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.TestsIntegration.Controllers {
    [TestClass]
    public class InitialTest : BaseTestIntegration {
        [TestMethod]
        [Priority(0)]
        public void CheckSettings() {
#if TEST
            //Arrange
            string testIndicatorText = Guid.NewGuid().ToString();
            new TestIndicatorRepository().SetTestIndicatorText(testIndicatorText);

            //Act
            using (IWebDriver driver = GetWebDriver(0)) {
                string url = AppRootUrl + "TestIndicator";
                driver.Url = url;

                WebDriverWait webDriverWait;
                webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                IWebElement divTestIndicator = webDriverWait.Until(c => c.FindElement(By.Id("divTestIndicator")));

                IsTestDbConnected = (testIndicatorText == divTestIndicator.GetAttribute("innerHTML"));
            }

            //Assert
            Assert.IsTrue(IsTestDbConnected);
            return;
#else
            IsTestDbConnected = false;
            Assert.Fail();
            return;
#endif


        }
    }
}
