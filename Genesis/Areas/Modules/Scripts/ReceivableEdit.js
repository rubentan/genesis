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


//here


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

    $("#txtChequeAmount").keyup(
        function (e) {
            _self.clearComputations();
            _self.clearDocumentAdd();
            _self.payDocuments.removeAll();
            _self.PaymentAmount($("#txtChequeAmount").val());
        });


};

