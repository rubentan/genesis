﻿var vm = {};

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
    self.listPayables = ko.observableArray();
    self.records = ko.observable("0");
    self.page = ko.observable("1");

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

    self.exportPayments = function () {

        var referenceNumber = $('#referenceNumber').val();
        var supplierName = $('#supplierName').val();
        var supplierCode = $('#supplierCode').val();
        var dateFrom = $('#dateFrom').val();
        var dateTo = $('#dateTo').val();
        var dataUrl = $("#hdnExportBranchPaymentsUrl").attr("data-url");
        //alert(dataUrl + "?documentNumber=" + documentNumber + "&clientName=" + clientName + "&clientCode=" + clientCode + "&dateFrom=" + dateFrom + "&dateTo=" + dateTo);
        window.location = dataUrl + "?referenceNumber=" + referenceNumber+ "&supplierName=" + supplierName + "&supplierCode=" + supplierCode + "&dateFrom=" + dateFrom + "&dateTo=" + dateTo;
    };


    self.asyncOperation = function () {
        self.isLoading(true);
        var dataUrl = $("#hdnGetBranchPaymentsUrl").attr("data-url");
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
                self.listPayables(d);
            },
            error: function () {
                $(".problemAjax").show();
                self.isLoading(false);
            }
        });
    };

    self.editRow = function (listPayables) {
        var dataUrl = $("#hdnEditPurchasePaymentUrl").attr("data-url");
        window.location = dataUrl+"?id="+listPayables.receivableId;
        //window.location = "/Modules/Sales/EditBranchReceivable?id=" + listPayables.receivableId;
    };

    self.addRow = function () {
        var dataUrl = $("#hdnAddPurchasePaymentUrl").attr("data-url");
        window.location = dataUrl;
        //window.location = "/Modules/Sales/NewBranchReceivable";
    };



    self.ResetFilter = function () {
        $('#referenceNumber').val('');
        $('#supplierName').val('');
        $('#supplierCode').val('');
        //$('#dateFrom').val('');
        //$('#dateTo').val('');
    };

    self.filterSubmit();
};