using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using SistemaLaboratorioEnmanuel.Models;

namespace SistemaLaboratorioEnmanuel.Controllers
{
    public class ProveedorController : Controller
    {
        private LaboratorioBDEntities db = new LaboratorioBDEntities();

        // GET: Proveedor
        public ActionResult Index()
        {
            return View();
        }
        public bool Create(Proveedor _proveedor)
        {
            db.Proveedor.Add(_proveedor);
            if (db.SaveChanges() > 0)
            {
                return true;
            }
            else
                return false;
        }
        public bool Update(Proveedor _proveedor)
        {
            /*Editar*/
            Proveedor proveedorSel = db.Proveedor.Where(p => p.Id.Equals(_proveedor.Id)).First();
            proveedorSel.NombreProveedor = _proveedor.NombreProveedor;
            proveedorSel.Descripcion = _proveedor.Descripcion;
            proveedorSel.Estado = _proveedor.Estado;
            db.Entry(proveedorSel).State = EntityState.Modified;
            if (db.SaveChanges() > 0)
            {
                return true;
            }
            else
                return false;
        }
        public bool Delete(Proveedor _proveedor)
        {
            Proveedor proveedorSel = db.Proveedor.Where(p => p.Id.Equals(_proveedor.Id)).First();
            proveedorSel.Estado = _proveedor.Estado;
            db.Entry(proveedorSel).State = EntityState.Modified;
            if (db.SaveChanges() > 0)
            {
                return true;
            }
            else
                return false;
        }
        public JsonResult ListarProveedores()
        {
            var lista = db.Proveedor.Where(p => p.Estado.Equals("A"))
                .Select(p => new { p.Id, p.NombreProveedor, p.Descripcion, p.Estado }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RecuperarInformacion(int id)
        {
            var lista = db.Proveedor.Where(p => p.Id.Equals(id))
                .Select(p => new { p.Id, p.NombreProveedor, p.Descripcion, p.Estado }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        public JsonResult BuscarProveedorNombre(string nombreProveedor)
        {
            var lista = db.Proveedor.Where(p => p.Estado.Equals("A") && p.NombreProveedor.Contains(nombreProveedor))
                .Select(p => new { p.Id, p.NombreProveedor, p.Descripcion, p.Estado }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GenerarExcel()
        {
            int cont = 1;
            var proveedores = db.Proveedor.Where(p => p.Estado.Equals("A")).Select(p => new { p.Id, p.NombreProveedor, p.Descripcion }).ToList();
            var nombreArchivo = $"Proveedores" + DateTime.Now.ToString("MM-dd-yyyy") + ".xlsx";

            DataTable dataTable = new DataTable("Proveedores");
            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("No."),
                new DataColumn("Nombre Proveedor"),
                new DataColumn("Descripcion"),
            });

            foreach (var proveedor in proveedores)
            {
                dataTable.Rows.Add(cont++, proveedor.NombreProveedor, proveedor.Descripcion);
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
