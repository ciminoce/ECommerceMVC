using System.ComponentModel.DataAnnotations;

namespace ECommerceMVC.Web.ViewModels.Ciudad
{
    public class CiudadListViewModel
    {
        public int CiudadId { get; set; }

        [Display(Name = @"Ciudad")]

        public string NombreCiudad { get; set; }

        [Display(Name = @"País")]
        public string NombrePais { get; set; }

    }
}