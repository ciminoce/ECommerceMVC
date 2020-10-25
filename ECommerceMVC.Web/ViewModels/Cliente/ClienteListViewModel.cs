using System.ComponentModel.DataAnnotations;

namespace ECommerceMVC.Web.ViewModels.Cliente
{
    public class ClienteListViewModel
    {
        public int ClienteId { get; set; }

        [Display(Name = "Cliente")]
        public string NombreCliente { get; set; }

        [Display(Name = "País")]
        public string Pais { get; set; }

        public string Ciudad { get; set; }

    }
}