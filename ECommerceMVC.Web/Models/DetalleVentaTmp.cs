namespace ECommerceMVC.Web.Models
{
    public class DetalleVentaTmp
    {
        public int DetalleVentaTmpId { get; set; }
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Cantidad { get; set; }

    }
}