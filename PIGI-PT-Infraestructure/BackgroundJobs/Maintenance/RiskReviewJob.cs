using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PIGI_PT_Application.Ports.Services;
using PIGI_PT_Infraestructure.Persistence.DbContext;

namespace PIGI_PT_Infraestructure.BackgroundJobs.Maintenance
{
    /// <summary>
    /// Job programado recurrente que revisa todos los riesgos operacionales de la plataforma.
    /// Si un riesgo lleva más de 90 días sin revisión, genera una alerta y envía un correo de notificación.
    /// </summary>
    public class RiskReviewJob
    {
        private readonly PigiPtDbContext _dbContext;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RiskReviewJob> _logger;

        public RiskReviewJob(
            PigiPtDbContext dbContext,
            IEmailService emailService,
            IConfiguration configuration,
            ILogger<RiskReviewJob> logger)
        {
            _dbContext = dbContext;
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Método de ejecución programado.
        /// </summary>
        public async Task ExecuteAsync()
        {
            _logger.LogInformation("Iniciando job recurrente de revisión de riesgos operacionales.");

            var limitDate = DateTime.UtcNow.AddDays(-90);
            
            // Obtener todos los riesgos que requieren revisión (sin importar inquilino para mantenimiento global)
            var pendingRisks = await _dbContext.RiesgosOperacionales
                .Include(r => r.InquilinoId) // Cargar inquilino para saber a quién pertenece
                .Where(r => r.FechaUltimaRevision < limitDate)
                .ToListAsync();

            if (!pendingRisks.Any())
            {
                _logger.LogInformation("No se encontraron riesgos operacionales que requieran revisión (>90 días).");
                return;
            }

            _logger.LogWarning("Se encontraron {Count} riesgos operacionales que requieren revisión urgente.", pendingRisks.Count);

            foreach (var risk in pendingRisks)
            {
                _logger.LogWarning("RIESGO URGENTE - ID: {Id}, Servicio Afectado: '{Servicio}', Última Revisión: {Fecha}", 
                    risk.Id, risk.ServicioAfectado, risk.FechaUltimaRevision);
            }

            // Enviar correo resumen al administrador general si está configurado
            var adminEmail = _configuration["EmailSettings:AdminEmail"];
            if (!string.IsNullOrEmpty(adminEmail))
            {
                var bodyBuilder = new System.Text.StringBuilder();
                bodyBuilder.AppendLine("<h2>Alerta de Revisión de Riesgos Operacionales</h2>");
                bodyBuilder.AppendLine("<p>Los siguientes riesgos operacionales llevan más de 90 días sin revisión y requieren atención urgente:</p>");
                bodyBuilder.AppendLine("<table border='1' cellpadding='5' style='border-collapse: collapse;'>");
                bodyBuilder.AppendLine("<thead><tr><th>Servicio Afectado</th><th>Amenaza</th><th>Impacto</th><th>Última Revisión</th></tr></thead>");
                bodyBuilder.AppendLine("<tbody>");

                foreach (var risk in pendingRisks)
                {
                    bodyBuilder.AppendLine("<tr>");
                    bodyBuilder.AppendLine($"<td>{risk.ServicioAfectado}</td>");
                    bodyBuilder.AppendLine($"<td>{risk.DescripcionAmenaza}</td>");
                    bodyBuilder.AppendLine($"<td>{risk.NivelDeImpacto.Nombre}</td>");
                    bodyBuilder.AppendLine($"<td>{risk.FechaUltimaRevision:dd/MM/yyyy}</td>");
                    bodyBuilder.AppendLine("</tr>");
                }

                bodyBuilder.AppendLine("</tbody></table>");
                bodyBuilder.AppendLine("<br/><p>Por favor, acceda a la plataforma para reevaluar los riesgos.</p>");

                try
                {
                    await _emailService.SendEmailAsync(
                        adminEmail, 
                        $"[PIGI-PT] ALERTA: {pendingRisks.Count} riesgos requieren revisión", 
                        bodyBuilder.ToString()
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al enviar correo resumen de riesgos a {AdminEmail}", adminEmail);
                }
            }
        }
    }
}
