namespace PIGI_PT_Domain.Exceptions.Usuario
{
    /// <summary>
    /// Excepción lanzada cuando se intenta modificar un usuario inactivo.
    /// </summary>
    public class UsuarioInactivoException : UsuarioDomainException
    {
        public UsuarioInactivoException(Guid usuarioId, string detalles = "")
            : base(usuarioId,
                  $"El usuario {usuarioId} está inactivo. {detalles}",
                  "USUARIO_INACTIVO")
        {
        }
    }
}
