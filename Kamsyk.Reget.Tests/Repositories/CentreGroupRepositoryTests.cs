using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kamsyk.Reget.Controllers.Interface;
using Kamsyk.Reget.Model.ExtendedModel;
using Kamsyk.Reget.Model.Repositories;
using Kamsyk.Reget.Model.Repositories.Interfaces;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Kamsyk.Reget.Model.Repositories.Tests {
    [TestClass()]
    public class CentreGroupRepositoryTests {
        [TestMethod()]
        public void GetActiveCentreGroupstByUserIdTest() {
            //Assign
            var mockManager = MockRepository.GenerateMock<ICentreGroupRepository>();

            //Act
            List<CentreGroupAdminList> cgs = mockManager.GetActiveCentreGroupsByUserId(0, null);

            //Assert
            mockManager.AssertWasCalled(x => x.GetActiveCentreGroupsByUserId(0, null));
        }

        [TestMethod()]
        public void GetCentreGroupAllDataByIdTest() {
            //Assign
            var mockManager = MockRepository.GenerateMock<ICentreGroupRepository>();

            //Act
            Centre_Group cg = mockManager.GetCentreGroupAllDataById(0);

            //Assert
            mockManager.AssertWasCalled(x => x.GetCentreGroupAllDataById(0));
        }

        [TestMethod()]
        public void SaveCentreGroupDataTest() {
            //Assign
            var mockManager = MockRepository.GenerateMock<ICentreGroupRepository>();

            //Act
            CentreGroupExtended cg = new CentreGroupExtended();
            mockManager.SaveCentreGroupData(cg);

            //Assert
            mockManager.AssertWasCalled(x => x.SaveCentreGroupData(cg));
        }

        [TestMethod()]
        public void GetCentreGroupDataByIdTest() {
            //Assign
            var mockManager = MockRepository.GenerateMock<ICentreGroupRepository>();

            //Act
            CentreGroupExtended cg = mockManager.GetCentreGroupDataById(0, 0);

            //Assert
            mockManager.AssertWasCalled(x => x.GetCentreGroupDataById(0, 0));
        }

        [TestMethod()]
        public void IsCentreGroupReadOnlyTest() {
            //Assign
            var mockManager = MockRepository.GenerateMock<ICentreGroupRepository>();

            //Act
            CgAdminRoles cgAdminRoles = mockManager.GetCgAdminRoles(0, 0);

            //Assert
            mockManager.AssertWasCalled(x => x.GetCgAdminRoles(0, 0));
        }

        [TestMethod()]
        public void GetCurrencyDataTest() {
            //Assign
            var mockManager = MockRepository.GenerateMock<ICentreGroupRepository>();

            //Act
            List<CurrencyExtended> currencies = mockManager.GetCurrencyData(0, 0, 0);

            //Assert
            mockManager.AssertWasCalled(x => x.GetCurrencyData(0, 0, 0));
        }

        [TestMethod()]
        public void GetActiveCentreDataTest() {
            //Assign
            var mockManager = MockRepository.GenerateMock<ICentreGroupRepository>();

            //Act
            List<CentreExtended> centreExtended = mockManager.GetActiveCentreData();

            //Assert
            mockManager.AssertWasCalled(x => x.GetActiveCentreData());
        }

        [TestMethod()]
        public void GetCgActiveCentreDataTest() {
            //Assign
            var mockManager = MockRepository.GenerateMock<ICentreGroupRepository>();

            //Act
            List<Centre> centre = mockManager.GetCgActiveCentreData(0);

            //Assert
            mockManager.AssertWasCalled(x => x.GetCgActiveCentreData(0));
        }

        [TestMethod()]
        public void GetCgCurrenciesTest() {
            //Assign
            var mockManager = MockRepository.GenerateMock<ICentreGroupRepository>();

            //Act
            List<Currency> centre = mockManager.GetCgCurrencies(0);

            //Assert
            mockManager.AssertWasCalled(x => x.GetCgCurrencies(0));
        }

        [TestMethod()]
        public void GetActiveParticipantsDataTest() {
            //Assign
            var mockManager = MockRepository.GenerateMock<IRegetAdminController>();

            //Act
            ActionResult result = mockManager.GetActiveParticipantsData();

            //Assert
            mockManager.AssertWasCalled(x => x.GetActiveParticipantsData());
        }

        [TestMethod()]
        public void IsAuthorizedTest() {
            //Assign
            var mockManager = MockRepository.GenerateMock<ICentreGroupRepository>();

            //Act
            bool result = mockManager.IsAuthorized(0, 0, 0);

            //Assert
            mockManager.AssertWasCalled(x => x.IsAuthorized(0, 0, 0));
        }

        [TestMethod()]
        public void DeletePurchaseGroupTest() {
            //Assign
            var mockManager = MockRepository.GenerateMock<ICentreGroupRepository>();

            //Act
            bool result = mockManager.DeletePurchaseGroup(0, 0);

            //Assert
            mockManager.AssertWasCalled(x => x.DeletePurchaseGroup(0, 0));
        }
    }
}