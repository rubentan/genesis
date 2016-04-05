$(function () {
    //if (jQuery().datepicker) {
    //    $('.date-picker').datepicker({
    //        rtl: Metronic.isRTL(),
    //        orientation: "left",
    //        autoclose: true
    //    }).datepicker("setDate", new Date());;
    //}
    initSelect();
    vm = new viewModel();
    ko.applyBindings(vm);
});

var viewModel = function () {
    var _self = this;
    _self.orderItems = ko.observableArray();
    _self.grandTotal = ko.observable("0");
    _self.unitPrice = ko.observable("0");
    _self.discountA = ko.observable();
    _self.discountB = ko.observable();
    _self.discountC = ko.observable();


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

    var creditDiscount = function (currentDiscount, additionalDiscount) {

        if (additionalDiscount != null && additionalDiscount.trim() != '' && additionalDiscount != undefined)
            currentDiscount = currentDiscount * ((100 - Number(additionalDiscount)) / 100);

        return currentDiscount;

    };

    _self.netPrice = ko.computed(function () {
        var unitPrice = _self.unitPrice();

        if (_self.discountA() != 0 && _self.discountA() != '') {
            unitPrice = creditDiscount(unitPrice, _self.discountA());
        }

        if (_self.discountB() != 0 && _self.discountB() != '') {
            unitPrice = creditDiscount(unitPrice, _self.discountB());
        }

        if (_self.discountC() != 0 && _self.discountC() != '') {
            unitPrice = creditDiscount(unitPrice, _self.discountC());
        }
        return unitPrice;

    }, self);

    _self.backToList = function () {
        var dataUrl = $("#hdnReturnUrl").attr("data-url");
        window.location = dataUrl;
    };
    
    _self.addProduct = function () {


        if ($('#ddProduct').val() != "") {
            if ($('#txtUnitPrice').val() != "") {
                if ($('#txtQuantity').val() != "") {

                    var currentPrice = Number($('#txtUnitPrice').val());
                    if ($('#txtDiscountA').val() != "" && $('#txtDiscountA').val() != 0) {
                        //alert('apply dc a');
                        currentPrice = creditDiscount(currentPrice, $('#txtDiscountA').val());
                    }

                    if ($('#txtDiscountB').val() != "" && $('#txtDiscountB').val() != 0) {
                        //alert('apply dc b');
                    currentPrice = creditDiscount(currentPrice, $('#txtDiscountB').val());
                    }

                    if ($('#txtDiscountC').val() != "" && $('#txtDiscountC').val() != 0) {
                        //alert('apply dc c');
                        currentPrice = creditDiscount(currentPrice, $('#txtDiscountC').val());

                    }

                    //if (currentPrice == Number($('#txtUnitPrice').val()))
                      //  currentPrice = 0;

                    var newProduct = {
                        productId: $('#ddProduct').val(),
                        productName: $("#ddProduct").select2("data").text,
                        unitPrice: $('#txtUnitPrice').val(),
                        discountA: $('#txtDiscountA').val(),
                        discountB: $('#txtDiscountB').val(),
                        discountC: $('#txtDiscountC').val(),
                        quantity: $('#txtQuantity').val(),
                        total: currentPrice * Number($('#txtQuantity').val()),
                        
                        transactionType: 2
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
        $('#txtDiscountA').val('');
        $('#txtDiscountB').val('');
        $('#txtDiscountC').val('');
        $('#txtNetPrice').val('');
        
    };

    _self.addPurchaseOrder = function () {
        var dataUrl = $("#hdnAddPurchaseOrderUrl").attr("data-url");
        if ($('#txtDocumentNumber').val() != "") {
            if ($('#txtDocumentNumber').val().substring(0,2) == "PO" || $('#txtDocumentNumber').val().substring(0,2) == "RT" || $('#txtDocumentNumber').val().substring(0,2) == "DR" || $('#txtDocumentNumber').val().substring(0,2) == "OS") {
                if ($('#ddSupplier').val() != "") {
                    if ($('#txtTransactionDate').val() != "") {
                        if (_self.orderItems().length > 0 ) {
                            var param = {
                                header: {
                                    documentNumber: $('#txtDocumentNumber').val(),
                                    documentType: 2,
                                    transactionDate: $('#txtTransactionDate').val(),
                                    referenceId: $('#ddSupplier').val(),

                                },
                                details: _self.orderItems()
                            };

                            if (!checkExisting(param)) {

                                $.ajax({
                                    //url: '/Modules/Purchase/SavePurchaseTransaction',
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
                                        $("#ddSupplier").select2("val", "");
                                        _self.clearProductAdd();
                                        alert('Successfully Added Purchase Record.');
                                        document.body.scrollTop = document.documentElement.scrollTop = 0;
                                    }
                                });
                            } else {
                                alert('Transaction Date and Number has already been used.');
                            }
                        } else {
                            alert('Order Product Items are Required.');
                        }
                    } else {
                        alert('Transaction Date is Required.');
                    }
                } else {
                    alert('Supplier is Required.');
                }
            } else {
                alert('Document Number is Invalid (PO,RT,DR,OS).');
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
        _self.addPurchaseOrder();
    };

    _self.addSalesInvoiceReturn = function () {
        _self.addPurchaseOrder();
        var dataUrl = $("#hdnReturnUrl").attr("data-url");
        window.location = dataUrl;
        //window.location = "/Modules/Purchase/BranchPurchases";
    };

    var dataUrl = $("#hdnGetAllBranchProductsUrl").attr("data-url");

    $.ajax({
        //url: '/Modules/Purchase/GetSupplierForDropDown',
        url: dataUrl,
        type: 'POST',
        dataType: 'json',
        success: function(d) {

            $("#ddProduct").select2({
                placeholder: 'Select...',
                allowClear: true,
                minimumInputLength: 3,
                matcher: function (term, text, opt) {
                    return text.toUpperCase().indexOf(term.toUpperCase()) >= 0 || opt.parent("optgroup").attr("label").toUpperCase().indexOf(term.toUpperCase()) >= 0;
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
                    _self.Product.unitPrice('0');
                    _self.Product.beginning('0');
                    _self.Product.incoming('0');
                    _self.Product.outgoing('0');
                    _self.Product.ending('0');
                    _self.Product.uom('');
                }


            });
        },
        error: function() {

        }
    });
};

var initSelect = function() {
    var dataUrl = $("#hdnGetSupplierUrl").attr("data-url");
    $.ajax({
        //url: '/Modules/Purchase/GetSupplierForDropDown',
        url: dataUrl,
        type: 'POST',
        dataType: 'json',
        success: function(d) {


            $("#ddSupplier").select2({
                placeholder: "Select...",
                allowClear: true,
                minimumInputLength: 1,
                matcher: function(term, text, opt) {
                    return text.toUpperCase().indexOf(term.toUpperCase()) >= 0 || opt.parent("optgroup").attr("label").toUpperCase().indexOf(term.toUpperCase()) >= 0;
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
                    supplierId: $('#ddSupplier').val()
                };
            });
        },
        error: function() {

        }
    });

    
};
