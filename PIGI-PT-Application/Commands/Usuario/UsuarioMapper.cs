using PIGI_PT_Application.DTOs.Usuario;

namespace PIGI_PT_Application.Commands.Usuario
{
    public static class UsuarioMapper
    {
        public static UsuarioDto ToDto(PIGI_PT_Domain.Aggregates.Usuario.Usuario usuario)
        {
            return new UsuarioDto
            {
                Id = usuario.Id,
                InquilinoId = usuario.InquilinoId,
                FullName = usuario.FullName,
                Email = usuario.Email,
                UserName = usuario.UserName,
                Rol = usuario.Rol.Nombre,
                RolDescripcion = string.Empty,
                IsActive = usuario.IsActive,
                CreatedBy = usuario.CreatedBy,
                CreatedAt = usuario.CreatedAt,
                ModifiedBy = usuario.ModifiedBy,
                ModifiedAt = usuario.ModifiedAt
            };
        }
    }
}
