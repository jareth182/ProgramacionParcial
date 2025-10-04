using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Models;

namespace Examen_parcial_2.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Curso> Cursos { get; set; }
    public DbSet<Matricula> Matriculas { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
{
    base.OnModelCreating(builder);

    // Restricción: Código de curso debe ser único 
    builder.Entity<Curso>()
        .HasIndex(c => c.Codigo)
        .IsUnique();

    // Restricción: Un usuario no puede matricularse dos veces en el mismo curso
    builder.Entity<Matricula>()
        .HasIndex(m => new { m.UsuarioId, m.CursoId })
        .IsUnique();

    // Datos iniciales (Seed Data) para Cursos y Rol
    builder.Entity<Curso>().HasData(
        new Curso { Id = 1, Codigo = "PRG1", Nombre = "Programación I", Creditos = 4, CupoMaximo = 30, HorarioInicio = new TimeOnly(8, 0), HorarioFin = new TimeOnly(10, 0), Activo = true },
        new Curso { Id = 2, Codigo = "DBD1", Nombre = "Bases de Datos I", Creditos = 4, CupoMaximo = 25, HorarioInicio = new TimeOnly(10, 0), HorarioFin = new TimeOnly(12, 0), Activo = true },
        new Curso { Id = 3, Codigo = "MAT1", Nombre = "Matemática Básica", Creditos = 5, CupoMaximo = 35, HorarioInicio = new TimeOnly(14, 0), HorarioFin = new TimeOnly(16, 0), Activo = true }
    );

    builder.Entity<IdentityRole>().HasData(
        new IdentityRole { Id = "a6a58145-84d7-4279-840a-9e283d47c4c3", Name = "Coordinador", NormalizedName = "COORDINADOR" }
    );
}
}
