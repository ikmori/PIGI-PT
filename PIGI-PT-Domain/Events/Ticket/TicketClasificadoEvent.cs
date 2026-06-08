using PIGI_PT_Domain.Base;
using PIGI_PT_Domain.ValueObjects;

namespace PIGI_PT_Domain.Events.Ticket
{
    /// <summary>
    /// Evento emitido cuando se clasifica un ticket mediante IA.
    /// </summary>
    public class TicketClasificadoEvent : DomainEvent
    {
        public Guid TicketId { get; }
        public NivelPrioridad Prioridad { get; }
        public Guid CategoriaId { get; }

        public TicketClasificadoEvent(Guid ticketId, NivelPrioridad prioridad, Guid categoriaId)
        {
            TicketId = ticketId;
            Prioridad = prioridad;
            CategoriaId = categoriaId;
        }
    }
}
