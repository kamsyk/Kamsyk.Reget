/// <reference path="../typings/ui-grid/ui-grid.d.ts" />
/// <reference path="../RegetTypeScript/Base/reget-base-grid.ts" />
/// <reference path="../RegetTypeScript/Base/reget-entity.ts" />


module Kamsyk.RegetApp {
    export class CentreController extends BaseRegetGridTs implements angular.IController {
        //**********************************************************************
        //Loc Texts
        private locWarningText: string = $("#WarningText").val();
        private locNeverText: string  = $("#NeverText").val();
        private locAlwaysText: string  = $("#AlwaysText").val();
        private locOptionalText: string  = $("#OptionalText").val();
        private locNameText: string  = $("#NameText").val();
        private locCompanyText: string  = $("#CompanyText").val();
        private locExportPriceText: string  = $("#ExportPriceText").val();
        private locMultiOrdererText: string  = $("#MultiOrdererText").val();
        private locCanTakeOverText: string  = $("#CanTakeOverText").val();
        private locCanRequestorApproveText: string  = $("#CanRequestorApproveText").val();
        private locManagerText: string  = $("#ManagerText").val();
        private locAddressText: string  = $("#AddressText").val();
        private locActiveText: string  = $("#ActiveText").val();
        private locDuplicityCentreNameText: string  = $("#DuplicityCentreNameText").val();
        private locDeleteCentreConfirmText: string  = $("#DeleteCentreConfirmText").val();
        private locCentreWasDisabledText: string = $("#CentreWasDisabledText").val();
        private locNotFoundText: string = $("#NotFoundText").val();
                
        //**********************************************************************

        //*******************************************************************
        //Properties
        private isCentresLoaded: boolean = false;
        private isCompanyLoaded: boolean = false;
        //private isAddressesLoaded: boolean = false;
        private companies: AgDropDown[] = null;
        private addresses: AgDropDown[] = null;
        private searchstringcentreman: Participant = null;
        private searchstringaddress: Address = null;
        //private managerText: string = null;
        private userInterval: any = null;
        private addressInterval: any = null;

        private yesNo : any[] = [{ value: true, label: this.locYesText }, { value: false, label: this.locNoText }];
        private exportPrice : any[] = [{ value: this.locNeverText, label: this.locNeverText },
            { value: this.locAlwaysText, label: this.locAlwaysText },
            { value: this.locOptionalText, label: this.locOptionalText }];
        //*******************************************************************

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
            return this.getRegetRootUrl() + "Report/GetCentresReport?" +
                "filter=" + encodeURI(this.filterUrl) +
                "&sort=" + this.sortColumnsUrl +
                "&t=" + new Date().getTime();
        }

        public getControlColumnsCount(): number {
            return 2;
        }

        public getDuplicityErrMsg(centre: Centre): string {
            return this.getLocDuplicityCentreNameText().replace("{0}", centre.name);
        }

        public getSaveRowUrl(): string {
            return this.getRegetRootUrl() + "Centre/SaveCentreData?t=" + new Date().getTime();
        }

        public insertRow(): void {
            var newCentre: CentreAdminExtended = new CentreAdminExtended();

            newCentre.id = -10;
            newCentre.name = "";
            newCentre.company_name = "";
            newCentre.export_price_text = "";
            newCentre.multi_orderer = false;
            newCentre.other_orderer_can_takeover = false;
            newCentre.is_approved_by_requestor = false;
            newCentre.manager_id = null;
            newCentre.address_text = "";
            newCentre.active = true;

            this.insertBaseRow(newCentre);
        }

        public isRowChanged(): boolean {

            if (this.editRow === null) {
                return true;
            }

            var origCentre: CentreAdminExtended = this.editRowOrig;
            var updCentre: CentreAdminExtended = this.editRow;
            var tmpCentres: any = this.gridOptions.data;
            
            var id: number = updCentre.id;
            //var centre: CentreAdminExtended[] = this.$filter("filter")(centres, { id: id }, true);
            
            if (id < 0) {
                //new row
                this.newRowIndex = null;

                return true;
            } else {
                if (origCentre.name !== updCentre.name) {
                    return true;
                } else if (origCentre.company_name !== updCentre.company_name) {
                    return true;
                } else if (origCentre.export_price_text !== updCentre.export_price_text) {
                    return true;
                } else if (origCentre.multi_orderer !== updCentre.multi_orderer) {
                    return true;
                } else if (origCentre.other_orderer_can_takeover !== updCentre.other_orderer_can_takeover) {
                    return true;
                } else if (origCentre.is_approved_by_requestor !== updCentre.is_approved_by_requestor) {
                    return true;
                } else if (origCentre.manager_id !== updCentre.manager_id) {
                    return true;
                } else if (origCentre.address_text !== updCentre.address_text) {
                    return true;
                } else if (origCentre.active !== updCentre.active) {
                    return true;
                }
            }

            return false;
        }

        public isRowEntityValid(centre: CentreAdminExtended): string {

            if (this.isStringValueNullOrEmpty(centre.name)) {
                return this.locMissingMandatoryText;
            }
                        
            return null;
        }

        public loadGridData(): void {
            this.getCentresData();
        }

        public dbGridId: string = "grdCentre_rg";

        public getMsgDisabled(centre: Centre): string {
            return this.locCentreWasDisabledText.replace("{0}", centre.name);
        }


        public getMsgDeleteConfirm(centre: CentreAdminExtended): string {
            return this.locDeleteCentreConfirmText.replace("{0}", centre.name);;
        }

        public getErrorMsgByErrId(errId: number, msg: string): string {
            return this.locErrorMsgText;
        }

        public deleteUrl: string = this.getRegetRootUrl() + "Centre/DeleteCentre" + "?t=" + new Date().getTime();

        public getDbGridId(): string {
            return this.dbGridId;
        }


        //*****************************************************************************

        //******************************************************************************
        //overriden methods
        public addNewRow(): void {
            super.addNewRow();

            this.formatCentreGridLookups();
        }

        public gridEditRow(rowEntity: any): void {
            super.gridEditRow(rowEntity);

            this.formatCentreGridLookups();
        }
        //******************************************************************************

        //***********************************************************************************
        //Http methods

        private getCentresData(): void {

            if (this.isValueNullOrUndefined(this.$http)) {
                return;
            }

            this.showLoader(this.isError);

            this.$http.get(
                this.getRegetRootUrl() + "/Centre/GetCentresAdminData?filter=" + encodeURI(this.filterUrl) +
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
                    this.setGridColumFilters();
                    
                    //if (!this.isValueNullOrUndefined(this.gridApi)
                    //    && !this.isValueNullOrUndefined(this.gridApi.core)
                    //    && !this.isValueNullOrUndefined(this.uiGridConstants)) {
                    //    this.gridApi.core.notifyDataChange(this.uiGridConstants.dataChange.COLUMN);
                    //}

                    this.isCentresLoaded = true;
                    this.testLoadDataCount++;

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

        private getCompanies() : AgDropDown[] {
            if (this.companies !== null) {
                this.isCompanyLoaded = true;
                this.hideLoaderWrapper();

                return this.companies;
            }
                        
            this.showLoader(this.isError);
                        
            this.$http.get(
                this.getRegetRootUrl() + "Participant/GetParticipantCompanies?t=" + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    this.companies = tmpData;

                    //company
                    var pos: number = this.getColumnIndex("company_name");
                    if (pos > 0) {
                        this.gridOptions.columnDefs[pos].filter = {
                            type: this.uiGridConstants.filter.SELECT,
                            selectOptions: this.companies
                        };
                        this.gridOptions.columnDefs[pos].editDropdownOptionsArray = this.companies;
                        this.gridApi.core.notifyDataChange(this.uiGridConstants.dataChange.COLUMN);
                    }

                    //this.getCentresData();
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

            return this.companies;
        }
                
        //***********************************************************************************

        //*************************************************************************************
        //Methods
        private setGrid(): void {
            this.gridOptions.columnDefs = [
                {
                    name: "row_index",
                    field: "row_index",
                    displayName: '',
                    enableFiltering: false,
                    enableSorting: false,
                    enableCellEdit: false,
                    width: 40,
                    enableColumnResizing: true,
                    cellTemplate: '<div class="ui-grid-cell-contents ui-grid-top-panel" style="text-align:center;vertical-align:middle;font-weight:normal;">{{COL_FIELD}}</div>'
                },
                {
                    name: 'action_buttons',
                    displayName: '',
                    enableFiltering: false,
                    enableSorting: false,
                    enableCellEdit: false,
                    enableHiding: false,
                    enableColumnResizing: false,
                    width: 70,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellAction.html"

                },
                {
                    name: "name", displayName: this.locNameText, field: "name",
                    enableHiding: false,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextMandatoryTemplate.html",
                    enableCellEdit: false,
                    width: 80,
                    minWidth: 80
                },
                {
                    name: "company_name", displayName: this.locCompanyText, field: "company_name",
                    enableHiding: false,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellDropDownMandatoryTemplate.html",
                    enableCellEdit: false,
                    width: 160,
                    minWidth: 110
                },
                {
                    name: "export_price_text", displayName: this.locExportPriceText, field: "export_price_text",
                    enableHiding: true,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellDropDownMandatoryTemplate.html",
                    enableCellEdit: false,
                    width: 75,
                    minWidth: 75
                },
                {
                    name: 'multi_orderer', displayName: this.locMultiOrdererText, field: "multi_orderer",
                    enableCellEdit: false,
                    enableHiding: true,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellCheckboxTemplate.html",
                    minWidth: 90,
                    width: 90
                },
                {
                    name: 'other_orderer_can_takeover', displayName: this.locCanTakeOverText, field: "other_orderer_can_takeover",
                    enableCellEdit: false,
                    enableHiding: true,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellCheckboxTemplate.html",
                    minWidth: 90,
                    width: 90
                },
                {
                    name: 'is_approved_by_requestor', displayName: this.locCanRequestorApproveText, field: "is_approved_by_requestor",
                    enableCellEdit: false,
                    enableHiding: true,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellCheckboxTemplate.html",
                    minWidth: 90,
                    width: 90
                },
                {
                    name: "manager_name", displayName: this.locManagerText, field: "manager_name",
                    enableHiding: true,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellUserLookupTemplate.html",
                    enableCellEdit: false,
                    sortingAlgorithm: function (a, b, rowA, rowB, direction) {
                        return this.sortNullString(a, b, rowA, rowB, direction);
                    },
                    filter: {
                        condition: function (searchTerm, cellValue) {
                            return true;
                        }
                    },
                    width: 150,
                    minWidth: 150
                },
                {
                    name: "address_text", displayName: this.locAddressText, field: "address_text",
                    enableHiding: true,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellAddressLookupTemplate.html",
                    enableCellEdit: false,
                    sortingAlgorithm: function (a, b, rowA, rowB, direction) {
                        return this.sortNullString(a, b, rowA, rowB, direction);
                        //a = (IsStringValueNullOrEmpty(a)) ? ' ' : a;
                        //b = (IsStringValueNullOrEmpty(b)) ? ' ' : b;
                        //if (a === b) { return 0 };
                        //if (a < b) { return -1 };
                        //if (a > b) { return 1 };
                    },
                    minWidth: 120
                },
                {
                    name: 'active', displayName: this.locActiveText, field: "active",
                    enableCellEdit: false,
                    enableHiding: false,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellCheckboxTemplate.html",
                    minWidth: 90,
                    width: 90
                },
                {
                    name: 'manager_id', displayName: 'manager_id', field: "manager_id",
                    visible: false
                },
                {
                    name: 'address_id', displayName: 'address_id', field: "address_id",
                    visible: false
                }

            ];

            
        }

        private loadData(): void {
          
            this.companies = this.getCompanies();
   
        }

        public getLocDuplicityCentreNameText(): string {
            return this.locDuplicityCentreNameText;
        }
        
        private hideLoaderWrapper() : void {
            if (this.isError ||
                (this.isCentresLoaded
                 && this.isCompanyLoaded)) {
                this.hideLoader();
            }
        }

        private setGridColumFilters(): void {
            
            this.setGridColumFilter("export_price_text", this.exportPrice);
            this.setGridColumFilter("multi_orderer", this.yesNo);
            this.setGridColumFilter("other_orderer_can_takeover", this.yesNo);
            this.setGridColumFilter("is_approved_by_requestor", this.yesNo);
            this.setGridColumFilter("active", this.yesNo);
        }

        //private setGridColumFilter(columnName : string, filterValues: any[]): void {
        //    var pos: number = this.getColumnIndex(columnName);
        //    if (pos > 0) {
        //        this.gridOptions.columnDefs[pos].filter = {
        //            //term: '1',
        //            type: this.uiGridConstants.filter.SELECT,
        //            selectOptions: filterValues
        //        };
        //        this.gridOptions.columnDefs[pos].editDropdownOptionsArray = filterValues;
        //        this.gridApi.core.notifyDataChange(this.uiGridConstants.dataChange.COLUMN);
        //    }
        //}

        private userTextChanged(isSeleted : boolean) {
            var txtUser : any = document.getElementById("intGridUserAutocomplete");
            if (!isSeleted && txtUser.value === this.locManagerText) {
                txtUser.style.color = "#888";
            } else {
                txtUser.style.color = "#000";
            }
            
        }

        private addressTextChanged(isSeleted: boolean) {
            var txtAddress: any = document.getElementById("intGridAddressAutocomplete");
            if (!isSeleted && txtAddress.value === this.locAddressText) {
                txtAddress.style.color = "#888";
            } else {
                txtAddress.style.color = "#000";
            }

        }

        private searchParticipant(strName: string): ng.IPromise<Participant[]> {
            return this.filterParticipants(strName);
        }

        private searchAddress(strName: string, companyName: string): ng.IPromise<Address[]> {
            return this.filterAddresses(strName, companyName);
        }

        //Formats user lookup textbox in Centres grid, must be here
        private formatCentreGridLookups(): void {
            this.userInterval = setInterval(() => {
                this.formatGridLookup(
                    "gridFieldAutoUser",
                    "intGridUserAutocomplete",
                    "manager_name",
                    this.userInterval);
                
            }, 100);

            this.addressInterval = setInterval(() => {
                this.formatGridLookup(
                    "gridFieldAutoAddress",
                    "intGridAddressAutocomplete",
                    "address_text",
                    this.addressInterval);
                
            }, 100);

            
        }

        private centreManSelectedItemChange(participant : Participant) {
            
            if (this.isValueNullOrUndefined(participant)) {
                this.editRow["manager_name"] = null;
                this.editRow["manager_id"] = null;
            } else {
                if (!this.isValueNullOrUndefined(participant.id)) {
                    //******************** !!!!!!!!!!!!! **************************************************
                    //it is called 2 time, seccond tim it is skipped because item contains name, not object
                    this.editRow["manager_name"] = participant.surname + " " + participant.first_name;
                    this.editRow["manager_id"] = participant.id;

                }
            }

            this.userTextChanged(true);
        }

        private addressSelectedItemChange(address: Address) {
            if (this.isValueNullOrUndefined(address)) {
                this.editRow["address_text"] = null;
                this.editRow["address_id"] = null;
            } else {
                if (!this.isValueNullOrUndefined(address.id)) {
                    //******************** !!!!!!!!!!!!! **************************************************
                    //it is called 2 time, seccond tim it is skipped because item contains name, not object
                    this.editRow["address_id"] = address.id;
                    this.editRow["address_text"] = address.address_text;
                }
            }

            this.addressTextChanged(true);
        }

        public dropDownChanged(item: any): void {
            //var company: AgDropDown[] = this.$filter("filter")(this.companies, { company_name: item }, true);
            //var centre: CentreAdminExtended = this.editRow;
            //centre.
            var fieldName: string = item.$parent.col.field;
            if (fieldName == "company_name") {
                var centre: CentreAdminExtended = this.editRow;
                //centre.company_name = item;
                centre.address_text = null;
            }
        }

        //*************************************************************************************
    }
    angular.
        module("RegetApp").
        controller("CentreController", Kamsyk.RegetApp.CentreController);

    export class CentreAdminExtended {
        public id: number = null;
        public name: string = null;
        public company_name: string = null;
        public manager_id: number = null;
        public manager_name: string = null;
        public export_price_text: string = null;
        public address_text: string = null;
        public row_index: number = null;
        public multi_orderer: boolean = false;
        public other_orderer_can_takeover: boolean = false;
        public is_approved_by_requestor: boolean = false;
        public active: boolean = false;
    }
}
