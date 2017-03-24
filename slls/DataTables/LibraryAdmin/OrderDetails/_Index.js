$('#orderdetails').DataTable({
    //"orderCellsTop": true,
    "dom": sDom,
    "pagingType": sPagingType,
    "pageLength": sPageLength,
    "lengthMenu": sLengthMenu,
    "language": {
        "url": "/DataTables/lang.txt"
    },
    "deferRender": true,
    "order": [[12, "desc"]],
    "columnDefs": [
        {
            "width": "5%",
            "targets": [0],
            "data": 0
        },
        {
            "width": "10%",
            "orderData": [8],
            "targets": [1]
        },
        {
            "width": "20%",
            "targets": [2],
            "data": 2
        },
        {
            "width": "15%",
            "targets": [3]
        },
        {
            "width": "10%",
            "targets": [4, 5, 7]
        },
        {
            "width": "10%",
            "orderData": [10],
            "targets": [6]
        },
        {
            "visible": false,
            "targets": [8, 9, 10, 11, 12]
        },
        {
            "width": "10%",
            "orderable": false,
            "targets": [13]
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