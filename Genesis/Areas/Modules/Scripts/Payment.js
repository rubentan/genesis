var vm = {};

$(function () {
    handleDatePickers();

    vm = new PaymentViewModel();
    ko.applyBindings(vm);

    vm.init();
});

var PaymentViewModel = function() {
    var pay = this;

    //Observables
    pay.paymentList = ko.observable();

    //Methods || Functions
    pay.init = function() {
        pay.GetAllPayments();
    };

    pay.Search = function() {
        pay.GetAllPayments();
    };

    pay.Reset = function() {
        $('#txtReferenceNo').val('');
        $('#txtSupplierCode').val('');
        $('#txtSupplierName').val('');
        $('#txtDateFrom').val('');
        $('#txtDateTo').val('');
    };

    pay.GetAllPayments = function() {
        $.ajax({
            url: '/Modules/Purchase/GetAllPayments' + getFilters(),
            type: 'GET',
            dataType: 'json',
            data: {},
            success: function(d) {
                pay.paymentList(d);
            },
            error: function() { alert('ajax error'); }
        });
    };


};


var handleDatePickers = function () {

    if (jQuery().datepicker) {
        $('.date-picker').datepicker({
            rtl: Metronic.isRTL(),
            orientation: "left",
            autoclose: true
        });
        //$('body').removeClass("modal-open"); // fix bug when inline picker is used in modal
    }

    /* Workaround to restrict daterange past date select: http://stackoverflow.com/questions/11933173/how-to-restrict-the-selectable-date-ranges-in-bootstrap-datepicker */
};