using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EcoDinero.Models;

namespace EcoDinero.Controllers
{
    public class DatosController : Controller
    {
        // GET: Datos/dashboard
        public ActionResult dashboard()
        {
            using (EcoDineroModel context = new EcoDineroModel())
            {
                ViewBag.TotalCategorias    = context.Categorias.Count();
                ViewBag.TotalGastado       = context.Transacciones.Where(t => t.Tipo == "Gasto").Sum(t => (decimal?)t.Monto) ?? 0;
                ViewBag.TotalIngresado     = context.Transacciones.Where(t => t.Tipo == "Ingreso").Sum(t => (decimal?)t.Monto) ?? 0;
                ViewBag.TotalTransacciones = context.Transacciones.Count();

                // Datos para gráfica de línea: gastos últimos 7 días
                var hoy = DateTime.Today;
                var hace7dias = hoy.AddDays(-6);
                var gastosPorDia = context.Transacciones
                    .Where(t => t.Tipo == "Gasto" && t.Fecha >= hace7dias)
                    .GroupBy(t => t.Fecha.Day)
                    .Select(g => new { Dia = g.Key, Total = g.Sum(t => t.Monto) })
                    .ToList();

                // Datos para gráfica de pie: gastos por categoría
                var porCategoria = context.Transacciones
                    .Where(t => t.Tipo == "Gasto")
                    .GroupBy(t => t.Categoria.Nombre)
                    .Select(g => new { Nombre = g.Key, Total = g.Sum(t => t.Monto) })
                    .ToList();

                // Datos para gráfica de línea: ingresos últimos 7 días
                var ingresosPorDia = context.Transacciones
                    .Where(t => t.Tipo == "Ingreso" && t.Fecha >= hace7dias)
                    .GroupBy(t => t.Fecha.Day)
                    .Select(g => new { Dia = g.Key, Total = g.Sum(t => t.Monto) })
                    .ToList();

                // Construir labels y datos para los últimos 7 días
                var labelsLinea = new List<string>();
                var datosLinea  = new List<decimal>();
                for (int i = 6; i >= 0; i--)
                {
                    var dia = hoy.AddDays(-i);
                    labelsLinea.Add(dia.ToString("ddd", new System.Globalization.CultureInfo("es-MX")));
                    var gasto = gastosPorDia.FirstOrDefault(g => g.Dia == dia.Day);
                    datosLinea.Add(gasto != null ? gasto.Total : 0);
                }

                var datosLineaIngresos = new List<decimal>();
                for (int i = 6; i >= 0; i--)
                {
                    var dia = hoy.AddDays(-i);
                    var ingreso = ingresosPorDia.FirstOrDefault(g => g.Dia == dia.Day);
                    datosLineaIngresos.Add(ingreso != null ? ingreso.Total : 0);
                }

                ViewBag.DatosLineaIngresos = Newtonsoft.Json.JsonConvert.SerializeObject(datosLineaIngresos);
                ViewBag.LabelsLinea    = Newtonsoft.Json.JsonConvert.SerializeObject(labelsLinea);
                ViewBag.DatosLinea     = Newtonsoft.Json.JsonConvert.SerializeObject(datosLinea);
                ViewBag.LabelsPie      = Newtonsoft.Json.JsonConvert.SerializeObject(porCategoria.Select(c => c.Nombre).ToList());
                ViewBag.DatosPie       = Newtonsoft.Json.JsonConvert.SerializeObject(porCategoria.Select(c => c.Total).ToList());

                return View();
            }
        }

        // GET: Datos/transacciones
        public ActionResult transacciones()
        {
            using (EcoDineroModel context = new EcoDineroModel())
            {
                var lista = context.Transacciones.Include("Categoria").OrderByDescending(t => t.Fecha).ToList();
                ViewBag.CategoriasGasto   = context.Categorias.Where(c => c.Tipo == "Gasto").ToList();
                ViewBag.CategoriasIngreso = context.Categorias.Where(c => c.Tipo == "Ingreso").ToList();
                return View(lista);
            }
        }

        // POST: Datos/CrearTransaccion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearTransaccion(Transaccion transaccion)
        {
            if (!ModelState.IsValid)
            {
                using (EcoDineroModel context = new EcoDineroModel())
                {
                    ViewBag.CategoriasGasto   = context.Categorias.Where(c => c.Tipo == "Gasto").ToList();
                    ViewBag.CategoriasIngreso = context.Categorias.Where(c => c.Tipo == "Ingreso").ToList();
                    return View("transacciones", context.Transacciones.Include("Categoria").ToList());
                }
            }
            using (EcoDineroModel context = new EcoDineroModel())
            {
                transaccion.Fecha = DateTime.Now;
                context.Transacciones.Add(transaccion);
                context.SaveChanges();
            }
            return RedirectToAction("transacciones");
        }

        // POST: Datos/EliminarTransaccion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarTransaccion(int id)
        {
            using (EcoDineroModel context = new EcoDineroModel())
            {
                var transaccion = context.Transacciones.Find(id);
                if (transaccion != null)
                {
                    context.Transacciones.Remove(transaccion);
                    context.SaveChanges();
                }
            }
            return RedirectToAction("transacciones");
        }

        // GET: Datos/categorias
        public ActionResult categorias()
        {
            using (EcoDineroModel context = new EcoDineroModel())
            {
                var lista = context.Categorias.ToList();
                return View(lista);
            }
        }

        // POST: Datos/CrearCategoria
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearCategoria(Categoria categoria)
        {
            if (!ModelState.IsValid)
            {
                using (EcoDineroModel context = new EcoDineroModel())
                {
                    return View("categorias", context.Categorias.ToList());
                }
            }
            using (EcoDineroModel context = new EcoDineroModel())
            {
                context.Categorias.Add(categoria);
                context.SaveChanges();
            }
            return RedirectToAction("categorias");
        }

        // POST: Datos/EliminarCategoria
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarCategoria(int id)
        {
            using (EcoDineroModel context = new EcoDineroModel())
            {
                var categoria = context.Categorias.Find(id);
                if (categoria != null)
                {
                    context.Categorias.Remove(categoria);
                    context.SaveChanges();
                }
            }
            return RedirectToAction("categorias");
        }

        // POST: Datos/EditarCategoria
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarCategoria(Categoria categoria)
        {
            if (!ModelState.IsValid)
            {
                using (EcoDineroModel context = new EcoDineroModel())
                {
                    return View("categorias", context.Categorias.ToList());
                }
            }
            using (EcoDineroModel context = new EcoDineroModel())
            {
                var _categoria = (from c in context.Categorias
                                  where c.Id == categoria.Id
                                  select c).FirstOrDefault();
                if (_categoria != null)
                {
                    _categoria.Nombre = categoria.Nombre;
                    _categoria.Icono  = categoria.Icono;
                    _categoria.Tipo   = categoria.Tipo;
                    context.SaveChanges();
                }
            }
            return RedirectToAction("categorias");
        }
    }
}