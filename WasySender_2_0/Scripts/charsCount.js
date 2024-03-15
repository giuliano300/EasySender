function charcountupdate(str, lenght) {
    var lng = str.length;
    var remaining = lenght - lng;
    $('.counter').html("Caratteri rimanenti " + remaining);
}