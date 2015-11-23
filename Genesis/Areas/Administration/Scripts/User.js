
$(function () {
    initButton();
    var vm = new ViewModel();
    ko.applyBindings(vm);
});

var initButton = function () {

    $('#btnAddUserSubmit').click(function () {
        $('#formAddEditUser').submit();
    });

    $('#btnResetFilter').click(function () {
        $('#searchUserName').val('');
        $('#searchFirstName').val('');
        $('#searchLastName').val('');
        $('#searchEmailAddress').val('');
    });

};

var ViewModel = function () {
    var _self = this;
    this.isLoading = ko.observable(false);
    _self.users = ko.observableArray();
    _self.branches = ko.observableArray();
    _self.parentUser = ko.observable("");
    _self.records = ko.observable("0");
    _self.page = ko.observable("1");

    _self.pages = ko.pureComputed(function () {
        return Math.ceil(_self.records() / $('#recordPerPage').val());
    }, this);

    _self.user = {
        userId: ko.observable(),
        userName: ko.observable(),
        passWord: ko.observable(),
        passWord2: ko.observable(),
        emailAddress: ko.observable(),
        firstName: ko.observable(),
        middleName: ko.observable(),
        lastName: ko.observable(),
        branchId : ko.observable()
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
        var dataUrl = $("#hdnLoadUsersUrl").attr("data-url");

        $.ajax({
            //url: '/Administration/Administration/GetAllUsers' + getFilters(),
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
                _self.users(d);
            },
            error: function() {
                $(".problemAjax").show();
                _self.isLoading(false);

            }
        });
    };

    _self.loadBranches = function () {
        //alert(getFilters());
        //_self.isLoading(true);
        var dataUrl = $("#hdnLoadBranchesUrl").attr("data-url");

        $.ajax({
            //url: '/Administration/Branch/GetAllBranchesForDropDown',
            url: dataUrl,
            type: 'POST',
            dataType: 'json',
            async: false,
            success: function (d) {
                $(".problemAjax").hide();
                //_self.isLoading(false);
                _self.branches(d);
                //alert(d[0].branchName.toString());
            },
            error: function () {
                $(".problemAjax").show();
                _self.isLoading(false);

            }
        });
    };

    _self.deactivate = function(users) {
        var dataUrl = $("#hdnDeactivateUserUrl").attr("data-url");

        bootbox.dialog({
            message: "Are you sure you want to deactivate user: " + users.userName + " ?.",
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
                            //url: '/Administration/Administration/DeactivateUser',
                            url: dataUrl,
                            type: 'POST',
                            data: { id: users.userId},
                            dataType: 'json',
                            async: false,
                            success: function (d) {
                                $(".problemAjax").hide();
                                
                                toastr.success('Successfully Deactivated User', 'Notification');
                            },
                            error: function () { $(".problemAjax").show(); }
                        });
                        //_self.asyncOperation();
                    }
                    
                }

            }
        });
    };

    _self.activate = function(users) {

        var dataUrl = $("#hdnActivateUserUrl").attr("data-url");

        bootbox.dialog({
            message: "Are you sure you want to activate user: " + users.userId+ " ?.",
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
                            //url: '/Administration/Administration/ActivateUser',
                            url: dataUrl,
                            type: 'POST',
                            data: { id: users.userId },
                            async: false,
                            success: function (d) {
                                $(".problemAjax").hide();
                                toastr.success('Successfully Activated User','Notification');
                            },
                            error: function () { $(".problemAjax").show(); }
                        });
                        _self.asyncOperation();
                    }
                }

            }
        });
    };

    _self.addEditUser = function () {
        var user = {
            user: _self.user
        };

        var form1 = $('#AddEditUser');
        var error1 = $('.problemAjaxAddUser', form1);
        var dataUrl = $("#hdnAddEditUserUrl").attr("data-url");

        //alert(dataUrl);

        $.ajax({
            //url: '/Administration/Administration/AddEditUser',
            url: dataUrl,
            type: 'POST',
            contentType: 'application/json;charset=utf-8',
            data: ko.toJSON(user),
            dataType: 'json',
            success: function (d) {
                error1.hide();
                $('#AddEditUser').modal('hide');
                _self.asyncOperation();
            },
            error: function () {
                error1.show();
            }
        });


    };

    _self.editUserRow = function (users) {

        var form1 = $('#formAddEditUser');
        var validator = form1.validate();
        validator.resetForm();
        $('.problemForm', form1).hide();
        //$('#AddEditUser').find('form')[0].reset();
        //alert("["+users.passWord+"]");
        _self.user.userId(users.userId);
        _self.user.userName(users.userName);
        _self.user.passWord(users.passWord);
        _self.user.passWord2(users.passWord);
        _self.user.emailAddress(users.emailAddress);
        _self.user.firstName(users.firstName);
        _self.user.middleName(users.middleName);
        _self.user.lastName(users.lastName);
        _self.user.branchId(users.branchId);
        $('#AddEditUser').modal('show');

    };

    _self.addUserRow = function() {

        //var validator = $('#formAddEditUser').validate();
        //$('#AddEditUser').find('form')[0].reset();
        //validator.resetForm();
        _self.user.userId('0');
        _self.user.userName('');
        _self.user.passWord('');
        _self.user.passWord2('');
        _self.user.emailAddress('');
        _self.user.firstName('');
        _self.user.middleName('');
        _self.user.lastName('');
        _self.user.branchId('');
        $('#AddEditUser').modal('show');

    };

    _self.formValidation = function() {
        var form1 = $('#formAddEditUser');
        var error1 = $('.problemForm', form1);
        var success1 = $('.alert-success', form1);
        var dataUrl = $("#hdnCheckUserNameExistsUrl").attr("data-url");

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
                username: {
                    minlength: 4,
                    maxlength: 20,
                    required: true,
                    remote: {
                        //url: "/Administration/Administration/CheckUserNameExists",
                        url: dataUrl,
                        type: 'POST',
                        data: {
                            id: function () {
                                return $("#userId").val();
                            }
                        }
                    },
                },
                emailaddress: {
                    required: true,
                    email: true
                },
                password: {
                    minlength: 5,
                    required: true
                },
                password2: {
                    minlength: 5,
                    equalTo: "#password"
                },
                firstname: {
                    required: true,
                },
                lastname: {
                    required: true
                },
                middlename: {
                    required: false
                },
                branch: {
                    required: true
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
                _self.addEditUser();
            }
        });
    };

    _self.loadBranches();

    _self.filterSubmit();

    _self.formValidation();

};






   


