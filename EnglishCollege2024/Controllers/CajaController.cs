using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EnglishCollege2024.Controllers
{
    public class CajaController : Controller
    {
        private EnglishCollegeDbContext db = new EnglishCollegeDbContext();
        // GET: Deuda
        public ActionResult CajaDelDia()
        {
            //string fechaAct = DateTime.Now.ToString("dd-MM-yyyy");

            List<Caja> cajaDiaria = new List<Caja>();

            // Busca todos los cobros
            List<Cobro> cobros = db.Cobro.ToList();
            if (cobros != null && cobros.Count() > 0)
            {
                foreach (var cobro in cobros)
                {
                    //string fechaCobro = cobro.FechaPago.ToString("dd-MM-yyyy");

                    if(cobro.FechaPago >= DateTime.Today && cobro.FechaPago <= DateTime.Now)
                    {
                        // Obtén el concepto relacionado con el cobro
                        Concepto concepto = db.Concepto.FirstOrDefault(con => con.Id == cobro.idConcepto);

                        MPago MedPago = db.MPago.FirstOrDefault(med => med.Id == cobro.idMPago);

                        if (concepto != null)
                        {
                            // Crea un objeto Deudas para cada cobro
                            Caja cajaDia = new Caja();
                            cajaDia.id = cobro.Id;
                            cajaDia.Concepto = concepto.Nombre.ToString();
                            cajaDia.idMpago = MedPago.Id;
                            cajaDia.Mpago = MedPago.Nombre.ToString();
                            cajaDia.precioConcepto = cobro.precioTotal;
                            cajaDia.FechaCobroDesde = cobro.FechaPago;
                            //cajaDia.FechaCobro = cobro.FechaPago.ToString("dd-MM-yyyy");

                            cajaDiaria.Add(cajaDia);
                        }
                    }
                }
            }
            else
            {
                ViewBag.listaCaja = null;

                return View("Caja");
            }

            decimal sumaTotalEfectivo = 0;
            decimal sumaTotalVirtual = 0;

            foreach (var item in cajaDiaria)
            {
                if(item.idMpago == 1)
                {
                    decimal importe = 0;
                    importe = (decimal)item.precioConcepto;

                    sumaTotalEfectivo += importe;
                }
                else
                {
                    decimal importe = 0;
                    importe = (decimal)item.precioConcepto;

                    sumaTotalVirtual += importe;
                }
                
            }

            ViewBag.CajaTotalEfectivo = sumaTotalEfectivo;

            ViewBag.CajaTotalVirtual = sumaTotalVirtual;

            ViewBag.listaCaja = cajaDiaria;

            return View("Caja");
        }

        [HttpPost]
        public ActionResult CajaPorFecha(Caja fechas)
        {
            List<Caja> cajaPorFecha = new List<Caja>();

            // Busca todos los cobros
            List<Cobro> cobros = db.Cobro.ToList();
            if (cobros != null && cobros.Count() > 0)
            {
                foreach (var cobro in cobros)
                {
                    //string fechaCobro = cobro.FechaPago.ToString("yyyy-MM-dd");
                    //fechaElegida.fechaPago = cobro.FechaPago;

                    if (cobro.FechaPago >= fechas.FechaCobroDesde && cobro.FechaPago <= fechas.FechaCobroHasta)
                    {
                        // Obtén el concepto relacionado con el cobro
                        Concepto concepto = db.Concepto.FirstOrDefault(con => con.Id == cobro.idConcepto);

                        MPago MedPago = db.MPago.FirstOrDefault(med => med.Id == cobro.idMPago);

                        if (concepto != null)
                        {
                            // Crea un objeto Deudas para cada cobro
                            Caja cajaDia = new Caja();
                            cajaDia.id = cobro.Id;
                            cajaDia.Concepto = concepto.Nombre.ToString();
                            cajaDia.idMpago = MedPago.Id;
                            cajaDia.Mpago = MedPago.Nombre.ToString();
                            cajaDia.precioConcepto = cobro.precioTotal;
                            cajaDia.FechaCobroDesde = cobro.FechaPago;
                            //cajaDia.TotalCobrado += (decimal)cobro.precioTotal;

                            cajaPorFecha.Add(cajaDia);
                        }
                    }
                }
            }

            decimal sumaTotalEfectivo = 0;
            decimal sumaTotalVirtual = 0;

            foreach (var item in cajaPorFecha)
            {
                if (item.idMpago == 1)
                {
                    decimal importe = 0;
                    importe = (decimal)item.precioConcepto;

                    sumaTotalEfectivo += importe;
                }
                else
                {
                    decimal importe = 0;
                    importe = (decimal)item.precioConcepto;

                    sumaTotalVirtual += importe;
                }

            }

            ViewBag.CajaTotalEfectivo = sumaTotalEfectivo;

            ViewBag.CajaTotalVirtual = sumaTotalVirtual;

            ViewBag.listaCaja = cajaPorFecha;

            return View("Caja");
        }

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
