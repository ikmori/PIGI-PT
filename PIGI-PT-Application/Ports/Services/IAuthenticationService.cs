namespace PIGI_PT_Application.Ports.Services
{
    /// <summary>
    /// Puerto que define los servicios de autenticación y generación de tokens de seguridad (JWT)
    /// para el control de acceso en la capa de presentación.
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Genera de manera asíncrona un token JWT firmado para un usuario específico.
        /// </summary>
        /// <param name="userId">Identificador único del usuario.</param>
        /// <param name="userName">Nombre de usuario (username).</param>
        /// <param name="rol">Rol asignado al usuario (ej. Admin, Operador, Inquilino).</param>
        /// <returns>Una cadena que representa el token JWT generado.</returns>
        Task<string> GenerateTokenAsync(Guid userId, string userName, string rol);

        /// <summary>
        /// Valida de manera asíncrona si un token JWT proporcionado es válido y no ha expirado.
        /// </summary>
        /// <param name="token">El token JWT a validar.</param>
        /// <returns>Verdadero si el token es válido; en caso contrario, falso.</returns>
        Task<bool> ValidateTokenAsync(string token);
    }
}
