using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kamsyk.Reget.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Controllers.Tests {
    [TestClass()]
    public class StatisticsControllerTests {
        [TestMethod()]
        public void GetChartLabelsTest() {
            //Assign
            StatisticsController statisticsController = new StatisticsController();
            DateTime dateFrom = new DateTime(2019, 1, 1);
            DateTime dateTo = new DateTime(2019, 4, 9);
            int periodType = 30; //month

            //Act
            List<StatisticsController.Period> allPeriods = new StatisticsController().GetAllPeriods(periodType, dateFrom, dateTo);
            List<string> labels = statisticsController.GetChartLabels(periodType, dateFrom, dateTo, allPeriods);

            //Assert
            Assert.IsTrue(labels.Count == 4);
        }
    }
}