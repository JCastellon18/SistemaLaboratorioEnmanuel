listar(); /*--> Al iniciar la vista se carga la tabla <--*/

/*--> Metodo para listar datos en tabla <--*/
function listar() {
    let url = "/Area/ListarAreas";
    $.post(url, function (data) {
        crearListado(data);
    });
}/*--> Fin del metodo <--*/

/*JS para la busqueda de areas*/
var btnBuscar = document.getElementById("btnBuscarArea");
btnBuscar.onclick = function () {
    var nombre = document.getElementById("txtNombre").value;
    $.get("Area/BuscarAreaNombre/?nombreArea=" + nombre, function (data) {
        crearListado(data);
    });
}/*Fin de función para la busqueda de areas*/

/*JS para limpiar las busquedas de areas*/
var btnLimpiar = document.getElementById("btnLimpiar");

btnLimpiar.onclick = function () {
    $.get("Area/ListarAreas", function (data) {
        crearListado(data);
    });
    document.getElementById("txtNombre").value = ""; /*--> Limpiar caja de texto de busqueda <--*/
}/*--> Fin del metodo para limpiar las busquedas <--*/

/*--> Metodo para cargar el listado en tabla <--*/
function crearListado(data) {
    var contenido = "";
    /*   var llaves = Object.keys(data[0]);*/
    var llaves = Object.keys(data[0]);
    var llaveId = llaves[0];
    contenido += "<table id='tabla-areas' class='table table-hover table-bordered mt-4'>";
    contenido += "<thead class = 'bg fw-bold text-center'>";
    contenido += "<tr>";
    contenido += "<th width='3%'>Id</th>";
    contenido += "<th width='20%'>Nombre Area</th>";
    contenido += "<th width='35%'>Descripción</th>";
    contenido += "<th width='5%'>Estado</th>";
    contenido += "<th width='15%'>Acciones</td>";
    contenido += "</tr>";
    contenido += "</thead>";
    contenido += "<tbody>";

    for (var i = 0; i < data.length; i++) {
        contenido += "<tr>";
        contenido += "<td class='text-center'>" + data[i].Id + "</td>";
        contenido += "<td>" + data[i].NombreArea + "</td>";
        contenido += "<td>" + data[i].Descripcion + "</td>";
        contenido += "<td class= 'text-center'>" + data[i].Estado + "</td>";
        contenido += "<td class= 'text-center'>"
        contenido += "<button class='btn btn-sm btn-personalizados bi bi-pencil-fill' onclick='abrirModalEditar(" + data[i][llaveId] + ")' data-bs-toggle='modal' data-bs-target='#editarModal'></button>";
        contenido += "<button class='btn btn-sm btn-danger bi bi-trash-fill ms-2' onclick='Eliminar(" + data[i][llaveId] + ")'></button>";

        contenido += "</td>";
        contenido += "</tr>";
    }
    contenido += "</tbody>";
    contenido += "</table>";
    document.getElementById("divTablaArea").innerHTML = contenido;
    $("#tabla-areas").DataTable(
        {
/*            searching: true,*/
            "language": {
                "url": "https://cdn.datatables.net/plug-ins/1.12.1/i18n/es-ES.json"
            }
        }
    );
}/*--> Fin del metodo para llenar tabla <--*/

/*--> Metodo para limpiar campos en los     es<--*/
function limpiarCampos() {
    var controles = document.getElementsByClassName("limpiar")
    var ncontroles = controles.length;
    for (var i = 0; i < ncontroles; i++) {
        controles[i].value = "";
    }
}/*--> Fin del metodo <--*/

function limpiarErrores() {
    var controlValidaciones = document.getElementsByClassName("obligatorio");
    var ncontroles = controlValidaciones.length;
    for (var i = 0; i < ncontroles; i++) {
        controlValidaciones[i].parentNode.classList.remove("error");
    }
    var controlValidacionesEdit = document.getElementsByClassName("obligatorioE");
    var ncontrolesEdit = controlValidacionesEdit.length;
    for (var i = 0; i < ncontrolesEdit; i++) {
        controlValidacionesEdit[i].parentNode.classList.remove("error");
    }
}
/*--> Metodo para abrir el modal para agregar <--*/
function abrirModal(id) {
    limpiarErrores();
    limpiarCampos();
} /*--> Fin del metodo <--*/

/*--> Metodo para abrir el modal para editar <--*/
function abrirModalEditar(id) {
    limpiarErrores();
    $.get("Area/RecuperarInformacion/?id=" + id, function (data) {
        document.getElementById("txtIdAreaEditar").value = data[0].Id;
        document.getElementById("txtNombreAreaEditar").value = data[0].NombreArea;
        document.getElementById("txtDescripcionAreaEditar").value = data[0].Descripcion;
    });
}/*--> Fin del metodo <--*/

/*--> Metodo para validar campos en el modal de agregar <--*/
function validaciones() {
    var exito = true;
    var controlValidaciones = document.getElementsByClassName("obligatorio");
    var ncontroles = controlValidaciones.length;

    for (var i = 0; i < ncontroles; i++) {
        if (controlValidaciones[i].value == "") {
            exito = false;
            controlValidaciones[i].parentNode.classList.add("error");
        }
        else {
            controlValidaciones[i].parentNode.classList.remove("error");
        }
    }
    return exito;
}/*--> Fin del metodo <--*/

/*--> Metodo para validar campos en el modal de editar <--*/
function validacionesEditar() {
    var exito = true;
    var controlValidaciones = document.getElementsByClassName("obligatorioE");
    var ncontroles = controlValidaciones.length;

    for (var i = 0; i < ncontroles; i++) {
        if (controlValidaciones[i].value == "") {
            exito = false;
            controlValidaciones[i].parentNode.classList.add("error");
        }
        else {
            controlValidaciones[i].parentNode.classList.remove("error");
        }
    }
    return exito;
}/*--> Fin del metodo <--*/

/*--> Metodo para agregar nuevo registro a la BD <--*/
function Agregar() {
    if (validaciones() == true) {
        var frm = new FormData();
        var nombre = document.getElementById("txtNombreArea").value;
        var desc = document.getElementById("txtDescripcionArea").value;
        var estado = 'A';
        frm.append("NombreArea", nombre);
        frm.append("Descripcion", desc);
        frm.append("Estado", estado);
        if (confirm("¿Desea realmente guardar los cambios?") == 1) {
            $.ajax({
                type: "Post",
                url: "Area/Create",
                data: frm,
                contentType: false,
                processData: false,
                success: function (data) {
                    if (data != false) {
                        listar();
                        alert("Registro añadido con exito")
                        document.getElementById("btnCancelarAgregar").click();
                    } else {
                        alert("Ocurrio un error al agregar")
                    }
                }
            })
        }
    }
    else {
        alert("Debe de rellenar todos los campos")
    }
}/*--> Fin del metodo <--*/

/*--> Metodo para editar un registro de la BD <--*/
function Editar() {
    if (validacionesEditar() == true) {
        var frm = new FormData();
        var id = document.getElementById("txtIdAreaEditar").value;
        var nombre = document.getElementById("txtNombreAreaEditar").value;
        var desc = document.getElementById("txtDescripcionAreaEditar").value;
        var estado = 'A';
        frm.append("Id", id);
        frm.append("NombreArea", nombre);
        frm.append("Descripcion", desc);
        frm.append("Estado", estado);
        if (confirm("¿Desea realmente guardar los cambios?") == 1) {
            $.ajax({
                type: "Post",
                url: "Area/Update",
                data: frm,
                contentType: false,
                processData: false,
                success: function (data) {
                    if (data != false) {
                        listar();
                        alert("Registro editado con exito")
                        document.getElementById("btnCancelarEditar").click();
                    } else {
                        alert("Ocurrio un error al editar")
                    }
                }
            })
        }
    }
    else {
        alert("Debe de rellenar todos los campos");
    }
}/*--> Fin del metodo <--*/

/*--> Metodo para deshabilitar un registro de la BD <--*/
function Eliminar(id) {
    Swal.fire({
        title: 'Deshabilitar',
        text: "¿Está realmente seguro de realizar la acción?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Confirmar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            var estado = "I";
            frm = new FormData();
            frm.append("Id", id);
            frm.append("Estado", estado);
            $.ajax({
                type: "Post",
                url: "Area/Delete",
                data: frm,
                contentType: false,
                processData: false,
                success: function (data) {
                    if (data != false) {
                        listar();
                        Swal.fire(
                            'Examen Deshabilitado',
                            'Cambios realizados correctamente',
                            'success'
                        )
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Error...',
                            text: 'Ha habido un problema al eliminar'
                        })
                    }
                }
            })
        }
    })
}/*--> Fin del metodo <--*/
function crearListadoModal() {
    var contenido = "";
    /*   var llaves = Object.keys(data[0]);*/
    //var llaves = Object.keys(data[0]);
    //var llaveId = llaves[0];

    contenido += "<table id='tabla' class='table table-hover table-bordered mt-4'>";
    contenido += "<thead class = 'bg fw-bold text-center'>";
    contenido += "<tr>";
    contenido += "<th>Id</th>";
    contenido += "<th>Columna 1</th>";

    contenido += "</tr>";
    contenido += "</thead>";
    contenido += "<tbody>";

    for (var i = 0; i < 7; i++) {
        contenido += "<tr>";
        contenido += "<td class='text-center'> Fila </td>";
        contenido += "<td class='text-center'> Fila </td>";
        contenido += "</tr>";
    }
    contenido += "</tbody>";
    contenido += "</table>";
    document.getElementById("divtablaModal").innerHTML = contenido;
    document.getElementById("divtablaModal2").innerHTML = contenido;
}/*--> Fin del metodo para llenar tabla <--*/
