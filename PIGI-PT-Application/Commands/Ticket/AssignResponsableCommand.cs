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
    public class AssignResponsableCommand : IRequest<TicketDto>
    {
        public Guid TicketId { get; set; }
        public Guid ResponsableId { get; set; }
        public Guid UserId { get; set; } // Admin que realiza la asignación
    }

    /// <summary>
    /// Handler para AssignResponsableCommand.
    /// Valida que el responsable exista y sea activo, luego invoca Ticket.AsignarResponsable().
    /// </summary>
    public class AssignResponsableCommandHandler : IRequestHandler<AssignResponsableCommand, TicketDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AssignResponsableCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TicketDto> Handle(AssignResponsableCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _unitOfWork.Tickets.GetByIdAsync(request.TicketId)
                ?? throw new KeyNotFoundException($"Ticket con ID '{request.TicketId}' no encontrado.");

            // Validar que el responsable exista y esté activo
            var responsable = await _unitOfWork.Usuarios.GetByIdAsync(request.ResponsableId)
                ?? throw new KeyNotFoundException($"Responsable con ID '{request.ResponsableId}' no encontrado.");

            if (!responsable.IsActive)
                throw new InvalidOperationException("No se puede asignar un responsable inactivo.");

            if (!responsable.EsDepartamentoTecnologia() && !responsable.EsAdministrador())
                throw new InvalidOperationException($"El usuario con ID '{request.ResponsableId}' no tiene el rol de Departamento de Tecnología ni de Administrador.");

            // Delegar la lógica de negocio al dominio
            ticket.AsignarResponsable(request.ResponsableId, request.UserId);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return TicketMapper.ToDto(ticket);
        }
    }
}
