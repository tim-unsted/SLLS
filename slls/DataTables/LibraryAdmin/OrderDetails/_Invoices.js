$('#invoices').DataTable({
    "aaSorting": [[2, "desc"]],
    "dom": sDom,
    "pagingType": sPagingType,
    "pageLength": sPageLength,
    "lengthMenu": sLengthMenu,
    "language": {
        "url": "/DataTables/lang.txt"
    },
    "columnDefs": [
        { "width": "5%", "data": 0, "render": function (data, type, row) { return '<a href="Edit/' + row[12] + '#receipts" class="btn-link">' + data + '</a>'; }, "targets": [0] },
        { "width": "25%", "data": 1, "render": function (data, type, row) { return '<a href="EditTitle/' + row[13] + '" class="btn-link">' + data + '</a>'; }, "targets": [1] },
        { "width": "9%", "orderData": [9], "targets": [2] },
        { "width": "9%", "targets": [3] },
        { "width": "9%", "targets": [4] },
        { "width": "9%", "orderData": [10], "targets": [5] },
        { "width": "9%", "orderData": [11], "targets": [6] },
        { "width": "9%", "targets": [7] },
        { "width": "9%", "targets": [8] },
        { "visible": false, "targets": [9, 10, 11, 12, 13] },
        { "width": "7%", "targets": [14] }
    ],
    initComplete: function () {
        this.api().columns('.select-filter').every(function () {
            var column = this;
            var select = $('<select class="form-control" style="width:100%;"><option value=""></option></select>')
                .appendTo($(column.footer()).empty())
                .on('change', function () {
                    var val = $.fn.dataTable.util.escapeRegex(
                        $(this).val()
                    );

                    column
                        .search(val ? '^' + val + '$' : '', true, false)
                        .draw();
                });

            column.data().unique().sort().each(function (d, j) {
                select.append('<option value="' + d + '">' + d + '</option>');
            });
        });
    }
});