/// <reference path="../typings/ui-grid/ui-grid.d.ts" />
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
        var AddressController = /** @class */ (function (_super) {
            __extends(AddressController, _super);
            //*************************************************************
            //**********************************************************
            //Constructor
            function AddressController($scope, $http, $filter, $mdDialog, $mdToast, uiGridConstants, $q, $timeout) {
                var _this = _super.call(this, $scope, $http, $filter, $mdDialog, $mdToast, $q, uiGridConstants, $timeout) || this;
                _this.$scope = $scope;
                _this.$http = $http;
                _this.$filter = $filter;
                _this.$mdDialog = $mdDialog;
                _this.$mdToast = $mdToast;
                _this.uiGridConstants = uiGridConstants;
                _this.$q = $q;
                _this.$timeout = $timeout;
                //**************************************************************
                //Properties
                _this.isAddressesLoaded = false;
                _this.isCompanyLoaded = false;
                _this.companies = null;
                //**************************************************************
                //*************************************************************
                //Get Loc Texts
                _this.locWarningText = $("#WarningText").val();
                _this.locErrMsg = $("#ErrMsgText").val();
                _this.locCompanyText = $("#CompanyText").val();
                _this.locCompanyNameText = $("#CompanyNameText").val();
                _this.locStreetText = $("#StreetText").val();
                _this.locCityText = $("#CityText").val();
                _this.locZipText = $("#ZipText").val();
                _this.locDeleteAddressConfirmText = $("#DeleteAddressConfirmText").val();
                _this.locNotFoundText = $("#NotFoundText").val();
                _this.locDuplicityAddressText = $("#DuplicityAddressText").val();
                //***************************************************************
                _this.$onInit = function () { };
                _this.dbGridId = "grdAddress_rg";
                _this.deleteUrl = _this.getRegetRootUrl() + "Address/DeleteAddress" + "?t=" + new Date().getTime();
                _this.setGrid();
                _this.loadData();
                return _this;
            }
            //****************************************************************************
            //Abstract
            AddressController.prototype.exportToXlsUrl = function () {
                return this.getRegetRootUrl() + "Report/GetAddressesReport?" +
                    "filter=" + encodeURI(this.filterUrl) +
                    "&sort=" + this.sortColumnsUrl +
                    "&t=" + new Date().getTime();
            };
            AddressController.prototype.getControlColumnsCount = function () {
                return 2;
            };
            AddressController.prototype.getDuplicityErrMsg = function (rowEntity) {
                var addressText = this.getFullAddress(rowEntity);
                return this.locDuplicityAddressText.replace("{0}", addressText);
            };
            AddressController.prototype.getSaveRowUrl = function () {
                return this.getRegetRootUrl() + "Address/SaveAddressData?t=" + new Date().getTime();
            };
            AddressController.prototype.insertRow = function () {
                var newAddress = new RegetApp.Address();
                newAddress.id = -10;
                newAddress.company_name = "";
                newAddress.company_name_text = "";
                newAddress.street = "";
                newAddress.city = "";
                newAddress.zip = "";
                this.insertBaseRow(newAddress);
            };
            AddressController.prototype.isRowChanged = function () {
                if (this.editRow === null) {
                    return true;
                }
                var origAddress = this.editRowOrig;
                var updAddress = this.editRow;
                var tmpAddresses = this.gridOptions.data;
                var addresses = tmpAddresses;
                var isChanged = false;
                var id = updAddress.id;
                var address = this.$filter("filter")(addresses, { id: id }, true);
                if (id < 0) {
                    //new row
                    this.newRowIndex = null;
                    return true;
                }
                else {
                    if (origAddress.company_name !== updAddress.company_name) {
                        return true;
                    }
                    else if (origAddress.company_name_text !== updAddress.company_name_text) {
                        return true;
                    }
                    else if (origAddress.street !== updAddress.street) {
                        return true;
                    }
                    else if (origAddress.city !== updAddress.city) {
                        return true;
                    }
                    else if (origAddress.zip !== updAddress.zip) {
                        return true;
                    }
                }
                return false;
            };
            AddressController.prototype.isRowEntityValid = function (address) {
                if (this.isStringValueNullOrEmpty(address.company_name)) {
                    return this.locMissingMandatoryText;
                }
                if (this.isStringValueNullOrEmpty(address.company_name_text)) {
                    return this.locMissingMandatoryText;
                }
                return null;
            };
            AddressController.prototype.loadGridData = function () {
                this.getAddressData();
            };
            AddressController.prototype.getMsgDisabled = function (address) {
                return null; //this.locSupplierWasDisabledText.replace("{0}", supplier.supp_name);
            };
            AddressController.prototype.getMsgDeleteConfirm = function (address) {
                return this.locDeleteAddressConfirmText.replace("{0}", address.company_name);
                ;
            };
            AddressController.prototype.getErrorMsgByErrId = function (errId, msg) {
                return this.locErrorMsgText;
            };
            AddressController.prototype.getDbGridId = function () {
                return this.dbGridId;
            };
            //*****************************************************************************
            //****************************************************************************
            //http
            AddressController.prototype.getAddressData = function () {
                var _this = this;
                this.showLoader(this.isError);
                this.$http.get(this.getRegetRootUrl() + "Address/GetAddressesAdminData?filter=" + encodeURI(this.filterUrl) +
                    "&pageSize=" + this.pageSize +
                    "&currentPage=" + this.currentPage +
                    "&sort=" + this.sortColumnsUrl +
                    "&t=" + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        _this.gridOptions.data = tmpData.db_data;
                        _this.rowsCount = tmpData.rows_count;
                        if (!_this.isValueNullOrUndefined(_this.gridApi)) {
                            _this.gridApi.core.notifyDataChange(_this.uiGridConstants.dataChange.COLUMN);
                        }
                        _this.isAddressesLoaded = true;
                        _this.testLoadDataCount++;
                        //if (this.userGridSettings === null) {
                        //    this.defaultColumnDefs = angular.copy(this.gridOptions.columnDefs);
                        //    this.getDataGridSettings();
                        //}
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
            AddressController.prototype.getCompanies = function () {
                var _this = this;
                this.showLoader(this.isError);
                this.$http.get(this.getRegetRootUrl() + "Participant/GetParticipantCompanies?t=" + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        _this.companies = tmpData;
                        //company
                        var pos = _this.getColumnIndex("company_name_text");
                        if (pos > 0) {
                            _this.gridOptions.columnDefs[pos].filter = {
                                //term: "1",
                                type: _this.uiGridConstants.filter.SELECT,
                                selectOptions: _this.companies
                            };
                            //var filCompanies: uiGrid.edit.IEditDropdown[] = this.companies;
                            _this.gridOptions.columnDefs[pos].editDropdownOptionsArray = _this.companies;
                            _this.gridApi.core.notifyDataChange(_this.uiGridConstants.dataChange.COLUMN);
                        }
                        //this.loadGridData();
                        _this.getLoadDataGridSettings();
                        _this.isCompanyLoaded = true;
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
            //******************************************************************************
            //****************************************************************************
            //Methods
            AddressController.prototype.setGrid = function () {
                var _this = this;
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
                        sortingAlgorithm: function (a, b, rowA, rowB, direction) {
                            return _this.sortNullString(a, b, rowA, rowB, direction);
                        },
                        minWidth: 110
                    },
                    {
                        name: "city", displayName: this.locCityText, field: "city",
                        enableHiding: false,
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextTemplate.html",
                        enableCellEdit: false,
                        sortingAlgorithm: function (a, b, rowA, rowB, direction) {
                            return _this.sortNullString(a, b, rowA, rowB, direction);
                        },
                        minWidth: 110
                    },
                    {
                        name: "zip", displayName: this.locZipText, field: "zip",
                        enableHiding: false,
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextTemplate.html",
                        enableCellEdit: false,
                        sortingAlgorithm: function (a, b, rowA, rowB, direction) {
                            return _this.sortNullString(a, b, rowA, rowB, direction);
                        },
                        minWidth: 50
                    }
                ];
            };
            AddressController.prototype.loadData = function () {
                this.getCompanies();
            };
            AddressController.prototype.hideLoaderWrapper = function () {
                if (this.isError || (this.isAddressesLoaded && this.isCompanyLoaded)) {
                    this.hideLoader();
                    this.isError = false;
                }
            };
            AddressController.prototype.getFullAddress = function (address) {
                var strFullAddress = address.company_name;
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
            };
            return AddressController;
        }(RegetApp.BaseRegetGridTs));
        RegetApp.AddressController = AddressController;
        angular.
            module("RegetApp").
            controller("AddressController", Kamsyk.RegetApp.AddressController);
    })(RegetApp = Kamsyk.RegetApp || (Kamsyk.RegetApp = {}));
})(Kamsyk || (Kamsyk = {}));
//# sourceMappingURL=reget-address.js.map