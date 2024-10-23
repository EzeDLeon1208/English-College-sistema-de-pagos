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
    public class CursoController : Controller
    {
        private EnglishCollegeDbContext db = new EnglishCollegeDbContext();

        // GET: Curso
        public ActionResult Index(string buscar)
        {
            var curso = from Curso in db.Curso select Curso;

            if (!String.IsNullOrEmpty(buscar))
            {
                curso = curso.Where(s => s.Nombre.Contains(buscar));
            }
            return View(curso.ToList());
        }

        // GET: Curso/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Curso curso = db.Curso.Find(id);
        //    if (curso == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(curso);
        //}

        // GET: Curso/Create
        public ActionResult Create()
        {
            //ViewBag.idCuento = new SelectList(db.Cuento, "Id", "Nombre");
            //ViewBag.idLibro = new SelectList(db.Libro, "Id", "Nombre");
            return View();
        }

        // POST: Curso/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nombre,Precio,PrecioLibro,idLibro,idCuento")] Curso curso)
        {
            if (ModelState.IsValid)
            {
                db.Curso.Add(curso);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.idCuento = new SelectList(db.Cuento, "Id", "Nombre", curso.idCuento);
            //ViewBag.idLibro = new SelectList(db.Libro, "Id", "Nombre", curso.idLibro);
            return View(curso);
        }

        // GET: Curso/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Curso curso = db.Curso.Find(id);
            if (curso == null)
            {
                return HttpNotFound();
            }
            //ViewBag.idCuento = new SelectList(db.Cuento, "Id", "Nombre", curso.idCuento);
            //ViewBag.idLibro = new SelectList(db.Libro, "Id", "Nombre", curso.idLibro);
            return View(curso);
        }

        // POST: Curso/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nombre,Precio,PrecioLibro,idLibro,idCuento")] Curso curso)
        {
            if (ModelState.IsValid)
            {
                db.Entry(curso).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.idCuento = new SelectList(db.Cuento, "Id", "Nombre", curso.idCuento);
            //ViewBag.idLibro = new SelectList(db.Libro, "Id", "Nombre", curso.idLibro);
            return View(curso);
        }

        // GET: Curso/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Curso curso = db.Curso.Find(id);
            if (curso == null)
            {
                return HttpNotFound();
            }
            return View(curso);
        }

        // POST: Curso/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Curso curso = db.Curso.Find(id);
            db.Curso.Remove(curso);
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
