namespace PIGI_PT_Application.Ports.Infrastructure
{
    /// <summary>
    /// Puerto que expone la capacidad de programar y encolar tareas en segundo plano (Background Jobs)
    /// utilizando Hangfire como motor de procesamiento.
    /// </summary>
    public interface IHangfireService
    {
        /// <summary>
        /// Encola una tarea en segundo plano para sanitizar la descripción de un ticket específico.
        /// </summary>
        /// <param name="ticketId">Identificador único del ticket.</param>
        /// <param name="description">Texto descriptivo original a sanitizar.</param>
        void EnqueueSanitization(Guid ticketId, string description);

        /// <summary>
        /// Encola una tarea en segundo plano para clasificar automáticamente un ticket (prioridad y categoría).
        /// </summary>
        /// <param name="ticketId">Identificador único del ticket.</param>
        /// <param name="description">Texto descriptivo a clasificar.</param>
        void EnqueueClassification(Guid ticketId, string description);
    }
}
