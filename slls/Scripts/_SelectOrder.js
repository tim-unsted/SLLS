﻿function selectAllText(textbox) {
    textbox.focus();
    textbox.select();
}

$('#SelectOrder').click(function () { selectAllText(jQuery(this)) });

var columns = [{ name: '', minWidth: '0%', valueField: 'Id' }, { name: 'Title', minWidth: '30%', valueField: 'title' }, { name: 'Edition', minWidth: '10%', valueField: 'edition' }, { name: 'Year', minWidth: '10%', valueField: 'year' }, { name: 'Supplier', minWidth: '20%', valueField: 'suppliername' }, { name: 'Order No.', minWidth: '10%', valueField: 'orderno' }, { name: 'Ordered', minWidth: '10%', valueField: 'orderdate' }];

$("#SelectOrder").mcautocomplete({
    showHeader: true,
    columns: columns,
    source: function (request, response) {
        var title = new Array();
        var urlGetTerms = $("#AutoSuggestUrl").val();
        $.ajax({
            async: false,
            cache: false,
            type: "POST",
            url: urlGetTerms,
            data: { "term": request.term },
            success: function (data) {
                for (var i = 0; i < data.length; i++) {
                    title[i] = { OrderId: data[i].OrderId, title: data[i].Title, edition: data[i].Edition, year: data[i].Year, suppliername: data[i].SupplierName, orderno: data[i].OrderNo, orderdate: data[i].OrderDate };
                }
            }
        });
        response(title);
    },
    select: function (event, ui) {
        event.preventDefault();
        if (ui.item) {
            if (ui.item.OrderId != null) {
                $('#SelectOrder').val(ui.item.title);
                $('#OrderID').val(ui.item.OrderId);
                var url = $("#DestUrl").val();
                window.location.href = url.replace('REPLACEME', ui.item.OrderId);
            }
        }
        return true;
    }
});