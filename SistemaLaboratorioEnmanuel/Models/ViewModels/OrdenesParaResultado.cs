using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaLaboratorioEnmanuel.Models.ViewModels
{
    public class OrdenesParaResultado
    {
        public int IdOrden { get; set; }
        public List<DetalleOrden> ExamenesOrden;
    }
}