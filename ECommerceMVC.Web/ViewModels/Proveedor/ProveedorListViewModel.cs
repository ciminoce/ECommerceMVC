using System.ComponentModel.DataAnnotations;

namespace ECommerceMVC.Web.ViewModels.Proveedor
{
    public class ProveedorListViewModel
    {
        public int ProveedorId { get; set; }

        [Display(Name = "Proveedor")]
        public string NombreCompania { get; set; }

        [Display(Name = "País")]
        public string Pais { get; set; }

        public string Ciudad { get; set; }

    }
}