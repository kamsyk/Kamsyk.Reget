/// <reference path="../RegetTypeScript/Base/reget-base.ts" />
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
        var SearchController = /** @class */ (function (_super) {
            __extends(SearchController, _super);
            //***********************************************************************
            //**********************************************************
            //Constructor
            function SearchController($scope, $http, $filter, $mdDialog, $mdToast, $q, $timeout, $sce) {
                var _this = _super.call(this, $scope, $http, $filter, $mdDialog, $mdToast, $q, $timeout) || this;
                _this.$scope = $scope;
                _this.$http = $http;
                _this.$filter = $filter;
                _this.$mdDialog = $mdDialog;
                _this.$mdToast = $mdToast;
                _this.$q = $q;
                _this.$timeout = $timeout;
                _this.$sce = $sce;
                //***********************************************************************
                //Properties
                _this.strSelectedSearch = null;
                _this.searchsearch = null;
                _this.selectedSearchResult = null;
                _this.searchResults = null;
                _this.rowsCount = null;
                _this.pageNumbers = null;
                _this.currentPage = 1;
                _this.startDisplayPage = 1;
                _this.isMyRequestsOnly = false;
                //***************************************************************
                _this.$onInit = function () { };
                _this.angSceAny = $sce;
                return _this;
                //this.loadData();
            }
            //******************************************************
            SearchController.prototype.searchRequests = function (searchText, isSimpleSearch, isPageNrsRenew, strErrorMsgId) {
                var _this = this;
                if (this.checkSearchMandatory(strErrorMsgId) === false) {
                    return;
                }
                this.showLoaderBoxOnly();
                var deferred = this.$q.defer();
                this.$http.get(this.getRegetRootUrl() + "/Search/GetSearch?searchText=" + encodeURI(searchText)
                    + "&currentPage=" + this.currentPage
                    + "&isSimpleSearch=" + isSimpleSearch
                    + "&isMyRequestsOnly=" + this.isMyRequestsOnly
                    + "&t=" + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        //this.gridOptions.data = tmpData.db_data;
                        _this.rowsCount = tmpData.rows_count;
                        //var tmpData: any = response.data.db_data;
                        var results = tmpData.db_data;
                        if (!isSimpleSearch) {
                            _this.searchResults = results;
                            if (isPageNrsRenew) {
                                _this.pageNumbers = [];
                                if (_this.startDisplayPage > 1) {
                                    _this.pageNumbers.push("...p");
                                }
                                var iPageNr = _this.startDisplayPage;
                                while ((iPageNr * 10) < _this.rowsCount && _this.pageNumbers.length < 10) {
                                    _this.pageNumbers.push(iPageNr.toString());
                                    iPageNr++;
                                }
                                if (((iPageNr - 1) * 10) < _this.rowsCount && _this.pageNumbers.length < 10) {
                                    _this.pageNumbers.push(iPageNr.toString());
                                }
                                if (((iPageNr) * 10) < _this.rowsCount) {
                                    _this.pageNumbers.push("...n");
                                }
                            }
                        }
                        return deferred.resolve(results);
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
            //*****************************************************
            //****************************************************************************
            //Methods
            SearchController.prototype.searchSelectedItemChange = function (item, strErrorMsgId) {
                //this.selectedSearchResult = item;
                this.displayResults(strErrorMsgId);
            };
            SearchController.prototype.searchText = function (strText, strErrorMsgId) {
                return this.searchRequests(strText, true, false, strErrorMsgId);
            };
            SearchController.prototype.searchOnBlur = function (strErrorMsgId) {
                this.checkSearchMandatory(strErrorMsgId);
            };
            SearchController.prototype.checkSearchMandatory = function (strErrorMsgId) {
                if (this.isStringValueNullOrEmpty(strErrorMsgId)) {
                    return true;
                }
                if (this.isValueNullOrUndefined(this.searchsearch) || this.searchsearch.length < 2) {
                    $("#" + strErrorMsgId).show("slow");
                    return false;
                }
                else {
                    $("#" + strErrorMsgId).slideUp("slow");
                }
                return true;
            };
            SearchController.prototype.displayResults = function (strErrorMsgId) {
                this.currentPage = 1;
                this.searchRequests(this.searchsearch, false, true, strErrorMsgId);
            };
            SearchController.prototype.getHtmlText = function (dbText) {
                //return this.angSceAny.trustAsHtml("sdds");
                if (this.isValueNullOrUndefined(dbText)) {
                    return null;
                }
                //let htmlText: string = dbText.replace("&lt;br /&gt;", "<br />");
                //let htmlText: string = (dbText.replace(/\n/g, '<br />'));
                //let htmlText: string = dbText.replace(/(\\r\\n)|([\r\n])/gmi, '<br/>');
                return this.angSceAny.trustAsHtml(dbText);
            };
            SearchController.prototype.displayPage = function (strPageNr, strErrorMsgId) {
                var isPageNrsRenew = false;
                if (strPageNr === "...n") {
                    isPageNrsRenew = true;
                    this.currentPage = this.startDisplayPage + 10;
                    this.startDisplayPage = this.currentPage;
                }
                else if (strPageNr === "...p") {
                    isPageNrsRenew = true;
                    this.currentPage = this.startDisplayPage - 10;
                    this.startDisplayPage = this.currentPage;
                }
                else {
                    this.currentPage = parseInt(strPageNr);
                }
                this.searchRequests(this.searchsearch, false, isPageNrsRenew, strErrorMsgId);
            };
            SearchController.prototype.toggleMyRequestsOnly = function () {
                if (this.isMyRequestsOnly === true) {
                    this.isMyRequestsOnly = false;
                }
                else {
                    this.isMyRequestsOnly = true;
                }
            };
            SearchController.prototype.getLinkText = function (rawString) {
                if (rawString === "...n" || rawString === "...p") {
                    return "...";
                }
                return rawString;
            };
            return SearchController;
        }(RegetApp.BaseRegetTs));
        RegetApp.SearchController = SearchController;
        angular.
            module('RegetApp').
            controller('SearchController', Kamsyk.RegetApp.SearchController);
    })(RegetApp = Kamsyk.RegetApp || (Kamsyk.RegetApp = {}));
})(Kamsyk || (Kamsyk = {}));
//# sourceMappingURL=search.js.map