using MediatR;
using Microsoft.Extensions.Logging;
using PIGI_PT_Application.Ports.Infrastructure;
using PIGI_PT_Application.Ports.Repositories;
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
        private readonly ITicketRepository _ticketRepository;
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;
        private readonly ILogger<ResponsableAsignadoEventHandler> _logger;

        public ResponsableAsignadoEventHandler(
            IUnitOfWork unitOfWork,
            ITicketRepository ticketRepository,
            INotificationService notificationService, 
            IEmailService emailService,
            ILogger<ResponsableAsignadoEventHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _ticketRepository = ticketRepository;
            _notificationService = notificationService;
            _emailService = emailService;
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

                    // 2. Enviar correo electrónico con formato enriquecido
                    if (!string.IsNullOrEmpty(responsable.Email))
                    {
                        
                        var ticket = await _ticketRepository.GetByIdAsync(notification.TicketId);
                        if (ticket != null)
                        {
                            var subject = $"Nuevo Ticket Asignado: {ticket.Titulo}";
                            var body = $@"
                                <h2>Ticket Asignado</h2>
                                <p>Hola {responsable.FullName},</p>
                                <p>Se te ha asignado un nuevo ticket para revisión y resolución.</p>
                                <ul>
                                    <li><strong>ID:</strong> {ticket.Id}</li>
                                    <li><strong>Título:</strong> {ticket.Titulo}</li>
                                    <li><strong>Prioridad:</strong> {ticket.Prioridad}</li>
                                </ul>
                                <p>Por favor, accede a la plataforma para gestionar este requerimiento.</p>
                            ";

                            await _emailService.SendEmailAsync(responsable.Email, subject, body, cancellationToken);
                        }
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