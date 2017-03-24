$('#titles').DataTable({
    "dom": sDom,
    "pagingType": sPagingType,
    "pageLength": sPageLength,
    "lengthMenu": sLengthMenu,
    "language": {
        "url": "/DataTables/lang.txt"
    },
    "order": [9, 'asc'],
    "autoWidth": false,
    "columnDefs": [
        {
            "width": "10%",
            "sortable": false,
            "searchable": false,
            "targets": [0]
        },
        {
            "width": "20%",
            "orderData": 9,
            "targets": [1],
            "orderSequence": ["desc", "asc"]
        },
        {
            "width": "10%",
            "targets": [2, 4]
        },
        {
            "width": "15%",
            "targets": [3, 5, 6]
        },
        {
            "width": "10%",
            "targets": [7]
        },
        {
            "visible": false,
            "targets": [8, 9]
        },
        {
            "sortable": false,
            "width": "10%",
            "targets": [10]
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