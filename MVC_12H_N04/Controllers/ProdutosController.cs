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
    }
}