$('#partsreceived').DataTable({
    "dom": 'rt<"inline"i><"inline float-right"p><"clear">',
    "pageLength": 10,
    "order": [[3, 'desc'], [0, 'asc']],
    "columnDefs": [
        {
            "searchable": false,
            "sortable": false,
            "visible": false,
            "targets": [3]
        },
        {
            "searchable": false,
            "sortable": false,
            "targets": [2, 4]
        },
        {
            "orderData": [3],
            "targets": [1]
        }
    ]
});