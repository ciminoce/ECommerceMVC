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
            _dbContext=new NeptunoDbContext();
        }
        // GET: Ventas
        public ActionResult Index(int? page=null)
        {
            page = (page ?? 1);
            var ventas = _dbContext.Ventas
                .Include(v=>v.Cliente)
                .Include(v=>v.Estado)
                .OrderBy(v=>v.FechaVenta)
                .ToList();

            var ventasVm = Mapper.Map<List<Venta>, List<VentaListViewModel>>(ventas);


            return View(ventasVm.ToPagedList((int)page,10));
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
                .Include(dt=>dt.Producto)
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
            return View(ventaVm);
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

        public JsonResult GetProductos(int categoriaId)
        {
            _dbContext.Configuration.ProxyCreationEnabled = true;
            var productos = _dbContext.Productos
                .Where(p => p.CategoriaId == categoriaId).ToList();
            return Json(productos, JsonRequestBehavior.AllowGet);
        }
    }
}