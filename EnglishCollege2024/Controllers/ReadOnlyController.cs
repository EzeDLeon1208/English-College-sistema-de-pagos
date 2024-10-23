using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace EnglishCollege2024.Controllers
{
    public class ReadOnlyController : Controller
    {
        private EnglishCollegeDbContext db = new EnglishCollegeDbContext();
        

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Estudiante");
        }

        // GET: ReadOnly/Details/5
        public ActionResult ReadOnly(int estudianteId)
        {
            Estudiante estudiante = db.Estudiante.Find(estudianteId);
            if (estudiante == null)
            {
                return HttpNotFound();
            }

            // Combo barrios
            List<SelectListItem> barrios = new List<SelectListItem>();

            List<Barrio> Barriodb = db.Barrio.ToList();

            foreach (var item in Barriodb)
            {
                SelectListItem barrio = new SelectListItem();
                barrio.Text = item.Nombre.ToString();
                barrio.Value = item.Id.ToString();
                barrio.Selected = false;
                barrios.Add(barrio);
            }

            ViewBag.barrios = barrios;

            // Combo Cursos
            List<SelectListItem> cursos = new List<SelectListItem>();

            List<Curso> Cursodb = db.Curso.ToList();

            foreach (var item in Cursodb)
            {
                SelectListItem curso = new SelectListItem();
                curso.Text = item.Nombre.ToString();
                curso.Value = item.Id.ToString();
                curso.Selected = false;
                cursos.Add(curso);
            }

            ViewBag.Cursos = cursos;

            HttpContext.Session["idEst"] = estudiante.Id;

            //return RedirectToAction("ReadOnly", "Estudiante", new { idEstudiante = estudiante.Id });
            return View(estudiante);
        }

        public ActionResult EditEstudiante(int estudianteId)
        {
            Estudiante estudiante = db.Estudiante.Find(estudianteId);
            if (estudiante == null)
            {
                return HttpNotFound();
            }

            HttpContext.Session["idEst"] = estudiante.Id;

            // Combo barrios
            List<SelectListItem> barrios = new List<SelectListItem>();

            List<Barrio> Barriodb = db.Barrio.ToList();

            foreach (var item in Barriodb)
            {
                SelectListItem barrio = new SelectListItem();
                barrio.Text = item.Nombre.ToString();
                barrio.Value = item.Id.ToString();
                barrio.Selected = false;
                barrios.Add(barrio);
            }

            ViewBag.barrios = barrios;

            // Combo Cursos
            List<SelectListItem> cursos = new List<SelectListItem>();

            List<Curso> Cursodb = db.Curso.ToList();

            foreach (var item in Cursodb)
            {
                SelectListItem curso = new SelectListItem();
                curso.Text = item.Nombre.ToString();
                curso.Value = item.Id.ToString();
                curso.Selected = false;
                cursos.Add(curso);
            }

            ViewBag.Cursos = cursos;

            return View(estudiante);
        }

        public ActionResult GenerarPago(int estudianteId)
        {
            Estudiante estudiante = db.Estudiante.Find(estudianteId);
            if (estudiante == null)
            {
                return HttpNotFound();
            }

            return RedirectToAction("GenerarPago", "Estudiante", new { id = estudiante.Id });
        }

        // POST: ReadOnly/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,DNI,NombreCompleto,FechaNacimiento,Direccion,Telefono1,Telefono2,idBarrio,idCurso,PendientePago,TieneDeuda")] Estudiante estudiante)
        {
            if (ModelState.IsValid)
            {
                estudiante.Activo = true;
                db.Entry(estudiante).State = EntityState.Modified;
                db.SaveChanges();

                HttpContext.Session["idEstudiante"] = estudiante.Id;
                var estudianteId = HttpContext.Session["idEstudiante"];
                ViewBag.idEstudiante = HttpContext.Session["idEstudiante"];

                return RedirectToAction("ReadOnly", "ReadOnly", new { estudianteId });
            }
            //ViewBag.idBarrio = new SelectList(db.Barrio, "Id", "Nombre", estudiante.idBarrio);
            //ViewBag.idCurso = new SelectList(db.Curso, "Id", "Nombre", estudiante.idCurso);
            return View(estudiante);
        }

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
