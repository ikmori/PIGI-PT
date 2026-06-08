using PIGI_PT_Domain.Base;

namespace PIGI_PT_Domain.Events.Inquilino
{
    /// <summary>
    /// Evento de dominio emitido cuando se modifican los permisos de IA de un inquilino.
    /// Registra el cambio de privacidad para auditoría y notificaciones.
    /// </summary>
    public class PrivacidadIAModificadaEvent: DomainEvent
    {
        public Guid InquilinoId { get; }
        public bool PermitirAi { get; }
        public Guid AdministradorId { get; }

        public PrivacidadIAModificadaEvent(Guid inquilinoId, bool permitirAi, Guid administradorId)
        {
            InquilinoId = inquilinoId;
            PermitirAi = permitirAi;
            AdministradorId = administradorId;
        }
    }
}
