/// <reference path="../typings/ui-grid/ui-grid.d.ts" />
/// <reference path="../RegetTypeScript/Base/reget-base-grid.ts" />
/// <reference path="../RegetTypeScript/Base/reget-entity.ts" />

/// <reference path="../typings/moment/moment.d.ts" />

module Kamsyk.RegetApp {

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

    export class UserSubstitutionDetailController extends BaseRegetTs implements angular.IController {
        //********************* Properties **********************************
        private substId: number = null;
        private substitution: UserSubstitution = null;
        private isApproveButonVisible: boolean = false;
        private isRejectButonVisible: boolean = false;
        private isDeactivateButonVisible: boolean = false;
        private isEditableAuthor: boolean = false;
        private filter: string = null;
        private sort: string = null;
        private pageSize: string = null;
        private currentPage: string = null;
        private newSubstDisplayed: string = null;
        //public toDate: Date = null;
        //public fromDate: moment.Moment = moment();
        private dirToOptions: any = {};//must be here
        private isReadOnly: boolean = true;
        private isFromDateEdit: boolean = false;
        private minValueErrMsg: string = null;
        private isActive: boolean = false;
        private isWaitForApproval: boolean = false;
        private dateFromMin: Date = new Date();
        private dateToMin: Date = null;
        private substErrMsg: string = null;
        private isDateFromValid: boolean = false;
        private isDateToValid: boolean = false;
        private newRemarkItem: string = null;
        private discussion: Discussion = null;
        private isSubstitutionLoaded: boolean = false;
        private isDiscussionLoaded: boolean = false;
        

        //private currentUserName: string = $("#CurrentUserName").val();
        //********************************************************************

        //****************** Localization ************************************
        private locMinDateText: string = angular.element("#MinDateText").val();
        private locConfirmationText: string = angular.element("#ConfirmationText").val();
        private locSubstDeactivateConfirmText: string = angular.element("#SubstDeactivateConfirmText").val();
        private locDeactivateText: string = angular.element("#DeactivateText").val();
        private locActiveText: string = angular.element("#ActiveText").val();
        private locNonActiveText: string = angular.element("#NonActiveText").val();
        private locNotAuthorizedPerformActionText: string = $("#NotAuthorizedPerformActionText").val();
        private locFromText: string = $("#FromText").val();
        private locToText: string = $("#ToText").val();
        private locMissingDateFromText: string = $("#MissingDateFromText").val();
        private locMissingDateToText: string = $("#MissingDateToText").val();
        private locSubstitutionPastText: string = $("#SubstitutionPastText").val();
        private locApproveConfirmText: string = angular.element("#ApproveConfirmText").val();
        private locRejectConfirmText: string = angular.element("#RejectConfirmText").val();
        private locApprovedText: string = $("#ApprovedText").val();
        private locRejectedText: string = $("#RejectedText").val();

       //*******************************************************************

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
            protected $q: ng.IQService,
            protected $timeout: ng.ITimeoutService

        ) {
            super($scope, $http, $filter, $mdDialog, $mdToast, $q, $timeout);

            this.loadData();

        }
        //***************************************************************

        $onInit = () => { };

        //************************************************************************
        //Http
        private getSubstitutionData() {
            this.showLoader();
                       
            this.$http.get(
                this.getRegetRootUrl() + "Participant/GetUserSubstitutionDetail?" +
                "id=" + this.substId +
                "&t=" + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpRes: any = response
                    this.substitution = tmpRes.data; 

                    this.isApproveButonVisible = (!this.substitution.is_approve_hidden);
                    this.isDeactivateButonVisible = (this.substitution.is_can_be_deleted);
                    this.isEditableAuthor = (this.substitution.is_editable_author);
                    this.isReadOnly = !this.isEditableAuthor;
                    //this.modifDate = this.convertJsonDate(this.substitution.modified_date).format();
                                        
                    this.substitution.substitute_end_date = this.convertJsonDate(this.substitution.substitute_end_date).toDate();
                    if (this.isEditableAuthor) {
                        this.dirToOptions.setDirTime(this.substitution.substitute_end_date);
                    }

                    //this.fromDate = this.convertJsonDate(this.substitution.substitute_start_date);
                    this.substitution.substitute_start_date = this.convertJsonDate(this.substitution.substitute_start_date).toDate();

                    this.isActive = this.substitution.active;
                    if (this.substitution.approval_status == ApprovalStatus.Rejected
                        || this.substitution.approval_status == ApprovalStatus.WaitForApproval) {
                        this.isActive = false;
                    } 

                    this.isWaitForApproval = (this.substitution.approval_status == ApprovalStatus.WaitForApproval);

                    this.isFromDateEdit = this.isEditableAuthor && (this.substitution.substitute_start_date >= new Date());

                    this.isSubstitutionLoaded = true;

                    this.getSubstitutionDiscussionData();
                    
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

        private getSubstitutionDiscussionData() {
            this.showLoader(this.isError);

            this.$http.get(
                this.getRegetRootUrl() + "Discussion/GetSubstitutionDiscussion?" +
                "substId=" + this.substId +
                "&t=" + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpRes: any = response
                    this.discussion = tmpRes.data;

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

        private deactivate(): void {
            this.showLoaderBoxOnly(this.isError);
            var jsonSubst = JSON.stringify(this.substitution);

            this.$http.post(
                this.getRegetRootUrl() + 'Participant/DeactivateUserSubstitution?&t=' + new Date().getTime(),
                jsonSubst
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    let httpResult: HttpResult = tmpData;

                    if (!this.isValueNullOrUndefined(httpResult.error_id) && httpResult.error_id > 0) {
                        if (httpResult.error_id == 110) {
                            //Not Auhorized
                            this.displayErrorMsg(this.locNotAuthorizedPerformActionText);
                        } else {
                            this.displayErrorMsg();
                        }

                        return;
                    }

                    this.substitution.active = false;
                    this.substitution.active_status_text = this.locNonActiveText;
                    this.substitution.modified_date_text = moment().format(this.locDateTimePickerFormatMomentText);
                    this.substitution.modified_user_name = this.currentUserName;

                    this.isApproveButonVisible = false;
                    this.isRejectButonVisible = false;
                    this.isDeactivateButonVisible = false;
                    this.isEditableAuthor = false;
                    this.isReadOnly = true;

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

        private saveSubstitution(): void {
            if (this.isSubstitutionValid() !== true) {
                var strMsg: string = this.locDataCannotBeSavedText;
                if (!this.isStringValueNullOrEmpty(this.substErrMsg)) {
                    strMsg += "<br>" + this.substErrMsg;
                }
                this.displayErrorMsg(strMsg);
                return;
            }

            this.showLoaderBoxOnly(this.isError);
            let jsonSubst = JSON.stringify(this.substitution);

            this.$http.post(
                this.getRegetRootUrl() + 'Participant/SaveUserSubstitution?&t=' + new Date().getTime(),
                jsonSubst
            ).then((response) => {
                try {
                    
                    this.substitution.modified_date_text = moment().format(this.locDateTimePickerFormatMomentText);
                    this.substitution.modified_user_name = this.currentUserName;
                                        
                    this.hideLoader();
                } catch (e) {
                    //this.substitution.is_allow_take_over = !this.substitution.is_allow_take_over;
                    this.hideLoader();
                    this.displayErrorMsg();
                } finally {
                    this.hideLoader();
                }
            }, (response: any) => {
                //this.substitution.is_allow_take_over = !this.substitution.is_allow_take_over;
                this.hideLoader();
                this.displayErrorMsg();
            });
            
        }

        private rejectSubstitution(): void {

            this.showLoaderBoxOnly(this.isError);

            var jsonEntityData = JSON.stringify({ substId: this.substId });

            this.$http.post(
                this.getRegetRootUrl() + "Participant/RejectSubstitution?t=" + new Date().getTime(),
                jsonEntityData
            ).then((response) => {
                try {
                    var result: any = response.data;
                    var strResult = result.string_value;
                    if (this.isStringValueNullOrEmpty(strResult)) {

                        this.substitution.modified_date_text = moment().format(this.locDateTimePickerFormatMomentText);
                        this.substitution.modified_user_name = this.currentUserName;
                        this.substitution.approval_status = ApprovalStatus.Rejected;
                        this.substitution.approval_status_text = this.locRejectedText;
                        this.substitution.active = false;
                        this.substitution.active_status_text = this.locRejectedText;

                        this.isApproveButonVisible = false;
                        this.isRejectButonVisible = false;
                        this.isDeactivateButonVisible = false;
                        this.isEditableAuthor = false;
                        this.isReadOnly = true;
                        this.isActive = false;
                        this.isWaitForApproval = false;

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

        private approveSubstitution(): void {

            this.showLoaderBoxOnly(this.isError);

            var jsonEntityData = JSON.stringify({ substId: this.substId });

            this.$http.post(
                this.getRegetRootUrl() + "Participant/ApproveSubstitution?t=" + new Date().getTime(),
                jsonEntityData
            ).then((response) => {
                try {
                    var result: any = response.data;
                    var strResult = result.string_value;
                    if (this.isStringValueNullOrEmpty(strResult)) {

                        this.substitution.modified_date_text = moment().format(this.locDateTimePickerFormatMomentText);
                        this.substitution.modified_user_name = this.currentUserName;
                        this.substitution.approval_status = ApprovalStatus.Approved;
                        this.substitution.approval_status_text = this.locApprovedText;
                        this.substitution.active = false;
                        this.substitution.active_status_text = this.locActiveText;

                        this.isApproveButonVisible = false;
                        this.isRejectButonVisible = false;
                        this.isDeactivateButonVisible = false;
                        this.isEditableAuthor = false;
                        this.isReadOnly = true;
                        this.isActive = true;
                        this.isWaitForApproval = false;

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

        private addRemark(): void {
            if (this.isStringValueNullOrEmpty(this.newRemarkItem)) {
                return;
            }

            
            this.showLoaderBoxOnly(this.isError);

            var jsonEntityData = JSON.stringify({ substId: this.substId, remark: this.newRemarkItem });

            this.$http.post(
                this.getRegetRootUrl() + "Discussion/AddSubstitutiuonRemark?t=" + new Date().getTime(),
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
        //******************************************************************

        //******************************************************************
        //Methods
        private loadData(): void {
            try {
                this.dateFromMin = new Date();
                this.dateToMin = new Date();
                //let fromDate: moment.Moment = moment();
                this.minValueErrMsg = this.locMinDateText; //.replace("{0}", fromDate.format(this.locDateTimePickerFormatMomentText));

                let urlParams : UrlParam[] = this.getAllUrlParams();
                if (!this.isValueNullOrUndefined(urlParams)) {
                    for (var i: number = 0; i < urlParams.length; i++) {
                        if (urlParams[i].param_name.toLowerCase() === "id") {
                            this.substId = parseInt(urlParams[i].param_value);
                        } else if (urlParams[i].param_name.toLowerCase() === "filter") {
                            this.filter = urlParams[i].param_value;
                        } else if (urlParams[i].param_name.toLowerCase() === "sort") {
                            this.sort = urlParams[i].param_value;
                        } else if (urlParams[i].param_name.toLowerCase() === "pagesize") {
                            this.pageSize = urlParams[i].param_value;
                        } else if (urlParams[i].param_name.toLowerCase() === "currpage") {
                            this.currentPage = urlParams[i].param_value;
                        } else if (urlParams[i].param_name.toLowerCase() === "newsubstdisplayed") {
                            this.newSubstDisplayed = urlParams[i].param_value;
                        }
                    }
                }

                if (this.substId == null) {
                    return;
                }
                               
                this.getSubstitutionData();
            } catch (e) {
                this.displayErrorMsg();
            }
        }

       
        private goBack(): void {
            let url = this.getRegetRootUrl() + "Participant/UserSubstitution";
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
        } 

        private handOverCheck(): void {
            this.substitution.is_allow_take_over = !this.substitution.is_allow_take_over;

            //this.saveSubstitution();
        }

        private deactivateDialog(): void {
            this.$mdDialog.show(
                {
                    template: this.getConfirmDialogTemplate(
                        this.locSubstDeactivateConfirmText,
                        this.locConfirmationText,
                        this.locDeactivateText,
                        this.locCloseText,
                        "confirmDeactivateDialog()",
                        "closeDialog()"),
                    locals: {
                        userSubst: this
                    },
                    controller: this.dialogConfirmController
                });

        }

        private rejectDialog(): void {
            let strReplMsg: string = this.substitution.substituted_name_surname_first + " -> " + this.substitution.substitutee_name_surname_first
                + " " + (this.substitution.substitute_start_date_text) + " - " + (this.substitution.substitute_end_date_text);
            let strMsg: string = this.locRejectConfirmText.replace("{0}", strReplMsg);

            this.$mdDialog.show(
                {
                    template: this.getConfirmDialogTemplate(
                        strMsg,
                        this.locConfirmationText,
                        this.locYesText,
                        this.locNoText,
                        "confirmRejectDialog()",
                        "closeDialog()"),
                    locals: {
                        userSubstControl: this
                    },
                    controller: this.dialogConfirmController
                });

        }

        private approveDialog(): void {
            let strReplMsg: string = this.substitution.substituted_name_surname_first + " -> " + this.substitution.substitutee_name_surname_first
                + " " + (this.substitution.substitute_start_date_text) + " - " + (this.substitution.substitute_end_date_text);
            let strMsg: string = this.locApproveConfirmText.replace("{0}", strReplMsg);

            this.$mdDialog.show(
                {
                    template: this.getConfirmDialogTemplate(
                        strMsg,
                        this.locConfirmationText,
                        this.locYesText,
                        this.locNoText,
                        "confirmApproveDialog()",
                        "closeDialog()"),
                    locals: {
                        userSubstControl: this
                    },
                    controller: this.dialogConfirmController
                });

        }

        private dialogConfirmController($scope, $mdDialog, userSubstControl): void {

            $scope.closeDialog = function () {
                $mdDialog.hide();
            }

            $scope.confirmDeactivateDialog =() => {
                $mdDialog.hide();
                userSubstControl.deactivate();
            }

            $scope.confirmRejectDialog = () => {
                $mdDialog.hide();
                userSubstControl.rejectSubstitution();
            }

            $scope.confirmApproveDialog = () => {
                $mdDialog.hide();
                userSubstControl.approveSubstitution();
            }
        }

        public dateTimeToChanged(controlIndex: string, modifiedDate: Date): void {
            if (controlIndex === "0") {
                this.dateToMin = modifiedDate;
            }
        }

        private isSubstitutionValid(): boolean {
            try {
                this.substErrMsg = null;

                if (this.isValueNullOrUndefined(this.substitution.substitute_start_date) || this.isDateFromValid===false) {
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
            } catch {
                this.substErrMsg = this.locErrorMsgText;
                return false;
            }
        } 

        private hideLoaderWrapper(): void {
            if (this.isError || (this.isSubstitutionLoaded === true &&
                this.isDiscussionLoaded === true)) {
                this.hideLoader();
            }
        }
        //******************************************************************
    }

    angular.
        module('RegetApp').
        controller('UserSubstitutionDetailController', Kamsyk.RegetApp.UserSubstitutionDetailController).
        config(function ($mdDateLocaleProvider) { //only because of Date TIME picker is implemented
            this.SetDatePicker($mdDateLocaleProvider);
            //it is neccessary to set IsGenerateDatePickerLocalization = true
        });
    
}
