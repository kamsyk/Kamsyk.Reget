using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Kamsyk.Reget.TestsIntegration.BaseTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.TestsIntegration.Controllers {
    [TestClass]
    public class DataGridIntegrationTest : BaseTestIntegration {
        #region Constructor
        public DataGridIntegrationTest() {
            if (!IsTestDbConnected) {
                throw new Exception("Test DB is not connected");
            }
        }
        #endregion

        #region Test Methods
        [TestMethod]
        public void ZzInt_SetUserGridSettings_MoveColumn_SaveChanges() {

            using (IWebDriver driver = GetWebDriver(0)) {
                //Arange
                string url = AppRootUrl + "Participant";

                //Act
                driver.Url = url;
                WaitUntilLoadDialogIsClosed(driver);


                //"//*[@id="1532257325464 - grid - container"]/div[1]/div/div/div/div/div/div[4]/div[2]"
                var elHeaders = driver.FindElements(By.ClassName("ui-grid-header-cell"));
                //IList<IWebElement> inputs = driver.FindElements(By.XPath("[@id=\"1532257325464-grid-container\"]/div[1]/div/div/div/div/div/div[4]/div[2]"));
                //var parentElement = elHeaders[3].FindElement(By.XPath("..")); //parent relative to current element
                //var elHeaders = driver.FindElements(By.LinkText("columnheader"));
               // var el = driver.FindElement(By.Id("532258871550-grid-container"));
                Actions ac = new Actions(driver);
                //ac.DragAndDrop(source element, target element);
                //ac.DragAndDropToOffset(elHeaders[3], 200, 0);
                ac.DragAndDrop(elHeaders[3], elHeaders[6]);
                ac.Build().Perform();

                //WebDriverWait webDriverWait;
                //webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                //webDriverWait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("cmbCgList")));
                //IWebElement cmbCg = driver.FindElement(By.Id("cmbCgList"));
                //cmbCg.Click();

                //webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                //webDriverWait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("select_option_5")));
                //IWebElement cmbSelect_option_5 = driver.FindElement(By.Id("select_option_5"));
                //cmbSelect_option_5.Click();

                //IWebElement txtCgName = driver.FindElement(By.Id("txtCgName"));
                //string newCgName = (txtCgName.GetAttribute("value").Length > 25) ? txtCgName.GetAttribute("value").Substring(0, 24) : txtCgName.GetAttribute("value") + "1";
                //txtCgName.Clear();
                //txtCgName.SendKeys(newCgName);

                //webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                //webDriverWait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("btnSaveCgDetails")));
                //IWebElement btnSaveCgDetails = driver.FindElement(By.Id("btnSaveCgDetails"));
                //btnSaveCgDetails.Click();

                //webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                //webDriverWait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("btnRefresh")));
                //IWebElement btnRefresh = driver.FindElement(By.Id("btnRefresh"));
                //btnRefresh.Click();

                //txtCgName = driver.FindElement(By.Id("txtCgName"));

                ////Assert
                //Assert.IsTrue(txtCgName.GetAttribute("value") == newCgName);
            }
        }
        #endregion
    }
}
