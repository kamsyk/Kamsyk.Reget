/// <reference path="../typings/jasmine/jasmine.d.ts" />
/// <reference path="../../../Kamsyk.Reget/Scripts/RegetTypeScript/request.ts" />
/// <reference path="../../../Kamsyk.Reget/Scripts/typings/ng-file-upload/ng-file-upload.d.ts" />

describe("Request Controller", () => {
    let requestController: Kamsyk.RegetApp.RequestController;

    let $scope: ng.IScope;
    let $http: ng.IHttpService;
    let $filter: ng.IFilterService;
    let $mdDialog: angular.material.IDialogService;
    let $mdToast: angular.material.IToastService;
    let uiGridConstants: uiGrid.IUiGridConstants;
    let $q: ng.IQService;
    let $timeout: ng.ITimeoutService;
    let $compile: ng.ICompileService;
    let Upload: ng.angularFileUpload.IUploadService;

    beforeEach(() => {
        //spyOn(requestController, "$scope.$watch");;
        //spyOn(requestController, "$scope.$watch");
        //let getWatch_Spy = spyOn(Kamsyk.RegetApp.RequestController.prototype, "$scope.$watch");
        let getLoadInit_Spy = spyOn(Kamsyk.RegetApp.RequestController.prototype, "loadInit");

        requestController = new Kamsyk.RegetApp.RequestController(
            $scope,
            $http,
            $filter,
            $mdDialog,
            $mdToast,
            uiGridConstants,
            $q,
            $timeout,
            Upload,
            $compile);
    });

    it("Update Empty App Men", () => {
        //Arrange
        requestController.request = new Kamsyk.RegetApp.Request();

        //Act
        requestController.updateAppMen();

        //Asset
        let appManCount: number = requestController.request.request_event_approval.length;
        expect(appManCount).toEqual(0);
    });
        
    it("Privacy Not Authorized", () => {
        //Arrange
        requestController.request = new Kamsyk.RegetApp.Request();
        let getLoadInit_Spy = spyOn(Kamsyk.RegetApp.RequestController.prototype, "showAlert");
        document.body.innerHTML =
            '<div id="divRequestPlaceholder"></div>'
            + '<div id="divNotAuthorizedPlaceholder"></div>';

        let result: any = {};
        result.string_value = "NotAuthorized";
        result.request_nr = "111";

        //Act
        requestController.getRequestLoadedStatus(result);

        //Asset
        let divRequestPlaceholder : HTMLDivElement = <HTMLDivElement> document.getElementById("divRequestPlaceholder");
        let divNotAuthorizedPlaceholder: HTMLDivElement = <HTMLDivElement>  document.getElementById("divNotAuthorizedPlaceholder");

        let isOk = true;
        isOk = isOk && (divRequestPlaceholder.style.display === "none");
        isOk = isOk && (divNotAuthorizedPlaceholder.style.display === "block");

        expect(isOk).toEqual(true); 
    });

    //it("Is Draft Not Authorized", () => {
    //    //Arrange
    //    requestController.request = new Kamsyk.RegetApp.Request();

    //    //Act
    //    requestController.updateAppMen();

    //    //Asset
    //    //let appManCount: number = requestController.request.request_event_approval.length;
    //    expect(true).toEqual(false);
    //});

    it("Is Request Valid App Man Mandarory Check", () => {
        //Arrange
        requestController.request = new Kamsyk.RegetApp.Request();

        //Act
       

        //Asset
        expect(true).toEqual(false);
    });

    it("Is Request Valid Is Orderer Mandarory Check", () => {
        //Arrange
        requestController.request = new Kamsyk.RegetApp.Request();

        //Act
        

        //Asset
        expect(true).toEqual(false);
    });

    it("Is Request Valid Is Multi Orderer Mandarory Check", () => {
        //Arrange
        requestController.request = new Kamsyk.RegetApp.Request();

        //Act
        

        //Asset
        expect(true).toEqual(false);
    });

});