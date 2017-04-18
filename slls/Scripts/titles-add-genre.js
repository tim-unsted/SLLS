$('#addGenreLink').click(function () {
    var createFormUrl = $(this).attr('href');
    $('#newGenreDialog').html('')
        .load(createFormUrl, function () {
            jQuery.validator.unobtrusive.parse('#addNewFrm');
            $('#newGenreDialog').dialog('open');
        });
    return false;
});

$('#GenreID').change(function () {
    if (this.value === "-1") {
        var createFormUrl = $('#addGenreLink').attr('href');
        $('#newGenreDialog').html('')
            .load(createFormUrl, function () {
                jQuery.validator.unobtrusive.parse('#addNewFrm');
                $('#newGenreDialog').dialog('open');
            });
    }
    return false;
});

$(function () {
    $("#newGenreDialog").dialog({
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
                    var addNewFrm = $('#addGenreForm');
                    if (addNewFrm.valid()) {
                        $.ajax({
                            url: $("#AddGenreUrl").val(),
                            type: 'Post',
                            data: addNewFrm.serialize(),
                            async: false,
                            success: function (data) {
                                if (data.success) {
                                    $('#GenreID').append(
                                        $('<option></option>')
                                        .val(data.newData.GenreID)
                                        .html(data.newData.Genre1)
                                        .prop('selected', true)
                                    );
                                    $('#newGenreDialog').dialog('close');
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
                    $('#GenreID').val(0);
                    $(this).dialog('close');
                }
            }
        ]
    });
});

