function ApplyFilter() {
    $('.preload-operation').show();
    $('.bulletin-show').hide();
    var usrname = "";
    var type = "";
    var code = "";
    var esito = "";
    var dataDa = "";
    var dataA = "";
    var dataDaPayments = "";
    var dataAPayments = "";
    var bollettini = "";
    var pagato = "";
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

    if ($('.ricevutaRitorno').val() !== "") {
        ar = "&ricevutaRitorno=" + $('.ricevutaRitorno').val();
    };

    if ($('.dataDa').val() !== "") {
        dataDa = "&dataDa=" + $('.dataDa').val();
    };

    if ($('.dataA').val() !== "") {
        dataA = "&dataA=" + $('.dataA').val();
    };

    if ($('.dataDaPayments').val() !== "" && $('.dataDaPayments').val() !== undefined) {
        dataDaPayments = "&dataDaPayments=" + $('.dataDaPayments').val();
    };

    if ($('.dataAPayments').val() !== "" && $('.dataAPayments').val() !== undefined) {
        dataAPayments = "&dataAPayments=" + $('.dataAPayments').val();
    };

    bollettini = "&bollettini=true";
    $('.bulletin-show').show();

    if ($('.pagato').val() !== "" && $('.pagato').val() !== undefined) {
        pagato = "&pagato=" + $('.pagato').val();
    };

    var u = $('.utenti').val();
    if (u !== "" && u != undefined)
        userId = $('.utenti').val();
    else
        userId = myUserId;


    $('.export').attr("disabled", "disabled");

    $.get("/Default/GetAllOperationsWithBulletins?guidUser=" + guiduser + "&userId=" + userId + usrname + code + esito + dataDa + dataA + type + bollettini + pagato + dataDaPayments + dataAPayments + ar, function (res) {
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
                var type = o.operationType;
                var mittente = o.sender.businessName + " " + o.sender.name + " " + o.sender.surname
                var v = o.recipient;
                var cod = v.bulletin != null ? "codice: " + v.bulletin.CodiceCliente + (v.bulletin.Pagato === true ? "<br><span style='color:#46a289'>PAGATO</span>" : "<br><span style='color:red'>NON PAGATO</span>") : "";
                var liHeight = v.bulletin != null ? "style=height:85px!important;" : "";


                var fn = "";
                var classe = "";
                if (rr == "True") {
                    fn = "onclick='GetPdfGED(" + v.recipient.id + ")'";
                    classe = " class='blue' title='clicca per la cartolina di ritorno' ";
                }


                if (v.bulletin != null)
                    granTotal += v.bulletin.ImportoEuro;

                var file = v.recipient.fileName.split('\\');
                var fileName = file[file.length -1];
                var ul = "<tr " + liHeight + ">" +
                    "<td>" + type + "</td>" +
                    "<td>" + mittente + "</td>" +
                    "<td>" + cod + "<br>" + v.recipient.businessName + " " + v.recipient.name + " " + v.recipient.surname +
                    " " + v.recipient.complementNames + "<br>" + v.recipient.dug + " " + v.recipient.address + " " + v.recipient.houseNumber +
                    "<br>" + v.recipient.cap + " " + v.recipient.city + " (" + v.recipient.province + ")</td>" +
                    "<td>" + (v.recipient.valid ? "OK" : "ERRORE") + "</td>" +
                    "<td>" + new Date(v.recipient.presaInCaricoDate).toLocaleDateString() + " " + new Date(v.recipient.presaInCaricoDate).toLocaleTimeString() + "</td>";
                    if (hidePrice != "TRUE") {
                        ul += "<td>€ " + v.recipient.totalPrice + "</td>" +
                              "<td>€ " + v.bulletin.ImportoEuro + "</td>";
                    }
                    
                     ul += "<td>" + fileName.split('/')[1] + "</td>" +
                    "<td style='cursor:pointer!important;' " + fn + classe + ">" + (v.recipient.codice === null ? "" : v.recipient.codice) + "</td>" +
                    "<td class='status' id='" + v.recipient.requestId + "'>";
                if (v.recipient.stato === null) {
                    ul += "Non disponibile";
                }
                else
                {
                    if (type != "LOL")
                        ul += "<a href='https://www.poste.it/cerca/index.html#/risultati-spedizioni/" + v.recipient.codice + "' target='_blank' title='verifica lo stato sul sito di poste'>" + v.recipient.stato + "</a>";
                    else
                        ul += v.recipient.stato;
                }
                ul += "</td></tr>";
                $('.names').append(ul);
            });

            $('.bulletinTotal').html(granTotal.toFixed(2) + "€");
        }
        else {
            var ul = "<tr><td class='empty-td' colspan='10'>Nessun nominativo trovato</td></tr>";
            $('.names').append(ul);
            $('.bulletinTotal').html("0€");
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

        $.redirect("/Corrispondenza/DownloadCsvBulletin",
            {
                username: $('.username').val(),
                code: $('.code').val(),
                esito: $('.esito').val(),
                dataDa: $('.dataDa').val(),
                dataA: $('.dataA').val(), 
                dataDaPayments: $('.dataDaPayments').val(),
                dataAPayments: $('.dataAPayments').val(),
                pagato: $('.pagato').val()
            },
            "POST", "_blank");
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
