using MediatR;

namespace PIGI_PT_Domain.Base
{
    /// <summary>
    /// Clase base abstracta para todos los Eventos de Dominio en el sistema PIGI-PT.
    /// Implementa <see cref="INotification"/> de MediatR para permitir su publicación
    /// y manejo desacoplado dentro de la capa de aplicación.
    /// </summary>
    public abstract class DomainEvent: INotification
    {
        /// <summary>
        /// Indica si el evento de dominio ya ha sido publicado o despachado al bus de eventos.
        /// </summary>
        public bool IsPublished { get; set; }

        /// <summary>
        /// Fecha y hora en la que ocurrió el evento (en formato UTC).
        /// </summary>
        public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
    }
}
