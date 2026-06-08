using PIGI_PT_Domain.Aggregates.Usuario;

namespace PIGI_PT_Application.Ports.Repositories
{
    /// <summary>
    /// Contrato de repositorio específico para gestionar la persistencia y consultas del agregado <see cref="Usuario"/>.
    /// Extiende de <see cref="IRepository{Usuario}"/>.
    /// </summary>
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        /// <summary>
        /// Obtiene un usuario a partir de su dirección de correo electrónico.
        /// Útil para procesos de autenticación y validación de unicidad.
        /// </summary>
        /// <param name="email">Dirección de correo electrónico del usuario.</param>
        /// <returns>El usuario que coincide con el correo o null si no se encuentra.</returns>
        Task<Usuario?> GetByEmailAsync(string email);

        /// <summary>
        /// Obtiene un usuario a partir de su nombre de usuario (username).
        /// Útil para la autenticación en el sistema.
        /// </summary>
        /// <param name="userName">Nombre de usuario.</param>
        /// <returns>El usuario que coincide con el username o null si no se encuentra.</returns>
        Task<Usuario?> GetByUserNameAsync(string userName);
    }
}
