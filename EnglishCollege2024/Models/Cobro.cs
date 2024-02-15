namespace EnglishCollege2024
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Cobro")]
    public partial class Cobro
    {
        public int Id { get; set; }

        public int? idEstudiante { get; set; }

        public int? idConcepto { get; set; }

        public int? idMPago { get; set; }

        public decimal? precioTotal { get; set; }

        public decimal Deuda { get; set; }

        public DateTime FechaPago { get; set; }

        public bool Activo { get; set; }

        public virtual Concepto Concepto { get; set; }

        public virtual Estudiante Estudiante { get; set; }

        public virtual MPago MPago { get; set; }
    }
}
