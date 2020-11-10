using System;
using System.Collections.Generic;

namespace ECommerceMVC.Web.Models
{
    public class Venta
    {
        public int VentaId { get; set; }
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }
        public DateTime FechaVenta { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public int EstadoId { get; set; }
        public Estado Estado { get; set; }
        public List<DetalleVenta> DetalleVentas { get; set; } = new List<DetalleVenta>();

    }
}