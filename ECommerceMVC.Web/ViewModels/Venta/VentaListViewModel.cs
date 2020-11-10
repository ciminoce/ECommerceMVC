using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerceMVC.Web.ViewModels.Venta
{
    public class VentaListViewModel
    {
        [Display(Name = "Vta. Nro.")]
        public int VentaId { get; set; }
        public string Cliente { get; set; }

        [Display(Name = "Fecha Vta.")]
        [DataType(DataType.Date)]
        public DateTime FechaVenta { get; set; }

        [Display(Name = "Fecha Entrega")]
        [DataType(DataType.Date)]
        public DateTime? FechaEntrega { get; set; }
        public string Estado { get; set; }

    }
}