using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_12H_N04.Models;

namespace MVC_12H_N04.Controllers
{
    public class ProdutosController : Controller
    {
        ProdutosBd _bd = new ProdutosBd();

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
        public ActionResult Create(ProdutosModel novo)
        {
            if (!ModelState.IsValid) return View(novo);
            _bd.AdicionarProdutos(novo);
            return RedirectToAction("Index");
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
            _bd.RemoverProduto(id);
            return RedirectToAction("index");
        }

        public ActionResult Edit(string id)
        {
            Error401();
            return View(_bd.Lista(id)[0]);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProdutosModel dados)
        {
            if (!ModelState.IsValid) return View(dados);
            _bd.AtualizarProduto(dados);
            return RedirectToAction("index");
        }
    }
}