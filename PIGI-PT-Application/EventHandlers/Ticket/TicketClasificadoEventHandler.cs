using MediatR;
using Microsoft.Extensions.Logging;
using PIGI_PT_Application.Ports.Services;
using PIGI_PT_Domain.Events.Ticket;
using System.Threading;
using System.Threading.Tasks;

namespace PIGI_PT_Application.EventHandlers.Ticket
{
    /// <summary>
    /// Handler para el evento TicketClasificadoEvent.
    /// Notifica que el ticket ha sido clasificado por la IA.
    /// </summary>
    public class TicketClasificadoEventHandler : INotificationHandler<TicketClasificadoEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<TicketClasificadoEventHandler> _logger;

        public TicketClasificadoEventHandler(
            INotificationService notificationService, 
            ILogger<TicketClasificadoEventHandler> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task Handle(TicketClasificadoEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Domain Event: Ticket Clasificado - ID: {TicketId}, Prioridad: {Prioridad}, Categoría ID: {CategoriaId}",
                notification.TicketId, notification.Prioridad.Nombre, notification.CategoriaId);

            // Simular envío de notificación push a operadores del sistema
            await _notificationService.SendPushNotificationAsync(
                System.Guid.Empty, 
                $"Nuevo ticket clasificado: {notification.TicketId} con prioridad {notification.Prioridad.Nombre}."
            );
        }
    }
}
