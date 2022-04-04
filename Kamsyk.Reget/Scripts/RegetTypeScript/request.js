/// <reference path="../RegetTypeScript/Base/reget-base.ts" />
/// <reference path="../RegetTypeScript/Base/reget-common.ts" />
/// <reference path="../RegetTypeScript/Base/reget-entity.ts" />
/// <reference path="../RegetTypeScript/Base/reget-base-grid.ts" />
/// <reference path="../RegetTypeScript/Base/reget-data-convert.ts" />
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
/// <reference path="../typings/ui-grid/ui-grid.d.ts" />
/// <reference path="../typings/ng-file-upload/ng-file-upload.d.ts" />
var Kamsyk;
(function (Kamsyk) {
    var RegetApp;
    (function (RegetApp) {
        var app = angular.module('RegetApp', ['ngMaterial', 'ngMessages', 'ngFileUpload', 'ui.grid', 'ui.grid.pagination', 'ui.grid.resizeColumns', 'ui.grid.selection', 'ui.grid.moveColumns', 'ui.grid.edit']);
        angular.module('RegetApp').directive("customfield", function () {
            return {
                scope: {
                    containerid: '@',
                    label: '@',
                    leftlabelcss: '@',
                    isreadonly: '@',
                    text: '='
                },
                templateUrl: RegetCommonTs.getRegetRootUrl() + 'Content/Html/CustomField/AngCustomfield.html'
            };
        });
        angular.module('RegetApp').directive("shiptoaddress", function () {
            return {
                scope: {
                    isonlyone: '@',
                    //ischecked: '@',
                    addressitem: '=',
                    addresscheck: '&'
                },
                templateUrl: RegetCommonTs.getRegetRootUrl() + 'Content/Html/AngShipToAddress.html'
            };
        });
        angular.module('RegetApp').directive("discussion", function () {
            return {
                scope: {
                    newitem: '=',
                    discussionitems: '=',
                    additem: '&',
                    addtext: '@',
                    commenttext: '@'
                },
                templateUrl: RegetCommonTs.getRegetRootUrl() + 'Content/Html/AngDiscussion.html'
            };
        });
        var RequestController = /** @class */ (function (_super) {
            __extends(RequestController, _super);
            //*****************************************************************************
            ////************************************************************
            ////Other Texts
            //private dateTimeMomentFormatText = $("#DateTimeMomentFormatText").val();
            ////************************************************************
            //**********************************************************
            //Constructor
            function RequestController($scope, $http, $filter, $mdDialog, $mdToast, uiGridConstants, $q, $timeout, Upload, $compile) {
                var _this = _super.call(this, $scope, $http, $filter, $mdDialog, $mdToast, $q, uiGridConstants, $timeout) || this;
                _this.$scope = $scope;
                _this.$http = $http;
                _this.$filter = $filter;
                _this.$mdDialog = $mdDialog;
                _this.$mdToast = $mdToast;
                _this.uiGridConstants = uiGridConstants;
                _this.$q = $q;
                _this.$timeout = $timeout;
                _this.Upload = Upload;
                _this.$compile = $compile;
                //**********************************************************
                //Properties
                _this.request = null;
                _this.requestorCentres = null;
                _this.purchaseGroupsCmb = null;
                _this.purchaseGroups = null;
                _this.currencyList = null;
                _this.itemCurrencyList = null;
                _this.currencyListGrid = null;
                _this.unitList = null;
                _this.unitListGrid = null;
                _this.privacyList = null;
                _this.exchangeRates = null;
                _this.shipToAddress = null;
                _this.isRequestorCentreLoaded = false;
                _this.isPgsLoaded = false;
                _this.isCurrenciesLoaded = false;
                _this.isUnitsLoaded = false;
                _this.isPrivacyLoaded = false;
                _this.isRequestLoaded = false;
                _this.isExchangeRateLoaded = false;
                _this.isShipToAddressLoaded = false;
                _this.isDiscussionLoaded = false;
                _this.isNewRequest = false;
                _this.isNewRequestOrDraft = false;
                _this.isRequestorCentreReadOnly = true;
                _this.iRequestId = null;
                _this.selectedCentreId = null;
                _this.cgId = null;
                _this.pgId = null;
                _this.currencyId = null;
                _this.privacyId = null;
                _this.supplierId = null;
                _this.requestText = null;
                _this.supplierText = null;
                _this.requestRemark = null;
                _this.requestNr = null;
                _this.requestorName = null;
                _this.selectedCmbPg = null;
                _this.selectedPg = null;
                _this.selectedOrdererId = null;
                _this.selectedOrdererName = null;
                _this.selectedSupplier = null;
                _this.selectedDeliveryDate = null;
                _this.strSelectedSupplier = null;
                _this.searchstringsupplier = null;
                _this.centreName = null;
                _this.pgName = null;
                _this.selectedShipToAddress = null;
                _this.currencyCode = null;
                _this.itemCurrencyCode = null;
                _this.unitCode = null;
                //private currentUserName: string = $("#txtCurrentUserName").val();
                //private currentUserId: number = parseInt($("#txtCurrentUserId").val());
                //private currentUserPhotoUrl: string = $("#txtCurrentUserPhotoUrl").val();
                _this.pgType = RegetApp.PurchaseGroupType.Unknow;
                _this.isPgItemOrder = false;
                _this.isPgStandard = false;
                _this.requestItemName = null;
                _this.requestItemAmount = null;
                _this.requestItemUnitPrice = null;
                _this.requestItemCurrencyId = null;
                _this.requestItemUnitId = null;
                _this.isCurrencyRO = false;
                _this.errRequestItem = null;
                _this.isRequestItemsDisplayed = false;
                _this.isFiltersSet = false;
                //private progressPercentage: number = null;
                //private isUploadFileProgressBarVisible: boolean = false;
                _this.centre_currency_id = null;
                _this.is_approval_needed = false;
                _this.is_order_needed = false;
                _this.is_self_ordered = false;
                _this.is_multi_orderer_cmb_dispalyed = false;
                _this.shipAddressIsOnlyOne = false;
                _this.isCurrencyOnlyOne = false;
                _this.isCustomFieldVisible = false;
                _this.isPrivacyReadOnly = true;
                _this.isCustomFieldReadOnly = false;
                _this.privacyName = null;
                _this.customFields = null;
                //private tmpContractNr: string = "ssdfdf";
                //private custFieldValues: any[] = null;
                _this.requestAppMen = null;
                // private pgLimits: PurchaseGroupLimit[] = null;
                _this.requestPgLimits = null;
                _this.orderers = null;
                //private isMandatoryCheck: boolean = false;
                _this.isKeepErrMsgHidden = true;
                _this.requestErrorMsg = null;
                _this.isAppManOrdererDisplayed = false;
                _this.isAttachmentDisplayed = true;
                _this.isAttachmentEditable = false;
                _this.isAddressDisplayed = true;
                _this.requestDiscussion = null;
                _this.isDiscussionVisible = false;
                _this.isSendForApprovalVisible = true;
                _this.isRevertVisible = false;
                _this.isDeleteVisible = false;
                _this.newRemarkItem = null;
                _this.discussion = null;
                _this.remindText = null;
                _this.remindConfirmText = null;
                _this.isWorkflowDisplayed = false;
                _this.isActivatedDisplayed = false;
                _this.isRequestCanceled = false;
                _this.isRequestRejected = false;
                _this.isFreeSupplierAllowed = false;
                _this.isFreeSupplier = false;
                _this.accessRequest = null;
                _this.isAccessRequestSent = false;
                _this.isAccessRequestSentWaitForApproval = false;
                _this.isAccessRequestSentRejected = false;
                _this.strAccessRequestDate = null;
                _this.isWorkflowRemindDisplayed = false;
                _this.isApprovable = false;
                _this.isGenerateOrderAvailable = false;
                _this.appStatusApproved = RegetApp.ApprovalStatus.Approved;
                _this.appStatusRejected = RegetApp.ApprovalStatus.Rejected;
                //private imgStatusUrl: string = null;
                //**********************************************************
                //***************************************************************************
                //Localization
                _this.locMissingRequestItemNameText = $("#MissingRequestItemNameText").val();
                _this.locMissingRequestAmountText = $("#MissingRequestAmountText").val();
                _this.locMissingRequestUnitText = $("#MissingRequestUnitText").val();
                _this.locMissingRequestPriceText = $("#MissingRequestPriceText").val();
                _this.locMissingRequestCurrencyText = $("#MissingRequestCurrencyText").val();
                _this.locMissingRequestText = $("#MissingRequestText").val();
                _this.locItemNameText = $("#ItemNameText").val();
                _this.locAmountText = $("#AmountText").val();
                _this.locUnitText = $("#UnitText").val();
                _this.locUnitPriceText = $("#UnitPriceText").val();
                _this.locCurrencyText = $("#CurrencyText").val();
                _this.locNotAuthorizedPerformActionText = $("#NotAuthorizedPerformActionText").val();
                _this.locExchangeRateMissingText = $("#ExchangeRateMissingText").val();
                _this.locSelectCentreText = $("#SelectCentreText").val();
                //private ĺocFileSizeOverLimitText: string = $("#FileSizeOverLimitText").val();
                _this.locEnterMandatoryValuesText = $("#EnterMandatoryValuesText").val();
                _this.locRevertText = $("#RevertText").val();
                _this.locRevertRequestConfirmText = $("#RevertRequestConfirmText").val();
                _this.locRequestWasRevertedText = $("#RequestWasRevertedText").val();
                _this.locRemindAppManText = $("#RemindAppManText").val();
                _this.locRemindOrdererText = $("#RemindOrdererText").val();
                _this.locResendApprovalRequestConfirmText = $("#ResendApprovalRequestConfirmText").val();
                _this.locResendOrderRequestConfirmText = $("#ResendOrderRequestConfirmText").val();
                _this.locSendText = $("#SendText").val();
                _this.locSentText = $("#SentText").val();
                _this.locCanceledConfirmText = $("#CanceledConfirmText").val();
                _this.locCanceledByRequestorText = $("#CanceledByRequestorText").val();
                _this.locActivateConfirmText = $("#ActivateConfirmText").val();
                _this.locFreeSupplierText = $("#FreeSupplierText").val();
                _this.locAskAcessRequestConfirmText = $("#AskAcessRequestConfirmText").val();
                _this.locAccessRequestWasSentText = $("#AccessRequestWasSentText").val();
                _this.locApproveConfirmText = $("#ApproveConfirmText").val();
                _this.locRejectConfirmText = $("#RejectConfirmText").val();
                _this.locApprovedText = $("#ApprovedText").val();
                _this.locRejectedText = $("#RejectedText").val();
                //***************************************************************
                _this.$onInit = function () { };
                _this.dbGridId = "grdRequestItems_rg";
                _this.deleteUrl = null; //this.getRegetRootUrl() + "Centre/DeleteCentre" + "?t=" + new Date().getTime();
                //there must be - let app = angular.module('RegetApp', ['ngFileUpload' .. in scrit wher upload file is used e.g. request.ts
                _this.fileUploadInit($scope, _this.Upload);
                /*
                if (!this.isValueNullOrUndefined($scope)) { //because of jasmine test
                    $scope.$watch('files', () => {
                        $scope.upload($scope.files);
                    });
                    $scope.$watch('file', () => {
                        if ($scope.file != null) {
                            $scope.files = [$scope.file];
                        }
                    });
                                   
                    $scope.upload = (files: any[]) => {
                        if (!this.isValueNullOrUndefined(files) && files.length > 0) {
                            let iUploadFinishedCount: number = 0;
                            this.isUploadFileProgressBarVisible = true;
                            for (let i: number = 0; i < files.length; i++) {
    
                                let file = files[i];
                                if (file.size > 26214400) {
                                    //alert(file.size);
                                    let fileLenghtMb: number = (file.size / (1024 * 1024));
                                    let msg: string = this.ĺocFileSizeOverLimitText
                                        .replace("{0}", "'" + file.name + "'")
                                        .replace("{1}", fileLenghtMb.toFixed(2) + " MB")
                                        .replace("{2}", "25" + " MB");
                                    this.displayErrorMsg(msg);
                                    iUploadFinishedCount++;
                                    if (iUploadFinishedCount == files.length) {
                                        this.isUploadFileProgressBarVisible = false;
                                    }
                                    continue;
                                }
                                let config: ng.angularFileUpload.IFileUploadConfigFile = {
                                    url: this.getRegetRootUrl() + "Attachment/UploadAttachment",
                                    data: { file: file },
                                    method: "POST"
                                };
    
                                Upload.upload(config).progress((evt: any) => {
                                    this.progressPercentage = 100.0 * parseInt(evt.loaded) / parseInt(evt.total);
    
                                }).success((data: any, status: any, headers: any, config: any) => {
                                    iUploadFinishedCount++;
                                    let attUpload: Attachment = data;
                                    attUpload.is_can_be_deleted = true;
                                    if (this.isValueNullOrUndefined(this.request.attachments)) {
                                        this.request.attachments = [];
                                    }
                                    this.request.attachments.push(attUpload);
                                    this.progressPercentage = null;
                                    if (iUploadFinishedCount == files.length) {
                                        this.isUploadFileProgressBarVisible = false;
                                    }
    
                                    //alert("finished");
                                }).catch((callback: any) => {
                                    this.displayErrorMsg();
                                    this.progressPercentage = null;
                                    this.isUploadFileProgressBarVisible = false;
                                });
    
    
    
                                //**************** Works ************************************************
                                //var UploadAny: any = Upload;
                                //var upld = Upload;
                                //var file = files[i];
                                //UploadAny.upload({
                                //    //url: 'https://angular-file-upload-cors-srv.appspot.com/upload',
                                //    url: 'http://localhost:26077/FileUpload/',
                                //    fields: {
                                //        'username': ""
                                //    },
                                //    file: file
                                //}).progress(function (evt) {
                                //    //var progressPercentage = parseInt(100.0 * evt.loaded / evt.total);
                                //    var progressPercentage = 100.0 * parseInt(evt.loaded) / parseInt(evt.total);
                                //    alert(progressPercentage);
                                //    //$scope.log = 'progress: ' + progressPercentage + '% ' +
                                //    //    evt.config.file.name + '\n' + $scope.log;
                                //}).success(function (data, status, headers, config) {
                                //    $timeout(function () {
                                //        //$scope.log = 'file: ' + config.file.name + ', Response: ' + JSON.stringify(data) + '\n' + $scope.log;
                                //        alert("Loaded");
                                //    });
                                //});
                                //**************** Works ************************************************
                            }
    
    
                        }
                    };
                }*/
                //window.addEventListener('scroll', function () {
                //    alert("scrol");
                //    document.getElementById('showScroll').innerHTML = window.pageYOffset + 'px';
                //});
                _this.loadInit();
                return _this;
            }
            //******************************************************************
            //Methods
            //************************************************************************
            //Overide
            RequestController.prototype.cellClicked = function (row, col) {
                if (this.isFiltersSet === false) {
                    this.setGridColumFilters();
                }
                _super.prototype.cellClicked.call(this, row, col);
            };
            RequestController.prototype.gridSaveRow = function () {
                if (this.isValueNullOrUndefined(this.request.id)) {
                    this.editRowChanged(false);
                    this.setEstPrice();
                    return true;
                }
                else {
                    return _super.prototype.gridSaveRow.call(this);
                }
            };
            //************************************************************************
            //****************************************************************************
            //Abstract
            RequestController.prototype.exportToXlsUrl = function () {
                return this.getRegetRootUrl() + "Report/GetRequestItemsReport?" +
                    "filter=" + encodeURI(this.filterUrl) +
                    "&sort=" + this.sortColumnsUrl +
                    "&t=" + new Date().getTime();
            };
            RequestController.prototype.getControlColumnsCount = function () {
                return 2;
            };
            RequestController.prototype.getDuplicityErrMsg = function (centre) {
                if (this.isNewRequest) {
                    return null;
                }
                return null; //this.getLocDuplicityCentreNameText().replace("{0}", centre.name);
            };
            RequestController.prototype.getSaveRowUrl = function () {
                return null; //this.getRegetRootUrl() + "Centre/SaveCentreData?t=" + new Date().getTime();
            };
            RequestController.prototype.insertRow = function () {
            };
            RequestController.prototype.isRowChanged = function () {
                if (this.editRow === null) {
                    return true;
                }
                //var origCentre: CentreAdminExtended = this.editRowOrig;
                //var updCentre: CentreAdminExtended = this.editRow;
                //var tmpCentres: any = this.gridOptions.data;
                //var id: number = updCentre.id;
                ////var centre: CentreAdminExtended[] = this.$filter("filter")(centres, { id: id }, true);
                //if (id < 0) {
                //    //new row
                //    this.newRowIndex = null;
                //    return true;
                //} else {
                //    if (origCentre.name !== updCentre.name) {
                //        return true;
                //    } else if (origCentre.company_name !== updCentre.company_name) {
                //        return true;
                //    } else if (origCentre.export_price_text !== updCentre.export_price_text) {
                //        return true;
                //    } else if (origCentre.multi_orderer !== updCentre.multi_orderer) {
                //        return true;
                //    } else if (origCentre.other_orderer_can_takeover !== updCentre.other_orderer_can_takeover) {
                //        return true;
                //    } else if (origCentre.is_approved_by_requestor !== updCentre.is_approved_by_requestor) {
                //        return true;
                //    } else if (origCentre.manager_id !== updCentre.manager_id) {
                //        return true;
                //    } else if (origCentre.address_text !== updCentre.address_text) {
                //        return true;
                //    } else if (origCentre.active !== updCentre.active) {
                //        return true;
                //    }
                //}
                return false;
            };
            RequestController.prototype.isRowEntityValid = function (centre) {
                if (this.isStringValueNullOrEmpty(centre.name)) {
                    return this.locMissingMandatoryText;
                }
                return null;
            };
            RequestController.prototype.loadGridData = function () {
                //this.getCentresData();
            };
            RequestController.prototype.getMsgDisabled = function (centre) {
                return null; //this.locCentreWasDisabledText.replace("{0}", centre.name);
            };
            RequestController.prototype.getMsgDeleteConfirm = function (centre) {
                return null; // this.locDeleteCentreConfirmText.replace("{0}", centre.name);;
            };
            RequestController.prototype.getErrorMsgByErrId = function (errId, msg) {
                return this.locErrorMsgText;
            };
            RequestController.prototype.getDbGridId = function () {
                return this.dbGridId;
            };
            //*****************************************************************************
            //******************************************************************
            //Http
            RequestController.prototype.getRequestCentres = function () {
                var _this = this;
                this.showLoader(this.isError);
                this.$http.get(this.getRegetRootUrl() + "Request/GetRequestCentres?t=" + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        _this.requestorCentres = tmpData;
                        _this.populateCentre();
                        //this.loadUnits();
                        _this.isRequestorCentreLoaded = true;
                        _this.hideLoaderWrapper();
                    }
                    catch (e) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        _this.hideLoaderWrapper();
                    }
                }, function (response) {
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
            };
            RequestController.prototype.loadPgs = function () {
                var _this = this;
                this.showLoader(this.isError);
                if (this.isValueNullOrUndefined(this.selectedCentreId)) {
                    this.isPgsLoaded = true;
                    this.isCurrenciesLoaded = true;
                    this.isExchangeRateLoaded = true;
                    this.isUnitsLoaded = true;
                    this.isPrivacyLoaded = true;
                    this.isShipToAddressLoaded = true;
                    return;
                }
                var requestId = -1;
                if (!this.isValueNullOrUndefined(this.request)) {
                    requestId = this.request.id;
                }
                this.$http.get(this.getRegetRootUrl() + "Request/GetPurchaseGroups?centreId=" + this.selectedCentreId
                    + "&requestId=" + requestId
                    + "&t=" + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        _this.purchaseGroups = tmpData;
                        _this.loadCurrencies();
                        _this.setPurchaseGroupCmbList();
                        _this.selectedPg = null;
                        _this.requestAppMen = null;
                        _this.orderers = null;
                        _this.is_approval_needed = false;
                        _this.is_order_needed = false;
                        //this.isAppManOrdererDisplayed = false;
                        _this.isPgsLoaded = true;
                        _this.hideLoaderWrapper();
                    }
                    catch (e) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        _this.hideLoaderWrapper();
                    }
                }, function (response) {
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
                var tmpCentre = this.$filter("filter")(this.requestorCentres, { id: (this.selectedCentreId) }, true);
                if (!this.isValueNullOrUndefined(tmpCentre[0]) && tmpCentre[0].is_free_supplier_allowed) {
                    if (this.isNewRequestOrDraft) {
                        this.isFreeSupplierAllowed = true;
                    }
                    else {
                        this.isFreeSupplierAllowed = (this.request.use_supplier_list);
                    }
                }
                else {
                    this.isFreeSupplierAllowed = false;
                }
            };
            RequestController.prototype.loadCurrencies = function () {
                var _this = this;
                this.showLoader(this.isError);
                //this.currencyId = null;
                this.$http.get(this.getRegetRootUrl() + "Request/GetCurrencies?centreId=" + this.selectedCentreId + "&t=" + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        _this.currencyList = tmpData.currency_drop_down;
                        _this.centre_currency_id = tmpData.default_currency_id;
                        _this.currencyListGrid = [];
                        _this.itemCurrencyList = [];
                        if (!_this.isValueNullOrUndefined(_this.currencyList)) {
                            for (var i = 0; i < _this.currencyList.length; i++) {
                                var grdDropDowmItem = {
                                    label: _this.currencyList[i].name,
                                    id: _this.currencyList[i].name,
                                    value: _this.currencyList[i].name
                                };
                                var cmbCurrencyItem = {
                                    id: _this.currencyList[i].id,
                                    name: _this.currencyList[i].name,
                                    is_disabled: false
                                };
                                _this.itemCurrencyList.push(cmbCurrencyItem);
                                _this.currencyListGrid.push(grdDropDowmItem);
                            }
                        }
                        if (!_this.isValueNullOrUndefined(_this.itemCurrencyList) && _this.itemCurrencyList.length === 1) {
                            _this.isCurrencyOnlyOne = true;
                            _this.currencyId = _this.itemCurrencyList[0].id;
                            _this.currencyCode = _this.itemCurrencyList[0].name;
                        }
                        else {
                            _this.isCurrencyOnlyOne = false;
                        }
                        _this.isCurrenciesLoaded = true;
                        _this.loadExchangeRates();
                        _this.hideLoaderWrapper();
                    }
                    catch (e) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        _this.hideLoaderWrapper();
                    }
                }, function (response) {
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
            };
            RequestController.prototype.loadExchangeRates = function () {
                var _this = this;
                this.showLoader(this.isError);
                this.$http.get(this.getRegetRootUrl() + "Request/GetExchangeRates?t=" + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        _this.exchangeRates = tmpData;
                        _this.loadUnits();
                        _this.isExchangeRateLoaded = true;
                        _this.hideLoaderWrapper();
                    }
                    catch (e) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        _this.hideLoaderWrapper();
                    }
                }, function (response) {
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
            };
            RequestController.prototype.loadUnits = function () {
                var _this = this;
                this.showLoader(this.isError);
                this.$http.get(this.getRegetRootUrl() + "Request/GetUnitsOfMeasurement?t=" + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        _this.unitList = tmpData;
                        _this.unitListGrid = [];
                        for (var i = 0; i < _this.unitList.length; i++) {
                            var grdDropDowmItem = {
                                label: _this.unitList[i].name,
                                id: _this.unitList[i].name,
                                value: _this.unitList[i].name
                            };
                            _this.unitListGrid.push(grdDropDowmItem);
                        }
                        _this.loadPrivacies(false);
                        _this.isUnitsLoaded = true;
                        _this.hideLoaderWrapper();
                    }
                    catch (e) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        _this.hideLoaderWrapper();
                    }
                }, function (response) {
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
            };
            RequestController.prototype.loadPrivacies = function (isPrivacyOnly) {
                var _this = this;
                this.showLoader(this.isError);
                this.$http.get(this.getRegetRootUrl() + "Request/GetPrivacyItems?t=" + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        _this.privacyList = tmpData;
                        if (isPrivacyOnly) {
                            _this.isPrivacyLoaded = true;
                            return;
                        }
                        _this.loadShipToAddress();
                        _this.isPrivacyLoaded = true;
                        _this.hideLoaderWrapper();
                    }
                    catch (e) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        _this.hideLoaderWrapper();
                    }
                }, function (response) {
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
            };
            RequestController.prototype.loadShipToAddress = function () {
                var _this = this;
                this.showLoader(this.isError);
                this.$http.get(this.getRegetRootUrl() + "Request/GetRequestorAddress?centreId=" + this.selectedCentreId + "&t=" + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        _this.shipToAddress = tmpData;
                        _this.selectPg();
                        if (!_this.isValueNullOrUndefined(_this.shipToAddress) && _this.shipToAddress.length === 1) {
                            _this.shipAddressIsOnlyOne = true;
                        }
                        else {
                            _this.shipAddressIsOnlyOne = false;
                            _this.tickAddress();
                        }
                        _this.isShipToAddressLoaded = true;
                        _this.hideLoaderWrapper();
                    }
                    catch (e) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        _this.hideLoaderWrapper();
                    }
                }, function (response) {
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
            };
            RequestController.prototype.deleteAttachment = function (id) {
                var _this = this;
                if (this.isNewRequest === false) {
                    this.showLoader(this.isError);
                }
                var jsonId = JSON.stringify({ id: id });
                this.$http.post(this.getRegetRootUrl() + "Attachment/DeleteAttachment?t=" + new Date().getTime(), jsonId).then(function (response) {
                    try {
                        var tmpData = response.data;
                        var httpResult = tmpData;
                        _this.hideLoaderWrapper();
                        if (_this.isValueNullOrUndefined(httpResult.error_id) || httpResult.error_id === 0) {
                            //Deleted
                            for (var i = _this.request.attachments.length - 1; i >= 0; i--) {
                                if (_this.request.attachments[i].id === id) {
                                    _this.request.attachments.splice(i, 1);
                                    break;
                                }
                            }
                        }
                        else {
                            if (httpResult.error_id == 110) {
                                //Not Auhorized
                                _this.displayErrorMsg(_this.locNotAuthorizedPerformActionText);
                            }
                            else {
                                _this.displayErrorMsg();
                            }
                        }
                    }
                    catch (e) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        _this.hideLoaderWrapper();
                    }
                }, function (response) {
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
            };
            RequestController.prototype.saveRequestDraft = function () {
                var _this = this;
                this.showLoaderBoxOnly(this.isError);
                var jsonRequest = JSON.stringify(this.request);
                this.$http.post(this.getRegetRootUrl() + "Request/SaveRequestDraft?t=" + new Date().getTime(), jsonRequest).then(function (response) {
                    try {
                        var result = response.data;
                        var strResult = result.string_value;
                        var requestId = result.request_id;
                        var requestNr = result.request_nr;
                        if (_this.isStringValueNullOrEmpty(strResult)) {
                            _this.isNewRequest = false;
                            _this.request.id = requestId;
                            _this.request.request_nr = requestNr;
                            _this.showToast(_this.locDataWasSavedText);
                        }
                        else {
                            var msgError = _this.getErrorMessage(strResult);
                            _this.showAlert(_this.locErrorTitleText, msgError, _this.locCloseText);
                        }
                        _this.hideLoaderWrapper();
                    }
                    catch (e) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        _this.hideLoaderWrapper();
                    }
                }, function (response) {
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
            };
            RequestController.prototype.sendForApprovalPost = function () {
                var _this = this;
                this.showLoaderBoxOnly(this.isError);
                var jsonRequest = JSON.stringify(this.request);
                this.$http.post(this.getRegetRootUrl() + "Request/SendForApproval?t=" + new Date().getTime(), jsonRequest).then(function (response) {
                    try {
                        var result = response.data;
                        var strResult = result.string_value;
                        if (_this.isStringValueNullOrEmpty(strResult)) {
                            _this.showToast(_this.locDataWasSavedText);
                            var requestId = result.request_id;
                            window.location.href = _this.getRegetRootUrl() + "Request?id=" + requestId;
                        }
                        else {
                            var msgError = _this.getErrorMessage(strResult);
                            if (msgError.trim().length == 0) {
                                msgError = null;
                            }
                            //this.showAlert(this.locErrorTitleText, msgError, this.locCloseText);
                            _this.displayErrorMsg(msgError);
                        }
                        _this.hideLoaderWrapper();
                    }
                    catch (e) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        _this.hideLoaderWrapper();
                    }
                }, function (response) {
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
            };
            RequestController.prototype.loadRequest = function () {
                var _this = this;
                this.showLoaderBoxOnly(this.isError);
                this.$http.get(this.getRegetRootUrl() + "Request/GetRequest?requestId=" + this.iRequestId + "&t=" + new Date().getTime(), {}).then(function (response) {
                    try {
                        var result = response.data;
                        if (!_this.getRequestLoadedStatus(result)) {
                            return;
                        }
                        _this.request = result;
                        _this.requestNr = _this.request.request_nr;
                        _this.selectedCentreId = _this.request.request_centre_id;
                        _this.centreName = _this.request.centre_name;
                        _this.pgId = _this.request.purchase_group_id;
                        _this.pgName = _this.request.pg_name;
                        _this.currencyId = _this.request.currency_id;
                        _this.currencyCode = _this.request.currency_code;
                        if (_this.request.id < 0) {
                            _this.isNewRequest = true;
                        }
                        if (_this.request.id < 0 || _this.request.request_status === RegetApp.RequestStatus.Draft) {
                            _this.isNewRequestOrDraft = true;
                        }
                        else {
                            _this.isNewRequestOrDraft = false;
                        }
                        _this.privacyId = _this.request.privacy_id;
                        _this.supplierId = _this.request.supplier_id;
                        _this.isFreeSupplier = !_this.request.use_supplier_list;
                        _this.supplierText = _this.request.supplier_remark;
                        if (_this.isFreeSupplier && !_this.isNewRequestOrDraft) {
                            if (_this.isStringValueNullOrEmpty(_this.supplierText)) {
                                _this.supplierText = _this.locFreeSupplierText;
                            }
                            else {
                                _this.supplierText = _this.locFreeSupplierText
                                    + " \n" + _this.supplierText;
                            }
                        }
                        _this.selectedDeliveryDate = _this.convertJsonDateToJs(_this.request.lead_time);
                        _this.requestText = _this.request.request_text;
                        _this.isPrivacyReadOnly = (_this.request.requestor !== _this.currentUserId && _this.isNewRequest === false);
                        _this.isCustomFieldReadOnly = (_this.request.requestor !== _this.currentUserId || _this.isNewRequestOrDraft === false);
                        _this.requestorName = _this.request.requestor_name_surname_first;
                        _this.isRequestLoaded = true;
                        _this.isDiscussionLoaded = true;
                        _this.isPrivacyLoaded = true;
                        _this.privacyName = _this.request.privacy_name;
                        if (_this.isNewRequestOrDraft === true) {
                            _this.getRequestCentres();
                        }
                        if (!_this.isValueNullOrUndefined(_this.supplierId) && _this.supplierId > -1) {
                            var promSupp = _this.filterSuppliersById(_this.supplierId);
                            promSupp.then(function (supp) {
                                if (!_this.isValueNullOrUndefined(supp)) {
                                    _this.selectedSupplier = supp;
                                    _this.strSelectedSupplier = _this.selectedSupplier.supp_name;
                                }
                            }).catch(function () {
                                _this.displayErrorMsg();
                            });
                            //this.selectedSupplier = promSupp;
                        }
                        if (!_this.isValueNullOrUndefined(_this.request)) {
                            if (_this.request.is_approval_needed || _this.request.is_order_needed) {
                                _this.isAppManOrdererDisplayed = true;
                            }
                            else if (!_this.isValueNullOrUndefined(_this.selectedPg)) {
                                if (_this.selectedPg.is_order_needed || _this.selectedPg.is_approval_needed) {
                                    _this.isAppManOrdererDisplayed = true;
                                }
                            }
                            if (!_this.isNewRequestOrDraft) {
                                if (!_this.isValueNullOrUndefined(_this.request.request_event_approval) && _this.request.request_event_approval.length > 0) {
                                    _this.is_approval_needed = true;
                                    _this.requestAppMen = [];
                                    for (var i = 0; i < _this.request.request_event_approval.length; i++) {
                                        var manRole = new RegetApp.ManagerRole();
                                        var part = new RegetApp.Participant();
                                        part.id = _this.request.request_event_approval[i].app_man_id;
                                        part.surname = _this.request.request_event_approval[i].app_man_surname;
                                        part.first_name = _this.request.request_event_approval[i].app_man_first_name;
                                        manRole.participant = part;
                                        manRole.approve_level_id = _this.request.request_event_approval[i].app_level_id;
                                        manRole.approve_status = _this.request.request_event_approval[i].approve_status;
                                        manRole.modif_date_text = _this.request.request_event_approval[i].modif_date_text;
                                        _this.requestAppMen.push(manRole);
                                    }
                                }
                                if (!_this.isValueNullOrUndefined(_this.request.orderers) && _this.request.orderers.length > 0) {
                                    _this.is_order_needed = true;
                                    _this.orderers = [];
                                    for (var i = 0; i < _this.request.orderers.length; i++) {
                                        var orderer = new RegetApp.Orderer();
                                        orderer.participant_id = _this.request.orderers[i].participant_id;
                                        orderer.surname = _this.request.orderers[i].surname;
                                        orderer.first_name = _this.request.orderers[i].first_name;
                                        _this.orderers.push(orderer);
                                    }
                                }
                            }
                            _this.setStatusImage();
                        }
                        _this.isSendForApprovalVisible = _this.isNewRequestOrDraft
                            && (_this.isNewRequest || _this.currentUserId === _this.request.requestor);
                        _this.isRevertVisible = _this.request.is_revertable && !_this.isCanceled();
                        _this.isDeleteVisible = _this.request.is_deletable;
                        _this.isApprovable = _this.request.is_approvable;
                        _this.isGenerateOrderAvailable = _this.request.is_order_available;
                        _this.setAttachmetVisibility();
                        if (_this.currentUserId == _this.request.requestor) {
                            _this.isPrivacyLoaded = false;
                            _this.loadPrivacies(true);
                        }
                        if (!_this.isNewRequestOrDraft) {
                            //load custom fields
                            _this.getPurchaseGroup();
                        }
                        _this.requestRemark = _this.request.remarks;
                        _this.isDiscussionVisible = !_this.isNewRequestOrDraft;
                        if (_this.isNewRequest === true) {
                            _this.isDiscussionLoaded = true;
                        }
                        else {
                            _this.isDiscussionLoaded = false;
                            _this.loadDiscussion();
                        }
                        if (_this.request.request_status == RegetApp.RequestStatus.WaitForApproval) {
                            _this.remindText = _this.locRemindAppManText;
                            _this.remindConfirmText = _this.locResendApprovalRequestConfirmText.replace('{0}', _this.getCurrAppMan());
                        }
                        else if (_this.request.request_status == RegetApp.RequestStatus.Approved) {
                            _this.remindText = _this.locRemindOrdererText;
                            _this.remindConfirmText = _this.locResendOrderRequestConfirmText.replace('{0}', _this.getCurrOrderer());
                        }
                        else {
                            _this.remindText = "";
                        }
                        _this.isActivatedDisplayed = false;
                        if (_this.request.request_status != RegetApp.RequestStatus.New
                            && _this.request.request_status != RegetApp.RequestStatus.CanceledOrderer
                            && _this.request.request_status != RegetApp.RequestStatus.CanceledRequestor
                            && _this.request.request_status != RegetApp.RequestStatus.CanceledSystem) {
                            _this.isWorkflowDisplayed = true;
                        }
                        else {
                            _this.isWorkflowDisplayed = false;
                            if (_this.currentUserId == _this.request.requestor) {
                                _this.isActivatedDisplayed = true;
                            }
                        }
                        _this.isWorkflowRemindDisplayed = _this.isActivatedDisplayed && _this.request.requestor == _this.currentUserId;
                        _this.isRequestCanceled = _this.isCanceled();
                        _this.isRequestRejected = (_this.request.request_status == RegetApp.RequestStatus.Rejected);
                        _this.isFreeSupplier = !_this.request.use_supplier_list;
                        //this.setIsApprovable();
                        _this.hideLoaderWrapper();
                    }
                    catch (e) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        _this.hideLoaderWrapper();
                    }
                }, function (response) {
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
            };
            RequestController.prototype.approveRequestConfirmed = function () {
                var _this = this;
                this.showLoaderBoxOnly(this.isError);
                var jsonRequestId = JSON.stringify({ requestId: this.request.id });
                this.$http.post(this.getRegetRootUrl() + "Request/Approve?t=" + new Date().getTime(), jsonRequestId).then(function (response) {
                    try {
                        var result = response.data;
                        var strResult = result.string_value;
                        if (_this.isStringValueNullOrEmpty(strResult)) {
                            //this.showToast(this.locDataWasSavedText);
                            //let requestId: number = result.request_id;
                            //window.location.href = this.getRegetRootUrl() + "Request?id=" + this.request.id;
                            _this.showToast(_this.locApprovedText);
                            _this.loadRequest();
                        }
                        else {
                            var msgError = _this.getErrorMessage(strResult);
                            if (msgError.trim().length == 0) {
                                msgError = null;
                            }
                            _this.displayErrorMsg(msgError);
                        }
                        _this.hideLoaderWrapper();
                    }
                    catch (e) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        _this.hideLoaderWrapper();
                    }
                }, function (response) {
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
            };
            RequestController.prototype.rejectRequestConfirmed = function () {
                var _this = this;
                this.showLoaderBoxOnly(this.isError);
                var jsonRequestId = JSON.stringify({ requestId: this.request.id });
                this.$http.post(this.getRegetRootUrl() + "Request/Reject?t=" + new Date().getTime(), jsonRequestId).then(function (response) {
                    try {
                        var result = response.data;
                        var strResult = result.string_value;
                        if (_this.isStringValueNullOrEmpty(strResult)) {
                            //this.showToast(this.locDataWasSavedText);
                            //let requestId: number = result.request_id;
                            //window.location.href = this.getRegetRootUrl() + "Request?id=" + this.request.id;
                            _this.showToast(_this.locRejectedText);
                            _this.loadRequest();
                        }
                        else {
                            var msgError = _this.getErrorMessage(strResult);
                            if (msgError.trim().length == 0) {
                                msgError = null;
                            }
                            _this.displayErrorMsg(msgError);
                        }
                        _this.hideLoaderWrapper();
                    }
                    catch (e) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        _this.hideLoaderWrapper();
                    }
                }, function (response) {
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
            };
            //private setIsApprovable(): void {
            //    this.isApprovable = false;
            //    if (this.request.request_status == RequestStatus.WaitForApproval) {
            //        if (!this.isValueNullOrUndefined(this.request.request_event_approval)) {
            //            let iWaitAppLevel: number = -1;
            //            for (let i: number = 0; i < this.request.request_event_approval.length; i++) {
            //                if (this.request.request_event_approval[i].approve_status == ApprovalStatus.WaitForApproval
            //                    && (iWaitAppLevel === -1 || this.request.request_event_approval[i].approve_status === iWaitAppLevel)) {
            //                    if (this.request.request_event_approval[i].app_man_id === this.currentUserId) {
            //                        this.isApprovable = true;
            //                        break;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            RequestController.prototype.sendAccessRequest = function () {
                var _this = this;
                this.showLoader(this.isError);
                //let iVersion: number = 0;
                //if (!this.isValueNullOrUndefined(this.request)) {
                //    iVersion = this.request.version;
                //}
                this.$http.get(this.getRegetRootUrl() + "Request/SendAccessRequest?requestId=" + this.iRequestId
                    //+ "&requestVersion = " + iVersion
                    + "&t = " + new Date().getTime(), {}).then(function (response) {
                    try {
                        var httpResult = response.data;
                        var strResult = httpResult.string_value;
                        if (_this.isStringValueNullOrEmpty(strResult)) {
                            _this.showToast(_this.locAccessRequestWasSentText);
                            _this.getRequestAccessRequests();
                        }
                        else {
                            if (httpResult.error_id == 110) {
                                //Not Auhorized - 
                                _this.displayErrorMsg(_this.locNotAuthorizedPerformActionText);
                            }
                            else {
                                _this.displayErrorMsg();
                            }
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
            RequestController.prototype.tickAddress = function () {
                if (this.isNewRequestOrDraft && !this.isNewRequest) {
                    //set ship to address
                    if (!this.isValueNullOrUndefined(this.shipToAddress)) {
                        for (var i = 0; i < this.shipToAddress.length; i++) {
                            var isTheSame = true;
                            if (this.request.ship_to_address_company_name != this.shipToAddress[i].company_name) {
                                isTheSame = false;
                                continue;
                            }
                            if (this.request.ship_to_address_street != this.shipToAddress[i].street) {
                                isTheSame = false;
                                continue;
                            }
                            if (this.request.ship_to_address_city != this.shipToAddress[i].city) {
                                isTheSame = false;
                                continue;
                            }
                            if (this.request.ship_to_address_zip != this.shipToAddress[i].zip) {
                                isTheSame = false;
                                continue;
                            }
                            if (isTheSame) {
                                this.shipToAddress[i].is_selected = true;
                                this.selectedShipToAddress = this.shipToAddress[i];
                                break;
                            }
                        }
                    }
                }
            };
            RequestController.prototype.setStatusImage = function () {
                var imgStatusUrl = null;
                if (this.request.request_status == RegetApp.RequestStatus.Draft) {
                    imgStatusUrl = this.getRegetRootUrl() + "Content/Images/Request/RequestStatus/Draft.png";
                }
                else if (this.request.request_status == RegetApp.RequestStatus.WaitForApproval) {
                    imgStatusUrl = this.getRegetRootUrl() + "Content/Images/Request/RequestStatus/WaitForApproval.png";
                }
                else if (this.request.request_status == RegetApp.RequestStatus.Approved) {
                    imgStatusUrl = this.getRegetRootUrl() + "Content/Images/Request/RequestStatus/Approved.png";
                }
                else if (this.request.request_status == RegetApp.RequestStatus.Rejected) {
                    imgStatusUrl = this.getRegetRootUrl() + "Content/Images/Request/RequestStatus/Rejected.png";
                }
                else if (this.request.request_status == RegetApp.RequestStatus.CanceledRequestor
                    || this.request.request_status == RegetApp.RequestStatus.CanceledOrderer
                    || this.request.request_status == RegetApp.RequestStatus.CanceledSystem) {
                    imgStatusUrl = this.getRegetRootUrl() + "Content/Images/Request/RequestStatus/Canceled.png";
                }
                if (!this.isValueNullOrUndefined(imgStatusUrl)) {
                    var imgStatus = document.getElementById("imgHeader");
                    imgStatus.src = imgStatusUrl;
                    imgStatus.style.border = "1px solid #fff";
                    imgStatus.style.borderRadius = "12px";
                    imgStatus.style.height = "24px";
                    imgStatus.style.width = "24px";
                    //let divHeader: HTMLDivElement = <HTMLDivElement>document.getElementById("divHeader");
                    //divHeader.style.backgroundImage = "url('" + imgStatusUrl + "')";
                    //divHeader.style.border = "1px solid #fff";
                }
            };
            RequestController.prototype.updatePrivacy = function () {
                var _this = this;
                this.showLoaderBoxOnly();
                var jsonPrivacy = JSON.stringify({ requestId: this.request.id, privacyId: this.privacyId });
                this.$http.post(this.getRegetRootUrl() + "Request/SetPrivacy?t=" + new Date().getTime(), jsonPrivacy).then(function (response) {
                    try {
                        var tmpData = response.data;
                        var httpResult = tmpData;
                        _this.hideLoaderWrapper();
                        if (!_this.isValueNullOrUndefined(httpResult.error_id) && httpResult.error_id !== 0) {
                            if (httpResult.error_id === 110) {
                                //Not Auhorized
                                _this.displayErrorMsg(_this.locNotAuthorizedPerformActionText);
                            }
                            else {
                                _this.displayErrorMsg();
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
            RequestController.prototype.revertConfirmed = function () {
                var _this = this;
                this.showLoaderBoxOnly();
                var jsonRequestId = JSON.stringify({ requestId: this.request.id });
                this.$http.post(this.getRegetRootUrl() + "Request/Revert?t=" + new Date().getTime(), jsonRequestId).then(function (response) {
                    try {
                        _this.hideLoader();
                        var result = response.data;
                        var strResult = result.string_value;
                        if (_this.isStringValueNullOrEmpty(strResult)) {
                            _this.showToast(_this.locRequestWasRevertedText);
                            _this.loadRequest();
                        }
                        else {
                            var msgError = _this.getErrorMessage(strResult);
                            if (msgError.trim().length == 0) {
                                msgError = null;
                            }
                            _this.displayErrorMsg(msgError);
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
            RequestController.prototype.getPurchaseGroup = function () {
                var _this = this;
                this.showLoaderBoxOnly(this.isError);
                this.$http.get(this.getRegetRootUrl() + "Request/GetPurchaseGroup?pgId=" + this.pgId
                    + "&requestId=" + this.iRequestId
                    + "&t=" + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpRes = response;
                        _this.selectedPg = tmpRes.data;
                    }
                    catch (e) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        _this.hideLoaderWrapper();
                    }
                }, function (response) {
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
            };
            RequestController.prototype.getRequestAccessRequests = function () {
                var _this = this;
                this.showLoaderBoxOnly(this.isError);
                this.$http.get(this.getRegetRootUrl() + "Request/GetAccessRequest?requestId=" + this.iRequestId
                    + "&t=" + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpRes = response;
                        _this.accessRequest = tmpRes.data;
                        if (!_this.isValueNullOrUndefined(_this.accessRequest)) {
                            _this.isAccessRequestSent = true;
                            _this.strAccessRequestDate = _this.convertDateTimeToString(_this.convertJsonDate(_this.accessRequest.request_date).toDate());
                            if (_this.accessRequest.status_id == RegetApp.ApprovalStatus.WaitForApproval) {
                                _this.isAccessRequestSentWaitForApproval = true;
                            }
                            if (_this.accessRequest.status_id == RegetApp.ApprovalStatus.Rejected) {
                                _this.isAccessRequestSentRejected = true;
                            }
                        }
                    }
                    catch (e) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        _this.hideLoaderWrapper();
                    }
                }, function (response) {
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
            };
            RequestController.prototype.loadDiscussion = function () {
                var _this = this;
                this.showLoaderBoxOnly(this.isError);
                this.$http.get(this.getRegetRootUrl() + "Discussion/GetRequestDiscussion?requestId=" + this.iRequestId + "&t=" + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpRes = response;
                        _this.discussion = tmpRes.data;
                        if (!_this.isValueNullOrUndefined(_this.discussion)
                            && !_this.isValueNullOrUndefined(_this.discussion.discussion_items)
                            && _this.discussion.discussion_items.length) {
                            _this.isDiscussionVisible = true;
                        }
                        _this.isDiscussionLoaded = true;
                    }
                    catch (e) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        _this.hideLoaderWrapper();
                    }
                }, function (response) {
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
            };
            RequestController.prototype.addRemark = function () {
                var _this = this;
                if (this.isStringValueNullOrEmpty(this.newRemarkItem)) {
                    return;
                }
                this.showLoaderBoxOnly(this.isError);
                var jsonEntityData = JSON.stringify({ requestId: this.request.id, remark: this.newRemarkItem });
                this.$http.post(this.getRegetRootUrl() + "Discussion/AddRequestRemark?t=" + new Date().getTime(), jsonEntityData).then(function (response) {
                    try {
                        var result = response.data;
                        var strResult = result.string_value;
                        if (_this.isStringValueNullOrEmpty(strResult)) {
                            _this.addDiscussionItem(_this.newRemarkItem, _this.discussion);
                            //let disItem: DiscussionItem = new DiscussionItem();
                            //disItem.disc_text = this.newRemarkItem;
                            //let nameParts: string[] = this.currentUserName.split(" ");
                            //let initials: string = "";
                            //if (nameParts.length > 1) {
                            //    initials += nameParts[1].substring(0, 1).toUpperCase();
                            //}
                            //initials += nameParts[0].substring(0, 1).toUpperCase();
                            //disItem.author_initials = initials;
                            //disItem.author_name = this.currentUserName;
                            //if (!this.isStringValueNullOrEmpty(this.currentUserPhotoUrl)) {
                            //    disItem.author_photo_url = this.currentUserPhotoUrl;
                            //} else {
                            //    disItem.author_photo_url = null;
                            //}
                            //disItem.modif_date_text = moment().format(this.dateTimeMomentFormatText);
                            //disItem.bkg_color = this.discussion.discussion_bkg_color;
                            //disItem.border_color = this.discussion.discussion_border_color;
                            //disItem.user_color = this.discussion.discussion_user_color;
                            //if (this.isValueNullOrUndefined(this.discussion.discussion_items)) {
                            //    this.discussion.discussion_items = [];
                            //}
                            //this.discussion.discussion_items.push(disItem);
                            _this.newRemarkItem = "";
                        }
                        else {
                            var msgError = _this.getErrorMessage(strResult);
                            _this.showAlert(_this.locErrorTitleText, msgError, _this.locCloseText);
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
            RequestController.prototype.cancelRequestConfirmed = function () {
                var _this = this;
                this.showLoaderBoxOnly(this.isError);
                var jsonEntityData = JSON.stringify({ requestId: this.request.id });
                this.$http.post(this.getRegetRootUrl() + "Request/CancelRequest?t=" + new Date().getTime(), jsonEntityData).then(function (response) {
                    try {
                        var result = response.data;
                        var strResult = result.string_value;
                        if (_this.isStringValueNullOrEmpty(strResult)) {
                            //this.isWorkflowDisplayed = false;
                            //this.isRevertVisible = false;
                            //this.isSendForApprovalVisible = false;
                            //this.isDeleteVisible = false;
                            _this.showToast(_this.locCanceledByRequestorText);
                            _this.loadRequest();
                        }
                        else {
                            var msgError = _this.getErrorMessage(strResult);
                            _this.showAlert(_this.locErrorTitleText, msgError, _this.locCloseText);
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
            //******************************************************************************
            //**************************** Data Grid ****************************************
            RequestController.prototype.setGrid = function () {
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
                        name: "name", displayName: this.locItemNameText, field: "name",
                        enableHiding: false,
                        enableFiltering: false,
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextMandatoryTemplate.html",
                        enableCellEdit: false,
                        width: 250,
                        minWidth: 250,
                    },
                    {
                        name: "amount_text", displayName: this.locAmountText, field: "amount_text",
                        enableHiding: false,
                        enableFiltering: false,
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellDecimalNumberMandatoryTemplate.html",
                        enableCellEdit: false,
                        width: 120,
                        minWidth: 120
                    },
                    {
                        name: "unit_code", displayName: this.locUnitText, field: "unit_code",
                        enableHiding: false,
                        enableFiltering: false,
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellDropDownMandatoryTemplate.html",
                        enableCellEdit: false,
                        width: 120,
                        minWidth: 120
                    },
                    {
                        name: "unit_price_text", displayName: this.locUnitPriceText, field: "unit_price_text",
                        enableHiding: false,
                        enableFiltering: false,
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellDecimalNumberMandatoryTemplate.html",
                        enableCellEdit: false,
                        width: 120,
                        minWidth: 120
                    },
                    {
                        name: "currency_code", displayName: this.locCurrencyText, field: "currency_code",
                        enableHiding: false,
                        enableFiltering: false,
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellDropDownMandatoryTemplate.html",
                        enableCellEdit: false,
                        width: 120,
                        minWidth: 120
                    }
                ];
            };
            RequestController.prototype.filterSuppliersById = function (supplierId) {
                var _this = this;
                var deferred = this.$q.defer();
                this.$http.get(this.getRegetRootUrl() + "/Supplier/GetActiveSupplierByIdJs"
                    + "?supplierId=" + supplierId
                    + "&t=" + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        var supplier = tmpData;
                        return deferred.resolve(supplier);
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
                return deferred.promise;
            };
            //*******************************************************************************
            //******************************************************************
            RequestController.prototype.loadInit = function () {
                this.showLoader(this.isError);
                try {
                    this.iRequestId = this.getUrlParamValueInt("id");
                    this.isNewRequest = (this.isValueNullOrUndefined(this.iRequestId) || this.iRequestId < 0);
                    if (this.isNewRequest) {
                        this.isNewRequestOrDraft = true;
                        this.initializeNewRequest();
                        var divRequestPlaceholder = document.getElementById("divRequestPlaceholder");
                        divRequestPlaceholder.style.display = "block";
                        this.requestorName = this.currentUserName;
                        this.isPrivacyReadOnly = false;
                        this.setAttachmetVisibility();
                    }
                    else {
                        this.loadRequest();
                    }
                    //this.setGrid();
                    //this.setGridColumFilters();
                }
                catch (ex) {
                    this.hideLoader();
                    this.displayErrorMsg();
                }
            };
            RequestController.prototype.hideLoaderWrapper = function () {
                //console.log("isRequestorCentreLoaded " + this.isRequestorCentreLoaded);
                //console.log("isPgsLoaded " +this.isPgsLoaded);
                //console.log("isCurrenciesLoaded " +this.isCurrenciesLoaded);
                //console.log("isUnitsLoaded " +this.isUnitsLoaded);
                //console.log("isPrivacyLoaded " +this.isPrivacyLoaded);
                //console.log("isRequestLoaded " +this.isRequestLoaded);
                //console.log("isShipToAddressLoaded " + this.isShipToAddressLoaded);
                //console.log("isExchangeRateLoaded " + this.isExchangeRateLoaded);
                //console.log("isDiscussionLoaded " + this.isDiscussionLoaded);
                if (this.isError || (this.isRequestorCentreLoaded
                    && this.isPgsLoaded
                    && this.isCurrenciesLoaded
                    && this.isUnitsLoaded
                    && this.isPrivacyLoaded
                    && this.isExchangeRateLoaded
                    && this.isRequestLoaded
                    && this.isShipToAddressLoaded
                    && this.isDiscussionLoaded)) {
                    this.hideLoader();
                    this.isError = false;
                }
            };
            RequestController.prototype.populateCentre = function () {
                if (this.isNewRequestOrDraft) {
                    if (!this.isValueNullOrUndefined(this.selectedCentreId) && this.selectedCentreId > -1) {
                        if (!this.isValueNullOrUndefined(this.requestorCentres) && this.requestorCentres.length === 1) {
                            this.isRequestorCentreReadOnly = true;
                        }
                        else {
                            this.isRequestorCentreReadOnly = false;
                        }
                    }
                    else if (!this.isValueNullOrUndefined(this.requestorCentres) && this.requestorCentres.length === 1) {
                        this.isRequestorCentreReadOnly = true;
                        this.selectedCentreId = (this.requestorCentres[0].id);
                        this.centreName = this.requestorCentres[0].name;
                    }
                    else {
                        this.isPgsLoaded = true;
                        this.isRequestorCentreReadOnly = false;
                        this.selectedCentreId = this.requestorCentres[0].id;
                        this.centreName = this.requestorCentres[0].name;
                    }
                    this.loadPgs();
                }
            };
            RequestController.prototype.centreChanged = function () {
                if (this.isValueNullOrUndefined(this.selectedCentreId)) {
                    this.isFreeSupplierAllowed = false;
                    return;
                }
                this.loadPgs();
                this.hideAppMatrix();
            };
            RequestController.prototype.selectPg = function () {
                //this.isAppManOrdererDisplayed = false;
                this.isPgStandard = true;
                this.isPgItemOrder = false;
                if (this.isValueNullOrUndefined(this.pgId)) {
                    return;
                }
                var tmpPg = this.$filter("filter")(this.purchaseGroupsCmb, { id: (this.pgId) }, true);
                if (this.isValueNullOrUndefined(tmpPg[0])) {
                    return;
                }
                this.selectedCmbPg = tmpPg[0];
                if (this.selectedCmbPg.pg_type === RegetApp.PurchaseGroupType.Items) {
                    this.isPgItemOrder = true;
                    this.isPgStandard = false;
                }
                else if (this.selectedCmbPg.pg_type === RegetApp.PurchaseGroupType.Standard) {
                    this.isPgStandard = true;
                    //} else if (this.selectedPg.pg_type === PurchaseGroupType.ApprovalOnly) {
                    //    this.isPgStandard = true;
                }
                //this.loadCustomFields(this.selectedPg.id);
                //this.getPgLimits(this.selectedPg.id);
                var selPgFilter = this.$filter("filter")(this.purchaseGroups, { id: this.selectedCmbPg.id }, true);
                this.selectedPg = selPgFilter[0];
                this.getPgLimits();
                this.getOrderers();
                //this.getCustomFields();
                if (this.isNewRequestOrDraft && !this.isValueNullOrUndefined(this.selectedPg.custom_field) && this.selectedPg.custom_field.length > 0) {
                    this.isCustomFieldVisible = true;
                }
                else {
                    this.isCustomFieldVisible = false;
                }
            };
            RequestController.prototype.selectCurrency = function () {
                this.getPgLimits();
            };
            RequestController.prototype.getPgLimits = function () {
                var selCurrency = this.$filter("filter")(this.currencyList, { id: this.currencyId }, true);
                if (!this.isValueNullOrUndefined(selCurrency) && selCurrency.length > 0) {
                    this.currencyCode = selCurrency[0].name;
                }
                else {
                    this.currencyCode = null;
                }
                var divErrMsg = document.getElementById("divErrAppMen");
                divErrMsg.style.display = "none";
                var selPgFilter = this.$filter("filter")(this.purchaseGroups, { id: this.selectedCmbPg.id }, true);
                var selPg = selPgFilter[0];
                this.requestAppMen = [];
                if (this.isValueNullOrUndefined(selPg.purchase_group_limit)) {
                    return;
                }
                if (this.isValueNullOrUndefined(this.currencyId)) {
                    return;
                }
                if (this.isValueNullOrUndefined(this.request.estimated_price)) {
                    return;
                }
                this.is_approval_needed = selPg.is_approval_needed;
                if (!selPg.is_approval_needed) {
                    this.hideAppMatrix();
                    return;
                }
                this.isAppManOrdererDisplayed = true;
                var estPrice = this.request.estimated_price;
                var priceInCentreCurr = estPrice;
                var exchangeRate = null;
                var exchangeRateMultipl = null;
                if (this.currencyId != this.centre_currency_id) {
                    var exchRates = this.$filter("filter")(this.exchangeRates, { source_currency_id: this.centre_currency_id, destin_currency_id: this.currencyId }, true);
                    if (this.isValueNullOrUndefined(exchRates[0])) {
                        exchRates = this.$filter("filter")(this.exchangeRates, { source_currency_id: this.currencyId, destin_currency_id: this.centre_currency_id }, true);
                        if (this.isValueNullOrUndefined(exchRates[0])) {
                            var filterCurrFrom = this.$filter("filter")(this.currencyList, { id: this.currencyId }, true);
                            var filterCurrToEx1 = this.$filter("filter")(this.currencyList, { id: this.centre_currency_id }, true);
                            var curr1 = filterCurrFrom[0].name;
                            var curr2 = filterCurrToEx1[0].name;
                            var msg = this.locExchangeRateMissingText.replace('{0}', curr1).replace('{1}', curr2);
                            this.displayErrorMsg(msg);
                            return;
                        }
                        exchangeRate = exchRates[0];
                        priceInCentreCurr = estPrice / exchangeRate.exchange_rate1;
                        exchangeRateMultipl = exchangeRate.exchange_rate1;
                    }
                    else {
                        exchangeRate = exchRates[0];
                        priceInCentreCurr = estPrice * exchangeRate.exchange_rate1;
                        exchangeRateMultipl = 1 / exchangeRate.exchange_rate1;
                    }
                }
                if (!this.isValueNullOrUndefined(selPg.purchase_group_limit)) {
                    for (var i = 0; i < selPg.purchase_group_limit.length; i++) {
                        if (selPg.purchase_group_limit[i].is_bottom_unlimited
                            || selPg.purchase_group_limit[i].limit_bottom < priceInCentreCurr) {
                            if (!this.isValueNullOrUndefined(selPg.purchase_group_limit[i].manager_role)) {
                                for (var j = 0; j < selPg.purchase_group_limit[i].manager_role.length; j++) {
                                    var tmpManRole = angular.copy(selPg.purchase_group_limit[i].manager_role[j]);
                                    this.requestAppMen.push(tmpManRole);
                                }
                            }
                        }
                    }
                }
                if (this.isValueNullOrUndefined(this.requestAppMen) || this.requestAppMen.length == 0) {
                    //let divErrMsg: HTMLDivElement = <HTMLDivElement>document.getElementById("divErrAppMen");
                    divErrMsg.style.display = "block";
                }
                if (!this.isValueNullOrUndefined(this.selectedPg)) {
                    for (var i = 0; i < this.selectedPg.purchase_group_limit.length; i++) {
                        if (this.isValueNullOrUndefined(exchangeRateMultipl)) {
                            this.selectedPg.purchase_group_limit[i].limit_bottom_loc_curr_text_ro = this.selectedPg.purchase_group_limit[i].limit_bottom_text_ro;
                            this.selectedPg.purchase_group_limit[i].limit_top_loc_curr_text_ro = this.selectedPg.purchase_group_limit[i].limit_top_text_ro;
                        }
                        else {
                            if (!this.isValueNullOrUndefined(this.selectedPg.purchase_group_limit[i].limit_bottom)) {
                                this.selectedPg.purchase_group_limit[i].limit_bottom_loc_curr_text_ro = this.convertDecimalToText(this.selectedPg.purchase_group_limit[i].limit_bottom * exchangeRateMultipl);
                            }
                            if (!this.isValueNullOrUndefined(this.selectedPg.purchase_group_limit[i].limit_top)) {
                                this.selectedPg.purchase_group_limit[i].limit_top_loc_curr_text_ro = this.convertDecimalToText(this.selectedPg.purchase_group_limit[i].limit_top * exchangeRateMultipl);
                            }
                        }
                    }
                }
            };
            RequestController.prototype.getOrderers = function () {
                var divErrMsg = document.getElementById("divErrOrderer");
                divErrMsg.style.display = "none";
                this.orderers = [];
                var selPgFilter = this.$filter("filter")(this.purchaseGroups, { id: this.selectedCmbPg.id }, true);
                var selPg = selPgFilter[0];
                this.is_order_needed = selPg.is_order_needed;
                this.is_self_ordered = selPg.self_ordered;
                if (this.is_order_needed === false) {
                    return;
                }
                if (selPg.self_ordered == true) {
                    var orderer = new RegetApp.Orderer();
                    orderer.participant_id = this.currentUserId;
                    orderer.surname = this.currentUserName;
                    this.orderers.push(orderer);
                    this.selectedOrdererId = this.orderers[0].participant_id;
                    this.isAppManOrdererDisplayed = true;
                    return;
                }
                this.isAppManOrdererDisplayed = true;
                if (!this.isValueNullOrUndefined(selPg.orderer)) {
                    for (var i = 0; i < selPg.orderer.length; i++) {
                        var orderer = angular.copy(selPg.orderer[i]);
                        this.orderers.push(orderer);
                    }
                }
                if (selPg.is_multi_orderer === true && !this.isValueNullOrUndefined(this.orderers) && this.orderers.length > 0) {
                    this.is_multi_orderer_cmb_dispalyed = true;
                }
                if (selPg.is_order_needed === false
                    && (this.isValueNullOrUndefined(this.orderers) || this.orderers.length === 0)) {
                    //let divErrMsg: HTMLDivElement = <HTMLDivElement>document.getElementById("divErrOrderer");
                    divErrMsg.style.display = "block";
                }
                if (!this.isValueNullOrUndefined(this.orderers) && this.orderers.length == 1) {
                    this.selectedOrdererId = this.orderers[0].participant_id;
                }
            };
            RequestController.prototype.priceWasChanged = function (strPrice, input) {
                if (this.validateDecimalNumber(strPrice, input)) {
                    this.request.estimated_price = this.convertTextToDecimal(strPrice);
                    this.getPgLimits();
                }
                else {
                    this.requestAppMen = [];
                }
            };
            RequestController.prototype.addRequestItem = function () {
                if (this.isNewRequestItemValid() === true) {
                    var requestItem = new RegetApp.RequestItem();
                    requestItem.name = this.requestItemName;
                    requestItem.amount = RegetDataConvert.convertStringToDecimal(this.requestItemAmount);
                    requestItem.amount_text = this.requestItemAmount;
                    requestItem.unit_price = RegetDataConvert.convertStringToDecimal(this.requestItemUnitPrice);
                    requestItem.unit_price_text = this.requestItemUnitPrice;
                    var units = this.$filter("filter")(this.unitList, { id: this.requestItemUnitId }, true);
                    if (!this.isValueNullOrUndefined(units[0])) {
                        this.unitCode = units[0].name;
                    }
                    requestItem.unit_code = this.unitCode;
                    var currencies = this.$filter("filter")(this.currencyList, { id: this.requestItemCurrencyId }, true);
                    if (!this.isValueNullOrUndefined(currencies[0])) {
                        this.currencyCode = currencies[0].name;
                    }
                    requestItem.currency_code = this.currencyCode;
                    requestItem.row_index = this.request.request_items.length + 1;
                    if (this.isValueNullOrUndefined(this.request.estimated_price)) {
                        this.request.estimated_price = 0;
                    }
                    this.request.request_items.push(requestItem);
                    this.setEstPrice();
                    if (this.isRequestItemsDisplayed === false) {
                        //this.setGrid();
                        this.isRequestItemsDisplayed = true;
                        this.isFiltersSet = this.setGridColumFilters();
                    }
                    //this.gridApi.grid.isScrollingVertically = false;
                    //this.gridApi.grid.isScrollingHorizontally = false;
                }
                else {
                    var strMsg = this.locDataCannotBeSavedText;
                    if (!this.isStringValueNullOrEmpty(this.errRequestItem)) {
                        strMsg += "<br>" + this.errRequestItem;
                    }
                    this.displayErrorMsg(strMsg);
                }
            };
            RequestController.prototype.isNewRequestItemValid = function () {
                //ATTENTION - this is for request item not for whole request
                try {
                    this.errRequestItem = null;
                    if (this.isValueNullOrUndefined(this.requestItemName)) {
                        this.errRequestItem = this.locMissingRequestItemNameText;
                        return false;
                    }
                    if (this.isValueNullOrUndefined(this.requestItemAmount)) {
                        this.errRequestItem = this.locMissingRequestAmountText;
                        return false;
                    }
                    if (this.isValueNullOrUndefined(this.requestItemUnitId)) {
                        this.errRequestItem = this.locMissingRequestUnitText;
                        return false;
                    }
                    if (this.isValueNullOrUndefined(this.requestItemUnitPrice)) {
                        this.errRequestItem = this.locMissingRequestPriceText;
                        return false;
                    }
                    if (this.isValueNullOrUndefined(this.requestItemCurrencyId)) {
                        this.errRequestItem = this.locMissingRequestCurrencyText;
                        return false;
                    }
                    return true;
                }
                catch (_a) {
                    this.errRequestItem = this.locErrorMsgText;
                    return false;
                }
            };
            RequestController.prototype.isRequestValid = function () {
                if (this.isKeepErrMsgHidden === true) {
                    return true;
                }
                var isValid = true;
                if (this.isValueNullOrUndefined(this.selectedCentreId)) {
                    if (this.isRequestorCentreReadOnly === false
                        && this.angScopeAny.frmRequest.cmbRequestorCentre.$valid == false) {
                        this.angScopeAny.frmRequest.cmbRequestorCentre.$setTouched();
                    }
                    isValid = false;
                }
                if (this.isFreeSupplier === false) {
                    if (this.isValueNullOrUndefined(this.supplierId)) {
                        //this.angScopeAny.frmRequest.agSupplier.$setTouched();
                        this.supplierOnBlur("agSupplier_errmandatory");
                        //this.requestErrorMsg = this.locEnterMandatoryValuesText;
                        isValid = false;
                    }
                }
                if (this.isValueNullOrUndefined(this.selectedDeliveryDate)) {
                    isValid = false;
                }
                if (this.is_approval_needed && (this.isValueNullOrUndefined(this.requestAppMen) || this.requestAppMen.length == 0)) {
                    var divErrMsgAppMen = document.getElementById("divErrAppMen");
                    divErrMsgAppMen.style.display = "block";
                    isValid = false;
                }
                if (this.is_order_needed && !this.selectedPg.is_multi_orderer && this.isValueNullOrUndefined(this.selectedOrdererId)) {
                    var divErrMsgOrderer = document.getElementById("divErrOrderer");
                    divErrMsgOrderer.style.display = "block";
                    isValid = false;
                }
                if (this.is_order_needed && this.selectedPg.is_multi_orderer && (this.isValueNullOrUndefined(this.orderers) || this.orderers.length == 0)) {
                    var divErrMsgOrderer = document.getElementById("divErrOrderer");
                    divErrMsgOrderer.style.display = "block";
                    isValid = false;
                }
                if (this.isValueNullOrUndefined(this.requestText) || this.requestText.trim().length == 0) {
                    if (this.angScopeAny.frmRequest.txtRequestText.$valid == false) {
                        this.angScopeAny.frmRequest.txtRequestText.$setTouched();
                    }
                    isValid = false;
                }
                if (this.isAddressValid() === false) {
                    isValid = false;
                }
                return isValid;
            };
            RequestController.prototype.isAddressValid = function () {
                var divErrMsgAddress = document.getElementById("divErrMsgAddress");
                if (!this.isValueNullOrUndefined(this.shipToAddress)
                    && this.shipToAddress.length > 1) {
                    if (this.isValueNullOrUndefined(this.selectedShipToAddress)) {
                        divErrMsgAddress.style.display = "block";
                        return false;
                    }
                }
                divErrMsgAddress.style.display = "none";
                return true;
            };
            RequestController.prototype.initiateRequest = function () {
                if (!this.isValueNullOrUndefined(this.request)) {
                    return;
                }
                this.request = new RegetApp.Request();
                this.request.request_items = [];
                this.gridOptions.data = this.request.request_items;
                //this.setGridColumFilters();
            };
            RequestController.prototype.setGridColumFilters = function () {
                var isSet = this.setGridColumFilter("currency_code", this.currencyListGrid);
                if (isSet) {
                    isSet = this.setGridColumFilter("unit_code", this.unitListGrid);
                }
                return isSet;
            };
            RequestController.prototype.openAttachment = function (id) {
                try {
                    var url = this.getRegetRootUrl() + "Attachment/GetAttachment?id=" + id + "&t=" + new Date().getTime();
                    window.open(url);
                }
                catch (e) {
                    this.displayErrorMsg();
                }
                finally {
                    this.hideLoader();
                }
            };
            RequestController.prototype.setEstPrice = function () {
                if (this.isValueNullOrUndefined(this.request.request_items)) {
                    return;
                }
                var isCurrencySet = false;
                this.request.estimated_price = 0;
                for (var i = 0; i < this.request.request_items.length; i++) {
                    this.request.estimated_price += (this.request.request_items[i].amount * this.request.request_items[i].unit_price);
                    if (isCurrencySet === false) {
                        this.request.currency_id = this.request.request_items[i].currency_id;
                    }
                }
                this.request.estimated_price_text = RegetDataConvert.convertDecimalToString(this.request.estimated_price, 2);
            };
            RequestController.prototype.saveDraft = function () {
                this.updateRequest();
                this.saveRequestDraft();
            };
            RequestController.prototype.sendForApproval = function () {
                this.showLoaderBoxOnly();
                try {
                    this.isKeepErrMsgHidden = false;
                    if (this.isRequestValid() === false) {
                        this.displayErrorMsg(this.locEnterMandatoryValuesText);
                        return;
                    }
                    this.updateRequest();
                    this.sendForApprovalPost();
                }
                catch (_a) {
                    this.displayErrorMsg();
                }
                finally {
                    this.hideLoaderWrapper();
                }
            };
            RequestController.prototype.updateRequest = function () {
                this.request.requestor = this.currentUserId;
                this.request.request_centre_id = this.selectedCentreId;
                this.request.purchase_group_id = this.pgId;
                this.request.estimated_price = this.convertTextToDecimal(this.request.estimated_price_text);
                this.request.currency_id = this.currencyId;
                this.request.privacy_id = this.privacyId;
                this.request.lead_time = this.selectedDeliveryDate;
                this.request.request_text = this.requestText;
                this.request.remarks = this.requestRemark;
                this.request.orderer_id = this.selectedOrdererId;
                this.request.supplier_id = this.supplierId;
                this.request.use_supplier_list = !this.isFreeSupplier;
                this.request.supplier_remark = this.supplierText;
                this.updateAppMen();
                this.updateOrderers();
                this.updateShipToAddress();
                this.updateCustomField();
            };
            RequestController.prototype.updateAppMen = function () {
                //let this.request.request_event_approval = []; // DOES NOT WORK !!!
                var reqEvApp = [];
                //Add App Men
                if (!this.isValueNullOrUndefined(this.requestAppMen)) {
                    for (var i = 0; i < this.requestAppMen.length; i++) {
                        var app_1 = new RegetApp.RequestEventApproval();
                        app_1.app_man_id = this.requestAppMen[i].participant_id;
                        app_1.app_level_id = this.requestAppMen[i].approve_level_id;
                        app_1.approve_status = RegetApp.ApprovalStatus.Empty;
                        reqEvApp.push(app_1);
                    }
                }
                this.request.request_event_approval = reqEvApp;
            };
            RequestController.prototype.updateOrderers = function () {
                //let this.request.request_event_approval = []; // DOES NOT WORK !!!
                var orderers = [];
                //Add Orderer
                if (!this.isValueNullOrUndefined(this.orderers)) {
                    for (var i = 0; i < this.orderers.length; i++) {
                        var orderer = new RegetApp.Orderer();
                        orderer.participant_id = this.orderers[i].participant_id;
                        orderers.push(orderer);
                    }
                }
                this.request.orderers = orderers;
            };
            RequestController.prototype.updateShipToAddress = function () {
                this.request.ship_to_address_city = null;
                this.request.ship_to_address_company_name = null;
                this.request.ship_to_address_street = null;
                this.request.ship_to_address_zip = null;
                if (!this.isValueNullOrUndefined(this.selectedShipToAddress)) {
                    this.request.ship_to_address_id = this.selectedShipToAddress.id;
                    this.request.ship_to_address_city = this.selectedShipToAddress.city;
                    this.request.ship_to_address_company_name = this.selectedShipToAddress.company_name;
                    this.request.ship_to_address_street = this.selectedShipToAddress.street;
                    this.request.ship_to_address_zip = this.selectedShipToAddress.zip;
                }
            };
            RequestController.prototype.updateCustomField = function () {
                this.request.custom_fields = [];
                //let selPgFilter: any[] = this.$filter("filter")(this.purchaseGroups, { id: this.selectedCmbPg.id }, true);
                //if (this.isValueNullOrUndefined(selPgFilter) || selPgFilter.length === 0) {
                //    return;
                //}
                //let selPg: PurchaseGroup = selPgFilter[0];
                if (this.isValueNullOrUndefined(this.selectedPg)) {
                    return;
                }
                if (!this.isValueNullOrUndefined(this.selectedPg.custom_field)) {
                    for (var i = 0; i < this.selectedPg.custom_field.length; i++) {
                        var custF = angular.copy(this.selectedPg.custom_field[i]);
                        //if (custF.data_type_id == DataType.String) {
                        custF.string_value = this.selectedPg.custom_field[i].string_value;
                        //}
                        this.request.custom_fields.push(custF);
                    }
                }
                //if (!this.isValueNullOrUndefined(this.request.custom_fields) && this.request.custom_fields.length > 0) {
                //    this.isCustomFieldVisible = true;
                //}
            };
            RequestController.prototype.getRequestLoadedStatus = function (result) {
                var strResult = result.string_value;
                var divRequestPlaceholder = document.getElementById("divRequestPlaceholder");
                if (!this.isStringValueNullOrEmpty(strResult)) {
                    if (strResult === "NotAuthorized") {
                        divRequestPlaceholder.style.display = "none";
                        var divNotAuthorizedPlaceholder = document.getElementById("divNotAuthorizedPlaceholder");
                        divNotAuthorizedPlaceholder.style.display = "block";
                        this.getRequestAccessRequests();
                    }
                    this.requestNr = result.request_nr;
                    var msgError = this.getErrorMessage(strResult);
                    this.showAlert(this.locErrorTitleText, msgError, this.locCloseText);
                    return false;
                }
                divRequestPlaceholder.style.display = "block";
                return true;
            };
            RequestController.prototype.setAttachmetVisibility = function () {
                this.isAttachmentEditable = (this.currentUserId === this.request.requestor) && this.isNewRequestOrDraft;
                this.isAttachmentDisplayed = (this.isNewRequestOrDraft || (!this.isValueNullOrUndefined(this.request.attachments) && this.request.attachments.length > 0));
            };
            //private isRevertBtnVisible(): boolean {
            //    if (this.request.request_status == RequestStatus.WaitForApproval
            //        && this.request.requestor == this.currentUserId) {
            //        if (this.isValueNullOrUndefined(this.request.request_event_approval) || this.request.request_event_approval.length == 0) {
            //            this.isRevertVisible = true;
            //            return true;
            //        } else {
            //            for (let i: number = 0; i < this.request.request_event_approval.length; i++) {
            //            if (this.request.request_event_approval[0].approve_status != ApprovalStatus.WaitForApproval) {
            //                this.isRevertVisible = false;
            //                return false;
            //            }
            //            }
            //            this.isRevertVisible = true;
            //            return true;
            //        }
            //    }
            //    this.isRevertVisible = false;
            //    return false;
            //}
            //private setCustomFields() {
            //    if (this.isValueNullOrUndefined(this.request)) {
            //        this.initializeNewRequest();
            //    }
            //    if (!this.isValueNullOrUndefined(this.customFields)) {
            //        for (let i = 0; i < this.customFields.length; i++) {
            //            if (this.customFields[i].data_type_id === DataType.String) {
            //                this.customFields[i].string_value = this.custFieldValues[i];
            //            }
            //        }
            //    }
            //    this.request.custom_fields = this.customFields;
            //}
            RequestController.prototype.initializeNewRequest = function () {
                if (this.isValueNullOrUndefined(this.request)) {
                    this.request = new RegetApp.Request();
                    this.request.id = -1;
                    this.request.privacy_id = RegetApp.Privacy.Private;
                    this.request.requestor = this.currentUserId;
                    this.privacyId = RegetApp.Privacy.Private;
                    this.isRequestLoaded = true;
                    this.isDiscussionLoaded = true;
                    this.getRequestCentres();
                }
            };
            RequestController.prototype.setPurchaseGroupCmbList = function () {
                this.purchaseGroupsCmb = [];
                if (!this.isValueNullOrUndefined(this.purchaseGroups)) {
                    for (var i = 0; i < this.purchaseGroups.length; i++) {
                        var pgCmb = new RegetApp.PgReqDropDownExtend();
                        pgCmb.id = this.purchaseGroups[i].id;
                        pgCmb.name = this.purchaseGroups[i].group_name;
                        pgCmb.pg_type = this.purchaseGroups[i].purchase_type;
                        //dlPg.name = pgLocName;
                        //dlPg.pg_type = pg.purchase_type;
                        this.purchaseGroupsCmb.push(pgCmb);
                    }
                }
            };
            RequestController.prototype.supplierOnBlur = function (strErrorMsgId) {
                this.checkSupplierMandatory(this.selectedSupplier, strErrorMsgId);
            };
            RequestController.prototype.checkSupplierMandatory = function (selectedSupplier, strErrorMsgId) {
                if (this.isKeepErrMsgHidden === true) {
                    return true;
                }
                if (this.isStringValueNullOrEmpty(strErrorMsgId)) {
                    return true;
                }
                if (this.isValueNullOrUndefined(selectedSupplier)) {
                    $("#" + strErrorMsgId).show("slow");
                    return false;
                }
                else {
                    $("#" + strErrorMsgId).slideUp("slow");
                }
                return true;
            };
            RequestController.prototype.searchSupplier = function (strSupp) {
                return this.filterSuppliers(strSupp, this.selectedCentreId);
            };
            RequestController.prototype.supplierSelectedItemChange = function (item, strErrorMsgId) {
                this.selectedSupplier = item;
                this.checkSupplierMandatory(item, strErrorMsgId);
                if (this.isValueNullOrUndefined(this.selectedSupplier)) {
                    this.supplierId = null;
                }
                if (!(!this.isValueNullOrUndefined(this.selectedSupplier)
                    && (this.isValueNullOrUndefined(this.selectedSupplier.id)))) {
                    //it is workaround - if page is loaded the item is not type of supplier but a string
                    this.supplierId = this.selectedSupplier.id;
                }
            };
            RequestController.prototype.addressCheck = function (address) {
                //let checkedAddress: Address[] = this.$filter("filter")(this.shipToAddress, { id: addressid }, true);
                if (address.is_selected) {
                    for (var i = 0; i < this.shipToAddress.length; i++) {
                        if (this.shipToAddress[i].id != address.id) {
                            this.shipToAddress[i].is_selected = false;
                        }
                    }
                    this.selectedShipToAddress = address;
                }
                else {
                    this.selectedShipToAddress = null;
                }
                this.isAddressValid();
            };
            RequestController.prototype.getCurrAppMan = function () {
                if (this.isValueNullOrUndefined(this.request.request_event_approval)) {
                    return "";
                }
                var appMen = "";
                var iWaitAppLevel = -1;
                for (var i = 0; i < this.request.request_event_approval.length; i++) {
                    if (iWaitAppLevel >= 0 && this.request.request_event_approval[i].app_level_id != iWaitAppLevel) {
                        return appMen;
                    }
                    if (this.request.request_event_approval[i].approve_status == RegetApp.ApprovalStatus.WaitForApproval) {
                        iWaitAppLevel = this.request.request_event_approval[i].app_level_id;
                        if (appMen.length > 0) {
                            appMen += ", ";
                        }
                        appMen += this.request.request_event_approval[i].app_man_surname + " " + this.request.request_event_approval[i].app_man_first_name;
                    }
                }
                return "";
            };
            RequestController.prototype.getCurrOrderer = function () {
                if (this.isValueNullOrUndefined(this.request.orderers)) {
                    return "";
                }
                var orderers = "";
                for (var i = 0; i < this.request.orderers.length; i++) {
                    if (orderers.length > 0) {
                        orderers += ", ";
                    }
                    orderers += this.request.orderers[i].surname + " " + this.request.orderers[i].first_name;
                }
            };
            RequestController.prototype.selectPrivacy = function () {
                if (!this.isNewRequest) {
                    this.updatePrivacy();
                }
                //alert(this.privacyId);
            };
            RequestController.prototype.sendReminder = function () {
                this.showToast(this.locSentText);
            };
            RequestController.prototype.isCanceled = function () {
                if (this.isValueNullOrUndefined(this.request)) {
                    return false;
                }
                var isCanceled = (this.request.request_status == RegetApp.RequestStatus.CanceledOrderer
                    || this.request.request_status == RegetApp.RequestStatus.CanceledRequestor
                    || this.request.request_status == RegetApp.RequestStatus.CanceledSystem);
                return isCanceled;
            };
            RequestController.prototype.revert = function () {
                this.$mdDialog.show({
                    template: this.getConfirmDialogTemplate(this.locRevertRequestConfirmText, this.locConfirmationText, this.locRevertText, this.locCloseText, "confirmDialog()", "closeDialog()"),
                    locals: {
                        requestControl: this
                    },
                    controller: this.dialogConfirmRevertController
                });
            };
            RequestController.prototype.activate = function () {
                this.$mdDialog.show({
                    template: this.getConfirmDialogTemplate(this.locActivateConfirmText, this.locConfirmationText, this.locYesText, this.locNoText, "confirmDialog()", "closeDialog()"),
                    locals: {
                        requestControl: this
                    },
                    controller: this.dialogConfirmRevertController
                });
            };
            RequestController.prototype.dialogConfirmRevertController = function ($scope, $mdDialog, requestControl) {
                $scope.closeDialog = function () {
                    $mdDialog.hide();
                };
                $scope.confirmDialog = function () {
                    $mdDialog.hide();
                    requestControl.revertConfirmed();
                };
            };
            RequestController.prototype.remindMan = function () {
                this.$mdDialog.show({
                    template: this.getConfirmDialogTemplate(this.remindConfirmText, this.locConfirmationText, this.locSendText, this.locCloseText, "confirmDialog()", "closeDialog()"),
                    locals: {
                        requestControl: this
                    },
                    controller: this.dialogConfirmRemindController
                });
            };
            RequestController.prototype.dialogConfirmRemindController = function ($scope, $mdDialog, requestControl) {
                $scope.closeDialog = function () {
                    $mdDialog.hide();
                };
                $scope.confirmDialog = function () {
                    $mdDialog.hide();
                    requestControl.sendReminder();
                };
            };
            RequestController.prototype.cancelRequest = function () {
                this.$mdDialog.show({
                    template: this.getConfirmDialogTemplate(this.locCanceledConfirmText, this.locConfirmationText, this.locYesText, this.locNoText, "confirmDialog()", "closeDialog()"),
                    locals: {
                        requestControl: this
                    },
                    controller: this.dialogCancelRequestController
                });
            };
            RequestController.prototype.dialogCancelRequestController = function ($scope, $mdDialog, requestControl) {
                $scope.closeDialog = function () {
                    $mdDialog.hide();
                };
                $scope.confirmDialog = function () {
                    $mdDialog.hide();
                    requestControl.cancelRequestConfirmed();
                };
            };
            RequestController.prototype.toggleFreeSupplier = function () {
                this.isFreeSupplier = !this.isFreeSupplier;
            };
            RequestController.prototype.displayAppMatrix = function () {
                var iScrollX = $(".reget-body").scrollLeft();
                var iScrollY = $(".reget-body").scrollTop() + window.pageYOffset;
                var imgAppMatrix = document.getElementById("imgAppMatrix");
                var iLeft = iScrollX + Math.floor(imgAppMatrix.getBoundingClientRect().left);
                var iTop = iScrollY + Math.floor(imgAppMatrix.getBoundingClientRect().top);
                iLeft += imgAppMatrix.width + 10;
                var divAppMatrix = document.getElementById("divAppMatrix");
                divAppMatrix.style.top = iTop + "px";
                divAppMatrix.style.left = iLeft + "px";
                $("#divAppMatrix").show();
            };
            RequestController.prototype.hideAppMatrix = function () {
                $("#divAppMatrix").fadeOut();
            };
            RequestController.prototype.openOrder = function () {
                window.location.href = this.getRegetRootUrl() + "Request/Order?id=" + this.request.id;
            };
            RequestController.prototype.uploadFile = function (attUpload) {
                attUpload.is_can_be_deleted = true;
                if (this.isValueNullOrUndefined(this.request.attachments)) {
                    this.request.attachments = [];
                }
                this.request.attachments.push(attUpload);
            };
            RequestController.prototype.sendRequestForAccess = function () {
                this.$mdDialog.show({
                    template: this.getConfirmDialogTemplate(this.locAskAcessRequestConfirmText, this.locConfirmationText, this.locYesText, this.locNoText, "confirmDialog()", "closeDialog()"),
                    locals: {
                        requestControl: this
                    },
                    controller: this.dialogConfirmSendAccessRequest
                });
            };
            RequestController.prototype.dialogConfirmSendAccessRequest = function ($scope, $mdDialog, requestControl) {
                $scope.closeDialog = function () {
                    $mdDialog.hide();
                };
                $scope.confirmDialog = function () {
                    $mdDialog.hide();
                    requestControl.sendAccessRequest();
                };
            };
            RequestController.prototype.approveRequest = function () {
                this.$mdDialog.show({
                    template: this.getConfirmDialogTemplate(this.locApproveConfirmText, this.locConfirmationText, this.locYesText, this.locNoText, "confirmDialog()", "closeDialog()"),
                    locals: {
                        requestControl: this
                    },
                    controller: this.dialogConfirmRequestApproval
                });
            };
            RequestController.prototype.dialogConfirmRequestApproval = function ($scope, $mdDialog, requestControl) {
                $scope.closeDialog = function () {
                    $mdDialog.hide();
                };
                $scope.confirmDialog = function () {
                    $mdDialog.hide();
                    requestControl.approveRequestConfirmed();
                };
            };
            RequestController.prototype.rejectRequest = function () {
                this.$mdDialog.show({
                    template: this.getConfirmDialogTemplate(this.locRejectConfirmText, this.locConfirmationText, this.locYesText, this.locNoText, "confirmDialog()", "closeDialog()"),
                    locals: {
                        requestControl: this
                    },
                    controller: this.dialogConfirmRequestReject
                });
            };
            RequestController.prototype.dialogConfirmRequestReject = function ($scope, $mdDialog, requestControl) {
                $scope.closeDialog = function () {
                    $mdDialog.hide();
                };
                $scope.confirmDialog = function () {
                    $mdDialog.hide();
                    requestControl.rejectRequestConfirmed();
                };
            };
            return RequestController;
        }(RegetApp.BaseRegetGridTs));
        RegetApp.RequestController = RequestController;
        angular.
            module('RegetApp').
            controller('RequestController', Kamsyk.RegetApp.RequestController).
            config(function ($mdDateLocaleProvider) {
            this.SetDatePicker($mdDateLocaleProvider);
            //it is neccessary to set IsGenerateDatePickerLocalization = true
        });
    })(RegetApp = Kamsyk.RegetApp || (Kamsyk.RegetApp = {}));
})(Kamsyk || (Kamsyk = {}));
//# sourceMappingURL=request.js.map