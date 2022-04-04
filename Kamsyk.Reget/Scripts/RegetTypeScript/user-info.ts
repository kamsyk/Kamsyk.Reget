/// <reference path="../RegetTypeScript/Base/reget-base.ts" />
/// <reference path="../RegetTypeScript/Base/reget-common.ts" />
/// <reference path="../RegetTypeScript/Base/reget-entity.ts" />
/// <reference path="../RegetTypeScript/replace-user.ts" />

module Kamsyk.RegetApp {
    export class UserInfoController extends UserReplaceController implements angular.IController {

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

            //this.loadData();

        }
        //***************************************************************

        $onInit = () => { };

    }

    angular.
        module('RegetApp').
        controller('UserInfoController', Kamsyk.RegetApp.UserInfoController);

    //angular.module('RegetApp').
    //    config(['$tooltipProvider', function ($tooltipProvider) {
    //        $tooltipProvider.options({
    //            appendToBody: true, // 
    //            placement: 'bottom' // Set Default Placement
    //        });
    //    }]);
        
    
}

