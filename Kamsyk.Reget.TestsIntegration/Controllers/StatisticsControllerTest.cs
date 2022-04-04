using Kamsyk.Reget.Model;
using Kamsyk.Reget.Model.Repositories;
using Kamsyk.Reget.TestsIntegration.BaseTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kamsyk.Reget.TestsIntegration.Controllers {
    [TestClass]
    public class StatisticsControllerTest : BaseTestIntegration {
        #region Constructor
        public StatisticsControllerTest() {
            if (!IsTestDbConnected) {
                throw new Exception("Test DB is not connected");
            }
        }
        #endregion

        #region Test Methods
        [TestMethod]
        public void ZzInt_DisplayStatistics_AsAdmin() {
            using (IWebDriver driver = GetWebDriver(0, "en-US")) {
                string url = AppRootUrl + "Statistics";
                driver.Url = url;
                var webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));

                var dtpFromDate = FindElementByName(webDriverWait, "dtpFromDate");
                var inputs = dtpFromDate.FindElements(By.TagName("input"));
                inputs[0].SendKeys("1/1/2010");

                var dtpToDate = FindElementByName(webDriverWait, "dtpToDate");
                inputs = dtpToDate.FindElements(By.TagName("input"));
                inputs[0].SendKeys("1/1/2015");

                var ckbAll = FindElementById(webDriverWait, "ckbAll");
                ckbAll.Click();
                
                //Y Axis
                var cmbAxisYItems = FindElementById(webDriverWait, "cmbAxisYItems");
                cmbAxisYItems.Click();
                Thread.Sleep(1000);

                var options = FindCmbOptionsElements(cmbAxisYItems, webDriverWait);
                options[0].Click();

                //X Axis
                var cmbAxisXItems = FindElementById(webDriverWait, "cmbAxisXItems");
                cmbAxisXItems.Click();
                Thread.Sleep(1000);

                options = FindCmbOptionsElements(cmbAxisXItems, webDriverWait);
                options[0].Click();

                //Period
                var cmbAxisXPeriodItems = FindElementById(webDriverWait, "cmbAxisXPeriodItems");
                cmbAxisXPeriodItems.Click();
                Thread.Sleep(1000);

                options = FindCmbOptionsElements(cmbAxisXPeriodItems, webDriverWait);
                options[0].Click();

                var btnDisplay = FindElementById(webDriverWait, "btnDisplay");
                btnDisplay.Click();

                Thread.Sleep(5000);

                var divChartDataPanel = FindElementById(webDriverWait, "divChartDataPanel");
                bool isDisplayed = divChartDataPanel.Displayed;
                
                Assert.IsTrue(isDisplayed);
            }
        }

        [TestMethod]
        public void ZzInt_DisplayStatistics_ExportToExcel() {
            //Arrange
            string strDownloadFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            strDownloadFolder = Path.Combine(strDownloadFolder, "Downloads");
            var lastFileWriteDate = DateTime.MinValue;
            var sortedFiles = new DirectoryInfo(strDownloadFolder).GetFiles()
                                              .OrderByDescending(f => f.LastWriteTime)
                                              .ToList();
            if (sortedFiles.Count > 0) {
                lastFileWriteDate = (sortedFiles.ElementAt(0).LastWriteTime);
            }

            //Act
            using (IWebDriver driver = GetWebDriver(0, "en-US")) {
                string url = AppRootUrl + "Statistics";
                driver.Url = url;
                var webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));

                var dtpFromDate = FindElementByName(webDriverWait, "dtpFromDate");
                var inputs = dtpFromDate.FindElements(By.TagName("input"));
                inputs[0].SendKeys("1/1/2010");

                var dtpToDate = FindElementByName(webDriverWait, "dtpToDate");
                inputs = dtpToDate.FindElements(By.TagName("input"));
                inputs[0].SendKeys("1/1/2015");

                var ckbAll = FindElementById(webDriverWait, "ckbAll");
                ckbAll.Click();

                //Y Axis
                var cmbAxisYItems = FindElementById(webDriverWait, "cmbAxisYItems");
                cmbAxisYItems.Click();
                Thread.Sleep(1000);

                var options = FindCmbOptionsElements(cmbAxisYItems, webDriverWait);
                options[0].Click();

                //X Axis
                var cmbAxisXItems = FindElementById(webDriverWait, "cmbAxisXItems");
                cmbAxisXItems.Click();
                Thread.Sleep(1000);

                options = FindCmbOptionsElements(cmbAxisXItems, webDriverWait);
                options[1].Click();

                //Period
                var cmbAxisXPeriodItems = FindElementById(webDriverWait, "cmbAxisXPeriodItems");
                cmbAxisXPeriodItems.Click();
                Thread.Sleep(1000);

                options = FindCmbOptionsElements(cmbAxisXPeriodItems, webDriverWait);
                options[0].Click();

                var btnDisplay = FindElementById(webDriverWait, "btnDisplay");
                btnDisplay.Click();

                Thread.Sleep(15000);

                var divChartDataPanel = FindElementById(webDriverWait, "divChartDataPanel");
                bool isDisplayed = divChartDataPanel.Displayed;
                if (!isDisplayed) {
                    Assert.Fail();
                }

                var btnExportExcell = FindElementById(webDriverWait, "btnExportExcell");
                btnExportExcell.Click();

                Thread.Sleep(5000);

                if (IsErrorDisplayed(driver)) {
                    Assert.Fail();
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
                        isOk = true;
                    } else {
                        Thread.Sleep(3000);
                        iStep++;
                    }
                }

                Assert.IsTrue(isOk);
            }
        }
        #endregion

        #region Methods

        #endregion
    }
}
