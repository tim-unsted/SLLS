﻿$(document).ready(function() {
    var results = $('#titles').DataTable({
        "dom": sDom,
        "pagingType": sPagingType,
        "pageLength": parseInt($('#SearchResultSize').val()),
        "lengthMenu": sLengthMenu,
        "language": {
            "lengthMenu": "Show _MENU_ results per page",
            "info": "Displaying _START_ to _END_ of _TOTAL_ results",
            "infoEmpty": "",
            "emptyTable": "No results were found!",
            "search": "Search within results",
            "paginate": {
                "previous": "Previous",
                "next": "Next"
            }
        },
        "deferRender": true,
        //"order": [5, 'desc'],
        "columnDefs": [
            {
                "name": "results",
                "sortable": false,
                "targets": [0]
            },
            {
                "name": "title",
                "visible": false,
                "sortable": false,
                "targets": [1]
            },
            {
                "name": "titleId",
                "visible": false,
                "sortable": false,
                "targets": [2]
            },
            {
                "name": "classmark",
                "visible": false,
                "sortable": false,
                "targets": [3]
            },
            {
                "name": "author",
                "visible": false,
                "sortable": false,
                "targets": [4]
            },
            {
                "name": "pubyear",
                "visible": false,
                "sortable": false,
                "targets": [5]
            },
            {
                "name": "commenced",
                "visible": false,
                "sortable": false,
                "targets": [6]
            }
        ]
    });

    //Set the initial sort order ...
    var selectedOrder = $('#OrderBy').val();
    var arrOrderBy = selectedOrder.split('.');
    var colIndex = results.column(arrOrderBy[0] + ':name').index();
    var direction = arrOrderBy[1];
    $('#titles').DataTable().order([colIndex, direction]).draw();

    //Change the sort order in the client ...
    $('#OrderResults').change(function() {
        selectedOrder = $("#OrderResults").val();
        arrOrderBy = selectedOrder.split('.');
        colIndex = results.column(arrOrderBy[0] + ':name').index();
        direction = arrOrderBy[1];
        $('#titles').DataTable().order([colIndex, direction]).draw();
    });

});