using PIGI_PT_Domain.Specifications;
using PIGI_PT_Domain.ValueObjects;

namespace PIGI_PT_Domain.Specifications.Ticket
{
    /// <summary>
    /// Especificación para obtener tickets creados por un usuario específico.
    /// Utilizada por el UsuarioGeneral para ver solo sus propios tickets.
    /// </summary>
    public class TicketsByCreadorSpec : Specification<Aggregates.Ticket.Ticket>
    {
        public TicketsByCreadorSpec(Guid inquilinoId, Guid creadorId)
        {
            Criteria = t => t.InquilinoId == inquilinoId && t.CreatedBy == creadorId;
            OrderByDescending = t => t.CreatedAt;
        }
    }

    /// <summary>
    /// Especificación para obtener tickets asignados a un responsable específico.
    /// </summary>
    public class TicketsByResponsableAsignadoSpec : Specification<Aggregates.Ticket.Ticket>
    {
        public TicketsByResponsableAsignadoSpec(Guid inquilinoId, Guid responsableId)
        {
            Criteria = t => t.InquilinoId == inquilinoId && t.ResponsableTecnologiaId == responsableId;
            OrderByDescending = t => t.CreatedAt;
        }
    }

    /// <summary>
    /// Especificación para obtener tickets por categorías del área técnica.
    /// Incluye tickets clasificados en cualquiera de las categorías asignadas.
    /// </summary>
    public class TicketsByCategoriasSpec : Specification<Aggregates.Ticket.Ticket>
    {
        public TicketsByCategoriasSpec(Guid inquilinoId, IEnumerable<Guid> categoriaIds)
        {
            var categoriasSet = categoriaIds.ToHashSet();
            Criteria = t => t.InquilinoId == inquilinoId
                         && t.CategoriaId.HasValue
                         && categoriasSet.Contains(t.CategoriaId.Value)
                         && t.Estado != EstadoTicket.Cancelado
                         && t.Estado != EstadoTicket.Rechazado;
            OrderByDescending = t => t.CreatedAt;
        }
    }

    /// <summary>
    /// Especificación para obtener todos los tickets de un inquilino sin importar estado ni categoría.
    /// Utilizada por el Admin.
    /// </summary>
    public class AllTicketsByInquilinoSpec : Specification<Aggregates.Ticket.Ticket>
    {
        public AllTicketsByInquilinoSpec(Guid inquilinoId)
        {
            Criteria = t => t.InquilinoId == inquilinoId;
            OrderByDescending = t => t.CreatedAt;
        }
    }
}
