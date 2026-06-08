using PIGI_PT_Domain.Aggregates.Inquilino;

namespace PIGI_PT_Application.Ports.Repositories
{
    /// <summary>
    /// Contrato de repositorio específico para gestionar la persistencia y consultas del agregado <see cref="Inquilino"/>.
    /// Extiende de <see cref="IRepository{Inquilino}"/>.
    /// </summary>
    public interface IInquilinoRepository : IRepository<Inquilino>
    {
        /// <summary>
        /// Obtiene un inquilino a partir de su dominio de red registrado.
        /// Útil para la resolución dinámica de multi-tenant durante las peticiones HTTP.
        /// </summary>
        /// <param name="dominio">Dominio de red del inquilino (ej. "empresa.com").</param>
        /// <returns>El inquilino que coincide con el dominio o null si no se encuentra.</returns>
        Task<Inquilino?> GetByDominioAsync(string dominio);
    }
}
