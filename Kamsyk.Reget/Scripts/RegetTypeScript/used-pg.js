/// <reference path="../RegetTypeScript/Base/reget-entity.ts" />
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
        var UsedPgController = /** @class */ (function (_super) {
            __extends(UsedPgController, _super);
            //***********************************************************************
            //**********************************************************
            //Constructor
            function UsedPgController($scope, $http, $filter, $mdDialog, $mdToast, uiGridConstants, $q, $timeout) {
                var _this = _super.call(this, $scope, $http, $filter, $mdDialog, $mdToast, $q, uiGridConstants, $timeout) || this;
                _this.$scope = $scope;
                _this.$http = $http;
                _this.$filter = $filter;
                _this.$mdDialog = $mdDialog;
                _this.$mdToast = $mdToast;
                _this.uiGridConstants = uiGridConstants;
                _this.$q = $q;
                _this.$timeout = $timeout;
                //**********************************************************************
                //Loc Texts
                _this.locActiveText = $("#ActiveText").val();
                _this.locPurchaseGroupText = $("#PurchaseGroupText").val();
                _this.locParentPgText = $("#ParentPgText").val();
                _this.locAreaText = $("#AreaText").val();
                _this.locCompanyText = $("#CompanyText").val();
                _this.locDuplicityPgText = $("#DuplicityPgNameText").val();
                _this.locDeletePgConfirmText = $("#DeletePgConfirmText").val();
                _this.locLoadingLocalizationText = $("#LoadingLocalizationText").val();
                //**********************************************************************
                //***********************************************************************
                // Properties
                _this.isPpgLoaded = false;
                _this.isUsedPgLoaded = false;
                _this.isCompanyLoaded = false;
                _this.isMissingPgTextAdded = false;
                _this.isMissingPpgTextAdded = false;
                _this.isGridHidden = true;
                _this.skipLoad = false;
                _this.parentPgs = null;
                _this.parentPgId = null;
                _this.parentPgsDropDown = null;
                //This one "uiGrid.ISelectOptio" does NOT work !!!!!! Default Filter is not set properly
                //private yesNo: Array<uiGrid.ISelectOption> = [{ value: "true", label: this.locYesText }, { value: "false", label: this.locNoText }];
                _this.yesNo = [{ value: true, label: _this.locYesText }, { value: false, label: _this.locNoText }];
                _this.usedPg = null;
                _this.companies = null;
                _this.misPpgLocCount = 0;
                _this.misPgLocCount = 0;
                //***************************************************************
                _this.$onInit = function () { };
                //****************************************************************************
                //Abstract
                _this.locTranslationText = $("#TranslationText").val();
                _this.locMandatoryTextFieldText = $("#MandatoryTextFieldText").val();
                _this.isDefaultLang = ($("#IsDefaultLang").val() == "1");
                _this.dbGridId = "grdUsedPg_rg";
                _this.deleteUrl = _this.getRegetRootUrl() + "PurchaseGroup/DeleteUsedPg" + "?t=" + new Date().getTime();
                _this.setGrid();
                _this.loadData();
                return _this;
            }
            UsedPgController.prototype.exportToXlsUrl = function () {
                return this.getRegetRootUrl() + "Report/GetUsedPgReport?" +
                    "filter=" + encodeURI(this.filterUrl) +
                    "&sort=" + this.sortColumnsUrl +
                    "&t=" + new Date().getTime();
            };
            UsedPgController.prototype.getControlColumnsCount = function () {
                return 2;
            };
            UsedPgController.prototype.getDuplicityErrMsg = function (usedPg) {
                return this.locDuplicityPgText.replace("{0}", usedPg.pg_loc_name);
            };
            UsedPgController.prototype.getSaveRowUrl = function () {
                return this.getRegetRootUrl() + "ParentPg/SaveUsedPgData?t=" + new Date().getTime();
            };
            UsedPgController.prototype.insertRow = function () {
                //not used    
            };
            UsedPgController.prototype.isRowChanged = function () {
                if (this.editRow === null) {
                    return true;
                }
                var origUsedPg = this.editRowOrig;
                var updUsedPg = this.editRow;
                if (origUsedPg.pg_loc_name !== updUsedPg.pg_loc_name) {
                    return true;
                }
                if (origUsedPg.parent_pg_loc_name !== updUsedPg.parent_pg_loc_name) {
                    return true;
                }
                return false;
            };
            UsedPgController.prototype.isRowEntityValid = function (usedPg) {
                if (this.isStringValueNullOrEmpty(usedPg.pg_loc_name)) {
                    return this.locMissingMandatoryText;
                }
                if (this.isStringValueNullOrEmpty(usedPg.parent_pg_loc_name)) {
                    return this.locMissingMandatoryText;
                }
                return null;
            };
            UsedPgController.prototype.loadGridData = function () {
                this.getUsedPgs();
            };
            UsedPgController.prototype.getMsgDisabled = function (centre) {
                return null;
            };
            UsedPgController.prototype.getMsgDeleteConfirm = function (usedPg) {
                return this.locDeletePgConfirmText.replace("{0}", usedPg.pg_loc_name);
                ;
            };
            UsedPgController.prototype.getErrorMsgByErrId = function (errId, msg) {
                return this.locErrorMsgText;
            };
            UsedPgController.prototype.saveTranslationToDb = function () {
                var _this = this;
                try {
                    this.hideTranslation();
                    var modifUsedPg = angular.copy(this.usedPg);
                    modifUsedPg.local_text[0].text = this.translateText1;
                    if (modifUsedPg.local_text.length > 1) {
                        modifUsedPg.local_text[1].text = this.translateText2;
                    }
                    var updTexts = {};
                    updTexts.pgId = this.entityId;
                    updTexts.localTexts = modifUsedPg.local_text;
                    var jsonEntityData = JSON.stringify(updTexts);
                    this.showLoaderBoxOnly(this.isError);
                    this.$http.post(this.getRegetRootUrl() + 'PurchaseGroup/SavePgTranslation', jsonEntityData).then(function (response) {
                        try {
                            var result = response.data;
                            if (!_this.isValueNullOrUndefined(result)
                                && !_this.isValueNullOrUndefined(result.string_value)) {
                                var errMsg = _this.locErrorMsgText;
                                var splitIndex = result.string_value.indexOf(",");
                                if (result.string_value.indexOf(",") > -1) {
                                    var errType = result.string_value.substring(0, splitIndex);
                                    if (errType === "DUPLICITY") {
                                        var duplicityPpgName = result.string_value.substring(splitIndex + 1).trim();
                                        errMsg = _this.locDuplicityPgText.replace("{0}", duplicityPpgName);
                                    }
                                }
                                _this.hideLoader();
                                _this.displayErrorMsg(errMsg);
                                return;
                            }
                            _this.usedPg.local_text[0].text = _this.translateText1;
                            if (_this.usedPg.local_text.length > 1) {
                                _this.usedPg.local_text[1].text = _this.translateText2;
                            }
                            //if (this.parentPgExtended.local_text.length > 2) {
                            //    this.parentPgExtended.local_text[2].text = this.translateText3;
                            //}
                            _this.usedPg.pg_loc_name = _this.usedPg.local_text[0].text;
                            if (!_this.isValueNullOrUndefined(_this.editRowOrig)) {
                                var origUsedPg = _this.editRowOrig;
                                origUsedPg.pg_loc_name = _this.usedPg.local_text[0].text;
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
            UsedPgController.prototype.openTranslationDialog = function (row, col, event) {
                event.stopPropagation();
                this.usedPg = row.entity;
                if (this.isValueNullOrUndefined(this.usedPg.local_text) || this.usedPg.local_text.length == 0) {
                    return;
                }
                //this.modifLocalTexts: LocalText[] = angular.copy(this.usedPg.local_text);
                this.entityId = this.usedPg.id;
                this.translateLabel1 = this.usedPg.local_text[0].label;
                this.translateFlagUrl1 = this.usedPg.local_text[0].flag_url;
                this.translateText1 = this.usedPg.local_text[0].text;
                if (!this.isDefaultLang) {
                    this.translateLabel2 = this.usedPg.local_text[1].label;
                    this.translateFlagUrl2 = this.usedPg.local_text[1].flag_url;
                    this.translateText2 = this.usedPg.local_text[1].text;
                }
                this.displayGridTranslation(event.target);
            };
            UsedPgController.prototype.updateFirstLocalText = function (entity) {
                var usedPg = entity;
                if (this.isValueNullOrUndefined(usedPg.local_text)
                    || this.isValueNullOrUndefined(usedPg.local_text.length == 0)) {
                    return;
                }
                usedPg.local_text[0].text = usedPg.pg_loc_name;
            };
            UsedPgController.prototype.getDbGridId = function () {
                return this.dbGridId;
            };
            //*****************************************************************************
            //***********************************************************************************
            //Http methods
            UsedPgController.prototype.getParentPgs = function () {
                var _this = this;
                this.isPpgLoaded = false;
                this.isUsedPgLoaded = false;
                this.isCompanyLoaded = false;
                this.isError = false;
                var divBox = $(".reget-se-pre-con");
                if ($(divBox).is(':visible') === false) {
                    this.showLoaderBoxOnly(this.isError);
                }
                this.$http.get(this.getRegetRootUrl() + "ParentPg/GetParentPgs?t=" + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        _this.parentPgs = tmpData;
                        //this.getUsedPgs();
                        _this.getCompanies();
                        _this.isPpgLoaded = true;
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
            };
            UsedPgController.prototype.getUsedPgs = function () {
                var _this = this;
                var divBox = $(".reget-se-pre-con");
                if ($(divBox).is(':visible') === false) {
                    this.showLoaderBoxOnly(this.isError);
                }
                var strUrl = this.getRegetRootUrl() + "/ParentPg/GetUsedParentPgs?" +
                    'filter=' + encodeURI(this.filterUrl) +
                    '&pageSize=' + this.pageSize +
                    '&currentPage=' + this.currentPage +
                    '&sort=' + this.sortColumnsUrl +
                    '&t=' + new Date().getTime();
                //if (!this.isValueNullOrUndefined(this.parentPgId) && this.parentPgId > -1) {
                //    strUrl += "&parentPgId=" + this.parentPgId;
                //}
                this.$http.get(strUrl, {}).then(function (response) {
                    _this.isGridHidden = false;
                    try {
                        _this.testLoadDataCount++;
                        var tmpData = response.data;
                        _this.gridOptions.data = tmpData.db_data;
                        _this.rowsCount = tmpData.rows_count;
                        if (!_this.isValueNullOrUndefined(_this.gridApi)) {
                            //active
                            var pos = _this.getColumnIndex("active");
                            if (pos > 0) {
                                _this.gridOptions.columnDefs[pos].filter = {
                                    type: _this.uiGridConstants.filter.SELECT,
                                    selectOptions: _this.yesNo
                                };
                                _this.gridApi.core.notifyDataChange(_this.uiGridConstants.dataChange.COLUMN);
                            }
                            //parent pgs
                            if (_this.parentPgsDropDown === null && !_this.isValueNullOrUndefined(_this.parentPgs)) {
                                _this.parentPgsDropDown = [];
                                for (var i = 0; i < _this.parentPgs.length; i++) {
                                    var parentPgDropDown = new RegetApp.AgDropDown();
                                    parentPgDropDown.value = _this.parentPgs[i].name;
                                    parentPgDropDown.label = _this.parentPgs[i].name;
                                    _this.parentPgsDropDown.push(parentPgDropDown);
                                }
                            }
                            var pos = _this.getColumnIndex("parent_pg_loc_name");
                            if (pos > 0) {
                                _this.gridOptions.columnDefs[pos].filter = {
                                    type: _this.uiGridConstants.filter.SELECT,
                                    selectOptions: _this.parentPgsDropDown
                                };
                                _this.gridOptions.columnDefs[pos].editDropdownOptionsArray = _this.parentPgsDropDown;
                                _this.gridApi.core.notifyDataChange(_this.uiGridConstants.dataChange.COLUMN);
                            }
                        }
                        if (_this.isValueNullOrUndefined(_this.parentPgId)) {
                            //this.loadGridSettings();
                            _this.setGridSettingData();
                        }
                        else {
                            //Set Filter Parent Pg
                            var ppg = _this.$filter("filter")(_this.parentPgs, { id: _this.parentPgId }, true);
                            if (!_this.isValueNullOrUndefined(ppg)) {
                                angular.forEach(_this.gridApi.grid.columns, function (col) {
                                    if (col.name === "parent_pg_loc_name") {
                                        col.filters[0].term = ppg[0].name;
                                    }
                                });
                            }
                        }
                        //********************************************************************
                        //it is very important otherwise 50 lines are nod diplaye dproperly !!!
                        _this.gridOptions.virtualizationThreshold = _this.rowsCount + 1;
                        //********************************************************************
                        _this.isUsedPgLoaded = true;
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
            };
            UsedPgController.prototype.getCompanies = function () {
                var _this = this;
                this.showLoader(this.isError);
                this.$http.get(this.getRegetRootUrl() + "Participant/GetParticipantCompanies?t=" + new Date().getTime(), {}).then(function (response) {
                    try {
                        var tmpData = response.data;
                        _this.companies = tmpData;
                        //company
                        var pos = _this.getColumnIndex("company_name");
                        if (pos > 0) {
                            _this.gridOptions.columnDefs[pos].filter = {
                                type: _this.uiGridConstants.filter.SELECT,
                                selectOptions: _this.companies
                            };
                            //var filCompanies: uiGrid.edit.IEditDropdown[] = this.companies;
                            _this.gridOptions.columnDefs[pos].editDropdownOptionsArray = _this.companies;
                            _this.gridApi.core.notifyDataChange(_this.uiGridConstants.dataChange.COLUMN);
                        }
                        //this.getUsedPgs();
                        _this.getLoadDataGridSettings();
                        _this.isCompanyLoaded = true;
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
            };
            UsedPgController.prototype.getMissingPpgLocalTextCount = function () {
                var _this = this;
                try {
                    this.hideTranslation();
                    this.isMissingPpgTextAdded = false;
                    this.showLoader(this.isError);
                    this.$http.get(this.getRegetRootUrl() + 'ParentPg/GetMissingParentPgLocalTextCount?t=' + new Date().getTime(), {}).then(function (response) {
                        try {
                            var tmpData = response.data;
                            _this.misPpgLocCount = tmpData.int_value;
                            if (_this.misPpgLocCount == 0) {
                                _this.isMissingPpgTextAdded = true;
                                _this.getMissingPgLocalTextCount();
                            }
                            else {
                                _this.setMissingPpgLocalText(0);
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
            UsedPgController.prototype.setMissingPpgLocalText = function (ppgIndex) {
                var _this = this;
                try {
                    this.hideTranslation();
                    this.isMissingPpgTextAdded = false;
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
                                _this.setMissingPpgLocalText(ppgIndex);
                            }
                            else {
                                _this.getMissingPgLocalTextCount();
                                _this.isMissingPpgTextAdded = true;
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
            UsedPgController.prototype.getMissingPgLocalTextCount = function () {
                var _this = this;
                try {
                    this.hideTranslation();
                    this.isMissingPgTextAdded = false;
                    this.showLoader(this.isError);
                    this.$http.get(this.getRegetRootUrl() + 'ParentPg/GetMissingPgLocalTextCount?t=' + new Date().getTime(), {}).then(function (response) {
                        try {
                            var tmpData = response.data;
                            _this.misPgLocCount = tmpData.int_value;
                            if (_this.misPgLocCount == 0) {
                                _this.isMissingPgTextAdded = true;
                                _this.getParentPgs();
                            }
                            else {
                                _this.setMissingPgLocalText(0);
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
            UsedPgController.prototype.setMissingPgLocalText = function (pgIndex) {
                var _this = this;
                try {
                    this.hideTranslation();
                    this.isMissingPgTextAdded = false;
                    this.showLoader(this.isError);
                    var strCount = this.misPgLocCount.toString();
                    var index = pgIndex + 1;
                    if (index > this.misPgLocCount) {
                        index = this.misPgLocCount;
                    }
                    var strCountOutOf = index.toString();
                    var loadingMsg = this.locLoadingLocalizationText.replace("##", strCount).replace("#", strCountOutOf);
                    angular.element("#spanLoading").html(loadingMsg);
                    this.$http.get(this.getRegetRootUrl() + "ParentPg/SetMissingPgLocalText?ppgIndex=" + pgIndex + "&t=" + new Date().getTime(), {}).then(function (response) {
                        try {
                            if (pgIndex < _this.misPgLocCount) {
                                pgIndex++;
                                _this.setMissingPgLocalText(pgIndex);
                            }
                            else {
                                _this.getParentPgs();
                                _this.isMissingPgTextAdded = true;
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
                //try {
                //    this.hideTranslation();
                //    this.isMissingPgTextAdded = false;
                //    this.showLoaderBoxOnly(this.isError);
                //    this.$http.get(
                //        this.getRegetRootUrl() + 'ParentPg/SetMissingPgLocalText?t=' + new Date().getTime(),
                //        {}
                //    ).then((response) => {
                //        try {
                //            this.getParentPgs();
                //            this.isMissingPgTextAdded = true;
                //            this.hideLoaderWrapper();
                //        } catch (e) {
                //            this.hideLoader();
                //            this.displayErrorMsg();
                //        } finally {
                //            this.hideLoaderWrapper();
                //        }
                //    }, (response: any) => {
                //        this.hideLoader();
                //        this.displayErrorMsg();
                //    });
                //} catch (e) {
                //    this.hideLoader();
                //    this.displayErrorMsg();
                //}
            };
            //***********************************************************************************
            //*************************************************************************************
            //Methods
            UsedPgController.prototype.setGrid = function () {
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
                        name: "pg_loc_name", displayName: this.locPurchaseGroupText, field: "pg_loc_name",
                        enableHiding: false,
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTranslationTextMandatoryTemplate.html",
                        enableCellEdit: false,
                        width: 350,
                        minWidth: 350
                    },
                    {
                        name: "parent_pg_loc_name", displayName: this.locParentPgText, field: "parent_pg_loc_name",
                        enableHiding: false,
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellDropDownMandatoryTemplate.html",
                        enableCellEdit: false,
                        width: 250,
                        minWidth: 250
                    },
                    {
                        name: "centre_group_name", displayName: this.locAreaText, field: "centre_group_name",
                        enableHiding: false,
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextTemplateReadOnly.html",
                        enableCellEdit: false,
                        width: 200,
                        minWidth: 200
                    },
                    {
                        name: "company_name", displayName: this.locCompanyText, field: "company_name",
                        enableHiding: false,
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellTextTemplateReadOnly.html",
                        enableCellEdit: false,
                        width: 200,
                        minWidth: 200
                    },
                    {
                        name: "active", displayName: this.locActiveText, field: "active",
                        enableCellEdit: false,
                        enableHiding: false,
                        cellTemplate: this.getRegetRootUrl() + "Content/Html/Grid/TsGridCellCheckboxTemplate.html",
                        minWidth: 90,
                        width: 90
                    }
                ];
            };
            UsedPgController.prototype.loadData = function () {
                this.getMissingPpgLocalTextCount();
                this.parentPgId = this.getUrlParamValueInt("ppgid");
                //var urlParams: UrlParam[] = this.getAllUrlParams();
                //if (!this.isValueNullOrUndefined(urlParams)) {
                //    for (var i: number = 0; i < urlParams.length; i++) {
                //        if (urlParams[i].param_name.toLowerCase() == "ppgid") {
                //            this.parentPgId = parseInt(urlParams[i].param_value);
                //        }
                //    }
                //}
            };
            UsedPgController.prototype.hideLoaderWrapper = function () {
                if (this.isError || (this.isPpgLoaded
                    && this.isUsedPgLoaded
                    && this.isCompanyLoaded
                    && this.isMissingPpgTextAdded
                    && this.isMissingPgTextAdded)) {
                    this.hideLoader();
                }
            };
            UsedPgController.prototype.searchParentPg = function (strName) {
                return this.filterParentPg(strName);
            };
            UsedPgController.prototype.filterParentPg = function (name) {
                var _this = this;
                var searchParentPgs = [];
                if (this.isStringValueNullOrEmpty(name)) {
                    return this.parentPgs;
                }
                angular.forEach(this.parentPgs, function (parentPg) {
                    if (!_this.isStringValueNullOrEmpty(parentPg.name) && parentPg.name.toLowerCase().indexOf(name.toLowerCase()) > -1 ||
                        !_this.isStringValueNullOrEmpty(parentPg.name_wo_diacritics) && parentPg.name_wo_diacritics.trim().toLowerCase().indexOf(name.toLowerCase()) > -1) {
                        searchParentPgs.push(parentPg);
                    }
                });
                return searchParentPgs;
            };
            return UsedPgController;
        }(RegetApp.BaseRegetGridTranslationTs));
        RegetApp.UsedPgController = UsedPgController;
        angular.
            module("RegetApp").
            controller("UsedPgController", Kamsyk.RegetApp.UsedPgController);
    })(RegetApp = Kamsyk.RegetApp || (Kamsyk.RegetApp = {}));
})(Kamsyk || (Kamsyk = {}));
//# sourceMappingURL=used-pg.js.map