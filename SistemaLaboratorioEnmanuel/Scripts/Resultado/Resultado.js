function EditarResultado(IdResultado) {
    $("#IdModalContenedorTabla").html("");

    $("#TituloModalContenedor").html("Resultado");
    let url = "/Resultado/GetResultado";

    let mapeado = { IdResultado: IdResultado };
    $.post(url, mapeado).done(function (data) {
        $("#IdModalContenedorTabla").html(data)
    });
}


