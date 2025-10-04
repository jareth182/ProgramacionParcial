using System.ComponentModel.DataAnnotations;

namespace PortalAcademico.Models
{
    public class Curso
    {
        public int Id { get; set; }

        [Required]
        public required string Codigo { get; set; }

        [Required]
        public required string Nombre { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Los créditos deben ser mayores a 0.")]
        public int Creditos { get; set; }

        [Required]
        public int CupoMaximo { get; set; }

        [Required]
        public TimeOnly HorarioInicio { get; set; }

        [Required]
        public TimeOnly HorarioFin { get; set; }

        public bool Activo { get; set; } = true;
    }
}