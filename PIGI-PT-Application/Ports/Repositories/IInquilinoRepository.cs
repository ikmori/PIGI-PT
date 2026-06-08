using PIGI_PT_Domain.Aggregates.Inquilino;

namespace PIGI_PT_Application.Ports.Repositories
{
    public interface IInquilinoRepository : IRepository<Inquilino>
    {
        Task<Inquilino?> GetByDominioAsync(string dominio);
    }
}
