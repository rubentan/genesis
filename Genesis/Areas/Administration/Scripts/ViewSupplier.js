
$(function () {

    vm = new ViewModel();
    ko.applyBindings(vm);

});


var backToList = function () {
    var dataUrl = $("#hdnBackToListUrl").attr("data-url");
    window.location = "/Administration/Supplier/Suppliers";
};

var ViewModel = function() {
    var _self = this;
    var supplierId = $("#supplierId").val();
    this.isLoading1 = ko.observable(false);
    this.isLoading2 = ko.observable(false);
    this.isLoading3 = ko.observable(false);
    _self.PurchaseOrders = ko.observableArray();
    _self.Payments = ko.observableArray();
    _self.SupplierInfo = ko.observable();


    _self.GetSupplierInfo = function () {
        _self.isLoading3(true);
        var dataUrl = $("#hdnGetSupplierInfoUrl").attr("data-url");

        $.ajax({
            //url: '/Administration/Supplier/GetSupplierInfo',
            url: dataUrl,
            type: 'POST',
            async: true,
            data: {id:supplierId},
            dataType: 'json',
            success: function (d) {
                $(".problemAjax").hide();
                _self.isLoading3(false);
                //alert(JSON.stringify(d));
                _self.SupplierInfo(d);

            },
            error: function () { $(".problemAjax").show(); }
        });
    };

    _self.GetPurchaseOrders = function() {
        _self.isLoading1(true);
        var dataUrl = $("#hdnLoadPurchaseOrdersUrl").attr("data-url");

        $.ajax({
            //url: '/Administration/Supplier/GetSupplierPurchaseOrders',
            url: dataUrl,
            type: 'POST',
            async: true,
            dataType: 'json',
            data: { id: supplierId },
            success: function(d) {
                $(".problemAjax").hide();
                _self.isLoading1(false);
                _self.PurchaseOrders(d);
            },
            error: function() { $(".problemAjax").show(); }
        });
    };

    _self.GetPayments = function() {
            _self.isLoading2(true);
            var dataUrl = $("#hdnLoadPaymentsUrl").attr("data-url");

            $.ajax({
                //url: '/Administration/Supplier/GetSupplierPayments',
                url: dataUrl,
                type: 'POST',
                async: true,
                dataType: 'json',
                data: { id: supplierId },
                success: function(d) {
                    $(".problemAjax").hide();
                    _self.isLoading2(false);
                    _self.Payments(d);
                },
                error: function() { $(".problemAjax").show(); }
            });
        };

    _self.ViewPayment = function (Payments) {
        var dataUrl = $("#hdnViewPaymentUrl").attr("data-url");
        window.location = dataUrl + "?id=" + Payments.paymentId;
        //window.location = "/Modules/Sales/ViewBranchReceivable?receivableId=" + Payments.paymentId;
    };

    _self.ViewPurchaseOrder = function (PurchaseOrders) {
        var dataUrl = $("#hdnPurchaseOrderUrl").attr("data-url");
        window.location = dataUrl + "?id=" + PurchaseOrders.documentId;
        //window.location = "/Modules/Sales/AddBranchSalesInvoice?documentId=" + PurchaseOrders.documentId;
    };

    _self.GetPayments();
    _self.GetPurchaseOrders();
    _self.GetSupplierInfo();
};





