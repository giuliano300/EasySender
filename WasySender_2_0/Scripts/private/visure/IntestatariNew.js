$("div#myId").dropzone({
    url: "/Visure/UploadList",
    paramName: "file",
    maxFilesize: 50,
    maxFiles: 1,
    chunking: true,
    acceptedFiles: ".csv",
    dictDefaultMessage: "<i class='fas fa-file-upload'></i><br>Trascina un file oppure clicca per effettuare l'upload",
    init: function () {
        this.on("maxfilesexceeded", function (data) {
            $('.error-msg').html("File csv supera la dimensione consentita.");
            $('.error-msg').fadeIn(300);
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
                $('.error-msg').hide();
                $('.reqDestinatari').val(true);
                $(".dz-hidden-input").prop("disabled", true);
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
            "<li>" + item.businessName + "</li>" +
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
    $.post("/Visure/ValidateName", {
        nominativo: $('.nominativoNames').val(),
        provincia: $('.provinciaNames').val(),
        fiscalCode: $('.fiscalCodeNames').val(),
        NREA: $('.NREANames').val(),
    }, function (response) {
        if (response !== "") {
            var res = jQuery.parseJSON(response);
            if (res.Valido === false) {
                $('.error-popup').html(res.Errore);
                $('.error-popup').fadeIn(300);
            }
            else {
                $.post("/Visure/SaveNameList", {
                    nominativo: $('.nominativoNames').val(),
                    provincia: $('.provinciaNames').val(),
                    fiscalCode: $('.fiscalCodeNames').val(),
                    NREA: $('.NREANames').val(),
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
    });
}

function OpenEditName(index) {
    $.post("/Visure/GetNames", {
        index: index
    }, function (result) {
        if (result !== "null") {
            var res = jQuery.parseJSON(result);
            $('.nominativoNames').val(res[0].businessName);
            $('.provinciaNames').val(res[0].province);
            $('.fiscalCodeNames').val(res[0].fiscalCode);
            $('.NREANames').val(res[0].NREA);
            $('.bg-pop-up').show();
       }
        else {
            $('.error-msg').html("Errore generico!");
            $('.error-msg').fadeIn(300);
        }
    });
}


function PreloadOperation() {
    $('.prosegui').attr("disabled", "disabled");
    $('.preload-operation').show();
}

function onlyNumbers(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}
