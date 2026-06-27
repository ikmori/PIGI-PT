using MediatR;
using Microsoft.Extensions.Logging;
using PIGI_PT_Domain.Events.Ticket;
using System.Threading;
using System.Threading.Tasks;

namespace PIGI_PT_Application.EventHandlers.Ticket
{
    public class TicketCreadoEventHandler : INotificationHandler<TicketCreadoEvent>
    {
        private readonly ILogger<TicketCreadoEventHandler> _logger;

        public TicketCreadoEventHandler(ILogger<TicketCreadoEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(TicketCreadoEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Domain Event: Ticket Creado - ID: {TicketId}, Titulo: '{Titulo}', Creado por: {CreadorId}",
                notification.TicketId, notification.Titulo, notification.CreadorId);

            // Simulación o preparación para servicios asíncronos futuros (ej. Hangfire/Sanitización)
            await Task.CompletedTask;
        }
    }
}
