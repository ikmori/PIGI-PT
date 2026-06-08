using PIGI_PT_Domain.Base;
using PIGI_PT_Domain.ValueObjects;

namespace PIGI_PT_Domain.Events.User
{
    /// <summary>
    /// Evento de dominio emitido cuando un nuevo usuario es creado.
    /// </summary>
    public class UsuarioCreadoEvent: DomainEvent
    {
        public Guid UsuarioId { get; }
        public Guid InquilinoId { get; }
        public string NombreCompleto { get; }
        public string Email { get; }
        public string NombreUsuario { get; }
        public Rol Rol { get; }

        public UsuarioCreadoEvent(Guid usuarioId, Guid inquilinoId, string nombreCompleto, string email, string nombreUsuario, Rol rol)
        {
            UsuarioId = usuarioId;
            InquilinoId = inquilinoId;
            NombreCompleto = nombreCompleto;
            Email = email;
            NombreUsuario = nombreUsuario;
            Rol = rol;
        }
    }
}
