using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EcoDinero.Controllers
{
    public class DatosController : Controller
    {
        // GET: Datos
        public ActionResult dashboard()
        {
            return View();
        }
        public ActionResult transacciones()
        {
            return View();
        }

        public ActionResult categorias()
        {
            return View();
        }
    }
}