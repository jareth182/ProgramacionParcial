using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Examen_parcial_2.Data;
using PortalAcademico.Models;

namespace PortalAcademico.Controllers
{
    public class CursosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CursosController(ApplicationDbContext context)
        {
            _context = context;
        }

    public async Task<IActionResult> Index(string searchString, int? minCreditos, int? maxCreditos)
    {
        // Empezamos con la consulta base que solo trae cursos activos
        var cursosQuery = _context.Cursos.Where(c => c.Activo);

        // Filtro 1: Por nombre del curso
        if (!String.IsNullOrEmpty(searchString))
        {
            cursosQuery = cursosQuery.Where(c => c.Nombre.Contains(searchString));
        }

        // Filtro 2: Rango de créditos (mínimo)
        if (minCreditos.HasValue)
        {
            cursosQuery = cursosQuery.Where(c => c.Creditos >= minCreditos.Value);
        }

        // Filtro 3: Rango de créditos (máximo)
        if (maxCreditos.HasValue)
        {
            cursosQuery = cursosQuery.Where(c => c.Creditos <= maxCreditos.Value);
        }

        // Guardamos los filtros en ViewData para que los inputs en la vista recuerden los valores
        ViewData["CurrentFilter"] = searchString;
        ViewData["MinCreditos"] = minCreditos;
        ViewData["MaxCreditos"] = maxCreditos;

        var cursos = await cursosQuery.ToListAsync();
        return View(cursos);
    }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var curso = await _context.Cursos
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (curso == null)
            {
                return NotFound();
            }

            return View(curso);
        }
    }
}