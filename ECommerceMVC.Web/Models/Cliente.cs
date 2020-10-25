namespace ECommerceMVC.Web.Models
{
    public class Cliente
    {
        public int ClienteId { get; set; }
        public string NombreCliente { get; set; }
        public string Direccion { get; set; }
        public string CodPostal { get; set; }
        public int PaisId { get; set; }
        public int CiudadId { get; set; }
        public Pais Pais { get; set; }
        public Ciudad Ciudad { get; set; }
        public string Email { get; set; }
    }
}