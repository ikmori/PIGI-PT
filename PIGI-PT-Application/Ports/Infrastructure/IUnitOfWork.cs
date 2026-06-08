using PIGI_PT_Application.Ports.Repositories;

namespace PIGI_PT_Application.Ports.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        ITicketRepository Tickets { get; }
        IInquilinoRepository Inquilinos { get; }
        IUsuarioRepository Usuarios { get; }
        IRiesgoOperacionalRepository RiesgosOperacionales { get; }
        ICategoriaRepository Categorias { get; }
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<bool> BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task<bool> CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task<bool> RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
