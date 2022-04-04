/// <reference path="../typings/jasmine/jasmine.d.ts" />
/// <reference path="../../../Kamsyk.Reget/Scripts/RegetTypeScript/request.ts" />
/// <reference path="../../../Kamsyk.Reget/Scripts/typings/ng-file-upload/ng-file-upload.d.ts" />
describe("Request Controller", function () {
    var requestController;
    var $scope;
    var $http;
    var $filter;
    var $mdDialog;
    var $mdToast;
    var uiGridConstants;
    var $q;
    var $timeout;
    var $compile;
    var Upload;
    beforeEach(function () {
        //spyOn(requestController, "$scope.$watch");;
        //spyOn(requestController, "$scope.$watch");
        //let getWatch_Spy = spyOn(Kamsyk.RegetApp.RequestController.prototype, "$scope.$watch");
        var getLoadInit_Spy = spyOn(Kamsyk.RegetApp.RequestController.prototype, "loadInit");
        requestController = new Kamsyk.RegetApp.RequestController($scope, $http, $filter, $mdDialog, $mdToast, uiGridConstants, $q, $timeout, Upload, $compile);
    });
    it("Update Empty App Men", function () {
        //Arrange
        requestController.request = new Kamsyk.RegetApp.Request();
        //Act
        requestController.updateAppMen();
        //Asset
        var appManCount = requestController.request.request_event_approval.length;
        expect(appManCount).toEqual(0);
    });
    it("Privacy Not Authorized", function () {
        //Arrange
        requestController.request = new Kamsyk.RegetApp.Request();
        var getLoadInit_Spy = spyOn(Kamsyk.RegetApp.RequestController.prototype, "showAlert");
        document.body.innerHTML =
            '<div id="divRequestPlaceholder"></div>'
                + '<div id="divNotAuthorizedPlaceholder"></div>';
        var result = {};
        result.string_value = "NotAuthorized";
        result.request_nr = "111";
        //Act
        requestController.getRequestLoadedStatus(result);
        //Asset
        var divRequestPlaceholder = document.getElementById("divRequestPlaceholder");
        var divNotAuthorizedPlaceholder = document.getElementById("divNotAuthorizedPlaceholder");
        var isOk = true;
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
    it("Is Request Valid App Man Mandarory Check", function () {
        //Arrange
        requestController.request = new Kamsyk.RegetApp.Request();
        //Act
        //Asset
        expect(true).toEqual(false);
    });
    it("Is Request Valid Is Orderer Mandarory Check", function () {
        //Arrange
        requestController.request = new Kamsyk.RegetApp.Request();
        //Act
        //Asset
        expect(true).toEqual(false);
    });
    it("Is Request Valid Is Multi Orderer Mandarory Check", function () {
        //Arrange
        requestController.request = new Kamsyk.RegetApp.Request();
        //Act
        //Asset
        expect(true).toEqual(false);
    });
});
//# sourceMappingURL=request-test.js.map