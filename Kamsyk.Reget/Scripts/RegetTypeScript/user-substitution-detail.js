/// <reference path="../typings/ui-grid/ui-grid.d.ts" />
/// <reference path="../RegetTypeScript/Base/reget-base-grid.ts" />
/// <reference path="../RegetTypeScript/Base/reget-entity.ts" />
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
/// <reference path="../typings/moment/moment.d.ts" />
var Kamsyk;
(function (Kamsyk) {
    var RegetApp;
    (function (RegetApp) {
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
        var UserSubstitutionDetailController = /** @class */ (function (_super) {
            __extends(UserSubstitutionDetailController, _super);
            //*******************************************************************
            ////************************************************************
            ////Other Texts
            //private dateTimeMomentFormatText = $("#DateTimeMomentFormatText").val();
            ////************************************************************
            //**********************************************************
            //Constructor
            function UserSubstitutionDetailController($scope, $http, $filter, $mdDialog, $mdToast, $q, $timeout) {
                var _this = _super.call(this, $scope, $http, $filter, $mdDialog, $mdToast, $q, $timeout) || this;
                _this.$scope = $scope;
                _this.$http = $http;
                _this.$filter = $filter;
                _this.$mdDialog = $mdDialog;
                _this.$mdToast = $mdToast;
                _this.$q = $q;
                _this.$timeout = $timeout;
                //********************* Properties **********************************
                _this.substId = null;
                _this.substitution = null;
                _this.isApproveButonVisible = false;
                _this.isRejectButonVisible = false;
                _this.isDeactivateButonVisible = false;
                _this.isEditableAuthor = false;
                _this.filter = null;
                _this.sort = null;
                _this.pageSize = null;
                _this.currentPage = null;
                _this.newSubstDisplayed = null;
                //public toDate: Date = null;
                //public fromDate: moment.Moment = moment();
                _this.dirToOptions = {}; //must be here
                _this.isReadOnly = true;
                _this.isFromDateEdit = false;
                _this.minValueErrMsg = null;
                _this.isActive = false;
                _this.isWaitForApproval = false;
                _this.dateFromMin = new Date();
                _this.dateToMin = null;
                _this.substErrMsg = null;
                _this.isDateFromValid = false;
                _this.isDateToValid = false;
                _this.newRemarkItem = null;
                _this.discussion = null;
                _this.isSubstitutionLoaded = false;
                _this.isDiscussionLoaded = false;
                //private currentUserName: string = $("#CurrentUserName").val();
                //********************************************************************
                //****************** Localization ************************************
                _this.locMinDateText = angular.element("#MinDateText").val();
                _this.locConfirmationText = angular.element("#ConfirmationText").val();
                _this.locSubstDeactivateConfirmText = angular.element("#SubstDeactivateConfirmText").val();
                _this.locDeactivateText = angular.element("#DeactivateText").val();
                _this.locActiveText = angular.element("#ActiveText").val();
                _this.locNonActiveText = angular.element("#NonActiveText").val();
                _this.locNotAuthorizedPerformActionText = $("#NotAuthorizedPerformActionText").val();
                _this.locFromText = $("#FromText").val();
                _this.locToText = $("#ToText").val();
                _this.locMissingDateFromText = $("#MissingDateFromText").val();
                _this.locMissingDateToText = $("#MissingDateToText").val();
                _this.locSubstitutionPastText = $("#SubstitutionPastText").val();
                _this.locApproveConfirmText = angular.element("#ApproveConfirmText").val();
                _this.locRejectConfirmText = angular.element("#RejectConfirmText").val();
                _this.locApprovedText = $("#ApprovedText").val();
                _this.locRejectedText = $("#RejectedText").val();
                //***************************************************************
                _this.$onInit = function () { };
                _this.loadData();
                return _this;
            }
            //************************************************************************
            //Http
            UserSubstitutionDetailController.prototype.getSubstitutionData = function () {
                var _this = this;
                this.showLoader();
                this.$http.get(this.getRegetRootUrl() + "Participant/GetUserSubstitutionDetail?" +
                    "id=" + this.substId +
                    "&t=" + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpRes = response;
                        _this.substitution = tmpRes.data;
                        _this.isApproveButonVisible = (!_this.substitution.is_approve_hidden);
                        _this.isDeactivateButonVisible = (_this.substitution.is_can_be_deleted);
                        _this.isEditableAuthor = (_this.substitution.is_editable_author);
                        _this.isReadOnly = !_this.isEditableAuthor;
                        //this.modifDate = this.convertJsonDate(this.substitution.modified_date).format();
                        _this.substitution.substitute_end_date = _this.convertJsonDate(_this.substitution.substitute_end_date).toDate();
                        if (_this.isEditableAuthor) {
                            _this.dirToOptions.setDirTime(_this.substitution.substitute_end_date);
                        }
                        //this.fromDate = this.convertJsonDate(this.substitution.substitute_start_date);
                        _this.substitution.substitute_start_date = _this.convertJsonDate(_this.substitution.substitute_start_date).toDate();
                        _this.isActive = _this.substitution.active;
                        if (_this.substitution.approval_status == RegetApp.ApprovalStatus.Rejected
                            || _this.substitution.approval_status == RegetApp.ApprovalStatus.WaitForApproval) {
                            _this.isActive = false;
                        }
                        _this.isWaitForApproval = (_this.substitution.approval_status == RegetApp.ApprovalStatus.WaitForApproval);
                        _this.isFromDateEdit = _this.isEditableAuthor && (_this.substitution.substitute_start_date >= new Date());
                        _this.isSubstitutionLoaded = true;
                        _this.getSubstitutionDiscussionData();
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
            UserSubstitutionDetailController.prototype.getSubstitutionDiscussionData = function () {
                var _this = this;
                this.showLoader(this.isError);
                this.$http.get(this.getRegetRootUrl() + "Discussion/GetSubstitutionDiscussion?" +
                    "substId=" + this.substId +
                    "&t=" + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpRes = response;
                        _this.discussion = tmpRes.data;
                        _this.isDiscussionLoaded = true;
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
            UserSubstitutionDetailController.prototype.deactivate = function () {
                var _this = this;
                this.showLoaderBoxOnly(this.isError);
                var jsonSubst = JSON.stringify(this.substitution);
                this.$http.post(this.getRegetRootUrl() + 'Participant/DeactivateUserSubstitution?&t=' + new Date().getTime(), jsonSubst).then(function (response) {
                    try {
                        var tmpData = response.data;
                        var httpResult = tmpData;
                        if (!_this.isValueNullOrUndefined(httpResult.error_id) && httpResult.error_id > 0) {
                            if (httpResult.error_id == 110) {
                                //Not Auhorized
                                _this.displayErrorMsg(_this.locNotAuthorizedPerformActionText);
                            }
                            else {
                                _this.displayErrorMsg();
                            }
                            return;
                        }
                        _this.substitution.active = false;
                        _this.substitution.active_status_text = _this.locNonActiveText;
                        _this.substitution.modified_date_text = moment().format(_this.locDateTimePickerFormatMomentText);
                        _this.substitution.modified_user_name = _this.currentUserName;
                        _this.isApproveButonVisible = false;
                        _this.isRejectButonVisible = false;
                        _this.isDeactivateButonVisible = false;
                        _this.isEditableAuthor = false;
                        _this.isReadOnly = true;
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
            UserSubstitutionDetailController.prototype.saveSubstitution = function () {
                var _this = this;
                if (this.isSubstitutionValid() !== true) {
                    var strMsg = this.locDataCannotBeSavedText;
                    if (!this.isStringValueNullOrEmpty(this.substErrMsg)) {
                        strMsg += "<br>" + this.substErrMsg;
                    }
                    this.displayErrorMsg(strMsg);
                    return;
                }
                this.showLoaderBoxOnly(this.isError);
                var jsonSubst = JSON.stringify(this.substitution);
                this.$http.post(this.getRegetRootUrl() + 'Participant/SaveUserSubstitution?&t=' + new Date().getTime(), jsonSubst).then(function (response) {
                    try {
                        _this.substitution.modified_date_text = moment().format(_this.locDateTimePickerFormatMomentText);
                        _this.substitution.modified_user_name = _this.currentUserName;
                        _this.hideLoader();
                    }
                    catch (e) {
                        //this.substitution.is_allow_take_over = !this.substitution.is_allow_take_over;
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        _this.hideLoader();
                    }
                }, function (response) {
                    //this.substitution.is_allow_take_over = !this.substitution.is_allow_take_over;
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
            };
            UserSubstitutionDetailController.prototype.rejectSubstitution = function () {
                var _this = this;
                this.showLoaderBoxOnly(this.isError);
                var jsonEntityData = JSON.stringify({ substId: this.substId });
                this.$http.post(this.getRegetRootUrl() + "Participant/RejectSubstitution?t=" + new Date().getTime(), jsonEntityData).then(function (response) {
                    try {
                        var result = response.data;
                        var strResult = result.string_value;
                        if (_this.isStringValueNullOrEmpty(strResult)) {
                            _this.substitution.modified_date_text = moment().format(_this.locDateTimePickerFormatMomentText);
                            _this.substitution.modified_user_name = _this.currentUserName;
                            _this.substitution.approval_status = RegetApp.ApprovalStatus.Rejected;
                            _this.substitution.approval_status_text = _this.locRejectedText;
                            _this.substitution.active = false;
                            _this.substitution.active_status_text = _this.locRejectedText;
                            _this.isApproveButonVisible = false;
                            _this.isRejectButonVisible = false;
                            _this.isDeactivateButonVisible = false;
                            _this.isEditableAuthor = false;
                            _this.isReadOnly = true;
                            _this.isActive = false;
                            _this.isWaitForApproval = false;
                        }
                        else {
                            var msgError = _this.getErrorMessage(strResult);
                            _this.showAlert(_this.locErrorTitleText, msgError, _this.locCloseText);
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
            UserSubstitutionDetailController.prototype.approveSubstitution = function () {
                var _this = this;
                this.showLoaderBoxOnly(this.isError);
                var jsonEntityData = JSON.stringify({ substId: this.substId });
                this.$http.post(this.getRegetRootUrl() + "Participant/ApproveSubstitution?t=" + new Date().getTime(), jsonEntityData).then(function (response) {
                    try {
                        var result = response.data;
                        var strResult = result.string_value;
                        if (_this.isStringValueNullOrEmpty(strResult)) {
                            _this.substitution.modified_date_text = moment().format(_this.locDateTimePickerFormatMomentText);
                            _this.substitution.modified_user_name = _this.currentUserName;
                            _this.substitution.approval_status = RegetApp.ApprovalStatus.Approved;
                            _this.substitution.approval_status_text = _this.locApprovedText;
                            _this.substitution.active = false;
                            _this.substitution.active_status_text = _this.locActiveText;
                            _this.isApproveButonVisible = false;
                            _this.isRejectButonVisible = false;
                            _this.isDeactivateButonVisible = false;
                            _this.isEditableAuthor = false;
                            _this.isReadOnly = true;
                            _this.isActive = true;
                            _this.isWaitForApproval = false;
                        }
                        else {
                            var msgError = _this.getErrorMessage(strResult);
                            _this.showAlert(_this.locErrorTitleText, msgError, _this.locCloseText);
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
            UserSubstitutionDetailController.prototype.addRemark = function () {
                var _this = this;
                if (this.isStringValueNullOrEmpty(this.newRemarkItem)) {
                    return;
                }
                this.showLoaderBoxOnly(this.isError);
                var jsonEntityData = JSON.stringify({ substId: this.substId, remark: this.newRemarkItem });
                this.$http.post(this.getRegetRootUrl() + "Discussion/AddSubstitutiuonRemark?t=" + new Date().getTime(), jsonEntityData).then(function (response) {
                    try {
                        var result = response.data;
                        var strResult = result.string_value;
                        if (_this.isStringValueNullOrEmpty(strResult)) {
                            _this.addDiscussionItem(_this.newRemarkItem, _this.discussion);
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
                            //disItem.modif_date_text = moment().format(this.dateTimeMomentFormatText);
                            //disItem.bkg_color = this.discussion.discussion_bkg_color;
                            //disItem.border_color = this.discussion.discussion_border_color;
                            //disItem.user_color = this.discussion.discussion_user_color;
                            //if (this.isValueNullOrUndefined(this.discussion.discussion_items)) {
                            //    this.discussion.discussion_items = [];
                            //}
                            //this.discussion.discussion_items.push(disItem);
                            _this.newRemarkItem = "";
                        }
                        else {
                            var msgError = _this.getErrorMessage(strResult);
                            _this.showAlert(_this.locErrorTitleText, msgError, _this.locCloseText);
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
            //******************************************************************
            //******************************************************************
            //Methods
            UserSubstitutionDetailController.prototype.loadData = function () {
                try {
                    this.dateFromMin = new Date();
                    this.dateToMin = new Date();
                    //let fromDate: moment.Moment = moment();
                    this.minValueErrMsg = this.locMinDateText; //.replace("{0}", fromDate.format(this.locDateTimePickerFormatMomentText));
                    var urlParams = this.getAllUrlParams();
                    if (!this.isValueNullOrUndefined(urlParams)) {
                        for (var i = 0; i < urlParams.length; i++) {
                            if (urlParams[i].param_name.toLowerCase() === "id") {
                                this.substId = parseInt(urlParams[i].param_value);
                            }
                            else if (urlParams[i].param_name.toLowerCase() === "filter") {
                                this.filter = urlParams[i].param_value;
                            }
                            else if (urlParams[i].param_name.toLowerCase() === "sort") {
                                this.sort = urlParams[i].param_value;
                            }
                            else if (urlParams[i].param_name.toLowerCase() === "pagesize") {
                                this.pageSize = urlParams[i].param_value;
                            }
                            else if (urlParams[i].param_name.toLowerCase() === "currpage") {
                                this.currentPage = urlParams[i].param_value;
                            }
                            else if (urlParams[i].param_name.toLowerCase() === "newsubstdisplayed") {
                                this.newSubstDisplayed = urlParams[i].param_value;
                            }
                        }
                    }
                    if (this.substId == null) {
                        return;
                    }
                    this.getSubstitutionData();
                }
                catch (e) {
                    this.displayErrorMsg();
                }
            };
            UserSubstitutionDetailController.prototype.goBack = function () {
                var url = this.getRegetRootUrl() + "Participant/UserSubstitution";
                if (!this.isStringValueNullOrEmpty(this.filter)) {
                    url = this.addUrlParam(url, "filter", this.filter);
                }
                if (!this.isStringValueNullOrEmpty(this.sort)) {
                    url = this.addUrlParam(url, "sort", this.sort);
                }
                if (!this.isStringValueNullOrEmpty(this.pageSize)) {
                    url = this.addUrlParam(url, "pageSize", this.pageSize);
                }
                if (!this.isStringValueNullOrEmpty(this.currentPage)) {
                    url = this.addUrlParam(url, "currPage", this.currentPage);
                }
                if (!this.isStringValueNullOrEmpty(this.newSubstDisplayed)) {
                    url = this.addUrlParam(url, "newSubstDisplayed", this.newSubstDisplayed);
                }
                window.location.href = url;
            };
            UserSubstitutionDetailController.prototype.handOverCheck = function () {
                this.substitution.is_allow_take_over = !this.substitution.is_allow_take_over;
                //this.saveSubstitution();
            };
            UserSubstitutionDetailController.prototype.deactivateDialog = function () {
                this.$mdDialog.show({
                    template: this.getConfirmDialogTemplate(this.locSubstDeactivateConfirmText, this.locConfirmationText, this.locDeactivateText, this.locCloseText, "confirmDeactivateDialog()", "closeDialog()"),
                    locals: {
                        userSubst: this
                    },
                    controller: this.dialogConfirmController
                });
            };
            UserSubstitutionDetailController.prototype.rejectDialog = function () {
                var strReplMsg = this.substitution.substituted_name_surname_first + " -> " + this.substitution.substitutee_name_surname_first
                    + " " + (this.substitution.substitute_start_date_text) + " - " + (this.substitution.substitute_end_date_text);
                var strMsg = this.locRejectConfirmText.replace("{0}", strReplMsg);
                this.$mdDialog.show({
                    template: this.getConfirmDialogTemplate(strMsg, this.locConfirmationText, this.locYesText, this.locNoText, "confirmRejectDialog()", "closeDialog()"),
                    locals: {
                        userSubstControl: this
                    },
                    controller: this.dialogConfirmController
                });
            };
            UserSubstitutionDetailController.prototype.approveDialog = function () {
                var strReplMsg = this.substitution.substituted_name_surname_first + " -> " + this.substitution.substitutee_name_surname_first
                    + " " + (this.substitution.substitute_start_date_text) + " - " + (this.substitution.substitute_end_date_text);
                var strMsg = this.locApproveConfirmText.replace("{0}", strReplMsg);
                this.$mdDialog.show({
                    template: this.getConfirmDialogTemplate(strMsg, this.locConfirmationText, this.locYesText, this.locNoText, "confirmApproveDialog()", "closeDialog()"),
                    locals: {
                        userSubstControl: this
                    },
                    controller: this.dialogConfirmController
                });
            };
            UserSubstitutionDetailController.prototype.dialogConfirmController = function ($scope, $mdDialog, userSubstControl) {
                $scope.closeDialog = function () {
                    $mdDialog.hide();
                };
                $scope.confirmDeactivateDialog = function () {
                    $mdDialog.hide();
                    userSubstControl.deactivate();
                };
                $scope.confirmRejectDialog = function () {
                    $mdDialog.hide();
                    userSubstControl.rejectSubstitution();
                };
                $scope.confirmApproveDialog = function () {
                    $mdDialog.hide();
                    userSubstControl.approveSubstitution();
                };
            };
            UserSubstitutionDetailController.prototype.dateTimeToChanged = function (controlIndex, modifiedDate) {
                if (controlIndex === "0") {
                    this.dateToMin = modifiedDate;
                }
            };
            UserSubstitutionDetailController.prototype.isSubstitutionValid = function () {
                try {
                    this.substErrMsg = null;
                    if (this.isValueNullOrUndefined(this.substitution.substitute_start_date) || this.isDateFromValid === false) {
                        this.substErrMsg = this.locMissingDateFromText;
                        return false;
                    }
                    if (this.isValueNullOrUndefined(this.substitution.substitute_end_date || this.isDateToValid === false)) {
                        this.substErrMsg = this.locMissingDateToText;
                        return false;
                    }
                    if (this.substitution.substitute_start_date > this.substitution.substitute_end_date) {
                        this.substErrMsg = this.locFromText + ' > ' + this.locToText;
                        return false;
                    }
                    if (this.substitution.substitute_start_date < moment(this.dateFromMin).add(-1, "minutes").toDate()) {
                        this.substErrMsg = this.locSubstitutionPastText;
                        return false;
                    }
                    //if (this.substitution.substitute_end_date < new Date()) {
                    //    this.substErrMsg = this.locSubstitutionPastText;
                    //    return false;
                    //}
                    return true;
                }
                catch (_a) {
                    this.substErrMsg = this.locErrorMsgText;
                    return false;
                }
            };
            UserSubstitutionDetailController.prototype.hideLoaderWrapper = function () {
                if (this.isError || (this.isSubstitutionLoaded === true &&
                    this.isDiscussionLoaded === true)) {
                    this.hideLoader();
                }
            };
            return UserSubstitutionDetailController;
        }(RegetApp.BaseRegetTs));
        RegetApp.UserSubstitutionDetailController = UserSubstitutionDetailController;
        angular.
            module('RegetApp').
            controller('UserSubstitutionDetailController', Kamsyk.RegetApp.UserSubstitutionDetailController).
            config(function ($mdDateLocaleProvider) {
            this.SetDatePicker($mdDateLocaleProvider);
            //it is neccessary to set IsGenerateDatePickerLocalization = true
        });
    })(RegetApp = Kamsyk.RegetApp || (Kamsyk.RegetApp = {}));
})(Kamsyk || (Kamsyk = {}));
//# sourceMappingURL=user-substitution-detail.js.map