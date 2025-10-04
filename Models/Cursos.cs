using System.ComponentModel.DataAnnotations;

namespace PortalAcademico.Models;

public class Curso
{
    public int Id { get; set; }

    [Required, MaxLength(10)]
    public string Codigo { get; set; } = default!;

    [Required, MaxLength(120)]
    public string Nombre { get; set; } = default!;

    [Range(1, int.MaxValue, ErrorMessage = "Créditos debe ser > 0")]
    public int Creditos { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Cupo máximo debe ser > 0")]
    public int CupoMaximo { get; set; }

    [Display(Name = "Inicio")]
    public TimeSpan HorarioInicio { get; set; }

    [Display(Name = "Fin")]
    public TimeSpan HorarioFin { get; set; }

    public bool Activo { get; set; } = true;

    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
}
