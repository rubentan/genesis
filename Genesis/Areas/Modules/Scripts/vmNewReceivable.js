var vm = {};

$(function () {
    initSelect();
    $(".cashDiv").removeClass("hidden");
    $(".chequeDiv").addClass("hidden");

    //$('.datePicker').datepicker({
    //    rtl: Metronic.isRTL(),
    //    orientation: "left",
    //    autoclose: true
    //});

    vm = viewModel();
    ko.applyBindings(vm);

});

var receivableOrders = function (item) {

    var self = this;
    
    self.receivableDetailsId = item.receivableDetailsId;
    self.documentId = ko.observable(item.documentId);
    self.documentNumber = ko.observable(item.documentNumber);
    self.totalAmount = ko.observable(item.totalAmount);

    

    self.amountToPay = ko.observable(0);
    self.totalPayment = ko.observable(item.totalPayment);
    self.remove = function () {
        self.remove();
    };

   

    self.remainingBalance = ko.computed(function () {
        return self.totalAmount() - self.totalPayment() - self.amountToPay()
    }, this);


    
    

    return self;
};


var viewModel = function () {
    var self = this;

    self.referenceId = ko.observable(0);
    self.isNewPayment = ko.observable("true");

    self.referenceNo = ko.observable();
    self.isCash = ko.observable("true");
    
    self.cashAmount = ko.observable(0);
    self.chequeNumber = ko.observable();
    self.chequeDate = ko.observable();
    self.chequeBank = ko.observable();

    self.arrReceivables = ko.observableArray();

    self.selectedDocument = ko.observable();
    self.selectedDocumentClear = function () {
        self.selectedDocument(null);
    };

    self.arrReceivableOrders = ko.observableArray();

    if (getQueryStringParam("id") != "0") {
        
        $.ajax({
            async:false,
            url: '/Modules/Sales/GetNewBranchRecievable',
            type: 'POST',
            data: JSON.stringify({ id: getQueryStringParam("id") }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (d) {
                self.referenceId(d.header.clientId)
                $("#ddClient").select2("data", { id: d.header.clientId, text: d.header.clientName });
                self.isNewPayment(d.header.isNewPayment.toString());
                self.referenceNo(d.header.referenceNumber);                        
                self.isCash(d.header.isCash.toString());
               

                self.cashAmount(d.header.cashAmount);
                self.chequeNumber(d.header.chequeNumber);
                
                
                self.chequeDate(convertDateFromJson(d.header.chequeDate));

                self.chequeBank(d.header.chequeBank);
                

                var details = $.map(d.details, function (item, i) {
                   
                    return new receivableOrders(item);
                    //return {
                    //    documentNumber: ko.observable(item.documentNumber),
                    //    totalAmount: ko.observable(item.totalAmount),
                    //    totalPayment: ko.observable(item.totalPayment),
                    //    remainingBalance: ko.observable(),
                    //    amountToPay: ko.observable()
                    //};

                });

                self.arrReceivableOrders(details);
           
            },
            error: function (a, b, c) {
                alert(c);
            }
        });

        
       
    }

    self.showDetails = function () {
        var item = self.selectedDocument();

        $('#divTotalPrice').text('0.00');
        $('#divPaid').text('0.00');
        $('#divRemainingBal').text('0.00');

        if (item == undefined) return;

        $('#divTotalPrice').text(item.totalAmount);
        $('#divPaid').text(item.totalAmount - item.remainingBalance);
        $('#divRemainingBal').text(item.remainingBalance);

    };

    

    self.arrReceivableOrdersAdd = function () {
        self.arrReceivableOrders.push(new receivableOrders(self.selectedDocument()))
        self.selectedDocumentClear();
    }



    self.getAllNotYetReceivables = function () {

        $.ajax({

            url: '/Modules/Sales/GetAllNotYetReceive',
            type: 'POST',
            data: JSON.stringify({ referenceId: self.referenceId() }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (d) {
                self.arrReceivables(d);
            },
            error: function () {
                
            }

        });
    };

   


    self.isCash.subscribe(function(newValue) {
       
        if (newValue == 'true') {            
            $(".cashDiv").removeClass("hidden");
            $(".chequeDiv").addClass("hidden");
        }
        else if (newValue == 'false') {
            $(".chequeDiv").removeClass("hidden");
            $(".cashDiv").addClass("hidden");
        }
        

    });



    self.totalPaymentAmount = ko.computed(function () {
        var total = 0;
        ko.utils.arrayForEach(self.arrReceivableOrders(), function (item) {
           
          
            total += item.totalAmount();
            
           
        });

        return total; //.toFixed(2);
    });

    self.totalRemainingBalance = ko.computed(function () {
        var total = 0;
        ko.utils.arrayForEach(self.arrReceivableOrders(), function (item) {

            var temp = item.remainingBalance()
            total += temp;
        });

        return total; //.toFixed(2);
    });

   

    self.totalPaidAmount = ko.computed(function () {
        var total = 0;
        ko.utils.arrayForEach(self.arrReceivableOrders(), function (item) {

            var temp = item.totalPayment() + parseFloat(item.amountToPay());
            total += temp;

        });

        return total; //.toFixed(2);
    });

   
    self.excessPaymentAmount = ko.computed(function () {
        var total = 0;
        ko.utils.arrayForEach(self.arrReceivableOrders(), function (item) {


            var temp = parseFloat(item.amountToPay());
            total += temp;

        });

        return total =  self.cashAmount() - total;
        //return (total > -1 ) ? total : 0; //.toFixed(2);
    });


    self.saveTransaction = function () {
        var param = {

            header: {
                receivableId: getQueryStringParam('id'),
                clientId: self.referenceId(),
                isNewPayment: self.isNewPayment(),
                referenceNumber: self.referenceNo(),
                isCash: self.isCash(),
                cashAmount: self.cashAmount(),
                chequeNumber: self.chequeNumber(),
                chequeDate: self.chequeDate(),
                chequeBank: self.chequeBank()
                
                //clientId: null,
                //isNewPayment: null,
                //referenceNumber: null,
                //isCash: null,
                //cashAmount: null,
                //chequeNumber: null,
                //chequeDate: null,
                //chequeBank: null
            },
            details: $.map(self.arrReceivableOrders(), function (item) {
                
                return {
                    receivableDetailsId: item.receivableDetailsId,
                    documentId: item.documentId(),
                    //receivableId:null,
                    paymentPrice: item.amountToPay(),
                   
                    //dateCreated:null,
                    //createdBy: null,
                    //dateLastModified: null,
                    //lastModifiedBy: null
                };
            })

        };

        $.ajax({
            url: '/Modules/Sales/SaveTransaction',
            type: 'POST',
            data: JSON.stringify(param),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function () { alert('success') },
            error: function (a, b, c) {
                alert(a)
                alert(b)
                alert(c)
            }
        });
    };
    
};

var initSelect = function () {

    var clients = [];


    $.ajax({
        async:false,
        url: '/Modules/Sales/GetClientsForDropDown',
        type: 'POST',
        //data: JSON.stringify(param),
        //contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (d) {


            $("#ddClient").select2({
                placeholder: "Select...",
                allowClear: true,
                minimumInputLength: 1,
                matcher: function (term, text, opt) {
                    return text.toUpperCase().indexOf(term.toUpperCase()) >= 0 || opt.parent("optgroup").attr("label").toUpperCase().indexOf(term.toUpperCase()) >= 0
                },
                query: function (query) {

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
            }).on('change', function (e) {
                var param = {
                    clientId: $('#ddClient').val()
                };

                referenceId($('#ddClient').val());

                


            });
        },
        error: function () {

        }
    });
}



ko.bindingHandlers.datepicker = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        //initialize datepicker with some optional options
        var options = allBindingsAccessor().datepickerOptions || {},
            $el = $(element);

        $el.datepicker(options);

        //handle the field changing by registering datepicker's changeDate event
        ko.utils.registerEventHandler(element, "changeDate", function () {
            var observable = valueAccessor();
            observable($el.datepicker("getDate"));
        });

        //handle disposal (if KO removes by the template binding)
        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            $el.datepicker("destroy");
        });

    },
    update: function (element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor()),
            $el = $(element);

        //handle date data coming via json from Microsoft
        if (String(value).indexOf('/Date(') == 0) {
            value = new Date(parseInt(value.replace(/\/Date\((.*?)\)\//gi, "$1")));
        }

        var current = $el.datepicker("getDate");

        if (value - current !== 0) {
            $el.datepicker("setDate", value);
        }
    }
};