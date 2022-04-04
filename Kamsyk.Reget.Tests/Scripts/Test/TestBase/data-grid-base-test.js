var KamsykTest;
(function (KamsykTest) {
    var DataGridBaseTest = /** @class */ (function () {
        function DataGridBaseTest() {
        }
        DataGridBaseTest.getNewRowIndex = function (pageSize, gridRowsCount) {
            var newRowIndex = pageSize;
            if (gridRowsCount < pageSize || gridRowsCount > pageSize) {
                newRowIndex = gridRowsCount;
            }
            if (newRowIndex < 0) {
                newRowIndex = 0;
            }
            return newRowIndex;
        };
        return DataGridBaseTest;
    }());
    KamsykTest.DataGridBaseTest = DataGridBaseTest;
})(KamsykTest || (KamsykTest = {}));
//# sourceMappingURL=data-grid-base-test.js.map