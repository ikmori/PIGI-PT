using PIGI_PT_Domain.Aggregates.Usuario;

namespace PIGI_PT_Application.Ports.Repositories
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Task<Usuario?> GetByEmailAsync(string email);
        Task<Usuario?> GetByUserNameAsync(string userName);
    }
}
