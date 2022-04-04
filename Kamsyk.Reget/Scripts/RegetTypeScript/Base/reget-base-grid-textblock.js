//var isDateTimePicker: boolean;
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
        angular.module('RegetApp').directive("regettranslategrid", function () {
            return {
                restrict: "E",
                transclude: true,
                scope: {
                    //localtexts: "=",
                    text1: "=",
                    text2: "=",
                    //text3: "=",
                    translateerrormsg: "=",
                    isdefaultlang: "=",
                    label1: "@",
                    label2: "@",
                    //label3: "@",
                    flag1: "@",
                    flag2: "@",
                    //flag3: "@",
                    savetext: "@",
                    canceltext: "@",
                    mandatorytext: "@",
                    text1changed: "&",
                    text2changed: "&",
                    //text3changed: "&",
                    saveparentpgtrans: "&",
                    closeparentpgtrans: "&"
                },
                templateUrl: RegetCommonTs.getRegetRootUrl() + "Content/Html/AngGridTranslate.html"
            };
        });
        var BaseRegetGridTextBox = /** @class */ (function (_super) {
            __extends(BaseRegetGridTextBox, _super);
            //public modifLocalTexts: LocalText[] = null;
            //*****************************************************************
            //*****************************************************************
            //Constructors
            function BaseRegetGridTextBox($scope, $http, $filter, $mdDialog, $mdToast, $q, uiGridConstants, $timeout
            //protected $moment: moment.Moment
            ) {
                var _this = _super.call(this, $scope, $http, $filter, $mdDialog, $mdToast, $q, uiGridConstants, $timeout) || this;
                _this.$scope = $scope;
                _this.$http = $http;
                _this.$filter = $filter;
                _this.$mdDialog = $mdDialog;
                _this.$mdToast = $mdToast;
                _this.$q = $q;
                _this.uiGridConstants = uiGridConstants;
                _this.$timeout = $timeout;
                //*****************************************************************
                //Properties
                _this.translateLabel1 = null;
                _this.translateLabel2 = null;
                //public translateLabel3: string = null;
                _this.translateFlagUrl1 = null;
                _this.translateFlagUrl2 = null;
                //public translateFlagUrl3: string = null;
                _this.translateText1 = null;
                _this.translateText2 = null;
                //public translateText3: string = null;
                //public translateText1Modif: string = null;
                //public translateText2Modif: string = null;
                _this.entityId = null;
                _this.translateErrorMsg = null;
                _this.isDefaultLang = false;
                _this.dbGridId = null;
                _this.deleteUrl = null;
                _this.locTranslationText = null;
                _this.locMandatoryTextFieldText = null;
                return _this;
            }
            //********************************************************************
            //********************************************************************
            BaseRegetGridTextBox.prototype.saveGridRowtoDb = function (rowEntity) {
                var isSaved = _super.prototype.saveGridRowtoDb.call(this, rowEntity);
                if (isSaved) {
                    this.updateFirstLocalText(rowEntity);
                }
                return isSaved;
            };
            //*********************************************************************
            //*******************************************************************
            //Methods
            BaseRegetGridTextBox.prototype.displayGridTranslation = function (target) {
                this.removeFormReleativePosition();
                //this.translateText1Orig = this.translateText1;
                //this.translateText2Orig = this.translateText2;
                this.translateErrorMsg = null;
                var divRow = target;
                var divTranslation = document.getElementById("divtranslatedirective");
                var txt1LocErr = document.getElementById("txt1LocErr");
                var txt2LocErr = document.getElementById("txt2LocErr");
                //var txt3LocErr: HTMLDivElement = <HTMLDivElement>document.getElementById("txt3LocErr");
                txt1LocErr.style.display = "none";
                txt2LocErr.style.display = "none";
                //txt3LocErr.style.display = "none";
                var iScrollY = $(".reget-body").scrollTop() + window.scrollY;
                ;
                var iTop = iScrollY + Math.floor(divRow.getBoundingClientRect().top) + Math.floor(divRow.scrollHeight);
                var iScrollX = $(".reget-body").scrollLeft();
                var divCol = target.parentNode.parentNode.parentNode.parentNode.parentNode;
                var iScrollX = $(".reget-body").scrollLeft();
                var iLeft = iScrollX + Math.floor(divCol.getBoundingClientRect().left);
                divTranslation.style.top = iTop + "px";
                divTranslation.style.left = iLeft + "px";
                divTranslation.style.visibility = "visible";
                divTranslation.style.opacity = "1";
                var divRegetToolTip = document.getElementById("divRegetToolTip");
                var iTooltipLeft = iLeft + Math.floor(divTranslation.clientWidth / 2);
                divRegetToolTip.style.visibility = "visible";
                divRegetToolTip.style.opacity = "1";
                divRegetToolTip.style.top = iTop + "px";
                divRegetToolTip.style.left = iTooltipLeft + "px";
            };
            BaseRegetGridTextBox.prototype.localtext1changed = function (strValue) {
                if (this.isStringValueNullOrEmpty(strValue)) {
                    $("#txt1LocErr").slideDown("slow");
                    this.translateErrorMsg = this.locMissingMandatoryText;
                }
                else {
                    $("#txt1LocErr").slideUp("slow");
                    this.translateErrorMsg = null;
                }
                //var input = this.angScopeAny.frmParentPg['txtLang1'];
                //input.$error.required = true;
                //input.$setValidity("required", false);
                //var divError: HTMLDivElement = <HTMLDivElement>document.getElementById("txt1LocErr");
                //divError.style.display = "block";
            };
            //private localtext3changed(strValue: string): void {
            //    if (this.isStringValueNullOrEmpty(strValue)) {
            //        $("#txt3LocErr").slideDown("slow");
            //    } else {
            //        $("#txt3LocErr").slideUp("slow");
            //    }
            //}  
            BaseRegetGridTextBox.prototype.localtext2changed = function (strValue) {
                if (this.isStringValueNullOrEmpty(strValue)) {
                    $("#txt2LocErr").slideDown("slow");
                    this.translateErrorMsg = this.locMissingMandatoryText;
                }
                else {
                    $("#txt2LocErr").slideUp("slow");
                    this.translateErrorMsg = null;
                }
            };
            //public revetTranslations(): void {
            //    this.translateText1 = this.translateText1Orig;
            //    this.translateText2 = this.translateText2Orig;
            //}
            BaseRegetGridTextBox.prototype.isTranslationValid = function () {
                if (this.isValueNullOrUndefined(this.translateText1)) {
                    return false;
                }
                if (!this.isDefaultLang && this.isValueNullOrUndefined(this.translateText2)) {
                    return false;
                }
                return true;
            };
            BaseRegetGridTextBox.prototype.saveTranslation = function () {
                this.translateErrorMsg = null;
                if (!this.isTranslationValid()) {
                    this.displayErrorMsg(this.locMissingMandatoryText);
                    this.translateErrorMsg = this.locMissingMandatoryText;
                    return;
                }
                this.hideTranslation();
                this.saveTranslationToDb();
                //this.translateText1Orig = null;
                //this.translateText2Orig = null;
            };
            BaseRegetGridTextBox.prototype.closeTranslation = function () {
                // this.revetTranslations();
                this.hideTranslation();
            };
            BaseRegetGridTextBox.prototype.hideTranslation = function () {
                var divTranslation = document.getElementById("divtranslatedirective");
                if (this.isValueNullOrUndefined(divTranslation)) {
                    return;
                }
                divTranslation.style.visibility = "hidden";
                var divRegetToolTip = document.getElementById("divRegetToolTip");
                divRegetToolTip.style.opacity = "0";
                divRegetToolTip.style.visibility = "hidden";
                //this.translateText1Orig = null;
                //this.translateText2Orig = null;
            };
            return BaseRegetGridTextBox;
        }(RegetApp.BaseRegetGridTs));
        RegetApp.BaseRegetGridTextBox = BaseRegetGridTextBox;
    })(RegetApp = Kamsyk.RegetApp || (Kamsyk.RegetApp = {}));
})(Kamsyk || (Kamsyk = {}));
//# sourceMappingURL=reget-base-grid-textblock.js.map