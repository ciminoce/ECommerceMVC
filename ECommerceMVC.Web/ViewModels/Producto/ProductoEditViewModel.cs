using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using ECommerceMVC.Web.Models;

namespace ECommerceMVC.Web.ViewModels.Producto
{
    public class ProductoEditViewModel
    {
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MaxLength(255, ErrorMessage = "El campo {0} debe contener no más de {1} caracteres")]
        [Display(Name = "Producto")]

        public string NombreProducto { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un proveedor")]
        [Display(Name = @"Proveedor")]
        public int ProveedorId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una categoría")]
        [Display(Name = @"Categoría")]

        public int CategoriaId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = @"Precio Vta.")]
        [Range(0, int.MaxValue, ErrorMessage = "El campo {0} puede tomar valores entre {1} y {2}")]
        public decimal PrecioUnitario { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = @"Stock")]
        [Range(0, 100, ErrorMessage = "El campo {0} puede tomar valores entre {1} y {2}")]
        public decimal UnidadesEnExistencia { get; set; }
        public bool Suspendido { get; set; }

        public List<Proveedor> Proveedores { get; set; }
        public List<Models.Categoria> Categorias { get; set; }

        [DataType(DataType.ImageUrl)]
        public string Foto { get; set; }

        [Display(Name = "Foto")]
        public HttpPostedFileBase FotoFile { get; set; }


    }
}