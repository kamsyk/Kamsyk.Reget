var URL_PARAMETER_DELIMITER = "|";
var URL_PARAMETER_VALUE_DELIMITER = "~";

//$(".md-dialog-container").

$("#aRegetInfo").click(function (event) {
    //alert("asdas");
    //alert(window.GetRootUrl());

    var isBtnMenu = $("#btnMenu").is(":visible");
    //alert(isMenuBarVisible);
    if (isBtnMenu) {
        //this collapce menu bar - it is because of Info button, if the button is clicked the info popup is displayed, 
        //in case the app is displayed on mobile phone (width < 768) the expnaded menu overlaps the displayed popup
        //this is a workaround, and works only for mobule phone mode otherwise screen blinks

        //commented out because after the menu is collapsed the click does not work then

       // $(".navbar-collapse").toggle('in');
        
    }
});

//$(window).load(function () { does not work, there must be "on"
$(window).on('load', function () {
   
    HideLoader();
    
});

//$(window).load(function () {
//    $(".reget-se-pre-con").fadeOut("slow");
//});


$(".reget-display-loader").click(function () {
    ShowLoader();
    //$(".reget-se-pre-con").show();
    //setTimeout(showSpinner, 1);
        
});

function ShowLoader(isError) {
    if (IsValueNullOrUndefined(isError) || isError === false) {
        $(".reget-se-pre-con").show();
    }
}

function ShowLoaderBoxOnly(isError) {
    if (IsValueNullOrUndefined(isError) || isError === false) {
        $(".reget-se-pre-con-box-only").show();
    }
}


function HideLoader() {
    $(".reget-se-pre-con").fadeOut();
    $(".reget-se-pre-con-box-only").fadeOut();
}

function GetRegetRootUrl() {
    var rootUrl = window.location.protocol + "//" + window.location.host;

    if (window.location.host.toLowerCase().indexOf('localhost') < 0) {
        var appName = window.location.pathname.split('/')[1];
        //var lastChar = appName.substr(appName.length - 1);
        //if (lastChar !== "/") {
        //    rootUrl += "/";
        //}

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



function validateIntNumber(e) {
    if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110]) !== -1 ||
        // Allow: Ctrl/cmd+A
            (e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
        // Allow: Ctrl/cmd+C
            (e.keyCode === 67 && (e.ctrlKey === true || e.metaKey === true)) ||
        // Allow: Ctrl/cmd+X
            (e.keyCode === 88 && (e.ctrlKey === true || e.metaKey === true)) ||
        // Allow: home, end, left, right
            (e.keyCode >= 35 && e.keyCode <= 39)) {
        // let it happen, don't do anything
        return;
    }
    // Ensure that it is a number and stop the keypress
    if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
        e.preventDefault();
    }


}

function validateDecimalNumber(e, strDecimailSep) {
    
    if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110]) !== -1 ||
        // Allow: Ctrl/cmd+A
            (e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
        // Allow: Ctrl/cmd+C
            (e.keyCode === 67 && (e.ctrlKey === true || e.metaKey === true)) ||
        // Allow: Ctrl/cmd+X
            (e.keyCode === 88 && (e.ctrlKey === true || e.metaKey === true)) ||
        // Allow: home, end, left, right
            (e.keyCode >= 35 && e.keyCode <= 39)) {
        // let it happen, don't do anything
        return;
    }

    if (IsStringValueNullOrEmpty(strDecimailSep)) {
        strDecimailSep = $("#DecimalSeparator").val();
    }

    if (!IsStringValueNullOrEmpty(strDecimailSep) && e.key === strDecimailSep) {
        return;
    }

    // Ensure that it is a number and stop the keypress
    if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
        e.preventDefault();
    }
}

function validateDecimalNumberSimple(e) {
    var decimalSeparator = document.getElementById("DecimalSeparator").value;
    validateDecimalNumber(e, decimalSeparator);
}

function validatePhoneNumber(e) {
    if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110]) !== -1 ||
        // Allow: Ctrl/cmd+A
        (e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
        // Allow: Ctrl/cmd+C
        (e.keyCode === 67 && (e.ctrlKey === true || e.metaKey === true)) ||
        // Allow: Ctrl/cmd+V
        (e.keyCode === 86 && (e.ctrlKey === true || e.metaKey === true)) ||
        // Allow: Ctrl/cmd+X
        (e.keyCode === 88 && (e.ctrlKey === true || e.metaKey === true)) ||
        // Allow: home, end, left, right
        (e.keyCode >= 35 && e.keyCode <= 39)) {
        // let it happen, don't do anything
        return;
    }

    if (e.key === "+"
        || e.key === " "
        || e.key === "("
        || e.key === ")") {
        return;
    }

    // Ensure that it is a number and stop the keypress
    if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
        e.preventDefault();
    }


}

function displayElement(elementId) {
    $("#" + elementId).show();
}

function hideElement(elementId) {
    $("#" + elementId).hide();
}

function displayElementSlow(elementId) {
    $("#" + elementId).show("slow");
}

function hideElementSlow(elementId) {
    $("#" + elementId).slideUp();
}

function sleep(milliseconds) {
    var start = new Date().getTime();
    for (var i = 0; i < 1e7; i++) {
        if ((new Date().getTime() - start) > milliseconds) {
            break;
        }
    }
}

//$('.dropdown-submenu').click(function (event) {
//    alert('event');
//    event.stopPropagation();
//});

//$('.notActiveSelectSubmenuCss').click(function (event) {
//    alert('event');
//    event.stopPropagation();
//});

function TogleSubmenu(event, submenuId) {
    if ($("#" + submenuId).is(':visible')) {
        $("#" + submenuId).hide();
        
    } else {
        $("#" + submenuId).show();
        
    }

    StopPropagation(event);
}



function StopPropagation(event) {
    //alert('event');
    event.stopPropagation();
}

function IsStringValueNullOrEmpty(strValue) {
    if (IsValueNullOrUndefined(strValue) || String(strValue).trim().length === 0) {
        return true;
    } else {
        return false;
    }
}

function IsValueNullOrUndefined(strValue) {
    if (strValue === null || strValue === undefined || strValue === "" || strValue === "null") {
        return true;
    } else {
        return false;
    }
}

function IsTrue(strValue) {
    if (IsStringValueNullOrEmpty(strValue)) {
        return false;
    }

    return (strValue.toLowerCase().trim() === "true");
}

function IsSmartPhone() {
    var winWidth = window.innerWidth;
    if (winWidth < 767) {
        return true;
    } else {
        return false;
    }
}

function ConvertDateToString(dDate) {
    if (IsValueNullOrUndefined(dDate)) {
        return null;
    }

    var dd = dDate.getDate().toString();
    if (dd.length < 2) {
        dd = "0" + dd;
    }

    var mm = (dDate.getMonth() + 1).toString();
    if (mm.length < 2) {
        mm = "0" + mm;
    }

    var yyyy = dDate.getFullYear().toString();

    return yyyy + "-" + mm + "-" + dd;
}

function debounce(func, interval) {
    var timeout;
    return function () {
        var context = this, args = arguments;
        var later = function () {
            timeout = null;
            func.apply(context, args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, interval || 200);
    };
}



