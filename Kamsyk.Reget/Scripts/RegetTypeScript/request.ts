/// <reference path="../RegetTypeScript/Base/reget-base.ts" />
/// <reference path="../RegetTypeScript/Base/reget-common.ts" />
/// <reference path="../RegetTypeScript/Base/reget-entity.ts" />
/// <reference path="../RegetTypeScript/Base/reget-base-grid.ts" />
/// <reference path="../RegetTypeScript/Base/reget-data-convert.ts" />

/// <reference path="../typings/ui-grid/ui-grid.d.ts" />
/// <reference path="../typings/ng-file-upload/ng-file-upload.d.ts" />

module Kamsyk.RegetApp {
    
    let app = angular.module('RegetApp', ['ngMaterial', 'ngMessages', 'ngFileUpload', 'ui.grid', 'ui.grid.pagination', 'ui.grid.resizeColumns', 'ui.grid.selection', 'ui.grid.moveColumns', 'ui.grid.edit']);
       
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
       
    export class RequestController extends BaseRegetGridTs implements angular.IController {
        //**********************************************************
        //Properties
        public request: Request = null;
       
        private requestorCentres: CentreReqDropDownItem[] = null;
        private purchaseGroupsCmb: PgReqDropDownExtend[] = null;
        private purchaseGroups: PurchaseGroup[] = null;
        private currencyList: DropDownExtend[] = null;
        private itemCurrencyList: DropDownExtend[] = null;
        private currencyListGrid: AgDropDown[] = null;
        private unitList: DropDownExtend[] = null;
        private unitListGrid: AgDropDown[] = null;
        private privacyList: AgDropDown[] = null;
        private exchangeRates: ExchangeRate[] = null;
        private shipToAddress: Address[] = null;

        private isRequestorCentreLoaded: boolean = false;
        private isPgsLoaded: boolean = false;
        private isCurrenciesLoaded: boolean = false;
        private isUnitsLoaded: boolean = false;
        private isPrivacyLoaded: boolean = false;
        private isRequestLoaded: boolean = false;
        private isExchangeRateLoaded: boolean = false;
        private isShipToAddressLoaded: boolean = false;
        private isDiscussionLoaded: boolean = false;

        private isNewRequest: boolean = false;
        private isNewRequestOrDraft: boolean = false;
        private isRequestorCentreReadOnly: boolean = true;

        private iRequestId: number = null;
        private selectedCentreId: number = null;
        private cgId: number = null;
        private pgId: number = null;
        private currencyId: number = null;
        private privacyId: number = null;
        private supplierId: number = null;
        private requestText: string = null;
        private supplierText: string = null;
        private requestRemark: string = null;
        private requestNr: string = null;
        private requestorName: string = null;

        private selectedCmbPg: PgReqDropDownExtend = null;
        private selectedPg: PurchaseGroup = null;
        private selectedOrdererId: number = null;
        private selectedOrdererName: string = null;
        private selectedSupplier: Supplier = null;
        private selectedDeliveryDate: Date = null;
        private strSelectedSupplier: string = null;
        private searchstringsupplier: string = null;
        private centreName: string = null;    
        private pgName: string = null;  
        private selectedShipToAddress: Address = null;
                
        private currencyCode: string = null; 
        private itemCurrencyCode: string = null;
        private unitCode: string = null; 

        //private currentUserName: string = $("#txtCurrentUserName").val();
        //private currentUserId: number = parseInt($("#txtCurrentUserId").val());
        //private currentUserPhotoUrl: string = $("#txtCurrentUserPhotoUrl").val();
        
        private pgType: number = PurchaseGroupType.Unknow;
        private isPgItemOrder: boolean = false;
        private isPgStandard: boolean = false;
        

        private requestItemName: string = null;
        private requestItemAmount: string = null;
        private requestItemUnitPrice: string = null;
        private requestItemCurrencyId: number = null;
        private requestItemUnitId: number = null;

        private isCurrencyRO: boolean = false;
        private errRequestItem: string = null;
        private isRequestItemsDisplayed: boolean = false;
        private isFiltersSet: boolean = false;
        //private progressPercentage: number = null;
        //private isUploadFileProgressBarVisible: boolean = false;
        private centre_currency_id: number = null;
        private is_approval_needed: boolean = false;
        private is_order_needed: boolean = false;
        private is_self_ordered = false;
        private is_multi_orderer_cmb_dispalyed: boolean = false;
        private shipAddressIsOnlyOne: boolean = false;
        private isCurrencyOnlyOne: boolean = false;
        private isCustomFieldVisible: boolean = false;
        private isPrivacyReadOnly: boolean = true;
        private isCustomFieldReadOnly: boolean = false;
        private privacyName: string = null;
       
        private customFields: CustomField[] = null;

        //private tmpContractNr: string = "ssdfdf";
        //private custFieldValues: any[] = null;
        private requestAppMen: ManagerRole[] = null
       // private pgLimits: PurchaseGroupLimit[] = null;
        private requestPgLimits: PurchaseGroupLimit[] = null;
        private orderers: Orderer[] = null
        //private isMandatoryCheck: boolean = false;
        private isKeepErrMsgHidden: boolean = true;
        private requestErrorMsg: string = null;
        private isAppManOrdererDisplayed: boolean = false;
        private isAttachmentDisplayed: boolean = true;
        private isAttachmentEditable: boolean = false;
        private isAddressDisplayed: boolean = true;
        private requestDiscussion: Discussion[] = null;
        private isDiscussionVisible: boolean = false;
        private isSendForApprovalVisible: boolean = true;
        private isRevertVisible: boolean = false;
        private isDeleteVisible: boolean = false;

        private newRemarkItem: string = null;
        private discussion: Discussion = null;
        private remindText: string = null;
        private remindConfirmText: string = null;
        private isWorkflowDisplayed: boolean = false;
        private isActivatedDisplayed: boolean = false;
        private isRequestCanceled: boolean = false;
        private isRequestRejected: boolean = false;
        private isFreeSupplierAllowed: boolean = false;
        private isFreeSupplier: boolean = false;
        private accessRequest: Request_AccessRequest = null;
        private isAccessRequestSent: boolean = false;
        private isAccessRequestSentWaitForApproval: boolean = false;
        private isAccessRequestSentRejected: boolean = false;
        private strAccessRequestDate: string = null;
        private isWorkflowRemindDisplayed: boolean = false;
        private isApprovable: boolean = false;
        private isGenerateOrderAvailable: boolean = false;
        private appStatusApproved: number = ApprovalStatus.Approved;
        private appStatusRejected: number = ApprovalStatus.Rejected;
        //private imgStatusUrl: string = null;
        //**********************************************************

        //***************************************************************************
        //Localization
        private locMissingRequestItemNameText: string = $("#MissingRequestItemNameText").val();
        private locMissingRequestAmountText: string = $("#MissingRequestAmountText").val();
        private locMissingRequestUnitText: string = $("#MissingRequestUnitText").val();
        private locMissingRequestPriceText: string = $("#MissingRequestPriceText").val();
        private locMissingRequestCurrencyText: string = $("#MissingRequestCurrencyText").val();
        private locMissingRequestText: string = $("#MissingRequestText").val();
        private locItemNameText: string = $("#ItemNameText").val();
        private locAmountText: string = $("#AmountText").val();
        private locUnitText: string = $("#UnitText").val();
        private locUnitPriceText: string = $("#UnitPriceText").val();
        private locCurrencyText: string = $("#CurrencyText").val();
        private locNotAuthorizedPerformActionText: string = $("#NotAuthorizedPerformActionText").val();
        private locExchangeRateMissingText: string = $("#ExchangeRateMissingText").val();
        private locSelectCentreText: string = $("#SelectCentreText").val();
        //private ĺocFileSizeOverLimitText: string = $("#FileSizeOverLimitText").val();
        private locEnterMandatoryValuesText: string = $("#EnterMandatoryValuesText").val();
        private locRevertText: string = $("#RevertText").val();
        private locRevertRequestConfirmText: string = $("#RevertRequestConfirmText").val();
        private locRequestWasRevertedText: string = $("#RequestWasRevertedText").val();
        private locRemindAppManText: string = $("#RemindAppManText").val();
        private locRemindOrdererText: string = $("#RemindOrdererText").val();
        private locResendApprovalRequestConfirmText: string = $("#ResendApprovalRequestConfirmText").val();
        private locResendOrderRequestConfirmText: string = $("#ResendOrderRequestConfirmText").val();
        private locSendText: string = $("#SendText").val();
        private locSentText: string = $("#SentText").val();
        private locCanceledConfirmText: string = $("#CanceledConfirmText").val();
        private locCanceledByRequestorText: string = $("#CanceledByRequestorText").val();
        private locActivateConfirmText: string = $("#ActivateConfirmText").val();
        private locFreeSupplierText: string = $("#FreeSupplierText").val();
        private locAskAcessRequestConfirmText: string = $("#AskAcessRequestConfirmText").val();
        private locAccessRequestWasSentText: string = $("#AccessRequestWasSentText").val();
        private locApproveConfirmText: string = $("#ApproveConfirmText").val();
        private locRejectConfirmText: string = $("#RejectConfirmText").val();
        private locApprovedText: string = $("#ApprovedText").val();
        private locRejectedText: string = $("#RejectedText").val();
        
        //*****************************************************************************

        ////************************************************************
        ////Other Texts
        //private dateTimeMomentFormatText = $("#DateTimeMomentFormatText").val();
        ////************************************************************

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
            protected $timeout: ng.ITimeoutService,
            protected Upload: ng.angularFileUpload.IUploadService,
            protected $compile: ng.ICompileService
            
        ) {
            super($scope, $http, $filter, $mdDialog, $mdToast, $q, uiGridConstants, $timeout);

            //there must be - let app = angular.module('RegetApp', ['ngFileUpload' .. in scrit wher upload file is used e.g. request.ts
            this.fileUploadInit($scope, this.Upload);

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
           
            this.loadInit();

        }
        

       
        //***************************************************************

        $onInit = () => { };

        //******************************************************************
        //Methods

        //************************************************************************
        //Overide
        public cellClicked(row?: any, col?: any): void {
            if (this.isFiltersSet === false) {
                this.setGridColumFilters();
            }
            super.cellClicked(row, col);
        }

        public gridSaveRow(): boolean {
            if (this.isValueNullOrUndefined(this.request.id)) {
                this.editRowChanged(false);
                this.setEstPrice();
                return true;
            } else {
                return super.gridSaveRow();
            }
        }
        //************************************************************************

        //****************************************************************************
        //Abstract
        public exportToXlsUrl(): string {
            return this.getRegetRootUrl() + "Report/GetRequestItemsReport?" +
                "filter=" + encodeURI(this.filterUrl) +
                "&sort=" + this.sortColumnsUrl +
                "&t=" + new Date().getTime();
        }

        public getControlColumnsCount(): number {
            return 2;
        }

        public getDuplicityErrMsg(centre: Centre): string {
            if (this.isNewRequest) {
                return null;
            }
            return null;//this.getLocDuplicityCentreNameText().replace("{0}", centre.name);
        }

        public getSaveRowUrl(): string {
            return null;//this.getRegetRootUrl() + "Centre/SaveCentreData?t=" + new Date().getTime();
        }

        public insertRow(): void {           
        }

        public isRowChanged(): boolean {

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
        }

        public isRowEntityValid(centre: CentreAdminExtended): string {

            if (this.isStringValueNullOrEmpty(centre.name)) {
                return this.locMissingMandatoryText;
            }

            return null;
        }

        public loadGridData(): void {
            //this.getCentresData();
        }

        public dbGridId: string = "grdRequestItems_rg";

        public getMsgDisabled(centre: Centre): string {
            return null;//this.locCentreWasDisabledText.replace("{0}", centre.name);
        }


        public getMsgDeleteConfirm(centre: CentreAdminExtended): string {
            return null;// this.locDeleteCentreConfirmText.replace("{0}", centre.name);;
        }

        public getErrorMsgByErrId(errId: number, msg: string): string {
            return this.locErrorMsgText;
        }

        public deleteUrl: string = null;//this.getRegetRootUrl() + "Centre/DeleteCentre" + "?t=" + new Date().getTime();

        public getDbGridId(): string {
            return this.dbGridId;
        }
        //*****************************************************************************

        //******************************************************************
        //Http

        private getRequestCentres(): void {
            
            this.showLoader(this.isError);

            this.$http.get(
                this.getRegetRootUrl() + "Request/GetRequestCentres?t=" + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    this.requestorCentres = tmpData;
                    this.populateCentre();
                    //this.loadUnits();

                    this.isRequestorCentreLoaded = true;
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

        private loadPgs(): void {
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

            let requestId = -1;
            if (!this.isValueNullOrUndefined(this.request)) {
                requestId = this.request.id;
            }

            this.$http.get(
                this.getRegetRootUrl() + "Request/GetPurchaseGroups?centreId=" + this.selectedCentreId
                + "&requestId=" + requestId
                + "&t=" + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    this.purchaseGroups = tmpData;
                                                            
                    this.loadCurrencies();

                    this.setPurchaseGroupCmbList();

                    this.selectedPg = null;
                    this.requestAppMen = null;
                    this.orderers = null;
                    this.is_approval_needed = false;
                    this.is_order_needed = false;
                    //this.isAppManOrdererDisplayed = false;

                    this.isPgsLoaded = true;
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


            let tmpCentre = this.$filter("filter")(this.requestorCentres, { id: (this.selectedCentreId) }, true);
            if (!this.isValueNullOrUndefined(tmpCentre[0]) && tmpCentre[0].is_free_supplier_allowed) {
                if (this.isNewRequestOrDraft) {
                    this.isFreeSupplierAllowed = true;
                } else {
                    this.isFreeSupplierAllowed = (this.request.use_supplier_list);
                }
            } else {
                this.isFreeSupplierAllowed = false;
            }
        }

        private loadCurrencies(): void {
            this.showLoader(this.isError);

            //this.currencyId = null;

            this.$http.get(
                this.getRegetRootUrl() + "Request/GetCurrencies?centreId=" + this.selectedCentreId + "&t=" + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    this.currencyList = tmpData.currency_drop_down;
                    this.centre_currency_id = tmpData.default_currency_id;

                    this.currencyListGrid = [];
                    this.itemCurrencyList = [];

                    if (!this.isValueNullOrUndefined(this.currencyList)) {
                        for (let i = 0; i < this.currencyList.length; i++) {
                            let grdDropDowmItem: AgDropDown = {
                                label: this.currencyList[i].name,
                                id: this.currencyList[i].name,
                                value: this.currencyList[i].name
                            };

                            let cmbCurrencyItem: DropDownExtend = {
                                id: this.currencyList[i].id,
                                name: this.currencyList[i].name,
                                is_disabled: false
                            };

                            this.itemCurrencyList.push(cmbCurrencyItem);
                            this.currencyListGrid.push(grdDropDowmItem);
                        }
                    }

                    if (!this.isValueNullOrUndefined(this.itemCurrencyList) && this.itemCurrencyList.length === 1) {
                        this.isCurrencyOnlyOne = true;
                        this.currencyId = this.itemCurrencyList[0].id;
                        this.currencyCode = this.itemCurrencyList[0].name;
                    } else {
                        this.isCurrencyOnlyOne = false;
                    }
                    

                    this.isCurrenciesLoaded = true;
                    this.loadExchangeRates();

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

        private loadExchangeRates(): void {
            this.showLoader(this.isError);

            this.$http.get(
                this.getRegetRootUrl() + "Request/GetExchangeRates?t=" + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    this.exchangeRates = tmpData;

                    this.loadUnits();

                    this.isExchangeRateLoaded = true;
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

        private loadUnits(): void {
            this.showLoader(this.isError);

            this.$http.get(
                this.getRegetRootUrl() + "Request/GetUnitsOfMeasurement?t=" + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    this.unitList = tmpData;

                    this.unitListGrid = [];
                    for (let i = 0; i < this.unitList.length; i++) {
                        let grdDropDowmItem: AgDropDown = {
                            label: this.unitList[i].name,
                            id: this.unitList[i].name,
                            value: this.unitList[i].name
                        };

                        this.unitListGrid.push(grdDropDowmItem);
                    }
                    
                    this.loadPrivacies(false);
                    this.isUnitsLoaded = true;
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

        private loadPrivacies(isPrivacyOnly : boolean): void {
            this.showLoader(this.isError);

            this.$http.get(
                this.getRegetRootUrl() + "Request/GetPrivacyItems?t=" + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    this.privacyList = tmpData;

                    if (isPrivacyOnly) {
                        this.isPrivacyLoaded = true;
                        return;
                    }
                                        
                    this.loadShipToAddress();

                    this.isPrivacyLoaded = true;
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

        private loadShipToAddress(): void {
            this.showLoader(this.isError);

            this.$http.get(
                this.getRegetRootUrl() + "Request/GetRequestorAddress?centreId=" + this.selectedCentreId + "&t=" + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    this.shipToAddress = tmpData;

                    this.selectPg();

                    if (!this.isValueNullOrUndefined(this.shipToAddress) && this.shipToAddress.length === 1) {
                        this.shipAddressIsOnlyOne = true;
                        
                    } else {
                        this.shipAddressIsOnlyOne = false;
                        this.tickAddress();
                    }
                                        
                    this.isShipToAddressLoaded = true;
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
                
        private deleteAttachment(id: number): void {

            if (this.isNewRequest === false) {
                this.showLoader(this.isError);
            }

            var jsonId = JSON.stringify({ id: id });
            this.$http.post(
                this.getRegetRootUrl() + "Attachment/DeleteAttachment?t=" + new Date().getTime(),
                jsonId
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    let httpResult: HttpResult = tmpData;

                    this.hideLoaderWrapper();

                    if (this.isValueNullOrUndefined(httpResult.error_id) || httpResult.error_id === 0) {
                        //Deleted
                        for (var i: number = this.request.attachments.length - 1; i >= 0; i--) {
                            if (this.request.attachments[i].id === id) {
                                this.request.attachments.splice(i, 1);
                                break;
                            }
                        }
                       
                    } else {
                        if (httpResult.error_id == 110) {
                            //Not Auhorized
                            this.displayErrorMsg(this.locNotAuthorizedPerformActionText);
                        } else {
                            this.displayErrorMsg();
                        }
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

        }       

       

        private saveRequestDraft(): void {
            this.showLoaderBoxOnly(this.isError);

            var jsonRequest = JSON.stringify(this.request);
                        
            this.$http.post(
                this.getRegetRootUrl() + "Request/SaveRequestDraft?t=" + new Date().getTime(),
                jsonRequest
            ).then((response) => {
                try {
                    let result: any = response.data;
                    let strResult = result.string_value;
                    let requestId = result.request_id;
                    let requestNr = result.request_nr;
                    if (this.isStringValueNullOrEmpty(strResult)) {
                        this.isNewRequest = false;
                        this.request.id = requestId;
                        this.request.request_nr = requestNr;
                        this.showToast(this.locDataWasSavedText);
                    } else {
                        let msgError = this.getErrorMessage(strResult);
                        this.showAlert(this.locErrorTitleText, msgError, this.locCloseText);

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
        }

        private sendForApprovalPost(): void {
            this.showLoaderBoxOnly(this.isError);

            var jsonRequest = JSON.stringify(this.request);

            this.$http.post(
                this.getRegetRootUrl() + "Request/SendForApproval?t=" + new Date().getTime(),
                jsonRequest
            ).then((response) => {
                try {
                    let result: any = response.data;
                    let strResult: string = result.string_value;
                    if (this.isStringValueNullOrEmpty(strResult)) {
                        this.showToast(this.locDataWasSavedText);
                        let requestId: number = result.request_id;
                        window.location.href = this.getRegetRootUrl() + "Request?id=" + requestId;
                    } else {
                        let msgError = this.getErrorMessage(strResult);
                        if (msgError.trim().length == 0) {
                            msgError = null;
                        }
                        //this.showAlert(this.locErrorTitleText, msgError, this.locCloseText);
                        this.displayErrorMsg(msgError);
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
        }

        private loadRequest(): void {
            this.showLoaderBoxOnly(this.isError);
                        
            this.$http.get(
                this.getRegetRootUrl() + "Request/GetRequest?requestId=" + this.iRequestId + "&t=" + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    let result: any = response.data;
                                       
                    if (!this.getRequestLoadedStatus(result)) {
                        return;
                    }

                    this.request = result;
                    this.requestNr = this.request.request_nr;

                    this.selectedCentreId = this.request.request_centre_id;
                    this.centreName = this.request.centre_name;

                    this.pgId = this.request.purchase_group_id;
                    this.pgName = this.request.pg_name;

                    this.currencyId = this.request.currency_id;
                    this.currencyCode = this.request.currency_code;

                    if (this.request.id < 0) {
                        this.isNewRequest = true;
                    }

                    if (this.request.id < 0 || this.request.request_status === RequestStatus.Draft) {
                        this.isNewRequestOrDraft = true;
                    } else {
                        this.isNewRequestOrDraft = false;
                    }

                    this.privacyId = this.request.privacy_id;
                    this.supplierId = this.request.supplier_id;
                    this.isFreeSupplier = !this.request.use_supplier_list;
                    this.supplierText = this.request.supplier_remark;
                    if (this.isFreeSupplier && !this.isNewRequestOrDraft) {
                        if (this.isStringValueNullOrEmpty(this.supplierText)) {
                            this.supplierText = this.locFreeSupplierText;
                        } else {
                            this.supplierText = this.locFreeSupplierText
                                + " \n" + this.supplierText;
                        }
                    }

                    this.selectedDeliveryDate = this.convertJsonDateToJs(this.request.lead_time);
                    this.requestText = this.request.request_text;
                    this.isPrivacyReadOnly = (this.request.requestor !== this.currentUserId && this.isNewRequest === false);
                    this.isCustomFieldReadOnly = (this.request.requestor !== this.currentUserId || this.isNewRequestOrDraft === false);

                    

                    this.requestorName = this.request.requestor_name_surname_first;

                    this.isRequestLoaded = true;
                    this.isDiscussionLoaded = true;
                    this.isPrivacyLoaded = true;
                    this.privacyName = this.request.privacy_name;
                    
                    if (this.isNewRequestOrDraft === true) {
                        this.getRequestCentres();
                    }

                    if (!this.isValueNullOrUndefined(this.supplierId) && this.supplierId > -1) {
                        let promSupp: ng.IPromise<Supplier> = this.filterSuppliersById(this.supplierId);
                        promSupp.then((supp: Supplier) => {
                            if (!this.isValueNullOrUndefined(supp)) {
                                this.selectedSupplier = supp;
                                this.strSelectedSupplier = this.selectedSupplier.supp_name;
                            }
                        }).catch(() => {
                            this.displayErrorMsg();
                        });
                        //this.selectedSupplier = promSupp;
                    }

                    if (!this.isValueNullOrUndefined(this.request)) {

                        if (this.request.is_approval_needed || this.request.is_order_needed) {
                            this.isAppManOrdererDisplayed = true;
                        } else if (!this.isValueNullOrUndefined(this.selectedPg)) {
                            if (this.selectedPg.is_order_needed || this.selectedPg.is_approval_needed) {
                                this.isAppManOrdererDisplayed = true;
                            }
                        } 
                                               
                        if (!this.isNewRequestOrDraft) {
                            if (!this.isValueNullOrUndefined(this.request.request_event_approval) && this.request.request_event_approval.length > 0) {
                                this.is_approval_needed = true;
                                this.requestAppMen = [];
                                for (let i: number = 0; i < this.request.request_event_approval.length; i++) {
                                    let manRole: ManagerRole = new ManagerRole();
                                    let part: Participant = new Participant();
                                    part.id = this.request.request_event_approval[i].app_man_id;
                                    part.surname = this.request.request_event_approval[i].app_man_surname;
                                    part.first_name = this.request.request_event_approval[i].app_man_first_name;
                                    manRole.participant = part;
                                    manRole.approve_level_id = this.request.request_event_approval[i].app_level_id;
                                    manRole.approve_status = this.request.request_event_approval[i].approve_status;
                                    manRole.modif_date_text =this.request.request_event_approval[i].modif_date_text;
                                    this.requestAppMen.push(manRole);
                                }
                            }

                            if (!this.isValueNullOrUndefined(this.request.orderers) && this.request.orderers.length > 0) {
                                this.is_order_needed = true;
                                this.orderers = [];
                                for (let i: number = 0; i < this.request.orderers.length; i++) {
                                    let orderer: Orderer = new Orderer();
                                    orderer.participant_id = this.request.orderers[i].participant_id;
                                    orderer.surname = this.request.orderers[i].surname;
                                    orderer.first_name = this.request.orderers[i].first_name;
                                    this.orderers.push(orderer);
                                }
                            }
                        }

                        this.setStatusImage();
                    }

                    this.isSendForApprovalVisible = this.isNewRequestOrDraft
                        && (this.isNewRequest || this.currentUserId === this.request.requestor);

                    this.isRevertVisible = this.request.is_revertable && !this.isCanceled();
                    this.isDeleteVisible = this.request.is_deletable;
                    this.isApprovable = this.request.is_approvable;
                    this.isGenerateOrderAvailable = this.request.is_order_available;

                    this.setAttachmetVisibility();

                    if (this.currentUserId == this.request.requestor) {
                        this.isPrivacyLoaded = false;
                        this.loadPrivacies(true);
                    }

                    if (!this.isNewRequestOrDraft) {
                        //load custom fields
                        this.getPurchaseGroup();
                    }

                    this.requestRemark = this.request.remarks;

                    this.isDiscussionVisible = !this.isNewRequestOrDraft;
                    if (this.isNewRequest === true) {
                        this.isDiscussionLoaded = true;
                    } else {
                        this.isDiscussionLoaded = false;
                        this.loadDiscussion();
                    } 

                    if (this.request.request_status == RequestStatus.WaitForApproval) {
                        this.remindText = this.locRemindAppManText;
                        this.remindConfirmText = this.locResendApprovalRequestConfirmText.replace('{0}', this.getCurrAppMan());
                    } else if (this.request.request_status == RequestStatus.Approved) {
                        this.remindText = this.locRemindOrdererText;
                        this.remindConfirmText = this.locResendOrderRequestConfirmText.replace('{0}', this.getCurrOrderer());
                    } else {
                        this.remindText = "";
                    }

                    this.isActivatedDisplayed = false;
                    if (this.request.request_status != RequestStatus.New
                        && this.request.request_status != RequestStatus.CanceledOrderer
                        && this.request.request_status != RequestStatus.CanceledRequestor
                        && this.request.request_status != RequestStatus.CanceledSystem
                    ) {
                        this.isWorkflowDisplayed = true;
                    } else {
                        this.isWorkflowDisplayed = false;
                        if (this.currentUserId == this.request.requestor) {
                            this.isActivatedDisplayed = true;
                        }
                    }

                    this.isWorkflowRemindDisplayed = this.isActivatedDisplayed && this.request.requestor == this.currentUserId;

                    this.isRequestCanceled = this.isCanceled();
                    this.isRequestRejected = (this.request.request_status == RequestStatus.Rejected);
                    this.isFreeSupplier = !this.request.use_supplier_list;

                    //this.setIsApprovable();
                                                            
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

        private approveRequestConfirmed(): void {
            this.showLoaderBoxOnly(this.isError);

            var jsonRequestId = JSON.stringify({ requestId: this.request.id });

            this.$http.post(
                this.getRegetRootUrl() + "Request/Approve?t=" + new Date().getTime(),
                jsonRequestId
            ).then((response) => {
                try {
                    let result: any = response.data;
                    let strResult: string = result.string_value;
                    if (this.isStringValueNullOrEmpty(strResult)) {
                        //this.showToast(this.locDataWasSavedText);
                        //let requestId: number = result.request_id;
                        //window.location.href = this.getRegetRootUrl() + "Request?id=" + this.request.id;
                        this.showToast(this.locApprovedText);
                        this.loadRequest();
                    } else {
                        let msgError = this.getErrorMessage(strResult);
                        if (msgError.trim().length == 0) {
                            msgError = null;
                        }
                        
                        this.displayErrorMsg(msgError);
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
        }

        private rejectRequestConfirmed(): void {
            this.showLoaderBoxOnly(this.isError);

            var jsonRequestId = JSON.stringify({ requestId: this.request.id });

            this.$http.post(
                this.getRegetRootUrl() + "Request/Reject?t=" + new Date().getTime(),
                jsonRequestId
            ).then((response) => {
                try {
                    let result: any = response.data;
                    let strResult: string = result.string_value;
                    if (this.isStringValueNullOrEmpty(strResult)) {
                        //this.showToast(this.locDataWasSavedText);
                        //let requestId: number = result.request_id;
                        //window.location.href = this.getRegetRootUrl() + "Request?id=" + this.request.id;
                        this.showToast(this.locRejectedText);
                        this.loadRequest();
                    } else {
                        let msgError = this.getErrorMessage(strResult);
                        if (msgError.trim().length == 0) {
                            msgError = null;
                        }

                        this.displayErrorMsg(msgError);
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
        }

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

        private sendAccessRequest(): void {
            this.showLoader(this.isError);

            //let iVersion: number = 0;
            //if (!this.isValueNullOrUndefined(this.request)) {
            //    iVersion = this.request.version;
            //}
            
            this.$http.get(
                this.getRegetRootUrl() + "Request/SendAccessRequest?requestId=" + this.iRequestId 
                //+ "&requestVersion = " + iVersion
                +"&t = " + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    let httpResult: any = response.data;
                    let strResult: string = httpResult.string_value;
                    if (this.isStringValueNullOrEmpty(strResult)) {
                        this.showToast(this.locAccessRequestWasSentText);
                        this.getRequestAccessRequests();
                    } else {
                        if (httpResult.error_id == 110) {
                            //Not Auhorized - 
                            this.displayErrorMsg(this.locNotAuthorizedPerformActionText);
                        } else {
                            this.displayErrorMsg();
                        }
                       
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

        private tickAddress() {
            if (this.isNewRequestOrDraft && !this.isNewRequest) {
                //set ship to address
                if (!this.isValueNullOrUndefined(this.shipToAddress)) {
                    for (let i: number = 0; i < this.shipToAddress.length; i++) {
                        let isTheSame: boolean = true;
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
        }

        private setStatusImage() {
            let imgStatusUrl = null;
            if (this.request.request_status == RequestStatus.Draft) {
                imgStatusUrl = this.getRegetRootUrl() + "Content/Images/Request/RequestStatus/Draft.png";
            } else if (this.request.request_status == RequestStatus.WaitForApproval) {
                imgStatusUrl = this.getRegetRootUrl() + "Content/Images/Request/RequestStatus/WaitForApproval.png";
            } else if (this.request.request_status == RequestStatus.Approved) {
                imgStatusUrl = this.getRegetRootUrl() + "Content/Images/Request/RequestStatus/Approved.png";
            } else if (this.request.request_status == RequestStatus.Rejected) {
                imgStatusUrl = this.getRegetRootUrl() + "Content/Images/Request/RequestStatus/Rejected.png";
            } else if (this.request.request_status == RequestStatus.CanceledRequestor
                || this.request.request_status == RequestStatus.CanceledOrderer
                || this.request.request_status == RequestStatus.CanceledSystem) {
                imgStatusUrl = this.getRegetRootUrl() + "Content/Images/Request/RequestStatus/Canceled.png";
            } 

            if (!this.isValueNullOrUndefined(imgStatusUrl)) {
                let imgStatus: HTMLImageElement = <HTMLImageElement>document.getElementById("imgHeader");
                imgStatus.src = imgStatusUrl;
                imgStatus.style.border = "1px solid #fff";
                imgStatus.style.borderRadius = "12px";
                imgStatus.style.height = "24px";
                imgStatus.style.width = "24px";

                //let divHeader: HTMLDivElement = <HTMLDivElement>document.getElementById("divHeader");
                //divHeader.style.backgroundImage = "url('" + imgStatusUrl + "')";
                //divHeader.style.border = "1px solid #fff";
                                
            }
        }

        private updatePrivacy(): void {
            this.showLoaderBoxOnly();
                       
            var jsonPrivacy = JSON.stringify({ requestId: this.request.id, privacyId: this.privacyId });
            this.$http.post(
                this.getRegetRootUrl() + "Request/SetPrivacy?t=" + new Date().getTime(),
                jsonPrivacy
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    let httpResult: HttpResult = tmpData;

                    this.hideLoaderWrapper();

                    if (!this.isValueNullOrUndefined(httpResult.error_id) && httpResult.error_id !== 0) {
                        if (httpResult.error_id === 110) {
                            //Not Auhorized
                            this.displayErrorMsg(this.locNotAuthorizedPerformActionText);
                        } else {
                            this.displayErrorMsg();
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

        public revertConfirmed() {
            this.showLoaderBoxOnly();

            var jsonRequestId = JSON.stringify({ requestId: this.request.id });
            this.$http.post(
                this.getRegetRootUrl() + "Request/Revert?t=" + new Date().getTime(),
                jsonRequestId
            ).then((response) => {
                try {
                    this.hideLoader();
                    let result: any = response.data;
                    let strResult: string = result.string_value;
                    if (this.isStringValueNullOrEmpty(strResult)) {
                        this.showToast(this.locRequestWasRevertedText);
                        this.loadRequest();
                    } else {
                        let msgError = this.getErrorMessage(strResult);
                        if (msgError.trim().length == 0) {
                            msgError = null;
                        }
                        this.displayErrorMsg(msgError);
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

        private getPurchaseGroup(): void {
            this.showLoaderBoxOnly(this.isError);

            this.$http.get(
                this.getRegetRootUrl() + "Request/GetPurchaseGroup?pgId=" + this.pgId
                + "&requestId=" + this.iRequestId
                + "&t=" + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpRes: any = response;
                    this.selectedPg = tmpRes.data;

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

        private getRequestAccessRequests(): void {
            this.showLoaderBoxOnly(this.isError);

            this.$http.get(
                this.getRegetRootUrl() + "Request/GetAccessRequest?requestId=" + this.iRequestId
                + "&t=" + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    let tmpRes: any = response;
                    this.accessRequest = tmpRes.data;

                    if (!this.isValueNullOrUndefined(this.accessRequest)) {
                        this.isAccessRequestSent = true;
                        
                        this.strAccessRequestDate = this.convertDateTimeToString(this.convertJsonDate(this.accessRequest.request_date).toDate());
                        if (this.accessRequest.status_id == ApprovalStatus.WaitForApproval) {
                            this.isAccessRequestSentWaitForApproval = true;
                        }
                        if (this.accessRequest.status_id == ApprovalStatus.Rejected) {
                            this.isAccessRequestSentRejected = true;
                        }
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
        }

        private loadDiscussion(): void {
            this.showLoaderBoxOnly(this.isError);

            this.$http.get(
                this.getRegetRootUrl() + "Discussion/GetRequestDiscussion?requestId=" + this.iRequestId + "&t=" + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpRes: any = response;
                    this.discussion = tmpRes.data;

                    if (!this.isValueNullOrUndefined(this.discussion)
                        && !this.isValueNullOrUndefined(this.discussion.discussion_items)
                        && this.discussion.discussion_items.length) {
                        this.isDiscussionVisible = true;
                    }

                    this.isDiscussionLoaded = true;
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

        private addRemark(): void {
            if (this.isStringValueNullOrEmpty(this.newRemarkItem)) {
                return;
            }

            this.showLoaderBoxOnly(this.isError);

            var jsonEntityData = JSON.stringify({ requestId: this.request.id, remark: this.newRemarkItem });

            this.$http.post(
                this.getRegetRootUrl() + "Discussion/AddRequestRemark?t=" + new Date().getTime(),
                jsonEntityData
            ).then((response) => {
                try {
                    var result: any = response.data;
                    var strResult = result.string_value;
                    if (this.isStringValueNullOrEmpty(strResult)) {

                        
                        this.addDiscussionItem(this.newRemarkItem, this.discussion);

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
                        this.newRemarkItem = "";

                    } else {
                        let msgError = this.getErrorMessage(strResult);
                        this.showAlert(this.locErrorTitleText, msgError, this.locCloseText);

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


        public cancelRequestConfirmed(): void {
            this.showLoaderBoxOnly(this.isError);

            var jsonEntityData = JSON.stringify({ requestId: this.request.id });

            this.$http.post(
                this.getRegetRootUrl() + "Request/CancelRequest?t=" + new Date().getTime(),
                jsonEntityData
            ).then((response) => {
                try {
                    var result: any = response.data;
                    var strResult = result.string_value;
                    if (this.isStringValueNullOrEmpty(strResult)) {
                        //this.isWorkflowDisplayed = false;
                        //this.isRevertVisible = false;
                        //this.isSendForApprovalVisible = false;
                        //this.isDeleteVisible = false;
                        this.showToast(this.locCanceledByRequestorText);
                        this.loadRequest();
                    } else {
                        let msgError = this.getErrorMessage(strResult);
                        this.showAlert(this.locErrorTitleText, msgError, this.locCloseText);

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

        
        //******************************************************************************

        //**************************** Data Grid ****************************************
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


        }

        public filterSuppliersById(supplierId: number): ng.IPromise<Supplier> {

            let deferred: ng.IDeferred<Supplier> = this.$q.defer<Supplier>();

            this.$http.get(
                this.getRegetRootUrl() + "/Supplier/GetActiveSupplierByIdJs"
                + "?supplierId=" + supplierId
                + "&t=" + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    let supplier = tmpData;

                    return deferred.resolve(supplier);

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

            return deferred.promise;
        }

       
        //*******************************************************************************

        
        //******************************************************************

        private loadInit(): void {
            this.showLoader(this.isError);

            try {
                this.iRequestId = this.getUrlParamValueInt("id");

                this.isNewRequest = (this.isValueNullOrUndefined(this.iRequestId) || this.iRequestId < 0);
                if (this.isNewRequest) {
                    this.isNewRequestOrDraft = true;
                    this.initializeNewRequest();
                    let divRequestPlaceholder: HTMLDivElement = <HTMLDivElement>document.getElementById("divRequestPlaceholder");
                    divRequestPlaceholder.style.display = "block";
                    this.requestorName = this.currentUserName;
                    this.isPrivacyReadOnly = false;
                    this.setAttachmetVisibility();
                } else {
                    this.loadRequest();
                }

                
                //this.setGrid();
                //this.setGridColumFilters();
                                
            } catch (ex) {
                this.hideLoader();
                this.displayErrorMsg();
            }
        }

        

        private hideLoaderWrapper(): void {
            
            //console.log("isRequestorCentreLoaded " + this.isRequestorCentreLoaded);
            //console.log("isPgsLoaded " +this.isPgsLoaded);
            //console.log("isCurrenciesLoaded " +this.isCurrenciesLoaded);
            //console.log("isUnitsLoaded " +this.isUnitsLoaded);
            //console.log("isPrivacyLoaded " +this.isPrivacyLoaded);
            //console.log("isRequestLoaded " +this.isRequestLoaded);
            //console.log("isShipToAddressLoaded " + this.isShipToAddressLoaded);
            //console.log("isExchangeRateLoaded " + this.isExchangeRateLoaded);
            //console.log("isDiscussionLoaded " + this.isDiscussionLoaded);
           

            if (this.isError || (
                this.isRequestorCentreLoaded
                && this.isPgsLoaded
                && this.isCurrenciesLoaded
                && this.isUnitsLoaded
                && this.isPrivacyLoaded
                && this.isExchangeRateLoaded
                && this.isRequestLoaded
                && this.isShipToAddressLoaded
                && this.isDiscussionLoaded
            )) {

                this.hideLoader();
                this.isError = false;
            }
        }

        private populateCentre(): void {
            if (this.isNewRequestOrDraft) {
                if (!this.isValueNullOrUndefined(this.selectedCentreId) && this.selectedCentreId > -1) {
                    if (!this.isValueNullOrUndefined(this.requestorCentres) && this.requestorCentres.length === 1) {
                        this.isRequestorCentreReadOnly = true;
                    } else {
                        this.isRequestorCentreReadOnly = false;
                    }
                } else if (!this.isValueNullOrUndefined(this.requestorCentres) && this.requestorCentres.length === 1) {
                    this.isRequestorCentreReadOnly = true;
                    this.selectedCentreId = (this.requestorCentres[0].id);
                    this.centreName = this.requestorCentres[0].name;
                    
                } else {
                    this.isPgsLoaded = true;
                    this.isRequestorCentreReadOnly = false;
                    this.selectedCentreId = this.requestorCentres[0].id;
                    this.centreName = this.requestorCentres[0].name;
                }
                this.loadPgs();
           
            }
        }

        private centreChanged(): void {
            if (this.isValueNullOrUndefined(this.selectedCentreId)) {
                this.isFreeSupplierAllowed = false;
                return;
            }
            this.loadPgs();

            this.hideAppMatrix();
        }

        private selectPg(): void {
            //this.isAppManOrdererDisplayed = false;
            this.isPgStandard = true;
            this.isPgItemOrder = false;

            if (this.isValueNullOrUndefined(this.pgId)) {
                return;
            }

            let tmpPg = this.$filter("filter")(this.purchaseGroupsCmb, { id: (this.pgId) }, true);
            if (this.isValueNullOrUndefined(tmpPg[0])) {
                return;
            }

            this.selectedCmbPg = tmpPg[0];
            if (this.selectedCmbPg.pg_type === PurchaseGroupType.Items) {
                this.isPgItemOrder = true;
                this.isPgStandard = false;
            } else if (this.selectedCmbPg.pg_type === PurchaseGroupType.Standard) {
                this.isPgStandard = true;
            //} else if (this.selectedPg.pg_type === PurchaseGroupType.ApprovalOnly) {
            //    this.isPgStandard = true;
            }

            //this.loadCustomFields(this.selectedPg.id);
            //this.getPgLimits(this.selectedPg.id);

            let selPgFilter: any = this.$filter("filter")(this.purchaseGroups, { id: this.selectedCmbPg.id }, true);
            this.selectedPg = selPgFilter[0];
                                    
            this.getPgLimits();
            this.getOrderers();
            //this.getCustomFields();

            
            if (this.isNewRequestOrDraft && !this.isValueNullOrUndefined(this.selectedPg.custom_field) && this.selectedPg.custom_field.length > 0) {
                this.isCustomFieldVisible = true;
            } else {
                this.isCustomFieldVisible = false;
            }
        }

        private selectCurrency(): void {
            this.getPgLimits();
        }

        private getPgLimits(): void {
            let selCurrency: DropDownExtend[] = this.$filter("filter")(this.currencyList, { id: this.currencyId }, true);
            if (!this.isValueNullOrUndefined(selCurrency) && selCurrency.length > 0) {
                this.currencyCode = selCurrency[0].name;
            } else {
                this.currencyCode = null;
            }

            let divErrMsg: HTMLDivElement = <HTMLDivElement>document.getElementById("divErrAppMen");
            divErrMsg.style.display = "none"; 

            let selPgFilter: any = this.$filter("filter")(this.purchaseGroups, { id: this.selectedCmbPg.id }, true);
            let selPg: PurchaseGroup = selPgFilter[0];
           
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
            
            let estPrice: number = this.request.estimated_price;
            let priceInCentreCurr: number = estPrice;
            let exchangeRate: ExchangeRate = null;
            let exchangeRateMultipl: number = null;

            if (this.currencyId != this.centre_currency_id) {
                let exchRates: any = this.$filter("filter")(this.exchangeRates, { source_currency_id: this.centre_currency_id, destin_currency_id: this.currencyId }, true);
                if (this.isValueNullOrUndefined(exchRates[0])) {
                    exchRates = this.$filter("filter")(this.exchangeRates, { source_currency_id: this.currencyId, destin_currency_id: this.centre_currency_id }, true);
                    if (this.isValueNullOrUndefined(exchRates[0])) {
                        let filterCurrFrom: DropDownExtend[] = this.$filter("filter")(this.currencyList, { id: this.currencyId }, true);
                        let filterCurrToEx1: DropDownExtend[] = this.$filter("filter")(this.currencyList, { id: this.centre_currency_id }, true);
                        let curr1: string = filterCurrFrom[0].name;
                        let curr2: string = filterCurrToEx1[0].name;
                        let msg: string = this.locExchangeRateMissingText.replace('{0}', curr1).replace('{1}', curr2);
                        this.displayErrorMsg(msg);
                        return;
                    }

                    exchangeRate = exchRates[0];
                    priceInCentreCurr = estPrice / exchangeRate.exchange_rate1;
                    exchangeRateMultipl =  exchangeRate.exchange_rate1;
                } else {
                    exchangeRate = exchRates[0];
                    priceInCentreCurr = estPrice * exchangeRate.exchange_rate1;
                    exchangeRateMultipl = 1 / exchangeRate.exchange_rate1;
                }
            }
            

            if (!this.isValueNullOrUndefined(selPg.purchase_group_limit)) {
                for (let i: number = 0; i < selPg.purchase_group_limit.length; i++) {
                    if (selPg.purchase_group_limit[i].is_bottom_unlimited
                        || selPg.purchase_group_limit[i].limit_bottom < priceInCentreCurr) {

                        if (!this.isValueNullOrUndefined(selPg.purchase_group_limit[i].manager_role)) {
                            for (let j: number = 0; j < selPg.purchase_group_limit[i].manager_role.length; j++) {
                                                                
                                let tmpManRole: ManagerRole = angular.copy(selPg.purchase_group_limit[i].manager_role[j]);
                                
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
                for (let i = 0; i < this.selectedPg.purchase_group_limit.length; i++) {
                    if (this.isValueNullOrUndefined(exchangeRateMultipl)) {
                        this.selectedPg.purchase_group_limit[i].limit_bottom_loc_curr_text_ro = this.selectedPg.purchase_group_limit[i].limit_bottom_text_ro;
                        this.selectedPg.purchase_group_limit[i].limit_top_loc_curr_text_ro = this.selectedPg.purchase_group_limit[i].limit_top_text_ro;
                    } else {
                        if (!this.isValueNullOrUndefined(this.selectedPg.purchase_group_limit[i].limit_bottom)) {
                            this.selectedPg.purchase_group_limit[i].limit_bottom_loc_curr_text_ro = this.convertDecimalToText(this.selectedPg.purchase_group_limit[i].limit_bottom * exchangeRateMultipl);
                        }
                        if (!this.isValueNullOrUndefined(this.selectedPg.purchase_group_limit[i].limit_top)) {
                            this.selectedPg.purchase_group_limit[i].limit_top_loc_curr_text_ro = this.convertDecimalToText(this.selectedPg.purchase_group_limit[i].limit_top * exchangeRateMultipl);
                        }
                    }
                }
            }
        }
               
        private getOrderers(): void {
            let divErrMsg: HTMLDivElement = <HTMLDivElement>document.getElementById("divErrOrderer");
            divErrMsg.style.display = "none";

            this.orderers = [];
            let selPgFilter: any = this.$filter("filter")(this.purchaseGroups, { id: this.selectedCmbPg.id }, true);
            let selPg: PurchaseGroup = selPgFilter[0];

            this.is_order_needed = selPg.is_order_needed;
            this.is_self_ordered = selPg.self_ordered;

            if (this.is_order_needed === false) {
                return;
            }
            
            if (selPg.self_ordered == true) {
                let orderer: Orderer = new Orderer();
                orderer.participant_id = this.currentUserId;
                orderer.surname = this.currentUserName;
                this.orderers.push(orderer);
                this.selectedOrdererId = this.orderers[0].participant_id;
                this.isAppManOrdererDisplayed = true;
                return;
            }

            this.isAppManOrdererDisplayed = true;

            if (!this.isValueNullOrUndefined(selPg.orderer)) {
                for (let i: number = 0; i < selPg.orderer.length; i++) {
                    let orderer = angular.copy(selPg.orderer[i]);
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
        }

        private priceWasChanged(strPrice : string, input : any) {
            if (this.validateDecimalNumber(strPrice, input)) {
                this.request.estimated_price = this.convertTextToDecimal(strPrice);
                this.getPgLimits();
            } else {
                this.requestAppMen = [];
            }
        }

        private addRequestItem(): void {
            if (this.isNewRequestItemValid() === true) {
                let requestItem: RequestItem = new RequestItem();
                requestItem.name = this.requestItemName;
                requestItem.amount = RegetDataConvert.convertStringToDecimal(this.requestItemAmount);
                requestItem.amount_text = this.requestItemAmount;
                requestItem.unit_price = RegetDataConvert.convertStringToDecimal(this.requestItemUnitPrice);
                requestItem.unit_price_text = this.requestItemUnitPrice;

                let units = this.$filter("filter")(this.unitList, { id: this.requestItemUnitId }, true);
                if (!this.isValueNullOrUndefined(units[0])) {
                    this.unitCode = units[0].name;
                }
                requestItem.unit_code = this.unitCode;
                
                let currencies : DropDownExtend[] = this.$filter("filter")(this.currencyList, { id: this.requestItemCurrencyId }, true);
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
            } else {
                var strMsg: string = this.locDataCannotBeSavedText;
                if (!this.isStringValueNullOrEmpty(this.errRequestItem)) {
                    strMsg += "<br>" + this.errRequestItem;
                }
                this.displayErrorMsg(strMsg);
            }
        }

        private isNewRequestItemValid(): boolean {
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
            } catch {
                this.errRequestItem = this.locErrorMsgText;
                return false;
            }
        }

        private isRequestValid() : boolean {
            if (this.isKeepErrMsgHidden === true) {
                return true;
            }

            let isValid: boolean = true;

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
                let divErrMsgAppMen: HTMLDivElement = <HTMLDivElement>document.getElementById("divErrAppMen");
                divErrMsgAppMen.style.display = "block";
                isValid = false;
            }

            if (this.is_order_needed && !this.selectedPg.is_multi_orderer && this.isValueNullOrUndefined(this.selectedOrdererId)) {
                let divErrMsgOrderer: HTMLDivElement = <HTMLDivElement>document.getElementById("divErrOrderer");
                divErrMsgOrderer.style.display = "block";
                isValid = false;
            }

            if (this.is_order_needed && this.selectedPg.is_multi_orderer && (this.isValueNullOrUndefined(this.orderers) || this.orderers.length == 0)) {
                let divErrMsgOrderer: HTMLDivElement = <HTMLDivElement>document.getElementById("divErrOrderer");
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
        }

        private isAddressValid(): boolean {
            let divErrMsgAddress: HTMLDivElement = <HTMLDivElement>document.getElementById("divErrMsgAddress");
            if (!this.isValueNullOrUndefined(this.shipToAddress)
                && this.shipToAddress.length > 1) {
                if (this.isValueNullOrUndefined(this.selectedShipToAddress)) {
                    divErrMsgAddress.style.display = "block";
                    return false;
                }
            }

            divErrMsgAddress.style.display = "none";
            return true;
        }

        private initiateRequest(): void {
            if (!this.isValueNullOrUndefined(this.request)) {
                return;
            }
               
            this.request = new Request();
            this.request.request_items = [];

            this.gridOptions.data = this.request.request_items;
            //this.setGridColumFilters();
        }

        private setGridColumFilters(): boolean {
            let isSet: boolean = this.setGridColumFilter("currency_code", this.currencyListGrid);
            if (isSet) {
                isSet = this.setGridColumFilter("unit_code", this.unitListGrid);
            }

            return isSet;
        }

        private openAttachment(id: number): void {

            try {
                let url = this.getRegetRootUrl() + "Attachment/GetAttachment?id="+ id + "&t=" + new Date().getTime();
                window.open(url);
            } catch (e) {
                this.displayErrorMsg();
            } finally {
                this.hideLoader();
            }

        }

        private setEstPrice(): void {
            
            if (this.isValueNullOrUndefined(this.request.request_items)) {
                return;
            }

            let isCurrencySet: boolean = false;
            this.request.estimated_price = 0;
            for (let i: number = 0; i < this.request.request_items.length; i++) {
                this.request.estimated_price += (this.request.request_items[i].amount * this.request.request_items[i].unit_price);
                if (isCurrencySet === false) {
                    this.request.currency_id = this.request.request_items[i].currency_id;
                }
            }

            this.request.estimated_price_text = RegetDataConvert.convertDecimalToString(this.request.estimated_price, 2);
                        
        }

        private saveDraft(): void {
  
            this.updateRequest();

            this.saveRequestDraft();
        }

        private sendForApproval(): void {
            this.showLoaderBoxOnly();
            try {
                this.isKeepErrMsgHidden = false;

                if (this.isRequestValid() === false) {
                    this.displayErrorMsg(this.locEnterMandatoryValuesText);
                    return;
                }

                this.updateRequest();
                this.sendForApprovalPost();

            } catch {
                this.displayErrorMsg();
            } finally {
                this.hideLoaderWrapper();
            }
        }

        private updateRequest() {
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
        }

        public updateAppMen(): void {
            
            //let this.request.request_event_approval = []; // DOES NOT WORK !!!
            let reqEvApp: RequestEventApproval[] = [];

            //Add App Men
            if (!this.isValueNullOrUndefined(this.requestAppMen)) {
                for (let i: number = 0; i < this.requestAppMen.length; i++) {
                    let app: RequestEventApproval = new RequestEventApproval();
                    app.app_man_id = this.requestAppMen[i].participant_id;
                    app.app_level_id = this.requestAppMen[i].approve_level_id;
                    app.approve_status = ApprovalStatus.Empty;
                    reqEvApp.push(app);
                }
            }

            this.request.request_event_approval = reqEvApp;
        }

        public updateOrderers(): void {

            //let this.request.request_event_approval = []; // DOES NOT WORK !!!
            let orderers: Orderer[] = [];

            //Add Orderer
            if (!this.isValueNullOrUndefined(this.orderers)) {
                for (let i: number = 0; i < this.orderers.length; i++) {
                    let orderer: Orderer = new Orderer();
                    orderer.participant_id = this.orderers[i].participant_id;
                    orderers.push(orderer);
                }
            }

            this.request.orderers = orderers;
        }

        private updateShipToAddress(): void {
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
        }

        private updateCustomField(): void {
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
                for (let i: number = 0; i < this.selectedPg.custom_field.length; i++) {
                    let custF: CustomField = angular.copy(this.selectedPg.custom_field[i]);
                    //if (custF.data_type_id == DataType.String) {
                        custF.string_value = this.selectedPg.custom_field[i].string_value;
                    //}
                    this.request.custom_fields.push(custF);
                }
            }

            //if (!this.isValueNullOrUndefined(this.request.custom_fields) && this.request.custom_fields.length > 0) {
            //    this.isCustomFieldVisible = true;
            //}
        }

        public getRequestLoadedStatus(result: any): boolean {
            let strResult: string = result.string_value;

            let divRequestPlaceholder: HTMLDivElement = <HTMLDivElement>document.getElementById("divRequestPlaceholder");
            if (!this.isStringValueNullOrEmpty(strResult)) {

                if (strResult === "NotAuthorized") {

                    divRequestPlaceholder.style.display = "none";

                    let divNotAuthorizedPlaceholder: HTMLDivElement = <HTMLDivElement>document.getElementById("divNotAuthorizedPlaceholder");
                    divNotAuthorizedPlaceholder.style.display = "block";
                    this.getRequestAccessRequests();
                }

                this.requestNr = result.request_nr;

                let msgError = this.getErrorMessage(strResult);
                this.showAlert(this.locErrorTitleText, msgError, this.locCloseText);
                return false;
            }

            divRequestPlaceholder.style.display = "block";

            return true;
        }

        private setAttachmetVisibility(): void {
            this.isAttachmentEditable = (this.currentUserId === this.request.requestor) && this.isNewRequestOrDraft;

            this.isAttachmentDisplayed = (this.isNewRequestOrDraft || (!this.isValueNullOrUndefined(this.request.attachments) && this.request.attachments.length > 0));
                
        }

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

        private initializeNewRequest() : void {
            if (this.isValueNullOrUndefined(this.request)) {
                this.request = new Request();
                this.request.id = -1;
                this.request.privacy_id = Privacy.Private;
                this.request.requestor = this.currentUserId;

                this.privacyId = Privacy.Private;

                this.isRequestLoaded = true;
                this.isDiscussionLoaded = true;

                this.getRequestCentres();
            }

        }

        private setPurchaseGroupCmbList(): void {
            this.purchaseGroupsCmb = [];

            if (!this.isValueNullOrUndefined(this.purchaseGroups)) {
                for (let i: number = 0; i < this.purchaseGroups.length; i++) {
                    let pgCmb: PgReqDropDownExtend = new PgReqDropDownExtend();

                    pgCmb.id = this.purchaseGroups[i].id;
                    pgCmb.name = this.purchaseGroups[i].group_name;
                    pgCmb.pg_type = this.purchaseGroups[i].purchase_type;
                //dlPg.name = pgLocName;
                //dlPg.pg_type = pg.purchase_type;

                    this.purchaseGroupsCmb.push(pgCmb);
                }
            }
        }

        private supplierOnBlur(strErrorMsgId: string) {
            this.checkSupplierMandatory(this.selectedSupplier, strErrorMsgId);
        }

        public checkSupplierMandatory(selectedSupplier: Supplier, strErrorMsgId: string): boolean {
            if (this.isKeepErrMsgHidden === true) {
                return true;
            }

            if (this.isStringValueNullOrEmpty(strErrorMsgId)) {
                return true;
            }

            if (this.isValueNullOrUndefined(selectedSupplier)) {
                $("#" + strErrorMsgId).show("slow");
                return false;
            } else {
                $("#" + strErrorMsgId).slideUp("slow");
            }

            return true;
        }


        private searchSupplier(strSupp: string): ng.IPromise<Supplier[]> {
            return this.filterSuppliers(strSupp, this.selectedCentreId);

        }

        public supplierSelectedItemChange(item: Supplier, strErrorMsgId: string): void {
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
        }

        private addressCheck(address: Address): void {
            //let checkedAddress: Address[] = this.$filter("filter")(this.shipToAddress, { id: addressid }, true);
            if (address.is_selected) {
                for (let i: number = 0; i < this.shipToAddress.length; i++) {
                    if (this.shipToAddress[i].id != address.id) {
                        this.shipToAddress[i].is_selected = false;
                    }
                }
                this.selectedShipToAddress = address;
            } else {
                this.selectedShipToAddress = null;
            }

            this.isAddressValid();
        }

        private getCurrAppMan(): string {
            if (this.isValueNullOrUndefined(this.request.request_event_approval)) {
                return "";
            }

            let appMen: string = "";
            let iWaitAppLevel: number = -1;
            for (let i: number = 0; i < this.request.request_event_approval.length; i++) {
                if (iWaitAppLevel >= 0 && this.request.request_event_approval[i].app_level_id != iWaitAppLevel) {
                    return appMen;
                }
                if (this.request.request_event_approval[i].approve_status == ApprovalStatus.WaitForApproval) {
                    iWaitAppLevel = this.request.request_event_approval[i].app_level_id;
                    if (appMen.length > 0) {
                        appMen += ", ";
                    }
                    appMen += this.request.request_event_approval[i].app_man_surname + " " + this.request.request_event_approval[i].app_man_first_name;
                }
            }

            return "";
        }

        private getCurrOrderer(): string {
            if (this.isValueNullOrUndefined(this.request.orderers)) {
                return "";
            }

            let orderers: string = "";
            for (let i: number = 0; i < this.request.orderers.length; i++) {
                if (orderers.length > 0) {
                    orderers += ", ";
                }
                orderers += this.request.orderers[i].surname + " " + this.request.orderers[i].first_name;
            }
        }

        private selectPrivacy() : void {
            if (!this.isNewRequest) {
                this.updatePrivacy();
            }
            //alert(this.privacyId);
        }

        public sendReminder(): void {
            this.showToast(this.locSentText);
        }

        private isCanceled() {
            if (this.isValueNullOrUndefined(this.request)) {
                return false;
            }

            let isCanceled = (this.request.request_status == RequestStatus.CanceledOrderer
                || this.request.request_status == RequestStatus.CanceledRequestor
                || this.request.request_status == RequestStatus.CanceledSystem);

            return isCanceled;
        }

                       
        private revert(): void {
            this.$mdDialog.show(
                {
                    template: this.getConfirmDialogTemplate(
                        this.locRevertRequestConfirmText,
                        this.locConfirmationText,
                        this.locRevertText,
                        this.locCloseText,
                        "confirmDialog()",
                        "closeDialog()"),
                    locals: {
                        requestControl: this
                    },
                    controller: this.dialogConfirmRevertController
                });
        }

        private activate(): void {
            this.$mdDialog.show(
                {
                    template: this.getConfirmDialogTemplate(
                        this.locActivateConfirmText,
                        this.locConfirmationText,
                        this.locYesText,
                        this.locNoText,
                        "confirmDialog()",
                        "closeDialog()"),
                    locals: {
                        requestControl: this
                    },
                    controller: this.dialogConfirmRevertController
                });
        }

        private dialogConfirmRevertController(
            $scope,
            $mdDialog,
            requestControl: RequestController
        ): void {

            $scope.closeDialog = function () {
                $mdDialog.hide();
            }

            $scope.confirmDialog = () => {
                $mdDialog.hide();
                requestControl.revertConfirmed();
            }
        }

        private remindMan() {
            this.$mdDialog.show(
                {
                    template: this.getConfirmDialogTemplate(
                        this.remindConfirmText,
                        this.locConfirmationText,
                        this.locSendText,
                        this.locCloseText,
                        "confirmDialog()",
                        "closeDialog()"),
                    locals: {
                        requestControl: this
                    },
                    controller: this.dialogConfirmRemindController
                });
        }

        private dialogConfirmRemindController(
            $scope,
            $mdDialog,
            requestControl: RequestController
        ): void {

            $scope.closeDialog = function () {
                $mdDialog.hide();
            }

            $scope.confirmDialog = () => {
                $mdDialog.hide();
                requestControl.sendReminder();
            }
        }

        private cancelRequest() {
            this.$mdDialog.show(
                {
                    template: this.getConfirmDialogTemplate(
                        this.locCanceledConfirmText,
                        this.locConfirmationText,
                        this.locYesText,
                        this.locNoText,
                        "confirmDialog()",
                        "closeDialog()"),
                    locals: {
                        requestControl: this
                    },
                    controller: this.dialogCancelRequestController
                });
        }

        private dialogCancelRequestController(
            $scope,
            $mdDialog,
            requestControl: RequestController
        ): void {

            $scope.closeDialog = function () {
                $mdDialog.hide();
            }

            $scope.confirmDialog = () => {
                $mdDialog.hide();
                requestControl.cancelRequestConfirmed();
            }
        }

        private toggleFreeSupplier(): void {
            this.isFreeSupplier = !this.isFreeSupplier;
        }

        private displayAppMatrix(): void {
            let iScrollX: number = $(".reget-body").scrollLeft();
            let iScrollY: number = $(".reget-body").scrollTop() + window.pageYOffset;

            let imgAppMatrix: HTMLImageElement = <HTMLImageElement>document.getElementById("imgAppMatrix");
            let iLeft: number = iScrollX + Math.floor(imgAppMatrix.getBoundingClientRect().left);
            let iTop: number = iScrollY + Math.floor(imgAppMatrix.getBoundingClientRect().top);

            iLeft += imgAppMatrix.width + 10;

            let divAppMatrix: HTMLDivElement = <HTMLDivElement>document.getElementById("divAppMatrix");
            divAppMatrix.style.top = iTop + "px";
            divAppMatrix.style.left = iLeft + "px";
           
            $("#divAppMatrix").show();
        }

        private hideAppMatrix(): void {
            $("#divAppMatrix").fadeOut();
        }

        private openOrder(): void {
            window.location.href = this.getRegetRootUrl() + "Request/Order?id=" + this.request.id;
        }

        protected uploadFile(attUpload: Attachment) {
            attUpload.is_can_be_deleted = true;
            if (this.isValueNullOrUndefined(this.request.attachments)) {
                this.request.attachments = [];
            }
            this.request.attachments.push(attUpload);
        }
        
        private sendRequestForAccess(): void {
            this.$mdDialog.show(
                {
                    template: this.getConfirmDialogTemplate(
                        this.locAskAcessRequestConfirmText,
                        this.locConfirmationText,
                        this.locYesText,
                        this.locNoText,
                        "confirmDialog()",
                        "closeDialog()"),
                    locals: {
                        requestControl: this
                    },
                    controller: this.dialogConfirmSendAccessRequest
                });
        }

        private dialogConfirmSendAccessRequest(
            $scope,
            $mdDialog,
            requestControl: RequestController
        ): void {

            $scope.closeDialog = function () {
                $mdDialog.hide();
            }

            $scope.confirmDialog = () => {
                $mdDialog.hide();
                requestControl.sendAccessRequest();
            }
        }

        private approveRequest(): void {
            this.$mdDialog.show(
                {
                    template: this.getConfirmDialogTemplate(
                        this.locApproveConfirmText,
                        this.locConfirmationText,
                        this.locYesText,
                        this.locNoText,
                        "confirmDialog()",
                        "closeDialog()"),
                    locals: {
                        requestControl: this
                    },
                    controller: this.dialogConfirmRequestApproval
                });
        }

        private dialogConfirmRequestApproval(
            $scope,
            $mdDialog,
            requestControl: RequestController
        ): void {

            $scope.closeDialog = () => {
                $mdDialog.hide();
            }

            $scope.confirmDialog = () => {
                $mdDialog.hide();
                requestControl.approveRequestConfirmed();
            }
        }

        private rejectRequest(): void {
            this.$mdDialog.show(
                {
                    template: this.getConfirmDialogTemplate(
                        this.locRejectConfirmText,
                        this.locConfirmationText,
                        this.locYesText,
                        this.locNoText,
                        "confirmDialog()",
                        "closeDialog()"),
                    locals: {
                        requestControl: this
                    },
                    controller: this.dialogConfirmRequestReject
                });
        }

        private dialogConfirmRequestReject(
            $scope,
            $mdDialog,
            requestControl: RequestController
        ): void {

            $scope.closeDialog = () => {
                $mdDialog.hide();
            }

            $scope.confirmDialog = () => {
                $mdDialog.hide();
                requestControl.rejectRequestConfirmed();
            }
        }
    }

    angular.
        module('RegetApp').
        controller('RequestController', Kamsyk.RegetApp.RequestController).
        config(function ($mdDateLocaleProvider) { //only because of Date picker is implemented
            this.SetDatePicker($mdDateLocaleProvider);
            //it is neccessary to set IsGenerateDatePickerLocalization = true
        });
  
}

