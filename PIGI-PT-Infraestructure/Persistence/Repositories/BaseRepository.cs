using Microsoft.EntityFrameworkCore;
using PIGI_PT_Application.Ports.Repositories;
using PIGI_PT_Domain.Base;
using PIGI_PT_Domain.Specifications;
using PIGI_PT_Infraestructure.Persistence.DbContext;

namespace PIGI_PT_Infraestructure.Persistence.Repositories
{
    /// <summary>
    /// Implementación genérica base del repositorio que provee operaciones CRUD
    /// y consultas basadas en el patrón Specification para todas las entidades del dominio.
    /// Utiliza Entity Framework Core como ORM subyacente.
    /// </summary>
    /// <typeparam name="T">Tipo de entidad que hereda de <see cref="BaseEntity"/>.</typeparam>
    public abstract class BaseRepository<T> : IRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// Contexto de base de datos inyectado.
        /// </summary>
        protected readonly PigiPtDbContext _context;

        /// <summary>
        /// Constructor que recibe el contexto de base de datos.
        /// </summary>
        /// <param name="context">Instancia del DbContext de PIGI-PT.</param>
        protected BaseRepository(PigiPtDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <inheritdoc/>
        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        /// <inheritdoc/>
        public async Task<List<T>> GetBySpecificationAsync(Specification<T> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }

        /// <inheritdoc/>
        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        /// <inheritdoc/>
        public Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Aplica una especificación al DbSet para construir la consulta LINQ correspondiente.
        /// Evalúa: filtros (Criteria), includes, ordenamiento y paginación.
        /// </summary>
        /// <param name="spec">Especificación a aplicar.</param>
        /// <returns>Consulta IQueryable con la especificación aplicada.</returns>
        private IQueryable<T> ApplySpecification(Specification<T> spec)
        {
            var query = _context.Set<T>().AsQueryable();

            // Aplicar filtro WHERE
            if (spec.Criteria != null)
                query = query.Where(spec.Criteria);

            // Aplicar INCLUDES (eager loading)
            query = spec.Includes.Aggregate(query,
                (current, include) => current.Include(include));

            // Aplicar ORDER BY
            if (spec.OrderBy != null)
                query = query.OrderBy(spec.OrderBy);
            else if (spec.OrderByDescending != null)
                query = query.OrderByDescending(spec.OrderByDescending);

            // Aplicar paginación
            if (spec.IsPagingEnabled)
                query = query.Skip(spec.PageIndex * spec.PageSize)
                             .Take(spec.PageSize);

            return query;
        }
    }
}
