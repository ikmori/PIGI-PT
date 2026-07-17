using System;
using System.Collections.Generic;

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
        public int RolValor { get; set; }
        public string RolDescripcion { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }

        /// <summary>
        /// Categorías asignadas al operador (vacío para otros roles).
        /// </summary>
        public Guid? DepartamentoId { get; set; }
    }
}

