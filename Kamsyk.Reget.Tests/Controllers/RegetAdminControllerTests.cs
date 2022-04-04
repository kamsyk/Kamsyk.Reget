using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kamsyk.Reget.Controllers;
using Kamsyk.Reget.Controllers.Interface;
using Kamsyk.Reget.Model.ExtendedModel;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Kamsyk.Reget.Controllers.Tests {
    [TestClass()]
    public class RegetAdminControllerTests {
        [TestMethod()]
        public void AministrationIndex_AdminHomeDisplay() {
            // Arrange
            RegetAdminController controller = new RegetAdminController();

            // Act
            ViewResult result = controller.Index(null) as ViewResult;

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));


        }

        [TestMethod()]
        public void GetCentreGroupDataByIdTest() {
            // Arrange
            RegetAdminController controller = new RegetAdminController();

            // Act
            ViewResult result = controller.Index(null) as ViewResult;

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod()]
        public void IsCentreGroupAdminCgReadOnlyTest() {
            // Arrange
            IRegetAdminController regetAdminController = MockRepository.GenerateStub<IRegetAdminController>();

            // Act
            var GetCgAdminRoles = regetAdminController.Stub(x => x.GetCgAdminRoles(0, 0));

            // Assert
            Assert.IsNotNull(GetCgAdminRoles);
        }

        [TestMethod()]
        public void GetCgActiveCentresTest() {
            // Arrange
            IRegetAdminController regetAdminController = MockRepository.GenerateStub<IRegetAdminController>();

            // Act
            var json = regetAdminController.GetCgActiveCentres(0);

            //Assert
            regetAdminController.VerifyAllExpectations();

        }

        [TestMethod()]
        public void SaveCentreGroupDataTest() {
            // Arrange
            IRegetAdminController regetAdminController = MockRepository.GenerateStub<IRegetAdminController>();

            // Act
            regetAdminController.SaveCentreGroupData(new Model.ExtendedModel.CentreGroupExtended());

            //Assert
            regetAdminController.VerifyAllExpectations();
        }

        [TestMethod()]
        public void GetPurchaseGroupsByCgIdTest() {
            // Arrange
            IRegetAdminController regetAdminController = MockRepository.GenerateStub<IRegetAdminController>();

            // Act
            ActionResult result = regetAdminController.GetPurchaseGroupsByCgId(0, 0, true, 1);

            //Assert
            regetAdminController.VerifyAllExpectations();
        }


        [TestMethod()]
        public void GetActiveParticipantsDataTest() {
            // Arrange
            IRegetAdminController regetAdminController = MockRepository.GenerateStub<IRegetAdminController>();

            // Act
            ActionResult result = regetAdminController.GetActiveParticipantsData();

            //Assert
            regetAdminController.VerifyAllExpectations();
        }

        [TestMethod()]
        public void SavePurchaseGroupDataTest() {
            // Arrange
            IRegetAdminController regetAdminController = MockRepository.GenerateStub<IRegetAdminController>();

            // Act
            PurchaseGroupExtended pg = new PurchaseGroupExtended();
            regetAdminController.SavePurchaseGroupData(pg);

            //Assert
            regetAdminController.VerifyAllExpectations();
        }

       
    }
}