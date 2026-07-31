namespace PIGI_PT_Application.Ports.Services
{
    /// <summary>
    /// Puerto que define el contrato básico para el envío de correos electrónicos.
    /// Su implementación real se delega a la capa de infraestructura (ej. SMTP, SendGrid, etc.).
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Envía un correo electrónico de manera asíncrona a un destinatario determinado.
        /// </summary>
        /// <param name="to">Dirección de correo electrónico del destinatario.</param>
        /// <param name="subject">Asunto del correo electrónico.</param>
        /// <param name="body">Cuerpo o contenido del mensaje (soporta texto plano o HTML).</param>
        Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
        
    }
}
