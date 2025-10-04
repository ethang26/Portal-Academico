namespace PortalAcademico.Models;

public class FiltrosCursoViewModel
{
    public string? Nombre { get; set; }
    public int? CreditosMin { get; set; }
    public int? CreditosMax { get; set; }
    public TimeSpan? HorarioInicio { get; set; }
    public TimeSpan? HorarioFin { get; set; }

    public IEnumerable<Curso>? Cursos { get; set; }
}
