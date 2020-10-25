using System.ComponentModel.DataAnnotations;

namespace ECommerceMVC.Web.ViewModels.Pais
{
    public class PaisListViewModel
    {
        public int PaisId { get; set; }

        [Display(Name = @"País")]
        public string NombrePais { get; set; }

    }
}