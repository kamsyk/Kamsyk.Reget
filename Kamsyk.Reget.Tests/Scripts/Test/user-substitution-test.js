/// <reference path="../typings/jasmine/jasmine.d.ts" />
/// <reference path="../../../Kamsyk.Reget/Scripts/RegetTypeScript/Base/reget-entity.ts" />
/// <reference path="../../../Kamsyk.Reget/Scripts/RegetTypeScript/user-substitution.ts" />
/// <reference path="../Test/TestBase/data-grid-base-test.ts" />
describe("User Substitution Controller", function () {
    var userSubstitutionController;
    var $scope;
    var $http;
    var $filter;
    var $mdDialog;
    var $mdToast;
    var uiGridConstants;
    var $q;
    var $timeout;
    //let substData: Kamsyk.RegetApp.UserSubstitution[] = null;
    beforeEach(function () {
        //substData = new UserSubstitutionAux().getInitSubstitutionData();
        spyOn(Kamsyk.RegetApp.UserSubstitutionController.prototype, "setGrid");
        //spyOn(Kamsyk.RegetApp.UserSubstitutionController.prototype, "loadGridData");
        spyOn(Kamsyk.RegetApp.UserSubstitutionController.prototype, "getSubstitutionData");
        spyOn(Kamsyk.RegetApp.UserSubstitutionController.prototype, "getLoadDataGridSettings");
        userSubstitutionController = new Kamsyk.RegetApp.UserSubstitutionController($scope, $http, $filter, $mdDialog, $mdToast, uiGridConstants, $q, $timeout);
        //spyOn(userSubstitutionController, "getNewRowIndex").and.returnValue(
        //    new Kamsyk.RegetTest.DataGridBaseTest().getNewRowIndex(userSubstitutionController.pageSize, substData.length));
    });
    it("Is User AppMan", function () {
        //Arrange
        var approvers = [];
        var appMan = new Kamsyk.RegetApp.Participant();
        appMan.id = 1;
        appMan.surname = "test";
        approvers.push(appMan);
        spyOn(userSubstitutionController, "getSubstitutedApprovers").and.returnValue(approvers);
        //Act
        var part = new Kamsyk.RegetApp.Participant();
        appMan.id = 1;
        appMan.surname = "test";
        userSubstitutionController.substitutedItemChange(part, null);
        //Asset
        var appMan0id = userSubstitutionController.approvers[0].id;
        expect(appMan0id).toEqual(1);
    });
    it("Is Row Changed", function () {
        //Arrange
        var origUserSubstitution = new Kamsyk.RegetApp.UserSubstitution(0, 1, 4, new Date("2019-01-16"), new Date("2019-02-16"), "substituted", "substitute", false, "remark", 1, "", "", "", 0, false, false, false, false, true, new Date("2019-02-16"), true);
        userSubstitutionController.editRowOrig = origUserSubstitution;
        //Act
        var isCorrect = true;
        var editUserSubstitution = angular.copy(origUserSubstitution);
        userSubstitutionController.editRow = editUserSubstitution;
        isCorrect = !(userSubstitutionController.isRowChanged());
        if (isCorrect) {
            editUserSubstitution = angular.copy(origUserSubstitution);
            editUserSubstitution.substituted_user_id = 86;
            userSubstitutionController.editRow = editUserSubstitution;
            isCorrect = (userSubstitutionController.isRowChanged());
        }
        if (isCorrect) {
            editUserSubstitution = angular.copy(origUserSubstitution);
            editUserSubstitution.substitute_user_id = 56;
            userSubstitutionController.editRow = editUserSubstitution;
            isCorrect = (userSubstitutionController.isRowChanged());
        }
        if (isCorrect) {
            editUserSubstitution = angular.copy(origUserSubstitution);
            editUserSubstitution.substitute_start_date = new Date(2020, 2, 3, 4, 5, 6);
            userSubstitutionController.editRow = editUserSubstitution;
            isCorrect = (userSubstitutionController.isRowChanged());
        }
        if (isCorrect) {
            editUserSubstitution = angular.copy(origUserSubstitution);
            editUserSubstitution.substitute_end_date = new Date(2020, 2, 3, 4, 5, 6);
            userSubstitutionController.editRow = editUserSubstitution;
            isCorrect = (userSubstitutionController.isRowChanged());
        }
        if (isCorrect) {
            editUserSubstitution = angular.copy(origUserSubstitution);
            editUserSubstitution.approval_status = 2;
            userSubstitutionController.editRow = editUserSubstitution;
            isCorrect = (userSubstitutionController.isRowChanged());
        }
        //Assert
        expect(isCorrect).toBe(true);
    });
    it("Is New Substitution Valid", function () {
        //Arrange
        spyOn(userSubstitutionController, "isNewSubsValid").and.returnValue(true);
        //spyOn(userSubstitutionController, "isNewSubstitutionValid").and.returnValue(true);
        userSubstitutionController.setIsDateFromValid(true);
        userSubstitutionController.setIsDateToValid(true);
        var substitutedUser = new Kamsyk.RegetApp.Participant();
        substitutedUser.id = 1;
        var substituteeUser = new Kamsyk.RegetApp.Participant();
        substituteeUser.id = 2;
        //Act
        var isCorrect = true;
        if (isCorrect) {
            userSubstitutionController.selectedsubstitutedUser = substitutedUser;
            userSubstitutionController.selectedsubstituteeUser = substituteeUser;
            userSubstitutionController.fromDate = new Date();
            userSubstitutionController.toDate = userSubstitutionController.fromDate;
            userSubstitutionController.toDate.setDate(userSubstitutionController.toDate.getDate() + 2);
            isCorrect = (userSubstitutionController.isSubstitutionValid());
        }
        if (isCorrect) {
            userSubstitutionController.selectedsubstitutedUser = null;
            isCorrect = (!userSubstitutionController.isSubstitutionValid());
            userSubstitutionController.selectedsubstitutedUser = substitutedUser;
        }
        if (isCorrect) {
            userSubstitutionController.selectedsubstituteeUser = null;
            isCorrect = (!userSubstitutionController.isSubstitutionValid());
            userSubstitutionController.selectedsubstituteeUser = substituteeUser;
        }
        if (isCorrect) {
            userSubstitutionController.selectedsubstitutedUser.id = null;
            isCorrect = !(userSubstitutionController.isSubstitutionValid());
            userSubstitutionController.selectedsubstitutedUser.id = 5;
        }
        if (isCorrect) {
            userSubstitutionController.selectedsubstituteeUser.id = null;
            isCorrect = !(userSubstitutionController.isSubstitutionValid());
            userSubstitutionController.selectedsubstituteeUser.id = 15;
        }
        if (isCorrect) {
            userSubstitutionController.selectedsubstitutedUser.id = 5;
            userSubstitutionController.selectedsubstituteeUser.id = 5;
            isCorrect = !(userSubstitutionController.isSubstitutionValid());
            userSubstitutionController.selectedsubstituteeUser.id = 15;
        }
        //Assert
        expect(isCorrect).toBe(true);
    });
});
var UserSubstitutionAux = /** @class */ (function () {
    function UserSubstitutionAux() {
    }
    UserSubstitutionAux.prototype.getInitSubstitutionData = function () {
        return [];
    };
    return UserSubstitutionAux;
}());
//# sourceMappingURL=user-substitution-test.js.map