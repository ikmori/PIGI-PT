namespace PIGI_PT_Domain.Exceptions.Usuario
{
    /// <summary>
    /// Excepción lanzada cuando se intenta reactivar un usuario que ya está activo.
    /// </summary>
    public class UsuarioYaActivoException : UsuarioDomainException
    {
        public UsuarioYaActivoException(Guid usuarioId, string detalles = "")
            : base(usuarioId,
                  $"El usuario {usuarioId} ya está activo. {detalles}",
                  "USUARIO_YA_ACTIVO")
        {
        }
    }
}
