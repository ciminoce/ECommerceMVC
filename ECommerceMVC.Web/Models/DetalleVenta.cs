namespace ECommerceMVC.Web.Models
{
    public class DetalleVenta
    {
        public int DetalleVentaId { get; set; }
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }

        public decimal PrecioUnitario { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Total => PrecioUnitario * Cantidad;

        public int VentaId { get; set; }
        public Venta Venta { get; set; }


    }
}