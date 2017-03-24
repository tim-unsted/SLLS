$(function () {
    $('#newMediaDialog').dialog({
        open: function () {
            $(this).closest(".ui-dialog")
                .find(".ui-dialog-titlebar-close")
                .removeClass("ui-icon-closethick")
                .addClass("btn btn-default")
                .text("X");
        },
        autoOpen: false,
        modal: true,
        overlay: {
            backgroundColor: '#000',
            opacity: 0.5
        },
        buttons: [
            {
                text: 'Save',
                "class": 'btn btn-success',
                click: function () {
                    var addNewFrm = $('#addMediaForm');
                    if (addNewFrm.valid()) {
                        $.ajax({
                            url: $("#AddMediaUrl").val(),
                            type: 'Post',
                            data: addNewFrm.serialize(),
                            async: false,
                            success: function (data) {
                                if (data.success) {
                                    $('#MediaID').append(
                                        $('<option></option>')
                                        .val(data.newData.MediaID)
                                        .html(data.newData.Media)
                                        .prop('selected', true)
                                    );
                                    $('#newMediaDialog').dialog('close');
                                } else {
                                    alert(data.errMsg);
                                }
                            },
                            error: function () {
                                alert("Oops, there has been an error.");
                            }
                        });
                    }
                }
            },
            {
                text: 'Cancel',
                "class": 'btn btn-default',
                click: function () {
                    $('#MediaID').val(0);
                    $(this).dialog('close');
                }
            }
        ]
    });
});

$('#addMediaLink').click(function () {
    var createFormUrl = $(this).attr('href');
    $('#newMediaDialog').html('')
        .load(createFormUrl, function () {
            //jQuery.validator.unobtrusive.parse('#addNewFrm');
            $('#newMediaDialog').dialog('open');
        });
    return false;
});

$('#MediaID').change(function () {
    if (this.value === "-1") {
        var createFormUrl = $('#addMediaLink').attr('href');
        $('#newMediaDialog').html('')
            .load(createFormUrl, function () {
                //jQuery.validator.unobtrusive.parse('#addNewFrm');
                $('#newMediaDialog').dialog('open');
            });
    }
    return false;
});