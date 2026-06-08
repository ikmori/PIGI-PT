namespace PIGI_PT_Domain.Base
{
    /// <summary>
    /// Clase base para todas las excepciones de dominio.
    /// 
    /// Las excepciones de dominio representan violaciones de invariantes y reglas de negocio.
    /// Se usan para comunicar errores de dominio de manera clara y específica.
    /// </summary>
    public abstract class DomainException : Exception
    {
        public string Code { get; protected set; }
        public DateTime OccurredAt { get; }

        protected DomainException(string message, string code = "DOMAIN_ERROR")
            : base(message)
        {
            Code = code;
            OccurredAt = DateTime.UtcNow;
        }
    }
}
