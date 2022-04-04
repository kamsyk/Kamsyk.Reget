/// <reference path="../typings/jasmine/jasmine.d.ts" />
/// <reference path="../../../Kamsyk.Reget/Scripts/RegetTypeScript/cg-admin.ts" />
describe("Cg Admin Controller", function () {
    var cgAdminController;
    var $scope;
    var $http;
    var $filter;
    var $mdDialog;
    var $mdToast;
    var uiGridConstants;
    var $q;
    var $timeout;
    var $compile;
    beforeEach(function () {
        //spyOn(window, "Kamsyk.RegetApp.CgAdminController");
        var getGeneralStats_Spy = spyOn(Kamsyk.RegetApp.CgAdminController.prototype, "loadInit");
        cgAdminController = new Kamsyk.RegetApp.CgAdminController($scope, $http, $filter, $mdDialog, $mdToast, $q, $timeout, $compile);
    });
    //Stub
    it("isAllCurreenciesChecked", function () {
        // Arrange
        //spyOn(cgAdminController, "isAllCurreenciesChecked");
        // Act
        var isAllChecked = cgAdminController.isAllCurreenciesChecked();
        // Assert
        expect(isAllChecked).toBe(false);
    });
});
//# sourceMappingURL=cg-admin-test.js.map