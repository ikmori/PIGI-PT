using Microsoft.EntityFrameworkCore;
using PIGI_PT_Application.Ports.Repositories;
using PIGI_PT_Domain.Aggregates.Usuario;
using PIGI_PT_Infraestructure.Persistence.DbContext;

namespace PIGI_PT_Infraestructure.Persistence.Repositories
{
    /// <summary>
    /// Implementación concreta del repositorio de <see cref="Usuario"/>.
    /// Extiende <see cref="BaseRepository{Usuario}"/> con métodos específicos
    /// para búsqueda por email y nombre de usuario para autenticación.
    /// </summary>
    public class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(PigiPtDbContext context) : base(context)
        {
        }

        /// <inheritdoc/>
        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <inheritdoc/>
        public async Task<Usuario?> GetByUserNameAsync(string userName)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.UserName == userName);
        }
    }
}
