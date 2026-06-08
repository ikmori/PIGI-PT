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
        /// <summary>
        /// Código de error único que identifica la naturaleza específica de la excepción de dominio.
        /// Útil para la internacionalización de mensajes o mapeos en la capa de presentación.
        /// </summary>
        public string Code { get; protected set; }

        /// <summary>
        /// Fecha y hora en la que ocurrió el error de dominio (en formato UTC).
        /// </summary>
        public DateTime OccurredAt { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="DomainException"/> con un mensaje y un código de error específico.
        /// </summary>
        /// <param name="message">El mensaje descriptivo del error de negocio.</param>
        /// <param name="code">El código de error único asociado al problema. Por defecto es "DOMAIN_ERROR".</param>
        protected DomainException(string message, string code = "DOMAIN_ERROR")
            : base(message)
        {
            Code = code;
            OccurredAt = DateTime.UtcNow;
        }
    }
}
