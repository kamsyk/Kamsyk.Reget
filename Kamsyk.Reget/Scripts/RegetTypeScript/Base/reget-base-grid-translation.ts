//var isDateTimePicker: boolean;

module Kamsyk.RegetApp {
    angular.module('RegetApp').directive("regettranslategrid", function () {
        return {
            restrict: "E",
            transclude: true,
            scope: {
                //localtexts: "=",
                text1: "=",
                text2: "=",
                //text3: "=",
                translateerrormsg: "=",
                isdefaultlang: "=",
                label1: "@",
                label2: "@",
                //label3: "@",
                flag1: "@",
                flag2: "@",
                //flag3: "@",
                savetext: "@",
                canceltext: "@",
                mandatorytext: "@",
                text1changed: "&",
                text2changed: "&",
                //text3changed: "&",
                saveparentpgtrans: "&",
                closeparentpgtrans: "&"
            },
            templateUrl: RegetCommonTs.getRegetRootUrl() + "Content/Html/AngGridTranslate.html"
        };
    });

    export abstract class BaseRegetGridTranslationTs extends BaseRegetGridTs {
        //*****************************************************************
        //Properties
        public translateLabel1: string = null;
        public translateLabel2: string = null;
        //public translateLabel3: string = null;

        public translateFlagUrl1: string = null;
        public translateFlagUrl2: string = null;
        //public translateFlagUrl3: string = null;

        public translateText1: string = null;
        public translateText2: string = null;
        //public translateText3: string = null;

        //public translateText1Modif: string = null;
        //public translateText2Modif: string = null;

        public entityId: number = null;

        public translateErrorMsg: string = null;
               
        //public modifLocalTexts: LocalText[] = null;
        //*****************************************************************

                        
        //*****************************************************************
        //Constructors
        constructor(
            protected $scope: ng.IScope,
            protected $http: ng.IHttpService,
            protected $filter: ng.IFilterService,
            protected $mdDialog: angular.material.IDialogService,
            protected $mdToast: angular.material.IToastService,
            protected $q: ng.IQService,
            protected uiGridConstants: uiGrid.IUiGridConstants,
            protected $timeout: ng.ITimeoutService
            
            //protected $moment: moment.Moment
        ) {
            super($scope, $http, $filter, $mdDialog, $mdToast, $q, uiGridConstants, $timeout);
                    
        }
        //******************************************************************

        //********************************************************************
        //abstract methods
        abstract loadGridData(): void;
        abstract isRowChanged(): boolean;
        abstract insertRow(): void;
        abstract exportToXlsUrl(): string;
        abstract isRowEntityValid(rowEntity: any): string;
        abstract getSaveRowUrl(): string;
        abstract getDuplicityErrMsg(rowEntity: any): string;
        abstract getControlColumnsCount(): number;
        abstract getMsgDisabled(entity: any): string;
        abstract getMsgDeleteConfirm(entity: any): string;
        abstract saveTranslationToDb(): void;
        abstract updateFirstLocalText(entity: any): void;
        abstract openTranslationDialog(row: any, col: any, event: MouseEvent);
        abstract isDefaultLang: boolean = false;
        
        abstract dbGridId: string = null;
        abstract deleteUrl: string = null;

        abstract locTranslationText: string = null;
        abstract locMandatoryTextFieldText: string = null;
        
        //********************************************************************

        //********************************************************************
        protected saveGridRowtoDb(rowEntity: any): boolean {
            var isSaved: boolean = super.saveGridRowtoDb(rowEntity)
            if (isSaved) {
                this.updateFirstLocalText(rowEntity);
            }

            return isSaved;
        }
        //*********************************************************************

        //*******************************************************************
        //Methods
        public displayGridTranslation(target: any): void {
            this.removeFormReleativePosition();

            //this.translateText1Orig = this.translateText1;
            //this.translateText2Orig = this.translateText2;
            this.translateErrorMsg = null;
            

            var divRow: HTMLDivElement = <HTMLDivElement>target;
            var divTranslation: HTMLDivElement = <HTMLDivElement>document.getElementById("divtranslatedirective");

            var txt1LocErr: HTMLDivElement = <HTMLDivElement>document.getElementById("txt1LocErr");
            var txt2LocErr: HTMLDivElement = <HTMLDivElement>document.getElementById("txt2LocErr");
            //var txt3LocErr: HTMLDivElement = <HTMLDivElement>document.getElementById("txt3LocErr");
            txt1LocErr.style.display = "none";
            txt2LocErr.style.display = "none";
            //txt3LocErr.style.display = "none";
            
            var iScrollY: number = $(".reget-body").scrollTop() + window.pageYOffset;//window.scrollY;;
            var iTop: number = iScrollY + Math.floor(divRow.getBoundingClientRect().top) + Math.floor(divRow.scrollHeight);

            var iScrollX: number = $(".reget-body").scrollLeft();
            var divCol: HTMLDivElement = target.parentNode.parentNode.parentNode.parentNode.parentNode;
            var iScrollX: number = $(".reget-body").scrollLeft();
            var iLeft: number = iScrollX + Math.floor(divCol.getBoundingClientRect().left);
            

            divTranslation.style.top = iTop + "px";
            divTranslation.style.left = iLeft + "px";
            divTranslation.style.visibility = "visible";
            divTranslation.style.opacity = "1";
            
            var divRegetToolTip: HTMLDivElement = <HTMLDivElement>document.getElementById("divRegetToolTip");
            var iTooltipLeft = iLeft + Math.floor(divTranslation.clientWidth / 2);
            divRegetToolTip.style.visibility = "visible";
            divRegetToolTip.style.opacity = "1";
            divRegetToolTip.style.top = iTop + "px";
            divRegetToolTip.style.left = iTooltipLeft + "px";
            
        }   

        private localtext1changed(strValue: string): void {
            if (this.isStringValueNullOrEmpty(strValue)) {
                $("#txt1LocErr").slideDown("slow");
                this.translateErrorMsg = this.locMissingMandatoryText;
            } else {
                $("#txt1LocErr").slideUp("slow");
                this.translateErrorMsg = null;
            }
                        
            //var input = this.angScopeAny.frmParentPg['txtLang1'];
            //input.$error.required = true;
            //input.$setValidity("required", false);
            //var divError: HTMLDivElement = <HTMLDivElement>document.getElementById("txt1LocErr");
            //divError.style.display = "block";
        }   
                
        
        //private localtext3changed(strValue: string): void {
        //    if (this.isStringValueNullOrEmpty(strValue)) {
        //        $("#txt3LocErr").slideDown("slow");
        //    } else {
        //        $("#txt3LocErr").slideUp("slow");
        //    }
        //}  

        private localtext2changed(strValue: string): void {
            if (this.isStringValueNullOrEmpty(strValue)) {
                $("#txt2LocErr").slideDown("slow");
                this.translateErrorMsg = this.locMissingMandatoryText;
            } else {
                $("#txt2LocErr").slideUp("slow");
                this.translateErrorMsg = null;
            }
                        
        }

        //public revetTranslations(): void {
        //    this.translateText1 = this.translateText1Orig;
        //    this.translateText2 = this.translateText2Orig;
        //}

        private isTranslationValid(): boolean {

            if (this.isValueNullOrUndefined(this.translateText1)) {
                return false;
            }

            if (!this.isDefaultLang && this.isValueNullOrUndefined(this.translateText2)) {
                return false;
            }

            return true;
        }

        

        private saveTranslation(): void {
            this.translateErrorMsg = null;
            if (!this.isTranslationValid()) {
                this.displayErrorMsg(this.locMissingMandatoryText);
                this.translateErrorMsg = this.locMissingMandatoryText;
                return;
            }
            this.hideTranslation();
            this.saveTranslationToDb();

            //this.translateText1Orig = null;
            //this.translateText2Orig = null;
        }

        private closeTranslation(): void {
           // this.revetTranslations();
            this.hideTranslation();
        }  

        public hideTranslation() : void {
            var divTranslation: HTMLDivElement = <HTMLDivElement>document.getElementById("divtranslatedirective");
            if (this.isValueNullOrUndefined(divTranslation)) {
                return;
            }
            divTranslation.style.visibility = "hidden";

            var divRegetToolTip: HTMLDivElement = <HTMLDivElement>document.getElementById("divRegetToolTip");
            divRegetToolTip.style.opacity = "0";
            divRegetToolTip.style.visibility = "hidden";

            //this.translateText1Orig = null;
            //this.translateText2Orig = null;
        }
        //******************************************************************
    }

    

    
}