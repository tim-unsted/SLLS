$(function () {
    $("a.delete-link").click(function () {
        var deleteLink = $(this);
        deleteLink.hide();
        var confirmButton = deleteLink.siblings(".delete-confirm");
        confirmButton.show();

        var cancelDelete = function () {
            removeEvents();
            showDeleteLink();
        };

        var deleteItem = function () {
            removeEvents();
            confirmButton.hide();
            $.post(
                '@Url.Action("Delete")',
                AddAntiForgeryToken({ id: confirmButton.attr('data-delete-id') }))
               .done(function () {
                   var parentRow = deleteLink.parents("tr:first");
                   parentRow.fadeOut('fast', function () {
                       parentRow.remove();
                   });
               }).fail(function (data) {
                   alert("error");
               });
            return false;
        };

        var removeEvents = function () {
            confirmButton.off("click", deleteItem);
            $(document).on("click", cancelDelete);
            $(document).off("keypress", onKeyPress);
        };

        var showDeleteLink = function () {
            confirmButton.hide();
            deleteLink.show();
        };

        var onKeyPress = function (e) {
            //Cancel if escape key pressed
            if (e.which == 27) {
                cancelDelete();
            }
        };

        confirmButton.on("click", deleteItem);
        $(document).on("click", cancelDelete);
        $(document).on("keypress", onKeyPress);

        return false;
    });

    AddAntiForgeryToken = function (data) {
        data.__RequestVerificationToken = $('input[name=__RequestVerificationToken]').val();
        return data;
    };
});
