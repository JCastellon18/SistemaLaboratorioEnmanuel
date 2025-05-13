let vdataTable;
let dataTableIsInitialized = false;

const dataTableOptions = {

    lengthMenu: [10, 25, 50, 100],
    columnDefs: [
        //{ className: "centered", targets: [0, 1, 2, 3, 4, 5, 6] },
        { orderable: false, targets: [4] },
        //{ searchable: false, targets: [1] }
        //{ width: "50%", targets: [0] }
    ],
    order : [3,"desc"],
    destroy: true,
    language: {
        lengthMenu: "Mostrar _MENU_ registros",
        zeroRecords: "Ningún usuario encontrado",
        info: "Mostrando _START_ a _END_ de _TOTAL_ registros",
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

    vdataTable = $("#datatable_factura").DataTable(dataTableOptions);

    dataTableIsInitialized = true;
};

window.addEventListener("load", () => {
    initDataTable();
});

function ListarDetalles(idF) {

    let mapeado = { idfact: idF };
    let urlExamenes = "/Facturacion/ListarExamenes";
    let urlAbonos = "/Facturacion/ListarAbonos";

    $.get(urlAbonos, mapeado).done(function (data) {
        CargarContenidoAbono(data);
    });

    $.get(urlExamenes, mapeado).done(function (data) {
        CargarContenidoExamen(data);
    });
}

function CargarContenidoExamen(datos) {
    var contenido = "";
    for (var i = 0; i < datos.length; i++) {
        contenido += "<div  class='row justify - content - center'>";
        contenido += "<div class='col-8 text-center' >";
        contenido += "<span>" + datos[i].NombreTipoExamen + "</span>";
        contenido += "</div >";
        contenido += "<div class='col-4'>"
        contenido += "<span>" + datos[i].Monto + "</span>";
        contenido += "</div>";
        contenido += "</div >";
        contenido += "<div class='px-0 px-lg-3'>";
        contenido += "<hr />";
        contenido += "</div>";
    }
    document.getElementById("tab1").innerHTML = contenido;
}

function CargarContenidoAbono(datos) {
    var contenido = "";
    for (var i = 0; i < datos.length; i++) {
        contenido += "<div  class='row justify - content - center'>";
        contenido += "<div class='col-4 text-center col-lg-6' >";
        contenido += "<span>" + datos[i].Monto + "</span>";
        contenido += "</div >";
        contenido += "<div class='col-8 text-center col-lg-6'>"
        contenido += "<span>" + datos[i].FechaAbono + "</span>";
        contenido += "</div>";
        contenido += "</div >";
        contenido += "<div class='px-0 px-lg-3'>";
        contenido += "<hr />";
        contenido += "</div>";
    }
    document.getElementById("tab2").innerHTML = contenido;
}

