using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PortalAcademico.Models
{
    public enum EstadoMatricula
    {
        Pendiente,
        Confirmada,
        Cancelada
    }

    public class Matricula
    {
        public int Id { get; set; }
        public int CursoId { get; set; }
        public required string UsuarioId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public EstadoMatricula Estado { get; set; }

        // Navigation properties
        public virtual required Curso Curso { get; set; }
        public virtual required IdentityUser Usuario { get; set; }
    }
}