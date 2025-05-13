using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SistemaLaboratorioEnmanuel.Models;

namespace SistemaLaboratorioEnmanuel.Util
{
    public static class ListUtil
    {
        //public static List<PerfilExamen> ExamenesPerfilTemp = new List<PerfilExamen>();
        public static List<int> ExamenesPerfilTemp = new List<int>();

        public static List<OrdenExamenes> listaExamenesOrden = new List<OrdenExamenes>();

        public static decimal MontoDescuento = 0;


        public class OrdenExamenes
        {
            public int PerfilExamenId { get; set; }
            public int PerfilId { get; set; }
            public decimal PrecioExamen { get; set;}
        }
    }
}