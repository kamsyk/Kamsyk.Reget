/// <reference path="../typings/jasmine/jasmine.d.ts" />
/// <reference path="../../../Kamsyk.Reget/Scripts/RegetTypeScript/Base/reget-data-convert.ts" />
describe("Convert Data", function () {
    //let regetDataConvert: RegetDataConvert;
    var $scope;
    var $http;
    var $filter;
    var $mdDialog;
    var $mdToast;
    var $q;
    var $timeout;
    beforeEach(function () {
    });
    it("Convert String To Decimal with comma", function () {
        //Arrange
        var strNumber = "3,5";
        //Act
        var convDecNumber = RegetDataConvert.convertStringToDecimal(strNumber);
        //Assert
        expect(convDecNumber).toBe(3.5);
    });
    it("Convert String To Decimal with dot", function () {
        //Arrange
        var strNumber = "3.5";
        //Act
        var convDecNumber = RegetDataConvert.convertStringToDecimal(strNumber);
        //Assert
        expect(convDecNumber).toBe(3.5);
    });
    it("Convert String To Decimal no decimal part", function () {
        //Arrange
        var strNumber = "4";
        //Act
        var convDecNumber = RegetDataConvert.convertStringToDecimal(strNumber);
        //Assert
        expect(convDecNumber).toBe(4);
    });
    it("Convert String To Decimal not a number", function () {
        //Arrange
        var strNumber = "ew3.5";
        //Act
        var isError = false;
        try {
            var convDecNumber = RegetDataConvert.convertStringToDecimal(strNumber);
        }
        catch (e) {
            isError = true;
        }
        //Assert
        expect(isError).toBe(true);
    });
    it("Convert Decimal To String - All decimal numbers comma decimal separator", function () {
        //Arrange
        if (document.getElementById("DecimalSeparator") == null) {
            document.body.innerHTML =
                '<div>' +
                    '  <input id="DecimalSeparator" value="," />' +
                    '  <button id="button" />' +
                    '</div>';
        }
        var decNumber = 5.59876;
        //Act
        var strDecNumber = RegetDataConvert.convertDecimalToString(decNumber);
        //Assert
        expect(strDecNumber).toBe("5,59876");
    });
    it("Convert Decimal To String - 2 decimal numbers comma decimal separator", function () {
        //Arrange
        if (document.getElementById("DecimalSeparator") == null) {
            document.body.innerHTML =
                '<div>' +
                    '  <input id="DecimalSeparator" value="," />' +
                    '  <button id="button" />' +
                    '</div>';
        }
        var decNumber = 5.59876;
        //Act
        var strDecNumber = RegetDataConvert.convertDecimalToString(decNumber, 2);
        //Assert
        expect(strDecNumber).toBe("5,60");
    });
    it("Convert Decimal To String - 2 decimal numbers dot decimal separator", function () {
        //Arrange
        if (document.getElementById("DecimalSeparator") == null) {
            document.body.innerHTML =
                '<div>' +
                    '  <input id="DecimalSeparator" value="," />' +
                    '  <button id="button" />' +
                    '</div>';
        }
        var inputDecSep = document.getElementById("DecimalSeparator");
        inputDecSep.value = ".";
        var decNumber = 5.59876;
        //Act
        var strDecNumber = RegetDataConvert.convertDecimalToString(decNumber, 2);
        //Assert
        expect(strDecNumber).toBe("5.60");
    });
    it("Convert Decimal To String not a number", function () {
        //Arrange
        var strNumber = "ew3.5";
        //Act
        var isError = false;
        try {
            var strDecNumber = RegetDataConvert.convertDecimalToString(strNumber);
        }
        catch (e) {
            isError = true;
        }
        //Assert
        expect(isError).toBe(true);
    });
});
//# sourceMappingURL=reget-data-convert-test.js.map