using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Examen_parcial_2.Data; // Corregido para consistencia
using PortalAcademico.Models; // Corregido para consistencia
using Microsoft.Extensions.Caching.Distributed; // Necesario para IDistributedCache
using System.Text.Json;

namespace Examen_parcial_2.Controllers 
{
    public class CursosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDistributedCache _cache;

        public CursosController(ApplicationDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<IActionResult> Index(string searchString, int? minCreditos, int? maxCreditos)
        {
            string cacheKey = "ListaCursosActivos";
            List<Curso> cursos;
            string serializedCursos;

            bool hasFilters = !string.IsNullOrEmpty(searchString) || minCreditos.HasValue || maxCreditos.HasValue;

            if (!hasFilters)
            {
                var cachedCursos = await _cache.GetStringAsync(cacheKey);
                if (cachedCursos != null)
                {
                    cursos = JsonSerializer.Deserialize<List<Curso>>(cachedCursos) ?? new List<Curso>();
                }
                else
                {
                    cursos = await _context.Cursos.Where(c => c.Activo).ToListAsync();
                    serializedCursos = JsonSerializer.Serialize(cursos);

                    await _cache.SetStringAsync(cacheKey, serializedCursos, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
                    });
                }
            }
            else
            {
                var cursosQuery = _context.Cursos.Where(c => c.Activo);

                if (!String.IsNullOrEmpty(searchString))
                {
                    cursosQuery = cursosQuery.Where(c => c.Nombre.Contains(searchString));
                }
                if (minCreditos.HasValue)
                {
                    cursosQuery = cursosQuery.Where(c => c.Creditos >= minCreditos.Value);
                }
                if (maxCreditos.HasValue)
                {
                    cursosQuery = cursosQuery.Where(c => c.Creditos <= maxCreditos.Value);
                }
                cursos = await cursosQuery.ToListAsync();
            }
            

            ViewData["CurrentFilter"] = searchString;
            ViewData["MinCreditos"] = minCreditos;
            ViewData["MaxCreditos"] = maxCreditos;

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
            
            HttpContext.Session.SetString("LastCourseName", curso.Nombre);
            HttpContext.Session.SetInt32("LastCourseId", curso.Id);
            
            return View(curso);
        }
    }
}