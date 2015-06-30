$(function () {
    //if (jQuery().datepicker) {
    //    $('.date-picker').datepicker({
    //        rtl: Metronic.isRTL(),
    //        orientation: "left",
    //        autoclose: true
    //    });
    //}

    vm = new viewModel();
    ko.applyBindings(vm);

});


var viewModel = function () {

    var _self = this;
    this.isLoading = ko.observable(false);
    _self.documents = ko.observableArray();

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


    _self.ResetFilter = function () {
        $('#documentNumber').val('');
        $('#supplierName').val('');
        $('#supplierCode').val('');
        $('#dateFrom').val('');
        $('#dateTo').val('');
    };

    _self.asyncOperation();
};

