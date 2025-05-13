using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using SistemaLaboratorioEnmanuel.Models;

namespace SistemaLaboratorioEnmanuel.Controllers
{
    [Authorize]
    public class EmpresaController : Controller
    {
        private LaboratorioBDEntities db = new LaboratorioBDEntities();

        // GET: Empresa
        public ActionResult Index()
        {
            Empresa emp = db.Empresas.FirstOrDefault() ?? new Empresa();
            return View(emp);
        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Add(Empresa modelo)
        {
            HttpPostedFileBase http = Request.Files[0];

            if (modelo.Id == 0) 
            {
                if (http.ContentLength > 0)
                {
                    WebImage webImage = new WebImage(http.InputStream);
                    modelo.Logo = webImage.GetBytes();
                }

                db.Empresas.Add(modelo);
            }
            else 
            {
                var empresaExistente = db.Empresas.FirstOrDefault(e => e.Id == modelo.Id);

                if (empresaExistente != null)
                {
                    empresaExistente.NombreEmpresa = modelo.NombreEmpresa;
                    empresaExistente.Direccion = modelo.Direccion;
                    empresaExistente.Leyenda = modelo.Leyenda;
                    empresaExistente.Telefono1 = modelo.Telefono1;
                    empresaExistente.Telefono2 = modelo.Telefono2;


                    if (http.ContentLength > 0)
                    {
                        WebImage webImage = new WebImage(http.InputStream);
                        empresaExistente.Logo = webImage.GetBytes();
                    }

                    db.Entry(empresaExistente).State = EntityState.Modified;
                }
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }


        // GET: Empresa/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empresa empresa = db.Empresas.Find(id);
            if (empresa == null)
            {
                return HttpNotFound();
            }
            return View(empresa);
        }

        // POST: Empresa/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NombreEmpresa,Direccion,Telefono1,Telefono2,Leyenda,Logo,Id")] Empresa empresa)
        {
            if (ModelState.IsValid)
            {
                db.Empresas.Add(empresa);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(empresa);
        }

        // GET: Empresa/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empresa empresa = db.Empresas.Find(id);
            if (empresa == null)
            {
                return HttpNotFound();
            }
            return View(empresa);
        }

        // POST: Empresa/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NombreEmpresa,Direccion,Telefono1,Telefono2,Leyenda,Logo,Id")] Empresa empresa)
        {
            if (ModelState.IsValid)
            {
                db.Entry(empresa).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(empresa);
        }

        // GET: Empresa/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empresa empresa = db.Empresas.Find(id);
            if (empresa == null)
            {
                return HttpNotFound();
            }
            return View(empresa);
        }

        // POST: Empresa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Empresa empresa = db.Empresas.Find(id);
            db.Empresas.Remove(empresa);
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
