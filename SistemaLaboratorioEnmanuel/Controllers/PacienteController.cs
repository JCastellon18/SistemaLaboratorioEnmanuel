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
    [Authorize]
    public class PacienteController : Controller
    {
        private LaboratorioBDEntities db = new LaboratorioBDEntities();

        // GET: Pacientes
        public ActionResult Index()
        {
            return View();
        }
        public bool Create(Paciente _paciente)
        {
            if (_paciente.Cedula != null)
            {
                _paciente.FechaNacimiento = db.sp_Edad_fechaNac(_paciente.Cedula).FirstOrDefault().FechaNac.Value;
            }
            //db.sp_Edad_fechaNac(_paciente.Cedula).FirstOrDefault().Edad.Value;
            db.Paciente.Add(_paciente);
            if (db.SaveChanges() > 0)
            {
                return true;
            }
            else
                return false;
        }
        public bool Update(Paciente _paciente)
        {
            /*Editar*/
            Paciente pacienteSel = db.Paciente.Where(p => p.Id.Equals(_paciente.Id)).First();
            pacienteSel.Nombres = _paciente.Nombres;
            pacienteSel.Apellidos = _paciente.Apellidos;
            pacienteSel.Cedula = _paciente.Cedula;
            pacienteSel.FechaNacimiento = db.sp_Edad_fechaNac(_paciente.Cedula).FirstOrDefault().FechaNac.Value;
            //db.sp_Edad_fechaNac(_paciente.Cedula).FirstOrDefault().Edad.Value;
            pacienteSel.Telefono = _paciente.Telefono;
            pacienteSel.Sexo = _paciente.Sexo;
            pacienteSel.Estado = _paciente.Estado;
            db.Entry(pacienteSel).State = EntityState.Modified;
            if (db.SaveChanges() > 0)
            {
                return true;
            }
            else
                return false;
        }
        public bool Delete(Paciente _paciente)
        {
            Paciente pacienteSel = db.Paciente.Where(p => p.Id.Equals(_paciente.Id)).First();
            pacienteSel.Estado = _paciente.Estado;
            db.Entry(pacienteSel).State = EntityState.Modified;
            if (db.SaveChanges() > 0)
            {
                return true;
            }
            else
                return false;
        }
        public JsonResult ListarPacientes()
        {
            var listp = new List<Paci>();
            var lista = db.Paciente.Where(p => p.Estado.Equals("A"))
                .Select(p => new
                {
                    Id = p.Id,
                    Nombres = p.Nombres,
                    Apellidos = p.Apellidos,
                    Telefono = p.Telefono,
                    Sexo = p.Sexo,
                    Estado = p.Estado,
                    Cedula = p.Cedula,
                    fecha_nac = p.FechaNacimiento
                }).ToList();
            lista.ForEach(x => listp.Add(new Paci()
            {
                Id = x.Id,
                Nombres = x.Nombres,
                Apellidos = x.Apellidos,
                Cedula = (x.Cedula == null) ? "N/E" : x.Cedula,
                Sexo = x.Sexo,
                Telefono = x.Telefono,
                Estado = x.Estado,
                Edad = Edad(x.Cedula, x.fecha_nac)
            }));
            return Json(listp, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RecuperarInformacion(int id)
        {

            var lista = db.Paciente.Where(p => p.Id.Equals(id))
                .Select(p => new { p.Id, p.Nombres, p.Apellidos, p.Cedula, p.FechaNacimiento, p.Telefono, p.Sexo, p.Estado }).ToList();
            var listp = new List<Paci>();

            lista.ForEach(x => listp.Add(new Paci()
            {
                Id = x.Id,
                Nombres = x.Nombres,
                Apellidos = x.Apellidos,
                Cedula = x.Cedula,
                Sexo = x.Sexo,
                Telefono = x.Telefono,
                Estado = x.Estado,
                Edad = Edad(x.Cedula, x.FechaNacimiento)
            }));
            return Json(listp, JsonRequestBehavior.AllowGet);
        }
        public JsonResult BuscarPacientes(string nombrePaciente)
        {
            var listp = new List<Paci>();
            var lista = db.Paciente.Where(p => p.Estado.Equals("A") && p.Nombres.Contains(nombrePaciente) || p.Apellidos.Contains(nombrePaciente))
                .Select(p => new
                {
                    Id = p.Id,
                    Nombres = p.Nombres,
                    Apellidos = p.Apellidos,
                    Telefono = p.Telefono,
                    Sexo = p.Sexo,
                    Estado = p.Estado,
                    p.Cedula,
                    Fecha_nac = p.FechaNacimiento
                }).ToList();

            lista.ForEach(x => listp.Add(new Paci()
            {
                Id = x.Id,
                Nombres = x.Nombres,
                Apellidos = x.Apellidos,
                Cedula = x.Cedula,
                Sexo = x.Sexo,
                Telefono = x.Telefono,
                Estado = x.Estado,
                Edad = Edad(x.Cedula, x.Fecha_nac)
            }));
            return Json(listp, JsonRequestBehavior.AllowGet);
        }
        private int Edad(string cedula, DateTime fecha_nac)
        {
            int edad = 0;
            try
            {
                if (fecha_nac == null && cedula == null)
                    edad = 0;
                else if (cedula != null)
                    edad = db.sp_Edad_fechaNac(cedula).FirstOrDefault().Edad.Value;
                else
                    edad = db.sp_edad(fecha_nac).FirstOrDefault().Value;
            }
            catch (Exception ex)
            {
                return 0;
            }
            return edad;
        }
        //private FileResult generarExcel()
        //{
        //    var lista = db.Paciente.Where(p => p.Estado.Equals("A"))
        //        .Select(p => new { p.Id, p.Nombres, p.Apellidos, p.Cedula, p.FechaNacimiento, p.Telefono, p.Sexo, p.Estado }).ToList();
        //    //    XLWorkbook workbook = new XLWorkbook();
        //    //    IXLWorksheet worksheet = workbook.Worksheets.Add("Pinetech");
        //    //    worksheet.Cell(1, 1).SetValue("Pine Excel Sheet");

        //    //    MemoryStream MS = new MemoryStream();
        //    //    workbook.SaveAs(MS);
        //    //    MS.Position = 0;
        //    //    return new FileStreamResult(MS, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        //    //    { FileDownloadName = "ReportePacientes.xlsx" };
        //    //}
        //    XLWorkbook libro = new XLWorkbook();
        //    var hoja = libro.Worksheets.Add(lista.Count);

        //}
        //[HttpGet]
        //public ActionResult ExportarPersonasAExcel()
        //{

        //    return GenerarExcel(nombreArchivo, pacientes);
        //}
        public ActionResult GenerarExcel()
        {

            var pacientes = db.Paciente.Where(p => p.Estado.Equals("A")).Select(p => new { p.Id, p.Nombres, p.Apellidos, p.Cedula, p.FechaNacimiento, p.Telefono, p.Sexo, p.Estado }).ToList();
            var nombreArchivo = $"Pacientes" + DateTime.Now.ToString("MM-dd-yyyy") + ".xlsx";

            DataTable dataTable = new DataTable("Pacientes");
            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("Id"),
                new DataColumn("Nombres"),
                new DataColumn("Apellidos"),
                new DataColumn("Cedula"),
                new DataColumn("FechaNacimiento"),
                new DataColumn("Telefono"),
                new DataColumn("Sexo"),
            });

            foreach (var paciente in pacientes)
            {
                dataTable.Rows.Add(paciente.Id,
                    paciente.Nombres, paciente.Apellidos, paciente.Cedula, paciente.FechaNacimiento, paciente.Telefono, paciente.Sexo);
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
        private class Paci
        {
            public int Id { get; set; }
            public string Nombres { get; set; }
            public string Apellidos { get; set; }
            public Nullable<int> Telefono { get; set; }
            public string Sexo { get; set; }
            public string Estado { get; set; }
            public string Cedula { get; set; }
            public int Edad { get; set; }
            public DateTime Fecha_nac { get; set; }

        }
    }
}
