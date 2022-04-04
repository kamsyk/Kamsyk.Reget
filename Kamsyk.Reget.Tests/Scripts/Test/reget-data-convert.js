/// <reference path="../typings/jasmine/jasmine.d.ts" />
/// <reference path="../../../Kamsyk.Reget/Scripts/RegetTypeScript/Base/reget-data-convert.ts" />
describe("Convert to Decimal", function () {
    //let regetDataConvert: RegetDataConvert;
    var $scope;
    var $http;
    var $filter;
    var $mdDialog;
    var $mdToast;
    var $q;
    var $timeout;
    it("Convert To Decimal", function () {
        //Arrange
        var strNumber = "3,5";
        var dNumber = strNumber;
        //Act
        var convDecNumber = RegetDataConvert.convertDecimal(dNumber);
        //Assert
        expect(convDecNumber).toBe(dNumber);
    });
});
//# sourceMappingURL=reget-data-convert.js.map