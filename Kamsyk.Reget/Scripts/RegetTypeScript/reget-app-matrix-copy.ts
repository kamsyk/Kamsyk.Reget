/// <reference path="../RegetTypeScript/Base/reget-base.ts" />
/// <reference path="../RegetTypeScript/Base/reget-common.ts" />


module Kamsyk.RegetApp {
    export class AppMatrixCopyController extends BaseRegetTs implements angular.IController {
        //************************************************************
        //Properties
        //private isError: boolean = false;
        private errMsg: string = null;
        private centreGroupListSource: CentreGroupAppMatrixCopy[] = null;
        private centreGroupListTarget: CentreGroupAppMatrixCopy[] = null;
        private isCentreGroupsLoaded: boolean = false;
        private selectedCentreGroupSourceId: number = null;
        private isCgPurchaseGroupsSourceLoaded: boolean = false;
        private selectedCentreGroupNameSource: string = null;
        private selectedCentreGroupTargetId: number = null;
        private selectedCentreGroupNameTarget: string = null;
        private purchaseGroupSourceList: PurchaseGroup[] = null;
        private purchaseGroupTargetList: PurchaseGroup[] = null;
        private isPgLoaded: boolean = false;
        private isCentreLoaded: boolean = false;
        private centreListSource: Centre[] = null;
        private centreListTarget: Centre[] = null;
        private selectedCentreGroupCurrencySource: string = null;
        private isCgPurchaseGroupsSourceLoadedEmpty: boolean = false;
        private isCgPurchaseGroupsTargetLoaded: boolean = false;
        private selectedCentreGroupCurrencyTarget: string = null;
        private pgItemCount: number = null;
        private purchaseGroupCount: number = null;
        //private selectedCentreGroupSource: string = null;
        //private selectedCentreGroupTarget: string = null;
        private loadingMsg: string = null;
        private searchstringcentresource: string = null;
        private searchstringcentretarget: string = null;
        private cgCentresSource: Centre[] = null;
        private cgCentresTarget: Centre[] = null;
        private pgCopyErrMsg: string = null;
        private isCoppied: boolean = false;
                        
        private locLoadDataErrorText: string = angular.element("#LoadDataErrorText").val();
        private locLoadingPurchaseGroupsText: string = angular.element("#LoadingPurchaseGroupsText").val();
        private locWarningText: string = angular.element("#WarningText").val();
        //private locCloseText: string = angular.element("#CloseText").val();
        private locErrMsgGenericText: string = angular.element("#ErrMsgGenericText").val();
        private locCancelSelectText: string = angular.element("#CancelSelectText").val();
        private locAppMatrixCannotCopyCurrencyText: string = angular.element("#AppMatrixCannotCopyCurrencyText").val();
        private locSourceTargetAreaSameText: string = angular.element("#SourceTargetAreaSameText").val();
        private locNoSourcePgIsSelectedText: string = angular.element("#NoSourcePgIsSelectedText").val();
        private locAreYouSureCopyAppMatrixText: string = angular.element("#AreYouSureCopyAppMatrixText").val();
        //private locYesText: string = $("#YesText").val();
        //private locNoText: string = $("#NoText").val();
        private locConfirmationText: string = $("#ConfirmationText").val();
        private locSaveErrorText: string = $("#SaveErrorText").val();
        //private locLoadingDataText: string = $("#LoadingDataText").val();
        private locCopyingPgText: string = $("#CopyingPgText").val();
        private locPgCopyReplaceConfirmText: string = $("#PgCopyReplaceConfirmText").val();
        //private locDataWasSavedText: string = $("#DataWasSavedText").val();
        private locTargetPgDoubledText: string = $("#TargetPgDoubledText").val();
        private locSourceAreaText: string = $("#SourceAreaText").val();
        private locTargetAreaText: string = $("#TargetAreaText").val();
        private locEnterText: string = $("#EnterText").val();
        private locEnterMandatoryValuesText: string = $("#EnterMandatoryValuesText").val();
        //************************************************************

        //**********************************************************
        //Constructor
        constructor(
            protected $scope: ng.IScope,
            protected $http: ng.IHttpService,
            protected $filter: ng.IFilterService,
            protected $mdDialog: angular.material.IDialogService,
            protected $mdToast: angular.material.IToastService,
            protected $q: ng.IQService,
            protected $timeout: ng.ITimeoutService

        ) {
            super($scope, $http, $filter, $mdDialog, $mdToast, $q, $timeout);

            this.loadData();

        }
        //***************************************************************

        $onInit = () => { };

        //************************************************************************
        //Methods

        //************************************************************************
        //Http Methods
        private populateCentreGroups() : void {
            this.isError = false;
            this.errMsg = this.locLoadDataErrorText;

            this.showLoader(this.isError);
            angular.element("#spanLoading").html(this.locLoadingDataText);

            this.$http.get(
                this.getRegetRootUrl() + 'RegetAdmin/GetAdministratedCentreGroups?isDeactivatedLoaded=false&t=' + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    this.centreGroupListSource = tmpData;
                    this.centreGroupListTarget = angular.copy(this.centreGroupListSource);
                    this.isCentreGroupsLoaded = true;;
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

        private displayAppMatrix(isSource: boolean, isSetDefaultCopy: boolean) {
            this.isPgLoaded = false;
            this.isCentreLoaded = false;

            if (isSource) {
                if (this.isValueNullOrUndefined(this.selectedCentreGroupSourceId)) {
                    return;
                }
            } else {
                if (this.isValueNullOrUndefined(this.selectedCentreGroupTargetId)) {
                    return;
                }
            }

            this.errMsg = this.locLoadDataErrorText;

            var cgId = (isSource) ? this.selectedCentreGroupSourceId : this.selectedCentreGroupTargetId;
            
            this.getCgCentres(isSource, cgId);

            this.isError = false;
            this.showLoader(this.isError);
            angular.element("#spanLoading").html(this.locLoadingDataText);

            this.$http.get(
                this.getRegetRootUrl() + '/RegetAdmin/GetPurchaseGroupsByCgId?cgId=' + cgId + '&pgRequestor=2' + '&indexFrom=-1&isDeativatedLoaded=false' + '&t=' + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    if (isSource) {
                        this.purchaseGroupSourceList = tmpData;
                        var cg: CentreGroupAppMatrixCopy = this.$filter("filter")(this.centreGroupListSource, { id: cgId }, true);
                        this.selectedCentreGroupCurrencySource = cg[0].currency_code;
                        this.selectedCentreGroupNameSource = cg[0].name;
                        this.isCgPurchaseGroupsSourceLoadedEmpty = (this.purchaseGroupSourceList.length === 0);
                    } else {
                        this.purchaseGroupTargetList = tmpData;
                        var cgT: CentreGroupAppMatrixCopy = this.$filter("filter")(this.centreGroupListTarget, { id: cgId }, true);
                        this.selectedCentreGroupCurrencyTarget = cgT[0].currency_code;
                        this.selectedCentreGroupNameTarget = cgT[0].name;
                    }

                    if (isSource) {
                        if (!this.isValueNullOrUndefined(this.purchaseGroupSourceList) && this.purchaseGroupSourceList.length > 25) {
                            //*** do not delete ****************
                            this.pgItemCount = 25;
                            //displayElement('divPgLoading');
                            this.showLoader(this.isError);
                            this.getPurchaseGroupsCount(cgId, true);
                            //************************************
                        } else {
                            this.isCgPurchaseGroupsSourceLoaded = true;
                            if (isSetDefaultCopy === true) {
                                this.setDefaultCopy();
                            }
                            this.isPgLoaded = true;
                            this.hideLoaderWrapperInitCg(false);
                        }
                    } else {
                        if (!this.isValueNullOrUndefined(this.purchaseGroupTargetList) && this.purchaseGroupTargetList.length > 25) {
                            //*** do not delete ****************
                            this.pgItemCount = 25;
                            //displayElement('divPgLoading');
                            this.showLoader(this.isError);
                            this.getPurchaseGroupsCount(cgId, false);
                            //************************************
                        } else {
                            this.isCgPurchaseGroupsTargetLoaded = true;
                            if (isSetDefaultCopy === true) {
                                this.setDefaultCopy();
                            }
                            this.isPgLoaded = true;
                            this.hideLoaderWrapperInitCg(false);
                        }
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

        private getPurchaseGroupsCount(cgId: number, isSource: boolean) {
            this.$http.get(
                this.getRegetRootUrl() + '/RegetAdmin/GetPurchaseGroupsCount?cgId=' + cgId + '&t=' + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    this.purchaseGroupCount = tmpData;
                    this.populateNextPurchaseGroup(cgId, isSource);
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

        private getCgCentres(isSource: boolean, cgId: number) : void {
            this.isError = false;
            this.errMsg = this.locLoadDataErrorText;

            this.showLoader(this.isError);
            angular.element("#spanLoading").html(this.locLoadingDataText);

            this.$http.get(
                this.getRegetRootUrl() + 'RegetAdmin/GetCgCentresByCgId?cgId=' + cgId + '&t=' + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    if (isSource) {
                        this.centreListSource = tmpData;
                    } else {
                        this.centreListTarget = tmpData;
                    }
                    this.isCentreLoaded = true;;
                    this.hideLoaderWrapperInitCg(false);
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

        private populateNextPurchaseGroup(cgId: number, isSource: boolean) {
            this.showLoader(this.isError);

            var strCount: string = "";
            if (!this.isValueNullOrUndefined(this.purchaseGroupCount)) {
                strCount = this.purchaseGroupCount.toString();
                if (this.purchaseGroupCount <= 25) {
                    this.loadingMsg = this.locLoadingDataText;
                    angular.element("#spanLoading").html(this.loadingMsg);
                    //hideElement('divPgLoading');
                }
            }

            var strCountOutOf = "";
            if (!this.isValueNullOrUndefined(this.pgItemCount)) {
                strCountOutOf = this.pgItemCount.toString();
            }

            this.loadingMsg = this.locLoadingPurchaseGroupsText.replace("##", strCount).replace("#", strCountOutOf);
            angular.element("#spanLoading").html(this.loadingMsg);

            var iLoadedPgsCount = (isSource) ? this.purchaseGroupSourceList.length : this.purchaseGroupTargetList.length;

            this.$http.get(
                this.getRegetRootUrl() + '/RegetAdmin/GetPurchaseGroupsByCgId?cgId=' + cgId + '&indexFrom=' + iLoadedPgsCount + '&pgRequestor=2' + '&t=' + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    if (tmpData.length > 0) {
                        this.pgItemCount++;
                        if (isSource) {
                            this.purchaseGroupSourceList.push(tmpData[0]);
                        } else {
                            this.purchaseGroupTargetList.push(tmpData[0]);
                        }
                        this.populateNextPurchaseGroup(cgId, isSource);
                    } else {
                        this.hideLoader();

                        //angular.element("#spanLoading").html(this.locLoadTextBkp);
                        if (isSource) {
                            this.isCgPurchaseGroupsSourceLoaded = true;
                        } else {
                            this.isCgPurchaseGroupsTargetLoaded = true;
                        }

                        this.setDefaultCopy();

                        this.hideLoaderWrapperInitCg(false);
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

        private populateCentresSearch(searchText: string, fullSearchText: string, deferred: any) {
            this.isError = false;
            this.errMsg = this.locLoadDataErrorText;
            if (this.isValueNullOrUndefined(deferred)) {
                return;
            }

            this.showLoaderBoxOnly(this.isError);
            angular.element("#spanLoading").html(this.locLoadingDataText);

            this.$http.get(
                this.getRegetRootUrl() + '/RegetAdmin/GetCgCentres?searchText=' + searchText + '&isDisabledCgCentresLoaded=false&t=' + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    this.cgCentresSource = tmpData;
                    angular.copy(tmpData, this.cgCentresTarget);
                    deferred.resolve(this.filterCentresSource(fullSearchText));
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

        private savePg(sourcePgs: PurchaseGroup[], currIndex: number, ev: MouseEvent) {
            if (sourcePgs.length == 0) {
                this.hideLoader();
                return;
            }
                        
            this.showLoader(this.isError);

            var sourcePgId: number = sourcePgs[currIndex].id;
            var isNew: boolean = sourcePgs[currIndex].is_coppied_default;
            var params: CoppiedPg = new CoppiedPg();
            params.sourcePgId = sourcePgId;
            params.isNew = isNew;
            params.targetCgId = this.selectedCentreGroupTargetId;
                
            var jsonPgData = JSON.stringify(params);

            this.errMsg = sourcePgs[currIndex].group_loc_name + " - " + this.locSaveErrorText;
            this.loadingMsg = this.locCopyingPgText.replace('{0}', (currIndex + 1).toString()).replace('{1}', sourcePgs.length.toString());
            angular.element("#spanLoading").html(this.loadingMsg);

            this.$http.post(
                this.getRegetRootUrl() + 'RegetAdmin/CopyAppMatrix',
                jsonPgData
            ).then((response) => {
                try {
                    var result = response.data;
                    this.isCoppied = true;
                    sourcePgs[currIndex].is_coppied = false;
                    sourcePgs[currIndex].is_coppied_default = false;
                    currIndex++;
                    if (currIndex < sourcePgs.length) {
                        this.savePgWithCheck(sourcePgs, currIndex, ev);
                    } else {
                        this.reloadTargetAppMatrix();
                    }
                } catch (e) {
                    currIndex = sourcePgs.length + 1;
                    this.hideLoader();
                    this.displayErrorMsg();
                    this.displayAppMatrix(false, false);
                } finally {
                    this.hideLoader();
                }
            }, (response: any) => {
                    currIndex = sourcePgs.length + 1;
                    this.hideLoader();
                    this.displayErrorMsg();
                    this.displayAppMatrix(false, false);
            });
                       
        }
        //************************************************************************

        private loadData() : void {
            this.populateCentreGroups();
        }

        private getCentreGroupSourceData() : void {
            this.isError = false;
            this.hideBtnErrorMsg();
            this.errMsg = this.locLoadingPurchaseGroupsText;
            this.isCgPurchaseGroupsSourceLoaded = false;

            this.clearSelectedCg();

            this.displayAppMatrix(true, true);
        }

        private clearSelectedCg() : void {
            //if (this.selectedCentreGroupSource === this.locCancelSelectText) {
            //    this.selectedCentreGroupSourceId = null;
            //    this.selectedCentreGroupNameSource = null;
            //    this.purchaseGroupSourceList = null;
            //}

            //if (this.selectedCentreGroupTarget === this.locCancelSelectText) {
            //    this.selectedCentreGroupTargetId = null;
            //    this.selectedCentreGroupNameTarget = null;
            //    this.purchaseGroupTargetList = null;
            //}
        }

        private hideLoaderWrapperInitCg(isError: boolean) {
            if (isError || (this.isCentreLoaded === true && this.isPgLoaded === true)) {
                this.hideLoader();
            }
        }

        private setDefaultCopy() : void {
            if (this.isCgPurchaseGroupsSourceLoaded === false || this.isCgPurchaseGroupsTargetLoaded === false) {
                return;
            }

            for (var i: number = 0; i < this.purchaseGroupSourceList.length; i++) {
                var pg: PurchaseGroup[] = this.$filter("filter")(this.purchaseGroupTargetList, { parent_pg_id: this.purchaseGroupSourceList[i].parent_pg_id }, true);
                if (pg.length === 0) {
                    this.purchaseGroupSourceList[i].is_coppied = true;
                    this.purchaseGroupSourceList[i].is_coppied_default = true;
                } else if (pg.length === 1) {
                    this.purchaseGroupSourceList[i].is_coppied = false;
                    this.purchaseGroupSourceList[i].is_coppied_default = false;
                    
                } else {
                    //cannot be coppied thera are 2 more pgs in target, cannot determined which one will be replaced
                    this.purchaseGroupSourceList[i].is_coppied = false;
                    this.purchaseGroupSourceList[i].is_coppied_default = false;
                    this.purchaseGroupSourceList[i].is_disabled = true;
                }
            }
        }

        private centreSourceSelectedItemChange(item: Centre) : void {
            this.isError = false;
            
            this.errMsg = this.locLoadDataErrorText;
            try {
                if (this.isValueNullOrUndefined(item)) {
                    return;
                }
                this.selectedCentreGroupSourceId = item.centre_group_id;
                this.getCentreGroupSourceData();
            } catch (ex) {
                this.displayErrorMsg();
            }
        }

        private centreSourceQuerySearch(query: string) : Centre[] {
            this.isError = false;
            
            this.errMsg = this.locLoadDataErrorText;
            try {
                var results = null;

                if (!this.isValueNullOrUndefined(query) && query.length === 1) {
                    var deferred: any = this.$q.defer<any>();
                    this.populateCentresSearch(query, query, deferred);
                    return deferred.promise;
                } else {
                    //if (this.isValueNullOrUndefined(this.centresSource)) {
                        var deferred: any = this.$q.defer<any>();
                        this.populateCentresSearch(query.substring(0, 2), query, deferred);
                        return deferred.promise;
                    //} else {
                    //    return this.filterCentresSource(query);
                    //}
                }
            } catch (ex) {
                this.displayErrorMsg();
            }
        }

        private filterCentresSource(query: string) : Centre[] {
            this.isError = false;
            this.errMsg = this.locLoadDataErrorText;
            try {
                var searchCentres: Centre[] = [];

                if (this.isValueNullOrUndefined(query) || query.length < 1) {
                    return null;
                }
                
                angular.forEach(this.cgCentresSource, (centre: Centre) => {
                    if (centre.name.toLowerCase().indexOf(query.toLowerCase()) > -1) {
                        searchCentres.push(centre);
                    }

                });
                return searchCentres;
            } catch (ex) {
                this.displayErrorMsg();
            }
        }

        private displaySearchCentreSource() : void {
            this.isError = false;
            this.errMsg = this.locLoadDataErrorText;
            try {
                var appCentre = angular.element("#appCentreSource");
                var btnFind = angular.element("#btnFindCentreSource");

                if (appCentre.is(':visible')) {
                    appCentre.hide("slow");
                    btnFind[0].style.marginTop = "5px";
                } else {
                    var childWrap = appCentre.children()[0];
                    childWrap.className += ' reget-search-text';
                    var childText = childWrap.children[0];
                    childText.setAttribute('style', 'margin-top:-4px;');
                    btnFind[0].style.marginTop = "2px";

                    appCentre.show("slow");
                }
            } catch (ex) {
                this.displayErrorMsg();
            }
        }

        private displaySearchCentreTarget () : void {
            this.isError = false;
            
            this.errMsg = this.locLoadDataErrorText;
            try {
                var appCentre = angular.element("#appCentreTarget");
                var btnFind = angular.element("#btnFindCentreTarget");

                if (appCentre.is(':visible')) {
                    appCentre.hide("slow");
                    btnFind[0].style.marginTop = "5px";
                } else {
                    var childWrap = appCentre.children()[0];
                    //childWrap.setAttribute('style', 'height:29px;margin-top:8px;box-shadow:1px 1px 1px 1px rgba(0,0,0,0.2)');
                    childWrap.className += ' reget-search-text';
                    var childText = childWrap.children[0];
                    childText.setAttribute('style', 'margin-top:-4px;');
                    btnFind[0].style.marginTop = "2px";

                    appCentre.show("slow");
                }
            } catch (ex) {
                this.displayErrorMsg();
            }
        }

        private centreTargetSelectedItemChange(item: Centre) : void {
            this.isError = false;
            
            this.errMsg = this.locLoadDataErrorText;
            try {
                if (this.isValueNullOrUndefined(item)) {
                    return;
                }
                this.selectedCentreGroupTargetId = item.centre_group_id;
                this.getCentreGroupTargetData();
            } catch (ex) {
                this.displayErrorMsg();
            }
        }

        private getCentreGroupTargetData(): void {
            this.isError = false;
            this.hideBtnErrorMsg();
            this.errMsg = this.locLoadingPurchaseGroupsText;
            this.isCgPurchaseGroupsTargetLoaded = false;

            this.clearSelectedCg();

            this.displayAppMatrix(false, true);
        }

        private centreTargetQuerySearch(query: string) : void {
            this.isError = false;
            this.errMsg = this.locLoadDataErrorText;
            try {
                var results = null;

                if (!this.isValueNullOrUndefined(query) && query.length === 1) {
                    var deferred: any = this.$q.defer<any>();
                    this.populateCentresSearch(query, query, deferred);
                    return deferred.promise;
                } else {
                    //if (this.isValueNullOrUndefined(this.centresTarget)) {
                        var deferred: any = this.$q.defer<any>();
                        this.populateCentresSearch(query.substring(0, 2), query, deferred);
                        return deferred.promise;
                    //} else {
                    //    return $scope.FilterCentresTarget(query);
                    //}
                }
            } catch (ex) {
                this.displayErrorMsg();
            }
        }

        //private isCopyDisabled() : boolean {
           
        //    var isDisabled: boolean = this.isValueNullOrUndefined(this.selectedCentreGroupSourceId)
        //        || this.isValueNullOrUndefined(this.selectedCentreGroupTargetId)
        //        || this.selectedCentreGroupCurrencySource != this.selectedCentreGroupCurrencyTarget
        //        || this.selectedCentreGroupSourceId === this.selectedCentreGroupTargetId;

        //    if (this.selectedCentreGroupCurrencySource !== this.selectedCentreGroupCurrencyTarget
        //        && !this.isValueNullOrUndefined(this.selectedCentreGroupCurrencySource)
        //        && !this.isValueNullOrUndefined(this.selectedCentreGroupCurrencyTarget)) {
        //        this.pgCopyErrMsg = this.locAppMatrixCannotCopyCurrencyText;
        //    } else if (this.selectedCentreGroupSourceId === this.selectedCentreGroupTargetId
        //        && !this.isValueNullOrUndefined(this.selectedCentreGroupSourceId)) {
        //        this.pgCopyErrMsg = this.locSourceTargetAreaSameText;
        //    } else {
        //        this.pgCopyErrMsg = null;
        //    }

        //    return isDisabled;
        //}

        private isCopyAppMatrixDataValid(): boolean {
            let isValid: boolean = true;
            
            if (this.isValueNullOrUndefined(this.selectedCentreGroupSourceId)) {
                if (this.angScopeAny.frmCgCopy.cmbCgListSource.$valid == false) {
                    this.angScopeAny.frmCgCopy.cmbCgListSource.$setTouched();
                }

                isValid = false;
            } 

            if (this.isValueNullOrUndefined(this.selectedCentreGroupTargetId)) {
                if (this.angScopeAny.frmCgCopy.cmbCgListTarget.$valid == false) {
                    this.angScopeAny.frmCgCopy.cmbCgListTarget.$setTouched();
                }

                isValid = false;
            } 

            if (this.selectedCentreGroupCurrencySource !== this.selectedCentreGroupCurrencyTarget
                && !this.isValueNullOrUndefined(this.selectedCentreGroupCurrencySource)
                && !this.isValueNullOrUndefined(this.selectedCentreGroupCurrencyTarget)) {
                this.pgCopyErrMsg = this.locAppMatrixCannotCopyCurrencyText;
                this.displayBtnErrorMsg();
                isValid = false;
            }

            if (this.selectedCentreGroupSourceId === this.selectedCentreGroupTargetId
                && !this.isValueNullOrUndefined(this.selectedCentreGroupSourceId)) {
                this.pgCopyErrMsg = this.locSourceTargetAreaSameText;
                this.displayBtnErrorMsg();
                isValid = false;
            }

            return isValid;
            //this.pgCopyErrMsg = null;
            
            //if (this.isValueNullOrUndefined(this.selectedCentreGroupSourceId)) {
            //    this.pgCopyErrMsg = this.locEnterText + " " + this.locSourceAreaText;
            //    return false;
            //}

            //if (this.isValueNullOrUndefined(this.selectedCentreGroupTargetId)) {
            //    this.pgCopyErrMsg = this.locEnterText + " " + this.locTargetAreaText;
            //    return false;
            //}

            //var sourcePgs: PurchaseGroup[] = this.$filter("filter")(this.purchaseGroupSourceList, { is_coppied: true }, true);
            //if (sourcePgs.length === 0) {
            //    this.pgCopyErrMsg = this.locNoSourcePgIsSelectedText;
            //    return false;
            //}

            //if (this.selectedCentreGroupCurrencySource !== this.selectedCentreGroupCurrencyTarget
            //    && !this.isValueNullOrUndefined(this.selectedCentreGroupCurrencySource)
            //    && !this.isValueNullOrUndefined(this.selectedCentreGroupCurrencyTarget)) {
            //    this.pgCopyErrMsg = this.locAppMatrixCannotCopyCurrencyText;
            //    return false;
            //}

            //if (this.selectedCentreGroupSourceId === this.selectedCentreGroupTargetId
            //    && !this.isValueNullOrUndefined(this.selectedCentreGroupSourceId)) {
            //    this.pgCopyErrMsg = this.locSourceTargetAreaSameText;
            //    return false;
            //}
                        
            //return true;
        }    

        private displayBtnErrorMsg() : void {
            let divErrMsg: HTMLDivElement = <HTMLDivElement>document.getElementById("divErrMsgCopy");
            divErrMsg.style.display = "block";
        }

        private hideBtnErrorMsg(): void {
            let divErrMsg: HTMLDivElement = <HTMLDivElement>document.getElementById("divErrMsgCopy");
            divErrMsg.style.display = "none";
        }

        private dialogConfirmController(
            $scope,
            $mdDialog,
            appMatrixCopyControl: AppMatrixCopyController,
            sourcePgs: PurchaseGroup[],
            ev: MouseEvent
        ): void {

            $scope.closeDialog = function () {
                $mdDialog.hide();
            }

            $scope.confirmDialog = () => {
                $mdDialog.hide();
                appMatrixCopyControl.copyAppMatrixConfirmed(sourcePgs, ev);
            }
        }

        private dialogReplaceConfirmController(
            $scope,
            $mdDialog,
            appMatrixCopyControl: AppMatrixCopyController,
            sourcePgs: PurchaseGroup[],
            currIndex: number,
            ev: MouseEvent
        ): void {

            $scope.closeDialog = function () {
                $mdDialog.hide();
                appMatrixCopyControl.copyAppMatrixReplaceConfirmedNo(sourcePgs, currIndex, ev);
            }

            $scope.confirmDialog = () => {
                $mdDialog.hide();
                appMatrixCopyControl.copyAppMatrixReplaceConfirmedYes(sourcePgs, currIndex, ev);
            }
        }

        private copyAppMatrix(ev: MouseEvent): void {
            if (!this.isCopyAppMatrixDataValid()) {
                this.displayErrorMsg(this.locEnterMandatoryValuesText);
                return;
            }

            this.isCoppied = false;
            let sourcePgs: PurchaseGroup[] = this.$filter("filter")(this.purchaseGroupSourceList, { is_coppied: true }, true);
            if (sourcePgs.length === 0) {
                this.showAlert(this.locWarningText, this.locNoSourcePgIsSelectedText, this.locCloseText);
                return;
            }

            this.$mdDialog.show(
                {
                    template: this.getConfirmDialogTemplate(
                        this.locAreYouSureCopyAppMatrixText,
                        this.locConfirmationText,
                        this.locYesText,
                        this.locCancelText,
                        "confirmDialog()",
                        "closeDialog()"),
                    locals: {
                        appMatrixCopyControl: this,
                        sourcePgs: sourcePgs,
                        ev: ev
                    },
                    controller: this.dialogConfirmController
                });
            
            //var confirm = this.$mdDialog.confirm()
            //    .title(this.locConfirmationText)
            //    .textContent(this.locAreYouSureCopyAppMatrixText)
            //    .ariaLabel("CopyAppMatrixConfirm")
            //    .targetEvent(ev)
            //    .ok(this.locNoText)
            //    .cancel(this.locYesText);

            //this.$mdDialog.show(confirm).then(() => {
            //    return;
            //}, () => {
            //    this.isError = false;
            //    this.showLoader(this.isError);

            //    this.savePgWithCheck(sourcePgs, 0, ev);
            //});
                        
        }

        private copyAppMatrixConfirmed(sourcePgs: PurchaseGroup[], ev: MouseEvent) {
            this.isError = false;
            this.showLoader(this.isError);

            this.savePgWithCheck(sourcePgs, 0, ev);
        }

        private savePgWithCheck(sourcePgs: PurchaseGroup[], currIndex: number, ev: MouseEvent) : void {
            if (sourcePgs.length == 0) {
                this.hideLoader();
                return;
            }

            if (currIndex >= sourcePgs.length) {
                this.reloadTargetAppMatrix();
                return;
            }

            if (sourcePgs[currIndex].is_coppied_default === false) {
                this.hideLoader();

                var msg = this.locPgCopyReplaceConfirmText.replace('{0}', sourcePgs[currIndex].group_loc_name);

                this.$mdDialog.show(
                    {
                        template: this.getConfirmDialogTemplate(
                            this.locAreYouSureCopyAppMatrixText,
                            this.locConfirmationText,
                            this.locYesText,
                            this.locCancelText,
                            "confirmDialog()",
                            "closeDialog()"),
                        locals: {
                            appMatrixCopyControl: this,
                            sourcePgs: sourcePgs,
                            currIndex: currIndex,
                            ev: ev
                        },
                        controller: this.dialogReplaceConfirmController
                    });

                //var confirm1 = this.$mdDialog.confirm()
                //    .title(this.locConfirmationText)
                //    .textContent(msg)
                //    .ariaLabel("ReplaceAppMatrixConfirm")
                //    .targetEvent(ev)
                //    .ok(this.locNoText)
                //    .cancel(this.locYesText);

                //this.$mdDialog.show(confirm1).then(() => {
                //    if (currIndex < (sourcePgs.length - 1)) {
                //        currIndex++;
                //        this.savePg(sourcePgs, currIndex, ev);
                //    } else {
                //        this.reloadTargetAppMatrix();
                //        return;
                //    }
                //}, function () {
                //    this.savePg(sourcePgs, currIndex, ev);
                //});
            } else {
                this.savePg(sourcePgs, currIndex, ev);
            }
        }

        private copyAppMatrixReplaceConfirmedYes(sourcePgs: PurchaseGroup[], currIndex:number, ev: MouseEvent) {
            this.savePg(sourcePgs, currIndex, ev);
        }

        private copyAppMatrixReplaceConfirmedNo(sourcePgs: PurchaseGroup[], currIndex: number, ev: MouseEvent) {
            if (currIndex < (sourcePgs.length - 1)) {
                currIndex++;
                this.savePg(sourcePgs, currIndex, ev);
            } else {
                this.reloadTargetAppMatrix();
                return;
            }
        }

        private reloadTargetAppMatrix() : void {
            angular.element("#spanLoading").html(this.locLoadingDataText);
            if (this.isCoppied === true) {
                this.showToast(this.locDataWasSavedText);
                this.displayAppMatrix(false, false);
            } else {
                this.hideLoader();
            }
        }

        private isAllPgChecked() : boolean {
            if (this.isValueNullOrUndefined(this.purchaseGroupSourceList)) {
                return false;
            }

            for (var i: number = 0; i < this.purchaseGroupSourceList.length; i++) {
                if (this.purchaseGroupSourceList[i].is_coppied === false && this.purchaseGroupSourceList[i].is_disabled === false) {
                    return false;
                }
            }

            return true;
        }

        private isPgIndeterminate() : boolean {
            if (this.isValueNullOrUndefined(this.purchaseGroupSourceList)) {
                return false;
            }

            return (this.purchaseGroupSourceList.length !== 0 &&
                !this.isAllPgChecked());
        }

        private togglePgCopy(pgItem: PurchaseGroup, event: MouseEvent) {
            var pg = this.$filter("filter")(this.purchaseGroupSourceList, { id: pgItem.id }, true);
            if (pg.length > 0) {
                if (pg[0].is_disabled === false) {
                    pg[0].is_coppied = !pg[0].is_coppied;
                } else {
                    var msg = this.locTargetPgDoubledText.replace('{0}', pg[0].parent_pg_loc_name);
                    this.showAlert(this.locWarningText, msg, this.locCloseText);
                }
            }

            this.stopPropagation(event);
        }

        private toggleAllPgCopy() : void {
            var isChecked = true;
            if (this.isAllPgChecked()) {
                isChecked = false;
            }
            for (var i: number = 0; i < this.purchaseGroupSourceList.length; i++) {
                if (this.purchaseGroupSourceList[i].is_disabled === true) {
                    continue;
                }
                this.purchaseGroupSourceList[i].is_coppied = isChecked;
            }

        }

        private isSourcePgNotEmpty() : boolean {
            return (this.isCgPurchaseGroupsSourceLoaded === false ||
                this.purchaseGroupSourceList.length > 0);
        }

        private isTargetPgNotEmpty() : boolean {
            return (this.isCgPurchaseGroupsTargetLoaded === false ||
                this.purchaseGroupTargetList.length > 0);
        };

        //*************************************************************************
    }

    angular.
        module('RegetApp').
        controller('AppMatrixCopyController', Kamsyk.RegetApp.AppMatrixCopyController);

    export class CentreGroupAppMatrixCopy {
    }

    export class CoppiedPg {
        public sourcePgId: number = null;
        public isNew: boolean = false;
        public targetCgId: number = null;
    }
}