using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Data;
using PortalAcademico.Models;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Microsoft.AspNetCore.Http; // <-- para Session SetString/GetString

namespace PortalAcademico.Controllers;

[Route("cursos")]
public class CursosController : Controller
{
    private readonly ApplicationDbContext _ctx;
    private readonly IDistributedCache _cache;

    public CursosController(ApplicationDbContext ctx, IDistributedCache cache)
    {
        _ctx = ctx;
        _cache = cache;
    }

    // GET /cursos
    [HttpGet("")]
    public async Task<IActionResult> Index([FromQuery] FiltrosCursoViewModel filtros)
    {
        const string cacheKey = "cursos_activos";
        string? cachedJson = await _cache.GetStringAsync(cacheKey);
        List<Curso> cursos;

        if (!string.IsNullOrEmpty(cachedJson))
        {
            cursos = JsonSerializer.Deserialize<List<Curso>>(cachedJson) ?? new List<Curso>();
        }
        else
        {
            cursos = await _ctx.Cursos
                .Where(c => c.Activo)
                .OrderBy(c => c.Codigo)
                .AsNoTracking()
                .ToListAsync();

            var options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(60));
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(cursos), options);
        }

        filtros.Cursos = cursos;
        return View(filtros);
    }

    [HttpPost("crear")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(Curso curso)
    {
        if (!ModelState.IsValid) return View(curso);

        _ctx.Cursos.Add(curso);
        await _ctx.SaveChangesAsync();

        await _cache.RemoveAsync("cursos_activos"); // invalidar cache
        return RedirectToAction("Index");
    }

    [HttpPost("editar/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, Curso curso)
    {
        if (id != curso.Id) return BadRequest();

        _ctx.Update(curso);
        await _ctx.SaveChangesAsync();

        await _cache.RemoveAsync("cursos_activos"); // invalidar cache
        return RedirectToAction("Index");
    }

    // GET /cursos/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Detalle(int id)
    {
        var curso = await _ctx.Cursos
            .Include(c => c.Matriculas)
            .FirstOrDefaultAsync(c => c.Id == id && c.Activo);

        if (curso == null) return NotFound();

        // Guardar el último curso visitado en sesión (requisito P4)
        HttpContext.Session.SetString("UltimoCursoId", curso.Id.ToString());
        HttpContext.Session.SetString("UltimoCursoNombre", curso.Nombre);

        ViewBag.CuposOcupados = curso.Matriculas.Count(m => m.Estado != EstadoMatricula.Cancelada);
        return View(curso);
    }
}
