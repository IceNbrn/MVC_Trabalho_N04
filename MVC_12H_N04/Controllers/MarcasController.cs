using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_12H_N04.Models;

namespace MVC_12H_N04.Controllers
{
    public class MarcasController : Controller
    {
        // GET: Marcas
        MarcasBd _bd = new MarcasBd();

        public HttpStatusCodeResult Error401()
            => Session["perfil"] == null || Session["perfil"].Equals(1) ? new HttpStatusCodeResult(401) : null;

        // GET: Produtos
        public ActionResult Index()
        {
            Error401();
            return View(_bd.Lista());
        }
        //Adicionar utilizadores
        public ActionResult Create()
        {
            Error401();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MarcasModel novo)
        {
            if (!ModelState.IsValid) return View(novo);
            _bd.AdicionarMarca(novo);
            ViewBag.listaMarcas = _bd.Lista();
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int id)
        {
            Error401();
            return View(_bd.Lista((int)id)[0]);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public ActionResult ConfirmarDelete(int? id)
        {
            _bd.RemoverMarca((int)id);
            ViewBag.listaMarcas = _bd.Lista();
            return RedirectToAction("index");
        }

        public ActionResult Edit(int? id)
        {
            Error401();
            return View(_bd.Lista((int)id)[0]);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MarcasModel dados)
        {
            if (!ModelState.IsValid) return View(dados);
            _bd.AtualizarMarca(dados);
            ViewBag.listaMarcas = _bd.Lista();
            return RedirectToAction("index");
        }
        
    }
}