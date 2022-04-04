using Kamsyk.Reget.TestsIntegration.BaseTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
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
    public class ReportControllerTest : BaseTestIntegration {
        #region Constructor
        public ReportControllerTest() {
            if (!IsTestDbConnected) {
                throw new Exception("Test DB is not connected");
            }
        }
        #endregion

        #region Test Methods
        [TestMethod]
        public void ZzInt_ReportCg_ExportToExcel() {

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

                string url = AppRootUrl + "RegetAdmin";
                driver.Url = url;
                WaitUntilLoadDialogIsClosed(driver);

                WebDriverWait webDriverWait;
                webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                IWebElement cmbCg = webDriverWait.Until(c => c.FindElement(By.Id("cmbCgList")));
                cmbCg.Click();
                Thread.Sleep(1000);

                //ChromeProfile profile = new FirefoxProfile();
                // profile.SetPreference("browser.helperApps.neverAsk.saveToDisk", "application/octet-stream;application/csv;text/csv;application/vnd.ms-excel;");
                                
                var options = webDriverWait.Until(c => c.FindElements(By.TagName("md-option")));
                options[0].Click();
                Thread.Sleep(3000);

                IWebElement btnExportExcel = webDriverWait.Until(c => c.FindElement(By.Id("btnExportExcel")));
                btnExportExcel.Click();
                Thread.Sleep(3000);


                //Assert
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
    }
}
