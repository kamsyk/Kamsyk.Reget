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
        var SupplierDetailController = /** @class */ (function (_super) {
            __extends(SupplierDetailController, _super);
            //private locDuplicityMailText: string = $("#DuplicityMailText").val();
            //**********************************************************
            //**********************************************************
            //Constructor
            function SupplierDetailController($scope, $http, $filter, $mdDialog, $mdToast, uiGridConstants, $q, $timeout) {
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
                _this.dbGridId = "grdSupplierDetail_rg";
                //*****************************************
                //**********************************************************
                //Properties
                _this.iSupplierId = null;
                _this.isSupplierLoaded = false;
                _this.isContactLoaded = false;
                _this.supplier = null;
                //**********************************************************
                //**********************************************************
                //Loc Text
                _this.locFirstName = $("#FirstNameText").val();
                _this.locSurname = $("#SurnameText").val();
                _this.locMail = $("#MailText").val();
                _this.locPhone = $("#PhoneText").val();
                _this.locDeleteSuppliereContactConfirmText = $("#DeleteSuppliereContactConfirmText").val();
                //***************************************************************
                _this.$onInit = function () { };
                _this.deleteUrl = _this.getRegetRootUrl() + 'Supplier/DeleteSupplierContact' + '?t=' + new Date().getTime();
                _this.setGrid();
                _this.loadInit();
                return _this;
            }
            //****************************************************************************
            //Abstract
            SupplierDetailController.prototype.exportToXlsUrl = function () {
                return null;
            };
            SupplierDetailController.prototype.getControlColumnsCount = function () {
                return 2;
            };
            SupplierDetailController.prototype.getDuplicityErrMsg = function (supplierContact) {
                //return this.locDuplicityMailText.replace("{0}", supplierContact.email);
                return null;
            };
            SupplierDetailController.prototype.getSaveRowUrl = function () {
                return this.getRegetRootUrl() + "Supplier/SaveContactData?t=" + new Date().getTime();
            };
            SupplierDetailController.prototype.insertRow = function () {
                var newContact = new RegetApp.SupplierContact();
                newContact.id = -10;
                newContact.supplier_id = this.iSupplierId;
                newContact.surname = "";
                newContact.first_name = "";
                newContact.email = "";
                newContact.phone = "";
                this.insertBaseRow(newContact);
            };
            SupplierDetailController.prototype.isRowChanged = function () {
                if (this.editRow === null) {
                    return true;
                }
                var origContact = this.editRowOrig;
                var updContact = this.editRow;
                var id = updContact.id;
                if (id < 0) {
                    //new row
                    this.newRowIndex = null;
                    return true;
                }
                else {
                    if (origContact.email !== updContact.email) {
                        return true;
                    }
                    else if (origContact.first_name !== updContact.first_name) {
                        return true;
                    }
                    else if (origContact.surname !== updContact.surname) {
                        return true;
                    }
                    else if (origContact.phone !== updContact.phone) {
                        return true;
                    }
                }
                return false;
            };
            SupplierDetailController.prototype.isRowEntityValid = function (supplierContact) {
                if (this.isStringValueNullOrEmpty(supplierContact.email)) {
                    return this.locMissingMandatoryText;
                }
                return null;
            };
            SupplierDetailController.prototype.loadGridData = function () {
                this.getContactsData();
            };
            SupplierDetailController.prototype.getMsgDisabled = function (supplierContact) {
                return null;
            };
            SupplierDetailController.prototype.getMsgDeleteConfirm = function (supplierContact) {
                return this.locDeleteSuppliereContactConfirmText
                    .replace("{0}", supplierContact.email);
            };
            SupplierDetailController.prototype.getErrorMsgByErrId = function (errId, msg) {
                return this.locErrorMsgText;
            };
            SupplierDetailController.prototype.getDbGridId = function () {
                return this.dbGridId;
            };
            //*****************************************************************************
            //***************************************************************************
            //HTTP Methods
            SupplierDetailController.prototype.loadSupplier = function () {
                var _this = this;
                this.showLoaderBoxOnly(this.isError);
                this.$http.get(this.getRegetRootUrl() + "Supplier/GetSupplierById?id=" + this.iSupplierId + "&t=" + new Date().getTime(), {}).then(function (response) {
                    try {
                        var result = response.data;
                        _this.supplier = result;
                        //this.getContactsData();
                        _this.getLoadDataGridSettings();
                        _this.isSupplierLoaded = true;
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
            SupplierDetailController.prototype.getContactsData = function () {
                var _this = this;
                if (this.isValueNullOrUndefined(this.$http)) {
                    return;
                }
                this.showLoader(this.isError);
                this.$http.get(this.getRegetRootUrl() + "/Supplier/GetContactsData?supplierId=" + this.iSupplierId +
                    "&filter = " + encodeURI(this.filterUrl) +
                    "&pageSize=" + this.pageSize +
                    "&currentPage=" + this.currentPage +
                    "&sort=" + this.sortColumnsUrl +
                    "&t=" + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        _this.gridOptions.data = tmpData.db_data;
                        _this.rowsCount = tmpData.rows_count;
                        _this.isContactLoaded = true;
                        _this.testLoadDataCount++;
                        //this.loadGridSettings();
                        _this.setGridSettingData();
                        //********************************************************************
                        //it is very important otherwise 50 lines are nod diplaye dproperly !!!
                        _this.gridOptions.virtualizationThreshold = _this.rowsCount + 1;
                        //********************************************************************
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
            //***************************************************************************
            //****************************************************************************
            //Methods
            SupplierDetailController.prototype.setGrid = function () {
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
            };
            //****************************************************************************
            //***************************************************************************
            //Methods
            SupplierDetailController.prototype.loadInit = function () {
                this.showLoader(this.isError);
                try {
                    this.iSupplierId = this.getUrlParamValueInt("id");
                    this.loadSupplier();
                }
                catch (ex) {
                    this.hideLoader();
                    this.displayErrorMsg();
                }
            };
            SupplierDetailController.prototype.hideLoaderWrapper = function () {
                if (this.isError || (this.isSupplierLoaded
                    && this.isContactLoaded)) {
                    this.hideLoader();
                    this.isError = false;
                }
            };
            return SupplierDetailController;
        }(RegetApp.BaseRegetGridTs));
        RegetApp.SupplierDetailController = SupplierDetailController;
        angular.
            module('RegetApp').
            controller('SupplierDetailController', Kamsyk.RegetApp.SupplierDetailController);
    })(RegetApp = Kamsyk.RegetApp || (Kamsyk.RegetApp = {}));
})(Kamsyk || (Kamsyk = {}));
//# sourceMappingURL=supplier-detail.js.map