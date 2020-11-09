var isPhone = false;
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

$(document).on('keyup', '.reset-email', function (event) {
    var email = $(this).val();
    if (email != '') {
        $('.forgot-password-submit').removeClass('disabled');
    }
    else {
        $('.forgot-password-submit').addClass('disabled');
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

    var emailReg = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    var intRegex = /^\d+$/;
    if (emailReg.test(username)) {
        isEmail = true;
    }
    else if (intRegex.test(username)) {
        isPhone = true;
    }

    if (window.location.pathname == "/") {
        if (isEmail) {
            if (username != '' && password != '' && password.length >= 4) {
                $('.signin-submit').removeClass('disabled');
            }
            else {
                $('.signin-submit').addClass('disabled');
            }
        }
        else if (isPhone) {
            if (username != '' && otp != '' && otp.length >= 6) {
                $('.signin-submit').removeClass('disabled');
            }
            else {
                $('.signin-submit').addClass('disabled');
            }
        }
    }
    else {
        if (isEmail) {
            if (name != '' && username != '' && password != '' && password.length >= 4) {
                $('.signup-submit').removeClass('disabled');
            }
            else {
                $('.signup-submit').addClass('disabled');
            }
        }
        else if (isPhone) {
            if (name != '' && username != '' && otp != '' && otp.length >= 6) {
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
    var intRegex = /^\d+$/;
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

function GenerateQRCode(url) {
    $('#parantcode').append('<div id="qrcode" class="d-none"></div>');
    var qrcodjs = new QRCode(document.getElementById("qrcode"), {
        text: url,
        width: 128,
        height: 128,
        colorDark: "#000000",
        colorLight: "#ffffff",
        correctLevel: QRCode.CorrectLevel.H
    });
    var imgBase64Data = qrcodjs._el.childNodes[0].toDataURL();
    $('#qrcode').remove();
    return imgBase64Data;
}

function displayPopupModel(message) {
    $('.display-message').text(message);
    $("#popupModel").modal();
}

$(document).on('keyup', '.pin-box', function (event) {
    var pin = $(this).val();
    if (pin.length >= 4) {
        if ($('.btn-redeem').hasClass("expired-offer") == false) {
            $('.btn-redeem').removeClass('disabled');
        }
        else {
            $('.btn-redeem').addClass('disabled');
        }
    }
    else {
        $('.btn-redeem').addClass('disabled');
    }
});

$('.pin-box').on("keydown", function (event) {
    if (event.keyCode === 8 || event.which === 8 || event.keyCode == 46 || event.which == 46) {
        event.preventDefault();
    }
});



function ParseJWT(token) {
    var base64Url = token.split('.')[1];
    var base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    var jsonPayload = decodeURIComponent(atob(base64).split('').map(function (c) {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
    }).join(''));

    return jsonPayload;
}

function removeModelBackdrop() {
    $('#popupModel').modal('hide');
    $('body').removeClass('modal-open');
    $('.modal-backdrop').remove();
}

$(document).on('keyup', '#resetpassword', function (event) {
    enableSubmit();
});

$(document).on('keyup', '#resetpasswordconfirm', function (event) {
    enableSubmit();
});

function enableSubmit() {
    var password = $('#resetpassword').val();
    var rePassword = $('#resetpasswordconfirm').val();

    if (password != '' && rePassword != '') {
        $('.reset-submit').removeClass('disabled');
    }
    else {
        $('.reset-submit').addClass('disabled');
    }
}

$(document).on('click', '.qr-code-scanner', function () {
    $('.coupons-section').addClass('d-none');
    $('.qr-code-scanner-container').removeClass('d-none');
    $('#reader').removeClass('d-none');
});

$(document).on('click', '.promotion-user-input .promotioncode', function () {
    $("#popupModelForPromotionCode").modal();
});

$(document).on('click', '.back-to-promotions a', function () {
    $('.coupons-section').removeClass('d-none');
    $('#reader').addClass('d-none');
    $('.qr-code-scanner-container').addClass('d-none');
});


$(document).on('keyup', '#promotioncode', function (event) {
    enableSubmitForPromotion();
});

function enableSubmitForPromotion() {
    var promotionCode = $('#promotioncode').val();
    if (promotionCode != '') {
        $('.promotion-code-submit').removeClass('disabled');
    }
    else {
        $('.promotion-code-submit').addClass('disabled');
    }
}




