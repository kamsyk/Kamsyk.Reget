/// <reference path="../typings/jasmine/jasmine.d.ts" />
/// <reference path="../../../Kamsyk.Reget/Scripts/RegetTypeScript/cg-admin.ts" />


describe("Cg Admin Controller", () => {
    var cgAdminController: Kamsyk.RegetApp.CgAdminController;

    var $scope: ng.IScope;
    var $http: ng.IHttpService;
    var $filter: ng.IFilterService;
    var $mdDialog: angular.material.IDialogService;
    var $mdToast: angular.material.IToastService;
    var uiGridConstants: uiGrid.IUiGridConstants;
    var $q: ng.IQService;
    var $timeout: ng.ITimeoutService;
    var $compile: ng.ICompileService;

    beforeEach(() => {
        //spyOn(window, "Kamsyk.RegetApp.CgAdminController");
        let getGeneralStats_Spy = spyOn(Kamsyk.RegetApp.CgAdminController.prototype, "loadInit");

        cgAdminController = new Kamsyk.RegetApp.CgAdminController(
            $scope,
            $http,
            $filter,
            $mdDialog,
            $mdToast,
            $q,
            $timeout,
            $compile);
    });

        
    //Stub
    it("isAllCurreenciesChecked", () => {
        // Arrange
        //spyOn(cgAdminController, "isAllCurreenciesChecked");

        // Act
        let isAllChecked:boolean = cgAdminController.isAllCurreenciesChecked();
        
        // Assert
        expect(isAllChecked).toBe(false);
    });

});