function selectAllText(textbox) {
    textbox.focus();
    textbox.select();
}

var columns = [{ name: '', minWidth: '0%', valueField: 'CopyId' }, { name: 'Title', minWidth: '40%', valueField: 'title' }, { name: 'Copy', minWidth: '5%', valueField: 'copyNumber' }, { name: 'Edition', minWidth: '20%', valueField: 'edition' }, { name: 'Year', minWidth: '10%', valueField: 'year' }, { name: 'Author', minWidth: '15%', valueField: 'authors' }];

$('#SelectCopy').click(function() { selectAllText(jQuery(this)) });

$("#SelectCopy").mcautocomplete({
    autoFocus: true,
    showHeader: true,
    columns: columns,
    source: function(request, response) {
        var title = new Array();
        var urlGetTerms = $("#AutoSuggestUrl").val();
        $.ajax({
            async: false,
            cache: false,
            type: "POST",
            url: urlGetTerms,
            data: { "term": request.term },
            success: function(data) {
                for (var i = 0; i < data.length; i++) {
                    title[i] = { CopyId: data[i].CopyId, title: data[i].Title, copyNumber: data[i].CopyNumber, edition: data[i].Edition, year: data[i].Year, authors: data[i].AuthorString };
                }
            }
        });
        response(title);
    },
    select: function(event, ui) {
        event.preventDefault();
        if (ui.item) {
            if (ui.item.CopyId != null) {
                $('#SelectCopy').val(ui.item.title + " - Copy: " + ui.item.copyNumber);
                var url = $("#DestUrl").val();
                window.location.href = url.replace('REPLACEME', ui.item.CopyId);
            }
        }
    }
});