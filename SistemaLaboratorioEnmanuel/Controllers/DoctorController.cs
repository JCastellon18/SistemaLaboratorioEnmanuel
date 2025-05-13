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
    public class DoctorController : Controller
    {
        private LaboratorioBDEntities db = new LaboratorioBDEntities();

        // GET: Doctors
        public ActionResult Index()
        {
            return View();
        }
        public bool Create(Doctor _doctor)
        {
            db.Doctor.Add(_doctor);
            if (db.SaveChanges() > 0)
            {
                return true;
            }
            else
                return false;
        }
        public bool Update(Doctor _doctor)
        {
            /*Editar*/
            Doctor doctorSel = db.Doctor.Where(p => p.Id.Equals(_doctor.Id)).First();
            doctorSel.Nombres = _doctor.Nombres;
            doctorSel.Apellidos = _doctor.Apellidos;
            doctorSel.Descripcion = _doctor.Descripcion;
            doctorSel.Telefono = _doctor.Telefono;
            doctorSel.Estado = _doctor.Estado;
            db.Entry(doctorSel).State = EntityState.Modified;
            if (db.SaveChanges() > 0)
            {
                return true;
            }
            else
                return false;
        }
        public bool Delete(Doctor _doctor)
        {
            Doctor doctorSel = db.Doctor.Where(p => p.Id.Equals(_doctor.Id)).First();
            doctorSel.Estado = _doctor.Estado;
            db.Entry(doctorSel).State = EntityState.Modified;
            if (db.SaveChanges() > 0)
            {
                return true;
            }
            else
                return false;
        }
        public JsonResult ListarDoctores()
        {
            var lista = db.Doctor.Where(p => p.Estado.Equals("A"))
                .Select(p => new { p.Id, p.Nombres, p.Apellidos, p.Descripcion, p.Telefono, p.Estado }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RecuperarInformacion(int id)
        {
            var lista = db.Doctor.Where(p => p.Id.Equals(id))
                .Select(p => new { p.Id, p.Nombres, p.Apellidos, p.Descripcion, p.Telefono, p.Estado }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        public JsonResult BuscarDoctorNombre(string nombreDoctor)
        {

            var lista = db.Doctor.Where(p => p.Estado.Equals("A") && p.Nombres.Contains(nombreDoctor) || p.Apellidos.Contains(nombreDoctor))
                .Select(p => new { p.Id, p.Nombres, p.Apellidos, p.Descripcion, p.Telefono, p.Estado }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GenerarExcel()
        {
            int cont = 1;
            var doctores = db.Doctor.Where(d => d.Estado.Equals("A")).Select(p => new { p.Id, p.Nombres, p.Apellidos, p.Descripcion, p.Telefono }).ToList();
            var nombreArchivo = $"Doctores" + DateTime.Now.ToString("MM-dd-yyyy") + ".xlsx";

            DataTable dataTable = new DataTable("Doctores");
            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("No."),
                new DataColumn("Nombres"),
                new DataColumn("Apellidos"),
                new DataColumn("Descripcion"),
                new DataColumn("Telefono")
            });

            foreach (var doctor in doctores)
            {
                dataTable.Rows.Add(cont++, doctor.Nombres, doctor.Apellidos, doctor.Descripcion, doctor.Telefono);
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
