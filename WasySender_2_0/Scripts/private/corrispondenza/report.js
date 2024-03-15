function ApplyFilter() {
    $('.preload-operation').show();
    $('.bulletin-show').hide();
    var usrname = "";
    var type = "";
    var code = "";
    var esito = "";
    var dataDa = "";
    var dataA = "";
    var mittente = "";
    var ar = "";

    if ($('.username').val() !== "") {
        usrname = "&username=" + $('.username').val();
    };

    if ($('.code').val() !== "") {
        code = "&code=" + $('.code').val();
    };

    if ($('.prodotto').val() !== "") {
        var id = $('.prodotto').val();
        if (userMol === "True") {
            if (id == 1)
                id = 4;
        }
        if (userCol === "True") {
            if (id == 2)
                id = 5;
        }
        type = "&prodotto=" + id;
    };

    if ($('.esito').val() !== "") {
        esito = "&esito=" + $('.esito').val();
    };

    if ($('.dataDa').val() !== "") {
        dataDa = "&dataDa=" + $('.dataDa').val();
    };

    if ($('.ricevutaRitorno').val() !== "") {
        ar = "&ricevutaRitorno=" + $('.ricevutaRitorno').val();
    };

    if ($('.dataA').val() !== "") {
        dataA = "&dataA=" + $('.dataA').val();
    };

    if ($('.mittente').val() !== "") {
        mittente = "&mittente=" + $('.mittente').val();
    };

    var u = $('.utenti').val();
    if (u !== "" && u != undefined)
        userId = $('.utenti').val();
    else
        userId = myUserId;


    $('.export').attr("disabled", "disabled");

    $.get("/Default/GetAllOperationsNoBulletins?guidUser=" + guiduser + "&userId=" + userId + usrname + code + esito + dataDa + dataA + type + mittente + ar, function (res) {
        $('.names').empty();
        $('.preload-operation').hide();
        var i = 0;
        var r = jQuery.parseJSON(res);
        if (r.length > 0) {
            $('.export').removeAttr("disabled");
            var ids = "";
            var granTotal = 0;
            $.each(r, function (k, o) {
                i++;
                var type = "ROL";
                switch (o.operationType) {
                    case 2:
                        type = "LOL";
                        break;
                    case 3:
                        type = "TOL";
                        break;
                    case 4:
                        type = "MOL";
                        break;
                    case 5:
                        type = "COL";
                        break;

                }
                var fn = "";
                var classe = "";
                if (rr == "True") {
                    fn = "onclick='GetPdfGED()'";
                    classe = " class='blue' title='clicca per la cartolina di ritorno' ";
                }

                var file = o.fileName == null ? "" : o.fileName.split('\\');
                var fileName = file=="" ? "" : file[file.length -1];
                var ul = "<tr>" +
                    "<td>" + type + "</td>" +
                    "<td>" + o.Mittente + "</td>" +
                    "<td>" + o.Nominativo + "<br>" + o.Indirizzo +
                    "<br>" + o.Cap + " " + o.Citta + " (" + o.Provincia + ")</td>" +
                    "<td>" + new Date(o.DataInserimento).toLocaleDateString() +
                    " " + new Date(o.DataInserimento).toLocaleTimeString() + "</td>";
                    if (hidePrice != "TRUE") {
                        ul += "<td>€ " + o.totalPrice + "</td>";
                    }
                     ul += "<td style='overflow-x:scroll'>" + fileName + "</td>" +
                    "<td style='cursor:pointer!important;' " + fn + classe + ">" + (o.codice === null ? "" : o.codice) + "</td>" +
                    "<td class='status' id='" + o.requestId + "'>";
                if (o.stato === null) {
                    ul += "Non disponibile";
                }
                else
                {
                    if (type != 5 && type != 2)
                        ul += "<a href='https://www.poste.it/cerca/index.html#/risultati-spedizioni/" + o.codice + "' target='_blank' title='verifica lo stato sul sito di poste'>" + o.stato + "</a>";
                    else
                        ul += o.stato;
                }
                ul += "</td></tr>";
                $('.names').append(ul);
            });
        }
        else {
            var ul = "<tr><td class='empty-td' colspan='9'>Nessun nominativo trovato</td></tr>";
            $('.names').append(ul);
        };
        $('.numberOfResult').html(i + " risultati trovati");
    });

}

    function ExportCsv() {

        if ($('.prodotto').val() !== "") {
            var id = $('.prodotto').val();
            if (userMol === "True") {
                if (id == 1)
                    id = 4;
            }
            if (userCol === "True") {
                if (id == 2)
                    id = 5;
            }
        };

        $.redirect("/Corrispondenza/DownloadCsv",
            {
                username: $('.username').val(),
                code: $('.code').val(),
                esito: $('.esito').val(),
                dataDa: $('.dataDa').val(),
                dataA: $('.dataA').val(),
                type: id
            },
            "POST", "_blank");
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


    $(function () {
        $.get("/User/GetSenders", function (r) {
            var res = jQuery.parseJSON(r);
            if (res.length > 1) {
                for (var i = 0; i < res.length; i++) {
                    $('.mittente').append("<option>" + res[i] + "</option>")
                }
            }
            else {
                $('.mit').hide();
            }
        });
        $.get("/User/GetUsers", function (r) {
            var res = jQuery.parseJSON(r);
            if (res.length > 1) {
                for (var i = 0; i < res.length; i++) {
                    $('.utenti').append("<option value=" + res[i].id + ">" + res[i].name + " " + res[i].lastName + "</option>")
                }
            }
            else {
                $('.usr').hide();
            }
        });

        $('.prodotto').on('change', function () {
            if ($(this).val() == 1)
                $('.ars').show();
            else
                $('.ars').hide();
        })

    })
