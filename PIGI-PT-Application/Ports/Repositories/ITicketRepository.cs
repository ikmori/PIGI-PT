using PIGI_PT_Domain.Aggregates.Ticket;

namespace PIGI_PT_Application.Ports.Repositories
{
    /// <summary>
    /// Contrato de repositorio específico para gestionar la persistencia del agregado <see cref="Ticket"/>.
    /// </summary>
    public interface ITicketRepository : IRepository<Ticket>
    {
        /// <summary>
        /// Obtiene la lista de todos los tickets activos asociados a un inquilino específico.
        /// Un ticket está activo si no se encuentra en estado Final (Resuelto, Cancelado o Rechazado).
        /// </summary>
        /// <param name="inquilinoId">Identificador único del inquilino.</param>
        /// <returns>Lista de tickets activos del inquilino.</returns>
        Task<List<Ticket>> GetActiveByInquilinoAsync(Guid inquilinoId);
    }
}
