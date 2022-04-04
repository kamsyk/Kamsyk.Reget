/// <reference path="../RegetTypeScript/Base/reget-base.ts" />
/// <reference path="../RegetTypeScript/Base/reget-common.ts" />
/// <reference path="../RegetTypeScript/Base/reget-entity.ts" />

module Kamsyk.RegetApp {
    export class UserReplaceController extends BaseRegetTs implements angular.IController {
        //**********************************************************
        //Properties

        private selectedUserToBeReplaced: string = null;
        private searchstringusertobereplaced: string = null;
                                
        private userToBeReplaced: Participant = null;
        private userInfo: Participant = null;
        private replaceEntities: ReplaceEntity = null;

        private selectedReplaceAppMan: string = null;
        private searchstringreplaceappman: string = null;
        private replaceAppMan: Participant = null;

        private selectedReplaceRequestor: string = null;
        private searchstringreplacerequestor: string = null;
        private replaceRequestor: Participant = null;

        private selectedReplaceOrderer: string = null;
        private searchstringreplaceorderer: string = null;
        private replaceOrderer: Participant = null;

        private selectedCentreMan: string = null;
        private searchstringreplacecentreman: string = null;
        private replaceCentreMan: Participant = null;

        private replaceRequestorErrMsg: string = null;
        private replaceAppManErrMsg: string = null;
        private replaceOrdererErrMsg: string = null;
        private replaceCentreManErrMsg: string = null;

        private isEntityDataLoaded: boolean = false;
        private isParticipantDataLoaded: boolean = false;
                                        
        //**********************************************************

        //***************************************************************************
        //Localization
        private locEnterText: string = $("#EnterText").val();
        private locRequestorText: string = $("#RequestorText").val();
        private locOrdererText: string = $("#OrdererText").val();
        private locApproveManText: string = $("#ApproveManText").val();
        private locCentreManagerText: string = $("#CentreManagerText").val();
        private locReplacerText: string = $("#ReplacerText").val();
        //*****************************************************************************

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

        //******************************************************************
        //Methods

        //******************************************************************
        //Http

        private getReplaceEntityData(): void {
            this.showLoaderBoxOnly();


            this.$http.get(
                this.getRegetRootUrl() + "Participant/GetReplaceEntities?userToBeReplacedId=" + this.userToBeReplaced.id + "&t=" + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    this.replaceEntities = tmpData;
                    this.isEntityDataLoaded = true;
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

        private getUserData(): void {
            this.showLoaderBoxOnly();


            this.$http.get(
                this.getRegetRootUrl() + "Participant/GetParticipantById?userId=" + this.userToBeReplaced.id + "&t=" + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    this.userInfo = tmpData;
                    this.isParticipantDataLoaded = true;
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

        private saveReplaceRequestorToDb() : void {
            this.showLoader();

            for (var i: number = this.replaceEntities.cg_requestors.length - 1; i >= 0; i--) {
                this.replaceEntities.cg_requestors[i].replace_user_id = this.replaceRequestor.id;
            }

            var jsonData = JSON.stringify(this.replaceEntities.cg_requestors);

            this.$http.post(
                this.getRegetRootUrl() + "Participant/ReplaceRequestor",
                jsonData
            ).then((response) => {
                try {
                    for (var i: number = this.replaceEntities.cg_requestors.length - 1; i >= 0; i--) {
                        if (this.replaceEntities.cg_requestors[i].is_selected) {
                            this.replaceEntities.cg_requestors.splice(i, 1);
                        }
                    }

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

        private saveReplaceAppManToDb(): void {
            this.showLoader();

            for (var i: number = this.replaceEntities.cg_app_men.length - 1; i >= 0; i--) {
                this.replaceEntities.cg_app_men[i].replace_user_id = this.replaceAppMan.id;
            }

            var jsonData = JSON.stringify(this.replaceEntities.cg_app_men);

            this.$http.post(
                this.getRegetRootUrl() + "Participant/ReplaceAppMan",
                jsonData
            ).then((response) => {
                try {
                    for (var i: number = this.replaceEntities.cg_app_men.length - 1; i >= 0; i--) {
                        if (this.replaceEntities.cg_app_men[i].is_selected) {
                            this.replaceEntities.cg_app_men.splice(i, 1);
                        }
                    }

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

        private saveReplaceCentreManToDb(): void {
            this.showLoader();

            for (var i: number = this.replaceEntities.centre_man.length - 1; i >= 0; i--) {
                this.replaceEntities.centre_man[i].replace_user_id = this.replaceCentreMan.id;
            }

            var jsonData = JSON.stringify(this.replaceEntities.centre_man);

            this.$http.post(
                this.getRegetRootUrl() + "Participant/ReplaceCentreMan",
                jsonData
            ).then((response) => {
                try {
                    for (var i: number = this.replaceEntities.centre_man.length - 1; i >= 0; i--) {
                        if (this.replaceEntities.centre_man[i].is_selected) {
                            this.replaceEntities.centre_man.splice(i, 1);
                        }
                    }

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

        private saveReplaceOrdererToDb(): void {
            this.showLoader();

            for (var i: number = this.replaceEntities.cg_orderers.length - 1; i >= 0; i--) {
                this.replaceEntities.cg_orderers[i].replace_user_id = this.replaceOrderer.id;
            }

            var jsonData = JSON.stringify(this.replaceEntities.cg_orderers);

            this.$http.post(
                this.getRegetRootUrl() + "Participant/ReplaceOrderer",
                jsonData
            ).then((response) => {
                try {
                    for (var i: number = this.replaceEntities.cg_orderers.length - 1; i >= 0; i--) {
                        if (this.replaceEntities.cg_orderers[i].is_selected) {
                            this.replaceEntities.cg_orderers.splice(i, 1);
                        }
                    }

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

        private getParticipantData(id: number): void {
            this.showLoader();
            
            this.$http.get(
                this.getRegetRootUrl() + "Participant/GetParticipantById?userId=" + id + "&t=" + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    var tmpPartData: Participant = tmpData;
                    this.selectedUserToBeReplaced = tmpPartData.name_surname_first;
                    //this.userToBeReplaced = tmpData[0];

                    this.userToBeReplacedSelectedItemChange(tmpPartData, null);
                } catch (e) {
                    this.hideLoader();
                    this.displayErrorMsg();
                } finally {
                    //this.hideLoader();
                }
            }, (response: any) => {
                this.hideLoader();
                this.displayErrorMsg();
            });
        
        }

        //private getParticipantData(): void {
        //    this.showLoader(this.isError);

        //    this.$http.get(
        //        this.getRegetRootUrl() + "Participant/GetCompanyAdminActiveParticipantsData?t=" + new Date().getTime(),
        //        {}
        //    ).then((response) => {
        //        try {
        //            $scope.Participants = response.data;
        //            $scope.IsParticipantLoaded = true;

        //            if (IsStringValueNullOrEmpty($scope.searchstringusertobereplaced)) {

        //                var initReplUserId = $("#UserToBeReplacedId").val();
        //                if (!(IsStringValueNullOrEmpty(initReplUserId))) {
        //                    $scope.searchstringusertobereplaced = $("#UserToBeReplacedName").val();

        //                    initReplUserId = parseInt(initReplUserId);
        //                    var tmpParticipants = $filter("filter")($scope.Participants, { id: initReplUserId }, true);
        //                    if (!IsValueNullOrUndefined(tmpParticipants) && !IsValueNullOrUndefined(tmpParticipants[0])) {
        //                        $scope.UserToBeReplaced = tmpParticipants[0];
        //                        $scope.SelectedUserToBeReplaced = tmpParticipants[0];
        //                    }
        //                }
        //            }
        //        } catch (e) {
        //            this.hideLoader();
        //            this.displayErrorMsg();
        //        } finally {
        //            this.hideLoader();
        //        }
        //    }, (response: any) => {
        //        this.hideLoader();
        //        this.displayErrorMsg();
        //    });

        //}

        //******************************************************************

        public loadData(): void {
            try {
                var strInitReplUserId: string = $("#UserToBeReplacedId").val();
                if (!(this.isStringValueNullOrEmpty(strInitReplUserId))) {
                    this.searchstringusertobereplaced = $("#UserToBeReplacedName").val();

                    var initReplUserId: number = parseInt(strInitReplUserId);

                    this.getParticipantData(initReplUserId);

                    //var tmpParticipants: Participant[] = this.searchParticipant(this.searchstringusertobereplaced);
                    //if (!this.isValueNullOrUndefined(tmpParticipants) && tmpParticipants.length > 0) {
                    //    var tmpParticipantsFilter: Participant[] = this.$filter("filter")(tmpParticipants, { id: initReplUserId }, true);
                    //    if (!this.isValueNullOrUndefined(tmpParticipantsFilter)) {
                    //        this.userToBeReplaced = tmpParticipantsFilter[0];
                    //        this.selectedUserToBeReplaced = tmpParticipantsFilter[0];
                    //    }
                    //}                  
                }

            } catch (ex) {
                this.hideLoader();
                this.displayErrorMsg();
            }
        }

        //private searchParticipant(strName: string): ng.IPromise<Participant[]> {
        //    var a: ng.IPromise<Participant[]>;
        //    setTimeout(() => { console.log("zapis"); a = this.filterParticipants(strName); }, 5500);

        //    return a;

        //    //return this.filterParticipants(strName);

        //    //var y: ng.IPromise<Participant[]> = null;
        //    //setTimeout(x => this.filterParticipants(strName), 5000);
        //    //return (x);
            
        //}

        private searchParticipant(strName: string): ng.IPromise<Participant[]> {
            return this.filterParticipants(strName);

        }

        
        //private searchParticipant(strName: string): any {
        //    var deferred: any = this.$q.defer<any>();

        //    this.debounce(() => {
        //        console.log("participant load debounce");
        //        return deferred.resolve(this.filterParticipants(strName));
        //    }, 1000);

        //    return deferred.promise; 
        //}
        

        private userToBeReplacedSelectedItemChange(item: Participant, strErrorMsgId: string): void {
            this.selectedReplaceAppMan = null;
            this.selectedReplaceRequestor = null;
            this.selectedReplaceOrderer = null;
            this.selectedCentreMan = null;

            this.searchstringreplaceappman = null;
            this.searchstringreplacerequestor = null;
            this.searchstringreplaceorderer = null;
            this.searchstringreplacecentreman = null;

            this.userToBeReplaced = item;
            this.checkUserMandatory(this.userToBeReplaced, strErrorMsgId);
            this.userInfo = null;

            if (!this.isValueNullOrUndefined(this.userToBeReplaced) && !this.isValueNullOrUndefined(this.userToBeReplaced.id)) {
                this.isParticipantDataLoaded = false;
                this.isEntityDataLoaded = false;

                this.getReplaceEntityData();
                this.getUserData();
            } else {
                this.hideLoader();
            }
        }

        private userToBeReplacedOnBlur(strErrorMsgId: string) : void {
            this.checkUserMandatory(this.userToBeReplaced, strErrorMsgId);
        }

        private replaceAppManOnBlur(strErrorMsgId: string) : void {
            this.checkUserMandatory(this.replaceAppMan, strErrorMsgId);
        }

        private replaceRequestorOnBlur(strErrorMsgId: string) {
            this.checkUserMandatory(this.replaceRequestor, strErrorMsgId);
        }

        private replaceOrdererOnBlur(strErrorMsgId: string) {
            this.checkUserMandatory(this.replaceOrderer, strErrorMsgId);
        }

        private replaceAppManSelectedItemChange(item: Participant, strErrorMsgId: string) : void {
            this.replaceAppMan = item;
            this.checkUserMandatory(this.replaceAppMan, strErrorMsgId);
        }

        private replaceRequestorSelectedItemChange(item: Participant, strErrorMsgId: string): void {
            this.replaceRequestor = item;
            this.checkUserMandatory(this.replaceRequestor, strErrorMsgId);
        }

        private replaceOrdererSelectedItemChange(item: Participant, strErrorMsgId: string) {
            this.replaceOrderer = item;
            this.checkUserMandatory(this.replaceOrderer, strErrorMsgId);
        }

        private replaceCentreManSelectedItemChange(item: Participant, strErrorMsgId: string) {
            this.replaceCentreMan = item;
            this.checkUserMandatory(this.replaceCentreMan, strErrorMsgId);
        }

        private displayCentreGroup(cgId: number, $event: MouseEvent) {
            var strCgLink: string = this.getRegetRootUrl() + "RegetAdmin/CentreGroup?cgId=" + cgId;
            window.open(strCgLink);
            $event.stopPropagation();
        }

        private isNothingToReplace() : boolean {
            var isNothingReplacing: boolean = (!this.isValueNullOrUndefined(this.selectedUserToBeReplaced)) &&
                ((this.isValueNullOrUndefined(this.replaceEntities)) || (
                (this.isValueNullOrUndefined(this.replaceEntities.cg_app_men) || this.replaceEntities.cg_app_men.length === 0) &&
                (this.isValueNullOrUndefined(this.replaceEntities.cg_requestors) || this.replaceEntities.cg_requestors.length === 0) &&
                (this.isValueNullOrUndefined(this.replaceEntities.cg_orderers) || this.replaceEntities.cg_orderers.length === 0) &&
                (this.isValueNullOrUndefined(this.replaceEntities.centre_man) || this.replaceEntities.centre_man.length === 0)
                ));

            return isNothingReplacing;
        }

        private isIndeterminate(cgReplaces: CentreGroupReplace[]) : boolean {
            if (this.replaceEntities === null || cgReplaces === null) {
                return false;
            }

            let isAllChecked : boolean = this.isAllChecked(cgReplaces);

            return (cgReplaces.length !== 0 &&
                !isAllChecked);
        }


        private toggleAllCgReplace(cgReplaces: CentreGroupReplace[]) {
            
            if (this.isAllChecked(cgReplaces)) {
                for (var i = 0; i < cgReplaces.length; i++) {
                    cgReplaces[i].is_selected = false;
                }
            } else {
                for (var i = 0; i < cgReplaces.length; i++) {
                    cgReplaces[i].is_selected = true;
                    cgReplaces[i].orig_user_id = this.userToBeReplaced.id;
                }
            }
        }

        private isAllChecked(cgReplaces: CentreGroupReplace[]) {
            let isSelectedAll = true;
            if (this.replaceEntities === null || cgReplaces === null) {
                return false;
            }
            for (var j: number = 0; j < cgReplaces.length; j++) {
                if (!cgReplaces[j].is_selected) {
                    isSelectedAll = false;
                    break;
                }
            }

            return isSelectedAll;
        }

        private toggleReplace(item: CentreGroupReplace) : void {

            if (item.is_selected) {
                item.is_selected = false;
            } else {
                item.is_selected = true;
                item.orig_user_id = this.userToBeReplaced.id;
                
            }

        }

        private isReplaceRequestorValid(): boolean {
            if (this.isValueNullOrUndefined(this.replaceRequestor)) {
                this.replaceRequestorErrMsg = this.locEnterText + " " + this.locReplacerText;
                return false;
            }

            return true;
        }

        private isReplaceAppManValid(): boolean {
            if (this.isValueNullOrUndefined(this.replaceAppMan)) {
                this.replaceAppManErrMsg = this.locEnterText + " " + this.locReplacerText;
                return false;
            }

            return true;
        }

        private isReplaceOrdererValid(): boolean {
            if (this.isValueNullOrUndefined(this.replaceOrderer)) {
                this.replaceOrdererErrMsg = this.locEnterText + " " + this.locReplacerText;
                return false;
            }

            return true;
        }

        private isReplaceCentreManValid(): boolean {
            if (this.isValueNullOrUndefined(this.replaceCentreMan)) {
                this.replaceCentreManErrMsg = this.locEnterText + " " + this.locReplacerText;
                return false;
            }

            return true;
        }
                                
        private saveReplaceRequestor(): void {
            if (!this.isReplaceRequestorValid()) {
                return;
            }

            this.saveReplaceRequestorToDb();
        }

        private saveReplaceAppMan(): void {
            if (!this.isReplaceAppManValid()) {
                return;
            }

            this.saveReplaceAppManToDb();
        }

        private saveReplaceOrderer(): void {
            if (!this.isReplaceOrdererValid()) {
                return;
            }

            this.saveReplaceOrdererToDb();
        }

        private saveReplaceCentreMan(): void {
            if (!this.isReplaceCentreManValid()) {
                return;
            }

            this.saveReplaceCentreManToDb();
        }

        private hideLoaderWrapper() {
            if (this.isError || (
                this.isEntityDataLoaded
                && this.isParticipantDataLoaded)) {
                this.hideLoader();
            }
        }
          
        //******************************************************************
    }

    angular.
        module('RegetApp').
        controller('UserReplaceController', Kamsyk.RegetApp.UserReplaceController);

    export class CentreGroupReplace {
        public cg_id: number = null;
        public name: string = null;
        public is_selected: boolean = false;
        public orig_user_id: number = null;
        public replace_user_id: number = null;
        public centres: string = null;
    }

    export class ReplaceEntity {
        public cg_app_men: CentreGroupReplace[] = null;
        public cg_requestors: CentreGroupReplace[] = null;
        public cg_orderers: CentreGroupReplace[] = null;
        public centre_man: CentreGroupReplace[] = null;
    }
}

