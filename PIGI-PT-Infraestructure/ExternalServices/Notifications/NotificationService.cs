using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PIGI_PT_Application.Ports.Services;

namespace PIGI_PT_Infraestructure.ExternalServices.Notifications
{
    /// <summary>
    /// Adaptador que implementa INotificationService para el envío multicanal de alertas.
    /// Utiliza IEmailService para correos y simula notificaciones push a través de logs de consola/SignalR.
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IEmailService emailService, ILogger<NotificationService> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            await _emailService.SendEmailAsync(to, subject, body);
        }

        public async Task SendPushNotificationAsync(Guid userId, string message)
        {
            _logger.LogInformation("=== NOTIFICACION PUSH ENVIADA ===");
            _logger.LogInformation("Para Usuario ID: {UserId}", userId);
            _logger.LogInformation("Mensaje: {Message}", message);
            _logger.LogInformation("=================================");
            
            // Simular entrega push instantánea en desarrollo
            await Task.CompletedTask;
        }
    }
}
