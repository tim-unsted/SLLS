$('#copies').DataTable({
    "dom": sDom,
    "pagingType": sPagingType,
    "pageLength": sPageLength,
    "lengthMenu": sLengthMenu,
    "language": {
        "url": "/DataTables/lang.txt"
    },
    "columnDefs": [
        {
            "width": "30%",
            "orderData": 5,
            "targets": 0
        },
        {
            "width": "10%",
            "targets": [1]
        },
        {
            "width": "20%",
            "targets": [2, 3]
        },
        {
            "visible": false,
            "targets": [5]
        },
        {
            "width": "10%",
            "searchable": false,
            "sortable": false,
            "targets": [6]
        }
    ]
});