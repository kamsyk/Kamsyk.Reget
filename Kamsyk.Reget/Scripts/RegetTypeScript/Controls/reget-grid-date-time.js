/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../../RegetTypeScript/Base/reget-common.ts" />
/// <reference path="../../typings/moment/moment.d.ts" />
var Kamsyk;
(function (Kamsyk) {
    var RegetApp;
    (function (RegetApp) {
        var griddatetimedirective = /** @class */ (function () {
            function griddatetimedirective() {
                this.scope = {
                    id: "@",
                    ismandatory: "@",
                    placeholder: "@",
                    datetimeformat: "@",
                    ngModel: "=",
                    datetimeerrmsg: "=",
                    options: "="
                    //inputid: "="
                };
                this.templateUrl = RegetCommonTs.getRegetRootUrl() + "Content/Html/TsDirective/AngGridDateTime.html";
                this.link = function (scope, element, attrs, controller) {
                    scope.strDateTime;
                    scope.dateTimeChanged = function () {
                        var m = moment(scope.strDateTime, scope.datetimeformat, true);
                        if (m.isValid()) {
                            scope.ngModel = m.toDate();
                        }
                        else {
                            scope.ngModel = null;
                        }
                        //alert(scope.ngModel);
                    };
                    scope.dirInit = function () {
                        if (RegetCommonTs.isValueNullOrUndefined(scope.ngModel)) {
                        }
                        else {
                            scope.strDateTime = moment(scope.ngModel).format(scope.datetimeformat);
                            //scope.strDateTime = "datum";
                        }
                    };
                    scope.dirInit();
                };
            }
            return griddatetimedirective;
        }());
        RegetApp.griddatetimedirective = griddatetimedirective;
        angular.module('RegetApp').directive("griddatetimedirective", [function () { return new griddatetimedirective(); }]);
    })(RegetApp = Kamsyk.RegetApp || (Kamsyk.RegetApp = {}));
})(Kamsyk || (Kamsyk = {}));
//# sourceMappingURL=reget-grid-date-time.js.map