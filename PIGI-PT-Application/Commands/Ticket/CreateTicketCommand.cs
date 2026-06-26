using MediatR;
using PIGI_PT_Application.DTOs.Ticket;
using PIGI_PT_Application.Ports.Infrastructure;

namespace PIGI_PT_Application.Commands.Ticket
{
    /// <summary>
    /// Command para crear un nuevo ticket de incidente.
    /// </summary>
    public class CreateTicketCommand : IRequest<TicketDto>
    {
        public Guid InquilinoId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public Guid UserId { get; set; }
    }

    /// <summary>
    /// Handler que procesa el comando CreateTicketCommand.
    /// Crea la entidad Ticket en el dominio y la persiste via UnitOfWork.
    /// </summary>
    public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, TicketDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateTicketCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TicketDto> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
        {
            // Crear entidad de dominio (la lógica de validación está en el constructor)
            var ticket = new PIGI_PT_Domain.Aggregates.Ticket.Ticket(
                request.InquilinoId,
                request.Titulo,
                request.Descripcion,
                request.UserId
            );

            // Persistir mediante el repositorio
            await _unitOfWork.Tickets.AddAsync(ticket);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return TicketMapper.ToDto(ticket);
        }
    }
}
