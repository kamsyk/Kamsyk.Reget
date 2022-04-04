/// <reference path="../typings/jasmine/jasmine.d.ts" />
/// <reference path="../../../Kamsyk.Reget/Scripts/RegetTypeScript/centre.ts" />

describe("Centre", () => {
    let centreController: Kamsyk.RegetApp.CentreController;

    let $scope: ng.IScope;
    let $http: ng.IHttpService;
    let $filter: ng.IFilterService;
    let $mdDialog: angular.material.IDialogService;
    let $mdToast: angular.material.IToastService;
    let uiGridConstants: uiGrid.IUiGridConstants;
    let $q: ng.IQService;
    let $timeout: ng.ITimeoutService;

    let centreData: Kamsyk.RegetApp.Centre[] = null;
    let companyData: Kamsyk.RegetApp.AgDropDown[] = null;

    beforeEach(() => {
        //companyData = new DataGridTestAux().getCompanyData();
        spyOn(Kamsyk.RegetApp.CentreController.prototype, "getCompanies");

        centreController = new Kamsyk.RegetApp.CentreController(
            $scope,
            $http,
            $filter,
            $mdDialog,
            $mdToast,
            uiGridConstants,
            $q,
            $timeout);
    });

    it("Export To Xls Url", () => {
        expect(centreController.exportToXlsUrl()).not.toBeNull();
    });

    it("Get Save Url", () => {
        expect(centreController.getSaveRowUrl()).not.toBeNull();
    });

    it("Control Columns Count", () => {
        expect(centreController.getControlColumnsCount()).toBe(2);
    });

    it("Get Duplicity Error Msg", () => {
        //Arrange
        spyOn(centreController, "getLocDuplicityCentreNameText").and.returnValue("Duplicity centre {0}");

        //Act
        let centre: Kamsyk.RegetApp.Centre = new Kamsyk.RegetApp.Centre(0, "Centre1");

        //Assert
        expect(centreController.getDuplicityErrMsg(centre)).toBe("Duplicity centre Centre1");
    });

    it("Is Row Changed", () => {
        //Arrange
        let origCentre: Kamsyk.RegetApp.CentreAdminExtended = new Kamsyk.RegetApp.CentreAdminExtended();
        origCentre.id = 0
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
        let isCorrect: boolean = true;
        let editCentre: Kamsyk.RegetApp.CentreAdminExtended = angular.copy(origCentre);
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