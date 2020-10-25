using System;
using System.Collections.Generic;
using System.Linq;
using ECommerceMVC.Web.Context;
using ECommerceMVC.Web.Models;

namespace ECommerceMVC.Web.Classes
{
    public class CombosHelper:IDisposable
    {
        private static readonly NeptunoDbContext Db = new NeptunoDbContext();

        public static List<Pais> GetPaises()
        {
            var defaultPais = new Pais
            {
                PaisId = 0,
                NombrePais = "[Seleccione País]"
            };
            var listaPaises = Db.Paises.OrderBy(p => p.NombrePais).ToList();
            listaPaises.Insert(0,defaultPais);
            return listaPaises;
        }

        public static List<Ciudad> GetCiudades(int paisId=0)
        {
            var defaultCiudad = new Ciudad
            {
                CiudadId = 0,
                NombreCiudad = "[Seleccione Ciudad]"
            };
            var listaCiudades = Db.Ciudades
                .Where(c=>c.PaisId==paisId)
                .OrderBy(c => c.NombreCiudad).ToList();
            listaCiudades.Insert(0,defaultCiudad);
            return listaCiudades;
        }
        public void Dispose()
        {
            Db.Dispose();
        }

        public static List<Categoria> GetCategorias()
        {
            var defaultCategoria = new Categoria
            {
                CategoriaId = 0,
                NombreCategoria = "[Seleccione Categoría]"
            };
            var listaCategorias = Db.Categorias
                .OrderBy(c => c.NombreCategoria).ToList();
            listaCategorias.Insert(0, defaultCategoria);
            return listaCategorias;
        }

        public static List<Proveedor> GetProveedores()
        {
            var defaultProveedor = new Proveedor
            {
                ProveedorId = 0,
                NombreCompania = "[Seleccione Proveedor]"
            };
            var lista = Db.Proveedores
                .OrderBy(c => c.NombreCompania).ToList();
            lista.Insert(0, defaultProveedor);
            return lista;
        }
    }
}