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
        ProdutosBD bd = new ProdutosBD();
        // GET: Produtos
        public ActionResult Index()
        {
            return View();
        }
    }
}