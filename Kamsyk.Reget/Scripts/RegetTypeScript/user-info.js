/// <reference path="../RegetTypeScript/Base/reget-base.ts" />
/// <reference path="../RegetTypeScript/Base/reget-common.ts" />
/// <reference path="../RegetTypeScript/Base/reget-entity.ts" />
/// <reference path="../RegetTypeScript/replace-user.ts" />
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
        var UserInfoController = /** @class */ (function (_super) {
            __extends(UserInfoController, _super);
            //**********************************************************
            //Constructor
            function UserInfoController($scope, $http, $filter, $mdDialog, $mdToast, $q, $timeout) {
                var _this = _super.call(this, $scope, $http, $filter, $mdDialog, $mdToast, $q, $timeout) || this;
                _this.$scope = $scope;
                _this.$http = $http;
                _this.$filter = $filter;
                _this.$mdDialog = $mdDialog;
                _this.$mdToast = $mdToast;
                _this.$q = $q;
                _this.$timeout = $timeout;
                //***************************************************************
                _this.$onInit = function () { };
                return _this;
                //this.loadData();
            }
            return UserInfoController;
        }(RegetApp.UserReplaceController));
        RegetApp.UserInfoController = UserInfoController;
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
    })(RegetApp = Kamsyk.RegetApp || (Kamsyk.RegetApp = {}));
})(Kamsyk || (Kamsyk = {}));
//# sourceMappingURL=user-info.js.map