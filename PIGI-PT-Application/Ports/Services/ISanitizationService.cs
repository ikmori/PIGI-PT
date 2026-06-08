namespace PIGI_PT_Application.Ports.Services
{
    /// <summary>
    /// Puerto responsable de coordinar la sanitización de descripciones de tickets.
    /// Consume los servicios de IA para realizar el proceso de limpieza y persistir los cambios correspondientes.
    /// </summary>
    public interface ISanitizationService
    {
        /// <summary>
        /// Sanitiza la descripción de un ticket específico y guarda el resultado.
        /// Remueve información de identificación personal (PII) de forma asíncrona.
        /// </summary>
        /// <param name="ticketId">Identificador único del ticket.</param>
        /// <param name="originalDescription">Texto original sin procesar.</param>
        Task SanitizeTicketAsync(Guid ticketId, string originalDescription);
    }
}
