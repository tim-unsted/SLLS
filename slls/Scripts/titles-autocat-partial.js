$(document).ready(function () {
    $('#Isbn').focus();
});

var typingTimer; //timer identifier
var doneTypingInterval = 500; //time in ms, 0.5 seconds for example
var input = $('#Isbn');
var submit = $('#sources');
var isbn = '';

//Show waiting/progress cursor ...
submit.click(function () {
    if (checkInput() === false) {
        return false;
    }
    $(document.body).css({ 'cursor': 'wait' });
});

function checkInput() {
    var isbn = $("#Isbn").val();
    isbn = cleanIsbn(isbn);
    if (isbn.length !== 8 && isbn.length !== 10 && isbn.length !== 13) {
        jQuery(input).closest('.form-group').removeClass('has-success').addClass('has-error');
        $("#feedback").removeClass('glyphicon-ok');
        $("#warning-msg").show();
        return false;
    }
    return true;
}

//on keyup, start the countdown
input.on('keyup', function () {
    clearTimeout(typingTimer);
    typingTimer = setTimeout(doneTyping, doneTypingInterval);
});

//on keydown, clear the countdown
input.on('keydown', function () {
    clearTimeout(typingTimer);
});

//on keydown, clear the countdown
input.on('paste', function () {
    clearTimeout(typingTimer);
    typingTimer = setTimeout(doneTyping, doneTypingInterval);
    //doArray();
});

//Prevent the Enter/Return key - this happens automatically when using a scanner.
$(document).keypress(function (e) {
    if (e.which == 10 || e.which == 13) {
        event.preventDefault();
        clearTimeout(typingTimer);
        doneTyping();
    }
});

//user is "finished typing," do something ...
function doneTyping() {
    $(".field-validation-error").hide();
    var isbn = $("#Isbn").val();
    isbn = cleanIsbn(isbn);

    if ((isbn.length === 10 && isbn.slice(0, 3) != '978' && isbn.slice(0, 3) != '979') || (isbn.length === 13) || (isbn.length === 8)) {
        if (checkIsbn(isbn)) {
            jQuery(input).closest('.form-group').removeClass('has-error').addClass('has-success');
            $("#feedback").addClass('glyphicon-ok');
            $("#warning-msg").hide();

        } else {
            jQuery(input).closest('.form-group').removeClass('has-success').addClass('has-error');
            $("#feedback").removeClass('glyphicon-ok');
            $("#warning-msg").show();
        }
    } else {
        //jQuery(input).closest('.form-group').removeClass('has-success').removeClass('has-error');
        //$("#feedback").removeClass('glyphicon-ok');
        //$("#warning-msg").hide();
        jQuery(input).closest('.form-group').removeClass('has-success').addClass('has-error');
        $("#feedback").removeClass('glyphicon-ok');
        $("#warning-msg").show();
    }
    //Do something here to check the db for a matching ISBN...
    checkIsbnUnique(isbn);
}

//Remove any spaces or hyphons from the ISBN ...
function cleanIsbn(isbn) {
    isbn = isbn.replace(/-/g, "");
    isbn = isbn.replace(/ /g, '');
    return isbn;
}

//Try to verify a true ISBN. This is ultimately done by calculating a check-sum and comparing that to the last digit
function checkIsbn(isbn) {
    if (isbn.length < 10) {
        return true;
    };
    //if (isbn.length === 10 && isbn.slice(0, 3) != '978' && isbn.slice(0, 3) != '979') {
    if (isbn.length === 10) {
        if (checkIsbn10(isbn)) {
            return true;
        } else {
            return false;
        }
    }
    if (isbn.length === 13) {
        if (checkIsbn13(isbn)) {
            return true;
        } else {
            return false;
        }
    }
    return true;
}

function checkIsbn10(isbn) {
    var message = isbn.slice(0, 9); // get the first 9 characters
    var checkDigit = isbn.slice(9, 10); // get the last character
    if (checkDigit == getIsbnCheckDigit(message)) {
        return true;
    } else {
        return false;
    }
}

function checkIsbn13(isbn) {
    var message = isbn.slice(0, 12); // get the first 12 characters
    var checkDigit = isbn.slice(12, 13); // get the last character
    if (checkDigit == getEANCheckDigit(message)) {
        return true;
    } else {
        return false;
    }
}

function getIsbnCheckDigit(message) {
    if (message.length < 9) {
        return false;
    }

    var digit;
    var addn = 0;
    for (var i = 0; i < 9; i++) {
        digit = message.slice(i, i + 1);
        addn = addn + digit * (i + 1);
    }

    var checkDigit = addn % 11;
    if (checkDigit === 10) {
        checkDigit = "X";
    }

    return checkDigit;
}

function getEANCheckDigit(message) {
    if (message.length !== 12) {
        return false;
    }

    var digit;
    var addn = 0;
    for (var i = 0; i < 12; i++) {
        digit = message.slice(i, i + 1);
        if ((i + 1) % 2 == 0) {
            addn = addn + digit * 3;
        } else {
            addn = addn + digit * 1;
        }
    }

    var checkDigit = 10 - (addn % 10);
    if (checkDigit == 10) {
        checkDigit = 0;
    }

    return checkDigit;
}

function checkIsbnUnique(isbn) {
    var isbnToTest = { isbn: isbn };
    $.ajax({
        type: "POST",
        url: $("#IsbnUnique").val(),
        contentType: 'application/json',
        data: JSON.stringify(isbnToTest),
        dataType: "json",
        success: function (data) {
            if (data.titleId > 0) {
                $('#titleId').val(data.titleId);
                jQuery(input).closest('.form-group').removeClass('has-success').addClass('has-error');
                $("#feedback").removeClass('glyphicon-ok');
                $("#warning-msg").text('A @Model.EntityName with this ISBN already exists!');
                $("#warning-msg").show();
                $('#addNewCopyDiv').show();
                $('#showTitleDiv').show();
            } else {
                $("#warning-msg").text('This ISBN does not appear to be valid!');
                $('#addNewCopyDiv').hide();
                $('#showTitleDiv').hide();
            }
        }
    });
}

$("#btnAddCopy").click(function () {
    var id = $('#titleId').val();
    this.href = this.href.replace("xxx", id);
});

$("#showTitleLink").click(function () {
    var id = $('#titleId').val();
    this.href = this.href.replace("xxx", id);
});