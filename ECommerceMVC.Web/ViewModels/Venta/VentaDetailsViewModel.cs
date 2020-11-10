using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ECommerceMVC.Web.ViewModels.DetalleVenta;

namespace ECommerceMVC.Web.ViewModels.Venta
{
    public class VentaDetailsViewModel
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

        public List<DetalleVentaListViewModel> Detalles { get; set; }
    }
}