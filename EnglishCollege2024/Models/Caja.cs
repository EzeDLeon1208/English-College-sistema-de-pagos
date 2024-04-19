namespace EnglishCollege2024
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Cobro")]
    public partial class Caja
    {
        public int? id { get; set; }

        public int idConcepto { get; set; }

        public string Concepto { get; set; }

        public decimal? precioConcepto { get; set; }

        public int idMpago { get; set; }

        public string Mpago { get; set; }

        public DateTime FechaCobroDesde { get; set; }

        public DateTime FechaCobroHasta { get; set; }

        //public DateTime fechaPago { get; set; }

    }
}