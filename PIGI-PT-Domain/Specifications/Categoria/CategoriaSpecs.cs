using PIGI_PT_Domain.Specifications;

namespace PIGI_PT_Domain.Specifications.Categoria
{
    /// <summary>
    /// Especificación para obtener las categorías de un inquilino,
    /// ordenadas alfabéticamente por nombre.
    /// </summary>
    public class CategoriasByInquilinoSpec : Specification<Aggregates.Categoria.Categoria>
    {
        public CategoriasByInquilinoSpec(Guid inquilinoId)
        {
            Criteria = c => c.InquilinoId == inquilinoId;
            OrderBy = c => c.NombreCategoria;
        }
    }

    /// <summary>
    /// Especificación para obtener una categoría por nombre dentro de un inquilino.
    /// Utilizado para verificar unicidad de nombres.
    /// </summary>
    public class CategoriaByNombreSpec : Specification<Aggregates.Categoria.Categoria>
    {
        public CategoriaByNombreSpec(Guid inquilinoId, string nombre)
        {
            Criteria = c => c.InquilinoId == inquilinoId
                         && c.NombreCategoria == nombre;
        }
    }
}
