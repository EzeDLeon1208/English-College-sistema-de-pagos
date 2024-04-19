namespace EnglishCollege2024
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Cobro")]
    public partial class DetalleDeudas
    {
        public int? id { get; set; }

        public int idConcepto { get; set; }

        public bool TieneDeuda { get; set; }

        public string NombreCompleto { get; set; }

        public string DNI { get; set; }

        public string Concepto { get; set; }

        public decimal deudaPorConcepto { get; set; }

    }
}