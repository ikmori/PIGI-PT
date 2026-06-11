using PIGI_PT_Domain.Specifications;

namespace PIGI_PT_Domain.Specifications.Ticket
{
    /// <summary>
    /// Especificación para obtener tickets paginados de un inquilino.
    /// Soporta paginación basada en índice y tamaño de página,
    /// ordenados por fecha de creación descendente.
    /// </summary>
    public class PaginatedTicketsSpec : Specification<Aggregates.Ticket.Ticket>
    {
        /// <summary>
        /// Crea una especificación para consultas paginadas de tickets.
        /// </summary>
        /// <param name="inquilinoId">Identificador del inquilino.</param>
        /// <param name="pageIndex">Índice de la página (basado en 0).</param>
        /// <param name="pageSize">Número de elementos por página.</param>
        public PaginatedTicketsSpec(Guid inquilinoId, int pageIndex, int pageSize)
        {
            Criteria = t => t.InquilinoId == inquilinoId;
            OrderByDescending = t => t.CreatedAt;
            IsPagingEnabled = true;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }
    }
}
