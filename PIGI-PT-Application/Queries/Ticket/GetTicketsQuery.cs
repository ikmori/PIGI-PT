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
        public Guid? ResponsableTecnologiaId { get; set; }

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
            var spec = new PIGI_PT_Domain.Specifications.Ticket.AllTicketsByInquilinoSpec(request.InquilinoId);
            var tickets = await _unitOfWork.Tickets.GetBySpecificationAsync(spec);

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

            // Filtrar por responsable asignado
            if (request.ResponsableTecnologiaId.HasValue)
            {
                tickets = tickets
                    .Where(t => t.ResponsableTecnologiaId == request.ResponsableTecnologiaId.Value)
                    .ToList();
            }

            // Filtrar por creador
            if (request.CreatedByUserId.HasValue)
            {
                tickets = tickets
                    .Where(t => t.CreatedBy == request.CreatedByUserId.Value)
                    .ToList();
            }

            // Obtener todas las categorías y usuarios para mapear nombres
            var categoriasSpec = new PIGI_PT_Domain.Specifications.Categoria.CategoriasByInquilinoSpec(request.InquilinoId);
            var categorias = await _unitOfWork.Categorias.GetBySpecificationAsync(categoriasSpec);
            var dictCategorias = categorias.ToDictionary(c => c.Id, c => c.NombreCategoria);

            var usuariosSpec = new PIGI_PT_Domain.Specifications.Usuario.ActiveUsuariosByInquilinoSpec(request.InquilinoId);
            var usuarios = await _unitOfWork.Usuarios.GetBySpecificationAsync(usuariosSpec);
            var dictUsuarios = usuarios.ToDictionary(u => u.Id, u => u);

            return tickets.Select(t => {
                string? catNombre = null;
                if (t.CategoriaId.HasValue && t.CategoriaId.Value != Guid.Empty && dictCategorias.TryGetValue(t.CategoriaId.Value, out var cn)) catNombre = cn;
                
                string? opNombre = null;
                if (t.ResponsableTecnologiaId.HasValue && dictUsuarios.TryGetValue(t.ResponsableTecnologiaId.Value, out var op)) opNombre = op.FullName;

                string? creadorNombre = null;
                string? creadorEmail = null;
                if (t.CreatedBy.HasValue && t.CreatedBy.Value != Guid.Empty && dictUsuarios.TryGetValue(t.CreatedBy.Value, out var cr)) 
                {
                    creadorNombre = cr.FullName;
                    creadorEmail = cr.Email;
                }

                return TicketMapper.ToDto(t, catNombre, opNombre, creadorNombre, creadorEmail);
            }).ToList();
        }
    }
}

