
$(function () {

    initButton();

    var vm = new ViewModel();

    ko.applyBindings(vm);

});

var initButton = function () {

    $('#btnAddSubmit').click(function() {
        $('#formAddEdit').submit();
    });

    $('#btnResetFilter').click(function () {
        $('#searchBranchCode').val('');
        $('#searchBranchName').val('');
    });


};


var ViewModel = function () {

    var _self = this;
    this.isLoading = ko.observable(false);
    _self.branches = ko.observableArray();
    _self.records = ko.observable("0");
    _self.page = ko.observable("1");

    _self.pages = ko.pureComputed(function () {
        return Math.ceil(_self.records() / $('#recordPerPage').val());
    }, this);

    _self.branch = {
        branchId: ko.observable(),
        branchName: ko.observable(),
        branchCode: ko.observable(),
        branchAddress: ko.observable()
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
        _self.isLoading(true);
        var dataUrl = $("#hdnGetBranchesUrl").attr("data-url");
        //alert(dataUrl);
        //alert(getFilters());
        $.ajax({
            //url: '/Administration/Branch/GetAllBranches' + getFilters(),
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
                _self.branches(d);
            },
            error: function() {
                $(".problemAjax").show();
                _self.isLoading(false);

            }
        });
    };

    _self.addEdit = function () {
        var param = {
            branch: _self.branch
        };
        var form1 = $('#formAddEdit');
        var error1 = $('.problemAjaxAdd', form1);
        var dataUrl = $("#hdnAddBranchUrl").attr("data-url");
        
        $.ajax({
            //Url: '/Administration/Branch/AddEditBranch',
            url: dataUrl,
            type: 'POST',
            contentType: 'application/json;charset=utf-8',
            data: ko.toJSON(param),
            dataType: 'json',
            success: function (d) {
                error1.hide();
                $('#AddEdit').modal('hide');
                _self.asyncOperation();

            },
            error: function () {
                error1.show();
            }
        });


    };

    _self.editRow = function (branches) {

        var form1 = $('#formAddEdit');
        var validator = form1.validate();
        validator.resetForm();
        $('.problemForm', form1).hide();
        
        _self.branch.branchId(branches.branchId);
        _self.branch.branchName(branches.branchName);
        _self.branch.branchCode(branches.branchCode);
        _self.branch.branchAddress(branches.branchAddress);
        $('#AddEdit').modal('show');

    };

    _self.addRow = function() {

        _self.branch.branchId('0');
        _self.branch.branchName('');
        _self.branch.branchCode('');
        _self.branch.branchAddress('');
        $('#AddEdit').modal('show');

    };

    _self.formValidation = function() {
        var form1 = $('#formAddEdit');
        var error1 = $('.problemForm', form1);
        var success1 = $('.alert-success', form1);
        var dataUrl = $("#hdnBranchCodeExistsUrl").attr("data-url");

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
                branchCode: {
                    minlength: 3,
                    maxlength: 20,
                    required: true,
                    remote: {
                        //url: "/Administration/branch/CheckBranchCodeExists",
                        url: dataUrl,
                        type: 'POST',
                        data: {
                            id: function () {
                                return $("#branchId").val();
                            }
                        }
                    },
                },
                branchName: {
                    required: true
                },
                
                branchAddress: {
                    required: true,
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






   


