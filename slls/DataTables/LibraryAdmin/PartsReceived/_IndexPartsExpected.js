$('#partsoverdue').DataTable({
    "dom": sDom,
    "pagingType": sPagingType,
    "pageLength": sPageLength,
    "lengthMenu": sLengthMenu,
    "language": {
        "url": "/DataTables/lang.txt"
    },
    "order": [[9, 'asc'], [7, 'asc'], [1, 'asc']],
    "columnDefs": [
        {
            "data": [0],
            "orderData": [7],
            "targets": [0]
        },
        {
            "searchable": false,
            "sortable": false,
            "visible": false,
            "targets": [7, 8, 9]
        },
        {
            "searchable": false,
            "sortable": false,
            "targets": [10]
        },
        {
            "orderData": [8],
            "targets": [4]
        },
        {
            "orderData": [9],
            "orderSequence": ["desc", "asc"],
            "targets": [6]
        }
    ]
});