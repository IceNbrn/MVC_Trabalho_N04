using MVC_12H_N04.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_12H_N04.Controllers
{
    public class RegistarController : Controller
    {
        UtilizadoresBD bd = new UtilizadoresBD();
        // GET: Registar
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Index(UtilizadoresModel novo)
        {
            if (ModelState.IsValid)
            {
                bd.adicionarUtilizadores(novo);
                return RedirectToAction("Index", "Login");
            }
            return View();
        }
    }
}