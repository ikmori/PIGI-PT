using PIGI_PT_Domain.Base;
using PIGI_PT_Domain.ValueObjects;

namespace PIGI_PT_Domain.Events.User
{
    /// <summary>
    /// Evento de dominio emitido cuando se modifica el rol de un usuario.
    /// </summary>
    public class RolModificadoEvent: DomainEvent
    {
        public Guid UsuarioId { get; }
        public Guid InquilinoId { get; }
        public Rol RolAnterior { get; }
        public Rol RolNuevo { get; }
        public Guid ModificadorId { get; }

        public RolModificadoEvent(Guid usuarioId, Guid inquilinoId, Rol rolAnterior, Rol rolNuevo, Guid modificadorId)
        {
            UsuarioId = usuarioId;
            InquilinoId = inquilinoId;
            RolAnterior = rolAnterior;
            RolNuevo = rolNuevo;
            ModificadorId = modificadorId;
        }
    }
}
