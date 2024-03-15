function ValidateList() {
    if ($('.destinatari').val() === "") {
        alert("Selezionare una lista.");
    }
    else {
        var ids = [];
        $.each($('.parent ul'), function (i, item) {
            if ($(".selectItem", item).is(":checked")) {
                ids.push($(".selectItem", item).val() + "|" + $(".labelfileName", item).html());
            }
        });
        if (ids.length === 0) {
            alert("Selezionare almeno un nominativo.");
        }
        else {
            $.post("/Raccomandata/CreateDestinatari", {
                listId: $('.destinatari').val(),
                ids: ids
            }, function (s) {
                    if (s === "") {
                        location.href = "/Home/Error500";
                    }
                    else
                        location.href = "Step3?ListId=" + s + "&Bollettini=" + $(".Bollettini").val() + "&Sender=" + $(".Sender").val() + "&SenderAR=" + $(".SenderAR").val() + "&TipoStampa=" + $(".TipoStampa").val() + "&FronteRetro=" + $(".FronteRetro").val() + "&RicevutaRitorno=" + $(".RicevutaRitorno").val() + "&Formato=" + $(".Formato").val();
            })
        };
    }
}

$(function () {
    $('.destinatari').on("change", function () {
        var l = $(this).val();
        if (l === "") {
            $('.nothing').show();
            var parent = $('.parent');
            parent.empty();
        }
        else {
            $.get("/Raccomandata/GetList?id=" + l, function (r) {
                var res = jQuery.parseJSON(r);
                $('.nothing').hide();
                var parent = $('.parent');
                parent.empty();
                $.each(res.recipients, function (i, item) {
                    var ul = "<ul>" +
                        "<li style='width:80px; padding:10px 0'><input type='checkbox' value='" + item.id + "' name='selectItem' class='selectItem' /></li>" +
                        "<li>" + item.businessName + "</li>" +
                        "<li>" + item.name + " " + item.surname + "</li>" +
                        "<li style='width:262px'>" + item.dug + " " + item.address + " " + item.houseNumber + "</li>" +
                        "<li style='width:140px'>" + item.cap + "</li>" +
                        "<li>" + item.city + "</li>" +
                        "<li style='width:175px'>" + item.province + "</li>" +
                        "<li class='fileName'><span class='labelfileName span-" + i + "'>" + item.fileName + "</span><input type='text' value='" + item.fileName + "' onmouseleave='showLabel(" + i + ")' class='input-" + i + "'></li>" +
                        "</ul>";
                    parent.append(ul);
                });
                showLabel();
                showTextBox();
            });
        }
    });
});

function showLabel(index) {
    $('.fileName span').show();
    $('.fileName input').hide();
    $('.span-' + index).html(($('.input-' + index).val()));
};

function showTextBox() {
    $('.labelfileName').click(function () {
        $(this).hide();
        var parentTag = $(this).parent().get(0);
        $(parentTag).find("input").show();
    })
}

function SelectAll() {
    if ($('.selectItemTop').is(":checked"))
        $('.selectItem').prop("checked", true);
    else
        $('.selectItem').prop("checked", false);
}
