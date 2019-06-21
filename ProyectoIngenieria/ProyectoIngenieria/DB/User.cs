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

    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            this.Activity = new HashSet<Activity>();
        }


        [Required]
        [StringLength(50)]
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
        [StringLength(50)]
        [Display(Name = "Correo")]
        public string email { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Número de Teléfono")]
        public string phone_number { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Nombre de Usuario")]
        public string user_name { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(50)]
        [Display(Name = "Contraseña")]
        public string password { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Dirección")]
        public string address { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Descripción")]
        public string description { get; set; }
        public int photo_id { get; set; }
    
        [StringLength(50)]
        [Display(Name = "Red Social")]
        public string link { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Activity> Activity { get; set; }
        public virtual Photo Photo { get; set; }
    }
}
