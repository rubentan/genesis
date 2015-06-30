var vm = {};

$(document).ready(function () {


    // jQuery methods go here...
    $(".mask_currency2").inputmask('₱ 999,999,999.99', {
        numericInput: true,
        rightAlignNumerics: false,
        greedy: false
    }); //123456  =>  € ___.__1.234,56

    vm = new paymentViewModel();
    ko.applyBindings(vm);
    vm.init();




    //$('.modeOfPayment').click(function () {
    //    if ($('#radioCash').is(':checked')) {
    //        $(".cashDiv").removeClass("hidden");
    //        $(".chequeDiv").addClass("hidden");
    //    }
    //    else if ($('#radioCheque').is(':checked')) {
    //        $(".chequeDiv").removeClass("hidden");
    //        $(".cashDiv").addClass("hidden");
    //    }
    //});

    //$('.typeOfPayment').click(function () {
    //    if ($('#radioExistingPayment').is(':checked')) {

    //        $("#existingPaymentDiv").removeClass("hidden");
    //        $("#newPaymentDiv").addClass("hidden");
    //    }
    //    else if ($('#radioNewPayment').is(':checked')) {

    //        $("#newPaymentDiv").removeClass("hidden");
    //        $("#existingPaymentDiv").addClass("hidden");
    //    }
    //});

    $("#txtSupplier").select2({
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

    $('#txtSupplier').change(function () {
        //var theID = $(test).val(); // works
        //var theSelection = $(test).filter(':selected').text(); // doesn't work
        var theID = $('#txtSupplier').select2('data').id;
        var theSelection = $('#txtSupplier').select2('data').text;
        vm.GetExistingPaymentList(theID);
    });

    $('#txtChequeDate').datepicker();
    $('#txtChequeDateE').datepicker();
    
    //$(".chequeExistingDiv").addClass("hidden");
    //$(".cashExistingDiv").addClass("hidden");
});


var paymentViewModel = function () {
    var pay = this;

    pay.paymentId = ko.observable();
    pay.SupplierId = ko.observable();
    pay.TypeOfPayment = ko.observable();
    pay.ReferenceNo = ko.observable();
    pay.ModeOfPayment = ko.observable();
    pay.ModeOfPaymentE = ko.observable();
    pay.CashAmount = ko.observable();
    pay.ChequeAmount = ko.observable();
    pay.ChequeNumber = ko.observable();
    pay.ChequeDate = ko.observable();
    pay.IssuingBank = ko.observable();
    pay.ExistingPaymentId = ko.observable();

    //PaymentInfo
    pay.PaymentInfo = ko.observable();

    //Totals
    pay.TransactionTotalPaid = ko.observable('0.00');
    pay.ExcessPayment = ko.observable('0.00');


    //Purchase Order Modal
    pay.SelectedPurchaseOrder = ko.observable();
    pay.mTotalPrice = ko.observable();
    pay.mPaid = ko.observable();
    pay.mRemainingBalance = ko.observable();

    pay.PaymentList = ko.observableArray();
    pay.ExistingPaymentList = ko.observableArray();
    pay.PurchaseOrderList = ko.observableArray();
    pay.PurchaseOrderDetails = ko.observableArray();
    
    //Subscription
    pay.TypeOfPayment.subscribe(function (item) {
        if (item == 'New') {
            $('#radioNewPayment').attr('checked', true);
        }
        else {
            $('#radioExistingPayment').attr('checked', true);
        }
        $.uniform.update("#radioNewPayment");
        $.uniform.update("#radioExistingPayment");
    });

    pay.ModeOfPayment.subscribe(function (item) {

        if (item == 'Cash') {
            $('#radioCash').attr('checked', true);
            $('#radioExsitingCash').attr('checked', true);
        }
        else {
            $('#radioCheque').attr('checked', true);
            $('#radioExsitingCheque').attr('checked', true);
        }
        $.uniform.update("#radioCash");
        $.uniform.update("#radioCheque");
        $.uniform.update("#radioExsitingCash");
        $.uniform.update("#radioExsitingCheque");
    });
    
    //Computed
    pay.TransactionTotalAmount = ko.computed(function () {
        var total = 0;
        ko.utils.arrayForEach(pay.PurchaseOrderDetails(), function (item) {
            var value = parseFloat(item.AmountToPay());
            if (!isNaN(value)) {
                total += value;
            }
        });
        return total.toFixed(2);
    });

    pay.TransactionTotalAmount.subscribe(function (value) {
        var total = 0;
        var unmaskedCashAmount = $('#txtCashAmount').inputmask('unmaskedvalue');
        var parsedCashAmount = (unmaskedCashAmount != undefined && unmaskedCashAmount != null && unmaskedCashAmount != '' && unmaskedCashAmount != "") ? unmaskedCashAmount : pay.CashAmount(); 
        var cashAmountUsed = (pay.TypeOfPayment() == 'New') ? parsedCashAmount : pay.CashAmount();
            
        var paymentAmount = (pay.ModeOfPayment() == 'Cash') ? cashAmountUsed : pay.ChequeAmount();
        paymentAmount = (paymentAmount == undefined && paymentAmount == null && paymentAmount == '') ? 0 : paymentAmount;
        var transactionAmt = parseFloat(value);
        if (paymentAmount >= transactionAmt) {
            total = transactionAmt;
            }
        else {
            total = paymentAmount;
            }
            
        pay.TransactionTotalPaid(total.toFixed(2));
        var excess = (paymentAmount - transactionAmt) * 1;
        pay.ExcessPayment(excess.toFixed(2));

        });

    pay.init = function () {
        pay.GetAllPayments();
        pay.GetPurchaseOrders();

        if (getQueryStringParam('paymentId') != "") {
            pay.paymentId(getQueryStringParam('paymentId'));

            pay.GetPaymentInfo({ paymentId: pay.paymentId() });
           
        }
    };

    pay.GetAllPayments = function() {
        $.ajax({
            url: '/Modules/Purchase/GetAllPayments' + getFilters(),
            type: 'GET',
            dataType: 'json',
            data: {},
            success: function(data) {
                pay.PaymentList(data);
            },
            error: function() { alert('ajax error'); }
        });
    };

    pay.GetPaymentInfo = function(filter) {
        $.ajax({
            url: '/Modules/Purchase/GetPaymentsByFilter',
            type: 'GET',
            dataType: 'json',
            data: filter,
            success: function(data) {
                pay.PaymentInfo(data[0]);

                $('#txtSupplier').val(pay.PaymentInfo().supplierId);
                $("#txtSupplier").select2("data", { id: pay.PaymentInfo().supplierId, text: pay.PaymentInfo().supplierName });
                $('#txtReferenceNumber').val(pay.PaymentInfo().referenceNumber);
                pay.TypeOfPayment(pay.PaymentInfo().TypeOfPayment);
                pay.ModeOfPayment(pay.PaymentInfo().ModeOfPayment);
                if (pay.ModeOfPayment() == 'Cash') {
                    pay.CashAmount(pay.PaymentInfo().cashAmount);
                }
                else {
                    pay.ChequeAmount(pay.PaymentInfo().chequeAmount);
                    pay.ChequeNumber(pay.PaymentInfo().chequeNumber);
                    pay.ChequeDate(convertDateFromJson(pay.PaymentInfo().chequeDate));
                    pay.IssuingBank(pay.PaymentInfo().IssuingBank);
                }
                pay.GetPurchaseOrders();
                pay.GetPaymentDetails({ paymentId: pay.paymentId() });
            },
            error: function() { alert('ajax error'); }
        });
    };

    pay.GetPaymentDetails = function(filter) {
        $.ajax({
            url: '/Modules/Purchase/GetPaymentDetails',
            type: 'GET',
            dataType: 'json',
            data: filter,
            success: function(data) {
                var filteredItems = $.map(data, function(item, i) {
                    var totalPaid = (item.totalPaid == undefined || item.totalPaid == null || item.totalPaid == '') ? 0.00 : item.totalPaid;
                    var remaining = (totalPaid > 0.00) ? (item.totalAmount - totalPaid) : 0.00;
                    remaining = (remaining < 0) ? 0.00 : remaining;
                    return {
                        Index: i + 1,
                        paymentDetailsId: item.paymentDetailsId,
                        DocumentId: item.documentId,
                        DocumentNo: item.documentNo,
                        TotalAmount: item.totalAmount,
                        RemainingBalance: remaining,
                        AmountToPay: ko.observable(item.paymentPrice),
                        totalPaid: totalPaid
                    };
                });

                pay.PurchaseOrderDetails(filteredItems);
            },
            error: function () { alert('ajax error') }
        });
    }

    pay.GetExistingPaymentList = function (sup) {

        console.log('ExistingPayment: '+ sup);
        $.ajax({
            url: '/Modules/Purchase/GetPaymentsByFilter',
            type: 'GET',
            dataType: 'json',
            data: { supplierId : sup },
            success: function (data) {
                console.log(data);
                pay.ExistingPaymentList(data);
            },
            error: function () { alert('ajax error') }
        });
    }

    pay.GetExistingPaymentInfo = function () {
        var item = ko.utils.arrayFirst(pay.PaymentList(), function (item) {
            return item.paymentId == pay.ExistingPaymentId();
        });

        $('#txtReferenceNumber').val(item.referenceNumber);
        pay.CashAmount(undefined);
        pay.ChequeAmount(undefined);
        pay.ChequeNumber(undefined);
        pay.ChequeDate(undefined);
        pay.IssuingBank(undefined);

        pay.ModeOfPayment(item.ModeOfPayment);
        pay.CashAmount(item.cashAmount);
        pay.ChequeAmount(item.chequeAmount);
        pay.ChequeNumber(item.chequeNumber);
        pay.ChequeDate(convertDateFromJson(item.chequeDate));
        pay.IssuingBank(item.IssuingBank);
        
    }

    pay.AddPurchaseOrder = function() {
        if ($('#txtSupplier').val() == undefined || $('#txtSupplier').val() == null || $('#txtSupplier').val() == '') {
            alert('You need to select a Supplier.');
            return;
        }

        pay.GetPurchaseOrders();
        $('#AddPurchaseOrder').modal('show');

    };

    pay.GetPurchaseOrders = function() {
        var param = {
            referenceId: $('#txtSupplier').val()
        };

        $.ajax({
            url: '/Modules/Purchase/GetPurchaseOrdersByFilter',
            type: 'GET',
            dataType: 'json',
            data: param,
            success: function(data) {
                pay.PurchaseOrderList(data);
            },
            error: function() { alert('ajax error'); }
        });
    };

    pay.RemovePurchaseOrderOnList = function(data) {
        pay.PurchaseOrderDetails.remove(function(item) { return item.Index == data.Index; });
    };

    pay.GetPurchaseOrderDetails = function () {
        pay.mTotalPrice(undefined);
        pay.mPaid(undefined);
        pay.mRemainingBalance(undefined);

        if (vm.SelectedPurchaseOrder() != undefined || vm.SelectedPurchaseOrder() != null) {
            var detail = ko.utils.arrayFirst(pay.PurchaseOrderList(), function (item) {
            return item.documentId == vm.SelectedPurchaseOrder().documentId;
        });

        var totalPaid = (detail.totalPaid == undefined || detail.totalPaid == null || detail.totalPaid == '') ? 0.00 : detail.totalPaid;
            var remaining = detail.purchasePrice - totalPaid;
        pay.mTotalPrice(detail.purchasePrice);
        pay.mPaid(totalPaid);
        pay.mRemainingBalance(remaining);
        }
    }

    pay.AddDetail = function(action) {

        if (vm.SelectedPurchaseOrder() == undefined) {
            alert('Please select Purchase Order.');
            return;
        }

        var detail = ko.utils.arrayFirst(pay.PurchaseOrderList(), function(item) {
            return item.documentId == vm.SelectedPurchaseOrder().documentId;
        });

        var newIndex = pay.PurchaseOrderDetails().length + 1;

        var item = {
            Index: newIndex,
            DocumentId: detail.documentId,
            DocumentNo: detail.documentNumber,
            TotalAmount: pay.mTotalPrice(),
            RemainingBalance: pay.mRemainingBalance(),
            AmountToPay: ko.observable('')
        };
        
        pay.PurchaseOrderDetails.push(item);

        if (action == 'AddAndClose') {
            $('#AddPurchaseOrder').modal('hide');
        }

        pay.ClearModal();
    };

    pay.ClearModal = function() {
        pay.SelectedPurchaseOrder(undefined);
        pay.mTotalPrice(undefined);
        pay.mPaid(undefined);
        pay.mRemainingBalance(undefined);
    };

    pay.FullPayment = function(data) {
        var selected = ko.utils.arrayFirst(pay.PurchaseOrderDetails(), function(item) {
            return item.Index == data.Index;
        });
        selected.AmountToPay(selected.TotalAmount);
        pay.PurchaseOrderDetails.replace(pay.PurchaseOrderDetails()[data.Index - 1], selected);
    };

    pay.HalfPayment = function(data) {
        var selected = ko.utils.arrayFirst(pay.PurchaseOrderDetails(), function(item) {
            return item.Index == data.Index;
        });

        var newAmountToPay = selected.TotalAmount;
        newAmountToPay = newAmountToPay / 2;
        newAmountToPay = Math.round(newAmountToPay);
        if (newAmountToPay % 1 == 0) {
            newAmountToPay = newAmountToPay + ".00";
        }

        selected.AmountToPay(newAmountToPay);
        pay.PurchaseOrderDetails.replace(pay.PurchaseOrderDetails()[data.Index - 1], selected);
    };

    pay.SavePaymentTransaction = function() {

        var refNo = '';
        var cashAmount = 0.00;
        if (pay.TypeOfPayment() == 'New')
        {
            cashAmount = $('#txtCashAmount').inputmask('unmaskedvalue');
        }
        else
        {
            cashAmount = pay.CashAmount();
        }

        var param = {
            Header: {
                paymentId: pay.paymentId(),
                supplierId: $('#txtSupplier').val(),
                referenceNumber: $('#txtReferenceNumber').val(),
                TypeOfPayment: pay.TypeOfPayment(),
                ModeOfPayment: pay.ModeOfPayment(),
                cashAmount: cashAmount,
                chequeAmount:   pay.ChequeAmount(),
                chequeNumber:   pay.ChequeNumber(), 
                chequeDate:     pay.ChequeDate(),
                chequeBank:     pay.IssuingBank(),
                totalPayment:   pay.TransactionTotalAmount()
                                
            },
            Details: $.map(pay.PurchaseOrderDetails(), function(item) {
                return {
                    paymentDetailsId: (item.paymentDetailsId == null || item.paymentDetailsId == undefined || item.paymentDetailsId == '') ? null : item.paymentDetailsId,
                    documentId: item.DocumentId,
                    paymentPrice: item.AmountToPay()
                };
            })
        };

        $.ajax({
            url: '/Modules/Purchase/SavePaymentTransaction',
            type: 'POST',
            data: ko.toJSON(param),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function(data) {
                if (data.isSuccessful) {
                    alert('SAVE COMPLETE');
                    window.location.href = PaymentListURL;
                } else {
                    alert('ERROR: ' + data.errorMsg);
                }
            },
            error: function() {

            }
        });
    };

    pay.CancelSave = function() {
        window.location.href = PaymentListURL;
    };
}
