let vdataTable;
let dataTableIsInitialized = false;

const dataTableOptions = {

    lengthMenu: [5, 10, 15, 20, 100, 200, 500],
    columnDefs: [
        //{ className: "centered", targets: [0, 1, 2, 3, 4, 5, 6] },
        //{ orderable: false, targets: [4] },
        //{ searchable: false, targets: [1] }
        //{ width: "50%", targets: [0] }
    ],
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

const initDataTable =  () => {
    if (dataTableIsInitialized) {
        vdataTable.destroy();
    }

    vdataTable = $("#datatable_inventario").DataTable(dataTableOptions);

    dataTableIsInitialized = true;
};

window.addEventListener("load", () => {
    initDataTable();
});

function confirmarGuardarSalida() {
    const inputs = document.querySelectorAll("#tabla_InsumosSeleccionados input[type='number']");
    let isValid = true;

    inputs.forEach(input => {
        if (parseInt(input.value) <= 0) {
            alert("Las cantidades deben ser mayores a cero.");
            isValid = false;
            return;
        }
    });

    if (isValid) {
        if (confirm("¿Está seguro de registrar estas salidas de inventario?")) {
            document.getElementById("formGuardarSalida").submit();
        }
    }
}

function AgregarInsumo(id, nombre, stock) {
    // Evita duplicados
    if (document.getElementById("fila_" + id)) {
        alert("El insumo ya fue seleccionado.");
        return;
    }

    // Obtener el índice actual basado en la cantidad de filas existentes
    const currentIndex = document.querySelectorAll("#tabla_InsumosSeleccionados tr").length;

    // Agregar nueva fila a la tabla de insumos seleccionados
    let nuevaFila = `
        <tr id="fila_${id}">
            <td>
                ${nombre}
                <input type="hidden" name="InsumosSeleccionados[${currentIndex}].Id" value="${id}" />
                <input type="hidden" name="InsumosSeleccionados[${currentIndex}].Nombre" value="${nombre}" />
            </td>
            <td>
                ${stock}
                <input type="hidden" name="InsumosSeleccionados[${currentIndex}].Stock" value="${stock}" />
            </td>
            <td>
                <input type="number" name="InsumosSeleccionados[${currentIndex}].Cantidad" class="form-control" value="1" min="1" max="${stock}" required />
            </td>
            <td class="text-center">
                <button type="button" class="btn btn-danger" onclick="EliminarInsumo(${id})">
                    <span class="bi bi-trash"></span>
                </button>
            </td>
        </tr>`;
    document.getElementById("tabla_InsumosSeleccionados").insertAdjacentHTML("beforeend", nuevaFila);
}



function EliminarInsumo(id) {
    document.getElementById("fila_" + id).remove();
}


function prepareAndSubmitForm() {
    const inputs = document.querySelectorAll("#tabla_InsumosSeleccionados input[type='number']");
    let isValid = true;

    inputs.forEach(input => {
        if (parseInt(input.value) <= 0) {
            alert("Las cantidades deben ser mayores a cero.");
            isValid = false;
            return;
        }
    });

    if (isValid) {
        if (confirm("¿Está seguro de registrar estas salidas de inventario?")) {
            document.getElementById("formGuardarSalida").submit();
        }
    }
}

