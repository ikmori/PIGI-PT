using MediatR;
using PIGI_PT_Application.Commands.Ticket;
using PIGI_PT_Application.DTOs.Ticket;
using PIGI_PT_Application.Ports.Infrastructure;

namespace PIGI_PT_Application.Queries.Ticket
{
    /// <summary>
    /// Query para obtener todos los tickets de un inquilino.
    /// Soporta filtrado opcional por estado.
    /// </summary>
    public class GetTicketsQuery : IRequest<List<TicketDto>>
    {
        public Guid InquilinoId { get; set; }

        /// <summary>
        /// Filtro opcional por estado (ej: "Clasificado", "EnProgreso").
        /// Si es null, devuelve todos los tickets activos.
        /// </summary>
        public string? Estado { get; set; }
    }

    /// <summary>
    /// Handler para GetTicketsQuery.
    /// </summary>
    public class GetTicketsQueryHandler : IRequestHandler<GetTicketsQuery, List<TicketDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTicketsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<TicketDto>> Handle(GetTicketsQuery request, CancellationToken cancellationToken)
        {
            var tickets = await _unitOfWork.Tickets.GetActiveByInquilinoAsync(request.InquilinoId);

            // Filtrar por estado si se proporcionó
            if (!string.IsNullOrWhiteSpace(request.Estado))
            {
                tickets = tickets
                    .Where(t => t.Estado.Nombre.Equals(request.Estado, StringComparison.OrdinalIgnoreCase)
                             || t.Estado.Valor.ToString() == request.Estado)
                    .ToList();
            }

            return tickets.Select(TicketMapper.ToDto).ToList();
        }
    }
}
