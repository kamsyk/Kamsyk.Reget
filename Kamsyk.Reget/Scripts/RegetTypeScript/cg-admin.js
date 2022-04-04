/// <reference path="../RegetTypeScript/Base/reget-base.ts" />
/// <reference path="../RegetTypeScript/Base/reget-common.ts" />
/// <reference path="../RegetTypeScript/Base/reget-entity.ts" />
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
        angular.module('RegetApp').directive("appMatrix", function ($timeout) {
            return {
                templateUrl: RegetCommonTs.getRegetRootUrl() + 'Content/Html/AngAppMatrix.html',
                link: function postLink(elem, attrs, transclude) {
                    $timeout(function () {
                        // This code will run after
                        // templateUrl has been loaded, cloned
                        // and transformed by directives.
                        //$scope.HideAddLimitButton();
                        if ($("#trLimit_5").is(':visible')) {
                            $("#btnAddLimit").hide();
                        }
                        else {
                            $("#btnAddLimit").show();
                        }
                    }, 0);
                }
            };
        });
        angular.module('RegetApp').directive("appManager", function () {
            return {
                scope: {
                    limitid: '@',
                    userid: '@',
                    surname: '@',
                    firstname: '@',
                    removetext: '@',
                    rooturl: '@',
                    deleteappman: '&'
                },
                templateUrl: RegetCommonTs.getRegetRootUrl() + 'Content/Html/AngAppManager.html'
            };
        });
        angular.module('RegetApp').directive("orderersupplier", function () {
            return {
                scope: {
                    supplierid: '@',
                    suppliername: '@',
                    userid: '@',
                    surname: '@',
                    firstname: '@',
                    removetext: '@',
                    rooturl: '@',
                    deleteorderersupplier: '&'
                },
                //scope: { tmpappmanuser: '=appmanuser' },
                templateUrl: RegetCommonTs.getRegetRootUrl() + 'Content/Html/AngOrdererSupplier.html'
            };
        });
        angular.module('RegetApp').directive("requestoredit", function () {
            return {
                scope: {
                    userid: '@',
                    centreid: '@',
                    surname: '@',
                    firstname: '@',
                    isdefault: '@',
                    removetext: '@',
                    displaydetailtext: '@',
                    hidedetailtext: '@',
                    defaultrequestortext: '@',
                    allrequestortext: '@',
                    rooturl: '@',
                    expandrequestor: '&',
                    collapserequestor: '&',
                    deleterequestor: '&',
                    checkallrequestor: '&'
                },
                templateUrl: RegetCommonTs.getRegetRootUrl() + 'Content/Html/AngRequestor.html'
            };
        });
        angular.module('RegetApp').directive("ordereredit", function () {
            return {
                scope: {
                    userid: '@',
                    surname: '@',
                    firstname: '@',
                    isdefault: '@',
                    removetext: '@',
                    displaydetailtext: '@',
                    hidedetailtext: '@',
                    allorderertext: '@',
                    rooturl: '@',
                    expandorderer: '&',
                    collapseorderer: '&',
                    deleteorderer: '&',
                    checkallorderer: '&'
                },
                templateUrl: RegetCommonTs.getRegetRootUrl() + 'Content/Html/AngOrderer.html'
            };
        });
        angular.module('RegetApp').directive("cgadmin", function ($compile) {
            return {
                scope: {
                    userid: '@',
                    surname: '@',
                    firstname: '@',
                    iscompanyadmin: '@',
                    iscgpropadmin: '@',
                    isrequestoradmin: '@',
                    isordereradmin: '@',
                    isappmatrixadmin: '@',
                    removetext: '@',
                    cgadmintext: '@',
                    appmatrixadmintext: '@',
                    requestoradmintext: '@',
                    ordereradmintext: '@',
                    displaydetailtext: '@',
                    hidedetailtext: '@',
                    expandcgadmin: '&',
                    collapsecgadmin: '&',
                    rooturl: '@',
                    deletecgadmin: '&',
                    cgpropadmintoggle: '&',
                    appmatrixadmintoggle: '&',
                    requestoradmintoggle: '&',
                    ordereradmintoggle: '&'
                },
                templateUrl: RegetCommonTs.getRegetRootUrl() + 'Content/Html/AngCgAdmin.html'
            };
        });
        angular.module('RegetApp').directive("regettranslate", function () {
            return {
                restrict: "E",
                transclude: true,
                scope: {
                    localtexts: "=",
                    text1: "=",
                    text2: "=",
                    text3: "=",
                    label1: "@",
                    label2: "@",
                    label3: "@",
                    flag1: "@",
                    flag2: "@",
                    flag3: "@",
                    text1changed: "&",
                    text2changed: "&",
                    text3changed: "&"
                },
                templateUrl: RegetCommonTs.getRegetRootUrl() + 'Content/Html/AngTranslate.html'
            };
        });
        angular.module('RegetApp').directive("centre", function () {
            return {
                scope: {
                    centreid: "@",
                    name: "@",
                    removetext: "@",
                    rooturl: "@",
                    deletecentre: "&"
                },
                templateUrl: RegetCommonTs.getRegetRootUrl() + 'Content/Html/AngCentre.html'
            };
        });
        angular.module('RegetApp').directive("checkboxreadonly", function () {
            return {
                scope: {
                    ischecked: "@",
                    labextext: "@",
                    yestext: "@",
                    notext: "@",
                    rooturl: "@"
                },
                templateUrl: RegetCommonTs.getRegetRootUrl() + 'Content/Html/AngCheckBoxReadOnly.html'
            };
        });
        var CgAdminController = /** @class */ (function (_super) {
            __extends(CgAdminController, _super);
            //**************************************************************************
            //**********************************************************
            //Constructor
            function CgAdminController($scope, $http, $filter, $mdDialog, $mdToast, $q, $timeout, $compile) {
                var _this = _super.call(this, $scope, $http, $filter, $mdDialog, $mdToast, $q, $timeout) || this;
                _this.$scope = $scope;
                _this.$http = $http;
                _this.$filter = $filter;
                _this.$mdDialog = $mdDialog;
                _this.$mdToast = $mdToast;
                _this.$q = $q;
                _this.$timeout = $timeout;
                _this.$compile = $compile;
                //**************************************************************************
                //Constants
                //**************************************************************************
                //**************************************************************************
                //Properties
                _this.isNewCg = false;
                _this.isCentresLoaded = false;
                _this.isCgPurchaseGroupsLoaded = false;
                _this.isCurrenciesLoaded = false;
                //private isParticipantLoaded: boolean = false;
                _this.isOfficeLoaded = false;
                _this.selectedCentreGroupId = null;
                //private participantId: number = null;
                _this.participantId = $("#ParticipantId").val();
                //private participants: Participant[] = null;
                _this.offices = null;
                _this.loadTextBkp = null;
                _this.editPurchaseGroup = null;
                _this.purchaseGroupOrig = null;
                _this.isDeactivatedPgLoaded = false;
                _this.isDeactivatedPgDisplayed = false;
                _this.centreGroup = null;
                _this.selectedOfficeId = null;
                _this.searchstringdefaultsupplier = null;
                _this.searchstringdeputyorderer = null;
                _this.searchstringcentre = null;
                _this.selectedDefaultSupplier = null;
                _this.selectedOfficeName = null;
                _this.selectedSearchCentre = null;
                _this.selectedOrdererSupplierOrderer = null;
                _this.selectedOrdererSupplierSupplier = null;
                _this.searchstringorderersupplierorderer = null;
                _this.searchstringorderersuppliersupplier = null;
                _this.searchstringcgadmin = null;
                _this.searchstringrequestor = null;
                _this.selectedRequestor = null;
                _this.searchstringappman = null;
                _this.selectedAppMan = null;
                _this.searchstringorderer = null;
                _this.selectedOrderer = null;
                _this.searchstringnotassignedcentre = null;
                _this.selectedNotAssignedCentre = null;
                _this.selectedDeputyOrderer = null;
                _this.selectedCgAdmin = null;
                _this.isDeactivatedCgDisplayed = false;
                _this.centreGroupList = null;
                _this.strInitCgId = null;
                _this.purchaseGroupList = null;
                _this.pgItemCount = null;
                _this.isDeactivatedPgLoading = false;
                _this.currenciesList = null;
                _this.selectedCurrencyId = null;
                _this.isCgPropertyAdmin = false;
                _this.isCgAppMatrixAdmin = false;
                _this.isCgOrdererAdmin = false;
                _this.isCgRequestorAdmin = false;
                _this.isReadOnly = false;
                _this.adminRoles = null;
                _this.purchaseGroupCount = null;
                _this.selectedCurrencyCode = null;
                _this.selectedCurrencyCodeName = null;
                _this.isAllCgReadOnly = false;
                _this.notAssignedCentresList = null;
                _this.centres = null;
                _this.deleteAllRequestorList = null;
                _this.deleteAllOrdererList = null;
                _this.suppliers = null;
                _this.cgPropErrMsg = null;
                _this.isCancelEvent = false;
                _this.pgExpandImgs = null;
                _this.saveErrMsg = null;
                _this.isErrorAppMatrix = false;
                _this.parentPurchaseGroups = null;
                //private frmCgAdmin: any = null;
                _this.limitBottom = null;
                _this.limitTop = null;
                _this.isCgReadOnly = false;
                //**************************************************************************
                //***************************************************************************
                //Localization
                _this.loadingPurchaseGroupText = $("#LoadingPurchaseGroupText").val();
                _this.cancelSelectText = $("#CancelSelectText").val();
                _this.loadDataErrorText = $("#LoadDataErrorText").val();
                _this.loadingDataText = $("#LoadingDataText").val();
                _this.warningText = $("#WarningText").val();
                _this.closeText = $("#CloseText").val();
                _this.cannotDeleteCompanyAdminText = $("#CannotDeleteCompanyAdminText").val();
                _this.moveCentreMsgText = $("#MoveCentreMsgText").val();
                _this.moveCentreHeaderText = $("#MoveCentreHeaderText").val();
                _this.cancelText = $("#BackText").val();
                _this.moveText = $("#MoveText").val();
                _this.lowerLimitText = $("#LowerLimitText").val();
                _this.upperLimitText = $("#UpperLimitText").val();
                _this.approveManText = $("#ApproveManText").val();
                _this.mandatoryText = $("#MandatoryTextFieldText").val();
                _this.mandatoryDecimal = $("#EnterDecimalNumberText").val();
                _this.noLimitText = $("#NoLimitText").val();
                _this.multiplText = $("#MultiplText").val();
                _this.addText = $("#AddText").val();
                _this.removeText = $("#DeleteText").val();
                _this.upText = $("#UpText").val();
                _this.downText = $("#DownText").val();
                _this.selectAppManUserText = $("#WhoIsAppManText").val();
                _this.notFoundText = $("#NotFoundText").val();
                _this.saveText = $("#SaveText").val();
                _this.dataWasSavedText = $("#DataWasSavedText").val();
                _this.displayDetailText = $("#DisplayDetailsText").val();
                _this.hideDetailText = $("#HideDetailsText").val();
                _this.purchaseGroupNameText = $("#NameText").val();
                _this.parentPgNameText = $("#ParentPgNameText").val();
                _this.defaultRequestorText = $("#DefaultRequestorText").val();
                _this.allRequestorText = $("#AllRequestorText").val();
                _this.allOrdererText = $("#AllOrdererText").val();
                _this.defaultOrdererText = $("#DefaultOrdererText").val();
                _this.deleteAllRequestorTitleText = $("#DeleteAllRequestorTitleText").val();
                _this.deleteAllRequestorMsgText = $("#DeleteAllRequestorMsgText").val();
                _this.selectRequestorText = $("#SelectRequestoText").val();
                _this.deleteAllOrdererTitleText = $("#DeleteAllOrdererTitleText").val();
                _this.deleteAllOrdererMsgText = $("#DeleteAllOrdererMsgText").val();
                _this.selectOrdererText = $("#SelectOrdererPlaceholderText").val();
                _this.saveErrorText = $("#SaveErrorText").val();
                _this.requestorsText = $("#RequestorsText").val();
                _this.orderersText = $("#OrderersText").val();
                _this.defaultSupplierText = $("#DefaultSupplierText").val();
                _this.whoWillBeDefaultSupplierText = $("#WhoIsDefaultSupplierText").val();
                _this.translateText = $("#TranslationText").val();
                _this.cgDeleteConfirmationText = $("#CgDeleteConfirmationText").val();
                _this.cgDeleteConfirmationHeaderText = $("#CgDeleteConfirmationHeaderText").val();
                _this.appMatrixAdminText = $("#AppMatrixAdminText").val();
                _this.requestorAdminText = $("#RequestorAdminText").val();
                _this.ordererAdminText = $("#OrdererAdminText").val();
                _this.centreGroupPropertyAdminText = $("#CentreGroupPropertyAdminText").val();
                _this.managerSubstitutionText = $("#ManagerSubstitutionText").val();
                _this.displayText = $("#DisplayText").val();
                _this.hideText = $("#HideText").val();
                _this.collapseAllText = $("#CollapseAllText").val();
                _this.expandAllText = $("#ExpandAllText").val();
                //private approvalOnlyText: string =$("#ApprovalOnlyText").val();
                _this.orderAllowedText = $("#OrderAllowedText").val();
                _this.approvalNeededText = $("#ApprovalNeededText").val();
                _this.selfOrderedText = $("#SelfOrderedText").val();
                _this.deleteText = $("#DeleteText").val();
                _this.backText = $("#BackText").val();
                _this.purchaseGroupDisabledText = $("#PurchaseGroupDisabledText").val();
                _this.centreGroupDisabledText = $("#CentreGroupDisabledText").val();
                _this.purchaseGroupDeletedText = $("#PurchaseGroupDeletedText").val();
                _this.centreGroupDeletedText = $("#CentreGroupDeletedText").val();
                _this.notificationText = $("#NotificationText").val();
                _this.activeText = $("#ActiveText").val();
                _this.aprovalLevelText = $("#AprovalLevelText").val();
                _this.purchaseGroupEditingText = $("#PurchaseGroupEditingText").val();
                _this.fromText = $("#FromText").val();
                _this.toText = $("#ToText").val();
                _this.enterText = $("#EnterText").val();
                _this.nameText = $("#NameText").val();
                _this.companyText = $("#CompanyText").val();
                _this.currencyText = $("#CurrencyText").val();
                _this.loadingPurchaseGroupsText = $("#LoadingPurchaseGroupText").val();
                _this.decimalSeparator = $("#DecimalSeparator").val();
                //***************************************************************
                _this.$onInit = function () { };
                _this.loadInit();
                return _this;
            }
            //***************************************************************
            //Methods
            CgAdminController.prototype.loadInit = function () {
                //if (this.isValueNullOrUndefined(this.$http)) {
                //    //because of jasmine test
                //    return;
                //}
                try {
                    this.isNewCg = ($("#IsNewCg").val() === "1");
                    if (this.isNewCg === true) {
                        this.isCentresLoaded = true;
                        this.isCgPurchaseGroupsLoaded = true;
                        this.selectedCentreGroupId = -1;
                        this.getCentreGroupData();
                        this.getOfficeData();
                        //this.frmCgAdmin = this.angScopeAny.frmCgAdmin;
                        $("#btnSaveCgDetails").show();
                    }
                    else {
                        this.populateCentreGroups();
                        this.loadTextBkp = $("#spanLoading").html();
                        this.rootUrl = this.getRegetRootUrl();
                    }
                    if (this.isSmartPhone() === true) {
                        $("#divCgParam").slideUp();
                        $("#imgCgExpand").attr("src", this.getRegetRootUrl() + "Content/Images/Panel/Expand.png");
                        $("#imgCgExpand").attr("title", $("#DisplayText").val());
                        $("#divCgAppMatrix").slideUp();
                        $("#imgCgMatrixExpand").attr("src", this.getRegetRootUrl() + "Content/Images/Panel/Expand.png");
                        $("#imgCgMatrixExpand").attr("title", $("#DisplayText").val());
                    }
                    else {
                        $("#imgCgExpand").attr("src", this.getRegetRootUrl() + "Content/Images/Panel/Collapse.png");
                        $("#imgCgExpand").attr("title", $("#HideText").val());
                        $("#imgCgMatrixExpand").attr("src", this.getRegetRootUrl() + "Content/Images/Panel/Collapse.png");
                        $("#imgCgMatrixExpand").attr("title", $("#HideText").val());
                    }
                    //$(".reget-appmatrix-expcolimg").attr("src", this.getRegetRootUrl() + "Content/Images/Panel/Expand.png");
                    if (this.isNewCg !== true) {
                        var urlParams = this.getAllUrlParams();
                        if (!this.isValueNullOrUndefined(urlParams)) {
                            for (var i = 0; i < urlParams.length; i++) {
                                if (urlParams[i].param_name.toLowerCase() === "cgid") {
                                    this.selectedCentreGroupId = parseInt(urlParams[i].param_value);
                                    this.getCentreGroupData();
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (ex) {
                    this.displayErrorMsg();
                }
            };
            CgAdminController.prototype.getCentreGroup = function (jsonData) {
                var centreGroup = jsonData;
                return centreGroup;
            };
            CgAdminController.prototype.getCentreGroupList = function (jsonData) {
                var tmpCgList = jsonData;
                //for (var i = 0; i < jsonData.length; i++) {
                //    var cg: CentreGroup = jsonData[i];
                //    tmpCgList.push(cg);
                //}
                return tmpCgList;
            };
            CgAdminController.prototype.getParticipants = function (jsonData) {
                var tmpParticipants = jsonData;
                return tmpParticipants;
            };
            CgAdminController.prototype.getCompanies = function (jsonData) {
                var tmpCompanies = jsonData;
                return tmpCompanies;
            };
            CgAdminController.prototype.getPurchaseGroupList = function (jsonData) {
                var tmpPgList = jsonData;
                return tmpPgList;
            };
            CgAdminController.prototype.getCentresList = function (jsonData) {
                var tmpCentreList = jsonData;
                return tmpCentreList;
            };
            CgAdminController.prototype.getCurrenciesList = function (jsonData) {
                var tmpCurrList = jsonData;
                return tmpCurrList;
            };
            CgAdminController.prototype.getCentreList = function (jsonData) {
                var tmpCentreList = jsonData;
                return tmpCentreList;
            };
            CgAdminController.prototype.getUserRoles = function (jsonData) {
                var tmpRoles = jsonData;
                return tmpRoles;
            };
            CgAdminController.prototype.getCentreGroupData = function () {
                this.isError = false;
                this.errMsg = this.loadDataErrorText;
                //if (this.selectedCentreGroupId === this.cancelSelectText) {
                //    this.selectedCentreGroupId = null;
                //}
                if (this.isValueNullOrUndefined(this.selectedCentreGroupId)) {
                    this.hideLoader();
                    this.hideCentreGroupDetails();
                    $("#btnRefresh").hide();
                    return;
                }
                this.populateCentreGroupData();
                if (!this.isNewCg) {
                    this.populatePurchaseGroups(false);
                }
                this.hideLoaderAfterAllIsLoaded();
            };
            //********************************************************************
            //Http Get, Post
            CgAdminController.prototype.populateCentreGroupData = function () {
                var _this = this;
                this.isError = false;
                $("#btnSaveCgDetails").hide();
                this.showLoader(this.isError);
                if (!this.isNewCg) {
                    this.isCurrenciesLoaded = false;
                    this.isCentresLoaded = false;
                    //this.isParticipantLoaded = false;
                    this.isOfficeLoaded = false;
                    this.isCgPurchaseGroupsLoaded = false;
                }
                this.editPurchaseGroup = null;
                this.clearDefaultSupp();
                this.hideCentreAutoCompl();
                this.hideOrdererSupplierAutoCompl();
                this.hideDeputyOrdererAutoCompl();
                this.isDeactivatedPgLoaded = false;
                this.isDeactivatedPgDisplayed = false;
                this.$http.get(this.getRegetRootUrl() + '/RegetAdmin/GetCentreGroupDataById?cgId=' + this.selectedCentreGroupId + '&t=' + new Date().getTime(), {}).then(function (response) {
                    try {
                        _this.centreGroup = _this.getCentreGroup(response.data);
                        _this.selectedOfficeId = _this.centreGroup.company_id;
                        _this.populateCurrencies(_this.selectedCentreGroupId, _this.centreGroup.currency_id, _this.selectedOfficeId);
                        _this.selectedOfficeName = _this.centreGroup.company_name;
                        _this.populateCgCentres();
                        $("#divCgDetailPanel").show();
                        if (!_this.isNewCg) {
                            $("#divCgAppMatrixPanel").show();
                        }
                        $("#btnRefresh").show();
                    }
                    catch (e) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        _this.hideLoaderAfterAllIsLoaded();
                    }
                }, function (response) {
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
            };
            CgAdminController.prototype.getOfficeData = function () {
                var _this = this;
                if (this.isValueNullOrUndefined(this.offices)) {
                    this.$http.get(this.getRegetRootUrl() + '/RegetAdmin/GetActiveOfficeData?t=' + new Date().getTime(), {}).then(function (response) {
                        try {
                            _this.offices = _this.getCompanies(response.data);
                            if (!_this.isValueNullOrUndefined(_this.centreGroup)) {
                                _this.selectedOfficeId = _this.centreGroup.company_id;
                            }
                            _this.isOfficeLoaded = true;
                            _this.getIsCentreGroupReadOnly();
                        }
                        catch (e) {
                            _this.hideLoader();
                            _this.displayErrorMsg();
                        }
                        finally {
                            _this.hideLoaderAfterAllIsLoaded();
                        }
                    }, function (response) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    });
                }
                else {
                    this.isOfficeLoaded = true;
                }
            };
            CgAdminController.prototype.populateCentreGroups = function (isStandAlone) {
                var _this = this;
                if (isStandAlone === true) {
                    this.showLoaderBoxOnly();
                }
                else {
                    this.showLoader(this.isError);
                }
                this.$http.get(this.getRegetRootUrl() + 'RegetAdmin/GetCentreGroups?isDeactivatedLoaded='
                    + this.isDeactivatedCgDisplayed + '&t=' + new Date().getTime(), {}).then(function (response) {
                    try {
                        _this.centreGroupList = _this.getCentreGroupList(response.data);
                        if (!_this.isStringValueNullOrEmpty(_this.strInitCgId)) {
                            var iInitCgId = parseInt(_this.strInitCgId);
                            _this.selectedCentreGroupId = iInitCgId;
                            _this.getCentreGroupData();
                        }
                    }
                    catch (e) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        if (isStandAlone === true) {
                            _this.hideLoader();
                        }
                        else {
                            _this.hideLoaderAfterAllIsLoaded();
                        }
                    }
                }, function (response) {
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
            };
            CgAdminController.prototype.populatePurchaseGroups = function (isDisabledLoaded) {
                var _this = this;
                this.showLoader(this.isError);
                this.$http.get(this.getRegetRootUrl() + '/RegetAdmin/GetPurchaseGroupsByCgId?cgId=' + this.selectedCentreGroupId
                    + '&indexFrom=-1&isDeativatedLoaded=' + isDisabledLoaded + '&t=' + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        _this.purchaseGroupList = tmpData; //this.getPurchaseGroupList(response.data);
                        _this.isCgPurchaseGroupsLoaded = true;
                        if (!_this.isValueNullOrUndefined(_this.purchaseGroupList) && _this.purchaseGroupList.length > 25) {
                            //*** do not delete ****************
                            _this.pgItemCount = 25;
                            _this.showLoader(_this.isError);
                            _this.getPurchaseGroupsCount();
                            //************************************
                        }
                        else {
                            if (_this.isDeactivatedPgLoading) {
                                _this.isDeactivatedPgLoading = false;
                                _this.isDeactivatedPgLoaded = true;
                                _this.displayHideDisablePgs();
                            }
                        }
                    }
                    catch (e) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        _this.hideLoaderAfterAllIsLoaded();
                    }
                }, function (response) {
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
            };
            CgAdminController.prototype.populateCurrencies = function (cgId, currencyId, officeId) {
                var _this = this;
                this.showLoader(this.isError);
                this.$http.get(this.getRegetRootUrl() + '/RegetAdmin/GetCurrencies?cgId=' + cgId +
                    '&currentCurrencyId=' + currencyId +
                    '&companyId=' + officeId +
                    '&t=' + new Date().getTime(), {}).then(function (response) {
                    try {
                        _this.currenciesList = _this.getCurrenciesList(response.data);
                        _this.isCurrenciesLoaded = true;
                        if (!_this.isValueNullOrUndefined(_this.centreGroup)) {
                            var cgCurr = _this.$filter("filter")(_this.currenciesList, { id: _this.centreGroup.currency_id }, true);
                            if (_this.isValueNullOrUndefined(cgCurr) || cgCurr.length === 0) {
                                _this.centreGroup.currency_id = null;
                            }
                            if (_this.isValueNullOrUndefined(_this.centreGroup.currency_id)) {
                                var cgOffice = _this.$filter("filter")(_this.offices, { company_id: _this.centreGroup.company_id }, true);
                                if (!_this.isValueNullOrUndefined(cgOffice) && cgOffice.length > 0) {
                                    _this.centreGroup.currency_id = cgOffice[0].default_currency_id;
                                }
                            }
                            _this.selectedCurrencyId = _this.centreGroup.currency_id;
                            _this.setSelectedCurrencyCode();
                            _this.getIsCentreGroupReadOnly();
                        }
                        //foreign currencies
                        var tmpForeignCurrencies = angular.copy(_this.currenciesList);
                        _this.centreGroup.currencies = [];
                        if (!_this.isValueNullOrUndefined(_this.currenciesList)) {
                            for (var i = 0; i < _this.currenciesList.length; i++) {
                                var fCurr = _this.$filter("filter")(tmpForeignCurrencies, { id: _this.currenciesList[i].id }, true);
                                var isSet = false;
                                if (!_this.isValueNullOrUndefined(fCurr)) {
                                    isSet = fCurr[0].is_set;
                                }
                                _this.centreGroup.currencies.push({
                                    id: _this.currenciesList[i].id,
                                    currency_name: _this.currenciesList[i].currency_name,
                                    currency_code: _this.currenciesList[i].currency_code,
                                    currency_code_name: _this.currenciesList[i].currency_code_name,
                                    is_set: isSet
                                });
                            }
                        }
                    }
                    catch (e) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        _this.hideLoaderAfterAllIsLoaded();
                    }
                }, function (response) {
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
            };
            CgAdminController.prototype.getIsCentreGroupReadOnly = function () {
                var _this = this;
                this.isCgPropertyAdmin = false;
                this.isCgAppMatrixAdmin = false;
                this.isCgOrdererAdmin = false;
                this.isCgRequestorAdmin = false;
                if (this.isNewCg) {
                    this.isCgPropertyAdmin = true;
                    this.isCgAppMatrixAdmin = true;
                    this.isCgOrdererAdmin = true;
                    this.isCgRequestorAdmin = true;
                    this.isCgReadOnly = false;
                    return;
                }
                this.showLoader(this.isError);
                this.$http.get(this.getRegetRootUrl() + '/RegetAdmin/GetCgAdminRoles?cgId=' + this.selectedCentreGroupId +
                    '&userId=' + this.participantId +
                    '&t=' + new Date().getTime(), {}).then(function (response) {
                    try {
                        if (!_this.isValueNullOrUndefined(response.data)) {
                            _this.adminRoles = _this.getUserRoles(response.data);
                            _this.isCgPropertyAdmin = _this.adminRoles.is_cg_property_admin;
                            _this.isCgAppMatrixAdmin = _this.adminRoles.is_cg_appmatrix_admin;
                            _this.isCgOrdererAdmin = _this.adminRoles.is_cg_orderer_admin;
                            _this.isCgRequestorAdmin = _this.adminRoles.is_cg_requestor_admin;
                            _this.isReadOnly = _this.adminRoles.is_readonly;
                        }
                        _this.setReadOnly();
                        _this.isCentresLoaded = true;
                        if (_this.isReadOnly) {
                            //this.isParticipantLoaded = true;
                            _this.isOfficeLoaded = true;
                            _this.isCentresLoaded = true;
                            _this.isCgReadOnly = true;
                        }
                        else {
                            //this.getParticipantData();
                            _this.getOfficeData();
                            _this.getNotAssignedCentresData();
                        }
                        _this.isCurrenciesLoaded = true;
                    }
                    catch (e) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        _this.hideLoaderAfterAllIsLoaded();
                    }
                }, function (response) {
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
            };
            CgAdminController.prototype.getPurchaseGroupsCount = function () {
                var _this = this;
                this.showLoader(this.isError);
                this.$http.get(this.getRegetRootUrl() + '/RegetAdmin/GetPurchaseGroupsCount?cgId='
                    + this.selectedCentreGroupId + '&t=' + new Date().getTime(), {}).then(function (response) {
                    try {
                        _this.purchaseGroupCount = Number(response.data);
                        _this.populateNextPurchaseGroup();
                    }
                    catch (e) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        _this.hideLoaderAfterAllIsLoaded();
                    }
                }, function (response) {
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
            };
            CgAdminController.prototype.getNotAssignedCentresData = function () {
                var _this = this;
                this.showLoader(this.isError);
                this.$http.get(this.getRegetRootUrl() + '/RegetAdmin/GetActiveCompanyCentresData?cgId=' + this.selectedCentreGroupId
                    + '&officeId=' + this.selectedOfficeId + '&t=' + new Date().getTime(), {}).then(function (response) {
                    try {
                        _this.notAssignedCentresList = _this.getCentreList(response.data);
                        _this.isCentresLoaded = true;
                    }
                    catch (e) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        _this.hideLoaderAfterAllIsLoaded();
                    }
                }, function (response) {
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
            };
            CgAdminController.prototype.populateNextPurchaseGroup = function () {
                var _this = this;
                this.showLoader(this.isError);
                var strCount = "";
                if (!this.isValueNullOrUndefined(this.purchaseGroupCount)) {
                    strCount = this.purchaseGroupCount.toString();
                    if (this.purchaseGroupCount <= 25) {
                        var loadingMsg = this.loadingDataText;
                        angular.element("#spanLoading").html(loadingMsg);
                    }
                }
                var strCountOutOf = "";
                if (!this.isValueNullOrUndefined(this.pgItemCount)) {
                    strCountOutOf = this.pgItemCount.toString();
                }
                loadingMsg = this.loadingPurchaseGroupsText.replace("##", strCount).replace("#", strCountOutOf);
                $("#spanLoading").html(loadingMsg);
                this.$http.get(this.getRegetRootUrl() + '/RegetAdmin/GetPurchaseGroupsByCgId?cgId=' + this.selectedCentreGroupId
                    + '&indexFrom=' + this.purchaseGroupList.length + '&t=' + new Date().getTime(), {}).then(function (response) {
                    try {
                        var data = response.data;
                        if (data.length > 0) {
                            _this.pgItemCount++;
                            _this.purchaseGroupList.push(response.data[0]);
                            _this.populateNextPurchaseGroup();
                        }
                        else {
                            $("#divPgLoading").hide();
                            _this.hideLoaderAfterAllIsLoaded();
                            $("#spanLoading").html(_this.loadTextBkp);
                            if (_this.isDeactivatedPgLoading) {
                                _this.isDeactivatedPgLoading = false;
                                _this.isDeactivatedPgLoaded = true;
                                _this.displayHideDisablePgs();
                            }
                        }
                    }
                    catch (e) {
                        $("#spanLoading").html(_this.loadTextBkp);
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        _this.hideLoaderAfterAllIsLoaded();
                    }
                }, function (response) {
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
            };
            CgAdminController.prototype.populateCentresSearch = function (searchText, fullSearchText, deferred) {
                var _this = this;
                if (this.isValueNullOrUndefined(deferred)) {
                    return;
                }
                //this.showLoaderBoxOnly();
                this.$http.get(this.getRegetRootUrl() + '/RegetAdmin/GetCgCentres?searchText=' + searchText
                    + '&isDisabledCgCentresLoaded=' + this.isDeactivatedCgDisplayed + '&t=' + new Date().getTime(), {}).then(function (response) {
                    try {
                        _this.centres = _this.getCentresList(response.data);
                        deferred.resolve(_this.filterCentres(fullSearchText));
                    }
                    catch (e) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        _this.hideLoaderAfterAllIsLoaded();
                    }
                }, function (response) {
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
            };
            CgAdminController.prototype.isCentreAssigned = function (centre) {
                var _this = this;
                this.$http.get(this.getRegetRootUrl() + '/RegetAdmin/IsCentreAssigned?centreId=' + centre.id +
                    '&cgId=' + this.selectedCentreGroupId +
                    '&t=' + new Date().getTime(), {}).then(function (response) {
                    try {
                        if (_this.isValueNullOrUndefined(_this.centreGroup.centre)) {
                            _this.centreGroup.centre = [];
                        }
                        var strRes = response.data.toString();
                        if (_this.isStringValueNullOrEmpty(strRes)) {
                            var tmpCentre = new RegetApp.Centre(centre.id, centre.name);
                            _this.centreGroup.centre.push(tmpCentre);
                        }
                        else {
                            var msg = _this.moveCentreMsgText.replace("{0}", centre.name).replace("{1}", String(response.data));
                            var confirm = _this.$mdDialog.confirm()
                                .title(_this.moveCentreHeaderText)
                                .textContent(msg)
                                .ariaLabel(_this.moveCentreHeaderText)
                                .targetEvent()
                                .ok(_this.cancelText)
                                .cancel(_this.moveText);
                            _this.$mdDialog.show(confirm).then(function () {
                            }, function () {
                                var tmpCentre = new RegetApp.Centre(centre.id, centre.name);
                                _this.centreGroup.centre.push(tmpCentre);
                            });
                        }
                    }
                    catch (e) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        _this.hideLoaderAfterAllIsLoaded();
                    }
                }, function (response) {
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
            };
            CgAdminController.prototype.populateSuppliers = function (supplierGroupId, searchText, fullSearchText, deferred) {
                var _this = this;
                if (this.isValueNullOrUndefined(deferred)) {
                    return;
                }
                this.$http.get(this.getRegetRootUrl() + '/RegetAdmin/GetActiveSuppliers?supplierGroupId=' + supplierGroupId
                    + '&searchText=' + searchText + '&t=' + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        _this.suppliers = tmpData;
                        deferred.resolve(_this.filterSupplier(fullSearchText));
                    }
                    catch (e) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        _this.hideLoaderAfterAllIsLoaded();
                    }
                }, function (response) {
                    _this.hideLoader();
                    deferred.resolve(_this.suppliers);
                    _this.displayErrorMsg();
                });
            };
            CgAdminController.prototype.saveCentreGroupDetails = function (savedMsg, missingMandatoryMsg, errMsg) {
                var _this = this;
                if (!(this.isCentregGroupDataValid())) {
                    this.showAlert(this.warningText, missingMandatoryMsg, this.closeText);
                    return;
                }
                this.errMsg = errMsg;
                this.showLoader(this.isError);
                var jsonCentreGroupData = JSON.stringify(this.centreGroup);
                this.$http.post(this.getRegetRootUrl() + 'RegetAdmin/SaveCentreGroupData', jsonCentreGroupData).then(function (response) {
                    try {
                        var tmpResponse = response;
                        if (!_this.isNewCg) {
                            _this.updateCgDropdownName(_this.centreGroup.id);
                        }
                        _this.hideLoader();
                        if (!_this.isStringValueNullOrEmpty(tmpResponse.data.string_value)) {
                            //duplicity name
                            _this.showAlert(_this.warningText, tmpResponse.data.string_value, _this.closeText);
                            return;
                        }
                        _this.showToast(savedMsg);
                        if (_this.isNewCg) {
                            _this.selectedCentreGroupId = tmpResponse.data.int_value;
                            ;
                            window.location.href = (_this.getRegetRootUrl() + 'RegetAdmin/CentreGroup?cgId=' + _this.selectedCentreGroupId + '&t=' + new Date().getTime());
                            return;
                        }
                        //Add centre to all of the Pg
                        if (!_this.isValueNullOrUndefined(_this.purchaseGroupList) && _this.purchaseGroupList.length > 0) {
                            for (var i = 0; i < _this.centreGroup.centre.length; i++) {
                                var centre = _this.$filter("filter")(_this.purchaseGroupList[0].centre, { id: _this.centreGroup.centre[i].id }, true);
                                if (_this.isValueNullOrUndefined(centre[0])) {
                                    for (var j = 0; j < _this.purchaseGroupList.length; j++) {
                                        var tmpCentre = new RegetApp.Centre(_this.centreGroup.centre[i].id, _this.centreGroup.centre[i].name);
                                        _this.purchaseGroupList[j].centre.push(tmpCentre);
                                    }
                                }
                            }
                        }
                        //Remove centre
                        if (!_this.isValueNullOrUndefined(_this.purchaseGroupList) && _this.purchaseGroupList.length > 0) {
                            for (var i = 0; i < _this.purchaseGroupList.length; i++) {
                                for (var j = _this.purchaseGroupList[i].centre.length - 1; j >= 0; j--) {
                                    var delCentre = _this.$filter("filter")(_this.centreGroup.centre, { id: _this.purchaseGroupList[i].centre[j].id }, true);
                                    if (_this.isValueNullOrUndefined(delCentre[0])) {
                                        _this.purchaseGroupList[i].centre.splice(j, 1);
                                    }
                                }
                            }
                        }
                        //set active
                        var modCg = _this.$filter("filter")(_this.centreGroupList, { id: _this.centreGroup.id }, true);
                        if (!_this.isValueNullOrUndefined(modCg[0])) {
                            modCg[0].is_active = _this.centreGroup.is_active;
                        }
                    }
                    catch (e) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        _this.hideLoaderAfterAllIsLoaded();
                    }
                }, function (response) {
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
            };
            CgAdminController.prototype.getParentPurchaseGroupData = function (purchaseGroup) {
                var _this = this;
                if (this.isValueNullOrUndefined(this.parentPurchaseGroups)) {
                    this.showLoader(this.isError);
                    this.$http.get(this.getRegetRootUrl() + '/RegetAdmin/GetParentPurchaseGroups?t=' + new Date().getTime(), {}).then(function (response) {
                        try {
                            var tmpData = response.data;
                            _this.parentPurchaseGroups = tmpData;
                            _this.setLoadPgEditData(purchaseGroup);
                        }
                        catch (e) {
                            _this.hideLoader();
                            _this.displayErrorMsg();
                        }
                        finally {
                            _this.hideLoaderAfterAllIsLoaded();
                        }
                    }, function (response) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    });
                }
            };
            CgAdminController.prototype.deletePurchaseGroup = function (iPgId, strPgName, sindex, ev, strTitleText, strMsgText, strDeleteText, strCancelText, strSavedMsgText) {
                var _this = this;
                var strTitle = strTitleText;
                var strMsg = strMsgText.replace('{0}', strPgName).replace('{0}', strPgName);
                var confirm = this.$mdDialog.confirm()
                    .title(strTitle)
                    .textContent(strMsg)
                    .ariaLabel(strTitle)
                    .targetEvent(ev)
                    .ok(strCancelText)
                    .cancel(strDeleteText);
                this.$mdDialog.show(confirm).then(function () {
                }, function () {
                    _this.showLoader(_this.isError);
                    _this.$http.post(_this.getRegetRootUrl() + 'RegetAdmin/DeletePurchaseGroup?pgId=' + iPgId
                        + '&cgId=' + _this.selectedCentreGroupId + '&t=' + new Date().getTime(), {}).then(function (response) {
                        try {
                            _this.hideLoader();
                            var result = response.data;
                            if (!_this.isStringValueNullOrEmpty(result.string_value) && (result.string_value === "disabled")) {
                                //pg disabled
                                var disPg = _this.$filter("filter")(_this.purchaseGroupList, { id: iPgId }, true);
                                if (!_this.isValueNullOrUndefined(disPg[0])) {
                                    disPg[0].is_active = false;
                                }
                                disPg[0].is_visible = _this.isDeactivatedPgDisplayed;
                                if (_this.isDeactivatedPgDisplayed) {
                                    $("#divAppMatrix_" + disPg[0].id).show();
                                }
                                else {
                                    $("#divAppMatrix_" + disPg[0].id).hide();
                                }
                                var msg = _this.purchaseGroupDisabledText.replace('{0}', strPgName);
                                _this.showAlert(_this.notificationText, msg, _this.closeText);
                            }
                            else {
                                //pg deleted
                                _this.purchaseGroupList.splice(sindex, 1);
                                var msg = _this.purchaseGroupDeletedText.replace('{0}', strPgName);
                                _this.showAlert(_this.notificationText, msg, _this.closeText);
                            }
                        }
                        catch (e) {
                            _this.hideLoader();
                            _this.displayErrorMsg();
                        }
                        finally {
                            _this.hideLoaderAfterAllIsLoaded();
                        }
                    }, function (response) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    });
                });
            };
            CgAdminController.prototype.savePurchaseGroup = function (savedMsg, errMsg) {
                var _this = this;
                for (var i = 0; i < this.editPurchaseGroup.purchase_group_limit.length; i++) {
                    this.editPurchaseGroup.purchase_group_limit[i].limit_bottom = this.convertTextToDecimal(this.editPurchaseGroup.purchase_group_limit[i].limit_bottom_text);
                    this.editPurchaseGroup.purchase_group_limit[i].limit_top = this.convertTextToDecimal(this.editPurchaseGroup.purchase_group_limit[i].limit_top_text);
                    this.editPurchaseGroup.purchase_group_limit[i].limit_bottom_text_ro = this.editPurchaseGroup.purchase_group_limit[i].limit_bottom_text;
                    this.editPurchaseGroup.purchase_group_limit[i].limit_top_text_ro = this.editPurchaseGroup.purchase_group_limit[i].limit_top_text;
                }
                if (!this.isPurchaseGroupValid()) {
                    return;
                }
                this.errMsg = errMsg;
                this.showLoader(this.isError);
                if (this.editPurchaseGroup.id < 0) {
                    //New Group
                    this.editPurchaseGroup.centre_group_id = this.selectedCentreGroupId;
                    var jsonPurchaseGroupData = JSON.stringify(this.editPurchaseGroup);
                    this.$http.post(this.getRegetRootUrl() + 'RegetAdmin/AddNewPurchaseGroupData', jsonPurchaseGroupData).then(function (response) {
                        try {
                            var tmpPg = response.data;
                            var newPg = tmpPg;
                            var tmpNewEmptyPg = angular.copy(_this.purchaseGroupList[_this.purchaseGroupList.length - 1]);
                            _this.purchaseGroupList.splice(_this.purchaseGroupList.length - 1, 1);
                            _this.purchaseGroupList.push(newPg);
                            _this.purchaseGroupList.push(tmpNewEmptyPg);
                            _this.hideLoader();
                            _this.closePurchaseGroup();
                            _this.showToast(savedMsg);
                        }
                        catch (e) {
                            _this.hideLoader();
                            _this.displayErrorMsg();
                        }
                        finally {
                            _this.hideLoaderAfterAllIsLoaded();
                        }
                    }, function (response) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    });
                }
                else {
                    this.purchaseGroupOrig.group_loc_name = this.editPurchaseGroup.group_loc_name;
                    this.purchaseGroupOrig.parent_pg_id = this.editPurchaseGroup.parent_pg_id;
                    this.purchaseGroupOrig.self_ordered = this.editPurchaseGroup.self_ordered;
                    this.purchaseGroupOrig.is_active = this.editPurchaseGroup.is_active;
                    //this.purchaseGroupOrig.is_approval_only = this.editPurchaseGroup.is_approval_only;
                    this.purchaseGroupOrig.is_approval_needed = this.editPurchaseGroup.is_approval_needed;
                    this.purchaseGroupOrig.is_order_needed = this.editPurchaseGroup.is_order_needed;
                    this.purchaseGroupOrig.purchase_type = this.editPurchaseGroup.purchase_type;
                    this.updateLocalTexts(this.editPurchaseGroup, this.purchaseGroupOrig);
                    this.updateAppMatrix();
                    this.updateRequestors();
                    this.updateOrderers();
                    this.purchaseGroupOrig.default_supplier = this.editPurchaseGroup.default_supplier;
                    var jsonPurchaseGroupData = JSON.stringify(this.editPurchaseGroup);
                    this.$http.post(this.getRegetRootUrl() + 'RegetAdmin/SavePurchaseGroupData', jsonPurchaseGroupData).then(function (response) {
                        try {
                            _this.hideLoader();
                            _this.closePurchaseGroup();
                            _this.showToast(savedMsg);
                        }
                        catch (e) {
                            _this.hideLoader();
                            _this.displayErrorMsg();
                        }
                        finally {
                            _this.hideLoaderAfterAllIsLoaded();
                        }
                    }, function (response) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    });
                }
                if (this.editPurchaseGroup.is_active === false) {
                    var disPg = this.$filter("filter")(this.purchaseGroupList, { id: this.editPurchaseGroup.id }, true);
                    disPg[0].is_visible = this.isDeactivatedPgDisplayed;
                    if (this.isDeactivatedPgDisplayed) {
                        $("#divAppMatrix_" + disPg[0].id).show();
                    }
                    else {
                        $("#divAppMatrix_" + disPg[0].id).hide();
                    }
                }
            };
            CgAdminController.prototype.scrollToPgHeader = function () {
                try {
                    if (this.isValueNullOrUndefined(this.editPurchaseGroup)) {
                        return;
                    }
                    var element = $("#divAppMatrix_" + this.editPurchaseGroup.id);
                    if (!this.isValueNullOrUndefined(element)) {
                        element[0].scrollIntoView();
                    }
                }
                catch (_a) { }
            };
            CgAdminController.prototype.isPurchaseGroupValid = function () {
                this.saveErrMsg = null;
                if (this.isValueNullOrUndefined(this.editPurchaseGroup)) {
                    this.saveErrMsg = this.enterText + " " + this.purchaseGroupNameText;
                    return false;
                }
                //var isNotValid = false;
                if (this.editPurchaseGroup.is_active === false) {
                    return true;
                }
                if (this.isStringValueNullOrEmpty(this.editPurchaseGroup.local_text[0].text)) {
                    this.saveErrMsg = this.enterText + " " + this.purchaseGroupNameText;
                    return false;
                }
                if (this.editPurchaseGroup.parent_pg_id < 0) {
                    this.saveErrMsg = this.enterText + " " + this.parentPgNameText;
                    return false;
                }
                if (!this.isValueNullOrUndefined(this.editPurchaseGroup.requestor) &&
                    this.editPurchaseGroup.requestor.length > 0) {
                    if (this.editPurchaseGroup.is_approval_needed) {
                        if (this.isValueNullOrUndefined(this.editPurchaseGroup.purchase_group_limit)
                            || this.editPurchaseGroup.purchase_group_limit.length === 0) {
                            this.saveErrMsg = "At least 1 Approval Level must be set. Please delete requestors or save Purchase group as Non Active, then you can save it without approval level";
                            return false;
                        }
                        else {
                            var isSet = false;
                            for (var i = 0; i < this.editPurchaseGroup.purchase_group_limit.length; i++) {
                                if (this.editPurchaseGroup.purchase_group_limit[i].is_visible === true) {
                                    isSet = true;
                                    break;
                                }
                            }
                            if (isSet === false) {
                                this.saveErrMsg = "At least 1 Approval Level must be set. Please delete requestors or save Purchase group as Non Active, then you can save it without approval level";
                                return false;
                            }
                        }
                    }
                    if (this.editPurchaseGroup.self_ordered === false) {
                        if (this.editPurchaseGroup.is_order_needed) {
                            if (this.isValueNullOrUndefined(this.editPurchaseGroup.orderer)
                                || this.editPurchaseGroup.orderer.length === 0) {
                                this.saveErrMsg = "At least 1 Orderer must be set. Please delete requestors or save Purchase group as Non Active, then you can save it without orderer";
                                return false;
                            }
                        }
                    }
                }
                for (var iLimitId = 0; iLimitId < this.editPurchaseGroup.purchase_group_limit.length; iLimitId++) {
                    if (!(this.editPurchaseGroup.purchase_group_limit[iLimitId].is_visible)) {
                        continue;
                    }
                    var dBottom = this.editPurchaseGroup.purchase_group_limit[iLimitId].limit_bottom;
                    //if (!this.isStringValueNullOrEmpty(strBottom)) {
                    //    strBottom = strBottom.replace(this.decimalSeparator, '.');
                    //}
                    if (!(this.editPurchaseGroup.purchase_group_limit[iLimitId].is_bottom_unlimited) &&
                        (this.isValueNullOrUndefined(this.editPurchaseGroup.purchase_group_limit[iLimitId].limit_bottom)) ||
                        isNaN(dBottom)) {
                        this.saveErrMsg = "Bottom Limit " + iLimitId + " is empty";
                        return false;
                    }
                    if (!(this.editPurchaseGroup.purchase_group_limit[iLimitId].is_top_unlimited) &&
                        (this.isValueNullOrUndefined(this.editPurchaseGroup.purchase_group_limit[iLimitId].limit_top) ||
                            isNaN(this.editPurchaseGroup.purchase_group_limit[iLimitId].limit_top))) {
                        this.saveErrMsg = "Top Limit " + iLimitId + " is empty";
                        return false;
                    }
                    if (this.editPurchaseGroup.purchase_group_limit[iLimitId].manager_role.length === 0) {
                        this.saveErrMsg = "Approval Men are empty";
                        return false;
                    }
                    if (!this.editPurchaseGroup.purchase_group_limit[iLimitId].is_bottom_unlimited &&
                        !this.editPurchaseGroup.purchase_group_limit[iLimitId].is_top_unlimited) {
                        if (this.editPurchaseGroup.purchase_group_limit[iLimitId].limit_bottom > this.editPurchaseGroup.purchase_group_limit[iLimitId].limit_top) {
                            this.saveErrMsg = "Bottom Limit is greater than Top Limit";
                            return false;
                        }
                    }
                    if (this.isValueNullOrUndefined(this.editPurchaseGroup.purchase_group_limit[iLimitId].manager_role) ||
                        this.isValueNullOrUndefined(this.editPurchaseGroup.purchase_group_limit[iLimitId].manager_role.length === 0)) {
                        this.saveErrMsg = "At least 1 Approval manager must be set";
                        return false;
                    }
                }
                return true;
            };
            //*****************************************************************************************************
            CgAdminController.prototype.clearDefaultSupp = function () {
                this.selectedDefaultSupplier = undefined;
                this.searchstringdefaultsupplier = undefined;
            };
            CgAdminController.prototype.isCentregGroupDataValid = function () {
                this.cgPropErrMsg = null;
                //var isValid = true;
                if (this.isStringValueNullOrEmpty(this.centreGroup.name)) {
                    this.cgPropErrMsg = this.enterText + " " + this.nameText;
                    return false;
                }
                if (this.centreGroup.is_active === true) {
                    if (this.isValueNullOrUndefined(this.centreGroup.company_id)) {
                        this.cgPropErrMsg = this.enterText + " " + this.companyText;
                        return false;
                    }
                }
                if (this.centreGroup.is_active === true) {
                    if (this.isValueNullOrUndefined(this.centreGroup.currency_id)) {
                        this.cgPropErrMsg = this.enterText + " " + this.currencyText;
                        return false;
                    }
                }
                return true;
            };
            CgAdminController.prototype.populateCgCentres = function () {
                this.showLoader(this.isError);
                try {
                    if (this.isValueNullOrUndefined(this.centreGroup.centre) ||
                        this.centreGroup.centre.length === 0) {
                        $("#divCgAppMatrixPanel").hide();
                    }
                }
                catch (ex) {
                    this.displayErrorMsg();
                }
                finally {
                    this.hideOrdererSupplierAutoCompl();
                }
            };
            CgAdminController.prototype.displayHideDisablePgs = function () {
                //display, hide dissabled pgs
                var disPgs = this.$filter("filter")(this.purchaseGroupList, { is_active: false }, true);
                if (!this.isValueNullOrUndefined(disPgs[0])) {
                    for (var i = 0; i < disPgs.length; i++) {
                        disPgs[i].is_visible = this.isDeactivatedPgDisplayed;
                        if (this.isDeactivatedPgDisplayed) {
                            $("#divAppMatrix_" + disPgs[i].id).show();
                        }
                        else {
                            $("#divAppMatrix_" + disPgs[i].id).hide();
                        }
                    }
                }
            };
            CgAdminController.prototype.displayCgAdminDetail = function (userId) {
                this.displayElement("btnCollapseCgAdmin_" + userId);
                this.hideElement("btnExpandCgAdmin_" + userId);
                this.displayElementSlow("divCgAdmin_" + userId);
            };
            CgAdminController.prototype.hideCgAdminDetail = function (userId) {
                this.hideElement("btnCollapseCgAdmin_" + userId);
                this.displayElement("btnExpandCgAdmin_" + userId);
                this.hideElementSlow("divCgAdmin_" + userId);
            };
            CgAdminController.prototype.setSelectedCurrencyCode = function () {
                var _this = this;
                var selectedCurr = this.currenciesList.filter(function (currency) {
                    return currency.id === _this.selectedCurrencyId;
                });
                if (!this.isValueNullOrUndefined(selectedCurr) && selectedCurr.length > 0) {
                    this.selectedCurrencyCode = selectedCurr[0].currency_code;
                    this.selectedCurrencyCodeName = selectedCurr[0].currency_code_name;
                }
            };
            CgAdminController.prototype.setReadOnly = function () {
                var _this = this;
                if (this.isAllCgReadOnly) {
                    return;
                }
                var isOfficeReadOnly = this.getIsOfficeReadOnly();
                var selectedCurr = this.currenciesList.filter(function (currency) {
                    return currency.id === _this.selectedCurrencyId;
                });
                if (!this.isCgPropertyAdmin || this.isError) {
                    $("#btnSaveCgDetails").hide();
                    $("#hrCgDetail").hide();
                }
                else {
                    $("#btnSaveCgDetails").show();
                    $("#hrCgDetail").show();
                }
            };
            CgAdminController.prototype.getIsOfficeReadOnly = function () {
                var isOfficeReadOnly = this.$scope.IsReadOnly || this.offices === null || this.offices.length < 2;
                if (!isOfficeReadOnly) {
                    var currOffice = this.$filter("filter")(this.offices, { company_id: this.selectedOfficeId }, true);
                    if (currOffice === null || currOffice.length === 0) {
                        isOfficeReadOnly = true;
                    }
                }
                return isOfficeReadOnly;
            };
            CgAdminController.prototype.displaySearchCentre = function () {
                var appCentre = angular.element("#appCentre");
                var btnFind = angular.element("#btnFindCentre");
                if (appCentre.is(':visible')) {
                    appCentre.hide();
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
            };
            CgAdminController.prototype.centreQuerySearch = function (query) {
                var results = null;
                if (!this.isValueNullOrUndefined(query) && query.length === 1) {
                    var deferred = this.$q.defer();
                    this.populateCentresSearch(query, query, deferred);
                    return deferred.promise;
                }
                else {
                    if (this.isValueNullOrUndefined(this.centres)) {
                        var deferred = this.$q.defer();
                        this.populateCentresSearch(query.substring(0, 2), query, deferred);
                        return deferred.promise;
                    }
                    else {
                        return this.filterCentres(query);
                    }
                }
            };
            CgAdminController.prototype.centreSelectedItemChange = function (item) {
                if (this.isValueNullOrUndefined(item)) {
                    return;
                }
                this.selectedCentreGroupId = item.centre_group_id;
                this.getCentreGroupData();
            };
            CgAdminController.prototype.filterCentres = function (query) {
                var searchCentres = [];
                if (this.isValueNullOrUndefined(query) || query.length < 1) {
                    return null;
                }
                angular.forEach(this.centres, function (centre) {
                    if (centre.name.toLowerCase().indexOf(query.toLowerCase()) > -1) {
                        searchCentres.push(centre);
                    }
                });
                return searchCentres;
            };
            CgAdminController.prototype.exportToExcel = function () {
                this.showLoaderBoxOnly();
                try {
                    window.open(this.getRegetRootUrl() + 'Report/GetCentreGroup?cgId=' + this.selectedCentreGroupId + '&t=' + new Date().getTime());
                }
                catch (ex) {
                    this.displayErrorMsg();
                }
                finally {
                    this.hideLoader();
                }
            };
            CgAdminController.prototype.setCurrency = function () {
                this.centreGroup.currency_id = this.selectedCurrencyId;
            };
            CgAdminController.prototype.setOffice = function () {
                this.centreGroup.company_id = this.selectedOfficeId;
                this.getNotAssignedCentresData();
                this.populateCurrencies(-1, -1, this.selectedOfficeId);
            };
            CgAdminController.prototype.getIsAllCgReadOnly = function () {
                var iAllCgReadOnly = $("#spanIsAllCgReadOnly").text();
                this.isAllCgReadOnly = (iAllCgReadOnly === '1');
            };
            CgAdminController.prototype.displayAppManAutoCompl = function (limitId, isMobile) {
                if (isMobile) {
                    $("#appManAutoComplMobile_" + limitId).show();
                }
                else {
                    $("#appManAutoCompl_" + limitId).show();
                }
            };
            CgAdminController.prototype.displayDeputyOrdererAutoCompl = function () {
                $("#autoDeputyOrderer").show();
            };
            CgAdminController.prototype.displayCgAdminAutoCompl = function () {
                $("#autoCgAdmin").show();
            };
            CgAdminController.prototype.displayCentreAutoCompl = function () {
                $("#autoCentre").show();
            };
            CgAdminController.prototype.displayOrdererSupplierAutoCompl = function () {
                this.selectedOrdererSupplierOrderer = null;
                this.selectedOrdererSupplierSupplier = null;
                $("#divOrdererSupplierAuto").show();
            };
            CgAdminController.prototype.hideOrdererSupplierAutoCompl = function () {
                this.searchstringorderersupplierorderer = null;
                var $autWrap = $("#divOrdererSupplierAuto").children().first();
                var $autChild = $autWrap.children().first();
                $autChild.val('');
                $("#divOrdererSupplierAuto").hide();
            };
            CgAdminController.prototype.hideDeputyOrdererAutoCompl = function () {
                this.searchstringdeputyorderer = null;
                var $autWrap = $("#autoDeputyOrderer").children().first();
                var $autChild = $autWrap.children().first();
                $autChild.val('');
                $("#autoDeputyOrderer").hide();
            };
            CgAdminController.prototype.hideCgAdminAutoCompl = function () {
                this.searchstringcgadmin = null;
                var $autWrap = $("#autoDeputyOrderer").children().first();
                var $autChild = $autWrap.children().first();
                $autChild.val('');
                $("#autoCgAdmin").hide();
            };
            CgAdminController.prototype.hideCentreAutoCompl = function () {
                var $autWrap = $("#autoCentre").children().first();
                var $autChild = $autWrap.children().first();
                $autChild.val('');
                $("#autoCentre").hide();
            };
            CgAdminController.prototype.hideCentreGroupDetails = function () {
                $("#divCgDetailPanel").hide();
                $("#divCgAppMatrixPanel").hide();
            };
            CgAdminController.prototype.hideLoaderAfterAllIsLoaded = function () {
                if (this.isCurrenciesLoaded &&
                    this.isCentresLoaded &&
                    this.isCgPurchaseGroupsLoaded &&
                    //this.isParticipantLoaded &&
                    this.isOfficeLoaded) {
                    this.hideLoader();
                }
            };
            CgAdminController.prototype.displayRequestorAutoCompl = function (centreId) {
                $("#autoRequestor" + "_" + centreId).show();
            };
            CgAdminController.prototype.displayOrdererAutoCompl = function (limitId) {
                $("#autoOrderer").show();
            };
            CgAdminController.prototype.hideRequestorAutoCompl = function (centreId) {
                this.searchstringrequestor = null;
                this.selectedRequestor = null;
                var $autWrap = $("#autoRequestor_" + centreId).children().first();
                var $autChild = $autWrap.children().first();
                $autChild.val('');
                $("#autoRequestor_" + centreId).hide();
            };
            CgAdminController.prototype.hideOrdererAutoCompl = function () {
                this.searchstringorderer = null;
                this.selectedOrderer = null;
                var $autWrap = $("#autoOrderer").children().first();
                var $autChild = $autWrap.children().first();
                $autChild.val('');
                $("#autoOrderer").hide();
            };
            CgAdminController.prototype.searchNotAssignedCentre = function (strName) {
                var results = strName ? this.filterNotAssignedCentres(strName) : this.notAssignedCentresList, deferred;
                var deferred = this.$q.defer();
                if (strName) {
                    deferred.resolve(results);
                }
                else {
                    this.$timeout(function () { deferred.resolve(results); }, 3000, false);
                }
                return deferred.promise;
            };
            CgAdminController.prototype.filterNotAssignedCentres = function (name) {
                var searchCentres = [];
                if (this.isStringValueNullOrEmpty(name)) {
                    return this.notAssignedCentresList;
                }
                angular.forEach(this.notAssignedCentresList, function (centre) {
                    if (centre.name.toLowerCase().indexOf(name.toLowerCase()) > -1) {
                        searchCentres.push(centre);
                    }
                });
                return searchCentres;
            };
            CgAdminController.prototype.centreCgSelectedItemChange = function (item) {
                if (this.isValueNullOrUndefined(item)) {
                    return;
                }
                try {
                    var centre = this.$filter("filter")(this.centreGroup.centre, { id: item.id }, true);
                    if (this.isValueNullOrUndefined(centre[0])) {
                        this.isCentreAssigned(item);
                    }
                }
                catch (e) {
                    this.displayErrorMsg();
                }
                finally {
                    this.hideCentreAutoCompl();
                }
            };
            CgAdminController.prototype.toggleDisplayDeactivatedCg = function () {
                this.isDeactivatedCgDisplayed = !this.isDeactivatedCgDisplayed;
                this.populateCentreGroups(true);
            };
            CgAdminController.prototype.isForeignCurrency = function () {
                if (this.isValueNullOrUndefined(this.centreGroup) || this.isValueNullOrUndefined(this.centreGroup.currency)) {
                    return false;
                }
                for (var i = 0; i < this.centreGroup.currency.length; i++) {
                    if (this.centreGroup.currency[i].is_set && this.selectedCurrencyId !== this.centreGroup.currency[i].id) {
                        return true;
                    }
                }
                return false;
            };
            CgAdminController.prototype.isForeignCurrencyAvailable = function () {
                if (this.isValueNullOrUndefined(this.centreGroup)) {
                    return false;
                }
                if (this.isValueNullOrUndefined(this.centreGroup.currency)) {
                    return false;
                }
                return (this.centreGroup.currency.length > 1);
            };
            CgAdminController.prototype.isAllCurreenciesChecked = function () {
                var isSelectedAll = true;
                if (this.isValueNullOrUndefined(this.centreGroup) || this.isValueNullOrUndefined(this.centreGroup.currency)) {
                    return false;
                }
                for (var i = 0; i < this.centreGroup.currency.length; i++) {
                    if (!this.centreGroup.currency[i].is_set) {
                        isSelectedAll = false;
                        break;
                    }
                }
                return isSelectedAll;
            };
            CgAdminController.prototype.isCurrenciesIndeterminate = function () {
                if (this.isValueNullOrUndefined(this.centreGroup) || this.isValueNullOrUndefined(this.centreGroup.currency)) {
                    return false;
                }
                return (this.centreGroup.currency.length !== 0 &&
                    !this.isAllCurreenciesChecked());
            };
            CgAdminController.prototype.toggleAllForeignCurrencies = function () {
                if (this.isValueNullOrUndefined(this.centreGroup) || this.isValueNullOrUndefined(this.centreGroup.currency)) {
                    return;
                }
                if (this.isAllCurreenciesChecked()) {
                    for (var i = 0; i < this.centreGroup.currency.length; i++) {
                        this.centreGroup.currency[i].is_set = false;
                    }
                }
                else {
                    for (var i = 0; i < this.centreGroup.currency.length; i++) {
                        this.centreGroup.currency[i].is_set = true;
                    }
                }
            };
            CgAdminController.prototype.toggleForeignCurrency = function (item) {
                if (item.is_set) {
                    item.is_set = false;
                }
                else {
                    item.is_set = true;
                }
            };
            CgAdminController.prototype.toggleFreeSupplier = function () {
                if (this.centreGroup.allow_free_supplier) {
                    this.centreGroup.allow_free_supplier = false;
                }
                else {
                    this.centreGroup.allow_free_supplier = true;
                }
            };
            CgAdminController.prototype.togglePgActive = function (item) {
                if (item.is_active) {
                    item.is_active = false;
                }
                else {
                    item.is_active = true;
                }
                var divPgHeader = angular.element("#divCgMatrixCollHeader_" + item.id);
                if (divPgHeader === null || divPgHeader.length === 0) {
                    return;
                }
                if (item.is_active) {
                    divPgHeader[0].className = 'reget-box-header reget-box-header-level1';
                }
                else {
                    divPgHeader[0].className = 'reget-box-header reget-box-header-level1-disabled';
                }
            };
            CgAdminController.prototype.togglePgOrderNeeded = function (item) {
                //if (item.is_approval_only) {
                //    item.is_approval_only = false;
                //    item.purchase_type = PurchaseGroupType.Standard;
                //} else {
                //    item.is_approval_only = true;
                //    item.purchase_type = PurchaseGroupType.ApprovalOnly;
                //}
                if (item.is_order_needed) {
                    item.is_order_needed = false;
                }
                else {
                    item.is_order_needed = true;
                }
            };
            CgAdminController.prototype.togglePgApprovalNeeded = function (item) {
                if (item.is_approval_needed) {
                    item.is_approval_needed = false;
                }
                else {
                    item.is_approval_needed = true;
                }
            };
            CgAdminController.prototype.toggleTopUnlimited = function (item) {
                if (item.is_top_unlimited) {
                    item.is_top_unlimited = false;
                }
                else {
                    item.is_top_unlimited = true;
                }
                this.checkAppLimitBottomLowerThanTop(item, false);
            };
            CgAdminController.prototype.toggleBottomUnlimited = function (item) {
                if (item.is_bottom_unlimited) {
                    item.is_bottom_unlimited = false;
                }
                else {
                    item.is_bottom_unlimited = true;
                }
                this.checkAppLimitBottomLowerThanTop(item, true);
            };
            CgAdminController.prototype.toggleBottomMultipl = function (item) {
                if (item.is_limit_bottom_multipl) {
                    item.is_limit_bottom_multipl = false;
                }
                else {
                    item.is_limit_bottom_multipl = true;
                }
            };
            CgAdminController.prototype.toggleTopMultipl = function (item) {
                if (item.is_limit_top_multipl) {
                    item.is_limit_top_multipl = false;
                }
                else {
                    item.is_limit_top_multipl = true;
                }
            };
            CgAdminController.prototype.toggleActive = function () {
                if (this.centreGroup.is_active) {
                    this.centreGroup.is_active = false;
                }
                else {
                    this.centreGroup.is_active = true;
                }
            };
            CgAdminController.prototype.toggleDisplayDeactivatedPg = function () {
                this.isDeactivatedPgDisplayed = !this.isDeactivatedPgDisplayed;
                if (!this.isDeactivatedPgLoaded) {
                    this.isDeactivatedPgLoading = true;
                    this.populatePurchaseGroups(true);
                    this.editPurchaseGroup = null;
                }
                else {
                    this.displayHideDisablePgs();
                }
            };
            CgAdminController.prototype.togleAppMatrix = function (pgId) {
                if (this.isCancelEvent === true) {
                    this.isCancelEvent = false;
                    return;
                }
                var pg = this.$filter("filter")(this.purchaseGroupList, { id: pgId }, true);
                if (this.isValueNullOrUndefined(pg[0])) {
                    return;
                }
                if (pg[0].is_html_show !== true) {
                    pg[0].is_html_show = true;
                }
                var phAppMatrix = 'phAppMatrix_' + pgId;
                var imgExpandCollapse = 'impAppMatrixCollExp_' + pgId;
                var spanExpCol = 'spanExpCol_' + pgId;
                if ($("#" + phAppMatrix).is(':visible')) {
                    $("#" + phAppMatrix).slideUp();
                    $("#" + imgExpandCollapse).attr("src", this.getRegetRootUrl() + "Content/Images/Panel/Expand.png");
                    $("#" + imgExpandCollapse).attr("title", this.displayText);
                    $("#" + spanExpCol).text(this.collapseAllText);
                }
                else {
                    $("#" + phAppMatrix).show("slow");
                    $("#" + imgExpandCollapse).attr("src", this.getRegetRootUrl() + "Content/Images/Panel/Collapse.png");
                    $("#" + imgExpandCollapse).attr("title", this.hideText);
                    $("#" + spanExpCol).text(this.expandAllText);
                }
            };
            CgAdminController.prototype.toggleAllPurchaseGroups = function (isCollapse) {
                if (isCollapse) {
                    if (!isCollapse) {
                        $(".reget-appmatrix").show("slow");
                    }
                    else {
                        $(".reget-appmatrix").slideUp();
                    }
                    for (var i = 0; i < this.pgExpandImgs.length; i++) {
                        if (!isCollapse) {
                            this.pgExpandImgs[i].src = this.getRegetRootUrl() + "Content/Images/Panel/Collapse.png";
                            this.pgExpandImgs[i].title = "@Html.Raw(RequestResource.Hide)";
                        }
                        else {
                            this.pgExpandImgs[i].src = this.getRegetRootUrl() + "Content/Images/Panel/Expand.png";
                            this.pgExpandImgs[i].title = "@Html.Raw(RequestResource.Display)";
                        }
                    }
                    return;
                }
                this.isCancelEvent = true;
                this.showLoaderBoxOnly();
                this.pgExpandImgs = document.getElementsByClassName("reget-appmatrix-expcolimg"); //$(".reget-appmatrix-expcolimg");
                this.togglePurchaseGroupRecursive(0, isCollapse);
            };
            CgAdminController.prototype.togglePurchaseGroupRecursive = function (iIndex, isCollapse) {
                var _this = this;
                var idParts = this.pgExpandImgs[iIndex].id.split('_');
                var pgId = parseInt(idParts[1]);
                var promise = this.displayPurchaseGroupAsync(pgId, isCollapse);
                promise.then(function () {
                    //success
                    try {
                        var strCount = iIndex + 1;
                        var loadingMsg = _this.loadingPurchaseGroupsText.replace("##", _this.pgExpandImgs.length.toString()).replace("#", strCount.toString());
                        $("#spanLoadingBoxOnly").html(loadingMsg);
                        if (iIndex < _this.pgExpandImgs.length - 1) {
                            _this.togglePurchaseGroupRecursive(iIndex + 1, isCollapse);
                        }
                        else {
                            if (!isCollapse) {
                                $(".reget-appmatrix").show("slow");
                            }
                            else {
                                $(".reget-appmatrix").slideUp();
                            }
                            for (var i = 0; i < _this.pgExpandImgs.length; i++) {
                                if (!isCollapse) {
                                    _this.pgExpandImgs[i].src = _this.getRegetRootUrl() + "Content/Images/Panel/Collapse.png";
                                    _this.pgExpandImgs[i].title = "@Html.Raw(RequestResource.Hide)";
                                }
                                else {
                                    _this.pgExpandImgs[i].src = _this.getRegetRootUrl() + "Content/Images/Panel/Expand.png";
                                    _this.pgExpandImgs[i].title = "@Html.Raw(RequestResource.Display)";
                                }
                            }
                            _this.hideLoader();
                        }
                    }
                    catch (e) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                }, function (reason) {
                    //fail
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
            };
            CgAdminController.prototype.displayPurchaseGroupAsync = function (pgId, isCollapse) {
                var deferredPg = this.$q.defer();
                var pg = this.$filter("filter")(this.purchaseGroupList, { id: pgId }, true);
                if (this.isValueNullOrUndefined(pg) || pg.length === 0) {
                    deferredPg.resolve('OK');
                }
                else {
                    if (pg[0].is_html_show === true) {
                        deferredPg.resolve('OK');
                    }
                    else {
                        setTimeout(function () {
                            if (pg[0].is_html_show !== true) {
                                pg[0].is_html_show = true;
                            }
                            deferredPg.resolve('OK');
                        }, 10);
                    }
                }
                return deferredPg.promise;
            };
            CgAdminController.prototype.deleteRequestor = function (iRequestorId, iCentreId, sindex, ev) {
                var requestor = this.$filter("filter")(this.editPurchaseGroup.requestor, { participant_id: iRequestorId, centre_id: iCentreId }, true);
                if (!this.isValueNullOrUndefined(requestor[0])) {
                    if (requestor[0].is_all) {
                        this.$mdDialog.show({
                            template: this.getConfirmDialogTemplate(this.deleteAllRequestorTitleText, this.deleteAllRequestorMsgText, this.removeText, this.cancelText, "confirmDialog()", "closeDialog()"),
                            locals: {
                                cgAdminController: this,
                                requestor: requestor[0],
                                sindex: sindex
                            },
                            controller: this.dialogConfirmRequestorDeleteController
                        });
                        //let confirm = this.$mdDialog.confirm()
                        //    .title(this.deleteAllRequestorTitleText)
                        //    .textContent(this.deleteAllRequestorMsgText)
                        //    .ariaLabel(this.deleteAllRequestorTitleText)
                        //    .targetEvent(ev)
                        //    .ok(this.cancelText)
                        //    .cancel(this.removeText);
                        //this.$mdDialog.show(confirm).then(() => {
                        //}, () => {
                        //    if (this.isValueNullOrUndefined(this.deleteAllRequestorList)) {
                        //        this.deleteAllRequestorList = [];
                        //    }
                        //        var tmpRequestor: Requestor = new Requestor();
                        //        tmpRequestor.participant_id = requestor[0].participant_id;
                        //        tmpRequestor.centre_id = requestor[0].centre_id;
                        //        this.deleteAllRequestorList.push(tmpRequestor);
                        //    this.editPurchaseGroup.requestor.splice(sindex, 1);
                        //});
                    }
                    else {
                        this.editPurchaseGroup.requestor.splice(sindex, 1);
                    }
                }
            };
            CgAdminController.prototype.deleteRequestorConfirmed = function (requestor, sindex) {
                if (this.isValueNullOrUndefined(this.deleteAllRequestorList)) {
                    this.deleteAllRequestorList = [];
                }
                var tmpRequestor = new RegetApp.Requestor();
                tmpRequestor.participant_id = requestor.participant_id;
                tmpRequestor.centre_id = requestor.centre_id;
                this.deleteAllRequestorList.push(tmpRequestor);
                this.editPurchaseGroup.requestor.splice(sindex, 1);
            };
            CgAdminController.prototype.dialogConfirmRequestorDeleteController = function ($scope, $mdDialog, cgAdminController, requestor, sindex) {
                $scope.closeDialog = function () {
                    $mdDialog.hide();
                };
                $scope.confirmDialog = function () {
                    $mdDialog.hide();
                    cgAdminController.deleteRequestorConfirmed(requestor, sindex);
                };
            };
            CgAdminController.prototype.deleteOrderer = function (iOrdererId, sindex, ev) {
                var _this = this;
                var orderer = this.$filter("filter")(this.editPurchaseGroup.orderer, { participant_id: iOrdererId }, true);
                if (!this.isValueNullOrUndefined(orderer[0])) {
                    if (orderer[0].is_all) {
                        var confirm = this.$mdDialog.confirm()
                            .title(this.deleteAllOrdererTitleText)
                            .textContent(this.deleteAllOrdererMsgText)
                            .ariaLabel(this.deleteAllOrdererTitleText)
                            .targetEvent(ev)
                            .ok(this.cancelText)
                            .cancel(this.removeText);
                        this.$mdDialog.show(confirm).then(function () {
                        }, function () {
                            if (_this.isValueNullOrUndefined(_this.deleteAllOrdererList)) {
                                _this.deleteAllOrdererList = [];
                            }
                            var tmpOrderer = new RegetApp.Orderer();
                            tmpOrderer.participant_id = orderer[0].participant_id;
                            _this.deleteAllOrdererList.push(tmpOrderer);
                            _this.editPurchaseGroup.orderer.splice(sindex, 1);
                        });
                    }
                    else {
                        this.editPurchaseGroup.orderer.splice(sindex, 1);
                    }
                }
            };
            CgAdminController.prototype.deleteCentre = function (iCentreId, sindex) {
                var centre = this.$filter("filter")(this.centreGroup.centre, { id: iCentreId }, true);
                if (!this.isValueNullOrUndefined(centre[0])) {
                    this.centreGroup.centre.splice(sindex, 1);
                }
            };
            CgAdminController.prototype.deleteDeputyOrderer = function (iOrdererId, sindex) {
                var orderer = this.$filter("filter")(this.centreGroup.deputy_orderer, { participant_id: iOrdererId }, true);
                if (!this.isValueNullOrUndefined(orderer[0])) {
                    this.centreGroup.deputy_orderer.splice(sindex, 1);
                }
            };
            CgAdminController.prototype.deleteCgAdmin = function (iAdminId, sindex, isCompanyAdmin) {
                if (isCompanyAdmin === true) {
                    this.showAlert(this.warningText, this.cannotDeleteCompanyAdminText, this.closeText);
                }
                else {
                    this.centreGroup.cg_admin.splice(sindex, 1);
                }
            };
            CgAdminController.prototype.deleteOrdererSupplier = function (iOrdererId, iSupplierId, sindex) {
                var orderer = this.$filter("filter")(this.centreGroup.orderer_supplier_appmatrix, { orderer_id: iOrdererId, supplier_id: iSupplierId }, true);
                if (!this.isValueNullOrUndefined(orderer[0])) {
                    this.centreGroup.orderer_supplier_appmatrix.splice(sindex, 1);
                }
            };
            CgAdminController.prototype.searchParticipant = function (strName) {
                return this.filterParticipants(strName);
                //var results = strName ? this.filterParticipants(strName) : this.participants, deferred;
                //var deferred: any = this.$q.defer<any>();
                //if (strName) {
                //    deferred.resolve(results);
                //} else {
                //    this.$timeout(() => { deferred.resolve(results); }, 3000, false);
                //}
                //return deferred.promise;
            };
            CgAdminController.prototype.deputyOrdererSelectedItemChange = function (item) {
                if (this.isValueNullOrUndefined(item)) {
                    return;
                }
                try {
                    var requestor = this.$filter("filter")(this.centreGroup.deputy_orderer, { participant_id: item.id }, true);
                    if (this.isValueNullOrUndefined(requestor[0])) {
                        var tmpDeputyOrder = new RegetApp.Orderer();
                        tmpDeputyOrder.participant_id = item.id;
                        tmpDeputyOrder.surname = item.surname;
                        tmpDeputyOrder.first_name = item.first_name;
                        tmpDeputyOrder.substituted_by = item.substituted_by;
                        tmpDeputyOrder.substituted_until = item.substituted_until;
                        this.centreGroup.deputy_orderer.push(tmpDeputyOrder);
                    }
                }
                catch (e) {
                    this.displayErrorMsg();
                }
                finally {
                    this.hideDeputyOrdererAutoCompl();
                }
            };
            CgAdminController.prototype.ordererSupplierSelectedItemChange = function () {
                if (this.isValueNullOrUndefined(this.selectedOrdererSupplierOrderer) ||
                    this.isValueNullOrUndefined(this.selectedOrdererSupplierSupplier)) {
                    return;
                }
                try {
                    var ordererSupplierAppmatrix = this.$filter("filter")(this.centreGroup.orderer_supplier_appmatrix, {
                        orderer_id: this.selectedOrdererSupplierOrderer.id,
                        supplier_id: this.selectedOrdererSupplierSupplier.id
                    }, true);
                    if (this.isValueNullOrUndefined(ordererSupplierAppmatrix[0])) {
                        var tmpOrdererSupplier = new RegetApp.OrdererSupplier();
                        tmpOrdererSupplier.orderer_id = this.selectedOrdererSupplierOrderer.id;
                        tmpOrdererSupplier.surname = this.selectedOrdererSupplierOrderer.surname;
                        tmpOrdererSupplier.first_name = this.selectedOrdererSupplierOrderer.first_name;
                        tmpOrdererSupplier.supplier_id = this.selectedOrdererSupplierSupplier.id;
                        tmpOrdererSupplier.supplier_name = this.selectedOrdererSupplierSupplier.supp_name;
                        this.centreGroup.orderer_supplier_appmatrix.push(tmpOrdererSupplier);
                    }
                }
                catch (e) {
                    this.displayErrorMsg();
                }
                finally {
                    this.selectedOrdererSupplierOrderer = null;
                    this.selectedOrdererSupplierSupplier = null;
                    this.hideOrdererSupplierAutoCompl();
                }
            };
            CgAdminController.prototype.defaultSupplierQuerySearch = function (query) {
                var results = null;
                $('#txtDefaultSupplier').attr('style', 'max-width:none;');
                if (!this.isValueNullOrUndefined(query) && query.length === 2) {
                    var deferred = this.$q.defer();
                    this.populateSuppliers(this.centreGroup.supplier_group_id, query, query, deferred);
                    return deferred.promise;
                }
                else {
                    if (this.isValueNullOrUndefined(this.suppliers)) {
                        var deferred = this.$q.defer();
                        this.populateSuppliers(this.centreGroup.supplier_group_id, query.substring(0, 2), query, deferred);
                        return deferred.promise;
                    }
                    else {
                        return this.filterSupplier(query);
                    }
                }
            };
            CgAdminController.prototype.filterSupplier = function (query) {
                var searchSuppliers = [];
                $("#txtDefaultSupplier").width(550);
                if (this.isValueNullOrUndefined(query) || query.length < 2) {
                    return null;
                }
                angular.forEach(this.suppliers, function (supplier) {
                    if (supplier.supp_name.toLowerCase().indexOf(query.toLowerCase()) > -1) {
                        searchSuppliers.push(supplier);
                    }
                });
                return searchSuppliers;
            };
            CgAdminController.prototype.checkAppLimitBottomLowerThanTop = function (limitItem, isBottom) {
                var strBottomId = (this.isSmartPhone()) ? 'txtLimitBottomMobile_' : 'txtLimitBottom_';
                strBottomId += limitItem.limit_id;
                var strTopId = (this.isSmartPhone()) ? 'txtLimitTopMobile_' : 'txtLimitTop_';
                strTopId += limitItem.limit_id;
                this.angScopeAny.frmCgAdmin[strBottomId].$error.bottomhighererror = false;
                this.angScopeAny.frmCgAdmin[strBottomId].$setValidity("bottomhighererror", true);
                this.angScopeAny.frmCgAdmin[strTopId].$error.bottomhighererror = false;
                this.angScopeAny.frmCgAdmin[strTopId].$setValidity("bottomhighererror", true);
                if (limitItem.is_bottom_unlimited || limitItem.is_top_unlimited) {
                    return;
                }
                if (isBottom) {
                    if (!this.isStringDecimalNumber(limitItem.limit_bottom_text)) {
                        this.angScopeAny.frmCgAdmin[strBottomId].$error.required = true;
                        this.angScopeAny.frmCgAdmin[strBottomId].$setValidity("required", false);
                        return;
                    }
                }
                else {
                    if (!this.isStringDecimalNumber(limitItem.limit_top_text)) {
                        this.angScopeAny.frmCgAdmin[strTopId].$error.required = true;
                        this.angScopeAny.frmCgAdmin[strTopId].$setValidity("required", false);
                        return;
                    }
                }
                if (this.convertTextToDecimal(limitItem.limit_bottom_text) > this.convertTextToDecimal(limitItem.limit_top_text)) {
                    this.angScopeAny.frmCgAdmin[strBottomId].$error.bottomhighererror = true;
                    this.angScopeAny.frmCgAdmin[strBottomId].$setValidity("bottomhighererror", false);
                    this.angScopeAny.frmCgAdmin[strTopId].$error.bottomhighererror = true;
                    this.angScopeAny.frmCgAdmin[strTopId].$setValidity("bottomhighererror", false);
                }
            };
            CgAdminController.prototype.cgAdminSelectedItemChange = function (item) {
                if (this.isValueNullOrUndefined(item)) {
                    return;
                }
                try {
                    var requestor = this.$filter("filter")(this.centreGroup.cg_admin, { id: item.id }, true);
                    if (this.isValueNullOrUndefined(requestor[0])) {
                        this.centreGroup.cg_admin.push({
                            id: item.id,
                            surname: item.surname,
                            first_name: item.first_name,
                            is_cg_prop_admin: true,
                            is_appmatrix_admin: true,
                            is_orderer_admin: true,
                            is_requestor_admin: true
                        });
                    }
                }
                catch (e) {
                    this.displayErrorMsg();
                }
                finally {
                    this.hideCgAdminAutoCompl();
                }
            };
            CgAdminController.prototype.checkCgPropAdminToggle = function (iCgAdmin) {
                var cgadmn = this.$filter("filter")(this.centreGroup.cg_admin, { id: iCgAdmin }, true);
                if (!this.isValueNullOrUndefined(cgadmn[0])) {
                    if (cgadmn[0].is_cg_prop_admin === true) {
                        cgadmn[0].is_cg_prop_admin = false;
                    }
                    else {
                        cgadmn[0].is_cg_prop_admin = true;
                    }
                }
            };
            CgAdminController.prototype.checkAppMatrixAdminToggle = function (iCgAdmin) {
                var cgadmn = this.$filter("filter")(this.centreGroup.cg_admin, { id: iCgAdmin }, true);
                if (!this.isValueNullOrUndefined(cgadmn[0])) {
                    if (cgadmn[0].is_appmatrix_admin === true) {
                        cgadmn[0].is_appmatrix_admin = false;
                    }
                    else {
                        cgadmn[0].is_appmatrix_admin = true;
                    }
                }
            };
            CgAdminController.prototype.checkRequestorAdminToggle = function (iCgAdmin) {
                var cgadmn = this.$filter("filter")(this.centreGroup.cg_admin, { id: iCgAdmin }, true);
                if (!this.isValueNullOrUndefined(cgadmn[0])) {
                    if (cgadmn[0].is_requestor_admin === true) {
                        cgadmn[0].is_requestor_admin = false;
                    }
                    else {
                        cgadmn[0].is_requestor_admin = true;
                    }
                }
            };
            CgAdminController.prototype.checkOrdererAdminToggle = function (iCgAdmin) {
                var cgadmn = this.$filter("filter")(this.centreGroup.cg_admin, { id: iCgAdmin }, true);
                if (!this.isValueNullOrUndefined(cgadmn[0])) {
                    if (cgadmn[0].is_orderer_admin === true) {
                        cgadmn[0].is_orderer_admin = false;
                    }
                    else {
                        cgadmn[0].is_orderer_admin = true;
                    }
                }
            };
            CgAdminController.prototype.updateCgDropdownName = function (cgId) {
                for (var i = 0; i < this.centreGroupList.length; i++) {
                    if (this.centreGroupList[i].id === cgId) {
                        this.centreGroupList[i].name = this.centreGroup.name;
                        break;
                    }
                }
            };
            CgAdminController.prototype.editAppMatrix = function (purchaseGroup) {
                this.deleteAllRequestorList = null;
                this.suppliers = null;
                this.saveErrMsg = null;
                if (!this.isValueNullOrUndefined(this.editPurchaseGroup)) {
                    var strMessage;
                    if (this.isValueNullOrUndefined(this.purchaseGroupEditingText)) {
                        strMessage = ("Another Purchase Group is Edited");
                    }
                    else {
                        strMessage = (this.purchaseGroupEditingText);
                    }
                    this.showAlert(this.warningText, strMessage, this.closeText);
                    return;
                }
                this.showLoaderBoxOnly();
                this.isErrorAppMatrix = false;
                try {
                    this.getLoadPgEditData(purchaseGroup);
                    if (this.isValueNullOrUndefined(purchaseGroup.default_supplier)) {
                        this.selectedDefaultSupplier = undefined;
                        this.searchstringdefaultsupplier = undefined;
                    }
                    else {
                        this.selectedDefaultSupplier = purchaseGroup.default_supplier;
                        this.searchstringdefaultsupplier = purchaseGroup.default_supplier.supp_name;
                    }
                }
                catch (ex) {
                    this.hideLoader();
                    this.displayErrorMsg();
                }
            };
            CgAdminController.prototype.getLoadPgEditData = function (purchaseGroup) {
                if (!this.isValueNullOrUndefined(this.parentPurchaseGroups)) {
                    this.setLoadPgEditData(purchaseGroup);
                }
                else {
                    this.getParentPurchaseGroupData(purchaseGroup);
                }
            };
            CgAdminController.prototype.setLoadPgEditData = function (purchaseGroup) {
                var pgId = purchaseGroup.id;
                var phAppMatrix = "phAppMatrix_" + pgId;
                var divAppMatrixRO = "divAppMatrixRO_" + pgId;
                var imgEdit = "imgEdit_" + pgId;
                var appendHtml = this.$compile('<app-matrix></app-matrix>')(this.$scope);
                $("#" + phAppMatrix).append(appendHtml);
                $("#" + divAppMatrixRO).hide();
                $("#" + imgEdit).show();
                this.purchaseGroupOrig = purchaseGroup;
                this.editPurchaseGroup = angular.copy(purchaseGroup);
                this.hideLoader();
            };
            CgAdminController.prototype.displayErrorMsgHideForm = function () {
                $("#divCgDetailPanel").hide();
                $("#divCgAppMatrixPanel").hide();
                _super.prototype.displayErrorMsg.call(this);
            };
            CgAdminController.prototype.addPurchaseGroup = function () {
                this.deleteAllRequestorList = null;
                this.suppliers = null;
                if (!this.isValueNullOrUndefined(this.editPurchaseGroup)) {
                    var strMessage;
                    if (this.isValueNullOrUndefined(this.purchaseGroupEditingText)) {
                        strMessage = ("Another Purchase Group is Edited");
                    }
                    else {
                        strMessage = (this.purchaseGroupEditingText);
                    }
                    this.showAlert("Reget Dialog", strMessage, this.cancelText);
                    return;
                }
                $("#divAppMatrix_-1").show();
                if (!($("#phAppMatrix_-1").is(':visible'))) {
                    $("#phAppMatrix_-1").show();
                    $("#impAppMatrixCollExp_-1").attr("src", this.getRegetRootUrl() + "Content/Images/Panel/Collapse.png");
                    $("#impAppMatrixCollExp_-1").attr("title", "Hide");
                    $("#spanExpCol_-1").text("Expand All");
                }
                var newPg = this.$filter("filter")(this.purchaseGroupList, { id: -1 }, true);
                var newPgItem = newPg[0];
                newPgItem.is_order_needed = true;
                newPgItem.is_approval_needed = true;
                this.editAppMatrix(newPgItem);
            };
            //***********************************************************************************************************
            //App Matrix 
            CgAdminController.prototype.parentPgChanged = function () {
                if ((!this.isValueNullOrUndefined(this.editPurchaseGroup.local_text[0]) &&
                    this.isStringValueNullOrEmpty(this.editPurchaseGroup.local_text[0].text)) ||
                    this.editPurchaseGroup.id < 0) {
                    if (!this.isValueNullOrUndefined(this.editPurchaseGroup.parent_pg_id)) {
                        var parentPg = this.$filter("filter")(this.parentPurchaseGroups, { id: this.editPurchaseGroup.parent_pg_id }, true);
                        if (!this.isValueNullOrUndefined(parentPg[0])) {
                            this.editPurchaseGroup.local_text[0].text = parentPg[0].name;
                        }
                    }
                }
            };
            CgAdminController.prototype.closePurchaseGroup = function () {
                if (this.editPurchaseGroup.id === -1) {
                    //new 
                    $("#divAppMatrix_-1").hide();
                }
                var divAppMatrixRO = "divAppMatrixRO_" + this.editPurchaseGroup.id;
                var imgEdit = "imgEdit_" + this.editPurchaseGroup.id;
                $("#divAppMatrixEdit").remove();
                $("#" + divAppMatrixRO).show();
                $("#" + imgEdit).hide();
                this.clearDefaultSupp();
                this.scrollToPgHeader();
                this.editPurchaseGroup = null;
            };
            CgAdminController.prototype.localtext1changed = function (strValue) {
                this.editPurchaseGroup.local_text[0].text = strValue;
            };
            CgAdminController.prototype.localtext2changed = function (strValue) {
                this.editPurchaseGroup.local_text[1].text = strValue;
            };
            CgAdminController.prototype.localtext3changed = function (strValue) {
                this.editPurchaseGroup.local_text[2].text = strValue;
            };
            CgAdminController.prototype.deleteLimit = function (limitId) {
                for (var i = 0; i < this.editPurchaseGroup.purchase_group_limit.length; i++) {
                    if (this.editPurchaseGroup.purchase_group_limit[i].limit_id === limitId) {
                        for (var j = i; j < this.editPurchaseGroup.purchase_group_limit.length - 1; j++) {
                            this.replaceUp(this.editPurchaseGroup.purchase_group_limit[j].limit_id);
                            this.clearAppLimit(this.editPurchaseGroup.purchase_group_limit[j + 1]);
                        }
                        this.clearAppLimit(this.editPurchaseGroup.purchase_group_limit[this.editPurchaseGroup.purchase_group_limit.length - 1]);
                        this.editPurchaseGroup.purchase_group_limit[this.editPurchaseGroup.purchase_group_limit.length - 1].is_visible = false;
                        this.setAppLimitFirstLast();
                        break;
                    }
                }
                if (this.editPurchaseGroup.purchase_group_limit[this.editPurchaseGroup.purchase_group_limit.length - 1].is_visible === false) {
                    $("#btnAddLimit").show();
                }
            };
            CgAdminController.prototype.replaceUp = function (limitId) {
                for (var i = 0; i < this.editPurchaseGroup.purchase_group_limit.length; i++) {
                    if (this.editPurchaseGroup.purchase_group_limit[i].limit_id === limitId) {
                        this.updateAppMatrixUpdateLimit(this.editPurchaseGroup.purchase_group_limit[i + 1], this.editPurchaseGroup.purchase_group_limit[i]);
                        if (this.editPurchaseGroup.purchase_group_limit[i].is_first) {
                            this.editPurchaseGroup.purchase_group_limit[i].is_first = true;
                            this.editPurchaseGroup.purchase_group_limit[i + 1].is_first = false;
                        }
                        if (this.editPurchaseGroup.purchase_group_limit[i].is_last) {
                            this.editPurchaseGroup.purchase_group_limit[i].is_last = false;
                            this.editPurchaseGroup.purchase_group_limit[i + 1].is_last = true;
                        }
                        break;
                    }
                }
            };
            CgAdminController.prototype.clearAppLimit = function (appLimit) {
                appLimit.limit_bottom = null;
                appLimit.limit_top = null;
                appLimit.is_bottom_unlimited = false;
                appLimit.is_top_unlimited = false;
                this.angScopeAny.frmCgAdmin['txtLimitBottom_' + appLimit.limit_id].$error.required = true;
                this.angScopeAny.frmCgAdmin['txtLimitTop_' + appLimit.limit_id].$error.required = true;
                this.angScopeAny.frmCgAdmin['txtLimitBottom_' + appLimit.limit_id].$setValidity("required", false);
                this.angScopeAny.frmCgAdmin['txtLimitTop_' + appLimit.limit_id].$setValidity("required", false);
                for (var i = 0; i < appLimit.manager_role.length; i++) {
                    appLimit.manager_role.splice(i, 1);
                }
                appLimit.is_app_man_selected = false;
            };
            CgAdminController.prototype.setAppLimitFirstLast = function () {
                this.editPurchaseGroup.purchase_group_limit[0].is_first = true;
                for (var i = 0; i < this.editPurchaseGroup.purchase_group_limit.length; i++) {
                    if (i < 5 && this.editPurchaseGroup.purchase_group_limit[i].is_visible &&
                        !(this.editPurchaseGroup.purchase_group_limit[i + 1].is_visible)) {
                        this.editPurchaseGroup.purchase_group_limit[i].is_last = true;
                    }
                    else {
                        this.editPurchaseGroup.purchase_group_limit[i].is_last = false;
                    }
                    if (i === 5 && this.editPurchaseGroup.purchase_group_limit[i].is_visible) {
                        this.editPurchaseGroup.purchase_group_limit[i].is_last = true;
                    }
                }
            };
            CgAdminController.prototype.updateAppMatrixUpdateLimit = function (modifPurchaseGroupLimit, origPurchaseGroupLimit) {
                //modifPurchaseGroupLimit.limit_bottom = this.convertTextToDecimal(origPurchaseGroupLimit.limit_bottom_text);
                //modifPurchaseGroupLimit.limit_top = this.convertTextToDecimal(origPurchaseGroupLimit.limit_top_text);
                origPurchaseGroupLimit.limit_bottom_text_ro = modifPurchaseGroupLimit.limit_bottom_text_ro;
                origPurchaseGroupLimit.limit_top_text_ro = modifPurchaseGroupLimit.limit_top_text_ro;
                origPurchaseGroupLimit.limit_bottom_text = modifPurchaseGroupLimit.limit_bottom_text;
                origPurchaseGroupLimit.limit_top_text = modifPurchaseGroupLimit.limit_top_text;
                origPurchaseGroupLimit.limit_bottom = modifPurchaseGroupLimit.limit_bottom;
                origPurchaseGroupLimit.limit_top = modifPurchaseGroupLimit.limit_top;
                origPurchaseGroupLimit.is_bottom_unlimited = modifPurchaseGroupLimit.is_bottom_unlimited;
                origPurchaseGroupLimit.is_top_unlimited = modifPurchaseGroupLimit.is_top_unlimited;
                origPurchaseGroupLimit.is_limit_bottom_multipl = modifPurchaseGroupLimit.is_limit_bottom_multipl;
                origPurchaseGroupLimit.is_limit_top_multipl = modifPurchaseGroupLimit.is_limit_top_multipl;
                origPurchaseGroupLimit.is_visible = modifPurchaseGroupLimit.is_visible;
                origPurchaseGroupLimit.is_first = modifPurchaseGroupLimit.is_first;
                origPurchaseGroupLimit.is_last = modifPurchaseGroupLimit.is_last;
                origPurchaseGroupLimit.is_app_man_selected = modifPurchaseGroupLimit.is_app_man_selected;
                this.updateAppMatrixDeleteAppMan(modifPurchaseGroupLimit, origPurchaseGroupLimit);
                this.updateAppMatrixNewAppMan(modifPurchaseGroupLimit, origPurchaseGroupLimit);
            };
            CgAdminController.prototype.updateAppMatrixDeleteAppMan = function (modifPurchaseGroupLimit, origPurchaseGroupLimit) {
                //Delete Manager Role
                for (var iManRoleIndex = origPurchaseGroupLimit.manager_role.length - 1; iManRoleIndex >= 0; iManRoleIndex--) {
                    var iParticipantId = origPurchaseGroupLimit.manager_role[iManRoleIndex].participant.id;
                    var managerRoleModif = this.$filter("filter")(modifPurchaseGroupLimit.manager_role, { participant_id: iParticipantId }, true);
                    if (this.isValueNullOrUndefined(managerRoleModif[0])) {
                        //Delete Manager
                        origPurchaseGroupLimit.manager_role.splice(iManRoleIndex, 1);
                    }
                }
            };
            CgAdminController.prototype.updateAppMatrixNewAppMan = function (modifPurchaseGroupLimit, origPurchaseGroupLimit) {
                //New Manager Role
                for (var iManRoleId = 0; iManRoleId < modifPurchaseGroupLimit.manager_role.length; iManRoleId++) {
                    var iParticipantId = modifPurchaseGroupLimit.manager_role[iManRoleId].participant_id;
                    var managerRoleOrig = this.$filter("filter")(origPurchaseGroupLimit.manager_role, { participant_id: iParticipantId });
                    if (this.isValueNullOrUndefined(managerRoleOrig[0])) {
                        //New Manager
                        var tmpManagerRole = new RegetApp.ManagerRole();
                        tmpManagerRole.participant_id = modifPurchaseGroupLimit.manager_role[iManRoleId].participant.id;
                        tmpManagerRole.approve_level_id = modifPurchaseGroupLimit.app_level_id;
                        var tmpParticipant = new RegetApp.Participant();
                        tmpParticipant.id = modifPurchaseGroupLimit.manager_role[iManRoleId].participant.id;
                        tmpParticipant.surname = modifPurchaseGroupLimit.manager_role[iManRoleId].participant.surname;
                        tmpParticipant.first_name = modifPurchaseGroupLimit.manager_role[iManRoleId].participant.first_name;
                        tmpParticipant.substituted_by = modifPurchaseGroupLimit.manager_role[iManRoleId].participant.substituted_by;
                        tmpParticipant.substituted_until = modifPurchaseGroupLimit.manager_role[iManRoleId].participant.substituted_until;
                        tmpManagerRole.participant = tmpParticipant;
                        origPurchaseGroupLimit.manager_role.push(tmpManagerRole);
                    }
                }
            };
            CgAdminController.prototype.moveLimitUp = function (limitId) {
                for (var i = 0; i < this.editPurchaseGroup.purchase_group_limit.length; i++) {
                    if (this.editPurchaseGroup.purchase_group_limit[i].limit_id === limitId) {
                        this.limitBottom = angular.copy(this.editPurchaseGroup.purchase_group_limit[i]);
                        this.limitTop = angular.copy(this.editPurchaseGroup.purchase_group_limit[i - 1]);
                        this.updateAppMatrixUpdateLimit(this.limitBottom, this.editPurchaseGroup.purchase_group_limit[i - 1]);
                        this.updateAppMatrixUpdateLimit(this.limitTop, this.editPurchaseGroup.purchase_group_limit[i]);
                        if (this.limitTop.is_first) {
                            this.editPurchaseGroup.purchase_group_limit[i - 1].is_first = true;
                            this.editPurchaseGroup.purchase_group_limit[i].is_first = false;
                        }
                        if (this.limitBottom.is_last) {
                            this.editPurchaseGroup.purchase_group_limit[i - 1].is_last = false;
                            this.editPurchaseGroup.purchase_group_limit[i].is_last = true;
                        }
                        break;
                    }
                }
            };
            CgAdminController.prototype.moveLimitDown = function (limitId) {
                for (var i = 0; i < this.editPurchaseGroup.purchase_group_limit.length; i++) {
                    if (this.editPurchaseGroup.purchase_group_limit[i].limit_id === limitId) {
                        this.limitTop = angular.copy(this.editPurchaseGroup.purchase_group_limit[i]);
                        this.limitBottom = angular.copy(this.editPurchaseGroup.purchase_group_limit[i + 1]);
                        this.updateAppMatrixUpdateLimit(this.limitBottom, this.editPurchaseGroup.purchase_group_limit[i]);
                        this.updateAppMatrixUpdateLimit(this.limitTop, this.editPurchaseGroup.purchase_group_limit[i + 1]);
                        if (this.limitTop.is_first) {
                            this.editPurchaseGroup.purchase_group_limit[i].is_first = true;
                            this.editPurchaseGroup.purchase_group_limit[i + 1].is_first = false;
                        }
                        if (this.limitBottom.is_last) {
                            this.editPurchaseGroup.purchase_group_limit[i].is_last = false;
                            this.editPurchaseGroup.purchase_group_limit[i + 1].is_last = true;
                        }
                        break;
                    }
                }
            };
            CgAdminController.prototype.deleteAppManager = function (limitid, sindex) {
                var limit = this.$filter("filter")(this.editPurchaseGroup.purchase_group_limit, { limit_id: limitid }, true);
                limit[0].manager_role.splice(sindex, 1);
                if (limit[0].manager_role.length === 0) {
                    limit[0].is_app_man_selected = false;
                }
            };
            CgAdminController.prototype.appManSelectedItemChange = function (item, limitItemId) {
                if (this.isValueNullOrUndefined(item)) {
                    return;
                }
                var limit = this.$filter("filter")(this.editPurchaseGroup.purchase_group_limit, { limit_id: limitItemId }, true);
                var isAdd = true;
                for (var i = 0; i < limit[0].manager_role.length; i++) {
                    if (limit[0].manager_role[i].participant.id === item.id) {
                        isAdd = false;
                        break;
                    }
                }
                if (isAdd) {
                    var tmpManagerRole = new RegetApp.ManagerRole();
                    //tmpManagerRole.purchase_group_limit_id: limitItemId;
                    tmpManagerRole.participant_id = item.id;
                    tmpManagerRole.approve_level_id = limit[0].app_level_id;
                    var tmpParticipant = new RegetApp.Participant();
                    tmpParticipant.id = item.id;
                    tmpParticipant.surname = item.surname;
                    tmpParticipant.first_name = item.first_name;
                    tmpParticipant.substituted_by = item.substituted_by;
                    tmpParticipant.substituted_until = item.substituted_until;
                    tmpManagerRole.participant = tmpParticipant;
                    limit[0].manager_role.push(tmpManagerRole);
                }
                limit[0].is_app_man_selected = true;
                this.hideAppManAutoCompl(limitItemId);
            };
            CgAdminController.prototype.hideAppManAutoCompl = function (limitId) {
                var strAutocomplId = (this.isSmartPhone()) ? "#appManAutoComplMobile_" : "#appManAutoCompl_";
                strAutocomplId += limitId;
                this.searchstringappman = null;
                this.selectedAppMan = null;
                var $autWrap = $(strAutocomplId).children().first();
                var $autChild = $autWrap.children().first();
                $autChild.val('');
                //$("#appManAutoCompl_" + limitId).children().each(function () {
                //    var $currentElement = $(this);
                //    //////////// Show element
                //    console.info($currentElement);
                //    //////////// Show events handlers of current element
                //    console.info($currentElement.data('events'));
                //    //////////// Loop her children
                //    recursiveEach($currentElement);
                //});
                //$("#appManAutoCompl_" + limitId).val(null);
                $(strAutocomplId).hide();
            };
            CgAdminController.prototype.toggleAppLimit = function (limitId) {
                var divLimitContent = 'divLimitContent_' + limitId;
                var imgLimitCollExp = 'imgLimitCollExp_' + limitId;
                if ($("#" + divLimitContent).is(':visible')) {
                    $("#" + divLimitContent).slideUp();
                    $("#" + imgLimitCollExp).attr("src", this.getRegetRootUrl() + "Content/Images/Panel/Expand.png");
                    $("#" + imgLimitCollExp).attr("title", this.displayText);
                }
                else {
                    $("#" + divLimitContent).show("slow");
                    $("#" + imgLimitCollExp).attr("src", this.getRegetRootUrl() + "Content/Images/Panel/Collapse.png");
                    $("#" + imgLimitCollExp).attr("title", this.hideText);
                }
            };
            CgAdminController.prototype.addAppLimit = function (limitId) {
                for (var iLimitId = 0; iLimitId < this.editPurchaseGroup.purchase_group_limit.length; iLimitId++) {
                    if (!(this.editPurchaseGroup.purchase_group_limit[iLimitId].is_visible)) {
                        this.clearAppLimit(this.editPurchaseGroup.purchase_group_limit[iLimitId]);
                        this.editPurchaseGroup.purchase_group_limit[iLimitId].is_visible = true;
                        this.editPurchaseGroup.purchase_group_limit[iLimitId].is_last = true;
                        if (iLimitId > 0) {
                            this.editPurchaseGroup.purchase_group_limit[iLimitId - 1].is_last = false;
                        }
                        else {
                            this.editPurchaseGroup.purchase_group_limit[iLimitId].is_first = true;
                        }
                        if (iLimitId === 5) {
                            $("#btnAddLimit").hide();
                        }
                        if (this.isSmartPhone()) {
                            this.showAppLimit(this.editPurchaseGroup.purchase_group_limit[iLimitId].limit_id);
                        }
                        break;
                    }
                }
            };
            CgAdminController.prototype.showAppLimit = function (limitId) {
                var divLimitContent = 'divLimitContent_' + limitId;
                var imgLimitCollExp = 'imgLimitCollExp_' + limitId;
                $("#" + divLimitContent).show("slow");
                $("#" + imgLimitCollExp).attr("src", this.getRegetRootUrl() + "Content/Images/Panel/Collapse.png");
                $("#" + imgLimitCollExp).attr("title", this.hideText);
            };
            CgAdminController.prototype.displayRequestorDetail = function (userId, centreId) {
                this.displayElement("btnCollapseReq_" + userId + "_" + centreId);
                this.hideElement("btnExpandReq_" + userId + "_" + centreId);
                this.displayElementSlow("divReq_" + userId + "_" + centreId);
            };
            CgAdminController.prototype.hideRequestorDetail = function (userId, centreId) {
                this.hideElement("btnCollapseReq_" + userId + "_" + centreId);
                this.displayElement("btnExpandReq_" + userId + "_" + centreId);
                this.hideElementSlow("divReq_" + userId + "_" + centreId);
            };
            CgAdminController.prototype.displayOrdererDetail = function (userId) {
                this.displayElement("btnCollapseOrd_" + userId);
                this.hideElement("btnExpandOrd_" + userId);
                this.displayElementSlow("divOrd_" + userId);
            };
            CgAdminController.prototype.hideOrdererDetail = function (userId) {
                this.hideElement("btnCollapseOrd_" + userId);
                this.displayElement("btnExpandOrd_" + userId);
                this.hideElementSlow("divOrd_" + userId);
            };
            CgAdminController.prototype.checkAllRequestorToggle = function (iRequestorId, iCentreId) {
                var requestor = this.$filter("filter")(this.editPurchaseGroup.requestor, { participant_id: iRequestorId, centre_id: iCentreId }, true);
                if (!this.isValueNullOrUndefined(requestor[0])) {
                    if (requestor[0].is_all) {
                        requestor[0].is_all = false;
                    }
                    else {
                        requestor[0].is_all = true;
                    }
                }
            };
            CgAdminController.prototype.checkAllOrdererToggle = function (iOrdererId) {
                var orderer = this.$filter("filter")(this.editPurchaseGroup.orderer, { participant_id: iOrdererId }, true);
                if (!this.isValueNullOrUndefined(orderer[0])) {
                    if (orderer[0].is_all) {
                        orderer[0].is_all = false;
                    }
                    else {
                        orderer[0].is_all = true;
                    }
                }
            };
            CgAdminController.prototype.requestorSelectedItemChange = function (item, centreId) {
                if (this.isValueNullOrUndefined(item)) {
                    return;
                }
                var requestor = this.$filter("filter")(this.editPurchaseGroup.requestor, { participant_id: item.id, centre_id: centreId }, true);
                if (this.isValueNullOrUndefined(requestor[0])) {
                    var tmpRequestor = new RegetApp.Requestor();
                    tmpRequestor.participant_id = item.id;
                    tmpRequestor.surname = item.surname;
                    tmpRequestor.first_name = item.first_name;
                    tmpRequestor.centre_id = centreId;
                    tmpRequestor.is_all = false;
                    this.editPurchaseGroup.requestor.push(tmpRequestor);
                }
                this.hideRequestorAutoCompl(centreId);
            };
            CgAdminController.prototype.checkSelfOrdered = function () {
                this.editPurchaseGroup.orderer = [];
                this.hideOrdererAutoCompl();
            };
            CgAdminController.prototype.ordererSelectedItemChange = function (item) {
                if (this.isValueNullOrUndefined(item)) {
                    return;
                }
                var orderer = this.$filter("filter")(this.editPurchaseGroup.orderer, { participant_id: item.id }, true);
                if (this.isValueNullOrUndefined(orderer[0])) {
                    var tmpOrderer = new RegetApp.Requestor();
                    tmpOrderer.participant_id = item.id;
                    tmpOrderer.surname = item.surname;
                    tmpOrderer.first_name = item.first_name;
                    tmpOrderer.substituted_by = item.substituted_by;
                    tmpOrderer.substituted_until = item.substituted_until;
                    this.editPurchaseGroup.orderer.push(tmpOrderer);
                }
                this.hideOrdererAutoCompl();
            };
            CgAdminController.prototype.defaultSupplierSelectedItemChange = function (item) {
                if (!this.isValueNullOrUndefined(this.editPurchaseGroup)) {
                    this.editPurchaseGroup.default_supplier = item;
                }
            };
            CgAdminController.prototype.updateLocalTexts = function (editPurchaseGroup, origPurchaseGroup) {
                for (var i = 0; i < editPurchaseGroup.local_text.length; i++) {
                    origPurchaseGroup.local_text[i].text = editPurchaseGroup.local_text[i].text;
                }
            };
            CgAdminController.prototype.updateAppMatrix = function () {
                for (var iLimitId = 0; iLimitId < this.editPurchaseGroup.purchase_group_limit.length; iLimitId++) {
                    var purchaseGroupLimitOrig = this.$filter("filter")(this.purchaseGroupOrig.purchase_group_limit, { limit_id: this.editPurchaseGroup.purchase_group_limit[iLimitId].limit_id }, true);
                    this.updateAppMatrixUpdateLimit(this.editPurchaseGroup.purchase_group_limit[iLimitId], purchaseGroupLimitOrig[0]);
                }
            };
            CgAdminController.prototype.updateRequestors = function () {
                this.deleteRequestorsOrderers(this.editPurchaseGroup, this.purchaseGroupOrig.requestor, this.editPurchaseGroup.requestor, this.deleteAllRequestorList, true);
                this.newRequestorsOrderers(this.purchaseGroupOrig.requestor, this.editPurchaseGroup.requestor, true);
                this.addAllRequestors();
            };
            CgAdminController.prototype.updateOrderers = function () {
                this.deleteRequestorsOrderers(this.editPurchaseGroup, this.purchaseGroupOrig.orderer, this.editPurchaseGroup.orderer, this.deleteAllOrdererList, false);
                this.newRequestorsOrderers(this.purchaseGroupOrig.orderer, this.editPurchaseGroup.orderer, false);
            };
            CgAdminController.prototype.deleteRequestorsOrderers = function (editPurchaseGroup, origRequestorsOrderers, modifRequestorOrderers, deleteAllRequestorOrderers, isRequestor) {
                //Delete requestors
                for (var i = origRequestorsOrderers.length - 1; i >= 0; i--) {
                    var modifParticipant = null;
                    if (isRequestor) {
                        modifParticipant = this.$filter("filter")(modifRequestorOrderers, { participant_id: origRequestorsOrderers[i].participant_id, centre_id: origRequestorsOrderers[i].centre_id }, true);
                    }
                    else {
                        modifParticipant = this.$filter("filter")(modifRequestorOrderers, { participant_id: origRequestorsOrderers[i].participant_id }, true);
                    }
                    if (this.isValueNullOrUndefined(modifParticipant[0])) {
                        //Delete Requestor
                        origRequestorsOrderers.splice(i, 1);
                    }
                }
                if (!this.isValueNullOrUndefined(deleteAllRequestorOrderers)) {
                    for (var j = 0; j < deleteAllRequestorOrderers.length; j++) {
                        for (var i = 0; i < this.purchaseGroupList.length; i++) {
                            if (isRequestor === true) {
                                var delParticipant = this.$filter("filter")(this.purchaseGroupList[i].requestor, { participant_id: deleteAllRequestorOrderers[j].participant_id, centre_id: deleteAllRequestorOrderers[j].centre_id }, true);
                                if (!this.isValueNullOrUndefined(delParticipant[0])) {
                                    var index = this.purchaseGroupList[i].requestor.indexOf(delParticipant[0]);
                                    this.purchaseGroupList[i].requestor.splice(index, 1);
                                }
                            }
                            else {
                                var delParticipant = this.$filter("filter")(this.purchaseGroupList[i].orderer, { participant_id: deleteAllRequestorOrderers[j].participant_id }, true);
                                if (!this.isValueNullOrUndefined(delParticipant[0])) {
                                    var index = this.purchaseGroupList[i].orderer.indexOf(delParticipant[0]);
                                    this.purchaseGroupList[i].orderer.splice(index, 1);
                                }
                            }
                        }
                        if (isRequestor) {
                            editPurchaseGroup.delete_requestors_all_categories.push({ participant_id: deleteAllRequestorOrderers[j].participant_id, centre_id: deleteAllRequestorOrderers[j].centre_id });
                        }
                        else {
                            editPurchaseGroup.delete_orderers_all_categories.push(deleteAllRequestorOrderers[j].participant_id);
                        }
                    }
                    deleteAllRequestorOrderers = null;
                }
            };
            CgAdminController.prototype.newRequestorsOrderers = function (origRequestorsOrderers, modifRequestorOrderers, isRequestor) {
                //New Requestors
                for (var i = 0; i < modifRequestorOrderers.length; i++) {
                    var origParticipant = null;
                    if (isRequestor) {
                        origParticipant = this.$filter("filter")(origRequestorsOrderers, { participant_id: modifRequestorOrderers[i].participant_id, centre_id: modifRequestorOrderers[i].centre_id }, true);
                    }
                    else {
                        origParticipant = this.$filter("filter")(origRequestorsOrderers, { participant_id: modifRequestorOrderers[i].participant_id }, true);
                    }
                    if (this.isValueNullOrUndefined(origParticipant[0])) {
                        //new
                        var tmpRequestorOrderer = new RegetApp.RequestorOrderer();
                        tmpRequestorOrderer.participant_id = modifRequestorOrderers[i].participant_id;
                        tmpRequestorOrderer.surname = modifRequestorOrderers[i].surname;
                        tmpRequestorOrderer.first_name = modifRequestorOrderers[i].first_name;
                        tmpRequestorOrderer.centre_id = modifRequestorOrderers[i].centre_id;
                        if (isRequestor) {
                            origRequestorsOrderers.push(tmpRequestorOrderer);
                        }
                        else {
                            origRequestorsOrderers.push(tmpRequestorOrderer);
                        }
                        if (modifRequestorOrderers[i].is_all) {
                            for (var j = 0; j < this.purchaseGroupList.length; j++) {
                                if (this.editPurchaseGroup.id === this.purchaseGroupList[j].id) {
                                    continue;
                                }
                                if (isRequestor) {
                                    var modifParticipant = this.$filter("filter")(this.purchaseGroupList[j].requestor, { participant_id: modifRequestorOrderers[i].participant_id }, true);
                                    if (this.isValueNullOrUndefined(modifParticipant[0])) {
                                        var tmpRequestor = new RegetApp.Requestor();
                                        tmpRequestor.participant_id = modifRequestorOrderers[i].participant_id;
                                        tmpRequestor.surname = modifRequestorOrderers[i].surname;
                                        tmpRequestor.first_name = modifRequestorOrderers[i].first_name;
                                        tmpRequestor.centre_id = modifRequestorOrderers[i].centre_id;
                                        this.purchaseGroupList[j].requestor.push(tmpRequestor);
                                    }
                                }
                                else {
                                    var modifParticipant = this.$filter("filter")(this.purchaseGroupList[j].orderer, { participant_id: modifRequestorOrderers[i].participant_id }, true);
                                    if (this.isValueNullOrUndefined(modifParticipant[0])) {
                                        var tmpOrderer = new RegetApp.Orderer();
                                        tmpOrderer.participant_id = modifRequestorOrderers[i].participant_id;
                                        tmpOrderer.surname = modifRequestorOrderers[i].surname;
                                        tmpOrderer.substituted_by = modifRequestorOrderers[i].substituted_by;
                                        tmpOrderer.substituted_until = modifRequestorOrderers[i].substituted_until;
                                        this.purchaseGroupList[j].orderer.push(tmpOrderer);
                                    }
                                }
                            }
                        }
                    }
                }
            };
            CgAdminController.prototype.addAllRequestors = function () {
                var allParticipants = this.$filter("filter")(this.editPurchaseGroup.requestor, { is_all: true }, true);
                for (var i = 0; i < allParticipants.length; i++) {
                    for (var j = 0; j < this.purchaseGroupList.length; j++) {
                        var origParticipant = this.$filter("filter")(this.purchaseGroupList[j].requestor, { participant_id: allParticipants[i].participant_id, centre_id: allParticipants[i].centre_id }, true);
                        if (this.isValueNullOrUndefined(origParticipant[0])) {
                            var tmpRequestor = new RegetApp.Requestor();
                            tmpRequestor.participant_id = allParticipants[i].participant_id;
                            tmpRequestor.first_name = allParticipants[i].first_name;
                            tmpRequestor.surname = allParticipants[i].surname;
                            tmpRequestor.centre_id = allParticipants[i].centre_id;
                            this.purchaseGroupList[j].requestor.push(tmpRequestor);
                        }
                    }
                }
            };
            CgAdminController.prototype.getPurchaseGroupHeaderBkgColor = function (pg) {
                //ng - class="pgItem.is_active ? 'reget-box-header reget-box-header-level1' : 'reget-box-header reget-box-header-level1-disabled'"
                if (pg.is_active === false) {
                    return "reget-box-header reget-box-header-level1-disabled";
                }
                //if (pg.is_order_needed === false) {
                //    return "reget-box-header reget-box-header-level1-approvalonly"
                //}
                return "reget-box-header reget-box-header-level1";
            };
            return CgAdminController;
        }(RegetApp.BaseRegetTs));
        RegetApp.CgAdminController = CgAdminController;
        angular.
            module('RegetApp').
            controller('CgAdminController', Kamsyk.RegetApp.CgAdminController);
    })(RegetApp = Kamsyk.RegetApp || (Kamsyk.RegetApp = {}));
})(Kamsyk || (Kamsyk = {}));
//# sourceMappingURL=cg-admin.js.map