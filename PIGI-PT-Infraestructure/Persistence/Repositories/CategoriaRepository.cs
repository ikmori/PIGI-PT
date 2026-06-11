using Microsoft.EntityFrameworkCore;
using PIGI_PT_Application.Ports.Repositories;
using PIGI_PT_Domain.Aggregates.Categoria;
using PIGI_PT_Infraestructure.Persistence.DbContext;

namespace PIGI_PT_Infraestructure.Persistence.Repositories
{
    /// <summary>
    /// Implementación concreta del repositorio de <see cref="Categoria"/>.
    /// Extiende <see cref="BaseRepository{Categoria}"/> con métodos específicos
    /// para búsqueda por nombre dentro de un inquilino (verificación de unicidad).
    /// </summary>
    public class CategoriaRepository : BaseRepository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(PigiPtDbContext context) : base(context)
        {
        }

        /// <inheritdoc/>
        public async Task<Categoria?> GetByNameAsync(Guid inquilinoId, string name)
        {
            return await _context.Categorias
                .FirstOrDefaultAsync(c => c.InquilinoId == inquilinoId
                                       && c.NombreCategoria == name);
        }
    }
}
