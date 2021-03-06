﻿using System;
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
using ECommerceMVC.Web.ViewModels.Cliente;

namespace ECommerceMVC.Web.Controllers
{
    public class ClientesController : Controller
    {
        private readonly NeptunoDbContext _dbContext;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly int _registrosPorPagina = 10;
        private Listador<ClienteListViewModel> _listador;
        private string emailAnterior;
        public ClientesController()
        {
            _dbContext = new NeptunoDbContext();
            _applicationDbContext=new ApplicationDbContext();
        }

        // GET: Clientes
        public ActionResult Index(int pagina = 1)
        {
            Session["emailAnterior"] = null;
            int totalRegistros = _dbContext.Clientes.Count();

            var clientes = _dbContext.Clientes
                .Include(c=>c.Pais)
                .Include(c=>c.Ciudad)
                .OrderBy(p => p.NombreCliente)
                .Skip((pagina - 1) * _registrosPorPagina)
                .Take(_registrosPorPagina)
                .ToList();
            
            var clientesVm = Mapper
                .Map<List<Cliente>, List<ClienteListViewModel>>(clientes);
            var totalPaginas = (int)Math.Ceiling((double)totalRegistros / _registrosPorPagina);
            _listador = new Listador<ClienteListViewModel>()
            {
                RegistrosPorPagina = _registrosPorPagina,
                TotalPaginas = totalPaginas,
                TotalRegistros = totalRegistros,
                PaginaActual = pagina,
                Registros = clientesVm
            };


            return View(_listador);
        }

        // GET: Clientes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = _dbContext.Clientes.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        // GET: Clientes/Create
        public ActionResult Create()
        {
            var clienteVm = new ClienteEditViewModel
            {
                Paises = CombosHelper.GetPaises(),
                Ciudades = CombosHelper.GetCiudades()
            };
            return View(clienteVm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create(ClienteEditViewModel clienteVm)
        {
            if (!ModelState.IsValid)
            {
                clienteVm.Paises = CombosHelper.GetPaises();
                clienteVm.Ciudades = CombosHelper.GetCiudades(clienteVm.PaisId);

                return View(clienteVm);
            }

            var cliente = Mapper.Map<ClienteEditViewModel, Cliente>(clienteVm);

            if (_dbContext.Clientes.Any(ct => ct.NombreCliente == cliente.NombreCliente ||
                                               ct.Email==cliente.Email))
            {
                clienteVm.Paises = CombosHelper.GetPaises();
                clienteVm.Ciudades = CombosHelper.GetCiudades(clienteVm.PaisId);

                ModelState.AddModelError(string.Empty, "Registro repetido...");
                return View(clienteVm);
                
            }

            using (var tran=_dbContext.Database.BeginTransaction())
            {
                try
                {
                    _dbContext.Clientes.Add(cliente);
                    _dbContext.SaveChanges();

                    UsersHelper.CreateUserAsp(cliente.Email,"Cliente");
                    tran.Commit();
                    TempData["Msg"] = "Registro agregado";

                    return RedirectToAction("Index");

                }
                catch (Exception)
                {
                    tran.Rollback();
                    clienteVm.Paises = CombosHelper.GetPaises();
                    clienteVm.Ciudades = CombosHelper.GetCiudades(clienteVm.PaisId);

                    ModelState.AddModelError(string.Empty, "Registro repetido...");
                    return View(clienteVm);
                }
            }


        }

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var cliente = _dbContext.Clientes
                .Include(c=>c.Pais)
                .Include(c=>c.Ciudad)
                .SingleOrDefault(ct => ct.ClienteId == id);
            if (cliente == null)
            {
                return HttpNotFound();
            }

            var clienteVm = Mapper.Map<Cliente, ClienteListViewModel>(cliente);
            return View(clienteVm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            var cliente = _dbContext.Clientes
                .SingleOrDefault(ct => ct.ClienteId == id);
            try
            {
                _dbContext.Clientes.Remove(cliente);
                _dbContext.SaveChanges();
                TempData["Msg"] = "Registro eliminado";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                cliente = _dbContext.Clientes
                    .Include(c => c.Pais)
                    .Include(c => c.Ciudad)
                    .SingleOrDefault(ct => ct.ClienteId == id);

                var clienteVm = Mapper
                    .Map<Cliente, ClienteListViewModel>(cliente);

                ModelState.AddModelError(string.Empty, "Error al intentar dar de baja un registro");
                return View(clienteVm);
            }
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var cliente = _dbContext.Clientes
                .SingleOrDefault(ct => ct.ClienteId == id);
            if (cliente == null)
            {
                return HttpNotFound();
            }

            emailAnterior = cliente.Email;
            Session["emailAnterior"] = emailAnterior;
            ClienteEditViewModel clienteVm = Mapper
                .Map<Cliente, ClienteEditViewModel>(cliente);
            clienteVm.Paises = CombosHelper.GetPaises();
            clienteVm.Ciudades = CombosHelper.GetCiudades(clienteVm.PaisId);

            return View(clienteVm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Edit(ClienteEditViewModel clienteVm)
        {
            if (!ModelState.IsValid)
            {
                return View(clienteVm);
            }

            var cliente = Mapper.Map<ClienteEditViewModel, Cliente>(clienteVm);
            clienteVm.Paises = CombosHelper.GetPaises();
            clienteVm.Ciudades = CombosHelper.GetCiudades(clienteVm.PaisId);
            if (_dbContext.Clientes.Any(ct => ct.NombreCliente == cliente.NombreCliente
                                                && ct.ClienteId != cliente.ClienteId))
            {
                clienteVm.Paises = CombosHelper.GetPaises();
                clienteVm.Ciudades = CombosHelper.GetCiudades(clienteVm.PaisId);

                ModelState.AddModelError(string.Empty, "Registro repetido");
                return View(clienteVm);
            }

            if (Session["emailAnterior"]!=null)
            {
                emailAnterior = (string) Session["emailAnterior"];
            }

            using (var tran=_dbContext.Database.BeginTransaction())
            {
                try
                {
                    _dbContext.Entry(cliente).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                    if (emailAnterior==null)
                    {
                        UsersHelper.CreateUserAsp(cliente.Email,"Cliente");
                        
                    }else if (emailAnterior!=cliente.Email)
                    {
                        UsersHelper.UpdateUserName(emailAnterior, cliente.Email);
                    }

                    tran.Commit();
                    Session["emailAnterior"] = null;

                    TempData["Msg"] = "Registro editado";
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    tran.Rollback();
                    clienteVm.Paises = CombosHelper.GetPaises();
                    clienteVm.Ciudades = CombosHelper.GetCiudades(clienteVm.PaisId);

                    ModelState.AddModelError(string.Empty, "Error inesperado al intentar editar un registro");
                    return View(clienteVm);
                }
                
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