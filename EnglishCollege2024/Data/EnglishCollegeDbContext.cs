using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace EnglishCollege2024
{
    public partial class EnglishCollegeDbContext : DbContext
    {
        public EnglishCollegeDbContext()
            : base("name=EnglishCollegeDbContext")
        {
        }

        public virtual DbSet<Barrio> Barrio { get; set; }
        public virtual DbSet<Cobro> Cobro { get; set; }
        public virtual DbSet<Concepto> Concepto { get; set; }
        public virtual DbSet<Cuento> Cuento { get; set; }
        public virtual DbSet<Curso> Curso { get; set; }
        public virtual DbSet<Estudiante> Estudiante { get; set; }
        public virtual DbSet<Libro> Libro { get; set; }
        public virtual DbSet<Localidad> Localidad { get; set; }
        public virtual DbSet<MPago> MPago { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Barrio>()
                .Property(e => e.Nombre)
                .IsUnicode(false);

            modelBuilder.Entity<Barrio>()
                .HasMany(e => e.Estudiante)
                .WithOptional(e => e.Barrio)
                .HasForeignKey(e => e.idBarrio);

            modelBuilder.Entity<Cobro>()
                .Property(e => e.precioTotal)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Cobro>()
                .Property(e => e.Deuda)
                .HasPrecision(20, 2);

            modelBuilder.Entity<Concepto>()
                .Property(e => e.Nombre)
                .IsUnicode(false);

            modelBuilder.Entity<Concepto>()
                .HasMany(e => e.Cobro)
                .WithRequired(e => e.Concepto)
                .HasForeignKey(e => e.idConcepto)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Cuento>()
                .Property(e => e.Nombre)
                .IsUnicode(false);

            modelBuilder.Entity<Cuento>()
                .Property(e => e.Precio)
                .HasPrecision(20, 2);

            modelBuilder.Entity<Cuento>()
                .HasMany(e => e.Curso)
                .WithOptional(e => e.Cuento)
                .HasForeignKey(e => e.idCuento);

            modelBuilder.Entity<Curso>()
                .Property(e => e.Nombre)
                .IsUnicode(false);

            modelBuilder.Entity<Curso>()
                .Property(e => e.Precio)
                .HasPrecision(20, 2);

            modelBuilder.Entity<Curso>()
                .Property(e => e.PrecioLibro)
                .HasPrecision(20, 2);

            modelBuilder.Entity<Curso>()
                .HasMany(e => e.Estudiante)
                .WithOptional(e => e.Curso)
                .HasForeignKey(e => e.idCurso);

            modelBuilder.Entity<Estudiante>()
                .Property(e => e.DNI)
                .IsUnicode(false);

            modelBuilder.Entity<Estudiante>()
                .Property(e => e.NombreCompleto)
                .IsUnicode(false);

            modelBuilder.Entity<Estudiante>()
                .Property(e => e.Direccion)
                .IsUnicode(false);

            modelBuilder.Entity<Estudiante>()
                .Property(e => e.Telefono1)
                .IsUnicode(false);

            modelBuilder.Entity<Estudiante>()
                .Property(e => e.Telefono2)
                .IsUnicode(false);

            modelBuilder.Entity<Estudiante>()
                .Property(e => e.PendientePago)
                .HasPrecision(20, 0);

            modelBuilder.Entity<Estudiante>()
                .HasMany(e => e.Cobro)
                .WithOptional(e => e.Estudiante)
                .HasForeignKey(e => e.idEstudiante);

            modelBuilder.Entity<Libro>()
                .Property(e => e.Nombre)
                .IsUnicode(false);

            modelBuilder.Entity<Libro>()
                .Property(e => e.Precio)
                .HasPrecision(20, 2);

            modelBuilder.Entity<Libro>()
                .HasMany(e => e.Curso)
                .WithOptional(e => e.Libro)
                .HasForeignKey(e => e.idLibro);

            modelBuilder.Entity<Localidad>()
                .Property(e => e.Nombre)
                .IsUnicode(false);

            modelBuilder.Entity<Localidad>()
                .HasMany(e => e.Barrio)
                .WithOptional(e => e.Localidad)
                .HasForeignKey(e => e.idLocalidad);

            modelBuilder.Entity<MPago>()
                .Property(e => e.Nombre)
                .IsUnicode(false);

            modelBuilder.Entity<MPago>()
                .HasMany(e => e.Cobro)
                .WithRequired(e => e.MPago)
                .HasForeignKey(e => e.idMPago)
                .WillCascadeOnDelete(false);
        }
    }
}
