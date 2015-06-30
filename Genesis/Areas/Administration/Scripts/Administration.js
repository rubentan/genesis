$(function () {
    handleLogin();
});


var handleLogin = function () {

    $('.login-form').validate({
        errorElement: 'span', //default input error message container
        errorClass: 'help-block', // default input error message class
        focusInvalid: false, // do not focus the last invalid input
        rules: {
            username: {
                required: true
            },
            password: {
                required: true
            },
            remember: {
                required: false
            }
        },


        invalidHandler: function (event, validator) { //display error alert on form submit   
            clearErrors();
            $('.error-required', $('.login-form')).show();
        },

        highlight: function (element) { // hightlight error inputs
            $(element)
                .closest('.form-group').addClass('has-error'); // set error class to the control group
        },

        success: function (label) {
            label.closest('.form-group').removeClass('has-error');
            label.remove();
        },

        errorPlacement: function (error, element) {
            error.insertAfter(element.closest('.input-icon'));
        },

        ////submitHandler: function (form) {
        ////    form.submit(); // form validation success, call ajax form submit
        ////}
    });

    //$('.login-form input').keypress(function (e) {
    //    if (e.which == 13) {
    //        if ($('.login-form').validate().form()) {
    //            login();
    //            //$('.login-form').submit(); //form validation success, call ajax form submit
    //        }
    //        return false;
    //    }
    //});

    var currentBoxNumber = 0;
    $(".formItem").keyup(function (event) {
        if (event.keyCode == 13) {
            textboxes = $("input.formItem");
            currentBoxNumber = textboxes.index(this);
            console.log(textboxes.index(this));
            if (textboxes[currentBoxNumber + 1] != null) {
                nextBox = textboxes[currentBoxNumber + 1];
                nextBox.focus();
                nextBox.select();
                event.preventDefault();
                return false;
            }
        }
    });


    $('#btnLogin').click(function () {
        if ($('.login-form').validate().form()) {
            login();
            //$('.login-form').submit(); //form validation success, call ajax form submit
        }
    });
};

var clearErrors = function() {
    $('.alert-danger', $('.login-form')).hide();
};

var login = function() {
    $('.loginLoad').show();
    $('#txtUsername').attr("readonly", true).attr("disabled", true);
    $('#txtPassword').attr("readonly", true).attr("disabled", true);
    $('#btnLogin').attr("readonly", true).attr("disabled", true);

    var user = {
        userName: $('#txtUsername').val(),
        passWord: $('#txtPassword').val()
    };

   // alert($('#btnLogin').attr('data-url'));

    $.ajax({
        async: true,
        cache: false,
        //url: '/Account/LoginUser',
        url: $('#btnLogin').attr('data-url'),
        type: 'POST',
        data: JSON.stringify(user),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (d) {
            successfulLogin(d.returnObj);
            $('.loginLoad').hide();
        },
        error: function() {
            $('.error-exception', $('.login-form')).show();
            $('.loginLoad').hide();
        }
    });
    return false;
};

var successfulLogin = function (d) {

    var dataUrl = $("#hdnIndexUrl").attr("data-url");
    if (d == null) {
        clearErrors();
        $('#txtUsername').attr("readonly", false).attr("disabled", false);
        $('#txtPassword').attr("readonly", false).attr("disabled", false);
        $('#btnLogin').attr("readonly", false).attr("disabled", false);
        $('.loginLoad').hide();

        $('.error-invalid', $('.login-form')).show();
    } else {
        window.location = dataUrl;
    }
};
