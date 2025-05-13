using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaLaboratorioEnmanuel.Models.ViewModels
{
    public class RegistrarSalidaViewModel
    {
        public int IdOrden { get; set; }
        public int IdResultado { get; set; }
        public int IdDetalleOrden { get; set; }
        public string NombreResultado { get; set; }
        public List<sp_Insumos_Result> InsumosDisponibles { get; set; }
        public List<InsumoViewModel> InsumosSeleccionados { get; set; }
    }



    public class InsumoViewModel
    {
        public int Id { get; set; } // ID del insumo
        public string Nombre { get; set; } // Nombre del insumo
        public int Stock { get; set; } // Stock actual
        public int Cantidad { get; set; } // Cantidad a retirar (solo en seleccionados)
    }
}