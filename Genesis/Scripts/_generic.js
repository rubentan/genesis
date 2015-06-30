//System.Web.Mvc

$(function () {
    initGlobal();
});

var initGlobal = function() {

    var currentBoxNumber = 0;
    $(".formItem").keyup(function (event) {
        if (event.keyCode == 13) {
            textboxes = $("input.formItem", "select.formItem");
            currentBoxNumber = textboxes.index(this);
            console.log(textboxes.index(this));
            if (textboxes[currentBoxNumber + 1] != null) {
                nextBox = textboxes[currentBoxNumber + 1];
                nextBox.focus();
                nextBox.select();
                event.preventDefault();
                return false;
            }
        }
    });


    //
    // Custom Binding for loading
    //
    ko.bindingHandlers.loadingWhen = {
        init: function (element) {
            var
                $element = $(element),
                currentPosition = $element.css("position"),
                $loader = $("<div>").addClass("loader").hide();


            //add the loader
            $element.append($loader);
            //$element.show();
            //make sure that we can absolutely position the loader against the original element
            if (currentPosition == "auto" || currentPosition == "static")
                $element.css("position", "relative");

            //center the loader
            $loader.css({
                position: "absolute",
                top: "50%",
                left: "50%",
                "margin-left": -($loader.width() / 2) + "px",
                "margin-top": -($loader.height() / 2) + "px"
            });
        },
        update: function (element, valueAccessor) {
            var isLoading = ko.utils.unwrapObservable(valueAccessor()),
                $element = $(element),
                $childrenToHide = $element.children(":not(div.loader)"),
                $loader = $element.find("div.loader");

            if (isLoading) {
                $childrenToHide.css("visibility", "hidden").attr("disabled", "disabled");
                $loader.show();
            }
            else {
                $loader.fadeOut("fast");
                $childrenToHide.css("visibility", "visible").removeAttr("disabled");
            }
        }
    };
};

var getFilters = function () {
    var queryString = "?";
    $(".filterContainer input,.filterContainer  select").each(function () {

        queryString += $(this).attr('name') + '=' + $(this).val() + '&';

    });

    return queryString.substring(0, queryString.length - 1);

};

var bindSelect = function (select, source) {
    
    $.ajax({       
        url: source,
        type: 'POST',
        dataType: 'json',
        success: function (result) {

            $.each(result, function () {
                select.append($("<option />").val(this.value).text(this.text));
            });

        },
        error: function () {  alert("error")}
    });

    ;
};

var getQueryStringParam = function(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
};


var convertDateFromJson = function (jsonDate) {
    if (jsonDate == null || jsonDate == undefined) return;
    var dateString = jsonDate.substr(6);
    var currentTime = new Date(parseInt(dateString));
    var month = currentTime.getMonth() + 1;
    var day = currentTime.getDate();
    var year = currentTime.getFullYear();
    var date = month + "/" + day + "/" + year;

    return date;
};

var convertDateTimeFromJson = function (jsonDate) {
   // alert("date: " + jsonDate);

    var date = "";
    if (jsonDate != null)
    {
        var dateString = jsonDate.substr(6);
        var currentTime = new Date(parseInt(dateString));
        var month = currentTime.getMonth() + 1;
        var day = currentTime.getDate();
        var year = currentTime.getFullYear();
        var hour = currentTime.getHours();
        var minutes = currentTime.getMinutes();
        date = month + "/" + day + "/" + year + " " + hour + ":" + minutes;
    }
    return date;
};


var convertToCurrency = function(stringOriginal) {
    //alert(stringOriginal);
    stringOriginal = Math.abs(stringOriginal);
    var stringCurrency = stringOriginal.toFixed(4).replace(/\d(?=(\d{3})+\.)/g, '$&,');

    stringCurrency = "₱ " + stringCurrency;
    return stringCurrency;
};

var convertToCurrencyNoSign = function (stringOriginal) {
    //alert(stringOriginal);
    stringOriginal = Math.abs(stringOriginal);
    var stringCurrency = stringOriginal.toFixed(4).replace(/\d(?=(\d{3})+\.)/g, '$&,');

    return stringCurrency;
};


