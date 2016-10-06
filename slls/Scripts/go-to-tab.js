$(function () {
    var pageName = document.location.pathname.match(/[^\/]+$/)[0];
    $('a[data-toggle="tab"]').on('click', function (e) {
        //save the latest tab ...
        sessionStorage.setItem('lastTab_' + pageName, $(e.target).attr('href'));
    });

    //go to the latest tab, if it exists:
    var lastTab = sessionStorage.getItem('lastTab_' + pageName);
    if (lastTab) {
        $('a[href="' + lastTab + '"]').click();
    }
});