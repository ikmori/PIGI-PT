using PIGI_PT_Domain.Base;
using PIGI_PT_Domain.Specifications;

namespace PIGI_PT_Application.Ports.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(Guid id);
        Task<List<T>> GetBySpecificationAsync(Specification<T> spec);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}
