using PIGI_PT_Domain.Base;
using PIGI_PT_Domain.ValueObjects;

namespace PIGI_PT_Domain.Events.User
{
    /// <summary>
    /// Evento de dominio emitido cuando un usuario es reactivado.
    /// </summary>
    public class UsuarioReactivadoEvent : DomainEvent
    {
        public Guid UsuarioId { get; }
        public Guid InquilinoId { get; }
        public Guid AdministradorId { get; }

        public UsuarioReactivadoEvent(Guid usuarioId, Guid inquilinoId, Guid administradorId)
        {
            UsuarioId = usuarioId;
            InquilinoId = inquilinoId;
            AdministradorId = administradorId;
        }
    }
}
