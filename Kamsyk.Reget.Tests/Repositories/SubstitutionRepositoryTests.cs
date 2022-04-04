using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kamsyk.Reget.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Mocks;
using Kamsyk.Reget.Model.Repositories.Interfaces;

namespace Kamsyk.Reget.Model.Repositories.Tests {
    [TestClass()]
    public class SubstitutionRepositoryTests {
        [TestMethod()]
        public void GetSubstitutedParticipantsDataTest() {
            //Assign
            var mockManager = MockRepository.GenerateMock<ISubstitutionRepository>();

            //Act
            mockManager.GetSubstitutedParticipantsData(null, null, 0);

            //Assert
            mockManager.AssertWasCalled(x => x.GetSubstitutedParticipantsData(null, null, 0));
        }
    }
}