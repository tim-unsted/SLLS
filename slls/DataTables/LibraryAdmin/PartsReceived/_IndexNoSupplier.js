$('#partsreceived').DataTable({
    "dom": sDom,
    "pagingType": sPagingType,
    "pageLength": sPageLength,
    "lengthMenu": sLengthMenu,
    "language": {
        "url": "/DataTables/lang.txt"
    },
    "order": [[9, 'asc'], [1, 'asc'], [10, 'desc']],
    "columnDefs": [
        {
            "data": [0],
            "orderData": [9],
            "targets": [0]
        },
        {
            "searchable": false,
            "sortable": false,
            "visible": false,
            "targets": [6, 7, 8, 9, 10]
        },
        {
            "width": "10%",
            "searchable": false,
            "sortable": false,
            "targets": [11]
        },
        {
            "orderData": [10],
            "targets": [3]
        }
    ]
});