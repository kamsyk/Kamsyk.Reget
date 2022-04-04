using Kamsyk.Reget.Model.Repositories.Interfaces;
using System.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Kamsyk.Reget.Model.ExtendedModel;
using Kamsyk.Reget.Model.Common;
using Kamsyk.Reget.Model.DataDictionary;
using System.Collections;

namespace Kamsyk.Reget.Model.Repositories {
    public class TestIndicatorRepository : BaseRepository<Test_Indicator> {
        #region Methods
        public void SetTestIndicatorText(string testText) {
            var testIndicator = (from dbInd in m_dbContext.Test_Indicator
                                 where dbInd.id == 0
                                 select dbInd).FirstOrDefault();
            if (testIndicator == null) {
                testIndicator = new Test_Indicator();
                testIndicator.id = 0;
                testIndicator.test_text = testText;

                m_dbContext.Test_Indicator.Add(testIndicator);
            } else {

                testIndicator.test_text = testText;
            }

            m_dbContext.SaveChanges();
        }


        public string GetTestIndicatorText() {
            var testIndicator = (from dbInd in m_dbContext.Test_Indicator
                                 where dbInd.id == 0
                                 select dbInd).FirstOrDefault();
            if (testIndicator == null) {
                return null;
            }

            return testIndicator.test_text;
        }      
        #endregion
    }
}
