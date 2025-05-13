using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SistemaLaboratorioEnmanuel.Models.ViewModels
{
    public class OrdenViewModel
    {
        //[Required]
        public DateTime FechaOrden { get; set; }
        //[Required]
        public string Descripcion { get; set; }
        //[Required]
        public string TipoOrden { get; set; }
        //[Required]
        public int IdDoctor { get; set; }
        //[Required]
        public int IdPaciente { get; set; }
        
    }

}