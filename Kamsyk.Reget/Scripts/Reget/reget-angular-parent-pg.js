var app = angular.module('RegetAppParentPg', ['ngMaterial', 'ngMessages', 'ui.grid', 'ui.grid.pagination', 'ui.grid.resizeColumns', 'ui.grid.selection', 'ui.grid.moveColumns', 'ui.grid.edit', 'RegetAppGridService', 'RegetCommonService']);

app.controller('ParentPgController', function ($scope, $compile, $http, $mdToast, $q, $filter, $element, $mdDialog, $timeout, uiGridConstants, i18nService, regetgridservice, regetcommonservice) {
    $scope.Request = null;
    $scope.IsError = false;
    $scope.IsPgLoaded = false;
    $scope.IsMaxCompIdLoaded = false;
    $scope.FilterUrl = "";
    $scope.PageSize = 10;
    $scope.CurrentPage = 1;
    $scope.SortColumnsUrl = "";
    $scope.CompanyNamePrefix = "companyName_";
    $scope.YesNo = [{ value: true, label: $scope.YesText }, { value: false, label: $scope.NoText }];
    $scope.EditRow = null;
    $scope.EditRowOrig = null;
    $scope.EditRowIndex = null;
    $scope.SkipLoad = false;
    $scope.MaxCompId = 0;
    $scope.parentPgs = [];

    $scope.WarningText = $("#WarningText").val();
    $scope.ErrMsg = $("#ErrMsgText").val();
    $scope.CloseText = $("#CloseText").val();
    $scope.YesText = $("#YesText").val();
    $scope.NoText = $("#NoText").val();
    $scope.EditText = $("#EditText").val();
    $scope.DeleteText = $("#DeleteText").val();
    $scope.SaveText = $("#SaveText").val();
    $scope.CancelText = $("#CancelText").val();
    $scope.ParentPgUsedText = $("#ParentPgUsedText").val();

    $scope.adminCompanies = $("#adminCompanies").val();
    
    $scope.gridOptions = {
        enableFiltering: true,
        enableRowSelection: true,
        enablePaging: true,
        paginationPageSizes: [10, 20, 50],
        paginationPageSize: $scope.PageSizeGrid,
        enableHorizontalScrollbar: 1, //0 - never, 1 when needed, 2 always
        enableVerticalScrollbar: 1,
        enablePaginationControls: false,

        columnDefs: [
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
                cellTemplate: GetRegetRootUrl() + "Content/Html/Grid/GridCellActionEditOnly.html"

            },
            {
                name: "name", displayName: $scope.NameText, field: "name",
                enableHiding: false,
                cellTemplate: GetRegetRootUrl() + "Content/Html/Grid/GridCellTextMandatoryTemplate.html",
                enableCellEdit: false,
                width: 350,
                minWidth: 350
            }
        ],
        onRegisterApi: function (gridApi) {

            $scope.gridApi = gridApi;

            $scope.gridApi.pagination.on.paginationChanged($scope, function (pageNumber, pageSize) {

            });

            $scope.gridApi.core.on.filterChanged($scope, debounce(function () {
                if ($scope.SkipLoad) {
                    return;
                }

                if (!$scope.GridSaveRow()) {
                    return;
                }


                $scope.CurrentPage = 1;

                var strFilter = $scope.FilterUrl;
                $scope.FilterUrl = regetgridservice.GetFilterUrl($scope.gridApi);
               
                if (strFilter !== $scope.FilterUrl) {
                    $scope.GetCommodities();
                }
            }, 500));

            $scope.gridApi.core.on.sortChanged($scope, function (grid, sortColumns) {
                if ($scope.SkipLoad) {
                    return;
                }

                if (!$scope.GridSaveRow()) {
                    return;
                }

                $scope.SortColumns = sortColumns;

                $scope.CurrentPage = 1;

                var strSort = $scope.SortColumnsUrl;
                $scope.SortColumnsUrl = "";
                if (sortColumns !== null) {
                    for (var i = 0; i < sortColumns.length; i++) {
                        var name = sortColumns[i].name;
                        var direction = sortColumns[i].sort.direction;
                        if ($scope.SortColumnsUrl.length > 0) {
                            $scope.SortColumnsUrl += $scope.UrlParamDelimiter;
                        }
                        $scope.SortColumnsUrl += name + $scope.UrlParamValueDelimiter + direction;
                    }
                    if (strSort !== $scope.SortColumnsUrl) {
                        $scope.GetCommodities();
                    }
                }
            });

            
        }
    };

    $scope.GetCommodities = function () {
        $scope.IsPgLoaded = false;
        ShowLoaderBoxOnly($scope.IsError);
        $http({
            method: 'GET',
            url: GetRegetRootUrl() + 'ParentPg/GetParentPgsAdmin?filter=' + encodeURI($scope.FilterUrl) +
                    '&pageSize=' + $scope.PageSize +
                    '&currentPage=' + $scope.CurrentPage +
                    '&sort=' + $scope.SortColumnsUrl +
                    '&t=' + new Date().getTime(),
            data: {}
        }).then(function (response) {
            try {
                $scope.gridOptions.data = response.data.db_data;
                $scope.RowsCount = response.data.rows_count;
                                
                if (!IsValueNullOrUndefined(response.data.db_data) && response.data.db_data.length > 0) {
                    angular.forEach(response.data.db_data[0], function (value, key) {
                        if (key.indexOf($scope.CompanyNamePrefix) >= 0) {
                            var colItems = value.split('|');
                            var pos = regetgridservice.GetColumnIndex(colItems[0], $scope.gridApi);
                            if (pos < 0) {
                                $scope.gridOptions.columnDefs.push({
                                    field: colItems[0],
                                    displayName: colItems[1],
                                    cellTemplate: GetRegetRootUrl() + "Content/Html/Grid/GridCellCheckboxTemplate.html",
                                });
                            }
                        }
                    });
                }
                                
                //********************************************************************
                //it is very important otherwise 50 lines are nod diplayed properly !!!
                $scope.gridOptions.virtualizationThreshold = $scope.RowsCount + 1;
                //********************************************************************

                $scope.IsPgLoaded = true;

                $scope.HideLoaderWrapper(false);
            } catch (ex) {
                $scope.HideLoaderWrapper(false);
                $scope.DisplayErrorMsg(true);
            }
        }, function errorCallback(response) {
            $scope.HideLoaderWrapper(true);
            $scope.DisplayErrorMsg(true);
        });
            
    };

    $scope.GetMaxCompanyId = function () {
        $scope.IsMaxCompIdLoaded = false;
        ShowLoaderBoxOnly($scope.IsError);
        $http({
            method: 'GET',
            url: GetRegetRootUrl() + 'ParentPg/GetMaxCompanyId?&t=' + new Date().getTime(),
            data: {}
        }).then(function (response) {
            try {
                $scope.MaxCompId = response.data;
                
                $scope.IsMaxCompIdLoaded = true;

                $scope.HideLoaderWrapper(false);
            } catch (ex) {
                $scope.HideLoaderWrapper(false);
                $scope.DisplayErrorMsg(true);
            }
        }, function errorCallback(response) {
            $scope.HideLoaderWrapper(true);
            $scope.DisplayErrorMsg(true);
        });

    };

    $scope.GetRowsCount = function () {
        return $scope.RowsCount;
    };

    $scope.GetCurrentPage = function () {
        return $scope.CurrentPage;
    };

    $scope.GridSaveRow = function () {

        try {
            if (IsValueNullOrUndefined($scope.EditRow)) {
                return true;
            }

            var isChanged = false;

            var id = $scope.EditRow["id"];
            var parentPg = $filter("filter")($scope.gridOptions.data, { id: id }, true);

            if (id < 0) {
                //new row
                isChanged = true;
                $scope.NewRowIndex = null;
            } else {
                if ($scope.EditRowOrig["name"] !== $scope.EditRow["name"]) {
                    isChanged = true;
                } else if (parentPg[0]["id"] < -1) {
                    isChanged = true;
                } else {
                    var iIndex = 0;
                    var compValue = "companySelected_" + iIndex;
                    while (iIndex <= $scope.MaxCompId && !isChanged) {
                        if ($scope.EditRowOrig[compValue] !== $scope.EditRow[compValue]) {
                            isChanged = true;
                        }
                        iIndex++;
                        compValue = "companySelected_" + iIndex;
                    }
                }
            }

            if (isChanged) {
                return $scope.SaveParentPg(parentPg[0], null);
            } else {
                $scope.EditRowChanged(false);
            }

            //$scope.RestoreFilter();
            regetgridservice.RestoreFilter($scope.GridFilter, $scope.gridApi);

            return true;
        } catch (ex) {
            throw ex;
        } finally {
            $scope.SkipLoad = false;
        }
    };

    $scope.SaveParentPg = function (parentPg) {

        if (!$scope.IsParentPgValid(parentPg)) {
            $scope.ShowAlert($scope.WarningText, $scope.MissingMandatoryText);
            return false;
        }

        $scope.IsError = false;

        ShowLoaderBoxOnly($scope.IsError);
        var parentPgExData = $scope.GetParentPgExtendedData(parentPg);
        var jsonParentPgData = JSON.stringify(parentPgExData);

        $http({
            method: 'POST',
            url: GetRegetRootUrl() + 'ParentPg/SaveParentPgData',
            data: jsonParentPgData
        }).then(function (response) {
            try {
                //var result = response.data;
                //var iId = result.int_value;

                //if (iId === -1 && !IsValueNullOrUndefined(response.data) && response.data.string_value === "DUPLICITY") {
                //    HideLoader();
                //    var errMSg = $scope.DuplicityCentreNameText.replace("{0}", centre.name);
                //    $scope.ShowAlert($scope.WarningText, errMSg);
                //    return;
                //} else if (iId === -1) {
                //    HideLoader();
                //    $scope.ShowAlert($scope.WarningText, $scope.ErrMsg);
                //    return;
                //}

                //var isNew = false;
                //if (centre.id < 0) {
                //    isNew = true;
                //    centre.id = iId;
                //    $scope.RowsCount++;
                //}

                $scope.EditRowChanged(false);
                $scope.NewRowIndex = null;
                
                if (isNew) {
                    $scope.EnableFiltering();
                }

                HideLoader();

            } catch (e) {
                HideLoader();
                $scope.ShowAlert($scope.WarningText, $scope.ErrMsg);
            }
        }, function errorCallback(response) {
            HideLoader();
            $scope.ShowAlert($scope.WarningText, $scope.ErrMsg);
        });

        return true;
    };

    $scope.GetParentPgExtendedData = function (parentPg) {
        var parentPgExtData = { id: parentPg.id, name: parentPg.name, selected_companies: [] };

        var iIndex = 0;
        var compValue = "companySelected_" + iIndex;
        while (iIndex <= $scope.MaxCompId) {
            if ($scope.EditRow[compValue] === true) {
                parentPgExtData.selected_companies.push(iIndex);
            }
            iIndex++;
            compValue = "companySelected_" + iIndex;
        }

        return parentPgExtData;
    }

    $scope.IsParentPgValid = function (parentPg) {
        var isValid = true;
        if (IsStringValueNullOrEmpty(parentPg["name"])) {
            isValid = false;
        }
                
        return isValid;
    };

    $scope.GridCancelEdit = function () {
        ShowLoaderBoxOnly($scope.IsError);

        try {
            var isNewRow = (!IsValueNullOrUndefined($scope.EditRow) && $scope.EditRow["id"] < -1);

            if (isNewRow) {
                regetgridservice.RestoreFilter($scope.GridFilter, $scope.gridApi);
            }

            $scope.NewRowIndex = null;
            if ($scope.EditRow !== null) {

                $scope.EditRow.editrow = false;

                if (isNewRow) {
                    var index = $scope.gridOptions.data.indexOf($scope.EditRow);
                    $scope.gridOptions.data.splice(index, 1);
                    $scope.gridOptions.paginationPageSize--;
                } else {
                    angular.forEach($scope.EditRow, function (value, key) {
                        $scope.EditRow[key] = $scope.EditRowOrig[key];
                    });
                }

                $scope.EditRow = null;
                $scope.EditRowIndex = null;
                
                if (isNewRow) {
                    $scope.EnableFiltering();
                }
            }
        } catch (ex) {
            throw ex;
        } finally {

            $scope.SkipLoad = false;
            $scope.HideLoaderWrapper(false);
        }
    };

    $scope.EnableFiltering = function () {
        $scope.SkipLoad = true;
        for (var i = 2; i < $scope.gridOptions.columnDefs.length; i++) {
            $scope.gridOptions.columnDefs[i].enableFiltering = true;
            $scope.gridApi.core.notifyDataChange(uiGridConstants.dataChange.COLUMN);
        }
        $scope.SkipLoad = false;
    }

    $scope.GetLastPageIndex = function () {
        return regetgridservice.GetLastPageIndex($scope.RowsCount, $scope.PageSize, $scope.CurrentPage);
    };

    $scope.GetDisplayItemsToInfo = function () {
        return regetgridservice.GetDisplayItemsToInfo($scope.RowsCount, $scope.PageSize, $scope.CurrentPage);

    };

    $scope.NextPage = function () {
        if (!$scope.GridSaveRow()) {
            return;
        }


        if (regetgridservice.IsLastPage($scope.RowsCount, $scope.PageSize, $scope.CurrentPage)) {
            return;
        } else {
            $scope.CurrentPage++;
        }


        $scope.GetCommodities();
    };

    $scope.PreviousPage = function () {
        if (!$scope.GridSaveRow()) {
            return;
        }

        if (regetgridservice.IsFirstPage($scope.CurrentPage)) {
            return;
        } else {
            $scope.CurrentPage--;
        }

        $scope.GetCommodities();


    };

    $scope.FirstPage = function () {
        if (!$scope.GridSaveRow()) {
            return;
        }

        $scope.CurrentPage = 1;
        $scope.GetCommodities();
    };

    $scope.LastPage = function () {
        if (!$scope.GridSaveRow()) {
            return;
        }


        $scope.CurrentPage = $scope.GetLastPageIndex();
        $scope.GetCommodities();

    };

    $scope.GotoPage = function () {
        if (!$scope.GridSaveRow()) {
            return;
        }

        //$scope.gridApi.pagination.seek(parseInt($scope.CurrentPage));
        if ($scope.CurrentPage < 1) {
            $scope.CurrentPage = 1;
        }

        if ($scope.CurrentPage > $scope.GetLastPageIndex()) {
            $scope.CurrentPage = $scope.GetLastPageIndex();
        }

        $scope.GetCommodities();
    };

    $scope.PageSizeChanged = function () {
        if (!$scope.GridSaveRow()) {
            return;
        }

        $scope.CurrentPage = 1;
        $scope.PageSizeGrid = $scope.PageSize + 1;
        $scope.gridOptions.paginationPageSize = $scope.PageSizeGrid;
        $scope.GetCommodities();
    };

    $scope.Refresh = function () {
        $scope.LoadData();

        $scope.EditRow = null;
        $scope.EditRowOrig = null;
        $scope.EditRowIndex = null;
        $scope.NewRowIndex = null;
        $scope.IsNewRow = false;
    };

    $scope.ExportToXls = function () {
        //window.open(GetRegetRootUrl() + 'Report/GetCentresReport?t=' + new Date().getTime());
        //ShowLoaderBoxOnly($scope.IsError);
        try {
            window.open(GetRegetRootUrl() + 'Report/GetCentresReport?t=' + new Date().getTime());
        } catch (e) {
            //$scope.HideLoaderWrapper(true);
            $scope.DisplayErrorMsg(true);
        } finally {
            // $scope.HideLoaderWrapper(true);
        }

    };

    $scope.CellClicked = function (row, col) {

        if (row.entity["id"] < -1) {
            return;
        }

        $scope.GridEditRow(row.entity);



    };

    $scope.GridEditRow = function (row) {
        $scope.EditRowIndex = $scope.gridOptions.data.indexOf(row);

        if ($scope.EditRow === null) {

            $scope.EditRowChanged(false);
        } else {
            $scope.GridSaveRow();
        }

        //$scope.FormatGridUserLookup();
        
    };

    
    $scope.EditRowChanged = function (isNewCentre) {
        $scope.EditRow = regetgridservice.CancelEditRow($scope.EditRow);

        
        if ($scope.EditRowIndex !== null) {
            $scope.EditRowOrig = angular.copy($scope.gridOptions.data[$scope.EditRowIndex]);
            $scope.gridOptions.data[$scope.EditRowIndex].editrow = true;
            $scope.EditRow = $scope.gridOptions.data[$scope.EditRowIndex];


            $scope.EditRowIndex = null;
        }


    };

    $scope.ToggleGridCheckbox = function (item, col, ev) {

        if (item[col.field]) {
            var fieldParts = col.field.split('_');
            var compId = parseInt(fieldParts[1]);
            if(item["companyIsUsed_" + compId] === true) {
                alert('used');
                var strMessage = $scope.ParentPgUsedText;
                var confirm = $mdDialog.confirm()
                         .title($scope.ConfirmationText)
                         .textContent(strMessage)
                         .ariaLabel("ActivePgEditConfirm")
                         .targetEvent(ev)
                         .ok($scope.NoText)
                         .cancel($scope.YesText);

                $mdDialog.show(confirm).then(function () {

                }, function () {
                    //$scope.GridDeleteRowFromDb(user, ev);
                    $mdDialog.show({
                        controller: DialogController,
                        templateUrl: GetRegetRootUrl() + 'ParentPg/UsedPg',
                        parent: angular.element(document.body),
                        targetEvent: ev,
                        clickOutsideToClose: true,
                        fullscreen: $scope.customFullscreen // Only for -xs, -sm breakpoints.
                    })
                    .then(function (answer) {
                        $scope.status = 'You said the information was "' + answer + '".';
                    }, function () {
                        $scope.status = 'You cancelled the dialog.';
                    });
                });
            } else {
                item[col.field] = false;
            }
        } else {
            item[col.field] = true;
        }

    };

    $scope.getParentPgsByCompany = function () {
    }

    
    $scope.HideLoaderWrapper = function (isError) {
        if (isError || ($scope.IsPgLoaded && $scope.IsMaxCompIdLoaded)) {

            HideLoader();
        }
    };

    $scope.DisplayErrorMsg = function (isHideGrid) {

        if (!$scope.IsError) {
            regetcommonservice.ShowErrorAlert($scope.WarningText, $scope.ErrMsg, $scope.CloseText);
        }
        if (isHideGrid) {
            angular.element("#angContainer").hide();
        }
        $scope.IsError = true;

    };

    $scope.LoadData = function () {
        $scope.GetMaxCompanyId();
        $scope.GetCommodities();
    };

    $scope.LoadData();

    function DialogController($scope, $mdDialog) {
        $scope.hide = function () {
            $mdDialog.hide();
        };

        $scope.cancel = function () {
            $mdDialog.cancel();
        };

        $scope.answer = function (answer) {
            $mdDialog.hide(answer);
        };
    }
});