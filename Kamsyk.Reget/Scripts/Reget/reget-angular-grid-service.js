var app = angular.module('RegetAppGridService', []);

app.service('regetgridservice', function () {
    //this.myFunc = function (x) {
    //    return x.toString(16);
    //}

    //this.GetRowsCount = function () {
    //    return $scope.RowsCount;
    //};

    var urlParamDelimiter = "|";
    var urlParamValueDelimiter = "~";

    this.GetLastPageIndex = function (rowsCount, pageSize, currentPage) {
        if (rowsCount < (pageSize * (currentPage - 1) + 1)) {
            if (currentPage > 1) {
                currentPage--;
            }
        }

        var iLastPageIndex = Math.ceil(rowsCount / pageSize);
               
        return iLastPageIndex;
    };

    this.GetDisplayItemsToInfo = function (rowsCount, pageSize, currentPage) {
        var iToInfo = (currentPage) * pageSize;
        if (iToInfo > rowsCount) {
            iToInfo = rowsCount;
        }
        return iToInfo;
    };

    this.IsLastPage = function (rowsCount, pageSize, currentPage) {
              
        currentPage++;
        if ((currentPage * pageSize) >= (rowsCount + pageSize)) {
            return true;
        }


        return false;
    };

    this.IsFirstPage = function (currentPage) {
        currentPage--;
        if (currentPage < 1) {
            return true;
        }

        return false;
    };

    this.GetNewRowIndex = function(pageSize, rowsCount) {
        var newRowIndex = pageSize;
        if (rowsCount < pageSize) {
            newRowIndex = rowsCount;
        }
        if (newRowIndex < 0) {
            newRowIndex = 0;
        }

        return newRowIndex;
    };

    this.CancelEditRow = function (editRow) {
        if (editRow !== null) {
            editRow.editrow = false;
            editRow = null;
        }
               
        return editRow;
    };

    this.ClearFilters = function (columns, gridFilter) {
        gridFilter = [];
        angular.forEach(columns, function (col) {
            if (col.enableFiltering && (col.filters[0].term !== '' || col.filters[0].term === true || col.filters[0].term === false)) {
                gridFilter.push({
                    column_name: col.name,
                    filter_value: col.filters[0].term
                });
                col.filters[0].term = '';
            }
        });

        return gridFilter;
    };

    this.GetNewRowIndexColValue = function (currentPage, pageSize, currEditPageSize, rowsCount) {
        var rowIndex = (currentPage - 1) * pageSize + currEditPageSize;
        if (rowIndex > rowsCount + 1) {
            rowIndex = rowsCount + 1;
        }

        return rowIndex;
    };

    this.SortNullString = function (a, b, rowA, rowB, direction) {
        a = (IsStringValueNullOrEmpty(a)) ? ' ' : a;
        b = (IsStringValueNullOrEmpty(b)) ? ' ' : b;
        if (a === b) { return 0 };
        if (a < b) { return -1 };
        if (a > b) { return 1 };
    };

    //workaround - the grid edit row is sometimes not closed, remains in edit mode, thats way this switch all of the edit rows to display mode
    this.CancelEditRows = function (gridData) {
        if (IsValueNullOrUndefined(gridData)) {
            return;
        }
        if (gridData.length === 0) {
            return;
        }
        for (var i = 0; i < gridData.length; i++) {
            if (gridData[i].editrow === true) {
                gridData[i].editrow = false;
            }
        }
    };

    this.IsHideGridControls = function (gridOptions) {
        var isHidden = IsValueNullOrUndefined(gridOptions.data) || gridOptions.data.length === 0 || (gridOptions.data.length === 1 && gridOptions.data[0].id < 0);

        return isHidden;
    };

    //this.GetGridSettings = function () {

    //};

    this.SetGridSettings = function (dbGridId, gridApi, pageSize) {
        var userGridSettings = {};
        //Grid Name
        userGridSettings.grid_name = dbGridId;

        //Page Size
        userGridSettings.grid_page_size = pageSize;
        //alert("Page size:" + pageSize);

        //Filter
        var filter = this.GetFilterUrl(gridApi);
        userGridSettings.filter = filter;
        //alert("Filter:" + filter);

        //Sort
        var sort = this.GetSortUrl(gridApi);
        userGridSettings.sort = sort;

        //Columns
        var columns = this.GetColumnstUrl(gridApi);
        userGridSettings.columns = columns;

        return userGridSettings;
    };

    this.GetFilterUrl = function (gridApi) {
        if (IsValueNullOrUndefined(gridApi)) {
            return '';
        }

        //Filter
        var filter = '';
        angular.forEach(gridApi.grid.columns, function (col) {
            if (col.enableFiltering && col.filters[0].term !== null && col.filters[0].term !== undefined && (col.filters[0].term !== '' || col.filters[0].term === true || col.filters[0].term === false)) {
                if (filter.length > 0) {
                    filter += urlParamDelimiter;
                }
                filter += col.name + urlParamValueDelimiter + col.filters[0].term;
            }
        });

        return filter;
    };

    this.GetSortUrl = function (gridApi) {

        //sort
        var sort = '';
        angular.forEach(gridApi.grid.columns, function (col) {
            if (!IsValueNullOrUndefined(col.sort) && !IsValueNullOrUndefined(col.sort.direction)) {
                if (sort.length > 0) {
                    sort += urlParamDelimiter;
                }
                sort += col.name + urlParamValueDelimiter + col.sort.direction;
            }
        });

        return sort;
    };

    this.GetColumnstUrl = function (gridApi) {

        //columns
        var columns = '';
        angular.forEach(gridApi.grid.columns, function (col) {
            //if (col.name !== 'id') {

                if (columns.length > 0) {
                    columns += urlParamDelimiter;
                }
                columns += col.name + urlParamValueDelimiter + col.visible;
            //}
        });

        return columns;
    };

    this.GetGridFilterFromUrlString = function (strUrlFilter) {
        var gridFilter = [];

        if (!IsStringValueNullOrEmpty(strUrlFilter)) {
            var filterItems = strUrlFilter.split(urlParamDelimiter);
            for (var i = 0; i < filterItems.length; i++) {
                var itemFields = filterItems[i].split(urlParamValueDelimiter);
                var oVal = itemFields[1];
                if (oVal === 'true') {
                    oVal = true;
                } else if (oVal === 'false') {
                    oVal = false;
                }
                gridFilter.push({
                    column_name: itemFields[0],
                    filter_value: oVal
                });
            }
        }

        return gridFilter;
    }

    this.GetGridSortFromUrlString = function (strUrlSort) {
        var gridSort = [];

        if (!IsStringValueNullOrEmpty(strUrlSort)) {
            var sortItems = strUrlSort.split(urlParamDelimiter);
            for (var i = 0; i < sortItems.length; i++) {
                var itemFields = sortItems[i].split(urlParamValueDelimiter);
                gridSort.push({
                    column_name: itemFields[0],
                    direction: itemFields[1]
                });
            }
        }

        return gridSort;
    }

    this.GetGridColumnsFromUrlString = function (strUrlColumns) {
        var gridColumns = [];

        if (!IsStringValueNullOrEmpty(strUrlColumns)) {
            var columnsItems = strUrlColumns.split(urlParamDelimiter);
            for (var i = 0; i < columnsItems.length; i++) {
                var itemFields = columnsItems[i].split(urlParamValueDelimiter);
                var oVal = itemFields[1];
                if (oVal === 'true') {
                    oVal = true;
                } else if (oVal === 'false') {
                    oVal = false;
                }
                gridColumns.push({
                    column_name: itemFields[0],
                    visible: oVal
                });
            }
        }

        return gridColumns;
    }

    //create a user defined array from UI Grid Columns
    this.GetGridColumnsFromGridApi = function (gridApiColumns) {
        var gridColumns = [];

        for (var i = 0; i < gridApiColumns.length; i++) {
            var isVisible = true;
            if (!IsValueNullOrUndefined(gridApiColumns[i].visible) && gridApiColumns[i].visible === false) {
                isVisible = false;
            }
            gridColumns.push({
                column_name: gridApiColumns[i].name,
                visible: isVisible
            });
        }
                
        return gridColumns;
    }


    this.RestoreFilter = function (gridFilter, gridApi) {
        if (gridFilter !== null) {
            for (var i = 0; i < gridFilter.length; i++) {
                angular.forEach(gridApi.grid.columns, function (col) {
                    if (col.name === gridFilter[i].column_name) {
                        col.filters[0].term = gridFilter[i].filter_value;
                    }
                });
            }
        }
    };

    this.RestoreSort = function (gridSort, gridApi) {
        if (gridSort !== null) {
            for (var i = 0; i < gridSort.length; i++) {
                angular.forEach(gridApi.grid.columns, function (col) {
                    if (col.name === gridSort[i].column_name) {
                        col.sort.direction = gridSort[i].direction;
                    }
                });
            }
        }
    };

    this.GetColumnIndex = function (strColumnName, gridApi) {
        for (var i = 0; i < gridApi.grid.columns.length; i++) {
            if (gridApi.grid.columns[i].name === strColumnName) {
                return i;
            }
        }

        return -1;
    };

    this.RestoreColumns = function (gridColumns, gridOptions) {
        if (gridColumns === null) {
            return;
        }

        for (var i = 0; i < gridColumns.length; i++) {
            if (gridColumns[i].visible === false) {
                for (var j = gridOptions.columnDefs.length - 1; j >= 0; j--) {
                    if (gridOptions.columnDefs[j].name === "id") {
                        continue;
                    }
                    if (gridOptions.columnDefs[j].name === gridColumns[i].column_name) {
                        gridOptions.columnDefs.splice(j, 1);
                        break;
                    }
                }
            }
            //angular.forEach(gridApi.grid.columns, function (col) {
            //    if (col.name === gridColumns[i].column_name) {
            //        if()
            //        vm.columns.splice($scope.columns.length - 1, 1);
            //        col.visible = gridColumns[i].visible;

            //    }
            //});
        }

        for (var j = gridOptions.columnDefs.length - 1; j >= 0; j--) {
            if (gridOptions.columnDefs[j].name === "id") {
                continue;
            }
            var isFound = false;
            for (var i = 0; i < gridColumns.length; i++) {
                if (gridOptions.columnDefs[j].name === gridColumns[i].column_name) {
                    isFound = true;
                    break;
                }
            }
            if (!isFound) {
                gridOptions.columnDefs.splice(j, 1);
            }
        }

        this.SetColumnsOrder(gridColumns, gridOptions);
        
    };

    this.SetColumnsOrder = function (gridColumns, gridOptions) {
        var orderedColumns = [];
        var isReordered = false;
        var colIndex = 0;

        for (var i = 0; i < gridColumns.length; i++) {
            if (gridColumns[i].visible === false && gridColumns[i].column_name !== "id") {
                continue;
            }
            
            var col = angular.copy(gridOptions.columnDefs[i]);

            if (gridOptions.columnDefs[colIndex].name === gridColumns[i].column_name) {
                orderedColumns.push(gridOptions.columnDefs[colIndex]);
            } else {
                for (var j = 0; j < gridOptions.columnDefs.length; j++) {
                    if (gridOptions.columnDefs[j].name === gridColumns[i].column_name) {
                        orderedColumns.push(gridOptions.columnDefs[j]);
                        isReordered = true;
                        break;
                    }
                }
            }

            colIndex++;
        }

        if(isReordered) {
            gridOptions.columnDefs = orderedColumns;
        }
        
    }

    this.IsColumnOrderChanged = function (currentColumnsString, origColumns) {
        var currentColumns = this.GetGridColumnsFromUrlString(currentColumnsString);
        if (currentColumns === null || origColumns === null) {
            return true;
        }

        if (currentColumns.length !== origColumns.length) {
            return true;
        }

        for (var i = 0; i < currentColumns.length; i++) {
            if (currentColumns[i].column_name !== origColumns[i].name) {
                return true;
            }
        }

        return false;
    }

    //this.SaveUserGriddSettings = function () {
    //    $http({
    //        method: 'POST',
    //        url: GetRegetRootUrl() + 'Grid/SaveGriddSettingsToDb',
    //        data: jsonPurchaseGroupData
    //    }).then(function (response) {
    //        try {
    //            var result = response.data;
                
    //            HideLoader();


    //        } catch (ex) {
    //            throw ex;
    //        }
    //    }, function errorCallback(response) {
    //        throw ex;
    //    });
    //};

    //this.GetUserGridSettings = function () {
    //    $http({
    //        method: 'GET',
    //        url: urlPart,
    //        data: {}
    //    }).then(function (response) {
    //        try {
    //            // $scope.Participants = response.data;
                               
    //        } catch (ex) {
    //            throw ex;
    //        }
    //    }, function errorCallback(response) {
    //        throw ex;
    //    });
    //}

});