using PIGI_PT_Domain.Base;

namespace PIGI_PT_Domain.Exceptions.RiesgoOperacional
{
    /// <summary>
    /// Base class para excepciones de dominio específicas del Agregado RiesgoOperacional.
    /// </summary>
    public abstract class RiesgoOperacionalDomainException : DomainException
    {
        public Guid RiesgoOperacionalId { get; }

        protected RiesgoOperacionalDomainException(Guid riesgoId, string message, string errorCode)
            : base(message, errorCode)
        {
            RiesgoOperacionalId = riesgoId;
        }
    }
}
