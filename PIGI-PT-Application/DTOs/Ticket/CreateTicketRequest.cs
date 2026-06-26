using System.ComponentModel.DataAnnotations;

namespace PIGI_PT_Application.DTOs.Ticket
{
    /// <summary>
    /// DTO de entrada para crear un nuevo ticket.
    /// </summary>
    public class CreateTicketRequest
    {
        /// <summary>ID del inquilino propietario del ticket.</summary>
        [Required(ErrorMessage = "El InquilinoId es obligatorio.")]
        public Guid InquilinoId { get; set; }

        /// <summary>Título del incidente (máx. 200 caracteres).</summary>
        [Required(ErrorMessage = "El título es obligatorio.")]
        [MaxLength(200, ErrorMessage = "El título no puede exceder 200 caracteres.")]
        public string Titulo { get; set; } = string.Empty;

        /// <summary>Descripción del incidente (máx. 5000 caracteres).</summary>
        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [MaxLength(5000, ErrorMessage = "La descripción no puede exceder 5000 caracteres.")]
        public string Descripcion { get; set; } = string.Empty;

        /// <summary>ID del usuario que crea el ticket.</summary>
        [Required(ErrorMessage = "El UserId es obligatorio.")]
        public Guid UserId { get; set; }
    }
}
