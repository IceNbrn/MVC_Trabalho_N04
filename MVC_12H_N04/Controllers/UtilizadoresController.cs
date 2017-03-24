using MVC_12H_N04.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_Tutorial_2017.Controllers
{
    public class UtilizadoresController : Controller
    {
        UtilizadoresBd _bd = new UtilizadoresBd();

        public HttpStatusCodeResult Error401()
            => Session["perfil"] == null || Session["perfil"].Equals(1) ? new HttpStatusCodeResult(401) : null;
        
        // GET: Utilizadores
        public ActionResult Index()
        {
            Error401();
            return View(_bd.Lista());
        }

        //adicionar utilizador
        public ActionResult Create()
        {
            Error401();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UtilizadoresModel novo)
        {
            if (ModelState.IsValid)
            {

                _bd.AdicionarUtilizadores(novo);
                return RedirectToAction("Index");
            }
            return View(novo);
        }
        public ActionResult Delete(string id)
        {
            Error401();
            return View(_bd.Lista(id)[0]);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public ActionResult ConfirmarDelete(string id)
        {
            _bd.RemoverUtilizador(id);
            return RedirectToAction("index");
        }

        public ActionResult Edit(string id)
        {
            Error401();
            return View(_bd.Lista(id)[0]);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UtilizadoresModel dados)
        {
            if (ModelState.IsValid)
            {
                _bd.EditarUtilizador(dados);
                return RedirectToAction("index");
            }
            return View(dados);
        }
    }
}