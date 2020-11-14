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
using ECommerceMVC.Web.ViewModels.Proveedor;

namespace ECommerceMVC.Web.Controllers
{
    public class ProveedoresController : Controller
    {
        private readonly NeptunoDbContext _dbContext;
        private readonly int _registrosPorPagina = 10;
        private Listador<ProveedorListViewModel> _listador;

        public ProveedoresController()
        {
            _dbContext = new NeptunoDbContext();
        }

        // GET: Proveedores
        public ActionResult Index(int pagina = 1)
        {
            int totalRegistros = _dbContext.Proveedores.Count();

            var proveedores = _dbContext.Proveedores
                .Include(c => c.Pais)
                .Include(c => c.Ciudad)
                .OrderBy(p => p.NombreCompania)
                .Skip((pagina - 1) * _registrosPorPagina)
                .Take(_registrosPorPagina)
                .ToList();

            var proveedorVm = Mapper
                .Map<List<Proveedor>, List<ProveedorListViewModel>>(proveedores);
            var totalPaginas = (int)Math.Ceiling((double)totalRegistros / _registrosPorPagina);
            _listador = new Listador<ProveedorListViewModel>()
            {
                RegistrosPorPagina = _registrosPorPagina,
                TotalPaginas = totalPaginas,
                TotalRegistros = totalRegistros,
                PaginaActual = pagina,
                Registros = proveedorVm
            };


            return View(_listador);
        }

        // GET: Proveedores/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proveedor proveedor = _dbContext.Proveedores.Find(id);
            if (proveedor == null)
            {
                return HttpNotFound();
            }
            return View(proveedor);
        }

        // GET: Proveedores/Create
        public ActionResult Create()
        {
            var proveedorVm = new ProveedorEditViewModel
            {
                Paises = CombosHelper.GetPaises(),
                Ciudades = CombosHelper.GetCiudades()
            };
            return View(proveedorVm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create(ProveedorEditViewModel proveedorVm)
        {
            if (!ModelState.IsValid)
            {
                proveedorVm.Paises = CombosHelper.GetPaises();
                proveedorVm.Ciudades = CombosHelper.GetCiudades(proveedorVm.PaisId);

                return View(proveedorVm);
            }

            var proveedor = Mapper.Map<ProveedorEditViewModel, Proveedor>(proveedorVm);

            if (!_dbContext.Proveedores.Any(ct => ct.NombreCompania == proveedor.NombreCompania))
            {
                _dbContext.Proveedores.Add(proveedor);
                _dbContext.SaveChanges();
                TempData["Msg"] = "Registro agregado";

                return RedirectToAction("Index");

            }
            proveedorVm.Paises = CombosHelper.GetPaises();
            proveedorVm.Ciudades = CombosHelper.GetCiudades(proveedorVm.PaisId);

            ModelState.AddModelError(string.Empty, "Registro repetido...");
            return View(proveedorVm);

        }

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var proveedor = _dbContext.Proveedores
                .Include(c => c.Pais)
                .Include(c => c.Ciudad)
                .SingleOrDefault(ct => ct.ProveedorId == id);
            if (proveedor == null)
            {
                return HttpNotFound();
            }

            var proveedorVm = Mapper.Map<Proveedor, ProveedorListViewModel>(proveedor);
            return View(proveedorVm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            var proveedor = _dbContext.Proveedores
                .SingleOrDefault(ct => ct.ProveedorId == id);
            try
            {
                _dbContext.Proveedores.Remove(proveedor);
                _dbContext.SaveChanges();
                TempData["Msg"] = "Registro eliminado";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                proveedor = _dbContext.Proveedores
                    .Include(c => c.Pais)
                    .Include(c => c.Ciudad)
                    .SingleOrDefault(ct => ct.ProveedorId == id);

                var proveedorVm = Mapper
                    .Map<Proveedor, ProveedorListViewModel>(proveedor);

                ModelState.AddModelError(string.Empty, "Error al intentar dar de baja un registro");
                return View(proveedorVm);
            }
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var proveedor = _dbContext.Proveedores
                .SingleOrDefault(ct => ct.ProveedorId == id);
            if (proveedor == null)
            {
                return HttpNotFound();
            }

            ProveedorEditViewModel proveedorVm = Mapper
                .Map<Proveedor, ProveedorEditViewModel>(proveedor);
            proveedorVm.Paises = CombosHelper.GetPaises();
            proveedorVm.Ciudades = CombosHelper.GetCiudades(proveedorVm.PaisId);

            return View(proveedorVm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Edit(ProveedorEditViewModel proveedorVm)
        {
            if (!ModelState.IsValid)
            {
                return View(proveedorVm);
            }

            var proveedor = Mapper.Map<ProveedorEditViewModel, Proveedor>(proveedorVm);
            proveedorVm.Paises = CombosHelper.GetPaises();
            proveedorVm.Ciudades = CombosHelper.GetCiudades(proveedorVm.PaisId);

            try
            {
                if (_dbContext.Proveedores.Any(ct => ct.NombreCompania == proveedor.NombreCompania
                                                    && ct.ProveedorId != proveedor.ProveedorId))
                {
                    proveedorVm.Paises = CombosHelper.GetPaises();
                    proveedorVm.Ciudades = CombosHelper.GetCiudades(proveedorVm.PaisId);

                    ModelState.AddModelError(string.Empty, "Registro repetido");
                    return View(proveedorVm);
                }
                _dbContext.Entry(proveedor).State = EntityState.Modified;
                _dbContext.SaveChanges();
                TempData["Msg"] = "Registro editado";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                proveedorVm.Paises = CombosHelper.GetPaises();
                proveedorVm.Ciudades = CombosHelper.GetCiudades(proveedorVm.PaisId);

                ModelState.AddModelError(string.Empty, "Error inesperado al intentar editar un registro");
                return View(proveedorVm);
            }
        }

        public JsonResult GetCiudades(int paisId)
        {
            _dbContext.Configuration.ProxyCreationEnabled = false;
            var ciudades = _dbContext.Ciudades.Where(c => c.PaisId == paisId).ToList();
            return Json(ciudades, JsonRequestBehavior.AllowGet);
        }
    }
}