var RegetCommonTs = /** @class */ (function () {
    function RegetCommonTs() {
    }
    RegetCommonTs.getRegetRootUrl = function () {
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
    };
    RegetCommonTs.isValueNullOrUndefined = function (value) {
        if (value === null || value === undefined || value === "" || value === "null" || value === "undefined") {
            return true;
        }
        else {
            return false;
        }
    };
    RegetCommonTs.isStringValueNullOrEmpty = function (strValue) {
        if (this.isValueNullOrUndefined(strValue) || strValue.trim().length === 0) {
            return true;
        }
        else {
            return false;
        }
    };
    RegetCommonTs.convertIntegerToString = function (iValue, iDigitNr) {
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
    };
    RegetCommonTs.addPrefixZeros = function (strNum, iDigitNr) {
        for (var i = strNum.length; i < iDigitNr; i++) {
            strNum = "0" + strNum;
        }
        return strNum;
    };
    return RegetCommonTs;
}());
//# sourceMappingURL=reget-common.js.map