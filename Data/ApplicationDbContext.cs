using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Models;

namespace PortalAcademico.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Curso> Cursos => Set<Curso>();
    public DbSet<Matricula> Matriculas => Set<Matricula>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Curso.Codigo único
        builder.Entity<Curso>()
            .HasIndex(c => c.Codigo)
            .IsUnique();

        // Un usuario no puede estar matriculado más de una vez en el mismo curso
        builder.Entity<Matricula>()
            .HasIndex(m => new { m.CursoId, m.UsuarioId })
            .IsUnique();

        // HorarioInicio < HorarioFin (check constraint)
        builder.Entity<Curso>()
            .ToTable(t => t.HasCheckConstraint("CK_Curso_Horario", "HorarioInicio < HorarioFin"));

        // Créditos > 0 (check constraint)
        builder.Entity<Curso>()
            .ToTable(t => t.HasCheckConstraint("CK_Curso_Creditos", "Creditos > 0"));
    }
}
