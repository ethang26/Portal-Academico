using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Data;
using PortalAcademico.Models;
using PortalAcademico.Utils;

namespace PortalAcademico.Controllers;

[Authorize]
[Route("matriculas")]
public class MatriculasController : Controller
{
    private readonly ApplicationDbContext _ctx;
    private readonly UserManager<IdentityUser> _userMgr;

    public MatriculasController(ApplicationDbContext ctx, UserManager<IdentityUser> userMgr)
    {
        _ctx = ctx;
        _userMgr = userMgr;
    }

    // POST /matriculas/inscribirse/{cursoId}
    [HttpPost("inscribirse/{cursoId:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Inscribirse(int cursoId)
    {
        // 1) Usuario autenticado (Authorize ya lo exige)
        var user = await _userMgr.GetUserAsync(User);
        if (user == null) return Challenge();

        // 2) Curso válido y ACTIVO
        var curso = await _ctx.Cursos
            .Include(c => c.Matriculas)
            .FirstOrDefaultAsync(c => c.Id == cursoId && c.Activo);
        if (curso == null)
        {
            TempData["Error"] = "Curso no encontrado o inactivo.";
            return RedirectToAction("Index", "Cursos");
        }

        // 3) No duplicar la matrícula del mismo usuario en el mismo curso
        var yaInscrito = await _ctx.Matriculas
            .AnyAsync(m => m.CursoId == cursoId && m.UsuarioId == user.Id && m.Estado != EstadoMatricula.Cancelada);
        if (yaInscrito)
        {
            TempData["Error"] = "Ya estás inscrito en este curso.";
            return RedirectToAction("Detalle", "Cursos", new { id = cursoId });
        }

        // 4) Validar cupo (matrículas no canceladas)
        var ocupados = await _ctx.Matriculas
            .CountAsync(m => m.CursoId == cursoId && m.Estado != EstadoMatricula.Cancelada);
        if (ocupados >= curso.CupoMaximo)
        {
            TempData["Error"] = "No hay cupos disponibles.";
            return RedirectToAction("Detalle", "Cursos", new { id = cursoId });
        }

        // 5) Validar NO solaparse con otros cursos del usuario (no cancelados)
        var misCursos = await _ctx.Matriculas
            .Where(m => m.UsuarioId == user.Id && m.Estado != EstadoMatricula.Cancelada)
            .Select(m => m.Curso)
            .ToListAsync();

        var choca = misCursos.Any(c =>
            HorarioHelper.SeSolapan(c.HorarioInicio, c.HorarioFin, curso.HorarioInicio, curso.HorarioFin));

        if (choca)
        {
            TempData["Error"] = "El horario de este curso se solapa con otro en el que ya estás matriculado.";
            return RedirectToAction("Detalle", "Cursos", new { id = cursoId });
        }

        // 6) Crear matrícula en estado Pendiente
        _ctx.Matriculas.Add(new Matricula
        {
            CursoId = cursoId,
            UsuarioId = user.Id,
            Estado = EstadoMatricula.Pendiente,
            FechaRegistro = DateTime.UtcNow
        });

        try
        {
            await _ctx.SaveChangesAsync();
            TempData["Ok"] = "Matrícula creada en estado Pendiente.";
        }
        catch (DbUpdateException)
        {
            // En caso de carrera, si otro ocupó el último cupo, revertimos
            TempData["Error"] = "No fue posible completar la inscripción (cupo agotado). Intenta de nuevo.";
        }

        return RedirectToAction("Detalle", "Cursos", new { id = cursoId });
    }
}
