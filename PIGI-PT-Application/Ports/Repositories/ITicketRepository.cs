using PIGI_PT_Domain.Aggregates.Ticket;

namespace PIGI_PT_Application.Ports.Repositories
{
    public interface ITicketRepository : IRepository<Ticket>
    {
        Task<List<Ticket>> GetActiveByInquilinoAsync(Guid inquilinoId);
    }
}
