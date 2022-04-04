/// <reference path="../RegetTypeScript/Base/reget-base.ts" />
/// <reference path="../RegetTypeScript/Base/reget-common.ts" />
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
        var AppMatrixExportController = /** @class */ (function (_super) {
            __extends(AppMatrixExportController, _super);
            //**********************************************************
            //**********************************************************
            //Constructor
            function AppMatrixExportController($scope, $http, $filter, $mdDialog, $mdToast, $q, $timeout) {
                var _this = _super.call(this, $scope, $http, $filter, $mdDialog, $mdToast, $q, $timeout) || this;
                _this.$scope = $scope;
                _this.$http = $http;
                _this.$filter = $filter;
                _this.$mdDialog = $mdDialog;
                _this.$mdToast = $mdToast;
                _this.$q = $q;
                _this.$timeout = $timeout;
                //**********************************************************
                //Properties
                _this.companies = null;
                _this.selectedCompanyId = null;
                _this.centreGroups = null;
                _this.locEnterMandatoryValuesText = $("#EnterMandatoryValuesText").val();
                //***************************************************************
                _this.$onInit = function () { };
                _this.loadData();
                return _this;
            }
            //****************************************************************
            //Methods
            AppMatrixExportController.prototype.getParticipantCompanies = function () {
                var _this = this;
                this.showLoader(this.isError);
                this.$http.get(this.getRegetRootUrl() + "RegetAdmin/GetActiveCompanies?t=" + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        _this.companies = tmpData;
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
            AppMatrixExportController.prototype.loadData = function () {
                this.getParticipantCompanies();
            };
            AppMatrixExportController.prototype.exportToExcel = function () {
                var _this = this;
                if (!this.isExportValid()) {
                    this.displayErrorMsg(this.locEnterMandatoryValuesText);
                    return;
                }
                this.showLoader(this.isError);
                this.$http.get(this.getRegetRootUrl() + "RegetAdmin/GetCompanyCentreGroupsActiveOnly?companyId="
                    + this.selectedCompanyId + "&t=" + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        _this.centreGroups = tmpData;
                        var companyName = 'ApprovalMartix';
                        var company = _this.$filter("filter")(_this.companies, { company_id: _this.selectedCompanyId }, true);
                        if (company !== null) {
                            companyName = _this.encodeUrl(company[0].country_code);
                        }
                        _this.exportCgToExcel(_this.centreGroups[0].id, 0, _this.centreGroups.length, companyName);
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
            AppMatrixExportController.prototype.exportCgToExcel = function (iCgId, iCgIndex, iCgCount, strCompanyName) {
                var _this = this;
                this.showLoader(this.isError);
                $("#spanLoading").html(this.locLoadingDataText + ' ' + (iCgIndex + 1) + ' / ' + iCgCount);
                if (iCgIndex < (iCgCount - 1)) {
                    this.$http.get(this.getRegetRootUrl() + "Report/GetCompanyCentreGroup?cgId=" + iCgId
                        + "&cgIndex=" + iCgIndex + "&cgCount=" + iCgCount + "&companyName=" + strCompanyName
                        + "&t=" + new Date().getTime(), {}).then(function (response) {
                        try {
                            if (iCgIndex < (iCgCount - 1)) {
                                var currIndex = iCgIndex + 1;
                                _this.exportCgToExcel(_this.centreGroups[currIndex].id, currIndex, _this.centreGroups.length, strCompanyName);
                            }
                            else {
                                _this.hideLoader();
                            }
                        }
                        catch (e) {
                            _this.hideLoader();
                            _this.displayErrorMsg();
                        }
                    }, function (response) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    });
                }
                else {
                    var url = this.getRegetRootUrl() + "Report/GetCompanyCentreGroup?cgId=" + iCgId
                        + "&cgIndex=" + iCgIndex + "&cgCount=" + iCgCount + "&companyName=" + this.encodeUrl(strCompanyName)
                        + "&t=" + new Date().getTime();
                    window.open(url);
                    this.hideLoader();
                }
            };
            AppMatrixExportController.prototype.isExportValid = function () {
                var isValid = true;
                if (this.isValueNullOrUndefined(this.selectedCompanyId)) {
                    this.angScopeAny.frmAppMatrixExport.cmbUserCompanies.$valid = false;
                    this.angScopeAny.frmAppMatrixExport.cmbUserCompanies.$setTouched();
                    isValid = false;
                }
                return isValid;
            };
            return AppMatrixExportController;
        }(RegetApp.BaseRegetTs));
        RegetApp.AppMatrixExportController = AppMatrixExportController;
        angular.
            module('RegetApp').
            controller('AppMatrixExportController', Kamsyk.RegetApp.AppMatrixExportController);
        var CentreGroupSimple = /** @class */ (function () {
            function CentreGroupSimple() {
                this.id = null;
                this.name = null;
            }
            return CentreGroupSimple;
        }());
        RegetApp.CentreGroupSimple = CentreGroupSimple;
        var CompanyDropDown = /** @class */ (function () {
            function CompanyDropDown() {
                this.company_id = null;
                this.country_code = null;
            }
            return CompanyDropDown;
        }());
        RegetApp.CompanyDropDown = CompanyDropDown;
    })(RegetApp = Kamsyk.RegetApp || (Kamsyk.RegetApp = {}));
})(Kamsyk || (Kamsyk = {}));
//# sourceMappingURL=appmatrix-export.js.map