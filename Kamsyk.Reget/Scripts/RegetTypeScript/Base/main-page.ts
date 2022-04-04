/// <reference path="../Base/reget-common.ts" />

function showTooltip(e: Event, msg: string) {
    MainPageTs.showTooltip(e, msg);
}

function hideTooltip(): void {
    MainPageTs.hideTooltip();
}


class MainPageTs {
    //private static isTooltipScrollDisplayed: boolean = false;

    //public static showMenuTooltip(e: Event, msg: string): void {
    //    MainPageTs.showTooltipBox(e, msg, true);
    //}

    //public static showTooltip(e: Event, msg: string): void {
    //    MainPageTs.showTooltipBox(e, msg, false);
    //}

    public static showTooltip(e: Event, msg: string): void {
        if (RegetCommonTs.isStringValueNullOrEmpty(msg)) {
            return;
        }

        let position: ElementPosition = MainPageTs.getPosition(e);
        let iLeft: number = position.xPos;
        let iTop: number = position.yPos;
        let iWidth: number = position.width;
        
        //if (MainPageTs.isTooltipScrollDisplayed === true) {
        //    return;
        //}

        var spanRegetTooltipText: HTMLSpanElement = <HTMLSpanElement>document.getElementById("spanMpRegetTooltipText");
        spanRegetTooltipText.innerHTML = msg;

        var divRegetToolTip: HTMLDivElement = <HTMLDivElement>document.getElementById("divMpRegetToolTip");
        divRegetToolTip.style.visibility = "visible";
        divRegetToolTip.style.opacity = "1";
        divRegetToolTip.style.top = iTop + "px";

        spanRegetTooltipText.style.minWidth = iWidth + "px";
        spanRegetTooltipText.style.width = iWidth + "px";

        
        divRegetToolTip.style.width = iWidth + "px";
        divRegetToolTip.style.left = iLeft + "px";

        
    }

    public static hideTooltip(): void {
        let spanRegetTooltipText: HTMLSpanElement = <HTMLSpanElement>document.getElementById("spanMpRegetTooltipText");
                
            let divRegetToolTip: HTMLDivElement = <HTMLDivElement>document.getElementById("divMpRegetToolTip");
            divRegetToolTip.style.opacity = "0";
            divRegetToolTip.style.visibility = "hidden";
        
        
    }

    public static getPosition(e: Event): ElementPosition {
        let el: any = e;
        let sourceElement: HTMLElement = <HTMLElement>el;

        let iScrollY: number = $(".reget-body").scrollTop() + window.pageYOffset;// window.scrollY;;
        let iTop: number = iScrollY + Math.floor(sourceElement.getBoundingClientRect().top) + 5;
        
        let iScrollX: number = $(".reget-body").scrollLeft();
        iTop += sourceElement.clientHeight;

        let iWidth: number = sourceElement.clientWidth + 80;
        let iLeft: number = iScrollX + Math.floor(
            (sourceElement.getBoundingClientRect().left + sourceElement.getBoundingClientRect().width / 2) 
            - (iWidth / 2)
        );
        
        let pos: ElementPosition = new ElementPosition(iLeft, iTop, iWidth);

        return pos;
    }
}

class ElementPosition {
    public xPos: number = null;
    public yPos: number = null;
    public width: number = null;
   
    constructor(xPos: number, yPos: number, width: number) {
        this.xPos = xPos;
        this.yPos = yPos;
        this.width = width;
        
    }
}