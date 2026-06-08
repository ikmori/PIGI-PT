using PIGI_PT_Domain.Base;

namespace PIGI_PT_Domain.Exceptions.Inquilino
{
    /// <summary>
    /// Base class para excepciones de dominio específicas del Agregado Inquilino.
    /// </summary>
    public abstract class InquilinoDomainException : DomainException
    {
        public Guid InquilinoId { get; }

        protected InquilinoDomainException(Guid inquilinoId, string message, string errorCode)
            : base(message, errorCode)
        {
            InquilinoId = inquilinoId;
        }
    }
}
