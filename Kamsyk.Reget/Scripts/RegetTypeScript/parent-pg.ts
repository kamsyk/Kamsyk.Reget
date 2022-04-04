module Kamsyk.RegetApp {
    
    export class ParentPgController extends BaseRegetGridTranslationTs implements angular.IController {

        //*************************************************************
        //Get Loc Texts
        private locWarningText: string = $("#WarningText").val();
        private locErrMsg: string = $("#ErrMsgText").val();
        private locParentPgUsedText: string = $("#ParentPgUsedText").val();
        private locParentPgUsedCompsText: string = $("#ParentPgUsedCompsText").val();
        private locNameText: string = $("#NameText").val();
        private locDuplicityParentPgText: string = $("#DuplicityParentPgNameText").val();
        //private locTranslationText: string = $("#TranslationText").val();
        private locDeleteParentPgConfirmText: string = $("#DeleteParentPgConfirmText").val();
        private locUsedPgText: string = $("#ConnectedPurchaseGroupsText").val();
        private locLoadingLocalizationText: string = $("#LoadingLocalizationText").val();
        
        //**********************************************************

        //**************************************************************
        private isPgLoaded: boolean = false;
        private isMissinTextAdded: boolean = false;
        private companyNamePrefix: string = "companyName_";
        //private isMaxCompIdLoaded: boolean = false;
        //private maxCompId: number = -1;
        private yesNo: any[] = [{ value: true, label: this.locYesText }, { value: false, label: this.locNoText }];
        private parentPgExtended: ParentPgExtended = null;
        private misPpgLocCount: number = 0;
        private isCompanyColumnsAdded: boolean = false;
        //**************************************************************

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
            return this.getRegetRootUrl() + "Report/GetParentPgReport?" +
                "filter=" + encodeURI(this.filterUrl) +
                "&sort=" + this.sortColumnsUrl +
                "&t=" + new Date().getTime();
            
        }

        public getControlColumnsCount(): number {
            return 3;
        }

        public getDuplicityErrMsg(rowEntity: ParentPgExtended): string {
            return this.locDuplicityParentPgText.replace("{0}", rowEntity.name);
        }

        public getSaveRowUrl(): string {
            return this.getRegetRootUrl() + "ParentPg/SaveParentPgData?t=" + new Date().getTime();
        }

        public insertRow(): void {
            var newParentPg: ParentPgExtended = new ParentPgExtended();

            newParentPg.id = -10;
            newParentPg.name = "";
            
            this.insertBaseRow(newParentPg);
            
        }

        public isRowChanged(): boolean {

            if (this.editRow === null) {
                return true;
            }

            var origParentPg: ParentPgExtended = this.editRowOrig;
            var updParentPg: ParentPgExtended = this.editRow;
            var tmpParentPgs: any = this.gridOptions.data;
            var parentPgs: ParentPgExtended[] = tmpParentPgs;
            var isChanged: boolean = false;

            var id: number = updParentPg.id;
            var parentPg: ParentPgExtended[] = this.$filter("filter")(parentPgs, { id: id }, true);


            if (id < 0) {
                //new row
                this.newRowIndex = null;

                return true;
            } else {
                if (origParentPg.name !== updParentPg.name) {
                    return true;
                } 

                if (this.isValueNullOrUndefined(origParentPg.selected_companies)
                    && !this.isValueNullOrUndefined(updParentPg.selected_companies)) {
                    return true;
                }

                if (!this.isValueNullOrUndefined(origParentPg.selected_companies)
                    && this.isValueNullOrUndefined(updParentPg.selected_companies)) {
                    return true;
                }

                if (!this.isValueNullOrUndefined(origParentPg.selected_companies)
                    && !this.isValueNullOrUndefined(updParentPg.selected_companies)) {
                    if (origParentPg.selected_companies.length != updParentPg.selected_companies.length) {
                        return true;
                    }

                    for (var i: number = 0; i < origParentPg.selected_companies.length; i++) {
                        if (origParentPg.selected_companies[i] !== updParentPg.selected_companies[i]) {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public isRowEntityValid(parentPg: ParentPgExtended): string {

            if (this.isStringValueNullOrEmpty(parentPg.name)) {
                return this.locMissingMandatoryText;
            }
                        
            return null;
        }

        public loadGridData(): void {
            this.getParentPgs();
        }

        public dbGridId: string = "grdParentPg_rg";

        public getMsgDisabled(parentPg: ParentPgExtended): string {
            return null;
        }


        public getMsgDeleteConfirm(parentPg: ParentPgExtended): string {
            return this.locDeleteParentPgConfirmText.replace("{0}", parentPg.name);
        }

        public deleteUrl: string = this.getRegetRootUrl() + "ParentPg/DeleteParentPg" + "?t=" + new Date().getTime();
        
        public saveTranslationToDb(): void {
            try {
                this.hideTranslation();

                var modifPg: ParentPgExtended = angular.copy(this.parentPgExtended);
                modifPg.local_text[0].text = this.translateText1;
                if (modifPg.local_text.length > 1) {
                    modifPg.local_text[1].text = this.translateText2;
                }

                var updTexts: any = {};
                updTexts.parentPgId = this.entityId;
                updTexts.localTexts = modifPg.local_text;

                var jsonEntityData = JSON.stringify(updTexts);

                this.showLoaderBoxOnly(this.isError);
                this.$http.post(
                    this.getRegetRootUrl() + 'ParentPg/SaveParentPgTranslation',
                    jsonEntityData
                ).then((response) => {
                    try {
                        var result: ISaveDataResponse = response.data as ISaveDataResponse;
                        if (!this.isValueNullOrUndefined(result)
                            && !this.isValueNullOrUndefined(result.string_value)) {
                            //&& result.string_value[] === "DUPLICITY") {
                            var errMsg: string = this.locErrorMsgText;
                            var splitIndex: number = result.string_value.indexOf(",");
                            if (result.string_value.indexOf(",") > -1) {
                                var errType: string = result.string_value.substring(0, splitIndex);
                                if (errType === "DUPLICITY") {
                                    var duplicityPpgName: string = result.string_value.substring(splitIndex + 1).trim();
                                    errMsg = this.locDuplicityParentPgText.replace("{0}", duplicityPpgName);
                                }
                            }

                            this.hideLoader();

                            this.displayErrorMsg(errMsg);
                            return;
                        }

                        //this.parentPgExtended = angular.copy(modifPg);

                        this.parentPgExtended.local_text[0].text = this.translateText1;
                        if (this.parentPgExtended.local_text.length > 1) {
                            this.parentPgExtended.local_text[1].text = this.translateText2;
                        }
                        //if (this.parentPgExtended.local_text.length > 2) {
                        //    this.parentPgExtended.local_text[2].text = this.translateText3;
                        //}

                        this.parentPgExtended.name = this.parentPgExtended.local_text[0].text;
                        if (!this.isValueNullOrUndefined(this.editRowOrig)) {
                            var origParentPg: ParentPgExtended = this.editRowOrig;
                            origParentPg.name = this.parentPgExtended.local_text[0].text;
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

            this.parentPgExtended = row.entity;
            if (this.isValueNullOrUndefined(this.parentPgExtended.local_text) || this.parentPgExtended.local_text.length == 0) {
                return;
            }

            this.entityId = this.parentPgExtended.id;
            this.translateLabel1 = this.parentPgExtended.local_text[0].label;
            this.translateFlagUrl1 = this.parentPgExtended.local_text[0].flag_url;
            this.translateText1 = this.parentPgExtended.local_text[0].text;
            //var txtParentPgTrans1: HTMLInputElement = <HTMLInputElement>document.getElementById("txtParentPgTrans1");
            //txtParentPgTrans1.value = parentPgExtended.local_text[0].text;
            //if (this.parentPgExtended.local_text.length > 1) {
            if (!this.isDefaultLang) {
                this.translateLabel2 = this.parentPgExtended.local_text[1].label;
                this.translateFlagUrl2 = this.parentPgExtended.local_text[1].flag_url;
                this.translateText2 = this.parentPgExtended.local_text[1].text;
            }
            
            this.displayGridTranslation(event.target);
  
        }

        public updateFirstLocalText(entity: any): void {
            var parentPg: ParentPgExtended = entity;
            if (this.isValueNullOrUndefined(parentPg.local_text)
                || this.isValueNullOrUndefined(parentPg.local_text.length == 0)) {
                return;
            }
            parentPg.local_text[0].text = parentPg.name;
        }

        public getErrorMsgByErrId(errId: number, msg: string): string {
            return this.locErrorMsgText;
        }

        public getDbGridId(): string {
            return this.dbGridId;
        }
        //*****************************************************************************

        //*****************************************************************************
        //Overwritten
        public gridEditRow(rowEntity: any): void {
            this.hideTranslation();
            super.gridEditRow(rowEntity);
        }

        public gridDeleteRow(entity: ParentPgExtended, ev: MouseEvent) {
            if (!this.isValueNullOrUndefined(entity.selected_companies)
                && entity.selected_companies.length > 0) {
                this.displayErrorMsg(this.locParentPgUsedText);
            } else {
                //check whether pg is used in companies out of grid, in companies where user is not admin
                try {

                    this.$http.get(
                        this.getRegetRootUrl() + "ParentPg/IsCanParentPgCanBeDeleted?ppgId=" + entity.id + "&t=" + new Date().getTime(),
                        {}
                    ).then((response) => {
                        var tmpData: any = response.data;
                        var usedComps: string[] = tmpData;
                        if (this.isValueNullOrUndefined(usedComps) || usedComps.length == 0) {
                            super.gridDeleteRow(entity, ev);
                        } else {
                            let strComps: string = "";
                            for (let i: number = 0; i < usedComps.length; i++) {
                                if (strComps.length > 100) {
                                    break;
                                }
                                if (!this.isStringValueNullOrEmpty(strComps)) {
                                    strComps += ", ";
                                }

                                strComps += usedComps[i];
                            }
                            let strMsg = this.locParentPgUsedCompsText + " " + strComps;
                            this.displayErrorMsg(strMsg);
                        }

                        this.hideLoader();
                    }, (response: any) => {
                        this.hideLoader();
                        this.displayErrorMsg();
                    });

                    
                } catch (e) {
                    this.hideLoader();
                    this.displayErrorMsg();
                }
              
            }
        }
                        
        //*****************************************************************************

        //*****************************************************************************
        //Http Methods

        private getParentPgs(): void {
            try {
                this.hideTranslation();
                this.isPgLoaded = false;
                                
                let divBox: any = $(".reget-se-pre-con");
                if ($(divBox).is(':visible') === false) {
                    this.showLoaderBoxOnly(this.isError);
                }

                this.$http.get(
                    this.getRegetRootUrl() + 'ParentPg/GetParentPgsAdmin?filter=' + encodeURI(this.filterUrl) +
                    '&pageSize=' + this.pageSize +
                    '&currentPage=' + this.currentPage +
                    '&sort=' + this.sortColumnsUrl +
                    '&t=' + new Date().getTime(),
                    {}
                ).then((response) => {
                    try {
                        //because of test, this is exception ParenPg is loaded twice
                        this.testLoadDataCount++;

                        var tmpData: any = response.data;
                        this.gridOptions.data = tmpData.db_data;
                        this.rowsCount = tmpData.rows_count;

                        if (!this.isCompanyColumnsAdded) {
                            this.setCompanyColumns(tmpData.db_data);
                            this.isCompanyColumnsAdded = true;
                        }
                        
                        this.loadGridSettings();
                        //this.setGridSettingData();

                        
                        //********************************************************************
                        //it is very important otherwise 50 lines are nod diplayed properly !!!
                        this.gridOptions.virtualizationThreshold = this.rowsCount + 1;
                        //********************************************************************

                        this.isPgLoaded = true;

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

        private getMissingLocalTextCount(): void {
            try {
                this.hideTranslation();
                this.isMissinTextAdded = false;

                this.showLoader(this.isError);
                this.$http.get(
                    this.getRegetRootUrl() + 'ParentPg/GetMissingParentPgLocalTextCount?t=' + new Date().getTime(),
                    {}
                ).then((response) => {
                    try {
                        var tmpData: any = response.data;
                        this.misPpgLocCount = tmpData.int_value;

                        this.isMissinTextAdded = true;
                        if (this.misPpgLocCount == 0) {
                            this.getParentPgs();
                            //this.getLoadDataGridSettings();
                        } else {
                            this.setMissingLocalText(0);
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

        private setMissingLocalText(ppgIndex: number): void {
            try {
                this.hideTranslation();
                this.isMissinTextAdded = false;

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
                            this.setMissingLocalText(ppgIndex);
                            
                        } else {
                            //this.getParentPgs();
                            this.getLoadDataGridSettings();
                            this.isMissinTextAdded = true;

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

        
        //*****************************************************************************

        //*****************************************************************************
        //Methods
        private setGrid() : void {
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
                    name: 'used_pg',
                    displayName: '',
                    enableFiltering: false,
                    enableSorting: false,
                    enableCellEdit: false,
                    enableHiding: false,
                    enableColumnResizing: false,
                    width: 35,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellUsedPg.html"

                },
                {
                    name: "name", displayName: this.locNameText, field: "name",
                    enableHiding: false,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTranslationTextMandatoryTemplate.html",
                    enableCellEdit: false,
                    width: 350,
                    minWidth: 350
                }
               
            ];
        }

        private loadData(): void {
            //this.getMaxCompanyId();
            this.getMissingLocalTextCount();
        }
                
        //private getMaxCompanyId () {
        //    this.isMaxCompIdLoaded = false;
        //    this.showLoader(this.isError);

        //    this.$http.get(
        //        this.getRegetRootUrl() + 'ParentPg/GetMaxCompanyId?&t=' + new Date().getTime(),
        //        {}
        //    ).then((response) => {
        //        try {
        //            var tmpData: any = response.data;
        //            //this.maxCompId = tmpData;

        //            this.isMaxCompIdLoaded = true;

        //            this.hideLoaderWrapper();
        //        } catch (ex) {
        //            this.hideLoaderWrapper();
        //            this.displayErrorMsg();
        //        }
        //    }, (response: any) => {
        //        this.hideLoaderWrapper();
        //        this.displayErrorMsg();
        //    });
        //}

        private hideLoaderWrapper() {
            if (this.isError || (this.isPgLoaded && this.isMissinTextAdded && this.isCompanyColumnsAdded)) {
                this.hideLoader();
                this.isError = false;
            }
        }

        public toggleGridCheckbox(item: ParentPgExtended, col: uiGrid.IColumnDefOf<any>) : void {

            var fieldParts = col.field.split('_');
            var compId: number = parseInt(fieldParts[1]);

            if (item[col.field]) {
                
                if (item["companyIsUsed_" + compId] === true) {
                    
                    var strMessage = this.locParentPgUsedText;
                    
                    this.$mdDialog.show(
                        this.$mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title(this.locConfirmationText)
                            .textContent(strMessage)
                            .ariaLabel('Alert Dialog')
                            .ok(this.locCloseText)
                    );
                } else {
                    item[col.field] = false;
                    this.getParentPgExtendedData(compId, false);
                }
            } else {
                item[col.field] = true;
                this.getParentPgExtendedData(compId, true);
            }
            
        }

        private getParentPgExtendedData(compId: number, isChecked : boolean): void {
            var editParentPg: ParentPgExtended = this.editRow;
            if (isChecked === true) {
                if (this.isValueNullOrUndefined(editParentPg.selected_companies)) {
                    editParentPg.selected_companies = [];
                }
                editParentPg.selected_companies.push(compId);
            } else {
                if (!this.isValueNullOrUndefined(editParentPg.selected_companies)) {
                    for (var i: number = 0; i < editParentPg.selected_companies.length; i++) {
                        if (editParentPg.selected_companies[i] == compId) {
                            editParentPg.selected_companies.splice(i, 1);
                            break;
                        }
                    }
                    
                }
            }
                        
        }

        private displayUsedPgs(parentPg: ParentPgExtended): void {
            window.location.href = this.getRegetRootUrl() + "ParentPg/UsedPg?ppgId=" + parentPg.id;
        }

        private setCompanyColumns(ppgData: any): void {
            try {
                var filterValues: any[] = this.yesNo;
                if (!this.isValueNullOrUndefined(ppgData) && ppgData.length > 0) {
                    angular.forEach(ppgData[0], (value: any, key: string) => {
                        if (key.indexOf(this.companyNamePrefix) >= 0) {
                            var colItems = value.split('|');
                            var pos = this.getColumnIndex(colItems[0]);
                            if (pos < 0) {

                                let compColName: string = colItems[0];

                                let comColumn: uiGrid.IColumnDefOf<any> = {
                                    field: compColName,
                                    name: compColName,
                                    displayName: colItems[1],
                                    filter: {
                                        type: this.uiGridConstants.filter.SELECT,
                                        selectOptions: filterValues
                                    },
                                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellCheckboxTemplate.html"
                                };

                                //Set filter
                                if (!this.isValueNullOrUndefined(this.gridFilter)) {
                                    for (var i: number = 0; i < this.gridFilter.length; i++) {
                                        if (compColName === this.gridFilter[i].column_name) {
                                            comColumn.filter.term = this.gridFilter[i].filter_value;
                                            this.isFilterApplied = true;
                                        }
                                    }
                                }

                                //Set sort
                                if (!this.isValueNullOrUndefined(this.gridSort)) {
                                    for (var i = 0; i < this.gridSort.length; i++) {
                                        if (comColumn.name === this.gridSort[i].column_name) {
                                            //comColumn.direction = this.gridSort[i].direction;
                                            comColumn.sort = {
                                                direction: this.gridSort[i].direction
                                            }
                                        }

                                    }
                                }


                                this.gridOptions.columnDefs.push(comColumn);
                            }
                        }
                    });

                    this.setColumnsCanBeHidden();
                }
            } catch (e) {
                this.displayErrorMsg();
            } 
        }

                        
        //*****************************************************************************
    }
    angular.
        module("RegetApp").
        controller("ParentPgController", Kamsyk.RegetApp.ParentPgController);
}