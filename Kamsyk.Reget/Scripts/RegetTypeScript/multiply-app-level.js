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
        var MultiplyAppLevelController = /** @class */ (function (_super) {
            __extends(MultiplyAppLevelController, _super);
            //**********************************************************
            //Constructor
            function MultiplyAppLevelController($scope, $http, $filter, $mdDialog, $mdToast, $q, $timeout) {
                var _this = _super.call(this, $scope, $http, $filter, $mdDialog, $mdToast, $q, $timeout) || this;
                _this.$scope = $scope;
                _this.$http = $http;
                _this.$filter = $filter;
                _this.$mdDialog = $mdDialog;
                _this.$mdToast = $mdToast;
                _this.$q = $q;
                _this.$timeout = $timeout;
                //private KEY_USER_DATA: string = "user_data";
                _this.userAdminCompanies = null;
                _this.multiplText = null;
                _this.multipl = null;
                _this.substErrMsg = null;
                _this.isAll = false;
                _this.copyOption = "Selected";
                _this.multiplyLimitResult = null;
                //private locEnterDecimalNumberText: string = $("#EnterDecimalNumberText").val();
                //private locSelectCompanyText: string  = $("#SelectCompanyText").val();
                _this.locMultiplyText = $("#MultiplyText").val();
                _this.locAppLevelMultiplyAllConfirmText = $("#AppLevelMultiplyAllConfirmText").val();
                _this.locAppLevelMultiplySelectedConfirmText = $("#AppLevelMultiplySelectedConfirmText").val();
                _this.locLimitMultiplyResultText = $("#LimitMultiplyResultText").val();
                _this.locFinishedText = $("#FinishedText").val();
                _this.locMultiplPgLimitErrorText = $("#MultiplPgLimitErrorText").val();
                _this.locConfirmationText = $("#ConfirmationText").val();
                _this.locEnterMandatoryValuesText = $("#EnterMandatoryValuesText").val();
                //***************************************************************
                _this.$onInit = function () { };
                _this.loadData();
                return _this;
            }
            //************************************************************************
            //Methods
            MultiplyAppLevelController.prototype.loadSubstUsers = function () {
                var _this = this;
                this.showLoader(this.isError);
                this.$http.get(this.getRegetRootUrl() + "RegetAdmin/GetUserAdminCompanies?t=" + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        _this.userAdminCompanies = tmpData;
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
            MultiplyAppLevelController.prototype.multiplyAppLevels = function () {
                if (this.isMultiplyValid() === false) {
                    this.displayErrorMsg(this.locEnterMandatoryValuesText);
                    return;
                }
                this.isAll = (this.copyOption == "All");
                var msg = (this.isAll) ? this.locAppLevelMultiplyAllConfirmText : this.locAppLevelMultiplySelectedConfirmText;
                this.$mdDialog.show({
                    template: this.getConfirmDialogTemplate(msg, this.locConfirmationText, this.locMultiplyText, this.locCancelText, "confirmDialog()", "closeDialog()"),
                    locals: {
                        multiplyControl: this
                    },
                    controller: this.dialogConfirmController
                });
                //var confirm = this.$mdDialog.confirm()
                //    .title(this.locMultiplyText)
                //    .textContent(msg)
                //    .ariaLabel(this.locMultiplyText)
                //    .targetEvent()
                //    .ok(this.locCancelText)
                //    .cancel(this.locMultiplyText);
                //this.$mdDialog.show(confirm).then(() => {
                //}, () => {
                //    this.multiplyAppLevelsConfirmed();
                //});
            };
            MultiplyAppLevelController.prototype.multiplWasChanged = function (strMultipl, input) {
                this.validateDecimalNumber(strMultipl, input);
            };
            //private isValid(): boolean {
            //    let isValid: boolean = true;
            //    if (this.isValueNullOrUndefined(this.multiplText)) {
            //        if (this.angScopeAny.frmMultiplyAppLevel.txtMultiFactor.$valid == false) {
            //            this.angScopeAny.frmMultiplyAppLevel.txtMultiFactor.$setTouched();
            //        }
            //        isValid = false;
            //    }
            //    return isValid;
            //}
            MultiplyAppLevelController.prototype.dialogConfirmController = function ($scope, $mdDialog, multiplyControl) {
                $scope.closeDialog = function () {
                    $mdDialog.hide();
                };
                $scope.confirmDialog = function () {
                    $mdDialog.hide();
                    multiplyControl.multiplyAppLevelsConfirmed();
                };
            };
            MultiplyAppLevelController.prototype.multiplyAppLevelsConfirmed = function () {
                var _this = this;
                this.showLoaderBoxOnly();
                var multipleCompany = new MultipleCompany(this.userAdminCompanies, this.multipl, this.isAll);
                var jsonMultipleCompany = JSON.stringify(multipleCompany);
                this.$http.post(this.getRegetRootUrl() + 'RegetAdmin/MultiplyAppLevels', jsonMultipleCompany).then(function (response) {
                    try {
                        var tmpData = response.data;
                        _this.multiplyLimitResult = tmpData;
                        if (_this.isValueNullOrUndefined(_this.multiplyLimitResult.err_msg) || _this.multiplyLimitResult.err_msg.length == 0) {
                            var limitAffected = _this.multiplyLimitResult.affected_limits_count;
                            var msg = _this.locLimitMultiplyResultText.replace("{0}", limitAffected.toString());
                            //this.showAlert(this.locFinishedText, msg, this.locCloseText);
                            _this.showInfo(_this.locFinishedText, msg, _this.locCloseText);
                        }
                        else {
                            _this.displayErrorMsg(_this.locMultiplPgLimitErrorText);
                        }
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
            MultiplyAppLevelController.prototype.toggleCompany = function (company) {
                company.is_selected = !company.is_selected;
                var divErrCompany = document.getElementById("divErrCompany");
                if (this.isCompanySelected()) {
                    divErrCompany.style.display = "none";
                }
                else {
                    divErrCompany.style.display = "block";
                }
            };
            MultiplyAppLevelController.prototype.loadData = function () {
                try {
                    this.loadSubstUsers();
                }
                catch (ex) {
                    this.displayErrorMsg();
                }
            };
            MultiplyAppLevelController.prototype.isMultiplyValid = function () {
                var isValid = true;
                this.substErrMsg = null;
                if (this.isValueNullOrUndefined(this.multiplText)) {
                    if (this.angScopeAny.frmMultiplyAppLevel.txtMultiFactor.$valid == false) {
                        this.angScopeAny.frmMultiplyAppLevel.txtMultiFactor.$setTouched();
                    }
                    isValid = false;
                }
                if (!this.isStringDecimalNumber(this.multiplText)) {
                    if (this.angScopeAny.frmMultiplyAppLevel.txtMultiFactor.$valid == false) {
                        this.angScopeAny.frmMultiplyAppLevel.txtMultiFactor.$setTouched();
                    }
                    isValid = false;
                }
                else {
                    if (this.convertTextToDecimal(this.multiplText) > 0) {
                        this.multipl = this.convertTextToDecimal(this.multiplText);
                    }
                    else {
                        if (this.angScopeAny.frmMultiplyAppLevel.txtMultiFactor.$valid == false) {
                            this.angScopeAny.frmMultiplyAppLevel.txtMultiFactor.$setTouched();
                        }
                        isValid = false;
                    }
                }
                //if (this.isValueNullOrUndefined(this.userAdminCompanies)) {
                //    this.substErrMsg = this.locSelectCompanyText;
                //    isValid = false;
                //}
                //var isCompanySelected = false;
                //for (var i: number = 0; i < this.userAdminCompanies.length; i++) {
                //    if (this.userAdminCompanies[i].is_selected === true) {
                //        isCompanySelected = true;
                //        break
                //    }
                //}
                var isCompSel = this.isCompanySelected();
                if (isCompSel === false) {
                    //this.substErrMsg = this.locSelectCompanyText;
                    //isValid = false;
                    var divErrCompany = document.getElementById("divErrCompany");
                    divErrCompany.style.display = "block";
                    isValid = false;
                }
                return isValid;
            };
            MultiplyAppLevelController.prototype.isCompanySelected = function () {
                for (var i = 0; i < this.userAdminCompanies.length; i++) {
                    if (this.userAdminCompanies[i].is_selected === true) {
                        return true;
                    }
                }
                return false;
            };
            MultiplyAppLevelController.prototype.toggleAllCompanies = function () {
                if (this.isValueNullOrUndefined(this.userAdminCompanies)) {
                    return;
                }
                if (this.isAllCompaniesChecked()) {
                    for (var i = 0; i < this.userAdminCompanies.length; i++) {
                        this.userAdminCompanies[i].is_selected = false;
                    }
                }
                else {
                    for (var j = 0; j < this.userAdminCompanies.length; j++) {
                        this.userAdminCompanies[j].is_selected = true;
                    }
                }
                var divErrCompany = document.getElementById("divErrCompany");
                if (this.isCompanySelected()) {
                    divErrCompany.style.display = "none";
                }
                else {
                    divErrCompany.style.display = "block";
                }
            };
            MultiplyAppLevelController.prototype.isAllCompaniesChecked = function () {
                var isSelectedAll = true;
                if (this.isValueNullOrUndefined(this.userAdminCompanies)) {
                    return false;
                }
                for (var i = 0; i < this.userAdminCompanies.length; i++) {
                    if (!this.userAdminCompanies[i].is_selected) {
                        isSelectedAll = false;
                        break;
                    }
                }
                return isSelectedAll;
            };
            MultiplyAppLevelController.prototype.isIndeterminate = function () {
                if (this.isValueNullOrUndefined(this.userAdminCompanies)) {
                    return false;
                }
                var isAllChecked = this.isAllCompaniesChecked();
                return (this.userAdminCompanies.length !== 0 &&
                    !isAllChecked);
            };
            return MultiplyAppLevelController;
        }(RegetApp.BaseRegetTs));
        RegetApp.MultiplyAppLevelController = MultiplyAppLevelController;
        angular.
            module('RegetApp').
            controller('MultiplyAppLevelController', Kamsyk.RegetApp.MultiplyAppLevelController);
        var CompanyCheckbox = /** @class */ (function () {
            function CompanyCheckbox() {
                this.company_id = null;
                this.country_code = null;
                this.is_selected = false;
            }
            return CompanyCheckbox;
        }());
        RegetApp.CompanyCheckbox = CompanyCheckbox;
        var MultipleCompany = /** @class */ (function () {
            function MultipleCompany(companies, multipl, isAll) {
                this.companies = null;
                this.multipl = null;
                this.isAll = false;
                this.companies = companies;
                this.multipl = multipl;
                this.isAll = isAll;
            }
            return MultipleCompany;
        }());
        RegetApp.MultipleCompany = MultipleCompany;
        var MultiplyLimitMsg = /** @class */ (function () {
            function MultiplyLimitMsg() {
                this.cgId = null;
                this.cgName = null;
                this.pgId = null;
                this.pgName = null;
                this.err_msg = null;
            }
            return MultiplyLimitMsg;
        }());
        RegetApp.MultiplyLimitMsg = MultiplyLimitMsg;
        var MultiplyLimitResult = /** @class */ (function () {
            function MultiplyLimitResult() {
                this.affected_limits_count = 0;
                this.err_msg = null;
            }
            return MultiplyLimitResult;
        }());
        RegetApp.MultiplyLimitResult = MultiplyLimitResult;
    })(RegetApp = Kamsyk.RegetApp || (Kamsyk.RegetApp = {}));
})(Kamsyk || (Kamsyk = {}));
//# sourceMappingURL=multiply-app-level.js.map