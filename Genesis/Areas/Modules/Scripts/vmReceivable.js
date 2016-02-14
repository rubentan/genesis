var vm = {};

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

    vm = viewModel();
    ko.applyBindings(vm);
});

var viewModel = function () {

    var self = this;
    this.isLoading = ko.observable(false);
    this.isLoadingDocuments = ko.observable(false);
    self.listReceivables = ko.observableArray();
    self.documents = ko.observableArray();
    self.records = ko.observable("0");
    self.page = ko.observable("1");
    self.receivableNumber = ko.observable();

    self.pages = ko.pureComputed(function () {
        //alert(self.records() + " : " + $('#recordPerPage').val() + " = " + Math.ceil(self.records() / $('#recordPerPage').val()) );
        return Math.ceil(self.records() / $('#recordPerPage').val());
    }, this);

    self.nextPage = function () {
        if (self.page() == self.pages()) {
            self.page("1");
        } else {
            self.page(parseInt(self.page()) + parseInt(1));
        }
        self.asyncOperation();
    };

    self.previousPage = function () {
        if (self.page() == 1) {
            self.page(self.pages());
        } else {
            self.page(parseInt(self.page()) - parseInt(1));
        }
        self.asyncOperation();
    };

    self.filterSubmit = function () {
        self.page("1");
        self.asyncOperation();
    };

    self.exportReceivables = function () {

        var referenceNumber = $('#referenceNumber').val();
        var clientName = $('#clientName').val();
        var clientCode = $('#clientCode').val();
        var dateFrom = $('#dateFrom').val();
        var dateTo = $('#dateTo').val();
        var dataUrl = $("#hdnExportBranchReceivablesUrl").attr("data-url");
        //alert(dataUrl + "?documentNumber=" + documentNumber + "&clientName=" + clientName + "&clientCode=" + clientCode + "&dateFrom=" + dateFrom + "&dateTo=" + dateTo);
        window.location = dataUrl + "?referenceNumber=" + referenceNumber+ "&clientName=" + clientName + "&clientCode=" + clientCode + "&dateFrom=" + dateFrom + "&dateTo=" + dateTo;
    };


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
                if (Object.keys(d).length > 0) {
                    self.records(d[0]["records"]);
                } else {
                    self.records("0");
                    self.page("1");
                }
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

    self.viewDocuments = function (listReceivables) {

        self.asyncOperationProducts(listReceivables.receivableId);
        self.receivableNumber(listReceivables.referenceNumber);
        $('#ViewDocuments').modal('show');

    };

    self.asyncOperationProducts = function (receivableId) {
        self.isLoadingDocuments(true);
        var param = { receivableId: receivableId };

        $.ajax({
            //url: '/Home/GetReceivables',
            url: $('#hdnGetAllReceivableItemsUrl').attr('data-url'),
            type: 'POST',
            data: JSON.stringify(param),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (d) {
                $(".problemAjaxDocuments").hide();
                self.isLoadingDocuments(false);
                //alert(JSON.stringify(d));
                self.documents(d);
            },
            error: function () {
                $(".problemAjaxDocuments").show();
                self.isLoadingDocuments(false);
            }
        });
    };

    self.ResetFilter = function () {
        $('#referenceNumber').val('');
        $('#clientName').val('');
        $('#clientCode').val('');
        //$('#dateFrom').val('');
        //$('#dateTo').val('');
    };

    self.filterSubmit();
};