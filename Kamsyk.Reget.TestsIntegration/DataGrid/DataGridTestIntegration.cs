using Kamsyk.Reget.Model.Repositories;
using Kamsyk.Reget.TestsIntegration.BaseTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.TestsIntegration.DataGrid {
    [TestClass]
    public class DataGridTestIntegration : BaseDbGridTestIntegration {
        #region Test Methods
        //Address
        [TestMethod]
        public void ZzInt_GridAddressTest() {
            using (IWebDriver driver = GetWebDriver(0, "en-US")) {

                //Arange
                string url = AppRootUrl + "Address";
                driver.Url = url;
                DlgClear dlgClear = new DlgClear(ClearAddress);

                //Act
                //DlgAddNewRecord dlgAddNewRecord = new DlgAddNewRecord(AddNewAddress);
                bool isPassed = TestDataGrid(
                    driver,
                    "grdAddress",
                    true,
                    dlgClear);

                //Assert
                Assert.IsTrue(isPassed);

            }
        }

        //Centre
        [TestMethod]
        public void ZzInt_GridCentreTest() {
            //try {
            using (IWebDriver driver = GetWebDriver(0, "en-US")) {

                //Arange
                string url = AppRootUrl + "Centre";
                driver.Url = url;

                DlgClear dlgClear = new DlgClear(ClearCentre);

                //Act
                bool isPassed = TestDataGrid(
                    driver,
                    "grdCentre",
                    true,
                    dlgClear);

                //Assert
                Assert.IsTrue(isPassed);

            }
            //} catch (Exception ex) {
            //    throw ex;
            //}
        }

        //[TestMethod]
        //public void ZzInt_Zz() {
        //    try {
        //        Assert.Fail();
        //    } catch (Exception ex) {
        //        Assert.Fail();
        //        //    if (ex is AssertFailedException) {
        //        //        Assert.Fail();
        //        //    } else {
        //        //        throw ex;
        //        //    }
        //        //}
        //    }
        //}

        //Parent PG
        [TestMethod]
        public void ZzInt_GridParentPgTest() {
            using (IWebDriver driver = GetWebDriver(0, "en-US")) {

                //Arange
                string url = AppRootUrl + "ParentPg";
                driver.Url = url;

                DlgClear dlgClear = new DlgClear(ClearParentPg);

                //Act
                bool isPassed = TestDataGrid(
                    driver,
                    "grdParentPg",
                    true,
                    dlgClear,
                    NewRowCheckboxValueType.None,
                    2);

                //Assert
                Assert.IsTrue(isPassed);

            }
        }

        //Used PG
        [TestMethod]
        public void ZzInt_GridUsedPgTest() {
            using (IWebDriver driver = GetWebDriver(0, "en-US")) {

                //Arange
                string url = AppRootUrl + "ParentPg/UsedPg";
                driver.Url = url;

                DlgClear dlgClear = new DlgClear(ClearParentPg);

                //Act
                bool isPassed = TestDataGrid(
                    driver,
                    "grdUsedPg",
                    false,
                    dlgClear);

                //Assert
                Assert.IsTrue(isPassed);

            }
        }

        //Users
        [TestMethod]
        public void ZzInt_GridUsersTest() {
            using (IWebDriver driver = GetWebDriver(0, "en-US")) {

                //Arange
                string url = AppRootUrl + "Participant";
                driver.Url = url;

                DlgClear dlgClear = new DlgClear(ClearUser);

                //Act
                bool isPassed = TestDataGrid(
                    driver,
                    "grdUser",
                    false,
                    dlgClear);

                //Assert
                Assert.IsTrue(isPassed);

            }
        }

        //Users
        [TestMethod]
        public void ZzInt_GridNonActiveUsersTest() {
            using (IWebDriver driver = GetWebDriver(0, "en-US")) {

                //Arange
                string url = AppRootUrl + "Participant/NonActiveUser";
                driver.Url = url;

                DlgClear dlgClear = new DlgClear(ClearUser);

                //Act
                bool isPassed = TestDataGrid(
                    driver,
                    "grdUser",
                    false,
                    dlgClear);

                //Assert
                Assert.IsTrue(isPassed);

            }
        }

        //Users
        [TestMethod]
        public void ZzInt_GridSubstitutionTest() {
            using (IWebDriver driver = GetWebDriver(0, "en-US")) {

                //Arange
                string url = AppRootUrl + "Participant/UserSubstitution";
                driver.Url = url;

                DlgClear dlgClear = new DlgClear(ClearSubstitution);

                //Act
                bool isPassed = TestDataGrid(
                    driver,
                    "grdUserSubstitution",
                    false,
                    dlgClear);

                //Assert
                Assert.IsTrue(isPassed);

            }
        }
        #endregion

        #region Methods
        private void ClearCentre() {
            new CentreRepository().DeleteCentreByName(NEW_ITEM_TEXT);
        }

        private void ClearAddress() {
            new AddressRepository().DeleteAddressByName(NEW_ITEM_TEXT);
        }

        private void ClearParentPg() {
            new ParentPgRepository().DeleteParentPgByName(NEW_ITEM_TEXT);
        }

        private void ClearUser() {
            new UserRepository().DeleteUsersByName(NEW_ITEM_TEXT);
        }

        private void ClearSubstitution() {
        }
        #endregion
    }
}
