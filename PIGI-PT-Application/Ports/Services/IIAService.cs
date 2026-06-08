namespace PIGI_PT_Application.Ports.Services
{
    /// <summary>
    /// Resultado de la clasificación automática de un incidente/ticket mediante IA.
    /// Contiene la prioridad inferida y el identificador de la categoría asignada.
    /// </summary>
    /// <param name="Prioridad">Nivel de prioridad estimado (Baja, Media, Alta, Crítica).</param>
    /// <param name="CategoriaId">Identificador único de la categoría inferida por el modelo de IA.</param>
    public record ClassificationResult(string Prioridad, Guid CategoriaId);

    /// <summary>
    /// Puerto que define el contrato de integración con el servicio de Inteligencia Artificial (IA).
    /// Permite realizar la categorización/clasificación automática y la sanitización (PII redaction) de textos de tickets.
    /// </summary>
    public interface IIAService
    {
        /// <summary>
        /// Clasifica de manera asíncrona un ticket a partir de su descripción.
        /// Determina la prioridad y la categoría más adecuada utilizando el backend de IA.
        /// </summary>
        /// <param name="description">Texto descriptivo del incidente.</param>
        /// <param name="ct">Token de cancelación opcional.</param>
        /// <returns>Un objeto <see cref="ClassificationResult"/> con la prioridad y categoría sugeridas.</returns>
        Task<ClassificationResult> ClassifyAsync(string description, CancellationToken ct = default);

        /// <summary>
        /// Sanitiza de manera asíncrona un texto (descripción de ticket) removiendo datos sensibles o información de identificación personal (PII).
        /// </summary>
        /// <param name="description">Texto descriptivo original.</param>
        /// <param name="ct">Token de cancelación opcional.</param>
        /// <returns>El texto sanitizado con los datos de PII redactados o removidos.</returns>
        Task<string> SanitizeAsync(string description, CancellationToken ct = default);
    }
}
