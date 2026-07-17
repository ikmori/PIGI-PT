using System.Threading.Tasks;

namespace PIGI_PT_Application.Ports.Services
{
    public class AiClassificationResult
    {
        public string CategoriaSugerida { get; set; } = string.Empty;
        public string PrioridadSugerida { get; set; } = string.Empty;
        public string Justificacion { get; set; } = string.Empty;
    }

    public interface ITicketAnalyzerService
    {
        Task<AiClassificationResult> AnalyzeTicketAsync(string titulo, string descripcionSanitizada, string categoriasDisponibles);
    }
}
