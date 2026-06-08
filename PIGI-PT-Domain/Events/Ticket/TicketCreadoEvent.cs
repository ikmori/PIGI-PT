using PIGI_PT_Domain.Base;

namespace PIGI_PT_Domain.Events.Ticket
{
    /// <summary>
    /// Evento emitido cuando se crea un nuevo ticket.
    /// </summary>
    public class TicketCreadoEvent : DomainEvent
    {
        public Guid TicketId { get; }
        public string Titulo { get; }
        public string DescripcionOriginal { get; }
        public Guid CreadorId { get; }

        public TicketCreadoEvent(Guid ticketId, string titulo, string descripcionOriginal, Guid creadorId)
        {
            TicketId = ticketId;
            Titulo = titulo;
            DescripcionOriginal = descripcionOriginal;
            CreadorId = creadorId;
        }
    }
}
