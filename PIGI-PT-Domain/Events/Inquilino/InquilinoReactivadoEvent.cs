using PIGI_PT_Domain.Base;

namespace PIGI_PT_Domain.Events.Inquilino
{
    /// <summary>
    /// Evento de dominio emitido cuando un inquilino es reactivado.
    /// </summary>
    public class InquilinoReactivadoEvent : DomainEvent
    {
        public Guid InquilinoId { get; }
        public Guid AdministradorId { get; }
        public string Motivo { get; }

        public InquilinoReactivadoEvent(Guid inquilinoId, Guid administradorId, string motivo)
        {
            InquilinoId = inquilinoId;
            AdministradorId = administradorId;
            Motivo = motivo;
        }
    }
}
