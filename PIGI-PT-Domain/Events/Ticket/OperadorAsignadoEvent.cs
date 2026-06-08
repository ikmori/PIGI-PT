using PIGI_PT_Domain.Base;

namespace PIGI_PT_Domain.Events.Ticket
{
    /// <summary>
    /// Evento emitido cuando se asigna un operador a un ticket.
    /// </summary>
    public class OperadorAsignadoEvent : DomainEvent
    {
        public Guid TicketId { get; }
        public Guid OperadorId { get; }

        public OperadorAsignadoEvent(Guid ticketId, Guid operadorId)
        {
            TicketId = ticketId;
            OperadorId = operadorId;
        }
    }
}
