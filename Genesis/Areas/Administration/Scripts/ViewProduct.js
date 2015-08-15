
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
    var productId = $("#productId").val();
    this.isLoading1 = ko.observable(false);
    this.isLoading2 = ko.observable(false);
    this.isLoading3 = ko.observable(false);
    this.isLoading4 = ko.observable(false);
    _self.Transactions = ko.observableArray();
    _self.PriceHistory = ko.observableArray();
    _self.ProductInfo = ko.observable();

    _self.Purchases = ko.observableArray();
    _self.Sales = ko.observableArray();

    _self.GetSales = function () {
        _self.isLoading4(true);
        var dataUrl = $("#hdnSalesUrl").attr("data-url");
        $.ajax({
            //url: '/Administration/Product/GetProductTransactions',
            url: dataUrl,
            type: 'POST',
            async: true,
            dataType: 'json',
            data: { id: productId },
            success: function (d) {
                $(".problemAjax").hide();
                _self.isLoading4(false);
                _self.Sales(d);
            },
            error: function () { $(".problemAjax").show(); }
        });
    };

    _self.GetPurchases = function () {
        _self.isLoading3(true);
        var dataUrl = $("#hdnPurchasesUrl").attr("data-url");
        $.ajax({
            //url: '/Administration/Product/GetProductTransactions',
            url: dataUrl,
            type: 'POST',
            async: true,
            dataType: 'json',
            data: { id: productId },
            success: function (d) {
                $(".problemAjax").hide();
                _self.isLoading3(false);
                _self.Purchases(d);
            },
            error: function () { $(".problemAjax").show(); }
        });
    };

    _self.GetProductInfo = function () {
        _self.isLoading3(true);
        var dataUrl = $("#hdnProductInfoUrl").attr("data-url");
        $.ajax({
            //url: '/Administration/Product/GetProductInfo',
            url:dataUrl,
            type: 'POST',
            async: true,
            data: { id: productId },
            dataType: 'json',
            success: function (d) {
                $(".problemAjax").hide();
                _self.isLoading3(false);
                //alert(JSON.stringify(d));
                _self.ProductInfo(d);

            },
            error: function () { $(".problemAjax").show(); }
        });
    };

    _self.GetTransactions = function() {
        _self.isLoading1(true);
        var dataUrl = $("#hdnTransactionsUrl").attr("data-url");
        $.ajax({
            //url: '/Administration/Product/GetProductTransactions',
            url: dataUrl,
            type: 'POST',
            async: true,
            dataType: 'json',
            data: { id: productId },
            success: function(d) {
                $(".problemAjax").hide();
                _self.isLoading1(false);
                _self.Transactions(d);
            },
            error: function() { $(".problemAjax").show(); }
        });
    };

    _self.GetPriceHistory = function () {
            _self.isLoading2(true);
            var dataUrl = $("#hdnPriceHistoryUrl").attr("data-url");
            $.ajax({
                //url: '/Administration/Product/GetProductPriceHistory',
                url: dataUrl,
                type: 'POST',
                async: true,
                dataType: 'json',
                data: { id: productId },
                success: function(d) {
                    $(".problemAjax").hide();
                    _self.isLoading2(false);
                    _self.PriceHistory(d);
                },
                error: function() { $(".problemAjax").show(); }
            });
        };

    _self.ViewDocument = function (Transactions) {
        
        if (Transactions.documentType == 1) {
            var dataUrl1 = $("#hdnViewSalesInvoiceUrl").attr("data-url");
            window.location =dataUrl1 + "?id=" + Transactions.documentId;
            //window.location = "/Modules/Sales/EditBranchSalesInvoice?id=" + Transactions.documentId;
        }
        else if (Transactions.documentType == 2) {
            var dataUrl2 = $("#hdnViewPurchaseOrderUrl").attr("data-url");
            window.location =dataUrl2 + "?id=" + Transactions.documentId;
            //window.location = "Modules/Purchase/EditBranchPurchaseOrder?id=" + Transactions.documentId;
        }

    };


    _self.GetPriceHistory();
    //_self.GetTransactions();
    _self.GetSales();
    _self.GetPurchases();
    _self.GetProductInfo();
};





