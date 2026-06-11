using PIGI_PT_Domain.Specifications;
using PIGI_PT_Domain.ValueObjects;

namespace PIGI_PT_Domain.Specifications.RiesgoOperacional
{
    /// <summary>
    /// Especificación para obtener los riesgos operacionales de un inquilino.
    /// Ordenados por nivel de impacto descendente (más críticos primero).
    /// </summary>
    public class RiesgosByInquilinoSpec : Specification<Aggregates.RiesgoOperacional.RiesgoOperacional>
    {
        public RiesgosByInquilinoSpec(Guid inquilinoId)
        {
            Criteria = r => r.InquilinoId == inquilinoId;
            OrderByDescending = r => r.NivelDeImpacto;
        }
    }

    /// <summary>
    /// Especificación para obtener riesgos operacionales que requieren revisión urgente
    /// (más de 90 días desde la última revisión).
    /// </summary>
    public class RiesgosRequierenRevisionSpec : Specification<Aggregates.RiesgoOperacional.RiesgoOperacional>
    {
        public RiesgosRequierenRevisionSpec(Guid inquilinoId)
        {
            var limitDate = DateTime.UtcNow.AddDays(-90);
            Criteria = r => r.InquilinoId == inquilinoId
                         && r.FechaUltimaRevision < limitDate;
            OrderBy = r => r.FechaUltimaRevision;
        }
    }
}
