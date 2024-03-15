function ApplyFilter() {
    $('.preload-operation').show();
    var usrname = "";
    var type = "";
    var code = "";
    var esito = "";
    var dataDa = "";
    var dataA = "";
    var pp = "";

    if ($('.username').val() !== "") {
        usrname = "&username=" + $('.username').val();
    };

    if ($('.code').val() !== "") {
        code = "&code=" + $('.code').val();
    };

    if ($('.esito').val() !== "") {
        esito = "&esito=" + $('.esito').val();
    };

    if ($('.dataDa').val() !== "") {
        dataDa = "&dataDa=" + $('.dataDa').val();
    };

    if ($('.dataA').val() !== "") {
        dataA = "&dataA=" + $('.dataA').val();
    };
    if ($('.pp').val() !== "") {
        pp = "&pp=" + $('.pp').val();
    };

    $('.export').attr("disabled", "disabled");

    $.getJSON("/Scripts/api/configuration.json", function (c) {
        $.get(c.apiurl + "/Operations/GetAllOperations?guidUser=" + guiduser + "&prodotto=6&userId=" + userId + usrname + code + esito + dataDa + dataA + type + pp, function (r) {
            $('.names').empty();
            $('.preload-operation').hide();
            var i = 0;
            if (r.length > 0) {
                $('.export').removeAttr("disabled");
                var ids = "";
                $.each(r, function (k, o) {
                    var type = o.operationType;
                    var mittente = o.sender.name + " " + o.sender.surname
                    i = i + parseInt(o.recipients.length);
                    $.each(o.recipients, function (k, v) {
                        var ul = "<ul>" +
                            "<li style='max-width: 120px'>" + (v.tipoDocumento===0 ? "CERTIFICATO" : "VISURA") + "</li>" +
                            "<li style='min-width: 220px'>" +  mittente + "</li>" +
                            "<li style='min-width: 220px'>" + v.businessName + "</li>" +
                            "<li style='width: 150px'>" + v.fiscalCode + "</li>" +
                            "<li style='width: 150px'>" + v.NREA + "</li>" +
                            "<li style='width: 100px'>" + (v.valid ? "OK" : "ERRORE") + "</li>" +
                            "<li style='max-width: 150px'>" + new Date(v.presaInCaricoDate).toLocaleDateString() + " " + new Date(v.presaInCaricoDate).toLocaleTimeString() + "</li>" +
                            "<li style='max-width: 100px'>€ " + v.totalPrice + "</li>" +
                            "<li style='max-width: 132px'>" + (v.codice === null ? "" : v.codice) + "</li>" +
                            "<li class='status' id='" + v.requestId + "' style='max-width: 130px'>" + v.stato + "</li>" +
                            "<li style='max-width:90px'>" + (v.tipoDocumento != 0 ? "<a onclick='GetPdf(" + v.id + ")'><img src='/Images/IcnDownload.png' /></a>" : "Non Dis.") + "</li></ul>";
                        $('.names').append(ul);
                    });
                });
                //LoadNamesState(ids);
            }
            else {
                var ul = "<ul><li class='empty-li'>Nessun nominativo trovato</li></ul>";
                $('.names').append(ul);
            };
            $('.numberOfResult').html(i + " risultati trovati");
        });
    });
    }

    function ExportCsv() {

        $.redirect("/Visure/DownloadCsv",
            {
                username: $('.username').val(),
                code: $('.code').val(),
                esito: $('.esito').val(),
                dataDa: $('.dataDa').val(),
                dataA: $('.dataA').val(),
                pp: $('.pp').val()
            },
            "POST", "_blank");
    }

function GetPdf(id) {
    $.get("/Visure/GetPdf/" + id, function (result) {
        if (result !== "")
            window.open(result, "document");
        else
            alert("Documento non disponibile");
        return false;
    });
}
