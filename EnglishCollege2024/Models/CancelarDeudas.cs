namespace EnglishCollege2024
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    [Table("Cobro")]
    public partial class CancelarDeudas
    {
        public int? id { get; set; }

        public int? idEstudiante { get; set; }

        public string Concepto { get; set; }

        public bool Activo { get; set; }

        public decimal deudaPorConcepto { get; set; }

        public decimal TotalAdeudado { get; set; }
    }
}