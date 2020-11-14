using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ECommerceMVC.Web.ViewModels.Proveedor
{
    public class ProveedorEditViewModel
    {
        public int ProveedorId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(150, ErrorMessage = "El campo {0} debe contener entre {2} and {1} caracteres", MinimumLength = 3)]
        [Display(Name = @"Proveedor")]
        public string NombreCompania { get; set; }

        [MaxLength(255, ErrorMessage = "El campo {0} debe contener no más de {1} caracteres")]
        [Display(Name = @"Dirección")]
        public string Direccion { get; set; }

        [MaxLength(20, ErrorMessage = "El campo {0} debe contener no más de {1} caracteres")]
        [Display(Name = @"Cod. Postal")]
        public string CodPostal { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un país")]
        [Display(Name = @"País")]
        public int PaisId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un país")]
        [Display(Name = @"Ciudad")]

        public int CiudadId { get; set; }
        public List<Models.Ciudad> Ciudades { get; set; }
        public List<Models.Pais> Paises { get; set; }


    }
}