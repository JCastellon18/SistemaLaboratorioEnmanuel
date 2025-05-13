using SistemaLaboratorioEnmanuel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SistemaLaboratorioEnmanuel.Util;

namespace SistemaLaboratorioEnmanuel.Controllers
{
    [Authorize]
    public class PerfilController : Controller
    {
        private LaboratorioBDEntities db = new LaboratorioBDEntities();
        public ActionResult Index()
        {
            try
            {
                var ListaPerfiles = db.Perfil.Where(x => x.Estado == "A").OrderByDescending(x=> x.FechaCreacion).ToList();
                return View(ListaPerfiles);
            }
            catch
            {
                return View();
            }
        }


        // GET: Perfil/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        [HttpPost]
        public PartialViewResult ListaAreas()
        {
            var areas = db.Area.ToList();
            return PartialView("Parciales/TablaPerfilAreas", areas);
        }

        

        public PartialViewResult ListaExamenesPerfil(int PerfilId)
        {
            var ListaExamenesxPerfil = db.PerfilExamen.Where(x => x.PerfilId == PerfilId).ToList();

            return PartialView("Parciales/TablaPerfilExamenesPorPerfil", ListaExamenesxPerfil);
        }

        [HttpPost]
        public PartialViewResult ListaExamenesArea(int AreaId)
        {
            var ListaExamenesxArea = db.Examen.Where(x => x.IdArea == AreaId).ToList();

            return PartialView("Parciales/TablaPerfilExamenesPorArea", ListaExamenesxArea);
        }

        [HttpPost]
        public JsonResult AddExamenTemp(int ExamenId)
        {
            int cantidad = 0;
            try
            {
                cantidad = ListUtil.ExamenesPerfilTemp.Count(x => x == ExamenId);
            }
            catch (Exception e)
            {
                cantidad = 0;
            }
           
            if(cantidad == 0)
            {
                ListUtil.ExamenesPerfilTemp.Add(ExamenId);
            }

            return Json(cantidad);
        }

        [HttpPost]
        public JsonResult RemoveExamenTemp(int ExamenId)
        {
            int cantidad = 0;
            try
            {
                cantidad = ListUtil.ExamenesPerfilTemp.Count(x => x == ExamenId);
            }
            catch (Exception e)
            {
                cantidad = 0;
            }

            if (cantidad == 1)
            {
                ListUtil.ExamenesPerfilTemp.Remove(ExamenId);
            }

            return Json(cantidad);
        }

        // GET: Perfil/Create
        public ActionResult Create()
        {
            return View();
        }



        // POST: Perfil/Create
        [HttpPost]
        public ActionResult Create(Perfil perfil)
        {
            if(ListUtil.ExamenesPerfilTemp.Count == 0)
            {
                return RedirectToAction("Create");
            }


            perfil.FechaCreacion = DateTime.Now;
            perfil.Estado = "A";
            try
            {
                db.Perfil.Add(perfil);
                if (db.SaveChanges() > 0)
                {
                    ListUtil.ExamenesPerfilTemp.ForEach(x=> {
                        PerfilExamen item = new PerfilExamen();
                        item.ExamenId = x;
                        item.PerfilId = perfil.Id;
                        db.PerfilExamen.Add(item);
                    });
                    db.SaveChanges();

                }
            }
            catch 
            {
                return RedirectToAction("Create");
            }

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

        // GET: Perfil/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Perfil/Edit/5
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

        // GET: Perfil/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Perfil/Delete/5
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
