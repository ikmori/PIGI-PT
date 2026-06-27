using System;

namespace PIGI_PT_Application.DTOs.Categoria
{
    public class CategoriaDto
    {
        public Guid Id { get; set; }
        public Guid InquilinoId { get; set; }
        public string NombreCategoria { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
