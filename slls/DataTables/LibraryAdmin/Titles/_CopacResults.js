$('#copacResults').DataTable({
    "dom": '<"inline"i>',
    "pageLength": 100,
    "language": {
        "url": "/DataTables/autocat.txt"
    },
    "order": [2, 'asc'],
    "columnDefs": [
        {
            "sortable": true,
            "orderSequence": ["desc", "asc"],
            "targets": [1]
        },
        {
            "searchable": false,
            "sortable": false,
            "targets": [0, 5]
        }
    ]
});
