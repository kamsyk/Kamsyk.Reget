/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../../RegetTypeScript/Base/reget-common.ts" />
/// <reference path="../../typings/moment/moment.d.ts" />

module Kamsyk.RegetApp {
    export interface IRegetDateTime extends ng.IScope {
        id: string,
        ismandatory: true;
        label: string;
        leftlabelcss: string;
        placeholder: string;
        datetimeformat: string;
        dateerrmsgtext: string;
        //datetimeerrmsg: string;
        datetimerangeerrmsgtext: string;
        mindate: Date;
        //maxdate: moment.Moment;
        isreadonly: boolean;
        //israngeerror: boolean;
        options: any; //must be here
        ngcontroller: any;
        controlindex: string;
        isvalid: boolean;
        //datetimechanged: any;
        //iHour: number;
        //iMinute: number;
        //isDateBlur: boolean;
    }
    
    export class datetimedirective implements ng.IDirective {
        restrict: "E";
        transclude: true;
        replace: true;
        require: 'ngModel';
        
                
        public scope: any = {
            id: "@",
            ismandatory: "@",
            label: "@",
            placeholder: "@",
            leftlabelcss: "@",
            datetimeformat: "@",
            dateerrmsgtext: "@",
            controlindex: "@",
            datetimerangeerrmsgtext: "=",
            mindate: "=",//must be here
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
        
        templateUrl = RegetCommonTs.getRegetRootUrl() + "Content/Html/TsDirective/AngDateTime.html";
        
        constructor() { 
            //$scope.$watch(scope.ngModel, function (newValue, oldValue) {
            //    alert('ok');
            //}, true);         
        }

   
        public link: ng.IDirectiveLinkFn = (scope: IRegetDateTime, element: ng.IAugmentedJQuery, attrs: ng.IAttributes, controller: ng.IController): void => {
            scope.iHour = 0;
            scope.iMinute = 0;
            scope.isDateBlur = false;
            scope.isHideError = false;
            scope.rangeErrMsg = "Date is out of Range";
            
            if (RegetCommonTs.isValueNullOrUndefined(scope.ngModel)) {
                scope.iHour = 0;
                scope.iMinute = 0;
            } else {
                scope.iHour = scope.ngModel.getHours();
                scope.iMinute = scope.ngModel.getMinutes();
            }

            scope.strHour = RegetCommonTs.convertIntegerToString(scope.iHour, 2);
            scope.strMinute = RegetCommonTs.convertIntegerToString(scope.iMinute, 2);
                        
            if (scope.ismandatory && scope.label !== null) {
                scope.labelMan = scope.label + " *";
            } else {
                scope.labelMan = scope.label;
            }
                        
            scope.minuteChanged = () => {
                //scope.strMinute = $("#txtMinute" + scope.id).val();

                if (RegetCommonTs.isStringValueNullOrEmpty(scope.strMinute)) {
                    scope.iMinute = 0;
                } else {

                    //scope.iMinute = Number(scope.strMinute);
                    if (!isNaN(scope.strMinute)) {
                        scope.iMinute = parseInt(scope.strMinute);
                    } else {
                        scope.iMinute = 0;
                    }
                }

                scope.setMinute();
            }

            scope.hourChanged = () => {
                //alert('hour');
                if (RegetCommonTs.isStringValueNullOrEmpty(scope.strHour)) {
                    scope.iHour = 0;
                } else {
                    
                    if (!isNaN(scope.strHour)) {
                        scope.iHour = parseInt(scope.strHour);
                    } else {
                        scope.iHour = 0;
                    }
                }

                scope.setHour();
            }

            //scope.hourChangedJs = () => {
            //    alert('hour');
            //}

            scope.minuteUp = () => {
                if (RegetCommonTs.isValueNullOrUndefined(scope.iMinute)) {
                    scope.iMinute = 0;
                }
                scope.iMinute++;

                scope.setMinute();
                
            }

            scope.minuteDown = () => {
                if (RegetCommonTs.isValueNullOrUndefined(scope.iMinute)) {
                    scope.iMinute = 0;
                }
                scope.iMinute--;

                scope.setMinute();

            }

            scope.setMinute = () => {
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
            }

            scope.hourUp = () => {
                if (RegetCommonTs.isValueNullOrUndefined(scope.iHour)) {
                    scope.iHour = 0;
                }
                scope.iHour++;

                scope.setHour();

            }

            scope.hourDown = () => {
                if (RegetCommonTs.isValueNullOrUndefined(scope.iHour)) {
                    scope.iHour = 0;
                }
                scope.iHour--;

                scope.setHour();

            }

            scope.setHour = () => {
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
            }

            scope.dateTimeBlur = () => {
                scope.isDateBlur = true;
                scope.isHideError = false;
            }

            scope.dateChanged = () => {
                scope.setDateTime();
                
            }

            scope.raiseModifEvent = (): void => {
                if (RegetCommonTs.isStringValueNullOrEmpty(scope.controlindex)) {
                    scope.controlindex = "0";
                }
                if (!RegetCommonTs.isValueNullOrUndefined(scope.ngcontroller)) {
                    scope.ngcontroller.dateTimeToChanged(scope.controlindex, scope.ngModel);
                }
            }
                        
            scope.isDateValid = (): boolean => {
               
                if (scope.isHideError == true) {
                    return true;
                }

                let modelController = element.find('input').controller('ngModel');
                let isDateValid : boolean = (!scope.isDateBlur || modelController.$valid);
                //var isMinuteValid = (scope.iMinute >= 0 && scope.iMinute < 60);

                //if (isDateValid === true) {
                //    scope.isDateRangeValid();
                //}

                scope.isvalid = isDateValid;
                                
                return (isDateValid);
                //return true;
            }

            scope.isDateRangeValid = (): boolean => {
                scope.isvalid = scope.isDateValid();

                if (scope.mindate == null || scope.isDateValid() == false) {
                    return true;
                }
                let tmpMomDate: moment.Moment = scope.getDateTime();
                if (tmpMomDate == null) {
                    return true;
                }

                let tmpDate: Date = tmpMomDate.add(1, "minutes").toDate();
                let tmpMinDate: Date = scope.mindate;
                if (tmpMinDate > tmpDate) {
                    scope.isvalid = false;
                    scope.rangeErrMsg = scope.getOutOfRangeErrMsg();
                    return false;
                }

                return true;
            }

            scope.isHourValid = (): boolean => {
                var isHourValid = (scope.iHour >= 0 && scope.iHour < 24 && !RegetCommonTs.isStringValueNullOrEmpty(scope.strHour));
                return (isHourValid);
            }

            scope.isMinuteValid = (): boolean => {
                var isMinuteValid = (scope.iMinute >= 0 && scope.iMinute < 60 && !RegetCommonTs.isStringValueNullOrEmpty(scope.strMinute));
                return (isMinuteValid);
            }

            scope.setDateTime = (): void => {
                if (RegetCommonTs.isValueNullOrUndefined(scope.ngModel)) {
                    return;
                }

                let controller = element.find('input').controller('ngModel');
                if (!controller.$valid) {
                    return;
                }

                let tmpDate: moment.Moment = moment(scope.ngModel);
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
                
            }

            scope.getDateTime = (): moment.Moment => {
                if (scope.ngModel === null) {
                    return null;
                }

                let tmpDate: moment.Moment = moment(scope.ngModel);
                let strDate: string = tmpDate.date() + '/' + (tmpDate.month() + 1) + '/' + tmpDate.year();
                strDate += " " + scope.strHour + ":" + scope.strMinute;
                //alert(strDate);
                tmpDate = moment(strDate, 'D/M/YYYY HH:mm', true);

                return tmpDate;
            }

            scope.getDateTimeText = (): string => {
                if (RegetCommonTs.isValueNullOrUndefined(scope.ngModel)) {
                    return "";
                }

                scope.setDateTimeFormat();
                
                let momDate: moment.Moment = moment(scope.ngModel);
                let strDate: string = momDate.format(scope.datetimeformat);

                return strDate;
            }

            scope.setDateTimeFormat = (): void => {
                if (RegetCommonTs.isStringValueNullOrEmpty(scope.datetimeformat)) {
                    scope.datetimeformat = "D/M/YYYY HH:mm";
                }
            }

            scope.getOutOfRangeErrMsg = (): string => {
                if (!RegetCommonTs.isStringValueNullOrEmpty(scope.datetimerangeerrmsgtext)) {
                    let errMsg: string = scope.datetimerangeerrmsgtext;
                    if (!RegetCommonTs.isValueNullOrUndefined(scope.mindate)) {
                        scope.setDateTimeFormat();
                        let strDate: string = moment(scope.mindate).format(scope.datetimeformat);
                        errMsg = errMsg.replace("{0}", strDate);
                    }

                    return errMsg;
                }

                return "Date is out of Range";
            }

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
        }

       

        //public getRegetRootUrl(): string {
        //    var rootUrl = window.location.protocol + "//" + window.location.host;

        //    if (window.location.host.toLowerCase().indexOf('localhost') < 0) {
        //        var appName = window.location.pathname.split('/')[1];

        //        if (appName.substr(0) !== "/") {
        //            rootUrl += "/";
        //        }
        //        rootUrl += appName;
        //    }

        //    var lastChar = rootUrl.substr(rootUrl.length - 1);
        //    if (lastChar !== "/") {
        //        rootUrl += "/";
        //    }
            
        //    return rootUrl;
        //}

        //static factory(): ng.IDirectiveFactory {
        //    let directive: ng.IDirectiveFactory = () => new timedirective();
        //    return directive;
        //}
        
    }
    
    //angular.module('RegetApp').directive("timedirective", timedirective.factory);
    angular.module('RegetApp').directive("datetimedirective", [() => new datetimedirective()]);
}