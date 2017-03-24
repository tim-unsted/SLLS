$('#itemsonloan').DataTable({
    "dom": sDom,
    "pagingType": sPagingType,
    "pageLength": sPageLength,
    "lengthMenu": sLengthMenu,
    "language": {
        "url": "/DataTables/lang.txt"
    },
    "columnDefs":
    [
        {
            "visible": false,
            "targets": [7, 8, 9]
        },
        {
            "orderData": [7],
            "targets": [5]
        },
        {
            "orderData": [8],
            "targets": [6]
        },
        {
            "orderData": [9],
            "targets": [1]
        },
        {
            "width": "15%",
            "searchable": false,
            "sortable": false,
            "targets": [10]
        }
    ]
});