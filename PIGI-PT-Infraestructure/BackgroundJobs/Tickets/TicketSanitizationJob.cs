using System;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Logging;
using PIGI_PT_Application.Ports.Infrastructure;
using PIGI_PT_Application.Ports.Services;

namespace PIGI_PT_Infraestructure.BackgroundJobs.Tickets
{
    /// <summary>
    /// Job de Hangfire encargado de sanitizar la descripción de un ticket removiendo PII de forma asíncrona.
    /// Al terminar la sanitización, encola el job de clasificación automática.
    /// </summary>
    public class TicketSanitizationJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIAService _iaService;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ILogger<TicketSanitizationJob> _logger;

        public TicketSanitizationJob(
            IUnitOfWork unitOfWork,
            IIAService iaService,
            IBackgroundJobClient backgroundJobClient,
            ILogger<TicketSanitizationJob> logger)
        {
            _unitOfWork = unitOfWork;
            _iaService = iaService;
            _backgroundJobClient = backgroundJobClient;
            _logger = logger;
        }

        /// <summary>
        /// Método de ejecución del Job.
        /// </summary>
        /// <param name="ticketId">ID del ticket a sanitizar.</param>
        /// <param name="originalDescription">Texto original de la descripción.</param>
        [Queue("default")]
        public async Task ExecuteAsync(Guid ticketId, string originalDescription)
        {
            _logger.LogInformation("Iniciando sanitización del Ticket: {TicketId}", ticketId);

            var ticket = await _unitOfWork.Tickets.GetByIdAsync(ticketId);
            if (ticket == null)
            {
                _logger.LogWarning("Ticket: {TicketId} no encontrado para sanitización. Abortando.", ticketId);
                return;
            }

            try
            {
                // 1. Llamar al servicio de IA (o mock) para enmascarar PII
                var sanitizedText = await _iaService.SanitizeAsync(originalDescription);

                // 2. Aplicar la sanitización al agregador de dominio
                ticket.AplicarSanitizacion(sanitizedText);

                // 3. Persistir cambios
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Sanitización del Ticket: {TicketId} completada. Encolando clasificación.", ticketId);

                // 4. Encolar la clasificación automática
                _backgroundJobClient.Enqueue<TicketClassificationJob>(job => job.ExecuteAsync(ticketId, sanitizedText));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la ejecución del job de sanitización para el Ticket: {TicketId}", ticketId);
                throw; // Re-lanzar para permitir que Hangfire reintente el Job si falla
            }
        }
    }
}
