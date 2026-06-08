using PIGI_PT_Domain.Base;

namespace PIGI_PT_Domain.Exceptions.Ticket
{
    /// <summary>
    /// Clase base para todas las excepciones de dominio relacionadas con Tickets.
    /// </summary>
    public abstract class TicketDomainException : DomainException
    {
        public Guid TicketId { get; }

        protected TicketDomainException(string message, Guid ticketId, string code)
            : base(message, code)
        {
            TicketId = ticketId;
        }
    }
}
