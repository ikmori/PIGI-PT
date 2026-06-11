using PIGI_PT_Domain.Specifications;

namespace PIGI_PT_Domain.Specifications.Usuario
{
    /// <summary>
    /// Especificación para obtener un usuario por su email.
    /// </summary>
    public class UsuarioByEmailSpec : Specification<Aggregates.Usuario.Usuario>
    {
        public UsuarioByEmailSpec(string email)
        {
            Criteria = u => u.Email == email;
        }
    }

    /// <summary>
    /// Especificación para obtener un usuario por su nombre de usuario.
    /// </summary>
    public class UsuarioByUserNameSpec : Specification<Aggregates.Usuario.Usuario>
    {
        public UsuarioByUserNameSpec(string userName)
        {
            Criteria = u => u.UserName == userName;
        }
    }

    /// <summary>
    /// Especificación para obtener todos los usuarios de un inquilino.
    /// </summary>
    public class UsuariosByInquilinoSpec : Specification<Aggregates.Usuario.Usuario>
    {
        public UsuariosByInquilinoSpec(Guid inquilinoId)
        {
            Criteria = u => u.InquilinoId == inquilinoId;
            OrderBy = u => u.FullName;
        }
    }

    /// <summary>
    /// Especificación para obtener usuarios activos de un inquilino.
    /// </summary>
    public class ActiveUsuariosByInquilinoSpec : Specification<Aggregates.Usuario.Usuario>
    {
        public ActiveUsuariosByInquilinoSpec(Guid inquilinoId)
        {
            Criteria = u => u.InquilinoId == inquilinoId && u.IsActive;
            OrderBy = u => u.FullName;
        }
    }
}
