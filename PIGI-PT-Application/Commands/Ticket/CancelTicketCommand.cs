using MediatR;
using PIGI_PT_Application.Ports.Infrastructure;

namespace PIGI_PT_Application.Commands.Ticket
{
    /// <summary>
    /// Command para cancelar un ticket existente.
    /// Transiciona el estado a Cancelado sin eliminar de la base de datos.
    /// </summary>
    public class CancelTicketCommand : IRequest
    {
        public Guid TicketId { get; set; }
        public Guid UserId { get; set; }
    }

    public class CancelTicketCommandHandler : IRequestHandler<CancelTicketCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CancelTicketCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CancelTicketCommand request, CancellationToken cancellationToken)
        {
            var spec = new PIGI_PT_Domain.Specifications.Ticket.TicketByIdSpec(request.TicketId);
            var tickets = await _unitOfWork.Tickets.GetBySpecificationAsync(spec);
            var ticket = tickets.FirstOrDefault()
                ?? throw new KeyNotFoundException($"Ticket {request.TicketId} no encontrado.");

            // Validar que el usuario que cancela sea Administrador
            var usuario = await _unitOfWork.Usuarios.GetByIdAsync(request.UserId)
                ?? throw new KeyNotFoundException($"Usuario {request.UserId} no encontrado.");

            if (!usuario.EsAdministrador())
                throw new InvalidOperationException("Solo los administradores pueden borrar o cancelar tickets.");

            ticket.Cancelar(request.UserId);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
