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
    public class EstudianteController : Controller
    {
        private EnglishCollegeDbContext db = new EnglishCollegeDbContext();

        // GET: Estudiante
        public ActionResult Index(string buscar)
        {
            //var estudiante = db.Estudiante.Include(e => e.Barrio).Include(e => e.Curso);
            var estudiantes = from Estudiante in db.Estudiante select Estudiante;

            if (!String.IsNullOrEmpty(buscar))
            {
                estudiantes = estudiantes.Where(s => s.NombreCompleto.Contains(buscar));
            }

            HttpContext.Session["PagoNuevo"] = null;


            //return View(es.ToListAsync())
            return View(estudiantes.ToList());
        }

        // GET: Estudiante/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estudiante estudiante = db.Estudiante.Find(id);
            if (estudiante == null)
            {
                return HttpNotFound();
            }

            List<DetallePagos> detPagos = new List<DetallePagos>();

            //buscamos IdEstudiante de tabla Cobro
            List<Cobro> cob = db.Cobro.Where(x => x.idEstudiante == estudiante.Id).ToList();
            //int idCobro = 0;
            string nombconcep = "";
            if (cob != null && cob.Count > 0)
            {
                List<DetallePagos> pagos = new List<DetallePagos>();
                foreach (var it in cob)
                {
                    List<Concepto> concep = db.Concepto.Where(x => x.Id == it.idConcepto).ToList();
                    if (concep != null && concep.Count > 0)
                    {
                        foreach (var item in concep)
                        {
                            nombconcep = item.Nombre.ToString();

                            DetallePagos detalPagos = new DetallePagos();
                            detalPagos.Concepto = nombconcep;
                            detalPagos.idCobro = it.Id;
                            detalPagos.importeTotal = (decimal)it.precioTotal;
                            detalPagos.FechaPago = it.FechaPago;
                            detalPagos.Activo = it.Activo;
                            pagos.Add(detalPagos);
                        }
                    }
                }
                // Agregar la lista pagos a detPagos fuera del bucle
                detPagos.AddRange(pagos);
            }

            ViewBag.detallePagos = detPagos;

            return View(estudiante);
        }

        public ActionResult Deudas(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var cobro = db.Cobro.Where(c => c.idEstudiante == id).FirstOrDefault();
            if (cobro == null)
            {
                return HttpNotFound();
            }

            Session["ListaDeudasCancel"] = null;

            List<Deudas> deudasTot = new List<Deudas>();

            //Session["ListaDeudas"] == null;

            bool debe = false;
            string nombconcep = "";

            List<Estudiante> estDeu = db.Estudiante.Where(x => x.Id == cobro.idEstudiante).ToList();
            if (estDeu != null && estDeu.Count() > 0)
            {
                var est = estDeu.FirstOrDefault();

                List<Deudas> debeP = new List<Deudas>();

                List<Cobro> cobroD = db.Cobro.Where(x => x.idEstudiante == est.Id).ToList();
                if (cobroD != null && cobroD.Count() > 0)
                {
                    foreach (var item in cobroD)
                    {
                        if (item.Deuda > 0)
                        {
                            List<Concepto> concep = db.Concepto.Where(x => x.Id == item.idConcepto).ToList();
                            if (concep != null && concep.Count > 0)
                            {
                                foreach (var it in concep)
                                {
                                    nombconcep = it.Nombre.ToString();

                                    Deudas deuda = new Deudas();
                                    deuda.id = item.Id;
                                    deuda.Concepto = nombconcep;
                                    deuda.deudaPorConcepto = (decimal)item.Deuda;
                                    deuda.TotalAdeudado += item.Deuda;
                                    deuda.Activo = true;

                                    debeP.Add(deuda);
                                }
                            }
                        }
                    }
                    deudasTot.AddRange(debeP);
                }
            }

            Session["ListaDeudas"] = deudasTot;

            var ListaDeuda = (List<Deudas>)Session["ListaDeudas"];

            decimal sumaTotal = 0;

            foreach (var item in ListaDeuda)
            {
                decimal importe = 0;
                importe = item.deudaPorConcepto;

                sumaTotal += importe;
            }

            ViewBag.SumaTotal = sumaTotal;

            ViewBag.DeudasTot = deudasTot;

            HttpContext.Session["idEstudiante"] = id;
            //}

            return View();
        }

        public ActionResult CancelarDeudas(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Cobro cobro = db.Cobro.Find(id);
            if (cobro == null)
            {
                return HttpNotFound();
            }

            Session["Cobro"] = cobro;

            var deudasTot = (List<Deudas>)Session["ListaDeudas"];

            List<CancelarDeudas> deudaCancel = new List<CancelarDeudas>();

            // si la sesion de pagos tiene datos, cargame los pagos con la lista de sesion
            if (HttpContext.Session["ListaDeudasCancel"] != null) deudaCancel = (List<CancelarDeudas>)Session["ListaDeudasCancel"];

            //bool debe = false;
            string nombconcep = "";

            List<Estudiante> estDeu = db.Estudiante.Where(x => x.Id == cobro.idEstudiante).ToList();
            if (estDeu != null && estDeu.Count() > 0)
            {
                var est = estDeu.FirstOrDefault();

                List<CancelarDeudas> deudasCan = new List<CancelarDeudas>();

                List<Cobro> cobroD = db.Cobro.Where(x => x.idEstudiante == est.Id).ToList();
                if (cobroD != null && cobroD.Count() > 0)
                {
                    var deudaSelec = db.Cobro.Find(id);
                    {
                        if (deudaSelec.Deuda > 0)
                        {
                            List<Concepto> concep = db.Concepto.Where(x => x.Id == deudaSelec.idConcepto).ToList();
                            if (concep != null && concep.Count > 0)
                            {
                                foreach (var it in concep)
                                {
                                    nombconcep = it.Nombre.ToString();

                                    CancelarDeudas cancelDeud = new CancelarDeudas();
                                    cancelDeud.id = deudaSelec.Id;
                                    cancelDeud.Concepto = nombconcep;
                                    cancelDeud.deudaPorConcepto = (decimal)deudaSelec.Deuda;
                                    cancelDeud.TotalAdeudado += deudaSelec.Deuda;
                                    cancelDeud.Activo = true;

                                    deudasCan.Add(cancelDeud);
                                }
                            }
                            deudaCancel.AddRange(deudasCan);

                            // Eliminar el objeto seleccionado de la lista original
                            var cancelarDeuda = deudasTot.FirstOrDefault(d => d.id == id);
                            if (cancelarDeuda != null)
                            {
                                cancelarDeuda.Activo = false;
                                deudasTot.Remove(cancelarDeuda);
                            }

                            // Actualizar la lista original en ViewBag
                            ViewBag.DeudasTot = deudasTot;
                        }

                    }
                }
            }

            Session["ListaDeudasCancel"] = deudaCancel;

            var ListaDeudaCancel = (List<CancelarDeudas>)Session["ListaDeudasCancel"];

            decimal sumaTotalCancel = 0;

            foreach (var item in ListaDeudaCancel)
            {
                decimal importe = 0;
                importe = item.deudaPorConcepto;

                sumaTotalCancel += importe;
            }

            decimal sumaDeudas = 0;
            if (deudasTot != null && deudasTot.Count() > 0)
            {
                foreach (var item in deudasTot)
                {
                    if (item.Activo == true)
                    {
                        decimal importe = 0;
                        importe = item.deudaPorConcepto;

                        sumaDeudas += importe;
                    }
                }
            }

            ViewBag.SumaTotal = sumaDeudas;

            ViewBag.SumaTotalCancel = sumaTotalCancel;

            ViewBag.DeudasTotCancel = deudaCancel;
            //}

            return View("Deudas");
        }

        public ActionResult Anular(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Cobro cobro = db.Cobro.Find(id);
            if (cobro == null)
            {
                return HttpNotFound();
            }

            List<Deudas> deudasTot = new List<Deudas>();

            // si la sesion de pagos tiene datos, cargame los pagos con la lista de sesion
            if (HttpContext.Session["ListaDeudas"] != null) deudasTot = (List<Deudas>)Session["ListaDeudas"];

            string nombconcep = "";

            List<Estudiante> estDeu = db.Estudiante.Where(x => x.Id == cobro.idEstudiante).ToList();
            if (estDeu != null && estDeu.Count() > 0)
            {
                var est = estDeu.FirstOrDefault();

                List<Deudas> debeP = new List<Deudas>();

                List<Cobro> cobroD = db.Cobro.Where(x => x.idEstudiante == est.Id).ToList();
                if (cobroD != null && cobroD.Count() > 0)
                {
                    var cobD = db.Cobro.Find(id);
                    List<Concepto> concep = db.Concepto.Where(x => x.Id == cobD.idConcepto).ToList();
                    if (concep != null && concep.Count > 0)
                    {
                        foreach (var it in concep)
                        {
                            nombconcep = it.Nombre.ToString();

                            Deudas deuda = new Deudas();
                            deuda.id = cobD.Id;
                            deuda.Concepto = nombconcep;
                            deuda.deudaPorConcepto = (decimal)cobD.Deuda;
                            deuda.TotalAdeudado += cobD.Deuda;
                            deuda.Activo = true;

                            debeP.Add(deuda);
                        }
                    }
                    deudasTot.AddRange(debeP);
                }
            }

            ViewBag.DeudasTot = deudasTot;

            decimal sumaDeudas = 0;
            if (deudasTot != null && deudasTot.Count() > 0)
            {
                foreach (var item in deudasTot)
                {
                    if (item.Activo == true)
                    {
                        decimal importe = 0;
                        importe = item.deudaPorConcepto;

                        sumaDeudas += importe;
                    }
                }
            }

            ViewBag.SumaTotal = sumaDeudas;

            var deudaCancel = (List<CancelarDeudas>)Session["ListaDeudasCancel"];
            if (deudaCancel != null && deudaCancel.Count() > 0)
            {
                // Buscar el objeto en la lista original y cambiar su propiedad Activo a true
                var habilitarDeuda = deudaCancel.FirstOrDefault(d => d.id == id);
                if (habilitarDeuda != null)
                {
                    deudaCancel.Remove(habilitarDeuda);
                    //habilitarDeuda.Activo = false;
                }

                ViewBag.DeudasTotCancel = deudaCancel;
            }

            decimal sumaDeudasCancel = 0;
            if (deudaCancel != null && deudaCancel.Count() > 0)
            {
                foreach (var item in deudaCancel)
                {
                    if (item.Activo == true)
                    {
                        decimal importe = 0;
                        importe = item.deudaPorConcepto;

                        sumaDeudasCancel += importe;
                    }
                }
            }

            ViewBag.SumaTotalCancel = sumaDeudasCancel;

            return View("Deudas");
        }

        public ActionResult CancelarDeuda()
        {
            int IdEstudiante = (int)HttpContext.Session["idEstudiante"];

            Estudiante estudiante = db.Estudiante.Find(IdEstudiante);
            if (estudiante == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var cobro = (Cobro)Session["Cobro"];

            var deudaCancel = (List<CancelarDeudas>)Session["ListaDeudasCancel"];
            if (deudaCancel != null && deudaCancel.Count() > 0)
            {
                foreach (var item in deudaCancel)
                {
                    // Obtener el objeto Cobro correspondiente y establecer Deuda a 0
                    var ActualizarCobro = db.Cobro.FirstOrDefault(c => c.Id == item.id);
                    if (ActualizarCobro != null)
                    {
                        ActualizarCobro.Activo = true;
                        ActualizarCobro.Deuda = 0;
                        //db.Entry(ActualizarCobro).State = EntityState.Modified;
                    }
                }
            }

            db.SaveChanges();

            // Establecer la propiedad Deuda a 0 para cada deuda

            decimal sumaDeudas = db.Cobro.Where(c => c.idEstudiante == estudiante.Id).Sum(c => c.Deuda);

            if (sumaDeudas > 0)
            {
                Estudiante estu = db.Estudiante.Find(estudiante.Id);
                if (estu != null)
                {
                    estu.TieneDeuda = true;
                    db.Entry(estu).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            else
            {
                Estudiante estu = db.Estudiante.Find(estudiante.Id);
                if (estu != null)
                {
                    estu.TieneDeuda = false;
                    db.Entry(estu).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return RedirectToAction("DeudaCancel", "DeudaCancelada", new { idEstudiante = IdEstudiante });
        }

        // GET: Estudiante/Create
        public ActionResult Create()
        {
            //ViewBag.idBarrio = new SelectList(db.Barrio, "Id", "Nombre");
            //ViewBag.idCurso = new SelectList(db.Curso, "Id", "Nombre");

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

            Session["PagoNuevo"] = null;

            return View();
        }

        // POST: Estudiante/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,DNI,NombreCompleto,FechaNacimiento,Direccion,Telefono1,Telefono2,idBarrio,idCurso,PendientePago,TieneDeuda")] Estudiante estudiante)
        {
            if (ModelState.IsValid)
            {
                var EstudDet = db.Estudiante.Where(x => x.DNI == estudiante.DNI);
                // si existe un estudiante ya agendado con el DNI ingresado
                if (EstudDet != null && EstudDet.Count() > 0)
                {
                    ViewBag.Error = "DNI de estudiante existente, por favor registre nuevo estudiante";

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

                    return View("Create");
                }
                else
                {
                    if (estudiante.idBarrio == null && estudiante.idCurso == null)
                    {
                        ViewBag.ErrorBarrio = "Dato no valido, debe seleccionar un barrio";

                        ViewBag.ErrorCurso = "Dato no valido, debe seleccionar un curso";

                        // Combo barrios
                        List<SelectListItem> barriosM = new List<SelectListItem>();

                        List<Barrio> Barriodb1 = db.Barrio.ToList();

                        foreach (var item in Barriodb1)
                        {
                            SelectListItem barrio = new SelectListItem();
                            barrio.Text = item.Nombre.ToString();
                            barrio.Value = item.Id.ToString();
                            barrio.Selected = false;
                            barriosM.Add(barrio);
                        }

                        ViewBag.barrios = barriosM;

                        // Combo Cursos
                        List<SelectListItem> cursosM = new List<SelectListItem>();

                        List<Curso> Cursodb1 = db.Curso.ToList();

                        foreach (var item in Cursodb1)
                        {
                            SelectListItem curso = new SelectListItem();
                            curso.Text = item.Nombre.ToString();
                            curso.Value = item.Id.ToString();
                            curso.Selected = false;
                            cursosM.Add(curso);
                        }

                        ViewBag.Cursos = cursosM;

                        return View("Create");
                    }

                    if (estudiante.idBarrio == null || estudiante.idCurso == null)
                    {
                        if (estudiante.idBarrio == null)
                        {
                            ViewBag.ErrorBarrio = "Dato no valido, debe seleccionar un barrio";

                            // Combo barrios
                            List<SelectListItem> barriosM = new List<SelectListItem>();

                            List<Barrio> Barriodb1 = db.Barrio.ToList();

                            foreach (var item in Barriodb1)
                            {
                                SelectListItem barrio = new SelectListItem();
                                barrio.Text = item.Nombre.ToString();
                                barrio.Value = item.Id.ToString();
                                barrio.Selected = false;
                                barriosM.Add(barrio);
                            }

                            ViewBag.barrios = barriosM;

                            // Combo Cursos
                            List<SelectListItem> cursosM = new List<SelectListItem>();

                            List<Curso> Cursodb1 = db.Curso.ToList();

                            foreach (var item in Cursodb1)
                            {
                                SelectListItem curso = new SelectListItem();
                                curso.Text = item.Nombre.ToString();
                                curso.Value = item.Id.ToString();
                                curso.Selected = false;
                                cursosM.Add(curso);
                            }

                            ViewBag.Cursos = cursosM;
                        }

                        if (estudiante.idCurso == null)
                        {
                            ViewBag.ErrorCurso = "Dato no valido, debe seleccionar un curso";
                            // Combo barrios
                            List<SelectListItem> barriosM = new List<SelectListItem>();

                            List<Barrio> Barriodb1 = db.Barrio.ToList();

                            foreach (var item in Barriodb1)
                            {
                                SelectListItem barrio = new SelectListItem();
                                barrio.Text = item.Nombre.ToString();
                                barrio.Value = item.Id.ToString();
                                barrio.Selected = false;
                                barriosM.Add(barrio);
                            }

                            ViewBag.barrios = barriosM;

                            // Combo Cursos
                            List<SelectListItem> cursosM = new List<SelectListItem>();

                            List<Curso> Cursodb1 = db.Curso.ToList();

                            foreach (var item in Cursodb1)
                            {
                                SelectListItem curso = new SelectListItem();
                                curso.Text = item.Nombre.ToString();
                                curso.Value = item.Id.ToString();
                                curso.Selected = false;
                                cursosM.Add(curso);
                            }

                            ViewBag.Cursos = cursosM;
                        }

                        return View("Create");
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


                    estudiante.Activo = true;
                    estudiante.TieneDeuda = false;
                    db.Estudiante.Add(estudiante);
                    db.SaveChanges();

                    Session["PagoNuevo"] = null;

                    Session["idEst"] = estudiante.Id;

                    TempData["MensajeExito"] = "El estudiante se ha guardado exitosamente.";

                    //return RedirectToAction("Create", "Estudiante", new { idEstudiante = estudiante.Id });
                    return RedirectToAction("Create", "Estudiante");
                }
            }

            //ViewBag.idBarrio = new SelectList(db.Barrio, "Id", "Nombre", estudiante.idBarrio);
            //ViewBag.idCurso = new SelectList(db.Curso, "Id", "Nombre", estudiante.idCurso);

            return View(estudiante);
        }

        public ActionResult GenerarPago()
        {
            var id = HttpContext.Session["idEst"];

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estudiante estudiante = db.Estudiante.Find(id);
            if (estudiante == null)
            {
                return HttpNotFound();
            }

            Session["ListaPagos"] = null;

            return RedirectToAction("Create", "Cobro", new { idEstudiante = estudiante.Id });
        }

        // GET: Estudiante/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estudiante estudiante = db.Estudiante.Find(id);
            if (estudiante == null)
            {
                return HttpNotFound();
            }
            //ViewBag.idBarrio = new SelectList(db.Barrio, "Id", "Nombre", estudiante.idBarrio);
            //ViewBag.idCurso = new SelectList(db.Curso, "Id", "Nombre", estudiante.idCurso);

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

        // POST: Estudiante/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
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
                var idEstudiante = HttpContext.Session["idEstudiante"];
                ViewBag.idEstudiante = HttpContext.Session["idEstudiante"];

                return RedirectToAction("Index", "Cobro", new { idEstudiante });
            }
            //ViewBag.idBarrio = new SelectList(db.Barrio, "Id", "Nombre", estudiante.idBarrio);
            //ViewBag.idCurso = new SelectList(db.Curso, "Id", "Nombre", estudiante.idCurso);
            return View(estudiante);
        }

        // GET: Estudiante/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estudiante estudiante = db.Estudiante.Find(id);
            if (estudiante == null)
            {
                return HttpNotFound();
            }
            return View(estudiante);
        }

        // POST: Estudiante/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Estudiante estudiante = db.Estudiante.Find(id);

            List<Cobro> cobros = db.Cobro.Where(x => x.idEstudiante == estudiante.Id).ToList();
            // Si existe algo en la tabla de cobros vinculado al estudiante creado
            if (cobros != null && cobros.Count() > 0)
            {
                foreach (var i in cobros)
                {
                    Cobro cobro = db.Cobro.Find(i.Id);
                    cobro.Activo = false;
                    //db.Cobro.Remove(cobro);
                    db.SaveChanges();
                }
            }
            estudiante.Activo = false;
            //db.Estudiante.Remove(estudiante);
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
