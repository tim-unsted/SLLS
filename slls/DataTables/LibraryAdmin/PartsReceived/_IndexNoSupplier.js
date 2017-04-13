$(document).ready(function () {
    // Array holding selected row IDs
    var rows_selected = [];

    $('#btnDeleteSelected').attr('disabled', 'disabled');

    var table = $('#partsreceived').DataTable({
        "dom": sDom,
        "pagingType": sPagingType,
        "pageLength": sPageLength,
        "lengthMenu": sLengthMenu,
        "language": {
            "url": "/DataTables/lang.txt"
        },
        "order": [[10, 'asc'], [2, 'asc'], [11, 'desc']],
        "columnDefs": [
            {
                "targets": [0],
                "searchable": false,
                "orderable": false,
                "width": "1%",
                "className": "dt-body-center",
                "render": function (data, type, full, meta) {
                    return '<input type="checkbox">';
                }
            },
            {
                "data": [1],
                "orderData": [10],
                "targets": [1]
            },
            {
                "searchable": false,
                "sortable": false,
                "visible": false,
                "targets": [7, 8, 9, 10, 11]
            },
            {
                "width": "10%",
                "searchable": false,
                "sortable": false,
                "targets": [12]
            },
            {
                "orderData": [11],
                "targets": [4]
            }
        ]

    });

    $('#confirmDialog').dialog({
        dialogClass: "no-close",
        autoOpen: false,
        width: 600,
        modal: true,
        resizable: false,
        buttons: [
            {
                text: 'Continue',
                "class": 'btn btn-success',
                click: function () {
                    $(this).dialog("close");
                    $.ajax({
                        url: $("#DestUrl").val(),
                        type: "POST",
                        dataType: "json",
                        data: JSON.stringify(rows_selected),
                        contentType: 'application/json; charset=utf-8',
                        success: function(data) {
                            if (data.success) {
                                location.reload();
                            }
                        }
                    });
                }
            },
            {
                text: 'Cancel',
                "class": 'btn btn-default',
                click: function () {
                    $(this).dialog("close");
                }
            }
        ]
    });

    // Handle click on "Select all" control
    $('thead input[name="select_all"]', table.table().container()).on('click', function (e) {
        if (this.checked) {
            $('#partsreceived tbody input[type="checkbox"]:not(:checked)').trigger('click');
            //alert('Show all');
            //$('#deleteSelected').show();
            $('#btnDeleteSelected').removeAttr('disabled');

        } else {
            $('#partsreceived tbody input[type="checkbox"]:checked').trigger('click');
            //alert('Hide all');
            //$('#deleteSelected').hide();
            $('#btnDeleteSelected').attr('disabled', 'disabled');
        }

        // Prevent click event from propagating to parent
        e.stopPropagation();
    });

    // Handle click on checkbox
    $('#partsreceived tbody').on('click', 'input[type="checkbox"]', function (e) {
        var $row = $(this).closest('tr');

        // Get row data
        var data = table.row($row).data();

        // Get row ID
        var rowId = data[7];

        // Determine whether row ID is in the list of selected row IDs 
        var index = $.inArray(rowId, rows_selected);

        // If checkbox is checked and row ID is not in list of selected row IDs
        if (this.checked && index === -1) {
            rows_selected.push(rowId);

            // Otherwise, if checkbox is not checked and row ID is in list of selected row IDs
        } else if (!this.checked && index !== -1) {
            rows_selected.splice(index, 1);
        }

        if (rows_selected.length > 0) {
            $('#btnDeleteSelected').removeAttr('disabled');
        } else {
            $('#btnDeleteSelected').attr('disabled', 'disabled');
        };

        // Prevent click event from propagating to parent
        e.stopPropagation();
    });

    $('#btnDeleteSelected').click(function () {
        var countSelected = rows_selected.length;
        if (countSelected === 0) {
            return false;
        }
        if (countSelected === 1)
        {
            $('#deleteMsg').html("You are about the delete the selected part.");
        } else {
            $('#deleteMsg').html("You are about the delete " + countSelected + " selected parts.");
        }
        
        $('#confirmDialog').dialog("open");
        return false;
    });

});