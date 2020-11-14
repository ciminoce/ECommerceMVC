using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ECommerceMVC.Web.ViewModels.Producto;

namespace ECommerceMVC.Web.ViewModels.Categoria
{
    public class CategoriaDetailsViewModel
    {
        public int CategoriaId { get; set; }

        [Display(Name = @"Categoría")]
        public string NombreCategoria { get; set; }

        public List<ProductoListViewModel> listaProductos { get; set; }

    }
}