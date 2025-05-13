Limpiar();

const defaultFile = document.getElementById('imagen').src;
const file = document.getElementById('foto');
const img = document.getElementById('imagen');

file.addEventListener('change', e => {
    if (e.target.files[0]) {
        const reader = new FileReader();
        reader.onload = function (e) {
            img.src = e.target.result;

            $("#btnLimpiarLogo").removeClass('btn-secondary');
            $("#btnLimpiarLogo").addClass('btn-danger');
            $("#btnLimpiarLogo").prop('disabled', false);
        }
        reader.readAsDataURL(e.target.files[0])
    } else {
        img.src = defaultFile;
    }
});
function Limpiar() {
    let file = document.getElementById('foto').files.length;
    if (file > 0) {
        document.getElementById('foto').value = '';
    }
}
function LimpiarLogo() {
    $("#btnLimpiarLogo").prop('disabled', true);
    $("#btnLimpiarLogo").removeClass('btn-danger');
    $("#btnLimpiarLogo").addClass('btn-secondary');
    document.getElementById('foto').value = '';
    document.getElementById('imagen').src = defaultFile;
}
function Validaciones() {
    let tamañoImagen = document.getElementById('foto').files[0].size; //Obtiene el tamaño de la imagen subida

    if (tamañoImagen > 1048576) {
        swal.fire({
            icon: "error",
            title: "Imagen Muy Grande",
            text: "La imagen no puede ser mayor a 1MB",
            confirmButtonText: 'Confirmar',
            confirmButtonColor: '#3085d6'
        }).then((result) => {
            if (result.isConfirmed) {
                LimpiarLogo();
            }
        });

    }
}