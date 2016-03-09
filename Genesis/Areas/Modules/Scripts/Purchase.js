$(function () {
    //if (jQuery().datepicker) {
    //    $('.date-picker').datepicker({
    //        rtl: Metronic.isRTL(),
    //        orientation: "left",
    //        autoclose: true
    //    });
    //}
    if (jQuery().datepicker) {
        $('.date-from').datepicker({
            rtl: Metronic.isRTL(),
            orientation: "left",
            autoclose: true
        }).datepicker("setDate", new Date());

        $('.date-to').datepicker({
            rtl: Metronic.isRTL(),
            orientation: "left",
            autoclose: true
        }).datepicker("setDate", new Date());
    }

    vm = new viewModel();
    ko.applyBindings(vm);

});


var viewModel = function () {

    var _self = this;
    this.isLoading = ko.observable(false);
    this.isLoadingProducts = ko.observable(false);
    _self.documents = ko.observableArray();
    _self.products = ko.observableArray();
    _self.records = ko.observable("0");
    _self.page = ko.observable("1");
    _self.documentNumber = ko.observable();

    _self.pages = ko.pureComputed(function () {
        return Math.ceil(_self.records() / $('#recordPerPage').val());
    }, this);

    _self.nextPage = function () {
        if (_self.page() == _self.pages()) {
            _self.page("1");
        } else {
            _self.page(parseInt(_self.page()) + parseInt(1));
        }
        _self.asyncOperation();
    };

    _self.previousPage = function () {
        if (_self.page() == 1) {
            _self.page(_self.pages());
        } else {
            _self.page(parseInt(_self.page()) - parseInt(1));
        }
        _self.asyncOperation();
    };

    _self.filterSubmit = function () {
        _self.page("1");
        _self.asyncOperation();
    };

    _self.exportPurchases = function () {

        var documentNumber = $('#documentNumber').val();
        var supplierName = $('#supplierName').val();
        var supplierCode = $('#supplierCode').val();
        var dateFrom = $('#dateFrom').val();
        var dateTo = $('#dateTo').val();
        var dataUrl = $("#hdnExportBranchPurchasesUrl").attr("data-url");
        //alert(dataUrl + "?documentNumber=" + documentNumber + "&clientName=" + clientName + "&clientCode=" + clientCode + "&dateFrom=" + dateFrom + "&dateTo=" + dateTo);
        window.location = dataUrl + "?documentNumber=" + documentNumber + "&supplierName=" + supplierName + "&supplierCode=" + supplierCode + "&dateFrom=" + dateFrom + "&dateTo=" + dateTo + "&export=true";
    };

    _self.asyncOperation = function () {
        _self.isLoading(true);
        var dataUrl = $("#hdnGetBranchPurchasesUrl").attr("data-url");
        $.ajax({
            //url: '/Modules/Purchase/GetAllPurchases' + getFilters(),
            url: dataUrl + getFilters(),
            type: 'POST',
            dataType: 'json',
            success: function (d) {
                $(".problemAjax").hide();
                _self.isLoading(false);
                if (Object.keys(d).length > 0) {
                    _self.records(d[0]["records"]);
                } else {
                    _self.records("0");
                    _self.page("1");
                }
                _self.documents(d);
            },
            error: function () {
                $(".problemAjax").show();
                _self.isLoading(false);
            }
        });
    };

    _self.editRow = function (documents) {
        var dataUrl = $("#hdnEditPurchaseOrderUrl").attr("data-url");
        window.location = dataUrl + "?id=" + documents.documentId;
        //window.location = "/Modules/Purchase/EditBranchPurchaseOrder?id=" + documents.documentId;
    };

    _self.addRow = function () {
        var dataUrl = $("#hdnAddPurchaseOrderUrl").attr("data-url");
        window.location = dataUrl;
        //window.location = "/Modules/Sales/AddBranchPurchaseOrder";
    };

    _self.viewProducts = function (documents) {

        _self.asyncOperationProducts(documents.documentId);
        _self.documentNumber(documents.documentNumber);
        $('#ViewProducts').modal('show');

    };

    _self.asyncOperationProducts = function (documentId) {
        _self.isLoadingProducts(true);
        var param = { documentId: documentId };

        $.ajax({
            //url: '/Home/GetReceivables',
            url: $('#hdnGetAllPurchaseItemsUrl').attr('data-url'),
            type: 'POST',
            data: JSON.stringify(param),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (d) {
                $(".problemAjaxProducts").hide();
                _self.isLoadingProducts(false);
                //alert(JSON.stringify(d));
                _self.products(d);
            },
            error: function () {
                $(".problemAjaxProducts").show();
                _self.isLoadingProducts(false);
            }
        });
    };

    _self.ResetFilter = function () {
        $('#documentNumber').val('');
        $('#supplierName').val('');
        $('#supplierCode').val('');
        $('#dateFrom').val('');
        $('#dateTo').val('');
    };

    _self.filterSubmit();
};

