using MediatR;
using PIGI_PT_Application.DTOs.Ticket;
using PIGI_PT_Application.Ports.Infrastructure;
using PIGI_PT_Domain.ValueObjects;

namespace PIGI_PT_Application.Commands.Ticket
{
    /// <summary>
    /// Command para clasificar un ticket mediante IA o manualmente.
    /// </summary>
    public class ClassifyTicketCommand : IRequest<TicketDto>
    {
        public Guid TicketId { get; set; }
        public int PrioridadValor { get; set; }
        public Guid CategoriaId { get; set; }
        public Guid UserId { get; set; }
    }

    /// <summary>
    /// Handler para ClassifyTicketCommand.
    /// Recupera el ticket, aplica la clasificación de dominio y persiste.
    /// </summary>
    public class ClassifyTicketCommandHandler : IRequestHandler<ClassifyTicketCommand, TicketDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ClassifyTicketCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TicketDto> Handle(ClassifyTicketCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _unitOfWork.Tickets.GetByIdAsync(request.TicketId)
                ?? throw new KeyNotFoundException($"Ticket con ID '{request.TicketId}' no encontrado.");

            var prioridad = NivelPrioridad.Create(request.PrioridadValor);

            ticket.ClasificarPorIA(prioridad, request.CategoriaId);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return TicketMapper.ToDto(ticket);
        }
    }
}
