/// <reference path="../typings/ui-grid/ui-grid.d.ts" />
/// <reference path="../RegetTypeScript/Base/reget-base.ts" />
/// <reference path="../RegetTypeScript/Base/reget-common.ts" />
/// <reference path="../RegetTypeScript/Base/reget-entity.ts" />


module Kamsyk.RegetApp {
   
       
    export class SupplierDetailController extends BaseRegetGridTs implements angular.IController {
        //****************************************
        //Abstract properties
        public dbGridId: string = "grdSupplierDetail_rg";
        //*****************************************

        //**********************************************************
        //Properties
        private iSupplierId: number = null;
        private isSupplierLoaded: boolean = false;
        private isContactLoaded: boolean = false;
        private supplier: Supplier = null;
        //**********************************************************
       
                
        //**********************************************************
        //Loc Text
        private locFirstName: string = $("#FirstNameText").val();
        private locSurname: string = $("#SurnameText").val();
        private locMail: string = $("#MailText").val();
        private locPhone: string = $("#PhoneText").val();
        private locDeleteSuppliereContactConfirmText: string = $("#DeleteSuppliereContactConfirmText").val();
        //private locDuplicityMailText: string = $("#DuplicityMailText").val();
        //**********************************************************

      
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
            this.loadInit();

        }
        //***************************************************************


        $onInit = () => { };

        //****************************************************************************
        //Abstract
        public exportToXlsUrl(): string {
            return null;
        }

        public getControlColumnsCount(): number {
            return 2;
        }

                
        public getDuplicityErrMsg(supplierContact: SupplierContact): string {
            //return this.locDuplicityMailText.replace("{0}", supplierContact.email);
            return null;
        }

        public getSaveRowUrl(): string {
            return this.getRegetRootUrl() + "Supplier/SaveContactData?t=" + new Date().getTime();
        }

        public insertRow() : void {
            let newContact: SupplierContact = new SupplierContact();

            newContact.id = -10;
            newContact.supplier_id = this.iSupplierId;
            newContact.surname = "";
            newContact.first_name = "";
            newContact.email = "";
            newContact.phone = "";
            
            this.insertBaseRow(newContact);
        }

        public isRowChanged(): boolean {
            if (this.editRow === null) {
                return true;
            }

            var origContact: SupplierContact = this.editRowOrig;
            var updContact: SupplierContact = this.editRow;
            
            var id: number = updContact.id;
            
            if (id < 0) {
                //new row
                this.newRowIndex = null;

                return true;
            } else {
                if (origContact.email !== updContact.email) {
                    return true;
                } else if (origContact.first_name !== updContact.first_name) {
                    return true;
                } else if (origContact.surname !== updContact.surname) {
                    return true;
                } else if (origContact.phone !== updContact.phone) {
                    return true;
                } 
            }
            
            return false;
        }

        public isRowEntityValid(supplierContact: SupplierContact): string {
            if (this.isStringValueNullOrEmpty(supplierContact.email)) {
                return this.locMissingMandatoryText;
            }

            return null;
        }

        public loadGridData(): void {
            this.getContactsData();
        }

        public getMsgDisabled(supplierContact: SupplierContact): string {
            return null;
        }

        
        public getMsgDeleteConfirm(supplierContact: SupplierContact): string {
            return this.locDeleteSuppliereContactConfirmText
                .replace("{0}", supplierContact.email);
        }

        public getErrorMsgByErrId(errId: number, msg: string): string {
            return this.locErrorMsgText;
        }

        public deleteUrl: string = this.getRegetRootUrl() + 'Supplier/DeleteSupplierContact' + '?t=' + new Date().getTime();

        public getDbGridId(): string {
            return this.dbGridId;
        }

        //*****************************************************************************


        //***************************************************************************
        //HTTP Methods
        private loadSupplier(): void {
            this.showLoaderBoxOnly(this.isError);

            this.$http.get(
                this.getRegetRootUrl() + "Supplier/GetSupplierById?id=" + this.iSupplierId + "&t=" + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    let result: any = response.data;
                    this.supplier = result;

                    //this.getContactsData();
                    this.getLoadDataGridSettings();

                    this.isSupplierLoaded = true;
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

        private getContactsData(): void {

            if (this.isValueNullOrUndefined(this.$http)) {
                return;
            }

            this.showLoader(this.isError);

            this.$http.get(
                this.getRegetRootUrl() + "/Supplier/GetContactsData?supplierId=" + this.iSupplierId +
                "&filter = " + encodeURI(this.filterUrl) +
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
                    
                    this.isContactLoaded = true;
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

        //***************************************************************************

                
        //****************************************************************************
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
                    enableHiding: false,
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
                    minWidth: 70,
                    width: 70,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellAction.html"
                },
                {
                    name: "first_name", displayName: this.locFirstName, field: "first_name",
                    enableHiding: true,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextTemplate.html",
                    enableCellEdit: false,
                    width: 180,
                    minWidth: 100
                },
                {
                    name: "surname", displayName: this.locSurname, field: "surname",
                    enableHiding: true,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextTemplate.html",
                    enableCellEdit: true,
                    width: 180,
                    minWidth: 100
                },
                {
                    name: "email", displayName: this.locMail, field: "email",
                    enableHiding: false,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextMailMandatoryTemplate.html",
                    enableCellEdit: false,
                    width: 180,
                    minWidth: 100
                },
                {
                    name: "phone", displayName: this.locPhone, field: "phone",
                    enableHiding: true,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextTemplate.html",
                    enableCellEdit: false,
                    width: 180,
                    minWidth: 100
                }
            ];


        }
        //****************************************************************************


        //***************************************************************************
        //Methods
        private loadInit(): void {
           
            this.showLoader(this.isError);

            try {
                this.iSupplierId = this.getUrlParamValueInt("id");
                this.loadSupplier();
                
            } catch (ex) {
                this.hideLoader();
                this.displayErrorMsg();
            }
        }

        private hideLoaderWrapper(): void {
            
            if (this.isError || (
                this.isSupplierLoaded
                && this.isContactLoaded
            )) {
                this.hideLoader();
                this.isError = false;
            }
        }
        //***************************************************************************
    }
    angular.
        module('RegetApp').
        controller('SupplierDetailController', Kamsyk.RegetApp.SupplierDetailController);

}