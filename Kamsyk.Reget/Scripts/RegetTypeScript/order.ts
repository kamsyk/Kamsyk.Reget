/// <reference path="../RegetTypeScript/Base/reget-base.ts" />
/// <reference path="../typings/ng-file-upload/ng-file-upload.d.ts" />

module Kamsyk.RegetApp {
    let app = angular.module('RegetApp', ['ngMaterial', 'ngMessages', 'ngFileUpload', 'ui.grid', 'ui.grid.pagination', 'ui.grid.resizeColumns', 'ui.grid.selection', 'ui.grid.moveColumns', 'ui.grid.edit']);

    //angular.module('RegetApp').directive('myValidator', function () {
    //    return {
    //        // element must have ng-model attribute
    //        // or $validators does not work
    //        require: 'ngModel',
    //        link: function (scope, elm, attrs, ctrl : any) {
    //            ctrl.$validators.myValidator = function (modelValue, viewValue) {
    //                // validate viewValue with your custom logic
    //                var valid = (viewValue && viewValue.length > 0) || false;
    //                return false;
    //            };
    //        }
    //    };
    //});

    export class OrderController extends BaseRegetTs implements angular.IController {
        //**********************************************************
        //Properties
        private iRequestId: number = null;
        private order: Order = null;
        private isOrderLoaded: boolean = false;
        private isLangLoaded: boolean = false;
        private orderLangCmb: AgDropDown[] = null;
       // private isNewOrder: boolean = false;
        //**********************************************************

        //***************************************************************************
        //Localization
        private locNotAuthorizedPerformActionText: string = $("#NotAuthorizedPerformActionText").val();

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
            protected Upload: ng.angularFileUpload.IUploadService

        ) {
            super($scope, $http, $filter, $mdDialog, $mdToast, $q, $timeout);

            //there must be - let app = angular.module('RegetApp', ['ngFileUpload' .. in scrit wher upload file is used e.g. request.ts
            this.fileUploadInit($scope, this.Upload);

            this.loadData();

        }
        //***************************************************************

        $onInit = () => { };

        //***************************************************************
        //Http
        private loadOrder(): void {
            this.iRequestId = this.getUrlParamValueInt("id");

            this.showLoaderBoxOnly(this.isError);

            this.$http.get(
                this.getRegetRootUrl() + "Request/GetOrder?requestId=" + this.iRequestId + "&t=" + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    let result: any = response.data;
                    this.order = result;

                    this.isOrderLoaded = true;

                    this.getOrderLangs();

                    this.hideLoaderWrapper();

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

        private getOrderLangs(): void {
            this.iRequestId = this.getUrlParamValueInt("id");

            this.showLoaderBoxOnly(this.isError);

            this.$http.get(
                this.getRegetRootUrl() + "Request/GetOrderLanguages?requestId=" + this.iRequestId + "&t=" + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    let result: any = response.data;
                    this.orderLangCmb = result;

                    this.isLangLoaded = true;

                    this.hideLoaderWrapper();

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

        //Methods
        private loadData(): void {
            this.loadOrder();
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
                this.isOrderLoaded
                && this.isLangLoaded
            )) {

                this.hideLoader();
                this.isError = false;
            }
        }

        protected uploadFile(attUpload: Attachment) {
            attUpload.is_can_be_deleted = true;
            if (this.isValueNullOrUndefined(this.order.attachments)) {
                this.order.attachments = [];
            }
            this.order.attachments.push(attUpload);
        }

        private deleteAttachment(id: number): void {
                       
            this.showLoader(this.isError);
                      
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
                        for (var i: number = this.order.attachments.length - 1; i >= 0; i--) {
                            if (this.order.attachments[i].id === id) {
                                this.order.attachments.splice(i, 1);
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

        private openSuppDetail(): void {
            window.location.href = this.getRegetRootUrl() + "Supplier/Detail?id=" + this.order.supplier_id;
        }

        //**************************************************************
    }
    angular.
        module("RegetApp").
        controller("OrderController", Kamsyk.RegetApp.OrderController);
}