$(function () {
    //initGrid();
    initButton();

    vm = new ViewModel();
    ko.applyBindings(vm);
});

var initButton = function () {
    $('#btnAddSubmit').click(function () {
        $('#formAddEdit').submit();
    });

    $('#btnResetFilter').click(function () {
        $('#txtSupplierCode').val('');
        $('#txtSupplierName').val('');
        $('#txtContactNumber').val('');
        $('#txtContactPerson').val('');
        $('#ddlStatus').val('');
    });

};


var ViewModel = function () {
    var _self = this;
    this.isLoading = ko.observable(false);
    _self.suppliers = ko.observableArray();
    _self.records = ko.observable("0");
    _self.page = ko.observable("1");

    _self.pages = ko.pureComputed(function () {
        return Math.ceil(_self.records() / $('#recordPerPage').val());
    }, this);


    _self.supplier = {
        supplierId: ko.observable(),
        supplierCode: ko.observable(),
        supplierName: ko.observable(),
        supplierAddress: ko.observable(),
        supplierContactNumber: ko.observable(),
        supplierContactPerson: ko.observable(),
        status: ko.observable(true),
        branchId: ko.observable()
    };

    _self.nextPage = function () {
        if (_self.page() == _self.pages()) {
            _self.page("1");
        } else {
            _self.page(parseInt(_self.page()) + parseInt(1));
        }
        _self.asyncOperation();
    };

    _self.previousPage = function () {
        if (_self.page() == 1) {
            _self.page(_self.pages());
        } else {
            _self.page(parseInt(_self.page()) - parseInt(1));
        }
        _self.asyncOperation();
    };


    _self.filterSubmit = function () {
        _self.page("1");
        _self.asyncOperation();
    };

    _self.asyncOperation = function () {
        //alert(getFilters());
        _self.isLoading(true);
        var dataUrl = $("#hdnLoadSuppliersUrl").attr("data-url");

        $.ajax({
            //url: '/Administration/Supplier/GetAllSuppliers' + getFilters(),
            url: dataUrl + getFilters(),
            type: 'POST',
            dataType: 'json',
            success: function (d) {
                $(".problemAjax").hide();
                _self.isLoading(false);
                if (Object.keys(d).length > 0) {
                    _self.records(d[0]["records"]);
                } else {
                    _self.records("0");
                    _self.page("0");
                }
                _self.suppliers(d);
            },
            error: function() {
                $(".problemAjax").show();
                _self.isLoading(false);
            }
        });
    };

    _self.viewSupplier = function (suppliers) {
        var dataUrl = $("#hdnViewSupplierUrl").attr("data-url");
        window.location = dataUrl + "?id=" + suppliers.supplierId;
        //window.location = "/Administration/Supplier/ViewSupplier?id=" + suppliers.supplierId;
    };

    _self.addEdit = function () {
        var param = {
            supplier: _self.supplier
        };
        var dataUrl = $("#hdnAddEditUrl").attr("data-url");

        $.ajax({
            //url: '/Administration/Supplier/AddEditSupplier',
            url:dataUrl,
            type: 'POST',
            contentType: 'application/json;charset=utf-8',
            data: ko.toJSON(param),
            dataType: 'json',
            success: function (d) {
                $('.problemAjaxAdd').Hide();
                $('#AddEdit').modal('hide');
                _self.asyncOperation();
            },
            error: function () {
                $('.problemAjaxAdd').Show();
            }
        });


    };

    _self.editRow = function (suppliers) {

        var form1 = $('#formAddEdit');
        var validator = form1.validate();
        validator.resetForm();
        $('.problemForm', form1).hide();

        _self.supplier.supplierId(suppliers.supplierId);
        _self.supplier.supplierCode(suppliers.supplierCode);
        _self.supplier.supplierName(suppliers.supplierName);
        _self.supplier.supplierAddress(suppliers.supplierAddress);
        _self.supplier.supplierContactNumber(suppliers.supplierContactNumber);
        _self.supplier.supplierContactPerson(suppliers.supplierContactPerson);

        
        if (suppliers.status == 1) {
            
            _self.supplier.status(true);
        } else {
            
            _self.supplier.status(false);
        }
        
        _self.supplier.branchId(suppliers.branchId);
        $('#AddEdit').modal('show');

    };

    _self.addRow = function () {

        var form1 = $('#formAddEdit');
        var validator = form1.validate();
        validator.resetForm();
        $('.problemForm', form1).hide();

        _self.supplier.supplierId('');
        _self.supplier.supplierCode('');
        _self.supplier.supplierName('');
        _self.supplier.supplierAddress('');
        _self.supplier.supplierContactNumber('');
        _self.supplier.supplierContactPerson('');
        _self.supplier.status(true);
        _self.supplier.branchId('');
        $('#AddEdit').modal('show');
    };

    _self.formValidation = function () {
        var form1 = $('#formAddEdit');
        var error1 = $('.problemForm', form1);
        var success1 = $('.alert-success', form1);
        var dataUrl = $("#hdnSupplierCodeExistsUrl").attr("data-url");
        form1.validate({
            errorElement: 'span', //default input error message container
            errorClass: 'help-block help-block-error', // default input error message class
            focusInvalid: false, // do not focus the last invalid input
            ignore: "", // validate all fields including form hidden input
            messages: {
                select_multi: {
                    maxlength: jQuery.validator.format("Max {0} items allowed for selection"),
                    minlength: jQuery.validator.format("At least {0} items must be selected"),
                }
            },
            rules: {
                supplierCode: {
                    minlength: 3,
                    maxlength: 20,
                    required: true,
                    remote: {
                        //url: "/Administration/supplier/CheckSupplierCodeExists",
                        url: dataUrl,
                        type: 'POST',
                        data: {
                            id: function () {
                                return $("#supplierId").val();
                            }
                        }
                    },
                },
                supplierName: {
                    required: true
                },

                supplierAddress: {
                    required: false,
                },

                supplierContactNumber: {
                    required: false,
                },


                supplierContactPerson: {
                    required: false,
                },
                status: {
                    required: false,
                }
            },

            invalidHandler: function (event, validator) { //display error alert on form submit              
                //success1.hide();
                error1.show();
                Metronic.scrollTo(error1, -200);
            },

            highlight: function (element) { // hightlight error inputs
                $(element)
                    .closest('.form-group').addClass('has-error'); // set error class to the control group
            },

            unhighlight: function (element) { // revert the change done by hightlight
                $(element)
                    .closest('.form-group').removeClass('has-error'); // set error class to the control group
            },

            success: function (label) {
                label
                    .closest('.form-group').removeClass('has-error'); // set success class to the control group
            },

            submitHandler: function (form) {
                //success1.show();

                error1.hide();
                _self.addEdit();
            }
        });
    };


    _self.filterSubmit();

    _self.formValidation();

};