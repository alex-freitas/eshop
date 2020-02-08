

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