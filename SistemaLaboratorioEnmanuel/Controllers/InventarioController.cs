using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SistemaLaboratorioEnmanuel.Models;
using ClosedXML.Excel;
using System.IO;
using System.Data.Entity;
using SistemaLaboratorioEnmanuel.Models.ViewModels;

namespace LaboratorioEnmanuel.Controllers
{
    public class InventarioController : Controller
    {
        private LaboratorioBDEntities db = new LaboratorioBDEntities();
        // GET: Inventario
        public ActionResult Index()
        {
            var listaInventario = db.sp_Insumos().ToList();

            return View(db.sp_Insumos().ToList());
        }

        public ActionResult RegistrarSalida(int? IdOrden, int? idDetalleOrden, int? idResultado)
        {
            if (!idResultado.HasValue)
            {
                TempData["Error"] = "Debe especificar un resultado.";
                return RedirectToAction("Index");
            }

            // Obtener información del resultado asociado
            var resultado = db.Resultado.FirstOrDefault(r => r.Id == idResultado.Value);

            if (resultado == null)
            {
                TempData["Error"] = "No se encontró el resultado.";
                return RedirectToAction("Index");
            }

            // Filtrar insumos con stock > 0
            var insumosDisponibles = db.sp_Insumos()
                .Where(i => i.Stock > 0)
                .ToList();


            var insumosSeleccionados = db.Salida
            .Where(s => s.IdResultado == idResultado.Value)
            .Select(s => new InsumoViewModel
            {
                Id = s.IdInsumos ?? 0,
                Nombre = s.Insumos.Nombre,
                Stock = s.Insumos.Stock ?? 0,
                Cantidad = s.Cantidad
            })
            .ToList();

            // Crear el modelo para la vista
            var viewModel = new RegistrarSalidaViewModel
            {
                IdOrden = IdOrden.Value,
                IdDetalleOrden = idDetalleOrden.Value,
                IdResultado = resultado.Id,
                NombreResultado = resultado.NombExamen,
                InsumosDisponibles = insumosDisponibles,
                InsumosSeleccionados = insumosSeleccionados
            };

            return View(viewModel);
        }


        /* [HttpPost]
         public ActionResult GuardarSalida(Salida model)
         {
             if (ModelState.IsValid)
             {
                 var insumo = db.Insumos.FirstOrDefault(i => i.Id == model.IdInsumos);

                 if (insumo == null)
                 {
                     TempData["Error"] = "El insumo no existe.";
                     return RedirectToAction("Index");
                 }

                 if (model.Cantidad > insumo.Stock)
                 {
                     TempData["Error"] = "La cantidad a retirar excede el stock disponible.";
                     return RedirectToAction("RegistrarSalida", new { id = model.IdInsumos });
                 }

                 // Actualizar el stock del insumo
                 insumo.Stock -= model.Cantidad;

                 // Registrar la salida
                 model.FechaSalida = DateTime.Now; // Fecha de la salida
                 db.Salida.Add(model);

                 // Guardar los cambios
                 db.SaveChanges();

                 TempData["Success"] = "Salida registrada correctamente.";
                 return RedirectToAction("Index");
             }

             TempData["Error"] = "Por favor, corrija los errores en el formulario.";
             return RedirectToAction("RegistrarSalida", new { id = model.IdInsumos });
         }*/


        [HttpPost]
        public ActionResult GuardarSalida(RegistrarSalidaViewModel model)
        {
            if (model.InsumosSeleccionados == null || !model.InsumosSeleccionados.Any())
            {
                TempData["Error"] = "Debe seleccionar al menos un insumo.";
                return RedirectToAction("RegistrarSalida", new { idResultado = model.IdResultado });
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var insumo in model.InsumosSeleccionados)
                    {
                        // Validar insumo y stock
                        var insumoDb = db.Insumos.FirstOrDefault(i => i.Id == insumo.Id);
                        if (insumoDb == null || insumoDb.Stock < insumo.Cantidad)
                        {
                            TempData["Error"] = $"El insumo '{insumo.Nombre}' no tiene suficiente stock.";
                            transaction.Rollback();
                            return RedirectToAction("RegistrarSalida", new { idResultado = model.IdResultado });
                        }

                        // Crear registro de salida
                        var salida = new Salida
                        {
                            IdInsumos = insumo.Id,
                            Cantidad = insumo.Cantidad,
                            FechaSalida = DateTime.Now,
                            IdResultado = model.IdResultado
                        };

                        db.Salida.Add(salida);

                        // Actualizar stock del insumo
                        insumoDb.Stock -= insumo.Cantidad;

                        db.Entry(insumoDb).State = EntityState.Modified;
                    }

                    // Guardar los cambios y confirmar la transacción
                    db.SaveChanges();
                    transaction.Commit();

                    TempData["Success"] = "Salidas registradas correctamente.";
                    return RedirectToAction("RegistrarResultado", "Resultado", new { idDetalleOrden = model.IdDetalleOrden });
                }
                catch (Exception ex)
                {
                    // Si hay un error, revertir todos los cambios
                    transaction.Rollback();
                    TempData["Error"] = "Ocurrió un error al registrar las salidas. Inténtelo nuevamente.";
                    System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                    return RedirectToAction("RegistrarSalida", new { idResultado = model.IdResultado });
                }
            }
        }




        // GET: Inventario/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Inventario/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Inventario/Create
        [HttpPost]
        public ActionResult Create([Bind(Include = "Id,CodigoBarra,Nombre,Descripcion,IdProveedor,UnidadMedida,Presentacion,Sobrante")] Insumos insumo)
        {
            try
            {
                insumo.Stock = 0;
                db.Insumos.Add(insumo);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public PartialViewResult ListaProveedores()
        {
            var proveedores = db.Proveedor.Where(x => x.Estado == "A").ToList();
            return PartialView("Parciales/TablaProveedorInsumos", proveedores);
        }

        [HttpPost]
        public ActionResult AgregarEntada([Bind(Include = "Id,Cantidad,Costo,IdInsumo")] DetalleEntrada detalle)
        {
            try
            {
                if (detalle.Cantidad > 0)
                {
                    Insumos insumo;
                    detalle.FechaEntrada = DateTime.Now;
                    db.DetalleEntrada.Add(detalle);
                    if (db.SaveChanges() > 0)
                    {
                        insumo = db.Insumos.FirstOrDefault(x => x.Id == detalle.IdInsumo);
                        insumo.Stock += detalle.Cantidad;
                        db.Entry(insumo).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }


                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Inventario/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Inventario/Edit/5
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

        // GET: Inventario/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Inventario/Delete/5
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
    }
}
