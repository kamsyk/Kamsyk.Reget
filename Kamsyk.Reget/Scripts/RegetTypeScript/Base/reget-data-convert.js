var RegetDataConvert = /** @class */ (function () {
    function RegetDataConvert() {
    }
    //public static convertToDecimal(value: number) : number {
    //    let strNumber: string = value.toString();
    //    strNumber = strNumber.replace(",", ".");
    //    let dNumber: number = parseFloat(strNumber);
    //    if (isNaN(dNumber)) {
    //        throw new Error("Not a Number");
    //    }
    //    return dNumber;
    //}
    RegetDataConvert.convertStringToDecimal = function (strNumber) {
        //let strNumber: string = value.toString();
        strNumber = strNumber.replace(",", ".");
        var dNumber = parseFloat(strNumber);
        if (isNaN(dNumber)) {
            throw new Error("Not a Number");
        }
        return dNumber;
    };
    RegetDataConvert.convertDecimalToString = function (dNumber, decimalNumbers) {
        if (isNaN(dNumber)) {
            throw new Error("Not a Number");
        }
        var intDecNumbers = -1;
        if (!RegetCommonTs.isValueNullOrUndefined(decimalNumbers) && decimalNumbers > -1) {
            intDecNumbers = parseInt(decimalNumbers.toFixed(0));
        }
        var htmlDecimalSeparator = document.getElementById("DecimalSeparator");
        if (RegetCommonTs.isValueNullOrUndefined(htmlDecimalSeparator)) {
            if (intDecNumbers > -1) {
                return dNumber.toFixed(intDecNumbers);
            }
            else {
                return dNumber.toString();
            }
        }
        var inDecimalSeparator = document.getElementById("DecimalSeparator");
        if (RegetCommonTs.isStringValueNullOrEmpty(inDecimalSeparator.value)) {
            if (intDecNumbers > -1) {
                return dNumber.toFixed(intDecNumbers);
            }
            else {
                return dNumber.toString();
            }
        }
        var strDecimalSeparator = inDecimalSeparator.value;
        var strNum = null;
        if (intDecNumbers > -1) {
            strNum = dNumber.toFixed(intDecNumbers);
        }
        else {
            strNum = dNumber.toString();
        }
        strNum = strNum.replace(".", strDecimalSeparator);
        return strNum;
    };
    RegetDataConvert.convertMomentDateToString = function (date, dateFormat) {
        //return date;
        return date ? moment(date).format(dateFormat) : '';
    };
    return RegetDataConvert;
}());
//# sourceMappingURL=reget-data-convert.js.map