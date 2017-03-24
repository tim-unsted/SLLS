$(function () {
    $('#newFrequencyDialog').dialog({
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
                    var addNewFrm = $('#addFrequencyForm');
                    if (addNewFrm.valid()) {
                        $.ajax({
                            url: $("#AddFrequencyUrl").val(),
                            type: 'Post',
                            data: addNewFrm.serialize(),
                            async: false,
                            success: function (data) {
                                if (data.success) {
                                    $('#FrequencyID').append(
                                        $('<option></option>')
                                        .val(data.newData.FrequencyID)
                                        .html(data.newData.Frequency1)
                                        .prop('selected', true)
                                    );
                                    $('#newFrequencyDialog').dialog('close');
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
                    $('#FrequencyID').val(0);
                    $(this).dialog('close');
                }
            }
        ]
    });
});

$('#addFrequencyLink').click(function () {
    var createFormUrl = $(this).attr('href');
    $('#newFrequencyDialog').html('')
        .load(createFormUrl, function () {
            jQuery.validator.unobtrusive.parse('#addNewFrm');
            $('#newFrequencyDialog').dialog('open');
        });
    return false;
});

$('#FrequencyID').change(function () {
    if (this.value === "-1") {
        var createFormUrl = $('#addFrequencyLink').attr('href');
        $('#newFrequencyDialog').html('')
            .load(createFormUrl, function () {
                jQuery.validator.unobtrusive.parse('#addNewFrm');
                $('#newFrequencyDialog').dialog('open');
            });
    }
    return false;
});