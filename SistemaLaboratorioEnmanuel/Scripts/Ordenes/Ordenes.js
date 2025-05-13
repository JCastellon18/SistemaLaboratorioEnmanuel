
let vdataTable;
let dataTableIsInitialized = false;


const dataTableOptions = {

    lengthMenu: [10, 20, 30, 40, 50, 100, 200],
    columnDefs: [
        //{ className: "centered", targets: [0, 1, 2, 3, 4, 5, 6] },
        { orderable: false, targets: [5] },
        //{ searchable: false, targets: [1] }
        //{ width: "50%", targets: [0] }

        {
            targets: 3, // La columna de la fecha
            render: function (data, type, row) {
                // Solo cambiar el formato si el tipo es 'display'
                if (type === 'display' || type === 'filter') {
                    // Separar la fecha en partes (año, mes, día)
                    var parts = data.split('-');
                    var year = parseInt(parts[0], 10);
                    var month = parseInt(parts[1], 10) - 1; // El mes en JavaScript es 0-indexed
                    var day = parseInt(parts[2], 10);

                    // Crear la fecha sin el desfase de zona horaria
                    var date = new Date(year, month, day);

                    // Formatear la fecha
                    var formattedDate = ("0" + date.getDate()).slice(-2) + '/' +
                        ("0" + (date.getMonth() + 1)).slice(-2) + '/' +
                        date.getFullYear();
                    return formattedDate;
                }
                return data;
            }
        }
    ],

    order: [3, 'desc'],
    destroy: true,
    language: {
        lengthMenu: "Mostrar _MENU_ registros por página",
        zeroRecords: "Ningún usuario encontrado",
        info: "Mostrando de _START_ a _END_ de un total de _TOTAL_ registros",
        infoEmpty: "Ningún usuario encontrado",
        infoFiltered: "(filtrados desde _MAX_ registros totales)",
        search: "Buscar:",
        loadingRecords: "Cargando...",
        paginate: {
            first: "Primero",
            last: "Último",
            next: "Siguiente",
            previous: "Anterior"
        }
    }
};

const initDataTable = () => {
    if (dataTableIsInitialized) {
        vdataTable.destroy();
    }

    vdataTable = $("#datatable_ordenes").DataTable(dataTableOptions);

    dataTableIsInitialized = true;
};

window.addEventListener("load", () => {
    initDataTable();
});

function ListarExamenes(idO) {
    let url = "/Orden/ListarExamenOrden";
    let mapeado = { idOrden: idO };

    $.get(url, mapeado, function (data) {
        CargarContenidoExamen(data)
    });

}

function CargarContenidoExamen(datos) {
    var contenido = "";
    for (var i = 0; i < datos.length; i++) {
        contenido += "<div class='row justify-content-center mt-3'>";
        contenido += "<div class='col-6 text-center'>";
        contenido += "<div>" + datos[i].Id;
        contenido += "</div>";
        contenido += "</div>";
        contenido += "<div class='col-6 text-center'>" + datos[i].NombreTipoExamen;
        contenido += "</div>";
        contenido += "</div >";
    }
    document.getElementById("idOrdenModal").innerHTML = contenido;
}

function añadirTxtPaciente(idp) {
    $.get("./BuscPac/?id=" + idp, function (data) {
        document.getElementById("txtPaciente").value = data[0].Nombres + ' ' + data[0].Apellidos;
        document.getElementById("txtPacienteHidden").value = data[0].Id;
        $("#btnlimpiartxtPaciente").removeClass("btn-secondary");
        $("#btnlimpiartxtPaciente").addClass("btn-danger");
        $("#btnlimpiartxtPaciente").prop("disabled", false);
        document.getElementById("btnCancelarPaciente").click();
    });
}
function LimpiartxtPaciente() {
    $("#btnlimpiartxtPaciente").prop("disabled", true);
    $("#btnlimpiartxtPaciente").removeClass("btn-danger");
    $("#btnlimpiartxtPaciente").addClass("btn-secondary");
    document.getElementById("txtPaciente").value = ""
    document.getElementById("txtPacienteHidden").value = "";
}
function añadirTxtDoctor(id) {

    $.get("./BuscDoct/?id=" + id, function (data) {
        document.getElementById("txtDoctores").value = data[0].Nombres + ' ' + data[0].Apellidos;
        document.getElementById("txtDoctoresHidden").value = data[0].Id;
        $("#btnlimpiartxtDoctor").prop("disabled", false);
        $("#btnlimpiartxtDoctor").removeClass("btn-secondary");
        $("#btnlimpiartxtDoctor").addClass("btn-danger");
        document.getElementById("btnCancelarDoctor").click();
    });
}
function LimpiartxtDoctores() {
    $("#btnlimpiartxtDoctor").prop("disabled", true);
    $("#btnlimpiartxtDoctor").removeClass("btn-danger");
    $("#btnlimpiartxtDoctor").addClass("btn-secondary");
    document.getElementById("txtDoctores").value = ""
    document.getElementById("txtDoctoresHidden").value = "";
}

function ListarAreas() {
    $("#IdModalContenedorTabla").html("");

    $("#TituloModalContenedor").html("Areas");
    let url = "/Orden/ListaAreas";

    $.post(url).done(function (data) {
        $("#IdModalContenedorTabla").html(data)
    });
}
function AgregarArea(AreaId, NombreArea) {

    document.getElementById("IdAreaHidden").value = AreaId;
    document.getElementById("NombreArea").value = NombreArea;

    $("#btnLimpiarArea").removeClass("btn-secondary");
    $("#btnLimpiarArea").addClass("btn-danger");
    $("#btnLimpiarArea").prop("disabled", false);
    document.getElementById("btnCancelarContenedor").click();
    LimpiartxtPerfil();
}
function LimpiartxtArea() {
    $("#btnLimpiarArea").prop("disabled", true);
    $("#btnLimpiarArea").removeClass("btn-danger");
    $("#btnLimpiarArea").addClass("btn-secondary");
    document.getElementById("IdAreaHidden").value = ""
    document.getElementById("NombreArea").value = "";
    LimpiartxtPerfil();
}

function ListarPerfiles() {
    $("#IdModalContenedorTabla").html("");
    $("#TituloModalContenedor").html("Perfiles");


    let AreaId = $('#IdAreaHidden').val();
    let mapeo = { AreaId: AreaId };
    let url = "/Orden/ListaPerfiles";

    $.post(url, mapeo).done(function (data) {
        $("#IdModalContenedorTabla").html(data)
    });
}

function AgregarPerfil(PerfilId, NombrePerfil) {

    document.getElementById("IdPerfilHidden").value = PerfilId;
    document.getElementById("NombrePerfil").value = NombrePerfil;

    $("#btnLimpiarPerfil").removeClass("btn-secondary");
    $("#btnLimpiarPerfil").addClass("btn-danger");
    $("#btnLimpiarPerfil").prop("disabled", false);
    document.getElementById("btnCancelarContenedor").click();
    CargarExamenesPerfil(PerfilId);
}

function LimpiartxtPerfil() {
    $("#btnLimpiarPerfil").prop("disabled", true);
    $("#btnLimpiarPerfil").removeClass("btn-danger");
    $("#btnLimpiarPerfil").addClass("btn-secondary");
    document.getElementById("IdPerfilHidden").value = ""
    document.getElementById("NombrePerfil").value = "";
    $("#TablaListaExamenesPerfil").html("");
}

function CargarExamenesPerfil(PerfilId) {
    let url = "/Orden/ListaExamamenesPerfil";
    let mapeado = { PerfilId: PerfilId };
    $("#TablaListaExamenesPerfil").html("");

    $.post(url, mapeado).done(function (data) {
        $("#TablaListaExamenesPerfil").html(data)
    });
}


function CargarExamenOrden(PerfilExamenId, NombreExamen, PrecioExamen, PerfilId, NombrePerfil) {
    let url = "/Orden/AddExamenTemp";
    let mapeado = { PerfilExamenId: PerfilExamenId, PrecioExamen: PrecioExamen, PerfilId: PerfilId };


    $.post(url, mapeado).done(function (data) {
        if (data == 0) {

            let nuevaFila =
                "<tr id='" + PerfilId + "_" + PerfilExamenId + "'>" +
                "<td width='30%'>" + NombrePerfil + "</td>" +
                "<td width='40%'>" + NombreExamen + "</td>" +
                "<td width='20%'>" + PrecioExamen + "</td>" +
                "<td width='10%' class='text-center'><button onclick='EliminaExamenOrden(" + PerfilId + "," + PerfilExamenId + ")' class='btn btn-sm btn-danger'><i class='bi bi-x-circle-fill'></i></button></td>" +
                "</tr>";

            $("#TablaListaExamenesOrden").append(nuevaFila);
            sumarTotal();
        }
    });

}

function EliminaExamenOrden(PerfilId, PerfilExamenId) {
    let url = "/Orden/RemoveExamenTemp";

    let mapeado = { PerfilExamenId: PerfilExamenId, PerfilId: PerfilId };


    $.post(url, mapeado).done(function (data) {
        if (data == 1) {
            $("#" + PerfilId + "_" + PerfilExamenId).remove();
            sumarTotal();
        }
    });
}


function ActivaDesactivaItems() {
    let btnFinalizar = $('#FinalizarOrden').val().toUpperCase();

    if (btnFinalizar == "FINALIZAR") {

        if (ValidaItems() == false) {
            return;
        }

        $('#FinalizarOrden').val("Cancelar");
        $('#FinalizarOrden').removeClass("btn-primary");
        $('#FinalizarOrden').addClass("btn-danger");

        $('#GuardarOrden').removeClass("d-none");


        $("#IdModalContenedorTabla").html("");
        $("#TituloModalContenedor").html("Descuento");

        $("#txtTipoOrden").prop("disabled", true);

        $("#txtDescripcion").prop("disabled", true);

        $("#btnlimpiartxtDoctor").prop("disabled", true);
        $("#btnBuscarDoctor").prop("disabled", true);

        $("#btnlimpiartxtPaciente").prop("disabled", true);
        $("#btnBuscarPaciente").prop("disabled", true);

        $("#btnLimpiarArea").prop("disabled", true);
        $("#btnBuscarArea").prop("disabled", true);

        $("#btnLimpiarPerfil").prop("disabled", true);
        $("#btnBuscarPerfil").prop("disabled", true);

        //$("#btnLimpiarDescuento").prop("disabled", true);
        $("#btnAddDescuento").prop("disabled", false);

        $("#TablaListaExamenesPerfil").find("button").attr("disabled", "disabled");
        $("#TablaListaExamenesOrden").find("button").attr("disabled", "disabled");

    }
    else {
        $('#FinalizarOrden').val("Finalizar");
        $('#FinalizarOrden').removeClass("btn-danger");
        $('#FinalizarOrden').addClass("btn-primary");

        $('#GuardarOrden').addClass("d-none");

        $("#txtTipoOrden").prop("disabled", false);

        $("#txtDescripcion").prop("disabled", false);

        $("#IdModalContenedorTabla").html("");
        $("#TituloModalContenedor").html("");

        $("#btnlimpiartxtDoctor").prop("disabled", false);
        $("#btnBuscarDoctor").prop("disabled", false);

        $("#btnlimpiartxtPaciente").prop("disabled", false);
        $("#btnBuscarPaciente").prop("disabled", false);

        $("#btnLimpiarArea").prop("disabled", false);
        $("#btnBuscarArea").prop("disabled", false);

        $("#btnLimpiarPerfil").prop("disabled", false);
        $("#btnBuscarPerfil").prop("disabled", false);

        //$("#btnLimpiarDescuento").prop("disabled", true);
        $("#btnLimpiarDescuento").prop("disabled", true);
        $("#btnAddDescuento").prop("disabled", true);

        $("#TablaListaExamenesPerfil").find("button").removeAttr('disabled');
        $("#TablaListaExamenesOrden").find("button").removeAttr('disabled');


    }
}

function ValidaItems() {
    if ($("#txtTipoOrden").val() == "") {
        alert("Debe escoger el tipo de orden antes de continuar");
        return false;
    }

    if ($("#txtDescripcion").val() == "") {
        alert("Debe agregar una descripción antes de continuar");
        return false;
    }

    if ($("#txtDoctores").val() == "") {
        alert("Debe escoger el doctor de la orden antes de continuar");
        return false;
    }

    if ($("#txtPaciente").val() == "") {
        alert("Debe escoger el paciente antes de continuar");
        return false;
    }

    if (contarTotal() == 0) {
        alert("Debe escoger al menos un examen");
        return false;
    }

    return true;
}



function sumarTotal() {
    let url = "/Orden/GetSumTotalExamenes";

    $.post(url).done(function (data) {
        $("#PrecioTotal").val("C$ " + data.total);
        $("#SubTotal").val("C$ " + data.subTotal);


        $("#PrecioSubTotalHidden").val(data.subTotal);
        $("#IdPrecioTotalHidden").val(data.total);
    });
}


function contarTotal() {
    let resultado;

    $.ajax({
        url: '/Orden/GetCantTotalExamenes',
        type: 'POST',
        async: false,  // Esto hace que la solicitud sea sincrónica
        success: function (response) {
            resultado = response;
        },
        error: function (error) {
            console.log('Error:', error);
        }
    });

    return resultado;
}

function CargarDescuento() {

    let url = "/Orden/CargarDescuento";

    $.post(url).done(function (data) {
        $("#IdModalContenedorTabla").html(data);
    });
}


function ActualizarDescuento() {
    let Monto_descuento = parseFloat($("#MontoDescu").val());
    let Monto_total = parseFloat($("#PrecioSubTotalHidden").val());

    console.log("Monto desc: " + Monto_descuento + "\nMontoTotal: " + Monto_total);

    if (Monto_descuento >= Monto_total) {
        alert("No puede aplicar un descuento que supere o iguale el costo total de los exámenes");
    } else if (Monto_descuento < 0) {
        alert("No puede aplicar un descuento negativo");
    } else {
        $("#descuentoId").val("C$ " + Monto_descuento);
        $("#MontoDescuentoHidden").val(Monto_descuento);

        $('#txtTipoOrdenHidden').val($("#txtTipoOrden").val());

        $('#txtDescripcionHidden').val($("#txtDescripcion").val());

        let url = "/Orden/ActualizarDescuento";

        let mapeo = { nuevoMonto: Monto_descuento };

        $.post(url, mapeo).done(function (data) {
            sumarTotal();
        });

    }
}

