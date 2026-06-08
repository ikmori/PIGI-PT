using PIGI_PT_Domain.Base;

namespace PIGI_PT_Domain.Events.User
{
    /// <summary>
    /// Evento de dominio emitido cuando se actualiza la contraseña de un usuario.
    /// </summary>
    public class ContrasenaActualizadaEvent: DomainEvent
    {
        public Guid UsuarioId { get; }
        public Guid InquilinoId { get; }
        public Guid ModificadorId { get; }

        public ContrasenaActualizadaEvent(Guid usuarioId, Guid inquilinoId, Guid modificadorId)
        {
            UsuarioId = usuarioId;
            InquilinoId = inquilinoId;
            ModificadorId = modificadorId;
        }
    }
}
