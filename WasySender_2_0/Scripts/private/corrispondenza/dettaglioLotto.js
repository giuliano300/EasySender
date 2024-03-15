function GetPdf(id) {
    $.get("/Corrispondenza/GetPdf/" + id, function (result) {
        if (result !== "")
            window.open(result, "document");
        else
            alert("Documento non disponibile");
        return false;
    });
}

function GetPdfGED(id) {
    $.get("/Corrispondenza/GetPdfGED/" + id, function (result) {
        if (result !== "")
            window.open(result, "document");
        else
            alert("Documento non disponibile");
        return false;
    });
}

$(function() {
    //GetStatus();
    $('.download-doc-zip').on('click', function () {
        $(this).attr('target', '_blank');
        createZIP(operationId);
    });

});

//function GetStatus() {
//    $.getJSON("/Scripts/api/configuration.json", function (c) {
//        var ids = []; 
//        $('.status').each(function () {
//           ids.push($(this).attr("id"));
//        });
//        var url = c.apiurl;
//        if (areaTestUser === "True")
//            url = c.apiurlTest;

//        for (var x = 0; x < ids.length; x++) {
//            $.get(url + "/api/Operations/RequestOperationStatusSingle?guidUser=" + guiduser + "&operationId=" + operationId + "&requestId=" + ids[x],
//                function (r) {
//                    $('#' + r.requestId).html(r.statoDescrizione.replace("Postel", ""));
//            });
//        }
//    });
//}

function ApplyFilter() {
    var usrname = "";
    var code = "";
    var esito = "";
    var ar = "";
    if ($('.username').val() !== "") {
        usrname = "&username=" + $('.username').val();
    };
    if ($('.code').val() !== "") {
        code = "&code=" + $('.code').val();
    };
    if ($('.esito').val() !== "") {
        esito = "&esito=" + $('.esito').val();
    };

    if ($('.ricevutaRitorno').val() !== "") {
        ar = "&ricevutaRitorno=" + $('.ricevutaRitorno').val();
    };

    $.getJSON("/Scripts/api/configuration.json", function (c) {
        $.get(c.apiurl + "/Operations/Items/" + operationId + "?guidUser=" + guiduser + usrname + code + esito, function (r) {
            $('.names').empty();
            $.each(r.recipients, function (k, v) {
                var cod = v.bulletin != null ? " <br>cod: " + v.bulletin.codiceCliente : "";

                var ul = "<ul>" +
                    "<li>" + v.recipient.businessName + " " + v.recipient.name + " " + v.recipient.surname + " " + cod + "</li>" +
                    "<li>" + v.recipient.dug + " " + v.recipient.address + " " + v.recipient.houseNumber + ", " + v.recipient.cap + "</li>" +
                    "<li>" + v.recipient.city + " (" + v.recipient.province + ")</li>" +
                    "<li>" + (v.recipient.valid ? "Accettato" : "ERRORE") + "</li>" +
                    "<li>" + new Date(v.recipient.presaInCaricoDate).toLocaleDateString() + " " + new Date(v.recipient.presaInCaricoDate).toLocaleTimeString() + "</li>" +
                    "<li>" + v.recipient.codice + "</li>";
                ul += "<li id='" + v.recipient.requestId + "'>";

                if (type != 2)
                    ul += "<a href='https://www.poste.it/cerca/index.html#/risultati-spedizioni/" + v.recipient.codice + "' target='_blank' title='verifica lo stato sul sito di poste'>" + v.recipient.stato + "</a>";
                else
                    ul += v.recipient.stato;

                ul += "</li>";

                    if (downloadFile == "True") {
                        ul += "<li><a onclick='GetPdf(" + v.recipient.id + ")'><img src='/Images/IcnDownload.png' /></a></li>"
                    };
                    if (downloadFile != "True") {
                        ul += "<li>Non disponibile</li>"
                    };
                    ul += "</ul>";
                $('.names').append(ul);
            });
            //GetStatus();
        });
    });
}

function OpenPopup(requestId) {
    $.getJSON("/Scripts/api/configuration.json", function (c) {
        $.get(c.apiurl + "/Names/Item?guidUser=" + guiduser + "&requestId=" + requestId, function (r) {
            var data = "NON ANCORA CONSEGNATO";
            if (r.consegnatoDate !== null) { data = new Date(r.consegnatoDate).toLocaleString() };
            $('.cod').html(r.codice);
            $('.dest').html(r.businessName + " " + r.name + " " + r.surname);
            $('.dataCar').html(new Date(r.insertDate).toLocaleString());
            $('.dataPre').html(new Date(r.presaInCaricoDate).toLocaleString());
            $('.dataCo').html(data);
        });
    });
    $(".popup-std").fadeIn(300);
}

function createZIP(operationId) {
    $('.download-doc-zip').hide();
    $('.wait').show();

    $.get("/Corrispondenza/GetZIP/" + operationId, function (r) {
        var res = jQuery.parseJSON(r);
        if (res.success)
            document.location.href = "https://app.easysender.it/" + res.pathFile;
        else
            alert("Errore nella generazione del file ZIP.");

        $('.wait').hide();
        $('.download-doc-zip').show();
    });
}