using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_12H_N04.Models;

namespace MVC_12H_N04.Controllers
{
    public class HomeController : Controller
    {
        ProdutosBd _bd = new ProdutosBd();
        public ActionResult Index()
        {
            ProdutosBd Bd = new ProdutosBd();
            ViewBag.ListaProdutos3 = Bd.listaPagina(1, 3);
            return View();
        }

        public ActionResult Produtos()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Produtos(string produto)
        {
            ViewBag.Produtos = _bd.Lista(produto);
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}