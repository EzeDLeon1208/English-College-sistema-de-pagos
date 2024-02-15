namespace EnglishCollege2024
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Barrio")]
    public partial class Barrio
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Barrio()
        {
            Estudiante = new HashSet<Estudiante>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Nombre { get; set; }

        public int? idLocalidad { get; set; }

        public virtual Localidad Localidad { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Estudiante> Estudiante { get; set; }
    }
}
