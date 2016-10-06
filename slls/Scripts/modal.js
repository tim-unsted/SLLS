function bindForm(dialog) {
    $('form', dialog).submit(function() {
        $.ajax({
            url: this.action,
            type: this.method,
            //traditional: true,
            data: $(this).serialize(),
            success: function(result) {
                if (result.success) {
                    showValidationErrors(false);
                    $('#simpleModal').modal('hide');
                    alert('Validation was successful.');
                } else {
                    fillErrorList(result);
                    showValidationErrors(true);
                }
            }
        });
        return false;
    });
};

$(function () {
    $.ajaxSetup({ cache: false });
    // Initialize modal dialog
    // attach stdModal bootstrap attributes to links with .modal-link class.
    // when a link is clicked with these attributes, bootstrap will display the href content in a modal dialog.
    $('body').on('click', '.modal-link', function (e) {
        e.preventDefault();
        $("#stdModal").removeData('modal');
        $(this).attr('data-target', '#stdModal');
        $(this).attr('data-toggle', 'modal');
    });
    // Attach listener to .modal-close-btn's so that when the button is pressed the modal dialog disappears
    $('body').on('click', '.modal-close-btn', function () {
        $('#stdModal').modal('hide');
    });
    //allow the modal to be dragged on-screen
    $("#stdModal").draggable({
        handle: ".modal-header"
    });
    ////clear modal cache, so that new content can be loaded
    $('body').on('hidden.bs.modal', '.modal', function () {
        $(this).removeData('bs.modal');
    });
});