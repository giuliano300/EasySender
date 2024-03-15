function getProgressBar() {
    $.getJSON("/Scripts/api/configuration.json", function (c) {
        $('.percentageText').each(function () {
            var id = $(this).attr("id").split("-")[1];
            $.get(c.apiurl + "/Names/GetCompleteCount?guidUser=" + guiduser + "&operationId=" + id, function (r) {
                $('#t-' + id).html(r.percentage + "%");
                $('.b-' + id).animate({ "width": r.percentage + "%" }, 300);
            });
        });
    });
}
$(function () {
    getProgressBar()
    setTimeout(function () {
        getProgressBar()
    }, 10000);
})
