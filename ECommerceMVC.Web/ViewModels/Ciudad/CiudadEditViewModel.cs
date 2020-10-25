using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ECommerceMVC.Web.ViewModels.Ciudad
{
    public class CiudadEditViewModel
    {
        public int CiudadId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(100, ErrorMessage = "El campo {0} debe contener entre {2} and {1} caracteres", MinimumLength = 3)]
        [Display(Name = @"Ciudad")]

        public string NombreCiudad { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un país")]
        [Display(Name = @"País")]
        public int PaisId { get; set; }
        public List<Models.Pais> Paises { get; set; }

    }
}