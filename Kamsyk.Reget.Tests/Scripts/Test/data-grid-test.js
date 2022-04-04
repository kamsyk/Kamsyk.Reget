/// <reference path="../typings/jasmine/jasmine.d.ts" />
/// <reference path="../../../Kamsyk.Reget/Scripts/RegetTypeScript/centre.ts" />
/// <reference path="../Test/TestBase/data-grid-base-test.ts" />
describe("Data Grid - Centre Grid Is Used", function () {
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
        companyData = new DataGridTestAux().getCompanyData();
        spyOn(Kamsyk.RegetApp.CentreController.prototype, "getCompanies").and.returnValue(companyData);
        centreController = new Kamsyk.RegetApp.CentreController($scope, $http, $filter, $mdDialog, $mdToast, uiGridConstants, $q, $timeout);
    });
    //it("Export To Xls Url", () => {
    //    expect(centreController.exportToXlsUrl()).not.toBeNull();
    //});
    //it("Get Save Url", () => {
    //    expect(centreController.getSaveRowUrl()).not.toBeNull();
    //});
    //it("Control Columns Count", () => {
    //    expect(centreController.getControlColumnsCount()).toBe(2);
    //});
    //it("Get Duplicity Error Msg", () => {
    //    //Arrange
    //    spyOn(centreController, "getLocDuplicityCentreNameText").and.returnValue("Duplicity centre {0}");
    //    //Act
    //    let centre: Kamsyk.RegetApp.Centre = new Kamsyk.RegetApp.Centre(0, "Centre1");
    //    //Assert
    //    expect(centreController.getDuplicityErrMsg(centre)).toBe("Duplicity centre Centre1");
    //});
    it("Insert Row", function () {
        //Arrange
        spyOn(centreController, "clearFilters");
        spyOn(centreController, "formatCentreGridLookups");
        spyOn(centreController, "editRowChanged");
        spyOn(centreController, "gridSaveRow");
        var centres = new DataGridTestAux().getCentreData(centreController.pageSize);
        centreController.gridOptions.data = centres;
        var displayRowsCountBefore = centreController.gridOptions.data.length;
        //Act
        var newRowIndex = DataGridTestAux.getNewRowIndex(centreController.pageSize, centres.length);
        spyOn(centreController, "getNewRowIndex").and.returnValue(newRowIndex);
        centreController.insertRow();
        //Assert
        var displayRowsCountAfter = centreController.gridOptions.data.length;
        expect(displayRowsCountBefore + 1).toBe(displayRowsCountAfter);
    });
    it("Add New Row", function () {
        //Arrange
        spyOn(centreController, "clearFilters");
        spyOn(centreController, "formatCentreGridLookups");
        spyOn(centreController, "editRowChanged");
        spyOn(centreController, "gridSaveRow");
        var centres = new DataGridTestAux().getCentreData(centreController.pageSize);
        centreController.gridOptions.data = centres;
        var displayRowsCountBefore = centreController.gridOptions.data.length;
        //Act
        var newRowIndex = DataGridTestAux.getNewRowIndex(centreController.pageSize, centres.length);
        spyOn(centreController, "getNewRowIndex").and.returnValue(newRowIndex);
        centreController.addNewRow();
        //Assert
        var displayRowsCountAfter = centreController.gridOptions.data.length;
        expect(displayRowsCountBefore + 1).toBe(displayRowsCountAfter);
    });
    it("firstPage", function () {
        // Arrange
        spyOn(centreController, "gridSaveRow").and.returnValue(true);
        // Act
        centreController.lastPage();
        centreController.firstPage();
        // Assert
        expect(centreController.currentPage).toBe(1);
    });
    it("lastPage 82 - 9", function () {
        // Arrange
        spyOn(centreController, "gridSaveRow").and.returnValue(true);
        centreController.currentPage = 1;
        // Act
        centreController.rowsCount = 82;
        centreController.pageSize = 10;
        centreController.lastPage();
        // Assert
        expect(centreController.currentPage).toBe(9);
    });
    it("lastPage 0 - 1", function () {
        // Arrange
        spyOn(centreController, "gridSaveRow").and.returnValue(true);
        centreController.currentPage = 1;
        // Act
        centreController.rowsCount = 0;
        centreController.pageSize = 10;
        centreController.lastPage();
        // Assert
        expect(centreController.currentPage).toBe(0);
    });
    it("lastPage 0 - 1, Curr Page = 0", function () {
        // Arrange
        spyOn(centreController, "gridSaveRow").and.returnValue(true);
        centreController.currentPage = 0;
        // Act
        centreController.rowsCount = 0;
        centreController.pageSize = 10;
        centreController.lastPage();
        // Assert
        expect(centreController.currentPage).toBe(0);
    });
});
var DataGridTestAux = /** @class */ (function () {
    function DataGridTestAux() {
    }
    DataGridTestAux.prototype.getCompanyData = function () {
        var companies = [];
        var company = new Kamsyk.RegetApp.AgDropDown();
        company.id = "0";
        company.label = "Test comp";
        company.value = "Test comp";
        companies.push(company);
        return companies;
    };
    DataGridTestAux.prototype.getCentreData = function (pageSize) {
        var centres = [];
        for (var i = 0; i < pageSize; i++) {
            var centre = new Kamsyk.RegetApp.CentreAdminExtended();
            centre.id = 0;
            centre.name = "Centre_" + i.toString();
            centre.active = true;
            centres.push(centre);
        }
        return centres;
    };
    DataGridTestAux.getNewRowIndex = function (pageSize, gridRowsCount) {
        var newRowIndex = pageSize;
        if (gridRowsCount < pageSize || gridRowsCount > pageSize) {
            newRowIndex = gridRowsCount;
        }
        if (newRowIndex < 0) {
            newRowIndex = 0;
        }
        return newRowIndex;
    };
    return DataGridTestAux;
}());
//# sourceMappingURL=data-grid-test.js.map