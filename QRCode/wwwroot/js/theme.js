// JavaScript Document
$(".quick-search #single").select2({
    placeholder: "Select Category",
    allowClear: true
});

$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();
});


$('.favrt').on('click', function (e) {
    e.preventDefault();
    $(this).toggleClass('favrtadded ');
    $(this).children('.favrt').toggleClass('favrtadded ');
});

$('.back-top a').click(function () {
    $('body,html').animate({
        scrollTop: 0
    }, 800);
    return false;
});

$(document).scroll(function () {
    if ($(this).scrollTop() > 100) {
        $('.back-top').css("display", "block");
    } else {
        $('.back-top').css("display", "none");
    }
});
