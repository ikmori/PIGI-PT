using PIGI_PT_Domain.Base;
namespace PIGI_PT_Domain.Aggregates.Ticket
{
    public class RegistroDeHistorial : InquilinoEntity
    {
        public Guid TicketId { get; private set; }
        public TipoDeAccionHistorial TipoDeAccion { get; private set; }
        public string? ValoresAnteriores { get; private set; }
        public string ValoresNuevos { get; private set; }

        private RegistroDeHistorial() : base()
        {
        }

        public RegistroDeHistorial(Guid inquilinoId, Guid ticketId, TipoDeAccionHistorial tipoDeAccion,
            string? valoresAnteriores, string valoresNuevos, Guid userId) : base(inquilinoId)
        {
            if (ticketId == Guid.Empty)
                throw new ArgumentException("El identificador del ticket es obligatorio.");

            if (userId == Guid.Empty)
                throw new ArgumentException("El identificador del usuario es obligatorio.");

            if (!Enum.IsDefined(typeof(TipoDeAccionHistorial), tipoDeAccion))
                throw new ArgumentException("El tipo de acción proporcionado no es válido.");

            if (string.IsNullOrWhiteSpace(valoresNuevos))
                throw new ArgumentException("Los nuevos valores son obligatorios para la auditoría.");

            TicketId = ticketId;
            TipoDeAccion = tipoDeAccion;
            ValoresAnteriores = valoresAnteriores;
            ValoresNuevos = valoresNuevos;

            CreatedBy = userId;
        }
    }
}