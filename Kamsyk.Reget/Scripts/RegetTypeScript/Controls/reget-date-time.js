/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../../RegetTypeScript/Base/reget-common.ts" />
/// <reference path="../../typings/moment/moment.d.ts" />
var Kamsyk;
(function (Kamsyk) {
    var RegetApp;
    (function (RegetApp) {
        var datetimedirective = /** @class */ (function () {
            function datetimedirective() {
                this.scope = {
                    id: "@",
                    ismandatory: "@",
                    label: "@",
                    placeholder: "@",
                    leftlabelcss: "@",
                    datetimeformat: "@",
                    dateerrmsgtext: "@",
                    controlindex: "@",
                    datetimerangeerrmsgtext: "=",
                    mindate: "=",
                    //maxdate: "=",
                    isreadonly: "=",
                    //israngeerror: "=",
                    ngModel: "=",
                    //datetimeerrmsg: "=",
                    options: "=",
                    ngcontroller: "=",
                    isvalid: "="
                    //datetimechanged: "&"
                    //ngChange: '&'
                    //iHour: "@"
                };
                this.templateUrl = RegetCommonTs.getRegetRootUrl() + "Content/Html/TsDirective/AngDateTime.html";
                this.link = function (scope, element, attrs, controller) {
                    scope.iHour = 0;
                    scope.iMinute = 0;
                    scope.isDateBlur = false;
                    scope.isHideError = false;
                    scope.rangeErrMsg = "Date is out of Range";
                    if (RegetCommonTs.isValueNullOrUndefined(scope.ngModel)) {
                        scope.iHour = 0;
                        scope.iMinute = 0;
                    }
                    else {
                        scope.iHour = scope.ngModel.getHours();
                        scope.iMinute = scope.ngModel.getMinutes();
                    }
                    scope.strHour = RegetCommonTs.convertIntegerToString(scope.iHour, 2);
                    scope.strMinute = RegetCommonTs.convertIntegerToString(scope.iMinute, 2);
                    if (scope.ismandatory && scope.label !== null) {
                        scope.labelMan = scope.label + " *";
                    }
                    else {
                        scope.labelMan = scope.label;
                    }
                    scope.minuteChanged = function () {
                        //scope.strMinute = $("#txtMinute" + scope.id).val();
                        if (RegetCommonTs.isStringValueNullOrEmpty(scope.strMinute)) {
                            scope.iMinute = 0;
                        }
                        else {
                            //scope.iMinute = Number(scope.strMinute);
                            if (!isNaN(scope.strMinute)) {
                                scope.iMinute = parseInt(scope.strMinute);
                            }
                            else {
                                scope.iMinute = 0;
                            }
                        }
                        scope.setMinute();
                    };
                    scope.hourChanged = function () {
                        //alert('hour');
                        if (RegetCommonTs.isStringValueNullOrEmpty(scope.strHour)) {
                            scope.iHour = 0;
                        }
                        else {
                            if (!isNaN(scope.strHour)) {
                                scope.iHour = parseInt(scope.strHour);
                            }
                            else {
                                scope.iHour = 0;
                            }
                        }
                        scope.setHour();
                    };
                    //scope.hourChangedJs = () => {
                    //    alert('hour');
                    //}
                    scope.minuteUp = function () {
                        if (RegetCommonTs.isValueNullOrUndefined(scope.iMinute)) {
                            scope.iMinute = 0;
                        }
                        scope.iMinute++;
                        scope.setMinute();
                    };
                    scope.minuteDown = function () {
                        if (RegetCommonTs.isValueNullOrUndefined(scope.iMinute)) {
                            scope.iMinute = 0;
                        }
                        scope.iMinute--;
                        scope.setMinute();
                    };
                    scope.setMinute = function () {
                        if (scope.iMinute > 59) {
                            scope.iMinute = 0;
                        }
                        if (scope.iMinute < 0) {
                            scope.iMinute = 59;
                        }
                        scope.strMinute = RegetCommonTs.convertIntegerToString(scope.iMinute, 2);
                        if (scope.isMinuteValid()) {
                            scope.setDateTime();
                        }
                    };
                    scope.hourUp = function () {
                        if (RegetCommonTs.isValueNullOrUndefined(scope.iHour)) {
                            scope.iHour = 0;
                        }
                        scope.iHour++;
                        scope.setHour();
                    };
                    scope.hourDown = function () {
                        if (RegetCommonTs.isValueNullOrUndefined(scope.iHour)) {
                            scope.iHour = 0;
                        }
                        scope.iHour--;
                        scope.setHour();
                    };
                    scope.setHour = function () {
                        if (scope.iHour > 23) {
                            scope.iHour = 0;
                        }
                        if (scope.iHour < 0) {
                            scope.iHour = 23;
                        }
                        scope.strHour = RegetCommonTs.convertIntegerToString(scope.iHour, 2);
                        if (scope.isHourValid()) {
                            scope.setDateTime();
                        }
                    };
                    scope.dateTimeBlur = function () {
                        scope.isDateBlur = true;
                        scope.isHideError = false;
                    };
                    scope.dateChanged = function () {
                        scope.setDateTime();
                    };
                    scope.raiseModifEvent = function () {
                        if (RegetCommonTs.isStringValueNullOrEmpty(scope.controlindex)) {
                            scope.controlindex = "0";
                        }
                        if (!RegetCommonTs.isValueNullOrUndefined(scope.ngcontroller)) {
                            scope.ngcontroller.dateTimeToChanged(scope.controlindex, scope.ngModel);
                        }
                    };
                    scope.isDateValid = function () {
                        if (scope.isHideError == true) {
                            return true;
                        }
                        var modelController = element.find('input').controller('ngModel');
                        var isDateValid = (!scope.isDateBlur || modelController.$valid);
                        //var isMinuteValid = (scope.iMinute >= 0 && scope.iMinute < 60);
                        //if (isDateValid === true) {
                        //    scope.isDateRangeValid();
                        //}
                        scope.isvalid = isDateValid;
                        return (isDateValid);
                        //return true;
                    };
                    scope.isDateRangeValid = function () {
                        scope.isvalid = scope.isDateValid();
                        if (scope.mindate == null || scope.isDateValid() == false) {
                            return true;
                        }
                        var tmpMomDate = scope.getDateTime();
                        if (tmpMomDate == null) {
                            return true;
                        }
                        var tmpDate = tmpMomDate.add(1, "minutes").toDate();
                        var tmpMinDate = scope.mindate;
                        if (tmpMinDate > tmpDate) {
                            scope.isvalid = false;
                            scope.rangeErrMsg = scope.getOutOfRangeErrMsg();
                            return false;
                        }
                        return true;
                    };
                    scope.isHourValid = function () {
                        var isHourValid = (scope.iHour >= 0 && scope.iHour < 24 && !RegetCommonTs.isStringValueNullOrEmpty(scope.strHour));
                        return (isHourValid);
                    };
                    scope.isMinuteValid = function () {
                        var isMinuteValid = (scope.iMinute >= 0 && scope.iMinute < 60 && !RegetCommonTs.isStringValueNullOrEmpty(scope.strMinute));
                        return (isMinuteValid);
                    };
                    scope.setDateTime = function () {
                        if (RegetCommonTs.isValueNullOrUndefined(scope.ngModel)) {
                            return;
                        }
                        var controller = element.find('input').controller('ngModel');
                        if (!controller.$valid) {
                            return;
                        }
                        var tmpDate = moment(scope.ngModel);
                        if (tmpDate.isValid() && !isNaN(scope.iHour) && !isNaN(scope.iMinute)) {
                            //var strDate = tmpDate.date() + '/' + (tmpDate.month() + 1) + '/' + tmpDate.year();
                            //strDate += " " + scope.strHour + ":" + scope.strMinute;
                            ////alert(strDate);
                            //tmpDate = moment(strDate, 'D/M/YYYY HH:mm', true);
                            tmpDate = scope.getDateTime();
                            if (tmpDate.isValid()) {
                                scope.ngModel = tmpDate;
                                scope.raiseModifEvent();
                            }
                            //alert(scope.ngModel);
                        }
                        //Do Not Delete this line
                        // scope.$emit('myCustomEvent', 'Data to send');
                    };
                    scope.getDateTime = function () {
                        if (scope.ngModel === null) {
                            return null;
                        }
                        var tmpDate = moment(scope.ngModel);
                        var strDate = tmpDate.date() + '/' + (tmpDate.month() + 1) + '/' + tmpDate.year();
                        strDate += " " + scope.strHour + ":" + scope.strMinute;
                        //alert(strDate);
                        tmpDate = moment(strDate, 'D/M/YYYY HH:mm', true);
                        return tmpDate;
                    };
                    scope.getDateTimeText = function () {
                        if (RegetCommonTs.isValueNullOrUndefined(scope.ngModel)) {
                            return "";
                        }
                        scope.setDateTimeFormat();
                        var momDate = moment(scope.ngModel);
                        var strDate = momDate.format(scope.datetimeformat);
                        return strDate;
                    };
                    scope.setDateTimeFormat = function () {
                        if (RegetCommonTs.isStringValueNullOrEmpty(scope.datetimeformat)) {
                            scope.datetimeformat = "D/M/YYYY HH:mm";
                        }
                    };
                    scope.getOutOfRangeErrMsg = function () {
                        if (!RegetCommonTs.isStringValueNullOrEmpty(scope.datetimerangeerrmsgtext)) {
                            var errMsg = scope.datetimerangeerrmsgtext;
                            if (!RegetCommonTs.isValueNullOrUndefined(scope.mindate)) {
                                scope.setDateTimeFormat();
                                var strDate = moment(scope.mindate).format(scope.datetimeformat);
                                errMsg = errMsg.replace("{0}", strDate);
                            }
                            return errMsg;
                        }
                        return "Date is out of Range";
                    };
                    angular.extend(scope.options, {
                        setDirTime: function (dateTime) {
                            //alert('ok');
                            scope.iHour = 0;
                            scope.iMinute = 0;
                            if (!RegetCommonTs.isValueNullOrUndefined(dateTime)) {
                                //    scope.strHour = "";
                                //    scope.strMinute = "";
                                //} else {
                                scope.iHour = dateTime.getHours();
                                scope.iMinute = dateTime.getMinutes();
                            }
                            scope.setHour();
                            scope.setMinute();
                            //scope.directiveCtrlCalled = true;
                        },
                        hideErrorMsg: function () {
                            scope.isHideError = true;
                        }
                    });
                    //scope.$watch(scope.ngModel, function (newValue, oldValue) {
                    //    alert('ok');
                    //}, true);  
                    //scope.$watch(scope.ngModel, function (value) {
                    //    if (value) {
                    //        alert(value);
                    //        //element[0].onchange();
                    //        //   element.trigger("change"); use this for jQuery
                    //    }
                    //});
                };
                //$scope.$watch(scope.ngModel, function (newValue, oldValue) {
                //    alert('ok');
                //}, true);         
            }
            return datetimedirective;
        }());
        RegetApp.datetimedirective = datetimedirective;
        //angular.module('RegetApp').directive("timedirective", timedirective.factory);
        angular.module('RegetApp').directive("datetimedirective", [function () { return new datetimedirective(); }]);
    })(RegetApp = Kamsyk.RegetApp || (Kamsyk.RegetApp = {}));
})(Kamsyk || (Kamsyk = {}));
//# sourceMappingURL=reget-date-time.js.map