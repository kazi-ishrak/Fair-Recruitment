function ValidateLogin() {

    var id = $("#id").val();
    var pass = $("#pass").val();

    var url = "/login";

    $.ajax({

        url: url,
        cache: false,
        data: { id: id, pass: pass },
        cache: false,
        type: "POST",
        success: function (data) {
            var token = data.token;
            console.log(data);
            localStorage.setItem("JwtToken", token);
            Snackbar.show({ text: 'Operation Success', pos: 'top-right', backgroundColor: '#4CAF50', actionTextColor: '#000000' });
            window.location.href = "/";
        },

        error: function (response) {
            var resp = 'Email or Password do not match';
            Snackbar.show({ text: resp, pos: 'top-right', backgroundColor: '#DC3545', actionTextColor: '#000000' });
        }

    });
};
(function ($) {
    "use strict";

    
    /*==================================================================
    [ Validate ]*/
    var input = $('.validate-input .input100');

    $('#btnLogin').on('click', function () {

        
        var check = true;

        for(var i=0; i<input.length; i++) {
            if(validate(input[i]) == false){
                showValidate(input[i]);
                check=false;
            }
        }

        

        if (check === true) {
            ValidateLogin();
        }
        else {
            return check;
        }      
    });


    $('.validate-form .input100').each(function(){
        $(this).focus(function(){
           hideValidate(this);
        });
    });

    function validate (input) {
        if ($(input).attr('name') == 'pass' || $(input).attr('name') == 'id') {

            var textRegex = /^[\w\-_\.@]+$/;

            if($(input).val().trim().match(/^([a-zA-Z0-9_\-\.@]+)$/) == null) {
                return false;
            }
        }
        else {
            if($(input).val().trim() == ''){
                return false;
            }
        }
    }

    function showValidate(input) {
        var thisAlert = $(input).parent();

        $(thisAlert).addClass('alert-validate');
    }

    function hideValidate(input) {
        var thisAlert = $(input).parent();

        $(thisAlert).removeClass('alert-validate');
    }
    
    

})(jQuery);