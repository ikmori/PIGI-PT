using System;

namespace PIGI_PT_Application.DTOs.Inquilino
{
    public class InquilinoDto
    {
        public Guid Id { get; set; }
        public string NombreComercial { get; set; } = string.Empty;
        public string DominioRed { get; set; } = string.Empty;
        public bool PermitirIA { get; set; }
        public string Estado { get; set; } = string.Empty;
        public int EstadoValor { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
