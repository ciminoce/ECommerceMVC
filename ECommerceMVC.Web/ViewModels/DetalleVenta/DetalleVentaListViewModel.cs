using System.ComponentModel.DataAnnotations;

namespace ECommerceMVC.Web.ViewModels.DetalleVenta
{
    public class DetalleVentaListViewModel
    {
        public int DetalleVentaId { get; set; }
        public string Producto { get; set; }

        [Display(Name = "Precio Unit.")]
        public decimal PrecioUnitario { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Total => PrecioUnitario * Cantidad;

    }
}