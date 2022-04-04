/// <reference path="../Base/reget-common.ts" />
function showTooltip(e, msg) {
    MainPageTs.showTooltip(e, msg);
}
function hideTooltip() {
    MainPageTs.hideTooltip();
}
var MainPageTs = /** @class */ (function () {
    function MainPageTs() {
    }
    //private static isTooltipScrollDisplayed: boolean = false;
    //public static showMenuTooltip(e: Event, msg: string): void {
    //    MainPageTs.showTooltipBox(e, msg, true);
    //}
    //public static showTooltip(e: Event, msg: string): void {
    //    MainPageTs.showTooltipBox(e, msg, false);
    //}
    MainPageTs.showTooltip = function (e, msg) {
        if (RegetCommonTs.isStringValueNullOrEmpty(msg)) {
            return;
        }
        var position = MainPageTs.getPosition(e);
        var iLeft = position.xPos;
        var iTop = position.yPos;
        var iWidth = position.width;
        //if (MainPageTs.isTooltipScrollDisplayed === true) {
        //    return;
        //}
        var spanRegetTooltipText = document.getElementById("spanMpRegetTooltipText");
        spanRegetTooltipText.innerHTML = msg;
        var divRegetToolTip = document.getElementById("divMpRegetToolTip");
        divRegetToolTip.style.visibility = "visible";
        divRegetToolTip.style.opacity = "1";
        divRegetToolTip.style.top = iTop + "px";
        spanRegetTooltipText.style.minWidth = iWidth + "px";
        spanRegetTooltipText.style.width = iWidth + "px";
        divRegetToolTip.style.width = iWidth + "px";
        divRegetToolTip.style.left = iLeft + "px";
    };
    MainPageTs.hideTooltip = function () {
        var spanRegetTooltipText = document.getElementById("spanMpRegetTooltipText");
        var divRegetToolTip = document.getElementById("divMpRegetToolTip");
        divRegetToolTip.style.opacity = "0";
        divRegetToolTip.style.visibility = "hidden";
    };
    MainPageTs.getPosition = function (e) {
        var el = e;
        var sourceElement = el;
        var iScrollY = $(".reget-body").scrollTop() + window.pageYOffset; // window.scrollY;;
        var iTop = iScrollY + Math.floor(sourceElement.getBoundingClientRect().top) + 5;
        var iScrollX = $(".reget-body").scrollLeft();
        iTop += sourceElement.clientHeight;
        var iWidth = sourceElement.clientWidth + 80;
        var iLeft = iScrollX + Math.floor((sourceElement.getBoundingClientRect().left + sourceElement.getBoundingClientRect().width / 2)
            - (iWidth / 2));
        var pos = new ElementPosition(iLeft, iTop, iWidth);
        return pos;
    };
    return MainPageTs;
}());
var ElementPosition = /** @class */ (function () {
    function ElementPosition(xPos, yPos, width) {
        this.xPos = null;
        this.yPos = null;
        this.width = null;
        this.xPos = xPos;
        this.yPos = yPos;
        this.width = width;
    }
    return ElementPosition;
}());
//# sourceMappingURL=main-page.js.map