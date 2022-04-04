/// <reference path="../RegetTypeScript/Base/reget-entity.ts" />

module Kamsyk.RegetApp {

    export class UsedPgController extends BaseRegetGridTranslationTs implements angular.IController {

        //**********************************************************************
        //Loc Texts
        private locActiveText: string = $("#ActiveText").val();
        private locPurchaseGroupText: string = $("#PurchaseGroupText").val();
        private locParentPgText: string = $("#ParentPgText").val();
        private locAreaText: string = $("#AreaText").val();
        private locCompanyText: string = $("#CompanyText").val();
        private locDuplicityPgText: string = $("#DuplicityPgNameText").val();
        private locDeletePgConfirmText: string = $("#DeletePgConfirmText").val();
        private locLoadingLocalizationText: string = $("#LoadingLocalizationText").val();
        //**********************************************************************

        //***********************************************************************
        // Properties

        private isPpgLoaded: boolean = false;
        private isUsedPgLoaded: boolean = false;
        private isCompanyLoaded: boolean = false;
        private isMissingPgTextAdded: boolean = false;
        private isMissingPpgTextAdded: boolean = false;
        private isGridHidden: boolean = true;
        private skipLoad: boolean = false;
        private parentPgs: ParentPgExtended[] = null;
        private parentPgId: number = null;
        private parentPgsDropDown: AgDropDown[] = null;

        //This one "uiGrid.ISelectOptio" does NOT work !!!!!! Default Filter is not set properly
        //private yesNo: Array<uiGrid.ISelectOption> = [{ value: "true", label: this.locYesText }, { value: "false", label: this.locNoText }];
        private yesNo: any[] = [{ value: true, label: this.locYesText }, { value: false, label: this.locNoText }];

        private usedPg: UsedPg = null;
        private companies: AgDropDown[] = null;
        private misPpgLocCount: number = 0;
        private misPgLocCount: number = 0;

        //***********************************************************************


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
            this.loadData();
        }
        //***************************************************************

        $onInit = () => { };

        //****************************************************************************
        //Abstract
        public locTranslationText: string = $("#TranslationText").val();
        public locMandatoryTextFieldText: string = $("#MandatoryTextFieldText").val();
        public isDefaultLang: boolean = ($("#IsDefaultLang").val() == "1");

        public exportToXlsUrl(): string {
            return this.getRegetRootUrl() + "Report/GetUsedPgReport?" +
                "filter=" + encodeURI(this.filterUrl) +
                "&sort=" + this.sortColumnsUrl +
                "&t=" + new Date().getTime();
        }

        public getControlColumnsCount(): number {
            return 2;
        }

        public getDuplicityErrMsg(usedPg: UsedPg): string {
            return this.locDuplicityPgText.replace("{0}", usedPg.pg_loc_name);
        }

        public getSaveRowUrl(): string {
            return this.getRegetRootUrl() + "ParentPg/SaveUsedPgData?t=" + new Date().getTime();
        }

        public insertRow(): void {
            //not used    
        }

        public isRowChanged(): boolean {

            if (this.editRow === null) {
                return true;
            }

            var origUsedPg: UsedPg = this.editRowOrig;
            var updUsedPg: UsedPg = this.editRow;

            if (origUsedPg.pg_loc_name !== updUsedPg.pg_loc_name) {
                return true;
            }

            if (origUsedPg.parent_pg_loc_name !== updUsedPg.parent_pg_loc_name) {
                return true;
            }
                        

            return false;
        }

        public isRowEntityValid(usedPg: UsedPg): string {

            if (this.isStringValueNullOrEmpty(usedPg.pg_loc_name)) {
                return this.locMissingMandatoryText;
            }

            if (this.isStringValueNullOrEmpty(usedPg.parent_pg_loc_name)) {
                return this.locMissingMandatoryText;
            }

            return null;
        }

        public loadGridData(): void {
            this.getUsedPgs();
        }

        public dbGridId: string = "grdUsedPg_rg";

        public getMsgDisabled(centre: Centre): string {
            return null;
        }


        public getMsgDeleteConfirm(usedPg: UsedPg): string {
            return this.locDeletePgConfirmText.replace("{0}", usedPg.pg_loc_name);;
        }

        public getErrorMsgByErrId(errId: number, msg: string): string {
            return this.locErrorMsgText;
        }

        public deleteUrl: string = this.getRegetRootUrl() + "PurchaseGroup/DeleteUsedPg" + "?t=" + new Date().getTime();

        public saveTranslationToDb(): void {
            try {
                this.hideTranslation();

                var modifUsedPg: UsedPg = angular.copy(this.usedPg);
                modifUsedPg.local_text[0].text = this.translateText1;
                if (modifUsedPg.local_text.length > 1) {
                    modifUsedPg.local_text[1].text = this.translateText2;
                }
                                
                var updTexts: any = {};
                updTexts.pgId = this.entityId;
                updTexts.localTexts = modifUsedPg.local_text;

                var jsonEntityData = JSON.stringify(updTexts);

                this.showLoaderBoxOnly(this.isError);
                this.$http.post(
                    this.getRegetRootUrl() + 'PurchaseGroup/SavePgTranslation',
                    jsonEntityData
                ).then((response) => {
                    try {
                        var result: ISaveDataResponse = response.data as ISaveDataResponse;
                        if (!this.isValueNullOrUndefined(result)
                            && !this.isValueNullOrUndefined(result.string_value)) {
                            
                            var errMsg: string = this.locErrorMsgText;
                            var splitIndex: number = result.string_value.indexOf(",");
                            if (result.string_value.indexOf(",") > -1) {
                                var errType: string = result.string_value.substring(0, splitIndex);
                                if (errType === "DUPLICITY") {
                                    var duplicityPpgName: string = result.string_value.substring(splitIndex + 1).trim();
                                    errMsg = this.locDuplicityPgText.replace("{0}", duplicityPpgName);
                                }
                            }

                            this.hideLoader();

                            this.displayErrorMsg(errMsg);
                            return;
                        }

                        this.usedPg.local_text[0].text = this.translateText1;
                        if (this.usedPg.local_text.length > 1) {
                            this.usedPg.local_text[1].text = this.translateText2;
                        }
                        //if (this.parentPgExtended.local_text.length > 2) {
                        //    this.parentPgExtended.local_text[2].text = this.translateText3;
                        //}

                        this.usedPg.pg_loc_name = this.usedPg.local_text[0].text;
                        if (!this.isValueNullOrUndefined(this.editRowOrig)) {
                            var origUsedPg: UsedPg = this.editRowOrig;
                            origUsedPg.pg_loc_name = this.usedPg.local_text[0].text;
                        }

                        this.hideLoaderWrapper();
                    } catch (e) {
                        this.hideLoader();
                        this.displayErrorMsg();
                    } finally {
                        this.hideLoaderWrapper();
                    }
                }, (response: any) => {
                    this.hideLoader();
                    this.displayErrorMsg();
                });
            } catch (e) {
                this.hideLoader();
                this.displayErrorMsg();
            }
            
        }

        public openTranslationDialog(row: any, col: any, event: MouseEvent): void {
            event.stopPropagation();

            this.usedPg = row.entity;
            if (this.isValueNullOrUndefined(this.usedPg.local_text) || this.usedPg.local_text.length == 0) {
                return;
            }

            //this.modifLocalTexts: LocalText[] = angular.copy(this.usedPg.local_text);

            this.entityId = this.usedPg.id;
            this.translateLabel1 = this.usedPg.local_text[0].label;
            this.translateFlagUrl1 = this.usedPg.local_text[0].flag_url;
            this.translateText1 = this.usedPg.local_text[0].text;
            
            if (!this.isDefaultLang) {
                this.translateLabel2 = this.usedPg.local_text[1].label;
                this.translateFlagUrl2 = this.usedPg.local_text[1].flag_url;
                this.translateText2 = this.usedPg.local_text[1].text;
                
            }
            
            this.displayGridTranslation(event.target);
        }

        public updateFirstLocalText(entity: any): void {
            var usedPg: UsedPg = entity;
            if (this.isValueNullOrUndefined(usedPg.local_text)
                || this.isValueNullOrUndefined(usedPg.local_text.length == 0)) {
                return;
            }
            usedPg.local_text[0].text = usedPg.pg_loc_name;
        }

        public getDbGridId(): string {
            return this.dbGridId;
        }
        //*****************************************************************************

        //***********************************************************************************
        //Http methods
        private getParentPgs() {
            this.isPpgLoaded = false;
            this.isUsedPgLoaded = false;
            this.isCompanyLoaded = false;
            this.isError = false;

            let divBox: any = $(".reget-se-pre-con");
            if ($(divBox).is(':visible') === false) {
                this.showLoaderBoxOnly(this.isError);
            }
            
            this.$http.get(
                this.getRegetRootUrl() + "ParentPg/GetParentPgs?t=" + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    this.parentPgs = tmpData;

                    //this.getUsedPgs();
                    this.getCompanies();

                    this.isPpgLoaded = true;

                    this.hideLoaderWrapper();
                } catch (e) {
                    this.hideLoader();
                    this.displayErrorMsg();
                } finally {
                    this.hideLoaderWrapper();
                }
            }, (response: any) => {
                this.hideLoader();
                this.displayErrorMsg();
            });
            
        }

        
        private getUsedPgs() : void {
           
            let divBox: any = $(".reget-se-pre-con");
            if ($(divBox).is(':visible') === false) {
                this.showLoaderBoxOnly(this.isError);
            }

            var strUrl = this.getRegetRootUrl() + "/ParentPg/GetUsedParentPgs?" +
                'filter=' + encodeURI(this.filterUrl) +
                '&pageSize=' + this.pageSize +
                '&currentPage=' + this.currentPage +
                '&sort=' + this.sortColumnsUrl +
                '&t=' + new Date().getTime();

            //if (!this.isValueNullOrUndefined(this.parentPgId) && this.parentPgId > -1) {
            //    strUrl += "&parentPgId=" + this.parentPgId;
            //}

            this.$http.get(
                strUrl,
                {}
            ).then((response) => {
                this.isGridHidden = false;

                try {
                    this.testLoadDataCount++;

                    var tmpData: any = response.data;
                    this.gridOptions.data = tmpData.db_data;
                    this.rowsCount = tmpData.rows_count;

                    if (!this.isValueNullOrUndefined(this.gridApi)) {
                        //active
                        var pos: number = this.getColumnIndex("active");
                        if (pos > 0) {
                            this.gridOptions.columnDefs[pos].filter = {
                                type: this.uiGridConstants.filter.SELECT,
                                selectOptions: this.yesNo

                            };
                            this.gridApi.core.notifyDataChange(this.uiGridConstants.dataChange.COLUMN);
                        }

                       
                        //parent pgs
                        if (this.parentPgsDropDown === null && !this.isValueNullOrUndefined(this.parentPgs)) {
                            this.parentPgsDropDown = [];
                            for (var i = 0; i < this.parentPgs.length; i++) {
                                var parentPgDropDown: AgDropDown = new AgDropDown();
                                parentPgDropDown.value = this.parentPgs[i].name;
                                parentPgDropDown.label = this.parentPgs[i].name;
                                this.parentPgsDropDown.push(parentPgDropDown);
                            }
                        }

                        var pos = this.getColumnIndex("parent_pg_loc_name");
                        if (pos > 0) {
                            this.gridOptions.columnDefs[pos].filter = {
                                type: this.uiGridConstants.filter.SELECT,
                                selectOptions: this.parentPgsDropDown

                            };
                            this.gridOptions.columnDefs[pos].editDropdownOptionsArray = this.parentPgsDropDown;
                            this.gridApi.core.notifyDataChange(this.uiGridConstants.dataChange.COLUMN);
                        }
                    }

                    if (this.isValueNullOrUndefined(this.parentPgId)) {
                        //this.loadGridSettings();
                        this.setGridSettingData();
                    } else {
                        //Set Filter Parent Pg
                        var ppg: ParentPgExtended[] = this.$filter("filter")(this.parentPgs, { id: this.parentPgId }, true);
                        if (!this.isValueNullOrUndefined(ppg)) {
                            angular.forEach(this.gridApi.grid.columns, (col: uiGrid.IColumnDefOf<any>) => {
                                if (col.name === "parent_pg_loc_name") {
                                    col.filters[0].term = ppg[0].name;
                                }
                            });
                        }
                    }
                    
                    //********************************************************************
                    //it is very important otherwise 50 lines are nod diplaye dproperly !!!
                    this.gridOptions.virtualizationThreshold = this.rowsCount + 1;
                    //********************************************************************

                    this.isUsedPgLoaded = true;

                    this.hideLoaderWrapper();
                } catch (e) {
                    this.hideLoader();
                    this.displayErrorMsg();
                } finally {
                    this.hideLoaderWrapper();
                }
            }, (response: any) => {
                this.hideLoader();
                this.displayErrorMsg();
            });
            
        }

        private getCompanies(): any {
            this.showLoader(this.isError);

            this.$http.get(
                this.getRegetRootUrl() + "Participant/GetParticipantCompanies?t=" + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    this.companies = tmpData;

                    //company
                    var pos: number = this.getColumnIndex("company_name");
                    if (pos > 0) {
                        this.gridOptions.columnDefs[pos].filter = {
                            type: this.uiGridConstants.filter.SELECT,
                            selectOptions: this.companies

                        };
                        //var filCompanies: uiGrid.edit.IEditDropdown[] = this.companies;
                        this.gridOptions.columnDefs[pos].editDropdownOptionsArray = this.companies;
                        this.gridApi.core.notifyDataChange(this.uiGridConstants.dataChange.COLUMN);
                    }

                    //this.getUsedPgs();
                    this.getLoadDataGridSettings();

                    this.isCompanyLoaded = true;
                    this.hideLoaderWrapper();
                } catch (e) {
                    this.hideLoader();
                    this.displayErrorMsg();
                } finally {
                    this.hideLoaderWrapper();
                }
            }, (response: any) => {
                this.hideLoader();
                this.displayErrorMsg();
            });

        }

        private getMissingPpgLocalTextCount(): void {
            try {
                this.hideTranslation();
                this.isMissingPpgTextAdded = false;

                this.showLoader(this.isError);
                this.$http.get(
                    this.getRegetRootUrl() + 'ParentPg/GetMissingParentPgLocalTextCount?t=' + new Date().getTime(),
                    {}
                ).then((response) => {
                    try {
                        var tmpData: any = response.data;
                        this.misPpgLocCount = tmpData.int_value;

                        if (this.misPpgLocCount == 0) {
                            this.isMissingPpgTextAdded = true;
                            this.getMissingPgLocalTextCount();
                        } else {
                            this.setMissingPpgLocalText(0);
                        }

                        this.hideLoaderWrapper();
                    } catch (e) {
                        this.hideLoader();
                        this.displayErrorMsg();
                    } finally {
                        this.hideLoaderWrapper();
                    }
                }, (response: any) => {
                    this.hideLoader();
                    this.displayErrorMsg();
                });
            } catch (e) {
                this.hideLoader();
                this.displayErrorMsg();
            }
        }

        private setMissingPpgLocalText(ppgIndex: number): void {
            try {
                this.hideTranslation();
                this.isMissingPpgTextAdded = false;

                this.showLoader(this.isError);

                let strCount: string = this.misPpgLocCount.toString();
                let index: number = ppgIndex + 1;
                if (index > this.misPpgLocCount) {
                    index = this.misPpgLocCount;
                }
                let strCountOutOf: string = index.toString();
                let loadingMsg: string = this.locLoadingLocalizationText.replace("##", strCount).replace("#", strCountOutOf);
                angular.element("#spanLoading").html(loadingMsg);
                this.$http.get(
                    this.getRegetRootUrl() + "ParentPg/SetMissingParentPgLocalText?ppgIndex=" + ppgIndex + "&t=" + new Date().getTime(),
                    {}
                ).then((response) => {
                    try {
                        if (ppgIndex < this.misPpgLocCount) {
                            ppgIndex++;
                            this.setMissingPpgLocalText(ppgIndex);
                        } else {
                            this.getMissingPgLocalTextCount();
                            this.isMissingPpgTextAdded = true;

                            this.hideLoaderWrapper();
                        }
                    } catch (e) {
                        this.hideLoader();
                        this.displayErrorMsg();
                    } finally {
                        this.hideLoaderWrapper();
                    }
                }, (response: any) => {
                    this.hideLoader();
                    this.displayErrorMsg();
                });
            } catch (e) {
                this.hideLoader();
                this.displayErrorMsg();
            }
        }

        private getMissingPgLocalTextCount(): void {
            try {
                this.hideTranslation();
                this.isMissingPgTextAdded = false;

                this.showLoader(this.isError);
                this.$http.get(
                    this.getRegetRootUrl() + 'ParentPg/GetMissingPgLocalTextCount?t=' + new Date().getTime(),
                    {}
                ).then((response) => {
                    try {
                        var tmpData: any = response.data;
                        this.misPgLocCount = tmpData.int_value;
                                                
                        if (this.misPgLocCount == 0) {
                            this.isMissingPgTextAdded = true;
                            this.getParentPgs();
                        } else {
                            this.setMissingPgLocalText(0);
                        }

                        this.hideLoaderWrapper();
                    } catch (e) {
                        this.hideLoader();
                        this.displayErrorMsg();
                    } finally {
                        this.hideLoaderWrapper();
                    }
                }, (response: any) => {
                    this.hideLoader();
                    this.displayErrorMsg();
                });
            } catch (e) {
                this.hideLoader();
                this.displayErrorMsg();
            }
        }

        private setMissingPgLocalText(pgIndex: number): void {
            try {
                this.hideTranslation();
                this.isMissingPgTextAdded = false;

                this.showLoader(this.isError);

                let strCount: string = this.misPgLocCount.toString();
                let index: number = pgIndex + 1;
                if (index > this.misPgLocCount) {
                    index = this.misPgLocCount;
                }
                let strCountOutOf: string = index.toString();
                let loadingMsg: string = this.locLoadingLocalizationText.replace("##", strCount).replace("#", strCountOutOf);
                angular.element("#spanLoading").html(loadingMsg);
                this.$http.get(
                    this.getRegetRootUrl() + "ParentPg/SetMissingPgLocalText?ppgIndex=" + pgIndex + "&t=" + new Date().getTime(),
                    {}
                ).then((response) => {
                    try {
                        if (pgIndex < this.misPgLocCount) {
                            pgIndex++;
                            this.setMissingPgLocalText(pgIndex);
                        } else {
                            this.getParentPgs();
                            this.isMissingPgTextAdded = true;

                            this.hideLoaderWrapper();
                        }
                    } catch (e) {
                        this.hideLoader();
                        this.displayErrorMsg();
                    } finally {
                        this.hideLoaderWrapper();
                    }
                }, (response: any) => {
                    this.hideLoader();
                    this.displayErrorMsg();
                });
            } catch (e) {
                this.hideLoader();
                this.displayErrorMsg();
            }
            //try {
            //    this.hideTranslation();
            //    this.isMissingPgTextAdded = false;

            //    this.showLoaderBoxOnly(this.isError);
            //    this.$http.get(
            //        this.getRegetRootUrl() + 'ParentPg/SetMissingPgLocalText?t=' + new Date().getTime(),
            //        {}
            //    ).then((response) => {
            //        try {

            //            this.getParentPgs();
            //            this.isMissingPgTextAdded = true;

            //            this.hideLoaderWrapper();
            //        } catch (e) {
            //            this.hideLoader();
            //            this.displayErrorMsg();
            //        } finally {
            //            this.hideLoaderWrapper();
            //        }
            //    }, (response: any) => {
            //        this.hideLoader();
            //        this.displayErrorMsg();
            //    });
            //} catch (e) {
            //    this.hideLoader();
            //    this.displayErrorMsg();
            //}
        }
        //***********************************************************************************


        //*************************************************************************************
        //Methods
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
                    name: "pg_loc_name", displayName: this.locPurchaseGroupText, field: "pg_loc_name",
                    enableHiding: false,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTranslationTextMandatoryTemplate.html",
                    enableCellEdit: false,
                    width: 350,
                    minWidth: 350
                },
                {
                    name: "parent_pg_loc_name", displayName: this.locParentPgText, field: "parent_pg_loc_name",
                    enableHiding: false,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellDropDownMandatoryTemplate.html",
                    enableCellEdit: false,
                    width: 250,
                    minWidth: 250
                },
                {
                    name: "centre_group_name", displayName: this.locAreaText, field: "centre_group_name",
                    enableHiding: false,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextTemplateReadOnly.html",
                    enableCellEdit: false,
                    width: 200,
                    minWidth: 200
                },
                {
                    name: "company_name", displayName: this.locCompanyText, field: "company_name",
                    enableHiding: false,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextTemplateReadOnly.html",
                    enableCellEdit: false,
                    width: 200,
                    minWidth: 200
                },
                {
                    name: "active", displayName: this.locActiveText, field: "active",
                    enableCellEdit: false,
                    enableHiding: false,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellCheckboxTemplate.html",
                    minWidth: 90,
                    width: 90
                }

            ];
        }

        private loadData(): void {
            this.getMissingPpgLocalTextCount();

            this.parentPgId = this.getUrlParamValueInt("ppgid");
           
            //var urlParams: UrlParam[] = this.getAllUrlParams();
            //if (!this.isValueNullOrUndefined(urlParams)) {
            //    for (var i: number = 0; i < urlParams.length; i++) {
            //        if (urlParams[i].param_name.toLowerCase() == "ppgid") {
            //            this.parentPgId = parseInt(urlParams[i].param_value);
            //        }
            //    }
            //}
        }

        private hideLoaderWrapper() {
            if (this.isError || (
                this.isPpgLoaded
                && this.isUsedPgLoaded
                && this.isCompanyLoaded
                && this.isMissingPpgTextAdded
                && this.isMissingPgTextAdded)) {
                this.hideLoader();
            }
        }

        private searchParentPg(strName: string): ParentPgExtended[] {
            return this.filterParentPg(strName);
        }

        private filterParentPg(name: string): ParentPgExtended[]{
            
            var searchParentPgs: ParentPgExtended[] = [];

            if (this.isStringValueNullOrEmpty(name)) {
                return this.parentPgs;
            }

            angular.forEach(this.parentPgs,  (parentPg : ParentPgExtended) => {
                if (!this.isStringValueNullOrEmpty(parentPg.name) && parentPg.name.toLowerCase().indexOf(name.toLowerCase()) > -1 ||
                    !this.isStringValueNullOrEmpty(parentPg.name_wo_diacritics) && parentPg.name_wo_diacritics.trim().toLowerCase().indexOf(name.toLowerCase()) > -1) {
                    searchParentPgs.push(parentPg);
                }

            });
            
            return searchParentPgs;
        }

                
        //*************************************************************************************

    }
    angular.
        module("RegetApp").
        controller("UsedPgController", Kamsyk.RegetApp.UsedPgController);
}