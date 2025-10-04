using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Examen_parcial_2.Data;
using PortalAcademico.Models;
using System.Security.Claims;

namespace PortalAcademico.Controllers
{
    [Authorize] 
    public class MatriculasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MatriculasController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Inscribir(int cursoId)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            TempData["ErrorMessage"] = "Error de autenticación. Por favor, inicia sesión nuevamente.";
            return RedirectToAction("Index", "Cursos");
        }

        var cursoAInscribir = await _context.Cursos.FindAsync(cursoId);

        if (cursoAInscribir == null)
        {
            TempData["ErrorMessage"] = "El curso que intentas inscribir no existe.";
            return RedirectToAction("Index", "Cursos");
        }

        var yaEstaInscrito = await _context.Matriculas
            .AnyAsync(m => m.CursoId == cursoId && m.UsuarioId == userId);

        if (yaEstaInscrito)
        {
            TempData["ErrorMessage"] = "Ya te encuentras matriculado en este curso.";
            return RedirectToAction("Details", "Cursos", new { id = cursoId });
        }

        var matriculasConfirmadas = await _context.Matriculas
            .CountAsync(m => m.CursoId == cursoId && m.Estado == EstadoMatricula.Confirmada);

        if (matriculasConfirmadas >= cursoAInscribir.CupoMaximo)
        {
            TempData["ErrorMessage"] = "No quedan cupos disponibles para este curso.";
            return RedirectToAction("Details", "Cursos", new { id = cursoId });
        }

        var misOtrasMatriculas = await _context.Matriculas
            .Where(m => m.UsuarioId == userId && m.Estado == EstadoMatricula.Confirmada)
            .Include(m => m.Curso)
            .ToListAsync();

        foreach (var matriculaExistente in misOtrasMatriculas)
        {
            if (matriculaExistente.Curso != null)
            {
                bool seSolapa = cursoAInscribir.HorarioInicio < matriculaExistente.Curso.HorarioFin &&
                                cursoAInscribir.HorarioFin > matriculaExistente.Curso.HorarioInicio;
                if (seSolapa)
                {
                    TempData["ErrorMessage"] = $"El horario de este curso se cruza con el de '{matriculaExistente.Curso.Nombre}'.";
                    return RedirectToAction("Details", "Cursos", new { id = cursoId });
                }
            }
        }

        var nuevaMatricula = new Matricula
        {
            CursoId = cursoId,
            UsuarioId = userId,
            FechaRegistro = DateTime.UtcNow,
            Estado = EstadoMatricula.Pendiente 
        };

        _context.Matriculas.Add(nuevaMatricula);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "¡Inscripción registrada con éxito! Tu matrícula está pendiente de confirmación.";
        return RedirectToAction("Details", "Cursos", new { id = cursoId });
    }
    }
}