/// <reference path="../typings/ui-grid/ui-grid.d.ts" />
/// <reference path="../RegetTypeScript/Base/reget-base-grid.ts" />
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
        var CentreController = /** @class */ (function (_super) {
            __extends(CentreController, _super);
            //*******************************************************************
            //**********************************************************
            //Constructor
            function CentreController($scope, $http, $filter, $mdDialog, $mdToast, uiGridConstants, $q, $timeout) {
                var _this = _super.call(this, $scope, $http, $filter, $mdDialog, $mdToast, $q, uiGridConstants, $timeout) || this;
                _this.$scope = $scope;
                _this.$http = $http;
                _this.$filter = $filter;
                _this.$mdDialog = $mdDialog;
                _this.$mdToast = $mdToast;
                _this.uiGridConstants = uiGridConstants;
                _this.$q = $q;
                _this.$timeout = $timeout;
                //**********************************************************************
                //Loc Texts
                _this.locWarningText = $("#WarningText").val();
                _this.locNeverText = $("#NeverText").val();
                _this.locAlwaysText = $("#AlwaysText").val();
                _this.locOptionalText = $("#OptionalText").val();
                _this.locNameText = $("#NameText").val();
                _this.locCompanyText = $("#CompanyText").val();
                _this.locExportPriceText = $("#ExportPriceText").val();
                _this.locMultiOrdererText = $("#MultiOrdererText").val();
                _this.locCanTakeOverText = $("#CanTakeOverText").val();
                _this.locCanRequestorApproveText = $("#CanRequestorApproveText").val();
                _this.locManagerText = $("#ManagerText").val();
                _this.locAddressText = $("#AddressText").val();
                _this.locActiveText = $("#ActiveText").val();
                _this.locDuplicityCentreNameText = $("#DuplicityCentreNameText").val();
                _this.locDeleteCentreConfirmText = $("#DeleteCentreConfirmText").val();
                _this.locCentreWasDisabledText = $("#CentreWasDisabledText").val();
                _this.locNotFoundText = $("#NotFoundText").val();
                //**********************************************************************
                //*******************************************************************
                //Properties
                _this.isCentresLoaded = false;
                _this.isCompanyLoaded = false;
                //private isAddressesLoaded: boolean = false;
                _this.companies = null;
                _this.addresses = null;
                _this.searchstringcentreman = null;
                _this.searchstringaddress = null;
                //private managerText: string = null;
                _this.userInterval = null;
                _this.addressInterval = null;
                _this.yesNo = [{ value: true, label: _this.locYesText }, { value: false, label: _this.locNoText }];
                _this.exportPrice = [{ value: _this.locNeverText, label: _this.locNeverText },
                    { value: _this.locAlwaysText, label: _this.locAlwaysText },
                    { value: _this.locOptionalText, label: _this.locOptionalText }];
                //***************************************************************
                _this.$onInit = function () { };
                _this.dbGridId = "grdCentre_rg";
                _this.deleteUrl = _this.getRegetRootUrl() + "Centre/DeleteCentre" + "?t=" + new Date().getTime();
                _this.setGrid();
                _this.loadData();
                return _this;
            }
            //****************************************************************************
            //Abstract
            CentreController.prototype.exportToXlsUrl = function () {
                return this.getRegetRootUrl() + "Report/GetCentresReport?" +
                    "filter=" + encodeURI(this.filterUrl) +
                    "&sort=" + this.sortColumnsUrl +
                    "&t=" + new Date().getTime();
            };
            CentreController.prototype.getControlColumnsCount = function () {
                return 2;
            };
            CentreController.prototype.getDuplicityErrMsg = function (centre) {
                return this.getLocDuplicityCentreNameText().replace("{0}", centre.name);
            };
            CentreController.prototype.getSaveRowUrl = function () {
                return this.getRegetRootUrl() + "Centre/SaveCentreData?t=" + new Date().getTime();
            };
            CentreController.prototype.insertRow = function () {
                var newCentre = new CentreAdminExtended();
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
            };
            CentreController.prototype.isRowChanged = function () {
                if (this.editRow === null) {
                    return true;
                }
                var origCentre = this.editRowOrig;
                var updCentre = this.editRow;
                var tmpCentres = this.gridOptions.data;
                var id = updCentre.id;
                //var centre: CentreAdminExtended[] = this.$filter("filter")(centres, { id: id }, true);
                if (id < 0) {
                    //new row
                    this.newRowIndex = null;
                    return true;
                }
                else {
                    if (origCentre.name !== updCentre.name) {
                        return true;
                    }
                    else if (origCentre.company_name !== updCentre.company_name) {
                        return true;
                    }
                    else if (origCentre.export_price_text !== updCentre.export_price_text) {
                        return true;
                    }
                    else if (origCentre.multi_orderer !== updCentre.multi_orderer) {
                        return true;
                    }
                    else if (origCentre.other_orderer_can_takeover !== updCentre.other_orderer_can_takeover) {
                        return true;
                    }
                    else if (origCentre.is_approved_by_requestor !== updCentre.is_approved_by_requestor) {
                        return true;
                    }
                    else if (origCentre.manager_id !== updCentre.manager_id) {
                        return true;
                    }
                    else if (origCentre.address_text !== updCentre.address_text) {
                        return true;
                    }
                    else if (origCentre.active !== updCentre.active) {
                        return true;
                    }
                }
                return false;
            };
            CentreController.prototype.isRowEntityValid = function (centre) {
                if (this.isStringValueNullOrEmpty(centre.name)) {
                    return this.locMissingMandatoryText;
                }
                return null;
            };
            CentreController.prototype.loadGridData = function () {
                this.getCentresData();
            };
            CentreController.prototype.getMsgDisabled = function (centre) {
                return this.locCentreWasDisabledText.replace("{0}", centre.name);
            };
            CentreController.prototype.getMsgDeleteConfirm = function (centre) {
                return this.locDeleteCentreConfirmText.replace("{0}", centre.name);
                ;
            };
            CentreController.prototype.getErrorMsgByErrId = function (errId, msg) {
                return this.locErrorMsgText;
            };
            CentreController.prototype.getDbGridId = function () {
                return this.dbGridId;
            };
            //*****************************************************************************
            //******************************************************************************
            //overriden methods
            CentreController.prototype.addNewRow = function () {
                _super.prototype.addNewRow.call(this);
                this.formatCentreGridLookups();
            };
            CentreController.prototype.gridEditRow = function (rowEntity) {
                _super.prototype.gridEditRow.call(this, rowEntity);
                this.formatCentreGridLookups();
            };
            //******************************************************************************
            //***********************************************************************************
            //Http methods
            CentreController.prototype.getCentresData = function () {
                var _this = this;
                if (this.isValueNullOrUndefined(this.$http)) {
                    return;
                }
                this.showLoader(this.isError);
                this.$http.get(this.getRegetRootUrl() + "/Centre/GetCentresAdminData?filter=" + encodeURI(this.filterUrl) +
                    "&pageSize=" + this.pageSize +
                    "&currentPage=" + this.currentPage +
                    "&sort=" + this.sortColumnsUrl +
                    "&t=" + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        _this.gridOptions.data = tmpData.db_data;
                        _this.rowsCount = tmpData.rows_count;
                        _this.setGridColumFilters();
                        //if (!this.isValueNullOrUndefined(this.gridApi)
                        //    && !this.isValueNullOrUndefined(this.gridApi.core)
                        //    && !this.isValueNullOrUndefined(this.uiGridConstants)) {
                        //    this.gridApi.core.notifyDataChange(this.uiGridConstants.dataChange.COLUMN);
                        //}
                        _this.isCentresLoaded = true;
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
            CentreController.prototype.getCompanies = function () {
                var _this = this;
                if (this.companies !== null) {
                    this.isCompanyLoaded = true;
                    this.hideLoaderWrapper();
                    return this.companies;
                }
                this.showLoader(this.isError);
                this.$http.get(this.getRegetRootUrl() + "Participant/GetParticipantCompanies?t=" + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        _this.companies = tmpData;
                        //company
                        var pos = _this.getColumnIndex("company_name");
                        if (pos > 0) {
                            _this.gridOptions.columnDefs[pos].filter = {
                                type: _this.uiGridConstants.filter.SELECT,
                                selectOptions: _this.companies
                            };
                            _this.gridOptions.columnDefs[pos].editDropdownOptionsArray = _this.companies;
                            _this.gridApi.core.notifyDataChange(_this.uiGridConstants.dataChange.COLUMN);
                        }
                        //this.getCentresData();
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
                return this.companies;
            };
            //***********************************************************************************
            //*************************************************************************************
            //Methods
            CentreController.prototype.setGrid = function () {
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
            };
            CentreController.prototype.loadData = function () {
                this.companies = this.getCompanies();
            };
            CentreController.prototype.getLocDuplicityCentreNameText = function () {
                return this.locDuplicityCentreNameText;
            };
            CentreController.prototype.hideLoaderWrapper = function () {
                if (this.isError ||
                    (this.isCentresLoaded
                        && this.isCompanyLoaded)) {
                    this.hideLoader();
                }
            };
            CentreController.prototype.setGridColumFilters = function () {
                this.setGridColumFilter("export_price_text", this.exportPrice);
                this.setGridColumFilter("multi_orderer", this.yesNo);
                this.setGridColumFilter("other_orderer_can_takeover", this.yesNo);
                this.setGridColumFilter("is_approved_by_requestor", this.yesNo);
                this.setGridColumFilter("active", this.yesNo);
            };
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
            CentreController.prototype.userTextChanged = function (isSeleted) {
                var txtUser = document.getElementById("intGridUserAutocomplete");
                if (!isSeleted && txtUser.value === this.locManagerText) {
                    txtUser.style.color = "#888";
                }
                else {
                    txtUser.style.color = "#000";
                }
            };
            CentreController.prototype.addressTextChanged = function (isSeleted) {
                var txtAddress = document.getElementById("intGridAddressAutocomplete");
                if (!isSeleted && txtAddress.value === this.locAddressText) {
                    txtAddress.style.color = "#888";
                }
                else {
                    txtAddress.style.color = "#000";
                }
            };
            CentreController.prototype.searchParticipant = function (strName) {
                return this.filterParticipants(strName);
            };
            CentreController.prototype.searchAddress = function (strName, companyName) {
                return this.filterAddresses(strName, companyName);
            };
            //Formats user lookup textbox in Centres grid, must be here
            CentreController.prototype.formatCentreGridLookups = function () {
                var _this = this;
                this.userInterval = setInterval(function () {
                    _this.formatGridLookup("gridFieldAutoUser", "intGridUserAutocomplete", "manager_name", _this.userInterval);
                }, 100);
                this.addressInterval = setInterval(function () {
                    _this.formatGridLookup("gridFieldAutoAddress", "intGridAddressAutocomplete", "address_text", _this.addressInterval);
                }, 100);
            };
            CentreController.prototype.centreManSelectedItemChange = function (participant) {
                if (this.isValueNullOrUndefined(participant)) {
                    this.editRow["manager_name"] = null;
                    this.editRow["manager_id"] = null;
                }
                else {
                    if (!this.isValueNullOrUndefined(participant.id)) {
                        //******************** !!!!!!!!!!!!! **************************************************
                        //it is called 2 time, seccond tim it is skipped because item contains name, not object
                        this.editRow["manager_name"] = participant.surname + " " + participant.first_name;
                        this.editRow["manager_id"] = participant.id;
                    }
                }
                this.userTextChanged(true);
            };
            CentreController.prototype.addressSelectedItemChange = function (address) {
                if (this.isValueNullOrUndefined(address)) {
                    this.editRow["address_text"] = null;
                    this.editRow["address_id"] = null;
                }
                else {
                    if (!this.isValueNullOrUndefined(address.id)) {
                        //******************** !!!!!!!!!!!!! **************************************************
                        //it is called 2 time, seccond tim it is skipped because item contains name, not object
                        this.editRow["address_id"] = address.id;
                        this.editRow["address_text"] = address.address_text;
                    }
                }
                this.addressTextChanged(true);
            };
            CentreController.prototype.dropDownChanged = function (item) {
                //var company: AgDropDown[] = this.$filter("filter")(this.companies, { company_name: item }, true);
                //var centre: CentreAdminExtended = this.editRow;
                //centre.
                var fieldName = item.$parent.col.field;
                if (fieldName == "company_name") {
                    var centre = this.editRow;
                    //centre.company_name = item;
                    centre.address_text = null;
                }
            };
            return CentreController;
        }(RegetApp.BaseRegetGridTs));
        RegetApp.CentreController = CentreController;
        angular.
            module("RegetApp").
            controller("CentreController", Kamsyk.RegetApp.CentreController);
        var CentreAdminExtended = /** @class */ (function () {
            function CentreAdminExtended() {
                this.id = null;
                this.name = null;
                this.company_name = null;
                this.manager_id = null;
                this.manager_name = null;
                this.export_price_text = null;
                this.address_text = null;
                this.row_index = null;
                this.multi_orderer = false;
                this.other_orderer_can_takeover = false;
                this.is_approved_by_requestor = false;
                this.active = false;
            }
            return CentreAdminExtended;
        }());
        RegetApp.CentreAdminExtended = CentreAdminExtended;
    })(RegetApp = Kamsyk.RegetApp || (Kamsyk.RegetApp = {}));
})(Kamsyk || (Kamsyk = {}));
//# sourceMappingURL=centre.js.map