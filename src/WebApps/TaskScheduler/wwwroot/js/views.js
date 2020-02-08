

$.ajaxSetup({ cache: false });


window.showProgressBar = function () {
    //Variavel para controlar o número de vezes que foi chamado para abrir a barra
    var progressCount = $.data(window, '_progressCount');

    if (!progressCount)
        progressCount = 0;


    //Caso não tenha sido chamada nenhuma vez, cria uma barra
    if (progressCount == 0) {

        var progTimeout = setTimeout(function () {

            var message = $.mensagem('<i style="color: white" class="fa fa-cog fa-spin fa-3x fa-fw margin-bottom"></i>', { milisegundos: 600000 });

            $.data(window, '_progressBar', message);

        }, 1000);

        $.data(window, '_progressBarTimeout', progTimeout);
    }

    $.data(window, '_progressCount', progressCount + 1);
}


window.closeProgressBar = function () {

    var progressCount = $.data(window, '_progressCount') - 1;
    var progressBar = $.data(window, '_progressBar');
    var progressBarTimeout = $.data(window, '_progressBarTimeout');

    if (progressCount == 0) {

        //Caso a progress bar tenha sido exibida, esconde ela e deleta ela do html
        if (progressBar)
            progressBar.fadeOut(150, function () { progressBar.remove() });


        //caso ela ainda não tenha sido mostrada, cancela o processo de criação
        if (progressBarTimeout)
            clearTimeout(progressBarTimeout);


        $.data(window, '_progressBar', null);
        $.data(window, '_progressBarTimeout', null);
    }


    $.data(window, '_progressCount', progressCount);
}


$(document).ajaxSend(showProgressBar);

$(document).ajaxComplete(closeProgressBar);

//$(document).ajaxSend(showProgressBar);

//$(document).ajaxComplete(closeProgressBar);



//Sobrescreve a função original do jQuery para não executar o handler de erro quando o código for 403 e 401
var originalAjax = $.ajax;

$.ajax = function (args) {


    if (args.error) {

        var originalError = args.error;

        args.error = function (xhr, status, errorThrown) {

            if (xhr.status != 403 && xhr.status != 401)
                return originalError(xhr, status, errorThrown);
        };
    }


    return originalAjax(args);
};



$(document).ajaxError(function (event, xhr, settings) {

    if (xhr.status === 403) {

        alert('Sessão expirada. Redirecionando para a tela de login...');

        window.location.href = xhr.getResponseHeader('Location');
    }
    else if (xhr.status == 401) {

        $.mensagemErro("Permissões insuficientes para realizar esta operação");
    }
    else
        $.mensagemErro("Ocorreu algum erro durante a execução do procedimento");
});


$.datepicker.setDefaults({
    dateFormat: 'dd/mm/yy',
    dayNames: ['Domingo', 'Segunda-feira', 'Terça-feira', 'Quarta-feira', 'Quinta-feira', 'Sexta-feira', 'Sábado'],
    dayNamesMin: ['Do', 'Se', 'Te', 'Qa', 'Qi', 'Se', 'Sa'],
    dayNamesShort: ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'Sab'],
    monthNames: ['Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho', 'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'],
    monthNamesShort: ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun', 'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez']
});


$.extend($.ui.dialog.prototype.options, {
    show: {
        effect: "fade",
        duration: 100
    },
    hide: {
        effect: "fade",
        duration: 100
    }
});


//--------------------------------------
//--------------------------------------
//Configuração do framework de validação
//--------------------------------------
//--------------------------------------
jQuery.extend(jQuery.validator.messages, {
    required: "Campo obrigatório",
    remote: "Please fix this field.",
    email: "Email inválido",
    url: "URL inválida",
    date: "Data inválida",
    dateISO: "Data inválida",
    number: "Use apenas números inteiros",
    decimal: "Número inválido",
    cpf: "Número de CPF inválido",
    digits: "Please enter only digits.",
    creditcard: "Please enter a valid credit card number.",
    equalTo: "Informe o mesmo valor novamente",
    accept: "Please enter a value with a valid extension.",
    maxlength: jQuery.validator.format("O máximo de caracteres permitido é {0}"),
    minlength: jQuery.validator.format("O mínimo de caracteres permitido é {0}"),
    rangelength: jQuery.validator.format("Informe um valor entre {0} e {1}"),
    range: jQuery.validator.format("Informe um valor entre {0} e {1}"),
    max: jQuery.validator.format("Informe um valor menor ou igual à {0}"),
    min: jQuery.validator.format("Informe um valor maior ou igual à {0}")
});



jQuery.validator.setDefaults({
    onkeyup: false,
    onfocusout: false,
    ignore: []
});


jQuery.validator.addMethod("date", function (value, element) {
    return (value ? moment(value, 'DD/MM/YYYY').isValid() : true);
});

jQuery.validator.addMethod("datetime", function (value, element) {
    return (value ? moment(value, 'DD/MM/YYYY hh:mi').isValid() : true);
});


jQuery.validator.addMethod("cpf", function (value, element) {
    return (value && value != "___.___.___-__" ? ValidarCPF(value) : true);
});


jQuery.validator.addMethod("decimal", function (value, element) {
    return this.optional(element)
        || /^-?(?:\d+|\d{1,3}(?:\.\d{3})+)(?:,\d+)?$/.test(value)
        || /^-?(?:\d+|\d{1,3}(?:,\d{3})+)(?:\.\d+)?$/.test(value);
});


jQuery.validator.addMethod(
    "DateGt",
    function (value, element, params) {

        if (
            value &&
            params.val() &&
            moment(value, 'DD/MM/YYYY').isValid() &&
            moment(params.val(), 'DD/MM/YYYY').isValid()) {

            return moment(value, 'DD/MM/YYYY').toDate() > moment(params.val(), 'DD/MM/YYYY').toDate();
        }

        return true;
    });


jQuery.validator.addMethod(
    "DateGtEq",
    function (value, element, params) {

        if (
            value &&
            params.val() &&
            moment(value, 'DD/MM/YYYY').isValid() &&
            moment(params.val(), 'DD/MM/YYYY').isValid()) {

            return moment(value, 'DD/MM/YYYY').toDate() >= moment(params.val(), 'DD/MM/YYYY').toDate();
        }

        return true;
    });


jQuery.validator.addClassRules({
    required: {
        required: true
    },
    date: {
        date: true
    },
    datetime: {
        datetime: true
    },
    number: {
        number: true,
        digits: false
    },
    decimal: {
        decimal: true
    },
    cpf: {
        cpf: true
    }
});


jQuery.validator.prototype.showErrors = function () {


    $.each(this.elements(), function (index, item) {
        $(item).removeClass("validationFail");
    });


    $.each(this.errorList, function (index, item) {

        var $elem = $(item.element);
        var $alerta = $("<div>");

        $elem.parent().append($alerta);

        $alerta.html(item.message);

        $alerta.addClass("validationMessage");
        $elem.addClass("validationFail");

        $alerta.css({
            width: "auto",
            position: "absolute",
            "z-index": "9999999"
        });

        $alerta.position({ of: $elem, my: "left top", at: "left bottom" });
        $alerta.hide();

        $alerta.showFor({ effect: "fade", callback: function () { $alerta.remove() } });
    });


    if (this.errorList.length > 0)
        $.mensagemErro("Foram encontrados erros de validação");
};




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
window.dialogs = {

    HttpJob: {
        Register: function (args) {
            return $.dialog({ action: actions.HttpJob.Register(args), title: "HTTP JOB", width: 800, height: 600 });
        }
    },

    SqlCommandJob: {
        Register: function (args) {
            return $.dialog({ action: actions.SqlCommandJob.Register(args), title: "SQL COMMAND JOB", width: 800, height: 600 });
        }
    },

    SqlMailReportJob: {
        Register: function (args) {
            return $.dialog({ action: actions.SqlMailReportJob.Register(args), title: "SQL MAIl REPORT JOB", width: 800, height: 600 });
        }
    },
};
window.views = (function (views) {

    views.Job = {

        Index: function () {

            var $my = $.myContext("#JobIndex");


            var $form = $my("form");


            var updateSearch = function () {

                var $def = $.Deferred();

                $my("input[name=SortBy]").val("Name ASC");

                if ($form.valid()) {

                    actions.Job.SearchCount($form.toJSON())
                        .done(function (rows) {

                            $my(".paginator").paginator({
                                currentPage: 1,
                                totalRows: parseInt(rows)
                            });

                            updateTable().done(function () { $def.resolve() });
                        }
                        );
                }

                return $def;
            };


            var updateTable = function () {
                return $my("tbody:first").ajaxLoad(actions.Job.SearchResult($form.toJSON()))
                    .done(function () {

                        $my("i[name=delete]").click(function () {

                            if (!$.confirmarExclusao())
                                return;

                            actions.Job.Delete({ id: $(this).parents("tr:first").data("id") })
                                .done(updateSearch);
                        });

                        $my("i[name=edit]").click(function () {

                            var $tr = $(this).parents("tr:first");
                            var jobType = $tr.data("jobType");

                            dialogs[jobType].Register({ id: $tr.data("id") })
                                .done(updateSearch);
                        });
                    });
            };


            $my(".paginator").paginator({
                startRowInput: $my("[name=StartRow]"),
                maxRowsInput: $my("[name=MaxRows]"),
                pageChanged: updateTable
            });


            $my("table:first").ordenator({
                sortByHidden: $my("input[name=SortBy]"),
                updateCallback: updateTable
            });


            $my("#search").click(function () {
                updateSearch().done(function () { $.mensagemConsultaAtualizada() });
            });


            $my("#addJob").click(function () { $my("#jobsToAdd").slideToggle(); });

            $my("#addSqlCommandJob").click(function () {
                dialogs.SqlCommandJob.Register().done(updateSearch);
            });

            $my("#addSqlMailReportJob").click(function () {
                dialogs.SqlMailReportJob.Register().done(updateSearch);
            });

            $my("#addHttpJob").click(function () {
                dialogs.HttpJob.Register().done(updateSearch);
            });

            $my("#toggleFilters").click(function () { $my("#filters").slideToggle(); });


            updateSearch();
        }
    };

    return views;

})(window.views || {});
window.views = (function (views) {

    views.SqlCommandJob = {

        Register: function () {

            var $my = $.myContext("#SqlCommandJobRegister");

            var $form = $my('form');

            $form.validate();


            $my('#save').click(function () {

                if ($form.valid()) {

                    actions.SqlCommandJob.Save($form.toJSON())
                        .done(function (result) {
                            if (result.Id) {
                                $.mensagemSalvo();
                                $form.closeDialog();
                            }
                        });
                }
            });


            $my('#cancel').click(function () { $form.closeDialog(); });
        }
    };

    return views;

})(window.views || {});