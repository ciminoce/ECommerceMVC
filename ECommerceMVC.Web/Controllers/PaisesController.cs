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
using ECommerceMVC.Web.ViewModels.Pais;

namespace ECommerceMVC.Web.Controllers
{
    public class PaisesController : Controller
    {
        private readonly NeptunoDbContext _dbContext;
        private readonly int _registrosPorPagina = 10;
        private Listador<PaisListViewModel> _listador;

        public PaisesController()
        {
            _dbContext=new NeptunoDbContext();
        }

        // GET: Paises
        public ActionResult Index(int pagina=1)
        {
            int totalRegistros = _dbContext.Paises.Count();
            
            var paises = _dbContext.Paises
                .OrderBy(p => p.NombrePais)
                .Skip((pagina - 1) * _registrosPorPagina)
                .Take(_registrosPorPagina)
                .ToList();
            var paisesVm = Mapper
                .Map<List<Pais>, List<PaisListViewModel>>(paises);
            var totalPaginas = (int)Math.Ceiling((double)totalRegistros / _registrosPorPagina);
            _listador = new Listador<PaisListViewModel>()
            {
                RegistrosPorPagina = _registrosPorPagina,
                TotalPaginas = totalPaginas,
                TotalRegistros = totalRegistros,
                PaginaActual = pagina,
                Registros = paisesVm
            };


            return View(_listador);
        }

        // GET: Paises/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pais pais = _dbContext.Paises.Find(id);
            if (pais == null)
            {
                return HttpNotFound();
            }
            return View(pais);
        }

        // GET: Paises/Create
        public ActionResult Create()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create(PaisEditViewModel paisVm)
        {
            if (!ModelState.IsValid)
            {
                return View(paisVm);
            }

            var pais = Mapper.Map<PaisEditViewModel, Pais>(paisVm);

            if (!_dbContext.Paises.Any(p => p.NombrePais == paisVm.NombrePais))
            {
                _dbContext.Paises.Add(pais);
                _dbContext.SaveChanges();
                TempData["Msg"] = "Registro agregado";

                return RedirectToAction("Index");

            }

            ModelState.AddModelError(string.Empty, "Registro repetido...");
            return View(paisVm);

        }

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var pais = _dbContext.Paises.SingleOrDefault(p => p.PaisId == id);
            if (pais == null)
            {
                return HttpNotFound();
            }

            var paisVm = Mapper.Map<Pais,PaisListViewModel>(pais);
            return View(paisVm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            var pais = _dbContext.Paises.SingleOrDefault(t => t.PaisId == id);
            try
            {
                _dbContext.Paises.Remove(pais);
                _dbContext.SaveChanges();
                TempData["Msg"] = "Registro eliminado";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                var paisVm = Mapper.Map<Pais,PaisListViewModel>(pais);

                ModelState.AddModelError(string.Empty, "Error al intentar dar de baja un registro");
                return View(paisVm);
            }
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var pais = _dbContext.Paises.SingleOrDefault(p => p.PaisId == id);
            if (pais == null)
            {
                return HttpNotFound();
            }

            PaisEditViewModel paisVm = Mapper.Map<Pais,PaisEditViewModel>(pais);
            return View(paisVm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Edit(PaisEditViewModel paisVm)
        {
            if (!ModelState.IsValid)
            {
                return View(paisVm);
            }

            var pais = Mapper.Map<PaisEditViewModel,Pais>(paisVm);
            try
            {
                if (_dbContext.Paises.Any(p => p.NombrePais == pais.NombrePais 
                                               && p.PaisId != pais.PaisId))
                {
                    ModelState.AddModelError(string.Empty, "Registro repetido");
                    return View(paisVm);
                }

                _dbContext.Entry(pais).State = EntityState.Modified;
                _dbContext.SaveChanges();
                TempData["Msg"] = "Registro editado";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, "Error inesperado al intentar editar un registro");
                return View(paisVm);
            }
        }
    }
}
