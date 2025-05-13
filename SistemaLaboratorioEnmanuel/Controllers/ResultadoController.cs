using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SistemaLaboratorioEnmanuel.Models;
using SistemaLaboratorioEnmanuel.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace SistemaLaboratorioEnmanuel.Controllers
{
    [Authorize]
    public class ResultadoController : Controller
    {

        private LaboratorioBDEntities db = new LaboratorioBDEntities();
        // GET: Resultado
        public ActionResult Index(int? id)
        {
            if (id.HasValue)
            {
                // Puedes usar el ID para realizar una consulta en la base de datos
                ViewBag.IdOrden = id.Value;

                // Ejemplo: Cargar datos relacionados con la orden
                var Examenes_orden = db.DetalleOrden.Where(X => X.IdOrden == id.Value).ToList();

                var dtExamenes = new OrdenesParaResultado() { IdOrden = id.Value, ExamenesOrden = Examenes_orden };

                return View(dtExamenes);
            }

            // Si no se recibe un ID, retornar la vista por defecto
            return View();
        }

        public ActionResult RegistrarResultado(int idDetalleOrden)
        {
            // Obtén los datos del DetalleOrden junto con PerfilExamen y Examen
            var detalle = db.DetalleOrden
                .Include("PerfilExamen") // Incluye PerfilExamen relacionado
                .Include("PerfilExamen.Examen") // Incluye Examen dentro de PerfilExamen
                .FirstOrDefault(d => d.Id == idDetalleOrden);

            if (detalle == null)
            {
                return HttpNotFound();
            }

            var resultadosRegistrados = db.Resultado
            .Where(r => r.IdDetalleOrden == idDetalleOrden && r.estado == "A")
            .Select(r => new ResultadoRegistradoViewModel
            {
                Id = r.Id,
                NombreExamen = r.NombExamen,
                Resultado = r.Resultado1,
                ValoresNormales = r.ValoresNormales,
                Unidad = r.Unidad,
                ValorReferencia = r.ValorReferencia
            })
            .ToList();



            // Crear el modelo de vista
            var model = new RegistrarResultadoViewModel
            {
                IdDetalleOrden = detalle.Id,
                PerfilNombre = detalle.PerfilExamen.Perfil.NombrePerfil,
                IdOrden = detalle.IdOrden ?? 0,
                Examenes = new List<ExamenResultadoViewModel>
                {
                    new ExamenResultadoViewModel
                    {
                        ExamenId = detalle.PerfilExamen.Examen.Id,
                        NombreExamen = detalle.PerfilExamen.Examen.NombreTipoExamen
                    }
                },
                ResultadosRegistrados = resultadosRegistrados
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult GuardarResultados(RegistrarResultadoViewModel model)
        {
            if (ModelState.IsValid)
            {
                foreach (var examen in model.Examenes)
                {

                    var validacion = ValidaInstancia(examen.NombreExamen, model, "RegistrarResultado");
                    if (validacion != null)
                    {
                        return validacion;
                    }


                    var resultado = new Resultado
                    {
                        NombExamen = examen.NombreExamen,
                        Resultado1 = examen.Resultado,
                        ValoresNormales = examen.ValoresNormales,
                        Unidad = examen.Unidad,
                        ValorReferencia = examen.ValorReferencia,
                        IdDetalleOrden = model.IdDetalleOrden,
                        estado = "A"                    
                    };

                    db.Resultado.Add(resultado);
                }

                try
                {
                    db.SaveChanges();
                    TempData["Success"] = "Resultados registrados exitosamente.";
                    return RedirectToAction("Index", new { id = model.IdOrden });
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            System.Diagnostics.Debug.WriteLine($"Propiedad: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                        }
                    }

                    // Opcional: Mostrar un mensaje de error amigable
                    TempData["Error"] = "Hubo un problema al guardar los resultados. Por favor verifica los datos ingresados.";
                    return View("RegistrarResultado", model);
                }

            }

            return View("RegistrarResultado", model);
        }
        private ActionResult ValidaInstancia<T>(string valorModelo, T model, string vista)
            where T : class
        {
            if (string.IsNullOrEmpty(valorModelo))
            {
                ModelState.AddModelError("", "El nombre del examen no puede estar vacío.");
                return View(vista, model);
            }

            return null; // Indica que la validación fue exitosa
        }

        [HttpPost]
        public ActionResult EliminarResultado(int id)
        {
            // Busca el resultado en la base de datos
            var resultado = db.Resultado.FirstOrDefault(r => r.Id == id);

            if (resultado != null)
            {
                // Actualiza el estado a "I" (inactivo)
                resultado.estado = "I";

                // Guarda los cambios en la base de datos
                db.SaveChanges();

                TempData["Success"] = "Resultado eliminado correctamente.";
            }
            else
            {
                TempData["Error"] = "No se encontró el resultado especificado.";
            }

            // Redirige a la vista de registrar resultados
            // Se asume que tienes un ViewBag con el IdDetalleOrden para redirigir correctamente
            return RedirectToAction("RegistrarResultado", new { idDetalleOrden = resultado?.IdDetalleOrden });
        }

        public ActionResult GetResultado(int IdResultado)
        {
            var resultado = db.Resultado.FirstOrDefault(r => r.Id == IdResultado && r.estado == "A");

            if (resultado == null)
            {
                return HttpNotFound();
            }
            return PartialView("Parciales/Resultado/ActualizarResultado", resultado);
        }

        [HttpPost]
        public ActionResult ActualizarResultado(Resultado model)
        {
            if (ModelState.IsValid)
            {
                var resultadoExistente = db.Resultado.FirstOrDefault(r => r.Id == model.Id);

                if (resultadoExistente != null)
                {
                    try
                    {
                        // Actualizar campos
                        resultadoExistente.Resultado1 = model.Resultado1;
                        resultadoExistente.ValoresNormales = model.ValoresNormales;
                        resultadoExistente.Unidad = model.Unidad;
                        resultadoExistente.ValorReferencia = model.ValorReferencia;

                        // Guardar cambios
                        db.SaveChanges();

                        TempData["Success"] = "Resultado actualizado correctamente.";
                    }
                    catch (DbEntityValidationException ex)
                    {
                        foreach (var validationErrors in ex.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                System.Diagnostics.Debug.WriteLine($"Propiedad: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                            }
                        }

                        TempData["Error"] = "Ocurrió un problema al guardar los cambios. Revisa los datos ingresados.";
                        return RedirectToAction("RegistrarResultado", new { idDetalleOrden = model.IdDetalleOrden });
                    }
                }
                else
                {
                    TempData["Error"] = "No se encontró el resultado.";
                }

                // Redirigir a la vista de registrar resultados
                return RedirectToAction("RegistrarResultado", new { idDetalleOrden = resultadoExistente.IdDetalleOrden });
            }

            TempData["Error"] = "Por favor, corrija los errores en el formulario.";
            return RedirectToAction("RegistrarResultado", new { idDetalleOrden = model.IdDetalleOrden });
        }




        [HttpGet]
        public FileStreamResult GenerarExamen(int OrdenId)
        {
            Empresa emp = db.Empresas.FirstOrDefault() ?? new Empresa();

            Orden orden = db.Orden.FirstOrDefault(x => x.Id == OrdenId);


            Document document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    // Configuración de la página
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial"));

                    // Encabezado del examen con el logo
                    page.Header().Column(column =>
                    {
                        column.Item().Row(row =>
                        {
                            if (emp.Logo != null && emp.Logo.Length > 0)
                            {
                                row.ConstantItem(100).Height(50).Element(imageContainer =>
                                {
                                    var imageDescriptor = imageContainer.Image(emp.Logo);
                                    imageDescriptor.FitArea();
                                });
                            }

                            row.RelativeItem().AlignCenter().Text(emp.NombreEmpresa)
                                .SemiBold().FontSize(24).FontColor(Colors.BlueGrey.Medium);
                        });

                        // Agregar margen inferior al Row
                        column.Item().PaddingBottom(20);

                        column.Item().Row(row =>
                           row.RelativeItem().AlignCenter().Text("Dirección: " + emp.Direccion).FontSize(12)
                            );

                        column.Item().PaddingBottom(10);


                        column.Item().Row(row => {
                            row.RelativeItem().AlignLeft().Text("Telefono: " + emp.Telefono1).FontSize(12);
                            if (emp.Telefono2 != null)
                                row.RelativeItem().AlignRight().Text("Telefono 2: " + emp.Telefono2).FontSize(12);

                        });
                        column.Item().PaddingBottom(20);


                        column.Item().PaddingBottom(5);
                    });



                    page.Content().Column(content =>
                    {

                        // Información del paciente

                        content.Item().Row(row => {
                            row.RelativeItem().AlignLeft().Text("DATOS DEL PACIENTE").FontSize(14).Bold();
                            row.RelativeItem().AlignRight().Text("Fecha: " + DateTime.Now.ToString("dd/MM/yyyy")).FontSize(14).Bold();
                        });

                        content.Item().PaddingBottom(5);

                        content.Item().Row(row =>
                        {
                            row.RelativeItem().AlignLeft().Text("Paciente: " + orden.Paciente.Nombres + " " + orden.Paciente.Apellidos);

                        });

                        if (orden.Paciente.FechaNacimiento != null)
                        {
                            content.Item().PaddingBottom(5);

                            content.Item().Row(row =>
                            {

                                if (orden.Paciente.Sexo != null)
                                {
                                    row.RelativeItem().AlignLeft().Text("Edad: " + db.sp_edad(orden.Paciente.FechaNacimiento).FirstOrDefault().Value + " años    " + "Sexo: " + orden.Paciente.Sexo);
                                }
                                else
                                {
                                    row.RelativeItem().AlignLeft().Text("Edad: " + db.sp_edad(orden.Paciente.FechaNacimiento).FirstOrDefault().Value);
                                }
                            });
                        }
                        else
                        {
                            if (orden.Paciente.Sexo != null)
                            {
                                content.Item().PaddingBottom(5);

                                content.Item().Row(row =>
                                {
                                    row.RelativeItem().AlignLeft().Text("Sexo: " + orden.Paciente.Sexo);
                                });
                            }
                        }


                        content.Item().PaddingBottom(5);

                        content.Item().Row(row =>
                        {

                            row.ConstantItem(200).Text("Doctor: " + orden.Doctor.Nombres + " " + orden.Doctor.Apellidos);
                            row.RelativeItem().AlignRight().Text("No. Examen: " + orden.Id);
                        });

                        // Espaciado
                        content.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        // Resultados del examen
                        content.Item().Container().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3); // Columna más ancha
                                columns.RelativeColumn(2); // Mediana
                                columns.RelativeColumn(2); // Mediana
                                columns.RelativeColumn(3); // Columna más ancha
                            });

                            // Encabezado de la tabla
                            table.Header(header =>
                            {
                                header.Cell().Text("Examen").SemiBold().FontSize(12);
                                header.Cell().Text("Resultado").SemiBold().FontSize(12);
                                header.Cell().Text("Valores normales").SemiBold().FontSize(12);
                                header.Cell().Text("Valores de referencia").SemiBold().FontSize(12);
                                header.Cell().Background(Colors.Grey.Lighten3);
                            });

                            // Datos de la tabla

                            foreach (DetalleOrden detOrden in orden.DetalleOrden)
                            {
                                foreach (Resultado resul in detOrden.Resultado.Where(x=> x.estado=="A"))
                                {
                                    table.Cell().Text(resul.NombExamen).FontSize(12);
                                    table.Cell().Text(resul.Resultado1).FontSize(12);
                                    table.Cell().Text(resul.ValoresNormales).FontSize(12);
                                    table.Cell().Text(resul.ValorReferencia).FontSize(12);
                                }

                            }
                        });
                    });



                    page.Footer().Column(column =>
                    {
                        column.Item().Row(row => {
                            row.RelativeItem().AlignCenter().Text(emp.Leyenda).Italic();
                        });
                        column.Item().Row(row => {
                            row.RelativeItem().AlignRight().Text(text =>
                            {
                                text.Span("Página ");
                                text.CurrentPageNumber();
                                text.Span(" de ");
                                text.TotalPages();
                            });
                        });
                    });

                });
            });

            // Generar el PDF en memoria
            byte[] pdfBytes = document.GeneratePdf();
            MemoryStream ms = new MemoryStream(pdfBytes);
            return new FileStreamResult(ms, "application/pdf");
        }


    }

}
