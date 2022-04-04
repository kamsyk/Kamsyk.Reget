
import { KamsykBaseTs } from "../KamsykTypeScript/kamsyk-base";

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
        string_value : string
    } 

    export interface IGridEntity {
        row_index: number;
    } 

export abstract class KamsykBaseGridTs extends KamsykBaseTs {
        public pageSize: number = 10;
        public pageSizeGrid: number = 10 + 1; //because of new row
        public currentPage: number = 1;
        public sortColumnsUrl: string = "";
        public filterUrl: string = "";
        public gridOptions: uiGrid.IGridOptions = {};
        public gridApi: uiGrid.IGridApi = null;
        public rowsCount: number = 0;
        public lang: string = "en";
        public editRow : any = null;
        public editRowOrig : any = null;
        public editRowIndex : number = -1;
        public newRowIndex: number = -1;
        public isNewRow: boolean = false;
        public gridFilter: IGridFilter[];
        public gridSort: IGridSort[];
        //public dbGridId: string = null;
        public userGridSettings: UserGridSettings = null;
        private isGridSettingsSaved: boolean = false;
        public defaultColumnDefs: uiGrid.IColumnDefOf<any>[] = null;
        public isSkipLoad: boolean = false;
                
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
            
        ) {
            super($scope, $http, $filter, $mdDialog, $mdToast, $q, $timeout);
            this.lang = $("#GridLangCode").val();
            this.setGridOptionBase();
            this.defaultColumnDefs = angular.copy(this.gridOptions.columnDefs);
            
        }
        //******************************************************************

        //********************************************************************
        //abstract methods
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

        abstract dbGridId: string = null;
        abstract deleteUrl: string = null;
        
        //********************************************************************

        //********************************************************************
        //Loc Texts
        public locMissingMandatoryText = $("#MissingMandatoryText").val();
        public locDateTimePickerFormatText = $("#DateTimePickerFormatText").val();
        public locConfirmationText = $("#ConfirmationText").val();
        public locYesText: string = $("#YesText").val();
        public locNoText: string = $("#NoText").val();
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
                this.gridApi.core.on.sortChanged(this.$scope, (grid : any, sortColumns : any[]) => {
                    if (this.isSkipLoad) {
                        return;
                    }

                    if (!this.gridSaveRow()) {
                        return;
                    }

                    this.currentPage = 1;

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

                    //console.log("participant load debounce");
                    if (this.isSkipLoad) {
                        return;
                    }

                    if (!this.gridSaveRow()) {
                        return;
                    }


                    this.currentPage = 1;

                    var strFilter = this.filterUrl;
                    this.filterUrl = this.getFilterUrl();

                    if (strFilter !== this.filterUrl) {
                        this.loadGridData();
                    }

                    
                }, 500));
           }
      
        }

        private getFilterUrl() {
            if (this.isValueNullOrUndefined(this.gridApi)) {
                return '';
            }

            //Filter
            var filter = '';
            angular.forEach(this.gridApi.grid.columns, (col: uiGrid.IColumnDefOf<any>) => {
                if (col.enableFiltering && col.filters[0].term !== null && col.filters[0].term !== undefined
                    && (col.filters[0].term !== '')) {
                    if (filter.length > 0) {
                        filter += this.urlParamDelimiter;
                    }
                    filter += col.name + this.urlParamValueDelimiter + col.filters[0].term;
                }
            });

            return filter;
        }
               
        public getLastPageIndex(): number {
            if (this.rowsCount < (this.pageSize * (this.currentPage - 1) + 1)) {
                if (this.currentPage > 1) {
                    this.currentPage--;
                }
            }

            var iLastPageIndex = Math.ceil(this.rowsCount / this.pageSize);

            return iLastPageIndex;
        }

        public nextPage() : void {
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

        public isLastPage() : boolean {

            var tmpCurrentPage = this.currentPage + 1;
            if ((tmpCurrentPage * this.pageSize) >= (this.rowsCount + this.pageSize)) {
                return true;
            }
            
            return false;
        }

        public isFirstPage() : boolean {
        var tmpCurrentPage = this.currentPage - 1;
            if (tmpCurrentPage < 1) {
                return true;
            }

            return false;
        }

        public gotoPage() : void {
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
        }

        public firstPage() : void {
            if (!this.gridSaveRow()) {
                return;
            }

            this.currentPage = 1;
            this.loadGridData();
        }

        public lastPage() : void {
            if (!this.gridSaveRow()) {
                return;
            }
            
            this.currentPage = this.getLastPageIndex();
            this.loadGridData();
        };

        public getDisplayItemsToInfo() : number {
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
                
        public refresh() : void {
            this.loadGridData();
            
            this.editRow = null;
            this.editRowOrig = null;
            this.editRowIndex = null;
            this.newRowIndex = null;
            this.isNewRow = false;
        }

        public cellClicked(row? : any, col? : any): void {
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

        public cancelEditRow(editRow : any) : any {
            if (!this.isValueNullOrUndefined(editRow)) {
                editRow.editrow = false;
                editRow = null;
            }

            return editRow;
        }

        public gridSaveRow() : boolean {
            
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
                for (var i = 0; i < this.gridFilter.length; i++) {
                    angular.forEach(this.gridApi.grid.columns, (col: uiGrid.IColumnDefOf<any>) => {
                        if (col.name === this.gridFilter[i].column_name) {
                            col.filters[0].term = this.gridFilter[i].filter_value;
                        }
                    });
                }
            }
        }

        private saveGridRowtoDb(rowEntity: any): boolean {
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
                    var result: ISaveDataResponse = response.data as ISaveDataResponse;
                    var iId: number = result.int_value;

                    if (iId === -1 && !this.isValueNullOrUndefined(response.data) && result.string_value === "DUPLICITY") {
                        this.hideLoader();
                        var errMsg : string = this.getDuplicityErrMsg(rowEntity);
                        if (this.isValueNullOrUndefined(errMsg)) {
                            errMsg = "Duplicity values";
                        } 
                        this.displayErrorMsg(errMsg);
                        return;
                    } else if (iId === -1) {
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

        private setDataGridSettings() : void {
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
                    this.gridOptions.columnDefs = angular.copy(this.defaultColumnDefs);
                    this.clearFilters();
                    this.clearSort();
                    this.isSkipLoad = false;

                    this.userGridSettings = null;
                    this.loadGridData();

                    this.hideLoader();
                } catch (ex) {
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

        public getDataGridSettings () : void {
            this.showLoader();

            this.$http.get(
                this.getRegetRootUrl() + 'DataGrid/GetUserGridSettings?gridId=' + this.dbGridId + '&t=' + new Date().getTime(),
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

                        var grdColumns : uiGrid.IColumnDefOf<any>[] = this.getGridColumnsFromUrlString(this.userGridSettings.columns);
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

        public addNewRow() : void{

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

        private getNewRowIndex() : number {
            var newRowIndex = this.pageSize;
            var grid: any = this.gridApi.grid;
            var gridRowsCount = grid.rows.length;
            if (gridRowsCount < this.pageSize) {
                newRowIndex = gridRowsCount;
            }
            if (newRowIndex < 0) {
                newRowIndex = 0;
            }

            return newRowIndex;
        }

        private enableFiltering() : void{
            this.isSkipLoad = true;
            for (var i = this.getControlColumnsCount(); i < this.gridOptions.columnDefs.length; i++) {
                this.gridOptions.columnDefs[i].enableFiltering = true;
                this.gridApi.core.notifyDataChange(this.uiGridConstants.dataChange.COLUMN);
            }
            this.isSkipLoad = false;
        }

        public pageSizeChanged() : void {
            if (!this.gridSaveRow()) {
                return;
            }

            this.currentPage = 1;
            this.pageSizeGrid = this.pageSize + 1;
            this.gridOptions.paginationPageSize = this.pageSizeGrid;
            this.loadGridData();
        };

        public exportToXls() : void {
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

        public hideEditGridButton(rowEntity : any): boolean {
            return false;
        }

        public hideDeleteGridButton(rowEntity: any): boolean {
            return false;
        }

       public  getColumnIndex(strColumnName) : number {
            for (var i = 0; i < this.gridApi.grid.columns.length; i++) {
                if (this.gridApi.grid.columns[i].name === strColumnName) {
                    return i;
                }
            }

            return -1;
        }

        //*******************************************
        //Save Grid Setting
        public saveGridSettings() : void{
            this.setGridSettings();
            this.setDataGridSettings();
        }

        private setGridSettings() : void {
            this.userGridSettings = new UserGridSettings();

            //Grid Name
            this.userGridSettings.grid_name = this.dbGridId;

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

        private isColumnOrderChanged(currentColumnsString: string, origColumns: uiGrid.IColumnDefOf<any>[]) : boolean {
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

        public clearFilters() : void {
            this.gridFilter = [];
                        
            angular.forEach(this.gridApi.grid.columns, (col : uiGrid.IColumnDefOf<any>) => {
                if (col.enableFiltering && (!this.isValueNullOrUndefined(col.filters[0].term))) {
                    var tmpFilter: any = {};
                    var tmpFilterGrid: IGridFilter = tmpFilter;
                    tmpFilterGrid.column_name = col.name;
                    tmpFilterGrid.filter_value = col.filters[0].term;

                    this.gridFilter.push(tmpFilterGrid);
                    col.filters[0].term = '';
                }
            });
        }

        private clearSort() : void {
            //var isFilter: boolean = false;
            angular.forEach(this.gridApi.grid.columns, (col: any) => {
                col.unsort();
                //col.sortCellFiltered = false;
            });

            //return isFilter;
        }

        private getGridFilterFromUrlString(strUrlFilter: string): IGridFilter[] {
            var gridFilter : IGridFilter[] = [];

            if (!this.isStringValueNullOrEmpty(strUrlFilter)) {
                var filterItems : string[] = strUrlFilter.split(this.urlParamDelimiter);
                for (var i = 0; i < filterItems.length; i++) {
                    var itemFields : string[] = filterItems[i].split(this.urlParamValueDelimiter);
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
            if (this.isValueNullOrUndefined(gridColumns)) {
                return;
            }

            for (var i: number = 0; i < gridColumns.length; i++) {
                if (gridColumns[i].visible === false) {
                    for (var j: number = this.gridOptions.columnDefs.length - 1; j >= 0; j--) {
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

            for (var j: number = this.gridOptions.columnDefs.length - 1; j >= 0; j--) {
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
        }

        private setColumnsOrder(gridColumns: uiGrid.IColumnDefOf<any>[]) {
            var orderedColumns: uiGrid.IColumnDefOf<any>[] = [];
            var isReordered: boolean = false;
            var colIndex: number = 0;

            for (var i = 0; i < gridColumns.length; i++) {
                if (gridColumns[i].visible === false && gridColumns[i].name !== "id") {
                    continue;
                }

                var col = angular.copy(this.gridOptions.columnDefs[i]);

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

                colIndex++;
            }

            if (isReordered) {
                this.gridOptions.columnDefs = orderedColumns;
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

        public getNewRowIndexColValue() : number {
            var rowIndex = (this.currentPage - 1) * this.pageSize + this.gridOptions.paginationPageSize;
            if (rowIndex > this.rowsCount + 1) {
                rowIndex = this.rowsCount + 1;
            }

            return rowIndex;
        }

        public removeRowFromArray(entity : any) {
            var tmpCurrPage = this.currentPage;

            var tmpData: any = this.gridOptions.data;

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

        private gridDeleteRowFromDb(entity: any, ev: MouseEvent) {
            this.showLoaderBoxOnly(this.isError);

            var jsonSupplierData = JSON.stringify(entity);

            this.$http.post(
                this.deleteUrl,
                //this.getRegetRootUrl() + 'Supplier/DeleteSupplier' + '?t=' + new Date().getTime(),
                jsonSupplierData
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    var strResult = tmpData.string_value;

                    if (this.isStringValueNullOrEmpty(strResult)) {
                        this.removeRowFromArray(entity);

                    } else if (strResult === "disabled") {
                        entity.active = false;
                        //var msgDisabled = this.locSupplierWasDisabledText.replace("{0}", supplier.supp_name);
                        this.showAlert(this.locConfirmationText, this.getMsgDisabled(entity), this.locCloseText);
                    }

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

        public gridDeleteRow(entity: any, ev: MouseEvent) {
            //var strMessage = this.locDeleteSupplierConfirmText.replace("{0}", supplier.supp_name);
            var confirm = this.$mdDialog.confirm()
                .title(this.locConfirmationText)
                .textContent(this.getMsgDeleteConfirm(entity))
                .ariaLabel("DeleteRowConfirm")
                .targetEvent(ev)
                .ok(this.locNoText)
                .cancel(this.locYesText);

            this.$mdDialog.show(confirm).then(() => {

            }, () => {
                this.gridDeleteRowFromDb(entity, ev);
            });

        }
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
    }

    
