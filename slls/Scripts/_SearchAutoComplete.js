if (autoSuggestEnabled) {
    $("#MainSearchString").autocomplete({
        source: function (request, response) {
            var terms = new Array();
            var urlGetTerms = $('#AutoCompleteUrl').val();
            $.ajax({
                async: false,
                cache: false,
                type: "POST",
                url: urlGetTerms,
                data: { "term": request.term },
                success: function (data) {
                    for (var i = 0; i < data.length; i++) {
                        terms[i] = { label: data[i].Value, Id: data[i].Key };
                    }
                }
            });
            response(terms);
        },
        select: function (event, ui) {
            event.preventDefault();
            if (ui.item) {
                if (ui.item.Id != null) {
                    $('#MainSearchString').val(ui.item.label);
                    $('form#searchForm').submit();
                }
            }
        }
    });
}