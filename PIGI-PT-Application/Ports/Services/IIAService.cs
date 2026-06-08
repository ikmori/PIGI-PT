namespace PIGI_PT_Application.Ports.Services
{
    public record ClassificationResult(string Prioridad, Guid CategoriaId);

    public interface IIAService
    {
        Task<ClassificationResult> ClassifyAsync(string description, CancellationToken ct = default);
        Task<string> SanitizeAsync(string description, CancellationToken ct = default);
    }
}
