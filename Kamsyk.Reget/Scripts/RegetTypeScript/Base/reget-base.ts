/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../../typings/kamsyk-material/angular-material.d.ts" />
/// <reference path="../../typings/moment/moment.d.ts" />
/// <reference path="../../RegetTypeScript/Base/reget-common.ts" />


//var isDateTimePicker: boolean;
//var dateTimeWa: Date;


//import moo = module('moo');

module Kamsyk.RegetApp {
    
    var app = angular.module('RegetApp', ['ngMaterial', 'ngMessages']);
    //var app = ['$http', '$mdDialog', '$mdToast', 'ui.grid', 'ui.grid.pagination', 'ui.grid.resizeColumns', 'ui.grid.selection', 'ui.grid.moveColumns', 'ui.grid.edit',];

    //app.config(['$tooltipProvider', function ($tooltipProvider) {
    //    $tooltipProvider.options({
    //        appendToBody: true, // 
    //        placement: 'bottom' // Set Default Placement
    //    });
    //}]);

    function dialogController($scope, $mdDialog) {

        $scope.closeDialog = function () {
            $mdDialog.hide();
        }

        $scope.confirmDialog = function () {
            $mdDialog.hide();
        }
    }

    

    function convertDateTimeToString(dateVal) {
        return dateVal ? moment(dateVal).format($("#DateTimePickerFormatMoment").val()) : '';
    }

    function convertDateToString(dateVal) {
        return dateVal ? moment(dateVal).format($("#DatePickerFormatMoment").val()) : '';
    }

    $(document).ready(function () {
        //var tmp = document.getElementById("txtUserSubstituted");
        //tmp.setCustomValidity("This email is already registered!");
        //alert('ok');
        setMandatoryCustomValidity();
    });

    function setMandatoryCustomValidity() {
        try {
            var inputs: any = document.getElementsByClassName("reget-custom-loc-valid-text");
            if (inputs === null) {
                return;
            }

            for (var i = 0; i < inputs.length; i++) {
                var strId = inputs[i].id;
                var elId = strId.replace("custLocV_", "");
                var elMandat: any = document.getElementById(elId);
                elMandat.setCustomValidity(inputs[i].value);
            }
        } catch { }   
    }
        
    export abstract class BaseRegetTs {
        //protected $inject = [/*'$scope',*/ '$http', '$mdDialog', '$mdToast'];
        public angScopeAny: any;
        public angSceAny: any;

        protected isError: boolean = false;
        protected isSkipLoad: boolean = false;

        public urlParamDelimiter : string = "|";
        public urlParamValueDelimiter: string = "~";
        public urlFromFilterDelimiter: string = "from:";
        public urlToFilterDelimiter: string = "to:";
   
        public filterDateFormat = "M/D/YYYY";
        public participants: Participant[] = null;
        //private participantSearchKey: string = null;

        protected isUploadFileProgressBarVisible: boolean = false;
        protected progressPercentage: number = null;

        //*********************************************************************
        //local texts
        public locCloseText: string = ($("#CloseText").val()); 
        public locErrorTitleText: string = ($("#WarningText").val()); 
        public locErrorMsgText: string = ($("#ErrorOccuredText").val()); 
        public locDatePickerFormatMomentText: string = ($("#DatePickerFormatMomentText").val());
        public locDateTimePickerFormatMomentText: string = ($("#DateTimePickerFormatMomentText").val());

        public locEnterDateText: string = ($("#EnterDateText").val()); 
        public locEnterDateTimeText: string = ($("#EnterDateTimeText").val()); 
        public locEditText: string = $("#EditText").val();
        public locDeleteText: string = $("#DeleteText").val();
        public locSaveText: string = $("#SaveText").val();
        public locCancelText: string = $("#CancelText").val();
        public locDataCannotBeSavedText: string = $("#DataCannotBeSavedText").val();
        public locDataWasSavedText: string = $("#DataWasSavedText").val();
        public locLoadingDataText: string = $("#LoadingDataText").val();
        private locHideText: string = $("#HideText").val();
        protected locDisplayText: string = $("#DisplayText").val();
        private locMsgNotAuthorizedPerformActionText: string = this.getLocalizeText("MsgNotAuthorizedPerformActionText");
        private locMsgMissingMandatoryFieldText: string = $("#MsgMissingMandatoryFieldText").val();
        private locMsgRequestCannotBeRevertedText: string = $("#MsgRequestCannotBeRevertedText").val();
        private locDateTimeFormatText: string = $("#DateTimeFormatText").val();
        private locDateFormatText: string = $("#DateFormatText").val();
        protected ĺocFileSizeOverLimitText: string = $("#FileSizeOverLimitText").val();

        public locYesText: string = $("#YesText").val();
        public locNoText: string = $("#NoText").val();
        //**********************************************************************************

        protected currentUserName: string = $("#txtCurrentUserName").val();
        protected currentUserId: number = parseInt($("#txtCurrentUserId").val());
        protected currentUserPhotoUrl: string = $("#txtCurrentUserPhotoUrl").val();
        protected dateTimeMomentFormatText = $("#DateTimeMomentFormatText").val();

        //*****************************************************************
        //Constructors
        constructor(
            protected $scope: ng.IScope,
            protected $http: ng.IHttpService,
            protected $filter: ng.IFilterService,
            protected $mdDialog: angular.material.IDialogService,
            protected $mdToast: angular.material.IToastService,
            protected $q: ng.IQService,
            protected $timeout: ng.ITimeoutService,
            //protected $sce: ng.ISCEProvider
            //protected $moment: moment.Moment
        ) {
            //allows to uses $scope out of typed limitations
            this.angScopeAny = $scope;
            //this.angSceAny = $sce;

            for (var i: number = 0; i < this.defaultDiacriticsRemovalMap.length; i++) {
                var letters = this.defaultDiacriticsRemovalMap[i].letters;
                for (var j = 0; j < letters.length; j++) {
                    this.diacriticsMap[letters[j]] = this.defaultDiacriticsRemovalMap[i].base;
                }
            }
        }
        //******************************************************************

        //********************************************************************
        //abstract methods
        //abstract handlerResponded(key: string, response: any, params: any): any;
        //abstract errorCallback(reason: any): any;
        //abstract getCloseText(): string;
        //abstract getErrorTitleText(): string;
        //abstract getErrorMsgText(): string;
        //********************************************************************

        //public datePickerChange(isTime: number): void {
        //    isDateTimePicker = (isTime == 1);
        //}

        //public datePickerChange(): void {
            
        //}

        //public datePickerBlur(): void {
        //    //isDateTimePicker = (isTime == 1);

        //    //if (isDateTimePicker) {
        //    //    //var frm : any = document.getElementById(frmName);
        //    //    //this.angScopeAny.frmUserSubstitution.dtpToDate.setError({ 'firstError': null });
        //    //    ////this.angScopeAny.frmUserSubstitution.dtpToDate.setError({ 'firstError': null });

        //    //    //this.angScopeAny.frmUserSubstitution.dtpToDate.updateValueAndValidity();
        //    //} 
        //    //var mdDateInput: HTMLInputElement = <HTMLInputElement>document.getElementById("msg" + mdDateId);
        //    //mdDateInput.style.display = "none";

        //    //var mdDateInput: HTMLInputElement = angular.element('#appBusyIndicator');
        //    //var mdDateInput: HTMLInputElement = <HTMLInputElement>document.getElementById(mdDateId);
        //    //dateModel = new Date();
        //    this.angScopeAny.toDate = new Date();
        //}

        //public useGetHandler(handlerUrl: string, key: string, params?: any): ng.IPromise<any> {
        //    try {
        //        var result: ng.IPromise<any> = this.$http.get(handlerUrl, params)
        //            .then((response: any): ng.IPromise<any> => this.handlerResponded(key, response, params),
        //            errorCallback => this.errorCallback(key));


        //        return result;
        //    } catch (ex) {
        //        this.hideLoader();
        //        this.displayErrorMsg();
        //    }
        //}

        //public usePostHandler(handlerUrl: string, key: string, params: any): ng.IPromise<any> {
        //    var result: ng.IPromise<any> = this.$http.post(handlerUrl, params)
        //        .then((response: any): ng.IPromise<any> => this.handlerResponded(key, response, params),
        //        this.errorCallback(key));
        //    return result;
        //}

        public getRegetRootUrl(): string {
            return RegetCommonTs.getRegetRootUrl();
        }

        public showLoader(isError?: boolean): void {
            if (this.isValueNullOrUndefined(isError) || isError === false) {
                $(".reget-se-pre-con").show();
            }
        }

        public showLoaderBoxOnly(isError?: boolean): void {
            if (this.isValueNullOrUndefined(isError) || isError === false) {
                $(".reget-se-pre-con-box-only").show();
            }
        }

        public hideLoader(): void {
                $(".reget-se-pre-con").fadeOut();
                $(".reget-se-pre-con-box-only").fadeOut();
        }

        public displayErrorMsg(errMsg?: string, urlLink?: string): void {
            let dispErrMsg : string = this.isValueNullOrUndefined(errMsg) ? this.locErrorMsgText : errMsg;
            let tempUrlLink : string = "";
            if (!this.isStringValueNullOrEmpty(urlLink)) {
                tempUrlLink = "<tr><td></td><td>" + urlLink + "</td></tr>";
            }

            this.$mdDialog.show(
                {
                    //template: GetRegetRootUrl() + 'Content/Html/DialogError.html'
                    template:
                    '<md-dialog aria-label="Error dialog">' +
                    '  <md-dialog-content class="md-dialog-content">' +
                    '    <table>' +
                    '        <tr>' +
                    '            <td>' +
                    '                <img src="' + this.getRegetRootUrl() + 'Content/Images/Error.png' + '" class="reget-error-img">' +
                    '            </td>' +
                    '            <td>' +
                    '                <h2 class="md-title ng-binding">' + this.locErrorTitleText + '</h2>' +
                    '                <p id=\"pErrMsg\" class="ng-binding">' + dispErrMsg + '</p>' +
                    '            </td>' +
                    '        </tr>' + tempUrlLink +
                    '    <table>' +
                    '  </md-dialog-content>' +
                    '  <md-dialog-actions>' +
                    '    <md-button id="btnErrClose" class="md-primary" ng-click="closeDialog()">' +
                    this.locCloseText +
                    '    </md-button>' +
                    '  </md-dialog-actions>' +
                    '</md-dialog>',
                    locals: {

                    },
                    controller: dialogController
                });


            this.isError = true;

        }

        //public displayConfirmDialog(
        //    confirmText: string,
        //    confirmTitleText: string,
        //    deleteFunct: string
        //) {

        //    this.$mdDialog.show(
        //        {
        //            template:
        //                '<md-dialog aria-label="Error dialog">' +
        //                '  <md-dialog-content class="md-dialog-content">' +
        //                '    <table>' +
        //                '        <tr>' +
        //                '            <td>' +
        //                '                <img src="' + this.getRegetRootUrl() + 'Content/Images/Error.png' + '" class="reget-error-img">' +
        //                '            </td>' +
        //                '            <td>' +
        //                '                <h2 class="md-title ng-binding">' + confirmTitleText + '</h2>' +
        //                '                <p id=\"pErrMsg\" class="ng-binding">' + confirmText + '</p>' +
        //                '            </td>' +
        //                '        </tr>' +
        //                '    <table>' +
        //                '  </md-dialog-content>' +
        //                '  <md-dialog-actions>' +
        //                '    <md-button id="btnPerform" class="md-primary" ng-click="' + deleteFunct + '">' + this.locCloseText +
        //                '    </md-button>' +
        //                //'    <md-button id="btnErrClose" class="md-primary" ng-click="' + deleteFunct + '">' + this.locCloseText +
        //                //'    </md-button>' +
        //                '  </md-dialog-actions>' +
        //                '</md-dialog>',
        //            locals: {

        //            },
        //            controller: this.dialogConfirmController
        //        });
        //}

        protected getConfirmDialogTemplate(
            confirmText: string,
            confirmTitleText: string,
            actionBtnText: string,
            closeButtonText: string,
            doFunct: string,
            closeFunct: string
        ): string {
            let strTempl: string = '<md-dialog aria-label="Error dialog">' +
                '  <md-dialog-content class="md-dialog-content">' +
                '    <table>' +
                '        <tr>' +
                '            <td>' +
                '                <img src="' + this.getRegetRootUrl() + 'Content/Images/QuestionMark.png' + '" class="reget-error-img">' +
                '            </td>' +
                '            <td>' +
                '                <h2 class="md-title ng-binding">' + confirmTitleText + '</h2>' +
                '                <p id=\"pErrMsg\" class="ng-binding">' + confirmText + '</p>' +
                '            </td>' +
                '        </tr>' +
                '    <table>' +
                '  </md-dialog-content>' +
                '  <md-dialog-actions>' +
                '    <md-button id="btnPerform" class="md-primary" ng-click="' + doFunct + '">' + actionBtnText +
                '    </md-button>' +
                '    <md-button id="btnErrClose" class="md-primary" ng-click="' + closeFunct + '">' + closeButtonText +
                '    </md-button>' +
                '  </md-dialog-actions>' +
                '</md-dialog>';

            return strTempl;
        }

        private getWarningDialogTemplate(
            warningText: string,
            warningTitleText: string,
            closeButtonText: string,
            closeFunct: string
        ): string {
            let strTempl: string = '<md-dialog aria-label="Error dialog">' +
                '  <md-dialog-content class="md-dialog-content">' +
                '    <table>' +
                '        <tr>' +
                '            <td>' +
                '                <img src="' + this.getRegetRootUrl() + 'Content/Images/Error.png' + '" class="reget-error-img">' +
                '            </td>' +
                '            <td>' +
                '                <h2 class="md-title ng-binding">' + warningTitleText + '</h2>' +
                '                <p id=\"pErrMsg\" class="ng-binding">' + warningText + '</p>' +
                '            </td>' +
                '        </tr>' +
                '    <table>' +
                '  </md-dialog-content>' +
                '  <md-dialog-actions>' +
                '    <md-button id="btnErrClose" class="md-primary" ng-click="' + closeFunct + '">' + closeButtonText +
                '    </md-button>' +
                '  </md-dialog-actions>' +
                '</md-dialog>';

            return strTempl;
        }

        private getInfoDialogTemplate(
            infoText: string,
            infoTitleText: string,
            closeButtonText: string,
            closeFunct: string
        ): string {
            let strTempl: string = '<md-dialog aria-label="Error dialog">' +
                '  <md-dialog-content class="md-dialog-content">' +
                '    <table>' +
                '        <tr>' +
                '            <td>' +
                '                <img src="' + this.getRegetRootUrl() + 'Content/Images/Info.png' + '" class="reget-error-img">' +
                '            </td>' +
                '            <td>' +
                '                <h2 class="md-title ng-binding">' + infoTitleText + '</h2>' +
                '                <p id=\"pErrMsg\" class="ng-binding">' + infoText + '</p>' +
                '            </td>' +
                '        </tr>' +
                '    <table>' +
                '  </md-dialog-content>' +
                '  <md-dialog-actions>' +
                '    <md-button id="btnErrClose" class="md-primary" ng-click="' + closeFunct + '">' + closeButtonText +
                '    </md-button>' +
                '  </md-dialog-actions>' +
                '</md-dialog>';

            return strTempl;
        }

        //private dialogConfirmController($scope, $mdDialog): void {

        //    $scope.closeDialog = function () {
        //        $mdDialog.hide();
        //    }

        //    $scope.confirmDialog = function () {
        //        $mdDialog.hide();
        //    }
        //}

        public showAlert(strTitle: string, strMsg: string, strCloseButtonLabel: string): void {
            let strClose: string = strCloseButtonLabel;
            if (this.isValueNullOrUndefined(strCloseButtonLabel)) {
                strClose = this.locCloseText;
            }

            this.$mdDialog.show(
                {
                    template: this.getWarningDialogTemplate(
                        strMsg,
                        strTitle,
                        strClose,
                        "closeDialog()"),
                    controller: this.dialogAlertController
                });

            //this.$mdDialog.show(
            //    this.$mdDialog.alert()
            //        .clickOutsideToClose(true)
            //        .title(strTitle)
            //        .textContent(strMsg)
            //        .ariaLabel('Alert Dialog')
            //        .ok(strCloseButtonLabel)
                    
            //);

        }

        public showInfo(strTitle: string, strMsg: string, strCloseButtonLabel: string): void {
            let strClose: string = strCloseButtonLabel;
            if (this.isValueNullOrUndefined(strCloseButtonLabel)) {
                strClose = this.locCloseText;
            }

            this.$mdDialog.show(
                {
                    template: this.getInfoDialogTemplate(
                        strMsg,
                        strTitle,
                        strClose,
                        "closeDialog()"),
                    controller: this.dialogAlertController
                });
                        
        }

        private dialogAlertController($scope, $mdDialog): void {

            $scope.closeDialog = function () {
                $mdDialog.hide();
            }

           
        }

        //public showErrorAlert(strTitle: string, strMsg: string, strCloseButtonLabel: string) : void {

        //    this.$mdDialog.show(
        //        this.$mdDialog.alert()
        //            .clickOutsideToClose(true)
        //            .title(strTitle)
        //            .textContent(strMsg)
        //            .ariaLabel('Alert Dialog')
        //            .ok(strCloseButtonLabel)
        //            .template("aweqfqwerqw")
        //            // You can specify either sting with query selector
        //            //.openFrom('#left')
        //            // or an element
        //            //.closeTo(angular.element(document.querySelector('#right')))
        //    );

        //}

        public debounce(func: any, wait: number, immediate?: any): any {
            var timeout;
            return function () {
                var context = this, args = arguments;
                var later = function () {
                    timeout = null;
                    if (!immediate) func.apply(context, args);
                };
                var callNow = immediate && !timeout;
                clearTimeout(timeout);
                timeout = setTimeout(later, wait);
                if (callNow) func.apply(context, args);
            }
        }

        public isValueNullOrUndefined(value: any) {
            return RegetCommonTs.isValueNullOrUndefined(value);
            
        }

        public isStringValueNullOrEmpty(strValue: string) {
            return RegetCommonTs.isStringValueNullOrEmpty(strValue);
            
        }

        public getDateTimeString(dateVal: Date): any {
            return convertDateTimeToString(dateVal); 
            
            //return "date";
        }

        public getDateString(dateVal: Date): any {
            return convertDateToString(dateVal);
        }

        public convertJsonDate(jsonDate: any): moment.Moment {
            if (this.isValueNullOrUndefined(jsonDate)) {
                return null;
            }
                        
            var m: moment.Moment = moment(jsonDate);

            return m;
        }

        public convertJsonDateToJs(jsonDate: any): Date {
            if (this.isValueNullOrUndefined(jsonDate)) {
                return null;
            }

            let parsedDate : Date = new Date(parseInt(jsonDate.substr(6)));
            //var jsDate = new Date(parsedDate); //Date object
            return parsedDate;
        }

        public isMomentDatesSame(date1: moment.Moment | Date, date2: moment.Moment | Date): boolean {
            try {
                if (this.isValueNullOrUndefined(date1) && this.isValueNullOrUndefined(date2)) {
                    return true;
                }

                if (this.isValueNullOrUndefined(date1) && !this.isValueNullOrUndefined(date2)) {
                    return false;
                }

                if (!this.isValueNullOrUndefined(date1) && this.isValueNullOrUndefined(date2)) {
                    return false;
                }

                //if (date1.toDate().getTime() == date2.toDate().getTime()) {
                //    return true;
                //}

                return (moment(date1).isSame(date2));

                //return false;
            } catch(e) {
                return false;
            }
        }

        
        private searchParticipants(name: string): Participant[] {
            return this.searchParticipantsFromArray(name, this.participants);
            
        }

        public searchParticipantsFromArray(name: string, participantsArray: Participant[]): Participant[] {
            var searchParticipants: Participant[] = [];

            if (this.isStringValueNullOrEmpty(name)) {
                return this.participants;
            }

            angular.forEach(participantsArray, (participant: any) => {
                var partWoDia: string = this.removeDiacritics(name.trim()).toLowerCase();
                if (!this.isStringValueNullOrEmpty(participant.surname) && participant.surname.toLowerCase().indexOf(name.toLowerCase()) > -1 ||
                    !this.isStringValueNullOrEmpty(participant.first_name) && participant.first_name.toLowerCase().indexOf(name.toLowerCase()) > -1 ||
                    !this.isStringValueNullOrEmpty(participant.user_search_key) && participant.user_search_key.trim().toLowerCase().indexOf(partWoDia) > -1) {
                    searchParticipants.push(participant);
                }

            });
            return searchParticipants;
        }

        private isPartCanBeFiltered: boolean = false;

        

        public filterParticipants(searchText: string): ng.IPromise<Participant[]> {
            
            var deferred: ng.IDeferred<Participant[]> = this.$q.defer<Participant[]>();

            this.$http.get(
                this.getRegetRootUrl() + '/RegetAdmin/GetActiveParticipantsDataByName?name=' + encodeURI(searchText)
                + '&t=' + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    var participants: Participant[] = tmpData;
                    
                    //console.log(participants.length);
                    return deferred.resolve(participants);
                    //}
                } catch (e) {
                    this.hideLoader();
                    this.displayErrorMsg();
                } finally {
                    this.hideLoader();
                }
            }, (response: any) => {
                //deferred.resolve($scope.Suppliers);
                this.hideLoader();
                this.displayErrorMsg();
            });
            // }

            return deferred.promise;
        }

        public filterAddresses(searchText: string, companyName: string): ng.IPromise<Address[]> {

            var deferred: ng.IDeferred<Address[]> = this.$q.defer<Address[]>();

            this.$http.get(
                this.getRegetRootUrl() + "/Address/GetActiveAddressesDataByText?addressText=" + encodeURI(searchText) + "&companyName=" + encodeURI(companyName)
                + "&t=" + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    var addresses: Address[] = tmpData;

                    return deferred.resolve(addresses);
                    
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
           
            return deferred.promise;
        }

        public filterSuppliers(searchText: string, centreId: number): ng.IPromise<Supplier[]> {

            var deferred: ng.IDeferred<Supplier[]> = this.$q.defer<Supplier[]>();

            this.$http.get(
                this.getRegetRootUrl() + "/Supplier/GetActiveSuppliersByNameIdJs?searchText=" + encodeURI(searchText)
                + "&centreId=" + centreId
                + "&t=" + new Date().getTime(),
                {}
            ).then((response) => {
                try {
                    var tmpData: any = response.data;
                    var suppliers: Supplier[] = tmpData;

                    return deferred.resolve(suppliers);

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

            return deferred.promise;
        }

        
        public checkUserMandatory(selectedUser: Participant, strErrorMsgId: string): void {
            if (this.isStringValueNullOrEmpty(strErrorMsgId)) {
                return;
            }

            if (this.isValueNullOrUndefined(selectedUser)) {
                $("#" + strErrorMsgId).show("slow");
            } else {
                $("#" + strErrorMsgId).slideUp("slow");
            }
        }

        public checkUserMandatorySelString(selectedUser: string, strErrorMsgId: string): void {
            if (this.isStringValueNullOrEmpty(strErrorMsgId)) {
                return;
            }

            if (this.isStringValueNullOrEmpty(selectedUser)) {
                $("#" + strErrorMsgId).show("slow");
            } else {
                $("#" + strErrorMsgId).slideUp("slow");
            }
        }

        public showToast(strText : string) : void {
            this.$mdToast.show(
                this.$mdToast.simple()
                    .textContent(strText)
                    //.position('top right')
                    .theme("success-toast"));

            this.removeFormReleativePosition();
        }

        //showtoast set form position=relatiove, and if there is then displayed an element (e.g. DIV - translations, htmltext area user substitution page) on absolute position
        //it is displayed on wrong position after toas is displayed
        //this is a workaround - removes the style.postion = "relative" which is set by showtoast methods
        public removeFormReleativePosition() {
            let froms: HTMLCollectionOf<HTMLFormElement> = document.getElementsByTagName("form");
            if (!this.isValueNullOrUndefined(froms) && froms.length > 0) {
                for (var i: number = 0; i < froms.length; i++) {
                    froms[i].style.position = "";
                }
            }
        }
        
        public isSmartPhone(): boolean {
            var winWidth = window.innerWidth;
            if (winWidth < 767) {
                return true;
            } else {
                return false;
            }
            
        }

        public displayElement(elementId: string) : void {
            $("#" + elementId).show();
        }

        public hideElement(elementId : string) : void {
            $("#" + elementId).hide();
        }

        public displayElementSlow(elementId: string) : void {
            $("#" + elementId).show("slow");
        }

        public hideElementSlow(elementId: string) : void {
            $("#" + elementId).slideUp();
        }

        public convertTextToDecimal(strNumber: string): number{
            if (this.isStringValueNullOrEmpty(strNumber)) {
                return null;
            }

            var strDecimailSep: string = this.getDecimalSeparator();//$("#DecimalSeparator").val();
            let tmpNumber: string = strNumber.replace(strDecimailSep, ".").replace(" ", "");
            return parseFloat(tmpNumber);
        }

        public convertDecimalToText(dNumber: number): string {
            if (this.isValueNullOrUndefined(dNumber)) {
                return null;
            }

            var strDecimailSep: string = this.getDecimalSeparator();
            
            let strDec: string = dNumber.toLocaleString(this.getCultureCode(), { maximumFractionDigits: 2, minimumFractionDigits: 2 });
            //let tmpNumb: number = dNumber.toFixed(2);
            //let strDec: string = tmpNumb.toString();
            //strDec = strDec.replace(".", strDecimailSep);
            return strDec;
        }

        private getDecimalSeparator(): string {
            let inpDec: HTMLInputElement = <HTMLInputElement>document.getElementById("DecimalSeparator");
            if (this.isValueNullOrUndefined(inpDec)) {
                return ".";
            }
            return inpDec.value;
        }

        private getCultureCode(): string {
            let inpDec: HTMLInputElement = <HTMLInputElement>document.getElementById("CultureCode");
            if (this.isValueNullOrUndefined(inpDec)) {
                return ".";
            }
            return inpDec.value;
        }

        public isStringDecimalNumber(strDec: string): boolean {
            if (this.isValueNullOrUndefined(strDec)) {
                return false;
            }

            var dParse: number = this.convertTextToDecimal(strDec);
            if (isNaN(dParse)) {
                return false;
            }

            let decimalPoint: string = this.getDecimalSeparator().trim();

            let parts: string[] = strDec.split(decimalPoint);
            if (parts.length > 2) {
                return false;
            }
                        
            let strCheckNum: string = dParse.toString().replace(".", decimalPoint);
            let strCheckOrig: string = strDec;

            

            if (strCheckOrig.indexOf(decimalPoint) > -1) {
                while (strCheckOrig.substring(0, 1) === "0") {
                    strCheckOrig = strCheckOrig.substring(1);
                }
                if (strCheckOrig.substring(0, 1) === decimalPoint) {
                    strCheckOrig = "0" + strCheckOrig;
                }

                while (strCheckOrig.substring(strCheckOrig.length - 1, strCheckOrig.length) === "0") {
                    strCheckOrig = strCheckOrig.substring(0, strCheckOrig.length - 1);
                }

                while (strCheckOrig.substring(strCheckOrig.length - 1, strCheckOrig.length) === decimalPoint) {
                    strCheckOrig = strCheckOrig.substring(0, strCheckOrig.length - 1);
                }
            }

            if (strCheckNum != strCheckOrig.trim()) {
                //because of ParseFloat works like this: parseFloat('60 years') = 60
                return false;
            }

            return true;
        }

        public validateDecimalNumber(strDecimal: string, input: any): boolean {
            if (this.isValueNullOrUndefined(input)) {
                return false;
            }

            input.$error.required = false;
            input.$setValidity("required", true);

            if (!this.isStringDecimalNumber(strDecimal)) {
                input.$error.required = true;
                input.$setValidity("required", false);
                return false;
            }

            return true;
        }

        public validateMultiEmails(strMails: string, input: any): boolean {
            let isMailValid = this.isValidMultiEmails(strMails);
            if (isMailValid === true) {
                input.$error.email = false;
                input.$setValidity("email", true);
                return true;
            } else {
                input.$error.email = true;
                input.$setValidity("email", false);
                return false;
            }
            //alert('mail err');
            //return true;
            //if (this.isValueNullOrUndefined(input)) {
            //    return false;
            //}

            //input.$error.required = false;
            //input.$setValidity("required", true);

            //if (!this.isStringDecimalNumber(strDecimal)) {
            //    input.$error.required = true;
            //    input.$setValidity("required", false);
            //    return false;
            //}

            
        }

        

        private isValidEmail(email: string) : boolean {
            let expr = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
            return expr.test(email);
        }

        protected isValidMultiEmails(emails: string): boolean {
            if (this.isStringValueNullOrEmpty(emails)) {
                return false;
            }

            let mailsItems: string[] = emails.split(";");
            for (let i: number = 0; i < mailsItems.length; i++) {
                if (this.isValidEmail(mailsItems[i]) === false) {
                    return false;
                }
            }

            return true;
        }

    //function ValidateEmails() {
    //    var message = ""
    //    var inputs = document.getElementsByTagName("input");
    //    for (var i = 0; i < inputs.length; i++) {
    //        if (inputs[i].type == "text" && inputs[i].className.indexOf("email") != -1) {
    //            var email = inputs[i].value;
    //            if (IsValidEmail(email)) {
    //                message += email + " - Valid Email.\n"
    //            } else {
    //                message += email + " - Invalid Email.\n"
    //            }
    //        }
    //    }
    //    alert(message);
    //}

        public stopPropagation(event: Event): void {
            event.stopPropagation();
        }

       public getAllUrlParams() : UrlParam[] {
           var keyPairs: UrlParam[] = [];
           var params: string[] = window.location.search.substring(1).split('&');
            for (var i: number = params.length - 1; i >= 0; i--) {
                var paramItems: string[] = params[i].split('=');
                var urlParam: UrlParam = new UrlParam();
                urlParam.param_name = paramItems[0];
                urlParam.param_value = paramItems[1];
                keyPairs.push(urlParam); 
            }
            return keyPairs;
        }

        public encodeUrl(rawUrl: string): string {
            return encodeURI(rawUrl);
        }

        public decodeUrl(rawUrl: string): string {
            return decodeURI(rawUrl);
        }

        public convertDateToDbString(dDate: Date) : string {
            if (this.isValueNullOrUndefined(dDate)) {
                return null;
            }

            var dd: string = dDate.getDate().toString();
            if (dd.length < 2) {
                dd = "0" + dd;
            }

            var mm: string = (dDate.getMonth() + 1).toString();
            if (mm.length < 2) {
                mm = "0" + mm;
            }

            var yyyy: string = dDate.getFullYear().toString();

            return yyyy + "-" + mm + "-" + dd;
        }

        public getErrorMessage(msgKey: string): string {
            switch (msgKey) {
                case "NotAuthorized":
                    return this.getLocalizeText("MsgNotAuthorizedPerformActionText"); 
                case "MissingMandatoryItem":
                    return this.getLocalizeText("MsgMissingMandatoryFieldText");
                case "RequestCannotBeReverted":
                    return this.getLocalizeText("MsgRequestCannotBeRevertedText");
            }

            return "";
        }

        public addUrlParam(url: string, paramName: string, paramValue: string): string {
            if (url.indexOf("?") < 0) {
                url += "?";
            } else {
                url += "&";
            }
            url += paramName + "=" + paramValue;

            return url;
        }

        private boxHeaderClick(divContentId: string, imgExpandId: string) : any {
            if ($("#" + divContentId).is(':visible')) {
                $("#" + divContentId).slideUp();
                $("#" + imgExpandId).attr("src", this.getRegetRootUrl() + "Content/Images/Panel/Expand.png");
                $("#" + imgExpandId).attr("title", this.locDisplayText);
            } else {
                $("#" + divContentId).show("slow");
                $("#" + imgExpandId).attr("src", this.getRegetRootUrl() + "Content/Images/Panel/Collapse.png");
                $("#" + imgExpandId).attr("title", this.locHideText);
            }
        }

        public getUrlParamValueString(paramName: string) : string {
            var urlParams: UrlParam[] = this.getAllUrlParams();
            if (!this.isValueNullOrUndefined(urlParams)) {
                for (var i: number = 0; i < urlParams.length; i++) {
                    if (urlParams[i].param_name.toLowerCase() == paramName.toLowerCase()) {
                        return (urlParams[i].param_value);
                    }
                }
            }

            return null;
        }

        public getUrlParamValueInt(paramName: string): number {
            var urlParams: UrlParam[] = this.getAllUrlParams();
            if (!this.isValueNullOrUndefined(urlParams)) {
                for (var i: number = 0; i < urlParams.length; i++) {
                    if (urlParams[i].param_name.toLowerCase() == paramName.toLowerCase()) {
                        return parseInt(urlParams[i].param_value);
                    }
                }
            }

            return null;
        }

        public gotoBottom(): void {
            //alert("scroll");
            //var element = document.getElementById("angContainer");
            //element.scrollTop = element.scrollHeight - element.clientHeight;
            window.scroll(0,1000);
        }

        public gotoTop(): void {
            window.scroll(0, 0);
        }

        public validateForm(form: angular.IFormController) {
            angular.forEach(form, (control: angular.INgModelController, name: string) => {
                // Excludes internal angular properties
                if (typeof name === 'string' && name.charAt(0) !== '$') {
                    // To display ngMessages
                    control.$setTouched();

                    // Runs each of the registered validators
                    control.$validate();
                }
            });
        }

        public convertMomentDateToString(date: any, dateFormat: string): string {
            //return date ? moment(date).format(dateFormat) : '';
            return RegetDataConvert.convertMomentDateToString(date, dateFormat);
        }

        private getLocalizeText(id: string): string {
            try {
                let input: HTMLInputElement = <HTMLInputElement>document.getElementById(id);

                return input.value;
            } catch {
                return "";
            }
        }

        protected getIntegerFromInput(elementId: string): number {
            let input: HTMLInputElement = <HTMLInputElement>document.getElementById(elementId);
            if (input == null) {
                return null;
            }

            let num = parseInt(input.value);

            return null;
        }

        protected close() : void {
            //var customWindow = window.open('', '_blank', '');
            //customWindow.close();
            //window.open('your current page URL', '_self', '');
            //window.close();
            //window.open("", '_self').window.close();
            window.opener = top;
            window.close();

            //return false;
        }
       
        protected addDiscussionItem(discText: string, discussion : Discussion) {
            let disItem: DiscussionItem = new DiscussionItem();
            disItem.disc_text = discText;

            let nameParts: string[] = this.currentUserName.split(" ");
            let initials: string = "";
            if (nameParts.length > 1) {
                initials += nameParts[1].substring(0, 1).toUpperCase();
            }
            initials += nameParts[0].substring(0, 1).toUpperCase();

            disItem.author_initials = initials;
            disItem.author_name = this.currentUserName;
            if (!this.isStringValueNullOrEmpty(this.currentUserPhotoUrl)) {
                disItem.author_photo_url = this.currentUserPhotoUrl;
            } else {
                disItem.author_photo_url = null;
            }
            disItem.modif_date_text = moment().format(this.dateTimeMomentFormatText);
            disItem.bkg_color = discussion.discussion_bkg_color;
            disItem.border_color = discussion.discussion_border_color;
            disItem.user_color = discussion.discussion_user_color;

            if (this.isValueNullOrUndefined(discussion.discussion_items)) {
                discussion.discussion_items = [];
            }
            discussion.discussion_items.push(disItem);
            //this.newRemarkItem = "";
        }

        public convertDateTimeToString(dateVal : Date) : string {
            return dateVal ? moment(dateVal).format(this.locDateTimeFormatText) : '';
            
        }

        protected fileUploadInit($scope: ng.IScope, Upload: ng.angularFileUpload.IUploadService): void {
            //there must be - let app = angular.module('RegetApp', ['ngFileUpload' .. in scrit wher upload file is used e.g. request.ts
            
            if (!this.isValueNullOrUndefined(this.$scope)) { //because of jasmine test
                $scope.$watch('files', () => {
                    $scope.upload($scope.files);
                });
                $scope.$watch('file', () => {
                    if ($scope.file != null) {
                        $scope.files = [$scope.file];
                    }
                });

                this.$scope.upload = (files: any[]) => {
                    if (!this.isValueNullOrUndefined(files) && files.length > 0) {

                        if (this.isValueNullOrUndefined(Upload)) {
                            this, this.displayErrorMsg();
                            return;
                        }

                        let iUploadFinishedCount: number = 0;
                        this.isUploadFileProgressBarVisible = true;
                        for (let i: number = 0; i < files.length; i++) {

                            let file = files[i];
                            if (file.size > 26214400) {
                                //alert(file.size);
                                let fileLenghtMb: number = (file.size / (1024 * 1024));
                                let msg: string = this.ĺocFileSizeOverLimitText
                                    .replace("{0}", "'" + file.name + "'")
                                    .replace("{1}", fileLenghtMb.toFixed(2) + " MB")
                                    .replace("{2}", "25" + " MB");
                                this.displayErrorMsg(msg);
                                iUploadFinishedCount++;
                                if (iUploadFinishedCount == files.length) {
                                    this.isUploadFileProgressBarVisible = false;
                                }
                                continue;
                            }
                            let config: ng.angularFileUpload.IFileUploadConfigFile = {
                                url: this.getRegetRootUrl() + "Attachment/UploadAttachment",
                                data: { file: file },
                                method: "POST"
                            };

                            Upload.upload(config).progress((evt: any) => {
                                this.progressPercentage = 100.0 * parseInt(evt.loaded) / parseInt(evt.total);

                            }).success((data: any, status: any, headers: any, config: any) => {
                                iUploadFinishedCount++;
                                let attUpload: Attachment = data;

                                this.uploadFile(attUpload);

                                //attUpload.is_can_be_deleted = true;
                                //if (this.isValueNullOrUndefined(this.request.attachments)) {
                                //    this.request.attachments = [];
                                //}
                                //this.request.attachments.push(attUpload);
                                this.progressPercentage = null;
                                if (iUploadFinishedCount == files.length) {
                                    this.isUploadFileProgressBarVisible = false;
                                }

                                //alert("finished");
                            }).catch((callback: any) => {
                                this.displayErrorMsg();
                                this.progressPercentage = null;
                                this.isUploadFileProgressBarVisible = false;
                            });



                            //**************** Works ************************************************
                            //var UploadAny: any = Upload;
                            //var upld = Upload;
                            //var file = files[i];
                            //UploadAny.upload({
                            //    //url: 'https://angular-file-upload-cors-srv.appspot.com/upload',
                            //    url: 'http://localhost:26077/FileUpload/',
                            //    fields: {
                            //        'username': ""
                            //    },
                            //    file: file
                            //}).progress(function (evt) {
                            //    //var progressPercentage = parseInt(100.0 * evt.loaded / evt.total);
                            //    var progressPercentage = 100.0 * parseInt(evt.loaded) / parseInt(evt.total);
                            //    alert(progressPercentage);
                            //    //$scope.log = 'progress: ' + progressPercentage + '% ' +
                            //    //    evt.config.file.name + '\n' + $scope.log;
                            //}).success(function (data, status, headers, config) {
                            //    $timeout(function () {
                            //        //$scope.log = 'file: ' + config.file.name + ', Response: ' + JSON.stringify(data) + '\n' + $scope.log;
                            //        alert("Loaded");
                            //    });
                            //});
                            //**************** Works ************************************************
                        }


                    }
                };
            }
        }

        protected uploadFile(attUpload: Attachment) {
            alert('uploadFile is not implemented');
        }
                
        private defaultDiacriticsRemovalMap: any = [
        { 'base': 'A', 'letters': '\u0041\u24B6\uFF21\u00C0\u00C1\u00C2\u1EA6\u1EA4\u1EAA\u1EA8\u00C3\u0100\u0102\u1EB0\u1EAE\u1EB4\u1EB2\u0226\u01E0\u00C4\u01DE\u1EA2\u00C5\u01FA\u01CD\u0200\u0202\u1EA0\u1EAC\u1EB6\u1E00\u0104\u023A\u2C6F' },
        { 'base': 'AA', 'letters': '\uA732' },
        { 'base': 'AE', 'letters': '\u00C6\u01FC\u01E2' },
        { 'base': 'AO', 'letters': '\uA734' },
        { 'base': 'AU', 'letters': '\uA736' },
        { 'base': 'AV', 'letters': '\uA738\uA73A' },
        { 'base': 'AY', 'letters': '\uA73C' },
        { 'base': 'B', 'letters': '\u0042\u24B7\uFF22\u1E02\u1E04\u1E06\u0243\u0182\u0181' },
        { 'base': 'C', 'letters': '\u0043\u24B8\uFF23\u0106\u0108\u010A\u010C\u00C7\u1E08\u0187\u023B\uA73E' },
        { 'base': 'D', 'letters': '\u0044\u24B9\uFF24\u1E0A\u010E\u1E0C\u1E10\u1E12\u1E0E\u0110\u018B\u018A\u0189\uA779\u00D0' },
        { 'base': 'DZ', 'letters': '\u01F1\u01C4' },
        { 'base': 'Dz', 'letters': '\u01F2\u01C5' },
        { 'base': 'E', 'letters': '\u0045\u24BA\uFF25\u00C8\u00C9\u00CA\u1EC0\u1EBE\u1EC4\u1EC2\u1EBC\u0112\u1E14\u1E16\u0114\u0116\u00CB\u1EBA\u011A\u0204\u0206\u1EB8\u1EC6\u0228\u1E1C\u0118\u1E18\u1E1A\u0190\u018E' },
        { 'base': 'F', 'letters': '\u0046\u24BB\uFF26\u1E1E\u0191\uA77B' },
        { 'base': 'G', 'letters': '\u0047\u24BC\uFF27\u01F4\u011C\u1E20\u011E\u0120\u01E6\u0122\u01E4\u0193\uA7A0\uA77D\uA77E' },
        { 'base': 'H', 'letters': '\u0048\u24BD\uFF28\u0124\u1E22\u1E26\u021E\u1E24\u1E28\u1E2A\u0126\u2C67\u2C75\uA78D' },
        { 'base': 'I', 'letters': '\u0049\u24BE\uFF29\u00CC\u00CD\u00CE\u0128\u012A\u012C\u0130\u00CF\u1E2E\u1EC8\u01CF\u0208\u020A\u1ECA\u012E\u1E2C\u0197' },
        { 'base': 'J', 'letters': '\u004A\u24BF\uFF2A\u0134\u0248' },
        { 'base': 'K', 'letters': '\u004B\u24C0\uFF2B\u1E30\u01E8\u1E32\u0136\u1E34\u0198\u2C69\uA740\uA742\uA744\uA7A2' },
        { 'base': 'L', 'letters': '\u004C\u24C1\uFF2C\u013F\u0139\u013D\u1E36\u1E38\u013B\u1E3C\u1E3A\u0141\u023D\u2C62\u2C60\uA748\uA746\uA780' },
        { 'base': 'LJ', 'letters': '\u01C7' },
        { 'base': 'Lj', 'letters': '\u01C8' },
        { 'base': 'M', 'letters': '\u004D\u24C2\uFF2D\u1E3E\u1E40\u1E42\u2C6E\u019C' },
        { 'base': 'N', 'letters': '\u004E\u24C3\uFF2E\u01F8\u0143\u00D1\u1E44\u0147\u1E46\u0145\u1E4A\u1E48\u0220\u019D\uA790\uA7A4' },
        { 'base': 'NJ', 'letters': '\u01CA' },
        { 'base': 'Nj', 'letters': '\u01CB' },
        { 'base': 'O', 'letters': '\u004F\u24C4\uFF2F\u00D2\u00D3\u00D4\u1ED2\u1ED0\u1ED6\u1ED4\u00D5\u1E4C\u022C\u1E4E\u014C\u1E50\u1E52\u014E\u022E\u0230\u00D6\u022A\u1ECE\u0150\u01D1\u020C\u020E\u01A0\u1EDC\u1EDA\u1EE0\u1EDE\u1EE2\u1ECC\u1ED8\u01EA\u01EC\u00D8\u01FE\u0186\u019F\uA74A\uA74C' },
        { 'base': 'OI', 'letters': '\u01A2' },
        { 'base': 'OO', 'letters': '\uA74E' },
        { 'base': 'OU', 'letters': '\u0222' },
        { 'base': 'OE', 'letters': '\u008C\u0152' },
        { 'base': 'oe', 'letters': '\u009C\u0153' },
        { 'base': 'P', 'letters': '\u0050\u24C5\uFF30\u1E54\u1E56\u01A4\u2C63\uA750\uA752\uA754' },
        { 'base': 'Q', 'letters': '\u0051\u24C6\uFF31\uA756\uA758\u024A' },
        { 'base': 'R', 'letters': '\u0052\u24C7\uFF32\u0154\u1E58\u0158\u0210\u0212\u1E5A\u1E5C\u0156\u1E5E\u024C\u2C64\uA75A\uA7A6\uA782' },
        { 'base': 'S', 'letters': '\u0053\u24C8\uFF33\u1E9E\u015A\u1E64\u015C\u1E60\u0160\u1E66\u1E62\u1E68\u0218\u015E\u2C7E\uA7A8\uA784' },
        { 'base': 'T', 'letters': '\u0054\u24C9\uFF34\u1E6A\u0164\u1E6C\u021A\u0162\u1E70\u1E6E\u0166\u01AC\u01AE\u023E\uA786' },
        { 'base': 'TZ', 'letters': '\uA728' },
        { 'base': 'U', 'letters': '\u0055\u24CA\uFF35\u00D9\u00DA\u00DB\u0168\u1E78\u016A\u1E7A\u016C\u00DC\u01DB\u01D7\u01D5\u01D9\u1EE6\u016E\u0170\u01D3\u0214\u0216\u01AF\u1EEA\u1EE8\u1EEE\u1EEC\u1EF0\u1EE4\u1E72\u0172\u1E76\u1E74\u0244' },
        { 'base': 'V', 'letters': '\u0056\u24CB\uFF36\u1E7C\u1E7E\u01B2\uA75E\u0245' },
        { 'base': 'VY', 'letters': '\uA760' },
        { 'base': 'W', 'letters': '\u0057\u24CC\uFF37\u1E80\u1E82\u0174\u1E86\u1E84\u1E88\u2C72' },
        { 'base': 'X', 'letters': '\u0058\u24CD\uFF38\u1E8A\u1E8C' },
        { 'base': 'Y', 'letters': '\u0059\u24CE\uFF39\u1EF2\u00DD\u0176\u1EF8\u0232\u1E8E\u0178\u1EF6\u1EF4\u01B3\u024E\u1EFE' },
        { 'base': 'Z', 'letters': '\u005A\u24CF\uFF3A\u0179\u1E90\u017B\u017D\u1E92\u1E94\u01B5\u0224\u2C7F\u2C6B\uA762' },
        { 'base': 'a', 'letters': '\u0061\u24D0\uFF41\u1E9A\u00E0\u00E1\u00E2\u1EA7\u1EA5\u1EAB\u1EA9\u00E3\u0101\u0103\u1EB1\u1EAF\u1EB5\u1EB3\u0227\u01E1\u00E4\u01DF\u1EA3\u00E5\u01FB\u01CE\u0201\u0203\u1EA1\u1EAD\u1EB7\u1E01\u0105\u2C65\u0250' },
        { 'base': 'aa', 'letters': '\uA733' },
        { 'base': 'ae', 'letters': '\u00E6\u01FD\u01E3' },
        { 'base': 'ao', 'letters': '\uA735' },
        { 'base': 'au', 'letters': '\uA737' },
        { 'base': 'av', 'letters': '\uA739\uA73B' },
        { 'base': 'ay', 'letters': '\uA73D' },
        { 'base': 'b', 'letters': '\u0062\u24D1\uFF42\u1E03\u1E05\u1E07\u0180\u0183\u0253' },
        { 'base': 'c', 'letters': '\u0063\u24D2\uFF43\u0107\u0109\u010B\u010D\u00E7\u1E09\u0188\u023C\uA73F\u2184' },
        { 'base': 'd', 'letters': '\u0064\u24D3\uFF44\u1E0B\u010F\u1E0D\u1E11\u1E13\u1E0F\u0111\u018C\u0256\u0257\uA77A' },
        { 'base': 'dz', 'letters': '\u01F3\u01C6' },
        { 'base': 'e', 'letters': '\u0065\u24D4\uFF45\u00E8\u00E9\u00EA\u1EC1\u1EBF\u1EC5\u1EC3\u1EBD\u0113\u1E15\u1E17\u0115\u0117\u00EB\u1EBB\u011B\u0205\u0207\u1EB9\u1EC7\u0229\u1E1D\u0119\u1E19\u1E1B\u0247\u025B\u01DD' },
        { 'base': 'f', 'letters': '\u0066\u24D5\uFF46\u1E1F\u0192\uA77C' },
        { 'base': 'g', 'letters': '\u0067\u24D6\uFF47\u01F5\u011D\u1E21\u011F\u0121\u01E7\u0123\u01E5\u0260\uA7A1\u1D79\uA77F' },
        { 'base': 'h', 'letters': '\u0068\u24D7\uFF48\u0125\u1E23\u1E27\u021F\u1E25\u1E29\u1E2B\u1E96\u0127\u2C68\u2C76\u0265' },
        { 'base': 'hv', 'letters': '\u0195' },
        { 'base': 'i', 'letters': '\u0069\u24D8\uFF49\u00EC\u00ED\u00EE\u0129\u012B\u012D\u00EF\u1E2F\u1EC9\u01D0\u0209\u020B\u1ECB\u012F\u1E2D\u0268\u0131' },
        { 'base': 'j', 'letters': '\u006A\u24D9\uFF4A\u0135\u01F0\u0249' },
        { 'base': 'k', 'letters': '\u006B\u24DA\uFF4B\u1E31\u01E9\u1E33\u0137\u1E35\u0199\u2C6A\uA741\uA743\uA745\uA7A3' },
        { 'base': 'l', 'letters': '\u006C\u24DB\uFF4C\u0140\u013A\u013E\u1E37\u1E39\u013C\u1E3D\u1E3B\u017F\u0142\u019A\u026B\u2C61\uA749\uA781\uA747' },
        { 'base': 'lj', 'letters': '\u01C9' },
        { 'base': 'm', 'letters': '\u006D\u24DC\uFF4D\u1E3F\u1E41\u1E43\u0271\u026F' },
        { 'base': 'n', 'letters': '\u006E\u24DD\uFF4E\u01F9\u0144\u00F1\u1E45\u0148\u1E47\u0146\u1E4B\u1E49\u019E\u0272\u0149\uA791\uA7A5' },
        { 'base': 'nj', 'letters': '\u01CC' },
        { 'base': 'o', 'letters': '\u006F\u24DE\uFF4F\u00F2\u00F3\u00F4\u1ED3\u1ED1\u1ED7\u1ED5\u00F5\u1E4D\u022D\u1E4F\u014D\u1E51\u1E53\u014F\u022F\u0231\u00F6\u022B\u1ECF\u0151\u01D2\u020D\u020F\u01A1\u1EDD\u1EDB\u1EE1\u1EDF\u1EE3\u1ECD\u1ED9\u01EB\u01ED\u00F8\u01FF\u0254\uA74B\uA74D\u0275' },
        { 'base': 'oi', 'letters': '\u01A3' },
        { 'base': 'ou', 'letters': '\u0223' },
        { 'base': 'oo', 'letters': '\uA74F' },
        { 'base': 'p', 'letters': '\u0070\u24DF\uFF50\u1E55\u1E57\u01A5\u1D7D\uA751\uA753\uA755' },
        { 'base': 'q', 'letters': '\u0071\u24E0\uFF51\u024B\uA757\uA759' },
        { 'base': 'r', 'letters': '\u0072\u24E1\uFF52\u0155\u1E59\u0159\u0211\u0213\u1E5B\u1E5D\u0157\u1E5F\u024D\u027D\uA75B\uA7A7\uA783' },
        { 'base': 's', 'letters': '\u0073\u24E2\uFF53\u00DF\u015B\u1E65\u015D\u1E61\u0161\u1E67\u1E63\u1E69\u0219\u015F\u023F\uA7A9\uA785\u1E9B' },
        { 'base': 't', 'letters': '\u0074\u24E3\uFF54\u1E6B\u1E97\u0165\u1E6D\u021B\u0163\u1E71\u1E6F\u0167\u01AD\u0288\u2C66\uA787' },
        { 'base': 'tz', 'letters': '\uA729' },
        { 'base': 'u', 'letters': '\u0075\u24E4\uFF55\u00F9\u00FA\u00FB\u0169\u1E79\u016B\u1E7B\u016D\u00FC\u01DC\u01D8\u01D6\u01DA\u1EE7\u016F\u0171\u01D4\u0215\u0217\u01B0\u1EEB\u1EE9\u1EEF\u1EED\u1EF1\u1EE5\u1E73\u0173\u1E77\u1E75\u0289' },
        { 'base': 'v', 'letters': '\u0076\u24E5\uFF56\u1E7D\u1E7F\u028B\uA75F\u028C' },
        { 'base': 'vy', 'letters': '\uA761' },
        { 'base': 'w', 'letters': '\u0077\u24E6\uFF57\u1E81\u1E83\u0175\u1E87\u1E85\u1E98\u1E89\u2C73' },
        { 'base': 'x', 'letters': '\u0078\u24E7\uFF58\u1E8B\u1E8D' },
        { 'base': 'y', 'letters': '\u0079\u24E8\uFF59\u1EF3\u00FD\u0177\u1EF9\u0233\u1E8F\u00FF\u1EF7\u1E99\u1EF5\u01B4\u024F\u1EFF' },
        { 'base': 'z', 'letters': '\u007A\u24E9\uFF5A\u017A\u1E91\u017C\u017E\u1E93\u1E95\u01B6\u0225\u0240\u2C6C\uA763' }
    ];

    private diacriticsMap: any = {};
    //for (var i = 0; i < defaultDiacriticsRemovalMap.length; i++) {
    //    var letters = defaultDiacriticsRemovalMap[i].letters;
    //    for (var j = 0; j < letters.length; j++) {
    //        diacriticsMap[letters[j]] = defaultDiacriticsRemovalMap[i].base;
    //    }
    //}

        //// "what?" version ... http://jsperf.com/diacritics/12
        public removeDiacritics(str) : any {
            return str.replace(/[^\u0000-\u007E]/g, (a: any) => {
                return this.diacriticsMap[a] || a;
            });
        }
    }

    
    export class HttpResult {
        public string_value: string = null;
        public error_id: number = null;
    }

    export class UrlParam {
        public param_name: string = null;
        public param_value: string = null;
    }

    //export class Supplier {
    //    public id: number = null;
    //    public company_id: number = null;
    //    public row_index: number = null;
    //    public supp_name: string = null;
    //    public supplier_id: string = null;
    //    public street_part1: string = null;
    //    public city: string = null;
    //    public zip: string = null;
    //    public country: string = null;
    //    public contact_person: string = null;
    //    public phone: string = null;
    //    public email: string = null;
    //    public active: boolean = false;
    //}
}