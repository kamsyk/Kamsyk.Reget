/// <reference path="../Base/reget-base.ts" />



module Kamsyk.RegetApp {
    var app = angular.module('RegetApp', ['ngMaterial', 'ngMessages', 'ui.grid', 'ui.grid.pagination', 'ui.grid.resizeColumns', 'ui.grid.selection', 'ui.grid.moveColumns', 'ui.grid.edit', 'ui.grid.autoResize', 'angularjs-dropdown-multiselect']);

    export interface IGridFilter {
        column_name: string;
        filter_value: any;
    }

    export interface IGridSort {
        column_name: string;
        direction: string;
    }

    export interface ISaveDataResponse {
        data: any;
        int_value: number;
        string_value: string;
        error_id: number;
        //error_msg: string;
    }

    export interface IGridEntity {
        row_index: number;
    }

    
    export abstract class BaseRegetGridTs extends BaseRegetTs {
        public pageSize: number = 10;
        public pageSizeGrid: number = 10 + 1; //because of new row
        public currentPage: number = 1;
        public sortColumnsUrl: string = "";
        public filterUrl: string = "";
        public gridOptions: uiGrid.IGridOptions = {};
        public gridApi: uiGrid.IGridApi = null;
        public rowsCount: number = 0;
        public lang: string = "en";
        public editRow: any = null;
        public editRowOrig: any = null;
        public editRowIndex: number = -1;
        public newRowIndex: number = -1;
        public isNewRow: boolean = false;
        public gridFilter: IGridFilter[];
        public gridSort: IGridSort[];
        //public dbGridId: string = null;
        public userGridSettings: UserGridSettings = null;
        private isGridSettingsSaved: boolean = false;
        public defaultColumnDefs: uiGrid.IColumnDefOf<any>[] = null;
        private isGridSettingsSet: boolean = false;
        //private isFilterIsLoaded: boolean = false;
        private isColumnHidden: boolean = false;
        protected isFilterApplied: boolean = false;
        public hiddenColNames: string[] = ["id"];
        protected testLoadDataCount: number = 0;
        private isInitDataSet: boolean = false;
        //private isUrlInit: boolean = false;
        private columnsWhichCanBeHidden: DropDownMultiSelectItem[] = null;
        private columnsWhichCanBeHiddenModel: DropDownMultiSelectItem[] = [];
        public isColumnDisplayHideBtnVisible: boolean = false;
        private hideColumnsIndexes: HideColIndex[] = [];
        //private isDisplHideColEnabled: boolean = false;

        private _urlParamDelimiter = "|";
        private _urlParamValueDelimiter = "~";


        //******************************************************************
        //Localization
        private rowMissingIdFieldText: string = $("#RowMissingIdFieldText").val()
        //******************************************************************

        //*****************************************************************
        //Constructors
        constructor(
            protected $scope: ng.IScope,
            protected $http: ng.IHttpService,
            protected $filter: ng.IFilterService,
            protected $mdDialog: angular.material.IDialogService,
            protected $mdToast: angular.material.IToastService,
            protected $q: ng.IQService,
            protected uiGridConstants: uiGrid.IUiGridConstants,
            protected $timeout: ng.ITimeoutService

            //protected $moment: moment.Moment
        ) {
            super($scope, $http, $filter, $mdDialog, $mdToast, $q, $timeout);
            this.lang = $("#GridLangCode").val();
            this.setGridOptionBase();
            this.defaultColumnDefs = angular.copy(this.gridOptions.columnDefs);

            $scope.dropDownMultiSelectStyle = {
                //scrollableHeight: '200px',
                scrollable: true,
                //enableSearch: true,
                //smartButtonMaxItems: 2,
                dynamicTitle: false,
                buttonClasses: 'btn btn-grid-multiselect'
            };

            $scope.onMultiSelect = (() => {
                //if (this.isDisplHideColEnabled === true) {
                    this.showHideColumns();
                //}
                
            });

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
        //******************************************************************

        //********************************************************************
        //abstract methods, properties
        abstract loadGridData(): void;
        abstract isRowChanged(): boolean;
        abstract insertRow(): void;
        abstract exportToXlsUrl(): string;
        abstract isRowEntityValid(rowEntity: any): string;
        abstract getSaveRowUrl(): string;
        abstract getDuplicityErrMsg(rowEntity: any): string;
        abstract getControlColumnsCount(): number;
        abstract getMsgDisabled(entity: any): string;
        abstract getMsgDeleteConfirm(entity: any): string;
        abstract getErrorMsgByErrId(errId: number, msg: string): string;
        abstract getDbGridId(): string;
        //abstract getColumsCanBeHidden: string[];
        //abstract getGridData(): any[];

        //abstract dbGridId: string = null;
        abstract deleteUrl: string = null;


        //********************************************************************

        //********************************************************************
        //Loc Texts
        public locMissingMandatoryText: string = $("#MissingMandatoryText").val();
        public locDateTimePickerFormatText: string = $("#DateTimePickerFormatText").val();
        public locConfirmationText: string = $("#ConfirmationText").val();
        //public locYesText: string = $("#YesText").val();
        //public locNoText: string = $("#NoText").val();
        public locTranslationText: string = $("#TranslationText").val();
        public locCancelText: string = $("#CancelText").val();
        public locEditText: string = $("#EditText").val();
        public locDeleteText: string = $("#DeleteText").val();
        public locSaveText: string = $("#SaveText").val();
        //********************************************************************

        //******************************************************************
        //Methods
        private setGridOptionBase(): void {
            this.gridOptions = {
                //data: this.m_substData,
                enableFiltering: true,
                enableRowSelection: true,
                enablePagination: true,
                paginationPageSizes: [10, 20, 50],
                paginationPageSize: this.pageSizeGrid,
                enableHorizontalScrollbar: 1, //0 - never, 1 when needed, 2 always
                enableVerticalScrollbar: 1,
                enablePaginationControls: false
            }

            this.gridOptions.appScopeProvider = this;

            this.gridOptions.onRegisterApi = (gridApi: uiGrid.IGridApi) => {
                this.gridApi = gridApi;

                //sort
                this.gridApi.core.on.sortChanged(this.$scope, (grid: any, sortColumns: any[]) => {
                    if (this.isSkipLoad) {
                        return;
                    }

                    if (!this.gridSaveRow()) {
                        return;
                    }

                    //if (this.isValueNullOrUndefined(this.currentPage) || this.currentPage < 1) {
                    //    this.currentPage = 1;
                    //}

                    var strSort = this.sortColumnsUrl;
                    this.sortColumnsUrl = "";
                    if (sortColumns !== null) {
                        for (var i = 0; i < sortColumns.length; i++) {
                            var name = sortColumns[i].name;
                            var direction = sortColumns[i].sort.direction;
                            if (this.sortColumnsUrl.length > 0) {
                                this.sortColumnsUrl += this._urlParamDelimiter;
                            }
                            this.sortColumnsUrl += name + this._urlParamValueDelimiter + direction;
                        }
                        if (strSort !== this.sortColumnsUrl) {
                            this.loadGridData();
                        }
                    }
                });

                //filter
                this.gridApi.core.on.filterChanged(this.$scope, this.debounce(() => {
                    this.filterChanged();
                }, 500));

                this.gridApi.core.on.columnVisibilityChanged(this.$scope, (col: uiGrid.IColumnDefOf<any>) => {
                    if (col.visible == false) {
                        this.isColumnHidden = true;
                        this.updateColumnsCanBeHidden(col.name);
                    }
                });

                //this.gridApi.core.on.filterChanged(this.$scope, this.debounce(this.filterChanged, 500));

                // this.gridApi.core.on.columnVisibilityChanged(this.$scope, this.columnVisibilityChanged);

            }

        }

        private updateColumnsCanBeHidden(colHidden: string): void {
            if (this.isValueNullOrUndefined(this.columnsWhichCanBeHiddenModel)) {
                return;
            }

            let colCanBeHidden: DropDownMultiSelectItem[] = this.$filter("filter")(this.columnsWhichCanBeHiddenModel, { id: colHidden }, true);
            if (this.isValueNullOrUndefined(colCanBeHidden)) {
                return;
            }
            if (colCanBeHidden.length == 0) {
                return;
            }

            for (let i: number = 0; i < this.columnsWhichCanBeHiddenModel.length; i++) {
                if (this.columnsWhichCanBeHiddenModel[i].id === colHidden) {
                    this.columnsWhichCanBeHiddenModel.splice(i, 1);
                    break;
                }
            }
        }

        public filterChanged(): void {
            //console.log("participant load debounce");
            if (this.isSkipLoad) {
                return;
            }

            if (!this.gridSaveRow()) {
                return;
            }


            // this.currentPage = 1;

            let strFilter: string = this.filterUrl;
            this.filterUrl = this.getFilterUrl();

            this.isFilterApplied = !this.isValueNullOrUndefined(this.filterUrl);

            if (strFilter !== this.filterUrl) {
                this.currentPage = 1;
                this.loadGridData();
            }
        }

        //public columnVisibilityChanged(column: any): void {
        //}

        private getFilterUrl(): string {
            if (this.isValueNullOrUndefined(this.gridApi)) {
                return "";
            }

            //Filter
            let filter: string = "";
            angular.forEach(this.gridApi.grid.columns, (col: uiGrid.IColumnDefOf<any>) => {
                if (col.enableFiltering
                    && !this.isValueNullOrUndefined(col.filters)
                    && col.filters.length > 0
                    && col.filters[0].term !== null && col.filters[0].term !== undefined
                    && (col.filters[0].term !== "")) {
                    if (filter.length > 0) {
                        filter += this.urlParamDelimiter;
                    }
                    filter += col.name + this.urlParamValueDelimiter + col.filters[0].term;
                }

                if (col.enableFiltering
                    && !this.isValueNullOrUndefined(col.filters)
                    && col.filters.length > 1
                    && col.filters[1].term !== null && col.filters[1].term !== undefined
                    && (col.filters[1].term !== "")) {
                    if (filter.length > 0) {
                        filter += this.urlParamDelimiter;
                    }

                    filter += col.name + this.urlParamValueDelimiter + col.filters[1].term;
                }

            });

            return filter;
        }

        public getLastPageIndex(): number {
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
        }

        public nextPage(): void {
            if (!this.gridSaveRow()) {
                return;
            }

            if (this.isLastPage()) {
                return;
            } else {
                this.currentPage++;
            }

            this.loadGridData();
        }

        public previousPage(): void {
            if (!this.gridSaveRow()) {
                return;
            }

            if (this.isFirstPage()) {
                return;
            } else {
                this.currentPage--;
            }

            this.loadGridData();

        }

        public isLastPage(): boolean {

            var tmpCurrentPage = this.currentPage + 1;
            if ((tmpCurrentPage * this.pageSize) >= (this.rowsCount + this.pageSize)) {
                return true;
            }

            return false;
        }

        public isFirstPage(): boolean {
            var tmpCurrentPage = this.currentPage - 1;
            if (tmpCurrentPage < 1) {
                return true;
            }

            return false;
        }

        public gotoPage(): void {
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
        }

        public firstPage(): void {
            if (!this.gridSaveRow()) {
                return;
            }

            this.currentPage = 1;
            this.loadGridData();
        }

        public lastPage(): void {
            if (!this.gridSaveRow()) {
                return;
            }

            this.currentPage = this.getLastPageIndex();
            this.loadGridData();
        };

        public getDisplayItemsToInfo(): number {
            var iToInfo = (this.currentPage) * this.pageSize;
            if (iToInfo > this.rowsCount) {
                iToInfo = this.rowsCount;
            }
            return iToInfo;
        }

        public getDisplayItemsFromInfo(): number {
            if (this.currentPage == 0) {
                return 0;
            }

            return (this.currentPage - 1) * this.pageSize + 1;

        }

        public refresh(): void {
            this.loadGridData();

            this.editRow = null;
            this.editRowOrig = null;
            this.editRowIndex = null;
            this.newRowIndex = null;
            this.isNewRow = false;
        }

        public cellClicked(row?: any, col?: any): void {
            //alert("clicked");
            if (!this.isValueNullOrUndefined(this.getRowIdFiledName()) && row.entity[this.getRowIdFiledName()] < -1) {
                return;
            }

            this.gridEditRow(row.entity);
        }

        public getRowIdFiledName(): string {
            return "id";
        }

        public gridEditRow(rowEntity: any): void {
            var tmpData: any = this.gridOptions.data;
            this.editRowIndex = tmpData.indexOf(rowEntity);  //syka ts

            if (this.isValueNullOrUndefined(this.editRow)) {
                this.editRowChanged(false);
            } else {
                this.gridSaveRow();
            }
        }

        public editRowChanged(isNewRow: boolean) {

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

        }

        public cancelEditRow(editRow: any): any {
            if (!this.isValueNullOrUndefined(editRow)) {
                editRow.editrow = false;
                editRow = null;
            }

            return editRow;
        }

        public gridSaveRow(): boolean {

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

                var rowEntity = this.$filter("filter")(this.gridOptions.data as any[], { id: id }, true);

                if (rowEntity[0][this.getRowIdFiledName()] < -1) {
                    isChanged = true;
                    this.newRowIndex = null;
                }

                if (!isChanged) {
                    isChanged = this.isRowChanged();
                }

                if (isChanged) {
                    return this.saveGridRowtoDb(rowEntity[0]);
                } else {
                    this.editRowChanged(false);
                }

                this.restoreFilter();

                return true;
            } catch (ex) {
                throw ex;
            } finally {
                this.isSkipLoad = false;
            }
        }

        public gridCancelEdit() {
            this.showLoaderBoxOnly();

            try {
                var isNewRow = (!this.isValueNullOrUndefined(this.editRow));
                if (!this.isValueNullOrUndefined(this.getRowIdFiledName())) {
                    isNewRow = isNewRow && this.editRow[this.getRowIdFiledName()] < -1
                }

                if (isNewRow) {
                    this.restoreFilter();
                }

                this.newRowIndex = null;
                if (this.editRow !== null) {

                    this.editRow.editrow = false;

                    if (isNewRow) {
                        var data: any = this.gridOptions.data;
                        var index = data.indexOf(this.editRow); //syka ts
                        data.splice(index, 1);
                        this.gridOptions.paginationPageSize--;
                    } else {
                        angular.forEach(this.editRow, (value, key) => {
                            this.editRow[key] = this.editRowOrig[key];
                        });
                    }

                    this.editRow = null;
                    this.editRowIndex = null;

                    if (isNewRow) {
                        this.enableFiltering();
                    }
                }
            } catch (ex) {
                throw ex;
            } finally {
                this.isSkipLoad = false;
                this.hideLoader();
            }
        }

        private restoreFilter() {
            if (!this.isValueNullOrUndefined(this.gridFilter)) {
                for (var i: number = 0; i < this.gridFilter.length; i++) {
                    angular.forEach(this.gridApi.grid.columns, (col: uiGrid.IColumnDefOf<any>) => {
                        if (col.name === this.gridFilter[i].column_name) {
                            col.filters[0].term = this.gridFilter[i].filter_value;
                            this.isFilterApplied = true;
                        }
                    });
                    //for (var j: number = 0; j < this.gridOptions.columnDefs.length; j++) {
                    //    if (this.gridOptions.columnDefs[j].name === this.gridFilter[i].column_name) {
                    //        this.gridOptions.columnDefs[j].filters[0].term = this.gridFilter[i].filter_value;
                    //    }
                    //}
                }
            }
        }

        protected saveGridRowtoDb(rowEntity: any): boolean {
            var strErrMsg = this.isRowEntityValid(rowEntity);
            if (!this.isStringValueNullOrEmpty(strErrMsg)) {
                this.displayErrorMsg(strErrMsg);
                return;
            }

            this.isError = false;


            this.showLoaderBoxOnly(this.isError);
            var jsonEntityData = JSON.stringify(rowEntity);

            this.$http.post(
                this.getSaveRowUrl(),
                jsonEntityData
            ).then((response) => {
                try {
                    let result: ISaveDataResponse = response.data as ISaveDataResponse;
                    let iId: number = result.int_value;


                    //if (!this.isValueNullOrUndefined(response.data)) {
                    let errMsg: string = null;
                    var splitIndex: number = result.string_value.indexOf(",");
                    if (result.string_value.indexOf(",") > -1) {
                        var errType: string = result.string_value.substring(0, splitIndex);
                        if (errType === "DUPLICITY") {
                            var duplicityPpgName: string = result.string_value.substring(splitIndex + 1).trim();
                            errMsg = this.getDuplicityErrMsg(rowEntity);
                            this.displayErrorMsg(errMsg);
                            return;
                        }
                    } else if (result.string_value === "DUPLICITY") {
                        this.hideLoader();
                        errMsg = this.getDuplicityErrMsg(rowEntity);
                        if (this.isValueNullOrUndefined(errMsg)) {
                            errMsg = "Duplicity values";
                        }
                        this.displayErrorMsg(errMsg);
                        return;
                    }

                    if (!this.isValueNullOrUndefined(result) && !this.isValueNullOrUndefined(result.error_id)) {
                        if (result.error_id != 0) {
                            let errMsg: string = this.getErrorMsgByErrId(result.error_id, result.string_value);
                            let errMsgDialog = errMsg;
                            let errLinkDialog = null;
                            if (!this.isStringValueNullOrEmpty(errMsg)) {
                                if (errMsg.indexOf(this.urlParamDelimiter) > 0) {
                                    let strItems = errMsg.split(this.urlParamDelimiter);
                                    errMsgDialog = strItems[0];
                                    errLinkDialog = strItems[1];
                                }
                            }
                            this.displayErrorMsg(errMsgDialog, errLinkDialog);
                            return;
                        }
                    }
                    //} else
                    if (iId === -1) {
                        this.hideLoader();
                        this.displayErrorMsg();
                        return;
                    }



                    var isNew: boolean = false;
                    if (rowEntity.id < 0) {
                        isNew = true;
                        rowEntity.id = iId;
                        this.rowsCount++;
                    }

                    this.editRowChanged(false);
                    this.newRowIndex = null;

                    if (isNew) {
                        this.enableFiltering();
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
                return false;
            });

            return true;

        }

        private setDataGridSettings(): void {
            this.showLoaderBoxOnly();
            var jsonGridData = JSON.stringify(this.userGridSettings);

            this.$http.post(
                this.getRegetRootUrl() + 'DataGrid/SetUserGridSettings?t=' + new Date().getTime(),
                jsonGridData
            ).then((response) => {
                try {
                    try {
                        this.isGridSettingsSaved = true;
                        this.hideLoader();
                    } catch (ex) {
                        this.hideLoader();
                        this.displayErrorMsg();
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

        public resetGridSettings(): void {
            this.showLoaderBoxOnly();
            var jsonUserGridData = JSON.stringify(this.userGridSettings);

            this.$http.post(
                this.getRegetRootUrl() + 'DataGrid/DeleteUserGridSettings?t=' + new Date().getTime(),
                jsonUserGridData
            ).then((response) => {

                try {
                    var result = response.data;

                    if (this.isGridSettingsSaved === true) {
                        //workaround - if the grid is loaded then columns order is changed and save and then Reset is called the columns order is nor reset - page reload is needed.
                        //if the reset is called wo saving before it works fine
                        var isColumnOrderModified = this.isColumnOrderChanged(
                            this.userGridSettings.columns,
                            this.gridOptions.columnDefs);

                        if (isColumnOrderModified) {
                            window.location.reload(true);
                            return;
                        }
                    }

                    this.filterUrl = "";
                    this.sortColumnsUrl = "";

                    this.pageSize = 10;
                    this.pageSizeGrid = this.pageSize + 1;
                    this.gridOptions.paginationPageSize = this.pageSizeGrid;

                    this.isSkipLoad = true;
                    //this.gridOptions.columnDefs = angular.copy(this.defaultColumnDefs);
                    //this.restoreColumns(this.defaultColumnDefs);
                    this.clearFilters();
                    this.clearSort();
                    this.isSkipLoad = false;

                    this.userGridSettings = null;
                    this.showAllColumns();

                    this.loadGridData();


                } catch (ex) {
                    this.hideLoader();
                    this.displayErrorMsg();

                } finally {
                    //this.hideLoader();
                }
            }, (response: any) => {
                this.hideLoader();
                this.displayErrorMsg();
            });

        }

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
        public getDataGridSettings(): void {
            this.showLoader();

            
            this.$http.get(
                this.getRegetRootUrl() + 'DataGrid/GetUserGridSettings?gridId=' + this.getDbGridId() + '&t=' + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpUserGridSettings: any = response.data;
                    this.userGridSettings = tmpUserGridSettings;
                    if (!this.isValueNullOrUndefined(this.userGridSettings)) {
                        this.pageSize = this.userGridSettings.grid_page_size;
                        this.pageSizeGrid = this.pageSize + 1;
                        this.gridOptions.paginationPageSize = this.pageSizeGrid;

                        this.gridFilter = this.getGridFilterFromUrlString(this.userGridSettings.filter);
                        this.restoreFilter();
                        this.filterUrl = this.getFilterUrl();

                        this.sortColumnsUrl = this.userGridSettings.sort;
                        this.getGridSortFromUrlString(this.userGridSettings.sort);
                        this.restoreSort();

                        var grdColumns: uiGrid.IColumnDefOf<any>[] = this.getGridColumnsFromUrlString(this.userGridSettings.columns);
                        this.restoreColumns(grdColumns);
                    }

                    
                    this.loadGridData();
                } catch (e) {
                    this.resetGridSettings();
                    this.hideLoader();
                    //this.displayErrorMsg();
                } finally {
                    this.hideLoader();
                }
            }, (response: any) => {
                this.resetGridSettings();
                this.hideLoader();
                //this.displayErrorMsg();
            });
        }

        public loadGridSettings(): void {
            //if (this.isUrlInit === true) {
            //    this.initDataLoadParams();
            //    return;
            //}

            //this.initDataLoadParams();

            if (this.userGridSettings === null) {
                this.defaultColumnDefs = angular.copy(this.gridOptions.columnDefs);
                this.getDataGridSettings();
            }
        }

        //***************************************************
        // Standart Method to Load grid Data
        //****************************************************
        public getLoadDataGridSettings(): void {
            this.columnsWhichCanBeHidden = this.getColumsCanBeHidden();

            this.$http.get(
                this.getRegetRootUrl() + 'DataGrid/GetUserGridSettings?gridId=' + this.getDbGridId() + '&t=' + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpUserGridSettings: any = response.data;
                    this.userGridSettings = tmpUserGridSettings;

                    //let isFilterChanged : boolean = false;
                    this.getUrlParams();

                    if (!this.isValueNullOrUndefined(this.userGridSettings)) {
                        this.pageSize = this.userGridSettings.grid_page_size;
                        this.pageSizeGrid = this.pageSize + 1;
                        this.gridOptions.paginationPageSize = this.pageSizeGrid;

                        if (!this.isValueNullOrUndefined(this.userGridSettings.page_nr)) {
                            this.currentPage = this.userGridSettings.page_nr;
                            if (this.currentPage < 1) {
                                this.currentPage = 1;
                            }
                        }

                        this.gridFilter = this.getGridFilterFromUrlString(this.userGridSettings.filter);
                        this.filterUrl = this.userGridSettings.filter;
                        //this.filterUrl = this.getFilterUrl();
                        //if (!this.isStringValueNullOrEmpty(this.userGridSettings.filter)) {
                        //    isFilterChanged = true;
                        //}

                        this.sortColumnsUrl = this.userGridSettings.sort;
                        this.getGridSortFromUrlString(this.userGridSettings.sort);

                    }

                    //if (!isFilterChanged) {
                    //if filter is changed the loadGridData is launched from this.gridApi.core.on.filterChanged
                    this.loadGridData();
                    //}
                } catch (e) {
                    this.resetGridSettings();
                    this.hideLoader();
                } finally {
                    this.hideLoader();
                }
            }, (response: any) => {
                this.resetGridSettings();
                this.hideLoader();
            });
        }

        public setGridSettingData(): void {
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

                let grdColumns: uiGrid.IColumnDefOf<any>[] = this.getGridColumnsFromUrlString(this.userGridSettings.columns);
                this.restoreColumns(grdColumns);
                this.isSkipLoad = false;
            } else {
                this.setColumnsWhichCanBeHiddenModel();
            }


        }

        public addNewRow(): void {
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
            } else {
                this.newRowIndex = this.getNewRowIndex();
                this.editRowIndex = this.newRowIndex;
                this.gridSaveRow();
            }

            //this.FormatGridUserLookup();
        }

        private getNewRowIndex(): number {
            let newRowIndex: number = this.pageSize;
            let grid: any = this.gridApi.grid;
            let gridRowsCount: number = grid.rows.length;
            if (gridRowsCount < this.pageSize || gridRowsCount > this.pageSize) {
                newRowIndex = gridRowsCount;
            }

            if (newRowIndex < 0) {
                newRowIndex = 0;
            }


            return newRowIndex;
        }

        private enableFiltering(): void {
            this.isSkipLoad = true;
            for (var i = this.getControlColumnsCount(); i < this.gridOptions.columnDefs.length; i++) {
                this.gridOptions.columnDefs[i].enableFiltering = true;
                this.gridApi.core.notifyDataChange(this.uiGridConstants.dataChange.COLUMN);
            }
            this.isSkipLoad = false;
        }

        public pageSizeChanged(): void {
            if (!this.gridSaveRow()) {
                return;
            }

            this.currentPage = 1;
            this.pageSizeGrid = this.pageSize + 1;
            this.gridOptions.paginationPageSize = this.pageSizeGrid;
            this.loadGridData();
        };

        public exportToXls(): void {
            try {
                window.open(this.exportToXlsUrl());
            } catch (e) {
                this.displayErrorMsg();
            } finally {
                this.hideLoader();
            }

        }

        public dateTimeChanged(row_entity: any, col: any): void {
            //try {
            //    if (!this.isValueNullOrUndefined(dateTimeWa)) {
            //        row_entity[col.field] = dateTimeWa;
            //    }
            //} catch (e) {
            //    this.displayErrorMsg();
            //} finally {
            //    dateTimeWa = null;
            //}
        }

        //public getMissinMandatoryText(): string {
        //    return $("#MissingMandatoryText").val();
        //}

        //public getDateTimePickerFormatText(): string {
        //    return $("#DateTimePickerFormatText").val();
        //}

        public hideEditGridButton(rowEntity: any): boolean {
            return false;
        }

        public hideDeleteGridButton(rowEntity: any): boolean {
            return false;
        }

        public getColumnIndex(strColumnName): number {
            if (this.isValueNullOrUndefined(this.gridApi)) {
                return -1;
            }
            for (var i = 0; i < this.gridApi.grid.columns.length; i++) {
                if (this.gridApi.grid.columns[i].name === strColumnName) {
                    return i;
                }
            }

            return -1;
        }

        //*******************************************
        //Save Grid Setting
        public saveGridSettings(): void {
            this.setGridSettings();
            this.setDataGridSettings();
        }

        private setGridSettings(): void {
            this.userGridSettings = new UserGridSettings();

            //Grid Name
            this.userGridSettings.grid_name = this.getDbGridId();

            //Page Size
            this.userGridSettings.grid_page_size = this.pageSize;

            //Filter
            var filter: string = this.getFilterUrl();
            this.userGridSettings.filter = filter;

            //Sort
            var sort: string = this.getSortUrl();
            this.userGridSettings.sort = sort;

            //Columns
            var columns: string = this.getColumnstUrl();
            this.userGridSettings.columns = columns;

        }

        private getColumnstUrl(): string {

            //columns
            var columns: string = '';
            angular.forEach(this.gridApi.grid.columns, (col: uiGrid.IColumnDefOf<any>) => {

                if (columns.length > 0) {
                    columns += this.urlParamDelimiter;
                }
                columns += col.name + this.urlParamValueDelimiter + col.visible;
            });

            return columns;
        }

        private getSortUrl(): string {

            //sort
            var sort = '';
            angular.forEach(this.gridApi.grid.columns, (col: uiGrid.IColumnDefOf<any>) => {
                if (!this.isValueNullOrUndefined(col.sort) && !this.isValueNullOrUndefined(col.sort.direction)) {
                    if (sort.length > 0) {
                        sort += this.urlParamDelimiter;
                    }
                    sort += col.name + this.urlParamValueDelimiter + col.sort.direction;
                }
            });

            return sort;
        }

        private isColumnOrderChanged(currentColumnsString: string, origColumns: uiGrid.IColumnDefOf<any>[]): boolean {
            var currentColumns: uiGrid.IColumnDefOf<any>[] = this.getGridColumnsFromUrlString(currentColumnsString);
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
        }

        private getGridColumnsFromUrlString(strUrlColumns: string): uiGrid.IColumnDefOf<any>[] {
            var gridColumns: uiGrid.IColumnDefOf<any>[] = [];

            if (!this.isStringValueNullOrEmpty(strUrlColumns)) {
                var columnsItems: string[] = strUrlColumns.split(this.urlParamDelimiter);
                for (var i: number = 0; i < columnsItems.length; i++) {
                    var itemFields: string[] = columnsItems[i].split(this.urlParamValueDelimiter);
                    var oVal: string = itemFields[1];
                    var isVal: boolean = (oVal === 'true') ? true : false;

                    var tmpCol: any = {};
                    var tmpGridCol: uiGrid.IColumnDefOf<any> = tmpCol;
                    tmpGridCol.name = itemFields[0];
                    tmpGridCol.visible = isVal;
                    gridColumns.push(tmpGridCol);
                }
            }

            return gridColumns;
        }

        public clearFilters(): void {
            this.gridFilter = [];

            angular.forEach(this.gridApi.grid.columns, (col: uiGrid.IColumnDefOf<any>) => {
                if (col.enableFiltering && (!this.isValueNullOrUndefined(col.filters[0].term))) {
                    var tmpFilter: any = {};
                    var tmpFilterGrid: IGridFilter = tmpFilter;
                    tmpFilterGrid.column_name = col.name;
                    tmpFilterGrid.filter_value = col.filters[0].term;

                    this.gridFilter.push(tmpFilterGrid);
                    col.filters[0].term = '';
                }
            });

            this.isFilterApplied = false;
        }

        public clearFiltersReload(): void {
            this.clearFilters();
            this.loadGridData();
        }

        private showAllColumns(): void {
            this.gridOptions.columnDefs = angular.copy(this.defaultColumnDefs);
            this.setColumnsWhichCanBeHiddenModel();
            //this.userGridSettings = null;
            this.isColumnHidden = false;
        }

        private showAllColumnsReload(): void {
            this.showAllColumns();
            this.loadGridData();
        }

        private clearSort(): void {
            //var isFilter: boolean = false;
            angular.forEach(this.gridApi.grid.columns, (col: any) => {
                col.unsort();
                //col.sortCellFiltered = false;
            });

            //return isFilter;
        }

        private getGridFilterFromUrlString(strUrlFilter: string): IGridFilter[] {
            var gridFilter: IGridFilter[] = [];

            if (!this.isStringValueNullOrEmpty(strUrlFilter)) {
                var filterItems: string[] = strUrlFilter.split(this.urlParamDelimiter);
                for (var i = 0; i < filterItems.length; i++) {
                    var itemFields: string[] = filterItems[i].split(this.urlParamValueDelimiter);
                    var oVal: any = itemFields[1];
                    if (oVal === 'true') {
                        oVal = true;
                    } else if (oVal === 'false') {
                        oVal = false;
                    }


                    gridFilter.push({
                        column_name: itemFields[0],
                        filter_value: oVal
                    });
                }
            }

            return gridFilter;
        }

        private getGridSortFromUrlString(strUrlSort: string): void {
            this.gridSort = [];

            if (!this.isStringValueNullOrEmpty(strUrlSort)) {
                var sortItems: string[] = strUrlSort.split(this.urlParamDelimiter);
                for (var i = 0; i < sortItems.length; i++) {
                    var itemFields: string[] = sortItems[i].split(this.urlParamValueDelimiter);
                    this.gridSort.push({
                        column_name: itemFields[0],
                        direction: itemFields[1]
                    });
                }
            }

        }

        private restoreSort(): void {
            if (!this.isValueNullOrUndefined(this.gridSort)) {
                for (var i = 0; i < this.gridSort.length; i++) {
                    angular.forEach(this.gridApi.grid.columns, (col: uiGrid.IColumnDefOf<any>) => {
                        if (col.name === this.gridSort[i].column_name) {
                            col.sort.direction = this.gridSort[i].direction;
                        }
                    });
                }
            }
        }

        private restoreColumns(gridColumns: uiGrid.IColumnDefOf<any>[]) {

            if (this.isValueNullOrUndefined(gridColumns) || gridColumns.length == 0) {
                return;
            }

            for (var i: number = 0; i < gridColumns.length; i++) {
                if (gridColumns[i].name === "id") {
                    continue;
                }

                if (gridColumns[i].visible === false) {
                    //let hidCol: string[] = this.$filter("filter")(this.hiddenColNames, { gridColumns[i].name }, true);
                    let isPermHiddenCol: boolean = false;
                    if (!this.isValueNullOrUndefined(this.hiddenColNames)) {
                        for (let j: number = 0; j < this.hiddenColNames.length; j++) {
                            //if (this.hiddenColNames[j].toLowerCase().trim().indexOf("action_buttons") > -1) {
                            //    continue;
                            //}
                            if (this.hiddenColNames[j].toLowerCase().trim() == gridColumns[i].name.toLowerCase().trim()) {
                                isPermHiddenCol = true;
                                break;
                            }
                        }
                    }

                    if (!isPermHiddenCol) {
                        this.isColumnHidden = true;
                    }

                    for (var j: number = this.gridOptions.columnDefs.length - 1; j >= 0; j--) {
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

            for (var j: number = this.gridOptions.columnDefs.length - 1; j >= 0; j--) {
                if (this.gridOptions.columnDefs[j].name === "id") {
                    continue;
                }
                let isFound: boolean = false;
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
        }

        private setColumnsOrder(gridColumns: uiGrid.IColumnDefOf<any>[]) {
            var orderedColumns: uiGrid.IColumnDefOf<any>[] = [];
            var isReordered: boolean = false;
            var colIndex: number = 0;

            for (var i = 0; i < gridColumns.length; i++) {
                if (gridColumns[i].visible === false && gridColumns[i].name !== "id") {
                    continue;
                }

                //var col = angular.copy(this.gridOptions.columnDefs[i]);

                if (this.gridOptions.columnDefs.length > colIndex) {
                    if (this.gridOptions.columnDefs[colIndex].name === gridColumns[i].name) {
                        orderedColumns.push(this.gridOptions.columnDefs[colIndex]);
                    } else {
                        for (var j: number = 0; j < this.gridOptions.columnDefs.length; j++) {
                            if (this.gridOptions.columnDefs[j].name === gridColumns[i].name) {
                                orderedColumns.push(this.gridOptions.columnDefs[j]);
                                isReordered = true;
                                break;
                            }
                        }
                    }
                } else {
                    for (var j: number = 0; j < this.gridOptions.columnDefs.length; j++) {
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

        }

        private setColumnsWhichCanBeHiddenModel(): void {
            //this.isDisplHideColEnabled = false;
            //this.isColumnDisplayHideBtnVisible = false;
            try {
                this.columnsWhichCanBeHiddenModel = [];
                for (let i: number = 0; i < this.gridOptions.columnDefs.length; i++) {
                    if (this.isValueNullOrUndefined(this.gridOptions.columnDefs[i].enableHiding)
                        || this.gridOptions.columnDefs[i].enableHiding === true) {
                        this.columnsWhichCanBeHiddenModel.push({
                            id: this.gridOptions.columnDefs[i].name
                            , label: this.gridOptions.columnDefs[i].displayName
                        });
                        //this.isColumnDisplayHideBtnVisible = true;
                    }
                }
            } catch (e) {
                this.displayErrorMsg();
            } finally {
                //this.isDisplHideColEnabled = true;
            }
        }

        public insertBaseRow(newEntity: IGridEntity): void {

            //clear filters
            this.isSkipLoad = true;
            this.clearFilters();

            if (this.isValueNullOrUndefined(this.rowsCount)) {
                this.rowsCount = 0;
            }


            if (this.isValueNullOrUndefined(this.gridOptions.data)) {
                this.gridOptions.data = [];
            }

            var grdData: any = this.gridOptions.data;
            var rowIndex = this.getNewRowIndexColValue();
            newEntity.row_index = rowIndex;

            grdData.push(newEntity);

            this.gridOptions.paginationPageSize++;

        }

        public getNewRowIndexColValue(): number {
            var rowIndex = (this.currentPage - 1) * this.pageSize + this.gridOptions.paginationPageSize;
            if (rowIndex > this.rowsCount + 1) {
                rowIndex = this.rowsCount + 1;
            }

            return rowIndex;
        }

        public removeRowFromArray(entity: any) {
            let tmpCurrPage = this.currentPage;

            let tmpData: any = this.gridOptions.data;

            //Remove row from array
            var index = tmpData.indexOf(entity); //syka ts
            var gridData: any = this.gridOptions.data;
            gridData.splice(index, 1);

            if (this.rowsCount > 1) {
                this.rowsCount--;

            }

            //check whether it was last record
            if (this.getLastPageIndex() < tmpCurrPage && this.currentPage > 0) {
                this.previousPage();
            }
        }

        public toggleGridCheckbox(item: any, col: uiGrid.IColumnDefOf<any>) {

            if (item[col.field]) {
                item[col.field] = false;
            } else {
                item[col.field] = true;
            }

        }

        public sortNullString(a: string, b: string, rowA: any, rowB: any, direction: any): number {
            a = (this.isStringValueNullOrEmpty(a)) ? ' ' : a;
            b = (this.isStringValueNullOrEmpty(b)) ? ' ' : b;
            if (a === b) { return 0 };
            if (a < b) { return -1 };
            if (a > b) { return 1 };
        }

        protected gridDeleteRowFromDb(entity: any, ev: MouseEvent): void {
            // var deferred: ng.IDeferred<boolean> = this.$q.defer<boolean>();

            this.showLoaderBoxOnly(this.isError);

            let jsonData: any = JSON.stringify(entity);

            this.$http.post(
                this.deleteUrl,
                //this.getRegetRootUrl() + 'Supplier/DeleteSupplier' + '?t=' + new Date().getTime(),
                jsonData
            ).then((response) => {
                try {
                    this.gridRowWasDeleted(response, entity);
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

            //return deferred.promise;
        }


        protected gridRowWasDeleted(response: any, entity: any): boolean {
            //let isOk: boolean = false;
            var tmpData: any = response.data;
            var strResult = tmpData.string_value;

            if (this.isStringValueNullOrEmpty(strResult)) {
                this.removeRowFromArray(entity);

            } else if (strResult === "disabled") {
                entity.active = false;
                this.showAlert(this.locConfirmationText, this.getMsgDisabled(entity), this.locCloseText);
            }

            if (!this.isValueNullOrUndefined(tmpData) && !this.isValueNullOrUndefined(tmpData.error_id)) {
                if (tmpData.error_id != 0) {
                    let errMsg: string = this.getErrorMsgByErrId(tmpData.error_id, tmpData.string_value);
                    let errMsgDialog = errMsg;
                    let errLinkDialog = null;
                    if (!this.isStringValueNullOrEmpty(errMsg)) {
                        if (errMsg.indexOf(this.urlParamDelimiter) > 0) {
                            let strItems = errMsg.split(this.urlParamDelimiter);
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
        }


        public gridDeleteRow(entity: any, ev: MouseEvent) {
            this.$mdDialog.show(
                {
                    template: this.getConfirmDialogTemplate(
                        this.getMsgDeleteConfirm(entity),
                        this.locConfirmationText,
                        this.locYesText,
                        this.locNoText,
                        "confirmDialog()",
                        "closeDialog()"),
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

        }

        private dialogConfirmController($scope, $mdDialog, regetBaseGrid, entity, ev): void {

            $scope.closeDialog = function () {
                $mdDialog.hide();
            }

            $scope.confirmDialog = () => {
                $mdDialog.hide();
                regetBaseGrid.gridDeleteRowFromDb(entity, ev);
            }
        }

        //Formats user lookup textbox in Centres grid, must be here
        public formatGridLookup(
            autoId: string,
            autoInputId: string,
            fieldName: string,
            interval: any): void {

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
                    } else {
                        gridUserInput.style.color = "#000";
                    }

                    clearInterval(interval);
                    //console.log(autoId);
                }
                //}, 100);
            } catch (e) { }
        }

        public dropDownChanged(item: any): void {

        }

        public setGridColumFilter(columnName: string, filterValues: any[]): boolean {
            var pos: number = this.getColumnIndex(columnName);
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
        }

        protected getRowDisabledBkg(activeItem: boolean): string {
            if (this.isValueNullOrUndefined(activeItem) || activeItem === false) {
                return 'grid-row-disabled';
            }

            return null;
        }

        protected insertDataGridColum(gridColumn: uiGrid.IColumnDefOf<any>, iEditColIndex: number): void {
            if (this.isValueNullOrUndefined(gridColumn) || iEditColIndex < 0) {
                return;
            }

            let bkgGridColumns = angular.copy(this.gridOptions.columnDefs);
            this.gridOptions.columnDefs = [];
            if (iEditColIndex > (bkgGridColumns.length - 1)) {
                iEditColIndex = (bkgGridColumns.length - 1);
            }
            for (var j: number = 0; j < bkgGridColumns.length; j++) {
                if (iEditColIndex == j) {
                    this.gridOptions.columnDefs.push(gridColumn);
                }
                this.gridOptions.columnDefs.push(bkgGridColumns[j]);
            }
        }

        private getUrlParams(): void {
            let strFilter: string = null;
            let strSort: string = null;
            let pageSize: number = null;
            let currentPage: number = null;
            let isUrlFilter: boolean = false;

            if (this.isInitDataSet === false) {
                this.isInitDataSet = true;
                let urlParams: UrlParam[] = this.getAllUrlParams();
                if (!this.isValueNullOrUndefined(urlParams)) {
                    for (var i: number = 0; i < urlParams.length; i++) {
                        if (urlParams[i].param_name.toLowerCase() === "pagesize") {
                            pageSize = parseInt(urlParams[i].param_value);
                            isUrlFilter = true;
                        } else if (urlParams[i].param_name.toLowerCase() === "currpage") {
                            currentPage = parseInt(urlParams[i].param_value);
                            isUrlFilter = true;
                        } else if (urlParams[i].param_name.toLowerCase() === "filter") {
                            strFilter = this.decodeUrl(urlParams[i].param_value);
                            isUrlFilter = true;
                        } else if (urlParams[i].param_name.toLowerCase() === "sort") {
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
                    } else {
                        this.userGridSettings.grid_page_size = pageSize;
                        this.userGridSettings.filter = strFilter;
                        this.userGridSettings.sort = strSort;
                        this.userGridSettings.page_nr = currentPage;
                    }
                    //this.getDataGridSettingsByParams(pageSize, strFilter, strSort, currentPage);
                }
            }
        }


        private getColumsCanBeHidden(): DropDownMultiSelectItem[] {
            this.isColumnDisplayHideBtnVisible = false;

            if (this.isValueNullOrUndefined(this.gridOptions.columnDefs) || this.gridOptions.columnDefs.length == 0) {
                return null;
            }

            let hidCoumns: DropDownMultiSelectItem[] = [];
            for (let i: number = 0; i < this.gridOptions.columnDefs.length; i++) {
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
        }

        protected setColumnsCanBeHidden() : void {
            this.columnsWhichCanBeHidden = this.getColumsCanBeHidden();
        }

        private showHideColumns(): void {
            try {
                if (this.isValueNullOrUndefined(this.defaultColumnDefs)) {
                    return;
                }

                if (this.isValueNullOrUndefined(this.gridOptions.columnDefs)) {
                    return;
                }

                if (this.isValueNullOrUndefined(this.columnsWhichCanBeHiddenModel) || this.columnsWhichCanBeHiddenModel.length == 0) {
                    //hide all hideable columns
                    for (let i: number = 0; i < this.gridOptions.columnDefs.length; i++) {
                        if (this.gridOptions.columnDefs[i].enableHiding === true) {
                            this.gridOptions.columnDefs.splice(i, 1);
                        }
                    }

                    return;
                }

               

                    //hide solumns
                    for (let i: number = 0; i < this.gridOptions.columnDefs.length; i++) {
                        if (this.gridOptions.columnDefs[i].enableHiding === true) {
                            let displayCol: DropDownMultiSelectItem[] = this.$filter("filter")(this.columnsWhichCanBeHiddenModel, { id: this.gridOptions.columnDefs[i].name }, true);
                            if (this.isValueNullOrUndefined(displayCol) || displayCol.length == 0) {
                                this.addHideColPosition(this.gridOptions.columnDefs[i].name, i);
                                this.gridOptions.columnDefs.splice(i, 1);
                            }
                        }
                    }

                    //display columns
                    for (let i: number = 0; i < this.columnsWhichCanBeHiddenModel.length; i++) {
                        let displayCol: uiGrid.IGridOptions[] = this.$filter("filter")(this.gridOptions.columnDefs, { name: this.columnsWhichCanBeHiddenModel[i].id }, true);
                        if (this.isValueNullOrUndefined(displayCol) || displayCol.length == 0) {
                            let displayColDef: uiGrid.IGridOptions[] = this.$filter("filter")(this.defaultColumnDefs, { name: this.columnsWhichCanBeHiddenModel[i].id }, true);
                            let iPos: number = this.getHideColPosition(this.columnsWhichCanBeHiddenModel[i].id);
                            if (iPos === -1) {
                                this.gridOptions.columnDefs.push(displayColDef[0]);
                            } else {
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
            } catch (e) {
                this.displayErrorMsg();
            }
        }

        private addHideColPosition(columnName: string, position: number): void {
            let hideColPos: HideColIndex[] = this.$filter("filter")(this.hideColumnsIndexes, { name: columnName }, true);
            if (!this.isValueNullOrUndefined(hideColPos) && hideColPos.length > 0) {
                for (let i: number = 0; i < this.hideColumnsIndexes.length; i++) {
                    if (this.hideColumnsIndexes[i].name === columnName) {
                        this.hideColumnsIndexes.splice(i);
                        break;
                    }
                    
                }
            }
            this.hideColumnsIndexes.push({ name: columnName, position: position });
        }

        private getHideColPosition(columnName : string): number {
            let hideColPos: HideColIndex[] = this.$filter("filter")(this.hideColumnsIndexes, { name: columnName }, true);
            if (!this.isValueNullOrUndefined(hideColPos) && hideColPos.length > 0) {
                return hideColPos[0].position;
            }

            return -1;
        }

        public validateMultiEmailsGrid(row: any, col: uiGrid.IColumnDefOf<any>): boolean {
            let cellTxtId = "grdMail" + row.uid + col.name;
            let mailCell: HTMLInputElement = <HTMLInputElement>document.getElementById(cellTxtId);
            if (this.isValueNullOrUndefined(mailCell)) {
                return false;
            }

            let isMailValid = this.isValidMultiEmails(mailCell.value);

            if (isMailValid === true) {
                //mailCell.style.setProperty("background-color", "yellow", "important");
                mailCell.style.border = "";
                return true;
            } else {
                mailCell.style.border = "3px #ff690f  solid";
                //mailCell.style.backgroundColor = "red";
                //mailCell.style.setProperty("background-color", "red", "important");
                //alert(mailCell.style.backgroundColor);
                //alert(row.uid);
                return false;
            }
        }

        //private resizeGridHeight(): void {
        //    let uiGrid = document.getElementById(this.getDbGridId());
        //    if (this.isValueNullOrUndefined(uiGrid)) {
        //        return;
        //    }

        //    let uiRows = document.getElementsByClassName('ui-grid-row');
        //    if (this.isValueNullOrUndefined(uiRows)) {
        //        return;
        //    }

        //    let iHeight = 0;
        //    for (let i = 0; i < uiRows.length; i++) {
        //        var rect = uiRows[i].getBoundingClientRect();
        //        iHeight += (rect.height);

        //    }

        //    uiGrid.setAttribute("style", "height:" + iHeight + "px!important");


        //    //uiGrid.css('height', iHeight + 'px');

        //    //uiGrid.style.height = iHeight + 'px!important';
        //    //angular.element(document.getElementsByClassName('grid')[0]).css('height', iHeight + 'px!important');
        //}

        //************************************************************************

        //public getShortText(strText: string, iLenght : number): string {
        //    if (this.isStringValueNullOrEmpty(strText)) {
        //        return "";
        //    }

        //    if (iLenght == 0) {
        //        return strText;
        //    }

        //    var shortText: string = strText.substring(0, iLenght);
        //    shortText = shortText.replace("\r\n", " ");

        //    return shortText;
        //}

        //******************************************************************
    }

    export class UserGridSettings {
        public grid_name: string = null;
        public grid_page_size: number = null;
        public filter: string = null;
        public sort: string = null;
        public columns: string = null;
        public page_nr: number = null;
    }

    export class HideColIndex {
        public name: string = null;
        public position: number = null;
       
    }
}