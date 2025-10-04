using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Data;

namespace PortalAcademico.Controllers;

[Route("cursos")]
public class CursosController : Controller
{
    private readonly ApplicationDbContext _ctx;
    public CursosController(ApplicationDbContext ctx) => _ctx = ctx;

    // GET /cursos
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var cursos = await _ctx.Cursos
            .Where(c => c.Activo)
            .OrderBy(c => c.Codigo)
            .ToListAsync();
        return View(cursos);
    }

    // GET /cursos/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Detalle(int id)
    {
        var curso = await _ctx.Cursos.FindAsync(id);
        if (curso == null) return NotFound();
        return View(curso);
    }
}
