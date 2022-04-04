/// <reference path="../typings/jasmine/jasmine.d.ts" />
/// <reference path="../../../Kamsyk.Reget/Scripts/RegetTypeScript/centre.ts" />
/// <reference path="../Test/TestBase/data-grid-base-test.ts" />

describe("Data Grid - Centre Grid Is Used", () => {
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
        companyData = new DataGridTestAux().getCompanyData();
        spyOn(Kamsyk.RegetApp.CentreController.prototype, "getCompanies").and.returnValue(companyData);
        
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

    it("Insert Row", () => {
        //Arrange
        spyOn(centreController, "clearFilters");
        spyOn(centreController, "formatCentreGridLookups");
        spyOn(centreController, "editRowChanged");
        spyOn(centreController, "gridSaveRow");

        let centres: Kamsyk.RegetApp.CentreAdminExtended[] = new DataGridTestAux().getCentreData(centreController.pageSize);
        centreController.gridOptions.data = centres;
        let displayRowsCountBefore: number = centreController.gridOptions.data.length;

        //Act
        let newRowIndex: number = DataGridTestAux.getNewRowIndex(centreController.pageSize, centres.length);
        spyOn(centreController, "getNewRowIndex").and.returnValue(newRowIndex);
        centreController.insertRow();

        //Assert
        let displayRowsCountAfter: number = centreController.gridOptions.data.length;
        expect(displayRowsCountBefore + 1).toBe(displayRowsCountAfter);
    });

    it("Add New Row", () => {
        //Arrange
        spyOn(centreController, "clearFilters");
        spyOn(centreController, "formatCentreGridLookups");
        spyOn(centreController, "editRowChanged");
        spyOn(centreController, "gridSaveRow");
                                                        
        let centres: Kamsyk.RegetApp.CentreAdminExtended[] = new DataGridTestAux().getCentreData(centreController.pageSize);
        centreController.gridOptions.data = centres;
        let displayRowsCountBefore: number = centreController.gridOptions.data.length;

        //Act
        let newRowIndex: number = DataGridTestAux.getNewRowIndex(centreController.pageSize, centres.length);
        spyOn(centreController, "getNewRowIndex").and.returnValue(newRowIndex);
        centreController.addNewRow(); 

        //Assert
        let displayRowsCountAfter: number = centreController.gridOptions.data.length;
        expect(displayRowsCountBefore + 1).toBe(displayRowsCountAfter);
    });

   
    it("firstPage", () => {
        // Arrange
        spyOn(centreController, "gridSaveRow").and.returnValue(true);
                
        // Act
        centreController.lastPage();
        centreController.firstPage();

        // Assert
        expect(centreController.currentPage).toBe(1);
    });

    it("lastPage 82 - 9", () => {
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

    it("lastPage 0 - 1", () => {
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

    it("lastPage 0 - 1, Curr Page = 0", () => {
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

class DataGridTestAux {
    public getCompanyData(): Kamsyk.RegetApp.AgDropDown[] {
        let companies: Kamsyk.RegetApp.AgDropDown[] = [];
        let company: Kamsyk.RegetApp.AgDropDown = new Kamsyk.RegetApp.AgDropDown();
        company.id = "0";
        company.label = "Test comp";
        company.value = "Test comp";
        companies.push(company);

        return companies;
    }

    public getCentreData(pageSize: number): Kamsyk.RegetApp.CentreAdminExtended[] {
        let centres: Kamsyk.RegetApp.CentreAdminExtended[] = [];

        for (let i: number = 0; i < pageSize; i++) {
            let centre: Kamsyk.RegetApp.CentreAdminExtended = new Kamsyk.RegetApp.CentreAdminExtended();
            
            centre.id = 0;
            centre.name = "Centre_" + i.toString()
            centre.active = true;

            centres.push(centre);
        }

        return centres;
    } 

    public static getNewRowIndex(pageSize: number, gridRowsCount: number): number {
        let newRowIndex = pageSize;
        if (gridRowsCount < pageSize || gridRowsCount > pageSize) {
            newRowIndex = gridRowsCount;
        }

        if (newRowIndex < 0) {
            newRowIndex = 0;
        }

        return newRowIndex;
    }
}