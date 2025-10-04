using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Data;
using PortalAcademico.Models;

namespace PortalAcademico.Controllers;

[Authorize(Roles = "Coordinador")]  
[Route("coordinador")]
public class CoordinadorController : Controller
{
    private readonly ApplicationDbContext _ctx;

    public CoordinadorController(ApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    // GET /coordinador/cursos
    [HttpGet("cursos")]
    public async Task<IActionResult> Cursos()
    {
        var cursos = await _ctx.Cursos.ToListAsync();
        return View(cursos);
    }

    // GET /coordinador/cursos/crear
    [HttpGet("cursos/crear")]
    public IActionResult CrearCurso() => View();

    // POST /coordinador/cursos/crear
    [HttpPost("cursos/crear")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearCurso(Curso curso)
    {
        if (ModelState.IsValid)
        {
            _ctx.Cursos.Add(curso);
            await _ctx.SaveChangesAsync();
            return RedirectToAction("Cursos");
        }
        return View(curso);
    }

    // GET /coordinador/cursos/editar/{id}
    [HttpGet("cursos/editar/{id:int}")]
    public async Task<IActionResult> EditarCurso(int id)
    {
        var curso = await _ctx.Cursos.FindAsync(id);
        if (curso == null) return NotFound();
        return View(curso);
    }

    // POST /coordinador/cursos/editar/{id}
    [HttpPost("cursos/editar/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarCurso(int id, Curso curso)
    {
        if (id != curso.Id) return BadRequest();

        if (ModelState.IsValid)
        {
            _ctx.Update(curso);
            await _ctx.SaveChangesAsync();
            return RedirectToAction("Cursos");
        }
        return View(curso);
    }

    // POST /coordinador/cursos/desactivar/{id}
    [HttpPost("cursos/desactivar/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DesactivarCurso(int id)
    {
        var curso = await _ctx.Cursos.FindAsync(id);
        if (curso == null) return NotFound();

        curso.Activo = false; // Desactivar el curso
        await _ctx.SaveChangesAsync();
        return RedirectToAction("Cursos");
    }
}
