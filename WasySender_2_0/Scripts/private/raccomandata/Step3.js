$("div#myId").dropzone({
    url: "/Raccomandata/UploadFile?ListId=" + ListId + "&fronteRetro=" + fronteRetro,
    paramName: "file",
    maxFilesize: 1000,
    maxFiles: 1,
    chunking: true,
    acceptedFiles: ".zip",
    dictDefaultMessage: "<i class='fas fa-file-upload'></i><br>Trascina un file in formato(zip) oppure clicca per effettuare l'upload<br>" +
        "N.B.Carica tutti i pdf di tutti i destinatari.<br>I destinatati senza pdf non saranno presi in carico per la spedizione.",
    init: function () {
        this.on("maxfilesexceeded", function (data) {
            var res = eval('(' + data.xhr.responseText + ')');
        });
        this.on("addedfile", function (file) {
            $('.preload-file').show();
            $('.error-msg').hide();
        });
        this.on("processing", function (file) {
            $('.progress-bar').css('width', '0%');
            $('.progress-bar').text('0%');
        });
        this.on("complete", function (file) {
            this.removeFile(file);
        });
        this.on("uploadprogress", function (file, a) {
            $('.progress-bar').css('width', parseInt(a) + '%');
            $('.progress-bar').text(parseInt(a) + '%');
            if (a === 100) {
                $('.preload-file').html('<i class="fas fa-file-upload"></i><br>Trascina un file in formato(zip) oppure clicca per effettuare l\'upload <br>N.B.Carica tutti i pdf di tutti i destinatari.<br>I destinatati senza pdf non saranno presi in carico per la spedizione.')
                $('.msg-base').html("Il file è stato caricato!<br>Attendi qualche minuto lo scompattamento del file zip, tempo, durante il quale, non potrai cliccare su calcola il preventivo.");
            }
        });
    },
    success: function (file, response) {
        if (response === "") {
            location.href = "/Home/Error500";
        }
        else {
            var res = jQuery.parseJSON(response);
            $('.file').val(res.filePath);
            ShowNames(res.name);
        }
    }
});

function ShowNames(res) {
    $('.top-result').show();
    $('.msg-static').hide();
    var r = $('.responseList');
    r.empty();

    var l = res.length;
    var error = 0;

    for (var i = 0; i < res.length; i++) {
        var ul = "<ul " + (!res[i].success ? "class='error-item'" : "class='valid-item'") + ">" +
            "<li>" + res[i].name.businessName + ' ' + res[i].name.name + ' ' + res[i].name.surname + "</li>" +
            "<li>" + (res[i].success ? "SI" : "NO") + "</li>" +
            "<li>" + (res[i].errorMessage === null ? "&nbsp;" : res[i].errorMessage) + "</li>";
        $(ul).appendTo(r);
        if (!res[i].success)
            error++;
    }

    $('.errori').html(error);
    $('.validi').html(parseInt(res.length) - parseInt(error));
    $('.totali').html(res.length);
}


function OpenRiepilogo() {
    $('.content-box-riepilogo').show();
}

function CloseRiepilogo() {
    $('.content-box-riepilogo').hide();
}

function GetPreventivo(molCol) {
    if ($('.file').val() === "") {
        alert('Aggiungi prima il file ZIP contenente tutti i pdf dei destinatari.');
    }
    else {
        $('.preload-operation').show();
        $.post("/Raccomandata/CalcolaPreventivo", {
            ListId: $('.ListId').val(),
            Bollettini: $('.Bollettini').val(),
            Sender: $('.Sender').val(),
            SenderAR: $('.SenderAR').val(),
            TipoStampa: $('.TipoStampa').val(),
            FronteRetro: $('.FronteRetro').val(),
            RicevutaRitorno: $('.RicevutaRitorno').val(),
            Formato: $('.Formato').val(),
            file: $('.file').val()

        }, function (response) {
            $('.preload-operation').hide();
            if (response !== "") {
                if (response === "InternalServerError") {
                    $('.preload-report').fadeOut(500);
                    $('.error-msg').html("ATTENZIONE!<br>Si è veriricato un errore del sistema.<br>Ricaricare il file .zip oppure effettuare di nuovo la procedura.");
                    $('.error-msg').fadeIn(300);
                    $('.progress-bar').css('width', '0%');
                    $('.progress-bar').text('0%');
                }
                else
                {
                    var res = jQuery.parseJSON(response);
                    if (res.numberOfNames === 0) {
                        $('.preload-report').fadeOut(500);
                        $('.error-msg').html("ATTENZIONE!<br>Non è stato possibile valorizzare nessun destinatario.<br>Effettuare di nuovo la procedura.");
                        $('.error-msg').fadeIn(300);
                        $('.progress-bar').css('width', '0%');
                        $('.progress-bar').text('0%');
                    }
                    else
                    {
                        if(molCol)
                            location.href = "StepEnd?id=" + res.operationId;
                        else
                            location.href = "Preventivo";
                    }
                }
            }
            else {
                    $('.preload-report').fadeOut(500);
                    $('.error-msg').fadeIn(300);
                    $('.progress-bar').css('width', '0%');
                    $('.progress-bar').text('0%');
            }
            });

    }
}
