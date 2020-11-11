using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ECommerceMVC.Web.ViewModels.DetalleVenta;

namespace ECommerceMVC.Web.ViewModels.Venta
{
    public class VentaEditViewModel
    {
        public int VentaId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un cliente")]
        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }

        [Display(Name = "Fecha Vta.")]
        public DateTime FechaVenta { get; set; }

        [Display(Name = "Fecha Entrega")]
        public DateTime? FechaEntrega { get; set; }

        public List<Models.Cliente> Clientes { get; set; }
        public List<DetalleVentaListViewModel> DetallesVenta { get; set; }=new List<DetalleVentaListViewModel>();
    }
}