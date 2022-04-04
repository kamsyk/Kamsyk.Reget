using System.Linq;
using System.Data.Entity;
using System;

namespace Kamsyk.Reget.Model.Repositories {
    public class TestFailRepository : BaseRepository<Test_Fail> {
        #region Methods
        public void SaveTestFail(string testName, string errMsg) {
            var lastId = (from tfDb in m_dbContext.Test_Fail
                          orderby tfDb.id descending
                          select new { tfDb.id}).Take(1).FirstOrDefault();

            int nextId = 0;
            if (lastId != null) {
                nextId = lastId.id;
                nextId++;
            }

            Test_Fail testFail = new Test_Fail();
            testFail.id = nextId;
            testFail.test_name = testName;
            testFail.error_msg = errMsg;
            testFail.fail_date = DateTime.Now;

            m_dbContext.Test_Fail.Add(testFail);

            m_dbContext.SaveChanges();
        }
        #endregion
    }
}