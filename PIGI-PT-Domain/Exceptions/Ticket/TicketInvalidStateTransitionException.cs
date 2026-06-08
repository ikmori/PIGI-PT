using PIGI_PT_Domain.ValueObjects;

namespace PIGI_PT_Domain.Exceptions.Ticket
{
    /// <summary>
    /// Se lanza cuando se intenta una transición de estado no válida en un ticket.
    /// </summary>
    public class TicketInvalidStateTransitionException : TicketDomainException
    {
        public EstadoTicket EstadoActual { get; }
        public EstadoTicket EstadoSolicitado { get; }

        public TicketInvalidStateTransitionException(
            Guid ticketId,
            EstadoTicket estadoActual,
            EstadoTicket estadoSolicitado)
            : base(
                $"El ticket {ticketId} no puede transicionar de '{estadoActual.Nombre}' a '{estadoSolicitado.Nombre}'.",
                ticketId,
                "TICKET_INVALID_STATE_TRANSITION")
        {
            EstadoActual = estadoActual;
            EstadoSolicitado = estadoSolicitado;
        }
    }
}
