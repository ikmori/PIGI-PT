using PIGI_PT_Domain.Specifications;
using PIGI_PT_Domain.ValueObjects;

namespace PIGI_PT_Domain.Specifications.Ticket
{
    /// <summary>
    /// Especificación para obtener los tickets activos de un inquilino.
    /// Un ticket activo es aquel que no se encuentra en estado final
    /// (Resuelto, Cancelado o Rechazado).
    /// Los resultados se ordenan por prioridad descendente (más urgentes primero).
    /// </summary>
    public class ActiveTicketsSpec : Specification<Aggregates.Ticket.Ticket>
    {
        public ActiveTicketsSpec(Guid inquilinoId)
        {
            Criteria = t => t.InquilinoId == inquilinoId
                         && t.Estado != EstadoTicket.Cancelado
                         && t.Estado != EstadoTicket.Rechazado;

            OrderByDescending = t => t.Prioridad;
        }
    }
}
