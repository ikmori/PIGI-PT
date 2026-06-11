using PIGI_PT_Application.Ports.Repositories;
using PIGI_PT_Domain.Aggregates.RiesgoOperacional;
using PIGI_PT_Infraestructure.Persistence.DbContext;

namespace PIGI_PT_Infraestructure.Persistence.Repositories
{
    /// <summary>
    /// Implementación concreta del repositorio de <see cref="RiesgoOperacional"/>.
    /// Extiende <see cref="BaseRepository{RiesgoOperacional}"/> con la funcionalidad
    /// base del repositorio genérico. Métodos específicos pueden agregarse según necesidad.
    /// </summary>
    public class RiesgoOperacionalRepository : BaseRepository<RiesgoOperacional>, IRiesgoOperacionalRepository
    {
        public RiesgoOperacionalRepository(PigiPtDbContext context) : base(context)
        {
        }
    }
}
