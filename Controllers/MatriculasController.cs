using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Data;
using PortalAcademico.Models;

namespace PortalAcademico.Controllers;

[Authorize]
[Route("matriculas")]
public class MatriculasController : Controller
{
    private readonly ApplicationDbContext _ctx;
    private readonly UserManager<IdentityUser> _userMgr;
    public MatriculasController(ApplicationDbContext ctx, UserManager<IdentityUser> userMgr)
    {
        _ctx = ctx; _userMgr = userMgr;
    }

    // POST /matriculas/inscribirse/{cursoId}
    [HttpPost("inscribirse/{cursoId:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Inscribirse(int cursoId)
    {
        var user = await _userMgr.GetUserAsync(User);
        if (user == null) return Challenge();

        // No duplicar matrícula del mismo usuario en el mismo curso
        var yaInscrito = await _ctx.Matriculas.AnyAsync(m => m.CursoId == cursoId && m.UsuarioId == user.Id);
        if (yaInscrito)
        {
            TempData["Error"] = "Ya estás inscrito en este curso.";
            return RedirectToAction("Detalle", "Cursos", new { id = cursoId });
        }

        // Validar cupo
        var cupo = await _ctx.Cursos
            .Where(c => c.Id == cursoId)
            .Select(c => new { c.CupoMaximo, Ocupado = c.Matriculas.Count(m => m.Estado != EstadoMatricula.Cancelada) })
            .FirstOrDefaultAsync();

        if (cupo == null)
            return NotFound();

        if (cupo.Ocupado >= cupo.CupoMaximo)
        {
            TempData["Error"] = "No hay cupos disponibles.";
            return RedirectToAction("Detalle", "Cursos", new { id = cursoId });
        }

        _ctx.Matriculas.Add(new Matricula { CursoId = cursoId, UsuarioId = user.Id, Estado = EstadoMatricula.Pendiente });
        await _ctx.SaveChangesAsync();

        TempData["Ok"] = "Matrícula creada en estado Pendiente.";
        return RedirectToAction("Detalle", "Cursos", new { id = cursoId });
    }
}
