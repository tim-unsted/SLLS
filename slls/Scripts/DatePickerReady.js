//http://bootstrap-datepicker.readthedocs.org/en/latest/options.html

$(function () {
    $.validator.addMethod("date",
    function (value, element) {
        if (this.optional(element)) {
            return true;
        }
        var ok = true;
        try {
            //$.datepicker.parseDate("dd/mm/yy", value);
            $.datepicker.parseDate(dateFormat, value);
        }
        catch (err) {
            ok = false;
        }
        return ok;
    });
    $(".datepicker").datepicker(
    {
        todayBtn: "linked",
        todayHighlight: true,
        weekStart: 0,
        dateFormat: dateFormat,
        //changeYear: true,
        //changeMonth: true,
        autoclose: true
    });
});