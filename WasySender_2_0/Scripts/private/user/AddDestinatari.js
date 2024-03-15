$("div#myId").dropzone({
    url: "/User/UploadList",
    paramName: "file",
    maxFilesize: 50,
    maxFiles: 1,
    chunking: true,
    acceptedFiles: ".csv",
    dictDefaultMessage: "<i class='fas fa-file-upload'></i><br>Trascina un file oppure clicca per effettuare l'upload",
    init: function () {
        this.on("maxfilesexceeded", function (data) {
            var res = eval('(' + data.xhr.responseText + ')');
        });
        this.on("addedfile", function (file) {
            $('.error-msg').hide();
            $('.preload-file').show();
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
                $('.preload-file').html('<i class="fas fa-file-upload"></i><br>Trascina un file oppure clicca per effettuare l\'upload');
                $('.preload-nominativi').show();
                TimeControl();
           }
        });
    },
    success: function (file, response) {
        $('.preload-nominativi').hide();
        $('.preload-file').hide();
        var res = jQuery.parseJSON(response);
        if (res.success) {
            if (res.errors >= res.numberOfNames) {
                $('.error-msg').html("Nessun destinatario valido nel csv caricato.");
                $('.error-msg').fadeIn(300);
                $('.progress-bar').css('width', '0%');
                $('.progress-bar').text('0%');
            }
            else {
                ShowNames(res);
                $('.msg').fadeIn(300);
                $('.error-msg').hide();
                $('.reqDestinatari').val(true);
            }
        }
        else {
            if (res.errorMessage === "")
                $('.error-msg').html("Nessun destinatario valido nel csv caricato.");
            else
                $('.error-msg').html(res.errorMessage + "<br>Ricarica il csv utilizzando tutti i campi obbligatori.");

            $('.error-msg').fadeIn(300);
            $('.progress-bar').css('width', '0%');
            $('.progress-bar').text('0%');
        }
    }
});

function ShowNames(res) {
    var r = $('.responseList');
    r.empty();
    $.each(res.NamesLists, function (i, item) {
        var ul = "<ul onclick='OpenEditName(" + i + ")' " + (!item.valid ? "class='error-item'" : "class='valid-item'") + ">" +
            "<li>" + item.businessName + ' ' + item.name + ' ' + item.surname + "</li>" +
            "<li>" + (item.valid ? "SI" : "NO") + "</li>" +
            "<li>" + (item.errorMessage === null ? "&nbsp;" : item.errorMessage) + "</li>";
        $(ul).appendTo(r);
    });
    $('.errori').html(res.errors);
    $('.validi').html(parseInt(res.numberOfNames) - parseInt(res.errors));
    $('.totali').html(res.numberOfNames);
    $('.NamesLists').val(res.NamesLists);
    if (res.errors > 0) {
        $('.msg p').html('Ci sono ' + res.errors + ' errori. Clicca sulla riga dell\'errore per correggere il nominativo.<br>I nominativi non corretti non saranno spediti');
        $('.download-errors').show();
    }
    else {
        $('.msg').addClass('msg-ok');
    }
    $('.msg').show();
}

function validateForm() {
    $('.error-msg').hide();
    $.post("/User/ValidateName", {
        ragsoc: $('.ragsoc').val(),
        nome: $('.nome').val(),
        cognome: $('.cognome').val(),
        dug: $('.dug').val(),
        indirizzo: $('.indirizzo').val(),
        numeroCivico: $('.numeroCivico').val(),
        cap: $('.cap').val(),
        citta: $('.citta').val(),
        provincia: $('.provincia').val(),
        stato: $('.stato').val(),
        complementNames: $('.completamentoNominativo').val(),
        complementAddress: $('.completamentoIndirizzo').val(),
        fiscalCode: $('.cf').val(),
    }, function (response) {
        if (response !== "") {
            var res = jQuery.parseJSON(response);
            if (res.Valido === false) {
                $('.error-popup').html(res.Errore);
                $('.error-popup').fadeIn(300);
            }
            else {
                $.post("/User/SaveNameList", {
                    ragsoc: $('.ragsoc').val(),
                    nome: $('.nome').val(),
                    cognome: $('.cognome').val(),
                    dug: $('.dug').val(),
                    indirizzo: $('.indirizzo').val(),
                    numeroCivico: $('.numeroCivico').val(),
                    cap: $('.cap').val(),
                    citta: $('.citta').val(),
                    provincia: $('.provincia').val(),
                    stato: $('.stato').val(),
                    fileName: $('.file').val(),
                    complementNames: $('.completamentoNominativo').val(),
                    complementAddress: $('.completamentoIndirizzo').val(),
                    fiscalCode: $('.cf').val(),
                    index: $('.index').val()
                }, function (response) {
                    var res = jQuery.parseJSON(response);
                    if (res.success) {
                        ShowNames(res);
                        $('.msg').fadeIn(300);
                        $('.error-msg').hide();
                        $('.reqDestinatari').val(true);
                        ClosePopup();
                    }
                });
            }
        }
    })
}

function ReadAndWriteCap() {
    var state = $('.stato').val().toLowerCase();
    if (state !== "italia")
        return;

    $.post("/Raccomandata/SearchCity", {
        cap: $('.cap').val()
    }, function (result) {
        if (result !== "null") {
            var res = jQuery.parseJSON(result);
            if (res.length > 1) {
                $('.citta').hide();
                $('.selCitta').show();
                $('.citta').removeAttr("required");
                $('.selCitta').attr("required", "required");
                $('.selCitta').empty();
                $('.selCitta').append("<option value=''>Seleziona un comune</option>");
                for (var i = 0; i < res.length; i++) {
                    $('.selCitta').append("<option value='" + res[i].sigla + "'>" + res[i].comune + "</option>");
                };
                $('.provincia').val("");
                $('.error-popup').html("");
                $('.error-popup').fadeOut(300);
            } else {
                $('.citta').show();
                $('.selCitta').hide();
                $('.citta').attr("required");
                $('.selCitta').removeAttr("required", "required");
                $('.citta').val(res[0].comune);
                $('.provincia').val(res[0].sigla);
                $('.error-popup').html("");
                $('.error-popup').fadeOut(300);
            }
        }
        else {
            $('.citta').val("");
            $('.provincia').val("");
            $('.error-popup').html("Cap inserito non corrisponde a nessun valore presente nel nostro database.");
            $('.error-popup').fadeIn(300);
        }
    });
}

function GetProvincia() {
    var sigla = "";
    if ($('.selCitta').val() !== "")
        sigla = $('.selCitta').val();
    $('.provincia').val(sigla);
    var c = $(".selCitta option:selected").text();
    $('.citta').val(c);
}

function ShowSelectOrInput(cap) {
    var state = $('.stato').val().toLowerCase();
    if (state !== "italia")
        return;

    $.post("/Raccomandata/SearchCity", {
        cap: cap
    }, function (result) {
        if (result !== "null") {
            var res = jQuery.parseJSON(result);
            if (res.length > 1) {
                $('.citta').hide();
                $('.selCitta').show();
                $('.citta').removeAttr("required");
                $('.selCitta').attr("required", "required");
                for (var i = 0; i < res.length; i++) {
                    $('.selCitta').append("<option value='" + res[i].sigla + "'>" + res[i].comune + "</option>");
                };
            } else {
                $('.citta').show();
                $('.selCitta').hide();
                $('.citta').attr("required");
                $('.selCitta').removeAttr("required", "required");
                $('.citta').val(res[0].comune);
            }
        }
        else {
            $('.citta').val("");
            $('.provincia').val("");
            $('.error-popup').html("Cap inserito non corrisponde a nessun valore presente nel nostro database.");
            $('.error-popup').fadeIn(300);
        }
    });
}


function OpenEditName(index) {
    $.post("/User/GetNames", {
        index: index
    }, function (result) {
        if (result !== "null") {
            $('.bg-pop-up').show();
            var res = jQuery.parseJSON(result);
            $('.ragsoc').val(res.businessName);
            $('.nome').val(res.name);
            $('.cognome').val(res.surname);
            $('.dug').val(res.dug);
            $('.indirizzo').val(res.address);
            $('.numeroCivico').val(res.houseNumber);
            $('.cap').val(res.cap);
            $('.citta').val(res.city);
            $('.provincia').val(res.province);
            $('.stato').val(res.state);
            $('.completamentoNominativo').val(res.complementNames);
            $('.completamentoIndirizzo').val(res.complementAddress);
            $('.cf').val(res.fiscalCode);
            $('.index').val(index);
            $('.error-msg').html("");
            $('.error-msg').fadeOut(300);
        }
        else {
            $('.error-msg').html("Errore generico!");
            $('.error-msg').fadeIn(300);
        }
    });
}

function TimeControl() {
    setInterval(function () {

        $.getJSON("/Scripts/api/configuration.json", function (c) {
            var url = c.apiurl;
            if (areaTestUser === "True")
                url = c.apiurlTest;

            $.get(url + "/TemporaryValidateTable/GetPercentage?userId=" + userId + "&sessionId=" + sessionId, function (w) {
                if (w !== null) {
                    $('.progress-bar-nominativi').html(w + "%");
                    $('.progress-bar-nominativi').animate({ "width": w + "%" }, 800);
                }
            });
        });
    }, 2000);
}

function onlyNumbers(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}
