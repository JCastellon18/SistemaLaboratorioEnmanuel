using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaLaboratorioEnmanuel.Models.ViewModels
{
    public class RegistrarResultadoViewModel
    {
        public int IdDetalleOrden { get; set; }
        public int IdOrden { get; set; }
        public string PerfilNombre { get; set; }
        public List<ExamenResultadoViewModel> Examenes { get; set; }
        public List<ResultadoRegistradoViewModel> ResultadosRegistrados { get; set; }
    }
    public class ExamenResultadoViewModel
    {
        public int ExamenId { get; set; }
        public string NombreExamen { get; set; }
        public string Resultado { get; set; }
        public string ValoresNormales { get; set; }
        public string Unidad { get; set; }
        public string ValorReferencia { get; set; }
    }

    public class ResultadoRegistradoViewModel
    {
        public int Id { get; set; }
        public string NombreExamen { get; set; }
        public string Resultado { get; set; }
        public string ValoresNormales { get; set; }
        public string Unidad { get; set; }
        public string ValorReferencia { get; set; }
    }

}