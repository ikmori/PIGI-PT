using PIGI_PT_Domain.Base;

namespace PIGI_PT_Domain.Exceptions.Usuario
{
    /// <summary>
    /// Base class para excepciones de dominio específicas del Agregado Usuario.
    /// </summary>
    public abstract class UsuarioDomainException : DomainException
    {
        public Guid UsuarioId { get; }

        protected UsuarioDomainException(Guid usuarioId, string message, string errorCode)
            : base(message, errorCode)
        {
            UsuarioId = usuarioId;
        }
    }
}
