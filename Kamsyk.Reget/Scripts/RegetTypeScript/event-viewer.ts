module Kamsyk.RegetApp {
    export class EventViewerController extends BaseRegetGridTs implements angular.IController {
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
            //this.setGrid();
            //this.loadData();

        }
        //***************************************************************


        $onInit = () => { };

        //****************************************************************************
        //Abstract
        
        public exportToXlsUrl(): string {
            //return this.getRegetRootUrl() + "Report/GetAddressesReport?" +
            //    "filter=" + encodeURI(this.filterUrl) +
            //    "&sort=" + this.sortColumnsUrl +
            //    "&t=" + new Date().getTime();
            return null;
        }

        public getControlColumnsCount(): number {
            return 2;
        }

        public getDuplicityErrMsg(rowEntity: Address): string {
            //var addressText: string = this.getFullAddress(rowEntity);
            //return this.locDuplicityAddressText.replace("{0}", addressText);
            return null;
        }

        public getSaveRowUrl(): string {
            //return this.getRegetRootUrl() + "Address/SaveAddressData?t=" + new Date().getTime();
            return null;
        }

        public insertRow(): void {
            //var newAddress: Address = new Address();

            //newAddress.id = -10;
            //newAddress.company_name = "";
            //newAddress.company_name_text = "";
            //newAddress.street = "";
            //newAddress.city = "";
            //newAddress.zip = "";

            //this.insertBaseRow(newAddress);
        }

        public isRowChanged(): boolean {

            //if (this.editRow === null) {
            //    return true;
            //}

            //var origAddress: Address = this.editRowOrig;
            //var updAddress: Address = this.editRow;
            //var tmpAddresses: any = this.gridOptions.data;
            //var addresses: Address[] = tmpAddresses;
            //var isChanged: boolean = false;

            //var id: number = updAddress.id;
            //var address: Address[] = this.$filter("filter")(addresses, { id: id }, true);


            //if (id < 0) {
            //    //new row
            //    this.newRowIndex = null;

            //    return true;
            //} else {
            //    if (origAddress.company_name !== updAddress.company_name) {
            //        return true;
            //    } else if (origAddress.company_name_text !== updAddress.company_name_text) {
            //        return true;
            //    } else if (origAddress.street !== updAddress.street) {
            //        return true;
            //    } else if (origAddress.city !== updAddress.city) {
            //        return true;
            //    } else if (origAddress.zip !== updAddress.zip) {
            //        return true;
            //    }
            //}

            return false;
        }

        public isRowEntityValid(address: Address): string {

            //if (this.isStringValueNullOrEmpty(address.company_name)) {
            //    return this.locMissingMandatoryText;
            //}

            //if (this.isStringValueNullOrEmpty(address.company_name_text)) {
            //    return this.locMissingMandatoryText;
            //}

            return null;
        }

        public loadGridData(): void {
            //this.getAddressData();
        }

        public dbGridId: string = "grdEventViewer_rg";

        public getMsgDisabled(address: Address): string {
            return null;//this.locSupplierWasDisabledText.replace("{0}", supplier.supp_name);
        }


        public getMsgDeleteConfirm(address: Address): string {
            //return this.locDeleteAddressConfirmText.replace("{0}", address.company_name);;
            return null;
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
                }
                
            ];


        }
        //*************************************************************************************************************************************
    }
    angular.
        module("RegetApp").
        controller("EventViewerController", Kamsyk.RegetApp.EventViewerController);
}