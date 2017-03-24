$(function () {
    $('#newPublisherDialog').dialog({
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
                    var addNewFrm = $('#addPublisherForm');
                    if (addNewFrm.valid()) {
                        $.ajax({
                            url: $("#AddPublisherUrl").val(),
                            type: 'Post',
                            data: addNewFrm.serialize(),
                            async: false,
                            success: function (data) {
                                if (data.success) {
                                    $('#PublisherID').append(
                                        $('<option></option>')
                                        .val(data.newData.PublisherID)
                                        .html(data.newData.PublisherName)
                                        .prop('selected', true)
                                    );
                                    $('#newPublisherDialog').dialog('close');
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
                    $('#PublisherID').val(0);
                    $(this).dialog('close');
                }
            }
        ]
    });
});

$('#addPublisherLink').click(function () {
    var createFormUrl = $(this).attr('href');
    $('#newPublisherDialog').html('')
        .load(createFormUrl, function () {
            jQuery.validator.unobtrusive.parse('#addNewFrm');
            $('#newPublisherDialog').dialog('open');
        });
    return false;
});

$('#PublisherID').change(function () {
    if (this.value === "-1") {
        var createFormUrl = $('#addPublisherLink').attr('href');
        $('#newPublisherDialog').html('')
            .load(createFormUrl, function () {
                jQuery.validator.unobtrusive.parse('#addNewFrm');
                $('#newPublisherDialog').dialog('open');
            });
    }
    return false;
});