//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SistemaLaboratorioEnmanuel.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class DetalleFactura
    {
        public int Id { get; set; }
        public decimal Monto { get; set; }
        public string Descripcion { get; set; }
        public Nullable<int> IdFactura { get; set; }
        public Nullable<int> PerfilExamenId { get; set; }
    
        public virtual PerfilExamen PerfilExamen { get; set; }
        public virtual Factura Factura { get; set; }
    }
}
