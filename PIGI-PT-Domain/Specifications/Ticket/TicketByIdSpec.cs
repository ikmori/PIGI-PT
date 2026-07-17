using PIGI_PT_Domain.Specifications;

namespace PIGI_PT_Domain.Specifications.Ticket
{
    /// <summary>
    /// Especificación para obtener un ticket por su ID único.
    /// </summary>
    public class TicketByIdSpec : Specification<Aggregates.Ticket.Ticket>
    {
        public TicketByIdSpec(Guid ticketId)
        {
            Criteria = t => t.Id == ticketId;
        }
    }
}
