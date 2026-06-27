using PIGI_PT_Application.DTOs.RiesgoOperacional;

namespace PIGI_PT_Application.Commands.RiesgoOperacional
{
    public static class RiesgoOperacionalMapper
    {
        public static RiesgoOperacionalDto ToDto(PIGI_PT_Domain.Aggregates.RiesgoOperacional.RiesgoOperacional riesgo)
        {
            return new RiesgoOperacionalDto
            {
                Id = riesgo.Id,
                InquilinoId = riesgo.InquilinoId,
                ServicioAfectado = riesgo.ServicioAfectado,
                DescripcionAmenaza = riesgo.DescripcionAmenaza,
                NivelDeImpacto = riesgo.NivelDeImpacto.Nombre,
                NivelDeImpactoValor = riesgo.NivelDeImpacto.Valor,
                PlanDeMitigacion = riesgo.PlanDeMitigacion,
                FechaUltimaRevision = riesgo.FechaUltimaRevision,
                RequiereRevisionUrgente = riesgo.RequiereRevisionUrgente(),
                DiasDesdeUltimaRevision = riesgo.ObtenerDiasDesdeUltimaRevision(),
                CreatedBy = riesgo.CreatedBy,
                CreatedAt = riesgo.CreatedAt,
                ModifiedBy = riesgo.ModifiedBy,
                ModifiedAt = riesgo.ModifiedAt
            };
        }
    }
}
