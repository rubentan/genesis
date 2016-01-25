$(function () {


    vm = new ViewModel();
    ko.applyBindings(vm);
});




var ViewModel = function () {
    var _self = this;
    this.isLoadingReceivables = ko.observable(false);
    this.isLoadingPayables = ko.observable(false);
    this.isLoadingReorders = ko.observable(false);
    _self.receivables = ko.observableArray();
    _self.payables = ko.observableArray();
    _self.reorders = ko.observableArray();
    

    _self.asyncOperationReceivables = function () {
        _self.isLoadingReceivables(true);
        
        $.ajax({
            //url: '/Home/GetReceivables',
            url: $('#portletReceivables').attr('data-url'),
            type: 'POST',
            dataType: 'json',
            success: function (d) {
                $(".problemAjax").hide();
                _self.isLoadingReceivables(false);
                //alert(JSON.stringify(d));
                _self.receivables(d);
            },
            error: function() {
                $(".problemAjax").show();
                _self.isLoadingReceivables(false);
            }
        });
    };

    _self.asyncOperationPayables = function () {
        _self.isLoadingPayables(true);

        $.ajax({
            //url: '/Home/GetPayables',
            url: $('#portletPayables').attr('data-url'),
            type: 'POST',
            dataType: 'json',
            success: function (d) {
                $(".problemAjax").hide();
                _self.isLoadingPayables(false);
                _self.payables(d);
            },
            error: function () {
                $(".problemAjax").show();
                _self.isLoadingPayables(false);
            }
        });
    };

    _self.asyncOperationReorders = function () {
        _self.isLoadingReorders(true);

        $.ajax({
            //url: '/Home/GetReorders',
            url: $('#portletReorders').attr('data-url'),
            type: 'POST',
            dataType: 'json',
            success: function (d) {
                $(".problemAjax").hide();
                _self.isLoadingReorders(false);
                _self.reorders(d);
            },
            error: function () {
                $(".problemAjax").show();
                _self.isLoadingReorders(false);
            }
        });
    };

    _self.viewReceivableDocument = function (receivables) {
        var url = $('#viewReceivableDocumentUrl').attr('data-url') + "?id=" + receivables.documentId;
        window.location = url;
        
    };

    _self.viewPayableDocument = function (payables) {
        //window.location = "/Modules/Purchase/EditBranchPurchaseOrder?id=" + payables.documentId;
        var url = $('#viewPurchaseDocumentUrl').attr('data-url') + "?id=" + payables.documentId;
        window.location = url;
    };

    _self.viewReorderProduct = function (reorders) {
        //window.location = "/Administration/Product/ViewProductDetails?id=" + reorders.productId;
        var url = $('#viewProductDocumentUrl').attr('data-url') + "?id=" + reorders.productId;
        window.location = url;
    };

    _self.viewClient = function (receivables) {
        //window.location = "/Administration/Product/ViewProductDetails?id=" + reorders.productId;
        var url = $('#viewClientUrl').attr('data-url') + "?id=" + receivables.clientId;
        window.location = url;
    };

    _self.viewSupplier = function (payables) {
        //window.location = "/Administration/Product/ViewProductDetails?id=" + reorders.productId;
        var url = $('#viewSupplierUrl').attr('data-url') + "?id=" + payables.supplierId;
        window.location = url;
    };

    _self.asyncOperationReceivables();

    _self.asyncOperationPayables();

    _self.asyncOperationReorders();



};