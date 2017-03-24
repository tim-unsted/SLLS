$('#addClassmarkLink').click(function () {
    var createFormUrl = $(this).attr('href');
    $('#newClassmarkDialog').html('')
        .load(createFormUrl, function () {
            jQuery.validator.unobtrusive.parse('#addNewFrm');
            $('#newClassmarkDialog').dialog('open');
        });
    return false;
});

$('#ClassmarkID').change(function () {
    if (this.value === "-1") {
        var createFormUrl = $('#addClassmarkLink').attr('href');
        $('#newClassmarkDialog').html('')
            .load(createFormUrl, function () {
                jQuery.validator.unobtrusive.parse('#addNewFrm');
                $('#newClassmarkDialog').dialog('open');
            });
    }
    return false;
});

$(function () {
    $("#newClassmarkDialog").dialog({
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
                    var addNewFrm = $('#addClassmarkForm');
                    if (addNewFrm.valid()) {
                        $.ajax({
                            url: $("#AddClassmarkUrl").val(),
                            type: 'Post',
                            data: addNewFrm.serialize(),
                            async: false,
                            success: function (data) {
                                if (data.success) {
                                    $('#ClassmarkID').append(
                                        $('<option></option>')
                                        .val(data.newData.ClassmarkID)
                                        .html(data.newData.Classmark1)
                                        .prop('selected', true)
                                    );
                                    $('#newClassmarkDialog').dialog('close');
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
                    $('#ClassmarkID').val(0);
                    $(this).dialog('close');
                }
            }
        ]
    });
});

