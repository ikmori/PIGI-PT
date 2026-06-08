namespace PIGI_PT_Application.Ports.Services
{
    /// <summary>
    /// Puerto que abstrae el acceso al reloj del sistema para permitir la inyección de dependencias
    /// y mejorar la testabilidad del código al manejar fechas y horas (DateTime).
    /// </summary>
    public interface IDateTimeProvider
    {
        /// <summary>
        /// Obtiene la fecha y hora actuales del sistema expresadas en formato UTC (Tiempo Universal Coordinado).
        /// </summary>
        DateTime UtcNow { get; }
    }
}
