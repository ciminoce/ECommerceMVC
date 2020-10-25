using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AutoMapper;
using ECommerceMVC.Web.Classes;
using ECommerceMVC.Web.Context;
using ECommerceMVC.Web.Models;
using ECommerceMVC.Web.ViewModels;
using ECommerceMVC.Web.ViewModels.Producto;

namespace ECommerceMVC.Web.Controllers
{
    public class ProductosController : Controller
    {
        private readonly NeptunoDbContext _dbContext;
        private readonly int _registrosPorPagina = 10;
        private Listador<ProductoListViewModel> _listador;

        public ProductosController()
        {
            _dbContext = new NeptunoDbContext();
        }

        // GET: Productos
        public ActionResult Index(int pagina = 1)
        {
            int totalRegistros = _dbContext.Productos.Count();

            var productos = _dbContext.Productos
                .Include(c => c.Categoria)
                .OrderBy(p => p.NombreProducto)
                .Skip((pagina - 1) * _registrosPorPagina)
                .Take(_registrosPorPagina)
                .ToList();

            var productosVm = Mapper
                .Map<List<Producto>, List<ProductoListViewModel>>(productos);
            var totalPaginas = (int)Math.Ceiling((double)totalRegistros / _registrosPorPagina);
            _listador = new Listador<ProductoListViewModel>()
            {
                RegistrosPorPagina = _registrosPorPagina,
                TotalPaginas = totalPaginas,
                TotalRegistros = totalRegistros,
                PaginaActual = pagina,
                Registros = productosVm
            };


            return View(_listador);
        }

        // GET: Productos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Producto producto = _dbContext.Productos.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        // GET: Productos/Create
        public ActionResult Create()
        {
            var productoVm = new ProductoEditViewModel
            {
                Categorias = CombosHelper.GetCategorias(),
                Proveedores = CombosHelper.GetProveedores()
            };
            return View(productoVm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create(ProductoEditViewModel productoVm)
        {
            if (!ModelState.IsValid)
            {
                productoVm.Categorias = CombosHelper.GetCategorias();
                productoVm.Proveedores = CombosHelper.GetProveedores();

                return View(productoVm);
            }

            var producto = Mapper.Map<ProductoEditViewModel, Producto>(productoVm);

            if (!_dbContext.Productos.Any(ct => ct.NombreProducto == producto.NombreProducto 
                                                && ct.CategoriaId==producto.CategoriaId))
            {
                using (var tran = _dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var folder = "~/Content/Images/Productos/";
                        var file = "";
                        _dbContext.Productos.Add(producto);
                        _dbContext.SaveChanges();
                        if (productoVm.FotoFile != null)
                        {
                            file = $"{producto.ProductoId}.jpg";
                            var response = FileHelper.UploadPhoto(productoVm.FotoFile, folder, file);
                            if (!response)
                            {
                                file = "SinImagenDisponible.jpg";
                            }
                        }
                        else
                        {
                            file = "SinImagenDisponible.jpg";
                        }

                        producto.Foto = $"{folder}{file}";
                        _dbContext.Entry(producto).State = EntityState.Modified;
                        _dbContext.SaveChanges();
                        tran.Commit();
                        TempData["Msg"] = "Registro agregado";
                        return RedirectToAction("Index");
                    }
                    catch (Exception e)
                    {
                        productoVm.Categorias = CombosHelper.GetCategorias();
                        productoVm.Proveedores = CombosHelper.GetProveedores();

                        ModelState.AddModelError(string.Empty, e.Message);
                        return View(productoVm);
                    }
                }
            }
            productoVm.Categorias = CombosHelper.GetCategorias();
            productoVm.Proveedores = CombosHelper.GetProveedores();

            ModelState.AddModelError(string.Empty, "Registro repetido...");
            return View(productoVm);

        }

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var producto = _dbContext.Productos
                .Include(c => c.Categoria)
                .SingleOrDefault(ct => ct.ProductoId == id);
            if (producto == null)
            {
                return HttpNotFound();
            }

            var productoVm = Mapper.Map<Producto, ProductoListViewModel>(producto);
            return View(productoVm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            var producto = _dbContext.Productos
                .SingleOrDefault(ct => ct.ProductoId == id);
            try
            {
                _dbContext.Productos.Remove(producto);
                _dbContext.SaveChanges();
                TempData["Msg"] = "Registro eliminado";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                producto = _dbContext.Productos
                    .Include(c => c.Categoria)
                    .SingleOrDefault(ct => ct.ProductoId == id);

                var productoVm = Mapper
                    .Map<Producto, ProductoListViewModel>(producto);

                ModelState.AddModelError(string.Empty, "Error al intentar dar de baja un registro");
                return View(productoVm);
            }
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var producto = _dbContext.Productos
                .SingleOrDefault(ct => ct.ProductoId == id);
            if (producto == null)
            {
                return HttpNotFound();
            }

            ProductoEditViewModel productoVm = Mapper
                .Map<Producto, ProductoEditViewModel>(producto);
            productoVm.Categorias = CombosHelper.GetCategorias();
            productoVm.Proveedores = CombosHelper.GetProveedores();

            return View(productoVm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Edit(ProductoEditViewModel productoVm)
        {
            if (!ModelState.IsValid)
            {
                return View(productoVm);
            }

            var producto = Mapper.Map<ProductoEditViewModel, Producto>(productoVm);
            productoVm.Categorias = CombosHelper.GetCategorias();
            productoVm.Proveedores = CombosHelper.GetProveedores();

            try
            {
                if (_dbContext.Productos.Any(ct => ct.NombreProducto == producto.NombreProducto
                                                    && ct.ProductoId != producto.ProductoId))
                {
                    productoVm.Categorias = CombosHelper.GetCategorias();
                    productoVm.Proveedores = CombosHelper.GetProveedores();

                    ModelState.AddModelError(string.Empty, "Registro repetido");
                    return View(productoVm);
                }
                //TODO:Ver si existe como usuario, caso contrario darlo de alta
                //TODO:Ver si cambió el mail=>cambiar en la tabla de users.
                _dbContext.Entry(producto).State = EntityState.Modified;
                _dbContext.SaveChanges();
                TempData["Msg"] = "Registro editado";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                productoVm.Categorias = CombosHelper.GetCategorias();
                productoVm.Proveedores = CombosHelper.GetProveedores();

                ModelState.AddModelError(string.Empty, "Error inesperado al intentar editar un registro");
                return View(productoVm);
            }
        }

    }
}