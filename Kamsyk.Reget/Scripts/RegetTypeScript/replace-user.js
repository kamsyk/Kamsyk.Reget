/// <reference path="../RegetTypeScript/Base/reget-base.ts" />
/// <reference path="../RegetTypeScript/Base/reget-common.ts" />
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
var Kamsyk;
(function (Kamsyk) {
    var RegetApp;
    (function (RegetApp) {
        var UserReplaceController = /** @class */ (function (_super) {
            __extends(UserReplaceController, _super);
            //*****************************************************************************
            //**********************************************************
            //Constructor
            function UserReplaceController($scope, $http, $filter, $mdDialog, $mdToast, $q, $timeout) {
                var _this = _super.call(this, $scope, $http, $filter, $mdDialog, $mdToast, $q, $timeout) || this;
                _this.$scope = $scope;
                _this.$http = $http;
                _this.$filter = $filter;
                _this.$mdDialog = $mdDialog;
                _this.$mdToast = $mdToast;
                _this.$q = $q;
                _this.$timeout = $timeout;
                //**********************************************************
                //Properties
                _this.selectedUserToBeReplaced = null;
                _this.searchstringusertobereplaced = null;
                _this.userToBeReplaced = null;
                _this.userInfo = null;
                _this.replaceEntities = null;
                _this.selectedReplaceAppMan = null;
                _this.searchstringreplaceappman = null;
                _this.replaceAppMan = null;
                _this.selectedReplaceRequestor = null;
                _this.searchstringreplacerequestor = null;
                _this.replaceRequestor = null;
                _this.selectedReplaceOrderer = null;
                _this.searchstringreplaceorderer = null;
                _this.replaceOrderer = null;
                _this.selectedCentreMan = null;
                _this.searchstringreplacecentreman = null;
                _this.replaceCentreMan = null;
                _this.replaceRequestorErrMsg = null;
                _this.replaceAppManErrMsg = null;
                _this.replaceOrdererErrMsg = null;
                _this.replaceCentreManErrMsg = null;
                _this.isEntityDataLoaded = false;
                _this.isParticipantDataLoaded = false;
                //**********************************************************
                //***************************************************************************
                //Localization
                _this.locEnterText = $("#EnterText").val();
                _this.locRequestorText = $("#RequestorText").val();
                _this.locOrdererText = $("#OrdererText").val();
                _this.locApproveManText = $("#ApproveManText").val();
                _this.locCentreManagerText = $("#CentreManagerText").val();
                _this.locReplacerText = $("#ReplacerText").val();
                //***************************************************************
                _this.$onInit = function () { };
                _this.loadData();
                return _this;
            }
            //******************************************************************
            //Methods
            //******************************************************************
            //Http
            UserReplaceController.prototype.getReplaceEntityData = function () {
                var _this = this;
                this.showLoaderBoxOnly();
                this.$http.get(this.getRegetRootUrl() + "Participant/GetReplaceEntities?userToBeReplacedId=" + this.userToBeReplaced.id + "&t=" + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        _this.replaceEntities = tmpData;
                        _this.isEntityDataLoaded = true;
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
            UserReplaceController.prototype.getUserData = function () {
                var _this = this;
                this.showLoaderBoxOnly();
                this.$http.get(this.getRegetRootUrl() + "Participant/GetParticipantById?userId=" + this.userToBeReplaced.id + "&t=" + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        _this.userInfo = tmpData;
                        _this.isParticipantDataLoaded = true;
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
            UserReplaceController.prototype.saveReplaceRequestorToDb = function () {
                var _this = this;
                this.showLoader();
                for (var i = this.replaceEntities.cg_requestors.length - 1; i >= 0; i--) {
                    this.replaceEntities.cg_requestors[i].replace_user_id = this.replaceRequestor.id;
                }
                var jsonData = JSON.stringify(this.replaceEntities.cg_requestors);
                this.$http.post(this.getRegetRootUrl() + "Participant/ReplaceRequestor", jsonData).then(function (response) {
                    try {
                        for (var i = _this.replaceEntities.cg_requestors.length - 1; i >= 0; i--) {
                            if (_this.replaceEntities.cg_requestors[i].is_selected) {
                                _this.replaceEntities.cg_requestors.splice(i, 1);
                            }
                        }
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
            UserReplaceController.prototype.saveReplaceAppManToDb = function () {
                var _this = this;
                this.showLoader();
                for (var i = this.replaceEntities.cg_app_men.length - 1; i >= 0; i--) {
                    this.replaceEntities.cg_app_men[i].replace_user_id = this.replaceAppMan.id;
                }
                var jsonData = JSON.stringify(this.replaceEntities.cg_app_men);
                this.$http.post(this.getRegetRootUrl() + "Participant/ReplaceAppMan", jsonData).then(function (response) {
                    try {
                        for (var i = _this.replaceEntities.cg_app_men.length - 1; i >= 0; i--) {
                            if (_this.replaceEntities.cg_app_men[i].is_selected) {
                                _this.replaceEntities.cg_app_men.splice(i, 1);
                            }
                        }
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
            UserReplaceController.prototype.saveReplaceCentreManToDb = function () {
                var _this = this;
                this.showLoader();
                for (var i = this.replaceEntities.centre_man.length - 1; i >= 0; i--) {
                    this.replaceEntities.centre_man[i].replace_user_id = this.replaceCentreMan.id;
                }
                var jsonData = JSON.stringify(this.replaceEntities.centre_man);
                this.$http.post(this.getRegetRootUrl() + "Participant/ReplaceCentreMan", jsonData).then(function (response) {
                    try {
                        for (var i = _this.replaceEntities.centre_man.length - 1; i >= 0; i--) {
                            if (_this.replaceEntities.centre_man[i].is_selected) {
                                _this.replaceEntities.centre_man.splice(i, 1);
                            }
                        }
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
            UserReplaceController.prototype.saveReplaceOrdererToDb = function () {
                var _this = this;
                this.showLoader();
                for (var i = this.replaceEntities.cg_orderers.length - 1; i >= 0; i--) {
                    this.replaceEntities.cg_orderers[i].replace_user_id = this.replaceOrderer.id;
                }
                var jsonData = JSON.stringify(this.replaceEntities.cg_orderers);
                this.$http.post(this.getRegetRootUrl() + "Participant/ReplaceOrderer", jsonData).then(function (response) {
                    try {
                        for (var i = _this.replaceEntities.cg_orderers.length - 1; i >= 0; i--) {
                            if (_this.replaceEntities.cg_orderers[i].is_selected) {
                                _this.replaceEntities.cg_orderers.splice(i, 1);
                            }
                        }
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
            UserReplaceController.prototype.getParticipantData = function (id) {
                var _this = this;
                this.showLoader();
                this.$http.get(this.getRegetRootUrl() + "Participant/GetParticipantById?userId=" + id + "&t=" + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        var tmpPartData = tmpData;
                        _this.selectedUserToBeReplaced = tmpPartData.name_surname_first;
                        //this.userToBeReplaced = tmpData[0];
                        _this.userToBeReplacedSelectedItemChange(tmpPartData, null);
                    }
                    catch (e) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    }
                    finally {
                        //this.hideLoader();
                    }
                }, function (response) {
                    _this.hideLoader();
                    _this.displayErrorMsg();
                });
            };
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
            UserReplaceController.prototype.loadData = function () {
                try {
                    var strInitReplUserId = $("#UserToBeReplacedId").val();
                    if (!(this.isStringValueNullOrEmpty(strInitReplUserId))) {
                        this.searchstringusertobereplaced = $("#UserToBeReplacedName").val();
                        var initReplUserId = parseInt(strInitReplUserId);
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
                }
                catch (ex) {
                    this.hideLoader();
                    this.displayErrorMsg();
                }
            };
            //private searchParticipant(strName: string): ng.IPromise<Participant[]> {
            //    var a: ng.IPromise<Participant[]>;
            //    setTimeout(() => { console.log("zapis"); a = this.filterParticipants(strName); }, 5500);
            //    return a;
            //    //return this.filterParticipants(strName);
            //    //var y: ng.IPromise<Participant[]> = null;
            //    //setTimeout(x => this.filterParticipants(strName), 5000);
            //    //return (x);
            //}
            UserReplaceController.prototype.searchParticipant = function (strName) {
                return this.filterParticipants(strName);
            };
            //private searchParticipant(strName: string): any {
            //    var deferred: any = this.$q.defer<any>();
            //    this.debounce(() => {
            //        console.log("participant load debounce");
            //        return deferred.resolve(this.filterParticipants(strName));
            //    }, 1000);
            //    return deferred.promise; 
            //}
            UserReplaceController.prototype.userToBeReplacedSelectedItemChange = function (item, strErrorMsgId) {
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
                }
                else {
                    this.hideLoader();
                }
            };
            UserReplaceController.prototype.userToBeReplacedOnBlur = function (strErrorMsgId) {
                this.checkUserMandatory(this.userToBeReplaced, strErrorMsgId);
            };
            UserReplaceController.prototype.replaceAppManOnBlur = function (strErrorMsgId) {
                this.checkUserMandatory(this.replaceAppMan, strErrorMsgId);
            };
            UserReplaceController.prototype.replaceRequestorOnBlur = function (strErrorMsgId) {
                this.checkUserMandatory(this.replaceRequestor, strErrorMsgId);
            };
            UserReplaceController.prototype.replaceOrdererOnBlur = function (strErrorMsgId) {
                this.checkUserMandatory(this.replaceOrderer, strErrorMsgId);
            };
            UserReplaceController.prototype.replaceAppManSelectedItemChange = function (item, strErrorMsgId) {
                this.replaceAppMan = item;
                this.checkUserMandatory(this.replaceAppMan, strErrorMsgId);
            };
            UserReplaceController.prototype.replaceRequestorSelectedItemChange = function (item, strErrorMsgId) {
                this.replaceRequestor = item;
                this.checkUserMandatory(this.replaceRequestor, strErrorMsgId);
            };
            UserReplaceController.prototype.replaceOrdererSelectedItemChange = function (item, strErrorMsgId) {
                this.replaceOrderer = item;
                this.checkUserMandatory(this.replaceOrderer, strErrorMsgId);
            };
            UserReplaceController.prototype.replaceCentreManSelectedItemChange = function (item, strErrorMsgId) {
                this.replaceCentreMan = item;
                this.checkUserMandatory(this.replaceCentreMan, strErrorMsgId);
            };
            UserReplaceController.prototype.displayCentreGroup = function (cgId, $event) {
                var strCgLink = this.getRegetRootUrl() + "RegetAdmin/CentreGroup?cgId=" + cgId;
                window.open(strCgLink);
                $event.stopPropagation();
            };
            UserReplaceController.prototype.isNothingToReplace = function () {
                var isNothingReplacing = (!this.isValueNullOrUndefined(this.selectedUserToBeReplaced)) &&
                    ((this.isValueNullOrUndefined(this.replaceEntities)) || ((this.isValueNullOrUndefined(this.replaceEntities.cg_app_men) || this.replaceEntities.cg_app_men.length === 0) &&
                        (this.isValueNullOrUndefined(this.replaceEntities.cg_requestors) || this.replaceEntities.cg_requestors.length === 0) &&
                        (this.isValueNullOrUndefined(this.replaceEntities.cg_orderers) || this.replaceEntities.cg_orderers.length === 0) &&
                        (this.isValueNullOrUndefined(this.replaceEntities.centre_man) || this.replaceEntities.centre_man.length === 0)));
                return isNothingReplacing;
            };
            UserReplaceController.prototype.isIndeterminate = function (cgReplaces) {
                if (this.replaceEntities === null || cgReplaces === null) {
                    return false;
                }
                var isAllChecked = this.isAllChecked(cgReplaces);
                return (cgReplaces.length !== 0 &&
                    !isAllChecked);
            };
            UserReplaceController.prototype.toggleAllCgReplace = function (cgReplaces) {
                if (this.isAllChecked(cgReplaces)) {
                    for (var i = 0; i < cgReplaces.length; i++) {
                        cgReplaces[i].is_selected = false;
                    }
                }
                else {
                    for (var i = 0; i < cgReplaces.length; i++) {
                        cgReplaces[i].is_selected = true;
                        cgReplaces[i].orig_user_id = this.userToBeReplaced.id;
                    }
                }
            };
            UserReplaceController.prototype.isAllChecked = function (cgReplaces) {
                var isSelectedAll = true;
                if (this.replaceEntities === null || cgReplaces === null) {
                    return false;
                }
                for (var j = 0; j < cgReplaces.length; j++) {
                    if (!cgReplaces[j].is_selected) {
                        isSelectedAll = false;
                        break;
                    }
                }
                return isSelectedAll;
            };
            UserReplaceController.prototype.toggleReplace = function (item) {
                if (item.is_selected) {
                    item.is_selected = false;
                }
                else {
                    item.is_selected = true;
                    item.orig_user_id = this.userToBeReplaced.id;
                }
            };
            UserReplaceController.prototype.isReplaceRequestorValid = function () {
                if (this.isValueNullOrUndefined(this.replaceRequestor)) {
                    this.replaceRequestorErrMsg = this.locEnterText + " " + this.locReplacerText;
                    return false;
                }
                return true;
            };
            UserReplaceController.prototype.isReplaceAppManValid = function () {
                if (this.isValueNullOrUndefined(this.replaceAppMan)) {
                    this.replaceAppManErrMsg = this.locEnterText + " " + this.locReplacerText;
                    return false;
                }
                return true;
            };
            UserReplaceController.prototype.isReplaceOrdererValid = function () {
                if (this.isValueNullOrUndefined(this.replaceOrderer)) {
                    this.replaceOrdererErrMsg = this.locEnterText + " " + this.locReplacerText;
                    return false;
                }
                return true;
            };
            UserReplaceController.prototype.isReplaceCentreManValid = function () {
                if (this.isValueNullOrUndefined(this.replaceCentreMan)) {
                    this.replaceCentreManErrMsg = this.locEnterText + " " + this.locReplacerText;
                    return false;
                }
                return true;
            };
            UserReplaceController.prototype.saveReplaceRequestor = function () {
                if (!this.isReplaceRequestorValid()) {
                    return;
                }
                this.saveReplaceRequestorToDb();
            };
            UserReplaceController.prototype.saveReplaceAppMan = function () {
                if (!this.isReplaceAppManValid()) {
                    return;
                }
                this.saveReplaceAppManToDb();
            };
            UserReplaceController.prototype.saveReplaceOrderer = function () {
                if (!this.isReplaceOrdererValid()) {
                    return;
                }
                this.saveReplaceOrdererToDb();
            };
            UserReplaceController.prototype.saveReplaceCentreMan = function () {
                if (!this.isReplaceCentreManValid()) {
                    return;
                }
                this.saveReplaceCentreManToDb();
            };
            UserReplaceController.prototype.hideLoaderWrapper = function () {
                if (this.isError || (this.isEntityDataLoaded
                    && this.isParticipantDataLoaded)) {
                    this.hideLoader();
                }
            };
            return UserReplaceController;
        }(RegetApp.BaseRegetTs));
        RegetApp.UserReplaceController = UserReplaceController;
        angular.
            module('RegetApp').
            controller('UserReplaceController', Kamsyk.RegetApp.UserReplaceController);
        var CentreGroupReplace = /** @class */ (function () {
            function CentreGroupReplace() {
                this.cg_id = null;
                this.name = null;
                this.is_selected = false;
                this.orig_user_id = null;
                this.replace_user_id = null;
                this.centres = null;
            }
            return CentreGroupReplace;
        }());
        RegetApp.CentreGroupReplace = CentreGroupReplace;
        var ReplaceEntity = /** @class */ (function () {
            function ReplaceEntity() {
                this.cg_app_men = null;
                this.cg_requestors = null;
                this.cg_orderers = null;
                this.centre_man = null;
            }
            return ReplaceEntity;
        }());
        RegetApp.ReplaceEntity = ReplaceEntity;
    })(RegetApp = Kamsyk.RegetApp || (Kamsyk.RegetApp = {}));
})(Kamsyk || (Kamsyk = {}));
//# sourceMappingURL=replace-user.js.map