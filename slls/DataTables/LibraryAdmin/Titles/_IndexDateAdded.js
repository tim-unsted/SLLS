$('#titles').DataTable({
    "dom": sDom,
    "pagingType": sPagingType,
    "pageLength": sPageLength,
    "lengthMenu": sLengthMenu,
    "language": {
        "url": "/DataTables/lang.txt"
    },
    "order": [10, 'desc'],
    "autoWidth": false,
    "columnDefs": [
        {
            "width": "25%",
            "orderData": 9,
            "targets": [0]
            //,
            //"render": function(data, type, row) {
            //    return '<a href="Edit/' + row[8] + '" class="btn-link">' + data + '</a>';
            //}
        },
        {
            "width": "10%",
            "targets": [1, 3]
        },
        {
            "width": "15%",
            "targets": [2, 4]
        },
        {
            "width": "10%",
            "orderData": [10],
            "targets": [5]
        },
        {
            "width": "5%",
            "targets": [6]
        },
        {
            "visible": false,
            "targets": [8, 9, 10]
        },
        {
            "sortable": false,
            "width": "10%",
            "targets": [11]
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