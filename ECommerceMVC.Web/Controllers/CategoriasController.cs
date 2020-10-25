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
using ECommerceMVC.Web.ViewModels.Categoria;

namespace ECommerceMVC.Web.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly NeptunoDbContext _dbContext;
        private readonly int _registrosPorPagina = 10;
        private Listador<CategoriaListViewModel> _listador;

        public CategoriasController()
        {
            _dbContext = new NeptunoDbContext();
        }

        // GET: Categorias
        public ActionResult Index(int pagina = 1)
        {
            int totalRegistros = _dbContext.Categorias.Count();

            var categorias = _dbContext.Categorias
                .OrderBy(p => p.NombreCategoria)
                .Skip((pagina - 1) * _registrosPorPagina)
                .Take(_registrosPorPagina)
                .ToList();
            var categoriasVm = Mapper
                .Map<List<Categoria>, List<CategoriaListViewModel>>(categorias);
            var totalPaginas = (int)Math.Ceiling((double)totalRegistros / _registrosPorPagina);
            _listador = new Listador<CategoriaListViewModel>()
            {
                RegistrosPorPagina = _registrosPorPagina,
                TotalPaginas = totalPaginas,
                TotalRegistros = totalRegistros,
                PaginaActual = pagina,
                Registros = categoriasVm
            };


            return View(_listador);
        }

        // GET: Categorias/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Categoria categoria = _dbContext.Categorias.Find(id);
            if (categoria == null)
            {
                return HttpNotFound();
            }
            return View(categoria);
        }

        // GET: Categorias/Create
        public ActionResult Create()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create(CategoriaEditViewModel categoriaVm)
        {
            if (!ModelState.IsValid)
            {
                return View(categoriaVm);
            }

            var categoria = Mapper.Map<CategoriaEditViewModel, Categoria>(categoriaVm);

            if (!_dbContext.Categorias.Any(ct => ct.NombreCategoria == categoriaVm.NombreCategoria))
            {
                _dbContext.Categorias.Add(categoria);
                _dbContext.SaveChanges();
                TempData["Msg"] = "Registro agregado";

                return RedirectToAction("Index");

            }

            ModelState.AddModelError(string.Empty, "Registro repetido...");
            return View(categoriaVm);

        }

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var categoria = _dbContext.Categorias
                .SingleOrDefault(ct => ct.CategoriaId == id);
            if (categoria == null)
            {
                return HttpNotFound();
            }

            var categoriaVm = Mapper.Map<Categoria, CategoriaListViewModel>(categoria);
            return View(categoriaVm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            var categoria = _dbContext.Categorias
                .SingleOrDefault(ct => ct.CategoriaId == id);
            try
            {
                _dbContext.Categorias.Remove(categoria);
                _dbContext.SaveChanges();
                TempData["Msg"] = "Registro eliminado";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                var categoriaVm = Mapper
                    .Map<Categoria, CategoriaListViewModel>(categoria);

                ModelState.AddModelError(string.Empty, "Error al intentar dar de baja un registro");
                return View(categoriaVm);
            }
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var categoria = _dbContext.Categorias
                .SingleOrDefault(ct => ct.CategoriaId == id);
            if (categoria == null)
            {
                return HttpNotFound();
            }

            CategoriaEditViewModel categoriaVm = Mapper.Map<Categoria, CategoriaEditViewModel>(categoria);
            return View(categoriaVm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Edit(CategoriaEditViewModel categoriaVm)
        {
            if (!ModelState.IsValid)
            {
                return View(categoriaVm);
            }

            var categoria = Mapper.Map<CategoriaEditViewModel, Categoria>(categoriaVm);
            try
            {
                if (_dbContext.Categorias.Any(ct => ct.NombreCategoria == categoria.NombreCategoria
                                                    && ct.CategoriaId != categoria.CategoriaId))
                {
                    ModelState.AddModelError(string.Empty, "Registro repetido");
                    return View(categoriaVm);
                }

                _dbContext.Entry(categoria).State = EntityState.Modified;
                _dbContext.SaveChanges();
                TempData["Msg"] = "Registro editado";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, "Error inesperado al intentar editar un registro");
                return View(categoriaVm);
            }
        }
    }
}