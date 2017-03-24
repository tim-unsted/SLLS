$(function () {
    $('#newLanguageDialog').dialog({
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
                    var addNewFrm = $('#addLanguageForm');
                    if (addNewFrm.valid()) {
                        var newValue = addNewFrm.serialize();
                        newValue = newValue.replace("Language1=", "");
                        $.ajax({
                            url: $("#AddLanguageUrl").val(),
                            type: 'Post',
                            data: addNewFrm.serialize(),
                            async: false,
                            success: function (data) {
                                if (data.success) {
                                    $('#LanguageID').append(
                                        $('<option></option>')
                                        .val(data.newData.LanguageID)
                                        .html(data.newData.Language1)
                                        .prop('selected', true)
                                    );
                                    $('#newLanguageDialog').dialog('close');
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
                    $('#LanguageID').val(0);
                    $(this).dialog('close');
                }
            }
        ]
    });
});

$('#addLanguageLink').click(function () {
    var createFormUrl = $(this).attr('href');
    $('#newLanguageDialog').html('')
        .load(createFormUrl, function () {
            jQuery.validator.unobtrusive.parse('#addNewFrm');
            $('#newLanguageDialog').dialog('open');
        });
    return false;
});

$('#LanguageID').change(function () {
    if (this.value === "-1") {
        var createFormUrl = $('#addLanguageLink').attr('href');
        $('#newLanguageDialog').html('')
            .load(createFormUrl, function () {
                jQuery.validator.unobtrusive.parse('#addNewFrm');
                $('#newLanguageDialog').dialog('open');
            });
    }
    return false;
});