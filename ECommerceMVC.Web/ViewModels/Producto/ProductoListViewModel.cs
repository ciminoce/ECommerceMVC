using System.ComponentModel.DataAnnotations;

namespace ECommerceMVC.Web.ViewModels.Producto
{
    public class ProductoListViewModel
    {
        public int ProductoId { get; set; }

        [Display(Name = "Producto")]
        public string NombreProducto { get; set; }
        public string Categoria { get; set; }

        [Display(Name = "Stock")]
        public decimal UnidadesEnExistencia { get; set; }

        [Display(Name = "Precio")]
        public decimal PrecioUnitario { get; set; }
        public bool Suspendido { get; set; }

        [DataType(DataType.ImageUrl)]
        public string Foto { get; set; }

    }
}