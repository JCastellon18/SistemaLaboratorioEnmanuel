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
    
    public partial class PerfilExamen
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PerfilExamen()
        {
            this.DetalleFactura = new HashSet<DetalleFactura>();
            this.DetalleOrden = new HashSet<DetalleOrden>();
        }
    
        public int Id { get; set; }
        public int PerfilId { get; set; }
        public int ExamenId { get; set; }
    
        public virtual Examen Examen { get; set; }
        public virtual Perfil Perfil { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DetalleFactura> DetalleFactura { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DetalleOrden> DetalleOrden { get; set; }
    }
}
