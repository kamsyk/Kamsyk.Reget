/// <reference path="../typings/chart.js/index.d.ts" />
/// <reference path="../typings/kamsyk-material/chart.d.ts" />

module Kamsyk.RegetApp {
    angular.module('RegetApp').directive("filterItem", function () {
        return {
            scope: {
                id: '@',
                name: '@',
                rooturl: '@',
                flagurl: '@',
                removetext: '@',
                deleteitem: '&'
            },
            templateUrl: RegetCommonTs.getRegetRootUrl() + 'Content/Html/AngStatisticsFilter.html'

        };
    });

    angular.module('RegetApp').filter("localeOrderBy", [function () {
        return function (array, sortPredicate, reverseOrder) {
            if (!Array.isArray(array)) return array;
            if (!sortPredicate) return array;

            var isString = function (value) {
                return (typeof value === "string");
            };

            var isNumber = function (value) {
                return (typeof value === "number");
            };

            var isBoolean = function (value) {
                return (typeof value === "boolean");
            };

            var arrayCopy = [];
            angular.forEach(array, function (item) {
                arrayCopy.push(item);
            });

            arrayCopy.sort(function (a, b) {
                var valueA = a[sortPredicate];
                var valueB = b[sortPredicate];

                if (isString(valueA))
                    return !reverseOrder ? valueA.localeCompare(valueB) : valueB.localeCompare(valueA);

                if (isNumber(valueA) || isBoolean(valueA))
                    return !reverseOrder ? valueA - valueB : valueB - valueA;

                return 0;
            });

            return arrayCopy;
        };
    }]);

    export class StatisticsController extends BaseRegetTs implements angular.IController {

        //***********************************************************
        //Constants
        private FILTER_REQUESTOR: number = 10;
        private FILTER_ORDERER: number = 20;
        private FILTER_SUPPLIER: number = 30;
        private FILTER_CENTER: number = 40;
        private FILTER_AREA: number = 50;
        private FILTER_COMMODITY: number = 60;
        private FILTER_TOTAL: number = 70;
        private FILTER_COMPANY: number = 80;
        private FILTER_APPROVE_MAN: number = 90;
        //***********************************************************

        //***********************************************************
        //Local Texts
        private locWarningText: string = $("#WarningText").val();
        private locErrMsg: string = $("#ErrMsgText").val();
        private locPriceText: string = $("#PriceText").val();
        private locNumOfRequestsText: string = $("#NumOfRequestsText").val();
        private locAllText: string = $("#AllText").val();
        private locYearText: string = $("#YearText").val();
        private locMonthText: string = $("#MonthText").val();
        private locSelectCompanyText: string = $("#SelectCompanyText").val();
        private locSelectDateFromText: string = $("#SelectDateFromText").val();
        private locSelectDateToText: string = $("#SelectDateToText").val();
        private locSelectAxisXText: string = $("#SelectAxisXText").val();
        private locSelectAxisYText: string = $("#SelectAxisYText").val();
        private locSelectPeriodText: string = $("#SelectPeriodText").val();
        private locNotFoundText: string = $("#NotFoundText").val();

        private locSelectRequestorsText: string = $("#SelectRequestorsText").val();
        private locOrdererText: string = $("#OrdererText").val();
        private locApproveManText: string = $("#ApproveManText").val();
        private locCentreText: string = $("#CentreText").val();
        private locSupplierText: string = $("#SupplierText").val();
        private locPurchaseGroupText: string = $("#PurchaseGroupText").val();
        private locAreaText: string = $("#AreaText").val();
        private locExchangeRateMissingText: string = $("#ExchangeRateMissingText").val();
        private locStatisticsAuthorizedOwnRequestOnlyText: string = $("#StatisticsAuthorizedOwnRequestOnlyText").val();
        private locStatisticsAuthorizedCgRequestOnlyText: string = $("#StatisticsAuthorizedCgRequestOnlyText").val();
        private locNotAuthorizedText: string = $("#NotAuthorizedText").val();
        private locItemWasAddedToFilter: string = $("#ItemWasAddedToFilterText").val();

        private locStatisticsRequestorNotAllowedText: string = $("#StatisticsRequestorNotAllowedText").val();
        private locStatisticsOrdererNotAllowedText: string = $("#StatisticsOrdererNotAllowedText").val();
        private locStatisticsAppManNotAllowedText: string = $("#StatisticsAppManNotAllowedText").val();
        private locStatisticsCentreNotAllowedText: string = $("#StatisticsCentreNotAllowedText").val();
        private locStatisticsAreaNotAllowedText: string = $("#StatisticsAreaNotAllowedText").val();
        private locStatisticsPgNotAllowedText: string = $("#StatisticsPgNotAllowedText").val();
        private locStatisticsSupplierNotAllowedText: string = $("#StatisticsSupplierNotAllowedText").val();
        private locEnterMandatoryValuesText: string = $("#EnterMandatoryValuesText").val();
        //***********************************************************

        //***********************************************************************
        //Properties
        private axisXItemsList: DropDownExtend[] = null;
        private axisXTopList: DropDownExtend[] = null;
        private axisYItemsList: DropDownExtend[] = null;
        private axisXPeriodItemsList: DropDownExtend[] = null;
        private chartTypeItemsList: DropDownExtend[] = null;
        private isChartAxisItemsLoaded: boolean = false;
        private isCompaniesLoaded: boolean = false;
        private isCurrenciesLoaded: boolean = false;
        private selectedAxisXTop: number = null;
        private rootUrl: string = null;
        private userCompanies: StatisticsCompany[] = null;
        private currenciesList: Currency[] = null;
        private selectedAxisXItem: number = null;
        private selectedAxisYItem: number = null;
        private selectedAxisXPeriodItem: number = null;
        private filterItems: StatisticsFilter[] = null;
        private isActiveOnly: boolean = false;
        private isCost: boolean = false;
        private isFilterUserOnly: boolean = false;
        private filterPlaceholder: string = null;
        private isSelectXItemsAvailable: boolean = false;
        private xFilterMsg: FilterMsg[] = null;
        private isSelectedXItems: boolean = false;
        private searchstringitem: string = null;
        private filterselecteditem: StatisticsFilter = null;
        private chartErrorMsg: string = null;
        private filterFromDate: Date = null;
        private filterToDate: Date = null;
        private chartItems: ChartItems = null;
        private chartOptions: ChartOptions = null;
        private chartOptionsDonut: ChartOptions = null;
        private regetChart: Chart = null;
        private selectedChartTypeItem: number = null;
        private chartData: KsLineChartData = null;
        private chartDataDonut: KsPieChartData = null;
        private selectedCurrencyId: number = null;
        private origCurrencyId: number = null;
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
            protected $timeout: ng.ITimeoutService

        ) {
            super($scope, $http, $filter, $mdDialog, $mdToast, $q, $timeout);

            this.loadData();

        }
        //***************************************************************

        $onInit = () => { };

        //***************************************************************
        //Http Methods
        private getAxisXItems(): void {
            this.showLoaderBoxOnly();
            
            this.$http.get(
                this.getRegetRootUrl() + "Statistics/GetXOptions?&t=" + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    this.axisXItemsList = tmpData;
                    this.isChartAxisItemsLoaded = true;
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

        private getCompanies(): void {
            this.showLoader();

            this.$http.get(
                this.getRegetRootUrl() + "Statistics/GetUserCompanies?&t=" + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    this.userCompanies = tmpData;
                    this.isCompaniesLoaded = true;
                    if (!this.isValueNullOrUndefined(this.userCompanies)
                        && this.userCompanies.length === 1) {
                        this.userCompanies[0].is_selected = true;
                    }

                    this.getCurrencies();

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

        private getCurrencies(): void {
            this.showLoader();

            this.$http.get(
                this.getRegetRootUrl() + "Statistics/GetCurrencies?&t=" + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    this.currenciesList = tmpData;
                    this.isCurrenciesLoaded = true;

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

        private getFilterXItems(companyId: number) : void {
            this.isError = false;

            if (this.isValueNullOrUndefined(this.userCompanies)) {
                return;
            }

            this.showLoaderBoxOnly(this.isError);
            try {

                var selCompanies: string = null;
                if (this.isValueNullOrUndefined(companyId)) {
                    this.filterItems !== null;

                    selCompanies = this.getSelectedCompanies();

                    if (selCompanies.length === 0) {
                        this.hideLoaderWrapper();
                        return;
                    }
                } else {
                    selCompanies = companyId.toString();
                }

                var urlLink = "";
                if (this.selectedAxisXItem === this.FILTER_REQUESTOR) {
                    urlLink = this.getRegetRootUrl() + "/Statistics/GetFilterRequestors?companies=" + selCompanies +
                        '&isActiveOnly=' + this.isActiveOnly +
                        '&t=' + new Date().getTime();
                } else if (this.selectedAxisXItem === this.FILTER_ORDERER) {
                    urlLink = this.getRegetRootUrl() + "/Statistics/GetFilterOrderers?companies=" + selCompanies +
                        '&isActiveOnly=' + this.isActiveOnly +
                        '&t=' + new Date().getTime();
                } else if (this.selectedAxisXItem === this.FILTER_AREA) {
                    urlLink = this.getRegetRootUrl() + "/Statistics/GetFilterAreas?companies=" + selCompanies +
                        '&isActiveOnly=' + this.isActiveOnly +
                        '&t=' + new Date().getTime();
                } else if (this.selectedAxisXItem === this.FILTER_CENTER) {
                    urlLink = this.getRegetRootUrl() + "/Statistics/GetFilterCentres?companies=" + selCompanies +
                        '&isActiveOnly=' + this.isActiveOnly +
                        '&t=' + new Date().getTime();
                } else if (this.selectedAxisXItem === this.FILTER_SUPPLIER) {
                    urlLink = this.getRegetRootUrl() + "/Statistics/GetFilterSuppliers?companies=" + selCompanies +
                        '&isActiveOnly=' + this.isActiveOnly +
                        '&t=' + new Date().getTime();
                } else if (this.selectedAxisXItem === this.FILTER_COMMODITY) {
                    urlLink = this.getRegetRootUrl() + "/Statistics/GetFilterCommodities?companies=" + selCompanies +
                        '&isActiveOnly=' + this.isActiveOnly +
                        '&t=' + new Date().getTime();
                }
            } catch (ex) {
                this.displayErrorMsg();
            } finally {
                this.hideLoaderWrapper();
            }

            if (this.isStringValueNullOrEmpty(urlLink)) {
                return;
            }

            this.showLoaderBoxOnly(this.isError);

            this.$http.get(
                urlLink,
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    if (this.isValueNullOrUndefined(companyId)) {
                        this.filterItems = tmpData;
                    } else {
                        if (this.isValueNullOrUndefined(this.filterItems)) {
                            this.filterItems = tmpData;
                        } else {
                            this.filterItems = this.filterItems.concat(tmpData);
                            this.filterItems = this.$filter('orderBy')(this.filterItems, 'name');
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


        private getChartData(): void {
            if (!this.isChartDataValid()) {
                this.displayErrorMsg(this.locEnterMandatoryValuesText + ". " + this.chartErrorMsg);
                return;
            }

            if (this.isCompanyAllowed() === false) {
                return;
            }

            this.isError = false;
            this.showLoaderBoxOnly(this.isError);

            this.$http.get(
                this.getRegetRootUrl() + "Statistics/GetChartData?" +
                "strDateFrom=" + this.convertDateToDbString(this.filterFromDate) +
                "&strDateTo=" + this.convertDateToDbString(this.filterToDate) +
                "&companiesList=" + this.getSelectedCompanies() +
                "&xItemType=" + this.selectedAxisXItem +
                "&yItemType=" + this.selectedAxisYItem +
                "&periodType=" + this.selectedAxisXPeriodItem +
                "&xItemList=" + this.getSelectedFilterItems() +
                "&currencyId=" + '-1' +
                "&iTop=" + this.selectedAxisXTop +
                "&isActiveOnly=" + this.isActiveOnly +
                "&t=" + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    var tmpChartItems : ChartItems = tmpData;
                    var errMsg: string = tmpChartItems.ErrMsg;
                    if (this.isStringValueNullOrEmpty(errMsg)) {
                        if (this.isValueNullOrUndefined(this.chartItems)) {
                            this.toggleChartSetings(true);
                            var divBody: HTMLDivElement = <HTMLDivElement>document.getElementById("divbody");
                            divBody.scrollTop = 1;
                            //$("#divbody").scrollTop = 1;
                        }

                        this.chartItems = tmpData;
                                                
                        this.displayChart();
                        this.chartDataDonut = null;
                        this.chartOptionsDonut = null;

                        this.selectedCurrencyId = this.chartItems.CurrencyId;
                        this.origCurrencyId = this.chartItems.CurrencyId;
                    } else {
                        this.displayErrorMsg(errMsg);
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

        //***************************************************************

        //***************************************************************
        //Methods
        private loadData() : void {
           
            this.rootUrl = this.getRegetRootUrl();
            this.getAxisItems();
            
            this.getCompanies();
            
        }

        private getAxisItems(): void {
            try {
                this.getAxisXItems();
                this.getAxisXTopsItems();
                                
                this.axisYItemsList = [];
                this.axisYItemsList.push({ id: 10, name: this.locPriceText, is_disabled: false});
                this.axisYItemsList.push({ id: 20, name: this.locNumOfRequestsText, is_disabled: false });

                this.axisXPeriodItemsList = [];
                this.axisXPeriodItemsList.push({ id: 10, name: this.locAllText, is_disabled: false });
                this.axisXPeriodItemsList.push({ id: 20, name: this.locYearText, is_disabled: false });
                this.axisXPeriodItemsList.push({ id: 30, name: this.locMonthText, is_disabled: false });

                this.getChartTypeItems();
            } catch (e) {
                this.displayErrorMsg();
            }
        }

        private getAxisXTopsItems() : void {

            this.axisXTopList = [];
            this.axisXTopList = [];
            this.axisXTopList.push({ id: 0, name: this.locAllText, is_disabled: false });
            this.axisXTopList.push({ id: 10, name: "10", is_disabled: false });
            this.axisXTopList.push({ id: 20, name: "20", is_disabled: false });
            this.axisXTopList.push({ id: 50, name: "50", is_disabled: false });
            this.axisXTopList.push({ id: 100, name: "100", is_disabled: false });
                        
            this.selectedAxisXTop = 0;
        }

        private getChartTypeItems(): void {
            this.chartTypeItemsList = [];

            this.chartTypeItemsList.push({ id: 20, name: 'Bar', is_disabled: false});
            this.chartTypeItemsList.push({ id: 10, name: 'Line', is_disabled: false });
            this.chartTypeItemsList.push({ id: 40, name: 'Pie', is_disabled: false });
            this.chartTypeItemsList.push({ id: 50, name: 'Doughnut', is_disabled: false });
            this.chartTypeItemsList.push({ id: 30, name: 'Radar', is_disabled: false });
            this.chartTypeItemsList.push({ id: 60, name: 'Polar Area', is_disabled: false });
            //$scope.ChartTypeItemsList.push({ id: 70, name: 'Bubble' });
            //$scope.ChartTypeItemsList.push({ id: 80, name: 'Scatter' });

        }

        private isAllCompaniesChecked() : boolean {
            var isSelectedAll = true;
            if (this.isValueNullOrUndefined(this.userCompanies)) {
                return false;
            }
            for (var i = 0; i < this.userCompanies.length; i++) {
                if (!this.userCompanies[i].is_selected) {
                    isSelectedAll = false;
                    break;
                }
            }

            return isSelectedAll;
        }

        private isCompaniesIndeterminate() : boolean {
            if (this.isValueNullOrUndefined(this.userCompanies)) {
                return false;
            }

            return (this.userCompanies.length !== 0 &&
                !this.isAllCompaniesChecked());
        }

        private toggleAllCompanies() : void {

            if (this.isValueNullOrUndefined(this.userCompanies)) {
                return;
            }

            if (this.isAllCompaniesChecked()) {
                for (var i = 0; i < this.userCompanies.length; i++) {
                    this.userCompanies[i].is_selected = false;
                }
            } else {
                for (var j = 0; j < this.userCompanies.length; j++) {
                    this.userCompanies[j].is_selected = true;
                }
            }
        }

        private toggleCompany(item : StatisticsCompany) {
            if (item.is_selected) {
                item.is_selected = false;
                this.removeFilterItems(item.id);
            } else {
                item.is_selected = true;
                this.addFilterItems(item.id);
            }

            this.setAxisXItem();
        }

        private addFilterItems(companyId: number) {
            if (this.selectedAxisXItem === this.FILTER_COMMODITY) {
                //commodity
                this.getFilterXItems(null);
            } else {
                this.getFilterXItems(companyId);
            }
        }

        private removeFilterItems(companyId: number) : void {
            this.isError = false;
            if (this.isValueNullOrUndefined(this.filterItems)) {
                return;
            }
            this.showLoaderBoxOnly(this.isError);
            try {
                if (this.selectedAxisXItem === this.FILTER_COMMODITY) {
                    //commodity
                    this.getFilterXItems(null);
                } else {
                    for (var i = this.filterItems.length - 1; i >= 0; i--) {
                        if (this.filterItems[i].company_id === companyId) {
                            this.filterItems.splice(i, 1);
                        }
                    }
                }
            } catch (ex) {
                this.displayErrorMsg();
            } finally {
                this.hideLoaderWrapper();
            }
        }

        private getSelectedCompanies() : string {
            var selCompanies = "";
            for (var i = 0; i < this.userCompanies.length; i++) {
                if (this.userCompanies[i].is_selected) {
                    if (selCompanies.length > 0) {
                        selCompanies += "|";
                    }
                    selCompanies += this.userCompanies[i].id;
                }
            }

            return selCompanies;
        }

        private setAxisYItem () : void {
            this.isCost = (this.selectedAxisYItem === 10);
        }

        private setAxisXPeriodItem() : void{
        }

        private setAxisXTopItem(): void {
        }

        private setAxisXItem(): void {
            //$scope.xFilterMsg = [];
            this.isFilterUserOnly = false;

            this.getFilterXItems(null);

            if (this.selectedAxisXItem === this.FILTER_REQUESTOR) {
                this.filterPlaceholder = this.locSelectRequestorsText;
                this.isSelectXItemsAvailable = true;
            } else if (this.selectedAxisXItem === this.FILTER_ORDERER) {
                this.filterPlaceholder = this.locOrdererText;
                this.isSelectXItemsAvailable = true;
            } else if (this.selectedAxisXItem === this.FILTER_AREA) {
                this.filterPlaceholder = this.locAreaText;
                this.isSelectXItemsAvailable = true;
            } else if (this.selectedAxisXItem === this.FILTER_CENTER) {
                this.filterPlaceholder = this.locCentreText;
                this.isSelectXItemsAvailable = true;
            } else if (this.selectedAxisXItem === this.FILTER_SUPPLIER) {
                this.filterPlaceholder = this.locSupplierText;
                this.isSelectXItemsAvailable = true;
            } else if (this.selectedAxisXItem === this.FILTER_COMMODITY) {
                this.filterPlaceholder = this.locPurchaseGroupText;
                this.isSelectXItemsAvailable = true;
            } else {
                this.isSelectXItemsAvailable = false;
            }

            this.setXFilterAuthorization();

            this.isSelectedXItems = false;
        }

        private setXFilterAuthorization() : void {
            this.isFilterUserOnly = true;

            var selCompanies: string = this.getSelectedCompanies();

            if (this.isStringValueNullOrEmpty(selCompanies)) {
                return;
            }

            var selCompaniesIds: string[] = selCompanies.split('|');
            for (var i = 0; i < selCompaniesIds.length; i++) {
                var comp: StatisticsCompany[] = this.$filter("filter")(this.userCompanies, { id: parseInt(selCompaniesIds[i]) }, true);

                if (this.selectedAxisXItem === this.FILTER_REQUESTOR) {
                    if (comp[0].is_user_company_stat_requestor !== true) {
                        this.xFilterMsg.push({ company: comp[0].country_code, msg: this.locNotAuthorizedText });
                    } else if (comp[0].is_user_company_stat_admin !== true &&
                        comp[0].is_user_company_stat_cgadmin !== true) {
                        this.xFilterMsg.push({ company: comp[0].country_code, msg: this.locStatisticsAuthorizedOwnRequestOnlyText });
                    } else if (comp[0].is_user_company_stat_admin !== true &&
                        comp[0].is_user_company_stat_cgadmin === true) {
                        this.xFilterMsg.push({ company: comp[0].country_code, msg: this.locStatisticsAuthorizedCgRequestOnlyText });
                        this.isFilterUserOnly = false;
                    } else {
                        this.isFilterUserOnly = false;
                    }
                } else if (this.selectedAxisXItem === this.FILTER_ORDERER) {
                    if (comp[0].is_user_company_stat_orderer !== true) {
                        this.xFilterMsg.push({ company: comp[0].country_code, msg: this.locNotAuthorizedText });
                    } else if (comp[0].is_user_company_stat_admin !== true &&
                        comp[0].is_user_company_stat_cgadmin !== true) {
                        this.xFilterMsg.push({ company: comp[0].country_code, msg: this.locStatisticsAuthorizedOwnRequestOnlyText });
                    } else if (comp[0].is_user_company_stat_admin !== true &&
                        comp[0].is_user_company_stat_cgadmin === true) {
                        this.xFilterMsg.push({ company: comp[0].country_code, msg: this.locStatisticsAuthorizedCgRequestOnlyText });
                        this.isFilterUserOnly = false;
                    } else {
                        this.isFilterUserOnly = false;
                    }
                } else if (this.selectedAxisXItem === this.FILTER_AREA) {
                    if (comp[0].is_user_company_stat_cgadmin !== true) {
                        this.xFilterMsg.push({ company: comp[0].country_code, msg: this.locNotAuthorizedText });
                    } else if (comp[0].is_user_company_stat_admin !== true &&
                        comp[0].is_user_company_stat_cgadmin === true) {
                        this.xFilterMsg.push({ company: comp[0].country_code, msg: this.locStatisticsAuthorizedCgRequestOnlyText });
                        this.isFilterUserOnly = false;
                    } else {
                        this.isFilterUserOnly = false;
                    }
                } else if (this.selectedAxisXItem === this.FILTER_CENTER) {
                    if (comp[0].is_user_company_stat_cgadmin !== true) {
                        this.xFilterMsg.push({ company: comp[0].country_code, msg: this.locNotAuthorizedText });
                    } else if (comp[0].is_user_company_stat_admin !== true &&
                        comp[0].is_user_company_stat_cgadmin === true) {
                        this.xFilterMsg.push({ company: comp[0].country_code, msg: this.locStatisticsAuthorizedCgRequestOnlyText });
                        this.isFilterUserOnly = false;
                    } else {
                        this.isFilterUserOnly = false;
                    }
                } else if (this.selectedAxisXItem === this.FILTER_SUPPLIER) {
                    if (comp[0].is_user_company_stat_admin !== true) {
                        this.xFilterMsg.push({ company: comp[0].country_code, msg: this.locNotAuthorizedText });
                    } else {
                        this.isFilterUserOnly = false;
                    }
                } else if (this.selectedAxisXItem === this.FILTER_COMMODITY) {
                    if (comp[0].is_user_company_stat_admin !== true) {
                        this.xFilterMsg.push({ company: comp[0].country_code, msg: this.locNotAuthorizedText });
                    } else {
                        this.isFilterUserOnly = false;
                    }
                }
            }
        }

        private searchFilterItems(searchText: string): StatisticsFilter[] {

            var searchFilterItems: StatisticsFilter[] = [];

            if (this.isStringValueNullOrEmpty(searchText)) {
                return this.filterItems;
            }

            angular.forEach(this.filterItems, (filterItem: StatisticsFilter) => {
                var searchKeyLower: string = searchText.trim().toLowerCase();
                if (!this.isStringValueNullOrEmpty(filterItem.name) && filterItem.name.toLowerCase().indexOf(searchKeyLower) > -1 ||
                    !this.isStringValueNullOrEmpty(filterItem.name_search_key) && filterItem.name_search_key.trim().toLowerCase().indexOf(searchKeyLower) > -1) {
                    searchFilterItems.push(filterItem);
                }
            });
            return searchFilterItems;

        }

        private filterSelectedItemChange(item : StatisticsFilter) {

            if (this.isValueNullOrUndefined(item)) {
                return;
            }

            var filterItem = this.$filter("filter")(this.filterItems, { id: item.id }, true);
            filterItem[0].is_selected = true;
            this.isSelectedXItems = true;

            var $autWrap = $("#acXFilter").children().first();
            var $autChild = $autWrap.children().first();
            $autChild.val('');

            this.searchstringitem = "";
            this.filterselecteditem = null;

            if (!this.isValueNullOrUndefined(this.angScopeAny.$$childTail.searchstringitem)) {
                this.angScopeAny.$$childTail.searchText = '';
                this.angScopeAny.$$childTail.searchstringitem = '';
            } else {
                //we need to find out the right one
                var prevSibl = this.angScopeAny.$$childTail.$$prevSibling;
                while (!this.isValueNullOrUndefined(prevSibl)) {
                    if (!this.isValueNullOrUndefined(prevSibl.searchstringitem)) {
                        prevSibl.searchstringitem = '';
                        break;
                    } else {
                        prevSibl = prevSibl.$$prevSibling;
                    }
                }

            }
            //var msg : string = this.locItemWasAddedToFilter.replace('{0}', item.name);
            //this.showToast(msg);

        }

        private filterSelectAll() : void {
            if (this.isCompanySelected() === false) {
                this.showAlert(this.locSelectCompanyText, this.locSelectCompanyText, this.locCloseText);
                return;
            }

            if (this.isValueNullOrUndefined(this.filterItems)) {
                return;
            }

            this.showLoaderBoxOnly(this.isError);
            try {
                for (var i: number = 0; i < this.filterItems.length; i++) {
                    this.filterItems[i].is_selected = true;
                }
                this.isSelectedXItems = true;
            } catch (ex) {
                this.displayErrorMsg();
            } finally {
                this.hideLoaderWrapper();
            }

        }

        private filterRemoveAll() : void {
            if (this.isValueNullOrUndefined(this.filterItems)) {
                return;
            }

            this.showLoaderBoxOnly(this.isError);
            try {
                for (var i = 0; i < this.filterItems.length; i++) {
                    this.filterItems[i].is_selected = false;
                }
                this.isSelectedXItems = false;
            } catch (ex) {
                this.displayErrorMsg();
            } finally {
                this.hideLoaderWrapper();
            }
        }

        private deleteFilterItem(iItemId : number) : void {
            var item = this.$filter("filter")(this.filterItems, { id: iItemId }, true);

            if (!this.isValueNullOrUndefined(item[0])) {
                item[0].is_selected = false;
            }

            for (var i = 0; i < this.filterItems.length; i++) {
                if (this.filterItems[i].is_selected === true) {
                    return;
                }
            }
            this.isSelectedXItems = false;
        }
        
        private isCompanySelected() : boolean {
            var isCompSelected : boolean = false;
            for (var i: number = 0; i < this.userCompanies.length; i++) {
                if (this.userCompanies[i].is_selected) {
                    isCompSelected = true;
                    break;
                }
            }

            return isCompSelected;
        }

        private toggleActiveOnly() : void {
            this.isActiveOnly = !this.isActiveOnly;
            this.getFilterXItems(null);
        }

        private isChartDataValid(): boolean {
            
            let isValid = true;

            //this line Validates all of the fields
            this.validateForm(this.angScopeAny.frmStatistics);
            this.chartErrorMsg = "";

            //alert(this.angScopeAny.frmStatistics.dtpFromDate.$valid);
            if (this.isValueNullOrUndefined(this.filterFromDate)
                || this.angScopeAny.frmStatistics.dtpFromDate.$valid == false) {
                this.angScopeAny.frmStatistics.dtpFromDate.$setTouched();
                if (!this.isStringValueNullOrEmpty(this.chartErrorMsg)) {
                    this.chartErrorMsg += ";";
                }
                this.chartErrorMsg += this.locSelectDateFromText;
                isValid = false;
            }

            if (this.isValueNullOrUndefined(this.filterToDate)
                || this.angScopeAny.frmStatistics.dtpToDate.$valid == false) {
                if (!this.isStringValueNullOrEmpty(this.chartErrorMsg)) {
                    this.chartErrorMsg += ";";
                }
                this.chartErrorMsg += this.locSelectDateToText;
                isValid = false;
            }

            if (this.isValueNullOrUndefined(this.userCompanies)) {
                if (!this.isStringValueNullOrEmpty(this.chartErrorMsg)) {
                    this.chartErrorMsg += ";";
                }
                this.chartErrorMsg += this.locSelectCompanyText;
                isValid = false;
            }

            var isCompSelected = this.isCompanySelected();
            
            if (isCompSelected === false) {
                if (!this.isStringValueNullOrEmpty(this.chartErrorMsg)) {
                    this.chartErrorMsg += ";";
                }
                this.chartErrorMsg += this.locSelectCompanyText;
                isValid = false;
            }

            if (this.isValueNullOrUndefined(this.selectedAxisYItem)) {
                this.chartErrorMsg += this.locSelectAxisYText;
                isValid = false;
            }

            if (this.isValueNullOrUndefined(this.selectedAxisXItem)) {
                if (!this.isStringValueNullOrEmpty(this.chartErrorMsg)) {
                    this.chartErrorMsg += ";";
                }
                this.chartErrorMsg += this.locSelectAxisXText;
                return false;
            }

            if (this.isValueNullOrUndefined(this.selectedAxisXPeriodItem)) {
                if (!this.isStringValueNullOrEmpty(this.chartErrorMsg)) {
                    this.chartErrorMsg += ";";
                }
                this.chartErrorMsg += this.locSelectPeriodText;
                isValid = false;
            }

            return isValid;
        }

        private isCompanyAllowed() : boolean {
            var selCompanies: string = this.getSelectedCompanies();
            if (this.isValueNullOrUndefined(selCompanies)) {
                return true;
            }

            var selCompaniesIds = selCompanies.split('|');
            for (var i = 0; i < selCompaniesIds.length; i++) {
                var comp = this.$filter("filter")(this.userCompanies, { id: parseInt(selCompaniesIds[i]) }, true);

                if (this.selectedAxisXItem === this.FILTER_REQUESTOR) {
                    if (comp[0].is_user_company_stat_requestor !== true) {
                        var msg: string = this.locStatisticsRequestorNotAllowedText.replace("{0}", comp[0].country_code) + ".";
                        this.displayErrorMsg(msg);
                        return false;
                    }
                }

                if (this.selectedAxisXItem === this.FILTER_ORDERER) {
                    if (comp[0].is_user_company_stat_orderer !== true) {
                        var msg: string = this.locStatisticsOrdererNotAllowedText.replace("{0}", comp[0].country_code) + ".";
                        this.displayErrorMsg(msg);
                        return false;
                    }
                }

                if (this.selectedAxisXItem === this.FILTER_APPROVE_MAN) {
                    if (comp[0].is_user_company_stat_appman !== true) {
                        var msg: string = this.locStatisticsAppManNotAllowedText.replace("{0}", comp[0].country_code) + ".";
                        this.displayErrorMsg(msg);
                        return false;
                    }
                }

                if (this.selectedAxisXItem === this.FILTER_CENTER) {
                    if (comp[0].is_user_company_stat_admin !== true && comp[0].is_user_company_stat_cgadmin !== true) {
                        var msg: string = this.locStatisticsCentreNotAllowedText.replace("{0}", comp[0].country_code) + ".";
                        this.displayErrorMsg(msg);
                        return false;
                    }
                }

                if (this.selectedAxisXItem === this.FILTER_AREA) {
                    if (comp[0].is_user_company_stat_admin !== true && comp[0].is_user_company_stat_cgadmin !== true) {
                        var msg: string = this.locStatisticsAreaNotAllowedText.replace("{0}", comp[0].country_code) + ".";
                        this.displayErrorMsg(msg);
                        return false;
                    }
                }

                if (this.selectedAxisXItem === this.FILTER_COMMODITY) {
                    if (comp[0].is_user_company_stat_admin !== true) {
                        var msg: string = this.locStatisticsPgNotAllowedText.replace("{0}", comp[0].country_code) + ".";
                        this.displayErrorMsg(msg);
                        return false;
                    }
                }

                if (this.selectedAxisXItem === this.FILTER_SUPPLIER) {
                    if (comp[0].is_user_company_stat_admin !== true) {
                        var msg: string = this.locStatisticsSupplierNotAllowedText.replace("{0}", comp[0].country_code) + ".";
                        this.displayErrorMsg(msg);
                        return false;
                    }
                }

                
            }

            return true;
        }

        private getSelectedFilterItems() : string {
            if (this.isValueNullOrUndefined(this.filterItems)) {
                return "";
            }

            var selItems: string = "";
            var isAll: boolean = true;
            for (var i: number = 0; i < this.filterItems.length; i++) {
                if (this.filterItems[i].is_selected === true) {
                    if (selItems.length > 0) {
                        selItems += ";"
                    }

                    selItems += this.filterItems[i].id;
                } else {
                    isAll = false;
                }
            }

            if (isAll === true) {
                return "";
            }

            return selItems;
        }

        private toggleChartSetings(isHide: boolean) {
            if ($("#divChartSettings").is(':visible') || isHide) {
                $("#divChartSettings").slideUp();
                $("#imgCgExpand").attr("src", this.getRegetRootUrl() + "Content/Images/Panel/Expand.png");
                $("#imgCgExpand").attr("title", "@Html.Raw(RequestResource.Display)");
            } else {
                $("#divChartSettings").show("slow");
                $("#imgCgExpand").attr("src", this.getRegetRootUrl() + "Content/Images/Panel/Collapse.png");
                $("#imgCgExpand").attr("title", "@Html.Raw(RequestResource.Hide)");
            }
        }

        private displayChart() : void {
            $("#chartWrap").show();

            var chartCanvas: any = document.getElementById("regetChart");
            var ctx: CanvasRenderingContext2D = chartCanvas.getContext('2d');

            var data = {};
            var options = {};

            if (this.isValueNullOrUndefined(this.chartItems)) {
                return;
            }

            this.chartData = this.chartItems.ChartData;
            this.chartOptions = this.chartItems.ChartOptions;

            if (!this.isValueNullOrUndefined(this.regetChart)) {
                this.regetChart.destroy();
            }

            if (this.isValueNullOrUndefined(this.selectedChartTypeItem)) {
                this.selectedChartTypeItem = 20;
            }
                        
            this.setChartType();
        }

        private setChartType() : void {
            
            if (!this.isValueNullOrUndefined(this.regetChart)) {
                this.regetChart.destroy();
            }

            var chartCanvas: any = document.getElementById("regetChart");
            var ctx: CanvasRenderingContext2D = chartCanvas.getContext('2d');

            var isDonut: boolean = false;
            
            if (this.selectedChartTypeItem === 40) {
                //chartType = "pie";
                isDonut = true;
            } else if (this.selectedChartTypeItem === 50) {
                //chartType = "doughnut";
                isDonut = true;
            } else if (this.selectedChartTypeItem === 60) {
                //chartType = "polarArea";
                isDonut = true;
            } 

            var chartType: string = this.getChartType();
            if (isDonut === true) {
                this.chartDataDonut = { datasets: [], labels: [] };
                for (var iDs: number = 0; iDs < this.chartData.datasets[0].data.length; iDs++) {
                    this.chartDataDonut.datasets.push({ data: [], backgroundColor: [], label: "" })
                }
                for (var i: number = 0; i < this.chartDataDonut.datasets.length; i++) {
                    for (var j = 0; j < this.chartData.datasets.length; j++) {
                        var sourceData: number = this.chartData.datasets[j].data[i] as number;
                        var targetData: number[] = this.chartDataDonut.datasets[i].data as number[];
                        targetData.push(sourceData);
                        this.chartDataDonut.datasets[i].backgroundColor.push(this.chartData.datasets[j].backgroundColor);
                        
                    }
                }

                for (var k = 0; k < this.chartData.datasets.length; k++) {
                    this.chartDataDonut.labels.push(this.chartData.datasets[k].label);
                }

                this.chartOptionsDonut = angular.copy(this.chartOptions);
                this.chartOptionsDonut.scales = null;
                                
                this.regetChart = new Chart(ctx, {
                    type: chartType,
                    data: this.chartDataDonut,
                    options: this.chartOptionsDonut
                });
            } else {

                this.regetChart = new Chart(ctx, {
                    type: chartType,
                    data: this.chartData,
                    options: this.chartOptions
                });
            }

            this.setChartWidth();
        }

        private getChartType() : string {
            
            if (this.selectedChartTypeItem === 10) {
                return "line";
            } else if (this.selectedChartTypeItem === 20) {
                return "bar";
            } else if (this.selectedChartTypeItem === 30) {
                return "radar";
            } else if (this.selectedChartTypeItem === 40) {
                return "pie";

            } else if (this.selectedChartTypeItem === 50) {
                return "doughnut";

            } else if (this.selectedChartTypeItem === 60) {
                return "polarArea";

            }

            return "bar";
        }

        private setChartWidth() : void {
            var height: number = 500;
            var labelsLength: number = 0;
            for (var i: number = 0; i < this.chartData.datasets.length; i++) {
                labelsLength += this.chartData.datasets[i].label.length + 7;
            }


            height = labelsLength / 6;
            if (height < 500) {
                height = 500;
            }

            var chart = $("#regetChart");
           
            chart.attr("style", "height:" + height + "px;max-height:" + height + "px;");
        }

        private setCurrency() : void {
            this.recalculate();
        }

        private recalculate() {
            this.isError = false;
            try {
                var exchangeRate: number = null;
                var filterItem : ExchangeRate[] = this.$filter("filter")(this.chartItems.ExchangeRates, { source_currency_id: this.origCurrencyId, destin_currency_id: this.selectedCurrencyId }, true);
                if (!this.isValueNullOrUndefined(filterItem) && filterItem.length > 0) {
                    exchangeRate = 1 / filterItem[0].exchange_rate1;
                }

                if (exchangeRate === null) {
                    var filterItemEx = this.$filter("filter")(this.chartItems.ExchangeRates, { source_currency_id: this.selectedCurrencyId, destin_currency_id: this.origCurrencyId }, true);
                    if (!this.isValueNullOrUndefined(filterItemEx) && filterItemEx.length > 0) {
                        exchangeRate = filterItemEx[0].exchange_rate1;
                    }
                }

                if (exchangeRate === null) {
                    let filterCurrFrom: Currency[] = this.$filter("filter")(this.currenciesList, { id: this.origCurrencyId }, true);
                    let filterCurrToEx1: Currency[] = this.$filter("filter")(this.currenciesList, { id: this.selectedCurrencyId }, true);
                    let curr1: string = filterCurrFrom[0].currency_code;
                    let curr2: string = filterCurrToEx1[0].currency_code;
                    let msg: string = this.locExchangeRateMissingText.replace('{0}', curr1).replace('{1}', curr2);
                    this.displayErrorMsg(msg);

                    this.selectedCurrencyId = this.origCurrencyId;

                    return;
                }

                var recChartData : KsLineChartData = angular.copy(this.chartData);
                for (var i = 0; i < recChartData.datasets.length; i++) {
                    for (var j = 0; j < recChartData.datasets[i].data.length; j++) {
                        if (recChartData.datasets[i].data[j] === 0) {
                            continue;
                        }
                        recChartData.datasets[i].data[j] = Math.round((recChartData.datasets[i].data[j] as number) * exchangeRate);
                    }
                }
                var filterCurrTo : Currency[] = this.$filter("filter")(this.currenciesList, { id: this.selectedCurrencyId }, true);
                var curr: string = filterCurrTo[0].currency_code;
                this.replaceCurrencyTitle(curr);
                this.chartData = recChartData;

                if (!this.isValueNullOrUndefined(this.chartDataDonut)) {
                    var recChartDataDonut: KsPieChartData = angular.copy(this.chartDataDonut);
                    for (var i: number = 0; i < recChartDataDonut.datasets.length; i++) {
                        for (var j: number = 0; j < recChartDataDonut.datasets[i].data.length; j++) {
                            if (recChartDataDonut.datasets[i].data[j] === 0) {
                                continue;
                            }
                            recChartDataDonut.datasets[i].data[j] = Math.round((recChartDataDonut.datasets[i].data[j] as number) * exchangeRate);
                        }
                    }
                    this.chartDataDonut = recChartDataDonut;
                }

                //recalculate table
                for (var i: number = 0; i < this.chartItems.ChartData.datasets.length; i++) {
                    for (var k = 0; k < this.chartItems.ChartData.datasets[i].data.length; k++) {
                        this.chartItems.ChartData.datasets[i].data[k] = Math.round((this.chartItems.ChartData.datasets[i].data[k] as number) * exchangeRate);
                    }
                }

                this.setChartType();

                this.origCurrencyId = this.selectedCurrencyId;
            } catch (e) {
                this.selectedCurrencyId = this.origCurrencyId;
                this.displayErrorMsg();
            }
        }

        private replaceCurrencyTitle(currency: string) {
            try {
                //title
                var tmpTitle: string = this.chartOptions.title.text;
                var iStart: number = tmpTitle.indexOf('(');
                var iStop: number = tmpTitle.indexOf(')');

                tmpTitle = tmpTitle.substring(0, iStart + 1) + currency + tmpTitle.substring(iStop);
                this.chartOptions.title.text = tmpTitle;

                if (!this.isValueNullOrUndefined(this.chartOptionsDonut)) {
                    this.chartOptionsDonut.title.text = tmpTitle;
                }

                //scales
                var tmpAxisTitle = this.chartOptions.scales.yAxes[0].scaleLabel.labelString;
                iStart = tmpAxisTitle.indexOf('(');
                iStop = tmpAxisTitle.indexOf(')');

                tmpAxisTitle = tmpAxisTitle.substring(0, iStart + 1) + currency + tmpAxisTitle.substring(iStop);
                this.chartOptions.scales.yAxes[0].scaleLabel.labelString = tmpAxisTitle;
            } catch (e) {
                this.displayErrorMsg();
            }
        }

        private changeChartColors(): void {
            try {
                if (this.isValueNullOrUndefined(this.regetChart)) {
                    return;
                }

                for (var i: number = 0; i < this.chartData.datasets.length; i++) {
                    this.chartData.datasets[i].backgroundColor = this.getRandomColor();
                    this.chartData.datasets[i].borderColor = this.chartData.datasets[i].backgroundColor;
                }

                this.regetChart.destroy();

                var chartCanvas: any = document.getElementById("regetChart");
                var ctx: CanvasRenderingContext2D = chartCanvas.getContext('2d');

                this.setChartType();
            } catch (e) {
                this.displayErrorMsg();
            }
        }

        private getRandomColor() : string {
            var letters: string = "0123456789ABCDEF";
            var color: string = "#";
            for (var i: number = 0; i < 6; i++) {
                color += letters[Math.floor(Math.random() * 16)];
            }
            return color;
        }

        private exportToExcel() : void {
            var url = this.getRegetRootUrl() + "Report/GetChart?" +
                "strDateFrom=" + this.convertDateToDbString(this.filterFromDate) +
                "&strDateTo=" + this.convertDateToDbString(this.filterToDate) +
                "&companiesList=" + this.getSelectedCompanies() +
                "&xItemType=" + this.selectedAxisXItem +
                "&yItemType=" + this.selectedAxisYItem +
                "&periodType=" + this.selectedAxisXPeriodItem +
                "&xItemList=" + this.getSelectedFilterItems() +
                "&currencyId=" + this.selectedCurrencyId +
                "&chartType=" + this.selectedChartTypeItem +
                "&iTop=" + this.selectedAxisXTop +
                "&isActiveOnly=" + this.isActiveOnly +
                "&t=" + new Date().getTime();
            window.open(url);

        }

        private getSelectedImgUrl(): string {
            return this.getRegetRootUrl() + "Content/Images/Controll/CheckGreen16.png";
        }

        private hideLoaderWrapper() : void {
            if (this.isError || (this.isChartAxisItemsLoaded
                && this.isCompaniesLoaded && this.isCurrenciesLoaded)) {
                this.hideLoader();
            }
        }

        //***************************************************************

    }
    angular.
        module('RegetApp').
        controller('StatisticsController', Kamsyk.RegetApp.StatisticsController).
        config(function ($mdDateLocaleProvider) { //only because of Date picker is implemented
            this.SetDatePicker($mdDateLocaleProvider);
            //it is neccessary to set IsGenerateDatePickerLocalization = true
        });

    export class StatisticsCompany {
        public id: number = null;
        public country_code: string = null; 
        public is_user_company_stat_admin: boolean = false;
        public is_user_company_stat_requestor: boolean = false;
        public is_user_company_stat_orderer: boolean = false;
        public is_user_company_stat_appman: boolean = false;
        public is_user_company_stat_cgadmin: boolean = false;
        public is_selected: boolean = false;
    }

    export class StatisticsFilter {
        public id: number = null;
        public name: string = null;
        public name_search_key: string = null;
        public company_id: number = null;
        public is_selected: boolean = null;
        public flag_url: string = null;
    }

    export class FilterMsg {
        public company: string = null;
        public msg: string = null;
    }

    export class ChartItems {
        //must be name like this
        public ChartData: KsLineChartData = null;
        public ChartOptions: ChartOptions = null;
        public CurrencyId: number = null;
        public ErrMsg: string = null;
        public ExchangeRates: ExchangeRate[] = null;
    }

    
}