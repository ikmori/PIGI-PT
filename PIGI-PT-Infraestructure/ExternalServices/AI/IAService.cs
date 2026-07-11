using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PIGI_PT_Application.Ports.Services;
using PIGI_PT_Infraestructure.Persistence.DbContext;

namespace PIGI_PT_Infraestructure.ExternalServices.AI
{
    /// <summary>
    /// Adaptador que implementa IIAService para comunicarse con la API de IA en Python FastAPI
    /// y proporciona un fallback local robusto para desarrollo sin el servicio de IA activo.
    /// </summary>
    public class IAService : IIAService
    {
        private readonly HttpClient _httpClient;
        private readonly PigiPtDbContext _dbContext;
        private readonly ILogger<IAService> _logger;
        private readonly string? _fastApiUrl;

        public IAService(
            HttpClient httpClient, 
            PigiPtDbContext dbContext, 
            IConfiguration configuration,
            ILogger<IAService> logger)
        {
            _httpClient = httpClient;
            _dbContext = dbContext;
            _logger = logger;
            _fastApiUrl = configuration["AISettings:FastApiUrl"];
        }

        public async Task<ClassificationResult> ClassifyAsync(string description, CancellationToken ct = default)
        {
            if (!string.IsNullOrEmpty(_fastApiUrl))
            {
                try
                {
                    var response = await _httpClient.PostAsJsonAsync($"{_fastApiUrl}/classify", new { description }, ct);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<ClassificationResultDto>(cancellationToken: ct);
                        if (result != null && result.CategoriaId != Guid.Empty)
                        {
                            return new ClassificationResult(result.Prioridad, result.CategoriaId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error al conectar con el servicio FastAPI de IA para clasificar. Usando fallback local.");
                }
            }

            // Fallback de simulación local en C#
            _logger.LogInformation("Ejecutando clasificación local simulada.");
            
            // Buscar la primera categoría registrada de la base de datos o asignar una por defecto
            var categoria = await _dbContext.Categorias.FirstOrDefaultAsync(ct);
            var categoriaId = categoria?.Id ?? Guid.NewGuid();

            // Lógica heurística de prioridad basada en palabras clave
            string prioridad = "Baja";
            var text = description.ToLower();
            if (text.Contains("crítico") || text.Contains("critico") || text.Contains("urgente") || 
                text.Contains("caído") || text.Contains("caido") || text.Contains("fatal") || 
                text.Contains("seguridad") || text.Contains("caída") || text.Contains("caida"))
            {
                prioridad = "Crítica";
            }
            else if (text.Contains("error") || text.Contains("fallo") || text.Contains("red") || 
                     text.Contains("no funciona") || text.Contains("bloqueado"))
            {
                prioridad = "Alta";
            }
            else if (text.Contains("ayuda") || text.Contains("solicitud") || text.Contains("soporte") || 
                     text.Contains("duda"))
            {
                prioridad = "Media";
            }

            return new ClassificationResult(prioridad, categoriaId);
        }

        public async Task<string> SanitizeAsync(string description, CancellationToken ct = default)
        {
            if (!string.IsNullOrEmpty(_fastApiUrl))
            {
                try
                {
                    var response = await _httpClient.PostAsJsonAsync($"{_fastApiUrl}/sanitize", new { description }, ct);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<SanitizationResponseDto>(cancellationToken: ct);
                        if (result != null && !string.IsNullOrEmpty(result.SanitizedDescription))
                        {
                            return result.SanitizedDescription;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error al conectar con el servicio FastAPI de IA para sanitización. Usando fallback local.");
                }
            }

            // Fallback de sanitización local básica (enmascaramiento PII local)
            _logger.LogInformation("Ejecutando sanitización local simulada.");
            
            var cleaned = description;
            
            // Enmascarar direcciones de correo electrónico
            cleaned = System.Text.RegularExpressions.Regex.Replace(
                cleaned, 
                @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}", 
                "[EMAIL_ENMASCARADO]"
            );
            
            // Enmascarar direcciones IP
            cleaned = System.Text.RegularExpressions.Regex.Replace(
                cleaned, 
                @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b", 
                "[IP_ENMASCARADA]"
            );
            
            // Enmascarar contraseñas explícitas
            cleaned = System.Text.RegularExpressions.Regex.Replace(
                cleaned, 
                @"(?i)(password|contraseña|clave|pass):\s*[^\s]+", 
                "$1: [CONTRASENA_ENMASCARADA]"
            );

            return cleaned;
        }

        private class ClassificationResultDto
        {
            public string Prioridad { get; set; } = string.Empty;
            public Guid CategoriaId { get; set; }
        }

        private class SanitizationResponseDto
        {
            public string SanitizedDescription { get; set; } = string.Empty;
        }
    }
}
