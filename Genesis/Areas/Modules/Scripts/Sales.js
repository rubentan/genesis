$(function () {
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
    //"+2d"
   

    vm = new viewModel();
    ko.applyBindings(vm);
});

var viewModel = function () {
    
    var _self = this;
    this.isLoading = ko.observable(false);
    _self.documents = ko.observableArray();
    _self.records = ko.observable("0");
    _self.page = ko.observable("1");

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

    _self.asyncOperation = function () {
        _self.isLoading(true);
        var dataUrl = $("#hdnGetBranchSalesUrl").attr("data-url");
        $.ajax({
            //url: '/Modules/Sales/GetAllSales' + getFilters(),
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

    _self.exportSales = function () {

        var documentNumber = $('#txtDocumentNo').val();
        var clientName = $('#clientName').val();
        var clientCode = $('#clientCode').val();
        var dateFrom = $('#dateFrom').val();
        var dateTo = $('#dateTo').val();
        var dataUrl = $("#hdnExportBranchSalesUrl").attr("data-url");
        //alert(dataUrl + "?documentNumber=" + documentNumber + "&clientName=" + clientName + "&clientCode=" + clientCode + "&dateFrom=" + dateFrom + "&dateTo=" + dateTo);
        window.location = dataUrl + "?documentNumber=" + documentNumber + "&clientName=" + clientName + "&clientCode=" + clientCode + "&dateFrom=" +dateFrom+ "&dateTo=" + dateTo;
    };



    _self.editRow = function (documents) {
        var dataUrl = $("#hdnEditSalesInvoiceUrl").attr("data-url");
        window.location = dataUrl + "?id=" + documents.documentId;
        //window.location = "/Modules/Sales/EditBranchSalesInvoice?id=" + documents.documentId;
    };

    _self.addRow = function () {
        var dataUrl = $("#hdnAddSalesInvoiceUrl").attr("data-url");
        window.location = dataUrl;
        //window.location = "/Modules/Sales/AddBranchSalesInvoice";
    };



    _self.ResetFilter = function () {
        $('#txtDocumentNo').val('');
        $('#clientName').val('');
        $('#clientCode').val('');
        $('#dateFrom').val('');
        $('#dateTo').val('');
    };

    _self.filterSubmit();
};
