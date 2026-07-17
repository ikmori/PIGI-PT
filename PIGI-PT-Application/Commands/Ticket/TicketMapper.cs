using PIGI_PT_Application.DTOs.Ticket;

namespace PIGI_PT_Application.Commands.Ticket
{
    /// <summary>
    /// Mapper estático para convertir la entidad Ticket del dominio a su DTO.
    /// Usado por todos los handlers de Ticket para mantener consistencia.
    /// </summary>
    public static class TicketMapper
    {
        public static TicketDto ToDto(PIGI_PT_Domain.Aggregates.Ticket.Ticket ticket, string? categoriaNombre = null, string? operadorNombre = null, string? creadorNombre = null, string? creadorEmail = null)
        {
            return new TicketDto
            {
                Id = ticket.Id,
                InquilinoId = ticket.InquilinoId,
                Titulo = ticket.Titulo,
                DescripcionOriginal = ticket.DescripcionOriginal,
                DescripcionSanitizada = ticket.DescripcionSanitizada,
                Estado = ticket.Estado.Nombre,
                EstadoValor = ticket.Estado.Valor,
                Prioridad = ticket.Prioridad.Nombre,
                PrioridadValor = ticket.Prioridad.Valor,
                CategoriaId = ticket.CategoriaId,
                NombreCategoria = categoriaNombre,
                ResponsableTecnologiaId = ticket.ResponsableTecnologiaId,
                ResponsableTecnologiaNombre = operadorNombre,
                CreatedAt = ticket.CreatedAt,
                CreatedBy = ticket.CreatedBy,
                CreadorNombre = creadorNombre,
                CreadorEmail = creadorEmail,
                ModifiedAt = ticket.ModifiedAt,
                FechaResolucion = ticket.FechaResolucion,
                FechaAsignacion = ticket.FechaAsignacion,
                EstaAtrasado = ticket.EstaAtrasado,
                RequiereEscalada = ticket.RequiereEscalada,
                IsActive = ticket.IsActive
            };
        }
    }
}
