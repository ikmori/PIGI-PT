using Microsoft.EntityFrameworkCore;
using PIGI_PT_Application.Ports.Repositories;
using PIGI_PT_Domain.Aggregates.Inquilino;
using PIGI_PT_Infraestructure.Persistence.DbContext;

namespace PIGI_PT_Infraestructure.Persistence.Repositories
{
    /// <summary>
    /// Implementación concreta del repositorio de <see cref="Inquilino"/>.
    /// Extiende <see cref="BaseRepository{Inquilino}"/> con métodos específicos
    /// para resolución de multi-tenant por dominio de red.
    /// </summary>
    public class InquilinoRepository : BaseRepository<Inquilino>, IInquilinoRepository
    {
        public InquilinoRepository(PigiPtDbContext context) : base(context)
        {
        }

        /// <inheritdoc/>
        public async Task<Inquilino?> GetByDominioAsync(string dominio)
        {
            return await _context.Inquilinos
                .FirstOrDefaultAsync(i => i.DominioRed == dominio);
        }
    }
}
