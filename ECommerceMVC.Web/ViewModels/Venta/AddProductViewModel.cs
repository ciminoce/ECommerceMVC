using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ECommerceMVC.Web.ViewModels.Venta
{
    public class AddProductViewModel
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(1,int.MaxValue,ErrorMessage = "Debe seleccionar una categoría")]
        [Display(Name = "Categoría")]
        public int CategoriaId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(1,int.MaxValue,ErrorMessage = "Debe seleccionar un producto")]
        [Display(Name = "Producto")]
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(1,20,ErrorMessage = "Debe seleccionar un producto")]
        public decimal Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }
        public decimal Total => Cantidad * PrecioUnitario;
        public List<Models.Categoria> Categorias { get; set; }
        public List<Models.Producto> Productos { get; set; }
    }
}