using MediatR;
using PIGI_PT_Application.DTOs.Ticket;
using PIGI_PT_Application.Ports.Infrastructure;

namespace PIGI_PT_Application.Commands.Ticket
{
    /// <summary>
    /// Command para resolver un ticket.
    /// </summary>
    public class ResolveTicketCommand : IRequest<TicketDto>
    {
        public Guid TicketId { get; set; }
        public Guid UserId { get; set; }
    }

    /// <summary>
    /// Handler para ResolveTicketCommand.
    /// Recupera el ticket y ejecuta la lógica de resolución del dominio.
    /// </summary>
    public class ResolveTicketCommandHandler : IRequestHandler<ResolveTicketCommand, TicketDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ResolveTicketCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TicketDto> Handle(ResolveTicketCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _unitOfWork.Tickets.GetByIdAsync(request.TicketId)
                ?? throw new KeyNotFoundException($"Ticket con ID '{request.TicketId}' no encontrado.");

            ticket.Resolver(request.UserId);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return TicketMapper.ToDto(ticket);
        }
    }
}
