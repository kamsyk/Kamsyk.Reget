using Kamsyk.Reget.TestsIntegration.BaseTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kamsyk.Reget.TestsIntegration.Controllers {
   
    public class SearchControllerTest : BaseTestIntegration {
        //+ " ON rtd." + RequestTextData.REQUEST_ID_FIELD + "=red." + RequestEventData.ID_FIELD

        #region Constructor
        public SearchControllerTest() {
            if (!IsTestDbConnected) {
                throw new Exception("Test DB is not connected");
            }
        }
        #endregion

        [Fact]
        public void ZzInt_UnexpectedSearchText_NoError() {
            string searchText = "\" ON rtd.\" + RequestTextData.REQUEST_ID_FIELD + \" = red.\" + RequestEventData.ID_FIELD";

            Assert.True(false);
        }
    }
}
