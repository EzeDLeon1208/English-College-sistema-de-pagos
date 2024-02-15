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
    public class CobroController : Controller
    {
        private EnglishCollegeDbContext db = new EnglishCollegeDbContext();

        // GET: Cobro
        public ActionResult Index(int? idEstudiante)
        {
            if (idEstudiante == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.idEstudiante = idEstudiante;
            HttpContext.Session["idEstudiante"] = idEstudiante;
            //var cobro = db.Cobro.Include(c => c.Concepto).Include(c => c.Estudiante);
            var cobro = db.Cobro.Where(x => x.idEstudiante == idEstudiante);
            if (cobro != null && cobro.Count() > 0)
            {
                var idcob = cobro.FirstOrDefault();
                HttpContext.Session["idCobro"] = idcob.Id;
            }
            HttpContext.Session["PagoNuevo"] = null;

            HttpContext.Session["ListaPagos"] = null;

            //Estudiante estud = (Estudiante)HttpContext.Session["idEstudiante"];

            return View(cobro.ToList());
        }


        // GET: Cobro/Details/5
        public ActionResult Details(int? id)
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
            //ViewBag.idestudiante = idEstudiante;

            return View(cobro);
        }

        // GET: Cobro/Create
        public ActionResult Create(int? idEstudiante)
        {
            //ViewBag.idConcepto = new SelectList(db.Concepto, "Id", "Nombre");
            //ViewBag.idEstudiante = new SelectList(db.Estudiante, "Id", "DNI");

            if (idEstudiante == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Estudiante est = db.Estudiante.Find(idEstudiante);

            string NombCur = "";
            decimal precioCurso = 0;
            decimal precioLibro = 0;
            if (est != null)
            {
                List<Curso> Cur = db.Curso.Where(x => x.Id == est.idCurso).ToList();
                if (Cur != null && Cur.Count > 0)
                {
                    var curso = Cur.FirstOrDefault();
                    NombCur = curso.Nombre;
                    precioCurso = (decimal)curso.Precio;
                    precioLibro = (decimal)curso.PrecioLibro;
                }
            }

            ViewBag.Curso = NombCur;

            List<SelectListItem> conceptos = new List<SelectListItem>();

            List<Concepto> Conceptodb = db.Concepto.ToList();
            decimal precioLib = 0;
            decimal fotocop = 0;
            string nomConcep = "";
            foreach (var item in Conceptodb)
            {
                SelectListItem concepto = new SelectListItem();
                concepto.Text = item.Nombre.ToString();
                concepto.Value = item.Id.ToString();
                if (item.Id == 5)
                {
                    fotocop = 400;
                    precioLib = fotocop;
                }
                else if (item.Id == 2)
                {
                    precioLib = precioLibro;
                }
                else
                {
                    precioLib = precioCurso;
                }
                conceptos.Add(concepto);
            }

            // Medios de Pago
            List<SelectListItem> medioP = new List<SelectListItem>();

            List<MPago> MedPdb = db.MPago.ToList();

            foreach (var item in MedPdb)
            {
                SelectListItem mPago = new SelectListItem();
                mPago.Text = item.Nombre.ToString();
                mPago.Value = item.Id.ToString();
                mPago.Selected = false;
                medioP.Add(mPago);
            }

            ViewBag.Conceptos = conceptos;

            ViewBag.precio = precioLib;

            ViewBag.MedioPago = medioP;

            //Session["PagoNuevo"] = false;

            var ListaPagos = (List<DetallePagos>)Session["ListaPagos"];
            if (ListaPagos != null && ListaPagos.Count() > 0)
            {
                ViewBag.listapago = true;
                Session["PagoNuevo"] = ViewBag.listapago;
            }
            else
            {
                ViewBag.listapago = false;
                Session["PagoNuevo"] = ViewBag.listapago;
                Session["SumaTotal"] = 0;
            }

            ViewBag.PagosGen = ListaPagos;

            ViewBag.SumaTotal = Session["SumaTotal"];

            HttpContext.Session["idEstudiante"] = idEstudiante;

            return View("Create");
        }

        public ActionResult CancelarConcepto(int? idCon)
        {
            if (idCon == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //int idCobro = (int)Session["idCobro"];

            Cobro cobro = (Cobro)Session["Cobro"];

            int idC = (int)Session["idConcepto"];

            int idEstudiante = 0;
            var estud = db.Estudiante.Where(x => x.Id == cobro.idEstudiante);
            if (estud != null && estud.Count() > 0)
            {
                var idEs = estud.FirstOrDefault();
                idEstudiante = idEs.Id;
            }

            //decimal Deuda = (decimal)Session["Deuda"];

            var ListaPagos = (List<DetallePagos>)Session["ListaPagos"];

            Cobro cob = db.Cobro.FirstOrDefault(x => x.Id == idCon);
            if (cob != null)
            {
                if (cob.Activo)
                {
                    cob.Activo = false;
                    if (ListaPagos != null && ListaPagos.Count() > 0)
                    {
                        var cobroP = ListaPagos.FirstOrDefault();
                        cobroP.Activo = cob.Activo;
                    }
                    if (cob.Deuda > 0)
                    {
                        cob.Deuda = 0;
                    }
                    db.Entry(cob).State = EntityState.Modified;
                    //db.SaveChanges();
                }

                db.SaveChanges();// Calcular la suma total de las deudas
                decimal deudaTotal = db.Cobro.Where(c => c.idEstudiante == idEstudiante).Sum(c => c.Deuda);


                //bool tieneDeuda = cob.Any(c => c.Deuda > 0);

                Estudiante estudiante = db.Estudiante.Find(idEstudiante);
                if (estudiante != null)
                {
                    estudiante.TieneDeuda = deudaTotal > 0;
                    db.Entry(estudiante).State = EntityState.Modified;
                }

                db.SaveChanges();
            }

            int idAEliminar = idCon.Value;
            ListaPagos.RemoveAll(detalle => detalle.Id == idAEliminar);


            decimal sumaImportes = 0;
            decimal importe = 0;
            foreach (var item in ListaPagos)
            {
                item.Activo = false;
                importe = item.importeTotal;
                sumaImportes += importe;
            }

            ViewBag.SumaTotal = sumaImportes;

            Session["SumaTotal"] = ViewBag.SumaTotal;

            Session["ListaPagos"] = ListaPagos;

            var idEst = HttpContext.Session["idEstudiante"];
            // Redirigir a la vista actual o a una vista específica
            return RedirectToAction("Create", "Cobro", new { idEstudiante = idEst });
            //return View(cobro);
        }



        [HttpPost]
        public ActionResult CambiarPrecio(int conceptoId)
        {
            decimal precioLib = ObtenerPrecioSegunConcepto(conceptoId);

            // Actualiza el ViewBag.precio con el nuevo valor
            ViewBag.precio = precioLib;

            // Devuelve el nuevo precio al cliente
            return Json(ViewBag.precio);
        }

        private decimal ObtenerPrecioSegunConcepto(int conceptoId)
        {

            Concepto concepto = db.Concepto.FirstOrDefault(c => c.Id == conceptoId);
            decimal precioCur = 0;
            if (concepto != null)
            {
                if (concepto.Id == 5) //FOTOCOPIAS
                {
                    return 400;
                }
                else if (concepto.Id == 2) //LIBRO
                {
                    var id = HttpContext.Session["idEstudiante"];

                    Estudiante estud = db.Estudiante.Find(id);
                    if (estud != null)
                    {
                        Curso curso = db.Curso.FirstOrDefault(x => x.Id == estud.idCurso);
                        decimal pLibro = (decimal)curso.PrecioLibro;
                        return pLibro;
                    }
                }
                else // RESTO CURSO
                {
                    var id = HttpContext.Session["idEstudiante"];

                    Estudiante estud = db.Estudiante.Find(id);
                    if (estud != null)
                    {
                        List<Curso> Cur = db.Curso.Where(x => x.Id == estud.idCurso).ToList();
                        if (Cur != null && Cur.Count > 0)
                        {
                            var curso = Cur.FirstOrDefault();
                            decimal precioCurso = (decimal)curso.Precio;
                            precioCur = precioCurso;
                        }
                    }

                    // Manejar el caso en que el concepto no se encuentra en la base de datos
                    //return precioCur;
                }
            }
            return precioCur;
        }


        // POST: Cobro/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = )] Cobro cobro)

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,idEstudiante,idConcepto,precioTotal,Deuda,idMPago,FechaPago")] Cobro cobro)
        {
            if (ModelState.IsValid)
            {
                // Valida que no sean nulos ni el Concepto, ni el Medio de Pago, ni el Precio
                if (cobro.idConcepto == null && cobro.idMPago == null && cobro.precioTotal == null)
                {
                    ViewBag.ErrorMPago = "Por favor, seleccione un medio de pago";

                    ViewBag.ErrorConcepto = "Por favor, seleccione un concepto";

                    ViewBag.ErrorMonto = "Por favor, agregue un monto";

                    Estudiante est = db.Estudiante.Find(cobro.idEstudiante);

                    string NombCur = "";
                    decimal precioCurso = 0;
                    decimal precioLibro = 0;
                    if (est != null)
                    {
                        List<Curso> Cur = db.Curso.Where(x => x.Id == est.idCurso).ToList();
                        if (Cur != null && Cur.Count > 0)
                        {
                            var curso = Cur.FirstOrDefault();
                            NombCur = curso.Nombre;
                            precioCurso = (decimal)curso.Precio;
                            precioLibro = (decimal)curso.PrecioLibro;
                        }
                    }

                    List<SelectListItem> conceptos = new List<SelectListItem>();

                    List<Concepto> Conceptodb = db.Concepto.ToList();
                    decimal precioLib = 0;
                    decimal fotocop = 0;
                    string nomConcep = "";
                    foreach (var item in Conceptodb)
                    {
                        SelectListItem concepto = new SelectListItem();
                        concepto.Text = item.Nombre.ToString();
                        concepto.Value = item.Id.ToString();

                        conceptos.Add(concepto);
                    }

                    if (cobro.idConcepto == 5)
                    {
                        fotocop = 400;
                        precioLib = fotocop;
                    }
                    else if (cobro.idConcepto == 2)
                    {
                        precioLib = precioLibro;
                    }
                    else
                    {
                        precioLib = precioCurso;
                    }

                    // Medios de Pago
                    List<SelectListItem> medioP = new List<SelectListItem>();

                    List<MPago> MedPdb = db.MPago.ToList();

                    foreach (var item in MedPdb)
                    {
                        SelectListItem mPago = new SelectListItem();
                        mPago.Text = item.Nombre.ToString();
                        mPago.Value = item.Id.ToString();
                        mPago.Selected = false;
                        medioP.Add(mPago);
                    }

                    ViewBag.Curso = NombCur;

                    ViewBag.precio = precioLib;

                    ViewBag.Conceptos = conceptos;

                    ViewBag.MedioPago = medioP;

                    var ListaPagos = (List<DetallePagos>)Session["ListaPagos"];
                    if (ListaPagos != null && ListaPagos.Count() > 0)
                    {
                        ViewBag.listapago = true;
                        Session["PagoNuevo"] = ViewBag.listapago;
                        decimal sumaImportes = 0;

                        foreach (var item in ListaPagos)
                        {
                            decimal importe = 0;
                            importe = item.importeTotal;

                            sumaImportes += importe;
                        }

                        ViewBag.SumaTotal = sumaImportes;

                        ViewBag.PagosGen = ListaPagos;
                    }
                    else
                    {
                        ViewBag.listapago = false;
                        Session["PagoNuevo"] = ViewBag.listapago;
                        Session["SumaTotal"] = 0;
                    }

                    return View("Create");
                } // Valida que sea nulo el Concepto y el Importe
                else if (cobro.idConcepto == null && cobro.precioTotal == null)
                {
                    ViewBag.ErrorConcepto = "Por favor, seleccione un concepto";

                    ViewBag.ErrorMonto = "Por favor, agregue un monto";

                    Estudiante est = db.Estudiante.Find(cobro.idEstudiante);

                    string NombCur = "";
                    decimal precioCurso = 0;
                    decimal precioLibro = 0;
                    if (est != null)
                    {
                        List<Curso> Cur = db.Curso.Where(x => x.Id == est.idCurso).ToList();
                        if (Cur != null && Cur.Count > 0)
                        {
                            var curso = Cur.FirstOrDefault();
                            NombCur = curso.Nombre;
                            precioCurso = (decimal)curso.Precio;
                            precioLibro = (decimal)curso.PrecioLibro;
                        }
                    }

                    List<SelectListItem> conceptos = new List<SelectListItem>();

                    List<Concepto> Conceptodb = db.Concepto.ToList();
                    decimal precioLib = 0;
                    decimal fotocop = 0;
                    string nomConcep = "";
                    foreach (var item in Conceptodb)
                    {
                        SelectListItem concepto = new SelectListItem();
                        concepto.Text = item.Nombre.ToString();
                        concepto.Value = item.Id.ToString();

                        conceptos.Add(concepto);
                    }

                    if (cobro.idConcepto == 5)
                    {
                        fotocop = 400;
                        precioLib = fotocop;
                    }
                    else if (cobro.idConcepto == 2)
                    {
                        precioLib = precioLibro;
                    }
                    else
                    {
                        precioLib = precioCurso;
                    }

                    // Medios de Pago
                    List<SelectListItem> medioP = new List<SelectListItem>();

                    List<MPago> MedPdb = db.MPago.ToList();

                    foreach (var item in MedPdb)
                    {
                        SelectListItem mPago = new SelectListItem();
                        mPago.Text = item.Nombre.ToString();
                        mPago.Value = item.Id.ToString();
                        mPago.Selected = false;
                        medioP.Add(mPago);
                    }

                    ViewBag.Curso = NombCur;

                    ViewBag.precio = precioLib;

                    ViewBag.Conceptos = conceptos;

                    ViewBag.MedioPago = medioP;

                    var ListaPagos = (List<DetallePagos>)Session["ListaPagos"];
                    if (ListaPagos != null && ListaPagos.Count() > 0)
                    {
                        ViewBag.listapago = true;
                        Session["PagoNuevo"] = ViewBag.listapago;
                        decimal sumaImportes = 0;

                        foreach (var item in ListaPagos)
                        {
                            decimal importe = 0;
                            importe = item.importeTotal;

                            sumaImportes += importe;
                        }

                        ViewBag.SumaTotal = sumaImportes;

                        ViewBag.PagosGen = ListaPagos;
                    }
                    else
                    {
                        ViewBag.listapago = false;
                        Session["PagoNuevo"] = ViewBag.listapago;
                        Session["SumaTotal"] = 0;
                    }

                    return View("Create");
                }
                // Valida que sea nulo el Importe y el Medio de Pago
                else if (cobro.precioTotal == null && cobro.idMPago == null)
                {
                    ViewBag.ErrorMonto = "Por favor, agregue un monto";

                    ViewBag.ErrorMPago = "Por favor, seleccione un medio de pago";

                    Estudiante est = db.Estudiante.Find(cobro.idEstudiante);

                    string NombCur = "";
                    decimal precioCurso = 0;
                    decimal precioLibro = 0;
                    if (est != null)
                    {
                        List<Curso> Cur = db.Curso.Where(x => x.Id == est.idCurso).ToList();
                        if (Cur != null && Cur.Count > 0)
                        {
                            var curso = Cur.FirstOrDefault();
                            NombCur = curso.Nombre;
                            precioCurso = (decimal)curso.Precio;
                            precioLibro = (decimal)curso.PrecioLibro;
                        }
                    }

                    List<SelectListItem> conceptos = new List<SelectListItem>();

                    List<Concepto> Conceptodb = db.Concepto.ToList();
                    decimal precioLib = 0;
                    decimal fotocop = 0;
                    string nomConcep = "";
                    foreach (var item in Conceptodb)
                    {
                        SelectListItem concepto = new SelectListItem();
                        concepto.Text = item.Nombre.ToString();
                        concepto.Value = item.Id.ToString();

                        conceptos.Add(concepto);
                    }

                    if (cobro.idConcepto == 5)
                    {
                        fotocop = 400;
                        precioLib = fotocop;
                    }
                    else if (cobro.idConcepto == 2)
                    {
                        precioLib = precioLibro;
                    }
                    else
                    {
                        precioLib = precioCurso;
                    }

                    // Medios de Pago
                    List<SelectListItem> medioP = new List<SelectListItem>();

                    List<MPago> MedPdb = db.MPago.ToList();

                    foreach (var item in MedPdb)
                    {
                        SelectListItem mPago = new SelectListItem();
                        mPago.Text = item.Nombre.ToString();
                        mPago.Value = item.Id.ToString();
                        mPago.Selected = false;
                        medioP.Add(mPago);
                    }

                    ViewBag.Curso = NombCur;

                    ViewBag.precio = precioLib;

                    ViewBag.Conceptos = conceptos;

                    ViewBag.MedioPago = medioP;

                    var ListaPagos = (List<DetallePagos>)Session["ListaPagos"];
                    if (ListaPagos != null && ListaPagos.Count() > 0)
                    {
                        ViewBag.listapago = true;
                        Session["PagoNuevo"] = ViewBag.listapago;
                        decimal sumaImportes = 0;

                        foreach (var item in ListaPagos)
                        {
                            decimal importe = 0;
                            importe = item.importeTotal;

                            sumaImportes += importe;
                        }

                        ViewBag.SumaTotal = sumaImportes;

                        ViewBag.PagosGen = ListaPagos;
                    }
                    else
                    {
                        ViewBag.listapago = false;
                        Session["PagoNuevo"] = ViewBag.listapago;
                        Session["SumaTotal"] = 0;
                    }

                    return View("Create");
                } // Valida que sea nulo el Medio de Pago
                else if (cobro.idMPago == null)
                {
                    ViewBag.ErrorMPago = "Por favor, seleccione un medio de pago";

                    Estudiante est = db.Estudiante.Find(cobro.idEstudiante);

                    string NombCur = "";
                    decimal precioCurso = 0;
                    decimal precioLibro = 0;
                    if (est != null)
                    {
                        List<Curso> Cur = db.Curso.Where(x => x.Id == est.idCurso).ToList();
                        if (Cur != null && Cur.Count > 0)
                        {
                            var curso = Cur.FirstOrDefault();
                            NombCur = curso.Nombre;
                            precioCurso = (decimal)curso.Precio;
                            precioLibro = (decimal)curso.PrecioLibro;
                        }
                    }

                    List<SelectListItem> conceptos = new List<SelectListItem>();

                    List<Concepto> Conceptodb = db.Concepto.ToList();
                    decimal precioLib = 0;
                    decimal fotocop = 0;
                    string nomConcep = "";
                    foreach (var item in Conceptodb)
                    {
                        SelectListItem concepto = new SelectListItem();
                        concepto.Text = item.Nombre.ToString();
                        concepto.Value = item.Id.ToString();

                        conceptos.Add(concepto);
                    }

                    if (cobro.idConcepto == 5)
                    {
                        fotocop = 400;
                        precioLib = fotocop;
                    }
                    else if (cobro.idConcepto == 2)
                    {
                        precioLib = precioLibro;
                    }
                    else
                    {
                        precioLib = precioCurso;
                    }

                    // Medios de Pago
                    List<SelectListItem> medioP = new List<SelectListItem>();

                    List<MPago> MedPdb = db.MPago.ToList();

                    foreach (var item in MedPdb)
                    {
                        SelectListItem mPago = new SelectListItem();
                        mPago.Text = item.Nombre.ToString();
                        mPago.Value = item.Id.ToString();
                        mPago.Selected = false;
                        medioP.Add(mPago);
                    }

                    ViewBag.Curso = NombCur;

                    ViewBag.precio = precioLib;

                    ViewBag.Conceptos = conceptos;

                    ViewBag.MedioPago = medioP;

                    var ListaPagos = (List<DetallePagos>)Session["ListaPagos"];
                    if (ListaPagos != null && ListaPagos.Count() > 0)
                    {
                        ViewBag.listapago = true;
                        Session["PagoNuevo"] = ViewBag.listapago;
                        decimal sumaImportes = 0;

                        foreach (var item in ListaPagos)
                        {
                            decimal importe = 0;
                            importe = item.importeTotal;

                            sumaImportes += importe;
                        }

                        ViewBag.SumaTotal = sumaImportes;

                        ViewBag.PagosGen = ListaPagos;
                    }
                    else
                    {
                        ViewBag.listapago = false;
                        Session["PagoNuevo"] = ViewBag.listapago;
                        Session["SumaTotal"] = 0;
                    }

                    return View("Create");
                } // Valida que sea nulo el Importe
                else if (cobro.precioTotal == null)
                {
                    ViewBag.ErrorMonto = "Por favor, agregue un monto";

                    Estudiante est = db.Estudiante.Find(cobro.idEstudiante);

                    string NombCur = "";
                    decimal precioCurso = 0;
                    decimal precioLibro = 0;
                    if (est != null)
                    {
                        List<Curso> Cur = db.Curso.Where(x => x.Id == est.idCurso).ToList();
                        if (Cur != null && Cur.Count > 0)
                        {
                            var curso = Cur.FirstOrDefault();
                            NombCur = curso.Nombre;
                            precioCurso = (decimal)curso.Precio;
                            precioLibro = (decimal)curso.PrecioLibro;
                        }
                    }

                    //ViewBag.ValorC = precioCurso;

                    List<SelectListItem> conceptos = new List<SelectListItem>();

                    List<Concepto> Conceptodb = db.Concepto.ToList();
                    decimal precioLib = 0;
                    decimal fotocop = 0;
                    string nomConcep = "";
                    foreach (var item in Conceptodb)
                    {
                        SelectListItem concepto = new SelectListItem();
                        concepto.Text = item.Nombre.ToString();
                        concepto.Value = item.Id.ToString();

                        conceptos.Add(concepto);
                    }

                    if (cobro.idConcepto == 5)
                    {
                        fotocop = 400;
                        precioLib = fotocop;
                    }
                    else if (cobro.idConcepto == 2)
                    {
                        precioLib = precioLibro;
                    }
                    else
                    {
                        precioLib = precioCurso;
                    }

                    // Medios de Pago
                    List<SelectListItem> medioP = new List<SelectListItem>();

                    List<MPago> MedPdb = db.MPago.ToList();

                    foreach (var item in MedPdb)
                    {
                        SelectListItem mPago = new SelectListItem();
                        mPago.Text = item.Nombre.ToString();
                        mPago.Value = item.Id.ToString();
                        mPago.Selected = false;
                        medioP.Add(mPago);
                    }

                    ViewBag.Curso = NombCur;

                    ViewBag.precio = precioLib;

                    ViewBag.Conceptos = conceptos;

                    ViewBag.MedioPago = medioP;

                    var ListaPagos = (List<DetallePagos>)Session["ListaPagos"];
                    if (ListaPagos != null && ListaPagos.Count() > 0)
                    {
                        ViewBag.listapago = true;
                        Session["PagoNuevo"] = ViewBag.listapago;

                        decimal sumaImportes = 0;

                        foreach (var item in ListaPagos)
                        {
                            decimal importe = 0;
                            importe = item.importeTotal;

                            sumaImportes += importe;
                        }

                        ViewBag.SumaTotal = sumaImportes;

                        ViewBag.PagosGen = ListaPagos;
                    }
                    else
                    {
                        ViewBag.listapago = false;
                        Session["PagoNuevo"] = ViewBag.listapago;
                        Session["SumaTotal"] = 0;
                    }

                    return View("Create");
                } // Valida que sea nulo el Concepto
                else if (cobro.idConcepto == null)
                {
                    ViewBag.ErrorConcepto = "Por favor, seleccione un concepto";

                    Estudiante est = db.Estudiante.Find(cobro.idEstudiante);

                    string NombCur = "";
                    decimal precioCurso = 0;
                    decimal precioLibro = 0;
                    if (est != null)
                    {
                        List<Curso> Cur = db.Curso.Where(x => x.Id == est.idCurso).ToList();
                        if (Cur != null && Cur.Count > 0)
                        {
                            var curso = Cur.FirstOrDefault();
                            NombCur = curso.Nombre;
                            precioCurso = (decimal)curso.Precio;
                            precioLibro = (decimal)curso.PrecioLibro;
                        }
                    }

                    List<SelectListItem> conceptos = new List<SelectListItem>();

                    List<Concepto> Conceptodb = db.Concepto.ToList();
                    decimal precioLib = 0;
                    decimal fotocop = 0;
                    string nomConcep = "";
                    foreach (var item in Conceptodb)
                    {
                        SelectListItem concepto = new SelectListItem();
                        concepto.Text = item.Nombre.ToString();
                        concepto.Value = item.Id.ToString();

                        conceptos.Add(concepto);
                    }

                    if (cobro.idConcepto == 5)
                    {
                        fotocop = 400;
                        precioLib = fotocop;
                    }
                    else if (cobro.idConcepto == 2)
                    {
                        precioLib = precioLibro;
                    }
                    else
                    {
                        precioLib = precioCurso;
                    }

                    // Medios de Pago
                    List<SelectListItem> medioP = new List<SelectListItem>();

                    List<MPago> MedPdb = db.MPago.ToList();

                    foreach (var item in MedPdb)
                    {
                        SelectListItem mPago = new SelectListItem();
                        mPago.Text = item.Nombre.ToString();
                        mPago.Value = item.Id.ToString();
                        mPago.Selected = false;
                        medioP.Add(mPago);
                    }

                    ViewBag.Curso = NombCur;

                    ViewBag.precio = precioLib;

                    ViewBag.Conceptos = conceptos;

                    ViewBag.MedioPago = medioP;

                    var ListaPagos = (List<DetallePagos>)Session["ListaPagos"];
                    if (ListaPagos != null && ListaPagos.Count() > 0)
                    {
                        ViewBag.listapago = true;
                        Session["PagoNuevo"] = ViewBag.listapago;

                        decimal sumaImportes = 0;

                        foreach (var item in ListaPagos)
                        {
                            decimal importe = 0;
                            importe = item.importeTotal;

                            sumaImportes += importe;
                        }

                        ViewBag.SumaTotal = sumaImportes;

                        ViewBag.PagosGen = ListaPagos;
                    }
                    else
                    {
                        ViewBag.listapago = false;
                        Session["PagoNuevo"] = ViewBag.listapago;
                        Session["SumaTotal"] = 0;
                    }

                    return View("Create");
                }
                else // Valida que ninguno de los conceptos anteriores sean nulos
                {
                    Estudiante est = db.Estudiante.Find(cobro.idEstudiante);
                    string NombCur = "";
                    decimal precioCurso = 0;
                    decimal precioLibro = 0;
                    if (est != null)
                    {
                        List<Curso> Cur = db.Curso.Where(x => x.Id == est.idCurso).ToList();
                        if (Cur != null && Cur.Count > 0)
                        {
                            var curso = Cur.FirstOrDefault();
                            NombCur = curso.Nombre;
                            precioCurso = (decimal)curso.Precio;
                            precioLibro = (decimal)curso.PrecioLibro;
                        }
                    }

                    // Conceptos
                    List<SelectListItem> conceptos = new List<SelectListItem>();

                    List<Concepto> Conceptodb = db.Concepto.ToList();
                    decimal precioLib = 0;
                    decimal fotocop = 0;
                    string nomConcep = "";
                    foreach (var item in Conceptodb)
                    {
                        SelectListItem concepto = new SelectListItem();
                        concepto.Text = item.Nombre.ToString();
                        concepto.Value = item.Id.ToString();
                        conceptos.Add(concepto);
                    }

                    if (cobro.idConcepto == 5)
                    {
                        fotocop = 400;
                        precioLib = fotocop;
                    }
                    else if (cobro.idConcepto == 2)
                    {
                        precioLib = precioLibro;
                    }
                    else
                    {
                        precioLib = precioCurso;
                    }

                    ViewBag.Curso = NombCur;

                    ViewBag.precio = precioLib;

                    ViewBag.Conceptos = conceptos;

                    decimal deudaFin = (decimal)(precioLib - cobro.precioTotal);

                    if (deudaFin > 0)
                    {
                        List<Estudiante> estu = db.Estudiante.Where(x => x.Id == cobro.idEstudiante).ToList();
                        if (estu != null && estu.Count() > 0)
                        {
                            var Est = estu.FirstOrDefault();
                            Est.TieneDeuda = true;
                            db.Entry(Est).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        List<Estudiante> estu = db.Estudiante.Where(x => x.Id == cobro.idEstudiante).ToList();
                        if (estu != null && estu.Count() > 0)
                        {
                            var Est = estu.FirstOrDefault();
                            Est.TieneDeuda = false;
                            db.Entry(Est).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    cobro.Deuda = deudaFin;
                    cobro.Activo = true;
                    cobro.FechaPago = DateTime.Now;
                    db.Cobro.Add(cobro);
                    db.SaveChanges();

                    Session["Cobro"] = cobro;

                    Session["idCobro"] = cobro.Id;

                    Session["Deuda"] = cobro.Deuda;

                    Session["idConcepto"] = cobro.idConcepto;

                    // Genera una lista de conceptos con sus montos para mostrarlos en pantalla una vez guardados
                    // en la tabla de cobros, los busca uno por uno y los agrega a la lista generada

                    List<DetallePagos> PagosGen = new List<DetallePagos>();

                    // si la sesion de pagos tiene datos, cargame los pagos con la lista de sesion
                    if (HttpContext.Session["ListaPagos"] != null) PagosGen = (List<DetallePagos>)Session["ListaPagos"];

                    //buscamos si existe el nuevo id en tabla Cobro
                    List<Cobro> cob = db.Cobro.Where(x => x.Id == cobro.Id).ToList();
                    if (cob != null && cob.Count() > 0)
                    {
                        var nombconcep = "";
                        List<DetallePagos> pagos = new List<DetallePagos>();
                        foreach (var it in cob)
                        {  //buscamos si existe el nuevo id en tabla Concepto
                            List<Concepto> concep = db.Concepto.Where(x => x.Id == it.idConcepto).ToList();
                            if (concep != null && concep.Count > 0)
                            {
                                foreach (var item in concep)
                                {
                                    nombconcep = item.Nombre.ToString();

                                    DetallePagos detalPagos = new DetallePagos();
                                    detalPagos.Id = it.Id;
                                    detalPagos.IdConcepto = (int)it.idConcepto;
                                    detalPagos.Concepto = nombconcep;
                                    detalPagos.importeTotal = (decimal)it.precioTotal;
                                    detalPagos.sumaImportes += (decimal)it.precioTotal;
                                    detalPagos.Activo = true;
                                    pagos.Add(detalPagos);
                                }
                            }
                        }
                        // Agregar la lista pagos a detPagos fuera del bucle
                        PagosGen.AddRange(pagos);
                    }

                    // Manda los pagos generados en la lista de pagos a una variable en sesion para luego
                    // de cada item de la lista hacer una suma total de cada uno de los importes y mostrar
                    // la suma total de los importes y mandarlos a un viewbag

                    Session["ListaPagos"] = PagosGen;

                    int idEstudiante = 0;
                    var estud = db.Estudiante.Where(x => x.Id == cobro.idEstudiante);
                    if (estud != null && estud.Count() > 0)
                    {
                        var idEs = estud.FirstOrDefault();
                        idEstudiante = idEs.Id;
                    }

                    var ListaPagos = (List<DetallePagos>)Session["ListaPagos"];

                    decimal sumaImportes = 0;

                    foreach (var item in ListaPagos)
                    {
                        decimal importe = 0;
                        importe = item.importeTotal;

                        sumaImportes += importe;


                    }

                    ViewBag.SumaTotal = sumaImportes;

                    // Calcular la suma total de las deudas
                    decimal sumaDeudas = db.Cobro.Where(c => c.idEstudiante == idEstudiante).Sum(c => c.Deuda);

                    if (sumaDeudas > 0)
                    {
                        Estudiante estu = db.Estudiante.Find(cobro.idEstudiante);
                        if (estu != null)
                        {
                            estu.TieneDeuda = true;
                            db.Entry(estu).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        Estudiante estu = db.Estudiante.Find(cobro.idEstudiante);
                        if (estu != null)
                        {
                            estu.TieneDeuda = false;
                            db.Entry(estu).State = EntityState.Modified;
                            //estu.ForEach(p => db.Entry(p).State = EntityState.Modified);
                            //db.Entry(sXusu).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    //Session["PagoNuevo"] = null;

                    // Sesion generar nuevo pago
                    if (Session["PagoNuevo"] != null || (bool)Session["PagoNuevo"] == false)
                    {
                        ViewBag.listapago = true;
                        Session["PagoNuevo"] = ViewBag.listapago;
                    }
                    else
                    {
                        ViewBag.listapago = false;
                        Session["PagoNuevo"] = ViewBag.listapago;
                    }

                    ViewBag.PagosGen = PagosGen;

                    Session["ListaPagos"] = PagosGen;

                    // Medios de Pago
                    List<SelectListItem> medioP = new List<SelectListItem>();

                    List<MPago> MedPdb = db.MPago.ToList();

                    foreach (var item in MedPdb)
                    {
                        SelectListItem mPago = new SelectListItem();
                        mPago.Text = item.Nombre.ToString();
                        mPago.Value = item.Id.ToString();
                        mPago.Selected = false;
                        medioP.Add(mPago);
                    }

                    ViewBag.MedioPago = medioP;

                    HttpContext.Session["idEstudiante"] = cobro.idEstudiante;

                    //return RedirectToAction("Create", "Cobro", new { idEstudiante = cobro.idEstudiante });
                }
            }
            return View(cobro);
        }

        public ActionResult GuardarPago()
        {
            var IdEstudiante = HttpContext.Session["idEstudiante"];

            return RedirectToAction("CobroR", "CobroRealizado", new { idEstudiante = IdEstudiante });
        }

        //private void PrintPage(object sender, PrintPageEventArgs e)
        //{


        //    //var conceptos = db.Concepto.Where(x => x.Id == Cobro.idConcepto).ToList();
        //    //if (conceptos != null && conceptos.Count() > 0)
        //    //{
        //    //    List<DetallePagos> concep = new List<DetallePagos>();
        //    //    foreach (var concept in conceptos)
        //    //    {
        //    //        nombreconcept += concept.Nombre;

        //    //        importe = Cobro.importeTotal;

        //    //        totalImporte += importe;
        //    //    }                
        //    //}
        //// Imprime el contenido en la página
        //e.Graphics.DrawString(ticketContent, font, Brushes.Black, new PointF(10, 10), stringFormat);
        //}

        // GET: Cobro/Edit/5
        public ActionResult Edit(int? id)
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

            return View(cobro);
        }

        // POST: Cobro/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,idEstudiante,idConcepto,importeTotal,Deuda,idMPago")] Cobro cobro)
        {
            if (ModelState.IsValid)
            {
                cobro.Activo = true;
                db.Entry(cobro).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.idConcepto = new SelectList(db.Concepto, "Id", "Nombre", cobro.idConcepto);
            //ViewBag.idEstudiante = new SelectList(db.Estudiante, "Id", "DNI", cobro.idEstudiante);
            return View(cobro);
        }

        // GET: Cobro/Delete/5
        public ActionResult Delete(int? id)
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
            return View(cobro);
        }

        // POST: Cobro/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Cobro cobro = db.Cobro.Find(id);
            cobro.Activo = false;
            //db.Cobro.Remove(cobro);
            db.SaveChanges();

            //var estudiante = (Estudiante)HttpContext.Session["Estudiante"];
            //HttpContext.Session["idEstudiante"] = estudiante.Id;
            var estId = HttpContext.Session["idEstudiante"];

            return RedirectToAction("Index", "Cobro", new { idEstudiante = estId });
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
