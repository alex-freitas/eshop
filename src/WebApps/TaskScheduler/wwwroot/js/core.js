



window.fn = function (callback, p1, p2, p3, p4, p5) {

    return function () {
        return callback(p1, p2, p3, p4, p5)
    }
};


$.fn.fadedload = function (url) {

    var $this = $(this);

    $this.fadeOut(100, fn($this.load, url, fn($this.fadeIn, 100)));
};


$.fn.ajaxLoad = function ($ajax) {

    var $elem = $(this);

    var $def = $.Deferred();

    $ajax.done(function (data) {
        $elem.html(data);

        $def.resolve(data);
    });

    return $def;
};


$.mensagem = function (texto, args) {

    var $body = $("body");
    var $div = $("<div>");

    var values = {
        milisegundos: 2000,
        cssClass: "mensagem"
    };

    $.extend(values, args);

    $body.append($div);
    $div.hide();


    if (Array.isArray(texto)) {

        $.each(texto, function (index, item) {
            $div.append(item + "<br/>");
        });
    }
    else
        $div.html(texto);


    $div.addClass(values.cssClass);

    $div.css({
        position: "fixed",
        left: ($body.outerWidth() / 2) - ($div.outerWidth() / 2),
        top: 0,
        zIndex: 999999999
    });

    $div.showFor({
        effect: "fade",
        milliseconds: values.milisegundos,
        effectDuration: 150,
        callback: function () { $div.remove() }
    });


    //Quando a div for deletada, remmove o evento para acompanhar o scroll
    $div.on("remove", function () {
        $(window).off("scroll", followScroll);
    });

    return $div;
};


//$.mensagemSalvo = function () { $.mensagem('Registro salvo com sucesso') };
$.mensagemSalvo = fn($.mensagem, "Registro salvo com sucesso");


//$.mensagemConsultaAtualizada = function () { $.mensagem('Consulta atualizada') };
$.mensagemConsultaAtualizada = fn($.mensagem, "Consulta atualizada");


//$.mensagemExcluido = function () { $.mensagem('Registro excluído com sucesso') };
$.mensagemExcluido = fn($.mensagem, "Registro excluído com sucesso");


$.mensagemErro = function (texto) {

    if (!texto)
        texto = "Algum erro ocorreu durante o processo";

    $.mensagem(texto, { cssClass: "mensagemErro" });
};


$.fn.showFor = function (args) {
    var values = {
        milliseconds: 2000,
        effect: "fade",
        effectDuration: 200,
        callback: undefined
    };


    $.extend(values, args);

    var $this = $(this);


    var effectShowFunction = $.fn.fadeIn;
    var effectHideFunction = $.fn.fadeOut;


    if (values.effect == "slide") {

        effectShowFunction = $.fn.slideDown;
        effectHideFunction = $.fn.slideUp;;
    }


    effectShowFunction.call($this, values.effectDuration, fn(setTimeout,
        function () { effectHideFunction.call($this, values.effectDuration, values.callback) },
        values.milliseconds
    ));
};


$.display = {

    mask: {

        cnpj: "99.999.999/9999-99",
        cpf: "999.999.999-99",
        cep: "99999-999",
        date: "99/99/9999"
    }
};

$.fn.setSpecialInputs = function () {

    $("input.price", this).priceFormat({ prefix: "", centsSeparator: ",", thousandsSeparator: "." });

    $("input.date", this).mask($.display.mask.date);
    $("input.date", this).datepicker();

    $("input.datetime", this).mask("99/99/9999 99:99");
    $("input.datetime", this).datetimepicker();

    $("input.cep", this).mask($.display.mask.cep);
    $("input.cpf", this).mask($.display.mask.cpf);
    $("input.cnpj", this).mask($.display.mask.cnpj);
};


$.myContext = function (selector) {
    return function (mySelector) { return $(mySelector, selector); }
};


$.fn.toJSON = function () {

    var $inputs = $(this).find("input,textarea,select");

    var json = {};

    $inputs.each(function (index, item) {

        if (item.tagName == "TEXTAREA")
            json[item["name"]] = $(item).val();

        else if ($(item).attr("type") == "checkbox")
            json[item["name"]] = $(item).prop("checked");

        else if ($(item).attr("type") == "radio") {
            if ($(item).prop("checked"))
                json[item["name"]] = item["value"];
        }

        else
            json[item["name"]] = item["value"];
    });

    return json;
};

$.confirmarExclusao = fn(confirm, "Deseja realmente excluir este registro?");

$.dialog = function (userArgs) {

    var args = {
        width: 500,
        height: 300,
        closeButton: true,
        header: true,
        fadeTime: 200,
        popCover: false
    };

    $.extend(args, userArgs);

    var $def = $.Deferred();

    var $body = $("body");

    var $background = $("<div>");
    var $popup = $("<div>");
    var $header = $("<div>");
    var $content = $("<div>");

    var $close = $("<button>");

    $popup.hide();
    $background.hide();

    //Controla o zIndex em caso de popups em cascata
    if ($(window).data("popup_zIndexCount"))
        //Sempre incrementa de 2 em 2, um zIndex para o background e o outro para o popup
        $(window).data("popup_zIndexCount", $(window).data("popup_zIndexCount") + 2);
    else
        $(window).data("popup_zIndexCount", 1100);

    var zIndex = $(window).data("popup_zIndexCount");

    $popup.data("args", args);


    var createPopup = function () {

        $background.css({
            zIndex: zIndex,
            margin: "0",
            padding: "0",
            border: "0",
            position: "fixed",
            left: "0",
            top: "0",
            backgroundColor: "#AAAAAA",
            opacity: "0.4"
        });

        var setBackgroundSize = function () {
            $background.width($(document).width());
            $background.height($(document).height());
        };

        $body.append($background);

        $(window).resize(setBackgroundSize);

        $popup.data("$background", $background);
        $popup.data("$def", $def);

        $popup.addClass("popup_class panel-primary");

        if (args.popCover || args.fullscreen) {

            $popup.css({
                zIndex: zIndex + 1,
                position: "fixed",
                left: 0,
                top: 0,
                height: "100%",
                width: "100%"
            });

        } else {

            $popup.width(args.width);
            $popup.height(args.height);

            var popupLeft = ($(window).outerWidth() / 2) - ($popup.outerWidth() / 2);
            var popupTop = ($(window).outerHeight() / 2) - ($popup.outerHeight() / 2) - 50;

            if (popupLeft < 0) popupLeft = 0;
            if (popupTop < 10) popupTop = 10;

            $popup.css({
                zIndex: zIndex + 1,
                position: "fixed",
                left: popupLeft,
                top: popupTop
            });
        }

        $body.append($popup);


        if (args.header) {

            $header.addClass("popup_header panel-heading");

            $header.html(args.title);

            $popup.append($header);
        }

        $popup.append($content);

        $content.css({
            padding: "5px",
            boxSizing: "border-box",
            width: "100%",
            maxHeight: $popup.innerHeight() - $header.outerHeight() - 12,
            overflowY: "auto"
        });

        $content.addClass("popup_content");


        if (args.closeButton) {
            $close.addClass("cancel popup_closeButton btn btn-sm btn-danger");

            $close.html("x");

            $popup.append($close);

            $close.click(function () { $close.closeDialog() });
        }

        $popup.fadeIn(args.fadeTime);
        $background.fadeIn(args.fadeTime);

        setBackgroundSize();
    };


    if (args.message) {

        $content.html(args.message);
        createPopup();
    }
    else if (args.messageList) {

        var message = new String();

        $.each(args.messageList, function (index, item) { message += item + "<br/>" });

        $content.html(message);
        createPopup();
    }

    else if (args.element) {
        $content.append(args.element);

        createPopup();
    }
    else if (args.elements) {

        $.each(args.elements, function (i, elem) {
            $content.append(elem);
        });

        createPopup();
    }
    else
        $content.ajaxLoad(args.action).done(createPopup);


    return $def;
};


$.fn.closeDialog = function (data) {

    var $popup = $(this).parents(".popup_class:first");

    var $background = $popup.data("$background");
    var $def = $popup.data("$def");

    var args = $popup.data("args");

    $popup.fadeOut(args.fadeTime, function () { $popup.remove() });
    $background.fadeOut(args.fadeTime, function () { $background.remove() });

    $def.resolve(data);
};


$.alerta = function (message) {

    if (Array.isArray(message))
        $.dialog({ title: "Alerta", messageList: message });

    else
        $.dialog({ title: "Alerta", message: message });
};


$.fn.paginator = function (args) {

    var $this = $(this);

    $this.data(args);

    var $maxRows = $this.data("maxRowsInput");
    var $startRow = $this.data("startRowInput");

    var atualizarStartRow = function () {
        $startRow.val(($this.data("currentPage") - 1) * $maxRows.val() + 1)
    };

    if (!$this.data("configured")) {
        $this.data("configured", true);

        var $totalPages = $("<span>");
        var $prev = $("<button>");
        var $currentPage = $("<input>");
        var $totalRows = $("<span>");


        $totalPages.addClass("pagging_totalPages pagging_text");
        $totalRows.addClass("pagging_totalRows pagging_text");

        $prev.addClass("pagging_buttons btn btn-primary");

        var $next = $prev.clone();

        $prev
            .html("<")
            .addClass("prevPage");

        $next
            .html(">")
            .addClass("nextPage");

        $currentPage.attr({ type: "text" }).css({ width: "40px" });

        $currentPage.addClass("pagging_currentPage");

        if (!$this.data("currentPage"))
            $this.data("currentPage", 1);

        $this
            .append($("<span>", { text: "Pag.: ", "class": "pagging_text" }))
            .append($prev)
            .append("&nbsp;")
            .append($currentPage)
            .append($("<span>", { text: " de ", "class": "pagging_text" }))
            .append($totalPages)
            .append($next)
            .append($totalRows);

        var changePage = function (page) {

            if (page >= 1 && page <= $this.data("totalPages")) {

                $this.data("currentPage", page);

                atualizarStartRow();

                $this.data("pageChanged")();
            }

            $("input", $this).val($this.data("currentPage"));
        };

        $(".prevPage", $this).click(function () { changePage($this.data("currentPage") - 1) });
        $(".nextPage", $this).click(function () { changePage($this.data("currentPage") + 1) });
        $("input", $this).change(function () { changePage(parseInt($(this).val())) });
    }

    $this.data("totalPages", Math.ceil($this.data("totalRows") / $maxRows.val()));

    atualizarStartRow();

    $("input", $this).val($this.data("currentPage"));
    $("span.pagging_totalPages", $this).text($this.data("totalPages"));
    $("span.pagging_totalRows", $this).text(" Total de Registros: " + $this.data("totalRows"));

    $this.css("display", $this.data("totalRows") > 0 ? "block" : "none");
};


$.fn.ordenator = function (args) {

    var $tb = $(this);
    var $ths = $tb.find("th");

    $ths.each(function (index, item) {

        var $th = $(item);

        var sortBy = $th.data("sortField");


        if (sortBy) {

            $th.css({ cursor: "pointer", textDecoration: "underline" });


            $th.click(function () {

                if (args.sortByHidden.val().indexOf(sortBy + " ") != 0)
                    $tb.data("sortOrder", "ASC");

                else
                    $tb.data("sortOrder", ($tb.data("sortOrder") == "ASC" ? "DESC" : "ASC"));


                args.sortByHidden.val(sortBy + " " + $tb.data("sortOrder"));

                args.updateCallback();
            });
        }
    });
};


$.fn.defaultButton = function ($button) {

    return $("input", this).keydown(function (e) {

        if (e.keyCode === 13) {

            e.preventDefault();

            $button.click();
        }
    });
};


$.fn.originalValid = $.fn.valid;

$.fn.valid = function (p) {

    var valid = true;

    $(this).each(function (index, elem) {

        valid = valid & $(elem).originalValid(p);
    });

    return valid;
};



$.fn.loadTabOnOpen = function (handler, alwaysLoad) {

    var $tab = $(this);

    var $def = $.Deferred();

    $tab.click(function () {

        if (alwaysLoad || !$tab.data("loaded")) {
            $tab.data("loaded", true);

            var $tabContent = $($tab.children("a:first").attr("href")).hide();

            handler().done(function (html) {

                $tabContent.html(html);

                setTimeout(function () { $tabContent.fadeIn(200) }, 100);
            });
        }
    });

    return $def;
};


$.message = function (msg, buttons) {

    var $msg = $("<div>")
        .text(msg)
        .css({
            textAlign: "center",
            marginTop: "50px"
        });

    var $buttons = $("<div>").css({
        textAlign: "center",
        position: "absolute",
        bottom: "10px",
        right: "10px"
    });

    $.each(Object.keys(buttons), function (index, key) {

        $buttons.append($("<button>")
            .css({ minWidth: "100px" })
            .text(key)
            .click(function () {

                if (buttons[key])
                    buttons[key]();

                $buttons.closeDialog();
            }));
    });

    return $.dialog({
        elements: [$msg, $buttons],
        header: false,
        closeButton: false,
        width: "400px",
        height: "200px"
    });
};


function ValidarCPF(strCPF) {

    var Soma;
    var Resto;

    strCPF = strCPF.replace(/[.-]/g, "");


    Soma = 0;

    if (strCPF === "00000000000")
        return false;

    for (i = 1; i <= 9; i++)
        Soma = Soma + parseInt(strCPF.substring(i - 1, i)) * (11 - i);

    Resto = (Soma * 10) % 11;

    if (Resto == 10 || Resto == 11)
        Resto = 0;

    if (Resto != parseInt(strCPF.substring(9, 10)))
        return false;

    Soma = 0;

    for (i = 1; i <= 10; i++)
        Soma = Soma + parseInt(strCPF.substring(i - 1, i)) * (12 - i);

    Resto = (Soma * 10) % 11;

    if (Resto == 10 || Resto == 11)
        Resto = 0;

    if (Resto != parseInt(strCPF.substring(10, 11)))
        return false;

    return true;
}