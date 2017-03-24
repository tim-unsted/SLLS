function selectAllText(textbox) {
    textbox.focus();
    textbox.select();
}

$('#SelectTitle').click(function () { selectAllText(jQuery(this)) });

var columns = [{ name: '', minWidth: '0%', valueField: 'Id' }, { name: 'Title', minWidth: '40%', valueField: 'title' }, { name: 'Edition', minWidth: '20%', valueField: 'edition' }, { name: 'Year', minWidth: '10%', valueField: 'year' }, { name: 'Author', minWidth: '20%', valueField: 'authors' }];

$("#SelectTitle").mcautocomplete({
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
                    title[i] = { Id: data[i].TitleId, title: data[i].Title, edition: data[i].Edition, year: data[i].Year, authors: data[i].Authors };
                }
            }
        });
        response(title);
    },
    select: function(event, ui) {
        event.preventDefault();
        if (ui.item) {
            if (ui.item.Id != null) {
                $('#SelectTitle').val(ui.item.title);
                var url = $("#DestUrl").val();
                window.location.href = url.replace('REPLACEME', ui.item.Id);
            }
        }
    }
});

$("#TitleID").change(function() {
    sessionStorage.removeItem('lastTab_EditTitle');
});