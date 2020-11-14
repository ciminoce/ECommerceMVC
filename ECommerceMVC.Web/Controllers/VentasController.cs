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
using ECommerceMVC.Web.ViewModels.DetalleVenta;
using ECommerceMVC.Web.ViewModels.Venta;
using PagedList;

namespace ECommerceMVC.Web.Controllers
{
    public class VentasController : Controller
    {
        private readonly NeptunoDbContext _dbContext;

        public VentasController()
        {
            _dbContext = new NeptunoDbContext();
        }

        // GET: Ventas
        public ActionResult Index(int? page = null)
        {
            page = (page ?? 1);
            var ventas = _dbContext.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Estado)
                .OrderBy(v => v.FechaVenta)
                .ToList();

            var ventasVm = Mapper.Map<List<Venta>, List<VentaListViewModel>>(ventas);


            return View(ventasVm.ToPagedList((int)page, 10));
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Venta venta = _dbContext.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Estado)
                .SingleOrDefault(v => v.VentaId == id);
            if (venta == null)
            {
                return HttpNotFound("Código de Venta inexistente");
            }

            VentaDetailsViewModel ventaVm = Mapper.Map<Venta, VentaDetailsViewModel>(venta);
            var detalles = _dbContext.DetalleVentas
                .Include(dt => dt.Producto)
                .Where(dt => dt.VentaId == id).ToList();
            var detallesVm = Mapper
                .Map<List<DetalleVenta>, List<DetalleVentaListViewModel>>(detalles);
            ventaVm.Detalles = detallesVm;
            return View(ventaVm);
        }

        public ActionResult Create()
        {
            VentaEditViewModel ventaVm = new VentaEditViewModel
            {
                Clientes = CombosHelper.GetClientes()
            };
            List<DetalleVentaTmp> detallesTmp = _dbContext
                .DetalleVentasTmp.Include(dvt => dvt.Producto)
                .ToList();
            ventaVm.DetallesVenta = Mapper
                .Map<List<DetalleVentaTmp>, List<DetalleVentaListViewModel>>(detallesTmp);
            //return View(ventaVm);
            return View("Create2", ventaVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(VentaEditViewModel ventaVm)
        {
            if (!ModelState.IsValid)
            {
                ventaVm.Clientes = CombosHelper.GetClientes();
                List<DetalleVentaTmp> detallesTmp = _dbContext
                    .DetalleVentasTmp.Include(dvt => dvt.Producto)
                    .ToList();
                ventaVm.DetallesVenta = Mapper
                    .Map<List<DetalleVentaTmp>, List<DetalleVentaListViewModel>>(detallesTmp);

                //return View(ventaVm);
                return View("Create2", ventaVm);
            }

            var cantidadItems = _dbContext.DetalleVentasTmp.Count();
            if (cantidadItems == 0)
            {
                ModelState.AddModelError(string.Empty, "Debe contener al menos un item");
                ventaVm.Clientes = CombosHelper.GetClientes();
                List<DetalleVentaTmp> detallesTmp = _dbContext
                    .DetalleVentasTmp.Include(dvt => dvt.Producto)
                    .ToList();
                ventaVm.DetallesVenta = Mapper
                    .Map<List<DetalleVentaTmp>, List<DetalleVentaListViewModel>>(detallesTmp);

                //return View(ventaVm);
                return View("Create2", ventaVm);

            }

            Venta venta = Mapper.Map<VentaEditViewModel, Venta>(ventaVm);
            using (var tran = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    venta.EstadoId = CombosHelper.GetEstado("Facturado");
                    _dbContext.Ventas.Add(venta);
                    _dbContext.SaveChanges();

                    var detalles = _dbContext.DetalleVentasTmp.ToList();
                    foreach (var detalleVentaTmp in detalles)
                    {
                        //Creo el detalle venta
                        DetalleVenta detalle = Mapper
                            .Map<DetalleVentaTmp, DetalleVenta>(detalleVentaTmp);
                        detalle.VentaId = venta.VentaId; //le asigno el nro de venta
                        _dbContext.DetalleVentas.Add(detalle); //guardo el detalle
                        //Busco el producto para modificar su stock
                        var productoInDb = _dbContext
                            .Productos.SingleOrDefault(p => p.ProductoId == detalle.ProductoId);
                        productoInDb.UnidadesEnExistencia -= detalle.Cantidad;
                        _dbContext.Entry(productoInDb).State = EntityState.Modified; //cambio el estado de la entidad
                        _dbContext.DetalleVentasTmp.Remove(detalleVentaTmp); //borro el detalle temporal
                    }

                    _dbContext.SaveChanges();
                    tran.Commit();
                    //TempData["Msg"] = "Venta guardada";
                    //return RedirectToAction("Index");
                    ViewBag.VentaId = venta.VentaId;
                    return View("SuccessfullySave");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, "Error al intentar guardar la venta");
                    //return View(ventaVm);
                    return View("Create2", ventaVm);

                }
            }
        }

        public ActionResult AddProduct()
        {
            AddProductViewModel producto = new AddProductViewModel
            {
                Categorias = CombosHelper.GetCategorias(),
                Productos = CombosHelper.GetProductos(0)
            };
            return View(producto);
        }

        [HttpPost]
        public ActionResult AddProduct(AddProductViewModel addProduct)
        {
            if (!ModelState.IsValid)
            {
                addProduct.Categorias = CombosHelper.GetCategorias();
                addProduct.Productos = CombosHelper.GetProductos(addProduct.CategoriaId);
                return View(addProduct);
            }

            var pr = _dbContext.Productos
                .SingleOrDefault(p => p.ProductoId == addProduct.ProductoId);
            if (pr.UnidadesEnExistencia < addProduct.Cantidad)
            {
                ModelState.AddModelError(string.Empty, "Stock insuficiente");
                addProduct.Categorias = CombosHelper.GetCategorias();
                addProduct.Productos = CombosHelper.GetProductos(addProduct.CategoriaId);
                ViewBag.Stock = pr.UnidadesEnExistencia;
                return View(addProduct);
            }

            try
            {
                DetalleVentaTmp detalleTmp = _dbContext.DetalleVentasTmp
                    .SingleOrDefault(dvt => dvt.ProductoId == addProduct.ProductoId);
                if (detalleTmp == null)
                {
                    detalleTmp = new DetalleVentaTmp
                    {
                        ProductoId = addProduct.ProductoId,
                        PrecioUnitario = addProduct.PrecioUnitario,
                        Cantidad = addProduct.Cantidad
                    };
                    _dbContext.DetalleVentasTmp.Add(detalleTmp);

                }
                else
                {
                    detalleTmp.Cantidad += addProduct.Cantidad;
                    _dbContext.Entry(detalleTmp).State = EntityState.Modified;
                }

                _dbContext.SaveChanges();
                return RedirectToAction("Create");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error al intentar guardar un item de vta");
                addProduct.Categorias = CombosHelper.GetCategorias();
                addProduct.Productos = CombosHelper.GetProductos(addProduct.CategoriaId);
                ViewBag.Stock = pr.UnidadesEnExistencia;

                return View(addProduct);
            }
        }

        public JsonResult GetProducto(int productoSeleccionadoId)
        {
            _dbContext.Configuration.ProxyCreationEnabled = true;
            var producto = _dbContext.Productos
                .SingleOrDefault(p => p.ProductoId == productoSeleccionadoId);
            return Json(producto, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductos(int categoriaId)
        {
            _dbContext.Configuration.ProxyCreationEnabled = true;
            var productos = _dbContext.Productos
                .Where(p => p.CategoriaId == categoriaId).ToList();
            return Json(productos, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteProduct(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var item = _dbContext.DetalleVentasTmp
                .SingleOrDefault(dvt => dvt.DetalleVentaTmpId == id);
            if (item == null)
            {
                return HttpNotFound();
            }

            try
            {
                _dbContext.DetalleVentasTmp.Remove(item);
                _dbContext.SaveChanges();
                TempData["Msg"] = "Item borrado";
                return RedirectToAction("Create");
            }
            catch (Exception e)
            {
                TempData["Msg"] = "Error al intentar borrar un item del temporal";
                return RedirectToAction("Create");
            }
        }

        public ActionResult GeneratePdf(int id)
        {
            return new Rotativa.ActionAsPdf("GeneratePdfPreview", new { VentaId = id });
        }

        public ActionResult GeneratePdfPreview(int VentaId)
        {
            var venta = _dbContext.Ventas
                .Include(v => v.Cliente)
                .SingleOrDefault(v => v.VentaId == VentaId);
            var ventaVm = Mapper.Map<Venta, VentaDetailsViewModel>(venta);
            var detalles = _dbContext.DetalleVentas
                .Include(dv => dv.Producto)
                .Where(dv => dv.VentaId == VentaId).ToList();
            ventaVm.Detalles = Mapper
                .Map<List<DetalleVenta>, List<DetalleVentaListViewModel>>(detalles);

            return View(ventaVm);

        }

        public ActionResult Up(int id)
        {
            var detalleTmp = _dbContext.DetalleVentasTmp
                .SingleOrDefault(dvt => dvt.DetalleVentaTmpId == id);
            var producto = _dbContext.Productos
                .SingleOrDefault(p => p.ProductoId == detalleTmp.ProductoId);
            VentaEditViewModel ventaVm = new VentaEditViewModel
            {
                Clientes = CombosHelper.GetClientes()
            };
            if (producto.UnidadesEnExistencia < detalleTmp.Cantidad + 1)
            {

                var detalles = _dbContext.DetalleVentasTmp
                    .Include(dvt => dvt.Producto)
                    .ToList();
                ventaVm.DetallesVenta = Mapper
                    .Map<List<DetalleVentaTmp>, List<DetalleVentaListViewModel>>(detalles);
                ModelState.AddModelError(string.Empty, "Stock insuficiente");

                return RedirectToAction("Create", ventaVm);
            }

            try
            {
                detalleTmp.Cantidad++;
                _dbContext.Entry(detalleTmp).State = EntityState.Modified;
                _dbContext.SaveChanges();
                var detalles = _dbContext.DetalleVentasTmp
                    .Include(dvt => dvt.Producto)
                    .ToList();
                ventaVm.DetallesVenta = Mapper
                    .Map<List<DetalleVentaTmp>, List<DetalleVentaListViewModel>>(detalles);

                return RedirectToAction("Create");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error al intentar guardar un item de vta");
                var detalles = _dbContext.DetalleVentasTmp
                    .Include(dvt => dvt.Producto)
                    .ToList();
                ventaVm.DetallesVenta = Mapper
                    .Map<List<DetalleVentaTmp>, List<DetalleVentaListViewModel>>(detalles);

                return RedirectToAction("Create");

            }
        }

        public ActionResult Down(int id)
        {
            var detalleTmp = _dbContext.DetalleVentasTmp
                .SingleOrDefault(dvt => dvt.DetalleVentaTmpId == id);
            VentaEditViewModel ventaVm = new VentaEditViewModel
            {
                Clientes = CombosHelper.GetClientes()
            };
            if (detalleTmp.Cantidad - 1 < 0)
            {
                var detalles = _dbContext.DetalleVentasTmp
                    .Include(dvt => dvt.Producto)
                    .ToList();
                ventaVm.DetallesVenta = Mapper
                    .Map<List<DetalleVentaTmp>, List<DetalleVentaListViewModel>>(detalles);
                ModelState.AddModelError(string.Empty, "La cantidad ya está en 0");

                return RedirectToAction("Create", ventaVm);

            }

            try
            {
                detalleTmp.Cantidad--;
                _dbContext.Entry(detalleTmp).State = EntityState.Modified;
                _dbContext.SaveChanges();
                var detalles = _dbContext.DetalleVentasTmp
                    .Include(dvt => dvt.Producto)
                    .ToList();
                ventaVm.DetallesVenta = Mapper
                    .Map<List<DetalleVentaTmp>, List<DetalleVentaListViewModel>>(detalles);

                return RedirectToAction("Create");
            }
            catch (Exception ex)
            {
                var detalles = _dbContext.DetalleVentasTmp
                    .Include(dvt => dvt.Producto)
                    .ToList();
                ventaVm.DetallesVenta = Mapper
                    .Map<List<DetalleVentaTmp>, List<DetalleVentaListViewModel>>(detalles);
                ModelState.AddModelError(string.Empty, "Error al intentar guardar un item de vta");

                return RedirectToAction("Create");

            }


        }
    }
}