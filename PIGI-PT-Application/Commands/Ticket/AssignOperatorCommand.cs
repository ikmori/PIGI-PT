using MediatR;
using PIGI_PT_Application.DTOs.Ticket;
using PIGI_PT_Application.Ports.Infrastructure;

namespace PIGI_PT_Application.Commands.Ticket
{
    /// <summary>
    /// Command para asignar manualmente un operador a un ticket.
    /// Transiciona el estado de Clasificado → EnProgreso.
    /// Solo Admin/SuperAdmin pueden ejecutar este comando.
    /// </summary>
    public class AssignOperatorCommand : IRequest<TicketDto>
    {
        public Guid TicketId { get; set; }
        public Guid OperadorId { get; set; }
        public Guid UserId { get; set; } // Admin que realiza la asignación
    }

    /// <summary>
    /// Handler para AssignOperatorCommand.
    /// Valida que el operador exista y sea activo, luego invoca Ticket.AsignarOperador().
    /// </summary>
    public class AssignOperatorCommandHandler : IRequestHandler<AssignOperatorCommand, TicketDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AssignOperatorCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TicketDto> Handle(AssignOperatorCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _unitOfWork.Tickets.GetByIdAsync(request.TicketId)
                ?? throw new KeyNotFoundException($"Ticket con ID '{request.TicketId}' no encontrado.");

            // Validar que el operador exista y esté activo
            var operador = await _unitOfWork.Usuarios.GetByIdAsync(request.OperadorId)
                ?? throw new KeyNotFoundException($"Operador con ID '{request.OperadorId}' no encontrado.");

            if (!operador.IsActive)
                throw new InvalidOperationException("No se puede asignar un operador inactivo.");

            if (!operador.EsOperador())
                throw new InvalidOperationException("El usuario seleccionado no tiene rol de Operador.");

            // Delegar la lógica de negocio al dominio
            ticket.AsignarOperador(request.OperadorId, request.UserId);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return TicketMapper.ToDto(ticket);
        }
    }
}
