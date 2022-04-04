/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../../RegetTypeScript/Base/reget-common.ts" />
/// <reference path="../../typings/moment/moment.d.ts" />

module Kamsyk.RegetApp {
    export interface IRegetGridDateTime extends ng.IScope {
        id: string,
        ismandatory: true;
        placeholder: string;
        datetimeformat: string;
        options: any;
        //inputid: string;
    }
    
    export class griddatetimedirective implements ng.IDirective {
        restrict: "E";
        transclude: true;
        replace: true;
        require: 'ngModel';
                
        public scope: any = {
            id: "@",
            ismandatory: "@",
            placeholder: "@",
            datetimeformat: "@",
            ngModel: "=",
            datetimeerrmsg: "=",
            options: "="
            //inputid: "="
        };
        
        templateUrl = RegetCommonTs.getRegetRootUrl() + "Content/Html/TsDirective/AngGridDateTime.html";
        
        constructor() { 
        }

   
        public link: ng.IDirectiveLinkFn = (scope: IRegetGridDateTime, element: ng.IAugmentedJQuery, attrs: ng.IAttributes, controller: ng.IController): void => {
            scope.strDateTime;

            scope.dateTimeChanged = () => {
                var m = moment(scope.strDateTime, scope.datetimeformat, true);
                if (m.isValid()) {
                    scope.ngModel = m.toDate();
                } else {
                    scope.ngModel = null;
                }
                //alert(scope.ngModel);
            }

            scope.dirInit = () => {
                if (RegetCommonTs.isValueNullOrUndefined(scope.ngModel)) {
                } else {
                    scope.strDateTime = moment(scope.ngModel).format(scope.datetimeformat);
                    //scope.strDateTime = "datum";
                }
            }

            scope.dirInit();
        }
     
    }
        
    angular.module('RegetApp').directive("griddatetimedirective", [() => new griddatetimedirective()]);
}