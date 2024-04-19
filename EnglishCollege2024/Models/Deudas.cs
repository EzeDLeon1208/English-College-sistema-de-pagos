namespace EnglishCollege2024
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Cobro")]
    public partial class Deudas
    {
        public int? id { get; set; }

        public int? idEstudiante { get; set; }

        public string Concepto { get; set; }

        public bool Activo { get; set; }

        public decimal deudaPorConcepto { get; set; }

        public decimal TotalAdeudado { get; set; }

        public virtual Estudiante Estudiante { get; set; }
    }
}