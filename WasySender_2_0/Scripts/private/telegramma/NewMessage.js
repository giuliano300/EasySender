function GetPreventivo() {
    if ($('.messaggio-telegramma').val() === "") {
        alert('Aggiungi il testo del messaggio da inviare.');
    }
    else {
        $('.preload-operation').show();
        $.post("/telegramma/CalcolaPreventivo", {
            ListId: $('.ListId').val(),
            Sender: $('.Sender').val(),
            msg: $('.messaggio-telegramma').val(),
            RicevutaRitorno: $('.RicevutaRitorno').val(),

        }, function (response) {
            if (response !== "") {
                location.href = "Preventivo";
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
