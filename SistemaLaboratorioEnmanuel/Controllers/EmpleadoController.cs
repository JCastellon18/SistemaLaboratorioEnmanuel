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
    public class EmpleadoController : Controller
    {
        private LaboratorioBDEntities db = new LaboratorioBDEntities();

        // GET: Empleadoes
        public ActionResult Index()
        {
            return View();
        }
        public bool Create(Empleado _empleado)
        {
            db.Empleado.Add(_empleado);
            if (db.SaveChanges() > 0)
            {
                return true;
            }
            else
                return false;
        }
        public bool Update(Empleado _empleado)
        {
            /*Editar*/
            Empleado empleadoSel = db.Empleado.Where(p => p.Id.Equals(_empleado.Id)).First();
            empleadoSel.Nombres = _empleado.Nombres;
            empleadoSel.Apellidos = _empleado.Apellidos;
            empleadoSel.Cedula = _empleado.Cedula;
            empleadoSel.Sexo = _empleado.Sexo;
            empleadoSel.Estado = _empleado.Estado;
            db.Entry(empleadoSel).State = EntityState.Modified;
            if (db.SaveChanges() > 0)
            {
                return true;
            }
            else
                return false;
        }
        public bool Delete(Empleado _empleado)
        {
            Empleado empleadoSel = db.Empleado.Where(p => p.Id.Equals(_empleado.Id)).First();
            empleadoSel.Estado = _empleado.Estado;
            db.Entry(empleadoSel).State = EntityState.Modified;
            if (db.SaveChanges() > 0)
            {
                return true;
            }
            else
                return false;
        }
        public JsonResult ListarEmpleados()
        {
            var lista = db.Empleado.Where(p => p.Estado.Equals("A"))
                .Select(p => new { p.Id, p.Nombres, p.Apellidos, p.Cedula, p.Sexo, p.Estado }).ToList();
            //var lista = db.SP_ListarEmpledoActivos().ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RecuperarInformacion(int id)
        {
            var lista = db.Empleado.Where(p => p.Id.Equals(id))
                .Select(p => new { p.Id, p.Nombres, p.Apellidos, p.Cedula, p.Sexo, p.Estado }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        public JsonResult BuscarEmpleadoNombre(string nombreEmpleado)
        {
            var lista = db.Empleado.Where(p => p.Estado.Equals("A") && p.Nombres.Contains(nombreEmpleado) || p.Apellidos.Contains(nombreEmpleado))
                .Select(p => new { p.Id, p.Nombres, p.Apellidos, p.Cedula, p.Sexo, p.Estado }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GenerarExcel()
        {
            int cont = 1;
            var empleados = db.Empleado.Where(e => e.Estado.Equals("A")).Select(p => new { p.Id, p.Nombres, p.Apellidos, p.Cedula, p.Sexo }).ToList();
            var nombreArchivo = $"Empleados" + DateTime.Now.ToString("MM-dd-yyyy") + ".xlsx";

            DataTable dataTable = new DataTable("Empleados");
            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("No."),
                new DataColumn("Nombres"),
                new DataColumn("Apellidos"),
                new DataColumn("Cedula"),
                new DataColumn("Sexo")
            });

            foreach (var empleado in empleados)
            {
                dataTable.Rows.Add(cont++, empleado.Nombres, empleado.Apellidos, empleado.Cedula, empleado.Sexo);
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
