$('#orderdetails').DataTable({
    "dom": '<"inline"i><"clear"><"inline"l><"inline float-right"p>rt<"inline"i><"inline float-right"p><"clear"><"inline"l><"clear">',
    "pagingType": sPagingType,
    "pageLength": sPageLength,
    "lengthMenu": sLengthMenu,
    "language": {
        "url": "/DataTables/lang.txt"
    },
    "order": [[0, "desc"]],
    "columnDefs": [
        {
            "orderData": [8],
            "targets": [1]
        },
        {
            "orderData": [9],
            "targets": [6]
        },
        {
            "searchable": false,
            "sortable": false,
            "visible": false,
            "targets": [8, 9]
        },
        {
            "width": "10%",
            "searchable": false,
            "sortable": false,
            "targets": [10]
        }
    ]
});