/// <reference path="../RegetTypeScript/Base/reget-base.ts" />
/// <reference path="../RegetTypeScript/Base/reget-common.ts" />
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
        var AppMatrixCopyController = /** @class */ (function (_super) {
            __extends(AppMatrixCopyController, _super);
            //************************************************************
            //**********************************************************
            //Constructor
            function AppMatrixCopyController($scope, $http, $filter, $mdDialog, $mdToast, $q, $timeout) {
                var _this = _super.call(this, $scope, $http, $filter, $mdDialog, $mdToast, $q, $timeout) || this;
                _this.$scope = $scope;
                _this.$http = $http;
                _this.$filter = $filter;
                _this.$mdDialog = $mdDialog;
                _this.$mdToast = $mdToast;
                _this.$q = $q;
                _this.$timeout = $timeout;
                //************************************************************
                //Properties
                //private isError: boolean = false;
                _this.errMsg = null;
                _this.centreGroupListSource = null;
                _this.centreGroupListTarget = null;
                _this.isCentreGroupsLoaded = false;
                _this.selectedCentreGroupSourceId = null;
                _this.isCgPurchaseGroupsSourceLoaded = false;
                _this.selectedCentreGroupNameSource = null;
                _this.selectedCentreGroupTargetId = null;
                _this.selectedCentreGroupNameTarget = null;
                _this.purchaseGroupSourceList = null;
                _this.purchaseGroupTargetList = null;
                _this.isPgLoaded = false;
                _this.isCentreLoaded = false;
                _this.centreListSource = null;
                _this.centreListTarget = null;
                _this.selectedCentreGroupCurrencySource = null;
                _this.isCgPurchaseGroupsSourceLoadedEmpty = false;
                _this.isCgPurchaseGroupsTargetLoaded = false;
                _this.selectedCentreGroupCurrencyTarget = null;
                _this.pgItemCount = null;
                _this.purchaseGroupCount = null;
                //private selectedCentreGroupSource: string = null;
                //private selectedCentreGroupTarget: string = null;
                _this.loadingMsg = null;
                _this.searchstringcentresource = null;
                _this.searchstringcentretarget = null;
                _this.cgCentresSource = null;
                _this.cgCentresTarget = null;
                _this.pgCopyErrMsg = null;
                _this.isCoppied = false;
                _this.locLoadDataErrorText = angular.element("#LoadDataErrorText").val();
                _this.locLoadingPurchaseGroupsText = angular.element("#LoadingPurchaseGroupsText").val();
                _this.locWarningText = angular.element("#WarningText").val();
                //private locCloseText: string = angular.element("#CloseText").val();
                _this.locErrMsgGenericText = angular.element("#ErrMsgGenericText").val();
                _this.locCancelSelectText = angular.element("#CancelSelectText").val();
                _this.locAppMatrixCannotCopyCurrencyText = angular.element("#AppMatrixCannotCopyCurrencyText").val();
                _this.locSourceTargetAreaSameText = angular.element("#SourceTargetAreaSameText").val();
                _this.locNoSourcePgIsSelectedText = angular.element("#NoSourcePgIsSelectedText").val();
                _this.locAreYouSureCopyAppMatrixText = angular.element("#AreYouSureCopyAppMatrixText").val();
                //private locYesText: string = $("#YesText").val();
                //private locNoText: string = $("#NoText").val();
                _this.locConfirmationText = $("#ConfirmationText").val();
                _this.locSaveErrorText = $("#SaveErrorText").val();
                //private locLoadingDataText: string = $("#LoadingDataText").val();
                _this.locCopyingPgText = $("#CopyingPgText").val();
                _this.locPgCopyReplaceConfirmText = $("#PgCopyReplaceConfirmText").val();
                //private locDataWasSavedText: string = $("#DataWasSavedText").val();
                _this.locTargetPgDoubledText = $("#TargetPgDoubledText").val();
                _this.locSourceAreaText = $("#SourceAreaText").val();
                _this.locTargetAreaText = $("#TargetAreaText").val();
                _this.locEnterText = $("#EnterText").val();
                _this.locEnterMandatoryValuesText = $("#EnterMandatoryValuesText").val();
                //***************************************************************
                _this.$onInit = function () { };
                _this.loadData();
                return _this;
            }
            //************************************************************************
            //Methods
            //************************************************************************
            //Http Methods
            AppMatrixCopyController.prototype.populateCentreGroups = function () {
                var _this = this;
                this.isError = false;
                this.errMsg = this.locLoadDataErrorText;
                this.showLoader(this.isError);
                angular.element("#spanLoading").html(this.locLoadingDataText);
                this.$http.get(this.getRegetRootUrl() + 'RegetAdmin/GetAdministratedCentreGroups?isDeactivatedLoaded=false&t=' + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        _this.centreGroupListSource = tmpData;
                        _this.centreGroupListTarget = angular.copy(_this.centreGroupListSource);
                        _this.isCentreGroupsLoaded = true;
                        ;
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
            AppMatrixCopyController.prototype.displayAppMatrix = function (isSource, isSetDefaultCopy) {
                var _this = this;
                this.isPgLoaded = false;
                this.isCentreLoaded = false;
                if (isSource) {
                    if (this.isValueNullOrUndefined(this.selectedCentreGroupSourceId)) {
                        return;
                    }
                }
                else {
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
                this.$http.get(this.getRegetRootUrl() + '/RegetAdmin/GetPurchaseGroupsByCgId?cgId=' + cgId + '&pgRequestor=2' + '&indexFrom=-1&isDeativatedLoaded=false' + '&t=' + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        if (isSource) {
                            _this.purchaseGroupSourceList = tmpData;
                            var cg = _this.$filter("filter")(_this.centreGroupListSource, { id: cgId }, true);
                            _this.selectedCentreGroupCurrencySource = cg[0].currency_code;
                            _this.selectedCentreGroupNameSource = cg[0].name;
                            _this.isCgPurchaseGroupsSourceLoadedEmpty = (_this.purchaseGroupSourceList.length === 0);
                        }
                        else {
                            _this.purchaseGroupTargetList = tmpData;
                            var cgT = _this.$filter("filter")(_this.centreGroupListTarget, { id: cgId }, true);
                            _this.selectedCentreGroupCurrencyTarget = cgT[0].currency_code;
                            _this.selectedCentreGroupNameTarget = cgT[0].name;
                        }
                        if (isSource) {
                            if (!_this.isValueNullOrUndefined(_this.purchaseGroupSourceList) && _this.purchaseGroupSourceList.length > 25) {
                                //*** do not delete ****************
                                _this.pgItemCount = 25;
                                //displayElement('divPgLoading');
                                _this.showLoader(_this.isError);
                                _this.getPurchaseGroupsCount(cgId, true);
                                //************************************
                            }
                            else {
                                _this.isCgPurchaseGroupsSourceLoaded = true;
                                if (isSetDefaultCopy === true) {
                                    _this.setDefaultCopy();
                                }
                                _this.isPgLoaded = true;
                                _this.hideLoaderWrapperInitCg(false);
                            }
                        }
                        else {
                            if (!_this.isValueNullOrUndefined(_this.purchaseGroupTargetList) && _this.purchaseGroupTargetList.length > 25) {
                                //*** do not delete ****************
                                _this.pgItemCount = 25;
                                //displayElement('divPgLoading');
                                _this.showLoader(_this.isError);
                                _this.getPurchaseGroupsCount(cgId, false);
                                //************************************
                            }
                            else {
                                _this.isCgPurchaseGroupsTargetLoaded = true;
                                if (isSetDefaultCopy === true) {
                                    _this.setDefaultCopy();
                                }
                                _this.isPgLoaded = true;
                                _this.hideLoaderWrapperInitCg(false);
                            }
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
            AppMatrixCopyController.prototype.getPurchaseGroupsCount = function (cgId, isSource) {
                var _this = this;
                this.$http.get(this.getRegetRootUrl() + '/RegetAdmin/GetPurchaseGroupsCount?cgId=' + cgId + '&t=' + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        _this.purchaseGroupCount = tmpData;
                        _this.populateNextPurchaseGroup(cgId, isSource);
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
            AppMatrixCopyController.prototype.getCgCentres = function (isSource, cgId) {
                var _this = this;
                this.isError = false;
                this.errMsg = this.locLoadDataErrorText;
                this.showLoader(this.isError);
                angular.element("#spanLoading").html(this.locLoadingDataText);
                this.$http.get(this.getRegetRootUrl() + 'RegetAdmin/GetCgCentresByCgId?cgId=' + cgId + '&t=' + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        if (isSource) {
                            _this.centreListSource = tmpData;
                        }
                        else {
                            _this.centreListTarget = tmpData;
                        }
                        _this.isCentreLoaded = true;
                        ;
                        _this.hideLoaderWrapperInitCg(false);
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
            AppMatrixCopyController.prototype.populateNextPurchaseGroup = function (cgId, isSource) {
                var _this = this;
                this.showLoader(this.isError);
                var strCount = "";
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
                this.$http.get(this.getRegetRootUrl() + '/RegetAdmin/GetPurchaseGroupsByCgId?cgId=' + cgId + '&indexFrom=' + iLoadedPgsCount + '&pgRequestor=2' + '&t=' + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        if (tmpData.length > 0) {
                            _this.pgItemCount++;
                            if (isSource) {
                                _this.purchaseGroupSourceList.push(tmpData[0]);
                            }
                            else {
                                _this.purchaseGroupTargetList.push(tmpData[0]);
                            }
                            _this.populateNextPurchaseGroup(cgId, isSource);
                        }
                        else {
                            _this.hideLoader();
                            //angular.element("#spanLoading").html(this.locLoadTextBkp);
                            if (isSource) {
                                _this.isCgPurchaseGroupsSourceLoaded = true;
                            }
                            else {
                                _this.isCgPurchaseGroupsTargetLoaded = true;
                            }
                            _this.setDefaultCopy();
                            _this.hideLoaderWrapperInitCg(false);
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
            AppMatrixCopyController.prototype.populateCentresSearch = function (searchText, fullSearchText, deferred) {
                var _this = this;
                this.isError = false;
                this.errMsg = this.locLoadDataErrorText;
                if (this.isValueNullOrUndefined(deferred)) {
                    return;
                }
                this.showLoaderBoxOnly(this.isError);
                angular.element("#spanLoading").html(this.locLoadingDataText);
                this.$http.get(this.getRegetRootUrl() + '/RegetAdmin/GetCgCentres?searchText=' + searchText + '&isDisabledCgCentresLoaded=false&t=' + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        _this.cgCentresSource = tmpData;
                        angular.copy(tmpData, _this.cgCentresTarget);
                        deferred.resolve(_this.filterCentresSource(fullSearchText));
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
            AppMatrixCopyController.prototype.savePg = function (sourcePgs, currIndex, ev) {
                var _this = this;
                if (sourcePgs.length == 0) {
                    this.hideLoader();
                    return;
                }
                this.showLoader(this.isError);
                var sourcePgId = sourcePgs[currIndex].id;
                var isNew = sourcePgs[currIndex].is_coppied_default;
                var params = new CoppiedPg();
                params.sourcePgId = sourcePgId;
                params.isNew = isNew;
                params.targetCgId = this.selectedCentreGroupTargetId;
                var jsonPgData = JSON.stringify(params);
                this.errMsg = sourcePgs[currIndex].group_loc_name + " - " + this.locSaveErrorText;
                this.loadingMsg = this.locCopyingPgText.replace('{0}', (currIndex + 1).toString()).replace('{1}', sourcePgs.length.toString());
                angular.element("#spanLoading").html(this.loadingMsg);
                this.$http.post(this.getRegetRootUrl() + 'RegetAdmin/CopyAppMatrix', jsonPgData).then(function (response) {
                    try {
                        var result = response.data;
                        _this.isCoppied = true;
                        sourcePgs[currIndex].is_coppied = false;
                        sourcePgs[currIndex].is_coppied_default = false;
                        currIndex++;
                        if (currIndex < sourcePgs.length) {
                            _this.savePgWithCheck(sourcePgs, currIndex, ev);
                        }
                        else {
                            _this.reloadTargetAppMatrix();
                        }
                    }
                    catch (e) {
                        currIndex = sourcePgs.length + 1;
                        _this.hideLoader();
                        _this.displayErrorMsg();
                        _this.displayAppMatrix(false, false);
                    }
                    finally {
                        _this.hideLoader();
                    }
                }, function (response) {
                    currIndex = sourcePgs.length + 1;
                    _this.hideLoader();
                    _this.displayErrorMsg();
                    _this.displayAppMatrix(false, false);
                });
            };
            //************************************************************************
            AppMatrixCopyController.prototype.loadData = function () {
                this.populateCentreGroups();
            };
            AppMatrixCopyController.prototype.getCentreGroupSourceData = function () {
                this.isError = false;
                this.hideBtnErrorMsg();
                this.errMsg = this.locLoadingPurchaseGroupsText;
                this.isCgPurchaseGroupsSourceLoaded = false;
                this.clearSelectedCg();
                this.displayAppMatrix(true, true);
            };
            AppMatrixCopyController.prototype.clearSelectedCg = function () {
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
            };
            AppMatrixCopyController.prototype.hideLoaderWrapperInitCg = function (isError) {
                if (isError || (this.isCentreLoaded === true && this.isPgLoaded === true)) {
                    this.hideLoader();
                }
            };
            AppMatrixCopyController.prototype.setDefaultCopy = function () {
                if (this.isCgPurchaseGroupsSourceLoaded === false || this.isCgPurchaseGroupsTargetLoaded === false) {
                    return;
                }
                for (var i = 0; i < this.purchaseGroupSourceList.length; i++) {
                    var pg = this.$filter("filter")(this.purchaseGroupTargetList, { parent_pg_id: this.purchaseGroupSourceList[i].parent_pg_id }, true);
                    if (pg.length === 0) {
                        this.purchaseGroupSourceList[i].is_coppied = true;
                        this.purchaseGroupSourceList[i].is_coppied_default = true;
                    }
                    else if (pg.length === 1) {
                        this.purchaseGroupSourceList[i].is_coppied = false;
                        this.purchaseGroupSourceList[i].is_coppied_default = false;
                    }
                    else {
                        //cannot be coppied thera are 2 more pgs in target, cannot determined which one will be replaced
                        this.purchaseGroupSourceList[i].is_coppied = false;
                        this.purchaseGroupSourceList[i].is_coppied_default = false;
                        this.purchaseGroupSourceList[i].is_disabled = true;
                    }
                }
            };
            AppMatrixCopyController.prototype.centreSourceSelectedItemChange = function (item) {
                this.isError = false;
                this.errMsg = this.locLoadDataErrorText;
                try {
                    if (this.isValueNullOrUndefined(item)) {
                        return;
                    }
                    this.selectedCentreGroupSourceId = item.centre_group_id;
                    this.getCentreGroupSourceData();
                }
                catch (ex) {
                    this.displayErrorMsg();
                }
            };
            AppMatrixCopyController.prototype.centreSourceQuerySearch = function (query) {
                this.isError = false;
                this.errMsg = this.locLoadDataErrorText;
                try {
                    var results = null;
                    if (!this.isValueNullOrUndefined(query) && query.length === 1) {
                        var deferred = this.$q.defer();
                        this.populateCentresSearch(query, query, deferred);
                        return deferred.promise;
                    }
                    else {
                        //if (this.isValueNullOrUndefined(this.centresSource)) {
                        var deferred = this.$q.defer();
                        this.populateCentresSearch(query.substring(0, 2), query, deferred);
                        return deferred.promise;
                        //} else {
                        //    return this.filterCentresSource(query);
                        //}
                    }
                }
                catch (ex) {
                    this.displayErrorMsg();
                }
            };
            AppMatrixCopyController.prototype.filterCentresSource = function (query) {
                this.isError = false;
                this.errMsg = this.locLoadDataErrorText;
                try {
                    var searchCentres = [];
                    if (this.isValueNullOrUndefined(query) || query.length < 1) {
                        return null;
                    }
                    angular.forEach(this.cgCentresSource, function (centre) {
                        if (centre.name.toLowerCase().indexOf(query.toLowerCase()) > -1) {
                            searchCentres.push(centre);
                        }
                    });
                    return searchCentres;
                }
                catch (ex) {
                    this.displayErrorMsg();
                }
            };
            AppMatrixCopyController.prototype.displaySearchCentreSource = function () {
                this.isError = false;
                this.errMsg = this.locLoadDataErrorText;
                try {
                    var appCentre = angular.element("#appCentreSource");
                    var btnFind = angular.element("#btnFindCentreSource");
                    if (appCentre.is(':visible')) {
                        appCentre.hide("slow");
                        btnFind[0].style.marginTop = "5px";
                    }
                    else {
                        var childWrap = appCentre.children()[0];
                        childWrap.className += ' reget-search-text';
                        var childText = childWrap.children[0];
                        childText.setAttribute('style', 'margin-top:-4px;');
                        btnFind[0].style.marginTop = "2px";
                        appCentre.show("slow");
                    }
                }
                catch (ex) {
                    this.displayErrorMsg();
                }
            };
            AppMatrixCopyController.prototype.displaySearchCentreTarget = function () {
                this.isError = false;
                this.errMsg = this.locLoadDataErrorText;
                try {
                    var appCentre = angular.element("#appCentreTarget");
                    var btnFind = angular.element("#btnFindCentreTarget");
                    if (appCentre.is(':visible')) {
                        appCentre.hide("slow");
                        btnFind[0].style.marginTop = "5px";
                    }
                    else {
                        var childWrap = appCentre.children()[0];
                        //childWrap.setAttribute('style', 'height:29px;margin-top:8px;box-shadow:1px 1px 1px 1px rgba(0,0,0,0.2)');
                        childWrap.className += ' reget-search-text';
                        var childText = childWrap.children[0];
                        childText.setAttribute('style', 'margin-top:-4px;');
                        btnFind[0].style.marginTop = "2px";
                        appCentre.show("slow");
                    }
                }
                catch (ex) {
                    this.displayErrorMsg();
                }
            };
            AppMatrixCopyController.prototype.centreTargetSelectedItemChange = function (item) {
                this.isError = false;
                this.errMsg = this.locLoadDataErrorText;
                try {
                    if (this.isValueNullOrUndefined(item)) {
                        return;
                    }
                    this.selectedCentreGroupTargetId = item.centre_group_id;
                    this.getCentreGroupTargetData();
                }
                catch (ex) {
                    this.displayErrorMsg();
                }
            };
            AppMatrixCopyController.prototype.getCentreGroupTargetData = function () {
                this.isError = false;
                this.hideBtnErrorMsg();
                this.errMsg = this.locLoadingPurchaseGroupsText;
                this.isCgPurchaseGroupsTargetLoaded = false;
                this.clearSelectedCg();
                this.displayAppMatrix(false, true);
            };
            AppMatrixCopyController.prototype.centreTargetQuerySearch = function (query) {
                this.isError = false;
                this.errMsg = this.locLoadDataErrorText;
                try {
                    var results = null;
                    if (!this.isValueNullOrUndefined(query) && query.length === 1) {
                        var deferred = this.$q.defer();
                        this.populateCentresSearch(query, query, deferred);
                        return deferred.promise;
                    }
                    else {
                        //if (this.isValueNullOrUndefined(this.centresTarget)) {
                        var deferred = this.$q.defer();
                        this.populateCentresSearch(query.substring(0, 2), query, deferred);
                        return deferred.promise;
                        //} else {
                        //    return $scope.FilterCentresTarget(query);
                        //}
                    }
                }
                catch (ex) {
                    this.displayErrorMsg();
                }
            };
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
            AppMatrixCopyController.prototype.isCopyAppMatrixDataValid = function () {
                var isValid = true;
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
            };
            AppMatrixCopyController.prototype.displayBtnErrorMsg = function () {
                var divErrMsg = document.getElementById("divErrMsgCopy");
                divErrMsg.style.display = "block";
            };
            AppMatrixCopyController.prototype.hideBtnErrorMsg = function () {
                var divErrMsg = document.getElementById("divErrMsgCopy");
                divErrMsg.style.display = "none";
            };
            AppMatrixCopyController.prototype.dialogConfirmController = function ($scope, $mdDialog, appMatrixCopyControl, sourcePgs, ev) {
                $scope.closeDialog = function () {
                    $mdDialog.hide();
                };
                $scope.confirmDialog = function () {
                    $mdDialog.hide();
                    appMatrixCopyControl.copyAppMatrixConfirmed(sourcePgs, ev);
                };
            };
            AppMatrixCopyController.prototype.dialogReplaceConfirmController = function ($scope, $mdDialog, appMatrixCopyControl, sourcePgs, currIndex, ev) {
                $scope.closeDialog = function () {
                    $mdDialog.hide();
                    appMatrixCopyControl.copyAppMatrixReplaceConfirmedNo(sourcePgs, currIndex, ev);
                };
                $scope.confirmDialog = function () {
                    $mdDialog.hide();
                    appMatrixCopyControl.copyAppMatrixReplaceConfirmedYes(sourcePgs, currIndex, ev);
                };
            };
            AppMatrixCopyController.prototype.copyAppMatrix = function (ev) {
                if (!this.isCopyAppMatrixDataValid()) {
                    this.displayErrorMsg(this.locEnterMandatoryValuesText);
                    return;
                }
                this.isCoppied = false;
                var sourcePgs = this.$filter("filter")(this.purchaseGroupSourceList, { is_coppied: true }, true);
                if (sourcePgs.length === 0) {
                    this.showAlert(this.locWarningText, this.locNoSourcePgIsSelectedText, this.locCloseText);
                    return;
                }
                this.$mdDialog.show({
                    template: this.getConfirmDialogTemplate(this.locAreYouSureCopyAppMatrixText, this.locConfirmationText, this.locYesText, this.locCancelText, "confirmDialog()", "closeDialog()"),
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
            };
            AppMatrixCopyController.prototype.copyAppMatrixConfirmed = function (sourcePgs, ev) {
                this.isError = false;
                this.showLoader(this.isError);
                this.savePgWithCheck(sourcePgs, 0, ev);
            };
            AppMatrixCopyController.prototype.savePgWithCheck = function (sourcePgs, currIndex, ev) {
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
                    this.$mdDialog.show({
                        template: this.getConfirmDialogTemplate(this.locAreYouSureCopyAppMatrixText, this.locConfirmationText, this.locYesText, this.locCancelText, "confirmDialog()", "closeDialog()"),
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
                }
                else {
                    this.savePg(sourcePgs, currIndex, ev);
                }
            };
            AppMatrixCopyController.prototype.copyAppMatrixReplaceConfirmedYes = function (sourcePgs, currIndex, ev) {
                this.savePg(sourcePgs, currIndex, ev);
            };
            AppMatrixCopyController.prototype.copyAppMatrixReplaceConfirmedNo = function (sourcePgs, currIndex, ev) {
                if (currIndex < (sourcePgs.length - 1)) {
                    currIndex++;
                    this.savePg(sourcePgs, currIndex, ev);
                }
                else {
                    this.reloadTargetAppMatrix();
                    return;
                }
            };
            AppMatrixCopyController.prototype.reloadTargetAppMatrix = function () {
                angular.element("#spanLoading").html(this.locLoadingDataText);
                if (this.isCoppied === true) {
                    this.showToast(this.locDataWasSavedText);
                    this.displayAppMatrix(false, false);
                }
                else {
                    this.hideLoader();
                }
            };
            AppMatrixCopyController.prototype.isAllPgChecked = function () {
                if (this.isValueNullOrUndefined(this.purchaseGroupSourceList)) {
                    return false;
                }
                for (var i = 0; i < this.purchaseGroupSourceList.length; i++) {
                    if (this.purchaseGroupSourceList[i].is_coppied === false && this.purchaseGroupSourceList[i].is_disabled === false) {
                        return false;
                    }
                }
                return true;
            };
            AppMatrixCopyController.prototype.isPgIndeterminate = function () {
                if (this.isValueNullOrUndefined(this.purchaseGroupSourceList)) {
                    return false;
                }
                return (this.purchaseGroupSourceList.length !== 0 &&
                    !this.isAllPgChecked());
            };
            AppMatrixCopyController.prototype.togglePgCopy = function (pgItem, event) {
                var pg = this.$filter("filter")(this.purchaseGroupSourceList, { id: pgItem.id }, true);
                if (pg.length > 0) {
                    if (pg[0].is_disabled === false) {
                        pg[0].is_coppied = !pg[0].is_coppied;
                    }
                    else {
                        var msg = this.locTargetPgDoubledText.replace('{0}', pg[0].parent_pg_loc_name);
                        this.showAlert(this.locWarningText, msg, this.locCloseText);
                    }
                }
                this.stopPropagation(event);
            };
            AppMatrixCopyController.prototype.toggleAllPgCopy = function () {
                var isChecked = true;
                if (this.isAllPgChecked()) {
                    isChecked = false;
                }
                for (var i = 0; i < this.purchaseGroupSourceList.length; i++) {
                    if (this.purchaseGroupSourceList[i].is_disabled === true) {
                        continue;
                    }
                    this.purchaseGroupSourceList[i].is_coppied = isChecked;
                }
            };
            AppMatrixCopyController.prototype.isSourcePgNotEmpty = function () {
                return (this.isCgPurchaseGroupsSourceLoaded === false ||
                    this.purchaseGroupSourceList.length > 0);
            };
            AppMatrixCopyController.prototype.isTargetPgNotEmpty = function () {
                return (this.isCgPurchaseGroupsTargetLoaded === false ||
                    this.purchaseGroupTargetList.length > 0);
            };
            ;
            return AppMatrixCopyController;
        }(RegetApp.BaseRegetTs));
        RegetApp.AppMatrixCopyController = AppMatrixCopyController;
        angular.
            module('RegetApp').
            controller('AppMatrixCopyController', Kamsyk.RegetApp.AppMatrixCopyController);
        var CentreGroupAppMatrixCopy = /** @class */ (function () {
            function CentreGroupAppMatrixCopy() {
            }
            return CentreGroupAppMatrixCopy;
        }());
        RegetApp.CentreGroupAppMatrixCopy = CentreGroupAppMatrixCopy;
        var CoppiedPg = /** @class */ (function () {
            function CoppiedPg() {
                this.sourcePgId = null;
                this.isNew = false;
                this.targetCgId = null;
            }
            return CoppiedPg;
        }());
        RegetApp.CoppiedPg = CoppiedPg;
    })(RegetApp = Kamsyk.RegetApp || (Kamsyk.RegetApp = {}));
})(Kamsyk || (Kamsyk = {}));
//# sourceMappingURL=reget-app-matrix-copy.js.map