using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Data;
using PortalAcademico.Models;

namespace PortalAcademico.Controllers;

[Route("cursos")]
public class CursosController : Controller
{
    private readonly ApplicationDbContext _ctx;
    public CursosController(ApplicationDbContext ctx) => _ctx = ctx;

    // GET /cursos
    [HttpGet("")]
     public async Task<IActionResult> Index([FromQuery] FiltrosCursoViewModel filtros)
    {
        var query = _ctx.Cursos.AsQueryable().Where(c => c.Activo);

        // Validaciones
        if (filtros.CreditosMin < 0 || filtros.CreditosMax < 0)
            ModelState.AddModelError("", "Los créditos no pueden ser negativos.");

        if (filtros.HorarioInicio.HasValue && filtros.HorarioFin.HasValue &&
            filtros.HorarioInicio >= filtros.HorarioFin)
            ModelState.AddModelError("", "El horario inicial debe ser anterior al horario final.");

        if (!ModelState.IsValid)
        {
            filtros.Cursos = await query.ToListAsync();
            return View(filtros);
        }

        // Filtros dinámicos
        if (!string.IsNullOrWhiteSpace(filtros.Nombre))
            query = query.Where(c => c.Nombre.Contains(filtros.Nombre));

        if (filtros.CreditosMin.HasValue)
            query = query.Where(c => c.Creditos >= filtros.CreditosMin);

        if (filtros.CreditosMax.HasValue)
            query = query.Where(c => c.Creditos <= filtros.CreditosMax);

        if (filtros.HorarioInicio.HasValue)
            query = query.Where(c => c.HorarioInicio >= filtros.HorarioInicio);

        if (filtros.HorarioFin.HasValue)
            query = query.Where(c => c.HorarioFin <= filtros.HorarioFin);

        filtros.Cursos = await query.OrderBy(c => c.Codigo).ToListAsync();

        return View(filtros);
    }

    // GET /cursos/{id}
    [HttpGet("{id:int}")]
     public async Task<IActionResult> Detalle(int id)
    {
       var curso = await _ctx.Cursos
        .Include(c => c.Matriculas)
        .FirstOrDefaultAsync(c => c.Id == id && c.Activo);

    if (curso == null) return NotFound();

    ViewBag.CuposOcupados = curso.Matriculas.Count(m => m.Estado != EstadoMatricula.Cancelada);
    return View(curso);
    }
}
