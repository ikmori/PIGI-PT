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

    /// <summary>
    /// Query para obtener tickets del área del departamento técnico.
    /// Retorna tickets clasificados en las categorías asignadas al responsable,
    /// incluyendo los tickets asignados directamente a él.
    /// </summary>
    public class GetTicketsByAreaTecnologiaQuery : IRequest<List<TicketDto>>
    {
        public Guid InquilinoId { get; set; }
        public Guid ResponsableId { get; set; }
    }

    public class GetTicketsByAreaTecnologiaQueryHandler : IRequestHandler<GetTicketsByAreaTecnologiaQuery, List<TicketDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTicketsByAreaTecnologiaQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<TicketDto>> Handle(GetTicketsByAreaTecnologiaQuery request, CancellationToken cancellationToken)
        {
            // Obtener las categorías del responsable
            var responsable = await _unitOfWork.Usuarios.GetByIdAsync(request.ResponsableId)
                ?? throw new KeyNotFoundException($"Responsable con ID '{request.ResponsableId}' no encontrado.");

            var departamentoId = responsable.DepartamentoId;
            var categoriaIds = new List<Guid>();
            if (departamentoId.HasValue)
            {
                categoriaIds.Add(departamentoId.Value);
            }

            // Obtener tickets de las categorías del responsable
            List<PIGI_PT_Domain.Aggregates.Ticket.Ticket> ticketsPorArea = new();
            if (categoriaIds.Any())
            {
                var specArea = new TicketsByCategoriasSpec(request.InquilinoId, categoriaIds);
                ticketsPorArea = await _unitOfWork.Tickets.GetBySpecificationAsync(specArea);
            }

            // Obtener tickets asignados directamente al responsable
            var specAsignados = new TicketsByResponsableAsignadoSpec(request.InquilinoId, request.ResponsableId);
            var ticketsAsignados = await _unitOfWork.Tickets.GetBySpecificationAsync(specAsignados);

            // Combinar y evitar duplicados
            var todosLosTicketsUnicos = ticketsPorArea.Concat(ticketsAsignados)
                .Distinct(new TicketIdComparer())
                .ToList();

            // Combinar y ordenar por fecha
            var todosLosTickets = todosLosTicketsUnicos
                .OrderByDescending(t => t.CreatedAt)
                .ToList();

            // Obtener todas las categorías y usuarios para mapear nombres
            var categoriasSpec = new PIGI_PT_Domain.Specifications.Categoria.CategoriasByInquilinoSpec(request.InquilinoId);
            var categorias = await _unitOfWork.Categorias.GetBySpecificationAsync(categoriasSpec);
            var dictCategorias = categorias.ToDictionary(c => c.Id, c => c.NombreCategoria);

            var usuariosSpec = new PIGI_PT_Domain.Specifications.Usuario.ActiveUsuariosByInquilinoSpec(request.InquilinoId);
            var usuarios = await _unitOfWork.Usuarios.GetBySpecificationAsync(usuariosSpec);
            var dictUsuarios = usuarios.ToDictionary(u => u.Id, u => u);

            return todosLosTickets.Select(t => {
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
