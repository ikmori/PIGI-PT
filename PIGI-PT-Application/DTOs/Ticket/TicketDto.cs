namespace PIGI_PT_Application.DTOs.Ticket
{
    /// <summary>
    /// DTO de respuesta que representa los datos de un ticket expuestos por la API.
    /// Nunca expone la entidad de dominio directamente.
    /// </summary>
    public class TicketDto
    {
        public Guid Id { get; set; }
        public Guid InquilinoId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string DescripcionOriginal { get; set; } = string.Empty;
        public string? DescripcionSanitizada { get; set; }
        public string Estado { get; set; } = string.Empty;
        public int EstadoValor { get; set; }
        public string Prioridad { get; set; } = string.Empty;
        public int PrioridadValor { get; set; }
        public Guid? CategoriaId { get; set; }
        public string? NombreCategoria { get; set; }
        public Guid? ResponsableTecnologiaId { get; set; }
        public string? ResponsableTecnologiaNombre { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }

        /// <summary>
        /// Nombre del creador del ticket (para contacto directo del operador).
        /// </summary>
        public string? CreadorNombre { get; set; }

        /// <summary>
        /// Email del creador del ticket (para contacto directo del operador).
        /// </summary>
        public string? CreadorEmail { get; set; }

        public DateTime? ModifiedAt { get; set; }
        public DateTime? FechaResolucion { get; set; }
        public DateTime? FechaAsignacion { get; set; }
        public bool EstaAtrasado { get; set; }
        public bool RequiereEscalada { get; set; }
        public bool IsActive { get; set; }
    }
}

