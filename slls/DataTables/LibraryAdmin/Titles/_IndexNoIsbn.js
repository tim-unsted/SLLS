$('#titles').DataTable({
    "dom": sDom,
    "pagingType": sPagingType,
    "pageLength": sPageLength,
    "lengthMenu": sLengthMenu,
    "language": {
        "url": "/DataTables/lang.txt"
    },
    "order": [0, 'asc'],
    "autoWidth": false,
    "columnDefs": [
        {
            "width": "25%",
            "orderData": 7,
            "targets": [0],
            "orderSequence": ["desc", "asc"]
            //,
            //"render": function(data, type, row) {
            //    return '<a href="Edit/' + row[6] + '" class="btn-link">' + data + '</a>';
            //}
        },
        {
            "width": "10%",
            "targets": [2]
        },
        {
            "width": "15%",
            "targets": [1, 3, 4]
        },
        {
            "width": "10%",
            "targets": [5]
        },
        {
            "visible": false,
            "targets": [6, 7]
        },
        {
            "sortable": false,
            "width": "10%",
            "targets": [8]
        }
    ],
    initComplete: function () {
        var api = this.api();
        this.api().columns('.select-filter').every(function () {
            var column = this;
            var select = $('<select class="form-control" style="width:100%;"><option value=""></option></select>')
                .appendTo($(column.footer()))
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