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
        MarcasBd _marcasBd = new MarcasBd();

        public HttpStatusCodeResult Error401()
            => Session["perfil"] == null || Session["perfil"].Equals(1) ? new HttpStatusCodeResult(401) : null;

        // GET: Produtos
        public ActionResult Index()
        {
            ViewBag.listaMarcas = _marcasBd.Lista();
            Error401();
            return View(_bd.Lista());
        }
        public ActionResult Index2(int? id)
        {
            if (id == null) id = 1;
            ViewBag.paginaAtual = id;
            return View(_bd.listaPagina((int)id, 5));
        }
        public ActionResult ProdutosDesativados()
        {
            Error401();
            return View(_bd.ListaDesativados());
        }
        //Adicionar Produtos
        public ActionResult Create()
        {
            ViewBag.listaProdutos = _bd.Lista();
            ViewBag.listaMarcas = _marcasBd.Lista();
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
            _bd.RemoverProduto((int)id);
            ViewBag.listaProdutos = _bd.Lista();
            return RedirectToAction("index");
        }

        public ActionResult Edit(int? id)
        {
            ViewBag.listaProdutos = _bd.Lista();
            ViewBag.listaMarcas = _marcasBd.Lista();
            Error401();
            return View(_bd.Lista((int)id)[0]);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProdutosModel dados)
        {
            if (!ModelState.IsValid) return View(dados);
            _bd.AtualizarProduto(dados);
            ViewBag.listaProdutos = _bd.Lista();
            return RedirectToAction("index");
        }
        public ActionResult Pesquisar()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Pesquisar(string tbNome)
        {
            ViewBag.listaProdutos = _bd.pesquisa(tbNome);
            return View();
        }
        public ActionResult Pesquisar2()
        {
            return View();
        }
        public ActionResult Pesquisar3()
        {
            return View();
        }
        public JsonResult PesquisarJson(string id)
        {
            if (String.IsNullOrEmpty(id))
                return Json(null, JsonRequestBehavior.AllowGet);
            return Json(_bd.pesquisa(id), JsonRequestBehavior.AllowGet);
        }
        public JsonResult PesquisarJson2(string nome)
        {
            if (String.IsNullOrEmpty(nome))
                return Json(null, JsonRequestBehavior.AllowGet);
            return Json(_bd.pesquisa(nome), JsonRequestBehavior.AllowGet);
        }
    }
}