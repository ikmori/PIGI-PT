using System.Linq.Expressions;
using PIGI_PT_Domain.Aggregates.Ticket;
using PIGI_PT_Domain.Specifications;

namespace PIGI_PT_Domain.Specifications.Ticket
{
    /// <summary>
    /// Especificación para obtener todos los tickets de un inquilino específico,
    /// ordenados por fecha de creación descendente.
    /// </summary>
    public class TicketsByInquilinoIdSpec : Specification<Aggregates.Ticket.Ticket>
    {
        public TicketsByInquilinoIdSpec(Guid inquilinoId)
        {
            Criteria = t => t.InquilinoId == inquilinoId;
            OrderByDescending = t => t.CreatedAt;
        }
    }
}
