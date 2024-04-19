using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EnglishCollege2024.Controllers
{
    public class DeudaController : Controller
    {
        private EnglishCollegeDbContext db = new EnglishCollegeDbContext();
        // GET: Deuda
        public ActionResult DetalleDeudas()
        {
            List<SelectListItem> conceptos = new List<SelectListItem>();

            List<Concepto> Conceptodb = db.Concepto.ToList();
            
            foreach (var item in Conceptodb)
            {
                SelectListItem concepto = new SelectListItem();
                concepto.Text = item.Nombre.ToString();
                concepto.Value = item.Id.ToString();
                conceptos.Add(concepto);
            }

            ViewBag.Conceptos = conceptos;

            List<DetalleDeudas> listaDeudas = new List<DetalleDeudas>();

            // Busco estudiante que de su Id con idEstudiante de tabla cobro
            List<Estudiante> estDeu = db.Estudiante.ToList();
            if (estDeu != null && estDeu.Count() > 0)
            {
                foreach(var item in estDeu)
                {
                    if (item.TieneDeuda == true)
                    {
                        // Busca los cobros relacionados con el estudiante
                        List<Cobro> cobros = db.Cobro.Where(c => c.idEstudiante == item.Id && c.Deuda > 0).ToList();

                        foreach (var cobro in cobros)
                        {
                            // Obtén el concepto relacionado con el cobro
                            Concepto concepto = db.Concepto.FirstOrDefault(con => con.Id == cobro.idConcepto);

                            if (concepto != null)
                            {
                                // Crea un objeto Deudas para cada cobro
                                DetalleDeudas detDeudas = new DetalleDeudas();
                                detDeudas.id = cobro.Id;
                                detDeudas.NombreCompleto = item.NombreCompleto.ToString();
                                detDeudas.DNI = item.DNI;
                                detDeudas.TieneDeuda = item.TieneDeuda;
                                detDeudas.Concepto = concepto.Nombre.ToString();
                                detDeudas.deudaPorConcepto = cobro.Deuda;
                                listaDeudas.Add(detDeudas);
                            }
                        }
                    }
                }
            }

            ViewBag.listaDeudas = listaDeudas;

            return View();
        }

        [HttpPost]
        public ActionResult DeudasPorConcepto(DetalleDeudas idDetalle)
        {
            List<DetalleDeudas> listaDeudas = new List<DetalleDeudas>();

            // Busco estudiante que de su Id con idEstudiante de tabla cobro
            List<Estudiante> estDeu = db.Estudiante.ToList();
            if (estDeu != null && estDeu.Count() > 0)
            {
                foreach (var item in estDeu)
                {
                    if (item.TieneDeuda == true)
                    {
                        // Busca los cobros relacionados con el estudiante
                        List<Cobro> cobros = db.Cobro.Where(c => c.idEstudiante == item.Id && c.Deuda > 0).ToList();

                        foreach (var cobro in cobros)
                        {
                            // Obtén el concepto relacionado con el cobro
                            Concepto concepto = db.Concepto.FirstOrDefault(con => con.Id == cobro.idConcepto);

                            if (concepto.Id != null && concepto.Id == idDetalle.idConcepto)
                            {
                                // Crea un objeto Deudas para cada cobro
                                DetalleDeudas detDeudas = new DetalleDeudas();
                                detDeudas.id = cobro.Id;
                                detDeudas.NombreCompleto = item.NombreCompleto.ToString();
                                detDeudas.DNI = item.DNI;
                                detDeudas.TieneDeuda = item.TieneDeuda;
                                detDeudas.Concepto = concepto.Nombre.ToString();
                                detDeudas.deudaPorConcepto = cobro.Deuda;

                                listaDeudas.Add(detDeudas);
                            }
                            else if(idDetalle.idConcepto == 0)
                            {
                                DetalleDeudas();

                                return View("DetalleDeudas");
                            }
                        }
                    }
                }
            }

            ViewBag.listaDeudas = listaDeudas;

            List<SelectListItem> conceptos = new List<SelectListItem>();

            List<Concepto> Conceptodb = db.Concepto.ToList();

            foreach (var item in Conceptodb)
            {
                SelectListItem concepto = new SelectListItem();
                concepto.Text = item.Nombre.ToString();
                concepto.Value = item.Id.ToString();
                conceptos.Add(concepto);
            }

            ViewBag.Conceptos = conceptos;

            return View("DetalleDeudas");
        }

        //// GET: Deuda/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: Deuda/Delete/5
        //[HttpPost]
        //public ActionResult Delete(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
