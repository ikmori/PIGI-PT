using PIGI_PT_Domain.Base;

namespace PIGI_PT_Domain.Events.Inquilino
{
    /// <summary>
    /// Evento de dominio emitido cuando un inquilino es suspendido.
    /// </summary>
    public class InquilinoSuspendidoEvent: DomainEvent
    {
        public Guid InquilinoId { get; }
        public Guid AdministradorId { get; }
        public string Motivo { get; }

        public InquilinoSuspendidoEvent(Guid inquilinoId, Guid administradorId, string motivo)
        {
            InquilinoId = inquilinoId;
            AdministradorId = administradorId;
            Motivo = motivo;
        }
    }
}

