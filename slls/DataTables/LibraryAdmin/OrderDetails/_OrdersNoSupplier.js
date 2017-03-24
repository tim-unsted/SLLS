$('#orderdetails').DataTable({
    "dom": sDom,
    "pagingType": sPagingType,
    "pageLength": sPageLength,
    "lengthMenu": sLengthMenu,
    "language": {
        "url": "/DataTables/lang.txt"
    },
    "order": [[3, 'desc'], [0, 'asc']],
    "columnDefs": [
        { "width": "5%", "orderable": true, "targets": [0] },
        { "width": "25%", "orderable": true, "targets": [1] },
        { "width": "10%", "orderable": true, "targets": [2] },
        { "width": "10%", "orderable": true, "targets": [3] },
        { "width": "10%", "orderable": true, "orderData": [8], "targets": [4] },
        { "width": "10%", "orderable": true, "orderData": [9], "targets": [5] },
        { "width": "10%", "orderable": true, "orderData": [10], "targets": [6] },
        { "width": "10%", "orderable": true, "targets": [7] },
        { "visible": false, "targets": [8, 9, 10] },
        { "width": "10%", "visible": true, "orderable": false, "targets": [11] }
    ]
});