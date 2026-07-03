using MediatR;
using PIGI_PT_Application.Commands.Ticket;
using PIGI_PT_Application.DTOs.Ticket;
using PIGI_PT_Application.Ports.Infrastructure;
using PIGI_PT_Domain.Specifications.Ticket;

namespace PIGI_PT_Application.Queries.Ticket
{
    /// <summary>
    /// Query para obtener tickets creados por el usuario actual.
    /// Utilizado por UsuarioGeneral para ver su historial de reportes.
    /// </summary>
    public class GetMisTicketsQuery : IRequest<List<TicketDto>>
    {
        public Guid InquilinoId { get; set; }
        public Guid UserId { get; set; }
    }

    public class GetMisTicketsQueryHandler : IRequestHandler<GetMisTicketsQuery, List<TicketDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetMisTicketsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<TicketDto>> Handle(GetMisTicketsQuery request, CancellationToken cancellationToken)
        {
            var spec = new TicketsByCreadorSpec(request.InquilinoId, request.UserId);
            var tickets = await _unitOfWork.Tickets.GetBySpecificationAsync(spec);
            return tickets.Select(TicketMapper.ToDto).ToList();
        }
    }

    /// <summary>
    /// Query para obtener tickets del área del operador.
    /// Retorna tickets clasificados en las categorías asignadas al operador,
    /// incluyendo los tickets asignados directamente al operador.
    /// </summary>
    public class GetTicketsByOperadorAreaQuery : IRequest<List<TicketDto>>
    {
        public Guid InquilinoId { get; set; }
        public Guid OperadorId { get; set; }
    }

    public class GetTicketsByOperadorAreaQueryHandler : IRequestHandler<GetTicketsByOperadorAreaQuery, List<TicketDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTicketsByOperadorAreaQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<TicketDto>> Handle(GetTicketsByOperadorAreaQuery request, CancellationToken cancellationToken)
        {
            // Obtener las categorías del operador
            var operador = await _unitOfWork.Usuarios.GetByIdAsync(request.OperadorId)
                ?? throw new KeyNotFoundException($"Operador con ID '{request.OperadorId}' no encontrado.");

            var categoriaIds = operador.CategoriasAsignadasIds;

            // Obtener tickets de las categorías del operador
            List<PIGI_PT_Domain.Aggregates.Ticket.Ticket> ticketsPorArea;
            if (categoriaIds.Any())
            {
                var specArea = new TicketsByCategoriasSpec(request.InquilinoId, categoriaIds);
                ticketsPorArea = await _unitOfWork.Tickets.GetBySpecificationAsync(specArea);
            }
            else
            {
                ticketsPorArea = new List<PIGI_PT_Domain.Aggregates.Ticket.Ticket>();
            }

            // También incluir tickets asignados directamente al operador
            var specAsignados = new TicketsByOperadorAsignadoSpec(request.InquilinoId, request.OperadorId);
            var ticketsAsignados = await _unitOfWork.Tickets.GetBySpecificationAsync(specAsignados);

            // Combinar sin duplicados, ordenar por fecha
            var todosLosTickets = ticketsPorArea
                .Union(ticketsAsignados, new TicketIdComparer())
                .OrderByDescending(t => t.CreatedAt)
                .ToList();

            return todosLosTickets.Select(TicketMapper.ToDto).ToList();
        }
    }

    /// <summary>
    /// Comparer para evitar duplicados al combinar listas de tickets por ID.
    /// </summary>
    internal class TicketIdComparer : IEqualityComparer<PIGI_PT_Domain.Aggregates.Ticket.Ticket>
    {
        public bool Equals(PIGI_PT_Domain.Aggregates.Ticket.Ticket? x, PIGI_PT_Domain.Aggregates.Ticket.Ticket? y)
        {
            if (x == null || y == null) return false;
            return x.Id == y.Id;
        }

        public int GetHashCode(PIGI_PT_Domain.Aggregates.Ticket.Ticket obj) => obj.Id.GetHashCode();
    }
}
