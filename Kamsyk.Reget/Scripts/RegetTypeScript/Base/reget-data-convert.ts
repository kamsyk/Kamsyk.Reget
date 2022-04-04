declare module "reget-data-convert" {
    var _: string;
    export = _;
}


class RegetDataConvert {
    //public static convertToDecimal(value: number) : number {
    //    let strNumber: string = value.toString();
    //    strNumber = strNumber.replace(",", ".");

    //    let dNumber: number = parseFloat(strNumber);

    //    if (isNaN(dNumber)) {
    //        throw new Error("Not a Number");
    //    }

    //    return dNumber;
    //}

    public static convertStringToDecimal(strNumber: string): number {
        //let strNumber: string = value.toString();
        strNumber = strNumber.replace(",", ".");

        let dNumber: number = parseFloat(strNumber);

        if (isNaN(dNumber)) {
            throw new Error("Not a Number");
        }

        return dNumber;
    }

    public static convertDecimalToString(dNumber: number, decimalNumbers?: Number): string {
        if (isNaN(dNumber)) {
            throw new Error("Not a Number");
        }

        let intDecNumbers: number = -1;
        if (!RegetCommonTs.isValueNullOrUndefined(decimalNumbers) && decimalNumbers > -1) {
            intDecNumbers = parseInt(decimalNumbers.toFixed(0));
        }

        let htmlDecimalSeparator = document.getElementById("DecimalSeparator");
        if (RegetCommonTs.isValueNullOrUndefined(htmlDecimalSeparator)) {
            if (intDecNumbers > -1) {
                return dNumber.toFixed(intDecNumbers);
            } else {
                return dNumber.toString();
            }
        }

        let inDecimalSeparator: HTMLInputElement = <HTMLInputElement>document.getElementById("DecimalSeparator");
        if (RegetCommonTs.isStringValueNullOrEmpty(inDecimalSeparator.value)) {
            if (intDecNumbers > -1) {
                return dNumber.toFixed(intDecNumbers);
            } else {
                return dNumber.toString();
            }
        }

        let strDecimalSeparator: string = inDecimalSeparator.value;
        let strNum: string = null;
        if (intDecNumbers > -1) {
            strNum = dNumber.toFixed(intDecNumbers);
        } else {
            strNum = dNumber.toString();
        }

        strNum = strNum.replace(".", strDecimalSeparator);

        return strNum;
    }

    public static convertMomentDateToString(date: any, dateFormat: string): string {
        //return date;
        return date ? moment(date).format(dateFormat) : '';
    }

    private static Get
}
        

   

