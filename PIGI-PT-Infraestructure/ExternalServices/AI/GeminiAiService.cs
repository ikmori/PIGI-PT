using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PIGI_PT_Application.Ports.Services;

namespace PIGI_PT_Infraestructure.ExternalServices.AI
{
    public class GeminiAiService : ITicketAnalyzerService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public GeminiAiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<AiClassificationResult> AnalyzeTicketAsync(string titulo, string descripcionSanitizada, string categoriasDisponibles)
        {
            var apiKey = _configuration["Gemini:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("API Key for Gemini is missing.");
            }

            var prompt =
                "Eres un sistema experto en soporte tecnico de TI. Tu tarea es clasificar tickets de incidentes.\n\n" +
                "INSTRUCCIONES ESTRICTAS:\n" +
                "- Responde UNICAMENTE con un objeto JSON valido, sin texto adicional, sin bloques de codigo markdown.\n" +
                "- El campo CategoriaSugerida debe ser el GUID exacto de la lista de abajo. Si ninguna aplica perfectamente, elige la mas cercana.\n" +
                "- El campo PrioridadSugerida debe ser exactamente uno de: Baja, Media, Alta, Critica.\n" +
                "- Usa Critica solo si afecta produccion o impide el trabajo de multiples personas.\n" +
                "- Usa Alta si bloquea el trabajo de una persona o area completa.\n" +
                "- Usa Media si afecta la productividad pero hay solucion temporal.\n" +
                "- Usa Baja si es cosmetico o puede esperar.\n\n" +
                "CATEGORIAS DISPONIBLES:\n" +
                categoriasDisponibles + "\n\n" +
                "TICKET A CLASIFICAR:\n" +
                "Titulo: " + titulo + "\n" +
                "Descripcion: " + descripcionSanitizada + "\n\n" +
                "Responde SOLO con JSON, sin ningun texto antes o despues. Ejemplo del formato exacto:\n" +
                "{\"CategoriaSugerida\":\"GUID-aqui\",\"PrioridadSugerida\":\"Alta\",\"Justificacion\":\"Breve explicacion\"}";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[] { new { text = prompt } }
                    }
                }
            };

            var requestJson = JsonSerializer.Serialize(requestBody);
            Console.WriteLine($"[GEMINI REQUEST]: {requestJson}");

            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.5-flash:generateContent?key={apiKey}", content);

            var rawResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[GEMINI RAW RESPONSE]: {rawResponse}");

            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = $"Gemini API Error: {response.ReasonPhrase} - {rawResponse}";
                Console.WriteLine(errorMsg);
                throw new Exception(errorMsg);
            }

            using var doc = JsonDocument.Parse(rawResponse);
            
            try
            {
                var text = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text").GetString();

                // Extraer el bloque JSON de la respuesta con Regex
                var match = Regex.Match(text ?? "", @"\{[\s\S]*\}");
                if (match.Success)
                {
                    text = match.Value;
                }
                else
                {
                    text = text?.Replace("```json", "")?.Replace("```", "")?.Trim() ?? "";
                }
                
                var result = JsonSerializer.Deserialize<AiClassificationResult>(text, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return result ?? new AiClassificationResult { PrioridadSugerida = "Media", CategoriaSugerida = Guid.NewGuid().ToString(), Justificacion = "Fallback: no se pudo parsear la respuesta." };
            }
            catch (Exception ex)
            {
                return new AiClassificationResult
                {
                    CategoriaSugerida = Guid.NewGuid().ToString(),
                    PrioridadSugerida = "Media",
                    Justificacion = "No se pudo parsear el resultado de la IA: " + ex.Message
                };
            }
        }
    }
}
