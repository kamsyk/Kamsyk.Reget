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
        var UserSubstitutionController = /** @class */ (function (_super) {
            __extends(UserSubstitutionController, _super);
            //************************************************************
            //Other Texts
            //private dateTimeMomentFormatText = $("#DateTimeMomentFormatText").val();
            //************************************************************
            //**********************************************************
            //Constructor
            function UserSubstitutionController($scope, $http, $filter, $mdDialog, $mdToast, uiGridConstants, $q, $timeout
            //protected $moment: moment.Moment
            ) {
                var _this = _super.call(this, $scope, $http, $filter, $mdDialog, $mdToast, $q, uiGridConstants, $timeout) || this;
                _this.$scope = $scope;
                _this.$http = $http;
                _this.$filter = $filter;
                _this.$mdDialog = $mdDialog;
                _this.$mdToast = $mdToast;
                _this.uiGridConstants = uiGridConstants;
                _this.$q = $q;
                _this.$timeout = $timeout;
                //****************************************
                //Abstract properties
                _this.dbGridId = "grdUserSubstitution_rg";
                //*****************************************
                _this.fromDate = null;
                _this.toDate = null;
                _this.isSubstLoaed = false;
                _this.isSubstUsersLoaed = false;
                _this.approval_status = -1;
                _this.substData = null;
                _this.isSkipSubstitutedUserLoad = false;
                _this.isSkipSubstituteeUserLoad = false;
                _this.isdatetoclear = false;
                _this.dirFromOptions = {}; //must be here
                _this.dirToOptions = {}; //must be here
                //private dirSubstHandOverFromOptions: any = {};
                _this.isTooltipScrollDisplayed = false;
                _this.tooltipRowIndex = -1;
                //private leftAddFix: number = 0;
                //private topAddFix: number = 0;
                _this.approversText = null;
                _this.approvers = null;
                //private currentUserId: number = $("#CurrentUserId").val();
                //private currentUserName: string = $("#CurrentUserName").val();
                _this.isNewValid = false;
                _this.isReadOnly = false;
                _this.isTakeOver = false;
                _this.isApproveColVisible = false;
                _this.isEditColVisible = false;
                _this.errMsg = null;
                _this.substitutedMen = null;
                //private actionCol: any = null;
                _this.iEditColIndex = -1;
                _this.editColumn = null;
                _this.iApproveColIndex = -1;
                _this.approveColumn = null;
                _this.isNewSustCollapsed = false;
                _this.dateFromMin = new Date();
                _this.dateToMin = null;
                _this.minFromValueErrMsg = null;
                _this.minToValueErrMsg = null;
                _this.isDateFromValid = false;
                _this.isDateToValid = false;
                //private yesNo: Array<uiGrid.ISelectOption> = [{ value: "true", label: this.locYesText }, { value: "false", label: this.locNoText }];
                _this.yesNo = [{ value: true, label: _this.locYesText }, { value: false, label: _this.locNoText }];
                //*********************************************************
                //Localized Texts
                _this.locSubstitutedText = $("#SubstitutedText").val();
                _this.locSubstituteeText = $("#SubstituteeText").val();
                _this.locFromText = $("#FromText").val();
                _this.locToText = $("#ToText").val();
                _this.locDateFromatText = $("#DateFromatText").val();
                _this.locPendingRequestsText = $("#PendigRequestsText").val();
                _this.locMissingSubstitutedText = $("#MissingSubstitutedText").val();
                _this.locMissingSubstituteeText = $("#MissingSubstituteeText").val();
                _this.locMissingDateFromText = $("#MissingDateFromText").val();
                _this.locMissingDateToText = $("#MissingDateToText").val();
                _this.locSubstitutionPastText = $("#SubstitutionPastText").val();
                _this.locRemarkText = $("#RemarkText").val();
                _this.locNotNeededText = $("#NotNeededText").val();
                _this.locApprovedText = $("#ApprovedText").val();
                _this.locRejectedText = $("#RejectedText").val();
                _this.locWaitForApprovalText = $("#WaitForApprovalText").val();
                _this.locDeleteSupplierConfirmText = $("#DeleteSupplierConfirmText").val();
                _this.locApprovalStatusText = $("#ApprovalStatusText").val();
                _this.locModifyDateText = $("#ModifyDateText").val();
                _this.locModifyUserText = $("#ModifyUserText").val();
                _this.locApproveText = $("#ApproveText").val();
                _this.locRejectText = $("#RejectText").val();
                _this.locAllowSubsToTakeOverShortText = $("#AllowSubsToTakeOverShortText").val();
                _this.locLoadDataErrorText = angular.element("#LoadDataErrorText").val();
                _this.locApproveManText = angular.element("#ApproveManText").val();
                _this.locApproveConfirmText = angular.element("#ApproveConfirmText").val();
                _this.locRejectConfirmText = angular.element("#RejectConfirmText").val();
                _this.locActiveText = angular.element("#ActiveText").val();
                _this.locDetailText = angular.element("#DetailText").val();
                _this.locMinDateText = angular.element("#MinDateText").val();
                //private locDeleteText: string = angular.element("#DeleteText").val();
                //ovevritten
                _this.locDeleteText = angular.element("#DeactivateText").val();
                //*****************************************************
                _this.approvalStatus = [
                    { value: _this.locNotNeededText, label: _this.locNotNeededText },
                    { value: _this.locWaitForApprovalText, label: _this.locWaitForApprovalText },
                    { value: _this.locApprovedText, label: _this.locApprovedText },
                    { value: _this.locRejectedText, label: _this.locRejectedText }
                ];
                //***************************************************************
                _this.$onInit = function () { };
                _this.deleteUrl = _this.getRegetRootUrl() + 'Participant/DeactivateUserSubstitution' + '?t=' + new Date().getTime();
                _this.setGrid();
                _this.loadData();
                return _this;
                //Do Not Delete it
                //this.angScopeAny.$on("myCustomEvent", function (event, data) {
                //    console.log(data); // 'Data to send'
                //    alert('sss');
                //});
            }
            //********************************************************************
            //Abstract Methods
            UserSubstitutionController.prototype.isRowChanged = function () {
                var isChanged = false;
                var origSubst = this.editRowOrig;
                var editSubst = this.editRow;
                if (origSubst.substituted_user_id !== editSubst.substituted_user_id) {
                    isChanged = true;
                }
                else if (origSubst.substitute_user_id !== editSubst.substitute_user_id) {
                    isChanged = true;
                }
                else if (!this.isMomentDatesSame(origSubst.substitute_start_date, editSubst.substitute_start_date)) {
                    isChanged = true;
                }
                else if (!this.isMomentDatesSame(origSubst.substitute_end_date, editSubst.substitute_end_date)) {
                    isChanged = true;
                }
                else if (origSubst.approval_status !== editSubst.approval_status) {
                    isChanged = true;
                }
                else if (origSubst.remark !== editSubst.remark) {
                    isChanged = true;
                }
                else if (origSubst.is_allow_take_over !== editSubst.is_allow_take_over) {
                    isChanged = true;
                }
                return isChanged;
            };
            UserSubstitutionController.prototype.insertRow = function () { };
            UserSubstitutionController.prototype.exportToXlsUrl = function () {
                return this.getRegetRootUrl() + 'Report/GetUserSubstitutionReport?filter=' + encodeURI(this.filterUrl) +
                    '&sort=' + this.sortColumnsUrl +
                    '&t=' + new Date().getTime();
            };
            UserSubstitutionController.prototype.isRowEntityValid = function (rowEntity) {
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
                var isFromReadOnly = (rowEntity["is_date_time_ro1"]);
                if (!isFromReadOnly) {
                    var dateNowMinusHour = new Date();
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
            };
            UserSubstitutionController.prototype.getSaveRowUrl = function () {
                return this.getRegetRootUrl() + 'Participant/SaveUserSubstitution?t=' + new Date().getTime();
            };
            UserSubstitutionController.prototype.getDuplicityErrMsg = function (rowEntity) {
                return null;
            };
            UserSubstitutionController.prototype.getControlColumnsCount = function () {
                return 3;
            };
            UserSubstitutionController.prototype.getMsgDisabled = function (userSubstitutionEntity) {
                return null; //this.locSupplierWasDisabledText.replace("{0}", supplier.supp_name);
            };
            UserSubstitutionController.prototype.getMsgDeleteConfirm = function (userSubstitutionEntity) {
                return this.locDeleteSupplierConfirmText.replace("{0}", userSubstitutionEntity.substituted_name_surname_first);
            };
            UserSubstitutionController.prototype.loadGridData = function () {
                this.getSubstitutionData();
            };
            UserSubstitutionController.prototype.getErrorMsgByErrId = function (errId, msg) {
                return this.locErrorMsgText;
            };
            UserSubstitutionController.prototype.getDbGridId = function () {
                return this.dbGridId;
            };
            //public insertEntity(rowIndex: number): void { }
            //************************************************************************
            //************************* Overwritten Methods *****************************
            UserSubstitutionController.prototype.cellClicked = function (row, col) {
                if (!this.isValueNullOrUndefined(row)) {
                    var userSubst = row.entity;
                    if (userSubst.is_edit_hidden == true) {
                        return;
                    }
                }
                _super.prototype.cellClicked.call(this, row, col);
            };
            //**************************************************************************
            //************************************************************************
            //Http
            UserSubstitutionController.prototype.getSubstitutionData = function () {
                var _this = this;
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
                this.$http.get(this.getRegetRootUrl() + "Participant/GetUserSubstitution?" +
                    "filter=" + encodeURI(this.filterUrl) +
                    "&pageSize=" + this.pageSize +
                    "&currentPage=" + this.currentPage +
                    "&sort=" + this.sortColumnsUrl +
                    "&t=" + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpRes = response;
                        _this.formatGridDate(tmpRes.data.user_substitution.db_data);
                        _this.substData = tmpRes.data.user_substitution.db_data;
                        _this.gridOptions.data = _this.substData;
                        _this.rowsCount = tmpRes.data.user_substitution.rows_count;
                        _this.isApproveColVisible = tmpRes.data.is_approve_visible;
                        _this.isEditColVisible = tmpRes.data.is_edit_visible;
                        if (_this.rowsCount == 0) {
                            _this.currentPage = 0;
                        }
                        if (!_this.isValueNullOrUndefined(_this.gridApi)) {
                            _this.gridApi.core.notifyDataChange(_this.uiGridConstants.dataChange.COLUMN);
                        }
                        _this.setGridSettingData();
                        _this.setEditApproveColButtons();
                        //********************************************************************
                        //it is very important otherwise 50 lines are not diplayed properly !!!
                        _this.gridOptions.virtualizationThreshold = _this.rowsCount + 1;
                        //********************************************************************
                        _this.isSubstLoaed = true;
                        _this.testLoadDataCount++;
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
                //return substData;
            };
            UserSubstitutionController.prototype.saveSubstitution = function (subst) {
                var _this = this;
                this.showLoaderBoxOnly(this.isError);
                var jsonEntityData = JSON.stringify(subst);
                this.$http.post(this.getRegetRootUrl() + "Participant/SaveUserSubstitution?t=" + new Date().getTime(), jsonEntityData).then(function (response) {
                    try {
                        var result = response.data;
                        var iId = result.int_value;
                        if (iId === -1) {
                            _this.hideLoader();
                            _this.displayErrorMsg();
                            return;
                        }
                        var isNew = false;
                        if (subst.id < 0) {
                            isNew = true;
                            subst.id = iId;
                            _this.substData.push(subst);
                            _this.gridOptions.paginationPageSize++;
                        }
                        _this.clearSubstitution();
                        //this.leftAddFix = -110;
                        //this.topAddFix = -75;
                        _this.showToast(_this.locDataWasSavedText);
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
                    return false;
                });
            };
            UserSubstitutionController.prototype.getSubstitutedApprovers = function (isLoadSubstData) {
                var _this = this;
                var approvers = null;
                this.showLoaderBoxOnly(this.isError);
                this.$http.get(this.getRegetRootUrl() + "Participant/GetSubsitutedApprovers?substitutedId=" + this.selectedsubstitutedUser.id + "&t=" + new Date().getTime()).then(function (response) {
                    try {
                        var result = response.data;
                        approvers = result;
                        //this.approvers = result;
                        if (_this.isValueNullOrUndefined(result) || result.length == 0) {
                            _this.approversText = _this.locNotNeededText;
                            _this.approval_status = RegetApp.ApprovalStatus.NotNeeded;
                        }
                        else {
                            _this.approversText = "";
                            for (var i = 0; i < result.length; i++) {
                                if (_this.approversText.length > 0) {
                                    _this.approversText += ", ";
                                }
                                _this.approversText += result[i].surname + " " + result[i].first_name;
                            }
                            _this.approval_status = RegetApp.ApprovalStatus.WaitForApproval;
                        }
                        if (isLoadSubstData) {
                            //this.loadGridData();
                            _this.getLoadDataGridSettings();
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
                    return false;
                });
                return approvers;
            };
            UserSubstitutionController.prototype.approveSubstitution = function (subst) {
                var _this = this;
                this.showLoaderBoxOnly(this.isError);
                var jsonEntityData = JSON.stringify({ substId: subst.id });
                this.$http.post(this.getRegetRootUrl() + "Participant/ApproveSubstitution?t=" + new Date().getTime(), jsonEntityData).then(function (response) {
                    try {
                        var result = response.data;
                        var strResult = result.string_value;
                        if (_this.isStringValueNullOrEmpty(strResult)) {
                            subst.approval_status = RegetApp.ApprovalStatus.Approved;
                            subst.approval_status_text = _this.locApprovedText;
                            subst.is_editable_app_man = false;
                            subst.is_approve_hidden = true;
                            subst.is_reject_hidden = true;
                            subst.is_can_be_deleted = false;
                            return;
                        }
                        else {
                            var msgError = _this.getErrorMessage(strResult);
                            _this.showAlert(_this.locErrorTitleText, msgError, _this.locCloseText);
                            return;
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
                    return false;
                });
            };
            UserSubstitutionController.prototype.rejectSubstitution = function (subst) {
                var _this = this;
                this.showLoaderBoxOnly(this.isError);
                var jsonEntityData = JSON.stringify({ substId: subst.id });
                this.$http.post(this.getRegetRootUrl() + "Participant/RejectSubstitution?t=" + new Date().getTime(), jsonEntityData).then(function (response) {
                    try {
                        var result = response.data;
                        var strResult = result.string_value;
                        if (_this.isStringValueNullOrEmpty(strResult)) {
                            subst.approval_status = RegetApp.ApprovalStatus.Rejected;
                            subst.approval_status_text = _this.locRejectedText;
                            subst.is_editable_app_man = false;
                            subst.is_approve_hidden = true;
                            subst.is_reject_hidden = true;
                            subst.is_can_be_deleted = false;
                            return true;
                        }
                        else {
                            var msgError = _this.getErrorMessage(strResult);
                            _this.showAlert(_this.locErrorTitleText, msgError, _this.locCloseText);
                            return false;
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
                    return false;
                });
                return true;
            };
            //************************************************************************
            //************************************************************************
            //Methods
            UserSubstitutionController.prototype.setGrid = function () {
                var _this = this;
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
                        cellClass: function (grid, row, col, rowRenderIndex, colRenderIndex) {
                            return _this.getSubsRowBkg(row.entity);
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
                        cellClass: function (grid, row, col, rowRenderIndex, colRenderIndex) {
                            return _this.getSubsRowBkg(row.entity);
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
                        cellClass: function (grid, row, col, rowRenderIndex, colRenderIndex) {
                            return _this.getSubsRowBkg(row.entity);
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
                        cellClass: function (grid, row, col, rowRenderIndex, colRenderIndex) {
                            return _this.getSubsRowBkg(row.entity);
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
                        cellClass: function (grid, row, col, rowRenderIndex, colRenderIndex) {
                            return _this.getSubsRowBkg(row.entity);
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
                        cellClass: function (grid, row, col, rowRenderIndex, colRenderIndex) {
                            return _this.getSubsRowBkg(row.entity);
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
                        cellClass: function (grid, row, col, rowRenderIndex, colRenderIndex) {
                            return _this.getSubsRowBkg(row.entity);
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
                        cellClass: function (grid, row, col, rowRenderIndex, colRenderIndex) {
                            return _this.getSubsRowBkg(row.entity);
                        }
                    },
                    {
                        name: "modified_user_name", displayName: this.locModifyUserText, field: "modified_user_name",
                        enableHiding: true,
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextTemplateReadOnly.html",
                        enableCellEdit: false,
                        width: 160,
                        minWidth: 100,
                        cellClass: function (grid, row, col, rowRenderIndex, colRenderIndex) {
                            return _this.getSubsRowBkg(row.entity);
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
                        cellClass: function (grid, row, col, rowRenderIndex, colRenderIndex) {
                            return _this.getSubsRowBkg(row.entity);
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
                        cellClass: function (grid, row, col, rowRenderIndex, colRenderIndex) {
                            return _this.getSubsRowBkg(row.entity);
                        }
                    }
                ];
            };
            UserSubstitutionController.prototype.isReadOnlySubstituted = function () {
                if ($("#IsSubstitutedReadOnly") === null || (this.isStringValueNullOrEmpty($("#IsSubstitutedReadOnly").val()))) {
                    return false;
                }
                return true;
            };
            UserSubstitutionController.prototype.getReadOnlySubstituted = function () {
                if (!this.isReadOnlySubstituted()) {
                    return;
                }
                //let substitutedId: number = this.getIntegerFromInput("CurrentUserId");
                this.selectedsubstitutedUser = new RegetApp.Participant();
                this.selectedsubstitutedUser.id = this.currentUserId;
                this.selectedsubstitutedUser.name_surname_first = this.currentUserName;
                this.getSubstitutedApprovers(true);
            };
            UserSubstitutionController.prototype.loadData = function () {
                try {
                    this.dateFromMin = new Date();
                    this.dateToMin = new Date();
                    this.minFromValueErrMsg = this.locMinDateText; //this.locMinDateText.replace("{0}", moment().format(this.dateTimeMomentFormatText));
                    this.minToValueErrMsg = this.locMinDateText; //this.locMinDateText.replace("{0}", moment().format(this.dateTimeMomentFormatText));
                    var urlParams = this.getAllUrlParams();
                    if (!this.isValueNullOrUndefined(urlParams)) {
                        for (var i = 0; i < urlParams.length; i++) {
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
                    }
                    else {
                        //this.loadGridData();
                        this.getLoadDataGridSettings();
                    }
                }
                catch (ex) {
                    this.displayErrorMsg();
                }
            };
            UserSubstitutionController.prototype.gridDeleteRowFromDb = function (entity, ev) {
                var _this = this;
                this.showLoaderBoxOnly(this.isError);
                var jsonData = JSON.stringify(entity);
                this.$http.post(this.deleteUrl, jsonData).then(function (response) {
                    try {
                        var isWasDeleted = _this.gridRowWasDeleted(response, entity);
                        if (isWasDeleted == true) {
                            _this.removeEditCol();
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
                //return deferred.promise;
            };
            //**********************************************************        
            //**********************************************************
            //Extra Methods
            UserSubstitutionController.prototype.displaySuccess = function (strText) {
                this.$mdToast.show(this.$mdToast.simple()
                    .textContent(strText)
                    .position('bottom center')
                    .theme("success-toast"));
                this.displayErrorMsg();
            };
            UserSubstitutionController.prototype.formatGridDate = function (gridData) {
                if (this.isValueNullOrUndefined(gridData)) {
                    return;
                }
                for (var i = 0; i < gridData.length; i++) {
                    var userSubstitution = gridData[i];
                    var jsonDateFrom = gridData[i].substitute_start_date;
                    var jDateFrom = this.convertJsonDate(jsonDateFrom);
                    gridData[i].substitute_start_date = jDateFrom;
                    var jsonDateTo = gridData[i].substitute_end_date;
                    var jDateTo = this.convertJsonDate(jsonDateTo);
                    gridData[i].substitute_end_date = jDateTo;
                    var jsonModifyDate = gridData[i].modified_date;
                    var jModifiedDate = this.convertJsonDate(jsonModifyDate);
                    gridData[i].modified_date = jModifiedDate;
                    //date = date ? moment(date).format($("#DateTimePickerFormatMoment").val()) : ''; 
                    //gridData[i].substitute_start_date = this.getDateTimeString(gridData[i].substitute_start_date);
                    //var m = moment.utc(moment(date).format('M/D/YYYY h:m A')).toDate();
                }
            };
            UserSubstitutionController.prototype.gridEditRow = function (rowEntity) {
                if (!rowEntity["is_editable_author"] || !this.isEditColVisible) {
                    return;
                }
                _super.prototype.gridEditRow.call(this, rowEntity);
                ////set colums access
                //let elDates: HTMLCollectionOf<HTMLDivElement> = <HTMLCollectionOf<HTMLDivElement>> (document.getElementsByClassName("div-date-edit"));
                //elDates[0].setAttribute("display", "none");
            };
            UserSubstitutionController.prototype.filterFromDatePickerChange = function (col) {
                //this.datePickerChange();
                //if (!moment(this.fromDateFilter).isValid()) {
                //    //this.displayErrorMsg(this.locDateFromatText);
                //    return;
                //}
                col.filters[0].term = moment(this.fromDateFilter).format(this.filterDateFormat);
                //this.loadGridData();
            };
            UserSubstitutionController.prototype.filterToDatePickerChange = function (col) {
                col.filters[0].term = moment(this.toDateFilter).format(this.filterDateFormat);
            };
            UserSubstitutionController.prototype.filterModifyDateFromPickerChange = function (col) {
                col.filters[0].term = this.urlFromFilterDelimiter + moment(this.modifyDateFromFilter).format(this.filterDateFormat);
            };
            UserSubstitutionController.prototype.filterModifyDateToPickerChange = function (col) {
                if (col.filters.length < 2) {
                    var filterOption = { term: this.urlToFilterDelimiter + moment(this.modifyDateToFilter).format(this.filterDateFormat) };
                    col.filters.push(filterOption);
                }
                else {
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
                var gridApiAny = this.gridApi;
                //gridApiAny.core.raise.filterChanged(col);
                gridApiAny.grid.clearAllFilters();
            };
            UserSubstitutionController.prototype.dateTimeFromFilterChanged = function (isTime, col) {
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
            };
            UserSubstitutionController.prototype.hideEditGridButton = function (rowEntity) {
                return !rowEntity["is_editable_author"];
                //return true;
            };
            UserSubstitutionController.prototype.hideDeleteGridButton = function (rowEntity) {
                return !rowEntity["is_can_be_deleted"];
            };
            UserSubstitutionController.prototype.searchParticipant = function (strName) {
                if (this.isSkipSubstitutedUserLoad || this.isSkipSubstituteeUserLoad) {
                    if (this.isSkipSubstitutedUserLoad) {
                        this.isSkipSubstitutedUserLoad = false;
                    }
                    else if (this.isSkipSubstituteeUserLoad) {
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
            };
            UserSubstitutionController.prototype.substitutedItemChange = function (item, strErrorMsgId) {
                this.selectedsubstitutedUser = item;
                this.checkUserMandatory(item, strErrorMsgId);
                if (!this.isValueNullOrUndefined(item)) {
                    this.approvers = this.getSubstitutedApprovers(false);
                }
            };
            UserSubstitutionController.prototype.substituteeItemChange = function (item, strErrorMsgId) {
                this.selectedsubstituteeUser = item;
                this.checkUserMandatory(item, strErrorMsgId);
            };
            UserSubstitutionController.prototype.substitutedQuerySearch = function (strName) {
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
            };
            UserSubstitutionController.prototype.filterSubstituted = function (searchText) {
                var _this = this;
                var deferred = this.$q.defer();
                this.$http.get(this.getRegetRootUrl() + '/Participant/GetSubstitutedMen?name=' + encodeURI(searchText)
                    + '&t=' + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        var participants = tmpData;
                        //console.log(participants.length);
                        return deferred.resolve(participants);
                        //}
                    }
                    catch (e) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        _this.hideLoader();
                    }
                }, function (response) {
                    //deferred.resolve($scope.Suppliers);
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
                // }
                return deferred.promise;
            };
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
            UserSubstitutionController.prototype.substitutedUserOnBlur = function (strErrorMsgId) {
                this.checkUserMandatory(this.selectedsubstitutedUser, strErrorMsgId);
            };
            UserSubstitutionController.prototype.substituteeUserOnBlur = function (strErrorMsgId) {
                this.checkUserMandatory(this.selectedsubstituteeUser, strErrorMsgId);
            };
            UserSubstitutionController.prototype.isSubstitutionValid = function () {
                if (this.isNewSubsValid()) {
                    return this.isNewSubstitutionValid();
                }
                return false;
            };
            UserSubstitutionController.prototype.isNewSubsValid = function () {
                return this.isNewValid;
            };
            UserSubstitutionController.prototype.isNewSubstitutionValid = function () {
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
                }
                catch (_a) {
                    this.substErrMsg = this.locErrorMsgText;
                    return false;
                }
            };
            UserSubstitutionController.prototype.addSubstitution = function () {
                try {
                    this.isNewValid = true;
                    if (this.isSubstitutionValid()) {
                        var isAppMan = this.isApproveMan();
                        var strRemark = this.remark;
                        if (this.isStringValueNullOrEmpty(strRemark)) {
                            strRemark = "";
                        }
                        var rowIndex = ((this.currentPage - 1) * this.pageSize) + this.substData.length + 1;
                        if (rowIndex < 1) {
                            rowIndex = 1;
                        }
                        var userSubstitutionEntity = new RegetApp.UserSubstitution(-1, this.selectedsubstitutedUser.id, this.selectedsubstituteeUser.id, this.fromDate, this.toDate, this.selectedsubstitutedUser.name_surname_first, this.selectedsubstituteeUser.name_surname_first, this.isTakeOver, strRemark, this.approval_status, this.getApprovalStatusText(this.approval_status), 
                        //this.getActiveStatus(userSubstitutionEntity),
                        this.approversText, this.currentUserName, rowIndex, true, isAppMan, false, false, true, new Date(), true);
                        userSubstitutionEntity.is_editable_author = true;
                        this.saveSubstitution(userSubstitutionEntity);
                        this.displayEditCol();
                    }
                    else {
                        var strMsg = this.locDataCannotBeSavedText;
                        if (!this.isStringValueNullOrEmpty(this.substErrMsg)) {
                            strMsg += "<br>" + this.substErrMsg;
                        }
                        this.displayErrorMsg(strMsg);
                    }
                }
                catch (_a) {
                    this.displayErrorMsg();
                }
                finally {
                    this.isNewValid = false;
                }
            };
            //private getActiveStatus(subst: UserSubstitution): string {
            //    return "";
            //}
            UserSubstitutionController.prototype.isApproveMan = function () {
                if (this.isValueNullOrUndefined(this.approvers) || this.approvers.length == 0) {
                    return false;
                }
                var tmpApproves = this.$filter("filter")(this.approvers, { id: this.currentUserId }, true);
                if (!this.isValueNullOrUndefined(this.approvers) && this.approvers.length > 0) {
                    return true;
                }
                return false;
            };
            UserSubstitutionController.prototype.getApprovalStatusText = function (appStatus) {
                if (appStatus === RegetApp.ApprovalStatus.Empty) {
                    return "";
                }
                if (appStatus === RegetApp.ApprovalStatus.Rejected) {
                    return this.locRejectedText;
                }
                if (appStatus === RegetApp.ApprovalStatus.NotNeeded) {
                    return this.locNotNeededText;
                }
                if (appStatus === RegetApp.ApprovalStatus.WaitForApproval) {
                    return this.locWaitForApprovalText;
                }
                if (appStatus === RegetApp.ApprovalStatus.Approved) {
                    return this.locApprovedText;
                }
                return "";
            };
            UserSubstitutionController.prototype.clearSubstitution = function () {
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
            };
            UserSubstitutionController.prototype.clearSubstitutionThread = function () {
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
            };
            UserSubstitutionController.prototype.displayGridCellTextArea = function (row) {
                try {
                    //this one is needed to eliminate problem with absolute position of the TextArea
                    //from some reason the position=relative is set for html form element after a substitution is added
                    var form = $("#frmUserSubstitution");
                    form[0].style.removeProperty('position');
                    var grdCellDivTextArea = document.getElementById("grdCellDivTextArea");
                    var grdCellTextArea = document.getElementById("grdCellTextArea");
                    var btnEdit = document.getElementById("btnTextAreaEdit");
                    var divRow = this.getRowElement(btnEdit);
                    var colRemarkIndex = this.getColumnIndex('remark');
                    var cells = divRow.getElementsByClassName("ui-grid-cell");
                    var divCol = cells[colRemarkIndex];
                    var rowId = row.entity.id;
                    var position = this.getPosition(rowId, true);
                    var iLeft = position.xPos;
                    var iTop = position.yPos;
                    var iWidth = position.width;
                    var editSubst = this.editRow;
                    grdCellDivTextArea.style.top = iTop + "px";
                    grdCellDivTextArea.style.left = iLeft + "px";
                    grdCellDivTextArea.style.width = (divCol.clientWidth + 2).toString() + "px";
                    //var newline = String.fromCharCode(13, 10);
                    //return input.replaceAll('\\n', newline);
                    grdCellTextArea.value = editSubst.remark;
                    grdCellDivTextArea.style.display = "block";
                    var divRegetToolTip = document.getElementById("divRegetToolTip");
                    divRegetToolTip.style.opacity = "0";
                    divRegetToolTip.style.visibility = "hidden";
                    grdCellTextArea.focus();
                }
                catch (_a) {
                    this.displayErrorMsg();
                }
            };
            UserSubstitutionController.prototype.getRowElement = function (childElement) {
                if (this.isValueNullOrUndefined(childElement.parentElement)) {
                    return null;
                }
                if (childElement.parentElement.className.indexOf("ui-grid-row") > -1) {
                    return childElement.parentElement;
                }
                return this.getRowElement(childElement.parentElement);
            };
            UserSubstitutionController.prototype.hideGridCellTextArea = function () {
                //return;
                var grdCellTextArea = document.getElementById("grdCellTextArea");
                var editSubst = this.editRow;
                editSubst.remark = grdCellTextArea.value;
                $("#grdCellDivTextArea").hide();
            };
            UserSubstitutionController.prototype.cellMouseOver = function (row) {
                if (this.isStringValueNullOrEmpty(row.entity.remark)) {
                    return;
                }
                var rowId = row.entity.id;
                var position = this.getPosition(rowId, false);
                var iLeft = position.xPos;
                var iTop = position.yPos;
                var iWidth = position.width;
                var iRowIndex = position.iRowIndex;
                if (this.isTooltipScrollDisplayed === true && (this.tooltipRowIndex + 1 === iRowIndex)) {
                    return;
                }
                var spanRegetTooltipText = document.getElementById("spanRegetTooltipText");
                spanRegetTooltipText.innerHTML = row.entity.remark.split("\n").join("<br>");
                var divRegetToolTip = document.getElementById("divRegetToolTip");
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
                }
                else {
                    this.isTooltipScrollDisplayed = false;
                }
                //alert("Left " + iLeft + ", Top " + iTop);
            };
            UserSubstitutionController.prototype.cellMouseLeave = function (row, col) {
                //if (this.isTooltipScrollLeave) {
                //    this.isTooltipScrollLeave = false;
                //    this.isTooltipOver = false;
                //}
                //if (this.isTooltipOver === true) {
                //    return;
                //}
                var spanRegetTooltipText = document.getElementById("spanRegetTooltipText");
                var hasVerticalScrollbar = spanRegetTooltipText.scrollHeight > spanRegetTooltipText.clientHeight;
                if (hasVerticalScrollbar) {
                    return;
                }
                var divRegetToolTip = document.getElementById("divRegetToolTip");
                divRegetToolTip.style.opacity = "0";
                divRegetToolTip.style.visibility = "hidden";
            };
            //public tooltipOver(): void {
            //    //this.isTooltipOver = true;
            //}
            UserSubstitutionController.prototype.tooltipLeave = function () {
                var spanRegetTooltipText = document.getElementById("spanRegetTooltipText");
                var hasVerticalScrollbar = spanRegetTooltipText.scrollHeight > spanRegetTooltipText.clientHeight;
                if (hasVerticalScrollbar) {
                    //this.isTooltipOver = false;
                    var divRegetToolTip = document.getElementById("divRegetToolTip");
                    divRegetToolTip.style.opacity = "0";
                    divRegetToolTip.style.visibility = "hidden";
                }
                this.isTooltipScrollDisplayed = false;
            };
            UserSubstitutionController.prototype.getPosition = function (rowId, isEdit) {
                //var tmp: any = document.getElementById("txtUserSubstituted");
                //tmp.setCustomValidity("This email is already registered!");
                var iRowIndex = -1;
                for (var i = 0; i < this.substData.length; i++) {
                    if (this.substData[i].id == rowId) {
                        iRowIndex = i;
                        break;
                    }
                }
                if (iRowIndex < 0) {
                    return;
                }
                var table = document.getElementById("grdUserSubstitution");
                var rows = table.getElementsByClassName("ui-grid-row");
                var divRow = rows[iRowIndex];
                var iScrollY = $(".reget-body").scrollTop() + window.pageYOffset; //window.scrollY;
                var iTop = iScrollY + Math.floor(divRow.getBoundingClientRect().top);
                if (!isEdit) {
                    iTop += 5;
                }
                var colRemarkIndex = this.getColumnIndex('remark');
                var cells = divRow.getElementsByClassName("ui-grid-cell");
                var divCol = cells[colRemarkIndex];
                var iScrollX = $(".reget-body").scrollLeft();
                var iLeft = iScrollX + Math.floor(divCol.getBoundingClientRect().left);
                iTop += divCol.clientHeight;
                var iWidth = divCol.clientWidth + 80;
                if (iWidth < 200) {
                    iWidth = 200;
                }
                if (!isEdit) {
                    var leftFix = Math.floor((iWidth - divCol.clientWidth) / 2);
                    iLeft -= leftFix;
                }
                //iTop = 20;
                var pos = new RowElementPosition(iLeft, iTop, iWidth, iRowIndex);
                return pos;
            };
            UserSubstitutionController.prototype.toggleTakeOver = function () {
                if (this.isTakeOver === true) {
                    this.isTakeOver = false;
                }
                else {
                    this.isTakeOver = true;
                }
                //this.isTakeOver = !this.isTakeOver;
            };
            UserSubstitutionController.prototype.setEditApproveColButtons = function () {
                if (this.isApproveColVisible === true) {
                    var isFound = false;
                    for (var j = this.gridOptions.columnDefs.length - 1; j >= 0; j--) {
                        if (this.gridOptions.columnDefs[j].name === "action_buttons_approve") {
                            isFound = true;
                            break;
                        }
                    }
                    if (isFound === false) {
                        //Add Column
                        this.insertDataGridColum(this.approveColumn, this.iApproveColIndex);
                    }
                }
                else {
                    for (var j = this.gridOptions.columnDefs.length - 1; j >= 0; j--) {
                        if (this.gridOptions.columnDefs[j].name === "action_buttons_approve") {
                            this.iApproveColIndex = j;
                            this.approveColumn = angular.copy(this.gridOptions.columnDefs[j]);
                            this.gridOptions.columnDefs.splice(j, 1);
                            break;
                        }
                    }
                }
                if (this.isEditColVisible === true) {
                    var isFound = false;
                    for (var j = this.gridOptions.columnDefs.length - 1; j >= 0; j--) {
                        if (this.gridOptions.columnDefs[j].name === "action_buttons") {
                            isFound = true;
                            break;
                        }
                    }
                    if (isFound === false) {
                        //Add Column
                        this.insertDataGridColum(this.editColumn, this.iEditColIndex);
                    }
                }
                else {
                    for (var j = this.gridOptions.columnDefs.length - 1; j >= 0; j--) {
                        if (this.gridOptions.columnDefs[j].name === "action_buttons") {
                            //this.actionCol = this.gridOptions.columnDefs[j];
                            this.iEditColIndex = j;
                            this.editColumn = angular.copy(this.gridOptions.columnDefs[j]);
                            this.gridOptions.columnDefs.splice(j, 1);
                            break;
                        }
                    }
                }
            };
            UserSubstitutionController.prototype.displayEditCol = function () {
                var isFound = false;
                for (var i = 0; i < this.gridOptions.columnDefs.length; i++) {
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
            };
            UserSubstitutionController.prototype.removeEditCol = function () {
                var isEditRemove = true;
                var isFound = true;
                var tmpEditColIndex = -1;
                for (var i = 0; i < this.gridOptions.columnDefs.length; i++) {
                    if (this.gridOptions.columnDefs[i].name === "action_buttons") {
                        tmpEditColIndex = i;
                        isFound = true;
                        break;
                    }
                }
                if (isFound === true) {
                    for (var i_1 = 0; i_1 < this.substData.length; i_1++) {
                        if (this.substData[i_1].is_editable_app_man
                            || this.substData[i_1].is_editable_author
                            || this.substData[i_1].is_can_be_deleted) {
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
            };
            UserSubstitutionController.prototype.gridApproveRow = function (entity, ev) {
                var subst = entity;
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
            };
            UserSubstitutionController.prototype.approveDialog = function (subst) {
                var strReplMsg = subst.substituted_name_surname_first + " -> " + subst.substitutee_name_surname_first
                    + " " + this.getDateTimeString(subst.substitute_start_date) + " - " + this.getDateTimeString(subst.substitute_end_date);
                var strMsg = this.locApproveConfirmText.replace("{0}", strReplMsg);
                this.$mdDialog.show({
                    template: this.getConfirmDialogTemplate(strMsg, this.locConfirmationText, this.locYesText, this.locNoText, "confirmDialog()", "closeDialog()"),
                    locals: {
                        userSubstControl: this,
                        subst: subst
                    },
                    controller: this.dialogApproveConfirmController
                });
            };
            UserSubstitutionController.prototype.dialogApproveConfirmController = function ($scope, $mdDialog, userSubstControl, subst) {
                $scope.closeDialog = function () {
                    $mdDialog.hide();
                };
                $scope.confirmDialog = function () {
                    $mdDialog.hide();
                    userSubstControl.approveSubstitution(subst);
                };
            };
            UserSubstitutionController.prototype.gridRejectRow = function (entity, ev) {
                var subst = entity;
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
            };
            UserSubstitutionController.prototype.rejectDialog = function (subst) {
                var strReplMsg = subst.substituted_name_surname_first + " -> " + subst.substitutee_name_surname_first
                    + " " + this.getDateTimeString(subst.substitute_start_date) + " - " + this.getDateTimeString(subst.substitute_end_date);
                var strMsg = this.locRejectConfirmText.replace("{0}", strReplMsg);
                this.$mdDialog.show({
                    template: this.getConfirmDialogTemplate(strMsg, this.locConfirmationText, this.locYesText, this.locNoText, "confirmDialog()", "closeDialog()"),
                    locals: {
                        userSubstControl: this,
                        subst: subst
                    },
                    controller: this.dialogRejectConfirmController
                });
            };
            UserSubstitutionController.prototype.dialogRejectConfirmController = function ($scope, $mdDialog, userSubstControl, subst) {
                $scope.closeDialog = function () {
                    $mdDialog.hide();
                };
                $scope.confirmDialog = function () {
                    $mdDialog.hide();
                    userSubstControl.rejectSubstitution(subst);
                };
            };
            UserSubstitutionController.prototype.getSubsRowBkg = function (substitution) {
                if (this.isValueNullOrUndefined(substitution)) {
                    return null;
                }
                if (substitution.approval_status == RegetApp.ApprovalStatus.Rejected) {
                    return 'grid-row-rejected';
                }
                else if (substitution.active === false) {
                    return 'grid-row-disabled';
                }
                return null;
            };
            UserSubstitutionController.prototype.gridRowDetail = function (rowEntity) {
                //let divNewSubst: HTMLDivElement = <HTMLDivElement>document.getElementById("divAddSubstContent");
                //let vis = divNewSubst.style.visibility;
                var urlParAddSubstVisible = "newSubstDisplayed=0";
                if ($("#divAddSubstContent").is(':visible')) {
                    urlParAddSubstVisible = "newSubstDisplayed=1";
                }
                var url = this.getRegetRootUrl() + "Participant/UserSubstitutionDetail?id=" + rowEntity.id
                    + "&filter=" + encodeURI(this.filterUrl)
                    + "&sort=" + encodeURI(this.sortColumnsUrl)
                    + "&pageSize=" + this.pageSize
                    + "&currPage=" + this.currentPage
                    + "&" + urlParAddSubstVisible
                    + "&t=" + new Date().getTime();
                window.location.href = url;
            };
            UserSubstitutionController.prototype.dateTimeToChanged = function (controlIndex, modifiedDate) {
                if (controlIndex === "0") {
                    this.dateToMin = modifiedDate;
                    //this.minToValueErrMsg = this.locMinDateText.replace("{0}", moment(modifiedDate).format(this.dateTimeMomentFormatText));
                }
            };
            //********** Because of Tests *****************************
            UserSubstitutionController.prototype.setIsDateFromValid = function (bValue) {
                this.isDateFromValid = bValue;
            };
            UserSubstitutionController.prototype.setIsDateToValid = function (bValue) {
                this.isDateToValid = bValue;
            };
            return UserSubstitutionController;
        }(RegetApp.BaseRegetGridTs));
        RegetApp.UserSubstitutionController = UserSubstitutionController;
        angular.
            module('RegetApp').
            controller('UserSubstitutionController', Kamsyk.RegetApp.UserSubstitutionController).
            config(function ($mdDateLocaleProvider) {
            this.SetDatePicker($mdDateLocaleProvider);
            //it is neccessary to set IsGenerateDatePickerLocalization = true
        });
        //angular.module('RegetApp').directive("timedirective", [() => new timedirective()]);
        //angular.module('RegetApp').directive("timedirective", timedirective.instance);
        //angular.module('RegetApp').directive("timedirective", timedirective.factory);
    })(RegetApp = Kamsyk.RegetApp || (Kamsyk.RegetApp = {}));
})(Kamsyk || (Kamsyk = {}));
var RowElementPosition = /** @class */ (function () {
    function RowElementPosition(xPos, yPos, width, iRowIndex) {
        this.xPos = null;
        this.yPos = null;
        this.width = null;
        this.iRowIndex = null;
        this.xPos = xPos;
        this.yPos = yPos;
        this.width = width;
        this.iRowIndex = iRowIndex;
    }
    return RowElementPosition;
}());
//# sourceMappingURL=user-substitution.js.map