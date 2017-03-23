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
        UtilizadoresBD bd = new UtilizadoresBD();
        public HttpStatusCodeResult error401()
        {
            if (Session["perfil"] == null || Session["perfil"].Equals(1))
                return new HttpStatusCodeResult(401);

            return null;
        }
        // GET: Utilizadores
        public ActionResult Index()
        {
            error401();
            return View(bd.lista());
        }

        //adicionar utilizador
        public ActionResult Create()
        {
            error401();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UtilizadoresModel novo)
        {
            if (ModelState.IsValid)
            {

                bd.adicionarUtilizadores(novo);
                return RedirectToAction("Index");
            }
            return View(novo);
        }
        public ActionResult Delete(string id)
        {
            error401();
            return View(bd.lista(id)[0]);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public ActionResult ConfirmarDelete(string id)
        {
            bd.removerUtilizador(id);
            return RedirectToAction("index");
        }

        public ActionResult Edit(string id)
        {
            error401();
            return View(bd.lista(id)[0]);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UtilizadoresModel dados)
        {
            if (ModelState.IsValid)
            {
                bd.editarUtilizador(dados);
                return RedirectToAction("index");
            }
            return View(dados);
        }
    }
}