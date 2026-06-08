using PIGI_PT_Domain.Aggregates.Categoria;

namespace PIGI_PT_Application.Ports.Repositories
{
    public interface ICategoriaRepository : IRepository<Categoria>
    {
        Task<Categoria?> GetByNameAsync(Guid inquilinoId, string name);
    }
}
