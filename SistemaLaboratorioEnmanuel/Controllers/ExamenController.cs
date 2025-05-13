using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SistemaLaboratorioEnmanuel.Models;
using ClosedXML.Excel;
using System.IO;

namespace SistemaLaboratorioEnmanuel.Controllers
{
    [Authorize]
    public class ExamenController : Controller
    {
        private LaboratorioBDEntities db = new LaboratorioBDEntities();

        // GET: Examen
        public ActionResult Index()
        {
            return View();
        }
        public bool Create(Examen _examen)
        {
            db.Examen.Add(_examen);
            if (db.SaveChanges() > 0)
            {
                return true;
            }
            else
                return false;
        }
        public bool Update(Examen _examen)
        {
            /*Editar*/
            Examen examenSel = db.Examen.Where(p => p.Id.Equals(_examen.Id)).First();
            examenSel.NombreTipoExamen = _examen.NombreTipoExamen;
            examenSel.Descripcion = _examen.Descripcion;
            examenSel.Precio = _examen.Precio;
            examenSel.IdArea = _examen.IdArea;
            examenSel.Estado = _examen.Estado;
            db.Entry(examenSel).State = EntityState.Modified;
            if (db.SaveChanges() > 0)
            {
                return true;
            }
            else
                return false;
        }
        public bool Delete(Examen _examen)
        {
            Examen examenSel = db.Examen.Where(p => p.Id.Equals(_examen.Id)).First();
            examenSel.Estado = _examen.Estado;
            db.Entry(examenSel).State = EntityState.Modified;
            if (db.SaveChanges() > 0)
            {
                return true;
            }
            else
                return false;
        }
        public JsonResult ListarExamenes()
        {
            var lista = db.Examen.Where(p => p.Estado.Equals("A"))
                .Select(p => new { p.Id, p.NombreTipoExamen, p.Descripcion, p.Precio, p.Estado, p.Area.NombreArea }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListarComboArea()
        {
            var lista = db.Area.Where(P => P.Estado.Equals("A")).Select(p => new { p.Id, p.NombreArea });
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        public JsonResult BuscarExamenNombre(string nombreExamen)
        {
            var lista = db.Examen.Where(p => p.Estado.Equals("A") && p.NombreTipoExamen.Contains(nombreExamen))
                .Select(p => new { p.Id, p.NombreTipoExamen, p.Descripcion, p.Precio, p.Estado, p.IdArea }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RecuperarInformacion(int id)
        {
            var lista = db.Examen.Where(p => p.Id.Equals(id))
               .Select(p => new { p.Id, p.NombreTipoExamen, p.Descripcion, p.Precio, p.IdArea }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GenerarExcel()
        {
            int cont = 1;
            var examenes = db.Examen.Where(e => e.Estado.Equals("A")).Select(p => new { p.Id, p.NombreTipoExamen, p.Descripcion, p.Area.NombreArea }).ToList();
            var nombreArchivo = $"Examenes" + DateTime.Now.ToString("MM-dd-yyyy") + ".xlsx";

            DataTable dataTable = new DataTable("Examenes");
            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("No."),
                new DataColumn("Nombre Examen"),
                new DataColumn("Descripcion"),
                new DataColumn("Area"),
            });

            foreach (var examen in examenes)
            {
                dataTable.Rows.Add(cont++, examen.NombreTipoExamen, examen.Descripcion, examen.NombreArea);
            }
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dataTable);

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        nombreArchivo);
                }
            }
        }
    }
}
