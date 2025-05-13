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
    public class AreaController : Controller
    {
        private LaboratorioBDEntities db = new LaboratorioBDEntities();

        // GET: Areas
        public ActionResult Index()
        {
            return View();
        }
        public bool Create(Area _area)
        {
            db.Area.Add(_area);
            if (db.SaveChanges() > 0)
            {
                return true;
            }
            else
                return false;
        }
        public bool Update(Area _area)
        {
            /*Editar*/
            Area areaSel = db.Area.Where(p => p.Id.Equals(_area.Id)).First();
            areaSel.NombreArea = _area.NombreArea;
            areaSel.Descripcion = _area.Descripcion;
            areaSel.Estado = _area.Estado;
            db.Entry(areaSel).State = EntityState.Modified;
            if (db.SaveChanges() > 0)
            {
                return true;
            }
            else
                return false;
        }
        public bool Delete(Area oarea)
        {
            Area areaSel = db.Area.Where(p => p.Id.Equals(oarea.Id)).First();
            areaSel.Estado = oarea.Estado;
            db.Entry(areaSel).State = EntityState.Modified;
            if (db.SaveChanges() > 0)
            {
                return true;
            }
            else
                return false;
        }
        [HttpPost]
        public JsonResult ListarAreas()
        {
            var lista = db.Area.Where(p => p.Estado.Equals("A"))
                .Select(p => new { p.Id, p.NombreArea, p.Descripcion, p.Estado }).ToList();

            //var listaProceso = db.sp_AreasActivas().ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RecuperarInformacion(int id)
        {
            var lista = db.Area.Where(p => p.Id.Equals(id))
                .Select(p => new { p.Id, p.NombreArea, p.Descripcion, p.Estado }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        public JsonResult BuscarAreaNombre(string nombreArea)
        {
            var lista = db.Area.Where(p => p.Estado.Equals("A") && p.NombreArea.Contains(nombreArea))
                .Select(p => new { p.Id, p.NombreArea, p.Descripcion, p.Estado }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GenerarExcel()
        {
            var areas = db.Area.Where(a => a.Estado.Equals("A")).Select(a => new { a.Id, a.NombreArea, a.Descripcion }).ToList();
            var nombreArchivo = $"Areas" + DateTime.Now.ToString("MM-dd-yyyy") + ".xlsx";

            DataTable dataTable = new DataTable("Pacientes");
            dataTable.Columns.AddRange(new DataColumn[]
            {
                //new DataColumn("Id"),
                new DataColumn("Nombre Area"),
                new DataColumn("Descripcion"),
            });

            foreach (var area in areas)
            {
                dataTable.Rows.Add(area.NombreArea, area.Descripcion);
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
