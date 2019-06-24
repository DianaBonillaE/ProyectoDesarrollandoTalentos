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

    public partial class News
    {
        public int id { get; set; }

        [Required]
        [Display(Name = "Fecha de la noticia")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime date { get; set; }

        [Required]
        [Display(Name = "Descripción")]
        [StringLength(300)]
        public string description { get; set; }

        [Required]
        public int album_id { get; set; }
    
        public virtual Album Album { get; set; }
    }
}
