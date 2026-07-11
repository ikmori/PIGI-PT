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
    /// Handler para el evento OperadorAsignadoEvent.
    /// Notifica al operador que se le ha asignado un ticket.
    /// </summary>
    public class OperadorAsignadoEventHandler : INotificationHandler<OperadorAsignadoEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly ILogger<OperadorAsignadoEventHandler> _logger;

        public OperadorAsignadoEventHandler(
            IUnitOfWork unitOfWork,
            INotificationService notificationService, 
            ILogger<OperadorAsignadoEventHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task Handle(OperadorAsignadoEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Domain Event: Operador Asignado - Ticket ID: {TicketId}, Operador ID: {OperadorId}",
                notification.TicketId, notification.OperadorId);

            try
            {
                var operador = await _unitOfWork.Usuarios.GetByIdAsync(notification.OperadorId);
                if (operador != null)
                {
                    // 1. Enviar notificación push
                    await _notificationService.SendPushNotificationAsync(
                        operador.Id, 
                        $"Se le ha asignado el ticket ID: {notification.TicketId}."
                    );

                    // 2. Enviar correo electrónico
                    if (!string.IsNullOrEmpty(operador.Email))
                    {
                        await _notificationService.SendEmailAsync(
                            operador.Email,
                            "[PIGI-PT] Nuevo ticket asignado",
                            $"Hola {operador.FullName},<br/><br/>Se te ha asignado el ticket con ID: <b>{notification.TicketId}</b>.<br/>Por favor, ingresa a la plataforma para revisarlo."
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar la notificación de asignación de operador para el ticket: {TicketId}", notification.TicketId);
            }
        }
    }
}
