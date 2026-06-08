namespace PIGI_PT_Domain.Exceptions.Ticket
{
    /// <summary>
    /// Se lanza cuando se intenta realizar una acción en un ticket que ya ha sido resuelto.
    /// </summary>
    public class TicketAlreadyResolvedException : TicketDomainException
    {
        public TicketAlreadyResolvedException(Guid ticketId)
            : base(
                $"El ticket {ticketId} ya ha sido resuelto y no puede modificarse.",
                ticketId,
                "TICKET_ALREADY_RESOLVED")
        {
        }
    }
}
