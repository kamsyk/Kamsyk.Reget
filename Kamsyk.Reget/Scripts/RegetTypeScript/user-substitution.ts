/// <reference path="../typings/ui-grid/ui-grid.d.ts" />
/// <reference path="../RegetTypeScript/Base/reget-base-grid.ts" />
/// <reference path="../RegetTypeScript/Base/reget-entity.ts" />

/// <reference path="../typings/moment/moment.d.ts" />

module Kamsyk.RegetApp {
    
    //*************************
    ////Enum
    //enum ApproveStatus {
    //    Empty = -1,
    //    Yes = 1,
    //    No = 0,
    //    NotNeeded = 2,
    //    WaitForApproval = 3
    //}
    //*************************

    export class UserSubstitutionController extends BaseRegetGridTs implements angular.IController {

        //****************************************
        //Abstract properties
        public dbGridId: string = "grdUserSubstitution_rg";
        //*****************************************

        public fromDate: Date = null;
        public toDate: Date = null;
        public fromDateFilter: Date;
        public toDateFilter: Date;
        public modifyDateFromFilter: Date;
        public modifyDateToFilter: Date;
        public substHandOverFromDate: Date;

        private isSubstLoaed: boolean = false;
        private isSubstUsersLoaed: boolean = false;
        public selectedsubstitutedUser: Participant;
        //private currentUser: Participant = null;
        
        private strSelectedsubstitutedUser: string;
        private searchstringusersubtituted: string;
        public selectedsubstituteeUser: Participant;
        private strSelectedsubstituteeUser: string;
        private searchstringusersubtitutee: string;
        private remark: string;
        private approval_status: number = -1;
        private substErrMsg: string;
        private substData: UserSubstitution[] = null;
        private fromDateErrMsg: string;
        private isSkipSubstitutedUserLoad: boolean = false;
        private isSkipSubstituteeUserLoad: boolean = false;
        private isdatetoclear: boolean = false;
        private dirFromOptions: any = {}; //must be here
        private dirToOptions: any = {};//must be here
        //private dirSubstHandOverFromOptions: any = {};
        private isTooltipScrollDisplayed: boolean = false;
        private tooltipRowIndex: number = -1;
        //private leftAddFix: number = 0;
        //private topAddFix: number = 0;
        private approversText: string = null;
        public approvers: Participant[] = null;

        //private currentUserId: number = $("#CurrentUserId").val();
        //private currentUserName: string = $("#CurrentUserName").val();
        private isNewValid: boolean = false;
        private isReadOnly: boolean = false;
        private isTakeOver: boolean = false;
        private isApproveColVisible: boolean = false;
        private isEditColVisible: boolean = false;
        private errMsg: string = null;
        private substitutedMen: Participant[] = null;
        //private actionCol: any = null;
        private iEditColIndex: number = -1;
        private editColumn: uiGrid.IColumnDefOf<any> = null;
        private iApproveColIndex: number = -1;
        private approveColumn: uiGrid.IColumnDefOf<any> = null;
        private isNewSustCollapsed: boolean = false;
        private dateFromMin: Date = new Date();
        private dateToMin: Date = null;
        private minFromValueErrMsg: string = null;
        private minToValueErrMsg: string = null;
        private isDateFromValid: boolean = false;
        private isDateToValid: boolean = false;
        //private yesNo: Array<uiGrid.ISelectOption> = [{ value: "true", label: this.locYesText }, { value: "false", label: this.locNoText }];
        private yesNo: any[] = [{ value: true, label: this.locYesText }, { value: false, label: this.locNoText }];

        //*********************************************************
        //Localized Texts
        private locSubstitutedText: string = $("#SubstitutedText").val();
        private locSubstituteeText: string = $("#SubstituteeText").val();
        private locFromText: string = $("#FromText").val();
        private locToText: string = $("#ToText").val();
        private locDateFromatText: string = $("#DateFromatText").val();
        private locPendingRequestsText: string = $("#PendigRequestsText").val();
        private locMissingSubstitutedText: string = $("#MissingSubstitutedText").val();
        private locMissingSubstituteeText: string = $("#MissingSubstituteeText").val();
        private locMissingDateFromText: string = $("#MissingDateFromText").val();
        private locMissingDateToText: string = $("#MissingDateToText").val();
        private locSubstitutionPastText: string= $("#SubstitutionPastText").val();
        private locRemarkText: string = $("#RemarkText").val();
        private locNotNeededText: string = $("#NotNeededText").val();
        private locApprovedText: string = $("#ApprovedText").val();
        private locRejectedText: string = $("#RejectedText").val();
        private locWaitForApprovalText: string = $("#WaitForApprovalText").val();
        private locDeleteSupplierConfirmText: string = $("#DeleteSupplierConfirmText").val();
        private locApprovalStatusText: string = $("#ApprovalStatusText").val();
        private locModifyDateText: string = $("#ModifyDateText").val();
        private locModifyUserText: string = $("#ModifyUserText").val();
        private locApproveText: string = $("#ApproveText").val();
        private locRejectText: string = $("#RejectText").val();
        private locAllowSubsToTakeOverShortText: string = $("#AllowSubsToTakeOverShortText").val();
        private locLoadDataErrorText: string = angular.element("#LoadDataErrorText").val();
        private locApproveManText: string = angular.element("#ApproveManText").val();
        private locApproveConfirmText: string = angular.element("#ApproveConfirmText").val();
        private locRejectConfirmText: string = angular.element("#RejectConfirmText").val();
        private locActiveText: string = angular.element("#ActiveText").val();
        private locDetailText: string = angular.element("#DetailText").val();
        private locMinDateText: string = angular.element("#MinDateText").val();
        //private locDeleteText: string = angular.element("#DeleteText").val();

        //ovevritten
        public locDeleteText: string = angular.element("#DeactivateText").val();
        //*****************************************************

        private approvalStatus: any[] = [
            { value: this.locNotNeededText, label: this.locNotNeededText },
            { value: this.locWaitForApprovalText, label: this.locWaitForApprovalText },
            { value: this.locApprovedText, label: this.locApprovedText },
            { value: this.locRejectedText, label: this.locRejectedText }
        ];

        //************************************************************
        //Other Texts
        //private dateTimeMomentFormatText = $("#DateTimeMomentFormatText").val();
        //************************************************************

        //**********************************************************
        //Constructor
        constructor(
            protected $scope: ng.IScope,
            protected $http: ng.IHttpService,
            protected $filter: ng.IFilterService,
            protected $mdDialog: angular.material.IDialogService,
            protected $mdToast: angular.material.IToastService,
            protected uiGridConstants: uiGrid.IUiGridConstants,
            protected $q: ng.IQService,
            protected $timeout: ng.ITimeoutService

            //protected $moment: moment.Moment
        ) {
            super($scope, $http, $filter, $mdDialog, $mdToast, $q, uiGridConstants, $timeout);
           
            this.setGrid();
            this.loadData();
                        
            //Do Not Delete it
            //this.angScopeAny.$on("myCustomEvent", function (event, data) {
            //    console.log(data); // 'Data to send'
            //    alert('sss');

            //});
        }
        //***************************************************************

        $onInit = () => { };

        //********************************************************************
        //Abstract Methods
        public isRowChanged(): boolean {
            let isChanged = false;

            let origSubst: UserSubstitution = this.editRowOrig;
            let editSubst: UserSubstitution = this.editRow;

            if (origSubst.substituted_user_id !== editSubst.substituted_user_id) {
                isChanged = true;
            } else if (origSubst.substitute_user_id !== editSubst.substitute_user_id) {
                isChanged = true;
            } else if (!this.isMomentDatesSame(origSubst.substitute_start_date, editSubst.substitute_start_date)) {
                isChanged = true;
            } else if (!this.isMomentDatesSame(origSubst.substitute_end_date, editSubst.substitute_end_date)) {
                isChanged = true;
            } else if (origSubst.approval_status !== editSubst.approval_status) {
                isChanged = true;
            } else if (origSubst.remark !== editSubst.remark) {
                isChanged = true;
            } else if (origSubst.is_allow_take_over !== editSubst.is_allow_take_over) {
                isChanged = true;
            }

            return isChanged;
        }

        public insertRow(): void { }

        public exportToXlsUrl(): string {
            return this.getRegetRootUrl() + 'Report/GetUserSubstitutionReport?filter=' + encodeURI(this.filterUrl) +
                '&sort=' + this.sortColumnsUrl +
                '&t=' + new Date().getTime();
        }

        public isRowEntityValid(rowEntity: any): string {
            //var isValid = true;
            if (this.isValueNullOrUndefined(rowEntity["substituted_user_id"])) {
                return this.locMissingMandatoryText;
            }

            if (this.isValueNullOrUndefined(rowEntity["substitute_user_id"])) {
                return this.locMissingMandatoryText;
            }

            if (this.isValueNullOrUndefined(rowEntity["substitute_start_date"])) {
                return this.locDateTimePickerFormatText;
            }

            if (this.isValueNullOrUndefined(rowEntity["substitute_end_date"])) {
                return this.locDateTimePickerFormatText;
            }

            var mFrom = moment(rowEntity["substitute_start_date"]);
            if (!mFrom.isValid()) {
                return this.locDateTimePickerFormatText;
            }

            let isFromReadOnly: boolean = (rowEntity["is_date_time_ro1"]);
            if (!isFromReadOnly) {
                let dateNowMinusHour = new Date();
                dateNowMinusHour.setHours(dateNowMinusHour.getHours() - 1);
                if (mFrom < moment(dateNowMinusHour)) {
                    return this.locSubstitutionPastText;
                }
            }

            var mTo = moment(rowEntity["substitute_end_date"]);
            if (!mTo.isValid()) {
                return this.locDateTimePickerFormatText;
            }
            if (mTo < moment(new Date())) {
                return this.locSubstitutionPastText;
            }

            if (mFrom > mTo) {
                var strFrom = moment(mFrom).format(this.dateTimeMomentFormatText);
                var strTo = moment(mTo).format(this.dateTimeMomentFormatText);
                return (strFrom + " > " + strTo);
            }

            return null;
        }

        public getSaveRowUrl(): string {
            return this.getRegetRootUrl() + 'Participant/SaveUserSubstitution?t=' + new Date().getTime();

        }

        public getDuplicityErrMsg(rowEntity: any): string {
            return null;
        }

        public getControlColumnsCount(): number {
            return 3;
        }

        public getMsgDisabled(userSubstitutionEntity: UserSubstitution): string {
            return null;//this.locSupplierWasDisabledText.replace("{0}", supplier.supp_name);
        }


        public getMsgDeleteConfirm(userSubstitutionEntity: UserSubstitution): string {
            return this.locDeleteSupplierConfirmText.replace("{0}", userSubstitutionEntity.substituted_name_surname_first);
        }

        public loadGridData(): void {
            this.getSubstitutionData();
        }

        public getErrorMsgByErrId(errId: number, msg: string): string {
            return this.locErrorMsgText;
        }

        public deleteUrl: string = this.getRegetRootUrl() + 'Participant/DeactivateUserSubstitution' + '?t=' + new Date().getTime();

        public getDbGridId(): string {
            return this.dbGridId;
        }

        //public insertEntity(rowIndex: number): void { }
        //************************************************************************

        //************************* Overwritten Methods *****************************
        public cellClicked(row?: any, col?: any): void {
            if (!this.isValueNullOrUndefined(row)) {
                let userSubst: UserSubstitution = row.entity;
                if (userSubst.is_edit_hidden == true) {
                    return;
                }
            }

            super.cellClicked(row, col);
        }
        //**************************************************************************
        
        //************************************************************************
        //Http
        public getSubstitutionData() {
            this.showLoaderBoxOnly(this.isError);

            if (this.currentPage < 1) {
                this.currentPage = 1;
            }
           // this.initDataLoadParams();

            ////let substData: UserSubstitution[] = null;
            //let strCurrPage: string = sessionStorage.getItem("currentPage");
            //if (!this.isStringValueNullOrEmpty(strCurrPage)) {
            //    this.currentPage = parseInt(strCurrPage);
            //}
            //sessionStorage.clear();

            this.$http.get(
                this.getRegetRootUrl() + "Participant/GetUserSubstitution?" +
                "filter=" + encodeURI(this.filterUrl) +
                "&pageSize=" + this.pageSize +
                "&currentPage=" + this.currentPage +
                "&sort=" + this.sortColumnsUrl +
                "&t=" + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpRes: any = response
                    this.formatGridDate(tmpRes.data.user_substitution.db_data);
                    this.substData = tmpRes.data.user_substitution.db_data;

                    this.gridOptions.data = this.substData;
                    this.rowsCount = tmpRes.data.user_substitution.rows_count;

                    this.isApproveColVisible = tmpRes.data.is_approve_visible;
                    this.isEditColVisible = tmpRes.data.is_edit_visible;
                    if (this.rowsCount == 0) {
                        this.currentPage = 0;
                    }

                    if (!this.isValueNullOrUndefined(this.gridApi)) {
                        this.gridApi.core.notifyDataChange(this.uiGridConstants.dataChange.COLUMN);
                    }
                    
                    this.setGridSettingData();
                    this.setEditApproveColButtons();

                    //********************************************************************
                    //it is very important otherwise 50 lines are not diplayed properly !!!
                    this.gridOptions.virtualizationThreshold = this.rowsCount + 1;
                    //********************************************************************

                    this.isSubstLoaed = true;
                    this.testLoadDataCount++;
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

            //return substData;
        }

        private saveSubstitution(subst: UserSubstitution): void {
            
            this.showLoaderBoxOnly(this.isError);
            var jsonEntityData = JSON.stringify(subst);

            this.$http.post(
                this.getRegetRootUrl() + "Participant/SaveUserSubstitution?t=" + new Date().getTime(),
                jsonEntityData
            ).then((response) => {
                try {
                    var result: ISaveDataResponse = response.data as ISaveDataResponse;
                    var iId = result.int_value;

                    if (iId === -1) {
                        this.hideLoader();
                        this.displayErrorMsg();
                        return;
                    }

                    var isNew = false;
                    if (subst.id < 0) {
                        isNew = true;
                        subst.id = iId;

                        this.substData.push(subst);
                        this.gridOptions.paginationPageSize++;
                    }

                    this.clearSubstitution();

                    //this.leftAddFix = -110;
                    //this.topAddFix = -75;

                    this.showToast(this.locDataWasSavedText);
                } catch (e) {
                    this.hideLoader();
                    this.displayErrorMsg();
                } finally {
                    this.hideLoader();
                }
            }, (response: any) => {
                this.hideLoader();
                this.displayErrorMsg();
                return false;
            });
        }

        private getSubstitutedApprovers(isLoadSubstData : boolean): Participant[] {
            let approvers: Participant[] = null;
            this.showLoaderBoxOnly(this.isError);

            this.$http.get(
                this.getRegetRootUrl() + "Participant/GetSubsitutedApprovers?substitutedId=" + this.selectedsubstitutedUser.id + "&t=" + new Date().getTime()
            ).then((response) => {
                try {
                    var result: any = response.data;
                    approvers = result;
                    //this.approvers = result;
                    if (this.isValueNullOrUndefined(result) || result.length == 0) {
                        this.approversText = this.locNotNeededText;
                        this.approval_status = ApprovalStatus.NotNeeded;
                    } else {
                        this.approversText = "";
                        for (var i = 0; i < result.length; i++) {
                            if (this.approversText.length > 0) {
                                this.approversText += ", ";
                            }
                            this.approversText += result[i].surname + " " + result[i].first_name;
                        }
                        this.approval_status = ApprovalStatus.WaitForApproval;
                    }

                    if (isLoadSubstData) {
                        //this.loadGridData();
                        this.getLoadDataGridSettings();
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
                return false;
            });

            return approvers;
        }


        public approveSubstitution(subst: UserSubstitution): void {

            this.showLoaderBoxOnly(this.isError);

            var jsonEntityData = JSON.stringify({ substId: subst.id });

            this.$http.post(
                this.getRegetRootUrl() + "Participant/ApproveSubstitution?t=" + new Date().getTime(),
                jsonEntityData
            ).then((response) => {
                try {
                    var result: any = response.data;
                    var strResult = result.string_value;
                    if (this.isStringValueNullOrEmpty(strResult)) {

                        subst.approval_status = ApprovalStatus.Approved;
                        subst.approval_status_text = this.locApprovedText;
                        subst.is_editable_app_man = false;
                        subst.is_approve_hidden = true;
                        subst.is_reject_hidden = true;
                        subst.is_can_be_deleted = false;

                        return;
                    } else {
                        let msgError = this.getErrorMessage(strResult);
                        this.showAlert(this.locErrorTitleText, msgError, this.locCloseText);
                        return;
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
                return false;
            });
        }

        private rejectSubstitution(subst: UserSubstitution): boolean {

            this.showLoaderBoxOnly(this.isError);

            var jsonEntityData = JSON.stringify({ substId: subst.id });

            this.$http.post(
                this.getRegetRootUrl() + "Participant/RejectSubstitution?t=" + new Date().getTime(),
                jsonEntityData
            ).then((response) => {
                try {
                    var result: any = response.data;
                    var strResult = result.string_value;
                    if (this.isStringValueNullOrEmpty(strResult)) {
                        subst.approval_status = ApprovalStatus.Rejected;
                        subst.approval_status_text = this.locRejectedText;
                        subst.is_editable_app_man = false;
                        subst.is_approve_hidden = true;
                        subst.is_reject_hidden = true;
                        subst.is_can_be_deleted = false;

                        return true;
                    } else {
                        let msgError = this.getErrorMessage(strResult);
                        this.showAlert(this.locErrorTitleText, msgError, this.locCloseText);
                        return false;
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
                return false;
            });

            return true;
        }
        //************************************************************************

        //************************************************************************
        //Methods
        private setGrid(): void {
            this.gridOptions.columnDefs = [
                {
                    name: "row_index",
                    field: "row_index",
                    displayName: '',
                    enableFiltering: false,
                    enableSorting: false,
                    enableCellEdit: false,
                    width: 40,
                    enableColumnResizing: true,
                    cellTemplate: '<div class="ui-grid-cell-contents ui-grid-top-panel" style="text-align:center;vertical-align:middle;font-weight:normal;">{{COL_FIELD}}</div>'
                },
                {
                    name: 'action_buttons_detail',
                    displayName: '',
                    enableFiltering: false,
                    enableSorting: false,
                    enableCellEdit: false,
                    enableHiding: false,
                    enableColumnResizing: false,
                    width: 35,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellDetail.html"

                },
                {
                    name: 'action_buttons_approve',
                    displayName: '',
                    enableFiltering: false,
                    enableSorting: false,
                    enableCellEdit: false,
                    enableHiding: false,
                    enableColumnResizing: false,
                    width: 70,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellApproveAction.html"

                },
                {
                    name: 'action_buttons',
                    displayName: '',
                    enableFiltering: false,
                    enableSorting: false,
                    enableCellEdit: false,
                    enableHiding: false,
                    enableColumnResizing: false,
                    width: 70,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellAction.html"

                },
                {
                    name: "substituted_name_surname_first", displayName: this.locSubstitutedText, field: "substituted_name_surname_first",
                    enableHiding: false,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextTemplateReadOnly.html",
                    enableCellEdit: false,
                    filter: {
                        condition: function (searchTerm, cellValue) {
                            return true;
                        }
                    },
                    width: 160,
                    minWidth: 100,
                    cellClass: (grid, row, col, rowRenderIndex, colRenderIndex) => {
                        return this.getSubsRowBkg(row.entity);
                    }
                },
                {
                    name: "substitutee_name_surname_first", displayName: this.locSubstituteeText, field: "substitutee_name_surname_first",
                    enableHiding: false,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextTemplateReadOnly.html",
                    enableCellEdit: false,
                    filter: {
                        condition: function (searchTerm, cellValue) {
                            return true;
                        }
                    },
                    width: 160,
                    minWidth: 100,
                    cellClass: (grid, row, col, rowRenderIndex, colRenderIndex) => {
                        return this.getSubsRowBkg(row.entity);
                    }
                },
                {
                    name: "substitute_start_date", displayName: this.locFromText, field: "substitute_start_date",
                    enableHiding: false,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellDateTimeMandatoryTemplate1.html",
                    enableCellEdit: false,
                    filterHeaderTemplate: "<md-datepicker id=\"grdFilterDateTimePickerFrom\""
                        + " ng-model=\"grid.appScope.fromDateFilter\""
                        + " ng-change=\"grid.appScope.filterFromDatePickerChange(col)\""
                        //+ " ng-blur=\"grid.appScope.dateTimeFromFilterChanged(1, col)\""
                        + " md-placeholder=\"{{grid.appScope.locEnterDateText}}\""
                        + " md-is-open=\"ctrl.isOpen\""
                        + " md-hide-icons=\"calendar\""
                        + " style=\"margin-left:8px;\">"
                        + "</md-datepicker>",
                    filter: {
                        condition: function (searchTerm, cellValue) {
                            return true;
                        }
                    },
                    sortingAlgorithm: function () { return 0; },
                    width: 160,
                    minWidth: 80,
                    cellClass: (grid, row, col, rowRenderIndex, colRenderIndex) => {
                        return this.getSubsRowBkg(row.entity);
                    }
                },
                {
                    name: "substitute_end_date", displayName: this.locToText, field: "substitute_end_date",
                    //type: 'date',
                    //cellFilter: 'date:"d.M.yyyy HH:mm"',
                    enableHiding: false,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellDateTimeMandatoryTemplate2.html",
                    filterHeaderTemplate: "<md-datepicker id=\"grdFilterDateTimePickerTo\""
                        + " ng-model=\"grid.appScope.toDateFilter\""
                        + " ng-change=\"grid.appScope.filterToDatePickerChange(col)\""
                        + " md-placeholder=\"{{grid.appScope.locEnterDateText}}\""
                        + " md-is-open=\"ctrl.isOpen\""
                        + " md-hide-icons=\"calendar\""
                        + " style=\"margin-left:8px;\">"
                        + " </md-datepicker>",
                    filter: {
                        condition: function (searchTerm, cellValue) {
                            return true;
                        }
                    },
                    sortingAlgorithm: function () { return 0; },
                    enableCellEdit: false,
                    width: 160,
                    minWidth: 80,
                    cellClass: (grid, row, col, rowRenderIndex, colRenderIndex) => {
                        return this.getSubsRowBkg(row.entity);
                    }
                },
                {
                    name: 'is_allow_take_over', displayName: this.locAllowSubsToTakeOverShortText, field: "is_allow_take_over",
                    enableCellEdit: false,
                    enableHiding: false,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellCheckboxTemplate.html",
                    filter: {
                        type: this.uiGridConstants.filter.SELECT,
                        selectOptions: this.yesNo
                    },
                    minWidth: 100,
                    width: 100,
                    cellClass: (grid, row, col, rowRenderIndex, colRenderIndex) => {
                        return this.getSubsRowBkg(row.entity);
                    }
                },
                {
                    name: "approval_status_text", displayName: this.locApprovalStatusText, field: "approval_status_text",
                    enableHiding: false,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextTemplateReadOnly.html",
                    enableCellEdit: false,
                    filter: {
                        type: this.uiGridConstants.filter.SELECT,
                        selectOptions: this.approvalStatus
                    },
                    width: 150,
                    minWidth: 75,
                    cellClass: (grid, row, col, rowRenderIndex, colRenderIndex) => {
                        return this.getSubsRowBkg(row.entity);
                    }
                },
                //{
                //    name: "approval_status_text", displayName: this.locApprovalStatusText, field: "approval_status_text",
                //    enableHiding: false,
                //    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellDropDownMandatoryTemplate.html",
                //    enableCellEdit: false,
                //    editDropdownOptionsArray: this.approvalStatus,
                //    filter: {
                //        type: this.uiGridConstants.filter.SELECT,
                //        selectOptions: this.approvalStatus

                //    },
                //    width: 150,
                //    minWidth: 75
                //},
                {
                    name: "approval_men", displayName: this.locApproveManText, field: "approval_men",
                    enableHiding: false,
                    enableFiltering: false,
                    enableSorting: false,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextTemplateReadOnly.html",
                    enableCellEdit: false,
                    filter: {
                        type: this.uiGridConstants.filter.SELECT,
                        selectOptions: this.approvalStatus
                    },
                    width: 150,
                    minWidth: 75,
                    cellClass: (grid, row, col, rowRenderIndex, colRenderIndex) => {
                        return this.getSubsRowBkg(row.entity);
                    }
                },
                {
                    name: "remark", displayName: this.locRemarkText, field: "remark",
                    enableHiding: true,
                    enableSorting: false,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextAreaReadOnlyTemplate.html",
                    enableCellEdit: false,
                    width: 160,
                    minWidth: 100,
                    cellClass: (grid, row, col, rowRenderIndex, colRenderIndex) => {
                        return this.getSubsRowBkg(row.entity);
                    }
                },
                {
                    name: "modified_user_name", displayName: this.locModifyUserText, field: "modified_user_name",
                    enableHiding: true,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextTemplateReadOnly.html",
                    enableCellEdit: false,
                    width: 160,
                    minWidth: 100,
                    cellClass: (grid, row, col, rowRenderIndex, colRenderIndex) => {
                        return this.getSubsRowBkg(row.entity);
                    }
                },
                {
                    name: "modified_date", displayName: this.locModifyDateText, field: "modified_date",
                    enableHiding: true,
                    enableFiltering: true,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellDateTimeReadOnlyTemplate.html",
                    filterHeaderTemplate: "<div><table><tr><td style=\"font-weight:normal;padding-bottom:10px;\">" + this.locFromText + ":</td><td style=\"padding-bottom:10px;\"><md-datepicker id=\"grdFilterDateTimePickerModifFrom\""
                        + " name=\"grdFilterDateTimePickerModifFrom\""
                        + " ng-model=\"grid.appScope.modifyDateFromFilter\""
                        + " ng-change=\"grid.appScope.filterModifyDateFromPickerChange(col)\""
                        + " md-placeholder=\"{{grid.appScope.locEnterDateText}}\""
                       // + " md-is-open=\"ctrl.isOpen\""
                        + " md-hide-icons=\"calendar\""
                        + " style=\"margin-left:8px;\">"
                        + " </md-datepicker></td></tr>"
                        + " <tr><td style=\"font-weight:normal;padding-bottom:10px;\">" + this.locToText + ":</td><td style=\"padding-bottom:10px;\"><md-datepicker id=\"grdFilterDateTimePickerModifTo\""
                        + " name=\"grdFilterDateTimePickerModifTo\""
                        + " ng-model=\"grid.appScope.modifyDateToFilter\""
                        + " ng-change=\"grid.appScope.filterModifyDateToPickerChange(col)\""
                        + " md-placeholder=\"{{grid.appScope.locEnterDateText}}\""
                        //+ " md-is-open=\"ctrl.isOpen\""
                        + " md-hide-icons=\"calendar\""
                        + " style=\"margin-left:8px;\">"
                        + " </md-datepicker></td></tr></table></div>",
                    filter: {
                        condition: function (searchTerm, cellValue) {
                            return true;
                        }
                    },
                    sortingAlgorithm: function () { return 0; },
                    enableCellEdit: false,
                    width: 160,
                    minWidth: 100,
                    cellClass: (grid, row, col, rowRenderIndex, colRenderIndex) => {
                        return this.getSubsRowBkg(row.entity);
                    }
                },
                {
                    name: 'active', displayName: this.locActiveText, field: "active",
                    enableCellEdit: false,
                    enableHiding: false,
                    cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellCheckboxTemplate.html",
                    filter: {
                        type: this.uiGridConstants.filter.SELECT,
                        selectOptions: this.yesNo
                    },
                    minWidth: 100,
                    width: 100,
                    cellClass: (grid, row, col, rowRenderIndex, colRenderIndex) => {
                        return this.getSubsRowBkg(row.entity);
                    }
                }
            ];

        }

        private isReadOnlySubstituted(): boolean {
            if ($("#IsSubstitutedReadOnly") === null || (this.isStringValueNullOrEmpty($("#IsSubstitutedReadOnly").val()))) {
                return false;
            }

            return true;
        }

        private getReadOnlySubstituted(): void {
            if (!this.isReadOnlySubstituted()) {
                return;
            }

            //let substitutedId: number = this.getIntegerFromInput("CurrentUserId");
            this.selectedsubstitutedUser = new Participant();
            this.selectedsubstitutedUser.id = this.currentUserId;
            this.selectedsubstitutedUser.name_surname_first = this.currentUserName;
            this.getSubstitutedApprovers(true);
        }

        public loadData(): void {
            try {
                this.dateFromMin = new Date();
                this.dateToMin = new Date();
                this.minFromValueErrMsg = this.locMinDateText; //this.locMinDateText.replace("{0}", moment().format(this.dateTimeMomentFormatText));
                this.minToValueErrMsg = this.locMinDateText; //this.locMinDateText.replace("{0}", moment().format(this.dateTimeMomentFormatText));

                let urlParams: UrlParam[] = this.getAllUrlParams();
                if (!this.isValueNullOrUndefined(urlParams)) {
                    for (var i: number = 0; i < urlParams.length; i++) {
                        if (urlParams[i].param_name.toLowerCase() === "newsubstdisplayed") {
                            if (urlParams[i].param_value === "0") {
                                $("#divAddSubstContent").slideUp();
                                $("#imgSubstExpand").attr("src", this.getRegetRootUrl() + "Content/Images/Panel/Expand.png");
                                $("#imgSubstExpand").attr("title", this.locDisplayText);
                            }
                            break;
                        } 
                    }
                }
                

                this.fromDate = new Date();
                if (this.isReadOnlySubstituted()) {
                    this.getReadOnlySubstituted();
                } else {
                    //this.loadGridData();
                    this.getLoadDataGridSettings();
                }
                
            } catch (ex) {
                this.displayErrorMsg();
            }
        }

        protected gridDeleteRowFromDb(entity: any, ev: MouseEvent): void {
            this.showLoaderBoxOnly(this.isError);

            let jsonData: any = JSON.stringify(entity);

            this.$http.post(
                this.deleteUrl,
                jsonData
            ).then((response) => {
                try {
                    let isWasDeleted: boolean = this.gridRowWasDeleted(response, entity);
                    if (isWasDeleted == true) {
                        this.removeEditCol();
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

            //return deferred.promise;
        }
        //**********************************************************        

        //**********************************************************
        //Extra Methods
        private displaySuccess(strText: string): void {
            this.$mdToast.show(
                this.$mdToast.simple()
                    .textContent(strText)
                    .position('bottom center')
                    .theme("success-toast"));
            this.displayErrorMsg();
        }

        private formatGridDate(gridData: any[]): void {
            if (this.isValueNullOrUndefined(gridData)) {
                return;
            }

            for (var i = 0; i < gridData.length; i++) {
                let userSubstitution: UserSubstitution = gridData[i] as UserSubstitution;
                let jsonDateFrom = gridData[i].substitute_start_date;
                let jDateFrom = this.convertJsonDate(jsonDateFrom);
                gridData[i].substitute_start_date = jDateFrom;

                let jsonDateTo = gridData[i].substitute_end_date;
                let jDateTo = this.convertJsonDate(jsonDateTo);
                gridData[i].substitute_end_date = jDateTo;

                let jsonModifyDate = gridData[i].modified_date;
                let jModifiedDate = this.convertJsonDate(jsonModifyDate);
                gridData[i].modified_date = jModifiedDate;

                //date = date ? moment(date).format($("#DateTimePickerFormatMoment").val()) : ''; 
                //gridData[i].substitute_start_date = this.getDateTimeString(gridData[i].substitute_start_date);
                //var m = moment.utc(moment(date).format('M/D/YYYY h:m A')).toDate();
            }
        }

        public gridEditRow(rowEntity: any): void {
            
            if (!rowEntity["is_editable_author"] || !this.isEditColVisible) {
                return;
            }

            super.gridEditRow(rowEntity);

            ////set colums access
            //let elDates: HTMLCollectionOf<HTMLDivElement> = <HTMLCollectionOf<HTMLDivElement>> (document.getElementsByClassName("div-date-edit"));
            //elDates[0].setAttribute("display", "none");
        }

        public filterFromDatePickerChange(col: any): void {
            //this.datePickerChange();

            //if (!moment(this.fromDateFilter).isValid()) {
            //    //this.displayErrorMsg(this.locDateFromatText);
            //    return;
            //}

            col.filters[0].term = moment(this.fromDateFilter).format(this.filterDateFormat);

            //this.loadGridData();
        }
                
        public filterToDatePickerChange(col: any): void {
            col.filters[0].term = moment(this.toDateFilter).format(this.filterDateFormat);
        }

        public filterModifyDateFromPickerChange(col: any): void {
            col.filters[0].term = this.urlFromFilterDelimiter + moment(this.modifyDateFromFilter).format(this.filterDateFormat);
        }

        public filterModifyDateToPickerChange(col: any): void {
            if (col.filters.length < 2) {
                let filterOption: uiGrid.IFilterOptions = { term: this.urlToFilterDelimiter + moment(this.modifyDateToFilter).format(this.filterDateFormat) };
                col.filters.push(filterOption);
            } else {
                col.filters[1].term = this.urlToFilterDelimiter + moment(this.modifyDateToFilter).format(this.filterDateFormat);
            }


            //workaround, I was not able to connect 2 fields to filter, if the second one date was modified the standard filter was run 
            //this causes no data are displayed even if there are retrieved
            /* this filter was not rise for second date - it causes the internal filter is run 
             * filter: {
                        condition: function (searchTerm, cellValue) {
                            return true;
                        }
                    }
             * */
            this.filterChanged();
            
            let gridApiAny: any = this.gridApi;

            
            //gridApiAny.core.raise.filterChanged(col);

            gridApiAny.grid.clearAllFilters();
        }

        public dateTimeFromFilterChanged(isTime: number, col: any): void {
            //try {
            //    if (!this.isValueNullOrUndefined(dateTimeWa)) {
            //        this.fromDateFilter = dateTimeWa;
            //        this.filterFromDatePickerChange(isTime, col);
            //    } else {

            //    }
            //} catch (e) {
            //    this.displayErrorMsg();
            //} finally {
            //    dateTimeWa = null;
            //}
        }

        public hideEditGridButton(rowEntity: any): boolean {
            return !rowEntity["is_editable_author"];
            //return true;
        }

        public hideDeleteGridButton(rowEntity: any): boolean {
            return !rowEntity["is_can_be_deleted"];
        }

        private searchParticipant(strName): ng.IPromise<Participant[]> {
            if (this.isSkipSubstitutedUserLoad || this.isSkipSubstituteeUserLoad) {
                if (this.isSkipSubstitutedUserLoad) {
                    this.isSkipSubstitutedUserLoad = false;
                } else if (this.isSkipSubstituteeUserLoad) {
                    this.isSkipSubstituteeUserLoad = false;
                }
                $("#agSubstitutedUser_errmandatory").slideUp("slow");
                $("#agSubstituteeUser_errmandatory").slideUp("slow");
                return null;
            }

            return this.filterParticipants(strName);

            ////var results = strName ? this.filterParticipants(strName) : this.substUsers, deferred;
            //var results = this.filterParticipants(strName), deferred;

            //var deferred : any = this.$q.defer<any>();
            //if (strName) {
            //    deferred.resolve(results);
            //} else {
            //    this.$timeout(() => { deferred.resolve(results); }, 3000, false);
            //}

            //return deferred.promise;

        }


        public substitutedItemChange(item: Participant, strErrorMsgId: string): void {
            this.selectedsubstitutedUser = item;
            this.checkUserMandatory(item, strErrorMsgId);
            if (!this.isValueNullOrUndefined(item)) {
                this.approvers = this.getSubstitutedApprovers(false);
            }
        }

        public substituteeItemChange(item: Participant, strErrorMsgId: string): void {
            this.selectedsubstituteeUser = item;
            this.checkUserMandatory(item, strErrorMsgId);

        }

        private substitutedQuerySearch(strName: string) : ng.IPromise<Participant[]> {
            //var results = null;

            //if (!this.isValueNullOrUndefined(query) && query.length === 1) {
            //    var deferred = this.$q.defer();
            //    this.populateSubstitutedSearch(query, query, deferred);
            //    return deferred.promise;
            //} else {
            //    if (this.isValueNullOrUndefined(this.substitutedMen)) {
            //        var deferred = this.$q.defer();
            //        this.populateSubstitutedSearch(query.substring(0, 2), query, deferred);
            //        return deferred.promise;
            //    } else {
            //        return this.filterSubstituted(query);
            //    }
            //}
            return this.filterSubstituted(strName);
        }

        public filterSubstituted(searchText: string): ng.IPromise<Participant[]> {

            var deferred: ng.IDeferred<Participant[]> = this.$q.defer<Participant[]>();

            this.$http.get(
                this.getRegetRootUrl() + '/Participant/GetSubstitutedMen?name=' + encodeURI(searchText)
                + '&t=' + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    var participants: Participant[] = tmpData;

                    //console.log(participants.length);
                    return deferred.resolve(participants);
                    //}
                } catch (e) {
                    this.hideLoader();
                    this.displayErrorMsg();
                } finally {
                    this.hideLoader();
                }
            }, (response: any) => {
                //deferred.resolve($scope.Suppliers);
                this.hideLoader();
                this.displayErrorMsg();
            });
            // }

            return deferred.promise;
        }


        //private filterSubstituted(query: string): Participant[] {
            
        //    if (this.isValueNullOrUndefined(query) || query.length < 1) {
        //        return null;
        //    }


        //    return this.searchParticipantsFromArray(query, this.substitutedMen);
        //}

        //private populateSubstitutedSearch(searchText: string, fullSearchText: string, deferred: any): void {
        //    if (this.isValueNullOrUndefined(deferred)) {
        //        return;
        //    }

        //    this.$http.get(
        //        this.getRegetRootUrl() + '/Participant/GetSubstitutedMen?searchText=' + searchText + '&t=' + new Date().getTime(),
        //        {}
        //    ).then((response) => {
        //        try {
        //            let substd: any = response.data;
        //            this.substitutedMen = substd;
        //            deferred.resolve(this.filterSubstituted(fullSearchText));
        //        } catch (e) {
        //            this.hideLoader();
        //            this.displayErrorMsg();
        //        } finally {
        //            //this.hideLoaderAfterAllIsLoaded();
        //        }
        //    }, (response: any) => {
        //        this.hideLoader();
        //        this.displayErrorMsg();
        //    });

        //}

        public substitutedUserOnBlur(strErrorMsgId: string): void {
            this.checkUserMandatory(this.selectedsubstitutedUser, strErrorMsgId);
        }

        public substituteeUserOnBlur(strErrorMsgId: string): void {
            this.checkUserMandatory(this.selectedsubstituteeUser, strErrorMsgId);
        }

        public isSubstitutionValid(): boolean {
            if (this.isNewSubsValid()) {
                return this.isNewSubstitutionValid();
            }
            
            return false;
        }

        private isNewSubsValid(): boolean {
            return this.isNewValid;
        }

        private isNewSubstitutionValid(): boolean {
            try {
                this.substErrMsg = null;

                if (this.isValueNullOrUndefined(this.selectedsubstitutedUser)
                    || this.isValueNullOrUndefined(this.selectedsubstitutedUser.id)) {
                    this.substErrMsg = this.locMissingSubstitutedText;
                    return false;
                }

                if (this.isValueNullOrUndefined(this.selectedsubstituteeUser)
                    || this.isValueNullOrUndefined(this.selectedsubstituteeUser.id)) {
                    this.substErrMsg = this.locMissingSubstituteeText;
                    return false;
                }

                if (this.selectedsubstitutedUser.id === this.selectedsubstituteeUser.id) {
                    this.substErrMsg = this.locSubstitutedText + " = " + this.locSubstituteeText;
                    return false;
                }
                                
                if (this.isValueNullOrUndefined(this.fromDate) || this.isDateFromValid !== true) {
                    this.substErrMsg = this.locMissingDateFromText;
                    return false;
                }

                if (this.isValueNullOrUndefined(this.toDate) || this.isDateToValid !== true) {
                    this.substErrMsg = this.locMissingDateToText;
                    return false;
                }

                if (this.fromDate > this.toDate) {
                    this.substErrMsg = this.locFromText + ' > ' + this.locToText;
                    return false;
                }

                //let dateNowMinusHour = new Date();
                //dateNowMinusHour.setHours(dateNowMinusHour.getHours() - 1);
                if (this.fromDate < moment(this.dateFromMin).add(-1, "minutes").toDate()) {
                    //this.fromDate.$valid = false;
                    this.substErrMsg = this.locSubstitutionPastText;
                    return false;
                }

                //if (this.toDate < new Date()) {
                //    this.substErrMsg = this.locSubstitutionPastText;
                //    return false;
                //}

                //if (!this.angScopeAny.frmUserSubstitution.$valid) {
                //    return false;
                //}

                return true;
            } catch {
                this.substErrMsg = this.locErrorMsgText;
                return false;
            }
        }

        private addSubstitution(): void {
            
            try {
                this.isNewValid = true;
                if (this.isSubstitutionValid()) {
                    let isAppMan: boolean = this.isApproveMan();
                    let strRemark = this.remark;
                    if (this.isStringValueNullOrEmpty(strRemark)) {
                        strRemark = "";
                    }

                    let rowIndex: number = ((this.currentPage - 1) * this.pageSize) + this.substData.length + 1
                    if (rowIndex < 1) {
                        rowIndex = 1;
                    }

                    var userSubstitutionEntity: UserSubstitution = new UserSubstitution(
                        -1,
                        this.selectedsubstitutedUser.id,
                        this.selectedsubstituteeUser.id,
                        this.fromDate,
                        this.toDate,
                        this.selectedsubstitutedUser.name_surname_first,
                        this.selectedsubstituteeUser.name_surname_first,
                        this.isTakeOver,
                        strRemark,
                        this.approval_status,
                        this.getApprovalStatusText(this.approval_status),
                        //this.getActiveStatus(userSubstitutionEntity),
                        this.approversText,
                        this.currentUserName,
                        rowIndex,
                        true,
                        isAppMan,
                        false,
                        false,
                        true,
                        new Date(),
                        true
                    );

                    userSubstitutionEntity.is_editable_author = true;

                    this.saveSubstitution(userSubstitutionEntity);
                    this.displayEditCol();
                } else {
                    var strMsg: string = this.locDataCannotBeSavedText;
                    if (!this.isStringValueNullOrEmpty(this.substErrMsg)) {
                        strMsg += "<br>" + this.substErrMsg;
                    }
                    this.displayErrorMsg(strMsg);
                }
            } catch {
                this.displayErrorMsg();
            } finally {
                this.isNewValid = false;
            }
        }

        //private getActiveStatus(subst: UserSubstitution): string {
        //    return "";
        //}

        private isApproveMan(): boolean {
            if (this.isValueNullOrUndefined(this.approvers) || this.approvers.length == 0) {
                return false;
            }

            var tmpApproves: Participant[] = this.$filter("filter")(this.approvers, { id: this.currentUserId }, true);
            if (!this.isValueNullOrUndefined(this.approvers) && this.approvers.length > 0) {
                return true;
            }

            return false;
        }

        private getApprovalStatusText(appStatus: ApprovalStatus): string {
            if (appStatus === ApprovalStatus.Empty) {
                return "";
            }
            if (appStatus === ApprovalStatus.Rejected) {
                return this.locRejectedText;
            }
            if (appStatus === ApprovalStatus.NotNeeded) {
                return this.locNotNeededText;
            }
            if (appStatus === ApprovalStatus.WaitForApproval) {
                return this.locWaitForApprovalText;
            }
            if (appStatus === ApprovalStatus.Approved) {
                return this.locApprovedText;
            }

            return "";
        }

        private clearSubstitution(): void {
            this.isSkipSubstitutedUserLoad = true;
            this.isSkipSubstituteeUserLoad = true;

            //this.isdatetoclear = true;
            this.dirToOptions.hideErrorMsg();
            //this.dirSubstHandOverFromOptions.hideErrorMsg();

            if (!this.isReadOnlySubstituted()) {
                this.selectedsubstitutedUser = null;
            }
            this.selectedsubstituteeUser = null;
            this.strSelectedsubstitutedUser = null;
            this.strSelectedsubstituteeUser = null;
            this.fromDate = new Date();
            this.toDate = null;
            this.remark = null;
            //this.approval_status = -1;
            this.isTakeOver = false;

            //var currHourFrom = RegetCommonTs.convertIntegerToString(new Date().getHours(), 2);
            //$("#txtHourdtFrom").val(currHourFrom);

            //var currMinFrom = RegetCommonTs.convertIntegerToString(new Date().getMinutes(), 2);
            //$("#txtMinutedtFrom").val(currMinFrom);


            //$("#txtHourdtTo").val("");
            //$("#txtMinutedtTo").val("");



            //clear errors
            $("#agSubstitutedUser_errmandatory").slideUp("slow");
            $("#agSubstituteeUser_errmandatory").slideUp("slow");

            //this.isdatetoclear = false;

            //this.clearDatesSubstitutionTimeout();
            //this.clearSubstitutionThread();

            this.dirFromOptions.setDirTime(new Date());
            this.dirToOptions.setDirTime(null);

            this.dateFromMin = new Date();
            //this.dirSubstHandOverFromOptions.setDirTime(null);
        }

        private clearSubstitutionThread(): void {

            //alert(this.$scope.frmUserSubstitution['dtpdtTo'].$error.required);
            //if (this.$scope.frmUserSubstitution['dtpdtTo'].$error.required == true) {
            //this.$scope.frmUserSubstitution['dtpdtFrom'].$error.required = false;
            //this.$scope.frmUserSubstitution['dtpdtFrom'].$setValidity("required", true);

            this.$scope.frmUserSubstitution['dtpdtTo'].$error.required = false;
            this.$scope.frmUserSubstitution['dtpdtTo'].$setValidity("required", true);

            //clearTimeout(this.dTimeout);
            //this.currencyForm.controls['currencyMaxSell'].setErrors

            //this.frmUserSubstitution.resetForm();
            //}
        }

        public displayGridCellTextArea(row : any): void {
            try {
                //this one is needed to eliminate problem with absolute position of the TextArea
                //from some reason the position=relative is set for html form element after a substitution is added
                let form = $("#frmUserSubstitution");
                form[0].style.removeProperty('position');
                
                let grdCellDivTextArea: HTMLDivElement = <HTMLDivElement>document.getElementById("grdCellDivTextArea");
                let grdCellTextArea: HTMLTextAreaElement = <HTMLTextAreaElement>document.getElementById("grdCellTextArea");
                                
                let btnEdit: HTMLInputElement = <HTMLInputElement>document.getElementById("btnTextAreaEdit");
                
                let divRow: HTMLDivElement = this.getRowElement(btnEdit);
                
                let colRemarkIndex: number = this.getColumnIndex('remark');
                let cells = divRow.getElementsByClassName("ui-grid-cell");
                let divCol: HTMLDivElement = <HTMLDivElement>cells[colRemarkIndex];
                
                let rowId: number = row.entity.id;
                let position: RowElementPosition = this.getPosition(rowId, true);

                let iLeft: number = position.xPos;
                let iTop: number = position.yPos;
                let iWidth: number = position.width;
                
                let editSubst: UserSubstitution = this.editRow;
                grdCellDivTextArea.style.top = iTop + "px";
                grdCellDivTextArea.style.left = iLeft + "px";
                grdCellDivTextArea.style.width = (divCol.clientWidth + 2).toString() + "px";

                //var newline = String.fromCharCode(13, 10);
                //return input.replaceAll('\\n', newline);

                grdCellTextArea.value = editSubst.remark;
                grdCellDivTextArea.style.display = "block";

                let divRegetToolTip: HTMLDivElement = <HTMLDivElement>document.getElementById("divRegetToolTip");
                divRegetToolTip.style.opacity = "0";
                divRegetToolTip.style.visibility = "hidden";

                grdCellTextArea.focus();
            } catch {
                this.displayErrorMsg();
            }
        }

        private getRowElement(childElement: HTMLElement): HTMLDivElement {
            if (this.isValueNullOrUndefined(childElement.parentElement)) {
                return null;
            }

            if (childElement.parentElement.className.indexOf("ui-grid-row") > -1) {
                return childElement.parentElement as HTMLDivElement;
            }

            return this.getRowElement(childElement.parentElement);
        }

        public hideGridCellTextArea(): void {
            //return;
            let grdCellTextArea: HTMLTextAreaElement = <HTMLTextAreaElement>document.getElementById("grdCellTextArea");

            let editSubst: UserSubstitution = this.editRow;
            editSubst.remark = grdCellTextArea.value;
            $("#grdCellDivTextArea").hide();
        }

        public cellMouseOver(row: any): void {

            if (this.isStringValueNullOrEmpty(row.entity.remark)) {
                return;
            }
                        
            let rowId: number = row.entity.id;
            let position: RowElementPosition = this.getPosition(rowId, false);
            let iLeft: number = position.xPos;
            let iTop: number = position.yPos;
            let iWidth: number = position.width;
            let iRowIndex: number = position.iRowIndex;

            if (this.isTooltipScrollDisplayed === true && (this.tooltipRowIndex + 1 === iRowIndex)) {
                return;
            }

            var spanRegetTooltipText: HTMLSpanElement = <HTMLSpanElement>document.getElementById("spanRegetTooltipText");
            spanRegetTooltipText.innerHTML = row.entity.remark.split("\n").join("<br>");
            
            var divRegetToolTip: HTMLDivElement = <HTMLDivElement>document.getElementById("divRegetToolTip");
            divRegetToolTip.style.visibility = "visible";
            divRegetToolTip.style.opacity = "1";
            divRegetToolTip.style.top = iTop + "px";
                        
            spanRegetTooltipText.style.minWidth = iWidth + "px";
            spanRegetTooltipText.style.width = iWidth + "px";

            //divRegetToolTip.position = abso
            divRegetToolTip.style.width = iWidth + "px";
            divRegetToolTip.style.left = iLeft + "px";

            var hasVerticalScrollbar = (spanRegetTooltipText.scrollHeight > spanRegetTooltipText.clientHeight);
            if (hasVerticalScrollbar) {
                this.isTooltipScrollDisplayed = true;

            } else {
                this.isTooltipScrollDisplayed = false;
            }

            //alert("Left " + iLeft + ", Top " + iTop);
        }

        public cellMouseLeave(row?: any, col?: any): void {
            //if (this.isTooltipScrollLeave) {
            //    this.isTooltipScrollLeave = false;
            //    this.isTooltipOver = false;
            //}

            //if (this.isTooltipOver === true) {
            //    return;
            //}
            var spanRegetTooltipText: HTMLSpanElement = <HTMLSpanElement>document.getElementById("spanRegetTooltipText");
            var hasVerticalScrollbar = spanRegetTooltipText.scrollHeight > spanRegetTooltipText.clientHeight;
            if (hasVerticalScrollbar) {
                return;

            }

            var divRegetToolTip: HTMLDivElement = <HTMLDivElement>document.getElementById("divRegetToolTip");
            divRegetToolTip.style.opacity = "0";
            divRegetToolTip.style.visibility = "hidden";
        }

        //public tooltipOver(): void {
        //    //this.isTooltipOver = true;
        //}

        public tooltipLeave(): void {
            var spanRegetTooltipText: HTMLSpanElement = <HTMLSpanElement>document.getElementById("spanRegetTooltipText");
            var hasVerticalScrollbar = spanRegetTooltipText.scrollHeight > spanRegetTooltipText.clientHeight;
            if (hasVerticalScrollbar) {
                //this.isTooltipOver = false;
                var divRegetToolTip: HTMLDivElement = <HTMLDivElement>document.getElementById("divRegetToolTip");
                divRegetToolTip.style.opacity = "0";
                divRegetToolTip.style.visibility = "hidden";
            }
            this.isTooltipScrollDisplayed = false;
        }

        private getPosition(rowId: number, isEdit: boolean): RowElementPosition {
            //var tmp: any = document.getElementById("txtUserSubstituted");
            //tmp.setCustomValidity("This email is already registered!");

            var iRowIndex: number = -1;
            for (var i = 0; i < this.substData.length; i++) {
                if (this.substData[i].id == rowId) {
                    iRowIndex = i;
                    break;
                }
            }
            
            if (iRowIndex < 0) {
                return;
            }

            let table: any = document.getElementById("grdUserSubstitution");
            let rows = table.getElementsByClassName("ui-grid-row");
            let divRow: HTMLDivElement = <HTMLDivElement>rows[iRowIndex];

            let iScrollY: number = $(".reget-body").scrollTop() + window.pageYOffset;//window.scrollY;

            let iTop: number = iScrollY + Math.floor(divRow.getBoundingClientRect().top);
            if (!isEdit) {
                iTop += 5;
            }
            
            let colRemarkIndex: number = this.getColumnIndex('remark');
            let cells = divRow.getElementsByClassName("ui-grid-cell");
            let divCol: HTMLDivElement = <HTMLDivElement>cells[colRemarkIndex];
            let iScrollX: number = $(".reget-body").scrollLeft();

            var iLeft: number = iScrollX + Math.floor(divCol.getBoundingClientRect().left);
            iTop += divCol.clientHeight;

            var iWidth: number = divCol.clientWidth + 80;
            if (iWidth < 200) {
                iWidth = 200;
            }

            if (!isEdit) {
                let leftFix: number = Math.floor((iWidth - divCol.clientWidth) / 2);
                iLeft -= leftFix;
            }

            //iTop = 20;
            
            let pos: RowElementPosition = new RowElementPosition(iLeft, iTop, iWidth, iRowIndex);
            
            return pos;
        }

        private toggleTakeOver(): void {
            if (this.isTakeOver === true) {
                this.isTakeOver = false;
            } else {
                this.isTakeOver = true;
            }

            //this.isTakeOver = !this.isTakeOver;
        }

        private setEditApproveColButtons(): void {
            if (this.isApproveColVisible === true) {
                let isFound: boolean = false;
                for (var j: number = this.gridOptions.columnDefs.length - 1; j >= 0; j--) {
                    if (this.gridOptions.columnDefs[j].name === "action_buttons_approve") {
                        isFound = true;
                        break;
                    }
                }
                if (isFound === false) {
                    //Add Column
                    this.insertDataGridColum(this.approveColumn, this.iApproveColIndex);
                }
            } else {
                for (var j: number = this.gridOptions.columnDefs.length - 1; j >= 0; j--) {
                    if (this.gridOptions.columnDefs[j].name === "action_buttons_approve") {
                        this.iApproveColIndex = j;
                        this.approveColumn = angular.copy(this.gridOptions.columnDefs[j]);
                        this.gridOptions.columnDefs.splice(j, 1);
                        break;
                    }
                }
            }


            if (this.isEditColVisible === true) {
                let isFound: boolean = false;
                for (var j: number = this.gridOptions.columnDefs.length - 1; j >= 0; j--) {
                    if (this.gridOptions.columnDefs[j].name === "action_buttons") {
                        isFound = true;
                        break;
                    }
                }
                if (isFound === false) {
                    //Add Column
                    this.insertDataGridColum(this.editColumn, this.iEditColIndex);
                }
            } else {
                for (var j: number = this.gridOptions.columnDefs.length - 1; j >= 0; j--) {
                    if (this.gridOptions.columnDefs[j].name === "action_buttons") {
                        //this.actionCol = this.gridOptions.columnDefs[j];
                        this.iEditColIndex = j;
                        this.editColumn = angular.copy(this.gridOptions.columnDefs[j]);
                        this.gridOptions.columnDefs.splice(j, 1);
                        break;
                    }
                }
            }

           
        }

        private displayEditCol(): void {
            let isFound : boolean = false;
            for (var i: number = 0; i < this.gridOptions.columnDefs.length; i++) {
                if (this.gridOptions.columnDefs[i].name === "action_buttons") {
                    isFound = true;
                    break;
                }
            }

            if (isFound === false) {
                this.insertDataGridColum(this.editColumn, this.iEditColIndex);
                //let gridCols: uiGrid.IColumnDefOf<any>[] = [];
                
                //for (var i: number = 0; i < this.gridOptions.columnDefs.length; i++) {
                //    if (i === 1) {
                //        gridCols.push(this.actionCol);
                //    }
                //    gridCols.push(this.gridOptions.columnDefs[i]);
                //}

                //this.gridOptions.columnDefs = gridCols;

                this.isEditColVisible = true;
            }
        }

        private removeEditCol() {
            let isEditRemove: boolean = true;
            let isFound: boolean = true;
            let tmpEditColIndex: number = -1;

            for (var i: number = 0; i < this.gridOptions.columnDefs.length; i++) {
                if (this.gridOptions.columnDefs[i].name === "action_buttons") {
                    tmpEditColIndex = i;
                    isFound = true;
                    break;
                }
            }

            if (isFound === true) {
                for (let i: number = 0; i < this.substData.length; i++) {
                    if (this.substData[i].is_editable_app_man
                        || this.substData[i].is_editable_author
                        || this.substData[i].is_can_be_deleted) {
                        isEditRemove = false;
                        break;
                    }
                }
            }

            if (isEditRemove === true) {
                this.iEditColIndex = tmpEditColIndex;
                this.editColumn = this.gridOptions.columnDefs[tmpEditColIndex];
                this.gridOptions.columnDefs.splice(tmpEditColIndex, 1);
            }
        }

        private gridApproveRow(entity: any, ev: MouseEvent): void {
            let subst: UserSubstitution = entity;
            this.approveDialog(subst);
            //let subst: UserSubstitution = entity;
            //let strReplMsg: string = subst.substituted_name_surname_first + " -> " + subst.substitutee_name_surname_first
            //    + " " + this.getDateTimeString(subst.substitute_start_date) + " - " + this.getDateTimeString(subst.substitute_end_date);
            //let strMsg: string = this.locApproveConfirmText.replace("{0}", strReplMsg);

            //var confirm = this.$mdDialog.confirm()
            //    .title(this.locConfirmationText)
            //    .textContent(strMsg)
            //    .ariaLabel("ApproveRowConfirm")
            //    .targetEvent(ev)
            //    .ok(this.locNoText)
            //    .cancel(this.locYesText);

            //this.$mdDialog.show(confirm).then(() => {

            //}, () => {
            //        let isApproved: boolean = this.approveSubstitution(subst.id);
            //        if (isApproved === true) {
            //            subst.approval_status = ApprovalStatus.Approved;
            //            subst.approval_status_text = this.locApprovedText;
            //            subst.is_editable_app_man = false;
            //            subst.is_approve_hidden = true;
            //            subst.is_reject_hidden = true;
            //            subst.is_can_be_deleted = false;
            //        }
            //});
        }

        private approveDialog(subst: UserSubstitution): void {
             let strReplMsg: string = subst.substituted_name_surname_first + " -> " + subst.substitutee_name_surname_first
                + " " + this.getDateTimeString(subst.substitute_start_date) + " - " + this.getDateTimeString(subst.substitute_end_date);
            let strMsg: string = this.locApproveConfirmText.replace("{0}", strReplMsg);

            this.$mdDialog.show(
                {
                    template: this.getConfirmDialogTemplate(
                        strMsg,
                        this.locConfirmationText,
                        this.locYesText,
                        this.locNoText,
                        "confirmDialog()",
                        "closeDialog()"),
                    locals: {
                        userSubstControl: this,
                        subst: subst
                    },
                    controller: this.dialogApproveConfirmController
                });

        }

        private dialogApproveConfirmController($scope, $mdDialog, userSubstControl, subst): void {

            $scope.closeDialog = function () {
                $mdDialog.hide();
            }

            $scope.confirmDialog = () => {
                $mdDialog.hide();
                userSubstControl.approveSubstitution(subst);
            }
        }

        private gridRejectRow(entity: any, ev: MouseEvent): void {
            let subst: UserSubstitution = entity;
            this.rejectDialog(subst);

            //let subst: UserSubstitution = entity;
            //let strReplMsg: string = subst.substituted_name_surname_first + " -> " + subst.substitutee_name_surname_first
            //    + " " + this.getDateTimeString(subst.substitute_start_date) + " - " + this.getDateTimeString(subst.substitute_end_date);
            //let strMsg: string = this.locRejectConfirmText.replace("{0}", strReplMsg);

            //var confirm = this.$mdDialog.confirm()
            //    .title(this.locConfirmationText)
            //    .textContent(strMsg)
            //    .ariaLabel("RejectRowConfirm")
            //    .targetEvent(ev)
            //    .ok(this.locNoText)
            //    .cancel(this.locYesText);

            //this.$mdDialog.show(confirm).then(() => {

            //}, () => {
            //        let isRejected: boolean = this.rejectSubstitution(subst.id);
            //        if (isRejected === true) {
            //            subst.approval_status = ApprovalStatus.Rejected;
            //            subst.approval_status_text = this.locRejectedText;
            //            subst.is_editable_app_man = false;
            //            subst.is_approve_hidden = true;
            //            subst.is_reject_hidden = true;
            //            subst.is_can_be_deleted = false;
                        
            //        }
            //});
        }

        private rejectDialog(subst: UserSubstitution): void {
            let strReplMsg: string = subst.substituted_name_surname_first + " -> " + subst.substitutee_name_surname_first
                + " " + this.getDateTimeString(subst.substitute_start_date) + " - " + this.getDateTimeString(subst.substitute_end_date);
            let strMsg: string = this.locRejectConfirmText.replace("{0}", strReplMsg);

            this.$mdDialog.show(
                {
                    template: this.getConfirmDialogTemplate(
                        strMsg,
                        this.locConfirmationText,
                        this.locYesText,
                        this.locNoText,
                        "confirmDialog()",
                        "closeDialog()"),
                    locals: {
                        userSubstControl: this,
                        subst: subst
                    },
                    controller: this.dialogRejectConfirmController
                });

        }

        private dialogRejectConfirmController($scope, $mdDialog, userSubstControl, subst): void {

            $scope.closeDialog = function () {
                $mdDialog.hide();
            }

            $scope.confirmDialog = () => {
                $mdDialog.hide();
                userSubstControl.rejectSubstitution(subst);
            }
        }

        protected getSubsRowBkg(substitution: UserSubstitution): string {
            if (this.isValueNullOrUndefined(substitution)) {
                return null;
            }

            if (substitution.approval_status == ApprovalStatus.Rejected) {
                return 'grid-row-rejected';
            } else if (substitution.active === false) {
                return 'grid-row-disabled';
            }

            return null;
        }

        private gridRowDetail(rowEntity: UserSubstitution): void {
            //let divNewSubst: HTMLDivElement = <HTMLDivElement>document.getElementById("divAddSubstContent");
            //let vis = divNewSubst.style.visibility;

            let urlParAddSubstVisible: string = "newSubstDisplayed=0";
            if ($("#divAddSubstContent").is(':visible')) {
                urlParAddSubstVisible = "newSubstDisplayed=1";
            } 
       
            let url: string = this.getRegetRootUrl() + "Participant/UserSubstitutionDetail?id=" + rowEntity.id
                + "&filter=" + encodeURI(this.filterUrl)
                + "&sort=" + encodeURI(this.sortColumnsUrl)
                + "&pageSize=" + this.pageSize
                + "&currPage=" + this.currentPage
                + "&" + urlParAddSubstVisible
                + "&t=" + new Date().getTime();

            window.location.href = url;
        }

        public dateTimeToChanged(controlIndex: string, modifiedDate: Date): void {
            if (controlIndex === "0" ) {
                this.dateToMin = modifiedDate;
                //this.minToValueErrMsg = this.locMinDateText.replace("{0}", moment(modifiedDate).format(this.dateTimeMomentFormatText));
            }
        }

        //********** Because of Tests *****************************
        public setIsDateFromValid(bValue : boolean) : void {
            this.isDateFromValid = bValue;
        }

        public setIsDateToValid(bValue: boolean): void {
            this.isDateToValid = bValue;
        }
        //*********************************************************

        //private getSubstRowDisabledBkg(subt: UserSubstitution): string {
        //    return super.getRowDisabledBkg(subt.active);
        //}


        //private clearDatesSubstitutionTimeout() {
        //    this.dTimeout = setTimeout(() => {
        //        //if (this.$scope.frmUserSubstitution['dtpdtTo'].$error.required == true) {
        //        //    this.clearSubstitutionThread();

        //        //}

        //        //

        //        //this.clearSubstitutionThread();
        //        //this.clearDatesSubstitutionTimeout();


        //    }, 3000);
        //} 

        //setTimeout(() => {
        //    this.router.navigate(['/']);
        //},
        //    5000);

        //private delay(ms: number) {
        //    return new Promise(resolve => setTimeout(resolve, ms));
        //}

        //private async sleepExample() {
        //    console.log("Beforep: " + new Date().toString());
        //    // Sleep thread for 3 seconds
        //    await this.delay(3000);
        //    //console.log("Afterp:  " + new Date().toString());
        //    this.clearSubstitutionThread();
        //}

        //public datePickerChange(isTime: number): void {
        //    isDateTimePicker = (isTime == 1);

        //    this.toDate = new Date();
        //}
        //**********************************************************
    }

    angular.
        module('RegetApp').
        controller('UserSubstitutionController', Kamsyk.RegetApp.UserSubstitutionController).
        config(function ($mdDateLocaleProvider) { //only because of Date TIME picker is implemented
            this.SetDatePicker($mdDateLocaleProvider);
            //it is neccessary to set IsGenerateDatePickerLocalization = true
        });
    //angular.module('RegetApp').directive("timedirective", [() => new timedirective()]);
    //angular.module('RegetApp').directive("timedirective", timedirective.instance);
    //angular.module('RegetApp').directive("timedirective", timedirective.factory);
}

class RowElementPosition {
    public xPos: number = null;
    public yPos: number = null;
    public width: number = null;
    public iRowIndex: number = null;

    constructor(xPos: number, yPos: number, width: number, iRowIndex: number) {
        this.xPos = xPos;
        this.yPos = yPos;
        this.width = width;
        this.iRowIndex = iRowIndex;
    }
}


