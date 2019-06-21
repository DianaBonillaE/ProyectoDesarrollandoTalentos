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

    public partial class Teacher
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Teacher()
        {
            this.Course = new HashSet<Course>();
        }

        [Required]
        [StringLength(20)]
        [Display(Name = "Identificación")]
        public string identification { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Nombre")]
        public string name { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Apellidos")]
        public string last_name { get; set; }
        [Required]
        [StringLength(150)]
        [Display(Name = "Dirección")]
        public string address { get; set; }
        [Required]
        [Display(Name = "Estado")]
        public bool state { get; set; }
        [Required]
        [StringLength(800)]
        [Display(Name = "Descripción")]
        public string description { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Correo")]
        public string email { get; set; }
        [Required]
        [Phone]
        [Display(Name = "Teléfono")]
        public string phone_number { get; set; }
        public int photo_id { get; set; }
        [StringLength(800)]
        [Display(Name = "Link a red social")]
        public string link { get; set; }
    
        public virtual Photo Photo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Course> Course { get; set; }
    }
}
