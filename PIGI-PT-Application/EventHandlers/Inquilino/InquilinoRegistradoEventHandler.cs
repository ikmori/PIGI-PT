using MediatR;
using Microsoft.Extensions.Logging;
using PIGI_PT_Domain.Events.Inquilino;
using System.Threading;
using System.Threading.Tasks;

namespace PIGI_PT_Application.EventHandlers.Inquilino
{
    public class InquilinoRegistradoEventHandler : INotificationHandler<InquilinoRegistradoEvent>
    {
        private readonly ILogger<InquilinoRegistradoEventHandler> _logger;

        public InquilinoRegistradoEventHandler(ILogger<InquilinoRegistradoEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(InquilinoRegistradoEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Domain Event: Inquilino Registrado - ID: {InquilinoId}, Nombre Comercial: '{NombreComercial}', Dominio: '{DominioRed}'",
                notification.InquilinoId, notification.NombreComercial, notification.DominioRed);

            await Task.CompletedTask;
        }
    }
}
