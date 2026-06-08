namespace PIGI_PT_Domain.Exceptions.Ticket
{
    /// <summary>
    /// Se lanza cuando se intenta resolver un ticket sin operador asignado.
    /// </summary>
    public class TicketMissingOperatorException : TicketDomainException
    {
        public TicketMissingOperatorException(Guid ticketId)
            : base(
                $"El ticket {ticketId} no puede resolverse sin un operador asignado.",
                ticketId,
                "TICKET_MISSING_OPERATOR")
        {
        }
    }
}
