using MediatR;
using PIGI_PT_Application.Commands.Ticket;
using PIGI_PT_Application.DTOs.Ticket;
using PIGI_PT_Application.Ports.Infrastructure;

namespace PIGI_PT_Application.Queries.Ticket
{
    /// <summary>
    /// Query para obtener un ticket específico por su ID.
    /// </summary>
    public class GetTicketByIdQuery : IRequest<TicketDto>
    {
        public Guid TicketId { get; set; }
    }

    /// <summary>
    /// Handler para GetTicketByIdQuery.
    /// </summary>
    public class GetTicketByIdQueryHandler : IRequestHandler<GetTicketByIdQuery, TicketDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTicketByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TicketDto> Handle(GetTicketByIdQuery request, CancellationToken cancellationToken)
        {
            var ticket = await _unitOfWork.Tickets.GetByIdAsync(request.TicketId)
                ?? throw new KeyNotFoundException($"Ticket con ID '{request.TicketId}' no encontrado.");

            return TicketMapper.ToDto(ticket);
        }
    }
}
