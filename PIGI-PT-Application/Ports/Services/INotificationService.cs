namespace PIGI_PT_Application.Ports.Services
{
    /// <summary>
    /// Puerto unificado para notificaciones en el sistema PIGI-PT.
    /// Define contratos para el envío de alertas multicanal (correo electrónico y push notifications).
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Envía una notificación por correo electrónico de manera asíncrona.
        /// </summary>
        /// <param name="to">Dirección de correo electrónico del destinatario.</param>
        /// <param name="subject">Asunto del correo.</param>
        /// <param name="body">Cuerpo o contenido del correo.</param>
        Task SendEmailAsync(string to, string subject, string body);

        /// <summary>
        /// Envía una notificación push en tiempo real de manera asíncrona a un usuario de la plataforma.
        /// </summary>
        /// <param name="userId">Identificador único del usuario destino.</param>
        /// <param name="message">Contenido del mensaje de la notificación push.</param>
        Task SendPushNotificationAsync(Guid userId, string message);
    }
}
