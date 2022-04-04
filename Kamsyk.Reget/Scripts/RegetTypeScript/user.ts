module Kamsyk.RegetApp {
    export class UserController extends BaseRegetGridTs implements angular.IController {

        //**********************************************************
        //Local Texts
        private locFirstNameText: string = $("#FirstNameText").val();
        private locSurnameText: string = $("#SurnameText").val();
        private locLoginText: string = $("#LoginText").val();
        private locMailText: string = $("#MailText").val();
        private locPhoneText: string = $("#PhoneText").val();
        private locOfficeText: string = $("#OfficeText").val();
        private locActiveText: string= $("#ActiveText").val();
        private locIsExternalText: string = $("#IsExternalText").val();
        private locEnterUserNameText: string = $("#EnterUserNameText").val();
        private locUserExistText: string = $("#UserExistText").val();
        private locWarningText: string = $("#WarningText").val();
        private locDeleteUserConfirmText: string = $("#DeleteUserConfirmText").val();
        private locUserWasDisabledText: string = $("#UserWasDisabledText").val();
        private locUserNotFoundText: string = $("#UserNotFoundText").val();
        private locErrMsg: string = $("#ErrMsgText").val();
        private locUserInfoText: string = $("#UserInfoText").val();
        private locUserActiveAppManText: string = $("#UserInfoText").val();
        private locUserCannotBeDeactivatedAppManText: string = $("#UserCannotBeDeactivatedAppManText").val();
        private locUserCannotBeDeactivatedOrdererText: string = $("#UserCannotBeDeactivatedOrdererText").val();
        private locUserCannotBeDeletedAppManText: string = $("#UserCannotBeDeletedAppManText").val();
        private locUserCannotBeDeletedOrdererText: string = $("#UserCannotBeDeletedOrdererText").val();
        private locUserRolesText: string = $("#UserRolesText").val();
        //**********************************************************

        //***************************************************************
        //Propertie
        private gridType: string = $("#GridType").val();
        private companies: AgDropDown[] = null;
        private isParticipantLoaded: boolean = false;
        private isCompanyListLoaded: boolean = false;
        private skipLoad: boolean = false;
        //private isNewRow: boolean = false;

        private yesNo: any[] = [{ value: true, label: this.locYesText }, { value: false, label: this.locNoText }];
        private yesFilter: any[] = [{ value: true, label: this.locYesText }];

        public hiddenColNames = ["id", "manager_id"];
        private delDeactParticipant : Participant = null;
        //***************************************************************

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

        ) {
            super($scope, $http, $filter, $mdDialog, $mdToast, $q, uiGridConstants, $timeout);
                        
            this.setGrid();
            this.loadData();
        }
        //***************************************************************


        $onInit = () => { };

        //****************************************************************************
        //Abstract
        public exportToXlsUrl(): string {
            if (this.gridType === "NonActiveUsers") {
                return this.getRegetRootUrl() + "Report/GetNonActiveParticipants?" +
                    "filter=" + encodeURI(this.filterUrl) +
                    "&sort=" + this.sortColumnsUrl +
                    "&t=" + new Date().getTime();
            } else {
                return this.getRegetRootUrl() + "Report/GetParticipants?" +
                    "filter=" + encodeURI(this.filterUrl) +
                    "&sort=" + this.sortColumnsUrl +
                    "&t=" + new Date().getTime();
            }
        }

        public getControlColumnsCount(): number {
            return 3;
        }

        public getDuplicityErrMsg(participant: Participant): string {
            return null;
        }

        public getSaveRowUrl(): string {
            return this.getRegetRootUrl() + "Participant/SaveUserData?t=" + new Date().getTime();
        }

        public insertRow(): void {
            this.isNewRow = true;
            var newParticipant: Participant = new Participant();
            newParticipant.id = -10;
            newParticipant.surname = "";
            newParticipant.first_name = "";
            newParticipant.phone = "";
            newParticipant.office_name = "";
            newParticipant.active = true;
                        
            this.insertBaseRow(newParticipant);
        }

        public isRowChanged(): boolean {

            if (this.editRow === null) {
                return true;
            }

            let origParticipant: Participant = this.editRowOrig;
            let updParticipant: Participant = this.editRow;
            this.delDeactParticipant = updParticipant;
            
            var id: number = updParticipant.id;
            
            if (id < 0) {
                //new row
                this.newRowIndex = null;

                return true;
            } else {
                if (origParticipant.surname !== updParticipant.surname) {
                    return true;
                } else if (origParticipant.first_name !== updParticipant.first_name) {
                    return true;
                } else if (origParticipant.phone !== updParticipant.phone) {
                    return true;
                } else if (origParticipant.office_name !== updParticipant.office_name) {
                    return true;
                } else if (origParticipant.email !== updParticipant.email) {
                    return true;
                } else if (origParticipant.is_external !== updParticipant.is_external) {
                    return true;
                } else if (origParticipant.active !== updParticipant.active) {
                    return true;
                } 
            }

            return false;

        }

        public isRowEntityValid(participant: Participant): string {
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
        }

        public loadGridData(): void {
            this.getParticipants();
        }

        public dbGridId: string = null;

        public getMsgDisabled(participant: Participant): string {
            return this.locUserWasDisabledText.replace("{0}", this.getUserNameSurnameFirst(participant));
        }


        public getMsgDeleteConfirm(participant: Participant): string {
            return this.locDeleteUserConfirmText.replace("{0}", this.getUserNameSurnameFirst(participant));
        }

        public getErrorMsgByErrId(errId: number, msg: string): string {
            let urlLink = this.getRegetRootUrl() + "Participant/UserInfo";
            if (!this.isValueNullOrUndefined(this.delDeactParticipant)) {
                urlLink += "?userId=" + this.delDeactParticipant.id;
            }
            let strUserRolesLink = "<a href=\"" + urlLink + "\" target=\"_blank\">" + this.locUserRolesText + "</a>";

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
        }

        public deleteUrl: string = this.getRegetRootUrl() + "Participant/DeleteUser" + "?t=" + new Date().getTime();

        public getDbGridId(): string {
            return this.dbGridId;
        }
        //*****************************************************************************


        //**************************** Overwritten Methods ****************************
        public gridDeleteRow(entity: any, ev: MouseEvent) {
            this.delDeactParticipant = entity;
            super.gridDeleteRow(entity, ev);
        }
        //*****************************************************************************

        //*************************************************************************************
        //Http
        private getCompanies(): void {

            this.showLoader(this.isError);

            this.$http.get(
                this.getRegetRootUrl() + "Participant/GetParticipantCompanies?t=" + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    this.companies = tmpData;
                    this.isCompanyListLoaded = true;

                    this.getLoadDataGridSettings();

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
                    this.setGridColumFilter("office_name", this.companies);

                    //external
                    this.setGridColumFilter("is_external", this.yesNo);
                    
                    //active
                    if (this.gridType === "NonActiveUsers") {
                        this.setGridColumFilter("active", this.yesFilter);
                    } else {
                        this.setGridColumFilter("active", this.yesNo);
                    }
                    
                    this.gridApi.core.notifyDataChange(this.uiGridConstants.dataChange.COLUMN);
                    
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

        private getParticipants() : void {
            if (this.skipLoad === true) {
                return;
            }

            this.showLoader();

            var urlPart: string = null;
            if (this.gridType === "NonActiveUsers") {
                urlPart = this.getRegetRootUrl() + "Participant/GetNonActiveParticipants?filter="
                    + this.encodeUrl(this.filterUrl) +
                    "&pageSize=" + this.pageSize +
                    "&currentPage=" + this.currentPage +
                    "&sort=" + this.sortColumnsUrl +
                    "&t=" + new Date().getTime();
            } else {
                urlPart = this.getRegetRootUrl() + "Participant/GetParticipants?filter="
                    + this.encodeUrl(this.filterUrl) +
                    "&pageSize=" + this.pageSize +
                    "&currentPage=" + this.currentPage +
                    "&sort=" + this.sortColumnsUrl +
                    "&t=" + new Date().getTime();
            }

            urlPart = this.encodeUrl(urlPart);

            this.$http.get(
                urlPart,
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    this.gridOptions.data = tmpData.db_data;
                    this.rowsCount = tmpData.rows_count;
                    
                    if (!this.isValueNullOrUndefined(this.gridApi)) {
                        this.gridApi.core.notifyDataChange(this.uiGridConstants.dataChange.COLUMN);
                    }

                    this.isParticipantLoaded = true;

                    //this.loadGridSettings();
                    this.testLoadDataCount++;
                    this.setGridSettingData();

                    if (this.gridType === "NonActiveUsers" &&
                        this.gridOptions.data.length === 0 &&
                        !this.isFilterApplied) {

                        $("#divNonActiveGrid").hide();
                        $("#divNonActiveInfo").show();

                    } else {
                        $("#divNonActiveGrid").show();
                        $("#divNonActiveInfo").hide();
                    }

                    //********************************************************************
                    //it is very important otherwise 50 lines are nod diplaye dproperly !!!
                    this.gridOptions.virtualizationThreshold = this.rowsCount + 1;
                    //********************************************************************

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

        private getParticipantFromActiveDirectory(userName : string) {
            if (userName === null || userName.length === 0) {
                this.displayErrorMsg(this.locEnterUserNameText);
                return;
            }

            this.isError = false;

            this.showLoaderBoxOnly(this.isError);

            this.$http.get(
                this.getRegetRootUrl() + "Participant/GetParticipantFromActiveDirectory?userName=" + userName + '&companyId=0&t=' + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    var adUser : Participant = tmpData;

                    if (this.isValueNullOrUndefined(adUser)) {
                        var notExistMsg : string = this.locUserNotFoundText.replace("{0}", userName);
                        this.displayErrorMsg(notExistMsg);
                    } else {
                        var participant: Participant = this.editRow;
                        participant.first_name = adUser.first_name;
                        participant.surname = adUser.surname;
                        participant.email = adUser.email;
  
                    }

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

        private getParticipantCompany(userName : string) {
            if (userName === null || userName.length === 0) {
                this.displayErrorMsg(this.locEnterUserNameText);
                return;
            }
            this.isError = false;
            this.showLoaderBoxOnly(this.isError);

            this.$http.get(
                this.getRegetRootUrl() + "Participant/GetParticipantCompanyByUserName?userName=" + userName + '&t=' + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    var company: Company = tmpData;
                    if (this.isValueNullOrUndefined(company)) {
                        this.getParticipantFromActiveDirectory(userName);
                    } else {
                        this.displayErrorMsg(this.locUserExistText.replace('{0}', userName).replace('{1}', company.country_code));
                    }

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

        //*************************************************************************************

        //*************************************************************************************
        //Methods
        private setGrid(): void {
            if (this.isNonActiveUsers()) {
                this.setGridNonActiveUsers();
            } else {
                this.setGridAllUsers();
            }
        }

        private setGridAllUsers(): void {
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
                    cellTemplate:
                        '<md-button class="reget-btn-info" style="margin-top:5px!important;margin-left:8px!important;" ng-click="grid.appScope.openUserRoles(row)" >' +
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
        }

        private setGridNonActiveUsers(): void {
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
                    cellTemplate:
                        '<md-button class="reget-btn-info" style="margin-top:5px!important;margin-left:8px!important;" ng-click="grid.appScope.openUserRoles(row)" >' +
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
        }

        private openUserRoles(e : any) {
            var userId: number = e.entity.id;
            var userRolesLink = this.getRegetRootUrl() + "Participant/UserInfo?userId=" + userId;
            window.open(userRolesLink);
        }

        private loadData() : void {
            if (this.gridType === "NonActiveUsers") {
                this.dbGridId = "grdUserNonActive_rg";
            } else {
                this.dbGridId = "grdUser_rg";
            }

            this.getCompanies();
        }

        private hideLoaderWrapper() : void {
            if (this.isError || (this.isParticipantLoaded && this.isCompanyListLoaded)) {
                this.hideLoader();
            }
        }

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

        private getUserByUsername(userName : string) {
            this.getParticipantCompany(userName);
        }

        private isNewRowActive(): boolean {
            if (this.isValueNullOrUndefined(this.editRow)) {
                return false;
            }

            var participant: Participant = this.editRow;
            if (participant.id < 0) {
                return true;
            }

            return false;
        }

        private isNonActiveUsers() : boolean {
            return (this.gridType === "NonActiveUsers");
        }

        private getUserNameSurnameFirst(participant: Participant): string {
            var userName: string = "";
            if (this.isValueNullOrUndefined(participant)) {
                return "";
            }

            if (!this.isStringValueNullOrEmpty(participant.surname)) {
                userName = participant.surname;
            }

            if (!this.isStringValueNullOrEmpty(participant.first_name)) {
                if (this.isStringValueNullOrEmpty(userName)) {
                    userName = participant.first_name;
                } else {
                    userName += " " + participant.first_name
                }
                
            }

            return userName;
        }
        //*************************************************************************************
    }
    angular.
        module("RegetApp").
        controller("UserController", Kamsyk.RegetApp.UserController);
}