using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AutoMapper;
using ECommerceMVC.Web.Context;
using ECommerceMVC.Web.Models;
using ECommerceMVC.Web.ViewModels;
using ECommerceMVC.Web.ViewModels.Ciudad;

namespace ECommerceMVC.Web.Controllers
{
    public class CiudadesController : Controller
    {
        private readonly NeptunoDbContext _dbContext;
        private readonly int _registrosPorPagina = 10;
        private Listador<CiudadListViewModel> _listador;

        public CiudadesController()
        {
            _dbContext = new NeptunoDbContext();
        }

        public ActionResult Index(int pagina = 1)
        {
            int totalRegistros = _dbContext.Ciudades.Count();

            var ciudades = _dbContext.Ciudades
                .Include(c=>c.Pais)
                .OrderBy(c => c.Pais.NombrePais)
                .ThenBy(c=>c.NombreCiudad)
                .Skip((pagina - 1) * _registrosPorPagina)
                .Take(_registrosPorPagina)
                .ToList();

            var ciudadVm = Mapper
                .Map<List<Ciudad>, List<CiudadListViewModel>>(ciudades);

            ciudadVm.ForEach(c =>
            {
                c.CantidadClientes = _dbContext
                    .Clientes.Count(cl => cl.CiudadId == c.CiudadId);
                c.CantidadProveedores = _dbContext
                    .Proveedores.Count(p => p.CiudadId == c.CiudadId);
            });

            var totalPaginas = (int)Math.Ceiling((double)totalRegistros / _registrosPorPagina);
            _listador = new Listador<CiudadListViewModel>()
            {
                RegistrosPorPagina = _registrosPorPagina,
                TotalPaginas = totalPaginas,
                TotalRegistros = totalRegistros,
                PaginaActual = pagina,
                Registros = ciudadVm
            };


            return View(_listador);
        }

        // GET: Ciudades/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ciudad ciudad = _dbContext.Ciudades.Find(id);
            if (ciudad == null)
            {
                return HttpNotFound();
            }
            return View(ciudad);
        }

        // GET: Ciudades/Create
        public ActionResult Create()
        {
            var ciudadVm = new CiudadEditViewModel
            {
                Paises = _dbContext.Paises
                    .OrderBy(p => p.NombrePais).ToList()
            };
            return View(ciudadVm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create(CiudadEditViewModel ciudadVm)
        {
            if (!ModelState.IsValid)
            {
                ciudadVm.Paises = _dbContext.Paises
                    .OrderBy(p => p.NombrePais).ToList();

                return View(ciudadVm);
            }

            var ciudad = Mapper.Map<CiudadEditViewModel, Ciudad>(ciudadVm);

            if (!_dbContext.Ciudades.Any(c =>c.NombreCiudad==ciudadVm.NombreCiudad 
                            && c.PaisId  != ciudadVm.PaisId))
            {
                _dbContext.Ciudades.Add(ciudad);
                _dbContext.SaveChanges();
                TempData["Msg"] = "Registro agregado";

                return RedirectToAction("Index");

            }

            ciudadVm.Paises = _dbContext.Paises
                    .OrderBy(p => p.NombrePais).ToList();

            ModelState.AddModelError(string.Empty, "Registro repetido...");
            return View(ciudadVm);

        }

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var ciudad = _dbContext.Ciudades.SingleOrDefault(t => t.PaisId == id);
            if (ciudad == null)
            {
                return HttpNotFound();
            }

            var ciudadVm = Mapper.Map<Ciudad, CiudadListViewModel>(ciudad);
            return View(ciudadVm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            var ciudad = _dbContext.Ciudades.SingleOrDefault(t => t.PaisId == id);
            try
            {
                _dbContext.Ciudades.Remove(ciudad);
                _dbContext.SaveChanges();
                TempData["Msg"] = "Registro eliminado";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                var ciudadVm = Mapper.Map<Ciudad, CiudadListViewModel>(ciudad);

                ModelState.AddModelError(string.Empty, "Error al intentar dar de baja un registro");
                return View(ciudadVm);
            }
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var ciudad = _dbContext.Ciudades.SingleOrDefault(t => t.PaisId == id);
            if (ciudad == null)
            {
                return HttpNotFound();
            }

            CiudadEditViewModel ciudadVm = Mapper
                .Map<Ciudad, CiudadEditViewModel>(ciudad);
            ciudadVm.Paises= _dbContext.Paises
                .OrderBy(p => p.NombrePais).ToList();
            return View(ciudadVm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Edit(CiudadEditViewModel ciudadVm)
        {
            if (!ModelState.IsValid)
            {
                ciudadVm.Paises = _dbContext.Paises
                    .OrderBy(p => p.NombrePais).ToList();

                return View(ciudadVm);
            }

            var ciudad = Mapper.Map<CiudadEditViewModel, Ciudad>(ciudadVm);
            try
            {
                if (!_dbContext.Ciudades.Any(c => c.NombreCiudad == ciudadVm.NombreCiudad
                                                  && c.PaisId != ciudadVm.PaisId))
                {
                    ciudadVm.Paises = _dbContext.Paises
                        .OrderBy(p => p.NombrePais).ToList();

                    ModelState.AddModelError(string.Empty, "Registro repetido");
                    return View(ciudadVm);
                }

                _dbContext.Entry(ciudad).State = EntityState.Modified;
                _dbContext.SaveChanges();
                TempData["Msg"] = "Registro editado";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ciudadVm.Paises = _dbContext.Paises
                    .OrderBy(p => p.NombrePais).ToList();

                ModelState.AddModelError(string.Empty, "Error inesperado al intentar editar un registro");
                return View(ciudadVm);
            }
        }
    }
}