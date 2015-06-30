var vm = {};

$(function () {
    if (jQuery().datepicker) {
        $('.date-picker').datepicker({
            rtl: Metronic.isRTL(),
            orientation: "left",
            autoclose: true
        });
        //$('body').removeClass("modal-open"); // fix bug when inline picker is used in modal
    }

    vm = viewModel();
    ko.applyBindings(vm);
});

var viewModel = function () {

    var self = this;
    this.isLoading = ko.observable(false);
    self.listReceivables = ko.observableArray();

    self.asyncOperation = function () {
        self.isLoading(true);
        var dataUrl = $("#hdnGetBranchReceivablesUrl").attr("data-url");
        $.ajax({
            //url: '/Modules/Sales/GetAllReceivables' + getFilters(),
            url: dataUrl+getFilters(),
            type: 'POST',
            dataType: 'json',
            success: function (d) {
                self.isLoading(false);
                self.listReceivables(d);
            },
            error: function () {
                $(".problemAjax").show();
                self.isLoading(false);
            }
        });
    };

    self.editRow = function (listReceivables) {
        var dataUrl = $("#hdnEditSalesReceivableUrl").attr("data-url");
        window.location = dataUrl+"?id="+listReceivables.receivableId;
        //window.location = "/Modules/Sales/EditBranchReceivable?id=" + listReceivables.receivableId;
    };

    self.addRow = function () {
        var dataUrl = $("#hdnAddSalesReceivablesUrl").attr("data-url");
        window.location = dataUrl;
        //window.location = "/Modules/Sales/NewBranchReceivable";
    };



    self.ResetFilter = function () {
        $('#referenceNumber').val('');
        $('#clientName').val('');
        $('#clientCode').val('');
        //$('#dateFrom').val('');
        //$('#dateTo').val('');
    };

    self.asyncOperation();
};