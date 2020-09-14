﻿var isPhone = false;
var isEmail = false;
let timerOn = true;
var remaining = 0;
var myTimer;
$(document).on('click', '.icon-eye i, .signin-icon-eye i', function (event) {
    var password = $('.txt-password');
    if (password != '') {
        if (password.attr("type") === "password") {
            password.attr("type", "text");
        } else {
            password.attr("type", "password");
        }
    }
});

$(document).on('click', '.signup-submit, .signin-submit', function (event) {
    $('.loader').removeClass('d-none');
});

$(document).on('keyup', '.txt-name, .txt-otp', function (event) {
    checkValues();
});

$(document).on('keyup', '.txt-password', function (event) {
    var password = $(this).val();
    if (password != '') {
        $('.icon-eye').removeClass('d-none');
    }
    else {
        $('.icon-eye').addClass('d-none');
    }
    checkValues();
});

$(document).on('keyup', '.txt-username', function (event) {
    checkEmailOrPhone();
    if (isPhone) {
        var username = $(this).val();
        if (username = '' || username.length != 10) {
            stopTimer();
        }
    }
    checkValues();
});

$(document).on('change', '.txt-username', function (event) {
    checkEmailOrPhone();
});

$(document).on('change', '.reset-email', function (event) {
    var email = $(this).val();
    if (email != '') {
        $('.reset-submit').removeClass('disabled');
    }
    else {
        $('.reset-submit').addClass('disabled');
    }
});

function clearValues() {
    $('.txt-name').val('');
    $('.txt-username').val('');
    $('.txt-password').val('');
    $('.txt-otp').val('');
    $('.signup-submit').addClass('disabled');
    $('.signup-submit span').addClass('d-none');
}

function checkValues() {
   var name = $('.txt-name').val();
    var username = $('.txt-username').val();
    var password = $('.txt-password').val();
    var otp = $('.txt-otp').val();

    if (window.location.pathname == "/") {
        if (isEmail) {
            if (username != '' && password != '') {
                $('.signin-submit').removeClass('disabled');
            }
            else {
                $('.signin-submit').addClass('disabled');
            }
        }
        else if (isPhone) {
            if (username != '' && otp != '') {
                $('.signin-submit').removeClass('disabled');
            }
            else {
                $('.signin-submit').addClass('disabled');
            }
        }
    }
    else {
        if (isEmail) {
            if (name != '' && username != '' && password != '') {
                $('.signup-submit').removeClass('disabled');
            }
            else {
                $('.signup-submit').addClass('disabled');
            }
        }
        else if (isPhone) {
            if (name != '' && username != '' && otp != '') {
                $('.signup-submit').removeClass('disabled');
            }
            else {
                $('.signup-submit').addClass('disabled');
            }
        }
    }

}

function checkEmailOrPhone() {
    var username = $('.txt-username').val();
    var emailReg = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    var intRegex = /[0-9 -()+]+$/;
    if (emailReg.test(username)) {
        isEmail = true;
    }
    else if (intRegex.test(username)) {
        isPhone = true;
    }
}

function timer(remaining) {
    var m = Math.floor(remaining / 60);
    var s = remaining % 60;

    m = m < 10 ? '0' + m : m;
    s = s < 10 ? '0' + s : s;
    document.getElementById('otpbox').placeholder = "Enter otp " + m + ':' + s;
    remaining -= 1;

    if (remaining >= 0 && timerOn) {
        myTimer = setTimeout(function () {
            timer(remaining);
        }, 1000);
        return myTimer;
    }

    if (!timerOn) {
        // Do validate stuff here
        return;
    }

    // Do timeout stuff here
    document.getElementById('otpbox').placeholder = "Enter otp";
    $('#resendotp').removeClass('d-none');
}

function stopTimer() {
    clearTimeout(myTimer);
    if (document.getElementById('otpbox') != null)
        document.getElementById('otpbox').placeholder = "Enter otp";
    $('.otp-send-success').addClass('d-none');
}