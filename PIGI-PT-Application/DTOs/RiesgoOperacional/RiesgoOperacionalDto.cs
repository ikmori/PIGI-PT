using System;

namespace PIGI_PT_Application.DTOs.RiesgoOperacional
{
    public class RiesgoOperacionalDto
    {
        public Guid Id { get; set; }
        public Guid InquilinoId { get; set; }
        public string ServicioAfectado { get; set; } = string.Empty;
        public string DescripcionAmenaza { get; set; } = string.Empty;
        public string NivelDeImpacto { get; set; } = string.Empty;
        public int NivelDeImpactoValor { get; set; }
        public string PlanDeMitigacion { get; set; } = string.Empty;
        public DateTime FechaUltimaRevision { get; set; }
        public bool RequiereRevisionUrgente { get; set; }
        public int DiasDesdeUltimaRevision { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
