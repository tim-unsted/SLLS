$(document).ready(function () {
    $("#searchForm").keypress(function (e) {
        kCode = e.keyCode || e.charCode; //for cross browser
        if (kCode === 13) {
            var defaultbtn = $(this).attr("DefaultButton");
            $("#" + defaultbtn).click();
            return false;
        }
        return true;
    });
    var width = $('.g-recaptcha').parent().width();
    if (width < 302) {
        var scale = width / 302;
        $('.g-recaptcha').css('transform', 'scale(' + scale + ')');
        $('.g-recaptcha').css('-webkit-transform', 'scale(' + scale + ')');
        $('.g-recaptcha').css('transform-origin', '0 0');
        $('.g-recaptcha').css('-webkit-transform-origin', '0 0');
    }
});