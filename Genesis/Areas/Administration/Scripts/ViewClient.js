
$(function () {


    initButton();

    vm = new ViewModel();
    ko.applyBindings(vm);

    

});

var initButton = function () {


};

var backToList = function () {
    var dataUrl = $("#hdnBackToListUrl").attr("data-url");
    window.location = dataUrl;
};

var ViewModel = function() {
    var _self = this;
    var clientId = $("#clientId").val();
    this.isLoading1 = ko.observable(false);
    this.isLoading2 = ko.observable(false);
    this.isLoading3 = ko.observable(false);
    _self.SalesInvoices = ko.observableArray();
    _self.Payments = ko.observableArray();
    _self.ClientInfo = ko.observable();


    _self.GetClientInfo = function () {
        var dataUrl = $("#hdnGetClientInfoUrl").attr("data-url");
        _self.isLoading3(true);
        $.ajax({
            //url: '/Administration/Client/GetClientInfo',
            url: dataUrl,
            type: 'POST',
            async: true,
            data: {id:clientId},
            dataType: 'json',
            success: function (d) {
                $(".problemAjax").hide();
                _self.isLoading3(false);
                //alert(JSON.stringify(d));
                _self.ClientInfo(d);

            },
            error: function () { $(".problemAjax").show(); }
        });
    };

    _self.GetSalesInvoices = function() {
        _self.isLoading1(true);
        var dataUrl = $("#hdnGetClientSalesInvoicesUrl").attr("data-url");
        $.ajax({
            //url: '/Administration/Client/GetClientSalesInvoices',
            url: dataUrl,
            type: 'POST',
            async: true,
            dataType: 'json',
            data: {id:clientId},
            success: function(d) {
                $(".problemAjax").hide();
                _self.isLoading1(false);
                _self.SalesInvoices(d);
            },
            error: function() { $(".problemAjax").show(); }
        });
    };

    _self.GetPayments = function() {
            _self.isLoading2(true);
            var dataUrl = $("#hdnGetClientPaymentsUrl").attr("data-url");
            $.ajax({
                //url: '/Administration/Client/GetClientPayments',
                url: dataUrl,
                type: 'POST',
                async: true,
                dataType: 'json',
                data: { id: clientId },
                success: function(d) {
                    $(".problemAjax").hide();
                    _self.isLoading2(false);
                    _self.Payments(d);
                },
                error: function() { $(".problemAjax").show(); }
            });
        };

    _self.ViewPayment = function(Payments) {
        var dataUrl = $("#hdnViewPaymentUrl").attr("data-url");
        window.location = dataUrl+"?receivableId="+Payments.documentId;
        //window.location = "/Modules/Sales/ViewBranchReceivable?receivableId=" + Payments.receivableId;
    };

    _self.ViewSalesInvoice = function (SalesInvoices) {
        var dataUrl = $("#hdnViewSalesInvoiceUrl").attr("data-url");
        window.location = dataUrl+"?id="+SalesInvoices.documentId;
        //window.location = "/Modules/Sales/AddBranchSalesInvoice?documentId=" + SalesInvoices.documentId;
    };

    _self.GetPayments();
    _self.GetSalesInvoices();
    _self.GetClientInfo();
};





