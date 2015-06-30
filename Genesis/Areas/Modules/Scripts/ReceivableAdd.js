$(function () {


    vm = new viewModel();
    ko.applyBindings(vm);
});

var viewModel = function () {
    var _self = this;

    _self.existingPayments = ko.observableArray();
    _self.clientDocuments = ko.observableArray();
    _self.payDocuments = ko.observableArray();

    _self.isNew = ko.observable();

    //Existing Payment Details
    _self.remainingChequeAmount = ko.observable('0.0000');
    _self.existingChequeNumber = ko.observable();
    _self.existingChequeDate = ko.observable();
    _self.existingIssuingBank = ko.observable();

    //New Payment Details
    _self.newChequeAmount = ko.observable('0.0000');
    _self.newChequeNumber = ko.observable();
    _self.newChequeDate = ko.observable();
    _self.newIssuingBank = ko.observable();

    //document details
    _self.totalPrice = ko.observable('0.0000');
    _self.totalPaid = ko.observable('0.0000');
    _self.totalRemaining = ko.observable('0.0000');
    _self.amountToPay = ko.observable();

    //Payment Computations
    _self.GrandTotal = ko.observable('0');
    _self.PaymentAmount = ko.observable('0');
    _self.ExcessPaymentAmount = ko.observable('0');


    //Existing Payments
    _self.loadExistingPayments = function () {
        $("#ddPayments").prop('disabled', 'disabled');
        var dataUrl = $("#hdnGetExistingPaymentsUrl").attr("data-url");
        $.ajax({
            url: dataUrl,
            type: 'POST',
            async: false,
            dataType: 'json',
            data: { clientId: $("#ddClient").val() },
            success: function (d) {
                _self.existingPayments(d);
                $("#ddPayments").prop('disabled', false);
            },
            error: function (e) {
                alert("Ajax error[loadExistingPayments]: " + e);
            }
        });
    };

    //Existing Payments
    _self.paymentsChanged = function (existingPayments, event) {

        if ($("#ddPayments").val() != "") {
            //$("#ddClientDocuments").prop('disabled', 'disabled');
            var dataUrl = $("#hdnGetExistingPaymentDetailsUrl").attr("data-url");
            $.ajax({
                url: dataUrl,
                type: 'POST',
                dataType: 'json',
                data: { paymentId: event.target.value },
                success: function (d) {

                    _self.remainingChequeAmount(d.cashAmount);
                    _self.existingChequeNumber(d.chequeNumber);
                    _self.existingChequeDate(convertDateFromJson(d.chequeDate));
                    _self.existingIssuingBank(d.chequeBank);

                    _self.clearDocumentAdd();
                    _self.payDocuments.removeAll();
                    _self.GrandTotal('0');
                    _self.ExcessPaymentAmount('0');
                    _self.PaymentAmount(d.cashAmount);

                },
                error: function (e) {

                    alert("Ajax error[PaymentsChanged]: " + e);
                }
            });
        } else {
            _self.clearPaymentInfo();
            _self.clearDocumentAdd();
            _self.payDocuments.removeAll();
            _self.clearComputations();
        }
    };

    //General
    _self.loadClientDocuments = function () {


        $("#ddClientDocuments").prop('disabled', 'disabled');
            var dataUrl = $("#hdnGetClientDocumentsUrl").attr("data-url");
            $.ajax({
                url: dataUrl,
                type: 'POST',
                dataType: 'json',
                async: false,
                data: { id: $("#ddClient").val() },
                success: function (d) {
                    _self.clientDocuments(d);
                    $("#ddClientDocuments").prop('disabled', false);
                },
                error: function (e) {
                    alert("Ajax error[loadClientDocuments]: " + e);
                }
            });
            
    };

    //General
    _self.removeSalesDocument = function () {
        _self.GrandTotal(Number(_self.GrandTotal()) - Number(this.totalPayAmount));
        _self.ExcessPaymentAmount(Number(_self.PaymentAmount()) - Number(_self.GrandTotal()));
        _self.payDocuments.remove(this);
    };

    //General
    _self.clearDocumentAdd = function () {
        _self.totalPrice('0.0000');
        _self.totalPaid('0.0000');
        _self.totalRemaining('0.0000');
        _self.amountToPay('');
        $('#ddClientDocuments').val('');

    };

    //General
    _self.documentChanged = function (clientDocuments, event) {

        //alert(event.target.value);
        if (event.target.value != null || event.target.value != "") {
            var dataUrl = $("#hdnGetDocumentDetailsUrl").attr("data-url");
            $.ajax({
                url: dataUrl,
                type: 'POST',
                dataType: 'json',
                data: { id: event.target.value },
                success: function(d) {

                    _self.totalPrice(d.totalPrice);
                    _self.totalPaid(d.totalPayments);
                    _self.totalRemaining(Number(_self.totalPrice()) - Number(_self.totalPaid()));
                    //$("#txtAmountToPay").prop('disabled', false);
                },
                error: function(e) {

                    alert("Ajax error: " + e);
                }
            });
        }
    };

    //General
    _self.clearComputations = function () {
        _self.GrandTotal('0');
        _self.PaymentAmount('0');
        _self.ExcessPaymentAmount('0');
    };

    //General
    _self.clearPaymentInfo = function () {
       
        _self.remainingChequeAmount('0.0000');
        _self.existingChequeNumber('');
        _self.existingChequeDate('');
        _self.existingIssuingBank('');

        _self.newChequeAmount('0.0000');
        _self.newChequeNumber('');
        _self.newChequeDate('');
        _self.newIssuingBank('');
    };

    //General
    _self.addDocument = function () {
        var paymentTotal = 0;
        if (_self.isNew() == true) {
            paymentTotal = $("#txtChequeAmount").val();
        } else {
            paymentTotal = $("#txtRemainingChequeAmount").val();
        }
        if (paymentTotal == null) {
            paymentTotal = 0;
        }

        if ($('#ddClientDocuments').val() != "") {
            if ($('#txtAmountToPay').val() != "" && Number($('#txtAmountToPay').val()) > 0) {
                if (Number($('#txtAmountToPay').val()) <= paymentTotal && (Number(_self.GrandTotal()) + Number($('#txtAmountToPay').val())) <= paymentTotal) {
                    //alert("grand: " + _self.GrandTotal() + ", PaymentTotal: " +paymentTotal);
                    if (Number($('#txtAmountToPay').val()) <= Number(_self.totalRemaining())) {
                        var newDocument = {
                            documentNumber: $('#ddClientDocuments option:selected').text(),
                            totalAmount: _self.totalPrice(),
                            remainingBalance: _self.totalRemaining(),
                            totalPayAmount: $('#txtAmountToPay').val()
                        };

                        _self.payDocuments.push(newDocument);
                        _self.GrandTotal(Number(_self.GrandTotal()) + Number(newDocument.totalPayAmount));
                        _self.PaymentAmount(paymentTotal);
                        _self.ExcessPaymentAmount(Number(_self.GrandTotal()) - Number(_self.PaymentAmount()));
                        _self.clearDocumentAdd();

                    } else {
                        alert('Document total is only: ' + convertToCurrency(_self.totalRemaining()));
                    }
                } else {
                    alert('You can only pay the maximum of: ' + convertToCurrency(paymentTotal));
                }
            } else {
                alert('Amount to Pay is required.');
            }
        } else {
            alert('Document is required.');
        }
    };

    


    $('input:radio[name="isNewPayment"]').change(
    function () {

        _self.clearPaymentInfo();
        _self.clearComputations();
        _self.clearDocumentAdd();

        _self.payDocuments.removeAll();
        _self.existingPayments.removeAll();


        if (this.value == "true") {
            _self.isNew(true);
            $("#existingPaymentDiv").hide();
            $("#newPaymentDiv").show();
            $("#divAddReceivableOrder").show();
            $("#divListReceivableOrder").show();
        } else {
            _self.isNew(false);
            $("#divAddReceivableOrder").hide();
            $("#divListReceivableOrder").hide();
            $("#newPaymentDiv").hide();
            $("#existingPaymentDiv").show();
            _self.loadExistingPayments();
        }

        _self.loadClientDocuments();
        $("#divAddReceivableOrder").show();
        $("#divListReceivableOrder").show();


    });



    var dataUrl = $("#hdnGetClientsUrl").attr("data-url");

    $("#ddClient").select2({
        placeholder: 'Select...',
        minimumInputLength: 3,
        allowClear: true,
        ajax: {
            //How long the user has to pause their typing before sending the next request
            quietMillis: 150,
            url: dataUrl,
            dataType: 'json',
            //Our search term and what page we are on
            data: function (term) {
                return {
                    search: term
                };
            },
            results: function (data) {
                return { results: data };
            }
        }
    }).on('change', function (e) {
        var clientId = $("#ddClient").val();
        _self.clearDocumentAdd();
        _self.payDocuments.removeAll();
        _self.existingPayments.removeAll();
        _self.clearComputations();
        _self.clearPaymentInfo();
        $('input[name=isNewPayment]').parent().removeClass("checked");
        $('input[name=isNewPayment]').prop('checked', false);
        $('input[name=isNewPayment]').attr('disabled', 'disabled');
        $("#newPaymentDiv").hide();
        $("#existingPaymentDiv").hide();
        $("#divAddReceivableOrder").hide();
        $("#divListReceivableOrder").hide();

        if (clientId != null && clientId > 0) {

            $('input[name=isNewPayment]').removeAttr('disabled');
        }
    });

    $("#txtChequeAmount").keypress(
        function (e) {
            _self.clearComputations();
            _self.clearDocumentAdd();
            _self.payDocuments.removeAll();
            _self.PaymentAmount($("#txtChequeAmount").val());
        });

    //$("#ddClientDocuments").change(
    //    function (e) {
            
    //    });
};

