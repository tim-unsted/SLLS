$('#orderdetails').DataTable({
    "dom": sDom,
    "pagingType": sPagingType,
    "pageLength": sPageLength,
    "lengthMenu": sLengthMenu,
    "language": {
        "url": "/DataTables/lang.txt"
    },
    "order": [[2, 'desc'], [0, 'asc']],
    "columnDefs": [
        {
            "width": "5%",
            "targets": [0],
            "data": 0,
            "render": function (data, type, row) {
                return '<a href="Edit/' + row[11] + '" class="btn-link">' + data + '</a>';
            }
        },
        {
            "width": "25%",
            "targets": [1],
            "data": 1,
            "render": function (data, type, row) {
                return '<a href="EditTitle/' + row[10] + '" class="btn-link">' + data + '</a>';
            }
        },
        {
            "width": "15%",
            "targets": [2]
        },
        {
            "width": "10%",
            "orderData": [9],
            "targets": [3]
        },
        {
            "width": "10%",
            "targets": [4, 5]
        },
        {
            "width": "10%",
            "targets": [6]
        },
        {
            "visible": false,
            "targets": [7, 8, 9, 10, 11]
        },
        {
            "width": "10%",
            "orderable": false,
            "targets": [12]
        }
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