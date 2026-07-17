using PIGI_PT_Application.Ports.Repositories;

namespace PIGI_PT_Application.Ports.Infrastructure
{
    /// <summary>
    /// Puerto que define el contrato de Unit of Work (Unidad de Trabajo).
    /// Coordina y agrupa la persistencia de cambios a través de múltiples repositorios
    /// asegurando la consistencia transaccional y la atomicidad de las operaciones.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Repositorio específico para la persistencia del agregado <see cref="PIGI_PT_Domain.Aggregates.Ticket.Ticket"/>.
        /// </summary>
        ITicketRepository Tickets { get; }

        /// <summary>
        /// Repositorio específico para la persistencia del agregado <see cref="PIGI_PT_Domain.Aggregates.Inquilino.Inquilino"/>.
        /// </summary>
        IInquilinoRepository Inquilinos { get; }

        /// <summary>
        /// Repositorio específico para la persistencia del agregado <see cref="PIGI_PT_Domain.Aggregates.Usuario.Usuario"/>.
        /// </summary>
        IUsuarioRepository Usuarios { get; }

        /// <summary>
        /// Repositorio específico para la persistencia del agregado <see cref="PIGI_PT_Domain.Aggregates.Categoria.Categoria"/>.
        /// </summary>
        ICategoriaRepository Categorias { get; }
        
        /// <summary>
        /// Guarda todos los cambios pendientes realizados en el contexto de datos de manera asíncrona.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación opcional.</param>
        /// <returns>Número de registros afectados en la base de datos.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Inicia una nueva transacción explícita en la base de datos de manera asíncrona.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación opcional.</param>
        /// <returns>Verdadero si la transacción se inició correctamente; de lo contrario, falso.</returns>
        Task<bool> BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Confirma todos los cambios realizados durante la transacción activa de manera asíncrona.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación opcional.</param>
        /// <returns>Verdadero si la transacción se confirmó con éxito; de lo contrario, falso.</returns>
        Task<bool> CommitTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Revierte y descarta todos los cambios realizados durante la transacción activa en caso de error de manera asíncrona.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación opcional.</param>
        /// <returns>Verdadero si la transacción se revirtió con éxito; de lo contrario, falso.</returns>
        Task<bool> RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
