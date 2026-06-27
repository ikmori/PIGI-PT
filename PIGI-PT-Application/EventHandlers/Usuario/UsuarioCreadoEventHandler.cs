using MediatR;
using Microsoft.Extensions.Logging;
using PIGI_PT_Domain.Events.User;
using System.Threading;
using System.Threading.Tasks;

namespace PIGI_PT_Application.EventHandlers.Usuario
{
    public class UsuarioCreadoEventHandler : INotificationHandler<UsuarioCreadoEvent>
    {
        private readonly ILogger<UsuarioCreadoEventHandler> _logger;

        public UsuarioCreadoEventHandler(ILogger<UsuarioCreadoEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(UsuarioCreadoEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Domain Event: Usuario Creado - ID: {UsuarioId}, Nombre Completo: '{NombreCompleto}', Email: '{Email}', Rol: '{Rol}'",
                notification.UsuarioId, notification.NombreCompleto, notification.Email, notification.Rol.Nombre);

            await Task.CompletedTask;
        }
    }
}
