if ($.validator) {
    $.validator.addMethod("date",
        function (value, element, params) {
            if (this.optional(element)) {
                return true;
            }

            var ok = true;
            try {
                $.datepicker.parseDate(dateFormat, value);
            }
            catch (err) {
                ok = false;
            }
            return ok;
        });
}

$(function () {
    $(".datepicker").datepicker({
        dateFormat: dateFormat,
        defaultDate: new Date()
    });
});