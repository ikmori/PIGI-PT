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
    /// Handler para el evento ResponsableAsignadoEvent.
    /// Notifica al responsable que se le ha asignado un ticket.
    /// </summary>
    public class ResponsableAsignadoEventHandler : INotificationHandler<ResponsableAsignadoEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly ILogger<ResponsableAsignadoEventHandler> _logger;

        public ResponsableAsignadoEventHandler(
            IUnitOfWork unitOfWork,
            INotificationService notificationService, 
            ILogger<ResponsableAsignadoEventHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task Handle(ResponsableAsignadoEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Domain Event: Responsable Asignado - Ticket ID: {TicketId}, Responsable ID: {ResponsableId}",
                notification.TicketId, notification.ResponsableId);

            try
            {
                var responsable = await _unitOfWork.Usuarios.GetByIdAsync(notification.ResponsableId);
                if (responsable != null)
                {
                    // 1. Enviar notificación push
                    await _notificationService.SendPushNotificationAsync(
                        responsable.Id, 
                        $"Se le ha asignado el ticket ID: {notification.TicketId}."
                    );

                    // 2. Enviar correo electrónico
                    if (!string.IsNullOrEmpty(responsable.Email))
                    {
                        await _notificationService.SendEmailAsync(
                            responsable.Email,
                            "[PIGI-PT] Nuevo ticket asignado",
                            $"Hola {responsable.FullName},<br/><br/>Se te ha asignado el ticket con ID: <b>{notification.TicketId}</b>.<br/>Por favor, ingresa a la plataforma para revisarlo."
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar la notificación de asignación de responsable para el ticket: {TicketId}", notification.TicketId);
            }
        }
    }
}
