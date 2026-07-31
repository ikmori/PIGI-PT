using System;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PIGI_PT_Application.Ports.Services;

namespace PIGI_PT_Infraestructure.ExternalServices.Email
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _configuration;
        private readonly bool _useConsoleFallback;
        private readonly string? _smtpHost;
        private readonly int _smtpPort;
        private readonly string? _smtpUser;
        private readonly string? _smtpPass;
        private readonly string? _fromAddress;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            _smtpHost = _configuration["EmailSettings:SmtpHost"];
            _fromAddress = _configuration["EmailSettings:FromAddress"] ?? "noreply@pigi-pt.com";
            
            int.TryParse(_configuration["EmailSettings:SmtpPort"], out _smtpPort);
            _smtpUser = _configuration["EmailSettings:SmtpUser"];
            _smtpPass = _configuration["EmailSettings:SmtpPass"];

            _useConsoleFallback = string.IsNullOrEmpty(_smtpHost);
        }

        public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
        {
            if (_useConsoleFallback)
            {
                _logger.LogInformation("=== SIMULACION DE EMAIL ===");
                _logger.LogInformation("De: {From}", _fromAddress);
                _logger.LogInformation("Para: {To}", to);
                _logger.LogInformation("Asunto: {Subject}", subject);
                _logger.LogInformation("Cuerpo:\n{Body}", body);
                _logger.LogInformation("==========================");
                await Task.CompletedTask;
                return;
            }

            try
            {
                using var mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(_fromAddress!);
                mailMessage.To.Add(to);
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;

                using var smtpClient = new SmtpClient(_smtpHost, _smtpPort);
                if (!string.IsNullOrEmpty(_smtpUser) && !string.IsNullOrEmpty(_smtpPass))
                {
                    smtpClient.Credentials = new System.Net.NetworkCredential(_smtpUser, _smtpPass);
                    smtpClient.EnableSsl = true;
                }

                await smtpClient.SendMailAsync(mailMessage, cancellationToken);
                _logger.LogInformation("Email enviado exitosamente a {To}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar email a {To}. Reintentando con log en consola...", to);
                
                _logger.LogInformation("=== SIMULACION DE EMAIL (FALLBACK POR ERROR) ===");
                _logger.LogInformation("De: {From}", _fromAddress);
                _logger.LogInformation("Para: {To}", to);
                _logger.LogInformation("Asunto: {Subject}", subject);
                _logger.LogInformation("Cuerpo:\n{Body}", body);
                _logger.LogInformation("===============================================");
            }
        }
    }
}