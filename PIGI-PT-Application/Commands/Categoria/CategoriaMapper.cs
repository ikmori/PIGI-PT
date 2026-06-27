using PIGI_PT_Application.DTOs.Categoria;

namespace PIGI_PT_Application.Commands.Categoria
{
    public static class CategoriaMapper
    {
        public static CategoriaDto ToDto(PIGI_PT_Domain.Aggregates.Categoria.Categoria categoria)
        {
            return new CategoriaDto
            {
                Id = categoria.Id,
                InquilinoId = categoria.InquilinoId,
                NombreCategoria = categoria.NombreCategoria,
                Descripcion = categoria.Descripcion,
                IsActive = categoria.IsActive,
                CreatedBy = categoria.CreatedBy,
                CreatedAt = categoria.CreatedAt,
                ModifiedBy = categoria.ModifiedBy,
                ModifiedAt = categoria.ModifiedAt
            };
        }
    }
}
