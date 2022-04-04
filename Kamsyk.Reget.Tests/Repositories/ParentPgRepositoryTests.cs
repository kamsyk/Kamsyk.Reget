using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kamsyk.Reget.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kamsyk.Reget.Model.Repositories.Interfaces;
using Rhino.Mocks;
using Kamsyk.Reget.Model.ExtendedModel.PurchaseGroup;

namespace Kamsyk.Reget.Model.Repositories.Tests {
    [TestClass()]
    public class ParentPgRepositoryTests {
        [TestMethod()]
        public void SaveParentPgTest() {
            //Assign
            var mockManager = MockRepository.GenerateMock<IParentPgRepository>();

            //Act
            mockManager.SaveParentPg(new ParentPgExtended());

            //Assert
            mockManager.AssertWasCalled(x => x.SaveParentPg(Arg<ParentPgExtended>.Is.Anything));
            
        }
    }
}