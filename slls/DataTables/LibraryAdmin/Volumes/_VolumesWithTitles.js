$('#volumes').DataTable({
    "dom": sDom,
    "pagingType": sPagingType,
    "pageLength": sPageLength,
    "lengthMenu": sLengthMenu,
    "language": {
        "url": "/DataTables/lang.txt"
    },
    "order": [[7, 'asc'], [11, 'asc'], [0, 'asc']],
    "autoWidth": false,
    "columnDefs": [
        {
            "render": function (data, type, row) {
                return '<a href="Volumes/EditTitle/' + row[8] + '" class="btn-link" title="View/Edit this item">' + data + '</a>';
            },
            "orderData": [7, 11, 0],
            "orderSequence": ["desc", "asc"],
            "targets": [2]
        },
        {
            "render": function (data, type, row) {
                return '<a href="Volumes/EditCopy/' + row[9] + '" class="btn-link" title="View/Edit this item">' + data + '</a>';
            },
            "orderData": [11],
            "targets": [3]
        },
        {
            "visible": false,
            "targets": [7, 8, 9, 10, 11]
        },
        {
            "sortable": false,
            "targets": [12]
        }
    ]
});