listar();
cargarcombo();
cargarcomboEditar();
function listar() {
    let url = "/Examen/ListarExamenes";
    $.get(url, function (data) {
        crearListado(data);
    });
}

/*JS para la busqueda de Examenes*/
var btnBuscar = document.getElementById("btnBuscarExamen");
btnBuscar.onclick = function () {
    var nombre = document.getElementById("txtNombre").value;
    $.get("Examen/BuscarExamenNombre/?nombreExamen=" + nombre, function (data) {
        crearListado(data);
    });
}/*Fin de función para la busqueda de Examenes*/

/*JS para limpiar las busquedas de Examenes*/
var btnLimpiar = document.getElementById("btnLimpiar");
btnLimpiar.onclick = function () {
    $.get("Examen/ListarExamenes", function (data) {

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
    contenido += "<table id='tablas' class='table table-hover table-bordered mt-4'>";
    contenido += "<thead class = 'bg fw-bold text-center'>";
    contenido += "<tr>";
    contenido += "<th class='align-middle' width='3%'>Id</th>";
    contenido += "<th class='align-middle' width='15%'>Nombre Examen</th>";
    contenido += "<th class='align-middle' width='30%'>Descripcion</th>";
    contenido += "<th class='align-middle' width='15%'>Precio</th>";
    contenido += "<th class='align-middle' width='15%'>Area</th>";
    contenido += "<th class='align-middle' width='5%'>Estado</th>";
    contenido += "<th class='align-middle' width=15%'>Acciones</td>";
    contenido += "</tr>";
    contenido += "</thead>";
    contenido += "<tbody>";

    for (var i = 0; i < data.length; i++) {
        contenido += "<tr>";
        contenido += "<td class= 'text-center align-middle'>" + data[i].Id + "</td>";
        contenido += "<td class='align-middle'>" + data[i].NombreTipoExamen + "</td>";
        contenido += "<td class='align-middle'>" + data[i].Descripcion + "</td>";
        contenido += "<td class='align-middle'>" + 'C$' + data[i].Precio + "</td>";
        contenido += "<td class='align-middle'>" + data[i].NombreArea + "</td>";
        contenido += "<td class= 'text-center align-middle'>" + data[i].Estado + "</td>";
        contenido += "<td class= 'text-center align-middle'>"
        contenido += "<button class='btn btn-sm btn-personalizados bi bi-pencil-fill' onclick='abrirModalEditar(" + data[i][llaveId] + ")' data-bs-toggle='modal' data-bs-target='#EditarModal'></button>";
        contenido += "<button class='btn btn-sm btn-danger bi bi-trash-fill ms-2' onclick='Eliminar(" + data[i][llaveId] + ")'></button>";
        contenido += "</td>";
        contenido += "</tr>";
    }
    contenido += "</tbody>";
    contenido += "</table>";
    document.getElementById("divTablas").innerHTML = contenido;
    $("#tablas").DataTable(
        {
            /*            searching: true,*/
            "language": {
                "url": "https://cdn.datatables.net/plug-ins/1.12.1/i18n/es-ES.json"
            }
        }
    );
}/*--> Fin del metodo para llenar tabla <--*/

function cargarcombo() {
    $.get("Examen/ListarComboArea", function (data) {
        var contenido = "";
        contenido += "<option value = 0>";
        contenido += "--Seleccione--"
        contenido += "</option>"
        for (var i = 0; i < data.length; i++) {
            contenido += "<option value = '" + data[i].Id + "'>";
            contenido += data[i].NombreArea;
            contenido += "</option>";
        }
        document.getElementById("cboArea").innerHTML = contenido;
        /*        document.getElementById("cboAreaEditar").innerHTML = contenido;*/
    })
}
function cargarcomboEditar() {
    $.get("Examen/ListarComboArea", function (data) {
        var contenido = "";
        for (var i = 0; i < data.length; i++) {
            contenido += "<option value = '" + data[i].Id + "'>";
            contenido += data[i].NombreArea;
            contenido += "</option>";
        }
        /*        document.getElementById("cboArea").innerHTML = contenido;*/
        document.getElementById("cboAreaEditar").innerHTML = contenido;
    })
}
function limpiarCampos() {
    document.getElementById("txtIdExamen").value = "";
    document.getElementById("txtNombreExamen").value = "";
    document.getElementById("txtDescripcionExamen").value = "";
    document.getElementById("txtPrecioExamen").value = "";
    cargarcombo();
}
function limpiarErrores() {
    var controlValidaciones = document.getElementsByClassName("obligatorio");
    var ncontroles = controlValidaciones.length;
    var controlValidacionesEdit = document.getElementsByClassName("obligatorioE");
    var ncontrolesEdit = controlValidacionesEdit.length;

    for (var i = 0; i < ncontroles; i++) {
        controlValidaciones[i].parentNode.classList.remove("error");
    }
    for (var i = 0; i < ncontrolesEdit; i++) {
        controlValidacionesEdit[i].parentNode.classList.remove("error");
    }
}
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

function abrirModal(id) {
    limpiarErrores;
    limpiarCampos();
}
function abrirModalEditar(id) {
    limpiarErrores();
    $.get("Examen/RecuperarInformacion/?id=" + id, function (data) {
        document.getElementById("txtIdExamenEditar").value = data[0].Id;
        document.getElementById("txtNombreExamenEditar").value = data[0].NombreTipoExamen;
        document.getElementById("txtDescripcionExamenEditar").value = data[0].Descripcion;
        document.getElementById("txtPrecioExamenEditar").value = data[0].Precio;
        document.getElementById("cboAreaEditar").value = data[0].IdArea;
    });
}
function Agregar() {
    if (validaciones() == true) {
        var frm = new FormData();
        var id = document.getElementById("txtIdExamen").value;
        var nombreExamen = document.getElementById("txtNombreExamen").value;
        var desc = document.getElementById("txtDescripcionExamen").value;
        var prec = document.getElementById("txtPrecioExamen").value;
        var area = document.getElementById("cboArea").value;
        var estado = 'A';
        frm.append("Id", id);
        frm.append("NombreTipoExamen", nombreExamen);
        frm.append("Descripcion", desc);
        frm.append("Precio", prec);
        frm.append("Estado", estado);
        frm.append("IdArea", area);

        if (confirm("¿Desea realmente guardar los cambios?") == 1) {
            $.ajax({
                type: "Post",
                url: "Examen/Create",
                data: frm,
                contentType: false,
                processData: false,
                success: function (data) {
                    if (data != false) {
                        listar();
                        alert("Cambios realizados con exito");
                        document.getElementById("btnCancelar").click();
                    } else {
                        alert("Ocurrió un error al agregar");
                    }
                }
            })
        }
    }
    else {
        alert("Debe de rellenar todos los campos")
    }
}
function Editar() {
    /*    if (validaciones() == true) {*/
    var frm = new FormData();
    var id = document.getElementById("txtIdExamenEditar").value;
    var nombreExamen = document.getElementById("txtNombreExamenEditar").value;
    var desc = document.getElementById("txtDescripcionExamenEditar").value;
    var prec = document.getElementById("txtPrecioExamenEditar").value;
    var area = document.getElementById("cboAreaEditar").value;
    var estado = 'A';
    frm.append("Id", id);
    frm.append("NombreTipoExamen", nombreExamen);
    frm.append("Descripcion", desc);
    frm.append("Precio", prec);
    frm.append("Estado", estado);
    frm.append("IdArea", area);

    if (confirm("¿Desea realmente guardar los cambios?") == 1) {
        $.ajax({
            type: "Post",
            url: "Examen/Update",
            data: frm,
            contentType: false,
            processData: false,
            success: function (data) {
                if (data != false) {
                    listar();
                    alert("Cambios realizados con exito");
                    document.getElementById("btnCancelarEditar").click();
                } else {
                    alert("Ocurrió un error al agregar");
                }
            }
        })
    }
    //}
    //else {
    //    alert("Debe de rellenar todos los campos")
    //}
}
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
                url: "Examen/Delete",
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
}