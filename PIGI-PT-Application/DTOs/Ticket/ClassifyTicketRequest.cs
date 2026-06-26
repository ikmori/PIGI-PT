using System.ComponentModel.DataAnnotations;

namespace PIGI_PT_Application.DTOs.Ticket
{
    /// <summary>
    /// DTO de entrada para clasificar un ticket existente.
    /// </summary>
    public class ClassifyTicketRequest
    {
        /// <summary>
        /// Prioridad a asignar: 1=Baja, 2=Media, 3=Alta, 4=Crítica.
        /// </summary>
        [Required(ErrorMessage = "La prioridad es obligatoria.")]
        [Range(1, 4, ErrorMessage = "La prioridad debe ser un valor entre 1 (Baja) y 4 (Crítica).")]
        public int Prioridad { get; set; }

        /// <summary>ID de la categoría a asignar al ticket.</summary>
        [Required(ErrorMessage = "El CategoriaId es obligatorio.")]
        public Guid CategoriaId { get; set; }

        /// <summary>ID del usuario que realiza la clasificación.</summary>
        [Required(ErrorMessage = "El UserId es obligatorio.")]
        public Guid UserId { get; set; }
    }
}
