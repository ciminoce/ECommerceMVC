using System.ComponentModel.DataAnnotations;

namespace ECommerceMVC.Web.ViewModels.Categoria
{
    public class CategoriaListViewModel
    {
        public int CategoriaId { get; set; }

        [Display(Name = @"Categoría")]
        public string NombreCategoria { get; set; }


    }
}