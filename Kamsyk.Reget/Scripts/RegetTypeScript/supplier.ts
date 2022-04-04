/// <reference path="../typings/ui-grid/ui-grid.d.ts" />
/// <reference path="../RegetTypeScript/Base/reget-base.ts" />
/// <reference path="../RegetTypeScript/Base/reget-common.ts" />
/// <reference path="../RegetTypeScript/Base/reget-entity.ts" />


module Kamsyk.RegetApp {
    angular.module('RegetApp').directive("suppadmin", () => {
        return {
            scope: {
                userid: '@',
                surname: '@',
                firstname: '@',
                iscompanyadmin: '@',
                removetext: '@',
                rooturl: '@',
                deletesuppadmin: '&'
            },
            templateUrl: RegetCommonTs.getRegetRootUrl() + 'Content/Html/AngSuppAdmin.html'
        };
    });

       
    export class SupplierController extends BaseRegetGridTs implements angular.IController {
        //****************************************
        //Abstract properties
        public dbGridId: string = "grdSupplier_rg";
        //*****************************************

        private selectedCompanyId: number = null;
        private isPgLoaded: boolean = false;
        private isCompanySuppAdmin: boolean = false;
        private isGridHidden: boolean = true;
        private isCompanyAdmin: boolean = false;
        private isCompanySuppManualAllowed: boolean = false;
        private isSuppAdminLoaded: boolean = false;
        private isParticipantLoaded: boolean = false;
        private isOfficeLoaded: boolean = false;
        private isSuppUploadDateLoaded: boolean = false;
        private isSuppLoadedUpToDate: boolean = false;
        private lastSuppUploadDate: string = null;
        private suppCompanies: Company[] = null;
        private supplierAdmins: Participant[] = null;
        private searchstringsuppadmin: string = null;
        private selectedCgAdmin: Participant = null;
                
        //**********************************************************
        //Loc Text
        private locWarningText: string = $("#WarningText").val();
        private locErrMsg = $("#ErrMsgText").val();
        private locNameText: string = $("#NameText").val();
        private locSuppliersText: string = $("#SuppliersText").val();
        private locStreetText: string = $("#StreetText").val();
        private locCityText: string = $("#CityText").val();
        private locZipText: string = $("#ZipText").val();
        private locCountryText: string = $("#CountryText").val();
        private locPhoneText: string = $("#PhoneText").val();
        private locMailText: string = $("#MailText").val();
        private locContactPersonText: string = $("#ContactPersonText").val();
        private locSupplierIdText: string = $("#SupplierIdText").val();
        private locActiveText: string = $("#ActiveText").val();
        private locSupplierUpdateConfirmText: string = $("#SupplierUpdateConfirmText").val();
        private locLoadSuccessfullyText: string = $("#LoadSuccessfullyText").val();
        private locLastUploadDateText: string = $("#LastUploadDateText").val();
        private locNeverText: string = $("#NeverText").val();
        private locSuppliersLoadedFromText: string = $("#SuppliersLoadedFromText").val();
        private locSwitchToAutoSuppMainConfirmText: string = $("#SwitchToAutoSuppMainConfirmText").val();
        private locManualSupplierMaintenanceIsNotAllowedText: string = $("#ManualSupplierMaintenanceIsNotAllowedText").val();
        private locDeleteSupplierConfirmText: string = $("#DeleteSupplierConfirmText").val();
        private locSupplierWasDisabledText: string = $("#SupplierWasDisabledText").val();
        private locDuplicitySuppllierNameText: string = $("#DuplicitySuppllierNameText").val();
        private locCannotDeleteCompanyAdminText: string = $("#CannotDeleteCompanyAdminText").val();
        private locNotificationText: string = $("#NotificationText").val();
        //**********************************************************

        //This one "uiGrid.ISelectOptio" does NOT work !!!!!! Default Filter is not set properly
        //private yesNo: Array<uiGrid.ISelectOption> = [{ value: "true", label: this.locYesText }, { value: "false", label: this.locNoText }];

        private yesNo: any[] = [{ value: true, label: this.locYesText }, { value: false, label: this.locNoText }];
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
            return this.getRegetRootUrl() + 'Report/GetSuppliersReport?companyId=' + this.selectedCompanyId +
                '&filter=' + encodeURI(this.filterUrl) +
                '&sort=' + this.sortColumnsUrl +
                '&t=' + new Date().getTime();
        }

        public getControlColumnsCount(): number {
            return 2;
        }

                
        public getDuplicityErrMsg(rowEntity: any): string {
            return this.locDuplicitySuppllierNameText.replace("{0}", rowEntity.supp_name);;;
        }

        public getSaveRowUrl(): string {
            return this.getRegetRootUrl() + 'Supplier/SaveSupplierData?t=' + new Date().getTime();
        }

        public insertRow() : void {

            var newSupplier: Supplier = new Supplier();
            newSupplier.id = -10;
            newSupplier.company_id = this.selectedCompanyId;
            newSupplier.supp_name = "";
            newSupplier.supplier_id = "";
            newSupplier.street_part1 = "";
            newSupplier.city = "";
            newSupplier.zip = "";
            newSupplier.country = "";
            newSupplier.contact_person = "";
            newSupplier.phone = "";
            newSupplier.email = "";
            newSupplier.active = true;

            this.insertBaseRow(newSupplier);

            
        }

        public isRowChanged(): boolean {
            
            if (this.editRow === null) {
                return true;
            }

            var origSupp: Supplier = this.editRowOrig;
            var updSupp: Supplier = this.editRow;
            var tmpSuppliers: any = this.gridOptions.data;
            var suppliers: Supplier[] = tmpSuppliers;
            var isChanged: boolean = false;

            var id: number = updSupp.id;
            var supplier: Supplier[] = this.$filter("filter")(suppliers, { id: id }, true);

            
            if (id < 0) {
                //new row
                this.newRowIndex = null;

                return true;
            } else {
                if (origSupp.supp_name !== updSupp.supp_name) {
                    return true;
                } else if (origSupp.supplier_id !== updSupp.supplier_id) {
                    return true;
                } else if (origSupp.street_part1 !== updSupp.street_part1) {
                    return true;
                } else if (origSupp.city !== updSupp.city) {
                    return true;
                } else if (origSupp.zip !== updSupp.zip) {
                    return true;
                } else if (origSupp.country !== updSupp.country) {
                    return true;
                } else if (origSupp.contact_person !== updSupp.contact_person) {
                    return true;
                } else if (origSupp.phone !== updSupp.phone) {
                    return true;
                } else if (origSupp.email !== updSupp.email) {
                    return true;
                } else if (origSupp.active !== updSupp.active) {
                    return true;
                } else if (supplier[0].id < -1) {
                    return true;
                }
            }

            return false;
        }

        public isRowEntityValid(supplier: Supplier): string {
            
            if (this.isStringValueNullOrEmpty(supplier.supp_name)) {
                return this.locMissingMandatoryText;
            }
                        
            return null;
        }

        public loadGridData(): void {
            this.getSupplierDataFromDb();
        }

        public getMsgDisabled(supplier: Supplier): string {
            return this.locSupplierWasDisabledText.replace("{0}", supplier.supp_name);
        }

        
        public getMsgDeleteConfirm(supplier: Supplier): string {
            return this.locDeleteSupplierConfirmText.replace("{0}", supplier.supp_name);;
        }

        public getErrorMsgByErrId(errId: number, msg: string): string {
            return this.locErrorMsgText;
        }

        public deleteUrl: string = this.getRegetRootUrl() + 'Supplier/DeleteSupplier' + '?t=' + new Date().getTime();

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
                    minWidth: 70,
                    width: 70,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellAction.html"
                },
                {
                    name: "supp_name", displayName: this.locNameText + "*", field: "supp_name",
                    enableHiding: false,
                    enableCellEdit: false,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextMandatoryTemplate.html",
                    minWidth: 125,
                    width: 180
                },
                {
                    name: "supplier_id", displayName: this.locSupplierIdText, field: "supplier_id",
                    enableHiding: false,
                    enableCellEdit: false,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextTemplate.html",
                    minWidth: 75,
                    width: 75
                },
                {
                    name: "street_part1", displayName: this.locStreetText, field: "street_part1",
                    enableHiding: true,
                    enableCellEdit: false,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextTemplate.html",
                    minWidth: 125,
                    width: 225
                },
                {
                    name: "city", displayName: this.locCityText, field: "city",
                    enableHiding: true,
                    enableCellEdit: false,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextTemplate.html",
                    minWidth: 100,
                    width: 150
                },
                {
                    name: "zip", displayName: this.locZipText, field: "zip",
                    enableHiding: true,
                    enableCellEdit: false,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextTemplate.html",
                    minWidth: 60,
                    width: 60
                },
                {
                    name: "country", displayName: this.locCountryText, field: "country",
                    enableHiding: true,
                    enableCellEdit: false,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextTemplate.html",
                    minWidth: 100,
                    width: 125
                },
                {
                    name: "contact_person", displayName: this.locContactPersonText, field: "contact_person",
                    enableHiding: true,
                    enableCellEdit: false,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextTemplate.html",
                    minWidth: 110,
                    width: 145
                },
                {
                    name: "phone", displayName: this.locPhoneText, field: "phone",
                    enableHiding: true,
                    enableCellEdit: false,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextTemplate.html",
                    minWidth: 90,
                    width: 110
                },
                {
                    name: "email", displayName: this.locMailText, field: "email",
                    enableHiding: true,
                    enableCellEdit: false,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextTemplate.html",
                    minWidth: 90,
                    width: 125
                },
                {
                    name: 'active', displayName: this.locActiveText, field: "active",
                    enableCellEdit: false,
                    enableHiding: false,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellCheckboxTemplate.html",
                    filter: {
                        type: this.uiGridConstants.filter.SELECT,
                        selectOptions: this.yesNo
                    },
                    minWidth: 100,
                    width: 100
                }
                
            ];


        }
        //****************************************************************************

        

        //***************************************************************************
        //Http
        private getOfficesData() : void {

            this.isError = false;
            this.showLoaderBoxOnly(this.isError);

            this.$http.get(
                this.getRegetRootUrl() + 'Supplier/GetUserOfficeData?t=' + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    this.suppCompanies = tmpData;
                    this.isOfficeLoaded = true;

                    if (!this.isValueNullOrUndefined(this.suppCompanies) && this.suppCompanies.length === 1) {
                        this.selectedCompanyId = this.suppCompanies[0].id;
                        this.getSupplierData();
                    }

                    this.hideLoaderWrapper();
                } catch (e) {
                    this.hideLoader();
                    this.displayErrorMsg();
                } finally {
                    this.hideLoaderWrapper();
                }
            }, (response: any) => {
                    this.hideLoaderWrapper();
                this.displayErrorMsg();
            });
            
        }

        private getSupplierDataFromDb() : void {
            this.isSuppUploadDateLoaded = false;
            this.isPgLoaded = false;

            this.isCompanySuppAdmin = false;
            this.isGridHidden = false;
            this.isCompanyAdmin = false;
            this.isCompanySuppManualAllowed = false;
            this.isSuppAdminLoaded = false;
            this.isParticipantLoaded = false;

            this.showLoaderBoxOnly(this.isError);

            this.$http.get(
                this.getRegetRootUrl() + '/Supplier/GetLastCompanySupplierUpload?companyId=' + this.selectedCompanyId +
                '&t=' + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    this.getSupplierAdmins();

                    var tmpData: any = response.data;
                    this.lastSuppUploadDate = tmpData;
                    this.isSuppLoadedUpToDate = false;
                    if (this.isStringValueNullOrEmpty(this.lastSuppUploadDate)) {
                        this.lastSuppUploadDate = this.locNeverText;
                    } else {
                        var suppInfoItems: string[] = this.lastSuppUploadDate.split('|');
                        this.lastSuppUploadDate = suppInfoItems[0];
                        if (suppInfoItems[1] === '1') {
                            this.isSuppLoadedUpToDate = true;
                        }
                    }

                    this.isSuppUploadDateLoaded = true;

                    this.getSupplierDataOnlyFromDb();

                    var office: Company[] = this.$filter("filter")(this.suppCompanies, { id: this.selectedCompanyId }, true);
                    if (!this.isValueNullOrUndefined(office[0])) {
                        if (office[0].is_supplier_admin) {
                            this.isCompanySuppAdmin = true;
                        }
                        if (office[0].is_company_admin) {
                            this.isCompanyAdmin = true;
                            this.isCompanySuppAdmin = true;
                        }

                        this.isCompanySuppManualAllowed = office[0].is_supplier_maintenance_allowed;
                        this.enableGrid();
                    }

                    this.enableGrid();

                    this.hideLoaderWrapper();
                } catch (e) {
                    this.hideLoader();
                    this.displayErrorMsg();
                } finally {
                    this.hideLoader();
                }
            }, (response: any) => {
                    this.hideLoader();
                this.displayErrorMsg();
            });
            
        }

        private getSupplierAdmins(): void {

            this.showLoaderBoxOnly(this.isError);

            this.$http.get(
                this.getRegetRootUrl() + '/Supplier/GetSuppliersAdmins?companyId=' + this.selectedCompanyId +
                '&t=' + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    this.supplierAdmins = tmpData;
                    this.isSuppAdminLoaded = true;

                    this.hideLoaderWrapper();

                    this.hideLoader();
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

        private getSupplierDataOnlyFromDb() : void {

            this.isError = false;

            $("#divGrdSupplier").show();
            this.showLoaderBoxOnly(this.isError);

            this.$http.get(
                this.getRegetRootUrl() + '/Supplier/GetSupplierData?companyId=' + this.selectedCompanyId +
                '&filter=' + encodeURI(this.filterUrl) +
                '&pageSize=' + this.pageSize +
                '&currentPage=' + this.currentPage +
                '&sort=' + this.sortColumnsUrl +
                '&t=' + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    if (this.isGridHidden === true) {
                        this.isGridHidden = false;
                        //because of filter Yes/No
                        this.getSupplierData();
                        return;
                    }

                    var tmpData: any = response.data;
                    this.gridOptions.data = tmpData.db_data;
                    this.rowsCount = tmpData.rows_count;

                    this.setGridHeader();

                    //if (!this.isValueNullOrUndefined(this.gridApi)) {
                    //    //active
                    //    var pos: number = this.getColumnIndex("active");
                    //    this.gridOptions.columnDefs[pos].filter = {
                    //        // term: '1',
                    //        type: this.uiGridConstants.filter.SELECT,
                    //        selectOptions: this.yesNo
                    //    };
                    //    //this.gridOptions.columnDefs[pos].filters[0].term = "true";
                    //    this.gridApi.core.notifyDataChange(this.uiGridConstants.dataChange.COLUMN);
                    //}

                    //if (this.userGridSettings === null) {
                    //    this.defaultColumnDefs = angular.copy(this.gridOptions.columnDefs);
                    //    this.getDataGridSettings();
                    //}
                    if (!this.isValueNullOrUndefined(this.gridApi)) {
                        this.gridApi.core.notifyDataChange(this.uiGridConstants.dataChange.COLUMN);
                    }
                    this.setGridSettingData();

                    //********************************************************************
                    //it is very important otherwise 50 lines are nod diplaye dproperly !!!
                    this.gridOptions.virtualizationThreshold = this.rowsCount + 1;
                    //********************************************************************

                    this.isPgLoaded = true;

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

        public importSupplierService() : void {

            this.isError = false;
            
            this.showLoader(this.isError);

            this.$http.get(
                this.getRegetRootUrl() + 'Supplier/ImportSuppliers?companyId=' + this.selectedCompanyId
                + '&t=' + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    this.getSupplierData();
                    this.hideLoaderWrapper();
                    this.showInfo(
                        this.locNotificationText,
                        this.locLoadSuccessfullyText,
                        this.locCloseText);
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

        private deleteSuppAdminFromDb(iAdminId: number, sindex: number) {

            this.showLoader(this.isError);

            this.$http.post(
                this.getRegetRootUrl() + '/Supplier/DeleteSuppAdmin?adminId=' + iAdminId +
                '&companyId=' + this.selectedCompanyId +
                '&t=' + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    var strResult: string = tmpData.string_value;

                    if (this.isStringValueNullOrEmpty(strResult)) {

                        this.supplierAdmins.splice(sindex, 1);

                    }
                } catch (e) {
                    this.hideLoader();
                    this.displayErrorMsg();
                } finally {
                    this.hideLoader();
                }
            }, (response: any) => {
                this.hideLoader();
                this.displayErrorMsg();
            });
        }

        private saveNewSuppAdmin(user: Participant) {
            this.showLoader(this.isError);

            this.$http.post(
                this.getRegetRootUrl() + '/Supplier/SaveNewSuppAdmin?adminId=' + user.id +
                '&companyId=' + this.selectedCompanyId +
                '&t=' + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    var strResult: string = tmpData.string_value;

                    if (this.isStringValueNullOrEmpty(strResult)) {
                        var newAdmin: Participant = new Participant();
                        newAdmin.id = user.id;
                        newAdmin.surname = user.surname;
                        newAdmin.first_name = user.first_name;

                        this.supplierAdmins.push(newAdmin);
                    }
                } catch (e) {
                    this.hideLoader();
                    this.displayErrorMsg();
                } finally {
                    this.hideLoader();
                }
            }, (response: any) => {
                this.hideLoader();
                this.displayErrorMsg();
            });
      
        }

        private saveSupplierMaintenance() : void {
            this.showLoaderBoxOnly(this.isError);
            var jsonUserGridData = JSON.stringify(this.userGridSettings);

            this.$http.post(
                this.getRegetRootUrl() + 'Supplier/SaveSupplierMaintenance?companyId='
                + this.selectedCompanyId + '&' + 'isManualSuppMaintenance='
                + this.isCompanySuppManualAllowed + '&t=' + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    //var tmpData: any = response.data;
                    var currCompany: Company[] = this.$filter("filter")(this.suppCompanies, { id: this.selectedCompanyId }, true);
                    currCompany[0].is_supplier_maintenance_allowed = this.isCompanySuppManualAllowed;

                    //neccessary to refresh grid settngs - show/hide columns
                    this.getSupplierDataOnlyFromDb();

                    this.enableGrid();

                    this.hideLoader();
                } catch (e) {
                    this.hideLoader();
                    this.displayErrorMsg();
                } finally {
                    this.hideLoader();
                }
            }, (response: any) => {
                this.hideLoader();
                this.displayErrorMsg();
            });

        }

        //private gridDeleteRowFromDb(supplier: Supplier, ev: MouseEvent) {
        //    this.showLoaderBoxOnly(this.isError);
                        
        //    var jsonSupplierData = JSON.stringify(supplier);

        //    this.$http.post(
        //        this.getRegetRootUrl() + 'Supplier/DeleteSupplier' + '?t=' + new Date().getTime(),
        //        jsonSupplierData
        //    ).then((response) => {
        //        try {
        //            var tmpData: any = response.data;
        //            var strResult = tmpData.string_value;

        //            if (this.isStringValueNullOrEmpty(strResult)) {
        //                this.removeRowFromArray(supplier);

        //            } else if (strResult === "disabled") {
        //                supplier.active = false;
        //                var msgDisabled = this.locSupplierWasDisabledText.replace("{0}", supplier.supp_name);
        //                this.showAlert(this.locConfirmationText, msgDisabled, this.locCloseText);
        //            }

        //            this.hideLoader();
        //        } catch (e) {
        //            this.hideLoader();
        //            this.displayErrorMsg();
        //        } finally {
        //            this.hideLoader();
        //        }
        //    }, (response: any) => {
        //        this.hideLoader();
        //        this.displayErrorMsg();
        //    });
            
        //}
        //***************************************************************************

        //***************************************************************************
        //Methods
        private loadData(): void {
            this.getOfficesData();

        }

        private getSupplierData() : void {
            if (this.isSkipLoad === true) {
                return;
            }

            this.showLoaderBoxOnly(this.isError);

            if (isNaN(this.selectedCompanyId)) {
                this.selectedCompanyId = -1;
                this.gridOptions.data = [];
                this.userGridSettings = null;
                this.hideLoader();
                this.isGridHidden = true;
                return;
            }

            //this.getSupplierDataFromDb();
            this.getLoadDataGridSettings();
        }

        private hideLoaderWrapper() : void {
            if (this.isError || (this.isSuppUploadDateLoaded === true &&
                this.isPgLoaded === true &&
                this.isSuppAdminLoaded === true &&
                this.isParticipantLoaded === true)) {
               this.hideLoader();
            }
        }

        private enableGrid() : void {
            var btnGrdNewRowId = "grdSupplier_tdNewRow";
            var btnGrdNewRowSeparatorId = "grdSupplier_btnNewRowSeparator";
            var btnNewRow = angular.element('#' + btnGrdNewRowId);
            var btnNewRowSeparator = angular.element('#' + btnGrdNewRowSeparatorId);
            var pos = this.getColumnIndex("action_buttons");

            if (this.isCompanySuppAdmin !== true) {
                btnNewRow.hide();
                btnNewRowSeparator.hide();
                if (pos > -1) {
                    this.gridOptions.columnDefs[pos].visible = false;
                }
                return;
            }

            if (this.isCompanySuppManualAllowed === true) {
                btnNewRow.show();
                btnNewRowSeparator.show();
                if (pos > -1) {
                    this.gridOptions.columnDefs[pos].visible = true;
                }
            } else {
                btnNewRow.hide();
                btnNewRowSeparator.hide();
                if (pos > -1) {
                    this.gridOptions.columnDefs[pos].visible = false;
                }
            }
        }

        private setGridHeader() : void {
            if (!this.isValueNullOrUndefined(this.selectedCompanyId)) {
                var currCompany: Company[] = this.$filter("filter")(this.suppCompanies, { id: this.selectedCompanyId }, true);
                var companyName: string = "";
                var supplierInfo: string = "";
                if (!this.isValueNullOrUndefined(currCompany) && currCompany.length > 0) {
                    companyName = currCompany[0].country_code;
                    if (this.isCompanySuppAdmin === true) {
                        supplierInfo = currCompany[0].supplier_source;
                    }
                    this.isCompanySuppManualAllowed = currCompany[0].is_supplier_maintenance_allowed;
                }

                if (this.isCompanySuppManualAllowed === true) {
                    strGridHeaderInfo = this.locSuppliersText + " " + companyName;
                } else {
                    
                    var strGridHeaderInfo: string = "";
                    if (this.isStringValueNullOrEmpty(supplierInfo)) {
                        supplierInfo = '';
                    } else {
                        supplierInfo = ', ' + this.locSuppliersLoadedFromText + ' ' + supplierInfo;
                    }
                    if (this.isSuppLoadedUpToDate === true) {
                        strGridHeaderInfo = (this.locSuppliersText + " " + companyName + "<span class=\"reget-text-small\"> ( "
                            + this.locLastUploadDateText + ": " + this.lastSuppUploadDate + supplierInfo + " )</span>");
                    } else {
                        strGridHeaderInfo = (this.locSuppliersText + " " + companyName + "<span class=\"reget-text-small\"> ( " + this.locLastUploadDateText + ": "
                            + this.lastSuppUploadDate + "</span><img src=\"" + this.getRegetRootUrl() + "Content/Images/Controll/Warning16.png" + "\" style=\"margin-left:5px;margin-right:5px;\"><span class=\"reget-text-small\">" + supplierInfo + ")</span>");
                    }

                }

                $("#grdHeaderTitle_grdSupplier").html(strGridHeaderInfo);
            } else {
                $("#grdHeaderTitle_grdSupplier").html(this.locSuppliersText);
            }
        }

        private importSuppliers(ev : MouseEvent) : void {
            let strMessage : string = this.locSupplierUpdateConfirmText;

            this.$mdDialog.show(
                {
                    template: this.getConfirmDialogTemplate(
                        strMessage,
                        this.locConfirmationText,
                        this.locYesText,
                        this.locNoText,
                        "confirmImportDialog()",
                        "closeDialog()"),
                    locals: {
                        suppCtrl: this
                    },
                    controller: this.dialogConfirmSuppController
                });

            //var confirm = this.$mdDialog.confirm()
            //    .title(this.locConfirmationText)
            //    .textContent(strMessage)
            //    .ariaLabel("LoadSuppliersConfirm")
            //    .targetEvent(ev)
            //    .ok(this.locNoText)
            //    .cancel(this.locYesText);

            //this.$mdDialog.show(confirm).then(() => {

            //}, () => {
            //    try {
            //        this.importSupplierService();

            //    } catch (ex) {
            //        this.hideLoaderWrapper();
            //        this.displayErrorMsg();
            //    }
            //});
        }

        private dialogConfirmSuppController($scope, $mdDialog, suppCtrl: SupplierController): void {

            $scope.closeDialog = function () {
                $mdDialog.hide();
            };

            $scope.confirmImportDialog = () => {
                $mdDialog.hide();
                suppCtrl.importSupplierService();
            };

            $scope.confirmManualDialog = () => {
                $mdDialog.hide();
                suppCtrl.saveSupplierMaintenanceSetComp();
            };
            
        }

        private deleteSuppAdmin(iAdminId: number, sindex: number, isCompanyAdmin: boolean) {
            if (isCompanyAdmin === true) {
                this.showAlert(this.locWarningText, this.locCannotDeleteCompanyAdminText, this.locCloseText);
            } else {
                this.deleteSuppAdminFromDb(iAdminId, sindex);
            }

        }

        private displaySuppAdminAutoCompl() : void {
            $("#autoSuppAdmin").show();
        }

        private searchParticipant(strName: string): ng.IPromise<Participant[]> {
            return this.filterParticipants(strName);
        }

        private suppAdminSelectedItemChange(item: Participant) : void {
            if (this.isValueNullOrUndefined(item)) {
                return;
            }

            try {
                var suppadmin: Participant[] = this.$filter("filter")(this.supplierAdmins, { participant_id: item.id }, true);

                if (this.isValueNullOrUndefined(suppadmin[0])) {
                    this.saveNewSuppAdmin(item)
                }
            } catch (e) {
                this.displayErrorMsg();
            } finally {
                this.hideSuppAdminAutoCompl();
            }
        }

        private hideSuppAdminAutoCompl() : void {
            this.searchstringsuppadmin = null;

            var $autWrap = $("#autoSuppAdmin").children().first();
            var $autChild = $autWrap.children().first();
            $autChild.val('');

            $("#autoSuppAdmin").hide();
        }

        private toggleManualMaint() : void {

            if (this.isCompanySuppManualAllowed) {
                //switch from manual to automatic
                let msg : string = this.locSwitchToAutoSuppMainConfirmText;
                //var confirm = this.$mdDialog.confirm()
                //    .title(this.locConfirmationText)
                //    .textContent(msg)
                //    .ariaLabel(this.locConfirmationText)
                //    .targetEvent()
                //    .ok(this.locCancelText)
                //    .cancel(this.locYesText);

                //this.$mdDialog.show(confirm).then(function () {

                //}, () => {
                //    this.saveSupplierMaintenanceSetComp();

                //});

                this.$mdDialog.show(
                    {
                        template: this.getConfirmDialogTemplate(
                            msg,
                            this.locConfirmationText,
                            this.locYesText,
                            this.locNoText,
                            "confirmManualDialog()",
                            "closeDialog()"),
                        locals: {
                            suppCtrl: this
                        },
                        controller: this.dialogConfirmSuppController
                    });

            } else {
                this.saveSupplierMaintenanceSetComp();
            }
        }

        public saveSupplierMaintenanceSetComp() : void {
            this.isCompanySuppManualAllowed = !this.isCompanySuppManualAllowed;

            this.saveSupplierMaintenance();
        }

        //private gridDeleteRow(supplier: Supplier, ev: MouseEvent) {
        //    var strMessage = this.locDeleteSupplierConfirmText.replace("{0}", supplier.supp_name);
        //    var confirm = this.$mdDialog.confirm()
        //        .title(this.locConfirmationText)
        //        .textContent(strMessage)
        //        .ariaLabel("DeleteRowConfirm")
        //        .targetEvent(ev)
        //        .ok(this.locNoText)
        //        .cancel(this.locYesText);

        //    this.$mdDialog.show(confirm).then(() => {

        //    }, () => {
        //        this.gridDeleteRowFromDb(supplier, ev);
        //    });

        //}

        public cellClicked(row?: any, col?: any) : void {
            if (this.isCompanySuppAdmin === false) {
                return;
            }

            if (this.isCompanySuppManualAllowed !== true) {
                return;
            }

           
            super.cellClicked(row, col);

        }
        //***************************************************************************
    }
    angular.
        module('RegetApp').
        controller('SupplierController', Kamsyk.RegetApp.SupplierController);

    //export class YesNo {
    //    public value: boolean = false;
    //    public label: string = null;
    //}

}