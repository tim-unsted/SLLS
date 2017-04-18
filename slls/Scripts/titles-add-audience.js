$('#addAudienceLink').click(function () {
    var createFormUrl = $(this).attr('href');
    $('#newAudienceDialog').html('')
        .load(createFormUrl, function () {
            jQuery.validator.unobtrusive.parse('#addNewFrm');
            $('#newAudienceDialog').dialog('open');
        });
    return false;
});

$('#AudienceID').change(function () {
    if (this.value === "-1") {
        var createFormUrl = $('#addAudienceLink').attr('href');
        $('#newAudienceDialog').html('')
            .load(createFormUrl, function () {
                jQuery.validator.unobtrusive.parse('#addNewFrm');
                $('#newAudienceDialog').dialog('open');
            });
    }
    return false;
});

$(function () {
    $("#newAudienceDialog").dialog({
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
                    var addNewFrm = $('#addAudienceForm');
                    if (addNewFrm.valid()) {
                        $.ajax({
                            url: $("#AddAudienceUrl").val(),
                            type: 'Post',
                            data: addNewFrm.serialize(),
                            async: false,
                            success: function (data) {
                                if (data.success) {
                                    $('#AudienceID').append(
                                        $('<option></option>')
                                        .val(data.newData.AudienceID)
                                        .html(data.newData.Audience1)
                                        .prop('selected', true)
                                    );
                                    $('#newAudienceDialog').dialog('close');
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
                    $('#AudienceID').val(0);
                    $(this).dialog('close');
                }
            }
        ]
    });
});

