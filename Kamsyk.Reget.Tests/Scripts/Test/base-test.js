/// <reference path="../typings/jasmine/jasmine.d.ts" />
/// <reference path="../../../Kamsyk.Reget/Scripts/RegetTypeScript/Base/reget-base.ts" />
/// <reference path="../../../Kamsyk.Reget/Scripts/RegetTypeScript/statistics.ts" />
describe("Statistics Is Used To Test Script functions - ", function () {
    var statisticsController;
    var $scope;
    var $http;
    var $filter;
    var $mdDialog;
    var $mdToast;
    var $q;
    var $timeout;
    beforeEach(function () {
        spyOn(Kamsyk.RegetApp.StatisticsController.prototype, "loadData");
        statisticsController = new Kamsyk.RegetApp.StatisticsController($scope, $http, $filter, $mdDialog, $mdToast, $q, $timeout);
    });
    it("Remove Form Relative Position", function () {
        //Arrange
        var dummyForm = document.createElement('form');
        dummyForm.style.position = "relative";
        var forms = [];
        forms.push(dummyForm);
        document.getElementsByTagName = jasmine.createSpy('HTML Form').and.returnValue(forms);
        //Act
        statisticsController.removeFormReleativePosition();
        //Assert
        expect(dummyForm.style.position).toBe("");
    });
    it("Root Url", function () {
        //Arrange
        //Act
        var rootUrl = statisticsController.getRegetRootUrl();
        //Assert
        expect(rootUrl.length).toBeGreaterThan(1);
    });
    it("Is Value Null, Undefined", function () {
        //Arrange
        var isValid = true;
        //Act
        var valTest = null;
        if (isValid) {
            isValid = statisticsController.isValueNullOrUndefined(valTest);
        }
        valTest = undefined;
        if (isValid) {
            isValid = statisticsController.isValueNullOrUndefined(valTest);
        }
        valTest = "";
        if (isValid) {
            isValid = statisticsController.isValueNullOrUndefined(valTest);
        }
        valTest = {};
        if (isValid) {
            isValid = !statisticsController.isValueNullOrUndefined(valTest);
        }
        valTest = [];
        if (isValid) {
            isValid = !statisticsController.isValueNullOrUndefined(valTest);
        }
        valTest = 0;
        if (isValid) {
            isValid = !statisticsController.isValueNullOrUndefined(valTest);
        }
        valTest = "text";
        if (isValid) {
            isValid = !statisticsController.isValueNullOrUndefined(valTest);
        }
        //Assert
        expect(isValid).toBe(true);
    });
    it("Is String Value Null, Empty", function () {
        //Arrange
        var isValid = true;
        //Act
        var valTest = null;
        if (isValid) {
            isValid = statisticsController.isStringValueNullOrEmpty(valTest);
        }
        valTest = undefined;
        if (isValid) {
            isValid = statisticsController.isStringValueNullOrEmpty(valTest);
        }
        valTest = "";
        if (isValid) {
            isValid = statisticsController.isStringValueNullOrEmpty(valTest);
        }
        valTest = " ";
        if (isValid) {
            isValid = statisticsController.isStringValueNullOrEmpty(valTest);
        }
        valTest = " c ";
        if (isValid) {
            isValid = !statisticsController.isStringValueNullOrEmpty(valTest);
        }
        //Assert
        expect(isValid).toBe(true);
    });
    it("Compare Moment Date", function () {
        //Arrange
        var isCorrect = true;
        //Act
        var date1 = new Date("2019-01-16");
        var date2 = new Date("2019-01-16");
        if (isCorrect) {
            isCorrect = statisticsController.isMomentDatesSame(date1, date2);
        }
        date1 = new Date("2019-01-16");
        date2 = new Date("2019-02-16");
        if (isCorrect) {
            isCorrect = !statisticsController.isMomentDatesSame(date1, date2);
        }
        date1 = new Date("2019-02-16");
        date2 = new Date("2019-02-17");
        if (isCorrect) {
            isCorrect = !statisticsController.isMomentDatesSame(date1, date2);
        }
        date1 = new Date("2019-02-17");
        date2 = new Date("2018-02-17");
        if (isCorrect) {
            isCorrect = !statisticsController.isMomentDatesSame(date1, date2);
        }
        date1 = new Date("2019-02-17");
        date2 = new Date("2018-82-17");
        if (isCorrect) {
            isCorrect = !statisticsController.isMomentDatesSame(date1, date2);
        }
        date1 = new Date(2019, 1, 16, 13, 25, 29, 15);
        date2 = new Date(2019, 1, 16, 13, 25, 29, 15);
        if (isCorrect) {
            isCorrect = statisticsController.isMomentDatesSame(date1, date2);
        }
        date1 = new Date(2019, 1, 16, 14, 25, 29, 15);
        date2 = new Date(2019, 1, 16, 13, 25, 29, 15);
        if (isCorrect) {
            isCorrect = !statisticsController.isMomentDatesSame(date1, date2);
        }
        date1 = new Date(2019, 1, 16, 13, 24, 29, 15);
        date2 = new Date(2019, 1, 16, 13, 25, 29, 15);
        if (isCorrect) {
            isCorrect = !statisticsController.isMomentDatesSame(date1, date2);
        }
        date1 = new Date(2019, 1, 16, 13, 25, 28, 15);
        date2 = new Date(2019, 1, 16, 13, 25, 29, 15);
        if (isCorrect) {
            isCorrect = !statisticsController.isMomentDatesSame(date1, date2);
        }
        date1 = new Date(2019, 1, 16, 13, 25, 29, 15);
        date2 = new Date(2019, 1, 16, 13, 25, 29, 14);
        if (isCorrect) {
            isCorrect = !statisticsController.isMomentDatesSame(date1, date2);
        }
        //Assert
        expect(isCorrect).toBe(true);
    });
    it("Convert Text to Number", function () {
        //Arrange
        var isCorrect = true;
        spyOn(statisticsController, "getDecimalSeparator").and.returnValue(",");
        //Act
        if (isCorrect) {
            var numbRes = statisticsController.convertTextToDecimal("1");
            isCorrect = (numbRes == 1);
        }
        if (isCorrect) {
            var numbRes = statisticsController.convertTextToDecimal("1,6");
            isCorrect = (numbRes == 1.6);
        }
        if (isCorrect) {
            var numbRes = statisticsController.convertTextToDecimal("1.8");
            isCorrect = (numbRes == 1.8);
        }
        if (isCorrect) {
            var numbRes = statisticsController.convertTextToDecimal("-42 451,5");
            isCorrect = (numbRes == -42451.5);
        }
        if (isCorrect) {
            var numbRes = statisticsController.convertTextToDecimal("c-42 451,5");
            isCorrect = (isNaN(numbRes));
        }
        //Assert
        expect(isCorrect).toBe(true);
    });
    it("Is Decimal Number - comma separator", function () {
        //Arrange
        var isCorrect = true;
        spyOn(statisticsController, "getDecimalSeparator").and.returnValue(",");
        //Act
        if (isCorrect) {
            isCorrect = statisticsController.isStringDecimalNumber("1");
        }
        if (isCorrect) {
            isCorrect = statisticsController.isStringDecimalNumber("-61");
        }
        if (isCorrect) {
            isCorrect = statisticsController.isStringDecimalNumber("-56,85");
        }
        if (isCorrect) {
            isCorrect = statisticsController.isStringDecimalNumber("18,80");
        }
        if (isCorrect) {
            isCorrect = statisticsController.isStringDecimalNumber("100,00");
        }
        if (isCorrect) {
            isCorrect = statisticsController.isStringDecimalNumber("3500");
        }
        if (isCorrect) {
            isCorrect = statisticsController.isStringDecimalNumber("052,00");
        }
        if (isCorrect) {
            isCorrect = statisticsController.isStringDecimalNumber("0");
        }
        if (isCorrect) {
            isCorrect = statisticsController.isStringDecimalNumber("0,4");
        }
        if (isCorrect) {
            isCorrect = !statisticsController.isStringDecimalNumber("-56.85");
        }
        if (isCorrect) {
            isCorrect = !statisticsController.isStringDecimalNumber("-556.85");
        }
        if (isCorrect) {
            isCorrect = !statisticsController.isStringDecimalNumber("15000,1,0");
        }
        if (isCorrect) {
            isCorrect = !statisticsController.isStringDecimalNumber("15000,1,");
        }
        if (isCorrect) {
            isCorrect = !statisticsController.isStringDecimalNumber(null);
        }
        if (isCorrect) {
            isCorrect = !statisticsController.isStringDecimalNumber("");
        }
        if (isCorrect) {
            isCorrect = !statisticsController.isStringDecimalNumber("daqfqw");
        }
        //Assert
        expect(isCorrect).toBe(true);
    });
    it("Is Decimal Number - dot separator", function () {
        //Arrange
        var isCorrect = true;
        spyOn(statisticsController, "getDecimalSeparator").and.returnValue(".");
        //Act
        if (isCorrect) {
            isCorrect = statisticsController.isStringDecimalNumber("1");
        }
        if (isCorrect) {
            isCorrect = statisticsController.isStringDecimalNumber("-61");
        }
        if (isCorrect) {
            isCorrect = statisticsController.isStringDecimalNumber("-56.85");
        }
        if (isCorrect) {
            isCorrect = statisticsController.isStringDecimalNumber("18.80");
        }
        if (isCorrect) {
            isCorrect = statisticsController.isStringDecimalNumber("100.00");
        }
        if (isCorrect) {
            isCorrect = statisticsController.isStringDecimalNumber("3500");
        }
        if (isCorrect) {
            isCorrect = statisticsController.isStringDecimalNumber("052.00");
        }
        if (isCorrect) {
            isCorrect = statisticsController.isStringDecimalNumber("0");
        }
        if (isCorrect) {
            isCorrect = statisticsController.isStringDecimalNumber("0.4");
        }
        if (isCorrect) {
            isCorrect = !statisticsController.isStringDecimalNumber("-56,85");
        }
        if (isCorrect) {
            isCorrect = !statisticsController.isStringDecimalNumber("-556,85");
        }
        if (isCorrect) {
            isCorrect = !statisticsController.isStringDecimalNumber("15000.1.0");
        }
        if (isCorrect) {
            isCorrect = !statisticsController.isStringDecimalNumber("15000.1.");
        }
        if (isCorrect) {
            isCorrect = !statisticsController.isStringDecimalNumber(null);
        }
        if (isCorrect) {
            isCorrect = !statisticsController.isStringDecimalNumber("");
        }
        if (isCorrect) {
            isCorrect = !statisticsController.isStringDecimalNumber("daqfqw");
        }
        //Assert
        expect(isCorrect).toBe(true);
    });
    it("Remove Diacritics", function () {
        //Arrange
        var isCorrect = true;
        //Act
        if (isCorrect) {
            isCorrect = (statisticsController.removeDiacritics("ěščřžýáíé") == "escrzyaie");
        }
        if (isCorrect) {
            isCorrect = (statisticsController.removeDiacritics("ŤÚŘ") == "TUR");
        }
        if (isCorrect) {
            isCorrect = (statisticsController.removeDiacritics("Ž") == "Z");
        }
        if (isCorrect) {
            isCorrect = (statisticsController.removeDiacritics("ĂÖñąɃ") == "AOnaB");
        }
        //Assert
        expect(isCorrect).toBe(true);
    });
    it("Convert Date to DbString", function () {
        //Arrange
        var isCorrect = true;
        //Act
        if (isCorrect) {
            var testDate = new Date(2020, 0, 31);
            isCorrect = (statisticsController.convertDateToDbString(testDate) == "2020-01-31");
        }
        if (isCorrect) {
            var testDate = new Date(2020, 5, 15, 6, 8, 2, 3);
            isCorrect = (statisticsController.convertDateToDbString(testDate) == "2020-06-15");
        }
        if (isCorrect) {
            var testDate = new Date(2020, 1, 29, 2, 8, 2, 3);
            isCorrect = (statisticsController.convertDateToDbString(testDate) == "2020-02-29");
        }
        //Assert
        expect(isCorrect).toBe(true);
    });
    it("Validate Decimal Number", function () {
        //Arrange
        var isCorrect = true;
        //Act
        if (isCorrect) {
            try {
                statisticsController.validateDecimalNumber("1", null);
            }
            catch (_a) {
                isCorrect = false;
            }
        }
        //Assert
        expect(isCorrect).toBe(true);
    });
    it("Get Error Message Are all of the strings covered", function () {
        //Arrange
        var isCorrect = true;
        document.body.innerHTML =
            '<div>' +
                '  <input id="MsgNotAuthorizedPerformActionText" value="Not Authorized" />' +
                '  <input id="MsgMissingMandatoryFieldText" value="Missing Mandatory Field" />' +
                '  <input id="MsgRequestCannotBeRevertedText" value="Request Cannot Be Reverted" />' +
                '</div>';
        //Act
        if (isCorrect) {
            try {
                var errMsg = statisticsController.getErrorMessage("NotAuthorized");
                isCorrect = isCorrect && (errMsg == "Not Authorized");
                errMsg = statisticsController.getErrorMessage("MissingMandatoryItem");
                isCorrect = isCorrect && (errMsg == "Missing Mandatory Field");
                errMsg = statisticsController.getErrorMessage("RequestCannotBeReverted");
                isCorrect = isCorrect && (errMsg == "Request Cannot Be Reverted");
            }
            catch (_a) {
                isCorrect = false;
            }
        }
        //Assert
        expect(isCorrect).toBe(true);
    });
});
//# sourceMappingURL=base-test.js.map