using PIGI_PT_Domain.Base;

namespace PIGI_PT_Domain.Events.Ticket
{
    /// <summary>
    /// Evento emitido cuando se marca un ticket como resuelto.
    /// </summary>
    public class TicketResueltoEvent : DomainEvent
    {
        public Guid TicketId { get; }
        public Guid CreadorId { get; }
        public DateTime FechaResolucion { get; }
        public TimeSpan TiempoDeResolucion { get; }

        public TicketResueltoEvent(Guid ticketId, Guid creadorId, DateTime fechaResolucion, TimeSpan tiempoDeResolucion)
        {
            TicketId = ticketId;
            CreadorId = creadorId;
            FechaResolucion = fechaResolucion;
            TiempoDeResolucion = tiempoDeResolucion;
        }
    }
}
