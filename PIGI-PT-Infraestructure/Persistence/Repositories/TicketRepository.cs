using Microsoft.EntityFrameworkCore;
using PIGI_PT_Application.Ports.Repositories;
using PIGI_PT_Domain.Aggregates.Ticket;
using PIGI_PT_Domain.Specifications.Ticket;
using PIGI_PT_Infraestructure.Persistence.DbContext;

namespace PIGI_PT_Infraestructure.Persistence.Repositories
{
    /// <summary>
    /// Implementación concreta del repositorio de <see cref="Ticket"/>.
    /// Extiende <see cref="BaseRepository{Ticket}"/> con métodos específicos
    /// para consultas de negocio sobre tickets.
    /// </summary>
    public class TicketRepository : BaseRepository<Ticket>, ITicketRepository
    {
        public TicketRepository(PigiPtDbContext context) : base(context)
        {
        }

        /// <inheritdoc/>
        public async Task<List<Ticket>> GetActiveByInquilinoAsync(Guid inquilinoId)
        {
            var spec = new ActiveTicketsSpec(inquilinoId);
            return await GetBySpecificationAsync(spec);
        }
    }
}
