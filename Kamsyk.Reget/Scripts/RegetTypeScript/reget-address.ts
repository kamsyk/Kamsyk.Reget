/// <reference path="../typings/ui-grid/ui-grid.d.ts" />

module Kamsyk.RegetApp {
    export class AddressController extends BaseRegetGridTs implements angular.IController {
        //**************************************************************
        //Properties
        private isAddressesLoaded: boolean = false;
        private isCompanyLoaded: boolean = false;
        private companies: AgDropDown[] = null;
        //**************************************************************

        //*************************************************************
        //Get Loc Texts
        private locWarningText: string = $("#WarningText").val();
        private locErrMsg: string = $("#ErrMsgText").val();
        private locCompanyText: string = $("#CompanyText").val();
        private locCompanyNameText: string = $("#CompanyNameText").val();
        private locStreetText: string = $("#StreetText").val();
        private locCityText: string = $("#CityText").val();
        private locZipText: string = $("#ZipText").val();
        private locDeleteAddressConfirmText: string = $("#DeleteAddressConfirmText").val();
        private locNotFoundText: string = $("#NotFoundText").val();
        private locDuplicityAddressText: string = $("#DuplicityAddressText").val();
        
        //*************************************************************

        //**********************************************************
        //Constructor
        constructor(
            protected $scope: ng.IScope,
            protected $http: ng.IHttpService,
            protected $filter: ng.IFilterService,
            protected $mdDialog: angular.material.IDialogService,
            protected $mdToast: angular.material.IToastService,
            protected uiGridConstants: uiGrid.IUiGridConstants,
            protected $q: ng.IQService,
            protected $timeout: ng.ITimeoutService

        ) {
            super($scope, $http, $filter, $mdDialog, $mdToast, $q, uiGridConstants, $timeout);
            this.setGrid();
            this.loadData();

        }
        //***************************************************************


        $onInit = () => { };

        //****************************************************************************
        //Abstract
        public exportToXlsUrl(): string {
            return this.getRegetRootUrl() + "Report/GetAddressesReport?" +
                "filter=" + encodeURI(this.filterUrl) +
                "&sort=" + this.sortColumnsUrl +
                "&t=" + new Date().getTime();
        }

        public getControlColumnsCount(): number {
            return 2;
        }
                
        public getDuplicityErrMsg(rowEntity: Address): string {
            var addressText : string =  this.getFullAddress(rowEntity);
            return this.locDuplicityAddressText.replace("{0}", addressText);
        }

        public getSaveRowUrl(): string {
            return this.getRegetRootUrl() + "Address/SaveAddressData?t=" + new Date().getTime();
        }

        public insertRow(): void {
            var newAddress: Address = new Address();

            newAddress.id = -10;
            newAddress.company_name = "";
            newAddress.company_name_text = "";
            newAddress.street = "";
            newAddress.city = "";
            newAddress.zip = "";
                                   
            this.insertBaseRow(newAddress);
        }

        public isRowChanged(): boolean {

            if (this.editRow === null) {
                return true;
            }

            var origAddress: Address = this.editRowOrig;
            var updAddress: Address = this.editRow;
            var tmpAddresses: any = this.gridOptions.data;
            var addresses: Address[] = tmpAddresses;
            var isChanged: boolean = false;

            var id: number = updAddress.id;
            var address: Address[] = this.$filter("filter")(addresses, { id: id }, true);


            if (id < 0) {
                //new row
                this.newRowIndex = null;

                return true;
            } else {
                if (origAddress.company_name !== updAddress.company_name) {
                    return true;
                } else if (origAddress.company_name_text !== updAddress.company_name_text) {
                    return true;
                } else if (origAddress.street !== updAddress.street) {
                    return true;
                } else if (origAddress.city !== updAddress.city) {
                    return true;
                } else if (origAddress.zip !== updAddress.zip) {
                    return true;
                }
            }

            return false;
        }

        public isRowEntityValid(address: Address): string {

            if (this.isStringValueNullOrEmpty(address.company_name)) {
                return this.locMissingMandatoryText;
            }

            if (this.isStringValueNullOrEmpty(address.company_name_text)) {
                return this.locMissingMandatoryText;
            }

            return null;
        }

        public loadGridData(): void {
            this.getAddressData();
        }

        public dbGridId: string = "grdAddress_rg";

        public getMsgDisabled(address: Address): string {
            return null;//this.locSupplierWasDisabledText.replace("{0}", supplier.supp_name);
        }


        public getMsgDeleteConfirm(address: Address): string {
            return this.locDeleteAddressConfirmText.replace("{0}", address.company_name);;
        }

        public getErrorMsgByErrId(errId: number, msg: string): string {
            return this.locErrorMsgText;
        }

        public deleteUrl: string = this.getRegetRootUrl() + "Address/DeleteAddress" + "?t=" + new Date().getTime();

        public getDbGridId(): string {
            return this.dbGridId;
        }
        //*****************************************************************************

        //****************************************************************************
        //http

        private getAddressData() {
            this.showLoader(this.isError);

            this.$http.get(
                this.getRegetRootUrl() + "Address/GetAddressesAdminData?filter=" + encodeURI(this.filterUrl) +
                "&pageSize=" + this.pageSize +
                "&currentPage=" + this.currentPage +
                "&sort=" + this.sortColumnsUrl +
                "&t=" + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    this.gridOptions.data = tmpData.db_data;
                    this.rowsCount = tmpData.rows_count;

                    if (!this.isValueNullOrUndefined(this.gridApi)) {
                        this.gridApi.core.notifyDataChange(this.uiGridConstants.dataChange.COLUMN);
                    }

                    this.isAddressesLoaded = true;
                    this.testLoadDataCount++;

                    //if (this.userGridSettings === null) {
                    //    this.defaultColumnDefs = angular.copy(this.gridOptions.columnDefs);
                    //    this.getDataGridSettings();
                    //}
                    //this.loadGridSettings();
                    this.setGridSettingData();

                    //********************************************************************
                    //it is very important otherwise 50 lines are nod diplaye dproperly !!!
                    this.gridOptions.virtualizationThreshold = this.rowsCount + 1;
                    //********************************************************************

                    this.hideLoaderWrapper();
                } catch (e) {
                    this.hideLoader();
                    this.displayErrorMsg();
                } finally {
                    this.hideLoaderWrapper();
                }
            }, (response: any) => {
                this.hideLoader();
                this.displayErrorMsg();
            });
        }

        private getCompanies() : any {
            this.showLoader(this.isError);

            this.$http.get(
                this.getRegetRootUrl() + "Participant/GetParticipantCompanies?t=" + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    this.companies = tmpData;

                    //company
                    var pos: number = this.getColumnIndex("company_name_text");
                    if (pos > 0) {
                        this.gridOptions.columnDefs[pos].filter = {
                            //term: "1",
                            type: this.uiGridConstants.filter.SELECT,
                            selectOptions: this.companies

                        };
                        //var filCompanies: uiGrid.edit.IEditDropdown[] = this.companies;
                        this.gridOptions.columnDefs[pos].editDropdownOptionsArray = this.companies;
                        this.gridApi.core.notifyDataChange(this.uiGridConstants.dataChange.COLUMN);
                    }

                    //this.loadGridData();
                    this.getLoadDataGridSettings();

                    this.isCompanyLoaded = true;
                    this.hideLoaderWrapper();
                } catch (e) {
                    this.hideLoader();
                    this.displayErrorMsg();
                } finally {
                    this.hideLoaderWrapper();
                }
            }, (response: any) => {
                this.hideLoader();
                this.displayErrorMsg();
            });
  
        }
        //******************************************************************************

        //****************************************************************************
        //Methods
        private setGrid(): void {
            this.gridOptions.columnDefs = [
                {
                    name: "row_index",
                    field: "row_index",
                    displayName: "",
                    enableFiltering: false,
                    enableSorting: false,
                    enableCellEdit: false,
                    width: 40,
                    enableColumnResizing: true,
                    cellTemplate: "<div class=\"ui-grid-cell-contents ui-grid-top-panel\" style=\"text-align:center;vertical-align:middle;font-weight:normal;\">{{COL_FIELD}}</div>"
                },
                {
                    name: "action_buttons",
                    displayName: "",
                    enableFiltering: false,
                    enableSorting: false,
                    enableCellEdit: false,
                    enableHiding: false,
                    enableColumnResizing: false,
                    width: 70,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellAction.html"

                },
                {
                    name: "company_name_text", displayName: this.locCompanyText, field: "company_name_text",
                    enableHiding: false,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellDropDownMandatoryTemplate.html",
                    enableCellEdit: false,
                    minWidth: 110
                },
                {
                    name: "company_name", displayName: this.locCompanyNameText, field: "company_name",
                    enableHiding: false,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextMandatoryTemplate.html",
                    enableCellEdit: false,
                    minWidth: 110
                },
                {
                    name: "street", displayName: this.locStreetText, field: "street",
                    enableHiding: false,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextTemplate.html",
                    enableCellEdit: false,
                    sortingAlgorithm: (a, b, rowA, rowB, direction) => {
                        return this.sortNullString(a, b, rowA, rowB, direction);
                    },
                    minWidth: 110
                },
                {
                    name: "city", displayName: this.locCityText, field: "city",
                    enableHiding: false,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextTemplate.html",
                    enableCellEdit: false,
                    sortingAlgorithm: (a, b, rowA, rowB, direction) => {
                        return this.sortNullString(a, b, rowA, rowB, direction);
                    },
                    minWidth: 110
                },
                {
                    name: "zip", displayName: this.locZipText, field: "zip",
                    enableHiding: false,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextTemplate.html",
                    enableCellEdit: false,
                    sortingAlgorithm: (a, b, rowA, rowB, direction) => {
                        return this.sortNullString(a, b, rowA, rowB, direction);
                    },
                    minWidth: 50
                }
            ];


        }

        private loadData(): void {
            this.getCompanies();
            
        }

        private hideLoaderWrapper() {
            if (this.isError || (this.isAddressesLoaded && this.isCompanyLoaded)) {
                this.hideLoader();
                this.isError = false;
            }
        }

        private getFullAddress(address : Address) : string {
            var strFullAddress : string = address.company_name;
            if (!this.isStringValueNullOrEmpty(address.street)) {
                strFullAddress += ", " + address.street;
            }
            if (!this.isStringValueNullOrEmpty(address.city)) {
                strFullAddress += ", " + address.city;
            }
            if (!this.isStringValueNullOrEmpty(address.zip)) {
                strFullAddress += ", " + address.zip;
            }

            return strFullAddress;
        }

        //****************************************************************************
    }
    angular.
        module("RegetApp").
        controller("AddressController", Kamsyk.RegetApp.AddressController);
}