using System.ComponentModel.DataAnnotations;

namespace ECommerceMVC.Web.ViewModels.Pais
{
    public class PaisEditViewModel
    {
        public int PaisId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(100, ErrorMessage = "El campo {0} debe contener entre {2} and {1} caracteres", MinimumLength = 3)]
        [Display(Name = @"País")]
        public string NombrePais { get; set; }

    }
}