
var documentId = 0;
var dt = null;
var vm = new Object();

$(function () {

    vm = viewModel();
    ko.applyBindings(vm);


    //dt = $('#dataTable_orderItem').DataTable();
    initSelect();
    //initGrid();

    $('#txtTransactionDate').datepicker();

    
});

var addProduct = function () {

}

var viewModel = function () {


    var self = this;

    self.Product = {
        productId: ko.observable(2),
        productCode: ko.observable(),
        productName: ko.observable(''),
        productDescription: ko.observable(),
        uom: ko.observable(),
        ending: ko.observable(),
        unitPrice: ko.observable(0),
        discountPrice: ko.observable(),
        quantity: ko.observable(),
        discountA: ko.observable(1),
        discountB: ko.observable(1),
        discountC: ko.observable(1),
        total: ko.observable()

    };

    if (getQueryStringParam('documentId') != "")
        documentId = getQueryStringParam('documentId');




    self.orderItems = ko.observableArray();

    self.getOrderItems = function () {
        var param = { documentId: documentId };

        $.ajax({
            url: '/Modules/Purchase/GetDocument',
            type: 'POST',
            data: JSON.stringify(param),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (d) {
                $('#txtDocumentNumber').val(d.documentNumber);     
                $("#ddSupplier").select2("data", { id: d.referenceId, text: d.supplierName });
                $('#txtTransactionDate').val(convertDateFromJson(d.transactionDate));
            },
            error: function () { alert('ajax call error'); }
        });

        $.ajax({
            url: '/Modules/Purchase/GetAllOrderItems2',
            type: 'POST',
            data: JSON.stringify(param),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (d) {
                self.orderItems(d);
            },
            error: function () { alert('ajax call error'); }
        });
    };

    getOrderItems();

    self.addProduct = function () {

        var currentPrice = Number($('#txtUnitPrice').val());
        currentPrice = creditDiscount(currentPrice, $('#txtDiscountA').val());
        currentPrice = creditDiscount(currentPrice, $('#txtDiscountB').val());
        currentPrice = creditDiscount(currentPrice, $('#txtDiscountC').val());

        if (currentPrice == Number($('#txtUnitPrice').val()))
            currentPrice = 0;

        var newProduct = {
            productId: $('#ddProduct').val(),
            productName: $("#ddSupplier").select2("data").text,
            unitPrice: $('#txtUnitPrice').val(),
            discountPrice: currentPrice,
            quantity: $('#txtQuantity').val(),
            discountA: $('#txtDiscountA').val(),
            discountB: $('#txtDiscountB').val(),
            discountC: $('#txtDiscountC').val(),
            documentId: $('#txtDocumentId').val(),
            total: currentPrice * Number($('#txtQuantity').val()),
            transactionType: 2
           
            
        };

        self.orderItems.push(newProduct);
    };

    var creditDiscount = function (currentDiscount, additionalDiscount) {

        if (additionalDiscount != null && additionalDiscount.trim() != '' && additionalDiscount != undefined)
            currentDiscount = currentDiscount * ((100 - Number(additionalDiscount)) / 100);

        return currentDiscount;

    };

    self.addPurchaseOrder = function () {
        var param = {
            document: {
                documentId: documentId,
                documentNumber: $('#txtDocumentNumber').val(),
                documentType: 2,
                transactionDate: $('#txtTransactionDate').val(),
                referenceId: $('#ddSupplier').val(),

            },
            products:self.orderItems()
        };

        $.ajax({
            url: '/Modules/Purchase/SaveTransaction',
            type: 'POST',
            data: JSON.stringify(param),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function () {
                alert('SAVE COMPLETE');
            },
            error: function () {

            }
        });
    };
};

var initGrid = function () {
    gridOrderItem();
};
var gridOrderItem = function () {

    dt.DataTable().clear().destroy();
    dt.dataTable({
        'bLengthChange': false,
        'bServerSide': true,
        "bProcessing": false,
        'bSort': false,
        'sAjaxSource': '/Modules/Purchase/GetAllOrderItems?documentId='+documentId,
        'aoColumns': [
            //{
            //    'mData': 'transactionId',
            //    'mRender': function (oObj) {
            //        return '<input type="checkbox" class="group-checkable" value="' + oObj + '">';
            //    }
            //},
            { 'mData': 'productName' },
            { 'mData': 'originalPrice' },
            { 'mData': 'discountPrice' },
            { 'mData': 'quantity' },
            { 'mData': 'discountA' },
            { 'mData': 'discountB' },
            { 'mData': 'discountC' },
            { 'mData': 'unitPrice' },
            {
                'mData': 'transactionId',
                'mRender': function (oObj) {
                    return '<button type="button" class="btn" onclick="return deleteRow(\'' + oObj + '\');"> Edit</button>';
                }
            },
        ]
    });
};

var initSelect = function () {
    
    $("#ddSupplier").select2({
        placeholder: 'Select...',
        //Does the user have to enter any data before sending the ajax request
        minimumInputLength: 1,
        allowClear: true,
        ajax: {
            //How long the user has to pause their typing before sending the next request
            quietMillis: 150,
            //The url of the json service
            url: window.supplierURL,
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
    });

    $("#ddProduct").select2({
        placeholder: 'Select...',
        //Does the user have to enter any data before sending the ajax request
        minimumInputLength: 1,
        allowClear: true,
        ajax: {
            //How long the user has to pause their typing before sending the next request
            quietMillis: 150,
            //The url of the json service
            url: window.productURL,
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
        
        $.ajax({
            url: '/Modules/Purchase/GetProduct',
            type: 'POST',
            data: JSON.stringify(param),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (d) {
                Product.unitPrice(d.unitPrice);
                Product.ending(d.ending);
                Product.uom(d.UOM);
            },
            error: function () {

            }
        });
       
        
    });
};

var addRow = function () {
    //dt.DataTable().clear().destroy();
    dt = $('#dataTable_orderItem').DataTable();
    //dt.row.add(['', '', '', '', '', '', '', '', '']).draw();
    //dt.row.add([{ "productName": "test", "originalPrice": "100" }]).draw();
    dt.row.add({
        "productName": "test",
        "originalPrice": "100",
        "discountPrice": " ",
        "quantity": " ",
        "discountA": " ",
        "discountB": " ",
        "discountC": " ",
        "unitPrice": " ",
        "transactionId": " "

    }).draw();

    return 0;

};

var deleteRow = function () {
    $('#dataTable_orderItem').row($(this).parents('tr'))
        .remove()
        .draw();
};
