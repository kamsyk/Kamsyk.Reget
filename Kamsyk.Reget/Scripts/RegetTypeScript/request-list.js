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
/// <reference path="../typings/moment/moment.d.ts" />
var Kamsyk;
(function (Kamsyk) {
    var RegetApp;
    (function (RegetApp) {
        var RequestListController = /** @class */ (function (_super) {
            __extends(RequestListController, _super);
            //****************************************************************
            //**********************************************************
            //Constructor
            function RequestListController($scope, $http, $filter, $mdDialog, $mdToast, uiGridConstants, $q, $timeout) {
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
                _this.dbGridId = "grdRequestList_rg";
                //*****************************************
                //***********************************************************************
                //Properties
                _this.requestData = null;
                //************************************************************************
                //*****************************************************************
                //Localized Texts
                _this.locRequestNrText = $("#RequestNrText").val();
                _this.locRequestorText = $("#RequestorText").val();
                _this.locCentreText = $("#CentreText").val();
                _this.locPurchaseGroupText = $("#PurchaseGroupText").val();
                _this.locCreatedText = $("#CreatedText").val();
                //***************************************************************
                _this.$onInit = function () { };
                _this.deleteUrl = null;
                _this.setGrid();
                _this.loadInit();
                return _this;
            }
            //********************************************************************
            //Abstract Methods
            RequestListController.prototype.isRowChanged = function () {
                return false;
            };
            RequestListController.prototype.insertRow = function () { };
            RequestListController.prototype.exportToXlsUrl = function () {
                return this.getRegetRootUrl() + 'Report/GetRequestListReport?filter=' + encodeURI(this.filterUrl) +
                    '&sort=' + this.sortColumnsUrl +
                    '&t=' + new Date().getTime();
            };
            RequestListController.prototype.isRowEntityValid = function (rowEntity) {
                return null;
            };
            RequestListController.prototype.getSaveRowUrl = function () {
                return null;
            };
            RequestListController.prototype.getDuplicityErrMsg = function (rowEntity) {
                return null;
            };
            RequestListController.prototype.getControlColumnsCount = function () {
                return 2;
            };
            RequestListController.prototype.getMsgDisabled = function (userSubstitutionEntity) {
                return null;
            };
            RequestListController.prototype.getMsgDeleteConfirm = function (userSubstitutionEntity) {
                return null;
            };
            RequestListController.prototype.loadGridData = function () {
                this.getRequestListData();
            };
            RequestListController.prototype.getErrorMsgByErrId = function (errId, msg) {
                return null;
            };
            RequestListController.prototype.getDbGridId = function () {
                return this.dbGridId;
            };
            //************************************************************************
            //******************** Http Methods ****************************************
            RequestListController.prototype.getRequestListData = function () {
                var _this = this;
                this.showLoaderBoxOnly(this.isError);
                if (this.currentPage < 1) {
                    this.currentPage = 1;
                }
                // this.initDataLoadParams();
                ////let substData: UserSubstitution[] = null;
                //let strCurrPage: string = sessionStorage.getItem("currentPage");
                //if (!this.isStringValueNullOrEmpty(strCurrPage)) {
                //    this.currentPage = parseInt(strCurrPage);
                //}
                //sessionStorage.clear();
                this.$http.get(this.getRegetRootUrl() + "Request/GetRequestList?" +
                    "filter=" + encodeURI(this.filterUrl) +
                    "&pageSize=" + this.pageSize +
                    "&pageNr=" + this.currentPage +
                    "&sort=" + this.sortColumnsUrl +
                    "&t=" + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpRes = response;
                        // this.formatGridDate(tmpRes.data.user_substitution.db_data);
                        var tmpData = response.data;
                        _this.formatGridDates(tmpData.db_data);
                        _this.requestData = tmpData.db_data;
                        for (var i = 0; i < _this.requestData.length; i++) {
                            var smRequest = new RegetApp.RequestSm();
                            smRequest.request_nr = _this.requestData[i].request_nr;
                            smRequest.requestor_name_surname_first = _this.requestData[i].requestor_name_surname_first;
                            _this.requestData[i].request_sm = smRequest;
                        }
                        _this.gridOptions.data = _this.requestData;
                        _this.rowsCount = tmpData.rows_count;
                        if (_this.rowsCount == 0) {
                            _this.currentPage = 0;
                        }
                        if (!_this.isValueNullOrUndefined(_this.gridApi)) {
                            _this.gridApi.core.notifyDataChange(_this.uiGridConstants.dataChange.COLUMN);
                        }
                        _this.setGridSettingData();
                        //********************************************************************
                        //it is very important otherwise 50 lines are not diplayed properly !!!
                        _this.gridOptions.virtualizationThreshold = _this.rowsCount + 1;
                        //********************************************************************
                        //this.isSubstLoaed = true;
                        _this.testLoadDataCount++;
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
            //**************************************************************************
            //*************************** Methods ************************************
            RequestListController.prototype.loadInit = function () {
                this.getLoadDataGridSettings();
                //this.loadGridData();
            };
            RequestListController.prototype.setGrid = function () {
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
                        //headerCellClass: (grid, row, col, rowRenderIndex, colRenderIndex) => {
                        //    return this.hiddenXs();
                        //},
                        //cellClass: (grid, row, col, rowRenderIndex, colRenderIndex) => {
                        //    return this.hiddenXs();
                        //},
                        cellTemplate: '<div class="ui-grid-cell-contents ui-grid-top-panel" style="text-align:center;vertical-align:middle;font-weight:normal;">{{COL_FIELD}}</div>'
                    },
                    {
                        name: 'action_buttons_detail',
                        displayName: '',
                        enableFiltering: false,
                        enableSorting: false,
                        enableCellEdit: false,
                        enableHiding: false,
                        enableColumnResizing: false,
                        width: 35,
                        //headerCellClass: (grid, row, col, rowRenderIndex, colRenderIndex) => {
                        //    return this.hiddenXs();
                        //},
                        //cellClass: (grid, row, col, rowRenderIndex, colRenderIndex) => {
                        //    return this.hiddenXs();
                        //},
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellDetail.html"
                    },
                    {
                        name: "request_nr", displayName: this.locRequestNrText, field: "request_nr",
                        enableHiding: false,
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellLinkImgTemplate.html",
                        enableCellEdit: false,
                        width: 140,
                        minWidth: 120,
                    },
                    {
                        name: "requestor_name_surname_first", displayName: this.locRequestorText, field: "requestor_name_surname_first",
                        enableHiding: false,
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextTemplateReadOnly.html",
                        enableCellEdit: false,
                        filter: {
                            condition: function (searchTerm, cellValue) {
                                return true;
                            }
                        },
                        width: 160,
                        minWidth: 100,
                    },
                    {
                        name: "centre_name", displayName: this.locCentreText, field: "centre_name",
                        enableHiding: false,
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextTemplateReadOnly.html",
                        enableCellEdit: false,
                        width: 100,
                        minWidth: 100,
                    },
                    {
                        name: "pg_name", displayName: this.locPurchaseGroupText, field: "pg_name",
                        enableHiding: false,
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextTemplateReadOnly.html",
                        enableCellEdit: false,
                        width: 180,
                        minWidth: 100,
                    },
                    {
                        name: "issued", displayName: this.locCreatedText, field: "issued",
                        enableHiding: false,
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellDateReadOnlyTemplate.html",
                        enableCellEdit: false,
                        width: 100,
                        minWidth: 100,
                    },
                ];
            };
            RequestListController.prototype.gridRowDetail = function (rowEntity) {
                var urlParAddSubstVisible = "newSubstDisplayed=0";
                if ($("#divAddSubstContent").is(':visible')) {
                    urlParAddSubstVisible = "newSubstDisplayed=1";
                }
                var url = this.getRegetRootUrl() + "Request?id=" + rowEntity.id;
                window.open(url, '_blank');
                //window.location.href = url;
            };
            RequestListController.prototype.formatGridDates = function (gridData) {
                if (this.isValueNullOrUndefined(gridData)) {
                    return;
                }
                for (var i = 0; i < gridData.length; i++) {
                    var jsonDateFrom = gridData[i].issued;
                    var jDateFrom = this.convertJsonDate(jsonDateFrom);
                    gridData[i].issued = jDateFrom;
                    //let jsonDateTo = gridData[i].substitute_end_date;
                    //let jDateTo = this.convertJsonDate(jsonDateTo);
                    //gridData[i].substitute_end_date = jDateTo;
                    //let jsonModifyDate = gridData[i].modified_date;
                    //let jModifiedDate = this.convertJsonDate(jsonModifyDate);
                    //gridData[i].modified_date = jModifiedDate;
                }
            };
            return RequestListController;
        }(RegetApp.BaseRegetGridTs));
        RegetApp.RequestListController = RequestListController;
        angular.
            module('RegetApp').
            controller('RequestListController', Kamsyk.RegetApp.RequestListController).
            config(function ($mdDateLocaleProvider) {
            this.SetDatePicker($mdDateLocaleProvider);
            //it is neccessary to set IsGenerateDatePickerLocalization = true
        });
    })(RegetApp = Kamsyk.RegetApp || (Kamsyk.RegetApp = {}));
})(Kamsyk || (Kamsyk = {}));
//# sourceMappingURL=request-list.js.map