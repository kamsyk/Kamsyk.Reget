/// <reference path="../RegetTypeScript/Base/reget-base.ts" />
/// <reference path="../RegetTypeScript/Base/reget-common.ts" />

module Kamsyk.RegetApp {
    export class AppMatrixExportController extends BaseRegetTs implements angular.IController {
        //**********************************************************
        //Properties
        private companies: CompanyDropDown[] = null;
        private selectedCompanyId: number = null;
        private centreGroups: CentreGroupSimple[] = null;

        private locEnterMandatoryValuesText: string = $("#EnterMandatoryValuesText").val();
        //**********************************************************

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

        //****************************************************************
        //Methods
        private getParticipantCompanies(): void {
            this.showLoader(this.isError);

            this.$http.get(
                this.getRegetRootUrl() + "RegetAdmin/GetActiveCompanies?t=" + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    this.companies = tmpData;

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

        private loadData(): void {
            this.getParticipantCompanies();
        }

        private exportToExcel() {
            if (!this.isExportValid()) {
                this.displayErrorMsg(this.locEnterMandatoryValuesText);
                return;
            }

            this.showLoader(this.isError);

            this.$http.get(
                this.getRegetRootUrl() + "RegetAdmin/GetCompanyCentreGroupsActiveOnly?companyId="
                + this.selectedCompanyId + "&t=" + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;

                    this.centreGroups = tmpData;
                    var companyName: string = 'ApprovalMartix';
                    var company = this.$filter("filter")(this.companies, { company_id: this.selectedCompanyId }, true);
                    if (company !== null) {
                        companyName = this.encodeUrl(company[0].country_code);
                    }
                    this.exportCgToExcel(this.centreGroups[0].id, 0, this.centreGroups.length, companyName);

                    
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

        private exportCgToExcel(iCgId: number, iCgIndex: number, iCgCount: number, strCompanyName: string) {
            this.showLoader(this.isError);

            $("#spanLoading").html(this.locLoadingDataText + ' ' + (iCgIndex + 1) + ' / ' + iCgCount);

            if (iCgIndex < (iCgCount - 1)) {
                this.$http.get(
                    this.getRegetRootUrl() + "Report/GetCompanyCentreGroup?cgId=" + iCgId
                    + "&cgIndex=" + iCgIndex + "&cgCount=" + iCgCount + "&companyName=" + strCompanyName
                    + "&t=" + new Date().getTime(),
                    {}
                ).then((response) => {
                    try {

                        if (iCgIndex < (iCgCount - 1)) {
                            var currIndex = iCgIndex + 1;
                            this.exportCgToExcel(this.centreGroups[currIndex].id, currIndex, this.centreGroups.length, strCompanyName);
                        } else {
                            this.hideLoader();
                        }

                    } catch (e) {
                        this.hideLoader();
                        this.displayErrorMsg();
                    }
                }, (response: any) => {
                    this.hideLoader();
                    this.displayErrorMsg();
                });
            } else {
                var url: string = this.getRegetRootUrl() + "Report/GetCompanyCentreGroup?cgId=" + iCgId
                    + "&cgIndex=" + iCgIndex + "&cgCount=" + iCgCount + "&companyName=" + this.encodeUrl(strCompanyName)
                    + "&t=" + new Date().getTime();
                window.open(url);
                this.hideLoader();
            }

            
        }

        private isExportValid(): boolean {
            let isValid: boolean = true;

            if (this.isValueNullOrUndefined(this.selectedCompanyId)) {
                this.angScopeAny.frmAppMatrixExport.cmbUserCompanies.$valid = false;
                this.angScopeAny.frmAppMatrixExport.cmbUserCompanies.$setTouched();

                isValid = false;
            }

            return isValid;
        }

        //private companyChanged() : void {

        //}
        //****************************************************************
    }
    angular.
        module('RegetApp').
        controller('AppMatrixExportController', Kamsyk.RegetApp.AppMatrixExportController);

    export class CentreGroupSimple {
        public id: number = null;
        public name: string = null;
    }

    export class CompanyDropDown {
        public company_id: number = null;
        public country_code: string = null;
    }
}