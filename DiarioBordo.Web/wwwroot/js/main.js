$(document).ready(function () {
    // Adiciona um zero a esquerda, caso seja menor que 10.
    function zeroLeft(num) {
        if (num < 10)
            return "0" + num.toString();

        return num.toString();
    }

    // Inicialização de tooltips.
    $('[data-toggle="tooltip"]').tooltip({placement: 'bottom'});

    // Calendário 
    $('#calendario').datepicker({
        language: 'pt-BR'
    })
    .on('changeDate', function (e) {
        var dt = e.date;
        var dia = dt.getDate();
        var mes = dt.getMonth() + 1;
        var ano = dt.getFullYear();

        var data = ano.toString() + zeroLeft(mes) + zeroLeft(dia);
        window.location = "/registros/data/" + data;
    });

    // Auto-completar para tags
    if ($('#tag').length > 0) {
        $('#tag').autocomplete({
            minLength: 3,
            focus: function (event, ui) {
                if (ui.item) {
                    $('#tag').val(ui.item.label);
                }
                return false;
            },
            select: function (event, ui) {
                if (ui.item) {
                    console.log(ui.item.value + " => " + ui.item.label);
                    $('#idTag').val(ui.item.value);
                    $('#tag').val(ui.item.label);
                }
                return false;
            },
            source: function (request, response) {
                $.ajax({
                    url: "/tags/",
                    type: "POST",
                    dataType: "json",
                    data: { term: request.term },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return { label: item.nome, value: item.id };
                        }));
                    }
                })
            },
            messages: {
                noResults: "", results: function () { }
            }
        });
    }
});