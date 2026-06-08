namespace PIGI_PT_Domain.Exceptions.Usuario
{
    /// <summary>
    /// Excepción lanzada cuando se intenta desactivar un usuario que ya está inactivo.
    /// </summary>
    public class UsuarioYaInactivoException : UsuarioDomainException
    {
        public UsuarioYaInactivoException(Guid usuarioId, string detalles = "")
            : base(usuarioId,
                  $"El usuario {usuarioId} ya está inactivo. {detalles}",
                  "USUARIO_YA_INACTIVO")
        {
        }
    }
}
