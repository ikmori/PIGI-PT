using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using PIGI_PT_Application.Ports.Infrastructure;
using PIGI_PT_Application.Ports.Repositories;
using PIGI_PT_Application.Ports.Services;
using PIGI_PT_Domain.Events.Ticket;

namespace PIGI_PT_Application.EventHandlers.Ticket
{
    public class TicketCreadoEventHandler : INotificationHandler<TicketCreadoEvent>
    {
        private readonly IHangfireService _hangfireService;
        private readonly ILogger<TicketCreadoEventHandler> _logger;
        private readonly IEmailService _emailService;
        private readonly ITicketRepository _ticketRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public TicketCreadoEventHandler(
            IHangfireService hangfireService, 
            ILogger<TicketCreadoEventHandler> logger,
            IEmailService emailService, 
            ITicketRepository ticketRepository, 
            IUsuarioRepository usuarioRepository)
        {
            _hangfireService = hangfireService;
            _logger = logger;
            _emailService = emailService;
            _ticketRepository = ticketRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task Handle(TicketCreadoEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Domain Event: Ticket Creado - ID: {TicketId}, Titulo: '{Titulo}', Creado por: {CreadorId}. Procesado síncronamente por Gemini AI.",
                notification.TicketId, notification.Titulo, notification.CreadorId);

            // ANTES: Encolaba la sanitización en Hangfire que lanzaba otra IA simulada.
            // AHORA: Todo se hace síncronamente con GeminiAiService en el CreateTicketCommandHandler.
            // _hangfireService.EnqueueSanitization(notification.TicketId, notification.DescripcionOriginal);

            var ticket = await _ticketRepository.GetByIdAsync(notification.TicketId);
            if (ticket == null) return;

            
            var creador = await _usuarioRepository.GetByIdAsync(notification.CreadorId);
            if (creador == null || string.IsNullOrEmpty(creador.Email)) return;

            var subject = $"Nuevo Ticket Creado: {ticket.Titulo}";
            var body = $@"
                <h2>Confirmación de Ticket</h2>
                <p>Hola {creador.FullName},</p>
                <p>Tu ticket ha sido registrado exitosamente en el sistema.</p>
                <ul>
                    <li><strong>ID:</strong> {ticket.Id}</li>
                    <li><strong>Título:</strong> {ticket.Titulo}</li>
                    <li><strong>Estado:</strong> {ticket.Estado}</li>
                </ul>
                <p>Te notificaremos cuando un responsable sea asignado.</p>
            ";

            await _emailService.SendEmailAsync(creador.Email, subject, body, cancellationToken);
        }
    }
}