using PIGI_PT_Domain.Specifications;

namespace PIGI_PT_Domain.Specifications.Inquilino
{
    /// <summary>
    /// Especificación para obtener un inquilino por su dominio de red.
    /// Utilizado para la resolución de multi-tenant durante las peticiones HTTP.
    /// </summary>
    public class InquilinoByDominioSpec : Specification<Aggregates.Inquilino.Inquilino>
    {
        public InquilinoByDominioSpec(string dominio)
        {
            Criteria = i => i.DominioRed == dominio;
        }
    }

    /// <summary>
    /// Especificación para obtener todos los inquilinos activos.
    /// </summary>
    public class ActiveInquilinosSpec : Specification<Aggregates.Inquilino.Inquilino>
    {
        public ActiveInquilinosSpec()
        {
            Criteria = i => i.IsActive;
            OrderBy = i => i.NombreComercial;
        }
    }
}
