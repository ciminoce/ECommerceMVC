using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;

namespace ECommerceMVC.Web.ViewModels.Cliente
{
    public class ClienteEditViewModel
    {
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(150, ErrorMessage = "El campo {0} debe contener entre {2} and {1} caracteres", MinimumLength = 3)]
        [Display(Name = @"Cliente")]
        public string NombreCliente { get; set; }

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


        [Required(ErrorMessage = "El campo {0} es requerido")]
        [DataType(DataType.EmailAddress)]
        [MaxLength(255, ErrorMessage = "El campo {0} debe contener no más de {1} caracteres")]
        [Display(Name = @"Mail")]
        public string Email { get; set; }

    }
}