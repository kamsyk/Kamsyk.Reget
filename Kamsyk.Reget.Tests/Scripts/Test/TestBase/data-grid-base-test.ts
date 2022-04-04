module KamsykTest {
    export class DataGridBaseTest {

        public static getNewRowIndex(pageSize: number, gridRowsCount: number): number {
            let newRowIndex = pageSize;
            if (gridRowsCount < pageSize || gridRowsCount > pageSize) {
                newRowIndex = gridRowsCount;
            }

            if (newRowIndex < 0) {
                newRowIndex = 0;
            }

            return newRowIndex;
        }
    }
}