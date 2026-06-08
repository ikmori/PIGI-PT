using System.ComponentModel.DataAnnotations.Schema;

namespace PIGI_PT_Domain.Base
{
    /// <summary>
    /// Clase base abstracta para todas las entidades del dominio en el sistema PIGI-PT.
    /// Proporciona propiedades comunes para auditoría (creación y modificación), control de estado activo/inactivo
    /// y soporte para la gestión y despacho de eventos de dominio (Domain Events).
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Identificador único de la entidad.
        /// </summary>
        public Guid Id { get; protected set; }

        /// <summary>
        /// Fecha y hora en la que se creó la entidad (en formato UTC).
        /// </summary>
        public DateTime CreatedAt { get; protected set; }

        /// <summary>
        /// Identificador del usuario que creó la entidad.
        /// </summary>
        public Guid? CreatedBy { get; protected set; }

        /// <summary>
        /// Fecha y hora de la última modificación de la entidad (en formato UTC).
        /// </summary>
        public DateTime? ModifiedAt { get; protected set; }

        /// <summary>
        /// Identificador del usuario que realizó la última modificación.
        /// </summary>
        public Guid? ModifiedBy { get; protected set; }

        /// <summary>
        /// Indica si la entidad está activa en el sistema (utilizado para borrado lógico).
        /// </summary>
        public bool IsActive { get; protected set; }

        private readonly List<DomainEvent> _domainEvents = new();

        /// <summary>
        /// Colección de sólo lectura de los eventos de dominio generados por la entidad
        /// que están pendientes de ser despachados o publicados.
        /// </summary>
        [NotMapped]
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        /// <summary>
        /// Constructor vacío requerido para el mapeo del ORM (Entity Framework Core).
        /// </summary>
        protected BaseEntity()
        {
        }

        /// <summary>
        /// Constructor base que permite inicializar la entidad generando un nuevo identificador único.
        /// </summary>
        /// <param name="generateId">Si es verdadero, genera un nuevo Guid para Id, establece la fecha de creación en UTC y activa la entidad.</param>
        protected BaseEntity(bool generateId)
        {
            if (generateId)
            {
                Id = Guid.NewGuid();
                CreatedAt = DateTime.UtcNow;
                IsActive = true;
            }
        }

        /// <summary>
        /// Registra un nuevo evento de dominio en la entidad para su posterior despacho.
        /// </summary>
        /// <param name="domainEvent">El evento de dominio a registrar.</param>
        public void AddDomainEvent(DomainEvent domainEvent) => _domainEvents.Add(domainEvent);

        /// <summary>
        /// Limpia todos los eventos de dominio registrados y pendientes de despacho.
        /// </summary>
        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}