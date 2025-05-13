using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SistemaLaboratorioEnmanuel.Models;

namespace SistemaLaboratorioEnmanuel.Controllers
{
    [Authorize]
    public class AbonoesController : Controller
    {
        private LaboratorioBDEntities db = new LaboratorioBDEntities();

        // GET: Abonoes
        public ActionResult Index()
        {
            var abono = db.Abono.Include(a => a.Factura);
            return View(abono.ToList());
        }

        // GET: Abonoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Abono abono = db.Abono.Find(id);
            if (abono == null)
            {
                return HttpNotFound();
            }
            return View(abono);
        }

        // GET: Abonoes/Create
        public ActionResult Create()
        {
            ViewBag.IdFactura = new SelectList(db.Factura, "Id", "Descripcion");
            return View();
        }

        // POST: Abonoes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Monto,FechaAbono,IdFactura")] Abono abono)
        {
            if (ModelState.IsValid)
            {
                db.Abono.Add(abono);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdFactura = new SelectList(db.Factura, "Id", "Descripcion", abono.IdFactura);
            return View(abono);
        }

        // GET: Abonoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Abono abono = db.Abono.Find(id);
            if (abono == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdFactura = new SelectList(db.Factura, "Id", "Descripcion", abono.IdFactura);
            return View(abono);
        }

        // POST: Abonoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Monto,FechaAbono,IdFactura")] Abono abono)
        {
            if (ModelState.IsValid)
            {
                db.Entry(abono).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdFactura = new SelectList(db.Factura, "Id", "Descripcion", abono.IdFactura);
            return View(abono);
        }

        // GET: Abonoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Abono abono = db.Abono.Find(id);
            if (abono == null)
            {
                return HttpNotFound();
            }
            return View(abono);
        }

        // POST: Abonoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Abono abono = db.Abono.Find(id);
            db.Abono.Remove(abono);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
