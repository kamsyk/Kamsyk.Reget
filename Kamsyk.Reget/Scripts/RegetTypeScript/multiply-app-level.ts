/// <reference path="../RegetTypeScript/Base/reget-base.ts" />
/// <reference path="../RegetTypeScript/Base/reget-common.ts" />

module Kamsyk.RegetApp {
    
    export class MultiplyAppLevelController extends BaseRegetTs implements angular.IController {
        //private KEY_USER_DATA: string = "user_data";

        private userAdminCompanies: CompanyCheckbox[] = null;
        private multiplText: string = null;
        private multipl: number = null;
        private substErrMsg: string = null;
        private isAll: boolean = false;
        private copyOption: string = "Selected";
        private multiplyLimitResult: MultiplyLimitResult = null;

        //private locEnterDecimalNumberText: string = $("#EnterDecimalNumberText").val();
        //private locSelectCompanyText: string  = $("#SelectCompanyText").val();
        private locMultiplyText: string  = $("#MultiplyText").val();
        private locAppLevelMultiplyAllConfirmText: string  = $("#AppLevelMultiplyAllConfirmText").val();
        private locAppLevelMultiplySelectedConfirmText: string  = $("#AppLevelMultiplySelectedConfirmText").val();
        private locLimitMultiplyResultText: string = $("#LimitMultiplyResultText").val();
        private locFinishedText: string = $("#FinishedText").val();
        private locMultiplPgLimitErrorText: string = $("#MultiplPgLimitErrorText").val();
        public locConfirmationText: string = $("#ConfirmationText").val();
        private locEnterMandatoryValuesText: string = $("#EnterMandatoryValuesText").val();
               
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
        //Methods
        private loadSubstUsers(): void {
            this.showLoader(this.isError);

            this.$http.get(
                this.getRegetRootUrl() + "RegetAdmin/GetUserAdminCompanies?t=" + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    this.userAdminCompanies = tmpData;
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

        private multiplyAppLevels(): void {
            if (this.isMultiplyValid() === false) {
                this.displayErrorMsg(this.locEnterMandatoryValuesText);
                return;
            }

            this.isAll = (this.copyOption == "All");

            let msg : string = (this.isAll) ? this.locAppLevelMultiplyAllConfirmText : this.locAppLevelMultiplySelectedConfirmText;

            this.$mdDialog.show(
                {
                    template: this.getConfirmDialogTemplate(
                        msg,
                        this.locConfirmationText,
                        this.locMultiplyText,
                        this.locCancelText,
                        "confirmDialog()",
                        "closeDialog()"),
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
            
        }

        private multiplWasChanged(strMultipl: string, input: any) {
            this.validateDecimalNumber(strMultipl, input);
        }

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

        private dialogConfirmController($scope, $mdDialog, multiplyControl: MultiplyAppLevelController): void {

            $scope.closeDialog = function () {
                $mdDialog.hide();
            }

            $scope.confirmDialog = () => {
                $mdDialog.hide();
                multiplyControl.multiplyAppLevelsConfirmed();
            }
        }

        private multiplyAppLevelsConfirmed(): void {
            this.showLoaderBoxOnly();

            var multipleCompany: MultipleCompany = new MultipleCompany(
                this.userAdminCompanies,
                this.multipl,
                this.isAll
            );

            var jsonMultipleCompany = JSON.stringify(multipleCompany);

            this.$http.post(
                this.getRegetRootUrl() + 'RegetAdmin/MultiplyAppLevels',
                jsonMultipleCompany
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    this.multiplyLimitResult = tmpData;
                    if (this.isValueNullOrUndefined(this.multiplyLimitResult.err_msg) || this.multiplyLimitResult.err_msg.length == 0) {
                        let limitAffected: number = this.multiplyLimitResult.affected_limits_count;
                        let msg: string = this.locLimitMultiplyResultText.replace("{0}", limitAffected.toString());

                        //this.showAlert(this.locFinishedText, msg, this.locCloseText);

                        this.showInfo(this.locFinishedText, msg, this.locCloseText);
                    } else {
                        this.displayErrorMsg(this.locMultiplPgLimitErrorText);
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

        private toggleCompany(company : any): void {
            company.is_selected = !company.is_selected;
            let divErrCompany: HTMLDivElement = <HTMLDivElement>document.getElementById("divErrCompany");
            
            if (this.isCompanySelected()) {
                divErrCompany.style.display = "none";
            } else {
                divErrCompany.style.display = "block";
            }
        }
        
        private loadData(): void {
            try {
                this.loadSubstUsers();
            } catch (ex) {
                this.displayErrorMsg();
            }
        }

        private isMultiplyValid(): boolean {
            let isValid : boolean = true;
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
            } else {
                if (this.convertTextToDecimal(this.multiplText) > 0) {
                    this.multipl = this.convertTextToDecimal(this.multiplText);
                } else {
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
            let isCompSel: boolean = this.isCompanySelected();
            if (isCompSel === false) {
                //this.substErrMsg = this.locSelectCompanyText;
                //isValid = false;

                let divErrCompany: HTMLDivElement = <HTMLDivElement>document.getElementById("divErrCompany");
                divErrCompany.style.display = "block";
                isValid = false;

            }

            return isValid;
        }

        private isCompanySelected(): boolean {
            
            for (var i: number = 0; i < this.userAdminCompanies.length; i++) {
                if (this.userAdminCompanies[i].is_selected === true) {
                    return true;
                }
            }

            return false;
        }
                
        private toggleAllCompanies() : void {

            if (this.isValueNullOrUndefined(this.userAdminCompanies)) {
                return;
            }

            if (this.isAllCompaniesChecked()) {
                for (var i = 0; i < this.userAdminCompanies.length; i++) {
                    this.userAdminCompanies[i].is_selected = false;
                }
            } else {
                for (var j = 0; j < this.userAdminCompanies.length; j++) {
                    this.userAdminCompanies[j].is_selected = true;
                }
            }

            let divErrCompany: HTMLDivElement = <HTMLDivElement>document.getElementById("divErrCompany");

            if (this.isCompanySelected()) {
                divErrCompany.style.display = "none";
            } else {
                divErrCompany.style.display = "block";
            }
        }

        private isAllCompaniesChecked() : boolean {
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
        }

        private isIndeterminate() {
            if (this.isValueNullOrUndefined(this.userAdminCompanies)) {
                return false;
            }

            var isAllChecked = this.isAllCompaniesChecked();

            return (this.userAdminCompanies.length !== 0 &&
                !isAllChecked);
        }

       
        //************************************************************************
    }
        

    angular.
        module('RegetApp').
        controller('MultiplyAppLevelController', Kamsyk.RegetApp.MultiplyAppLevelController);

    export class CompanyCheckbox {
        public company_id: number = null;
        public country_code: string = null;
        public is_selected: boolean = false;
    }

    export class MultipleCompany {
        public companies: CompanyCheckbox[] = null;
        public multipl: number = null;
        public isAll: boolean = false;

        constructor(companies: CompanyCheckbox[], multipl: number, isAll: boolean) {
            this.companies = companies;
            this.multipl = multipl;
            this.isAll = isAll;
        }
    }

    export class MultiplyLimitMsg {
        public cgId: number = null;
        public cgName: string = null;
        public pgId: number = null;
        public pgName: string = null;
        public err_msg: string = null;
    }

    export class MultiplyLimitResult {
        public affected_limits_count: number = 0;
        public err_msg : MultiplyLimitMsg[] = null;
    }
}