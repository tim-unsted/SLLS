$(document).ready(function () {
    if ($('#HasData').val() !== 'True') {
        $(function () {
            $('#noDataDialog').dialog({
                dialogClass: "no-close",
                modal: true,
                buttons: [
                    {
                        text: 'Ok',
                        "class": 'btn btn-success',
                        click: function () {
                            $(this).dialog('close');
                            window.history.back();
                        }
                    }
                ]
            });
        });
    }
});