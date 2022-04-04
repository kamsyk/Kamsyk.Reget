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
        var UserController = /** @class */ (function (_super) {
            __extends(UserController, _super);
            //***************************************************************
            //**********************************************************
            //Constructor
            function UserController($scope, $http, $filter, $mdDialog, $mdToast, uiGridConstants, $q, $timeout) {
                var _this = _super.call(this, $scope, $http, $filter, $mdDialog, $mdToast, $q, uiGridConstants, $timeout) || this;
                _this.$scope = $scope;
                _this.$http = $http;
                _this.$filter = $filter;
                _this.$mdDialog = $mdDialog;
                _this.$mdToast = $mdToast;
                _this.uiGridConstants = uiGridConstants;
                _this.$q = $q;
                _this.$timeout = $timeout;
                //**********************************************************
                //Local Texts
                _this.locFirstNameText = $("#FirstNameText").val();
                _this.locSurnameText = $("#SurnameText").val();
                _this.locLoginText = $("#LoginText").val();
                _this.locMailText = $("#MailText").val();
                _this.locPhoneText = $("#PhoneText").val();
                _this.locOfficeText = $("#OfficeText").val();
                _this.locActiveText = $("#ActiveText").val();
                _this.locIsExternalText = $("#IsExternalText").val();
                _this.locEnterUserNameText = $("#EnterUserNameText").val();
                _this.locUserExistText = $("#UserExistText").val();
                _this.locWarningText = $("#WarningText").val();
                _this.locDeleteUserConfirmText = $("#DeleteUserConfirmText").val();
                _this.locUserWasDisabledText = $("#UserWasDisabledText").val();
                _this.locUserNotFoundText = $("#UserNotFoundText").val();
                _this.locErrMsg = $("#ErrMsgText").val();
                _this.locUserInfoText = $("#UserInfoText").val();
                _this.locUserActiveAppManText = $("#UserInfoText").val();
                _this.locUserCannotBeDeactivatedAppManText = $("#UserCannotBeDeactivatedAppManText").val();
                _this.locUserCannotBeDeactivatedOrdererText = $("#UserCannotBeDeactivatedOrdererText").val();
                _this.locUserCannotBeDeletedAppManText = $("#UserCannotBeDeletedAppManText").val();
                _this.locUserCannotBeDeletedOrdererText = $("#UserCannotBeDeletedOrdererText").val();
                _this.locUserRolesText = $("#UserRolesText").val();
                //**********************************************************
                //***************************************************************
                //Propertie
                _this.gridType = $("#GridType").val();
                _this.companies = null;
                _this.isParticipantLoaded = false;
                _this.isCompanyListLoaded = false;
                _this.skipLoad = false;
                //private isNewRow: boolean = false;
                _this.yesNo = [{ value: true, label: _this.locYesText }, { value: false, label: _this.locNoText }];
                _this.yesFilter = [{ value: true, label: _this.locYesText }];
                _this.hiddenColNames = ["id", "manager_id"];
                _this.delDeactParticipant = null;
                //***************************************************************
                _this.$onInit = function () { };
                _this.dbGridId = null;
                _this.deleteUrl = _this.getRegetRootUrl() + "Participant/DeleteUser" + "?t=" + new Date().getTime();
                _this.setGrid();
                _this.loadData();
                return _this;
            }
            //****************************************************************************
            //Abstract
            UserController.prototype.exportToXlsUrl = function () {
                if (this.gridType === "NonActiveUsers") {
                    return this.getRegetRootUrl() + "Report/GetNonActiveParticipants?" +
                        "filter=" + encodeURI(this.filterUrl) +
                        "&sort=" + this.sortColumnsUrl +
                        "&t=" + new Date().getTime();
                }
                else {
                    return this.getRegetRootUrl() + "Report/GetParticipants?" +
                        "filter=" + encodeURI(this.filterUrl) +
                        "&sort=" + this.sortColumnsUrl +
                        "&t=" + new Date().getTime();
                }
            };
            UserController.prototype.getControlColumnsCount = function () {
                return 3;
            };
            UserController.prototype.getDuplicityErrMsg = function (participant) {
                return null;
            };
            UserController.prototype.getSaveRowUrl = function () {
                return this.getRegetRootUrl() + "Participant/SaveUserData?t=" + new Date().getTime();
            };
            UserController.prototype.insertRow = function () {
                this.isNewRow = true;
                var newParticipant = new RegetApp.Participant();
                newParticipant.id = -10;
                newParticipant.surname = "";
                newParticipant.first_name = "";
                newParticipant.phone = "";
                newParticipant.office_name = "";
                newParticipant.active = true;
                this.insertBaseRow(newParticipant);
            };
            UserController.prototype.isRowChanged = function () {
                if (this.editRow === null) {
                    return true;
                }
                var origParticipant = this.editRowOrig;
                var updParticipant = this.editRow;
                this.delDeactParticipant = updParticipant;
                var id = updParticipant.id;
                if (id < 0) {
                    //new row
                    this.newRowIndex = null;
                    return true;
                }
                else {
                    if (origParticipant.surname !== updParticipant.surname) {
                        return true;
                    }
                    else if (origParticipant.first_name !== updParticipant.first_name) {
                        return true;
                    }
                    else if (origParticipant.phone !== updParticipant.phone) {
                        return true;
                    }
                    else if (origParticipant.office_name !== updParticipant.office_name) {
                        return true;
                    }
                    else if (origParticipant.email !== updParticipant.email) {
                        return true;
                    }
                    else if (origParticipant.is_external !== updParticipant.is_external) {
                        return true;
                    }
                    else if (origParticipant.active !== updParticipant.active) {
                        return true;
                    }
                }
                return false;
            };
            UserController.prototype.isRowEntityValid = function (participant) {
                if (this.isStringValueNullOrEmpty(participant.surname)) {
                    return this.locMissingMandatoryText;
                }
                if (this.isStringValueNullOrEmpty(participant.first_name)) {
                    return this.locMissingMandatoryText;
                }
                if (this.isStringValueNullOrEmpty(participant.user_name)) {
                    return this.locMissingMandatoryText;
                }
                if (this.isStringValueNullOrEmpty(participant.office_name)) {
                    return this.locMissingMandatoryText;
                }
                if (this.isStringValueNullOrEmpty(participant.email)) {
                    return this.locMissingMandatoryText;
                }
                return null;
            };
            UserController.prototype.loadGridData = function () {
                this.getParticipants();
            };
            UserController.prototype.getMsgDisabled = function (participant) {
                return this.locUserWasDisabledText.replace("{0}", this.getUserNameSurnameFirst(participant));
            };
            UserController.prototype.getMsgDeleteConfirm = function (participant) {
                return this.locDeleteUserConfirmText.replace("{0}", this.getUserNameSurnameFirst(participant));
            };
            UserController.prototype.getErrorMsgByErrId = function (errId, msg) {
                var urlLink = this.getRegetRootUrl() + "Participant/UserInfo";
                if (!this.isValueNullOrUndefined(this.delDeactParticipant)) {
                    urlLink += "?userId=" + this.delDeactParticipant.id;
                }
                var strUserRolesLink = "<a href=\"" + urlLink + "\" target=\"_blank\">" + this.locUserRolesText + "</a>";
                switch (errId) {
                    case -10: //user cannot be deactivated App Mans is active
                        return this.locUserCannotBeDeactivatedAppManText.replace("{0}", msg) + this.urlParamDelimiter + strUserRolesLink;
                    case -15: //user cannot be deactivated App Mans is active
                        return this.locUserCannotBeDeletedAppManText.replace("{0}", msg) + this.urlParamDelimiter + strUserRolesLink;
                    case -20: //user cannot be deactivated Orderer is active
                        return this.locUserCannotBeDeactivatedOrdererText.replace("{0}", msg) + this.urlParamDelimiter + strUserRolesLink;
                    case -25: //user cannot be deactivated Orderer is active
                        return this.locUserCannotBeDeletedOrdererText.replace("{0}", msg) + this.urlParamDelimiter + strUserRolesLink;
                    default:
                        return this.locErrorMsgText;
                }
            };
            UserController.prototype.getDbGridId = function () {
                return this.dbGridId;
            };
            //*****************************************************************************
            //**************************** Overwritten Methods ****************************
            UserController.prototype.gridDeleteRow = function (entity, ev) {
                this.delDeactParticipant = entity;
                _super.prototype.gridDeleteRow.call(this, entity, ev);
            };
            //*****************************************************************************
            //*************************************************************************************
            //Http
            UserController.prototype.getCompanies = function () {
                var _this = this;
                this.showLoader(this.isError);
                this.$http.get(this.getRegetRootUrl() + "Participant/GetParticipantCompanies?t=" + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        _this.companies = tmpData;
                        _this.isCompanyListLoaded = true;
                        _this.getLoadDataGridSettings();
                        //this.getParticipants();
                        ////set action buttons column width
                        //var pos : number = this.getColumnIndex("action_buttons");
                        //if (this.gridType === "NonActiveUsers") {
                        //    this.gridOptions.columnDefs[pos].width = 35;
                        //    this.gridOptions.columnDefs[pos].minWidth = 35;
                        //} else {
                        //    this.gridOptions.columnDefs[pos].width = 70;
                        //    this.gridOptions.columnDefs[pos].minWidth = 70;
                        //}
                        //company
                        _this.setGridColumFilter("office_name", _this.companies);
                        //external
                        _this.setGridColumFilter("is_external", _this.yesNo);
                        //active
                        if (_this.gridType === "NonActiveUsers") {
                            _this.setGridColumFilter("active", _this.yesFilter);
                        }
                        else {
                            _this.setGridColumFilter("active", _this.yesNo);
                        }
                        _this.gridApi.core.notifyDataChange(_this.uiGridConstants.dataChange.COLUMN);
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
            UserController.prototype.getParticipants = function () {
                var _this = this;
                if (this.skipLoad === true) {
                    return;
                }
                this.showLoader();
                var urlPart = null;
                if (this.gridType === "NonActiveUsers") {
                    urlPart = this.getRegetRootUrl() + "Participant/GetNonActiveParticipants?filter="
                        + this.encodeUrl(this.filterUrl) +
                        "&pageSize=" + this.pageSize +
                        "&currentPage=" + this.currentPage +
                        "&sort=" + this.sortColumnsUrl +
                        "&t=" + new Date().getTime();
                }
                else {
                    urlPart = this.getRegetRootUrl() + "Participant/GetParticipants?filter="
                        + this.encodeUrl(this.filterUrl) +
                        "&pageSize=" + this.pageSize +
                        "&currentPage=" + this.currentPage +
                        "&sort=" + this.sortColumnsUrl +
                        "&t=" + new Date().getTime();
                }
                urlPart = this.encodeUrl(urlPart);
                this.$http.get(urlPart, {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        _this.gridOptions.data = tmpData.db_data;
                        _this.rowsCount = tmpData.rows_count;
                        if (!_this.isValueNullOrUndefined(_this.gridApi)) {
                            _this.gridApi.core.notifyDataChange(_this.uiGridConstants.dataChange.COLUMN);
                        }
                        _this.isParticipantLoaded = true;
                        //this.loadGridSettings();
                        _this.testLoadDataCount++;
                        _this.setGridSettingData();
                        if (_this.gridType === "NonActiveUsers" &&
                            _this.gridOptions.data.length === 0 &&
                            !_this.isFilterApplied) {
                            $("#divNonActiveGrid").hide();
                            $("#divNonActiveInfo").show();
                        }
                        else {
                            $("#divNonActiveGrid").show();
                            $("#divNonActiveInfo").hide();
                        }
                        //********************************************************************
                        //it is very important otherwise 50 lines are nod diplaye dproperly !!!
                        _this.gridOptions.virtualizationThreshold = _this.rowsCount + 1;
                        //********************************************************************
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
            UserController.prototype.getParticipantFromActiveDirectory = function (userName) {
                var _this = this;
                if (userName === null || userName.length === 0) {
                    this.displayErrorMsg(this.locEnterUserNameText);
                    return;
                }
                this.isError = false;
                this.showLoaderBoxOnly(this.isError);
                this.$http.get(this.getRegetRootUrl() + "Participant/GetParticipantFromActiveDirectory?userName=" + userName + '&companyId=0&t=' + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        var adUser = tmpData;
                        if (_this.isValueNullOrUndefined(adUser)) {
                            var notExistMsg = _this.locUserNotFoundText.replace("{0}", userName);
                            _this.displayErrorMsg(notExistMsg);
                        }
                        else {
                            var participant = _this.editRow;
                            participant.first_name = adUser.first_name;
                            participant.surname = adUser.surname;
                            participant.email = adUser.email;
                        }
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
            UserController.prototype.getParticipantCompany = function (userName) {
                var _this = this;
                if (userName === null || userName.length === 0) {
                    this.displayErrorMsg(this.locEnterUserNameText);
                    return;
                }
                this.isError = false;
                this.showLoaderBoxOnly(this.isError);
                this.$http.get(this.getRegetRootUrl() + "Participant/GetParticipantCompanyByUserName?userName=" + userName + '&t=' + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        var company = tmpData;
                        if (_this.isValueNullOrUndefined(company)) {
                            _this.getParticipantFromActiveDirectory(userName);
                        }
                        else {
                            _this.displayErrorMsg(_this.locUserExistText.replace('{0}', userName).replace('{1}', company.country_code));
                        }
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
            //*************************************************************************************
            //*************************************************************************************
            //Methods
            UserController.prototype.setGrid = function () {
                if (this.isNonActiveUsers()) {
                    this.setGridNonActiveUsers();
                }
                else {
                    this.setGridAllUsers();
                }
            };
            UserController.prototype.setGridAllUsers = function () {
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
                        name: 'action_buttons',
                        displayName: '',
                        enableFiltering: false,
                        enableSorting: false,
                        enableCellEdit: false,
                        enableHiding: false,
                        width: 70,
                        enableColumnResizing: false,
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellAction.html"
                    },
                    {
                        name: 'user_info',
                        displayName: '',
                        enableFiltering: false,
                        enableSorting: false,
                        enableCellEdit: false,
                        enableHiding: false,
                        minWidth: 35,
                        width: 35,
                        cellTemplate: '<md-button class="reget-btn-info" style="margin-top:5px!important;margin-left:8px!important;" ng-click="grid.appScope.openUserRoles(row)" >' +
                            '<md-tooltip>{{grid.appScope.locUserInfoText}}</md-tooltip>' +
                            '</md-button>'
                    },
                    { name: 'id', visible: false },
                    {
                        name: "surname", displayName: this.locSurnameText, field: "surname",
                        enableHiding: false,
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridUserCellTextManadatoryTemplate.html",
                        enableCellEdit: false,
                        filter: {
                            condition: function (searchTerm, cellValue) {
                                return true;
                            }
                        },
                        minWidth: 110
                    },
                    {
                        name: "first_name", displayName: this.locFirstNameText, field: "first_name",
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridUserCellTextManadatoryTemplate.html",
                        enableCellEdit: false,
                        filter: {
                            condition: function (searchTerm, cellValue) {
                                return true;
                            }
                        },
                        minWidth: 110
                    },
                    {
                        name: 'user_name', displayName: this.locLoginText, field: "user_name",
                        enableCellEdit: false,
                        enableHiding: false,
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridUserNameReadOnlyTemplate.html",
                        minWidth: 100
                    },
                    {
                        name: 'email', displayName: this.locMailText, field: "email",
                        enableHiding: false,
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextTemplateReadOnly.html",
                        sortingAlgorithm: function (a, b, rowA, rowB, direction) {
                            return this.sortNullString(a, b, rowA, rowB, direction);
                        },
                        minWidth: 190
                    },
                    {
                        name: 'phone', displayName: this.locPhoneText, field: "phone",
                        enableCellEdit: false,
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridUserCellTextTemplate.html",
                        minWidth: 125
                    },
                    {
                        name: 'office_name', displayName: this.locOfficeText, field: 'office_name',
                        enableHiding: false,
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellDropDownMandatoryTemplate.html",
                        minWidth: 175
                    },
                    {
                        name: 'is_external', displayName: this.locIsExternalText, field: "is_external",
                        enableCellEdit: false,
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellCheckboxTemplate.html",
                        minWidth: 100,
                        width: 100
                    },
                    {
                        name: 'active', displayName: this.locActiveText, field: "active",
                        enableCellEdit: false,
                        enableHiding: false,
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellCheckboxTemplate.html",
                        minWidth: 100,
                        width: 100
                    }
                ];
            };
            UserController.prototype.setGridNonActiveUsers = function () {
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
                        name: 'action_buttons',
                        displayName: '',
                        enableFiltering: false,
                        enableSorting: false,
                        enableCellEdit: false,
                        enableHiding: false,
                        width: 70,
                        enableColumnResizing: false,
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellAction.html"
                    },
                    {
                        name: 'user_info',
                        displayName: '',
                        enableFiltering: false,
                        enableSorting: false,
                        enableCellEdit: false,
                        enableHiding: false,
                        minWidth: 35,
                        width: 35,
                        cellTemplate: '<md-button class="reget-btn-info" style="margin-top:5px!important;margin-left:8px!important;" ng-click="grid.appScope.openUserRoles(row)" >' +
                            '<md-tooltip>{{grid.appScope.locUserInfoText}}</md-tooltip>' +
                            '</md-button>'
                    },
                    { name: 'id', visible: false },
                    {
                        name: "surname", displayName: this.locSurnameText, field: "surname",
                        enableHiding: false,
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextTemplateReadOnly.html",
                        enableCellEdit: false,
                        filter: {
                            condition: function (searchTerm, cellValue) {
                                return true;
                            }
                        },
                        minWidth: 110
                    },
                    {
                        name: "first_name", displayName: this.locFirstNameText, field: "first_name",
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextTemplateReadOnly.html",
                        enableCellEdit: false,
                        filter: {
                            condition: function (searchTerm, cellValue) {
                                return true;
                            }
                        },
                        minWidth: 110
                    },
                    {
                        name: 'user_name', displayName: this.locLoginText, field: "user_name",
                        enableCellEdit: false,
                        enableHiding: false,
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextTemplateReadOnly.html",
                        minWidth: 100
                    },
                    {
                        name: 'email', displayName: this.locMailText, field: "email",
                        enableHiding: false,
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextTemplateReadOnly.html",
                        sortingAlgorithm: function (a, b, rowA, rowB, direction) {
                            return this.sortNullString(a, b, rowA, rowB, direction);
                        },
                        minWidth: 190
                    },
                    {
                        name: 'phone', displayName: this.locPhoneText, field: "phone",
                        enableCellEdit: false,
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextTemplateReadOnly.html",
                        minWidth: 125
                    },
                    {
                        name: 'office_name', displayName: this.locOfficeText, field: 'office_name',
                        enableHiding: false,
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextTemplateReadOnly.html",
                        minWidth: 175
                    },
                    {
                        name: 'is_external', displayName: this.locIsExternalText, field: "is_external",
                        enableCellEdit: false,
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellCheckboxReadOnlyTemplate.html",
                        minWidth: 100,
                        width: 100
                    },
                    {
                        name: 'active', displayName: this.locActiveText, field: "active",
                        enableCellEdit: false,
                        enableHiding: false,
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellCheckboxTemplate.html",
                        minWidth: 100,
                        width: 100
                    }
                ];
            };
            UserController.prototype.openUserRoles = function (e) {
                var userId = e.entity.id;
                var userRolesLink = this.getRegetRootUrl() + "Participant/UserInfo?userId=" + userId;
                window.open(userRolesLink);
            };
            UserController.prototype.loadData = function () {
                if (this.gridType === "NonActiveUsers") {
                    this.dbGridId = "grdUserNonActive_rg";
                }
                else {
                    this.dbGridId = "grdUser_rg";
                }
                this.getCompanies();
            };
            UserController.prototype.hideLoaderWrapper = function () {
                if (this.isError || (this.isParticipantLoaded && this.isCompanyListLoaded)) {
                    this.hideLoader();
                }
            };
            //private isFilterApplied() : boolean {
            //    for (var i = 0; i < this.gridApi.grid.columns.length - 1; i++) {
            //        var col: any = this.gridApi.grid.columns[i];
            //        if (!this.isValueNullOrUndefined(col.filters[0].term)) {
            //            if (col.enableFiltering && (col.filters[0].term !== '' || col.filters[0].term === true || col.filters[0].term === false)) {
            //                return true;
            //            }
            //        }
            //    }
            //    return false;
            //}
            UserController.prototype.getUserByUsername = function (userName) {
                this.getParticipantCompany(userName);
            };
            UserController.prototype.isNewRowActive = function () {
                if (this.isValueNullOrUndefined(this.editRow)) {
                    return false;
                }
                var participant = this.editRow;
                if (participant.id < 0) {
                    return true;
                }
                return false;
            };
            UserController.prototype.isNonActiveUsers = function () {
                return (this.gridType === "NonActiveUsers");
            };
            UserController.prototype.getUserNameSurnameFirst = function (participant) {
                var userName = "";
                if (this.isValueNullOrUndefined(participant)) {
                    return "";
                }
                if (!this.isStringValueNullOrEmpty(participant.surname)) {
                    userName = participant.surname;
                }
                if (!this.isStringValueNullOrEmpty(participant.first_name)) {
                    if (this.isStringValueNullOrEmpty(userName)) {
                        userName = participant.first_name;
                    }
                    else {
                        userName += " " + participant.first_name;
                    }
                }
                return userName;
            };
            return UserController;
        }(RegetApp.BaseRegetGridTs));
        RegetApp.UserController = UserController;
        angular.
            module("RegetApp").
            controller("UserController", Kamsyk.RegetApp.UserController);
    })(RegetApp = Kamsyk.RegetApp || (Kamsyk.RegetApp = {}));
})(Kamsyk || (Kamsyk = {}));
//# sourceMappingURL=user.js.map