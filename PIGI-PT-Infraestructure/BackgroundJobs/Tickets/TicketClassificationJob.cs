using System;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Logging;
using PIGI_PT_Application.Ports.Infrastructure;
using PIGI_PT_Application.Ports.Services;
using PIGI_PT_Domain.ValueObjects;

namespace PIGI_PT_Infraestructure.BackgroundJobs.Tickets
{
    /// <summary>
    /// Job de Hangfire encargado de clasificar automáticamente un ticket (prioridad y categoría) usando IA de forma asíncrona.
    /// </summary>
    public class TicketClassificationJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIAService _iaService;
        private readonly ILogger<TicketClassificationJob> _logger;

        public TicketClassificationJob(
            IUnitOfWork unitOfWork,
            IIAService iaService,
            ILogger<TicketClassificationJob> logger)
        {
            _unitOfWork = unitOfWork;
            _iaService = iaService;
            _logger = logger;
        }

        /// <summary>
        /// Método de ejecución del Job.
        /// </summary>
        /// <param name="ticketId">ID del ticket a clasificar.</param>
        /// <param name="sanitizedDescription">Texto sanitizado a analizar.</param>
        [Queue("default")]
        public async Task ExecuteAsync(Guid ticketId, string sanitizedDescription)
        {
            _logger.LogInformation("Iniciando clasificación automática del Ticket: {TicketId}", ticketId);

            var ticket = await _unitOfWork.Tickets.GetByIdAsync(ticketId);
            if (ticket == null)
            {
                _logger.LogWarning("Ticket: {TicketId} no encontrado para clasificación. Abortando.", ticketId);
                return;
            }

            try
            {
                // 1. Llamar al servicio de IA para obtener la prioridad y categoría estimadas
                var result = await _iaService.ClassifyAsync(sanitizedDescription);

                // 2. Resolver el Value Object de NivelPrioridad a partir de la respuesta del servicio
                var prioridad = NivelPrioridad.DesdeString(result.Prioridad);

                // 3. Aplicar la clasificación en el agregador del dominio
                ticket.ClasificarPorIA(prioridad, result.CategoriaId);

                // 4. Persistir cambios (disparará también los eventos de dominio de Clasificado)
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Clasificación del Ticket: {TicketId} completada con éxito. Prioridad: {Prioridad}, Categoría ID: {CategoriaId}", 
                    ticketId, prioridad.Nombre, result.CategoriaId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la ejecución del job de clasificación para el Ticket: {TicketId}", ticketId);
                throw; // Re-lanzar para permitir reintentos automáticos
            }
        }
    }
}
