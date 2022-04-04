/// <reference path="../typings/jasmine/jasmine.d.ts" />
/// <reference path="../../../Kamsyk.Reget/Scripts/RegetTypeScript/Base/reget-base.ts" />
/// <reference path="../../../Kamsyk.Reget/Scripts/RegetTypeScript/statistics.ts" />

describe("Statistics Is Used To Test Script functions - ", () => {
    let statisticsController: Kamsyk.RegetApp.StatisticsController;

    let $scope: ng.IScope;
    let $http: ng.IHttpService;
    let $filter: ng.IFilterService;
    let $mdDialog: angular.material.IDialogService;
    let $mdToast: angular.material.IToastService;
    let $q: ng.IQService;
    let $timeout: ng.ITimeoutService;
        
    beforeEach(() => {
        spyOn(Kamsyk.RegetApp.StatisticsController.prototype, "loadData");
        
        statisticsController = new Kamsyk.RegetApp.StatisticsController(
            $scope,
            $http,
            $filter,
            $mdDialog,
            $mdToast,
            $q,
            $timeout);
    });
        
    it("Remove Form Relative Position", () => {
        //Arrange
        let dummyForm: HTMLFormElement = document.createElement('form');
        dummyForm.style.position = "relative";
        let forms: HTMLFormElement[] = [];
        forms.push(dummyForm);
        document.getElementsByTagName = jasmine.createSpy('HTML Form').and.returnValue(forms);
        
        //Act
        statisticsController.removeFormReleativePosition();

        //Assert
        expect(dummyForm.style.position).toBe("");
    });

    it("Root Url", () => {
        //Arrange
        
        //Act
        let rootUrl: string = statisticsController.getRegetRootUrl();

        //Assert
        expect(rootUrl.length).toBeGreaterThan(1);
    });

    it("Is Value Null, Undefined", () => {
        //Arrange
        let isValid: boolean = true;

        //Act
        let valTest: any = null;
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

    it("Is String Value Null, Empty", () => {
        //Arrange
        let isValid: boolean = true;

        //Act
        let valTest: string = null;
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

    it("Compare Moment Date", () => {
        //Arrange
        let isCorrect: boolean = true;

        //Act
        let date1: Date = new Date("2019-01-16");
        let date2: Date = new Date("2019-01-16");
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

    it("Convert Text to Number", () => {
        //Arrange
        let isCorrect: boolean = true;
        spyOn(statisticsController, "getDecimalSeparator").and.returnValue(",");
        
        //Act
        if (isCorrect) {
            let numbRes: number = statisticsController.convertTextToDecimal("1");
            isCorrect = (numbRes == 1);
        }

        if (isCorrect) {
            let numbRes: number = statisticsController.convertTextToDecimal("1,6");
            isCorrect = (numbRes == 1.6);
        }

        if (isCorrect) {
            let numbRes: number = statisticsController.convertTextToDecimal("1.8");
            isCorrect = (numbRes == 1.8);
        }

        if (isCorrect) {
            let numbRes: number = statisticsController.convertTextToDecimal("-42 451,5");
            isCorrect = (numbRes == -42451.5);
        }

        if (isCorrect) {
            let numbRes: number = statisticsController.convertTextToDecimal("c-42 451,5");
            isCorrect = (isNaN(numbRes));
        }

        //Assert
        expect(isCorrect).toBe(true);
    });

    it("Is Decimal Number - comma separator", () => {
        //Arrange
        let isCorrect: boolean = true;
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

    it("Is Decimal Number - dot separator", () => {
        //Arrange
        let isCorrect: boolean = true;
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

    it("Remove Diacritics", () => {
        //Arrange
        let isCorrect: boolean = true;

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

    it("Convert Date to DbString", () => {
        //Arrange
        let isCorrect: boolean = true;

        //Act
        if (isCorrect) {
            let testDate: Date = new Date(2020, 0, 31);
            isCorrect = (statisticsController.convertDateToDbString(testDate) == "2020-01-31");
        }

        if (isCorrect) {
            let testDate: Date = new Date(2020,5,15,6,8,2,3);
            isCorrect = (statisticsController.convertDateToDbString(testDate) == "2020-06-15");
        }

        if (isCorrect) {
            let testDate: Date = new Date(2020, 1, 29, 2, 8, 2, 3);
            isCorrect = (statisticsController.convertDateToDbString(testDate) == "2020-02-29");
        }
        
        //Assert
        expect(isCorrect).toBe(true);
    });

    it("Validate Decimal Number", () => {
        //Arrange
        let isCorrect: boolean = true;

        //Act
        if (isCorrect) {
            try {
                statisticsController.validateDecimalNumber("1", null);
            } catch {
                isCorrect = false;
            }
        }
                
        //Assert
        expect(isCorrect).toBe(true);
    });

    it("Get Error Message Are all of the strings covered", () => {
        //Arrange
        let isCorrect: boolean = true;
        document.body.innerHTML =
            '<div>' +
            '  <input id="MsgNotAuthorizedPerformActionText" value="Not Authorized" />' +
            '  <input id="MsgMissingMandatoryFieldText" value="Missing Mandatory Field" />' +
            '  <input id="MsgRequestCannotBeRevertedText" value="Request Cannot Be Reverted" />' +
            '</div>';
        
        //Act
        if (isCorrect) {
            try {
                let errMsg: string = statisticsController.getErrorMessage("NotAuthorized");
                isCorrect = isCorrect && (errMsg == "Not Authorized");

                errMsg = statisticsController.getErrorMessage("MissingMandatoryItem");
                isCorrect = isCorrect && (errMsg == "Missing Mandatory Field");

                errMsg = statisticsController.getErrorMessage("RequestCannotBeReverted");
                isCorrect = isCorrect && (errMsg == "Request Cannot Be Reverted");
            } catch {
                isCorrect = false;
            }
        }

        //Assert
        expect(isCorrect).toBe(true);
    });
});
