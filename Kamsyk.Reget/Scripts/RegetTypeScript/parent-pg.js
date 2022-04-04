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
        var ParentPgController = /** @class */ (function (_super) {
            __extends(ParentPgController, _super);
            //**************************************************************
            //**********************************************************
            //Constructor
            function ParentPgController($scope, $http, $filter, $mdDialog, $mdToast, uiGridConstants, $q, $timeout) {
                var _this = _super.call(this, $scope, $http, $filter, $mdDialog, $mdToast, $q, uiGridConstants, $timeout) || this;
                _this.$scope = $scope;
                _this.$http = $http;
                _this.$filter = $filter;
                _this.$mdDialog = $mdDialog;
                _this.$mdToast = $mdToast;
                _this.uiGridConstants = uiGridConstants;
                _this.$q = $q;
                _this.$timeout = $timeout;
                //*************************************************************
                //Get Loc Texts
                _this.locWarningText = $("#WarningText").val();
                _this.locErrMsg = $("#ErrMsgText").val();
                _this.locParentPgUsedText = $("#ParentPgUsedText").val();
                _this.locParentPgUsedCompsText = $("#ParentPgUsedCompsText").val();
                _this.locNameText = $("#NameText").val();
                _this.locDuplicityParentPgText = $("#DuplicityParentPgNameText").val();
                //private locTranslationText: string = $("#TranslationText").val();
                _this.locDeleteParentPgConfirmText = $("#DeleteParentPgConfirmText").val();
                _this.locUsedPgText = $("#ConnectedPurchaseGroupsText").val();
                _this.locLoadingLocalizationText = $("#LoadingLocalizationText").val();
                //**********************************************************
                //**************************************************************
                _this.isPgLoaded = false;
                _this.isMissinTextAdded = false;
                _this.companyNamePrefix = "companyName_";
                //private isMaxCompIdLoaded: boolean = false;
                //private maxCompId: number = -1;
                _this.yesNo = [{ value: true, label: _this.locYesText }, { value: false, label: _this.locNoText }];
                _this.parentPgExtended = null;
                _this.misPpgLocCount = 0;
                _this.isCompanyColumnsAdded = false;
                //***************************************************************
                _this.$onInit = function () { };
                //****************************************************************************
                //Abstract
                _this.locTranslationText = $("#TranslationText").val();
                _this.locMandatoryTextFieldText = $("#MandatoryTextFieldText").val();
                _this.isDefaultLang = ($("#IsDefaultLang").val() == "1");
                _this.dbGridId = "grdParentPg_rg";
                _this.deleteUrl = _this.getRegetRootUrl() + "ParentPg/DeleteParentPg" + "?t=" + new Date().getTime();
                _this.setGrid();
                _this.loadData();
                return _this;
            }
            ParentPgController.prototype.exportToXlsUrl = function () {
                return this.getRegetRootUrl() + "Report/GetParentPgReport?" +
                    "filter=" + encodeURI(this.filterUrl) +
                    "&sort=" + this.sortColumnsUrl +
                    "&t=" + new Date().getTime();
            };
            ParentPgController.prototype.getControlColumnsCount = function () {
                return 3;
            };
            ParentPgController.prototype.getDuplicityErrMsg = function (rowEntity) {
                return this.locDuplicityParentPgText.replace("{0}", rowEntity.name);
            };
            ParentPgController.prototype.getSaveRowUrl = function () {
                return this.getRegetRootUrl() + "ParentPg/SaveParentPgData?t=" + new Date().getTime();
            };
            ParentPgController.prototype.insertRow = function () {
                var newParentPg = new RegetApp.ParentPgExtended();
                newParentPg.id = -10;
                newParentPg.name = "";
                this.insertBaseRow(newParentPg);
            };
            ParentPgController.prototype.isRowChanged = function () {
                if (this.editRow === null) {
                    return true;
                }
                var origParentPg = this.editRowOrig;
                var updParentPg = this.editRow;
                var tmpParentPgs = this.gridOptions.data;
                var parentPgs = tmpParentPgs;
                var isChanged = false;
                var id = updParentPg.id;
                var parentPg = this.$filter("filter")(parentPgs, { id: id }, true);
                if (id < 0) {
                    //new row
                    this.newRowIndex = null;
                    return true;
                }
                else {
                    if (origParentPg.name !== updParentPg.name) {
                        return true;
                    }
                    if (this.isValueNullOrUndefined(origParentPg.selected_companies)
                        && !this.isValueNullOrUndefined(updParentPg.selected_companies)) {
                        return true;
                    }
                    if (!this.isValueNullOrUndefined(origParentPg.selected_companies)
                        && this.isValueNullOrUndefined(updParentPg.selected_companies)) {
                        return true;
                    }
                    if (!this.isValueNullOrUndefined(origParentPg.selected_companies)
                        && !this.isValueNullOrUndefined(updParentPg.selected_companies)) {
                        if (origParentPg.selected_companies.length != updParentPg.selected_companies.length) {
                            return true;
                        }
                        for (var i = 0; i < origParentPg.selected_companies.length; i++) {
                            if (origParentPg.selected_companies[i] !== updParentPg.selected_companies[i]) {
                                return true;
                            }
                        }
                    }
                }
                return false;
            };
            ParentPgController.prototype.isRowEntityValid = function (parentPg) {
                if (this.isStringValueNullOrEmpty(parentPg.name)) {
                    return this.locMissingMandatoryText;
                }
                return null;
            };
            ParentPgController.prototype.loadGridData = function () {
                this.getParentPgs();
            };
            ParentPgController.prototype.getMsgDisabled = function (parentPg) {
                return null;
            };
            ParentPgController.prototype.getMsgDeleteConfirm = function (parentPg) {
                return this.locDeleteParentPgConfirmText.replace("{0}", parentPg.name);
            };
            ParentPgController.prototype.saveTranslationToDb = function () {
                var _this = this;
                try {
                    this.hideTranslation();
                    var modifPg = angular.copy(this.parentPgExtended);
                    modifPg.local_text[0].text = this.translateText1;
                    if (modifPg.local_text.length > 1) {
                        modifPg.local_text[1].text = this.translateText2;
                    }
                    var updTexts = {};
                    updTexts.parentPgId = this.entityId;
                    updTexts.localTexts = modifPg.local_text;
                    var jsonEntityData = JSON.stringify(updTexts);
                    this.showLoaderBoxOnly(this.isError);
                    this.$http.post(this.getRegetRootUrl() + 'ParentPg/SaveParentPgTranslation', jsonEntityData).then(function (response) {
                        try {
                            var result = response.data;
                            if (!_this.isValueNullOrUndefined(result)
                                && !_this.isValueNullOrUndefined(result.string_value)) {
                                //&& result.string_value[] === "DUPLICITY") {
                                var errMsg = _this.locErrorMsgText;
                                var splitIndex = result.string_value.indexOf(",");
                                if (result.string_value.indexOf(",") > -1) {
                                    var errType = result.string_value.substring(0, splitIndex);
                                    if (errType === "DUPLICITY") {
                                        var duplicityPpgName = result.string_value.substring(splitIndex + 1).trim();
                                        errMsg = _this.locDuplicityParentPgText.replace("{0}", duplicityPpgName);
                                    }
                                }
                                _this.hideLoader();
                                _this.displayErrorMsg(errMsg);
                                return;
                            }
                            //this.parentPgExtended = angular.copy(modifPg);
                            _this.parentPgExtended.local_text[0].text = _this.translateText1;
                            if (_this.parentPgExtended.local_text.length > 1) {
                                _this.parentPgExtended.local_text[1].text = _this.translateText2;
                            }
                            //if (this.parentPgExtended.local_text.length > 2) {
                            //    this.parentPgExtended.local_text[2].text = this.translateText3;
                            //}
                            _this.parentPgExtended.name = _this.parentPgExtended.local_text[0].text;
                            if (!_this.isValueNullOrUndefined(_this.editRowOrig)) {
                                var origParentPg = _this.editRowOrig;
                                origParentPg.name = _this.parentPgExtended.local_text[0].text;
                            }
                            _this.hideLoaderWrapper();
                        }
                        catch (e) {
                            _this.hideLoader();
                            _this.displayErrorMsg();
                        }
                        finally {
                            _this.hideLoaderWrapper();
                        }
                    }, function (response) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    });
                }
                catch (e) {
                    this.hideLoader();
                    this.displayErrorMsg();
                }
            };
            ParentPgController.prototype.openTranslationDialog = function (row, col, event) {
                event.stopPropagation();
                this.parentPgExtended = row.entity;
                if (this.isValueNullOrUndefined(this.parentPgExtended.local_text) || this.parentPgExtended.local_text.length == 0) {
                    return;
                }
                this.entityId = this.parentPgExtended.id;
                this.translateLabel1 = this.parentPgExtended.local_text[0].label;
                this.translateFlagUrl1 = this.parentPgExtended.local_text[0].flag_url;
                this.translateText1 = this.parentPgExtended.local_text[0].text;
                //var txtParentPgTrans1: HTMLInputElement = <HTMLInputElement>document.getElementById("txtParentPgTrans1");
                //txtParentPgTrans1.value = parentPgExtended.local_text[0].text;
                //if (this.parentPgExtended.local_text.length > 1) {
                if (!this.isDefaultLang) {
                    this.translateLabel2 = this.parentPgExtended.local_text[1].label;
                    this.translateFlagUrl2 = this.parentPgExtended.local_text[1].flag_url;
                    this.translateText2 = this.parentPgExtended.local_text[1].text;
                }
                this.displayGridTranslation(event.target);
            };
            ParentPgController.prototype.updateFirstLocalText = function (entity) {
                var parentPg = entity;
                if (this.isValueNullOrUndefined(parentPg.local_text)
                    || this.isValueNullOrUndefined(parentPg.local_text.length == 0)) {
                    return;
                }
                parentPg.local_text[0].text = parentPg.name;
            };
            ParentPgController.prototype.getErrorMsgByErrId = function (errId, msg) {
                return this.locErrorMsgText;
            };
            ParentPgController.prototype.getDbGridId = function () {
                return this.dbGridId;
            };
            //*****************************************************************************
            //*****************************************************************************
            //Overwritten
            ParentPgController.prototype.gridEditRow = function (rowEntity) {
                this.hideTranslation();
                _super.prototype.gridEditRow.call(this, rowEntity);
            };
            ParentPgController.prototype.gridDeleteRow = function (entity, ev) {
                var _this = this;
                if (!this.isValueNullOrUndefined(entity.selected_companies)
                    && entity.selected_companies.length > 0) {
                    this.displayErrorMsg(this.locParentPgUsedText);
                }
                else {
                    //check whether pg is used in companies out of grid, in companies where user is not admin
                    try {
                        this.$http.get(this.getRegetRootUrl() + "ParentPg/IsCanParentPgCanBeDeleted?ppgId=" + entity.id + "&t=" + new Date().getTime(), {}).then(function (response) {
                            var tmpData = response.data;
                            var usedComps = tmpData;
                            if (_this.isValueNullOrUndefined(usedComps) || usedComps.length == 0) {
                                _super.prototype.gridDeleteRow.call(_this, entity, ev);
                            }
                            else {
                                var strComps = "";
                                for (var i = 0; i < usedComps.length; i++) {
                                    if (strComps.length > 100) {
                                        break;
                                    }
                                    if (!_this.isStringValueNullOrEmpty(strComps)) {
                                        strComps += ", ";
                                    }
                                    strComps += usedComps[i];
                                }
                                var strMsg = _this.locParentPgUsedCompsText + " " + strComps;
                                _this.displayErrorMsg(strMsg);
                            }
                            _this.hideLoader();
                        }, function (response) {
                            _this.hideLoader();
                            _this.displayErrorMsg();
                        });
                    }
                    catch (e) {
                        this.hideLoader();
                        this.displayErrorMsg();
                    }
                }
            };
            //*****************************************************************************
            //*****************************************************************************
            //Http Methods
            ParentPgController.prototype.getParentPgs = function () {
                var _this = this;
                try {
                    this.hideTranslation();
                    this.isPgLoaded = false;
                    var divBox = $(".reget-se-pre-con");
                    if ($(divBox).is(':visible') === false) {
                        this.showLoaderBoxOnly(this.isError);
                    }
                    this.$http.get(this.getRegetRootUrl() + 'ParentPg/GetParentPgsAdmin?filter=' + encodeURI(this.filterUrl) +
                        '&pageSize=' + this.pageSize +
                        '&currentPage=' + this.currentPage +
                        '&sort=' + this.sortColumnsUrl +
                        '&t=' + new Date().getTime(), {}).then(function (response) {
                        try {
                            //because of test, this is exception ParenPg is loaded twice
                            _this.testLoadDataCount++;
                            var tmpData = response.data;
                            _this.gridOptions.data = tmpData.db_data;
                            _this.rowsCount = tmpData.rows_count;
                            if (!_this.isCompanyColumnsAdded) {
                                _this.setCompanyColumns(tmpData.db_data);
                                _this.isCompanyColumnsAdded = true;
                            }
                            _this.loadGridSettings();
                            //this.setGridSettingData();
                            //********************************************************************
                            //it is very important otherwise 50 lines are nod diplayed properly !!!
                            _this.gridOptions.virtualizationThreshold = _this.rowsCount + 1;
                            //********************************************************************
                            _this.isPgLoaded = true;
                            _this.hideLoaderWrapper();
                        }
                        catch (e) {
                            _this.hideLoader();
                            _this.displayErrorMsg();
                        }
                        finally {
                            _this.hideLoaderWrapper();
                        }
                    }, function (response) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    });
                }
                catch (e) {
                    this.hideLoader();
                    this.displayErrorMsg();
                }
            };
            ParentPgController.prototype.getMissingLocalTextCount = function () {
                var _this = this;
                try {
                    this.hideTranslation();
                    this.isMissinTextAdded = false;
                    this.showLoader(this.isError);
                    this.$http.get(this.getRegetRootUrl() + 'ParentPg/GetMissingParentPgLocalTextCount?t=' + new Date().getTime(), {}).then(function (response) {
                        try {
                            var tmpData = response.data;
                            _this.misPpgLocCount = tmpData.int_value;
                            _this.isMissinTextAdded = true;
                            if (_this.misPpgLocCount == 0) {
                                _this.getParentPgs();
                                //this.getLoadDataGridSettings();
                            }
                            else {
                                _this.setMissingLocalText(0);
                            }
                            _this.hideLoaderWrapper();
                        }
                        catch (e) {
                            _this.hideLoader();
                            _this.displayErrorMsg();
                        }
                        finally {
                            _this.hideLoaderWrapper();
                        }
                    }, function (response) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    });
                }
                catch (e) {
                    this.hideLoader();
                    this.displayErrorMsg();
                }
            };
            ParentPgController.prototype.setMissingLocalText = function (ppgIndex) {
                var _this = this;
                try {
                    this.hideTranslation();
                    this.isMissinTextAdded = false;
                    this.showLoader(this.isError);
                    var strCount = this.misPpgLocCount.toString();
                    var index = ppgIndex + 1;
                    if (index > this.misPpgLocCount) {
                        index = this.misPpgLocCount;
                    }
                    var strCountOutOf = index.toString();
                    var loadingMsg = this.locLoadingLocalizationText.replace("##", strCount).replace("#", strCountOutOf);
                    angular.element("#spanLoading").html(loadingMsg);
                    this.$http.get(this.getRegetRootUrl() + "ParentPg/SetMissingParentPgLocalText?ppgIndex=" + ppgIndex + "&t=" + new Date().getTime(), {}).then(function (response) {
                        try {
                            if (ppgIndex < _this.misPpgLocCount) {
                                ppgIndex++;
                                _this.setMissingLocalText(ppgIndex);
                            }
                            else {
                                //this.getParentPgs();
                                _this.getLoadDataGridSettings();
                                _this.isMissinTextAdded = true;
                                _this.hideLoaderWrapper();
                            }
                        }
                        catch (e) {
                            _this.hideLoader();
                            _this.displayErrorMsg();
                        }
                        finally {
                            _this.hideLoaderWrapper();
                        }
                    }, function (response) {
                        _this.hideLoader();
                        _this.displayErrorMsg();
                    });
                }
                catch (e) {
                    this.hideLoader();
                    this.displayErrorMsg();
                }
            };
            //*****************************************************************************
            //*****************************************************************************
            //Methods
            ParentPgController.prototype.setGrid = function () {
                this.gridOptions.columnDefs = [
                    {
                        name: "row_index",
                        field: "row_index",
                        displayName: '',
                        enableFiltering: false,
                        enableSorting: false,
                        enableCellEdit: false,
                        width: 40,
                        enableColumnResizing: true,
                        cellTemplate: '<div class="ui-grid-cell-contents ui-grid-top-panel" style="text-align:center;vertical-align:middle;font-weight:normal;">{{COL_FIELD}}</div>'
                    },
                    {
                        name: 'action_buttons',
                        displayName: '',
                        enableFiltering: false,
                        enableSorting: false,
                        enableCellEdit: false,
                        enableHiding: false,
                        enableColumnResizing: false,
                        width: 70,
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellAction.html"
                    },
                    {
                        name: 'used_pg',
                        displayName: '',
                        enableFiltering: false,
                        enableSorting: false,
                        enableCellEdit: false,
                        enableHiding: false,
                        enableColumnResizing: false,
                        width: 35,
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellUsedPg.html"
                    },
                    {
                        name: "name", displayName: this.locNameText, field: "name",
                        enableHiding: false,
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTranslationTextMandatoryTemplate.html",
                        enableCellEdit: false,
                        width: 350,
                        minWidth: 350
                    }
                ];
            };
            ParentPgController.prototype.loadData = function () {
                //this.getMaxCompanyId();
                this.getMissingLocalTextCount();
            };
            //private getMaxCompanyId () {
            //    this.isMaxCompIdLoaded = false;
            //    this.showLoader(this.isError);
            //    this.$http.get(
            //        this.getRegetRootUrl() + 'ParentPg/GetMaxCompanyId?&t=' + new Date().getTime(),
            //        {}
            //    ).then((response) => {
            //        try {
            //            var tmpData: any = response.data;
            //            //this.maxCompId = tmpData;
            //            this.isMaxCompIdLoaded = true;
            //            this.hideLoaderWrapper();
            //        } catch (ex) {
            //            this.hideLoaderWrapper();
            //            this.displayErrorMsg();
            //        }
            //    }, (response: any) => {
            //        this.hideLoaderWrapper();
            //        this.displayErrorMsg();
            //    });
            //}
            ParentPgController.prototype.hideLoaderWrapper = function () {
                if (this.isError || (this.isPgLoaded && this.isMissinTextAdded && this.isCompanyColumnsAdded)) {
                    this.hideLoader();
                    this.isError = false;
                }
            };
            ParentPgController.prototype.toggleGridCheckbox = function (item, col) {
                var fieldParts = col.field.split('_');
                var compId = parseInt(fieldParts[1]);
                if (item[col.field]) {
                    if (item["companyIsUsed_" + compId] === true) {
                        var strMessage = this.locParentPgUsedText;
                        this.$mdDialog.show(this.$mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title(this.locConfirmationText)
                            .textContent(strMessage)
                            .ariaLabel('Alert Dialog')
                            .ok(this.locCloseText));
                    }
                    else {
                        item[col.field] = false;
                        this.getParentPgExtendedData(compId, false);
                    }
                }
                else {
                    item[col.field] = true;
                    this.getParentPgExtendedData(compId, true);
                }
            };
            ParentPgController.prototype.getParentPgExtendedData = function (compId, isChecked) {
                var editParentPg = this.editRow;
                if (isChecked === true) {
                    if (this.isValueNullOrUndefined(editParentPg.selected_companies)) {
                        editParentPg.selected_companies = [];
                    }
                    editParentPg.selected_companies.push(compId);
                }
                else {
                    if (!this.isValueNullOrUndefined(editParentPg.selected_companies)) {
                        for (var i = 0; i < editParentPg.selected_companies.length; i++) {
                            if (editParentPg.selected_companies[i] == compId) {
                                editParentPg.selected_companies.splice(i, 1);
                                break;
                            }
                        }
                    }
                }
            };
            ParentPgController.prototype.displayUsedPgs = function (parentPg) {
                window.location.href = this.getRegetRootUrl() + "ParentPg/UsedPg?ppgId=" + parentPg.id;
            };
            ParentPgController.prototype.setCompanyColumns = function (ppgData) {
                var _this = this;
                try {
                    var filterValues = this.yesNo;
                    if (!this.isValueNullOrUndefined(ppgData) && ppgData.length > 0) {
                        angular.forEach(ppgData[0], function (value, key) {
                            if (key.indexOf(_this.companyNamePrefix) >= 0) {
                                var colItems = value.split('|');
                                var pos = _this.getColumnIndex(colItems[0]);
                                if (pos < 0) {
                                    var compColName = colItems[0];
                                    var comColumn = {
                                        field: compColName,
                                        name: compColName,
                                        displayName: colItems[1],
                                        filter: {
                                            type: _this.uiGridConstants.filter.SELECT,
                                            selectOptions: filterValues
                                        },
                                        cellTemplate: _this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellCheckboxTemplate.html"
                                    };
                                    //Set filter
                                    if (!_this.isValueNullOrUndefined(_this.gridFilter)) {
                                        for (var i = 0; i < _this.gridFilter.length; i++) {
                                            if (compColName === _this.gridFilter[i].column_name) {
                                                comColumn.filter.term = _this.gridFilter[i].filter_value;
                                                _this.isFilterApplied = true;
                                            }
                                        }
                                    }
                                    //Set sort
                                    if (!_this.isValueNullOrUndefined(_this.gridSort)) {
                                        for (var i = 0; i < _this.gridSort.length; i++) {
                                            if (comColumn.name === _this.gridSort[i].column_name) {
                                                //comColumn.direction = this.gridSort[i].direction;
                                                comColumn.sort = {
                                                    direction: _this.gridSort[i].direction
                                                };
                                            }
                                        }
                                    }
                                    _this.gridOptions.columnDefs.push(comColumn);
                                }
                            }
                        });
                        this.setColumnsCanBeHidden();
                    }
                }
                catch (e) {
                    this.displayErrorMsg();
                }
            };
            return ParentPgController;
        }(RegetApp.BaseRegetGridTranslationTs));
        RegetApp.ParentPgController = ParentPgController;
        angular.
            module("RegetApp").
            controller("ParentPgController", Kamsyk.RegetApp.ParentPgController);
    })(RegetApp = Kamsyk.RegetApp || (Kamsyk.RegetApp = {}));
})(Kamsyk || (Kamsyk = {}));
//# sourceMappingURL=parent-pg.js.map