/// <reference path="../typings/chart.js/index.d.ts" />
/// <reference path="../typings/kamsyk-material/chart.d.ts" />
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
                    if (!Array.isArray(array))
                        return array;
                    if (!sortPredicate)
                        return array;
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
        var StatisticsController = /** @class */ (function (_super) {
            __extends(StatisticsController, _super);
            //***********************************************************************
            //**********************************************************
            //Constructor
            function StatisticsController($scope, $http, $filter, $mdDialog, $mdToast, $q, $timeout) {
                var _this = _super.call(this, $scope, $http, $filter, $mdDialog, $mdToast, $q, $timeout) || this;
                _this.$scope = $scope;
                _this.$http = $http;
                _this.$filter = $filter;
                _this.$mdDialog = $mdDialog;
                _this.$mdToast = $mdToast;
                _this.$q = $q;
                _this.$timeout = $timeout;
                //***********************************************************
                //Constants
                _this.FILTER_REQUESTOR = 10;
                _this.FILTER_ORDERER = 20;
                _this.FILTER_SUPPLIER = 30;
                _this.FILTER_CENTER = 40;
                _this.FILTER_AREA = 50;
                _this.FILTER_COMMODITY = 60;
                _this.FILTER_TOTAL = 70;
                _this.FILTER_COMPANY = 80;
                _this.FILTER_APPROVE_MAN = 90;
                //***********************************************************
                //***********************************************************
                //Local Texts
                _this.locWarningText = $("#WarningText").val();
                _this.locErrMsg = $("#ErrMsgText").val();
                _this.locPriceText = $("#PriceText").val();
                _this.locNumOfRequestsText = $("#NumOfRequestsText").val();
                _this.locAllText = $("#AllText").val();
                _this.locYearText = $("#YearText").val();
                _this.locMonthText = $("#MonthText").val();
                _this.locSelectCompanyText = $("#SelectCompanyText").val();
                _this.locSelectDateFromText = $("#SelectDateFromText").val();
                _this.locSelectDateToText = $("#SelectDateToText").val();
                _this.locSelectAxisXText = $("#SelectAxisXText").val();
                _this.locSelectAxisYText = $("#SelectAxisYText").val();
                _this.locSelectPeriodText = $("#SelectPeriodText").val();
                _this.locNotFoundText = $("#NotFoundText").val();
                _this.locSelectRequestorsText = $("#SelectRequestorsText").val();
                _this.locOrdererText = $("#OrdererText").val();
                _this.locApproveManText = $("#ApproveManText").val();
                _this.locCentreText = $("#CentreText").val();
                _this.locSupplierText = $("#SupplierText").val();
                _this.locPurchaseGroupText = $("#PurchaseGroupText").val();
                _this.locAreaText = $("#AreaText").val();
                _this.locExchangeRateMissingText = $("#ExchangeRateMissingText").val();
                _this.locStatisticsAuthorizedOwnRequestOnlyText = $("#StatisticsAuthorizedOwnRequestOnlyText").val();
                _this.locStatisticsAuthorizedCgRequestOnlyText = $("#StatisticsAuthorizedCgRequestOnlyText").val();
                _this.locNotAuthorizedText = $("#NotAuthorizedText").val();
                _this.locItemWasAddedToFilter = $("#ItemWasAddedToFilterText").val();
                _this.locStatisticsRequestorNotAllowedText = $("#StatisticsRequestorNotAllowedText").val();
                _this.locStatisticsOrdererNotAllowedText = $("#StatisticsOrdererNotAllowedText").val();
                _this.locStatisticsAppManNotAllowedText = $("#StatisticsAppManNotAllowedText").val();
                _this.locStatisticsCentreNotAllowedText = $("#StatisticsCentreNotAllowedText").val();
                _this.locStatisticsAreaNotAllowedText = $("#StatisticsAreaNotAllowedText").val();
                _this.locStatisticsPgNotAllowedText = $("#StatisticsPgNotAllowedText").val();
                _this.locStatisticsSupplierNotAllowedText = $("#StatisticsSupplierNotAllowedText").val();
                _this.locEnterMandatoryValuesText = $("#EnterMandatoryValuesText").val();
                //***********************************************************
                //***********************************************************************
                //Properties
                _this.axisXItemsList = null;
                _this.axisXTopList = null;
                _this.axisYItemsList = null;
                _this.axisXPeriodItemsList = null;
                _this.chartTypeItemsList = null;
                _this.isChartAxisItemsLoaded = false;
                _this.isCompaniesLoaded = false;
                _this.isCurrenciesLoaded = false;
                _this.selectedAxisXTop = null;
                _this.rootUrl = null;
                _this.userCompanies = null;
                _this.currenciesList = null;
                _this.selectedAxisXItem = null;
                _this.selectedAxisYItem = null;
                _this.selectedAxisXPeriodItem = null;
                _this.filterItems = null;
                _this.isActiveOnly = false;
                _this.isCost = false;
                _this.isFilterUserOnly = false;
                _this.filterPlaceholder = null;
                _this.isSelectXItemsAvailable = false;
                _this.xFilterMsg = null;
                _this.isSelectedXItems = false;
                _this.searchstringitem = null;
                _this.filterselecteditem = null;
                _this.chartErrorMsg = null;
                _this.filterFromDate = null;
                _this.filterToDate = null;
                _this.chartItems = null;
                _this.chartOptions = null;
                _this.chartOptionsDonut = null;
                _this.regetChart = null;
                _this.selectedChartTypeItem = null;
                _this.chartData = null;
                _this.chartDataDonut = null;
                _this.selectedCurrencyId = null;
                _this.origCurrencyId = null;
                //***************************************************************
                _this.$onInit = function () { };
                _this.loadData();
                return _this;
            }
            //***************************************************************
            //Http Methods
            StatisticsController.prototype.getAxisXItems = function () {
                var _this = this;
                this.showLoaderBoxOnly();
                this.$http.get(this.getRegetRootUrl() + "Statistics/GetXOptions?&t=" + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        _this.axisXItemsList = tmpData;
                        _this.isChartAxisItemsLoaded = true;
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
            StatisticsController.prototype.getCompanies = function () {
                var _this = this;
                this.showLoader();
                this.$http.get(this.getRegetRootUrl() + "Statistics/GetUserCompanies?&t=" + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        _this.userCompanies = tmpData;
                        _this.isCompaniesLoaded = true;
                        if (!_this.isValueNullOrUndefined(_this.userCompanies)
                            && _this.userCompanies.length === 1) {
                            _this.userCompanies[0].is_selected = true;
                        }
                        _this.getCurrencies();
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
            StatisticsController.prototype.getCurrencies = function () {
                var _this = this;
                this.showLoader();
                this.$http.get(this.getRegetRootUrl() + "Statistics/GetCurrencies?&t=" + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        _this.currenciesList = tmpData;
                        _this.isCurrenciesLoaded = true;
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
            StatisticsController.prototype.getFilterXItems = function (companyId) {
                var _this = this;
                this.isError = false;
                if (this.isValueNullOrUndefined(this.userCompanies)) {
                    return;
                }
                this.showLoaderBoxOnly(this.isError);
                try {
                    var selCompanies = null;
                    if (this.isValueNullOrUndefined(companyId)) {
                        this.filterItems !== null;
                        selCompanies = this.getSelectedCompanies();
                        if (selCompanies.length === 0) {
                            this.hideLoaderWrapper();
                            return;
                        }
                    }
                    else {
                        selCompanies = companyId.toString();
                    }
                    var urlLink = "";
                    if (this.selectedAxisXItem === this.FILTER_REQUESTOR) {
                        urlLink = this.getRegetRootUrl() + "/Statistics/GetFilterRequestors?companies=" + selCompanies +
                            '&isActiveOnly=' + this.isActiveOnly +
                            '&t=' + new Date().getTime();
                    }
                    else if (this.selectedAxisXItem === this.FILTER_ORDERER) {
                        urlLink = this.getRegetRootUrl() + "/Statistics/GetFilterOrderers?companies=" + selCompanies +
                            '&isActiveOnly=' + this.isActiveOnly +
                            '&t=' + new Date().getTime();
                    }
                    else if (this.selectedAxisXItem === this.FILTER_AREA) {
                        urlLink = this.getRegetRootUrl() + "/Statistics/GetFilterAreas?companies=" + selCompanies +
                            '&isActiveOnly=' + this.isActiveOnly +
                            '&t=' + new Date().getTime();
                    }
                    else if (this.selectedAxisXItem === this.FILTER_CENTER) {
                        urlLink = this.getRegetRootUrl() + "/Statistics/GetFilterCentres?companies=" + selCompanies +
                            '&isActiveOnly=' + this.isActiveOnly +
                            '&t=' + new Date().getTime();
                    }
                    else if (this.selectedAxisXItem === this.FILTER_SUPPLIER) {
                        urlLink = this.getRegetRootUrl() + "/Statistics/GetFilterSuppliers?companies=" + selCompanies +
                            '&isActiveOnly=' + this.isActiveOnly +
                            '&t=' + new Date().getTime();
                    }
                    else if (this.selectedAxisXItem === this.FILTER_COMMODITY) {
                        urlLink = this.getRegetRootUrl() + "/Statistics/GetFilterCommodities?companies=" + selCompanies +
                            '&isActiveOnly=' + this.isActiveOnly +
                            '&t=' + new Date().getTime();
                    }
                }
                catch (ex) {
                    this.displayErrorMsg();
                }
                finally {
                    this.hideLoaderWrapper();
                }
                if (this.isStringValueNullOrEmpty(urlLink)) {
                    return;
                }
                this.showLoaderBoxOnly(this.isError);
                this.$http.get(urlLink, {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        if (_this.isValueNullOrUndefined(companyId)) {
                            _this.filterItems = tmpData;
                        }
                        else {
                            if (_this.isValueNullOrUndefined(_this.filterItems)) {
                                _this.filterItems = tmpData;
                            }
                            else {
                                _this.filterItems = _this.filterItems.concat(tmpData);
                                _this.filterItems = _this.$filter('orderBy')(_this.filterItems, 'name');
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
            StatisticsController.prototype.getChartData = function () {
                var _this = this;
                if (!this.isChartDataValid()) {
                    this.displayErrorMsg(this.locEnterMandatoryValuesText + ". " + this.chartErrorMsg);
                    return;
                }
                if (this.isCompanyAllowed() === false) {
                    return;
                }
                this.isError = false;
                this.showLoaderBoxOnly(this.isError);
                this.$http.get(this.getRegetRootUrl() + "Statistics/GetChartData?" +
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
                    "&t=" + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        var tmpChartItems = tmpData;
                        var errMsg = tmpChartItems.ErrMsg;
                        if (_this.isStringValueNullOrEmpty(errMsg)) {
                            if (_this.isValueNullOrUndefined(_this.chartItems)) {
                                _this.toggleChartSetings(true);
                                var divBody = document.getElementById("divbody");
                                divBody.scrollTop = 1;
                                //$("#divbody").scrollTop = 1;
                            }
                            _this.chartItems = tmpData;
                            _this.displayChart();
                            _this.chartDataDonut = null;
                            _this.chartOptionsDonut = null;
                            _this.selectedCurrencyId = _this.chartItems.CurrencyId;
                            _this.origCurrencyId = _this.chartItems.CurrencyId;
                        }
                        else {
                            _this.displayErrorMsg(errMsg);
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
            //***************************************************************
            //***************************************************************
            //Methods
            StatisticsController.prototype.loadData = function () {
                this.rootUrl = this.getRegetRootUrl();
                this.getAxisItems();
                this.getCompanies();
            };
            StatisticsController.prototype.getAxisItems = function () {
                try {
                    this.getAxisXItems();
                    this.getAxisXTopsItems();
                    this.axisYItemsList = [];
                    this.axisYItemsList.push({ id: 10, name: this.locPriceText, is_disabled: false });
                    this.axisYItemsList.push({ id: 20, name: this.locNumOfRequestsText, is_disabled: false });
                    this.axisXPeriodItemsList = [];
                    this.axisXPeriodItemsList.push({ id: 10, name: this.locAllText, is_disabled: false });
                    this.axisXPeriodItemsList.push({ id: 20, name: this.locYearText, is_disabled: false });
                    this.axisXPeriodItemsList.push({ id: 30, name: this.locMonthText, is_disabled: false });
                    this.getChartTypeItems();
                }
                catch (e) {
                    this.displayErrorMsg();
                }
            };
            StatisticsController.prototype.getAxisXTopsItems = function () {
                this.axisXTopList = [];
                this.axisXTopList = [];
                this.axisXTopList.push({ id: 0, name: this.locAllText, is_disabled: false });
                this.axisXTopList.push({ id: 10, name: "10", is_disabled: false });
                this.axisXTopList.push({ id: 20, name: "20", is_disabled: false });
                this.axisXTopList.push({ id: 50, name: "50", is_disabled: false });
                this.axisXTopList.push({ id: 100, name: "100", is_disabled: false });
                this.selectedAxisXTop = 0;
            };
            StatisticsController.prototype.getChartTypeItems = function () {
                this.chartTypeItemsList = [];
                this.chartTypeItemsList.push({ id: 20, name: 'Bar', is_disabled: false });
                this.chartTypeItemsList.push({ id: 10, name: 'Line', is_disabled: false });
                this.chartTypeItemsList.push({ id: 40, name: 'Pie', is_disabled: false });
                this.chartTypeItemsList.push({ id: 50, name: 'Doughnut', is_disabled: false });
                this.chartTypeItemsList.push({ id: 30, name: 'Radar', is_disabled: false });
                this.chartTypeItemsList.push({ id: 60, name: 'Polar Area', is_disabled: false });
                //$scope.ChartTypeItemsList.push({ id: 70, name: 'Bubble' });
                //$scope.ChartTypeItemsList.push({ id: 80, name: 'Scatter' });
            };
            StatisticsController.prototype.isAllCompaniesChecked = function () {
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
            };
            StatisticsController.prototype.isCompaniesIndeterminate = function () {
                if (this.isValueNullOrUndefined(this.userCompanies)) {
                    return false;
                }
                return (this.userCompanies.length !== 0 &&
                    !this.isAllCompaniesChecked());
            };
            StatisticsController.prototype.toggleAllCompanies = function () {
                if (this.isValueNullOrUndefined(this.userCompanies)) {
                    return;
                }
                if (this.isAllCompaniesChecked()) {
                    for (var i = 0; i < this.userCompanies.length; i++) {
                        this.userCompanies[i].is_selected = false;
                    }
                }
                else {
                    for (var j = 0; j < this.userCompanies.length; j++) {
                        this.userCompanies[j].is_selected = true;
                    }
                }
            };
            StatisticsController.prototype.toggleCompany = function (item) {
                if (item.is_selected) {
                    item.is_selected = false;
                    this.removeFilterItems(item.id);
                }
                else {
                    item.is_selected = true;
                    this.addFilterItems(item.id);
                }
                this.setAxisXItem();
            };
            StatisticsController.prototype.addFilterItems = function (companyId) {
                if (this.selectedAxisXItem === this.FILTER_COMMODITY) {
                    //commodity
                    this.getFilterXItems(null);
                }
                else {
                    this.getFilterXItems(companyId);
                }
            };
            StatisticsController.prototype.removeFilterItems = function (companyId) {
                this.isError = false;
                if (this.isValueNullOrUndefined(this.filterItems)) {
                    return;
                }
                this.showLoaderBoxOnly(this.isError);
                try {
                    if (this.selectedAxisXItem === this.FILTER_COMMODITY) {
                        //commodity
                        this.getFilterXItems(null);
                    }
                    else {
                        for (var i = this.filterItems.length - 1; i >= 0; i--) {
                            if (this.filterItems[i].company_id === companyId) {
                                this.filterItems.splice(i, 1);
                            }
                        }
                    }
                }
                catch (ex) {
                    this.displayErrorMsg();
                }
                finally {
                    this.hideLoaderWrapper();
                }
            };
            StatisticsController.prototype.getSelectedCompanies = function () {
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
            };
            StatisticsController.prototype.setAxisYItem = function () {
                this.isCost = (this.selectedAxisYItem === 10);
            };
            StatisticsController.prototype.setAxisXPeriodItem = function () {
            };
            StatisticsController.prototype.setAxisXTopItem = function () {
            };
            StatisticsController.prototype.setAxisXItem = function () {
                //$scope.xFilterMsg = [];
                this.isFilterUserOnly = false;
                this.getFilterXItems(null);
                if (this.selectedAxisXItem === this.FILTER_REQUESTOR) {
                    this.filterPlaceholder = this.locSelectRequestorsText;
                    this.isSelectXItemsAvailable = true;
                }
                else if (this.selectedAxisXItem === this.FILTER_ORDERER) {
                    this.filterPlaceholder = this.locOrdererText;
                    this.isSelectXItemsAvailable = true;
                }
                else if (this.selectedAxisXItem === this.FILTER_AREA) {
                    this.filterPlaceholder = this.locAreaText;
                    this.isSelectXItemsAvailable = true;
                }
                else if (this.selectedAxisXItem === this.FILTER_CENTER) {
                    this.filterPlaceholder = this.locCentreText;
                    this.isSelectXItemsAvailable = true;
                }
                else if (this.selectedAxisXItem === this.FILTER_SUPPLIER) {
                    this.filterPlaceholder = this.locSupplierText;
                    this.isSelectXItemsAvailable = true;
                }
                else if (this.selectedAxisXItem === this.FILTER_COMMODITY) {
                    this.filterPlaceholder = this.locPurchaseGroupText;
                    this.isSelectXItemsAvailable = true;
                }
                else {
                    this.isSelectXItemsAvailable = false;
                }
                this.setXFilterAuthorization();
                this.isSelectedXItems = false;
            };
            StatisticsController.prototype.setXFilterAuthorization = function () {
                this.isFilterUserOnly = true;
                var selCompanies = this.getSelectedCompanies();
                if (this.isStringValueNullOrEmpty(selCompanies)) {
                    return;
                }
                var selCompaniesIds = selCompanies.split('|');
                for (var i = 0; i < selCompaniesIds.length; i++) {
                    var comp = this.$filter("filter")(this.userCompanies, { id: parseInt(selCompaniesIds[i]) }, true);
                    if (this.selectedAxisXItem === this.FILTER_REQUESTOR) {
                        if (comp[0].is_user_company_stat_requestor !== true) {
                            this.xFilterMsg.push({ company: comp[0].country_code, msg: this.locNotAuthorizedText });
                        }
                        else if (comp[0].is_user_company_stat_admin !== true &&
                            comp[0].is_user_company_stat_cgadmin !== true) {
                            this.xFilterMsg.push({ company: comp[0].country_code, msg: this.locStatisticsAuthorizedOwnRequestOnlyText });
                        }
                        else if (comp[0].is_user_company_stat_admin !== true &&
                            comp[0].is_user_company_stat_cgadmin === true) {
                            this.xFilterMsg.push({ company: comp[0].country_code, msg: this.locStatisticsAuthorizedCgRequestOnlyText });
                            this.isFilterUserOnly = false;
                        }
                        else {
                            this.isFilterUserOnly = false;
                        }
                    }
                    else if (this.selectedAxisXItem === this.FILTER_ORDERER) {
                        if (comp[0].is_user_company_stat_orderer !== true) {
                            this.xFilterMsg.push({ company: comp[0].country_code, msg: this.locNotAuthorizedText });
                        }
                        else if (comp[0].is_user_company_stat_admin !== true &&
                            comp[0].is_user_company_stat_cgadmin !== true) {
                            this.xFilterMsg.push({ company: comp[0].country_code, msg: this.locStatisticsAuthorizedOwnRequestOnlyText });
                        }
                        else if (comp[0].is_user_company_stat_admin !== true &&
                            comp[0].is_user_company_stat_cgadmin === true) {
                            this.xFilterMsg.push({ company: comp[0].country_code, msg: this.locStatisticsAuthorizedCgRequestOnlyText });
                            this.isFilterUserOnly = false;
                        }
                        else {
                            this.isFilterUserOnly = false;
                        }
                    }
                    else if (this.selectedAxisXItem === this.FILTER_AREA) {
                        if (comp[0].is_user_company_stat_cgadmin !== true) {
                            this.xFilterMsg.push({ company: comp[0].country_code, msg: this.locNotAuthorizedText });
                        }
                        else if (comp[0].is_user_company_stat_admin !== true &&
                            comp[0].is_user_company_stat_cgadmin === true) {
                            this.xFilterMsg.push({ company: comp[0].country_code, msg: this.locStatisticsAuthorizedCgRequestOnlyText });
                            this.isFilterUserOnly = false;
                        }
                        else {
                            this.isFilterUserOnly = false;
                        }
                    }
                    else if (this.selectedAxisXItem === this.FILTER_CENTER) {
                        if (comp[0].is_user_company_stat_cgadmin !== true) {
                            this.xFilterMsg.push({ company: comp[0].country_code, msg: this.locNotAuthorizedText });
                        }
                        else if (comp[0].is_user_company_stat_admin !== true &&
                            comp[0].is_user_company_stat_cgadmin === true) {
                            this.xFilterMsg.push({ company: comp[0].country_code, msg: this.locStatisticsAuthorizedCgRequestOnlyText });
                            this.isFilterUserOnly = false;
                        }
                        else {
                            this.isFilterUserOnly = false;
                        }
                    }
                    else if (this.selectedAxisXItem === this.FILTER_SUPPLIER) {
                        if (comp[0].is_user_company_stat_admin !== true) {
                            this.xFilterMsg.push({ company: comp[0].country_code, msg: this.locNotAuthorizedText });
                        }
                        else {
                            this.isFilterUserOnly = false;
                        }
                    }
                    else if (this.selectedAxisXItem === this.FILTER_COMMODITY) {
                        if (comp[0].is_user_company_stat_admin !== true) {
                            this.xFilterMsg.push({ company: comp[0].country_code, msg: this.locNotAuthorizedText });
                        }
                        else {
                            this.isFilterUserOnly = false;
                        }
                    }
                }
            };
            StatisticsController.prototype.searchFilterItems = function (searchText) {
                var _this = this;
                var searchFilterItems = [];
                if (this.isStringValueNullOrEmpty(searchText)) {
                    return this.filterItems;
                }
                angular.forEach(this.filterItems, function (filterItem) {
                    var searchKeyLower = searchText.trim().toLowerCase();
                    if (!_this.isStringValueNullOrEmpty(filterItem.name) && filterItem.name.toLowerCase().indexOf(searchKeyLower) > -1 ||
                        !_this.isStringValueNullOrEmpty(filterItem.name_search_key) && filterItem.name_search_key.trim().toLowerCase().indexOf(searchKeyLower) > -1) {
                        searchFilterItems.push(filterItem);
                    }
                });
                return searchFilterItems;
            };
            StatisticsController.prototype.filterSelectedItemChange = function (item) {
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
                }
                else {
                    //we need to find out the right one
                    var prevSibl = this.angScopeAny.$$childTail.$$prevSibling;
                    while (!this.isValueNullOrUndefined(prevSibl)) {
                        if (!this.isValueNullOrUndefined(prevSibl.searchstringitem)) {
                            prevSibl.searchstringitem = '';
                            break;
                        }
                        else {
                            prevSibl = prevSibl.$$prevSibling;
                        }
                    }
                }
                //var msg : string = this.locItemWasAddedToFilter.replace('{0}', item.name);
                //this.showToast(msg);
            };
            StatisticsController.prototype.filterSelectAll = function () {
                if (this.isCompanySelected() === false) {
                    this.showAlert(this.locSelectCompanyText, this.locSelectCompanyText, this.locCloseText);
                    return;
                }
                if (this.isValueNullOrUndefined(this.filterItems)) {
                    return;
                }
                this.showLoaderBoxOnly(this.isError);
                try {
                    for (var i = 0; i < this.filterItems.length; i++) {
                        this.filterItems[i].is_selected = true;
                    }
                    this.isSelectedXItems = true;
                }
                catch (ex) {
                    this.displayErrorMsg();
                }
                finally {
                    this.hideLoaderWrapper();
                }
            };
            StatisticsController.prototype.filterRemoveAll = function () {
                if (this.isValueNullOrUndefined(this.filterItems)) {
                    return;
                }
                this.showLoaderBoxOnly(this.isError);
                try {
                    for (var i = 0; i < this.filterItems.length; i++) {
                        this.filterItems[i].is_selected = false;
                    }
                    this.isSelectedXItems = false;
                }
                catch (ex) {
                    this.displayErrorMsg();
                }
                finally {
                    this.hideLoaderWrapper();
                }
            };
            StatisticsController.prototype.deleteFilterItem = function (iItemId) {
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
            };
            StatisticsController.prototype.isCompanySelected = function () {
                var isCompSelected = false;
                for (var i = 0; i < this.userCompanies.length; i++) {
                    if (this.userCompanies[i].is_selected) {
                        isCompSelected = true;
                        break;
                    }
                }
                return isCompSelected;
            };
            StatisticsController.prototype.toggleActiveOnly = function () {
                this.isActiveOnly = !this.isActiveOnly;
                this.getFilterXItems(null);
            };
            StatisticsController.prototype.isChartDataValid = function () {
                var isValid = true;
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
            };
            StatisticsController.prototype.isCompanyAllowed = function () {
                var selCompanies = this.getSelectedCompanies();
                if (this.isValueNullOrUndefined(selCompanies)) {
                    return true;
                }
                var selCompaniesIds = selCompanies.split('|');
                for (var i = 0; i < selCompaniesIds.length; i++) {
                    var comp = this.$filter("filter")(this.userCompanies, { id: parseInt(selCompaniesIds[i]) }, true);
                    if (this.selectedAxisXItem === this.FILTER_REQUESTOR) {
                        if (comp[0].is_user_company_stat_requestor !== true) {
                            var msg = this.locStatisticsRequestorNotAllowedText.replace("{0}", comp[0].country_code) + ".";
                            this.displayErrorMsg(msg);
                            return false;
                        }
                    }
                    if (this.selectedAxisXItem === this.FILTER_ORDERER) {
                        if (comp[0].is_user_company_stat_orderer !== true) {
                            var msg = this.locStatisticsOrdererNotAllowedText.replace("{0}", comp[0].country_code) + ".";
                            this.displayErrorMsg(msg);
                            return false;
                        }
                    }
                    if (this.selectedAxisXItem === this.FILTER_APPROVE_MAN) {
                        if (comp[0].is_user_company_stat_appman !== true) {
                            var msg = this.locStatisticsAppManNotAllowedText.replace("{0}", comp[0].country_code) + ".";
                            this.displayErrorMsg(msg);
                            return false;
                        }
                    }
                    if (this.selectedAxisXItem === this.FILTER_CENTER) {
                        if (comp[0].is_user_company_stat_admin !== true && comp[0].is_user_company_stat_cgadmin !== true) {
                            var msg = this.locStatisticsCentreNotAllowedText.replace("{0}", comp[0].country_code) + ".";
                            this.displayErrorMsg(msg);
                            return false;
                        }
                    }
                    if (this.selectedAxisXItem === this.FILTER_AREA) {
                        if (comp[0].is_user_company_stat_admin !== true && comp[0].is_user_company_stat_cgadmin !== true) {
                            var msg = this.locStatisticsAreaNotAllowedText.replace("{0}", comp[0].country_code) + ".";
                            this.displayErrorMsg(msg);
                            return false;
                        }
                    }
                    if (this.selectedAxisXItem === this.FILTER_COMMODITY) {
                        if (comp[0].is_user_company_stat_admin !== true) {
                            var msg = this.locStatisticsPgNotAllowedText.replace("{0}", comp[0].country_code) + ".";
                            this.displayErrorMsg(msg);
                            return false;
                        }
                    }
                    if (this.selectedAxisXItem === this.FILTER_SUPPLIER) {
                        if (comp[0].is_user_company_stat_admin !== true) {
                            var msg = this.locStatisticsSupplierNotAllowedText.replace("{0}", comp[0].country_code) + ".";
                            this.displayErrorMsg(msg);
                            return false;
                        }
                    }
                }
                return true;
            };
            StatisticsController.prototype.getSelectedFilterItems = function () {
                if (this.isValueNullOrUndefined(this.filterItems)) {
                    return "";
                }
                var selItems = "";
                var isAll = true;
                for (var i = 0; i < this.filterItems.length; i++) {
                    if (this.filterItems[i].is_selected === true) {
                        if (selItems.length > 0) {
                            selItems += ";";
                        }
                        selItems += this.filterItems[i].id;
                    }
                    else {
                        isAll = false;
                    }
                }
                if (isAll === true) {
                    return "";
                }
                return selItems;
            };
            StatisticsController.prototype.toggleChartSetings = function (isHide) {
                if ($("#divChartSettings").is(':visible') || isHide) {
                    $("#divChartSettings").slideUp();
                    $("#imgCgExpand").attr("src", this.getRegetRootUrl() + "Content/Images/Panel/Expand.png");
                    $("#imgCgExpand").attr("title", "@Html.Raw(RequestResource.Display)");
                }
                else {
                    $("#divChartSettings").show("slow");
                    $("#imgCgExpand").attr("src", this.getRegetRootUrl() + "Content/Images/Panel/Collapse.png");
                    $("#imgCgExpand").attr("title", "@Html.Raw(RequestResource.Hide)");
                }
            };
            StatisticsController.prototype.displayChart = function () {
                $("#chartWrap").show();
                var chartCanvas = document.getElementById("regetChart");
                var ctx = chartCanvas.getContext('2d');
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
            };
            StatisticsController.prototype.setChartType = function () {
                if (!this.isValueNullOrUndefined(this.regetChart)) {
                    this.regetChart.destroy();
                }
                var chartCanvas = document.getElementById("regetChart");
                var ctx = chartCanvas.getContext('2d');
                var isDonut = false;
                if (this.selectedChartTypeItem === 40) {
                    //chartType = "pie";
                    isDonut = true;
                }
                else if (this.selectedChartTypeItem === 50) {
                    //chartType = "doughnut";
                    isDonut = true;
                }
                else if (this.selectedChartTypeItem === 60) {
                    //chartType = "polarArea";
                    isDonut = true;
                }
                var chartType = this.getChartType();
                if (isDonut === true) {
                    this.chartDataDonut = { datasets: [], labels: [] };
                    for (var iDs = 0; iDs < this.chartData.datasets[0].data.length; iDs++) {
                        this.chartDataDonut.datasets.push({ data: [], backgroundColor: [], label: "" });
                    }
                    for (var i = 0; i < this.chartDataDonut.datasets.length; i++) {
                        for (var j = 0; j < this.chartData.datasets.length; j++) {
                            var sourceData = this.chartData.datasets[j].data[i];
                            var targetData = this.chartDataDonut.datasets[i].data;
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
                }
                else {
                    this.regetChart = new Chart(ctx, {
                        type: chartType,
                        data: this.chartData,
                        options: this.chartOptions
                    });
                }
                this.setChartWidth();
            };
            StatisticsController.prototype.getChartType = function () {
                if (this.selectedChartTypeItem === 10) {
                    return "line";
                }
                else if (this.selectedChartTypeItem === 20) {
                    return "bar";
                }
                else if (this.selectedChartTypeItem === 30) {
                    return "radar";
                }
                else if (this.selectedChartTypeItem === 40) {
                    return "pie";
                }
                else if (this.selectedChartTypeItem === 50) {
                    return "doughnut";
                }
                else if (this.selectedChartTypeItem === 60) {
                    return "polarArea";
                }
                return "bar";
            };
            StatisticsController.prototype.setChartWidth = function () {
                var height = 500;
                var labelsLength = 0;
                for (var i = 0; i < this.chartData.datasets.length; i++) {
                    labelsLength += this.chartData.datasets[i].label.length + 7;
                }
                height = labelsLength / 6;
                if (height < 500) {
                    height = 500;
                }
                var chart = $("#regetChart");
                chart.attr("style", "height:" + height + "px;max-height:" + height + "px;");
            };
            StatisticsController.prototype.setCurrency = function () {
                this.recalculate();
            };
            StatisticsController.prototype.recalculate = function () {
                this.isError = false;
                try {
                    var exchangeRate = null;
                    var filterItem = this.$filter("filter")(this.chartItems.ExchangeRates, { source_currency_id: this.origCurrencyId, destin_currency_id: this.selectedCurrencyId }, true);
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
                        var filterCurrFrom = this.$filter("filter")(this.currenciesList, { id: this.origCurrencyId }, true);
                        var filterCurrToEx1 = this.$filter("filter")(this.currenciesList, { id: this.selectedCurrencyId }, true);
                        var curr1 = filterCurrFrom[0].currency_code;
                        var curr2 = filterCurrToEx1[0].currency_code;
                        var msg = this.locExchangeRateMissingText.replace('{0}', curr1).replace('{1}', curr2);
                        this.displayErrorMsg(msg);
                        this.selectedCurrencyId = this.origCurrencyId;
                        return;
                    }
                    var recChartData = angular.copy(this.chartData);
                    for (var i = 0; i < recChartData.datasets.length; i++) {
                        for (var j = 0; j < recChartData.datasets[i].data.length; j++) {
                            if (recChartData.datasets[i].data[j] === 0) {
                                continue;
                            }
                            recChartData.datasets[i].data[j] = Math.round(recChartData.datasets[i].data[j] * exchangeRate);
                        }
                    }
                    var filterCurrTo = this.$filter("filter")(this.currenciesList, { id: this.selectedCurrencyId }, true);
                    var curr = filterCurrTo[0].currency_code;
                    this.replaceCurrencyTitle(curr);
                    this.chartData = recChartData;
                    if (!this.isValueNullOrUndefined(this.chartDataDonut)) {
                        var recChartDataDonut = angular.copy(this.chartDataDonut);
                        for (var i = 0; i < recChartDataDonut.datasets.length; i++) {
                            for (var j = 0; j < recChartDataDonut.datasets[i].data.length; j++) {
                                if (recChartDataDonut.datasets[i].data[j] === 0) {
                                    continue;
                                }
                                recChartDataDonut.datasets[i].data[j] = Math.round(recChartDataDonut.datasets[i].data[j] * exchangeRate);
                            }
                        }
                        this.chartDataDonut = recChartDataDonut;
                    }
                    //recalculate table
                    for (var i = 0; i < this.chartItems.ChartData.datasets.length; i++) {
                        for (var k = 0; k < this.chartItems.ChartData.datasets[i].data.length; k++) {
                            this.chartItems.ChartData.datasets[i].data[k] = Math.round(this.chartItems.ChartData.datasets[i].data[k] * exchangeRate);
                        }
                    }
                    this.setChartType();
                    this.origCurrencyId = this.selectedCurrencyId;
                }
                catch (e) {
                    this.selectedCurrencyId = this.origCurrencyId;
                    this.displayErrorMsg();
                }
            };
            StatisticsController.prototype.replaceCurrencyTitle = function (currency) {
                try {
                    //title
                    var tmpTitle = this.chartOptions.title.text;
                    var iStart = tmpTitle.indexOf('(');
                    var iStop = tmpTitle.indexOf(')');
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
                }
                catch (e) {
                    this.displayErrorMsg();
                }
            };
            StatisticsController.prototype.changeChartColors = function () {
                try {
                    if (this.isValueNullOrUndefined(this.regetChart)) {
                        return;
                    }
                    for (var i = 0; i < this.chartData.datasets.length; i++) {
                        this.chartData.datasets[i].backgroundColor = this.getRandomColor();
                        this.chartData.datasets[i].borderColor = this.chartData.datasets[i].backgroundColor;
                    }
                    this.regetChart.destroy();
                    var chartCanvas = document.getElementById("regetChart");
                    var ctx = chartCanvas.getContext('2d');
                    this.setChartType();
                }
                catch (e) {
                    this.displayErrorMsg();
                }
            };
            StatisticsController.prototype.getRandomColor = function () {
                var letters = "0123456789ABCDEF";
                var color = "#";
                for (var i = 0; i < 6; i++) {
                    color += letters[Math.floor(Math.random() * 16)];
                }
                return color;
            };
            StatisticsController.prototype.exportToExcel = function () {
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
            };
            StatisticsController.prototype.getSelectedImgUrl = function () {
                return this.getRegetRootUrl() + "Content/Images/Controll/CheckGreen16.png";
            };
            StatisticsController.prototype.hideLoaderWrapper = function () {
                if (this.isError || (this.isChartAxisItemsLoaded
                    && this.isCompaniesLoaded && this.isCurrenciesLoaded)) {
                    this.hideLoader();
                }
            };
            return StatisticsController;
        }(RegetApp.BaseRegetTs));
        RegetApp.StatisticsController = StatisticsController;
        angular.
            module('RegetApp').
            controller('StatisticsController', Kamsyk.RegetApp.StatisticsController).
            config(function ($mdDateLocaleProvider) {
            this.SetDatePicker($mdDateLocaleProvider);
            //it is neccessary to set IsGenerateDatePickerLocalization = true
        });
        var StatisticsCompany = /** @class */ (function () {
            function StatisticsCompany() {
                this.id = null;
                this.country_code = null;
                this.is_user_company_stat_admin = false;
                this.is_user_company_stat_requestor = false;
                this.is_user_company_stat_orderer = false;
                this.is_user_company_stat_appman = false;
                this.is_user_company_stat_cgadmin = false;
                this.is_selected = false;
            }
            return StatisticsCompany;
        }());
        RegetApp.StatisticsCompany = StatisticsCompany;
        var StatisticsFilter = /** @class */ (function () {
            function StatisticsFilter() {
                this.id = null;
                this.name = null;
                this.name_search_key = null;
                this.company_id = null;
                this.is_selected = null;
                this.flag_url = null;
            }
            return StatisticsFilter;
        }());
        RegetApp.StatisticsFilter = StatisticsFilter;
        var FilterMsg = /** @class */ (function () {
            function FilterMsg() {
                this.company = null;
                this.msg = null;
            }
            return FilterMsg;
        }());
        RegetApp.FilterMsg = FilterMsg;
        var ChartItems = /** @class */ (function () {
            function ChartItems() {
                //must be name like this
                this.ChartData = null;
                this.ChartOptions = null;
                this.CurrencyId = null;
                this.ErrMsg = null;
                this.ExchangeRates = null;
            }
            return ChartItems;
        }());
        RegetApp.ChartItems = ChartItems;
    })(RegetApp = Kamsyk.RegetApp || (Kamsyk.RegetApp = {}));
})(Kamsyk || (Kamsyk = {}));
//# sourceMappingURL=statistics.js.map