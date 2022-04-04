/// <reference path="../typings/ui-grid/ui-grid.d.ts" />
/// <reference path="../RegetTypeScript/Base/reget-base-grid.ts" />
/// <reference path="../RegetTypeScript/Base/reget-entity.ts" />

/// <reference path="../typings/moment/moment.d.ts" />

module Kamsyk.RegetApp {
    export class RequestListController extends BaseRegetGridTs implements angular.IController {
        //****************************************
        //Abstract properties
        public dbGridId: string = "grdRequestList_rg";
        //*****************************************

        //***********************************************************************
        //Properties
        private requestData: Request[] = null;
        //************************************************************************

        //*****************************************************************
        //Localized Texts
        private locRequestNrText: string = $("#RequestNrText").val();
        private locRequestorText: string = $("#RequestorText").val();
        private locCentreText: string = $("#CentreText").val();
        private locPurchaseGroupText: string = $("#PurchaseGroupText").val();
        private locCreatedText: string = $("#CreatedText").val();
        //****************************************************************

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
            this.loadInit();
  
        }
        //***************************************************************

        $onInit = () => { };

        //********************************************************************
        //Abstract Methods
        public isRowChanged(): boolean {
            return false;
        }

        public insertRow(): void { }

        public exportToXlsUrl(): string {
            return this.getRegetRootUrl() + 'Report/GetRequestListReport?filter=' + encodeURI(this.filterUrl) +
                '&sort=' + this.sortColumnsUrl +
                '&t=' + new Date().getTime();
        }

        public isRowEntityValid(rowEntity: any): string {
            return null;
        }

        public getSaveRowUrl(): string {
            return null;
        }

        public getDuplicityErrMsg(rowEntity: any): string {
            return null;
        }

        public getControlColumnsCount(): number {
            return 2;
        }

        public getMsgDisabled(userSubstitutionEntity: UserSubstitution): string {
            return null;
        }


        public getMsgDeleteConfirm(userSubstitutionEntity: UserSubstitution): string {
            return null;
        }

        public loadGridData(): void {
            this.getRequestListData();
        }

        public getErrorMsgByErrId(errId: number, msg: string): string {
            return null;
        }

        public deleteUrl: string = null;

        public getDbGridId(): string {
            return this.dbGridId;
        }

        
        //************************************************************************

        //******************** Http Methods ****************************************
        private getRequestListData(): void {
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

            this.$http.get(
                this.getRegetRootUrl() + "Request/GetRequestList?" +
                "filter=" + encodeURI(this.filterUrl) +
                "&pageSize=" + this.pageSize +
                "&pageNr=" + this.currentPage +
                "&sort=" + this.sortColumnsUrl +
                "&t=" + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpRes: any = response
                   // this.formatGridDate(tmpRes.data.user_substitution.db_data);
                    var tmpData: any = response.data;
                    this.formatGridDates(tmpData.db_data);
                    this.requestData = tmpData.db_data;

                    for (let i = 0; i < this.requestData.length; i++) {
                        let smRequest: RequestSm = new RequestSm();
                        smRequest.request_nr = this.requestData[i].request_nr;
                        smRequest.requestor_name_surname_first = this.requestData[i].requestor_name_surname_first;
                        this.requestData[i].request_sm = smRequest;
                    }

                    this.gridOptions.data = this.requestData;
                    this.rowsCount = tmpData.rows_count;
                                        
                    if (this.rowsCount == 0) {
                        this.currentPage = 0;
                    }

                    if (!this.isValueNullOrUndefined(this.gridApi)) {
                        this.gridApi.core.notifyDataChange(this.uiGridConstants.dataChange.COLUMN);
                    }

                    

                    this.setGridSettingData();
                    
                    //********************************************************************
                    //it is very important otherwise 50 lines are not diplayed properly !!!
                    this.gridOptions.virtualizationThreshold = this.rowsCount + 1;
                    //********************************************************************

                    //this.isSubstLoaed = true;
                    this.testLoadDataCount++;
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
        //**************************************************************************

        //*************************** Methods ************************************
        private loadInit(): void {
            this.getLoadDataGridSettings();
            //this.loadGridData();
        }

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
                    //headerCellClass: (grid, row, col, rowRenderIndex, colRenderIndex) => {
                    //    return this.hiddenXs();
                    //},
                    //cellClass: (grid, row, col, rowRenderIndex, colRenderIndex) => {
                    //    return this.hiddenXs();
                    //},
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
                    //headerCellClass: (grid, row, col, rowRenderIndex, colRenderIndex) => {
                    //    return this.hiddenXs();
                    //},
                    //cellClass: (grid, row, col, rowRenderIndex, colRenderIndex) => {
                    //    return this.hiddenXs();
                    //},
                },
                {
                    name: "centre_name", displayName: this.locCentreText, field: "centre_name",
                    enableHiding: false,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextTemplateReadOnly.html",
                    enableCellEdit: false,
                    width: 100,
                    minWidth: 100,
                    //headerCellClass: (grid, row, col, rowRenderIndex, colRenderIndex) => {
                    //    return this.hiddenXs();
                    //},
                    //cellClass: (grid, row, col, rowRenderIndex, colRenderIndex) => {
                    //    return this.hiddenXs();
                    //},
                },
                {
                    name: "pg_name", displayName: this.locPurchaseGroupText, field: "pg_name",
                    enableHiding: false,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextTemplateReadOnly.html",
                    enableCellEdit: false,
                    width: 180,
                    minWidth: 100,
                    //headerCellClass: (grid, row, col, rowRenderIndex, colRenderIndex) => {
                    //    return this.hiddenXs();
                    //},
                    //cellClass: (grid, row, col, rowRenderIndex, colRenderIndex) => {
                    //    return this.hiddenXs();
                    //},
                },
                {
                    name: "issued", displayName: this.locCreatedText, field: "issued",
                    enableHiding: false,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellDateReadOnlyTemplate.html",
                    enableCellEdit: false,
                    width: 100,
                    minWidth: 100,
                    //headerCellClass: (grid, row, col, rowRenderIndex, colRenderIndex) => {
                    //    return this.hiddenXs();
                    //},
                    //cellClass: (grid, row, col, rowRenderIndex, colRbenderIndex) => {
                    //    return this.hiddenXs();
                    //},
                },
                //{
                //    name: "request", displayName: this.locCreatedText, field: "request_sm",
                //    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellRequest.html",
                //    enableCellEdit: false,
                //    enableFiltering: false,
                //    enableSorting: false,
                //    enableHiding: false,
                //    width: 100,
                //    minWidth: 100,
                //    //headerCellClass: (grid, row, col, rowRenderIndex, colRenderIndex) => {
                //    //    return this.hiddenXs();
                //    //},
                //    //cellClass: (grid, row, col, rowRenderIndex, colRenderIndex) => {
                //    //    return this.hiddenXs();
                //    //},
                //}
            ];

        }

        private gridRowDetail(rowEntity: UserSubstitution): void {
            
            let urlParAddSubstVisible: string = "newSubstDisplayed=0";
            if ($("#divAddSubstContent").is(':visible')) {
                urlParAddSubstVisible = "newSubstDisplayed=1";
            }
                        
            let url: string = this.getRegetRootUrl() + "Request?id=" + rowEntity.id;

            window.open(url, '_blank');
            //window.location.href = url;
        }

        private formatGridDates(gridData: any[]): void {
            if (this.isValueNullOrUndefined(gridData)) {
                return;
            }

            for (var i = 0; i < gridData.length; i++) {
                let jsonDateFrom = gridData[i].issued;
                let jDateFrom = this.convertJsonDate(jsonDateFrom);
                gridData[i].issued = jDateFrom;

                //let jsonDateTo = gridData[i].substitute_end_date;
                //let jDateTo = this.convertJsonDate(jsonDateTo);
                //gridData[i].substitute_end_date = jDateTo;

                //let jsonModifyDate = gridData[i].modified_date;
                //let jModifiedDate = this.convertJsonDate(jsonModifyDate);
                //gridData[i].modified_date = jModifiedDate;


            }
        }

        //private hiddenXs(): string {
        //     return "hidden-xs"
        //}
        //************************************************************************

    }
    angular.
        module('RegetApp').
        controller('RequestListController', Kamsyk.RegetApp.RequestListController).
        config(function ($mdDateLocaleProvider) { //only because of Date picker is implemented
            this.SetDatePicker($mdDateLocaleProvider);
            //it is neccessary to set IsGenerateDatePickerLocalization = true
        });
}