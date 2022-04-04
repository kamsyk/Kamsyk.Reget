/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../../RegetTypeScript/Base/reget-common.ts" />
var Kamsyk;
(function (Kamsyk) {
    var RegetApp;
    (function (RegetApp) {
        var discussiondirective = /** @class */ (function () {
            function discussiondirective() {
                this.scope = {
                    discussionitem: "=",
                };
                this.templateUrl = RegetCommonTs.getRegetRootUrl() + "Content/Html/TsDirective/Discussion.html";
                this.link = function (scope, element, attrs, controller) {
                };
            }
            return discussiondirective;
        }());
        RegetApp.discussiondirective = discussiondirective;
        var DiscussionItem = /** @class */ (function () {
            function DiscussionItem() {
                this.id = null;
                this.disc_text = null;
            }
            return DiscussionItem;
        }());
        RegetApp.DiscussionItem = DiscussionItem;
        angular.module('RegetApp').directive("discussiondirective", [function () { return new discussiondirective(); }]);
    })(RegetApp = Kamsyk.RegetApp || (Kamsyk.RegetApp = {}));
})(Kamsyk || (Kamsyk = {}));
//# sourceMappingURL=reget-discussion.js.map