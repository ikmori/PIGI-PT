using MediatR;
using Microsoft.Extensions.Logging;
using PIGI_PT_Application.Ports.Infrastructure;
using PIGI_PT_Domain.Events.Ticket;
using System.Threading;
using System.Threading.Tasks;

namespace PIGI_PT_Application.EventHandlers.Ticket
{
    public class TicketCreadoEventHandler : INotificationHandler<TicketCreadoEvent>
    {
        private readonly IHangfireService _hangfireService;
        private readonly ILogger<TicketCreadoEventHandler> _logger;

        public TicketCreadoEventHandler(IHangfireService hangfireService, ILogger<TicketCreadoEventHandler> logger)
        {
            _hangfireService = hangfireService;
            _logger = logger;
        }

        public async Task Handle(TicketCreadoEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Domain Event: Ticket Creado - ID: {TicketId}, Titulo: '{Titulo}', Creado por: {CreadorId}. Procesado síncronamente por Gemini AI.",
                notification.TicketId, notification.Titulo, notification.CreadorId);

            // ANTES: Encolaba la sanitización en Hangfire que lanzaba otra IA simulada.
            // AHORA: Todo se hace síncronamente con GeminiAiService en el CreateTicketCommandHandler.
            // _hangfireService.EnqueueSanitization(notification.TicketId, notification.DescripcionOriginal);

            await Task.CompletedTask;
        }
    }
}

