namespace EnglishCollege2024
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Estudiante")]
    public partial class Estudiante
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Estudiante()
        {
            Cobro = new HashSet<Cobro>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(8)]
        public string DNI { get; set; }

        [Required]
        [StringLength(200)]
        public string NombreCompleto { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)] 
        public DateTime FechaNacimiento { get; set; }

        [StringLength(250)]
        public string Direccion { get; set; }

        [StringLength(20)]
        public string Telefono1 { get; set; }

        [StringLength(20)]
        public string Telefono2 { get; set; }

        public decimal PendientePago { get; set; }

        public int? idBarrio { get; set; }

        public int? idCurso { get; set; }

        public bool TieneDeuda { get; set; }

        public bool Activo { get; set; }

        public virtual Barrio Barrio { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cobro> Cobro { get; set; }

        public virtual Curso Curso { get; set; }
    }
}
