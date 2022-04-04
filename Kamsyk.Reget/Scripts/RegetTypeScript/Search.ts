/// <reference path="../RegetTypeScript/Base/reget-base.ts" />

module Kamsyk.RegetApp {
    export class SearchController extends BaseRegetTs implements angular.IController {
        //***********************************************************************
        //Properties
        private strSelectedSearch: string = null;
        private searchsearch: string = null;
        private selectedSearchResult: RequestSearchResult = null;
        private searchResults: RequestSearchResult[] = null;
        private rowsCount: number = null;
        private pageNumbers: string[] = null;
        private currentPage: number = 1;
        private startDisplayPage: number = 1;
        private isMyRequestsOnly: boolean = false;
        //***********************************************************************

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
            protected $sce: ng.ISCEProvider
        ) {
            super($scope, $http, $filter, $mdDialog, $mdToast, $q, $timeout);
            this.angSceAny = $sce;
            //this.loadData();

        }
        //***************************************************************

        $onInit = () => { };

        //******************************************************
        
        public searchRequests(searchText: string, isSimpleSearch: boolean, isPageNrsRenew: boolean, strErrorMsgId: string): ng.IPromise<RequestSearchResult[]> {
            if (this.checkSearchMandatory(strErrorMsgId) === false) {
                return;
            }

            this.showLoaderBoxOnly();
            var deferred: ng.IDeferred<RequestSearchResult[]> = this.$q.defer<RequestSearchResult[]>();

            this.$http.get(
                this.getRegetRootUrl() + "/Search/GetSearch?searchText=" + encodeURI(searchText)
                + "&currentPage=" + this.currentPage 
                + "&isSimpleSearch=" + isSimpleSearch
                + "&isMyRequestsOnly=" + this.isMyRequestsOnly
                + "&t=" + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    //this.gridOptions.data = tmpData.db_data;
                    this.rowsCount = tmpData.rows_count;
                    //var tmpData: any = response.data.db_data;
                    var results: RequestSearchResult[] = tmpData.db_data;

                    if (!isSimpleSearch) {
                        this.searchResults = results;
                        if (isPageNrsRenew) {
                            this.pageNumbers = [];

                            if (this.startDisplayPage > 1) {
                                this.pageNumbers.push("...p");
                            }

                            let iPageNr: number = this.startDisplayPage;
                            while ((iPageNr * 10) < this.rowsCount && this.pageNumbers.length < 10) {
                                this.pageNumbers.push(iPageNr.toString());
                                iPageNr++;
                            }
                            if (((iPageNr - 1) * 10) < this.rowsCount && this.pageNumbers.length < 10) {
                                this.pageNumbers.push(iPageNr.toString());
                            }
                            if (((iPageNr) * 10) < this.rowsCount) {
                                this.pageNumbers.push("...n");
                            }
                            
                        }
                    }
                   
                    return deferred.resolve(results);

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
        //*****************************************************

        //****************************************************************************
        //Methods
        public searchSelectedItemChange(item: RequestSearchResult, strErrorMsgId: string): void {
            //this.selectedSearchResult = item;
            this.displayResults(strErrorMsgId);
        }

        private searchText(strText: string, strErrorMsgId: string): ng.IPromise<RequestSearchResult[]> {
            return this.searchRequests(strText, true, false, strErrorMsgId);
        }

        private searchOnBlur(strErrorMsgId: string) {
            this.checkSearchMandatory(strErrorMsgId);
        }

        public checkSearchMandatory(strErrorMsgId: string): boolean {
            
            if (this.isStringValueNullOrEmpty(strErrorMsgId)) {
                return true;
            }

            if (this.isValueNullOrUndefined(this.searchsearch) || this.searchsearch.length < 2) {
                $("#" + strErrorMsgId).show("slow");
                return false;
            } else {
                $("#" + strErrorMsgId).slideUp("slow");
            }

            return true;
        }

        private displayResults(strErrorMsgId: string) {
            this.currentPage = 1;            
            this.searchRequests(this.searchsearch, false, true, strErrorMsgId);
        }

        protected getHtmlText(dbText: string): string {
            //return this.angSceAny.trustAsHtml("sdds");

            if (this.isValueNullOrUndefined(dbText)) {
                return null;
            }

            //let htmlText: string = dbText.replace("&lt;br /&gt;", "<br />");
            //let htmlText: string = (dbText.replace(/\n/g, '<br />'));

            //let htmlText: string = dbText.replace(/(\\r\\n)|([\r\n])/gmi, '<br/>');

            return this.angSceAny.trustAsHtml(dbText);
        }

        private displayPage(strPageNr: string, strErrorMsgId: string): void {
            let isPageNrsRenew: boolean = false;
            if (strPageNr === "...n") {
                isPageNrsRenew = true;
                this.currentPage = this.startDisplayPage + 10;
                this.startDisplayPage = this.currentPage;
            } else if (strPageNr === "...p") {
                isPageNrsRenew = true;
                this.currentPage = this.startDisplayPage - 10;
                this.startDisplayPage = this.currentPage;
            } else {
                this.currentPage = parseInt(strPageNr);
            }
            this.searchRequests(this.searchsearch, false, isPageNrsRenew, strErrorMsgId);
        }

        private toggleMyRequestsOnly() {
            if (this.isMyRequestsOnly === true) {
                this.isMyRequestsOnly = false;
            } else {
                this.isMyRequestsOnly = true;
            }
        }

        private getLinkText(rawString : string): string {
            if (rawString === "...n" || rawString === "...p") {
                return "...";
            }

            return rawString;
        }

        //***************************************************************
    }
    angular.
        module('RegetApp').
        controller('SearchController', Kamsyk.RegetApp.SearchController);
}