$(function () {
    if (jQuery().datepicker) {
        $('.date-picker').datepicker({
            rtl: Metronic.isRTL(),
            orientation: "left",
            autoclose: true
        });
    }

    vm = new viewModel();
    ko.applyBindings(vm);
});

var viewModel = function () {
    
    var _self = this;
    this.isLoading = ko.observable(false);
    _self.documents = ko.observableArray();

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
                _self.documents(d);
            },
            error: function () {
                $(".problemAjax").show();
                _self.isLoading(false);
            }
        });
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

    _self.asyncOperation();
};
