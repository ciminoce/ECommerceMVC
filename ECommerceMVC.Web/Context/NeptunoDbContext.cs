using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using ECommerceMVC.Web.Models;

namespace ECommerceMVC.Web.Context
{
    public class NeptunoDbContext:DbContext
    {
        public NeptunoDbContext():base("DefaultConnection")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<NeptunoDbContext>(null);
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Entity<Pais>().ToTable("Paises");
            modelBuilder.Entity<Ciudad>().ToTable("Ciudades");
            modelBuilder.Entity<Categoria>().ToTable("Categorias");
            modelBuilder.Entity<Cliente>().ToTable("Clientes");
            modelBuilder.Entity<Proveedor>().ToTable("Proveedores");
            modelBuilder.Entity<Producto>().ToTable("Productos");
            modelBuilder.Entity<Venta>().ToTable("Ventas");
            modelBuilder.Entity<DetalleVenta>().ToTable("DetalleVentas");
            modelBuilder.Entity<Estado>().ToTable("Estados");
        }

        public DbSet<Pais> Paises { get; set; }
        public DbSet<Ciudad> Ciudades { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetalleVentas { get; set; }
        public DbSet<Estado> Estados { get; set; }
    }
}