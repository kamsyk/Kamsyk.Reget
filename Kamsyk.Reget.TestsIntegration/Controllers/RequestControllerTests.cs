using Xunit;
using Kamsyk.Reget.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.TestsIntegration.Controllers {
    public class RequestControllerTests {
        [Fact()]
        public void GetRequest_ForeignCurrency_MissingExchangeRate() {
            Assert.True(false, "This test needs an implementation");
            //Assert.True(true);
        }

        [Fact()]
        public void ChangeOrderer_NewOrdererIsAuthorized() {
            Assert.True(false, "This test needs an implementation");
            
        }

        [Fact()]
        public void SubstituteeUser_IsAuthorizedForSubstitutedsRequests() {
            Assert.True(false, "This test needs an implementation");

        }

        [Fact()]
        public void MassingMandatoryItem_MsgIsDisplayed() {
            Assert.True(false, "This test needs an implementation");

        }

        [Fact()]
        public void SaveWithoutCentre_SaveAsDraft() {
            Assert.True(false, "This test needs an implementation");

        }

        [Fact()]
        public void RevertToDraft_DifferentAppMatrix() {
            Assert.True(false, "This test needs an implementation");

        }
    }
}