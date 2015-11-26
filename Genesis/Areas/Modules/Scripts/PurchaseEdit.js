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
    _self.supplier = ko.observable();
    _self.dateCreated = ko.observable();
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

                    var newProduct = {
                        productId: $('#ddProduct').val(),
                        productName: $("#ddProduct").select2("data").text,
                        unitPrice: $('#txtUnitPrice').val(),
                        quantity: $('#txtQuantity').val(),
                        discountA: $('#txtDiscountA').val(),
                        discountB: $('#txtDiscountB').val(),
                        discountC: $('#txtDiscountC').val(),
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

    var creditDiscount = function (currentDiscount, additionalDiscount) {

        if (additionalDiscount != null && additionalDiscount.trim() != '' && additionalDiscount != undefined)
            currentDiscount = currentDiscount * ((100 - Number(additionalDiscount)) / 100);

        return currentDiscount;

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
        
    };

    _self.updatePurchaseOrder = function () {
        var dataUrl = $("#hdnUpdatePurchaseOrderUrl").attr("data-url");
        if ($('#txtDocumentNumber').val() != "") {
            if ($('#ddClient').val() != "") {
                if ($('#txtTransactionDate').val() != "") {
                    if (_self.orderItems().length >= 0 ) {
                        var param = {
                            header: {
                                documentId : documentId,
                                documentNumber: $('#txtDocumentNumber').val(),
                                documentType: 2,
                                transactionDate: $('#txtTransactionDate').val(),
                                referenceId: $('#ddSupplier').val(),

                            },
                            details: _self.orderItems()
                        };

                        $.ajax({
                            //url: '/Modules/Purchase/SavePurchaseTransaction',
                            url: dataUrl,
                            type: 'POST',
                            data: ko.toJSON(param),
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            success: function (e) {
                                if (e.isSuccessful)
                                    alert('Successfully updated Purchase Order.');
                                else
                                    alert('Failed to update  Purchase Order.');
                            },
                            error: function (e) { alert('ajax error: ' + e); }
                        });
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
            alert('Document Number is Required.');
        }
    };

   

    _self.getDocumentDetails = function () {
        var dataUrl = $("#hdnGetDocumentDetailsUrl").attr("data-url");
        $.ajax({
            //Url: '/Modules/Sales/GetSaleById&id=' + documentId,
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
        var dataUrl1 = $("#hdnGetPurchaseByIdUrl").attr("data-url");
        $.ajax({
            //url: '/Modules/Purchase/GetDocument',
            //url: '/Modules/Purchase/GetPurchaseById',
            url: dataUrl1,
            type: 'POST',
            data: JSON.stringify(param),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (d) {
                //alert(d[0].referenceId);
                $('#txtDocumentNumber').val(d[0].documentNumber);
                $("#ddSupplier").select2("data", { id: d[0].referenceId, text: d[0].supplierName },true);
                //$("#ddClient").select2("val", d[0].referenceId);
                $('#txtTransactionDate').val(convertDateFromJson(d[0].dateCreated));
            },
            error: function () { alert('ajax call error: /Modules/Purchase/GetPurchaseById'); }
        });
        var dataUrl2 = $("#hdnGetAllPurchaseItemsUrl").attr("data-url");
        $.ajax({
            //url: '/Modules/Purchase/GetAllPurchaseItems',
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
            error: function () { alert('ajax call error: /Modules/Purchase/GetAllPurchaseItems'); }
        });
    };

    _self.backToList = function() {
        var dataUrl = $("#hdnBackToListUrl").attr("data-url");
        window.location = dataUrl;
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
        error: function () {

        }
    });
    //_self.getDocumentDetails();
    _self.getOrderItems();
};

var initSelect = function() {
    var dataUrl = $("#hdnGetSupplierForDropDownUrl").attr("data-url");
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

