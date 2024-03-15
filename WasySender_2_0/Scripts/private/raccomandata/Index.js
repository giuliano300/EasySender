function disabledAR() {
    $('.nominativoSenderAR').attr("disabled", true);
    $('.indirizzoSenderAR').attr("disabled", true);
    $('.capSenderAR').attr("disabled", true);
    $('.cittaSenderAR').attr("disabled", true);
    $('.selCittaSenderAR').attr("disabled", true);
    $('.provinciaSenderAR').attr("disabled", true);
    $('.statoSenderAR').attr("disabled", true);
}

function removeValuesAR() {
    $('.nominativoSenderAR').val('');
    $('.complementoNominativoSenderAR').val('');
    $('.indirizzoSenderAR').val('');
    $('.completamentoIndirizzoSenderAR').val('');
    $('.capSenderAR').val('');
    $('.cittaSenderAR').val('');
    $('.provinciaSenderAR').val('');
    $('.statoSenderAR').val('ITALIA');
};

$(function () {
    GetMittenti();
    disabledAR();
    $('.RicevutaRitorno').on('click', function () {
        if ($(this).val() === "1") {
            $('.nominativoSenderAR').attr("disabled", false);
            $('.indirizzoSenderAR').attr("disabled", false);
            $('.capSenderAR').attr("disabled", false);
            $('.cittaSenderAR').attr("disabled", false);
            $('.selCittaSenderAR').attr("disabled", false);
            $('.provinciaSenderAR').attr("disabled", false);
            $('.statoSenderAR').attr("disabled", false);
            $('.box-AR-multiple').show();
        }
        else {
            $('.box-AR-multiple').hide();
            disabledAR();
        }
    });

    $('.Formato').on('change', function () {
        if (this.value === "1")
            if (!confirm("Attenzione, selezionando formati speciali puoi inviare un pdf in vari formati A3,A5,ect. Confermi la selezione?")) {
                $('.special').removeAttr("checked");
                $('.a4').prop("checked", true);
            };
    })
});

function GetMittenti() {
    $.get("/User/GetMittenti", function (result) {
        if (result !== "null") {
            var res = jQuery.parseJSON(result);
            for (var i = 0; i < res.length; i++) {
                $('.senders').append("<option value=" + res[i].id + ">" + res[i].name + " " + res[i].surname + " " + res[i].businessName + "</option>")
            }
        }
        else {
            $('.senders').hide();
        }
    });
}

function GetMittenteAR(id) {
    if (id === "" || id === undefined)
        removeValuesAR();
    else {
        $.get("/User/GetMittente?Id=" + id, function (result) {
            if (result !== "null") {
                var res = jQuery.parseJSON(result);
                $('.nominativoSenderAR').val(res.businessName + " " + res.name + " " + res.surname);
                $('.complementoNominativoSenderAR').val(res.complementNames);
                $('.indirizzoSenderAR').val((res.dug !== "" ? res.dug + " " : "") + res.address + (res.houseNumber !== "" ? " " + res.houseNumber : ""));
                $('.completamentoIndirizzoSenderAR').val(res.complementAddress);
                $('.capSenderAR').val(res.cap);
                $('.cittaSenderAR').val(res.city);
                $('.provinciaSenderAR').val(res.province);
                $('.statoSenderAR').val(res.state);
            }
            else {
                removeValuesAR();
            }
        });
    }
}

function ReadAndWriteCapSenderAR() {
    $.post("/User/SearchCity", {
        cap: $('.capSenderAR').val()
    }, function (result) {
        if (result !== "null") {
            var res = jQuery.parseJSON(result);
            if (res.length > 1) {
                $('.cittaSenderAR').hide();
                $('.selCittaSenderAR').show();
                $('.cittaSenderAR').removeAttr("required");
                $('.selCittaSenderAR').attr("required", "required");
                $('.selCittaSenderAR').empty();
                $('.selCittaSenderAR').append("<option value=''>Seleziona un comune</option>");
                for (var i = 0; i < res.length; i++) {
                    $('.selCittaSenderAR').append("<option value='" + res[i].sigla + "'>" + res[i].comune + "</option>");
                };
                $('.provinciaSenderAR').val("");
            } else {
                $('.cittaSenderAR').show();
                $('.selCittaSenderAR').hide();
                $('.cittaSenderAR').attr("required");
                $('.selCittaSenderAR').removeAttr("required", "required");
                $('.cittaSenderAR').val(res[0].comune);
                $('.provinciaSenderAR').val(res[0].sigla);
            }
        }
        else {
            $('.cittaSenderAR').val("");
            $('.provinciaSenderAR').val("");
        }
    });
}

function GetProvinciaSenderAR() {
    var sigla = "";
    if ($('.selCittaSenderAR').val() !== "")
        sigla = $('.selCittaSenderAR').val();
    $('.provinciaSenderAR').val(sigla);
    var c = $(".selCittaSenderAR option:selected").text();
    $('.cittaSenderAR').val(c);
}


function onlyNumbers(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode > 31 && (charCode < 48 || charCode > 57))
        return false;

    return true;
}

function onlyLetters(evt) {
    var keyCode = (evt.which) ? evt.which : evt.keyCode
    if ((keyCode < 65 || keyCode > 90) && (keyCode < 97 || keyCode > 123) && keyCode !== 32)
        return false;

    return true;
}
