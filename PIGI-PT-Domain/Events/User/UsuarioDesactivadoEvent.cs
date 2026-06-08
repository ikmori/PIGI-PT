using PIGI_PT_Domain.Base;

namespace PIGI_PT_Domain.Events.User
{
    /// <summary>
    /// Evento de dominio emitido cuando un usuario es desactivado.
    /// </summary>
    public class UsuarioDesactivadoEvent: DomainEvent
    {
        public Guid UsuarioId { get; }
        public Guid InquilinoId { get; }
        public Guid ModificadorId { get; }

        public UsuarioDesactivadoEvent(Guid usuarioId, Guid inquilinoId, Guid modificadorId)
        {
            UsuarioId = usuarioId;
            InquilinoId = inquilinoId;
            ModificadorId = modificadorId;
        }
    }
}
