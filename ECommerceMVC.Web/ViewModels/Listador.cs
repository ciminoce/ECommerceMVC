using System.Collections.Generic;

namespace ECommerceMVC.Web.ViewModels
{
    public class Listador<T>:PaginadorGenerico where T:class
    {
        public IEnumerable<T> Registros { get; set; }

    }
}