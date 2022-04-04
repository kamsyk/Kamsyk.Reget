declare module "reget-commons" {
    var _: string;
    export = _;
}


class RegetCommonTs {
    public static getRegetRootUrl(): string {
        var rootUrl = window.location.protocol + "//" + window.location.host;

        if (window.location.host.toLowerCase().indexOf('localhost') < 0) {
            var appName = window.location.pathname.split('/')[1];

            if (appName.substr(0) !== "/") {
                rootUrl += "/";
            }
            rootUrl += appName;
        }

        var lastChar = rootUrl.substr(rootUrl.length - 1);
        if (lastChar !== "/") {
            rootUrl += "/";
        }

        return rootUrl;
        }

        public static isValueNullOrUndefined(value: any) {
            if (value === null || value === undefined || value === "" || value === "null" || value === "undefined") {
                return true;
            } else {
                return false;
            }
        }

        public static isStringValueNullOrEmpty(strValue: string) {
            if (this.isValueNullOrUndefined(strValue) || strValue.trim().length === 0) {
                return true;
            } else {
                return false;
            }
        }

        public static convertIntegerToString(iValue: number, iDigitNr: number): string {
            if (isNaN(iValue)) {
                return "";
            }
             
            if (iValue >= 0 && iValue < 10) {
                return this.addPrefixZeros(String(iValue), iDigitNr);
            }

            if (iValue >= -9 && iValue < 0) {
                return "-" + this.addPrefixZeros(String(-1 * iValue), iDigitNr);
            }

            return String(iValue);
        }

        private static addPrefixZeros(strNum: string, iDigitNr: number) : string {
            for (var i = strNum.length; i < iDigitNr; i++) {
                strNum = "0" + strNum;
            }

            return strNum;
        }

         
 }

