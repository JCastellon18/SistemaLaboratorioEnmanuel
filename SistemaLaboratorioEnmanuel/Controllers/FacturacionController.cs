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
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace SistemaLaboratorioEnmanuel.Controllers
{
    [Authorize]
    public class FacturacionController : Controller
    {

        private LaboratorioBDEntities db = new LaboratorioBDEntities();
        // GET: Facturacion
        public ActionResult Index()
        {
            var listaAbono = db.Abono.ToList();
            ViewBag.DetalleA = listaAbono;
            return View(db.sp_TodasFacturacionesAbonarresultado().OrderByDescending(x=> x.diferencia).ToList());
        }
        public JsonResult ListarAbonos(int? idfact)
        {
            var Lista = db.sp_DetalleFacturaAbonos(idfact).ToList().OrderBy(x => x.FechaAbono);
            var ListaConvert = Lista.Select(x => new
            {
                FechaAbono = x.FechaAbono.ToString("d"),
                Monto = x.Monto
            });
            return Json(ListaConvert, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListarExamenes(int? idfact)
        {
            var Lista = db.sp_DetalleFacturaExamenes(idfact.Value).ToList();
            return Json(Lista, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Abono([Bind(Include = "Id,Monto,FechaAbono,IdFactura")] Abono abono)
        {
            abono.FechaAbono = DateTime.Now;
            var id = abono.IdFactura;
            if (abono.Monto > 0)
            {
                db.Abono.Add(abono);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // GET: Facturacion/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Facturacion/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Facturacion/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Facturacion/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Facturacion/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Facturacion/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Facturacion/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        [HttpGet]
        public FileStreamResult GetPDF()
        {
            var rutaimagen = Path.Combine(Server.MapPath("~/Content/DashboardContent/img/" + "lablogo.png"));
            byte[] imagedata = System.IO.File.ReadAllBytes(rutaimagen);

            Document document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    var Total = 500;
                    page.Size(PageSizes.Letter);
                    page.Margin(30);
                    page.PageColor(Colors.White);
                    page.Header().ShowOnce().Row(fila =>
                    {
                        fila.ConstantItem(140).Width(100).Image(imagedata);
                        fila.RelativeItem().Column(columna =>
                        {
                            columna.Item().AlignCenter().Text("Laboratorio Clinico Emmanuel").Bold().FontSize(14).FontColor("#3a51a5");
                            columna.Item().AlignCenter().Text("Barrio Santa Isabel Isabel, De la chatiza \n1 C este, 1 Sur").FontSize(8);
                            columna.Item().AlignCenter().Text("Tel: 8745 7932").FontSize(8);
                        });
                        fila.RelativeItem().PaddingLeft(10).Column(columna =>
                        {
                            columna.Item().Border(1).BorderColor("#38afe3").AlignCenter().Text("Factura").Bold().FontSize(16);
                            columna.Item().Background("#38afe3").Border(1).BorderColor("#38afe3").AlignCenter().Text("No de Factura: 001").FontSize(8).FontColor("#FFF");
                            columna.Item().Border(1).BorderColor("#38afe3").AlignCenter().Text("Fecha:" + DateTime.Today.ToString()).FontSize(8);
                        });
                    });

                    page.Content().PaddingVertical(15).Column(columna1 =>
                    {
                        columna1.Item().Text("Datos del cliente").Underline().Bold();
                        columna1.Item().Text(txt =>
                        {
                            txt.Span("Nombre: ").SemiBold().FontSize(10);
                            txt.Span("Nombre de prueba").FontSize(10); // Se eliminara al añadir la variable
                        });

                        columna1.Item().Text(txt =>
                        {
                            txt.Span("Cedula: ").SemiBold().FontSize(10);
                            txt.Span("001-170496-0016K").FontSize(10); // Se eliminara al añadir la variable
                        });

                        columna1.Item().Text(txt =>
                        {
                            txt.Span("Celular: ").SemiBold().FontSize(10);
                            txt.Span("84263139").FontSize(10); // Se eliminara al añadir la variable
                        });
                        columna1.Item().PaddingVertical(12).Table(tabla =>
                        {
                            tabla.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn();
                            });
                            tabla.Header(header =>
                            {
                                header.Cell().Background("#38afe3").Padding(3).PaddingLeft(5).Text("Examenes").FontColor("#fff");
                                header.Cell().Background("#38afe3").Padding(3).AlignCenter().Text("Precio").FontColor("#fff");
                            });
                            foreach (var item in Enumerable.Range(1, 12))
                            {
                                var Examen = Placeholders.Random.Next(1, 5);
                                var Precio = Placeholders.Random.Next(5, 10);

                                tabla.Cell().PaddingLeft(5).PaddingBottom(5).PaddingTop(5).BorderBottom(0.5f).BorderColor("#D9D9D9").Text(Placeholders.Label()).FontSize(10);
                                tabla.Cell().PaddingRight(5).PaddingBottom(5).PaddingTop(5).BorderBottom(0.5f).BorderColor("#D9D9D9").AlignRight().Padding(2).Text(Precio.ToString()).FontSize(10);
                                Total += Total;
                            }

                        });
                        columna1.Item().PaddingVertical(3).Row(fila =>
                        {
                            fila.RelativeItem(2).Height(120).PaddingVertical(18).PaddingRight(10).Border(1).Column(columna =>
                            {
                                columna.Item().Background("#38afe3").PaddingLeft(5).Padding(2).Text("Comentarios").FontSize(10).FontColor("#fff");
                                columna.Item().PaddingLeft(5).Padding(2).Text("1. Comentario").FontSize(8);
                                columna.Item().PaddingLeft(5).Padding(2).Text("2. Comentario").FontSize(8);
                                columna.Item().PaddingLeft(5).Padding(2).Text("3. Comentario").FontSize(8);
                            });
                            fila.RelativeItem().Column(columna =>
                            {
                                columna.Item().Padding(5).Background("#38afe3").Text("Subtotal: C$" + Total).FontSize(10).FontColor("#fff");
                                columna.Item().Padding(5).BorderBottom(0.5f).Text("Descuento: C$0").FontSize(10);
                                //columna.Item().Padding(5).LineHorizontal(0.5f).LineColor("#d9d9d9");
                                columna.Item().Padding(5).Background("#38afe3").Text("Total: C$" + Total).FontSize(10).FontColor("#fff");
                            });
                        });
                    });
                });
            });

            byte[] pdfBytes = document.GeneratePdf();
            MemoryStream ms = new MemoryStream(pdfBytes);
            //return new FileStreamResult(ms, "application/pdf");
            return File(ms, "application/pdf", "myfile.pdf");
        }

        [HttpGet]
        public FileStreamResult GetTicket(int FacturaId)
        {
            Factura fact = null;

            try
            {
                fact = db.Factura.FirstOrDefault(x=> x.Id == FacturaId);
            }
            catch (Exception e)
            {

            }

            string nombreEmpresa = "Laboratorio Clinico Emmanuel";
            string direccionEmpresa = "Barrio Santa Isabel Isabel, De la chatiza 1C este, 1C Sur";
            string numerotelefono1 = "Tel: 8745 7932";
            string leyenda = "***         Gracias por su preferencia         ***";


            var rutaimagen2 = Path.Combine(Server.MapPath("~/Content/DashboardContent/img/" + "LogoCircle.png"));
            byte[] imagedata2 = System.IO.File.ReadAllBytes(rutaimagen2);
 
            Document document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter);
                    //page.Margin(30);
                    page.PageColor(Colors.White);
                    page.Header().Width(8, Unit.Centimetre).ShowOnce().Row(fila =>
                    {
                        fila.RelativeItem().Width(8, Unit.Centimetre).PaddingTop(20).PaddingLeft(20).PaddingRight(20).Column(columna =>
                        {
                            columna.Item().AlignCenter().Width(35).PaddingBottom(5).Image(imagedata2);
                            columna.Item().AlignCenter().Text(nombreEmpresa).Bold().FontSize(10);
                            columna.Item().AlignCenter().Text(direccionEmpresa).FontSize(6);
                            columna.Item().AlignCenter().Text(numerotelefono1).FontSize(6);
                        });


                    });
                    page.Content().Width(8, Unit.Centimetre).Padding(15).Column(columna1 =>
                    {
                        columna1.Item().AlignRight().Text("Factura").Bold().FontSize(10);
                        columna1.Item().AlignRight().Text(txt =>
                        {
                            txt.Span("No Factura: ").SemiBold().FontSize(6);
                            txt.Span(""+fact.Id).FontSize(6); 
                        });
                        columna1.Item().AlignRight().Text(txt =>
                        {
                            txt.Span("Fecha: " + DateTime.Today.ToString("d")).SemiBold().FontSize(6);
                        });

                        columna1.Item().Padding(1).Text("---------------------------------------------------------------------------------------------").FontColor("#000").FontSize(6);
                        columna1.Item().Text("Datos del cliente").Bold().FontSize(6);
                        columna1.Item().Text(txt =>
                        {
                            txt.Span("Nombre: ").SemiBold().FontSize(6);
                            txt.Span(fact.Paciente.Nombres +" "+ fact.Paciente.Apellidos).FontSize(6); 
                        });

                        columna1.Item().Text(txt =>
                        {
                            txt.Span("Cedula: ").SemiBold().FontSize(6);
                            txt.Span(fact.Paciente.Cedula).FontSize(6);
                        });

                        columna1.Item().Text(txt =>
                        {
                            txt.Span("Celular: ").SemiBold().FontSize(6);
                            txt.Span(""+fact.Paciente.Telefono).FontSize(6); 
                        });
                        columna1.Item().Padding(1).Text("---------------------------------------------------------------------------------------------").FontSize(6).FontColor("#000");
                        columna1.Item().PaddingTop(12).PaddingBottom(2).Table(tabla =>
                        {
                            tabla.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn();
                            });
                            tabla.Header(header =>
                            {
                                header.Cell().PaddingLeft(5).PaddingBottom(2).Text("Descripcion").Bold().FontColor("#000").FontSize(7);
                                header.Cell().PaddingRight(5).PaddingBottom(2).AlignRight().Text("Precio").Bold().FontColor("#000").FontSize(7);
                            });
                            foreach (var item in fact.DetalleFactura)
                            {

                                tabla.Cell().PaddingLeft(5).PaddingBottom(5).Text(item.PerfilExamen.Examen.NombreTipoExamen).FontSize(6);
                                tabla.Cell().PaddingRight(5).PaddingBottom(5).AlignRight().Text("C$ "+ item.Monto).FontSize(6);
                            }
                        });
                        columna1.Item().Padding(1).Text("---------------------------------------------------------------------------------------------").FontSize(6).FontColor("#000");
                        columna1.Item().PaddingVertical(2).Table(tabla2 =>
                        {
                            tabla2.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn();
                            });
                            tabla2.Cell().PaddingBottom(5).AlignRight().Text("Subtotal:").Bold().FontSize(6);
                            tabla2.Cell().PaddingRight(5).PaddingBottom(5).AlignRight().Text("C$ " + fact.SubTotal).FontSize(6);

                            tabla2.Cell().PaddingBottom(5).AlignRight().Text("Descuento:").Bold().FontSize(6);
                            tabla2.Cell().PaddingRight(5).PaddingBottom(5).AlignRight().Text("C$ "+fact.MontoDescuento).FontSize(6);

                            tabla2.Cell().PaddingBottom(5).AlignRight().Text("Total:").Bold().FontSize(6);
                            tabla2.Cell().PaddingRight(5).PaddingBottom(5).AlignRight().Text("C$ " + fact.Total).FontSize(6);
                        });
                        //Final de Ticket
                        columna1.Item().AlignCenter().PaddingTop(1, Unit.Centimetre).Text(txt =>
                        {
                            txt.Span("********************************************").FontSize(6);
                        });
                        columna1.Item().AlignCenter().Text(txt =>
                        {
                            txt.Span(leyenda).FontSize(6);
                        });
                        columna1.Item().AlignCenter().PaddingTop(1).Text(txt =>
                        {
                            txt.Span("********************************************").FontSize(6);
                        });
                    });
                });
            });
            byte[] pdfBytes = document.GeneratePdf();
            MemoryStream ms = new MemoryStream(pdfBytes);
            //return new FileStreamResult(ms, "application/pdf");
            return File(ms, "application/pdf", "myfile.pdf");
        }
    }
}
