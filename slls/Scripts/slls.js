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

    $(".modal-dialog").draggable({
        handle: ".modal-header"
    });
    $('.modal-dialog').resizable({
        minHeight: 300,
        minWidth: 300
    });


    $(function () {
        $("#dialogSuccess").dialog({
            dialogClass: "no-close",
            modal: true,
            buttons: [
                {
                    text: 'OK',
                    open: function (event, ui) {
                        setTimeout("$('#dialogSuccess').dialog('close')", popupTimeout);
                    },
                    click: function () {
                        $(this).dialog('close');
                    }
                }
            ]
        });
    });

    $(function () {
        $("#dialogError").dialog({
            dialogClass: "no-close",
            modal: true,
            buttons: [
                {
                    text: 'OK',
                    open: function (event, ui) {
                        setTimeout("$('#dialogError').dialog('close')", popupTimeout);
                    },
                    click: function () {
                        $(this).dialog('close');
                    }
                }
            ]
        });
    });

    $(function () {
        $("#dialogInfo").dialog({
            dialogClass: "no-close",
            modal: true,
            buttons: [
                {
                    text: 'OK',
                    open: function (event, ui) {
                        setTimeout("$('#dialogInfo').dialog('close')", popupTimeout);
                    },
                    click: function () {
                        $(this).dialog('close');
                    }
                }
            ]
        });
    });

    

});
