using PIGI_PT_Application.DTOs.Inquilino;

namespace PIGI_PT_Application.Commands.Inquilino
{
    public static class InquilinoMapper
    {
        public static InquilinoDto ToDto(PIGI_PT_Domain.Aggregates.Inquilino.Inquilino inquilino)
        {
            return new InquilinoDto
            {
                Id = inquilino.Id,
                NombreComercial = inquilino.NombreComercial,
                DominioRed = inquilino.DominioRed,
                PermitirIA = inquilino.PermitirIA,
                Estado = inquilino.Estado.Valor,
                EstadoValor = inquilino.Estado.EstaActivo ? 0 : 1,
                CreatedBy = inquilino.CreatedBy,
                CreatedAt = inquilino.CreatedAt,
                ModifiedBy = inquilino.ModifiedBy,
                ModifiedAt = inquilino.ModifiedAt
            };
        }
    }
}
