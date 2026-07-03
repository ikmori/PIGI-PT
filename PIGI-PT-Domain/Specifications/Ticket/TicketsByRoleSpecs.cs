using PIGI_PT_Domain.Specifications;

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
    /// Especificación para obtener tickets asignados a un operador específico.
    /// </summary>
    public class TicketsByOperadorAsignadoSpec : Specification<Aggregates.Ticket.Ticket>
    {
        public TicketsByOperadorAsignadoSpec(Guid inquilinoId, Guid operadorId)
        {
            Criteria = t => t.InquilinoId == inquilinoId && t.OperadorAsignadoId == operadorId;
            OrderByDescending = t => t.CreatedAt;
        }
    }

    /// <summary>
    /// Especificación para obtener tickets por categorías del área del operador.
    /// Incluye tickets clasificados en cualquiera de las categorías asignadas al operador.
    /// </summary>
    public class TicketsByCategoriasSpec : Specification<Aggregates.Ticket.Ticket>
    {
        public TicketsByCategoriasSpec(Guid inquilinoId, IEnumerable<Guid> categoriaIds)
        {
            var categoriasSet = categoriaIds.ToHashSet();
            Criteria = t => t.InquilinoId == inquilinoId
                         && t.CategoriaId.HasValue
                         && categoriasSet.Contains(t.CategoriaId.Value);
            OrderByDescending = t => t.CreatedAt;
        }
    }
}
