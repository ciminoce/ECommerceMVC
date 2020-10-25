namespace ECommerceMVC.Web.Models
{
    public class Proveedor
    {
        public int ProveedorId { get; set; }
        public string NombreCompania { get; set; }
        public string Direccion { get; set; }
        public string CodPostal { get; set; }
        public int PaisId { get; set; }
        public int CiudadId { get; set; }
        public Pais Pais { get; set; }
        public Ciudad Ciudad { get; set; }

    }
}