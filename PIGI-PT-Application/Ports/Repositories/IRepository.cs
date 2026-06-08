using PIGI_PT_Domain.Base;
using PIGI_PT_Domain.Specifications;

namespace PIGI_PT_Application.Ports.Repositories
{
    /// <summary>
    /// Contrato de repositorio genérico que define operaciones CRUD básicas
    /// y búsquedas basadas en el patrón Specification.
    /// </summary>
    /// <typeparam name="T">Tipo de la entidad de dominio.</typeparam>
    public interface IRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// Obtiene una entidad por su identificador único de manera asíncrona.
        /// </summary>
        /// <param name="id">El identificador único de la entidad.</param>
        /// <returns>La entidad encontrada o null si no existe.</returns>
        Task<T?> GetByIdAsync(Guid id);

        /// <summary>
        /// Obtiene una lista de entidades que coinciden con los criterios de la especificación proporcionada.
        /// </summary>
        /// <param name="spec">La especificación con los criterios de consulta, ordenamiento e inclusiones.</param>
        /// <returns>Una lista de entidades que cumplen con la especificación.</returns>
        Task<List<T>> GetBySpecificationAsync(Specification<T> spec);

        /// <summary>
        /// Agrega una nueva entidad al repositorio de manera asíncrona.
        /// </summary>
        /// <param name="entity">La entidad a agregar.</param>
        Task AddAsync(T entity);

        /// <summary>
        /// Actualiza una entidad existente en el repositorio de manera asíncrona.
        /// </summary>
        /// <param name="entity">La entidad con los cambios aplicados.</param>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Elimina una entidad del repositorio de manera asíncrona.
        /// </summary>
        /// <param name="entity">La entidad a eliminar.</param>
        Task DeleteAsync(T entity);
    }
}
