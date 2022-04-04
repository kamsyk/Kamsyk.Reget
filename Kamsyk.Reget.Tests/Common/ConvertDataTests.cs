//using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kamsyk.Reget.Model.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Kamsyk.Reget.Model.Common.Tests {
    //[TestClass()]
    public class ConvertDataTests {
        [Fact]
        public void RemoveDiacriticsTest() {
            //Arrange
            string strTextCz = "ěščřžýáíéÚů";
            string strTextPl = "ąćŃŁŹ";
            string strTextRu = "БбЖжЛл";

            //Act
            string strTextCzConvert = ConvertData.RemoveDiacritics(strTextCz);
            string strTextPlConvert = ConvertData.RemoveDiacritics(strTextPl);
            string strTextRuConvert = ConvertData.RemoveDiacritics(strTextRu);

            //Asert
            bool isOk = true;
            if (strTextCzConvert != "escrzyaieUu") {
                isOk = false;
            }
            if (strTextPlConvert != "acNLZ") {
                isOk = false;
            }
            if (strTextRuConvert != "БбЖжЛл") {
                isOk = false;
            }
            Assert.True(isOk);
        }

        [Fact]
        public void ConvertDateToText() {
            //Arrange
            DateTime date = new DateTime(2020, 1, 25);
            bool isOK = true;

            //Act
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            string strDate = ConvertData.ToTextFromDate(date);
            isOK = isOK && strDate == "1/25/2020";

            Thread.CurrentThread.CurrentCulture = new CultureInfo("cs-CZ");
            strDate = ConvertData.ToTextFromDate(date);
            isOK = isOK && strDate == "25.01.2020";

            //Asert
            Assert.True(isOK);
        }

        [Fact()]
        public void GetStringValueTest() {
            var strRes = ConvertData.GetStringValue(2.150M, "cs-CZ", false);
            if (strRes != "2,150") {
                Assert.True(false, "2.150");
            }

            strRes = ConvertData.GetStringValue(2.130M, "en-US", false);
            if (strRes != "2.130") {
                Assert.True(false, "2.130");
            }

            Assert.True(true);
        }

        [Fact()]
        public void GetStringValueRemoveUselessZerosTest() {
            var strRes = ConvertData.GetStringValueRemoveUselessZeros(2.150M, "cs-CZ", false);
            if (strRes != "2,15") {
                Assert.True(false, "2.150");
            }

            strRes = ConvertData.GetStringValueRemoveUselessZeros(2.130M, "en-US", false);
            if (strRes != "2.13") {
                Assert.True(false, "2.130");
            }

            strRes = ConvertData.GetStringValueRemoveUselessZeros(2.0M, "cs-CZ", false);
            if (strRes != "2") {
                Assert.True(false, "2.0");
            }

            Assert.True(true);
        }

        //[Theory]
        //[InlineData(1, 2, 3)]
        //[InlineData(-4, -6, -10)]
        //[InlineData(-2, 2, 0)]
        //[InlineData(int.MinValue, -1, int.MaxValue)]
        //public void CanAddTheory(int value1, int value2, int expected) {
        //    ConvertData.ToTextFromDate();

        //     var result = calculator.Add(value1, value2);

        //    Assert.Equal(expected, result);
        //}

    }
}