using MediatR;
using PIGI_PT_Application.Commands.Ticket;
using PIGI_PT_Application.DTOs.Ticket;
using PIGI_PT_Application.Ports.Infrastructure;

namespace PIGI_PT_Application.Queries.Ticket
{
    /// <summary>
    /// Query para obtener tickets de un inquilino con filtros opcionales.
    /// Soporta filtrado por estado, categoría, operador asignado y creador.
    /// </summary>
    public class GetTicketsQuery : IRequest<List<TicketDto>>
    {
        public Guid InquilinoId { get; set; }

        /// <summary>
        /// Filtro opcional por estado (ej: "Clasificado", "EnProgreso").
        /// Si es null, devuelve todos los tickets activos.
        /// </summary>
        public string? Estado { get; set; }

        /// <summary>
        /// Filtro opcional por categoría.
        /// </summary>
        public Guid? CategoriaId { get; set; }

        /// <summary>
        /// Filtro opcional por operador asignado.
        /// </summary>
        public Guid? OperadorAsignadoId { get; set; }

        /// <summary>
        /// Filtro opcional por creador del ticket (para UsuarioGeneral: solo sus tickets).
        /// </summary>
        public Guid? CreatedByUserId { get; set; }
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

            // Filtrar por categoría
            if (request.CategoriaId.HasValue)
            {
                tickets = tickets
                    .Where(t => t.CategoriaId == request.CategoriaId.Value)
                    .ToList();
            }

            // Filtrar por operador asignado
            if (request.OperadorAsignadoId.HasValue)
            {
                tickets = tickets
                    .Where(t => t.OperadorAsignadoId == request.OperadorAsignadoId.Value)
                    .ToList();
            }

            // Filtrar por creador
            if (request.CreatedByUserId.HasValue)
            {
                tickets = tickets
                    .Where(t => t.CreatedBy == request.CreatedByUserId.Value)
                    .ToList();
            }

            return tickets.Select(TicketMapper.ToDto).ToList();
        }
    }
}

