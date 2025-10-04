using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Examen_parcial_2.Data;
using PortalAcademico.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace Examen_parcial_2.Controllers
{
    [Authorize(Roles = "Coordinador")]
    public class CoordinadorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDistributedCache _cache;

        public CoordinadorController(ApplicationDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // --- ACCIONES CRUD PARA CURSOS ---

        // GET: /Coordinador
        public async Task<IActionResult> Index()
        {
            var cursos = await _context.Cursos.ToListAsync();
            return View(cursos);
        }

        // GET: /Coordinador/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Coordinador/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Codigo,Nombre,Creditos,CupoMaximo,HorarioInicio,HorarioFin,Activo")] Curso curso)
        {
            if (ModelState.IsValid)
            {
                if (curso.HorarioInicio >= curso.HorarioFin)
                {
                    ModelState.AddModelError("HorarioFin", "La hora de fin debe ser posterior a la hora de inicio.");
                    return View(curso);
                }

                _context.Add(curso);
                await _context.SaveChangesAsync();
                await _cache.RemoveAsync("ListaCursosActivos");
                
                return RedirectToAction(nameof(Index));
            }
            return View(curso);
        }

        // GET: /Coordinador/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null) return NotFound();
            return View(curso);
        }

        // POST: /Coordinador/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Codigo,Nombre,Creditos,CupoMaximo,HorarioInicio,HorarioFin,Activo")] Curso curso)
        {
            if (id != curso.Id) return NotFound();

            if (ModelState.IsValid)
            {
                // AÑADIDO: Validación de horario también al editar
                if (curso.HorarioInicio >= curso.HorarioFin)
                {
                    ModelState.AddModelError("HorarioFin", "La hora de fin debe ser posterior a la hora de inicio.");
                    return View(curso);
                }

                try
                {
                    _context.Update(curso);
                    await _context.SaveChangesAsync();
                    await _cache.RemoveAsync("ListaCursosActivos");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Cursos.Any(e => e.Id == curso.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(curso);
        }

        // --- AÑADIDO: ACCIONES PARA DESACTIVAR UN CURSO ---

        // GET: Coordinador/Deactivate/5
        public async Task<IActionResult> Deactivate(int? id)
        {
            if (id == null) return NotFound();

            var curso = await _context.Cursos.FirstOrDefaultAsync(m => m.Id == id);
            if (curso == null) return NotFound();

            return View(curso);
        }

        // POST: Coordinador/Deactivate/5
        [HttpPost, ActionName("Deactivate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateConfirmed(int id)
        {
            var curso = await _context.Cursos.FindAsync(id);
            if (curso != null)
            {
                curso.Activo = false; // Se cambia el estado a inactivo
                _context.Update(curso);
                await _context.SaveChangesAsync();
                await _cache.RemoveAsync("ListaCursosActivos"); // Invalidamos la caché
            }
            return RedirectToAction(nameof(Index));
        }

        // --- ACCIONES PARA GESTIONAR MATRÍCULAS ---

        // GET: /Coordinador/MatriculasPorCurso/5
        public async Task<IActionResult> MatriculasPorCurso(int? id)
        {
            if (id == null) return NotFound();
            
            var curso = await _context.Cursos
                .Include(c => c.Matriculas)
                .ThenInclude(m => m.Usuario)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (curso == null) return NotFound();

            return View(curso);
        }
        
        // POST: /Coordinador/ConfirmarMatricula
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarMatricula(int matriculaId, int cursoId)
        {
            var matricula = await _context.Matriculas.FindAsync(matriculaId);
            if(matricula != null)
            {
                matricula.Estado = EstadoMatricula.Confirmada;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("MatriculasPorCurso", new { id = cursoId });
        }

        // POST: /Coordinador/CancelarMatricula
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelarMatricula(int matriculaId, int cursoId)
        {
            var matricula = await _context.Matriculas.FindAsync(matriculaId);
            if (matricula != null)
            {
                matricula.Estado = EstadoMatricula.Cancelada;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("MatriculasPorCurso", new { id = cursoId });
        }
    }
}