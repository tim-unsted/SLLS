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
            "orderData": 6,
            "orderSequence": ["desc", "asc"],
            "targets": [0],
            "render": function (data, type, row) {
                return '<a href="EditTitle/' + row[5] + '" class="btn-link">' + data + '</a>';
            }
        },
        {
            "width": "10%",
            "targets": [1, 2]
        },
        {
            "width": "15%",
            "targets": [3, 4]
        },
        //{
        //    "width": "10%",
        //    "targets": [5]
        //},
        {
            "visible": false,
            "targets": [5, 6]
        },
        {
            "sortable": false,
            "width": "10%",
            "targets": [7]
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