listar();/*--> Al iniciar la vista se carga la tabla <--*/

/*--> Metodo para listar datos en tabla <--*/
function listar() {
    $.get("Empleado/ListarEmpleados", function (data) {
        crearListado(data);
    });
}/*--> Fin del metodo <--*/

/*JS para la busqueda de Empleados*/
var btnBuscar = document.getElementById("btnBuscarEmpleado");
btnBuscar.onclick = function () {
    var nombre = document.getElementById("txtNombre").value;
    $.get("Empleado/BuscarEmpleadoNombre/?nombreEmpleado=" + nombre, function (data) {
        crearListado(data);
    });
}/*Fin de función para la busqueda de areas*/

/*JS para limpiar las busquedas de areas*/
var btnLimpiar = document.getElementById("btnLimpiar");
btnLimpiar.onclick = function () {
    $.get("Empleado/ListarEmpleados", function (data) {

        crearListado(data);
    });
    document.getElementById("txtNombre").value = ""; /*--> Limpiar caja de texto de busqueda <--*/
}/*--> Fin del metodo para limpiar las busquedas <--*/

/*--> Metodo para limpiar campos en los modales<--*/
function limpiarCampos() {
    var controles = document.getElementsByClassName("limpiar")
    var ncontroles = controles.length;
    for (var i = 0; i < ncontroles; i++) {
        controles[i].value = "";
        document.getElementById("cboSexo").value = 0;
        document.getElementById("cboSexoEditar").value = 0;
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
/*--> Metodo para cargar el listado en tabla <--*/
function crearListado(data) {

    var contenido = "";
    var llaves = Object.keys(data[0]);
    var llaveId = llaves[0];
    contenido += "<table id='tablas' class='table table-hover table-striped table-bordered shadow-lg mt-4'>";
    contenido += "<thead class = 'fw-bold text-center'>";

    contenido += "<tr>";
    contenido += "<th width='3%'>Id</th>";
    contenido += "<th width='20%'>Nombres y Apellidos</th>";
    contenido += "<th width='10%'>No. Cedula</th>";
    contenido += "<th width='5%'>Sexo</th>";
    contenido += "<th width='5%'>Estado</th>";
    contenido += "<th width=10%'>Acciones</td>";
    contenido += "</tr>";

    contenido += "<tbody>";

    for (var i = 0; i < data.length; i++) {
        contenido += "<tr>";
        contenido += "<td class= 'text-center'>" + data[i].Id + "</td>";
        contenido += "<td>" + data[i].Nombres + " " + data[i].Apellidos + "</td>";
        contenido += "<td class= 'text-center'>" + data[i].Cedula + "</td>";
        contenido += "<td class= 'text-center'>" + data[i].Sexo + "</td>";
        contenido += "<td class= 'text-center'>" + data[i].Estado + "</td>";
        contenido += "<td class= 'text-center'>"
        contenido += "<button class='btn btn-sm btn-primary btn-personalizados bi bi-pencil-fill' onclick='abrirModalEditar(" + data[i][llaveId] + ")' data-bs-toggle='modal' data-bs-target='#EditarModal'></button>";
        contenido += "<button class='btn btn-sm btn-danger bi bi-trash-fill ms-2' onclick='Eliminar(" + data[i][llaveId] + ")'></button>";
        contenido += "</td>";
        contenido += "</tr>";
    }
    contenido += "</tbody>";
    contenido += "</table>";

    document.getElementById("divTablas").innerHTML = contenido;
    $("#tablas").dataTable(
        {
            searching: false,
            "language": {
                "url": "https://cdn.datatables.net/plug-ins/1.12.1/i18n/es-ES.json"
            }
        }
    );
}/*--> Fin del metodo <--*/

/*--> Metodo para abrir el modal para agregar <--*/
function abrirModalAgregar(id) {
    limpiarErrores();
    limpiarCampos();
}/*--> Fin del metodo <--*/

/*--> Metodo para abrir el modal para editar <--*/
function abrirModalEditar(id) {
    limpiarErrores();
    $.get("Empleado/RecuperarInformacion/?id=" + id, function (data) {
        document.getElementById("txtIdEmpleadoEditar").value = data[0].Id;
        document.getElementById("txtNombresEmpleadoEditar").value = data[0].Nombres;
        document.getElementById("txtApellidosEmpleadoEditar").value = data[0].Apellidos;
        document.getElementById("txtCedulaEditar").value = data[0].Cedula;
        document.getElementById("cboSexoEditar").value = data[0].Sexo;
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
        var id = document.getElementById("txtIdEmpleado").value;
        var nombres = document.getElementById("txtNombresEmpleado").value;
        var apellidos = document.getElementById("txtApellidosEmpleado").value;
        var cedula = document.getElementById("txtCedula").value;
        var sexo = document.getElementById("cboSexo").value;
        var estado = 'A';
        frm.append("Id", id);
        frm.append("Nombres", nombres);
        frm.append("Apellidos", apellidos);
        frm.append("Cedula", cedula);
        frm.append("Sexo", sexo);
        frm.append("Estado", estado);
        if (confirm("¿Desea realmente guardar los cambios?") == 1) {
            $.ajax({
                type: "Post",
                url: "Empleado/Create",
                data: frm,
                contentType: false,
                processData: false,
                success: function (data) {
                    if (data != false) {
                        listar();
                        alert("Cambios realizados con exito")
                        document.getElementById("btnCancelarAgregar").click();
                    } else {
                        alert("Ocurrio un error al agregar")
                    }
                }
            })
        }
    }
    else {
        alert("Debe de rellenar todos los campos");
    }
}/*--> Fin del metodo <--*/

/*--> Metodo para editar un registro a la BD <--*/
function Editar() {
    if (validacionesEditar() == true) {
        var frm = new FormData();
        var id = document.getElementById("txtIdEmpleadoEditar").value;
        var nombres = document.getElementById("txtNombresEmpleadoEditar").value;
        var apellidos = document.getElementById("txtApellidosEmpleadoEditar").value;
        var cedula = document.getElementById("txtCedulaEditar").value;
        var sexo = document.getElementById("cboSexoEditar").value;
        var estado = 'A';
        frm.append("Id", id);
        frm.append("Nombres", nombres);
        frm.append("Apellidos", apellidos);
        frm.append("Cedula", cedula);
        frm.append("Sexo", sexo);
        frm.append("Estado", estado);
        if (confirm("¿Desea realmente guardar los cambios?") == 1) {
            $.ajax({
                type: "Post",
                url: "Empleado/Update",
                data: frm,
                contentType: false,
                processData: false,
                success: function (data) {
                    if (data != false) {
                        listar();
                        alert("Cambios realizados con exito")
                        document.getElementById("btnCancelarEditar").click();
                    } else {
                        alert("Ocurrio un error al agregar")
                    }
                }
            })
        }
    }
    else {
        alert("Debe de rellenar todos los campos");
    }
}/*--> Fin del metodo <--*/

/*--> Metodo para deshabiliar un registro de la BD <--*/
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
                url: "Empleado/Delete",
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