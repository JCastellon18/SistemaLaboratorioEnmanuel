using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SistemaLaboratorioEnmanuel.Models;
using SistemaLaboratorioEnmanuel.Models.ViewModels;
using SistemaLaboratorioEnmanuel.Util;
using static SistemaLaboratorioEnmanuel.Util.ListUtil;

namespace SistemaLaboratorioEnmanuel.Controllers
{
    [Authorize]
    public class OrdenController : Controller
    {
        private LaboratorioBDEntities db = new LaboratorioBDEntities();

        // GET: Ordens
        public ActionResult Index()
        {
            return View(db.SP_ProcDatosOrden().OrderByDescending(x=> x.FechaOrden).ToList());
        }
        public ActionResult Create()
        {
            //PRUEBA MANDAR LISTA DE DOCTORES MEDIANTE VIEWBAG----  SI FUNCIONA
            var listaDoctores = db.Doctor.Where(e => e.Estado.Equals("A")).Select(e => new { e.Id, e.Nombres, e.Apellidos }).ToList();
            ViewBag.ListarDoctoresVB = db.SP_ListaDoctores().ToList();
            //VIEWBAG DE LISTA DE PACIENTES
            ViewBag.ListarPacientesVB = db.SP_ListaPacientes().ToList();
            //VIEWBAG DE LISTA DE PACIENTES
            ViewBag.ListarExamenesVB = db.SP_ListaExamenes().ToList();

            List<ListaExamenesViewModel> lista =
                (from x in db.Examen
                 where x.Estado.Equals("A")
                 select new ListaExamenesViewModel
                 {
                     Id = x.Id,
                     NombreTipoExamen = x.NombreTipoExamen
                 }).ToList();
            List<SelectListItem> listaExamen = lista.ConvertAll(x =>
            {
                return new SelectListItem()
                {
                    Text = x.NombreTipoExamen.ToString(),
                    Value = x.Id.ToString(),
                    Selected = false

                };
            });

            //--------------------------------------
            List<PacientesViewModel> lstPaciente =
                (from x in db.Paciente
                 where x.Estado.Equals("A")
                 select new PacientesViewModel
                 {
                     Id = x.Id,
                     NombreCompleto = x.Nombres + " " + x.Apellidos,
                 }).ToList();
            List<SelectListItem> ListaPaciente = lstPaciente.ConvertAll(x =>
            {
                return new SelectListItem()
                {
                    Text = x.NombreCompleto.ToString(),
                    Value = x.Id.ToString(),
                    Selected = false
                };
            });
            //---------------------------
            List<DoctoresViewModel> lstDoctores =
               (from x in db.Doctor
                where x.Estado.Equals("A")
                select new DoctoresViewModel
                {
                    Id = x.Id,
                    NombreCompleto = x.Nombres + " " + x.Apellidos,
                }).ToList();
            List<SelectListItem> ListaDoctores = lstDoctores.ConvertAll(x =>
            {
                return new SelectListItem()
                {
                    Text = x.NombreCompleto.ToString(),
                    Value = x.Id.ToString(),
                    Selected = false
                };
            });
            //-------------------------------
            ViewBag.listExamen = listaExamen;
            ViewBag.listPaciente = ListaPaciente;
            ViewBag.listaDoctores = ListaDoctores;


            OrdenViewModel orden= new OrdenViewModel();
            return View(orden);
        }

        [HttpPost]
        public ActionResult Add(OrdenViewModel model, decimal MontoDescuento, decimal PrecioTotal, decimal PrecioSubTotal)
        {
            Orden _orden = new Orden();
            _orden.FechaOrden = DateTime.Now;
            _orden.TipoOrden = model.TipoOrden;
            _orden.Descripcion = model.Descripcion;
            _orden.IdDoctor = model.IdDoctor;
            _orden.IdPaciente = model.IdPaciente;
            db.Orden.Add(_orden);
            db.SaveChanges();
            try
            {
                foreach (var _detalle in ListUtil.listaExamenesOrden)
                {
                    DetalleOrden _detalleOrden = new DetalleOrden();
                    _detalleOrden.IdOrden = _orden.Id;
                    _detalleOrden.PerfilExamenId = _detalle.PerfilExamenId;           //EEEEEEEhHHHHHHHHHHH
                    _detalleOrden.IdEmpleado = UsuarioUtil.IdUsuarioActual;
                    _detalleOrden.TipoExamen = "A";
                    db.DetalleOrden.Add(_detalleOrden);
                }
                db.SaveChanges();

                
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
            añadirFactDetalle(_orden, MontoDescuento, PrecioTotal, PrecioSubTotal);

            return RedirectToAction("Index");
        }

        /*public ActionResult Add(OrdenViewModel model)
        {
            Orden _orden = new Orden();
            _orden.FechaOrden = DateTime.Now;
            _orden.TipoOrden = model.TipoOrden;
            _orden.Descripcion = model.Descripcion;
            _orden.IdDoctor = model.IdDoctor;
            _orden.IdPaciente = model.IdPaciente;
            db.Orden.Add(_orden);
            db.SaveChanges();
            try
            {
                foreach (var _detalle in model.detalleOrdenes)
                {
                    DetalleOrden _detalleOrden = new DetalleOrden();
                    _detalleOrden.IdOrden = _orden.Id;
                    //_detalleOrden.IdExamen = _detalle.IdExamen;           //EEEEEEEhHHHHHHHHHHH
                    _detalleOrden.IdEmpleado = 1;
                    _detalleOrden.TipoExamen = _detalle.TipoExamen;
                    db.DetalleOrden.Add(_detalleOrden);
                }
                db.SaveChanges();
                añadirFactDetalle(_orden, model.detalleOrdenes);
            }
            catch (Exception ex)
            {
                return View(model);
            }

            return RedirectToAction("Index");
        }*/


        private void añadirFactDetalle(Orden model, decimal MontoDescuento, decimal Total, decimal SubTotal)
        {
            Factura fact = new Factura();
            fact.Descripcion = "Factura generada apartir de Orden de Examen(es)";
            fact.Fecha = DateTime.Now;
            fact.IdPaciente = model.IdPaciente;
            fact.IdOrden = model.Id;
            fact.MontoDescuento = MontoDescuento;
            fact.Total = Total;
            fact.SubTotal = SubTotal;
            fact.Estado = "A";
            db.Factura.Add(fact);
            db.SaveChanges();
           
            foreach (var item in ListUtil.listaExamenesOrden)
            {
                DetalleFactura df = new DetalleFactura();
                df.Descripcion = "Examen de ";
                df.Monto = item.PrecioExamen;
                df.IdFactura = fact.Id;
                df.PerfilExamenId = item.PerfilExamenId;                            //EEEEEEEhHHHHHHHHHHH
                db.DetalleFactura.Add(df);
             
            }
            db.SaveChanges();
        }

        public JsonResult ListarExamenOrden(int? idOrden)
        {
            var Lista = db.sp_ExamenDetalleOrdenes(idOrden).ToList();
            return Json(Lista, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult BuscDoct(int id)
        {
            var lista = db.Doctor.Where(p => p.Id.Equals(id))
               .Select(p => new { p.Id, p.Nombres, p.Apellidos, p.Estado }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult BuscPac(int id)
        {
            var lista = db.Paciente.Where(p => p.Id.Equals(id))
               .Select(p => new { p.Id, p.Nombres, p.Apellidos, p.Estado }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult BuscExam(int id)
        {
            var lista = db.Examen.Where(e => e.Id.Equals(id))
               .Select(e => new { e.Id, e.NombreTipoExamen, e.Estado }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public PartialViewResult ListaAreas()
        {
            var areas = db.Area.Where(x=> x.Estado =="A").ToList();
            return PartialView("Parciales/TablaOrdenAreas", areas);
        }

        [HttpPost]
        public PartialViewResult ListaPerfiles(int AreaId)
        {
            var perfiles = db.Perfil.Where(x=> x.AreaId == AreaId && x.Estado =="A").ToList();
            return PartialView("Parciales/TablaOrdenPerfiles", perfiles);
        }

        [HttpPost]
        public PartialViewResult ListaExamamenesPerfil(int PerfilId)
        {
            var examenes = db.PerfilExamen.Where(x => x.PerfilId == PerfilId).ToList();
            return PartialView("Parciales/TablaOrdenPerfilExamenes", examenes);
        }

        [HttpPost]
        public JsonResult AddExamenTemp(int PerfilExamenId, decimal PrecioExamen, int PerfilId)
        {
            int cantidad = 0;
            try
            {
                cantidad = ListUtil.listaExamenesOrden.Count(x => x.PerfilExamenId == PerfilExamenId && x.PerfilId == PerfilId);
            }
            catch (Exception e)
            {
                cantidad = 0;
            }

            if (cantidad == 0)
            {
                OrdenExamenes ordenExamenes = new OrdenExamenes() {PerfilExamenId = PerfilExamenId, PerfilId = PerfilId, PrecioExamen = PrecioExamen };
                ListUtil.listaExamenesOrden.Add(ordenExamenes);
            }

            return Json(cantidad);
        }

        [HttpPost]
        public JsonResult RemoveExamenTemp(int PerfilExamenId, int PerfilId)
        {
            int cantidad = 0;
            try
            {
                cantidad = ListUtil.listaExamenesOrden.Count(x => x.PerfilExamenId == PerfilExamenId && x.PerfilId == PerfilId);
            }
            catch (Exception e)
            {
                cantidad = 0;
            }

            if (cantidad == 1)
            {
                var temp = ListUtil.listaExamenesOrden.FirstOrDefault(x => x.PerfilExamenId == PerfilExamenId && x.PerfilId == PerfilId);
                ListUtil.listaExamenesOrden.Remove(temp);
            }

            return Json(cantidad);
        }

        [HttpPost]
        public JsonResult GetSumTotalExamenes()
        {
            OrdenExamenes temp = new OrdenExamenes() { PrecioExamen = 0 };
            decimal cantidad = 0;
            decimal subCantidad = 0;
            try
            {
                cantidad = ListUtil.listaExamenesOrden.DefaultIfEmpty(temp).Sum(x=> x.PrecioExamen);
                subCantidad = cantidad;
                cantidad = (cantidad - ListUtil.MontoDescuento);
            }
            catch (Exception e)
            {
                cantidad = 0;
                subCantidad = 0;
            }

            var totales = new {total = cantidad , subTotal = subCantidad };

            return Json( totales);
        }

        public JsonResult GetCantTotalExamenes()
        {
            int cantidad = 0;
            try
            {
                cantidad = ListUtil.listaExamenesOrden.Count;
            }
            catch (Exception e)
            {
                cantidad = 0;
            }

            return Json(cantidad);
        }

        [HttpPost]
        public PartialViewResult CargarDescuento()
        {
            return PartialView("Parciales/InputMontoDescuento", ListUtil.MontoDescuento);
        }

        [HttpPost]
        public decimal ActualizarDescuento(decimal nuevoMonto)
        {
            ListUtil.MontoDescuento = nuevoMonto;
            return ListUtil.MontoDescuento;
        }


        [HttpGet]
        public FileStreamResult GetPDF()
        {
            Document document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Header()
                        .Text("Hello PDF!")
                        .SemiBold().FontSize(36).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(x =>
                        {
                            x.Spacing(20);

                            x.Item().Text(Placeholders.LoremIpsum());
                            x.Item().Image(Placeholders.Image(200, 100));
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                        });
                });
            });
            byte[] pdfBytes = document.GeneratePdf();
            MemoryStream ms = new MemoryStream(pdfBytes);
            return new FileStreamResult(ms, "application/pdf");

        }

    }
}
