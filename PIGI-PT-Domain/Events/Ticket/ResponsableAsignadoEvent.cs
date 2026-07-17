using PIGI_PT_Domain.Base;

namespace PIGI_PT_Domain.Events.Ticket
{
    /// <summary>
    /// Evento emitido cuando se asigna un responsable a un ticket.
    /// </summary>
    public class ResponsableAsignadoEvent : DomainEvent
    {
        public Guid TicketId { get; }
        public Guid ResponsableId { get; }

        public ResponsableAsignadoEvent(Guid ticketId, Guid responsableId)
        {
            TicketId = ticketId;
            ResponsableId = responsableId;
        }
    }
}
