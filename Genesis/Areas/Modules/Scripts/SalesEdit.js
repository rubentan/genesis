$(function () {
    //if (jQuery().datepicker) {
    //    $('.date-picker').datepicker({
    //        rtl: Metronic.isRTL(),
    //        orientation: "left",
    //        autoclose: true
    //    });
    //}
    initSelect();
    vm = new viewModel();
    ko.applyBindings(vm);
});

var viewModel = function () {
    var _self = this;
    var documentId = $("#documentId").val();
    _self.orderItems = ko.observableArray();
    _self.documentNumber = ko.observable();
    _self.client = ko.observable();
    _self.dateCreated = ko.observable();
    _self.grandTotal = ko.observable("0");

    _self.Product = {
        productId: ko.observable(),
        productCode: ko.observable(),
        productName: ko.observable(),
        productDescription: ko.observable(),
        uom: ko.observable(),
        ending: ko.observable(),
        unitPrice: ko.observable('0'),
        discountPrice: ko.observable(),
        quantity: ko.observable(),
        discountA: ko.observable(),
        discountB: ko.observable(),
        discountC: ko.observable(),
        total: ko.observable()

    };

    
    
    _self.addProduct = function () {

        if ($('#ddProduct').val() != "") {
            if ($('#txtUnitPrice').val() != "") {
                if ($('#txtQuantity').val() != "") {
                    var newProduct = {
                        productId: $('#ddProduct').val(),
                        productName: $("#ddProduct").select2("data").text,
                        unitPrice: $('#txtUnitPrice').val(),
                        quantity: $('#txtQuantity').val(),
                        total: Number($('#txtUnitPrice').val()) * Number($('#txtQuantity').val()),
                        transactionType: 1
                    };

                    _self.orderItems.push(newProduct);
                    //alert(_self.grandTotal());
                    _self.grandTotal(Number(_self.grandTotal()) + Number(newProduct.total));
                    _self.clearProductAdd();
                    $('#ddProduct').select2('open');
                    
                } else {
                    alert('Quantity is required.');
                }
            } else {
                alert('Price is required.');
            }
        } else {
            alert('Product is required.');
        }
    };

    _self.removeProduct = function () {
        //alert('remove: '+ orderItems.productId);
        _self.grandTotal(Number(_self.grandTotal()) - Number(this.total));
        _self.orderItems.remove(this);
    };

    _self.clearProductAdd = function() {
        _self.Product.unitPrice('0');
        _self.Product.ending('');
        _self.Product.uom('');
        $('#ddProduct').val('');
        $("#ddProduct").select2("val", "");
        $('#txtUnitPrice').val('');
        $('#txtQuantity').val('');
        
    };

    _self.updateSalesInvoice = function () {
        var dataUrl = $("#hdnUpdateSalesInvoiceUrl").attr("data-url");
        if ($('#txtDocumentNumber').val() != "") {
            if ($('#ddClient').val() != "") {
                if ($('#txtTransactionDate').val() != "") {
                    if (_self.orderItems().length >= 0 ) {
                        var param = {
                            header: {
                                documentId : documentId,
                                documentNumber: $('#txtDocumentNumber').val(),
                                documentType: 1,
                                transactionDate: $('#txtTransactionDate').val(),
                                referenceId: $('#ddClient').val(),

                            },
                            details: _self.orderItems()
                        };

                        $.ajax({
                            //url: '/Modules/Sales/SaveInvoiceTransaction',
                            url: dataUrl,
                            type: 'POST',
                            data: ko.toJSON(param),
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            success: function (e) {
                                if (e.isSuccessful)
                                    alert('Successfully updated sales invoice.');
                                else
                                    alert('Failed to update sales invoice.');
                            },
                            error: function (e) { alert('ajax error: '+e); }
                        });
                    } else {
                        alert('Invoice Product Items are Required.');
                    }
                } else {
                    alert('Transaction Date is Required.');
                }
            } else {
                alert('Client is Required.');
            }
        } else {
            alert('Document Number is Required.');
        }
    };

   

    _self.getDocumentDetails = function () {
        var dataUrl = $("#hdnGetDocumentDetailsUrl").attr("data-url");
        $.ajax({
            //url: '/Modules/Sales/GetSaleById&id=' + documentId,
            url: dataUrl + "?id=" + documentId,
            type: 'POST',
            dataType: 'json',
            success: function (d) {
                _self.documentNumber(d.documentNumber);
                _self.dateCreated(d.dateCreated);
            },
            error: function () { alert('ajax error'); }
        });
    };

    _self.getOrderItems = function () {
        var param = { documentId: documentId };
        var dataUrl1 = $("#hdnGetSalesByIdUrl").attr("data-url");
        $.ajax({
            //url: '/Modules/Purchase/GetDocument',
            //url: '/Modules/Sales/GetSaleById',
            url: dataUrl1,
            type: 'POST',
            data: JSON.stringify(param),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (d) {
                //alert(d[0].referenceId);
                $('#txtDocumentNumber').val(d[0].documentNumber);
                $("#ddClient").select2("data", { id: d[0].referenceId, text: d[0].clientName },true);
                //$("#ddClient").select2("val", d[0].referenceId);
                $('#txtTransactionDate').val(convertDateFromJson(d[0].dateCreated));
            },
            error: function () { alert('ajax call error'); }
        });
        var dataUrl2 = $("#hdnGetAllSalesItemsUrl").attr("data-url");
        $.ajax({
            //url: '/Modules/Sales/GetAllSalesItems',
            url: dataUrl2,
            type: 'POST',
            data: JSON.stringify(param),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (d) {
                _self.orderItems(d);
                //alert(JSON.stringify(d));
                $.each(d, function (index) {
                    //alert(_self.grandTotal() + " + " + d[index].total);
                    _self.grandTotal(Number(_self.grandTotal()) + Number(d[index].total));
                });
                //_self.grandTotal(Number(d.));
            },
            error: function () { alert('ajax call error'); }
        });
    };

    _self.backToList = function () {
        var dataUrl = $("#hdnBackToListUrl").attr("data-url");
        window.location = dataUrl;
    };
    var dataUrl = $("#hdnGetAllBranchProductsUrl").attr("data-url");
    $("#ddProduct").select2({
        placeholder: 'Select...',
        //Does the user have to enter any data before sending the ajax request
        minimumInputLength: 3,
        allowClear: true,
        ajax: {
            //How long the user has to pause their typing before sending the next request
            quietMillis: 150,
            //The url of the json service
            //url: '/Modules/Sales/GetAllBranchProducts',
            url: dataUrl,
            dataType: 'json',
            //Our search term and what page we are on
            data: function (term) {
                return {
                    search: term
                };
            },
            results: function (data) {
                //Used to determine whether or not there are more results available,
                //and if requests for more data should be sent in the infinite scrolling
                return { results: data };
            }
        }
    }).on('change', function (e) {
        var param = {
            productId: $('#ddProduct').val()
        };
        var dataUrl = $("#hdnGetProductUrl").attr("data-url");
        if ($('#ddProduct').val() != "") {
            $.ajax({
                //url: '/Modules/Sales/GetProduct',
                url: dataUrl,
                type: 'POST',
                data: JSON.stringify(param),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function(d) {
                    _self.Product.unitPrice(d.unitPrice);
                    _self.Product.ending(d.ending);
                    _self.Product.uom(d.UOM);
                },
                error: function() {

                }
            });
        } else {
            _self.Product.unitPrice('');
            _self.Product.ending('');
            _self.Product.uom('');
        }


    });

    //_self.getDocumentDetails();
    _self.getOrderItems();
};

var initSelect = function() {
    var dataUrl = $("#hdnGetClientsForDropDownUrl").attr("data-url");
    $.ajax({
        //url: '/Modules/Sales/GetClientsForDropDown',
        url: dataUrl,
        type: 'POST',
        dataType: 'json',
        success: function(d) {


            $("#ddClient").select2({
                placeholder: "Select...",
                allowClear: true,
                minimumInputLength: 1,
                matcher: function(term, text, opt) {
                    return text.toUpperCase().indexOf(term.toUpperCase()) >= 0 || opt.parent("optgroup").attr("label").toUpperCase().indexOf(term.toUpperCase()) >= 0
                },
                query: function(query) {

                    var data = {
                        results: []
                    }


                    for (var i = 0; i < d.length; i++) {
                        if (d[i].text.toUpperCase().indexOf(query.term.toUpperCase()) > -1) {
                            data.results.push(d[i]);
                        }
                    }


                    query.callback(data);
                }
            }).on('change', function(e) {
                var param = {
                    clientId: $('#ddClient').val()
                };
            });
        },
        error: function() {

        }
    });

    
};
