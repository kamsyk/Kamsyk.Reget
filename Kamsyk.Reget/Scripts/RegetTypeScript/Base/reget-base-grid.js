/// <reference path="../Base/reget-base.ts" />
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
        var app = angular.module('RegetApp', ['ngMaterial', 'ngMessages', 'ui.grid', 'ui.grid.pagination', 'ui.grid.resizeColumns', 'ui.grid.selection', 'ui.grid.moveColumns', 'ui.grid.edit', 'ui.grid.autoResize', 'angularjs-dropdown-multiselect']);
        var BaseRegetGridTs = /** @class */ (function (_super) {
            __extends(BaseRegetGridTs, _super);
            //******************************************************************
            //*****************************************************************
            //Constructors
            function BaseRegetGridTs($scope, $http, $filter, $mdDialog, $mdToast, $q, uiGridConstants, $timeout
            //protected $moment: moment.Moment
            ) {
                var _this = _super.call(this, $scope, $http, $filter, $mdDialog, $mdToast, $q, $timeout) || this;
                _this.$scope = $scope;
                _this.$http = $http;
                _this.$filter = $filter;
                _this.$mdDialog = $mdDialog;
                _this.$mdToast = $mdToast;
                _this.$q = $q;
                _this.uiGridConstants = uiGridConstants;
                _this.$timeout = $timeout;
                _this.pageSize = 10;
                _this.pageSizeGrid = 10 + 1; //because of new row
                _this.currentPage = 1;
                _this.sortColumnsUrl = "";
                _this.filterUrl = "";
                _this.gridOptions = {};
                _this.gridApi = null;
                _this.rowsCount = 0;
                _this.lang = "en";
                _this.editRow = null;
                _this.editRowOrig = null;
                _this.editRowIndex = -1;
                _this.newRowIndex = -1;
                _this.isNewRow = false;
                //public dbGridId: string = null;
                _this.userGridSettings = null;
                _this.isGridSettingsSaved = false;
                _this.defaultColumnDefs = null;
                _this.isGridSettingsSet = false;
                //private isFilterIsLoaded: boolean = false;
                _this.isColumnHidden = false;
                _this.isFilterApplied = false;
                _this.hiddenColNames = ["id"];
                _this.testLoadDataCount = 0;
                _this.isInitDataSet = false;
                //private isUrlInit: boolean = false;
                _this.columnsWhichCanBeHidden = null;
                _this.columnsWhichCanBeHiddenModel = [];
                _this.isColumnDisplayHideBtnVisible = false;
                _this.hideColumnsIndexes = [];
                //private isDisplHideColEnabled: boolean = false;
                _this._urlParamDelimiter = "|";
                _this._urlParamValueDelimiter = "~";
                //******************************************************************
                //Localization
                _this.rowMissingIdFieldText = $("#RowMissingIdFieldText").val();
                //abstract getColumsCanBeHidden: string[];
                //abstract getGridData(): any[];
                //abstract dbGridId: string = null;
                _this.deleteUrl = null;
                //********************************************************************
                //********************************************************************
                //Loc Texts
                _this.locMissingMandatoryText = $("#MissingMandatoryText").val();
                _this.locDateTimePickerFormatText = $("#DateTimePickerFormatText").val();
                _this.locConfirmationText = $("#ConfirmationText").val();
                //public locYesText: string = $("#YesText").val();
                //public locNoText: string = $("#NoText").val();
                _this.locTranslationText = $("#TranslationText").val();
                _this.locCancelText = $("#CancelText").val();
                _this.locEditText = $("#EditText").val();
                _this.locDeleteText = $("#DeleteText").val();
                _this.locSaveText = $("#SaveText").val();
                _this.lang = $("#GridLangCode").val();
                _this.setGridOptionBase();
                _this.defaultColumnDefs = angular.copy(_this.gridOptions.columnDefs);
                $scope.dropDownMultiSelectStyle = {
                    //scrollableHeight: '200px',
                    scrollable: true,
                    //enableSearch: true,
                    //smartButtonMaxItems: 2,
                    dynamicTitle: false,
                    buttonClasses: 'btn btn-grid-multiselect'
                };
                $scope.onMultiSelect = (function () {
                    //if (this.isDisplHideColEnabled === true) {
                    _this.showHideColumns();
                    //}
                });
                return _this;
                //window.addEventListener("resize", () => {
                //    //if (window.matchMedia("(min-width: 767px)").matches) {
                //    //    //console.log("Screen width is at least 600px")
                //    //} else {
                //    //    //console.log("Screen less than 600px")
                //    //}
                //    //console.log("Screen less than 600px");
                //    this.resizeGridHeight();
                //});
                //$scope.onItemDeselect = (() => {
                //    console.log('deselecting : ' + this.columnsWhichCannBeHiddenModel);
                //});
            }
            //********************************************************************
            //******************************************************************
            //Methods
            BaseRegetGridTs.prototype.setGridOptionBase = function () {
                var _this = this;
                this.gridOptions = {
                    //data: this.m_substData,
                    enableFiltering: true,
                    enableRowSelection: true,
                    enablePagination: true,
                    paginationPageSizes: [10, 20, 50],
                    paginationPageSize: this.pageSizeGrid,
                    enableHorizontalScrollbar: 1,
                    enableVerticalScrollbar: 1,
                    enablePaginationControls: false
                };
                this.gridOptions.appScopeProvider = this;
                this.gridOptions.onRegisterApi = function (gridApi) {
                    _this.gridApi = gridApi;
                    //sort
                    _this.gridApi.core.on.sortChanged(_this.$scope, function (grid, sortColumns) {
                        if (_this.isSkipLoad) {
                            return;
                        }
                        if (!_this.gridSaveRow()) {
                            return;
                        }
                        //if (this.isValueNullOrUndefined(this.currentPage) || this.currentPage < 1) {
                        //    this.currentPage = 1;
                        //}
                        var strSort = _this.sortColumnsUrl;
                        _this.sortColumnsUrl = "";
                        if (sortColumns !== null) {
                            for (var i = 0; i < sortColumns.length; i++) {
                                var name = sortColumns[i].name;
                                var direction = sortColumns[i].sort.direction;
                                if (_this.sortColumnsUrl.length > 0) {
                                    _this.sortColumnsUrl += _this._urlParamDelimiter;
                                }
                                _this.sortColumnsUrl += name + _this._urlParamValueDelimiter + direction;
                            }
                            if (strSort !== _this.sortColumnsUrl) {
                                _this.loadGridData();
                            }
                        }
                    });
                    //filter
                    _this.gridApi.core.on.filterChanged(_this.$scope, _this.debounce(function () {
                        _this.filterChanged();
                    }, 500));
                    _this.gridApi.core.on.columnVisibilityChanged(_this.$scope, function (col) {
                        if (col.visible == false) {
                            _this.isColumnHidden = true;
                            _this.updateColumnsCanBeHidden(col.name);
                        }
                    });
                    //this.gridApi.core.on.filterChanged(this.$scope, this.debounce(this.filterChanged, 500));
                    // this.gridApi.core.on.columnVisibilityChanged(this.$scope, this.columnVisibilityChanged);
                };
            };
            BaseRegetGridTs.prototype.updateColumnsCanBeHidden = function (colHidden) {
                if (this.isValueNullOrUndefined(this.columnsWhichCanBeHiddenModel)) {
                    return;
                }
                var colCanBeHidden = this.$filter("filter")(this.columnsWhichCanBeHiddenModel, { id: colHidden }, true);
                if (this.isValueNullOrUndefined(colCanBeHidden)) {
                    return;
                }
                if (colCanBeHidden.length == 0) {
                    return;
                }
                for (var i = 0; i < this.columnsWhichCanBeHiddenModel.length; i++) {
                    if (this.columnsWhichCanBeHiddenModel[i].id === colHidden) {
                        this.columnsWhichCanBeHiddenModel.splice(i, 1);
                        break;
                    }
                }
            };
            BaseRegetGridTs.prototype.filterChanged = function () {
                //console.log("participant load debounce");
                if (this.isSkipLoad) {
                    return;
                }
                if (!this.gridSaveRow()) {
                    return;
                }
                // this.currentPage = 1;
                var strFilter = this.filterUrl;
                this.filterUrl = this.getFilterUrl();
                this.isFilterApplied = !this.isValueNullOrUndefined(this.filterUrl);
                if (strFilter !== this.filterUrl) {
                    this.currentPage = 1;
                    this.loadGridData();
                }
            };
            //public columnVisibilityChanged(column: any): void {
            //}
            BaseRegetGridTs.prototype.getFilterUrl = function () {
                var _this = this;
                if (this.isValueNullOrUndefined(this.gridApi)) {
                    return "";
                }
                //Filter
                var filter = "";
                angular.forEach(this.gridApi.grid.columns, function (col) {
                    if (col.enableFiltering
                        && !_this.isValueNullOrUndefined(col.filters)
                        && col.filters.length > 0
                        && col.filters[0].term !== null && col.filters[0].term !== undefined
                        && (col.filters[0].term !== "")) {
                        if (filter.length > 0) {
                            filter += _this.urlParamDelimiter;
                        }
                        filter += col.name + _this.urlParamValueDelimiter + col.filters[0].term;
                    }
                    if (col.enableFiltering
                        && !_this.isValueNullOrUndefined(col.filters)
                        && col.filters.length > 1
                        && col.filters[1].term !== null && col.filters[1].term !== undefined
                        && (col.filters[1].term !== "")) {
                        if (filter.length > 0) {
                            filter += _this.urlParamDelimiter;
                        }
                        filter += col.name + _this.urlParamValueDelimiter + col.filters[1].term;
                    }
                });
                return filter;
            };
            BaseRegetGridTs.prototype.getLastPageIndex = function () {
                //if (this.rowsCount == 0) {
                //    //it is needed because of url PeGeNumber does not work anyhow
                //    return;
                //}
                if (this.rowsCount < (this.pageSize * (this.currentPage - 1) + 1)) {
                    if (this.currentPage > 1) {
                        this.currentPage--;
                    }
                }
                var iLastPageIndex = Math.ceil(this.rowsCount / this.pageSize);
                return iLastPageIndex;
            };
            BaseRegetGridTs.prototype.nextPage = function () {
                if (!this.gridSaveRow()) {
                    return;
                }
                if (this.isLastPage()) {
                    return;
                }
                else {
                    this.currentPage++;
                }
                this.loadGridData();
            };
            BaseRegetGridTs.prototype.previousPage = function () {
                if (!this.gridSaveRow()) {
                    return;
                }
                if (this.isFirstPage()) {
                    return;
                }
                else {
                    this.currentPage--;
                }
                this.loadGridData();
            };
            BaseRegetGridTs.prototype.isLastPage = function () {
                var tmpCurrentPage = this.currentPage + 1;
                if ((tmpCurrentPage * this.pageSize) >= (this.rowsCount + this.pageSize)) {
                    return true;
                }
                return false;
            };
            BaseRegetGridTs.prototype.isFirstPage = function () {
                var tmpCurrentPage = this.currentPage - 1;
                if (tmpCurrentPage < 1) {
                    return true;
                }
                return false;
            };
            BaseRegetGridTs.prototype.gotoPage = function () {
                if (!this.gridSaveRow()) {
                    return;
                }
                if (this.isValueNullOrUndefined(this.currentPage)) {
                    return;
                }
                if (this.currentPage < 1) {
                    this.currentPage = 1;
                }
                if (this.currentPage > this.getLastPageIndex()) {
                    this.currentPage = this.getLastPageIndex();
                }
                this.loadGridData();
            };
            BaseRegetGridTs.prototype.firstPage = function () {
                if (!this.gridSaveRow()) {
                    return;
                }
                this.currentPage = 1;
                this.loadGridData();
            };
            BaseRegetGridTs.prototype.lastPage = function () {
                if (!this.gridSaveRow()) {
                    return;
                }
                this.currentPage = this.getLastPageIndex();
                this.loadGridData();
            };
            ;
            BaseRegetGridTs.prototype.getDisplayItemsToInfo = function () {
                var iToInfo = (this.currentPage) * this.pageSize;
                if (iToInfo > this.rowsCount) {
                    iToInfo = this.rowsCount;
                }
                return iToInfo;
            };
            BaseRegetGridTs.prototype.getDisplayItemsFromInfo = function () {
                if (this.currentPage == 0) {
                    return 0;
                }
                return (this.currentPage - 1) * this.pageSize + 1;
            };
            BaseRegetGridTs.prototype.refresh = function () {
                this.loadGridData();
                this.editRow = null;
                this.editRowOrig = null;
                this.editRowIndex = null;
                this.newRowIndex = null;
                this.isNewRow = false;
            };
            BaseRegetGridTs.prototype.cellClicked = function (row, col) {
                //alert("clicked");
                if (!this.isValueNullOrUndefined(this.getRowIdFiledName()) && row.entity[this.getRowIdFiledName()] < -1) {
                    return;
                }
                this.gridEditRow(row.entity);
            };
            BaseRegetGridTs.prototype.getRowIdFiledName = function () {
                return "id";
            };
            BaseRegetGridTs.prototype.gridEditRow = function (rowEntity) {
                var tmpData = this.gridOptions.data;
                this.editRowIndex = tmpData.indexOf(rowEntity); //syka ts
                if (this.isValueNullOrUndefined(this.editRow)) {
                    this.editRowChanged(false);
                }
                else {
                    this.gridSaveRow();
                }
            };
            BaseRegetGridTs.prototype.editRowChanged = function (isNewRow) {
                this.editRow = this.cancelEditRow(this.editRow);
                if (!this.isValueNullOrUndefined(this.newRowIndex) && this.newRowIndex !== -1 && !isNewRow) {
                    this.insertRow();
                    this.editRowIndex = this.newRowIndex;
                }
                if (!this.isValueNullOrUndefined(this.editRowIndex) && this.editRowIndex !== -1) {
                    this.editRowOrig = angular.copy(this.gridOptions.data[this.editRowIndex]);
                    this.gridOptions.data[this.editRowIndex].editrow = true;
                    this.editRow = this.gridOptions.data[this.editRowIndex];
                    this.editRowIndex = -1;
                }
            };
            BaseRegetGridTs.prototype.cancelEditRow = function (editRow) {
                if (!this.isValueNullOrUndefined(editRow)) {
                    editRow.editrow = false;
                    editRow = null;
                }
                return editRow;
            };
            BaseRegetGridTs.prototype.gridSaveRow = function () {
                try {
                    if (this.isValueNullOrUndefined(this.editRow)) {
                        return true;
                    }
                    if (this.isValueNullOrUndefined(this.getRowIdFiledName())) {
                        this.displayErrorMsg(this.rowMissingIdFieldText);
                        return false;
                    }
                    var isChanged = false;
                    var id = this.editRow[this.getRowIdFiledName()];
                    if (this.isValueNullOrUndefined(id)) {
                        this.displayErrorMsg(this.rowMissingIdFieldText);
                        return false;
                    }
                    var rowEntity = this.$filter("filter")(this.gridOptions.data, { id: id }, true);
                    if (rowEntity[0][this.getRowIdFiledName()] < -1) {
                        isChanged = true;
                        this.newRowIndex = null;
                    }
                    if (!isChanged) {
                        isChanged = this.isRowChanged();
                    }
                    if (isChanged) {
                        return this.saveGridRowtoDb(rowEntity[0]);
                    }
                    else {
                        this.editRowChanged(false);
                    }
                    this.restoreFilter();
                    return true;
                }
                catch (ex) {
                    throw ex;
                }
                finally {
                    this.isSkipLoad = false;
                }
            };
            BaseRegetGridTs.prototype.gridCancelEdit = function () {
                var _this = this;
                this.showLoaderBoxOnly();
                try {
                    var isNewRow = (!this.isValueNullOrUndefined(this.editRow));
                    if (!this.isValueNullOrUndefined(this.getRowIdFiledName())) {
                        isNewRow = isNewRow && this.editRow[this.getRowIdFiledName()] < -1;
                    }
                    if (isNewRow) {
                        this.restoreFilter();
                    }
                    this.newRowIndex = null;
                    if (this.editRow !== null) {
                        this.editRow.editrow = false;
                        if (isNewRow) {
                            var data = this.gridOptions.data;
                            var index = data.indexOf(this.editRow); //syka ts
                            data.splice(index, 1);
                            this.gridOptions.paginationPageSize--;
                        }
                        else {
                            angular.forEach(this.editRow, function (value, key) {
                                _this.editRow[key] = _this.editRowOrig[key];
                            });
                        }
                        this.editRow = null;
                        this.editRowIndex = null;
                        if (isNewRow) {
                            this.enableFiltering();
                        }
                    }
                }
                catch (ex) {
                    throw ex;
                }
                finally {
                    this.isSkipLoad = false;
                    this.hideLoader();
                }
            };
            BaseRegetGridTs.prototype.restoreFilter = function () {
                var _this = this;
                if (!this.isValueNullOrUndefined(this.gridFilter)) {
                    for (var i = 0; i < this.gridFilter.length; i++) {
                        angular.forEach(this.gridApi.grid.columns, function (col) {
                            if (col.name === _this.gridFilter[i].column_name) {
                                col.filters[0].term = _this.gridFilter[i].filter_value;
                                _this.isFilterApplied = true;
                            }
                        });
                        //for (var j: number = 0; j < this.gridOptions.columnDefs.length; j++) {
                        //    if (this.gridOptions.columnDefs[j].name === this.gridFilter[i].column_name) {
                        //        this.gridOptions.columnDefs[j].filters[0].term = this.gridFilter[i].filter_value;
                        //    }
                        //}
                    }
                }
            };
            BaseRegetGridTs.prototype.saveGridRowtoDb = function (rowEntity) {
                var _this = this;
                var strErrMsg = this.isRowEntityValid(rowEntity);
                if (!this.isStringValueNullOrEmpty(strErrMsg)) {
                    this.displayErrorMsg(strErrMsg);
                    return;
                }
                this.isError = false;
                this.showLoaderBoxOnly(this.isError);
                var jsonEntityData = JSON.stringify(rowEntity);
                this.$http.post(this.getSaveRowUrl(), jsonEntityData).then(function (response) {
                    try {
                        var result = response.data;
                        var iId = result.int_value;
                        //if (!this.isValueNullOrUndefined(response.data)) {
                        var errMsg = null;
                        var splitIndex = result.string_value.indexOf(",");
                        if (result.string_value.indexOf(",") > -1) {
                            var errType = result.string_value.substring(0, splitIndex);
                            if (errType === "DUPLICITY") {
                                var duplicityPpgName = result.string_value.substring(splitIndex + 1).trim();
                                errMsg = _this.getDuplicityErrMsg(rowEntity);
                                _this.displayErrorMsg(errMsg);
                                return;
                            }
                        }
                        else if (result.string_value === "DUPLICITY") {
                            _this.hideLoader();
                            errMsg = _this.getDuplicityErrMsg(rowEntity);
                            if (_this.isValueNullOrUndefined(errMsg)) {
                                errMsg = "Duplicity values";
                            }
                            _this.displayErrorMsg(errMsg);
                            return;
                        }
                        if (!_this.isValueNullOrUndefined(result) && !_this.isValueNullOrUndefined(result.error_id)) {
                            if (result.error_id != 0) {
                                var errMsg_1 = _this.getErrorMsgByErrId(result.error_id, result.string_value);
                                var errMsgDialog = errMsg_1;
                                var errLinkDialog = null;
                                if (!_this.isStringValueNullOrEmpty(errMsg_1)) {
                                    if (errMsg_1.indexOf(_this.urlParamDelimiter) > 0) {
                                        var strItems = errMsg_1.split(_this.urlParamDelimiter);
                                        errMsgDialog = strItems[0];
                                        errLinkDialog = strItems[1];
                                    }
                                }
                                _this.displayErrorMsg(errMsgDialog, errLinkDialog);
                                return;
                            }
                        }
                        //} else
                        if (iId === -1) {
                            _this.hideLoader();
                            _this.displayErrorMsg();
                            return;
                        }
                        var isNew = false;
                        if (rowEntity.id < 0) {
                            isNew = true;
                            rowEntity.id = iId;
                            _this.rowsCount++;
                        }
                        _this.editRowChanged(false);
                        _this.newRowIndex = null;
                        if (isNew) {
                            _this.enableFiltering();
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
                    return false;
                });
                return true;
            };
            BaseRegetGridTs.prototype.setDataGridSettings = function () {
                var _this = this;
                this.showLoaderBoxOnly();
                var jsonGridData = JSON.stringify(this.userGridSettings);
                this.$http.post(this.getRegetRootUrl() + 'DataGrid/SetUserGridSettings?t=' + new Date().getTime(), jsonGridData).then(function (response) {
                    try {
                        try {
                            _this.isGridSettingsSaved = true;
                            _this.hideLoader();
                        }
                        catch (ex) {
                            _this.hideLoader();
                            _this.displayErrorMsg();
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
            BaseRegetGridTs.prototype.resetGridSettings = function () {
                var _this = this;
                this.showLoaderBoxOnly();
                var jsonUserGridData = JSON.stringify(this.userGridSettings);
                this.$http.post(this.getRegetRootUrl() + 'DataGrid/DeleteUserGridSettings?t=' + new Date().getTime(), jsonUserGridData).then(function (response) {
                    try {
                        var result = response.data;
                        if (_this.isGridSettingsSaved === true) {
                            //workaround - if the grid is loaded then columns order is changed and save and then Reset is called the columns order is nor reset - page reload is needed.
                            //if the reset is called wo saving before it works fine
                            var isColumnOrderModified = _this.isColumnOrderChanged(_this.userGridSettings.columns, _this.gridOptions.columnDefs);
                            if (isColumnOrderModified) {
                                window.location.reload(true);
                                return;
                            }
                        }
                        _this.filterUrl = "";
                        _this.sortColumnsUrl = "";
                        _this.pageSize = 10;
                        _this.pageSizeGrid = _this.pageSize + 1;
                        _this.gridOptions.paginationPageSize = _this.pageSizeGrid;
                        _this.isSkipLoad = true;
                        //this.gridOptions.columnDefs = angular.copy(this.defaultColumnDefs);
                        //this.restoreColumns(this.defaultColumnDefs);
                        _this.clearFilters();
                        _this.clearSort();
                        _this.isSkipLoad = false;
                        _this.userGridSettings = null;
                        _this.showAllColumns();
                        _this.loadGridData();
                    }
                    catch (ex) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        //this.hideLoader();
                    }
                }, function (response) {
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
            };
            //public getDataGridSettingsByParams(pageSize: number, strFilter: string, strSort: string, currentPage : number): void {
            //    try {
            //        if (!this.isValueNullOrUndefined(pageSize)) {
            //            this.pageSize = pageSize;
            //            this.pageSizeGrid = this.pageSize + 1;
            //            this.gridOptions.paginationPageSize = this.pageSizeGrid;
            //        }
            //        if (!this.isStringValueNullOrEmpty(strFilter)) {
            //            this.gridFilter = this.getGridFilterFromUrlString(strFilter);
            //            this.restoreFilter();
            //            this.filterUrl = this.getFilterUrl();
            //        }
            //        if (!this.isStringValueNullOrEmpty(strSort)) {
            //            this.sortColumnsUrl = strSort;
            //            this.getGridSortFromUrlString(strSort);
            //            this.restoreSort();
            //        }
            //        var grdColumns: uiGrid.IColumnDefOf<any>[] = this.getGridColumnsFromUrlString(this.userGridSettings.columns);
            //        this.restoreColumns(grdColumns);
            //        this.currentPage = currentPage;
            //        this.loadGridData();
            //    } catch (e) {
            //        this.resetGridSettings();
            //        this.hideLoader();
            //        //this.displayErrorMsg();
            //    } finally {
            //        this.hideLoader();
            //    }
            //}
            //*******************************************************************************************************
            // !!!! Use it only for a special Data Grid Load - e.g. supplier, purchase groups
            //*******************************************************************************************************
            BaseRegetGridTs.prototype.getDataGridSettings = function () {
                var _this = this;
                this.showLoader();
                this.$http.get(this.getRegetRootUrl() + 'DataGrid/GetUserGridSettings?gridId=' + this.getDbGridId() + '&t=' + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpUserGridSettings = response.data;
                        _this.userGridSettings = tmpUserGridSettings;
                        if (!_this.isValueNullOrUndefined(_this.userGridSettings)) {
                            _this.pageSize = _this.userGridSettings.grid_page_size;
                            _this.pageSizeGrid = _this.pageSize + 1;
                            _this.gridOptions.paginationPageSize = _this.pageSizeGrid;
                            _this.gridFilter = _this.getGridFilterFromUrlString(_this.userGridSettings.filter);
                            _this.restoreFilter();
                            _this.filterUrl = _this.getFilterUrl();
                            _this.sortColumnsUrl = _this.userGridSettings.sort;
                            _this.getGridSortFromUrlString(_this.userGridSettings.sort);
                            _this.restoreSort();
                            var grdColumns = _this.getGridColumnsFromUrlString(_this.userGridSettings.columns);
                            _this.restoreColumns(grdColumns);
                        }
                        _this.loadGridData();
                    }
                    catch (e) {
                        _this.resetGridSettings();
                        _this.hideLoader();
                        //this.displayErrorMsg();
                    }
                    finally {
                        _this.hideLoader();
                    }
                }, function (response) {
                    _this.resetGridSettings();
                    _this.hideLoader();
                    //this.displayErrorMsg();
                });
            };
            BaseRegetGridTs.prototype.loadGridSettings = function () {
                //if (this.isUrlInit === true) {
                //    this.initDataLoadParams();
                //    return;
                //}
                //this.initDataLoadParams();
                if (this.userGridSettings === null) {
                    this.defaultColumnDefs = angular.copy(this.gridOptions.columnDefs);
                    this.getDataGridSettings();
                }
            };
            //***************************************************
            // Standart Method to Load grid Data
            //****************************************************
            BaseRegetGridTs.prototype.getLoadDataGridSettings = function () {
                var _this = this;
                this.columnsWhichCanBeHidden = this.getColumsCanBeHidden();
                this.$http.get(this.getRegetRootUrl() + 'DataGrid/GetUserGridSettings?gridId=' + this.getDbGridId() + '&t=' + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpUserGridSettings = response.data;
                        _this.userGridSettings = tmpUserGridSettings;
                        //let isFilterChanged : boolean = false;
                        _this.getUrlParams();
                        if (!_this.isValueNullOrUndefined(_this.userGridSettings)) {
                            _this.pageSize = _this.userGridSettings.grid_page_size;
                            _this.pageSizeGrid = _this.pageSize + 1;
                            _this.gridOptions.paginationPageSize = _this.pageSizeGrid;
                            if (!_this.isValueNullOrUndefined(_this.userGridSettings.page_nr)) {
                                _this.currentPage = _this.userGridSettings.page_nr;
                                if (_this.currentPage < 1) {
                                    _this.currentPage = 1;
                                }
                            }
                            _this.gridFilter = _this.getGridFilterFromUrlString(_this.userGridSettings.filter);
                            _this.filterUrl = _this.userGridSettings.filter;
                            //this.filterUrl = this.getFilterUrl();
                            //if (!this.isStringValueNullOrEmpty(this.userGridSettings.filter)) {
                            //    isFilterChanged = true;
                            //}
                            _this.sortColumnsUrl = _this.userGridSettings.sort;
                            _this.getGridSortFromUrlString(_this.userGridSettings.sort);
                        }
                        //if (!isFilterChanged) {
                        //if filter is changed the loadGridData is launched from this.gridApi.core.on.filterChanged
                        _this.loadGridData();
                        //}
                    }
                    catch (e) {
                        _this.resetGridSettings();
                        _this.hideLoader();
                    }
                    finally {
                        _this.hideLoader();
                    }
                }, function (response) {
                    _this.resetGridSettings();
                    _this.hideLoader();
                });
            };
            BaseRegetGridTs.prototype.setGridSettingData = function () {
                if (this.isGridSettingsSet === true) {
                    return;
                }
                this.isGridSettingsSet = true;
                this.defaultColumnDefs = angular.copy(this.gridOptions.columnDefs);
                if (!this.isValueNullOrUndefined(this.userGridSettings)) {
                    this.isSkipLoad = true;
                    this.restoreFilter();
                    this.restoreSort();
                    if (!this.isValueNullOrUndefined(this.userGridSettings.page_nr)) {
                        this.currentPage = this.userGridSettings.page_nr;
                    }
                    var grdColumns = this.getGridColumnsFromUrlString(this.userGridSettings.columns);
                    this.restoreColumns(grdColumns);
                    this.isSkipLoad = false;
                }
                else {
                    this.setColumnsWhichCanBeHiddenModel();
                }
            };
            BaseRegetGridTs.prototype.addNewRow = function () {
                if (!this.isValueNullOrUndefined(this.newRowIndex)
                    && this.newRowIndex > -1) {
                    //new row is edited already, new row cannot be added
                    return;
                }
                if (this.editRow === null) {
                    this.newRowIndex = this.getNewRowIndex();
                    this.editRowIndex = this.newRowIndex;
                    this.insertRow();
                    this.editRowChanged(true);
                }
                else {
                    this.newRowIndex = this.getNewRowIndex();
                    this.editRowIndex = this.newRowIndex;
                    this.gridSaveRow();
                }
                //this.FormatGridUserLookup();
            };
            BaseRegetGridTs.prototype.getNewRowIndex = function () {
                var newRowIndex = this.pageSize;
                var grid = this.gridApi.grid;
                var gridRowsCount = grid.rows.length;
                if (gridRowsCount < this.pageSize || gridRowsCount > this.pageSize) {
                    newRowIndex = gridRowsCount;
                }
                if (newRowIndex < 0) {
                    newRowIndex = 0;
                }
                return newRowIndex;
            };
            BaseRegetGridTs.prototype.enableFiltering = function () {
                this.isSkipLoad = true;
                for (var i = this.getControlColumnsCount(); i < this.gridOptions.columnDefs.length; i++) {
                    this.gridOptions.columnDefs[i].enableFiltering = true;
                    this.gridApi.core.notifyDataChange(this.uiGridConstants.dataChange.COLUMN);
                }
                this.isSkipLoad = false;
            };
            BaseRegetGridTs.prototype.pageSizeChanged = function () {
                if (!this.gridSaveRow()) {
                    return;
                }
                this.currentPage = 1;
                this.pageSizeGrid = this.pageSize + 1;
                this.gridOptions.paginationPageSize = this.pageSizeGrid;
                this.loadGridData();
            };
            ;
            BaseRegetGridTs.prototype.exportToXls = function () {
                try {
                    window.open(this.exportToXlsUrl());
                }
                catch (e) {
                    this.displayErrorMsg();
                }
                finally {
                    this.hideLoader();
                }
            };
            BaseRegetGridTs.prototype.dateTimeChanged = function (row_entity, col) {
                //try {
                //    if (!this.isValueNullOrUndefined(dateTimeWa)) {
                //        row_entity[col.field] = dateTimeWa;
                //    }
                //} catch (e) {
                //    this.displayErrorMsg();
                //} finally {
                //    dateTimeWa = null;
                //}
            };
            //public getMissinMandatoryText(): string {
            //    return $("#MissingMandatoryText").val();
            //}
            //public getDateTimePickerFormatText(): string {
            //    return $("#DateTimePickerFormatText").val();
            //}
            BaseRegetGridTs.prototype.hideEditGridButton = function (rowEntity) {
                return false;
            };
            BaseRegetGridTs.prototype.hideDeleteGridButton = function (rowEntity) {
                return false;
            };
            BaseRegetGridTs.prototype.getColumnIndex = function (strColumnName) {
                if (this.isValueNullOrUndefined(this.gridApi)) {
                    return -1;
                }
                for (var i = 0; i < this.gridApi.grid.columns.length; i++) {
                    if (this.gridApi.grid.columns[i].name === strColumnName) {
                        return i;
                    }
                }
                return -1;
            };
            //*******************************************
            //Save Grid Setting
            BaseRegetGridTs.prototype.saveGridSettings = function () {
                this.setGridSettings();
                this.setDataGridSettings();
            };
            BaseRegetGridTs.prototype.setGridSettings = function () {
                this.userGridSettings = new UserGridSettings();
                //Grid Name
                this.userGridSettings.grid_name = this.getDbGridId();
                //Page Size
                this.userGridSettings.grid_page_size = this.pageSize;
                //Filter
                var filter = this.getFilterUrl();
                this.userGridSettings.filter = filter;
                //Sort
                var sort = this.getSortUrl();
                this.userGridSettings.sort = sort;
                //Columns
                var columns = this.getColumnstUrl();
                this.userGridSettings.columns = columns;
            };
            BaseRegetGridTs.prototype.getColumnstUrl = function () {
                var _this = this;
                //columns
                var columns = '';
                angular.forEach(this.gridApi.grid.columns, function (col) {
                    if (columns.length > 0) {
                        columns += _this.urlParamDelimiter;
                    }
                    columns += col.name + _this.urlParamValueDelimiter + col.visible;
                });
                return columns;
            };
            BaseRegetGridTs.prototype.getSortUrl = function () {
                var _this = this;
                //sort
                var sort = '';
                angular.forEach(this.gridApi.grid.columns, function (col) {
                    if (!_this.isValueNullOrUndefined(col.sort) && !_this.isValueNullOrUndefined(col.sort.direction)) {
                        if (sort.length > 0) {
                            sort += _this.urlParamDelimiter;
                        }
                        sort += col.name + _this.urlParamValueDelimiter + col.sort.direction;
                    }
                });
                return sort;
            };
            BaseRegetGridTs.prototype.isColumnOrderChanged = function (currentColumnsString, origColumns) {
                var currentColumns = this.getGridColumnsFromUrlString(currentColumnsString);
                if (this.isValueNullOrUndefined(currentColumns) || this.isValueNullOrUndefined(origColumns)) {
                    return true;
                }
                if (currentColumns.length !== origColumns.length) {
                    return true;
                }
                for (var i = 0; i < currentColumns.length; i++) {
                    if (currentColumns[i].name !== origColumns[i].name) {
                        return true;
                    }
                }
                return false;
            };
            BaseRegetGridTs.prototype.getGridColumnsFromUrlString = function (strUrlColumns) {
                var gridColumns = [];
                if (!this.isStringValueNullOrEmpty(strUrlColumns)) {
                    var columnsItems = strUrlColumns.split(this.urlParamDelimiter);
                    for (var i = 0; i < columnsItems.length; i++) {
                        var itemFields = columnsItems[i].split(this.urlParamValueDelimiter);
                        var oVal = itemFields[1];
                        var isVal = (oVal === 'true') ? true : false;
                        var tmpCol = {};
                        var tmpGridCol = tmpCol;
                        tmpGridCol.name = itemFields[0];
                        tmpGridCol.visible = isVal;
                        gridColumns.push(tmpGridCol);
                    }
                }
                return gridColumns;
            };
            BaseRegetGridTs.prototype.clearFilters = function () {
                var _this = this;
                this.gridFilter = [];
                angular.forEach(this.gridApi.grid.columns, function (col) {
                    if (col.enableFiltering && (!_this.isValueNullOrUndefined(col.filters[0].term))) {
                        var tmpFilter = {};
                        var tmpFilterGrid = tmpFilter;
                        tmpFilterGrid.column_name = col.name;
                        tmpFilterGrid.filter_value = col.filters[0].term;
                        _this.gridFilter.push(tmpFilterGrid);
                        col.filters[0].term = '';
                    }
                });
                this.isFilterApplied = false;
            };
            BaseRegetGridTs.prototype.clearFiltersReload = function () {
                this.clearFilters();
                this.loadGridData();
            };
            BaseRegetGridTs.prototype.showAllColumns = function () {
                this.gridOptions.columnDefs = angular.copy(this.defaultColumnDefs);
                this.setColumnsWhichCanBeHiddenModel();
                //this.userGridSettings = null;
                this.isColumnHidden = false;
            };
            BaseRegetGridTs.prototype.showAllColumnsReload = function () {
                this.showAllColumns();
                this.loadGridData();
            };
            BaseRegetGridTs.prototype.clearSort = function () {
                //var isFilter: boolean = false;
                angular.forEach(this.gridApi.grid.columns, function (col) {
                    col.unsort();
                    //col.sortCellFiltered = false;
                });
                //return isFilter;
            };
            BaseRegetGridTs.prototype.getGridFilterFromUrlString = function (strUrlFilter) {
                var gridFilter = [];
                if (!this.isStringValueNullOrEmpty(strUrlFilter)) {
                    var filterItems = strUrlFilter.split(this.urlParamDelimiter);
                    for (var i = 0; i < filterItems.length; i++) {
                        var itemFields = filterItems[i].split(this.urlParamValueDelimiter);
                        var oVal = itemFields[1];
                        if (oVal === 'true') {
                            oVal = true;
                        }
                        else if (oVal === 'false') {
                            oVal = false;
                        }
                        gridFilter.push({
                            column_name: itemFields[0],
                            filter_value: oVal
                        });
                    }
                }
                return gridFilter;
            };
            BaseRegetGridTs.prototype.getGridSortFromUrlString = function (strUrlSort) {
                this.gridSort = [];
                if (!this.isStringValueNullOrEmpty(strUrlSort)) {
                    var sortItems = strUrlSort.split(this.urlParamDelimiter);
                    for (var i = 0; i < sortItems.length; i++) {
                        var itemFields = sortItems[i].split(this.urlParamValueDelimiter);
                        this.gridSort.push({
                            column_name: itemFields[0],
                            direction: itemFields[1]
                        });
                    }
                }
            };
            BaseRegetGridTs.prototype.restoreSort = function () {
                var _this = this;
                if (!this.isValueNullOrUndefined(this.gridSort)) {
                    for (var i = 0; i < this.gridSort.length; i++) {
                        angular.forEach(this.gridApi.grid.columns, function (col) {
                            if (col.name === _this.gridSort[i].column_name) {
                                col.sort.direction = _this.gridSort[i].direction;
                            }
                        });
                    }
                }
            };
            BaseRegetGridTs.prototype.restoreColumns = function (gridColumns) {
                if (this.isValueNullOrUndefined(gridColumns) || gridColumns.length == 0) {
                    return;
                }
                for (var i = 0; i < gridColumns.length; i++) {
                    if (gridColumns[i].name === "id") {
                        continue;
                    }
                    if (gridColumns[i].visible === false) {
                        //let hidCol: string[] = this.$filter("filter")(this.hiddenColNames, { gridColumns[i].name }, true);
                        var isPermHiddenCol = false;
                        if (!this.isValueNullOrUndefined(this.hiddenColNames)) {
                            for (var j_1 = 0; j_1 < this.hiddenColNames.length; j_1++) {
                                //if (this.hiddenColNames[j].toLowerCase().trim().indexOf("action_buttons") > -1) {
                                //    continue;
                                //}
                                if (this.hiddenColNames[j_1].toLowerCase().trim() == gridColumns[i].name.toLowerCase().trim()) {
                                    isPermHiddenCol = true;
                                    break;
                                }
                            }
                        }
                        if (!isPermHiddenCol) {
                            this.isColumnHidden = true;
                        }
                        for (var j = this.gridOptions.columnDefs.length - 1; j >= 0; j--) {
                            if (this.gridOptions.columnDefs[j].name === "id") {
                                continue;
                            }
                            if (this.gridOptions.columnDefs[j].enableHiding === false) {
                                continue;
                            }
                            if (this.gridOptions.columnDefs[j].name === gridColumns[i].name) {
                                this.gridOptions.columnDefs.splice(j, 1);
                                break;
                            }
                        }
                    }
                }
                for (var j = this.gridOptions.columnDefs.length - 1; j >= 0; j--) {
                    if (this.gridOptions.columnDefs[j].name === "id") {
                        continue;
                    }
                    var isFound = false;
                    for (var i = 0; i < gridColumns.length; i++) {
                        if (this.gridOptions.columnDefs[j].name === gridColumns[i].name) {
                            isFound = true;
                            break;
                        }
                    }
                    if (!isFound) {
                        this.gridOptions.columnDefs.splice(j, 1);
                        this.isColumnHidden = true;
                    }
                }
                this.setColumnsOrder(gridColumns);
                this.setColumnsWhichCanBeHiddenModel();
            };
            BaseRegetGridTs.prototype.setColumnsOrder = function (gridColumns) {
                var orderedColumns = [];
                var isReordered = false;
                var colIndex = 0;
                for (var i = 0; i < gridColumns.length; i++) {
                    if (gridColumns[i].visible === false && gridColumns[i].name !== "id") {
                        continue;
                    }
                    //var col = angular.copy(this.gridOptions.columnDefs[i]);
                    if (this.gridOptions.columnDefs.length > colIndex) {
                        if (this.gridOptions.columnDefs[colIndex].name === gridColumns[i].name) {
                            orderedColumns.push(this.gridOptions.columnDefs[colIndex]);
                        }
                        else {
                            for (var j = 0; j < this.gridOptions.columnDefs.length; j++) {
                                if (this.gridOptions.columnDefs[j].name === gridColumns[i].name) {
                                    orderedColumns.push(this.gridOptions.columnDefs[j]);
                                    isReordered = true;
                                    break;
                                }
                            }
                        }
                    }
                    else {
                        for (var j = 0; j < this.gridOptions.columnDefs.length; j++) {
                            if (this.gridOptions.columnDefs[j].name === gridColumns[i].name) {
                                orderedColumns.push(this.gridOptions.columnDefs[j]);
                                isReordered = true;
                                break;
                            }
                        }
                    }
                    colIndex++;
                }
                if (isReordered) {
                    this.gridOptions.columnDefs = orderedColumns;
                }
            };
            BaseRegetGridTs.prototype.setColumnsWhichCanBeHiddenModel = function () {
                //this.isDisplHideColEnabled = false;
                //this.isColumnDisplayHideBtnVisible = false;
                try {
                    this.columnsWhichCanBeHiddenModel = [];
                    for (var i = 0; i < this.gridOptions.columnDefs.length; i++) {
                        if (this.isValueNullOrUndefined(this.gridOptions.columnDefs[i].enableHiding)
                            || this.gridOptions.columnDefs[i].enableHiding === true) {
                            this.columnsWhichCanBeHiddenModel.push({
                                id: this.gridOptions.columnDefs[i].name,
                                label: this.gridOptions.columnDefs[i].displayName
                            });
                            //this.isColumnDisplayHideBtnVisible = true;
                        }
                    }
                }
                catch (e) {
                    this.displayErrorMsg();
                }
                finally {
                    //this.isDisplHideColEnabled = true;
                }
            };
            BaseRegetGridTs.prototype.insertBaseRow = function (newEntity) {
                //clear filters
                this.isSkipLoad = true;
                this.clearFilters();
                if (this.isValueNullOrUndefined(this.rowsCount)) {
                    this.rowsCount = 0;
                }
                if (this.isValueNullOrUndefined(this.gridOptions.data)) {
                    this.gridOptions.data = [];
                }
                var grdData = this.gridOptions.data;
                var rowIndex = this.getNewRowIndexColValue();
                newEntity.row_index = rowIndex;
                grdData.push(newEntity);
                this.gridOptions.paginationPageSize++;
            };
            BaseRegetGridTs.prototype.getNewRowIndexColValue = function () {
                var rowIndex = (this.currentPage - 1) * this.pageSize + this.gridOptions.paginationPageSize;
                if (rowIndex > this.rowsCount + 1) {
                    rowIndex = this.rowsCount + 1;
                }
                return rowIndex;
            };
            BaseRegetGridTs.prototype.removeRowFromArray = function (entity) {
                var tmpCurrPage = this.currentPage;
                var tmpData = this.gridOptions.data;
                //Remove row from array
                var index = tmpData.indexOf(entity); //syka ts
                var gridData = this.gridOptions.data;
                gridData.splice(index, 1);
                if (this.rowsCount > 1) {
                    this.rowsCount--;
                }
                //check whether it was last record
                if (this.getLastPageIndex() < tmpCurrPage && this.currentPage > 0) {
                    this.previousPage();
                }
            };
            BaseRegetGridTs.prototype.toggleGridCheckbox = function (item, col) {
                if (item[col.field]) {
                    item[col.field] = false;
                }
                else {
                    item[col.field] = true;
                }
            };
            BaseRegetGridTs.prototype.sortNullString = function (a, b, rowA, rowB, direction) {
                a = (this.isStringValueNullOrEmpty(a)) ? ' ' : a;
                b = (this.isStringValueNullOrEmpty(b)) ? ' ' : b;
                if (a === b) {
                    return 0;
                }
                ;
                if (a < b) {
                    return -1;
                }
                ;
                if (a > b) {
                    return 1;
                }
                ;
            };
            BaseRegetGridTs.prototype.gridDeleteRowFromDb = function (entity, ev) {
                // var deferred: ng.IDeferred<boolean> = this.$q.defer<boolean>();
                var _this = this;
                this.showLoaderBoxOnly(this.isError);
                var jsonData = JSON.stringify(entity);
                this.$http.post(this.deleteUrl, 
                //this.getRegetRootUrl() + 'Supplier/DeleteSupplier' + '?t=' + new Date().getTime(),
                jsonData).then(function (response) {
                    try {
                        _this.gridRowWasDeleted(response, entity);
                        //var tmpData: any = response.data;
                        //var strResult = tmpData.string_value;
                        //if (this.isStringValueNullOrEmpty(strResult)) {
                        //    this.removeRowFromArray(entity);
                        //} else if (strResult === "disabled") {
                        //    entity.active = false;
                        //    //var msgDisabled = this.locSupplierWasDisabledText.replace("{0}", supplier.supp_name);
                        //    this.showAlert(this.locConfirmationText, this.getMsgDisabled(entity), this.locCloseText);
                        //}
                        //if (!this.isValueNullOrUndefined(tmpData) && !this.isValueNullOrUndefined(tmpData.error_id)) {
                        //    if (tmpData.error_id != 0) {
                        //        let errMsg: string = this.getErrorMsgByErrId(tmpData.error_id, tmpData.string_value);
                        //        let errMsgDialog = errMsg;
                        //        let errLinkDialog = null;
                        //        if (!this.isStringValueNullOrEmpty(errMsg)) {
                        //            if (errMsg.indexOf(this.urlParamDelimiter) > 0) {
                        //                let strItems = errMsg.split(this.urlParamDelimiter);
                        //                errMsgDialog = strItems[0];
                        //                errLinkDialog = strItems[1];
                        //            }
                        //        }
                        //        this.displayErrorMsg(errMsgDialog, errLinkDialog);
                        //        return;
                        //    }
                        //} 
                        //this.hideLoader();
                        ////return true;
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
                //return deferred.promise;
            };
            BaseRegetGridTs.prototype.gridRowWasDeleted = function (response, entity) {
                //let isOk: boolean = false;
                var tmpData = response.data;
                var strResult = tmpData.string_value;
                if (this.isStringValueNullOrEmpty(strResult)) {
                    this.removeRowFromArray(entity);
                }
                else if (strResult === "disabled") {
                    entity.active = false;
                    this.showAlert(this.locConfirmationText, this.getMsgDisabled(entity), this.locCloseText);
                }
                if (!this.isValueNullOrUndefined(tmpData) && !this.isValueNullOrUndefined(tmpData.error_id)) {
                    if (tmpData.error_id != 0) {
                        var errMsg = this.getErrorMsgByErrId(tmpData.error_id, tmpData.string_value);
                        var errMsgDialog = errMsg;
                        var errLinkDialog = null;
                        if (!this.isStringValueNullOrEmpty(errMsg)) {
                            if (errMsg.indexOf(this.urlParamDelimiter) > 0) {
                                var strItems = errMsg.split(this.urlParamDelimiter);
                                errMsgDialog = strItems[0];
                                errLinkDialog = strItems[1];
                            }
                        }
                        this.displayErrorMsg(errMsgDialog, errLinkDialog);
                        return false;
                    }
                }
                this.hideLoader();
                return true;
            };
            BaseRegetGridTs.prototype.gridDeleteRow = function (entity, ev) {
                this.$mdDialog.show({
                    template: this.getConfirmDialogTemplate(this.getMsgDeleteConfirm(entity), this.locConfirmationText, this.locYesText, this.locNoText, "confirmDialog()", "closeDialog()"),
                    locals: {
                        regetBaseGrid: this,
                        entity: entity,
                        ev: ev
                    },
                    controller: this.dialogConfirmController
                });
                ////var strMessage = this.locDeleteSupplierConfirmText.replace("{0}", supplier.supp_name);
                //var confirm = this.$mdDialog.confirm()
                //    .title(this.locConfirmationText)
                //    .textContent(this.getMsgDeleteConfirm(entity))
                //    .ariaLabel("DeleteRowConfirm")
                //    .targetEvent(ev)
                //    .ok(this.locNoText)
                //    .cancel(this.locYesText);
                //this.$mdDialog.show(confirm).then(() => {
                //}, () => {
                //    this.gridDeleteRowFromDb(entity, ev);
                //});
            };
            BaseRegetGridTs.prototype.dialogConfirmController = function ($scope, $mdDialog, regetBaseGrid, entity, ev) {
                $scope.closeDialog = function () {
                    $mdDialog.hide();
                };
                $scope.confirmDialog = function () {
                    $mdDialog.hide();
                    regetBaseGrid.gridDeleteRowFromDb(entity, ev);
                };
            };
            //Formats user lookup textbox in Centres grid, must be here
            BaseRegetGridTs.prototype.formatGridLookup = function (autoId, autoInputId, fieldName, interval) {
                try {
                    //this.userInterval = setInterval(() => {
                    var gridUser = document.getElementById(autoId);
                    if (!this.isValueNullOrUndefined(gridUser)) {
                        var gridUserWrap = gridUser.children[0];
                        var gridUserInput = document.getElementById(autoInputId);
                        gridUserInput.style.paddingLeft = "5px";
                        gridUserInput.style.height = "30px";
                        gridUserInput.style.lineHeight = "30px";
                        if (this.isStringValueNullOrEmpty(this.editRow[fieldName])) {
                            gridUserInput.style.color = "#888";
                        }
                        else {
                            gridUserInput.style.color = "#000";
                        }
                        clearInterval(interval);
                        //console.log(autoId);
                    }
                    //}, 100);
                }
                catch (e) { }
            };
            BaseRegetGridTs.prototype.dropDownChanged = function (item) {
            };
            BaseRegetGridTs.prototype.setGridColumFilter = function (columnName, filterValues) {
                var pos = this.getColumnIndex(columnName);
                if (pos < 0) {
                    return false;
                }
                if (pos > 0 && !this.isValueNullOrUndefined(this.gridOptions.columnDefs)) {
                    this.gridOptions.columnDefs[pos].filter = {
                        //term: '1',
                        type: this.uiGridConstants.filter.SELECT,
                        selectOptions: filterValues
                    };
                    this.gridOptions.columnDefs[pos].editDropdownOptionsArray = filterValues;
                    this.gridApi.core.notifyDataChange(this.uiGridConstants.dataChange.COLUMN);
                }
                return true;
            };
            BaseRegetGridTs.prototype.getRowDisabledBkg = function (activeItem) {
                if (this.isValueNullOrUndefined(activeItem) || activeItem === false) {
                    return 'grid-row-disabled';
                }
                return null;
            };
            BaseRegetGridTs.prototype.insertDataGridColum = function (gridColumn, iEditColIndex) {
                if (this.isValueNullOrUndefined(gridColumn) || iEditColIndex < 0) {
                    return;
                }
                var bkgGridColumns = angular.copy(this.gridOptions.columnDefs);
                this.gridOptions.columnDefs = [];
                if (iEditColIndex > (bkgGridColumns.length - 1)) {
                    iEditColIndex = (bkgGridColumns.length - 1);
                }
                for (var j = 0; j < bkgGridColumns.length; j++) {
                    if (iEditColIndex == j) {
                        this.gridOptions.columnDefs.push(gridColumn);
                    }
                    this.gridOptions.columnDefs.push(bkgGridColumns[j]);
                }
            };
            BaseRegetGridTs.prototype.getUrlParams = function () {
                var strFilter = null;
                var strSort = null;
                var pageSize = null;
                var currentPage = null;
                var isUrlFilter = false;
                if (this.isInitDataSet === false) {
                    this.isInitDataSet = true;
                    var urlParams = this.getAllUrlParams();
                    if (!this.isValueNullOrUndefined(urlParams)) {
                        for (var i = 0; i < urlParams.length; i++) {
                            if (urlParams[i].param_name.toLowerCase() === "pagesize") {
                                pageSize = parseInt(urlParams[i].param_value);
                                isUrlFilter = true;
                            }
                            else if (urlParams[i].param_name.toLowerCase() === "currpage") {
                                currentPage = parseInt(urlParams[i].param_value);
                                isUrlFilter = true;
                            }
                            else if (urlParams[i].param_name.toLowerCase() === "filter") {
                                strFilter = this.decodeUrl(urlParams[i].param_value);
                                isUrlFilter = true;
                            }
                            else if (urlParams[i].param_name.toLowerCase() === "sort") {
                                strSort = this.decodeUrl(urlParams[i].param_value);
                                isUrlFilter = true;
                            }
                        }
                    }
                    if (isUrlFilter === true) {
                        if (this.isValueNullOrUndefined(this.userGridSettings)) {
                            this.userGridSettings = {
                                grid_name: this.getDbGridId(),
                                grid_page_size: pageSize,
                                filter: strFilter,
                                sort: strSort,
                                columns: null,
                                page_nr: currentPage
                            };
                        }
                        else {
                            this.userGridSettings.grid_page_size = pageSize;
                            this.userGridSettings.filter = strFilter;
                            this.userGridSettings.sort = strSort;
                            this.userGridSettings.page_nr = currentPage;
                        }
                        //this.getDataGridSettingsByParams(pageSize, strFilter, strSort, currentPage);
                    }
                }
            };
            BaseRegetGridTs.prototype.getColumsCanBeHidden = function () {
                this.isColumnDisplayHideBtnVisible = false;
                if (this.isValueNullOrUndefined(this.gridOptions.columnDefs) || this.gridOptions.columnDefs.length == 0) {
                    return null;
                }
                var hidCoumns = [];
                for (var i = 0; i < this.gridOptions.columnDefs.length; i++) {
                    if (this.isValueNullOrUndefined(this.gridOptions.columnDefs[i].enableHiding) || this.gridOptions.columnDefs[i].enableHiding === true) {
                        if (!this.isStringValueNullOrEmpty(this.gridOptions.columnDefs[i].displayName)) {
                            hidCoumns.push({ id: this.gridOptions.columnDefs[i].name, label: this.gridOptions.columnDefs[i].displayName });
                        }
                    }
                }
                if (hidCoumns.length > 0) {
                    this.isColumnDisplayHideBtnVisible = true;
                }
                return hidCoumns;
            };
            BaseRegetGridTs.prototype.setColumnsCanBeHidden = function () {
                this.columnsWhichCanBeHidden = this.getColumsCanBeHidden();
            };
            BaseRegetGridTs.prototype.showHideColumns = function () {
                try {
                    if (this.isValueNullOrUndefined(this.defaultColumnDefs)) {
                        return;
                    }
                    if (this.isValueNullOrUndefined(this.gridOptions.columnDefs)) {
                        return;
                    }
                    if (this.isValueNullOrUndefined(this.columnsWhichCanBeHiddenModel) || this.columnsWhichCanBeHiddenModel.length == 0) {
                        //hide all hideable columns
                        for (var i = 0; i < this.gridOptions.columnDefs.length; i++) {
                            if (this.gridOptions.columnDefs[i].enableHiding === true) {
                                this.gridOptions.columnDefs.splice(i, 1);
                            }
                        }
                        return;
                    }
                    //hide solumns
                    for (var i = 0; i < this.gridOptions.columnDefs.length; i++) {
                        if (this.gridOptions.columnDefs[i].enableHiding === true) {
                            var displayCol = this.$filter("filter")(this.columnsWhichCanBeHiddenModel, { id: this.gridOptions.columnDefs[i].name }, true);
                            if (this.isValueNullOrUndefined(displayCol) || displayCol.length == 0) {
                                this.addHideColPosition(this.gridOptions.columnDefs[i].name, i);
                                this.gridOptions.columnDefs.splice(i, 1);
                            }
                        }
                    }
                    //display columns
                    for (var i = 0; i < this.columnsWhichCanBeHiddenModel.length; i++) {
                        var displayCol = this.$filter("filter")(this.gridOptions.columnDefs, { name: this.columnsWhichCanBeHiddenModel[i].id }, true);
                        if (this.isValueNullOrUndefined(displayCol) || displayCol.length == 0) {
                            var displayColDef = this.$filter("filter")(this.defaultColumnDefs, { name: this.columnsWhichCanBeHiddenModel[i].id }, true);
                            var iPos = this.getHideColPosition(this.columnsWhichCanBeHiddenModel[i].id);
                            if (iPos === -1) {
                                this.gridOptions.columnDefs.push(displayColDef[0]);
                            }
                            else {
                                this.gridOptions.columnDefs.splice(iPos, 0, displayColDef[0]);
                            }
                        }
                    }
                    //this.defaultColumnDefs
                    //this.columnsWhichCannBeHiddenModel
                    /*if (this.isValueNullOrUndefined(this.defaultColumnDefs)) {
                        return;
                    }
        
                    if (this.isValueNullOrUndefined(this.columnsWhichCanBeHiddenModel)) {
                        return;
                    }
        
                    for (let i: number = this.gridOptions.columnDefs.length - 1; i >= 0; i--) {
                        if (this.gridOptions.columnDefs[i].name === "id") {
                            continue;
                        }
                        
                        let visibleCol = this.$filter("filter")(this.columnsWhichCanBeHiddenModel, { name: this.defaultColumnDefs[i].name }, true);
                        if (this.isValueNullOrUndefined(visibleCol) || visibleCol.length === 0) {
                            this.gridOptions.columnDefs.splice(i, 1);
                        }
                    }*/
                    //for (let i: number = 0; i < this.defaultColumnDefs.length; i++) {
                    //    if (this.defaultColumnDefs[i].enableHiding === true) {
                    //        let visibleCol = this.$filter("filter")(this.columnsWhichCannBeHiddenModel, { name: this.defaultColumnDefs[i].name }, true);
                    //        if (this.isValueNullOrUndefined(visibleCol) || visibleCol.length === 0) {
                    //        }
                    //    }
                    //}
                }
                catch (e) {
                    this.displayErrorMsg();
                }
            };
            BaseRegetGridTs.prototype.addHideColPosition = function (columnName, position) {
                var hideColPos = this.$filter("filter")(this.hideColumnsIndexes, { name: columnName }, true);
                if (!this.isValueNullOrUndefined(hideColPos) && hideColPos.length > 0) {
                    for (var i = 0; i < this.hideColumnsIndexes.length; i++) {
                        if (this.hideColumnsIndexes[i].name === columnName) {
                            this.hideColumnsIndexes.splice(i);
                            break;
                        }
                    }
                }
                this.hideColumnsIndexes.push({ name: columnName, position: position });
            };
            BaseRegetGridTs.prototype.getHideColPosition = function (columnName) {
                var hideColPos = this.$filter("filter")(this.hideColumnsIndexes, { name: columnName }, true);
                if (!this.isValueNullOrUndefined(hideColPos) && hideColPos.length > 0) {
                    return hideColPos[0].position;
                }
                return -1;
            };
            BaseRegetGridTs.prototype.validateMultiEmailsGrid = function (row, col) {
                var cellTxtId = "grdMail" + row.uid + col.name;
                var mailCell = document.getElementById(cellTxtId);
                if (this.isValueNullOrUndefined(mailCell)) {
                    return false;
                }
                var isMailValid = this.isValidMultiEmails(mailCell.value);
                if (isMailValid === true) {
                    //mailCell.style.setProperty("background-color", "yellow", "important");
                    mailCell.style.border = "";
                    return true;
                }
                else {
                    mailCell.style.border = "3px #ff690f  solid";
                    //mailCell.style.backgroundColor = "red";
                    //mailCell.style.setProperty("background-color", "red", "important");
                    //alert(mailCell.style.backgroundColor);
                    //alert(row.uid);
                    return false;
                }
            };
            return BaseRegetGridTs;
        }(RegetApp.BaseRegetTs));
        RegetApp.BaseRegetGridTs = BaseRegetGridTs;
        var UserGridSettings = /** @class */ (function () {
            function UserGridSettings() {
                this.grid_name = null;
                this.grid_page_size = null;
                this.filter = null;
                this.sort = null;
                this.columns = null;
                this.page_nr = null;
            }
            return UserGridSettings;
        }());
        RegetApp.UserGridSettings = UserGridSettings;
        var HideColIndex = /** @class */ (function () {
            function HideColIndex() {
                this.name = null;
                this.position = null;
            }
            return HideColIndex;
        }());
        RegetApp.HideColIndex = HideColIndex;
    })(RegetApp = Kamsyk.RegetApp || (Kamsyk.RegetApp = {}));
})(Kamsyk || (Kamsyk = {}));
//# sourceMappingURL=reget-base-grid.js.map