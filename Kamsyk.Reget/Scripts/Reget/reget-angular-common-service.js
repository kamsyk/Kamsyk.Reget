var app = angular.module('RegetCommonService', ['ngMaterial', 'ngMessages']);

app.service('regetcommonservice', function ($mdDialog, $mdToast) {
    
    this.ShowAlert = function (strTitle, strMsg, strCloseButtonLabel) {

        $mdDialog.show(
          $mdDialog.alert()
            .clickOutsideToClose(true)
            .title(strTitle)
            .textContent(strMsg)
            .ariaLabel('Alert Dialog')
            .ok(strCloseButtonLabel)
            // You can specify either sting with query selector
            .openFrom('#left')
            // or an element
            .closeTo(angular.element(document.querySelector('#right')))
        );

        
    };

    this.ShowErrorAlert = function (strTitle, strMsg, strCloseButtonLabel) {

        $mdDialog.show({
            //template: GetRegetRootUrl() + 'Content/Html/DialogError.html'
            template:
           '<md-dialog aria-label="Error dialog">' +
           '  <md-dialog-content class="md-dialog-content">' +
           '    <table>' +
           '        <tr>' +
           '            <td>' +
           '                <img src="' + GetRegetRootUrl() + 'Content/Images/Error.png' + '" class="reget-error-img">' +
           '            </td>' +
           '            <td>' +
           '                <h2 class="md-title ng-binding">' + strTitle + '</h2>' +
           '                <p class="ng-binding">' + strMsg + '</p>' +
           '            </td>' +
           '        </tr>' +
           '    <table>' +
           '  </md-dialog-content>' +
           '  <md-dialog-actions>' +
           '    <md-button class="md-primary" ng-click="closeDialog()">' +
           strCloseButtonLabel +
           '    </md-button>' +
           '  </md-dialog-actions>' +
           '</md-dialog>',
            locals: {
                
            },
            controller: DialogController
        });

        function DialogController($scope, $mdDialog) {
            
            $scope.closeDialog = function () {
                $mdDialog.hide();
            }
        }

        
    };

    this.IsTestMode = function () {
        var txtIsTestMode = $("#IsTestMode");
        var isTestMode = !IsValueNullOrUndefined(txtIsTestMode) && txtIsTestMode.val() === "1";
        return isTestMode;
    };

    //this.CloseDialog = function () {
    //    $mdDialog.hide();
    //};


    this.ShowToast = function (strText) {
        $mdToast.show(
                        $mdToast.simple()
                        .textContent(strText)
                        .position('bottom center')
                        .theme("success-toast"));
    };

    //this.BaseDatePickerChange = function (isTime) {
    //    isDateTimePicker = (isTime === 1);
    //};
});