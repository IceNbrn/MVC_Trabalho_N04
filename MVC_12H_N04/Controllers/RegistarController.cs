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
        UtilizadoresBd _bd = new UtilizadoresBd();
        // GET: Registar
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(UtilizadoresModel novo)
        {
            if (!ModelState.IsValid || _bd.EmailExist(novo.Email) || _bd.UsernameExist(novo.Username)) return View();
            _bd.AdicionarUtilizadores(novo);
            return RedirectToAction("Index", "Login");
        }
    }
}