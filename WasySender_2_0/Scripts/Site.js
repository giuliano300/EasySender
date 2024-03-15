$(function () {
    //$("body").fadeIn(200);


    $(".show-pwd").mousedown(function () {
        $(".pwd").attr("type", "text");
        $('.fa-eye').removeClass('hidden');
        $('.fa-eye-slash').addClass('hidden');
    });

    $(".show-pwd").mouseup(function () {
        $(".pwd").attr("type", "password");
        $('.fa-eye-slash').removeClass('hidden');
        $('.fa-eye').addClass('hidden');
    });


    createNiceScroll();

    $(".master aside ul li").click(function () {
        $("span").removeClass("active");
        $("span", this).addClass("active");
        $(".sub-menu").slideUp(300);
        $(".sub-menu", this).slideDown(300);

    });

    $(".creativita li").click(function () {
        $(".creativita li label").removeClass("input-on");
        $("label", this).addClass("input-on");
    });
    $(".lista li").click(function () {
        $(".lista li label").removeClass("input-on");
        $("label", this).addClass("input-on");
    });

    $('.open-fullscreen').click(function () {
        $('body').fullscreen();
        return false;
    });

    $("#slider-3").slider({
        range: true,
        min: 18,
        max: 99,
        values: [18, 45],
        slide: function (event, ui) {
            $("#price").val("da " + ui.values[0] + " fino a " + ui.values[1] + " anni");
            $('.minEta').val(ui.values[0]);
            $('.maxEta').val(ui.values[1]);
        }
    });
    $("#price").val("Da " + $("#slider-3").slider("values", 0) +
        " fino a " + $("#slider-3").slider("values", 1) + " anni");

    $("#slider-range-max").slider({
        range: "max",
        min: 500,
        max: 5000,
        value: 500,
        step: 10,
        slide: function (event, ui) {
            $("#amount").val(ui.value);
            $(".numberOfNames").val(ui.value);
            $(".number").val(ui.value);
        }
    });

    $("#amount").val($("#slider-range-max").slider("value"));

    $("input:radio[name=destinatari]").click(function () {
        var destinatariValue = $("input:radio[name=destinatari]:checked").val();

        switch (destinatariValue) {
            case '1':
                $("#NewList").show();
                $("#SelectList").hide();
                $("#RequestList").hide();
                $('.lists').removeAttr("required");
                break;
            case '2':
                $("#SelectList").show();
                $("#NewList").hide();
                $("#RequestList").hide();
                $('.lists').attr("required", "required");
                break;
            case '3':
                $("#RequestList").show();
                $("#SelectList").hide();
                $("#NewList").hide();
                $('.lists').removeAttr("required");
                break;
            default:
                break;
        }

    });


    $("#SubmitTelegramma").click(function () {
        if ($('.SelTelegramma').val() === "")
            alert("Selezionare una tipologia");
        else
            location.href = "/Telegramma/Index?id=" + $('.SelTelegramma').val();
    });

    $("#SubmitRaccomandata").click(function () {
        if ($('.SelRaccomandata').val() === "")
            alert("Selezionare una tipologia");
        else
            location.href = "/Raccomandata/Index?id=" + $('.SelRaccomandata').val();
    });

    $("#SubmitLettera").click(function () {
        if ($('.SelLettera').val() === "")
            alert("Selezionare una tipologia");
        else
            location.href = "/Lettera/Index?id=" + $('.SelLettera').val();

    });
});

function ClosePopup() {
    $(".popup-std").fadeOut(300);
}


function modifySlider() {
    var n = $(".number").val();
    if (n > 5000)
        n = 5000;
    if (n < 500)
        n = 500;
    $(".number").val(n);
    $("#amount").val(n);
    $(".numberOfNames").val(n);
    $("#slider-range-max").slider({
        range: "max",
        min: 500,
        max: 5000,
        value: n,
        step: 10
    });
    return true;
}

function soloNumeri(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}

function createNiceScroll() {
    $("html").niceScroll({
        cursorwidth: '10px',
        autohidemode: true,
        zindex: 9999,
        horizrailenabled: false,
        cursorborder: '0px solid #000',
        enablescrollonselection: false,
        cursorborderradius: '2px',
        cursoropacitymax: 0.6
    });
    $(".comunicazioni").niceScroll({
        cursorwidth: '3px',
        autohidemode: true,
        zindex: 9999,
        horizrailenabled: false,
        cursorborder: '0px solid #000',
        enablescrollonselection: false,
        cursorborderradius: '1px',
        cursoropacitymax: 0.6
    });
    $(".comunicazioni-poste").niceScroll({
        cursorwidth: '3px',
        autohidemode: true,
        zindex: 9999,
        horizrailenabled: false,
        cursorborder: '0px solid #000',
        enablescrollonselection: false,
        cursorborderradius: '1px',
        cursoropacitymax: 0.6
    });
    $(".stato-sped-grid").niceScroll({
        cursorwidth: '5px',
        autohidemode: true,
        zindex: 9999,
        horizrailenabled: false,
        cursorborder: '0px solid #000',
        enablescrollonselection: false,
        cursorborderradius: '1px',
        cursoropacitymax: 0.6
    });
    $(".stato-sped-grid-lotto").niceScroll({
        cursorwidth: '5px',
        autohidemode: true,
        zindex: 9999,
        horizrailenabled: false,
        cursorborder: '0px solid #000',
        enablescrollonselection: false,
        cursorborderradius: '1px',
        cursoropacitymax: 0.6
    });
    $(".content-table-responsive").niceScroll({
        cursorwidth: '5px',
        autohidemode: true,
        zindex: 9999,
        horizrailenabled: false,
        cursorborder: '0px solid #000',
        enablescrollonselection: false,
        cursorborderradius: '1px',
        cursoropacitymax: 0.6
    });
    $(".content-result-list").niceScroll({
        cursorwidth: '5px',
        autohidemode: true,
        zindex: 9999,
        horizrailenabled: false,
        cursorborder: '0px solid #000',
        enablescrollonselection: false,
        cursorborderradius: '1px',
        cursoropacitymax: 0.6
    });
}

function OpenMenuMobile() {
    $(".sub-menu-mobile").slideToggle(200);
}
function OpenSubMenuMobile() {
    $(".sub-menu-mobile ul li ul").slideToggle(200);
}

function LastStep() {
    location.href = "/DM/StepEnd";
}

function SelCreativita() {

        var creativitaValue = $("input:radio[name=creativita]:checked").val();

        switch (creativitaValue) {
            case '1':
                location.href = "/DM/Step4";
                break;
            case '2':
                location.href = "/DM/Step3";
                break;
            default:
                break;
        }

}


function destroyNiceScroll() {
    setTimeout(function () {
        $("html").getNiceScroll().remove();
    }, 100);
}


function ScrollPage() {
    $("html, body").animate({ scrollTop: $(window).height() }, 400);
}


function ScrollTop() {
    $('html, body').animate({
        scrollTop: $(".navigation").offset().top
    }, 700);
}

function goTo(Pagina) {
    var margin = 50;
    if ($('body').data('OpenMenu')) {
        $(".navigation").fadeOut(300, function () {
            $("body").delay(100).fadeOut(300, function () {
                location.href = Pagina;
        });
    });
    } else {
        $("body").delay(100).fadeOut(300, function () {
            location.href = Pagina;
        });
    }
}

/* WOW ANIMATE */
    var wow = new WOW(
{
    boxClass: 'wowload',      
    animateClass: 'animated', 
    offset: 0,      
    mobile: true,      
    live: true
}
    );
    wow.init();








