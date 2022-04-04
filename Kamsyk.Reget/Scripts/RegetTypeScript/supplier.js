/// <reference path="../typings/ui-grid/ui-grid.d.ts" />
/// <reference path="../RegetTypeScript/Base/reget-base.ts" />
/// <reference path="../RegetTypeScript/Base/reget-common.ts" />
/// <reference path="../RegetTypeScript/Base/reget-entity.ts" />
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var Kamsyk;
(function (Kamsyk) {
    var RegetApp;
    (function (RegetApp) {
        angular.module('RegetApp').directive("suppadmin", function () {
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
        var SupplierController = /** @class */ (function (_super) {
            __extends(SupplierController, _super);
            //**********************************************************
            //Constructor
            function SupplierController($scope, $http, $filter, $mdDialog, $mdToast, uiGridConstants, $q, $timeout) {
                var _this = _super.call(this, $scope, $http, $filter, $mdDialog, $mdToast, $q, uiGridConstants, $timeout) || this;
                _this.$scope = $scope;
                _this.$http = $http;
                _this.$filter = $filter;
                _this.$mdDialog = $mdDialog;
                _this.$mdToast = $mdToast;
                _this.uiGridConstants = uiGridConstants;
                _this.$q = $q;
                _this.$timeout = $timeout;
                //****************************************
                //Abstract properties
                _this.dbGridId = "grdSupplier_rg";
                //*****************************************
                _this.selectedCompanyId = null;
                _this.isPgLoaded = false;
                _this.isCompanySuppAdmin = false;
                _this.isGridHidden = true;
                _this.isCompanyAdmin = false;
                _this.isCompanySuppManualAllowed = false;
                _this.isSuppAdminLoaded = false;
                _this.isParticipantLoaded = false;
                _this.isOfficeLoaded = false;
                _this.isSuppUploadDateLoaded = false;
                _this.isSuppLoadedUpToDate = false;
                _this.lastSuppUploadDate = null;
                _this.suppCompanies = null;
                _this.supplierAdmins = null;
                _this.searchstringsuppadmin = null;
                _this.selectedCgAdmin = null;
                //**********************************************************
                //Loc Text
                _this.locWarningText = $("#WarningText").val();
                _this.locErrMsg = $("#ErrMsgText").val();
                _this.locNameText = $("#NameText").val();
                _this.locSuppliersText = $("#SuppliersText").val();
                _this.locStreetText = $("#StreetText").val();
                _this.locCityText = $("#CityText").val();
                _this.locZipText = $("#ZipText").val();
                _this.locCountryText = $("#CountryText").val();
                _this.locPhoneText = $("#PhoneText").val();
                _this.locMailText = $("#MailText").val();
                _this.locContactPersonText = $("#ContactPersonText").val();
                _this.locSupplierIdText = $("#SupplierIdText").val();
                _this.locActiveText = $("#ActiveText").val();
                _this.locSupplierUpdateConfirmText = $("#SupplierUpdateConfirmText").val();
                _this.locLoadSuccessfullyText = $("#LoadSuccessfullyText").val();
                _this.locLastUploadDateText = $("#LastUploadDateText").val();
                _this.locNeverText = $("#NeverText").val();
                _this.locSuppliersLoadedFromText = $("#SuppliersLoadedFromText").val();
                _this.locSwitchToAutoSuppMainConfirmText = $("#SwitchToAutoSuppMainConfirmText").val();
                _this.locManualSupplierMaintenanceIsNotAllowedText = $("#ManualSupplierMaintenanceIsNotAllowedText").val();
                _this.locDeleteSupplierConfirmText = $("#DeleteSupplierConfirmText").val();
                _this.locSupplierWasDisabledText = $("#SupplierWasDisabledText").val();
                _this.locDuplicitySuppllierNameText = $("#DuplicitySuppllierNameText").val();
                _this.locCannotDeleteCompanyAdminText = $("#CannotDeleteCompanyAdminText").val();
                _this.locNotificationText = $("#NotificationText").val();
                //**********************************************************
                //This one "uiGrid.ISelectOptio" does NOT work !!!!!! Default Filter is not set properly
                //private yesNo: Array<uiGrid.ISelectOption> = [{ value: "true", label: this.locYesText }, { value: "false", label: this.locNoText }];
                _this.yesNo = [{ value: true, label: _this.locYesText }, { value: false, label: _this.locNoText }];
                //***************************************************************
                _this.$onInit = function () { };
                _this.deleteUrl = _this.getRegetRootUrl() + 'Supplier/DeleteSupplier' + '?t=' + new Date().getTime();
                _this.setGrid();
                _this.loadData();
                return _this;
            }
            //****************************************************************************
            //Abstract
            SupplierController.prototype.exportToXlsUrl = function () {
                return this.getRegetRootUrl() + 'Report/GetSuppliersReport?companyId=' + this.selectedCompanyId +
                    '&filter=' + encodeURI(this.filterUrl) +
                    '&sort=' + this.sortColumnsUrl +
                    '&t=' + new Date().getTime();
            };
            SupplierController.prototype.getControlColumnsCount = function () {
                return 2;
            };
            SupplierController.prototype.getDuplicityErrMsg = function (rowEntity) {
                return this.locDuplicitySuppllierNameText.replace("{0}", rowEntity.supp_name);
                ;
                ;
            };
            SupplierController.prototype.getSaveRowUrl = function () {
                return this.getRegetRootUrl() + 'Supplier/SaveSupplierData?t=' + new Date().getTime();
            };
            SupplierController.prototype.insertRow = function () {
                var newSupplier = new RegetApp.Supplier();
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
            };
            SupplierController.prototype.isRowChanged = function () {
                if (this.editRow === null) {
                    return true;
                }
                var origSupp = this.editRowOrig;
                var updSupp = this.editRow;
                var tmpSuppliers = this.gridOptions.data;
                var suppliers = tmpSuppliers;
                var isChanged = false;
                var id = updSupp.id;
                var supplier = this.$filter("filter")(suppliers, { id: id }, true);
                if (id < 0) {
                    //new row
                    this.newRowIndex = null;
                    return true;
                }
                else {
                    if (origSupp.supp_name !== updSupp.supp_name) {
                        return true;
                    }
                    else if (origSupp.supplier_id !== updSupp.supplier_id) {
                        return true;
                    }
                    else if (origSupp.street_part1 !== updSupp.street_part1) {
                        return true;
                    }
                    else if (origSupp.city !== updSupp.city) {
                        return true;
                    }
                    else if (origSupp.zip !== updSupp.zip) {
                        return true;
                    }
                    else if (origSupp.country !== updSupp.country) {
                        return true;
                    }
                    else if (origSupp.contact_person !== updSupp.contact_person) {
                        return true;
                    }
                    else if (origSupp.phone !== updSupp.phone) {
                        return true;
                    }
                    else if (origSupp.email !== updSupp.email) {
                        return true;
                    }
                    else if (origSupp.active !== updSupp.active) {
                        return true;
                    }
                    else if (supplier[0].id < -1) {
                        return true;
                    }
                }
                return false;
            };
            SupplierController.prototype.isRowEntityValid = function (supplier) {
                if (this.isStringValueNullOrEmpty(supplier.supp_name)) {
                    return this.locMissingMandatoryText;
                }
                return null;
            };
            SupplierController.prototype.loadGridData = function () {
                this.getSupplierDataFromDb();
            };
            SupplierController.prototype.getMsgDisabled = function (supplier) {
                return this.locSupplierWasDisabledText.replace("{0}", supplier.supp_name);
            };
            SupplierController.prototype.getMsgDeleteConfirm = function (supplier) {
                return this.locDeleteSupplierConfirmText.replace("{0}", supplier.supp_name);
                ;
            };
            SupplierController.prototype.getErrorMsgByErrId = function (errId, msg) {
                return this.locErrorMsgText;
            };
            SupplierController.prototype.getDbGridId = function () {
                return this.dbGridId;
            };
            //*****************************************************************************
            //****************************************************************************
            //Methods
            SupplierController.prototype.setGrid = function () {
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
            };
            //****************************************************************************
            //***************************************************************************
            //Http
            SupplierController.prototype.getOfficesData = function () {
                var _this = this;
                this.isError = false;
                this.showLoaderBoxOnly(this.isError);
                this.$http.get(this.getRegetRootUrl() + 'Supplier/GetUserOfficeData?t=' + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        _this.suppCompanies = tmpData;
                        _this.isOfficeLoaded = true;
                        if (!_this.isValueNullOrUndefined(_this.suppCompanies) && _this.suppCompanies.length === 1) {
                            _this.selectedCompanyId = _this.suppCompanies[0].id;
                            _this.getSupplierData();
                        }
                        _this.hideLoaderWrapper();
                    }
                    catch (e) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        _this.hideLoaderWrapper();
                    }
                }, function (response) {
                    _this.hideLoaderWrapper();
                    _this.displayErrorMsg();
                });
            };
            SupplierController.prototype.getSupplierDataFromDb = function () {
                var _this = this;
                this.isSuppUploadDateLoaded = false;
                this.isPgLoaded = false;
                this.isCompanySuppAdmin = false;
                this.isGridHidden = false;
                this.isCompanyAdmin = false;
                this.isCompanySuppManualAllowed = false;
                this.isSuppAdminLoaded = false;
                this.isParticipantLoaded = false;
                this.showLoaderBoxOnly(this.isError);
                this.$http.get(this.getRegetRootUrl() + '/Supplier/GetLastCompanySupplierUpload?companyId=' + this.selectedCompanyId +
                    '&t=' + new Date().getTime(), {}).then(function (response) {
                    try {
                        _this.getSupplierAdmins();
                        var tmpData = response.data;
                        _this.lastSuppUploadDate = tmpData;
                        _this.isSuppLoadedUpToDate = false;
                        if (_this.isStringValueNullOrEmpty(_this.lastSuppUploadDate)) {
                            _this.lastSuppUploadDate = _this.locNeverText;
                        }
                        else {
                            var suppInfoItems = _this.lastSuppUploadDate.split('|');
                            _this.lastSuppUploadDate = suppInfoItems[0];
                            if (suppInfoItems[1] === '1') {
                                _this.isSuppLoadedUpToDate = true;
                            }
                        }
                        _this.isSuppUploadDateLoaded = true;
                        _this.getSupplierDataOnlyFromDb();
                        var office = _this.$filter("filter")(_this.suppCompanies, { id: _this.selectedCompanyId }, true);
                        if (!_this.isValueNullOrUndefined(office[0])) {
                            if (office[0].is_supplier_admin) {
                                _this.isCompanySuppAdmin = true;
                            }
                            if (office[0].is_company_admin) {
                                _this.isCompanyAdmin = true;
                                _this.isCompanySuppAdmin = true;
                            }
                            _this.isCompanySuppManualAllowed = office[0].is_supplier_maintenance_allowed;
                            _this.enableGrid();
                        }
                        _this.enableGrid();
                        _this.hideLoaderWrapper();
                    }
                    catch (e) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        _this.hideLoader();
                    }
                }, function (response) {
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
            };
            SupplierController.prototype.getSupplierAdmins = function () {
                var _this = this;
                this.showLoaderBoxOnly(this.isError);
                this.$http.get(this.getRegetRootUrl() + '/Supplier/GetSuppliersAdmins?companyId=' + this.selectedCompanyId +
                    '&t=' + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        _this.supplierAdmins = tmpData;
                        _this.isSuppAdminLoaded = true;
                        _this.hideLoaderWrapper();
                        _this.hideLoader();
                    }
                    catch (e) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        _this.hideLoaderWrapper();
                    }
                }, function (response) {
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
            };
            SupplierController.prototype.getSupplierDataOnlyFromDb = function () {
                var _this = this;
                this.isError = false;
                $("#divGrdSupplier").show();
                this.showLoaderBoxOnly(this.isError);
                this.$http.get(this.getRegetRootUrl() + '/Supplier/GetSupplierData?companyId=' + this.selectedCompanyId +
                    '&filter=' + encodeURI(this.filterUrl) +
                    '&pageSize=' + this.pageSize +
                    '&currentPage=' + this.currentPage +
                    '&sort=' + this.sortColumnsUrl +
                    '&t=' + new Date().getTime(), {}).then(function (response) {
                    try {
                        if (_this.isGridHidden === true) {
                            _this.isGridHidden = false;
                            //because of filter Yes/No
                            _this.getSupplierData();
                            return;
                        }
                        var tmpData = response.data;
                        _this.gridOptions.data = tmpData.db_data;
                        _this.rowsCount = tmpData.rows_count;
                        _this.setGridHeader();
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
                        if (!_this.isValueNullOrUndefined(_this.gridApi)) {
                            _this.gridApi.core.notifyDataChange(_this.uiGridConstants.dataChange.COLUMN);
                        }
                        _this.setGridSettingData();
                        //********************************************************************
                        //it is very important otherwise 50 lines are nod diplaye dproperly !!!
                        _this.gridOptions.virtualizationThreshold = _this.rowsCount + 1;
                        //********************************************************************
                        _this.isPgLoaded = true;
                        _this.hideLoaderWrapper();
                    }
                    catch (e) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        _this.hideLoaderWrapper();
                    }
                }, function (response) {
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
            };
            SupplierController.prototype.importSupplierService = function () {
                var _this = this;
                this.isError = false;
                this.showLoader(this.isError);
                this.$http.get(this.getRegetRootUrl() + 'Supplier/ImportSuppliers?companyId=' + this.selectedCompanyId
                    + '&t=' + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        _this.getSupplierData();
                        _this.hideLoaderWrapper();
                        _this.showInfo(_this.locNotificationText, _this.locLoadSuccessfullyText, _this.locCloseText);
                    }
                    catch (e) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        _this.hideLoaderWrapper();
                    }
                }, function (response) {
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
            };
            SupplierController.prototype.deleteSuppAdminFromDb = function (iAdminId, sindex) {
                var _this = this;
                this.showLoader(this.isError);
                this.$http.post(this.getRegetRootUrl() + '/Supplier/DeleteSuppAdmin?adminId=' + iAdminId +
                    '&companyId=' + this.selectedCompanyId +
                    '&t=' + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        var strResult = tmpData.string_value;
                        if (_this.isStringValueNullOrEmpty(strResult)) {
                            _this.supplierAdmins.splice(sindex, 1);
                        }
                    }
                    catch (e) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        _this.hideLoader();
                    }
                }, function (response) {
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
            };
            SupplierController.prototype.saveNewSuppAdmin = function (user) {
                var _this = this;
                this.showLoader(this.isError);
                this.$http.post(this.getRegetRootUrl() + '/Supplier/SaveNewSuppAdmin?adminId=' + user.id +
                    '&companyId=' + this.selectedCompanyId +
                    '&t=' + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        var strResult = tmpData.string_value;
                        if (_this.isStringValueNullOrEmpty(strResult)) {
                            var newAdmin = new RegetApp.Participant();
                            newAdmin.id = user.id;
                            newAdmin.surname = user.surname;
                            newAdmin.first_name = user.first_name;
                            _this.supplierAdmins.push(newAdmin);
                        }
                    }
                    catch (e) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        _this.hideLoader();
                    }
                }, function (response) {
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
            };
            SupplierController.prototype.saveSupplierMaintenance = function () {
                var _this = this;
                this.showLoaderBoxOnly(this.isError);
                var jsonUserGridData = JSON.stringify(this.userGridSettings);
                this.$http.post(this.getRegetRootUrl() + 'Supplier/SaveSupplierMaintenance?companyId='
                    + this.selectedCompanyId + '&' + 'isManualSuppMaintenance='
                    + this.isCompanySuppManualAllowed + '&t=' + new Date().getTime(), {}).then(function (response) {
                    try {
                        //var tmpData: any = response.data;
                        var currCompany = _this.$filter("filter")(_this.suppCompanies, { id: _this.selectedCompanyId }, true);
                        currCompany[0].is_supplier_maintenance_allowed = _this.isCompanySuppManualAllowed;
                        //neccessary to refresh grid settngs - show/hide columns
                        _this.getSupplierDataOnlyFromDb();
                        _this.enableGrid();
                        _this.hideLoader();
                    }
                    catch (e) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        _this.hideLoader();
                    }
                }, function (response) {
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
            };
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
            SupplierController.prototype.loadData = function () {
                this.getOfficesData();
            };
            SupplierController.prototype.getSupplierData = function () {
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
            };
            SupplierController.prototype.hideLoaderWrapper = function () {
                if (this.isError || (this.isSuppUploadDateLoaded === true &&
                    this.isPgLoaded === true &&
                    this.isSuppAdminLoaded === true &&
                    this.isParticipantLoaded === true)) {
                    this.hideLoader();
                }
            };
            SupplierController.prototype.enableGrid = function () {
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
                }
                else {
                    btnNewRow.hide();
                    btnNewRowSeparator.hide();
                    if (pos > -1) {
                        this.gridOptions.columnDefs[pos].visible = false;
                    }
                }
            };
            SupplierController.prototype.setGridHeader = function () {
                if (!this.isValueNullOrUndefined(this.selectedCompanyId)) {
                    var currCompany = this.$filter("filter")(this.suppCompanies, { id: this.selectedCompanyId }, true);
                    var companyName = "";
                    var supplierInfo = "";
                    if (!this.isValueNullOrUndefined(currCompany) && currCompany.length > 0) {
                        companyName = currCompany[0].country_code;
                        if (this.isCompanySuppAdmin === true) {
                            supplierInfo = currCompany[0].supplier_source;
                        }
                        this.isCompanySuppManualAllowed = currCompany[0].is_supplier_maintenance_allowed;
                    }
                    if (this.isCompanySuppManualAllowed === true) {
                        strGridHeaderInfo = this.locSuppliersText + " " + companyName;
                    }
                    else {
                        var strGridHeaderInfo = "";
                        if (this.isStringValueNullOrEmpty(supplierInfo)) {
                            supplierInfo = '';
                        }
                        else {
                            supplierInfo = ', ' + this.locSuppliersLoadedFromText + ' ' + supplierInfo;
                        }
                        if (this.isSuppLoadedUpToDate === true) {
                            strGridHeaderInfo = (this.locSuppliersText + " " + companyName + "<span class=\"reget-text-small\"> ( "
                                + this.locLastUploadDateText + ": " + this.lastSuppUploadDate + supplierInfo + " )</span>");
                        }
                        else {
                            strGridHeaderInfo = (this.locSuppliersText + " " + companyName + "<span class=\"reget-text-small\"> ( " + this.locLastUploadDateText + ": "
                                + this.lastSuppUploadDate + "</span><img src=\"" + this.getRegetRootUrl() + "Content/Images/Controll/Warning16.png" + "\" style=\"margin-left:5px;margin-right:5px;\"><span class=\"reget-text-small\">" + supplierInfo + ")</span>");
                        }
                    }
                    $("#grdHeaderTitle_grdSupplier").html(strGridHeaderInfo);
                }
                else {
                    $("#grdHeaderTitle_grdSupplier").html(this.locSuppliersText);
                }
            };
            SupplierController.prototype.importSuppliers = function (ev) {
                var strMessage = this.locSupplierUpdateConfirmText;
                this.$mdDialog.show({
                    template: this.getConfirmDialogTemplate(strMessage, this.locConfirmationText, this.locYesText, this.locNoText, "confirmImportDialog()", "closeDialog()"),
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
            };
            SupplierController.prototype.dialogConfirmSuppController = function ($scope, $mdDialog, suppCtrl) {
                $scope.closeDialog = function () {
                    $mdDialog.hide();
                };
                $scope.confirmImportDialog = function () {
                    $mdDialog.hide();
                    suppCtrl.importSupplierService();
                };
                $scope.confirmManualDialog = function () {
                    $mdDialog.hide();
                    suppCtrl.saveSupplierMaintenanceSetComp();
                };
            };
            SupplierController.prototype.deleteSuppAdmin = function (iAdminId, sindex, isCompanyAdmin) {
                if (isCompanyAdmin === true) {
                    this.showAlert(this.locWarningText, this.locCannotDeleteCompanyAdminText, this.locCloseText);
                }
                else {
                    this.deleteSuppAdminFromDb(iAdminId, sindex);
                }
            };
            SupplierController.prototype.displaySuppAdminAutoCompl = function () {
                $("#autoSuppAdmin").show();
            };
            SupplierController.prototype.searchParticipant = function (strName) {
                return this.filterParticipants(strName);
            };
            SupplierController.prototype.suppAdminSelectedItemChange = function (item) {
                if (this.isValueNullOrUndefined(item)) {
                    return;
                }
                try {
                    var suppadmin = this.$filter("filter")(this.supplierAdmins, { participant_id: item.id }, true);
                    if (this.isValueNullOrUndefined(suppadmin[0])) {
                        this.saveNewSuppAdmin(item);
                    }
                }
                catch (e) {
                    this.displayErrorMsg();
                }
                finally {
                    this.hideSuppAdminAutoCompl();
                }
            };
            SupplierController.prototype.hideSuppAdminAutoCompl = function () {
                this.searchstringsuppadmin = null;
                var $autWrap = $("#autoSuppAdmin").children().first();
                var $autChild = $autWrap.children().first();
                $autChild.val('');
                $("#autoSuppAdmin").hide();
            };
            SupplierController.prototype.toggleManualMaint = function () {
                if (this.isCompanySuppManualAllowed) {
                    //switch from manual to automatic
                    var msg = this.locSwitchToAutoSuppMainConfirmText;
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
                    this.$mdDialog.show({
                        template: this.getConfirmDialogTemplate(msg, this.locConfirmationText, this.locYesText, this.locNoText, "confirmManualDialog()", "closeDialog()"),
                        locals: {
                            suppCtrl: this
                        },
                        controller: this.dialogConfirmSuppController
                    });
                }
                else {
                    this.saveSupplierMaintenanceSetComp();
                }
            };
            SupplierController.prototype.saveSupplierMaintenanceSetComp = function () {
                this.isCompanySuppManualAllowed = !this.isCompanySuppManualAllowed;
                this.saveSupplierMaintenance();
            };
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
            SupplierController.prototype.cellClicked = function (row, col) {
                if (this.isCompanySuppAdmin === false) {
                    return;
                }
                if (this.isCompanySuppManualAllowed !== true) {
                    return;
                }
                _super.prototype.cellClicked.call(this, row, col);
            };
            return SupplierController;
        }(RegetApp.BaseRegetGridTs));
        RegetApp.SupplierController = SupplierController;
        angular.
            module('RegetApp').
            controller('SupplierController', Kamsyk.RegetApp.SupplierController);
        //export class YesNo {
        //    public value: boolean = false;
        //    public label: string = null;
        //}
    })(RegetApp = Kamsyk.RegetApp || (Kamsyk.RegetApp = {}));
})(Kamsyk || (Kamsyk = {}));
//# sourceMappingURL=supplier.js.map