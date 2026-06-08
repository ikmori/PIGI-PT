using PIGI_PT_Domain.Aggregates.Categoria;

namespace PIGI_PT_Application.Ports.Repositories
{
    /// <summary>
    /// Contrato de repositorio específico para gestionar la persistencia y consultas del agregado <see cref="Categoria"/>.
    /// Extiende de <see cref="IRepository{Categoria}"/>.
    /// </summary>
    public interface ICategoriaRepository : IRepository<Categoria>
    {
        /// <summary>
        /// Obtiene una categoría específica de un inquilino a partir de su nombre.
        /// </summary>
        /// <param name="inquilinoId">Identificador único del inquilino (Tenant).</param>
        /// <param name="name">Nombre de la categoría a buscar.</param>
        /// <returns>La categoría que coincide con el criterio o null si no se encuentra.</returns>
        Task<Categoria?> GetByNameAsync(Guid inquilinoId, string name);
    }
}
