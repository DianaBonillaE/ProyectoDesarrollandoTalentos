//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProyectoIngenieria.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Activity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Activity()
        {
            this.Sponsor = new HashSet<Sponsor>();
            this.User = new HashSet<User>();
            this.Voluntary = new HashSet<Voluntary>();
        }
    
        public int id { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Nombre")]
        public string name { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Descripción")]
        public string descripcion { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha inicio")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime start_date { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha Finalización")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",ApplyFormatInEditMode =true)]
        public System.DateTime end_date { get; set; }

        [Display(Name = "Foto actual")]
        public int photo_id { get; set; }
    
        public virtual Photo Photo { get; set; }

       
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Sponsor> Sponsor { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<User> User { get; set; }
        public virtual ICollection<Voluntary> Voluntary { get; set; }
    }
}
