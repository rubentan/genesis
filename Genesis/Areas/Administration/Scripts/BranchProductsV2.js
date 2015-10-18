
$(function () {

    initButton();

    vm = new ViewModel();
    ko.applyBindings(vm);

});

var initButton = function () {

    $('#btnAddSubmit').click(function() {
        $('#formAddEdit').submit();
    });
 

    $('#btnResetFilter').click(function () {
        $('#productCodeSearch').val('');
        $('#productDescriptionSearch').val('');
    });


};



var ViewModel = function () {
    var _self = this;
    this.isLoading = ko.observable(false);
    _self.products = ko.observableArray();
    _self.priceHistory = ko.observableArray();

    _self.product = {
        productId: ko.observable(),
        productCode: ko.observable(),
        productName: ko.observable(),
        productDescription: ko.observable(),
        categoryId: ko.observable(),
        categoryName: ko.observable(),
        reorderLevel: ko.observable(),
        UOM: ko.observable(),
        unitPrice: ko.observable(),
        beginning: ko.observable(),
        incoming: ko.observable(),
        outgoing: ko.observable(),
        ending: ko.observable()
};

    _self.asyncOperation = function () {
        _self.isLoading(true);
        var dataUrl = $("#hdnLoadBranchProductsUrl").attr("data-url");
        $.ajax({
            //url: '/Administration/Product/GetAllBranchProducts' + getFilters(),
            url: dataUrl + getFilters(),
            type: 'POST',
            dataType: 'json',
            success: function (d) {
                $(".problemAjax").hide();
                _self.isLoading(false);
                _self.products(d);
            },
            error: function() {
                $(".problemAjax").show();
                _self.isLoading(false);

            }
        });
    };

    _self.viewProduct = function (products) {
        var dataUrl = $("#hdnViewProductUrl").attr("data-url");
        window.location = dataUrl +"?id=" + products.productId;
        //window.location = "/Administration/Product/ViewProductDetails?id=" + products.productId;

    };

    _self.addEdit = function () {
        var param = {
            product: _self.product
        };
        var dataUrl = $("#hdnAddEditUrl").attr("data-url");
        $.ajax({
            //url: '/Administration/Product/AddEditProduct
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

    _self.viewPrice = function (products) {

        var dataUrl = $("#hdnPriceHistoryUrl").attr("data-url");
        $.ajax({
            //url: '/Administration/Product/GetProductPriceHistory',
            url: dataUrl,
            type: 'POST',
            async: true,
            dataType: 'json',
            data: { id: products.productId },
            success: function (d) {
                $(".problemAjax").hide();
                _self.priceHistory(d);
            },
            error: function () { $(".problemAjax").show(); }
        });

        $('#viewPrice').modal('show');

    };

    _self.saveRow = function(products) {
        //alert(products.beginning + '-' + products.incoming + '-' + products.outgoing );
        //_self.products = '123';

        _self.product.productId(products.productId);
        _self.product.beginning(products.beginning);
        _self.product.incoming(products.incoming);
        _self.product.outgoing(products.outgoing);

        var param = {
            product: _self.product
        };
        var dataUrl = $("#hdnInLineEditUrl").attr("data-url");
        $.ajax({
            url: dataUrl,
            type: 'POST',
            contentType: 'application/json;charset=utf-8',
            data: ko.toJSON(param),
            dataType: 'json',
            success: function (d) {
                _self.asyncOperation();
            },
            error: function () {
                alert("error");
            }
        });


    };

    _self.editRow = function (products) {

        var form1 = $('#formAddEdit');
        var validator = form1.validate();
        validator.resetForm();
        $('.problemForm', form1).hide();

        _self.product.productId(products.productId);
        _self.product.productCode(products.productCode);
        _self.product.productName(products.productName);
        _self.product.productDescription(products.productDescription);
        _self.product.categoryId(products.categoryId);
        _self.product.categoryName(products.categoryName);
        _self.product.reorderLevel(products.reorderLevel);
        _self.product.UOM(products.UOM);
        _self.product.unitPrice(products.unitPrice);
        _self.product.beginning(products.beginning);
        _self.product.incoming(products.incoming);
        _self.product.outgoing(products.outgoing);

        $('#select2-chosen-1').html(products.categoryName);
        $('#AddEdit').modal('show');

    };

    _self.addRow = function () {

        var form1 = $('#formAddEdit');
        var validator = form1.validate();
        validator.resetForm();
        $('.problemForm', form1).hide();

        _self.product.productId('');
        _self.product.productCode('');
        _self.product.productName('');
        _self.product.productDescription('');
        _self.product.categoryId('');
        _self.product.categoryName('');
        _self.product.reorderLevel('');
        _self.product.UOM('');
        _self.product.unitPrice('');
        _self.product.beginning('');
        _self.product.incoming('');
        _self.product.outgoing('');
        $('#select2-chosen-1').html('');
        $('#AddEdit').modal('show');
    };

    _self.formValidation = function () {
        var form1 = $('#formAddEdit');
        var error1 = $('.problemForm', form1);
        var success1 = $('.alert-success', form1);
        var dataUrl = $("#hdnProductCodeExistsUrl").attr("data-url");
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
                productCode: {
                    minlength: 3,
                    maxlength: 20,
                    required: true,
                    remote: {
                        //url: "/Administration/Product/CheckProductCodeExists",
                        url: dataUrl,
                        type: 'POST',
                        data: {
                            id: function () {
                                return $("#productId").val();
                            }
                        }
                    },
                },
                hdnCategoryId: {
                    required: true
                },
                productDescription: {
                    required: true
                },

                categoryId: {
                    required: true,
                },

                reorderLevel: {
                    required: true,
                },

                unitPrice: {
                    required: true,
                },
                beginning: {
                    required: true,
                },
                incoming: {
                    required: true,
                },
                outgoing: {
                    required: true,
                },
                UOM: {
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

    _self.handlePicker = function () {
        var dataUrl = $("#hdnLoadProductCategoriesUrl").attr("data-url");
        $("#categoryId").select2({
            placeholder: 'Select...',
            //Does the user have to enter any data before sending the ajax request
            minimumInputLength: 1,
            allowClear: true,
            ajax: {
                //How long the user has to pause their typing before sending the next request
                quietMillis: 150,
                //The url of the json service
                //url: 'GetAllProductCategories',
                url: dataUrl,
                dataType: 'json',
                //Our search term and what page we are on
                data: function(term) {
                    return {
                        search: term
                    };
                },
                results: function(data) {
                    //Used to determine whether or not there are more results available,
                    //and if requests for more data should be sent in the infinite scrolling
                    //_self.product.categoryId = data.id;
                    //alert(data.id);
                    return { results: data };
                }
            }
        }).on("change", function (e) {
            _self.product.categoryId($('#categoryId').val()); // what you would like to happen
        });
    };

    _self.asyncOperation();

    _self.formValidation();

    _self.handlePicker();
};

