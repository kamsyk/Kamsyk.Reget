/// <reference path="../typings/jasmine/jasmine.d.ts" />
/// <reference path="../../../Kamsyk.Reget/Scripts/RegetTypeScript/Base/reget-data-convert.ts" />

describe("Convert Data", () => {
    //let regetDataConvert: RegetDataConvert;

    let $scope: ng.IScope;
    let $http: ng.IHttpService;
    let $filter: ng.IFilterService;
    let $mdDialog: angular.material.IDialogService;
    let $mdToast: angular.material.IToastService;
    let $q: ng.IQService;
    let $timeout: ng.ITimeoutService;

    beforeEach(() => {
        
    });
           
    it("Convert String To Decimal with comma", () => {
        //Arrange
        let strNumber: string = "3,5";
                
        //Act
        let convDecNumber: number = RegetDataConvert.convertStringToDecimal(strNumber);

        //Assert
        expect(convDecNumber).toBe(3.5);
    });

    it("Convert String To Decimal with dot", () => {
        //Arrange
        let strNumber: string = "3.5";
        
        //Act
        let convDecNumber: number = RegetDataConvert.convertStringToDecimal(strNumber);

        //Assert
        expect(convDecNumber).toBe(3.5);
    });

    it("Convert String To Decimal no decimal part", () => {
        //Arrange
        let strNumber: string = "4";
        
        //Act
        let convDecNumber: number = RegetDataConvert.convertStringToDecimal(strNumber);

        //Assert
        expect(convDecNumber).toBe(4);
    });

    it("Convert String To Decimal not a number", () => {
        //Arrange
        let strNumber: string = "ew3.5";
        
        //Act
        let isError: boolean = false;
        try {
            let convDecNumber: number = RegetDataConvert.convertStringToDecimal(strNumber);
        } catch(e) {
            isError = true;
        }

        //Assert
        expect(isError).toBe(true);
    });

    it("Convert Decimal To String - All decimal numbers comma decimal separator", () => {
        //Arrange
        if (document.getElementById("DecimalSeparator") == null) {
            document.body.innerHTML =
                '<div>' +
                '  <input id="DecimalSeparator" value="," />' +
                '  <button id="button" />' +
                '</div>';
        }
        let decNumber: number = 5.59876;

        //Act
        let strDecNumber: string = RegetDataConvert.convertDecimalToString(decNumber);
        
        //Assert
        expect(strDecNumber).toBe("5,59876");
    });

    it("Convert Decimal To String - 2 decimal numbers comma decimal separator", () => {
        //Arrange
        if (document.getElementById("DecimalSeparator") == null) {
            document.body.innerHTML =
                '<div>' +
                '  <input id="DecimalSeparator" value="," />' +
                '  <button id="button" />' +
                '</div>';
        }
        let decNumber: number = 5.59876;

        //Act
        let strDecNumber: string = RegetDataConvert.convertDecimalToString(decNumber, 2);

        //Assert
        expect(strDecNumber).toBe("5,60");
    });

    it("Convert Decimal To String - 2 decimal numbers dot decimal separator", () => {
        //Arrange
        if (document.getElementById("DecimalSeparator") == null) {
            document.body.innerHTML =
                '<div>' +
                '  <input id="DecimalSeparator" value="," />' +
                '  <button id="button" />' +
                '</div>';
        }
        let inputDecSep: HTMLInputElement = <HTMLInputElement>document.getElementById("DecimalSeparator");
        inputDecSep.value = ".";

        let decNumber: number = 5.59876;

        //Act
        let strDecNumber: string = RegetDataConvert.convertDecimalToString(decNumber, 2);

        //Assert
        expect(strDecNumber).toBe("5.60");
    });

    it("Convert Decimal To String not a number", () => {
        //Arrange
        let strNumber: any = "ew3.5";

        //Act
        let isError: boolean = false;
        try {
            let strDecNumber: string = RegetDataConvert.convertDecimalToString(strNumber);
        } catch (e) {
            isError = true;
        }

        //Assert
        expect(isError).toBe(true);
    });

});
