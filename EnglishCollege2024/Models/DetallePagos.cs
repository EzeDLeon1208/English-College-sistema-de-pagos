namespace EnglishCollege2024
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class DetallePagos
    {
        public int Id { get; set; }

        public int idCobro { get; set; }

        public int IdConcepto { get; set; }

        public int IdEstudiante { get; set; }

        public string Concepto { get; set; }

        public decimal importeTotal { get; set; }

        public decimal sumaImportes { get; set; }

        public string fechaDetal { get; set; }

        public DateTime FechaPago { get; set; }

        public bool Activo { get; set; }

        public string MPago { get; set; }
    }
}