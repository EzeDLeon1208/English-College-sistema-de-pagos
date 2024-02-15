﻿using EnglishCollege2024;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace EnglishCollege2024.Controllers
{
    public class CobroRealizadoController : Controller
    {
        private EnglishCollegeDbContext db = new EnglishCollegeDbContext();
        // GET: CobroRealizado
        public ActionResult CobroR(int? idEstudiante)
        {
            if (idEstudiante == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //ViewBag.idEstudiante = idEstudiante;
            HttpContext.Session["idEstudiante"] = idEstudiante;

            return View();
        }

        //public ActionResult NuevoPago()
        //{
        //    var idEstudiante = HttpContext.Session["idEstudiante"];

        //    if (idEstudiante == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    return RedirectToAction("Create", "Cobro", new { idEstudiante = idEstudiante });
        //    //return View();
        //}

        public ActionResult ListaPagos()
        {
            var idEstudiante = HttpContext.Session["idEstudiante"];

            if (idEstudiante == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return RedirectToAction("Index", "Cobro", new { idEstudiante = idEstudiante });
            //return View();
        }

        public ActionResult GenerarPdf(string ticketContent)
        {
            //var ListaPagos = Session["ListaPagos"];

            // Creamos un MemoryStream para almacenar el PDF
            using (MemoryStream ms = new MemoryStream())
            {
                // Creamos un documento PDF
                using (PdfWriter writer = new PdfWriter(ms))
                {
                    using (PdfDocument pdf = new PdfDocument(writer))
                    {
                        // Creamos un nuevo documento PDF
                        Document documento = new Document(pdf);

                        // Agregamos el contenido al documento (tu string)
                        documento.Add(new Paragraph(ticketContent))
                            .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                            .SetFontSize(12)
                            .SetFontFamily("Arial");
                    }
                }

                // Convertimos el MemoryStream a un array de bytes
                byte[] pdfBytes = ms.ToArray();

                // Devolvemos el archivo PDF como un resultado de acción
                return File(pdfBytes, "application/pdf", "ticketPago.pdf");
            }
        }

        public ActionResult MostrarPdf()
        {
            var cobro = (Cobro)Session["Cobro"];

            //int idEst = 0;
            //List<Estudiante> idestu = db.Estudiante.Where(x => x.Id == cobro.Id).ToList();
            //if (idestu != null && idestu.Count() > 0)
            //{
            //    var idEstudi = idestu.FirstOrDefault();
            //    idEst = idEstudi.Id;
            //}

            //int IdEstudiante = idEst;

            string nombreEst = "";
            List<Estudiante> nombEstu = db.Estudiante.Where(x => x.Id == cobro.idEstudiante).ToList();
            if (nombEstu != null && nombEstu.Count() > 0)
            {
                var nomb = nombEstu.FirstOrDefault();
                nombreEst = nomb.NombreCompleto;
            }

            string nombreEstud = nombreEst;

            string medioPago = "";

            List<MPago> mPago = db.MPago.Where(x => x.Id == cobro.idMPago).ToList();
            if (mPago != null && mPago.Count() > 0)
            {
                var medPag = mPago.FirstOrDefault();
                medioPago = medPag.Nombre;
            }

            string medioP = medioPago;

            var ListaPagos = (List<DetallePagos>)Session["ListaPagos"];

            string nombreconcept = "";
            decimal totalImporte = 0;


            foreach (var concep in ListaPagos)
            {
                nombreconcept += concep.Concepto + "\n";

                decimal importe = 0;
                importe = concep.importeTotal;

                totalImporte += importe;
            }

            // Contenido del ticket
            string ticketContent = "                       English College                " + "\n" +
                                   "                     Ricardo Palma 2097               " + "\n" +
                                   "---------------------------------------------------------  \n" +
                                   "Comprobante de Pago: " + cobro.idEstudiante + "\n" +
                                   "---------------------------------------------------------  \n" +
                                   "Fecha: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n" +
                                   "                                                       " + "\n" +
                                   "Medio de Pago: " + medioP + "\n" +
                                   "                                                       " + "\n" +
                                   "Alumno/a: " + nombreEstud + "\n" +
                                   "                                                       " + "\n" +
                                   "Importe Total: " + totalImporte.ToString("0,00") + "\n" +
                                   "                                                       " + "\n" +
                                   "Conceptos a pagar:\n" + nombreconcept + "\n" + "\n";

            //// Configura la fuente y el formato
            //Font font = new Font("Arial", 10);
            //StringFormat stringFormat = new StringFormat();
            //stringFormat.Alignment = StringAlignment.Near;

            // Llamamos al método que genera el PDF y lo devuelve como un ActionResult
            return GenerarPdf(ticketContent);

        }
    }
}