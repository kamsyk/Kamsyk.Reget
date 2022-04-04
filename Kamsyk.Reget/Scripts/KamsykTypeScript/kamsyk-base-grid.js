"use strict";
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
Object.defineProperty(exports, "__esModule", { value: true });
var kamsyk_base_1 = require("../KamsykTypeScript/kamsyk-base");
var KamsykBaseGridTs = /** @class */ (function (_super) {
    __extends(KamsykBaseGridTs, _super);
    //******************************************************************
    //*****************************************************************
    //Constructors
    function KamsykBaseGridTs($scope, $http, $filter, $mdDialog, $mdToast, $q, uiGridConstants, $timeout) {
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
        _this.isSkipLoad = false;
        _this._urlParamDelimiter = "|";
        _this._urlParamValueDelimiter = "~";
        //******************************************************************
        //Localization
        _this.rowMissingIdFieldText = $("#RowMissingIdFieldText").val();
        _this.dbGridId = null;
        _this.deleteUrl = null;
        //********************************************************************
        //********************************************************************
        //Loc Texts
        _this.locMissingMandatoryText = $("#MissingMandatoryText").val();
        _this.locDateTimePickerFormatText = $("#DateTimePickerFormatText").val();
        _this.locConfirmationText = $("#ConfirmationText").val();
        _this.locYesText = $("#YesText").val();
        _this.locNoText = $("#NoText").val();
        _this.lang = $("#GridLangCode").val();
        _this.setGridOptionBase();
        _this.defaultColumnDefs = angular.copy(_this.gridOptions.columnDefs);
        return _this;
    }
    //********************************************************************
    //******************************************************************
    //Methods
    KamsykBaseGridTs.prototype.setGridOptionBase = function () {
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
                _this.currentPage = 1;
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
                //console.log("participant load debounce");
                if (_this.isSkipLoad) {
                    return;
                }
                if (!_this.gridSaveRow()) {
                    return;
                }
                _this.currentPage = 1;
                var strFilter = _this.filterUrl;
                _this.filterUrl = _this.getFilterUrl();
                if (strFilter !== _this.filterUrl) {
                    _this.loadGridData();
                }
            }, 500));
        };
    };
    KamsykBaseGridTs.prototype.getFilterUrl = function () {
        var _this = this;
        if (this.isValueNullOrUndefined(this.gridApi)) {
            return '';
        }
        //Filter
        var filter = '';
        angular.forEach(this.gridApi.grid.columns, function (col) {
            if (col.enableFiltering && col.filters[0].term !== null && col.filters[0].term !== undefined
                && (col.filters[0].term !== '')) {
                if (filter.length > 0) {
                    filter += _this.urlParamDelimiter;
                }
                filter += col.name + _this.urlParamValueDelimiter + col.filters[0].term;
            }
        });
        return filter;
    };
    KamsykBaseGridTs.prototype.getLastPageIndex = function () {
        if (this.rowsCount < (this.pageSize * (this.currentPage - 1) + 1)) {
            if (this.currentPage > 1) {
                this.currentPage--;
            }
        }
        var iLastPageIndex = Math.ceil(this.rowsCount / this.pageSize);
        return iLastPageIndex;
    };
    KamsykBaseGridTs.prototype.nextPage = function () {
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
    KamsykBaseGridTs.prototype.previousPage = function () {
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
    KamsykBaseGridTs.prototype.isLastPage = function () {
        var tmpCurrentPage = this.currentPage + 1;
        if ((tmpCurrentPage * this.pageSize) >= (this.rowsCount + this.pageSize)) {
            return true;
        }
        return false;
    };
    KamsykBaseGridTs.prototype.isFirstPage = function () {
        var tmpCurrentPage = this.currentPage - 1;
        if (tmpCurrentPage < 1) {
            return true;
        }
        return false;
    };
    KamsykBaseGridTs.prototype.gotoPage = function () {
        if (!this.gridSaveRow()) {
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
    KamsykBaseGridTs.prototype.firstPage = function () {
        if (!this.gridSaveRow()) {
            return;
        }
        this.currentPage = 1;
        this.loadGridData();
    };
    KamsykBaseGridTs.prototype.lastPage = function () {
        if (!this.gridSaveRow()) {
            return;
        }
        this.currentPage = this.getLastPageIndex();
        this.loadGridData();
    };
    ;
    KamsykBaseGridTs.prototype.getDisplayItemsToInfo = function () {
        var iToInfo = (this.currentPage) * this.pageSize;
        if (iToInfo > this.rowsCount) {
            iToInfo = this.rowsCount;
        }
        return iToInfo;
    };
    KamsykBaseGridTs.prototype.getDisplayItemsFromInfo = function () {
        if (this.currentPage == 0) {
            return 0;
        }
        return (this.currentPage - 1) * this.pageSize + 1;
    };
    KamsykBaseGridTs.prototype.refresh = function () {
        this.loadGridData();
        this.editRow = null;
        this.editRowOrig = null;
        this.editRowIndex = null;
        this.newRowIndex = null;
        this.isNewRow = false;
    };
    KamsykBaseGridTs.prototype.cellClicked = function (row, col) {
        //alert("clicked");
        if (!this.isValueNullOrUndefined(this.getRowIdFiledName()) && row.entity[this.getRowIdFiledName()] < -1) {
            return;
        }
        this.gridEditRow(row.entity);
    };
    KamsykBaseGridTs.prototype.getRowIdFiledName = function () {
        return "id";
    };
    KamsykBaseGridTs.prototype.gridEditRow = function (rowEntity) {
        var tmpData = this.gridOptions.data;
        this.editRowIndex = tmpData.indexOf(rowEntity); //syka ts
        if (this.isValueNullOrUndefined(this.editRow)) {
            this.editRowChanged(false);
        }
        else {
            this.gridSaveRow();
        }
    };
    KamsykBaseGridTs.prototype.editRowChanged = function (isNewRow) {
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
    KamsykBaseGridTs.prototype.cancelEditRow = function (editRow) {
        if (!this.isValueNullOrUndefined(editRow)) {
            editRow.editrow = false;
            editRow = null;
        }
        return editRow;
    };
    KamsykBaseGridTs.prototype.gridSaveRow = function () {
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
    KamsykBaseGridTs.prototype.gridCancelEdit = function () {
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
    KamsykBaseGridTs.prototype.restoreFilter = function () {
        var _this = this;
        if (!this.isValueNullOrUndefined(this.gridFilter)) {
            for (var i = 0; i < this.gridFilter.length; i++) {
                angular.forEach(this.gridApi.grid.columns, function (col) {
                    if (col.name === _this.gridFilter[i].column_name) {
                        col.filters[0].term = _this.gridFilter[i].filter_value;
                    }
                });
            }
        }
    };
    KamsykBaseGridTs.prototype.saveGridRowtoDb = function (rowEntity) {
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
                if (iId === -1 && !_this.isValueNullOrUndefined(response.data) && result.string_value === "DUPLICITY") {
                    _this.hideLoader();
                    var errMsg = _this.getDuplicityErrMsg(rowEntity);
                    if (_this.isValueNullOrUndefined(errMsg)) {
                        errMsg = "Duplicity values";
                    }
                    _this.displayErrorMsg(errMsg);
                    return;
                }
                else if (iId === -1) {
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
    KamsykBaseGridTs.prototype.setDataGridSettings = function () {
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
    KamsykBaseGridTs.prototype.resetGridSettings = function () {
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
                _this.gridOptions.columnDefs = angular.copy(_this.defaultColumnDefs);
                _this.clearFilters();
                _this.clearSort();
                _this.isSkipLoad = false;
                _this.userGridSettings = null;
                _this.loadGridData();
                _this.hideLoader();
            }
            catch (ex) {
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
    KamsykBaseGridTs.prototype.getDataGridSettings = function () {
        var _this = this;
        this.showLoader();
        this.$http.get(this.getRegetRootUrl() + 'DataGrid/GetUserGridSettings?gridId=' + this.dbGridId + '&t=' + new Date().getTime(), {}).then(function (response) {
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
    KamsykBaseGridTs.prototype.addNewRow = function () {
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
    KamsykBaseGridTs.prototype.getNewRowIndex = function () {
        var newRowIndex = this.pageSize;
        var grid = this.gridApi.grid;
        var gridRowsCount = grid.rows.length;
        if (gridRowsCount < this.pageSize) {
            newRowIndex = gridRowsCount;
        }
        if (newRowIndex < 0) {
            newRowIndex = 0;
        }
        return newRowIndex;
    };
    KamsykBaseGridTs.prototype.enableFiltering = function () {
        this.isSkipLoad = true;
        for (var i = this.getControlColumnsCount(); i < this.gridOptions.columnDefs.length; i++) {
            this.gridOptions.columnDefs[i].enableFiltering = true;
            this.gridApi.core.notifyDataChange(this.uiGridConstants.dataChange.COLUMN);
        }
        this.isSkipLoad = false;
    };
    KamsykBaseGridTs.prototype.pageSizeChanged = function () {
        if (!this.gridSaveRow()) {
            return;
        }
        this.currentPage = 1;
        this.pageSizeGrid = this.pageSize + 1;
        this.gridOptions.paginationPageSize = this.pageSizeGrid;
        this.loadGridData();
    };
    ;
    KamsykBaseGridTs.prototype.exportToXls = function () {
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
    KamsykBaseGridTs.prototype.dateTimeChanged = function (row_entity, col) {
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
    KamsykBaseGridTs.prototype.hideEditGridButton = function (rowEntity) {
        return false;
    };
    KamsykBaseGridTs.prototype.hideDeleteGridButton = function (rowEntity) {
        return false;
    };
    KamsykBaseGridTs.prototype.getColumnIndex = function (strColumnName) {
        for (var i = 0; i < this.gridApi.grid.columns.length; i++) {
            if (this.gridApi.grid.columns[i].name === strColumnName) {
                return i;
            }
        }
        return -1;
    };
    //*******************************************
    //Save Grid Setting
    KamsykBaseGridTs.prototype.saveGridSettings = function () {
        this.setGridSettings();
        this.setDataGridSettings();
    };
    KamsykBaseGridTs.prototype.setGridSettings = function () {
        this.userGridSettings = new UserGridSettings();
        //Grid Name
        this.userGridSettings.grid_name = this.dbGridId;
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
    KamsykBaseGridTs.prototype.getColumnstUrl = function () {
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
    KamsykBaseGridTs.prototype.getSortUrl = function () {
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
    KamsykBaseGridTs.prototype.isColumnOrderChanged = function (currentColumnsString, origColumns) {
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
    KamsykBaseGridTs.prototype.getGridColumnsFromUrlString = function (strUrlColumns) {
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
    KamsykBaseGridTs.prototype.clearFilters = function () {
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
    };
    KamsykBaseGridTs.prototype.clearSort = function () {
        //var isFilter: boolean = false;
        angular.forEach(this.gridApi.grid.columns, function (col) {
            col.unsort();
            //col.sortCellFiltered = false;
        });
        //return isFilter;
    };
    KamsykBaseGridTs.prototype.getGridFilterFromUrlString = function (strUrlFilter) {
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
    KamsykBaseGridTs.prototype.getGridSortFromUrlString = function (strUrlSort) {
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
    KamsykBaseGridTs.prototype.restoreSort = function () {
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
    KamsykBaseGridTs.prototype.restoreColumns = function (gridColumns) {
        if (this.isValueNullOrUndefined(gridColumns)) {
            return;
        }
        for (var i = 0; i < gridColumns.length; i++) {
            if (gridColumns[i].visible === false) {
                for (var j = this.gridOptions.columnDefs.length - 1; j >= 0; j--) {
                    if (this.gridOptions.columnDefs[j].name === "id") {
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
            }
        }
        this.setColumnsOrder(gridColumns);
    };
    KamsykBaseGridTs.prototype.setColumnsOrder = function (gridColumns) {
        var orderedColumns = [];
        var isReordered = false;
        var colIndex = 0;
        for (var i = 0; i < gridColumns.length; i++) {
            if (gridColumns[i].visible === false && gridColumns[i].name !== "id") {
                continue;
            }
            var col = angular.copy(this.gridOptions.columnDefs[i]);
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
            colIndex++;
        }
        if (isReordered) {
            this.gridOptions.columnDefs = orderedColumns;
        }
    };
    KamsykBaseGridTs.prototype.insertBaseRow = function (newEntity) {
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
    KamsykBaseGridTs.prototype.getNewRowIndexColValue = function () {
        var rowIndex = (this.currentPage - 1) * this.pageSize + this.gridOptions.paginationPageSize;
        if (rowIndex > this.rowsCount + 1) {
            rowIndex = this.rowsCount + 1;
        }
        return rowIndex;
    };
    KamsykBaseGridTs.prototype.removeRowFromArray = function (entity) {
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
    KamsykBaseGridTs.prototype.toggleGridCheckbox = function (item, col) {
        if (item[col.field]) {
            item[col.field] = false;
        }
        else {
            item[col.field] = true;
        }
    };
    KamsykBaseGridTs.prototype.sortNullString = function (a, b, rowA, rowB, direction) {
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
    KamsykBaseGridTs.prototype.gridDeleteRowFromDb = function (entity, ev) {
        var _this = this;
        this.showLoaderBoxOnly(this.isError);
        var jsonSupplierData = JSON.stringify(entity);
        this.$http.post(this.deleteUrl, 
        //this.getRegetRootUrl() + 'Supplier/DeleteSupplier' + '?t=' + new Date().getTime(),
        jsonSupplierData).then(function (response) {
            try {
                var tmpData = response.data;
                var strResult = tmpData.string_value;
                if (_this.isStringValueNullOrEmpty(strResult)) {
                    _this.removeRowFromArray(entity);
                }
                else if (strResult === "disabled") {
                    entity.active = false;
                    //var msgDisabled = this.locSupplierWasDisabledText.replace("{0}", supplier.supp_name);
                    _this.showAlert(_this.locConfirmationText, _this.getMsgDisabled(entity), _this.locCloseText);
                }
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
    KamsykBaseGridTs.prototype.gridDeleteRow = function (entity, ev) {
        var _this = this;
        //var strMessage = this.locDeleteSupplierConfirmText.replace("{0}", supplier.supp_name);
        var confirm = this.$mdDialog.confirm()
            .title(this.locConfirmationText)
            .textContent(this.getMsgDeleteConfirm(entity))
            .ariaLabel("DeleteRowConfirm")
            .targetEvent(ev)
            .ok(this.locNoText)
            .cancel(this.locYesText);
        this.$mdDialog.show(confirm).then(function () {
        }, function () {
            _this.gridDeleteRowFromDb(entity, ev);
        });
    };
    return KamsykBaseGridTs;
}(kamsyk_base_1.KamsykBaseTs));
exports.KamsykBaseGridTs = KamsykBaseGridTs;
var UserGridSettings = /** @class */ (function () {
    function UserGridSettings() {
        this.grid_name = null;
        this.grid_page_size = null;
        this.filter = null;
        this.sort = null;
        this.columns = null;
    }
    return UserGridSettings;
}());
exports.UserGridSettings = UserGridSettings;
//# sourceMappingURL=kamsyk-base-grid.js.map