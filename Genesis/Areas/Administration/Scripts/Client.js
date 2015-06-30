
$(function () {
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "positionClass": "toast-top-right",
        "onclick": null,
        "showDuration": "3000",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };

    initButton();

    vm = new ViewModel();
    ko.applyBindings(vm);

});

var initButton = function () {

    $('#btnAddSubmit').click(function () {
        $('#formAddEdit').submit();
    });

    $('#btnResetFilter').click(function () {
        $('#clientNameSearch').val('');
        $('#clientCodeSearch').val('');
        $('#clientContactNumberSearch').val('');
        $('#clientContactPersonSearch').val('');
        //$('#ddlStatus').val('');
    });
};

var ViewModel = function () {
    var _self = this;
    this.isLoading = ko.observable(false);
    _self.clients = ko.observableArray();

    _self.client = {
        clientId: ko.observable(),
        clientCode: ko.observable(),
        clientName: ko.observable(),
        clientAddress: ko.observable(),
        clientContactNumber: ko.observable(),
        clientContactPerson: ko.observable(),
        status: ko.observable(true),
        branchId: ko.observable()
    };

    _self.asyncOperation = function () {
        //alert(getFilters());
        _self.isLoading(true);
        var dataUrl = $("#hdnGetClientsUrl").attr("data-url");
        $.ajax({
            //url: '/Administration/Client/GetAllClients' + getFilters(),
            url: dataUrl + getFilters(),
            type: 'POST',
            dataType: 'json',
            success: function (d) {
                $(".problemAjax").hide();
                _self.isLoading(false);
                _self.clients(d);
            },
            error: function() {
                $(".problemAjax").show();
                _self.isLoading(false);

            }
        });
    };

    _self.deactivate = function(clients) {
        
        bootbox.dialog({
            message: "Are you sure you want to deactivate client: " + clients.clientName + " ?.",
            title: "Confirmation",
            buttons: {
                danger: {
                    label: "Cancel",
                    className: "default",
                    callback: function () {
                    }
                },
                success: {
                    label: "Yes",
                    className: "blue",
                    callback: function () {
                        $.ajax({
                            url: '/Administration/Client/DeactivateClient',
                            type: 'POST',
                            data: { id: clients.clientId },
                            dataType: 'json',
                            async: false,
                            success: function (d) {
                                $(".problemAjax").hide();
                                
                                toastr.success('Successfully Deactivated Client', 'Notification');
                            },
                            error: function () { $(".problemAjax").show(); }
                        });
                        //_self.asyncOperation();
                    }
                    
                }

            }
        });
    };

    _self.activate = function(clients) {
        

        bootbox.dialog({
            message: "Are you sure you want to activate client: " + clients.clientName + " ?.",
            title: "Confirmation",
            buttons: {
                danger: {
                    label: "Cancel",
                    className: "default",
                    callback: function () {
                    }
                },
                success: {
                    label: "Yes",
                    className: "blue",
                    callback: function () {
                        $.ajax({
                            url: '/Administration/Client/ActivateClient',
                            type: 'POST',
                            data: { id: clients.clientId },
                            async: false,
                            success: function (d) {
                                $(".problemAjax").hide();
                                toastr.success('Successfully Activated Client','Notification');
                            },
                            error: function () { $(".problemAjax").show(); }
                        });
                        _self.asyncOperation();
                    }
                }

            }
        });
    };

    _self.viewClient = function (clients) {
        var dataUrl = $("#hdnViewClientUrl").attr("data-url");
        window.location =dataUrl+"?id=" + clients.clientId;  
    };

    _self.addEdit = function () {

        var param = {
            client: _self.client
        };
        var dataUrl = $("#hdnAddEditClientUrl").attr("data-url");

        $.ajax({
            //url: '/Administration/Client/AddEditClient',
            url: dataUrl,
            type: 'POST',
            contentType: 'application/json;charset=utf-8',
            data: ko.toJSON(param),
            dataType: 'json',
            success: function (d) {
                $('#AddEdit').modal('hide');
                _self.asyncOperation();
            },
            error: function () {
                alert("error");
            }
        });


    };

    _self.editRow = function (clients) {

        var form1 = $('#formAddEdit');
        var validator = form1.validate();
        validator.resetForm();
        $('.problemForm', form1).hide();

        _self.client.clientId(clients.clientId);
        _self.client.clientCode(clients.clientCode);
        _self.client.clientName(clients.clientName);
        _self.client.clientAddress(clients.clientAddress);
        _self.client.clientContactNumber(clients.clientContactNumber);
        _self.client.clientContactPerson(clients.clientContactPerson);


        if (clients.status == 1) {

            _self.client.status(true);
        } else {

            _self.client.status(false);
        }

        _self.client.branchId(clients.branchId);
        $('#AddEdit').modal('show');

    };

    _self.addRow = function () {

        _self.client.clientId('');
        _self.client.clientCode('');
        _self.client.clientName('');
        _self.client.clientAddress('');
        _self.client.clientContactNumber('');
        _self.client.clientContactPerson('');
        _self.client.status(true);
        _self.client.branchId('');
        $('#AddEdit').modal('show');
    };

    _self.formValidation = function () {
        var form1 = $('#formAddEdit');
        var error1 = $('.problemForm', form1);
        var success1 = $('.alert-success', form1);
        var dataUrl = $("#hdnClientCodeExistsUrl").attr("data-url");
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
                clientCode: {
                    minlength: 3,
                    maxlength: 20,
                    required: true,
                    remote: {
                        //url: "/Administration/Client/CheckClientCodeExists",
                        url: dataUrl,
                        type: 'POST',
                        data: {
                            id: function () {
                                return $("#clientId").val();
                            }
                        }
                    },
                },
                clientName: {
                    required: true
                },

                clientAddress: {
                    required: false,
                },

                clientContactNumber: {
                    required: false,
                },


                clientContactPerson: {
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

    _self.asyncOperation();
    
    _self.formValidation();
};

   


