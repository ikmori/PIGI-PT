namespace PIGI_PT_Domain.Base
{
    /// <summary>
    /// Clase base abstracta para entidades del dominio que están aisladas por Inquilino (Multi-tenant).
    /// Asegura que cada entidad dependiente esté asociada obligatoriamente a un inquilino específico.
    /// </summary>
    public abstract class InquilinoEntity: BaseEntity
    {
        /// <summary>
        /// Identificador único del inquilino (Tenant) al que pertenece esta entidad.
        /// </summary>
        public Guid InquilinoId { get; protected set; }

        /// <summary>
        /// Constructor vacío requerido para el mapeo del ORM (Entity Framework Core).
        /// </summary>
        protected InquilinoEntity() : base()
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="InquilinoEntity"/> asociada a un inquilino específico y genera un nuevo ID de entidad.
        /// </summary>
        /// <param name="inquilinoId">Identificador único del inquilino propietario.</param>
        /// <exception cref="ArgumentException">Se lanza si el <paramref name="inquilinoId"/> está vacío.</exception>
        protected InquilinoEntity(Guid inquilinoId) : base(generateId: true)
        {
            if (inquilinoId == Guid.Empty)
                throw new ArgumentException("El identificador del inquilino es obligatorio.", nameof(inquilinoId));

            InquilinoId = inquilinoId;
        }
    }
}
