/// <reference path="../typings/jasmine/jasmine.d.ts" />
/// <reference path="../../../Kamsyk.Reget/Scripts/RegetTypeScript/centre.ts" />
describe("Centre", function () {
    var centreController;
    var $scope;
    var $http;
    var $filter;
    var $mdDialog;
    var $mdToast;
    var uiGridConstants;
    var $q;
    var $timeout;
    var centreData = null;
    var companyData = null;
    beforeEach(function () {
        //companyData = new DataGridTestAux().getCompanyData();
        spyOn(Kamsyk.RegetApp.CentreController.prototype, "getCompanies");
        centreController = new Kamsyk.RegetApp.CentreController($scope, $http, $filter, $mdDialog, $mdToast, uiGridConstants, $q, $timeout);
    });
    it("Export To Xls Url", function () {
        expect(centreController.exportToXlsUrl()).not.toBeNull();
    });
    it("Get Save Url", function () {
        expect(centreController.getSaveRowUrl()).not.toBeNull();
    });
    it("Control Columns Count", function () {
        expect(centreController.getControlColumnsCount()).toBe(2);
    });
    it("Get Duplicity Error Msg", function () {
        //Arrange
        spyOn(centreController, "getLocDuplicityCentreNameText").and.returnValue("Duplicity centre {0}");
        //Act
        var centre = new Kamsyk.RegetApp.Centre(0, "Centre1");
        //Assert
        expect(centreController.getDuplicityErrMsg(centre)).toBe("Duplicity centre Centre1");
    });
    it("Is Row Changed", function () {
        //Arrange
        var origCentre = new Kamsyk.RegetApp.CentreAdminExtended();
        origCentre.id = 0;
        origCentre.name = "Centre Test";
        origCentre.address_text = "Address";
        origCentre.company_name = "Company";
        origCentre.export_price_text = "No";
        origCentre.is_approved_by_requestor = false;
        origCentre.manager_id = 1;
        origCentre.manager_name = "Man Man1";
        origCentre.multi_orderer = false;
        origCentre.other_orderer_can_takeover = false;
        origCentre.active = true;
        centreController.editRowOrig = origCentre;
        //Act
        var isCorrect = true;
        var editCentre = angular.copy(origCentre);
        centreController.editRow = editCentre;
        isCorrect = !(centreController.isRowChanged());
        if (isCorrect) {
            editCentre = angular.copy(origCentre);
            editCentre.name = "Changed";
            centreController.editRow = editCentre;
            isCorrect = (centreController.isRowChanged());
        }
        if (isCorrect) {
            editCentre = angular.copy(origCentre);
            editCentre.address_text = "Changed";
            centreController.editRow = editCentre;
            isCorrect = (centreController.isRowChanged());
        }
        if (isCorrect) {
            editCentre = angular.copy(origCentre);
            editCentre.export_price_text = "Changed";
            centreController.editRow = editCentre;
            isCorrect = (centreController.isRowChanged());
        }
        if (isCorrect) {
            editCentre = angular.copy(origCentre);
            editCentre.is_approved_by_requestor = true;
            centreController.editRow = editCentre;
            isCorrect = (centreController.isRowChanged());
        }
        if (isCorrect) {
            editCentre = angular.copy(origCentre);
            editCentre.manager_id = 54;
            centreController.editRow = editCentre;
            isCorrect = (centreController.isRowChanged());
        }
        if (isCorrect) {
            editCentre = angular.copy(origCentre);
            editCentre.multi_orderer = true;
            centreController.editRow = editCentre;
            isCorrect = (centreController.isRowChanged());
        }
        if (isCorrect) {
            editCentre = angular.copy(origCentre);
            editCentre.other_orderer_can_takeover = true;
            centreController.editRow = editCentre;
            isCorrect = (centreController.isRowChanged());
        }
        if (isCorrect) {
            editCentre = angular.copy(origCentre);
            editCentre.active = false;
            centreController.editRow = editCentre;
            isCorrect = (centreController.isRowChanged());
        }
        //Assert
        expect(isCorrect).toBe(true);
    });
});
//# sourceMappingURL=centre-test.js.map