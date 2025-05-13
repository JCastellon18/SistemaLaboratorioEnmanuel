using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SistemaLaboratorioEnmanuel.Models;
using System.Globalization;
using System.Data.Entity;

namespace SistemaLaboratorioEnmanuel.Controllers
{

    public class DashboardController : Controller
    {
        private LaboratorioBDEntities db = new LaboratorioBDEntities();
        public ActionResult Index()
        {
            DateTime date = DateTime.Now;
            DateTime primerDiaDelMes = new DateTime(date.Year, date.Month, 1);
            DateTime ultimoDiaDelMes = primerDiaDelMes.AddMonths(1).AddDays(-1);
            string mesActual = DateTime.Now.ToString("MMMM");
            mesActual = mesActual.Substring(0, 1).ToUpper() + mesActual.Substring(1).ToLower();

            decimal sumTotalFacturado = 0;

            try
            {
                sumTotalFacturado = db.DetalleFactura.Where(x => x.Factura.Fecha > primerDiaDelMes && x.Factura.Fecha < ultimoDiaDelMes).DefaultIfEmpty(new DetalleFactura() { Monto = 0, Descripcion = "No hay" }).Select(e => e.Monto).Sum();
            }
            catch (Exception ex)
            {
                sumTotalFacturado = 0;
            }
            var countPacientes = db.Paciente.Where(x => x.Estado == "A").Count();
            var countOrdenes = db.Orden.Where(x => x.FechaOrden >= primerDiaDelMes && x.FechaOrden <= ultimoDiaDelMes).Count();

            var examenesMasVendidos = db.DetalleFactura
            .GroupBy(df => df.Descripcion)
            .Select(g => new
            {
                NombreExamen = g.Key,
                Cantidad = g.Count()
            })
            .OrderByDescending(e => e.Cantidad)
            .Take(5)
            .ToList();


            int Ordenes_pendientes = db.sp_TodasFacturacionesAbonarresultado().ToList().Where(x => x.diferencia > 0).Count();
            ViewBag.numeroPacientesVB = countPacientes;
            ViewBag.numeroOrdenesVB = countOrdenes;
            ViewBag.sumFacturaVB = sumTotalFacturado;
            ViewBag.mesActualVB = mesActual;
            ViewBag.ordenes_pendientes = Ordenes_pendientes;
            return View();
        }

        [HttpGet]
        public ActionResult GetExamenesMasVendidos()
        {
            var examenesMasVendidos = db.DetalleOrden
                .GroupBy(d => d.PerfilExamen.Examen.NombreTipoExamen)
                .Select(g => new
                {
                    NombreExamen = g.Key,
                    Cantidad = g.Count()
                })
                .OrderByDescending(e => e.Cantidad)
                .Take(5) // Obtener los 5 exámenes más vendidos
                .ToList();

            return Json(examenesMasVendidos, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetOrdenesPorEstado()
        {
            // Obtener todas las órdenes desde el procedimiento almacenado
            var todasLasOrdenes = db.sp_TodasFacturacionesAbonarresultado().ToList();

            // Contar órdenes pendientes (diferencia > 0)
            int ordenesPendientes = todasLasOrdenes.Count(o => o.diferencia > 0);

            // Las órdenes completadas serán el resto de órdenes con diferencia <= 0
            int ordenesCompletadas = todasLasOrdenes.Count(o => o.diferencia <= 0);

            var data = new
            {
                Completadas = ordenesCompletadas,
                Pendientes = ordenesPendientes
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetIngresosUltimos6Meses()
        {
            var ingresos = db.Abono
                .Where(a => a.FechaAbono >= DbFunctions.AddMonths(DateTime.Now, -6))
                .ToList()  // Materializar los datos aquí
                .GroupBy(a => new { Mes = a.FechaAbono.ToString("MMMM yyyy") })  // Aplicar ToString después de ToList()
                .Select(g => new
                {
                    Mes = g.Key.Mes,
                    Total = g.Sum(a => a.Monto)
                })
                .OrderBy(g => g.Mes)
                .ToList();

            return Json(ingresos, JsonRequestBehavior.AllowGet);
        }


    }
}