using Microsoft.EntityFrameworkCore.Storage;
using PIGI_PT_Application.Ports.Infrastructure;
using PIGI_PT_Application.Ports.Repositories;
using PIGI_PT_Infraestructure.Persistence.DbContext;

namespace PIGI_PT_Infraestructure.Persistence.Repositories
{
    /// <summary>
    /// Implementación del patrón Unit of Work (Unidad de Trabajo).
    /// Coordina la persistencia de cambios a través de múltiples repositorios
    /// asegurando la consistencia transaccional y la atomicidad de las operaciones.
    /// 
    /// Responsabilidades:
    /// - Centralizar el acceso a todos los repositorios
    /// - Gestionar la transacción de base de datos
    /// - Garantizar que todos los cambios se persisten o revierten juntos
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PigiPtDbContext _context;
        private IDbContextTransaction? _currentTransaction;
        private bool _disposed;

        /// <inheritdoc/>
        public ITicketRepository Tickets { get; }

        /// <inheritdoc/>
        public IInquilinoRepository Inquilinos { get; }

        /// <inheritdoc/>
        public IUsuarioRepository Usuarios { get; }

        /// <inheritdoc/>
        public ICategoriaRepository Categorias { get; }

        /// <summary>
        /// Constructor que recibe el DbContext y todos los repositorios vía inyección de dependencias.
        /// </summary>
        /// <param name="context">Contexto de base de datos.</param>
        /// <param name="tickets">Repositorio de tickets.</param>
        /// <param name="inquilinos">Repositorio de inquilinos.</param>
        /// <param name="usuarios">Repositorio de usuarios.</param>
        /// <param name="riesgosOperacionales">Repositorio de riesgos operacionales.</param>
        /// <param name="categorias">Repositorio de categorías.</param>
        public UnitOfWork(
            PigiPtDbContext context,
            ITicketRepository tickets,
            IInquilinoRepository inquilinos,
            IUsuarioRepository usuarios,
            ICategoriaRepository categorias)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            Tickets = tickets ?? throw new ArgumentNullException(nameof(tickets));
            Inquilinos = inquilinos ?? throw new ArgumentNullException(nameof(inquilinos));
            Usuarios = usuarios ?? throw new ArgumentNullException(nameof(usuarios));
            Categorias = categorias ?? throw new ArgumentNullException(nameof(categorias));
        }

        /// <inheritdoc/>
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<bool> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null)
                return false; // Ya hay una transacción activa

            _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction == null)
                return false; // No hay transacción activa

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                await _currentTransaction.CommitAsync(cancellationToken);
                return true;
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                await DisposeTransactionAsync();
            }
        }

        /// <inheritdoc/>
        public async Task<bool> RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction == null)
                return false; // No hay transacción activa

            try
            {
                await _currentTransaction.RollbackAsync(cancellationToken);
                return true;
            }
            finally
            {
                await DisposeTransactionAsync();
            }
        }

        /// <summary>
        /// Libera la transacción actual de forma segura.
        /// </summary>
        private async Task DisposeTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        /// <summary>
        /// Libera los recursos del UnitOfWork y su transacción asociada.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Patrón de dispose protegido.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _currentTransaction?.Dispose();
                    _context.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
