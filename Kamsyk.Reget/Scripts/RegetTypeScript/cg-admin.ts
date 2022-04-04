/// <reference path="../RegetTypeScript/Base/reget-base.ts" />
/// <reference path="../RegetTypeScript/Base/reget-common.ts" />
/// <reference path="../RegetTypeScript/Base/reget-entity.ts" />

module Kamsyk.RegetApp {
    angular.module('RegetApp').directive("appMatrix", ($timeout : any) => {
        return {
            templateUrl: RegetCommonTs.getRegetRootUrl() + 'Content/Html/AngAppMatrix.html',
            link: function postLink(elem, attrs, transclude) {
                $timeout(() => {
                    // This code will run after
                    // templateUrl has been loaded, cloned
                    // and transformed by directives.
                    //$scope.HideAddLimitButton();
                    if ($("#trLimit_5").is(':visible')) {
                        $("#btnAddLimit").hide();
                    } else {
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


    export class CgAdminController extends BaseRegetTs implements angular.IController {

        //**************************************************************************
        //Constants
        //**************************************************************************

        //**************************************************************************
        //Properties
        private isNewCg: boolean = false;
        private rootUrl: string;
        private isCentresLoaded: boolean = false;
        private isCgPurchaseGroupsLoaded: boolean = false;
        private isCurrenciesLoaded: boolean = false;
        //private isParticipantLoaded: boolean = false;
        private isOfficeLoaded: boolean = false;
        private selectedCentreGroupId: number = null;
        //private participantId: number = null;
        private participantId:number = $("#ParticipantId").val();
        private errMsg: string;
        //private participants: Participant[] = null;
        private offices: Company[] = null;
        private loadTextBkp: string = null;
        private editPurchaseGroup: PurchaseGroup = null;
        private purchaseGroupOrig: PurchaseGroup = null;
        private isDeactivatedPgLoaded: boolean = false;
        private isDeactivatedPgDisplayed: boolean = false;
        private centreGroup: CentreGroup = null;
        private selectedOfficeId: number = null;

        private searchstringdefaultsupplier: string = null;
        private searchstringdeputyorderer: string = null;
        private searchstringcentre: string = null;
        private selectedDefaultSupplier: any = null;
        private selectedOfficeName: string = null;
        private selectedSearchCentre: Centre = null;
        private selectedOrdererSupplierOrderer: Participant = null;
        private selectedOrdererSupplierSupplier: Supplier = null;
        private searchstringorderersupplierorderer: string = null;
        private searchstringorderersuppliersupplier: string = null;
        private searchstringcgadmin: string = null;
        private searchstringrequestor:string = null;
        private selectedRequestor: Requestor = null;
        private searchstringappman: string = null;
        private selectedAppMan: Participant = null;
        private searchstringorderer: string = null;
        private selectedOrderer: Participant = null;
        private searchstringnotassignedcentre: string = null;
        private selectedNotAssignedCentre: Centre = null;
        private selectedDeputyOrderer: Orderer = null;
        private selectedCgAdmin: CgAdmin = null;
        
        private isDeactivatedCgDisplayed: boolean = false;
        private centreGroupList: CentreGroup[] = null;
        private strInitCgId: string = null;
        private purchaseGroupList: PurchaseGroup[] = null;
        private pgItemCount: number = null;
        private isDeactivatedPgLoading: boolean = false;
        private currenciesList: Currency[] = null;
        private selectedCurrencyId: number = null;
        private isCgPropertyAdmin: boolean = false;
        private isCgAppMatrixAdmin: boolean = false;
        private isCgOrdererAdmin: boolean = false;
        private isCgRequestorAdmin: boolean = false;
        private isReadOnly: boolean = false;
        private adminRoles: UserRoles = null;
        private purchaseGroupCount: number = null;
        private selectedCurrencyCode: string = null;
        private selectedCurrencyCodeName: string = null;
        private isAllCgReadOnly: boolean = false;
        private notAssignedCentresList: Centre[] = null;
        private centres: Centre[] = null;
        private deleteAllRequestorList: Requestor[] = null;
        private deleteAllOrdererList: Orderer[] = null;
        private suppliers: Supplier[] = null;
        private cgPropErrMsg: string = null;
        private isCancelEvent: boolean = false;
        private pgExpandImgs: HTMLCollectionOf<HTMLImageElement> = null;
        private saveErrMsg: string = null;
        private isErrorAppMatrix: boolean = false;
        private parentPurchaseGroups: ParentPurchaseGroup[] = null;
        //private frmCgAdmin: any = null;
        private limitBottom: PurchaseGroupLimit = null;
        private limitTop: PurchaseGroupLimit = null;
        private isCgReadOnly: boolean = false;
        //**************************************************************************

        //***************************************************************************
        //Localization
        private loadingPurchaseGroupText = $("#LoadingPurchaseGroupText").val();
        private cancelSelectText: string =$("#CancelSelectText").val();
        private loadDataErrorText: string =$("#LoadDataErrorText").val();
        private loadingDataText: string =$("#LoadingDataText").val();
        private warningText: string =$("#WarningText").val();
        private closeText: string =$("#CloseText").val();
        private cannotDeleteCompanyAdminText: string =$("#CannotDeleteCompanyAdminText").val();
        private moveCentreMsgText: string =$("#MoveCentreMsgText").val();
        private moveCentreHeaderText: string =$("#MoveCentreHeaderText").val();
        private cancelText: string =$("#BackText").val();
        private moveText: string =$("#MoveText").val();
        private lowerLimitText: string =$("#LowerLimitText").val();
        private upperLimitText: string =$("#UpperLimitText").val();
        private approveManText: string =$("#ApproveManText").val();
        private mandatoryText: string = $("#MandatoryTextFieldText").val();
        private mandatoryDecimal: string = $("#EnterDecimalNumberText").val();
        private noLimitText: string =$("#NoLimitText").val();
        private multiplText: string =$("#MultiplText").val();
        private addText: string =$("#AddText").val();
        private removeText: string =$("#DeleteText").val();
        private upText: string =$("#UpText").val();
        private downText: string =$("#DownText").val();
        private selectAppManUserText: string =$("#WhoIsAppManText").val();
        private notFoundText: string =$("#NotFoundText").val();
        private saveText: string =$("#SaveText").val();
        private dataWasSavedText: string =$("#DataWasSavedText").val();
        private displayDetailText: string =$("#DisplayDetailsText").val();
        private hideDetailText: string =$("#HideDetailsText").val();
        private purchaseGroupNameText: string =$("#NameText").val();
        private parentPgNameText: string =$("#ParentPgNameText").val();
        private defaultRequestorText: string =$("#DefaultRequestorText").val();
        private allRequestorText: string =$("#AllRequestorText").val();
        private allOrdererText: string =$("#AllOrdererText").val();
        private defaultOrdererText: string =$("#DefaultOrdererText").val();
        private deleteAllRequestorTitleText: string =$("#DeleteAllRequestorTitleText").val();
        private deleteAllRequestorMsgText: string =$("#DeleteAllRequestorMsgText").val();
        private selectRequestorText: string =$("#SelectRequestoText").val();
        private deleteAllOrdererTitleText: string =$("#DeleteAllOrdererTitleText").val();
        private deleteAllOrdererMsgText: string =$("#DeleteAllOrdererMsgText").val();
        private selectOrdererText: string =$("#SelectOrdererPlaceholderText").val();
        private saveErrorText: string =$("#SaveErrorText").val();
        private requestorsText: string =$("#RequestorsText").val();
        private orderersText: string =$("#OrderersText").val();
        private defaultSupplierText: string =$("#DefaultSupplierText").val();
        private whoWillBeDefaultSupplierText: string =$("#WhoIsDefaultSupplierText").val();
        private translateText: string =$("#TranslationText").val();
        private cgDeleteConfirmationText: string =$("#CgDeleteConfirmationText").val();
        private cgDeleteConfirmationHeaderText: string =$("#CgDeleteConfirmationHeaderText").val();
        private appMatrixAdminText: string =$("#AppMatrixAdminText").val();
        private requestorAdminText: string =$("#RequestorAdminText").val();
        private ordererAdminText: string =$("#OrdererAdminText").val();
        private centreGroupPropertyAdminText: string =$("#CentreGroupPropertyAdminText").val();
        private managerSubstitutionText: string =$("#ManagerSubstitutionText").val();
        private displayText: string =$("#DisplayText").val();
        private hideText: string =$("#HideText").val();
        private collapseAllText: string =$("#CollapseAllText").val();
        private expandAllText: string =$("#ExpandAllText").val();
        //private approvalOnlyText: string =$("#ApprovalOnlyText").val();
        private orderAllowedText: string = $("#OrderAllowedText").val();
        private approvalNeededText: string = $("#ApprovalNeededText").val();
        private selfOrderedText: string =$("#SelfOrderedText").val();
        private deleteText: string = $("#DeleteText").val();
        private backText: string = $("#BackText").val();
        private purchaseGroupDisabledText: string = $("#PurchaseGroupDisabledText").val();
        private centreGroupDisabledText: string = $("#CentreGroupDisabledText").val();
        private purchaseGroupDeletedText: string = $("#PurchaseGroupDeletedText").val();
        private centreGroupDeletedText: string = $("#CentreGroupDeletedText").val();
        private notificationText: string = $("#NotificationText").val();
        private activeText: string = $("#ActiveText").val();
        private aprovalLevelText: string = $("#AprovalLevelText").val();
        private purchaseGroupEditingText: string = $("#PurchaseGroupEditingText").val();
        private fromText: string = $("#FromText").val();
        private toText: string = $("#ToText").val();
        private enterText: string = $("#EnterText").val();
        private nameText: string = $("#NameText").val();
        private companyText: string = $("#CompanyText").val();
        private currencyText: string = $("#CurrencyText").val();
        private loadingPurchaseGroupsText: string = $("#LoadingPurchaseGroupText").val();

        private decimalSeparator = $("#DecimalSeparator").val();
        //**************************************************************************
                
        //**********************************************************
        //Constructor
        constructor(
            protected $scope: ng.IScope,
            protected $http: ng.IHttpService,
            protected $filter: ng.IFilterService,
            protected $mdDialog: angular.material.IDialogService,
            protected $mdToast: angular.material.IToastService,
            protected $q: ng.IQService,
            protected $timeout: ng.ITimeoutService,
            protected $compile: ng.ICompileService
        ) {
            super($scope, $http, $filter, $mdDialog, $mdToast, $q, $timeout);
                        
            this.loadInit();
        }
        //***************************************************************

        $onInit = () => { };

        //***************************************************************
        //Methods
        private loadInit(): void {
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
                } else {
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
                } else {
                    $("#imgCgExpand").attr("src", this.getRegetRootUrl() + "Content/Images/Panel/Collapse.png");
                    $("#imgCgExpand").attr("title", $("#HideText").val());

                    $("#imgCgMatrixExpand").attr("src", this.getRegetRootUrl() + "Content/Images/Panel/Collapse.png");
                    $("#imgCgMatrixExpand").attr("title", $("#HideText").val());
                }

                //$(".reget-appmatrix-expcolimg").attr("src", this.getRegetRootUrl() + "Content/Images/Panel/Expand.png");

                if (this.isNewCg !== true) {
                    var urlParams: UrlParam[] = this.getAllUrlParams();
                    if (!this.isValueNullOrUndefined(urlParams)) {
                        for (var i: number = 0; i < urlParams.length; i++) {
                            if (urlParams[i].param_name.toLowerCase() === "cgid") {
                                this.selectedCentreGroupId = parseInt(urlParams[i].param_value);
                                this.getCentreGroupData();
                                break;
                            }
                        }
                    }
                }
                
            } catch (ex) {
                this.displayErrorMsg();
            }
        }
                
        private getCentreGroup(jsonData: any): CentreGroup {
            var centreGroup: CentreGroup = jsonData;

            return centreGroup;
        }

        private getCentreGroupList(jsonData: any): CentreGroup[] {
            var tmpCgList: CentreGroup[] = jsonData;

            //for (var i = 0; i < jsonData.length; i++) {
            //    var cg: CentreGroup = jsonData[i];
            //    tmpCgList.push(cg);
            //}

            return tmpCgList;
        }

        private getParticipants(jsonData: any): Participant[] {
            var tmpParticipants: Participant[] = jsonData;

            return tmpParticipants;
        }

        private getCompanies(jsonData: any): Company[] {
            var tmpCompanies: Company[] = jsonData;

            return tmpCompanies;
        }

        private getPurchaseGroupList(jsonData: any): PurchaseGroup[] {
            var tmpPgList: PurchaseGroup[] = jsonData;

            return tmpPgList;
        }

        private getCentresList(jsonData: any): Centre[] {
            var tmpCentreList: Centre[] = jsonData;

            return tmpCentreList;
        }

        private getCurrenciesList(jsonData: any): Currency[] {
            var tmpCurrList: Currency[] = jsonData;

            return tmpCurrList;
        }

        private getCentreList(jsonData: any): Centre[] {
            var tmpCentreList: Centre[] = jsonData;
            
            return tmpCentreList;
        }

        private getUserRoles(jsonData: any): UserRoles {
            var tmpRoles: UserRoles = jsonData;

            return tmpRoles;
        }

        private getCentreGroupData(): void {
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
        }

        //********************************************************************
        //Http Get, Post
        private populateCentreGroupData(): void {
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

            this.$http.get(
                this.getRegetRootUrl() + '/RegetAdmin/GetCentreGroupDataById?cgId=' + this.selectedCentreGroupId + '&t=' + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    this.centreGroup = this.getCentreGroup(response.data);
                    this.selectedOfficeId = this.centreGroup.company_id;

                    this.populateCurrencies(this.selectedCentreGroupId, this.centreGroup.currency_id, this.selectedOfficeId);
                    this.selectedOfficeName = this.centreGroup.company_name;

                    this.populateCgCentres();

                    $("#divCgDetailPanel").show();
                    if (!this.isNewCg) {
                        $("#divCgAppMatrixPanel").show();
                    }
                    $("#btnRefresh").show();
                } catch (e) {
                    this.hideLoader();
                    this.displayErrorMsg();
                } finally {
                    this.hideLoaderAfterAllIsLoaded();
                }
            }, (response: any) => {
                this.hideLoader();
                this.displayErrorMsg();
                
            });
            
        }
              
        
        private getOfficeData(): void {

            if (this.isValueNullOrUndefined(this.offices)) {

                this.$http.get(
                    this.getRegetRootUrl() + '/RegetAdmin/GetActiveOfficeData?t=' + new Date().getTime(),
                    {}
                ).then((response) => {
                    try {
                        this.offices = this.getCompanies(response.data);
                        if (!this.isValueNullOrUndefined(this.centreGroup)) {
                            this.selectedOfficeId = this.centreGroup.company_id;
                        }
                       
                        this.isOfficeLoaded = true;
                        this.getIsCentreGroupReadOnly();
                    } catch (e) {
                        this.hideLoader();
                        this.displayErrorMsg();
                    } finally {
                        this.hideLoaderAfterAllIsLoaded();
                    }
                }, (response: any) => {
                    this.hideLoader();
                    this.displayErrorMsg();
                    
                });
                                
            } else {
                this.isOfficeLoaded = true;
            }
        }

        private populateCentreGroups(isStandAlone?: boolean): void {
            if (isStandAlone === true) {
                this.showLoaderBoxOnly();
            } else {
                this.showLoader(this.isError);
            }

            this.$http.get(
                this.getRegetRootUrl() + 'RegetAdmin/GetCentreGroups?isDeactivatedLoaded='
                + this.isDeactivatedCgDisplayed + '&t=' + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    this.centreGroupList = this.getCentreGroupList(response.data);

                    if (!this.isStringValueNullOrEmpty(this.strInitCgId)) {
                        var iInitCgId = parseInt(this.strInitCgId);
                        this.selectedCentreGroupId = iInitCgId;
                        this.getCentreGroupData();
                    }
                    
                } catch (e) {
                    this.hideLoader();
                    this.displayErrorMsg();
                } finally {
                    if (isStandAlone === true) {
                        this.hideLoader();
                    } else {
                        this.hideLoaderAfterAllIsLoaded();
                    }
                }
            }, (response: any) => {
                this.hideLoader();
                this.displayErrorMsg();
                
            });
            
        }

        private populatePurchaseGroups(isDisabledLoaded : boolean) : void {
            this.showLoader(this.isError);

            this.$http.get(
                this.getRegetRootUrl() + '/RegetAdmin/GetPurchaseGroupsByCgId?cgId=' + this.selectedCentreGroupId
                + '&indexFrom=-1&isDeativatedLoaded=' + isDisabledLoaded + '&t=' + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    let tmpData: any = response.data;
                    this.purchaseGroupList = tmpData;//this.getPurchaseGroupList(response.data);

                    this.isCgPurchaseGroupsLoaded = true;
                    
                    if (!this.isValueNullOrUndefined(this.purchaseGroupList) && this.purchaseGroupList.length > 25) {
                        //*** do not delete ****************
                        this.pgItemCount = 25;
                        this.showLoader(this.isError);
                        this.getPurchaseGroupsCount();
                        //************************************
                    } else {
                        if (this.isDeactivatedPgLoading) {
                            this.isDeactivatedPgLoading = false;
                            this.isDeactivatedPgLoaded = true;
                            this.displayHideDisablePgs();
                        }
                    }
                } catch (e) {
                    this.hideLoader();
                    this.displayErrorMsg();
                } finally {
                    this.hideLoaderAfterAllIsLoaded();
                }
            }, (response: any) => {
                this.hideLoader();
                this.displayErrorMsg();
            });
        }

        private populateCurrencies(cgId: number, currencyId: number, officeId: number) : void {
            this.showLoader(this.isError);

            this.$http.get(
                this.getRegetRootUrl() + '/RegetAdmin/GetCurrencies?cgId=' + cgId +
                '&currentCurrencyId=' + currencyId +
                '&companyId=' + officeId +
                '&t=' + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    this.currenciesList = this.getCurrenciesList(response.data);
                    this.isCurrenciesLoaded = true;
                    if (!this.isValueNullOrUndefined(this.centreGroup)) {
                        var cgCurr = this.$filter("filter")(this.currenciesList, { id: this.centreGroup.currency_id }, true);
                        if (this.isValueNullOrUndefined(cgCurr) || cgCurr.length === 0) {
                            this.centreGroup.currency_id = null;
                        }

                        if (this.isValueNullOrUndefined(this.centreGroup.currency_id)) {
                            var cgOffice = this.$filter("filter")(this.offices, { company_id: this.centreGroup.company_id }, true);
                            if (!this.isValueNullOrUndefined(cgOffice) && cgOffice.length > 0) {
                                this.centreGroup.currency_id = cgOffice[0].default_currency_id;
                            }
                        }

                        this.selectedCurrencyId = this.centreGroup.currency_id;
                        this.setSelectedCurrencyCode();
                        this.getIsCentreGroupReadOnly();
                    }

                    //foreign currencies
                    var tmpForeignCurrencies : Currency[] = angular.copy(this.currenciesList);

                    this.centreGroup.currencies = [];
                    if (!this.isValueNullOrUndefined(this.currenciesList)) {
                        for (var i = 0; i < this.currenciesList.length; i++) {
                            var fCurr = this.$filter("filter")(tmpForeignCurrencies, { id: this.currenciesList[i].id }, true);
                            var isSet = false;
                            if (!this.isValueNullOrUndefined(fCurr)) {
                                isSet = fCurr[0].is_set;
                            }
                            this.centreGroup.currencies.push({
                                id: this.currenciesList[i].id,
                                currency_name: this.currenciesList[i].currency_name,
                                currency_code: this.currenciesList[i].currency_code,
                                currency_code_name: this.currenciesList[i].currency_code_name,
                                is_set: isSet
                            });
                        }
                    }

                } catch (e) {
                    this.hideLoader();
                    this.displayErrorMsg();
                } finally {
                    this.hideLoaderAfterAllIsLoaded();
                }
            }, (response: any) => {
                this.hideLoader();
                this.displayErrorMsg();
            });
            
        }

        private getIsCentreGroupReadOnly(): void {
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

            this.$http.get(
                this.getRegetRootUrl() + '/RegetAdmin/GetCgAdminRoles?cgId=' + this.selectedCentreGroupId +
                '&userId=' + this.participantId +
                '&t=' + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    if (!this.isValueNullOrUndefined(response.data)) {
                        this.adminRoles = this.getUserRoles(response.data);
                        this.isCgPropertyAdmin = this.adminRoles.is_cg_property_admin;
                        this.isCgAppMatrixAdmin = this.adminRoles.is_cg_appmatrix_admin;
                        this.isCgOrdererAdmin = this.adminRoles.is_cg_orderer_admin;
                        this.isCgRequestorAdmin = this.adminRoles.is_cg_requestor_admin;
                        this.isReadOnly = this.adminRoles.is_readonly;
                        
                    }
                    this.setReadOnly();
                    this.isCentresLoaded = true;
                 
                    if (this.isReadOnly) {
                        //this.isParticipantLoaded = true;
                        this.isOfficeLoaded = true;
                        this.isCentresLoaded = true;
                        this.isCgReadOnly = true;
                    } else {
                        //this.getParticipantData();
                        this.getOfficeData();
                        this.getNotAssignedCentresData();
                    }

                    this.isCurrenciesLoaded = true;
                } catch (e) {
                    this.hideLoader();
                    this.displayErrorMsg();
                } finally {
                    this.hideLoaderAfterAllIsLoaded();
                }
            }, (response: any) => {
                this.hideLoader();
                this.displayErrorMsg();
            });
                        
        }

        private getPurchaseGroupsCount(): void {
            this.showLoader(this.isError);

            this.$http.get(
                this.getRegetRootUrl() + '/RegetAdmin/GetPurchaseGroupsCount?cgId='
                + this.selectedCentreGroupId + '&t=' + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    this.purchaseGroupCount = Number(response.data);
                    this.populateNextPurchaseGroup();
                } catch (e) {
                    this.hideLoader();
                    this.displayErrorMsg();
                } finally {
                    this.hideLoaderAfterAllIsLoaded();
                }
            }, (response: any) => {
                this.hideLoader();
                this.displayErrorMsg();
            });
                        
        }

        private getNotAssignedCentresData(): void {
            this.showLoader(this.isError);

            this.$http.get(
                this.getRegetRootUrl() + '/RegetAdmin/GetActiveCompanyCentresData?cgId=' + this.selectedCentreGroupId
                + '&officeId=' + this.selectedOfficeId + '&t=' + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    this.notAssignedCentresList = this.getCentreList(response.data);
                    this.isCentresLoaded = true;
                } catch (e) {
                    this.hideLoader();
                    this.displayErrorMsg();
                } finally {
                    this.hideLoaderAfterAllIsLoaded();
                }
            }, (response: any) => {
                this.hideLoader();
                this.displayErrorMsg();
            });

        }

        private populateNextPurchaseGroup() : void {
            this.showLoader(this.isError);

            var strCount : string = "";
            if (!this.isValueNullOrUndefined(this.purchaseGroupCount)) {
                strCount = this.purchaseGroupCount.toString();
                if (this.purchaseGroupCount <= 25) {
                    var loadingMsg = this.loadingDataText;
                    angular.element("#spanLoading").html(loadingMsg);
                }
            }

            var strCountOutOf: string = "";
            if (!this.isValueNullOrUndefined(this.pgItemCount)) {
                strCountOutOf = this.pgItemCount.toString();
            }

            loadingMsg = this.loadingPurchaseGroupsText.replace("##", strCount).replace("#", strCountOutOf);
            $("#spanLoading").html(loadingMsg);

            this.$http.get(
                this.getRegetRootUrl() + '/RegetAdmin/GetPurchaseGroupsByCgId?cgId=' + this.selectedCentreGroupId
                + '&indexFrom=' + this.purchaseGroupList.length + '&t=' + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var data: any = response.data;
                    if (data.length > 0) {
                        this.pgItemCount++;
                        this.purchaseGroupList.push(response.data[0]);
                        this.populateNextPurchaseGroup();
                    } else {
                        $("#divPgLoading").hide();

                        this.hideLoaderAfterAllIsLoaded();
                        $("#spanLoading").html(this.loadTextBkp);

                        if (this.isDeactivatedPgLoading) {
                            this.isDeactivatedPgLoading = false;
                            this.isDeactivatedPgLoaded = true;
                            this.displayHideDisablePgs();
                        }
                    }
                } catch (e) {
                    $("#spanLoading").html(this.loadTextBkp);
                    this.hideLoader();
                    this.displayErrorMsg();
                } finally {
                    this.hideLoaderAfterAllIsLoaded();
                }
            }, (response: any) => {
                this.hideLoader();
                this.displayErrorMsg();
            });
            
        }

        private populateCentresSearch(searchText : string, fullSearchText : string, deferred : any) : void {
            if (this.isValueNullOrUndefined(deferred)) {
                return;
            }

            //this.showLoaderBoxOnly();

            this.$http.get(
                this.getRegetRootUrl() + '/RegetAdmin/GetCgCentres?searchText=' + searchText
                + '&isDisabledCgCentresLoaded=' + this.isDeactivatedCgDisplayed + '&t=' + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    this.centres = this.getCentresList(response.data);
                    deferred.resolve(this.filterCentres(fullSearchText));
                } catch (e) {
                    this.hideLoader();
                    this.displayErrorMsg();
                } finally {
                    this.hideLoaderAfterAllIsLoaded();
                }
            }, (response: any) => {
                this.hideLoader();
                this.displayErrorMsg();
            });
            
        }

        private isCentreAssigned(centre: Centre) {

            this.$http.get(
                this.getRegetRootUrl() + '/RegetAdmin/IsCentreAssigned?centreId=' + centre.id +
                '&cgId=' + this.selectedCentreGroupId +
                '&t=' + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    if (this.isValueNullOrUndefined(this.centreGroup.centre)) {
                        this.centreGroup.centre = [];
                    }

                    var strRes: string = response.data.toString();

                    if (this.isStringValueNullOrEmpty(strRes)) {
                        var tmpCentre: Centre = new Centre(centre.id, centre.name);
                        this.centreGroup.centre.push(tmpCentre);
                    } else {

                        var msg = this.moveCentreMsgText.replace("{0}", centre.name).replace("{1}", String(response.data));
                        var confirm = this.$mdDialog.confirm()
                            .title(this.moveCentreHeaderText)
                            .textContent(msg)
                            .ariaLabel(this.moveCentreHeaderText)
                            .targetEvent()
                            .ok(this.cancelText)
                            .cancel(this.moveText);

                        this.$mdDialog.show(confirm).then(() => {

                        }, () => {
                                var tmpCentre: Centre = new Centre(centre.id, centre.name);
                                this.centreGroup.centre.push(tmpCentre);
                        });

                    }
                } catch (e) {
                    this.hideLoader();
                    this.displayErrorMsg();
                } finally {
                    this.hideLoaderAfterAllIsLoaded();
                }
            }, (response: any) => {
                this.hideLoader();
                this.displayErrorMsg();
            });
            
        }

        private populateSuppliers(supplierGroupId: number, searchText: string, fullSearchText: string, deferred: any) {
            if (this.isValueNullOrUndefined(deferred)) {
                return;
            }

            this.$http.get(
                this.getRegetRootUrl() + '/RegetAdmin/GetActiveSuppliers?supplierGroupId=' + supplierGroupId
                + '&searchText=' + searchText + '&t=' + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    this.suppliers = tmpData;
                    deferred.resolve(this.filterSupplier(fullSearchText));
                } catch (e) {
                    this.hideLoader();
                    this.displayErrorMsg();
                } finally {
                    this.hideLoaderAfterAllIsLoaded();
                }
            }, (response: any) => {
                this.hideLoader();
                deferred.resolve(this.suppliers);
                this.displayErrorMsg();
            });
            
        }

        private saveCentreGroupDetails(savedMsg : string, missingMandatoryMsg: string, errMsg : string) : void {
            if (!(this.isCentregGroupDataValid())) {
                this.showAlert(this.warningText, missingMandatoryMsg, this.closeText);
                return;
            }
            
            this.errMsg = errMsg;
            this.showLoader(this.isError);
            
            var jsonCentreGroupData = JSON.stringify(this.centreGroup);

            this.$http.post(
                this.getRegetRootUrl() + 'RegetAdmin/SaveCentreGroupData',
                jsonCentreGroupData
            ).then((response) => {
                try {
                    var tmpResponse: any = response;
                    if (!this.isNewCg) {
                        this.updateCgDropdownName(this.centreGroup.id);
                    }

                    this.hideLoader();

                    if (!this.isStringValueNullOrEmpty(tmpResponse.data.string_value)) {
                        //duplicity name
                        this.showAlert(this.warningText, tmpResponse.data.string_value, this.closeText);
                        return;
                    }

                    this.showToast(savedMsg);

                    if (this.isNewCg) {
                        this.selectedCentreGroupId = tmpResponse.data.int_value;;
                        window.location.href = (this.getRegetRootUrl() + 'RegetAdmin/CentreGroup?cgId=' + this.selectedCentreGroupId + '&t=' + new Date().getTime());
                        return;
                    }

                    //Add centre to all of the Pg
                    if (!this.isValueNullOrUndefined(this.purchaseGroupList) && this.purchaseGroupList.length > 0) {
                        for (var i = 0; i < this.centreGroup.centre.length; i++) {
                            var centre = this.$filter("filter")(this.purchaseGroupList[0].centre, { id: this.centreGroup.centre[i].id }, true);
                            if (this.isValueNullOrUndefined(centre[0])) {
                                for (var j = 0; j < this.purchaseGroupList.length; j++) {
                                    var tmpCentre: Centre = new Centre(
                                        this.centreGroup.centre[i].id,
                                        this.centreGroup.centre[i].name
                                    );
                                    this.purchaseGroupList[j].centre.push(tmpCentre);
                                }
                            }
                        }
                    }

                    //Remove centre
                    if (!this.isValueNullOrUndefined(this.purchaseGroupList) && this.purchaseGroupList.length > 0) {
                        for (var i = 0; i < this.purchaseGroupList.length; i++) {
                            for (var j = this.purchaseGroupList[i].centre.length - 1; j >= 0; j--) {
                                var delCentre = this.$filter("filter")(this.centreGroup.centre, { id: this.purchaseGroupList[i].centre[j].id }, true);
                                if (this.isValueNullOrUndefined(delCentre[0])) {
                                    this.purchaseGroupList[i].centre.splice(j, 1);
                                }
                            }
                        }
                    }

                    //set active
                    var modCg = this.$filter("filter")(this.centreGroupList, { id: this.centreGroup.id }, true);
                    if (!this.isValueNullOrUndefined(modCg[0])) {
                        modCg[0].is_active = this.centreGroup.is_active;

                    }
                } catch (e) {
                    this.hideLoader();
                    this.displayErrorMsg();
                } finally {
                    this.hideLoaderAfterAllIsLoaded();
                }
            }, (response: any) => {
                    this.hideLoader();
                    this.displayErrorMsg();
            });
                     
        }

        private getParentPurchaseGroupData(purchaseGroup: PurchaseGroup): void {

            if (this.isValueNullOrUndefined(this.parentPurchaseGroups)) {
                this.showLoader(this.isError);

                this.$http.get(
                    this.getRegetRootUrl() + '/RegetAdmin/GetParentPurchaseGroups?t=' + new Date().getTime(),
                    {}
                ).then((response) => {
                    try {
                        var tmpData: any = response.data;
                        this.parentPurchaseGroups = tmpData;
                        this.setLoadPgEditData(purchaseGroup);
                    } catch (e) {
                        this.hideLoader();
                        this.displayErrorMsg();
                    } finally {
                        this.hideLoaderAfterAllIsLoaded();
                    }
                }, (response: any) => {
                        this.hideLoader();
                        this.displayErrorMsg();
                });
                                
            }
        }

        private deletePurchaseGroup(
            iPgId: number,
            strPgName: string,
            sindex: number,
            ev: any,
            strTitleText: string,
            strMsgText: string,
            strDeleteText: string,
            strCancelText: string,
            strSavedMsgText: string) {

            var strTitle = strTitleText;
            var strMsg = strMsgText.replace('{0}', strPgName).replace('{0}', strPgName);

            var confirm = this.$mdDialog.confirm()
                .title(strTitle)
                .textContent(strMsg)
                .ariaLabel(strTitle)
                .targetEvent(ev)
                .ok(strCancelText)
                .cancel(strDeleteText);

            this.$mdDialog.show(confirm).then(() => {

            }, () => {
                this.showLoader(this.isError);

                this.$http.post(
                    this.getRegetRootUrl() + 'RegetAdmin/DeletePurchaseGroup?pgId=' + iPgId
                    + '&cgId=' + this.selectedCentreGroupId + '&t=' + new Date().getTime(),
                    {}
                ).then((response) => {
                    try {
                        this.hideLoader();

                        var result : HttpResult = <HttpResult>response.data;

                        if (!this.isStringValueNullOrEmpty(result.string_value) && (result.string_value === "disabled")) {
                            //pg disabled
                            var disPg: PurchaseGroup[] = this.$filter("filter")(this.purchaseGroupList, { id: iPgId }, true);
                            if (!this.isValueNullOrUndefined(disPg[0])) {
                                disPg[0].is_active = false;
                            }

                            disPg[0].is_visible = this.isDeactivatedPgDisplayed;
                            if (this.isDeactivatedPgDisplayed) {
                                $("#divAppMatrix_" + disPg[0].id).show();
                            } else {
                                $("#divAppMatrix_" + disPg[0].id).hide();
                            }

                            var msg: string = this.purchaseGroupDisabledText.replace('{0}', strPgName);
                            this.showAlert(this.notificationText, msg, this.closeText);

                        } else {
                            //pg deleted
                            this.purchaseGroupList.splice(sindex, 1);

                            var msg: string = this.purchaseGroupDeletedText.replace('{0}', strPgName);
                            this.showAlert(this.notificationText, msg, this.closeText);
                        }
                    } catch (e) {
                        this.hideLoader();
                        this.displayErrorMsg();
                    } finally {
                        this.hideLoaderAfterAllIsLoaded();
                    }
                }, (response: any) => {
                    this.hideLoader();
                    this.displayErrorMsg();
                });

            });
        }


        private savePurchaseGroup(savedMsg: string, errMsg: string): void {
            for (var i: number = 0; i < this.editPurchaseGroup.purchase_group_limit.length; i++) {
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

                this.$http.post(
                    this.getRegetRootUrl() + 'RegetAdmin/AddNewPurchaseGroupData',
                    jsonPurchaseGroupData
                ).then((response) => {
                    try {
                        var tmpPg: any = response.data;
                        var newPg: PurchaseGroup = tmpPg;
                        var tmpNewEmptyPg = angular.copy(this.purchaseGroupList[this.purchaseGroupList.length - 1]);
                        this.purchaseGroupList.splice(this.purchaseGroupList.length - 1, 1);

                        this.purchaseGroupList.push(newPg);
                        this.purchaseGroupList.push(tmpNewEmptyPg);

                        this.hideLoader();

                        this.closePurchaseGroup();
                                                
                        this.showToast(savedMsg);
                    } catch (e) {
                        this.hideLoader();
                        this.displayErrorMsg();
                    } finally {
                        this.hideLoaderAfterAllIsLoaded();
                    }
                }, (response: any) => {
                    this.hideLoader();
                    this.displayErrorMsg();
                });

            } else {
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

                this.$http.post(
                    this.getRegetRootUrl() + 'RegetAdmin/SavePurchaseGroupData',
                    jsonPurchaseGroupData
                ).then((response) => {
                    try {
                        this.hideLoader();
                        this.closePurchaseGroup();
                                                
                        this.showToast(savedMsg);
                    } catch (e) {
                        this.hideLoader();
                        this.displayErrorMsg();
                    } finally {
                        this.hideLoaderAfterAllIsLoaded();
                    }
                }, (response: any) => {
                    this.hideLoader();
                    this.displayErrorMsg();
                });

                
            }


            if (this.editPurchaseGroup.is_active === false) {
                var disPg: PurchaseGroup[] = this.$filter("filter")(this.purchaseGroupList, { id: this.editPurchaseGroup.id }, true);
                disPg[0].is_visible = this.isDeactivatedPgDisplayed;
                if (this.isDeactivatedPgDisplayed) {
                    $("#divAppMatrix_" + disPg[0].id).show();
                } else {
                    $("#divAppMatrix_" + disPg[0].id).hide();
                }
            }
        }

        private scrollToPgHeader(): void {
            try {
                if (this.isValueNullOrUndefined(this.editPurchaseGroup)) {
                    return;
                }

                var element = $("#divAppMatrix_" + this.editPurchaseGroup.id);

                if (!this.isValueNullOrUndefined(element)) {
                    element[0].scrollIntoView();
                }
            } catch{ }
        }

        private isPurchaseGroupValid() : boolean {
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
                    } else {
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

                var dBottom: number = this.editPurchaseGroup.purchase_group_limit[iLimitId].limit_bottom;
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
        }
        
        //*****************************************************************************************************

        private clearDefaultSupp() : void {
            this.selectedDefaultSupplier = undefined;
            this.searchstringdefaultsupplier = undefined;
        }

        private isCentregGroupDataValid(): boolean {
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
        }             

        private populateCgCentres() : void {
            this.showLoader(this.isError);
            
            try {
                if (this.isValueNullOrUndefined(this.centreGroup.centre) ||
                    this.centreGroup.centre.length === 0) {
                    $("#divCgAppMatrixPanel").hide();
                }
               
            } catch (ex) {
                this.displayErrorMsg();
            } finally {
                this.hideOrdererSupplierAutoCompl();
            }
            
        }
               
        private displayHideDisablePgs() : void {
            //display, hide dissabled pgs
            var disPgs: PurchaseGroup[] = this.$filter("filter")(this.purchaseGroupList, { is_active: false }, true);
            if (!this.isValueNullOrUndefined(disPgs[0])) {
                for (var i = 0; i < disPgs.length; i++) {
                    disPgs[i].is_visible = this.isDeactivatedPgDisplayed;
                    if (this.isDeactivatedPgDisplayed) {
                        $("#divAppMatrix_" + disPgs[i].id).show();
                    } else {
                        $("#divAppMatrix_" + disPgs[i].id).hide();
                    }
                }
            }
        }

        private displayCgAdminDetail(userId: number) {
            this.displayElement("btnCollapseCgAdmin_" + userId);
            this.hideElement("btnExpandCgAdmin_" + userId);
            this.displayElementSlow("divCgAdmin_" + userId);
        }

        private hideCgAdminDetail(userId: number) {
            this.hideElement("btnCollapseCgAdmin_" + userId);
            this.displayElement("btnExpandCgAdmin_" + userId);
            this.hideElementSlow("divCgAdmin_" + userId);
        }

        private setSelectedCurrencyCode() : void {
            var selectedCurr : Currency[] = this.currenciesList.filter((currency: Currency) => {
                return currency.id === this.selectedCurrencyId;
            });

            if (!this.isValueNullOrUndefined(selectedCurr) && selectedCurr.length > 0) {
                this.selectedCurrencyCode = selectedCurr[0].currency_code;
                this.selectedCurrencyCodeName = selectedCurr[0].currency_code_name;
            }
        }

        private setReadOnly() : any {
            if (this.isAllCgReadOnly) {
                return;
            }
            
            var isOfficeReadOnly = this.getIsOfficeReadOnly();
                        
            var selectedCurr = this.currenciesList.filter((currency: Currency) => {
                return currency.id === this.selectedCurrencyId;
            });
                        
            if (!this.isCgPropertyAdmin || this.isError) {
                $("#btnSaveCgDetails").hide();
                $("#hrCgDetail").hide();
            } else {
                $("#btnSaveCgDetails").show();
                $("#hrCgDetail").show();
            }
        }

        private getIsOfficeReadOnly() : void {
            var isOfficeReadOnly = this.$scope.IsReadOnly || this.offices === null || this.offices.length < 2;
            if (!isOfficeReadOnly) {
                var currOffice = this.$filter("filter")(this.offices, { company_id: this.selectedOfficeId }, true);
                if (currOffice === null || currOffice.length === 0) {
                    isOfficeReadOnly = true;
                }
            }

            return isOfficeReadOnly;
        }

        private displaySearchCentre() : void {

            var appCentre = angular.element("#appCentre");
            var btnFind = angular.element("#btnFindCentre");

            if (appCentre.is(':visible')) {
                appCentre.hide();
                btnFind[0].style.marginTop = "5px";
            } else {
                var childWrap = appCentre.children()[0];
                childWrap.className += ' reget-search-text';
                var childText = childWrap.children[0];
                childText.setAttribute('style', 'margin-top:-4px;');
                btnFind[0].style.marginTop = "2px";

                appCentre.show("slow");
            }

        }

        private centreQuerySearch(query : string) : any {
            var results = null;

            if (!this.isValueNullOrUndefined(query) && query.length === 1) {
                var deferred = this.$q.defer();
                this.populateCentresSearch(query, query, deferred);
                return deferred.promise;
            } else {
                if (this.isValueNullOrUndefined(this.centres)) {
                    var deferred = this.$q.defer();
                    this.populateCentresSearch(query.substring(0, 2), query, deferred);
                    return deferred.promise;
                } else {
                    return this.filterCentres(query);
                }
            }

        }

        private centreSelectedItemChange(item : Centre) {
            if (this.isValueNullOrUndefined(item)) {
                return;
            }
            this.selectedCentreGroupId = item.centre_group_id;
            this.getCentreGroupData();

        }

        private filterCentres(query: string) : Centre[] {
            var searchCentres = [];

            if (this.isValueNullOrUndefined(query) || query.length < 1) {
                return null;
            }


            angular.forEach(this.centres, (centre: Centre) => {
                if (centre.name.toLowerCase().indexOf(query.toLowerCase()) > -1) {
                    searchCentres.push(centre);
                }

            });
            return searchCentres;
        }

        private exportToExcel() : void {
            this.showLoaderBoxOnly();
            try {
                window.open(this.getRegetRootUrl() + 'Report/GetCentreGroup?cgId=' + this.selectedCentreGroupId + '&t=' + new Date().getTime());
            } catch (ex) {
                this.displayErrorMsg();
            } finally {
                this.hideLoader();
            }
        }

        private setCurrency() : void {
            this.centreGroup.currency_id = this.selectedCurrencyId;
        }

        private setOffice() : void {
            this.centreGroup.company_id = this.selectedOfficeId;
            this.getNotAssignedCentresData();
            
            this.populateCurrencies(-1, -1, this.selectedOfficeId);
        }

        private getIsAllCgReadOnly() : void {
            var iAllCgReadOnly = $("#spanIsAllCgReadOnly").text();
            this.isAllCgReadOnly = (iAllCgReadOnly === '1');
        }
                
        private displayAppManAutoCompl(limitId: number, isMobile : boolean) : void {
            if (isMobile) {
                $("#appManAutoComplMobile_" + limitId).show();
            } else {
                $("#appManAutoCompl_" + limitId).show();
            }
        }

        private displayDeputyOrdererAutoCompl() : void {
            $("#autoDeputyOrderer").show();
        }

        private displayCgAdminAutoCompl() : void {
            $("#autoCgAdmin").show();
        }

        private displayCentreAutoCompl() : void {
            $("#autoCentre").show();
        }

        private displayOrdererSupplierAutoCompl() : void {
            this.selectedOrdererSupplierOrderer = null;
            this.selectedOrdererSupplierSupplier = null;
            
            $("#divOrdererSupplierAuto").show();
        }

        private hideOrdererSupplierAutoCompl(): void {
            this.searchstringorderersupplierorderer = null;

            var $autWrap = $("#divOrdererSupplierAuto").children().first();
            var $autChild = $autWrap.children().first();
            $autChild.val('');

            $("#divOrdererSupplierAuto").hide();
        }

        private hideDeputyOrdererAutoCompl(): void {
            this.searchstringdeputyorderer = null;

            var $autWrap = $("#autoDeputyOrderer").children().first();
            var $autChild = $autWrap.children().first();
            $autChild.val('');

            $("#autoDeputyOrderer").hide();
        }
                
        private hideCgAdminAutoCompl() : void {
            this.searchstringcgadmin = null;

            var $autWrap = $("#autoDeputyOrderer").children().first();
            var $autChild = $autWrap.children().first();
            $autChild.val('');

            $("#autoCgAdmin").hide();
        }

        private hideCentreAutoCompl() : void {
            var $autWrap = $("#autoCentre").children().first();
            var $autChild = $autWrap.children().first();
            $autChild.val('');

            $("#autoCentre").hide();
        }

        private hideCentreGroupDetails(): void {
            $("#divCgDetailPanel").hide();
            $("#divCgAppMatrixPanel").hide();
        }

        private hideLoaderAfterAllIsLoaded(): void {
            if (this.isCurrenciesLoaded &&
                this.isCentresLoaded &&
                this.isCgPurchaseGroupsLoaded &&
                //this.isParticipantLoaded &&
                this.isOfficeLoaded) {

                this.hideLoader();
            }
        }
        
        private displayRequestorAutoCompl(centreId : number) : void {
            $("#autoRequestor" + "_" + centreId).show();
        }

        private displayOrdererAutoCompl(limitId : number) : void {
            $("#autoOrderer").show();
        }
                
        private hideRequestorAutoCompl(centreId: number) : void {
            this.searchstringrequestor = null;
            this.selectedRequestor = null;

            var $autWrap = $("#autoRequestor_" + centreId).children().first();
            var $autChild = $autWrap.children().first();
            $autChild.val('');
            $("#autoRequestor_" + centreId).hide();
        }

        private hideOrdererAutoCompl() : void {
            this.searchstringorderer = null;
            this.selectedOrderer = null;

            var $autWrap = $("#autoOrderer").children().first();
            var $autChild = $autWrap.children().first();
            $autChild.val('');
            $("#autoOrderer").hide();
        }

        private searchNotAssignedCentre(strName : string) : void {
            var results = strName ? this.filterNotAssignedCentres(strName) : this.notAssignedCentresList, deferred;
            
            var deferred : any = this.$q.defer<any>();
            if (strName) {
                deferred.resolve(results);
            } else {
                this.$timeout(() => { deferred.resolve(results); }, 3000, false);
            }
            
            return deferred.promise;
           
        }

        private filterNotAssignedCentres(name : string) : Centre[] {
            var searchCentres = [];
            
            if (this.isStringValueNullOrEmpty(name)) {
                return this.notAssignedCentresList;
            }

            angular.forEach(this.notAssignedCentresList, (centre: Centre) => {
                if (centre.name.toLowerCase().indexOf(name.toLowerCase()) > -1) {
                    searchCentres.push(centre);
                }

            });

            return searchCentres;
        }

        private centreCgSelectedItemChange(item : Centre) : void {
            if (this.isValueNullOrUndefined(item)) {
                return;
            }

            try {

                var centre = this.$filter("filter")(this.centreGroup.centre, { id: item.id }, true);

                if (this.isValueNullOrUndefined(centre[0])) {
                    this.isCentreAssigned(item);
                    
                }
            } catch (e) {
                this.displayErrorMsg();
            } finally {
                this.hideCentreAutoCompl();
            }
        }

        private toggleDisplayDeactivatedCg(): void {
            this.isDeactivatedCgDisplayed = !this.isDeactivatedCgDisplayed;
            this.populateCentreGroups(true);
        }

        private isForeignCurrency() : boolean {
            if (this.isValueNullOrUndefined(this.centreGroup) || this.isValueNullOrUndefined(this.centreGroup.currency)) {
                return false;
            }

            for (var i = 0; i < this.centreGroup.currency.length; i++) {
                if (this.centreGroup.currency[i].is_set && this.selectedCurrencyId !== this.centreGroup.currency[i].id) {
                    return true;
                }
            }

            return false;
        }

        private isForeignCurrencyAvailable() : boolean {
            if (this.isValueNullOrUndefined(this.centreGroup)) {
                return false;
            }

            if (this.isValueNullOrUndefined(this.centreGroup.currency)) {
                return false;
            }

            return (this.centreGroup.currency.length > 1);
        }

        public isAllCurreenciesChecked() : boolean {
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
        }

        private isCurrenciesIndeterminate() : boolean {
            if (this.isValueNullOrUndefined(this.centreGroup) || this.isValueNullOrUndefined(this.centreGroup.currency)) {
                return false;
            }

            return (this.centreGroup.currency.length !== 0 &&
                !this.isAllCurreenciesChecked());
        }

        private toggleAllForeignCurrencies(): void{

            if (this.isValueNullOrUndefined(this.centreGroup) || this.isValueNullOrUndefined(this.centreGroup.currency)) {
                return;
            }
            
            if (this.isAllCurreenciesChecked()) {
                for (var i = 0; i < this.centreGroup.currency.length; i++) {
                    this.centreGroup.currency[i].is_set = false;
                }
            } else {
                for (var i = 0; i < this.centreGroup.currency.length; i++) {
                    this.centreGroup.currency[i].is_set = true;
                }
            }
        }

        private toggleForeignCurrency(item : Currency) {
            if (item.is_set) {
                item.is_set = false;
            } else {
                item.is_set = true;
            }
        }

        private toggleFreeSupplier() : void {
            if (this.centreGroup.allow_free_supplier) {
                this.centreGroup.allow_free_supplier = false;
            } else {
                this.centreGroup.allow_free_supplier = true;
            }
        }

        private togglePgActive(item : PurchaseGroup) : void {

            if (item.is_active) {
                item.is_active = false;
            } else {
                item.is_active = true;
            }

            var divPgHeader = angular.element("#divCgMatrixCollHeader_" + item.id);
            if (divPgHeader === null || divPgHeader.length === 0) {
                return;
            }

            if (item.is_active) {
                divPgHeader[0].className = 'reget-box-header reget-box-header-level1';
            } else {
                divPgHeader[0].className = 'reget-box-header reget-box-header-level1-disabled';
            }
        }

        private togglePgOrderNeeded(item: PurchaseGroup) {

            //if (item.is_approval_only) {
            //    item.is_approval_only = false;
            //    item.purchase_type = PurchaseGroupType.Standard;
            //} else {
            //    item.is_approval_only = true;
            //    item.purchase_type = PurchaseGroupType.ApprovalOnly;
            //}

            if (item.is_order_needed) {
                item.is_order_needed = false;
                
            } else {
                item.is_order_needed = true;
                
            }
        }

        private togglePgApprovalNeeded(item: PurchaseGroup) {

            if (item.is_approval_needed) {
                item.is_approval_needed = false;

            } else {
                item.is_approval_needed = true;

            }
        }

        private toggleTopUnlimited(item : PurchaseGroupLimit) : void {

            if (item.is_top_unlimited) {
                item.is_top_unlimited = false;
            } else {
                item.is_top_unlimited = true;
            }

            this.checkAppLimitBottomLowerThanTop(item, false);
        }

        private toggleBottomUnlimited(item: PurchaseGroupLimit) : void {

            if (item.is_bottom_unlimited) {
                item.is_bottom_unlimited = false;
            } else {
                item.is_bottom_unlimited = true;
            }

            this.checkAppLimitBottomLowerThanTop(item, true);
        }

        private toggleBottomMultipl(item : PurchaseGroupLimit) : void {

            if (item.is_limit_bottom_multipl) {
                item.is_limit_bottom_multipl = false;
            } else {
                item.is_limit_bottom_multipl = true;
            }

        }

        private toggleTopMultipl(item: PurchaseGroupLimit) : void  {

            if (item.is_limit_top_multipl) {
                item.is_limit_top_multipl = false;
            } else {
                item.is_limit_top_multipl = true;
            }
            
        }

        private toggleActive() : void {

            if (this.centreGroup.is_active) {
                this.centreGroup.is_active = false;
            } else {
                this.centreGroup.is_active = true;
            }
            
        }

        private toggleDisplayDeactivatedPg() : void {
            this.isDeactivatedPgDisplayed = !this.isDeactivatedPgDisplayed;

            if (!this.isDeactivatedPgLoaded) {
                this.isDeactivatedPgLoading = true;
                this.populatePurchaseGroups(true);
                this.editPurchaseGroup = null;
            } else {

                this.displayHideDisablePgs();
            }

        }

        private togleAppMatrix(pgId: number) : void {
            if (this.isCancelEvent === true) {
                this.isCancelEvent = false;
                return;
            }


            var pg: PurchaseGroup[] = this.$filter("filter")(this.purchaseGroupList, { id: pgId }, true);

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
            } else {
                $("#" + phAppMatrix).show("slow");
                $("#" + imgExpandCollapse).attr("src", this.getRegetRootUrl() + "Content/Images/Panel/Collapse.png");
                $("#" + imgExpandCollapse).attr("title", this.hideText);
                $("#" + spanExpCol).text(this.expandAllText);
            }

        }

        private toggleAllPurchaseGroups(isCollapse: boolean) : void {
            if (isCollapse) {
                if (!isCollapse) {
                    $(".reget-appmatrix").show("slow");

                } else {
                    $(".reget-appmatrix").slideUp();

                }

                for (var i = 0; i < this.pgExpandImgs.length; i++) {
                    if (!isCollapse) {
                        this.pgExpandImgs[i].src = this.getRegetRootUrl() + "Content/Images/Panel/Collapse.png";
                        this.pgExpandImgs[i].title = "@Html.Raw(RequestResource.Hide)";
                    } else {
                        this.pgExpandImgs[i].src = this.getRegetRootUrl() + "Content/Images/Panel/Expand.png";
                        this.pgExpandImgs[i].title = "@Html.Raw(RequestResource.Display)";
                    }
                }

                return;
            }

            this.isCancelEvent = true;
            this.showLoaderBoxOnly();

            this.pgExpandImgs = <HTMLCollectionOf<HTMLImageElement>> document.getElementsByClassName("reget-appmatrix-expcolimg");//$(".reget-appmatrix-expcolimg");

            this.togglePurchaseGroupRecursive(0, isCollapse);
        }

        private togglePurchaseGroupRecursive(iIndex: number, isCollapse: boolean) {

            var idParts = this.pgExpandImgs[iIndex].id.split('_');
            var pgId = parseInt(idParts[1]);

            var promise: any = this.displayPurchaseGroupAsync(pgId, isCollapse);

            promise.then(() => {
                //success
                try {
                    var strCount = iIndex + 1;
                    var loadingMsg = this.loadingPurchaseGroupsText.replace("##", this.pgExpandImgs.length.toString()).replace("#", strCount.toString());
                    $("#spanLoadingBoxOnly").html(loadingMsg);
                    if (iIndex < this.pgExpandImgs.length - 1) {
                        this.togglePurchaseGroupRecursive(iIndex + 1, isCollapse);
                    } else {
                        if (!isCollapse) {
                            $(".reget-appmatrix").show("slow");

                        } else {
                            $(".reget-appmatrix").slideUp();
                        }

                        for (var i = 0; i < this.pgExpandImgs.length; i++) {
                            if (!isCollapse) {
                                this.pgExpandImgs[i].src = this.getRegetRootUrl() + "Content/Images/Panel/Collapse.png";
                                this.pgExpandImgs[i].title = "@Html.Raw(RequestResource.Hide)";
                            } else {
                                this.pgExpandImgs[i].src = this.getRegetRootUrl() + "Content/Images/Panel/Expand.png";
                                this.pgExpandImgs[i].title = "@Html.Raw(RequestResource.Display)";
                            }
                        }

                        this.hideLoader();
                    }
                } catch (e) {
                    this.hideLoader();
                    this.displayErrorMsg();
                }
            }, (reason) => {
                //fail
                this.hideLoader();
                this.displayErrorMsg();
            });

        }

        private displayPurchaseGroupAsync(pgId: number, isCollapse: boolean) {
            var deferredPg = this.$q.defer();

            var pg: PurchaseGroup[] = this.$filter("filter")(this.purchaseGroupList, { id: pgId }, true);

            if (this.isValueNullOrUndefined(pg) || pg.length === 0) {
                deferredPg.resolve('OK');
            } else {

                if (pg[0].is_html_show === true) {
                    deferredPg.resolve('OK');
                } else {

                    setTimeout(() => {
                        if (pg[0].is_html_show !== true) {
                            pg[0].is_html_show = true;
                        }

                        deferredPg.resolve('OK');

                    }, 10);
                }
            }

            return deferredPg.promise;
        }

        private deleteRequestor(iRequestorId: number, iCentreId : number, sindex:number, ev:any) : void {
            let requestor : Requestor[] = this.$filter("filter")(this.editPurchaseGroup.requestor, { participant_id: iRequestorId, centre_id: iCentreId }, true);
            if (!this.isValueNullOrUndefined(requestor[0])) {

                if (requestor[0].is_all) {

                    this.$mdDialog.show(
                        {
                            template: this.getConfirmDialogTemplate(
                                this.deleteAllRequestorTitleText,
                                this.deleteAllRequestorMsgText,
                                this.removeText,
                                this.cancelText,
                                "confirmDialog()",
                                "closeDialog()"),
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

                } else {
                    this.editPurchaseGroup.requestor.splice(sindex, 1);
                }
            }
        }

        public deleteRequestorConfirmed(requestor: Requestor, sindex : number): void {
            if (this.isValueNullOrUndefined(this.deleteAllRequestorList)) {
                this.deleteAllRequestorList = [];
            }

            var tmpRequestor: Requestor = new Requestor();
            tmpRequestor.participant_id = requestor.participant_id;
            tmpRequestor.centre_id = requestor.centre_id;

            this.deleteAllRequestorList.push(tmpRequestor);

            this.editPurchaseGroup.requestor.splice(sindex, 1);
        }

        private dialogConfirmRequestorDeleteController(
            $scope,
            $mdDialog,
            cgAdminController: CgAdminController,
            requestor: Requestor,
            sindex: number
        ): void {

            $scope.closeDialog = function () {
                $mdDialog.hide();
            }

            $scope.confirmDialog = () => {
                $mdDialog.hide();
                cgAdminController.deleteRequestorConfirmed(requestor, sindex);
            }
        }

        private deleteOrderer(iOrdererId: number, sindex: number, ev:any) : void {
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

                    this.$mdDialog.show(confirm).then(() => {

                    }, () => {
                        if (this.isValueNullOrUndefined(this.deleteAllOrdererList)) {
                            this.deleteAllOrdererList = [];
                        }
                       
                        var tmpOrderer: Orderer = new Orderer();
                        tmpOrderer.participant_id = orderer[0].participant_id;
                        this.deleteAllOrdererList.push(tmpOrderer);

                        this.editPurchaseGroup.orderer.splice(sindex, 1);
                    });

                } else {
                    this.editPurchaseGroup.orderer.splice(sindex, 1);
                }
            }
        }

        private deleteCentre(iCentreId: number, sindex: number): void {
            var centre = this.$filter("filter")(this.centreGroup.centre, { id: iCentreId }, true);

            if (!this.isValueNullOrUndefined(centre[0])) {
                this.centreGroup.centre.splice(sindex, 1);
            }
        }

        private deleteDeputyOrderer(iOrdererId: number, sindex: number): void {
            var orderer = this.$filter("filter")(this.centreGroup.deputy_orderer, { participant_id: iOrdererId }, true);

            if (!this.isValueNullOrUndefined(orderer[0])) {
                this.centreGroup.deputy_orderer.splice(sindex, 1);
            }
        }

        private deleteCgAdmin(iAdminId: number, sindex: number, isCompanyAdmin: boolean): void {
            if (isCompanyAdmin === true) {
                this.showAlert(this.warningText, this.cannotDeleteCompanyAdminText, this.closeText);
            } else {
                this.centreGroup.cg_admin.splice(sindex, 1);
            }

        }

        private deleteOrdererSupplier(iOrdererId: number, iSupplierId: number, sindex: number): void {
            var orderer = this.$filter("filter")(this.centreGroup.orderer_supplier_appmatrix,
                { orderer_id: iOrdererId, supplier_id: iSupplierId }, true);

            if (!this.isValueNullOrUndefined(orderer[0])) {
                this.centreGroup.orderer_supplier_appmatrix.splice(sindex, 1);
            }
        }

        private searchParticipant(strName: string): ng.IPromise<Participant[]>  {

            return this.filterParticipants(strName);

            //var results = strName ? this.filterParticipants(strName) : this.participants, deferred;

            //var deferred: any = this.$q.defer<any>();
            //if (strName) {
            //    deferred.resolve(results);
            //} else {
            //    this.$timeout(() => { deferred.resolve(results); }, 3000, false);
            //}

            //return deferred.promise;
              
        }

        private deputyOrdererSelectedItemChange(item: Participant) : void {
            if (this.isValueNullOrUndefined(item)) {
                return;
            }

            try {
                var requestor = this.$filter("filter")(this.centreGroup.deputy_orderer, { participant_id: item.id }, true);

                if (this.isValueNullOrUndefined(requestor[0])) {
                    var tmpDeputyOrder: Orderer = new Orderer();
                    tmpDeputyOrder.participant_id = item.id;
                    tmpDeputyOrder.surname = item.surname;
                    tmpDeputyOrder.first_name = item.first_name;
                    tmpDeputyOrder.substituted_by = item.substituted_by;
                    tmpDeputyOrder.substituted_until = item.substituted_until;

                    this.centreGroup.deputy_orderer.push(tmpDeputyOrder);
                }
            } catch (e) {
                this.displayErrorMsg();
            } finally {
                this.hideDeputyOrdererAutoCompl();
            }
        }

        private ordererSupplierSelectedItemChange() : void {
            
            if (this.isValueNullOrUndefined(this.selectedOrdererSupplierOrderer) ||
                this.isValueNullOrUndefined(this.selectedOrdererSupplierSupplier)) {
                return;
            }

            try {
                var ordererSupplierAppmatrix: OrdererSupplier[] = this.$filter("filter")(
                    this.centreGroup.orderer_supplier_appmatrix, {
                    orderer_id: this.selectedOrdererSupplierOrderer.id,
                    supplier_id: this.selectedOrdererSupplierSupplier.id
                }, true);

                if (this.isValueNullOrUndefined(ordererSupplierAppmatrix[0])) {
                    var tmpOrdererSupplier: OrdererSupplier = new OrdererSupplier();

                    tmpOrdererSupplier.orderer_id = this.selectedOrdererSupplierOrderer.id;
                    tmpOrdererSupplier.surname = this.selectedOrdererSupplierOrderer.surname;
                    tmpOrdererSupplier.first_name = this.selectedOrdererSupplierOrderer.first_name;
                    tmpOrdererSupplier.supplier_id = this.selectedOrdererSupplierSupplier.id;
                    tmpOrdererSupplier.supplier_name = this.selectedOrdererSupplierSupplier.supp_name;

                    this.centreGroup.orderer_supplier_appmatrix.push(tmpOrdererSupplier);
                }
            } catch (e) {
                this.displayErrorMsg();
            } finally {
                this.selectedOrdererSupplierOrderer = null;
                this.selectedOrdererSupplierSupplier = null;
                this.hideOrdererSupplierAutoCompl();
            }
        }

        private defaultSupplierQuerySearch(query : string) : any {
            var results = null;

            $('#txtDefaultSupplier').attr('style', 'max-width:none;');

            if (!this.isValueNullOrUndefined(query) && query.length === 2) {
                var deferred: any = this.$q.defer<any>();
                this.populateSuppliers(this.centreGroup.supplier_group_id, query, query, deferred);
                return deferred.promise;
            } else {
                if (this.isValueNullOrUndefined(this.suppliers)) {
                    var deferred: any = this.$q.defer<any>();
                    this.populateSuppliers(this.centreGroup.supplier_group_id, query.substring(0, 2), query, deferred);
                    return deferred.promise;
                } else {
                    return this.filterSupplier(query);
                }
            }
      
        }   

        private filterSupplier(query : string) : Supplier[] {
            var searchSuppliers: Supplier[] = [];

            $("#txtDefaultSupplier").width(550);

            if (this.isValueNullOrUndefined(query) || query.length < 2) {
                return null;
            }
                       
            angular.forEach(this.suppliers, (supplier: Supplier) => {
                if (supplier.supp_name.toLowerCase().indexOf(query.toLowerCase()) > -1) {
                    searchSuppliers.push(supplier);
                }
            });

            return searchSuppliers;
        }      

        private checkAppLimitBottomLowerThanTop(limitItem : PurchaseGroupLimit, isBottom: boolean) : void {
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
            } else {
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

        }      

        private cgAdminSelectedItemChange(item : CgAdmin) : void {
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
            } catch (e) {
                this.displayErrorMsg();
            } finally {
                this.hideCgAdminAutoCompl();
            }
        }

        private checkCgPropAdminToggle(iCgAdmin: number) : void {
            var cgadmn = this.$filter("filter")(this.centreGroup.cg_admin, { id: iCgAdmin }, true);

            if (!this.isValueNullOrUndefined(cgadmn[0])) {
                if (cgadmn[0].is_cg_prop_admin === true) {
                    cgadmn[0].is_cg_prop_admin = false;
                } else {
                    cgadmn[0].is_cg_prop_admin = true;
                }
            }
        }

        private checkAppMatrixAdminToggle(iCgAdmin: number) : void{
            var cgadmn = this.$filter("filter")(this.centreGroup.cg_admin, { id: iCgAdmin }, true);

            if (!this.isValueNullOrUndefined(cgadmn[0])) {
                if (cgadmn[0].is_appmatrix_admin === true) {
                    cgadmn[0].is_appmatrix_admin = false;
                } else {
                    cgadmn[0].is_appmatrix_admin = true;
                }
            }
        }

        private checkRequestorAdminToggle(iCgAdmin : number) : void {
            var cgadmn = this.$filter("filter")(this.centreGroup.cg_admin, { id: iCgAdmin }, true);

            if (!this.isValueNullOrUndefined(cgadmn[0])) {
                if (cgadmn[0].is_requestor_admin === true) {
                    cgadmn[0].is_requestor_admin = false;
                } else {
                    cgadmn[0].is_requestor_admin = true;
                }
            }
        }

        private checkOrdererAdminToggle(iCgAdmin: number) : void {
            var cgadmn = this.$filter("filter")(this.centreGroup.cg_admin, { id: iCgAdmin }, true);

            if (!this.isValueNullOrUndefined(cgadmn[0])) {
                if (cgadmn[0].is_orderer_admin === true) {
                    cgadmn[0].is_orderer_admin = false;
                } else {
                    cgadmn[0].is_orderer_admin = true;
                }
            }
        }

        private updateCgDropdownName(cgId: number) : void {
            for (var i = 0; i < this.centreGroupList.length; i++) {
                if (this.centreGroupList[i].id === cgId) {
                    this.centreGroupList[i].name = this.centreGroup.name;
                    break;
                }
            }
        }

        private editAppMatrix(purchaseGroup: PurchaseGroup) {
           
            this.deleteAllRequestorList = null;
            this.suppliers = null;
            this.saveErrMsg = null;

            if (!this.isValueNullOrUndefined(this.editPurchaseGroup)) {
                var strMessage;
                if (this.isValueNullOrUndefined(this.purchaseGroupEditingText)) {
                    strMessage = ("Another Purchase Group is Edited");
                } else {
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
                } else {
                    this.selectedDefaultSupplier = purchaseGroup.default_supplier;
                    this.searchstringdefaultsupplier = purchaseGroup.default_supplier.supp_name;
                }
            } catch (ex) {
                this.hideLoader();
                this.displayErrorMsg();
            }

        }

        private getLoadPgEditData(purchaseGroup: PurchaseGroup) : void {
            if (!this.isValueNullOrUndefined(this.parentPurchaseGroups)) {
                this.setLoadPgEditData(purchaseGroup);
            } else {
                this.getParentPurchaseGroupData(purchaseGroup);
            }
        }

        private setLoadPgEditData(purchaseGroup: PurchaseGroup) : void {
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
        }
                              
        public displayErrorMsgHideForm() : void {
            $("#divCgDetailPanel").hide();
            $("#divCgAppMatrixPanel").hide();

            super.displayErrorMsg();
        }

        private addPurchaseGroup() : void {
           
            this.deleteAllRequestorList = null;
            this.suppliers = null;

            if (!this.isValueNullOrUndefined(this.editPurchaseGroup)) {
                var strMessage;
                if (this.isValueNullOrUndefined(this.purchaseGroupEditingText)) {
                    strMessage = ("Another Purchase Group is Edited");
                } else {
                    strMessage = (this.purchaseGroupEditingText);
                }
                this.showAlert(
                    "Reget Dialog",
                    strMessage,
                    this.cancelText);

                return;
            }

            $("#divAppMatrix_-1").show();
            if (!($("#phAppMatrix_-1").is(':visible'))) {
                $("#phAppMatrix_-1").show();
                $("#impAppMatrixCollExp_-1").attr("src", this.getRegetRootUrl() + "Content/Images/Panel/Collapse.png");
                $("#impAppMatrixCollExp_-1").attr("title", "Hide");
                $("#spanExpCol_-1").text("Expand All");
            }
                        
            let newPg: any = this.$filter("filter")(this.purchaseGroupList, { id: -1 }, true);
            let newPgItem: PurchaseGroup = newPg[0];
            newPgItem.is_order_needed = true;
            newPgItem.is_approval_needed = true;
            this.editAppMatrix(newPgItem);
            
        }

        //***********************************************************************************************************
        //App Matrix 
        private parentPgChanged() {
            if ((!this.isValueNullOrUndefined(this.editPurchaseGroup.local_text[0]) &&
                this.isStringValueNullOrEmpty(this.editPurchaseGroup.local_text[0].text)) ||
                this.editPurchaseGroup.id < 0) {

                if (!this.isValueNullOrUndefined(this.editPurchaseGroup.parent_pg_id)) {
                    var parentPg: ParentPurchaseGroup[] = this.$filter("filter")(this.parentPurchaseGroups, { id: this.editPurchaseGroup.parent_pg_id }, true);
                    if (!this.isValueNullOrUndefined(parentPg[0])) {
                        this.editPurchaseGroup.local_text[0].text = parentPg[0].name;
                    }
                }
            }
        }

        private closePurchaseGroup() : void {
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

        }

        private localtext1changed(strValue: string) : void {
            this.editPurchaseGroup.local_text[0].text = strValue;
        }

        private localtext2changed(strValue: string): void {
            this.editPurchaseGroup.local_text[1].text = strValue;
        }

        private localtext3changed(strValue: string): void {
            this.editPurchaseGroup.local_text[2].text = strValue;
        }

        private deleteLimit(limitId: number): void {
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
        }

        private replaceUp(limitId: number) : void {
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
        }

        private clearAppLimit(appLimit : PurchaseGroupLimit) : void {
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

        }

        private setAppLimitFirstLast() : void {
            this.editPurchaseGroup.purchase_group_limit[0].is_first = true;
            for (var i = 0; i < this.editPurchaseGroup.purchase_group_limit.length; i++) {
                if (i < 5 && this.editPurchaseGroup.purchase_group_limit[i].is_visible &&
                    !(this.editPurchaseGroup.purchase_group_limit[i + 1].is_visible)) {
                    this.editPurchaseGroup.purchase_group_limit[i].is_last = true;
                } else {
                    this.editPurchaseGroup.purchase_group_limit[i].is_last = false;
                }
                if (i === 5 && this.editPurchaseGroup.purchase_group_limit[i].is_visible) {
                    this.editPurchaseGroup.purchase_group_limit[i].is_last = true;
                }
            }
        }

        private updateAppMatrixUpdateLimit(modifPurchaseGroupLimit: PurchaseGroupLimit, origPurchaseGroupLimit: PurchaseGroupLimit) : void {
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
        }

        

        private updateAppMatrixDeleteAppMan(modifPurchaseGroupLimit: PurchaseGroupLimit, origPurchaseGroupLimit: PurchaseGroupLimit) : void {
            //Delete Manager Role
            for (var iManRoleIndex: number = origPurchaseGroupLimit.manager_role.length - 1; iManRoleIndex >= 0; iManRoleIndex--) {
                var iParticipantId: number = origPurchaseGroupLimit.manager_role[iManRoleIndex].participant.id;

                var managerRoleModif = this.$filter("filter")(modifPurchaseGroupLimit.manager_role, { participant_id: iParticipantId }, true);

                if (this.isValueNullOrUndefined(managerRoleModif[0])) {
                    //Delete Manager
                    origPurchaseGroupLimit.manager_role.splice(iManRoleIndex, 1);
                }
            }
        }

        private updateAppMatrixNewAppMan(modifPurchaseGroupLimit: PurchaseGroupLimit, origPurchaseGroupLimit: PurchaseGroupLimit) : void {
            //New Manager Role
            for (var iManRoleId: number = 0; iManRoleId < modifPurchaseGroupLimit.manager_role.length; iManRoleId++) {
                var iParticipantId: number = modifPurchaseGroupLimit.manager_role[iManRoleId].participant_id;

                var managerRoleOrig: ManagerRole[] = this.$filter("filter")(origPurchaseGroupLimit.manager_role, { participant_id: iParticipantId });

                if (this.isValueNullOrUndefined(managerRoleOrig[0])) {
                    //New Manager
                    var tmpManagerRole: ManagerRole = new ManagerRole();
                    tmpManagerRole.participant_id = modifPurchaseGroupLimit.manager_role[iManRoleId].participant.id;
                    tmpManagerRole.approve_level_id = modifPurchaseGroupLimit.app_level_id;

                    var tmpParticipant: Participant = new Participant();
                    tmpParticipant.id = modifPurchaseGroupLimit.manager_role[iManRoleId].participant.id;
                    tmpParticipant.surname = modifPurchaseGroupLimit.manager_role[iManRoleId].participant.surname;
                    tmpParticipant.first_name = modifPurchaseGroupLimit.manager_role[iManRoleId].participant.first_name;
                    tmpParticipant.substituted_by = modifPurchaseGroupLimit.manager_role[iManRoleId].participant.substituted_by;
                    tmpParticipant.substituted_until = modifPurchaseGroupLimit.manager_role[iManRoleId].participant.substituted_until;
                    tmpManagerRole.participant = tmpParticipant;
                        
                    origPurchaseGroupLimit.manager_role.push(tmpManagerRole);
                }
            }
        }


        private moveLimitUp(limitId: number) : void {
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
        }

        private moveLimitDown(limitId : number) : void {
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
        }

        private deleteAppManager(limitid: number, sindex: number) {
            var limit = this.$filter("filter")(this.editPurchaseGroup.purchase_group_limit, { limit_id: limitid }, true);
            
            limit[0].manager_role.splice(sindex, 1);

            if (limit[0].manager_role.length === 0) {
                limit[0].is_app_man_selected = false;
            }
        }         

        private appManSelectedItemChange(item: Participant, limitItemId: number) {
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
                var tmpManagerRole: ManagerRole = new ManagerRole();
                //tmpManagerRole.purchase_group_limit_id: limitItemId;
                tmpManagerRole.participant_id = item.id;
                tmpManagerRole.approve_level_id = limit[0].app_level_id;

                var tmpParticipant: Participant = new Participant();
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

        }

        private hideAppManAutoCompl(limitId: number) : void {

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
        }

        private toggleAppLimit(limitId: number): void {

            var divLimitContent = 'divLimitContent_' + limitId;
            var imgLimitCollExp = 'imgLimitCollExp_' + limitId;

            if ($("#" + divLimitContent).is(':visible')) {
                $("#" + divLimitContent).slideUp();
                $("#" + imgLimitCollExp).attr("src", this.getRegetRootUrl() + "Content/Images/Panel/Expand.png");
                $("#" + imgLimitCollExp).attr("title", this.displayText);
            } else {
                $("#" + divLimitContent).show("slow");
                $("#" + imgLimitCollExp).attr("src", this.getRegetRootUrl() + "Content/Images/Panel/Collapse.png");
                $("#" + imgLimitCollExp).attr("title", this.hideText);
            }
        }

        private addAppLimit(limitId: number) : void {

            for (var iLimitId: number = 0; iLimitId < this.editPurchaseGroup.purchase_group_limit.length; iLimitId++) {
                if (!(this.editPurchaseGroup.purchase_group_limit[iLimitId].is_visible)) {
                    this.clearAppLimit(this.editPurchaseGroup.purchase_group_limit[iLimitId]);
                    this.editPurchaseGroup.purchase_group_limit[iLimitId].is_visible = true;
                    this.editPurchaseGroup.purchase_group_limit[iLimitId].is_last = true;

                    if (iLimitId > 0) {
                        this.editPurchaseGroup.purchase_group_limit[iLimitId - 1].is_last = false;
                    } else {
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
        }

        private showAppLimit(limitId: number) : void {
            var divLimitContent: string = 'divLimitContent_' + limitId;
            var imgLimitCollExp: string = 'imgLimitCollExp_' + limitId;

            $("#" + divLimitContent).show("slow");
            $("#" + imgLimitCollExp).attr("src", this.getRegetRootUrl() + "Content/Images/Panel/Collapse.png");
            $("#" + imgLimitCollExp).attr("title", this.hideText);
        }

        private displayRequestorDetail(userId: number, centreId: number) : void {
            this.displayElement("btnCollapseReq_" + userId + "_" + centreId);
            this.hideElement("btnExpandReq_" + userId + "_" + centreId);
            this.displayElementSlow("divReq_" + userId + "_" + centreId);
        }

        private hideRequestorDetail(userId: number, centreId: number) : void {
            this.hideElement("btnCollapseReq_" + userId + "_" + centreId);
            this.displayElement("btnExpandReq_" + userId + "_" + centreId);
            this.hideElementSlow("divReq_" + userId + "_" + centreId);
        }

        private displayOrdererDetail(userId: number) : void {
            this.displayElement("btnCollapseOrd_" + userId);
            this.hideElement("btnExpandOrd_" + userId);
            this.displayElementSlow("divOrd_" + userId);
        }

        private hideOrdererDetail(userId: number) : void {
            this.hideElement("btnCollapseOrd_" + userId);
            this.displayElement("btnExpandOrd_" + userId);
            this.hideElementSlow("divOrd_" + userId);
        }

        private checkAllRequestorToggle(iRequestorId: number, iCentreId: number) : void {
            var requestor: Requestor[] = this.$filter("filter")(this.editPurchaseGroup.requestor, { participant_id: iRequestorId, centre_id: iCentreId }, true);

            if (!this.isValueNullOrUndefined(requestor[0])) {
                if (requestor[0].is_all) {
                    requestor[0].is_all = false;
                } else {
                    requestor[0].is_all = true;
                }
            }

        }

        private checkAllOrdererToggle(iOrdererId: number): void {
            var orderer: Orderer[] = this.$filter("filter")(this.editPurchaseGroup.orderer, { participant_id: iOrdererId }, true);

            if (!this.isValueNullOrUndefined(orderer[0])) {
                if (orderer[0].is_all) {
                    orderer[0].is_all = false;
                } else {
                    orderer[0].is_all = true;
                }
            }

        }

        private requestorSelectedItemChange(item: Participant, centreId: number) {
            if (this.isValueNullOrUndefined(item)) {
                return;
            }
            
            var requestor: Requestor[] = this.$filter("filter")(this.editPurchaseGroup.requestor, { participant_id: item.id, centre_id: centreId }, true);

            if (this.isValueNullOrUndefined(requestor[0])) {
                var tmpRequestor: Requestor = new Requestor();
                tmpRequestor.participant_id = item.id;
                tmpRequestor.surname = item.surname;
                tmpRequestor.first_name = item.first_name;
                tmpRequestor.centre_id = centreId;
                tmpRequestor.is_all = false;

                this.editPurchaseGroup.requestor.push(tmpRequestor);
            }

            this.hideRequestorAutoCompl(centreId);
        }

        private checkSelfOrdered() : void {
            this.editPurchaseGroup.orderer = [];
            this.hideOrdererAutoCompl();
        }

        private ordererSelectedItemChange(item: Participant) : void {
            if (this.isValueNullOrUndefined(item)) {
                return;
            }


            var orderer: Orderer[] = this.$filter("filter")(this.editPurchaseGroup.orderer, { participant_id: item.id }, true);

            if (this.isValueNullOrUndefined(orderer[0])) {
                var tmpOrderer: Orderer = new Requestor();
                tmpOrderer.participant_id = item.id;
                tmpOrderer.surname = item.surname;
                tmpOrderer.first_name = item.first_name;
                tmpOrderer.substituted_by = item.substituted_by;
                tmpOrderer.substituted_until = item.substituted_until;

                this.editPurchaseGroup.orderer.push(tmpOrderer);
            }

            this.hideOrdererAutoCompl();
        }

        private defaultSupplierSelectedItemChange(item: Supplier) : void {
            if (!this.isValueNullOrUndefined(this.editPurchaseGroup)) {
                this.editPurchaseGroup.default_supplier = item;
            }

        }      

        private updateLocalTexts(editPurchaseGroup: PurchaseGroup, origPurchaseGroup: PurchaseGroup) : void {
            for (var i = 0; i < editPurchaseGroup.local_text.length; i++) {
                origPurchaseGroup.local_text[i].text = editPurchaseGroup.local_text[i].text;
            }
        }

        private updateAppMatrix() : void {

            for (var iLimitId = 0; iLimitId < this.editPurchaseGroup.purchase_group_limit.length; iLimitId++) {
                var purchaseGroupLimitOrig = this.$filter("filter")(this.purchaseGroupOrig.purchase_group_limit, { limit_id: this.editPurchaseGroup.purchase_group_limit[iLimitId].limit_id }, true);
                this.updateAppMatrixUpdateLimit(this.editPurchaseGroup.purchase_group_limit[iLimitId], purchaseGroupLimitOrig[0]);
            }

        }
                
       
        private updateRequestors() : void {
            this.deleteRequestorsOrderers(
                this.editPurchaseGroup,
                this.purchaseGroupOrig.requestor,
                this.editPurchaseGroup.requestor,
                this.deleteAllRequestorList,
                true);

            this.newRequestorsOrderers(
                this.purchaseGroupOrig.requestor,
                this.editPurchaseGroup.requestor,
                true);

            this.addAllRequestors();
        }

        private updateOrderers(): void {
            this.deleteRequestorsOrderers(
                this.editPurchaseGroup,
                this.purchaseGroupOrig.orderer,
                this.editPurchaseGroup.orderer,
                this.deleteAllOrdererList,
                false);

            this.newRequestorsOrderers(
                this.purchaseGroupOrig.orderer,
                this.editPurchaseGroup.orderer,
                false);
            
        }   

        private deleteRequestorsOrderers(
            editPurchaseGroup : PurchaseGroup,
            origRequestorsOrderers: RequestorOrderer[],
            modifRequestorOrderers: RequestorOrderer[],
            deleteAllRequestorOrderers: RequestorOrderer[],
            isRequestor: boolean) : void {

            //Delete requestors
            for (var i: number = origRequestorsOrderers.length - 1; i >= 0; i--) {
                var modifParticipant: RequestorOrderer[] = null;

                if (isRequestor) {
                    modifParticipant = this.$filter("filter")(modifRequestorOrderers, { participant_id: origRequestorsOrderers[i].participant_id, centre_id: origRequestorsOrderers[i].centre_id }, true);
                } else {
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
                            var delParticipant: Requestor[] = this.$filter("filter")(this.purchaseGroupList[i].requestor, { participant_id: deleteAllRequestorOrderers[j].participant_id, centre_id: deleteAllRequestorOrderers[j].centre_id }, true);
                            if (!this.isValueNullOrUndefined(delParticipant[0])) {
                                var index = this.purchaseGroupList[i].requestor.indexOf(delParticipant[0]);
                                this.purchaseGroupList[i].requestor.splice(index, 1);
                            }
                        } else {
                            var delParticipant = this.$filter("filter")(this.purchaseGroupList[i].orderer, { participant_id: deleteAllRequestorOrderers[j].participant_id }, true);
                            if (!this.isValueNullOrUndefined(delParticipant[0])) {
                                var index = this.purchaseGroupList[i].orderer.indexOf(delParticipant[0]);

                                this.purchaseGroupList[i].orderer.splice(index, 1);
                            }
                        }
                    }

                    if (isRequestor) {
                        editPurchaseGroup.delete_requestors_all_categories.push({ participant_id: deleteAllRequestorOrderers[j].participant_id, centre_id: deleteAllRequestorOrderers[j].centre_id });
                    } else {
                        editPurchaseGroup.delete_orderers_all_categories.push(deleteAllRequestorOrderers[j].participant_id);
                    }
                }

                deleteAllRequestorOrderers = null;
            }
        }

        private newRequestorsOrderers(
            origRequestorsOrderers: RequestorOrderer[],
            modifRequestorOrderers: RequestorOrderer[],
            isRequestor: boolean): void  {
            //New Requestors
            for (var i = 0; i < modifRequestorOrderers.length; i++) {
                var origParticipant: RequestorOrderer[] = null;
                if (isRequestor) {
                    origParticipant = this.$filter("filter")(origRequestorsOrderers, { participant_id: modifRequestorOrderers[i].participant_id, centre_id: modifRequestorOrderers[i].centre_id }, true);
                } else {
                    origParticipant = this.$filter("filter")(origRequestorsOrderers, { participant_id: modifRequestorOrderers[i].participant_id }, true);
                }
                if (this.isValueNullOrUndefined(origParticipant[0])) {
                    //new
                    var tmpRequestorOrderer: RequestorOrderer = new RequestorOrderer();
                    tmpRequestorOrderer.participant_id = modifRequestorOrderers[i].participant_id;
                    tmpRequestorOrderer.surname = modifRequestorOrderers[i].surname;
                    tmpRequestorOrderer.first_name = modifRequestorOrderers[i].first_name;
                    tmpRequestorOrderer.centre_id = modifRequestorOrderers[i].centre_id;
                    if (isRequestor) {
                        origRequestorsOrderers.push(tmpRequestorOrderer);
                    } else {
                        origRequestorsOrderers.push(tmpRequestorOrderer);
                    }


                    if (modifRequestorOrderers[i].is_all) {
                        for (var j: number = 0; j < this.purchaseGroupList.length; j++) {
                            if (this.editPurchaseGroup.id === this.purchaseGroupList[j].id) {
                                continue;
                            }
                            if (isRequestor) {
                                var modifParticipant: RequestorOrderer[] = this.$filter("filter")(this.purchaseGroupList[j].requestor, { participant_id: modifRequestorOrderers[i].participant_id }, true);
                                if (this.isValueNullOrUndefined(modifParticipant[0])) {
                                    var tmpRequestor: Requestor = new Requestor();
                                    tmpRequestor.participant_id = modifRequestorOrderers[i].participant_id;
                                    tmpRequestor.surname = modifRequestorOrderers[i].surname;
                                    tmpRequestor.first_name = modifRequestorOrderers[i].first_name;
                                    tmpRequestor.centre_id = modifRequestorOrderers[i].centre_id;

                                    this.purchaseGroupList[j].requestor.push(tmpRequestor);
                                }
                            } else {
                                var modifParticipant: RequestorOrderer[] = this.$filter("filter")(this.purchaseGroupList[j].orderer, { participant_id: modifRequestorOrderers[i].participant_id }, true);
                                if (this.isValueNullOrUndefined(modifParticipant[0])) {
                                    var tmpOrderer: Orderer = new Orderer();
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
        }

        private addAllRequestors() : void {
            var allParticipants = this.$filter("filter")(this.editPurchaseGroup.requestor, { is_all: true }, true);

            for (var i: number = 0; i < allParticipants.length; i++) {
                for (var j: number = 0; j < this.purchaseGroupList.length; j++) {
                    var origParticipant: Requestor[] = this.$filter("filter")(this.purchaseGroupList[j].requestor, { participant_id: allParticipants[i].participant_id, centre_id: allParticipants[i].centre_id }, true);
                    if (this.isValueNullOrUndefined(origParticipant[0])) {
                        var tmpRequestor: Requestor = new Requestor();
                        tmpRequestor.participant_id = allParticipants[i].participant_id;
                        tmpRequestor.first_name = allParticipants[i].first_name;
                        tmpRequestor.surname = allParticipants[i].surname;
                        tmpRequestor.centre_id = allParticipants[i].centre_id;

                        this.purchaseGroupList[j].requestor.push(tmpRequestor);
                    }
                }
            }

           
        }

        private getPurchaseGroupHeaderBkgColor(pg : PurchaseGroup): string {
            //ng - class="pgItem.is_active ? 'reget-box-header reget-box-header-level1' : 'reget-box-header reget-box-header-level1-disabled'"

            if (pg.is_active === false) {
                return "reget-box-header reget-box-header-level1-disabled";
            }

            //if (pg.is_order_needed === false) {
            //    return "reget-box-header reget-box-header-level1-approvalonly"
            //}
            
            return "reget-box-header reget-box-header-level1";
        }
            
        //***********************************************************************************************************

        //***************************************************************
    }

    angular.
        module('RegetApp').
        controller('CgAdminController', Kamsyk.RegetApp.CgAdminController);

    
}

