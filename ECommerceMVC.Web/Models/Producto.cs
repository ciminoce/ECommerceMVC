namespace ECommerceMVC.Web.Models
{
    public class Producto
    {
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; }
        public int ProveedorId { get; set; }
        public Proveedor Proveedor { get; set; }
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; }
        public decimal UnidadesEnExistencia { get; set; }
        public decimal PrecioUnitario { get; set; }
        public bool Suspendido { get; set; }
        public string Foto { get; set; }
    }
}