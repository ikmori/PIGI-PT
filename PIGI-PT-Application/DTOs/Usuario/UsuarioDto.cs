using System;

namespace PIGI_PT_Application.DTOs.Usuario
{
    public class UsuarioDto
    {
        public Guid Id { get; set; }
        public Guid InquilinoId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string RolDescripcion { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
