using MediatR;
using Microsoft.Extensions.Logging;
using PIGI_PT_Application.Ports.Infrastructure;
using PIGI_PT_Application.Ports.Services;
using PIGI_PT_Domain.Events.Ticket;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PIGI_PT_Application.EventHandlers.Ticket
{
    /// <summary>
    /// Handler para el evento TicketResueltoEvent.
    /// Notifica al creador del ticket que su incidente ha sido resuelto.
    /// </summary>
    public class TicketResueltoEventHandler : INotificationHandler<TicketResueltoEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly ILogger<TicketResueltoEventHandler> _logger;

        public TicketResueltoEventHandler(
            IUnitOfWork unitOfWork,
            INotificationService notificationService, 
            ILogger<TicketResueltoEventHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task Handle(TicketResueltoEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Domain Event: Ticket Resuelto - ID: {TicketId}, Creado por: {CreadorId}, Tiempo de Resolución: {Tiempo}",
                notification.TicketId, notification.CreadorId, notification.TiempoDeResolucion);

            try
            {
                var creador = await _unitOfWork.Usuarios.GetByIdAsync(notification.CreadorId);
                if (creador != null)
                {
                    // 1. Enviar notificación push
                    await _notificationService.SendPushNotificationAsync(
                        creador.Id, 
                        $"Su ticket ID: {notification.TicketId} ha sido resuelto."
                    );

                    // 2. Enviar correo electrónico
                    if (!string.IsNullOrEmpty(creador.Email))
                    {
                        await _notificationService.SendEmailAsync(
                            creador.Email,
                            "[PIGI-PT] Su ticket ha sido resuelto",
                            $"Hola {creador.FullName},<br/><br/>Te informamos que tu ticket con ID: <b>{notification.TicketId}</b> ha sido resuelto.<br/>Tiempo total de resolución: {notification.TiempoDeResolucion.TotalHours:F1} horas.<br/>Gracias por utilizar la plataforma."
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar la notificación de resolución de ticket para el ticket: {TicketId}", notification.TicketId);
            }
        }
    }
}
