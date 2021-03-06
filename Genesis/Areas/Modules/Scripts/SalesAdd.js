﻿$(function () {
    //if (jQuery().datepicker) {
    //    $('.date-picker').datepicker({
    //        rtl: Metronic.isRTL(),
    //        orientation: "left",
    //        autoclose: true
    //    }).datepicker("setDate", new Date());
    //}
    initSelect();
    vm = new viewModel();
    ko.applyBindings(vm);
});

var viewModel = function () {
    var _self = this;
    _self.orderItems = ko.observableArray();
    _self.grandTotal = ko.observable("0");

    _self.Product = {
        productId: ko.observable(),
        productCode: ko.observable(),
        productName: ko.observable(),
        productDescription: ko.observable(),
        uom: ko.observable(),
        beginning: ko.observable('0'),
        incoming: ko.observable('0'),
        outgoing: ko.observable('0'),
        ending: ko.observable('0'),
        unitPrice: ko.observable('0'),
        discountPrice: ko.observable(),
        quantity: ko.observable(),
        discountA: ko.observable(),
        discountB: ko.observable(),
        discountC: ko.observable(),
        total: ko.observable()

    };

    
    _self.addProductPost = function () {

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
                   // _self.grandTotal(Number(_self.grandTotal()) + Number(newProduct.total));
                    _self.clearProductAdd();
                    //$('#ddProduct').select2('open');

                    _self.addSalesInvoice();


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
        _self.Product.ending('0');
        _self.Product.uom('');
        _self.Product.beginning('0');
        _self.Product.incoming('0');
        _self.Product.outgoing('0');
        $('#ddProduct').val('');
        $("#ddProduct").select2("val", "");
        $('#txtUnitPrice').val('');
        $('#txtQuantity').val('');
        
    };

    _self.addSalesInvoice = function () {
        var dataUrl = $("#hdnAddPurchaseOrderUrl").attr("data-url");
        if ($('#txtDocumentNumber').val() != "") {
            if ($('#txtDocumentNumber').val().substring(0,2) == "SI" || $('#txtDocumentNumber').val().substring(0,2) == "RF" || $('#txtDocumentNumber').val().substring(0,2) == "DR" || $('#txtDocumentNumber').val().substring(0,2) == "OS") {
                if ($('#ddClient').val() != "") {
                    if ($('#txtTransactionDate').val() != "") {
                            if (_self.orderItems().length > 0) {
                                var param = {
                                    header: {
                                        documentNumber: $('#txtDocumentNumber').val(),
                                        documentType: 1,
                                        transactionDate: $('#txtTransactionDate').val(),
                                        referenceId: $('#ddClient').val(),

                                    },
                                    details: _self.orderItems()
                                };
                                if (!checkExisting(param)) {
                                    $.ajax({
                                        //url: '/Modules/Sales/SaveInvoiceTransaction',
                                        url: dataUrl,
                                        type: 'POST',
                                        data: ko.toJSON(param),
                                        contentType: 'application/json; charset=utf-8',
                                        dataType: 'json',
                                        success: function() {
                                            _self.orderItems.removeAll();
                                            _self.grandTotal('0');
                                            $('#txtDocumentNumber').val('');
                                            $('#txtTransactionDate').val('');
                                            $("#ddClient").select2("val", "");
                                            _self.clearProductAdd();
                                            alert('Successfully Added Sales Record.');
                                            document.body.scrollTop = document.documentElement.scrollTop = 0;
                                        }
                                    });
                                } else {
                                    alert('Transaction Date and Number is has already been used.');
                                }
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
                alert('Document Number is Invalid (SI,RF,DR,OS).');
            }
        } else {
            alert('Document Number is Required.');
        }
    };

    function checkExisting(param) {
        
        var dataUrl = $("#hdnCheckExistingDocument").attr("data-url");
        var retVal = true;

        $.ajax({
            url: dataUrl,
            type: 'POST',
            data: ko.toJSON(param),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            async: false,
            success: function (d) {
                //alert(d);
                retVal = d;
            },
            error: function (error) {
                //alert(error.responseText);
                alert(error);
            }
        });

        
        return retVal;
    };

    _self.addSalesInvoiceContinue = function () {
        _self.addSalesInvoice(); 
    };

    _self.addSalesInvoiceReturn = function () {
        _self.addSalesInvoice();
        var dataUrl = $("#hdnReturnUrl").attr("data-url");
        window.location = dataUrl;
    };

    _self.backToList = function () {
        var dataUrl = $("#hdnReturnUrl").attr("data-url");
        window.location = dataUrl;
    };

    var dataUrl = $("#hdnGetAllBranchProductsUrl").attr("data-url");

    $.ajax({
        //url: '/Modules/Sales/GetClientsForDropDown',
        url: dataUrl,
        type: 'POST',
        dataType: 'json',
        success: function(d) {


                $("#ddProduct").select2({
                    placeholder: 'Select...',
                    allowClear: true,
                    minimumInputLength: 3,
                    matcher: function (term, text, opt) {
                        return text.toUpperCase().indexOf(term.toUpperCase()) >= 0 || opt.parent("optgroup").attr("label").toUpperCase().indexOf(term.toUpperCase()) >= 0
                    },
                    query: function (query) {

                        var data = {
                            results: []
                        };


                        for (var i = 0; i < d.length; i++) {
                            if (d[i].text.toUpperCase().indexOf(query.term.toUpperCase()) > -1) {
                                data.results.push(d[i]);
                            }
                        }


                        query.callback(data);
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
                                _self.Product.beginning(d.beginning);
                                _self.Product.incoming(d.incoming);
                                _self.Product.outgoing(d.outgoing);
                                _self.Product.ending(d.ending);
                                _self.Product.uom(d.UOM);
                            },
                            error: function() {

                            }
                        });
                    } else {
                        _self.Product.unitPrice('');
                        _self.Product.beginning('0');
                        _self.Product.incoming('0');
                        _self.Product.outgoing('0');
                        _self.Product.ending('0');
                        _self.Product.uom('');
                    }


                });
        },
        error: function () {

        }
    });
};

var initSelect = function() {
    var dataUrl = $("#hdnGetClientUrl").attr("data-url");
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
                    };


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
