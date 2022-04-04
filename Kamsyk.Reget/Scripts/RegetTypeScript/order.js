/// <reference path="../RegetTypeScript/Base/reget-base.ts" />
/// <reference path="../typings/ng-file-upload/ng-file-upload.d.ts" />
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
        var app = angular.module('RegetApp', ['ngMaterial', 'ngMessages', 'ngFileUpload', 'ui.grid', 'ui.grid.pagination', 'ui.grid.resizeColumns', 'ui.grid.selection', 'ui.grid.moveColumns', 'ui.grid.edit']);
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
        var OrderController = /** @class */ (function (_super) {
            __extends(OrderController, _super);
            //**********************************************************
            //Constructor
            function OrderController($scope, $http, $filter, $mdDialog, $mdToast, $q, $timeout, Upload) {
                var _this = _super.call(this, $scope, $http, $filter, $mdDialog, $mdToast, $q, $timeout) || this;
                _this.$scope = $scope;
                _this.$http = $http;
                _this.$filter = $filter;
                _this.$mdDialog = $mdDialog;
                _this.$mdToast = $mdToast;
                _this.$q = $q;
                _this.$timeout = $timeout;
                _this.Upload = Upload;
                //**********************************************************
                //Properties
                _this.iRequestId = null;
                _this.order = null;
                _this.isOrderLoaded = false;
                _this.isLangLoaded = false;
                _this.orderLangCmb = null;
                // private isNewOrder: boolean = false;
                //**********************************************************
                //***************************************************************************
                //Localization
                _this.locNotAuthorizedPerformActionText = $("#NotAuthorizedPerformActionText").val();
                //***************************************************************
                _this.$onInit = function () { };
                //there must be - let app = angular.module('RegetApp', ['ngFileUpload' .. in scrit wher upload file is used e.g. request.ts
                _this.fileUploadInit($scope, _this.Upload);
                _this.loadData();
                return _this;
            }
            //***************************************************************
            //Http
            OrderController.prototype.loadOrder = function () {
                var _this = this;
                this.iRequestId = this.getUrlParamValueInt("id");
                this.showLoaderBoxOnly(this.isError);
                this.$http.get(this.getRegetRootUrl() + "Request/GetOrder?requestId=" + this.iRequestId + "&t=" + new Date().getTime(), {}).then(function (response) {
                    try {
                        var result = response.data;
                        _this.order = result;
                        _this.isOrderLoaded = true;
                        _this.getOrderLangs();
                        _this.hideLoaderWrapper();
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
            OrderController.prototype.getOrderLangs = function () {
                var _this = this;
                this.iRequestId = this.getUrlParamValueInt("id");
                this.showLoaderBoxOnly(this.isError);
                this.$http.get(this.getRegetRootUrl() + "Request/GetOrderLanguages?requestId=" + this.iRequestId + "&t=" + new Date().getTime(), {}).then(function (response) {
                    try {
                        var result = response.data;
                        _this.orderLangCmb = result;
                        _this.isLangLoaded = true;
                        _this.hideLoaderWrapper();
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
            //Methods
            OrderController.prototype.loadData = function () {
                this.loadOrder();
            };
            OrderController.prototype.hideLoaderWrapper = function () {
                //console.log("isRequestorCentreLoaded " + this.isRequestorCentreLoaded);
                //console.log("isPgsLoaded " +this.isPgsLoaded);
                //console.log("isCurrenciesLoaded " +this.isCurrenciesLoaded);
                //console.log("isUnitsLoaded " +this.isUnitsLoaded);
                //console.log("isPrivacyLoaded " +this.isPrivacyLoaded);
                //console.log("isRequestLoaded " +this.isRequestLoaded);
                //console.log("isShipToAddressLoaded " + this.isShipToAddressLoaded);
                //console.log("isExchangeRateLoaded " + this.isExchangeRateLoaded);
                //console.log("isDiscussionLoaded " + this.isDiscussionLoaded);
                if (this.isError || (this.isOrderLoaded
                    && this.isLangLoaded)) {
                    this.hideLoader();
                    this.isError = false;
                }
            };
            OrderController.prototype.uploadFile = function (attUpload) {
                attUpload.is_can_be_deleted = true;
                if (this.isValueNullOrUndefined(this.order.attachments)) {
                    this.order.attachments = [];
                }
                this.order.attachments.push(attUpload);
            };
            OrderController.prototype.deleteAttachment = function (id) {
                var _this = this;
                this.showLoader(this.isError);
                var jsonId = JSON.stringify({ id: id });
                this.$http.post(this.getRegetRootUrl() + "Attachment/DeleteAttachment?t=" + new Date().getTime(), jsonId).then(function (response) {
                    try {
                        var tmpData = response.data;
                        var httpResult = tmpData;
                        _this.hideLoaderWrapper();
                        if (_this.isValueNullOrUndefined(httpResult.error_id) || httpResult.error_id === 0) {
                            //Deleted
                            for (var i = _this.order.attachments.length - 1; i >= 0; i--) {
                                if (_this.order.attachments[i].id === id) {
                                    _this.order.attachments.splice(i, 1);
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
            OrderController.prototype.openSuppDetail = function () {
                window.location.href = this.getRegetRootUrl() + "Supplier/Detail?id=" + this.order.supplier_id;
            };
            return OrderController;
        }(RegetApp.BaseRegetTs));
        RegetApp.OrderController = OrderController;
        angular.
            module("RegetApp").
            controller("OrderController", Kamsyk.RegetApp.OrderController);
    })(RegetApp = Kamsyk.RegetApp || (Kamsyk.RegetApp = {}));
})(Kamsyk || (Kamsyk = {}));
//# sourceMappingURL=order.js.map