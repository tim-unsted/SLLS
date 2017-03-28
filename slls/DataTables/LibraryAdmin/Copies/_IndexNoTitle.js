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
            "width": "10%",
            "targets": 0
        },
        {
            "width": "20%",
            "targets": [1, 2]
        },
        {
            "width": "30%",
            "targets": [3]
        },
        {
            "width": "10%",
            "searchable": false,
            "sortable": false,
            "targets": [5]
        }
    ]
});