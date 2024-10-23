using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EnglishCollege2024;

namespace EnglishCollege2024.Controllers
{
    public class ConceptoController : Controller
    {
        private EnglishCollegeDbContext db = new EnglishCollegeDbContext();

        // GET: Concepto
        public ActionResult ListaConcepto(string buscar)
        {
            var conceptos = from Concepto in db.Concepto select Concepto;

            if (!String.IsNullOrEmpty(buscar))
            {
                conceptos = conceptos.Where(s => s.Nombre.Contains(buscar));
            }
            return View(conceptos.ToList());
        }

        // GET: Concepto/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Concepto concepto = db.Concepto.Find(id);
        //    if (concepto == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(concepto);
        //}

        // GET: Concepto/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Concepto/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nombre,Precio")] Concepto concepto)
        {
            if (ModelState.IsValid)
            {
                db.Concepto.Add(concepto);
                db.SaveChanges();
                return RedirectToAction("ListaConcepto");
            }

            return View(concepto);
        }

        // GET: Concepto/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Concepto concepto = db.Concepto.Find(id);
            if (concepto == null)
            {
                return HttpNotFound();
            }
            return View(concepto);
        }

        // POST: Concepto/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nombre,Precio")] Concepto concepto)
        {
            if (ModelState.IsValid)
            {
                db.Entry(concepto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ListaConcepto");
            }
            return View(concepto);
        }

        // GET: Concepto/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Concepto concepto = db.Concepto.Find(id);
            if (concepto == null)
            {
                return HttpNotFound();
            }
            return View(concepto);
        }

        // POST: Concepto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Concepto concepto = db.Concepto.Find(id);
            db.Concepto.Remove(concepto);
            db.SaveChanges();
            return RedirectToAction("ListaConcepto");
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
