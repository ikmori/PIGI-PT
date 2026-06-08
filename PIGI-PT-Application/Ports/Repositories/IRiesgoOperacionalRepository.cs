using PIGI_PT_Domain.Aggregates.RiesgoOperacional;

namespace PIGI_PT_Application.Ports.Repositories
{
    /// <summary>
    /// Contrato de repositorio específico para gestionar la persistencia y consultas del agregado <see cref="RiesgoOperacional"/>.
    /// Extiende de <see cref="IRepository{RiesgoOperacional}"/>.
    /// </summary>
    public interface IRiesgoOperacionalRepository : IRepository<RiesgoOperacional>
    {
    }
}
