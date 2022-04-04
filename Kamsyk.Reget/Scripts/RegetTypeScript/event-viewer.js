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
        var EventViewerController = /** @class */ (function (_super) {
            __extends(EventViewerController, _super);
            //**********************************************************
            //Constructor
            function EventViewerController($scope, $http, $filter, $mdDialog, $mdToast, uiGridConstants, $q, $timeout) {
                var _this = _super.call(this, $scope, $http, $filter, $mdDialog, $mdToast, $q, uiGridConstants, $timeout) || this;
                _this.$scope = $scope;
                _this.$http = $http;
                _this.$filter = $filter;
                _this.$mdDialog = $mdDialog;
                _this.$mdToast = $mdToast;
                _this.uiGridConstants = uiGridConstants;
                _this.$q = $q;
                _this.$timeout = $timeout;
                //***************************************************************
                _this.$onInit = function () { };
                _this.dbGridId = "grdEventViewer_rg";
                _this.deleteUrl = _this.getRegetRootUrl() + "Address/DeleteAddress" + "?t=" + new Date().getTime();
                return _this;
                //this.setGrid();
                //this.loadData();
            }
            //****************************************************************************
            //Abstract
            EventViewerController.prototype.exportToXlsUrl = function () {
                //return this.getRegetRootUrl() + "Report/GetAddressesReport?" +
                //    "filter=" + encodeURI(this.filterUrl) +
                //    "&sort=" + this.sortColumnsUrl +
                //    "&t=" + new Date().getTime();
                return null;
            };
            EventViewerController.prototype.getControlColumnsCount = function () {
                return 2;
            };
            EventViewerController.prototype.getDuplicityErrMsg = function (rowEntity) {
                //var addressText: string = this.getFullAddress(rowEntity);
                //return this.locDuplicityAddressText.replace("{0}", addressText);
                return null;
            };
            EventViewerController.prototype.getSaveRowUrl = function () {
                //return this.getRegetRootUrl() + "Address/SaveAddressData?t=" + new Date().getTime();
                return null;
            };
            EventViewerController.prototype.insertRow = function () {
                //var newAddress: Address = new Address();
                //newAddress.id = -10;
                //newAddress.company_name = "";
                //newAddress.company_name_text = "";
                //newAddress.street = "";
                //newAddress.city = "";
                //newAddress.zip = "";
                //this.insertBaseRow(newAddress);
            };
            EventViewerController.prototype.isRowChanged = function () {
                //if (this.editRow === null) {
                //    return true;
                //}
                //var origAddress: Address = this.editRowOrig;
                //var updAddress: Address = this.editRow;
                //var tmpAddresses: any = this.gridOptions.data;
                //var addresses: Address[] = tmpAddresses;
                //var isChanged: boolean = false;
                //var id: number = updAddress.id;
                //var address: Address[] = this.$filter("filter")(addresses, { id: id }, true);
                //if (id < 0) {
                //    //new row
                //    this.newRowIndex = null;
                //    return true;
                //} else {
                //    if (origAddress.company_name !== updAddress.company_name) {
                //        return true;
                //    } else if (origAddress.company_name_text !== updAddress.company_name_text) {
                //        return true;
                //    } else if (origAddress.street !== updAddress.street) {
                //        return true;
                //    } else if (origAddress.city !== updAddress.city) {
                //        return true;
                //    } else if (origAddress.zip !== updAddress.zip) {
                //        return true;
                //    }
                //}
                return false;
            };
            EventViewerController.prototype.isRowEntityValid = function (address) {
                //if (this.isStringValueNullOrEmpty(address.company_name)) {
                //    return this.locMissingMandatoryText;
                //}
                //if (this.isStringValueNullOrEmpty(address.company_name_text)) {
                //    return this.locMissingMandatoryText;
                //}
                return null;
            };
            EventViewerController.prototype.loadGridData = function () {
                //this.getAddressData();
            };
            EventViewerController.prototype.getMsgDisabled = function (address) {
                return null; //this.locSupplierWasDisabledText.replace("{0}", supplier.supp_name);
            };
            EventViewerController.prototype.getMsgDeleteConfirm = function (address) {
                //return this.locDeleteAddressConfirmText.replace("{0}", address.company_name);;
                return null;
            };
            EventViewerController.prototype.getErrorMsgByErrId = function (errId, msg) {
                return this.locErrorMsgText;
            };
            EventViewerController.prototype.getDbGridId = function () {
                return this.dbGridId;
            };
            //*****************************************************************************
            //****************************************************************************
            //Methods
            EventViewerController.prototype.setGrid = function () {
                this.gridOptions.columnDefs = [
                    {
                        name: "row_index",
                        field: "row_index",
                        displayName: "",
                        enableFiltering: false,
                        enableSorting: false,
                        enableCellEdit: false,
                        width: 40,
                        enableColumnResizing: true,
                        cellTemplate: "<div class=\"ui-grid-cell-contents ui-grid-top-panel\" style=\"text-align:center;vertical-align:middle;font-weight:normal;\">{{COL_FIELD}}</div>"
                    }
                ];
            };
            return EventViewerController;
        }(RegetApp.BaseRegetGridTs));
        RegetApp.EventViewerController = EventViewerController;
        angular.
            module("RegetApp").
            controller("EventViewerController", Kamsyk.RegetApp.EventViewerController);
    })(RegetApp = Kamsyk.RegetApp || (Kamsyk.RegetApp = {}));
})(Kamsyk || (Kamsyk = {}));
//# sourceMappingURL=event-viewer.js.map