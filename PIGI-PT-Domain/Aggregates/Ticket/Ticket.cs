using PIGI_PT_Domain.Base;
using PIGI_PT_Domain.Events.Ticket;
using PIGI_PT_Domain.Exceptions.Ticket;
using PIGI_PT_Domain.ValueObjects;

namespace PIGI_PT_Domain.Aggregates.Ticket
{
    /// <summary>
    /// Agregado Raíz (Aggregate Root): Ticket
    /// 
    /// Representa un incidente o solicitud reportado en el sistema.
    /// Encapsula todo el ciclo de vida del ticket desde su creación hasta su resolución.
    /// 
    /// Responsabilidades:
    /// - Validar invariantes de estado
    /// - Gestionar transiciones de estado válidas
    /// - Emitir eventos de dominio cuando suceden cambios importantes
    /// - Mantener coherencia de datos
    /// </summary>
    public class Ticket : InquilinoEntity
    {
        public string Titulo { get; private set; }
        public string DescripcionOriginal { get; private set; }
        public string? DescripcionSanitizada { get; private set; }
        public EstadoTicket Estado { get; private set; }
        public NivelPrioridad Prioridad { get; private set; }
        public Guid? CategoriaId { get; private set; }
        public Guid? ResponsableTecnologiaId { get; private set; }
        public DateTime? FechaResolucion { get; private set; }
        public DateTime? FechaAsignacion { get; private set; }

        /// <summary>
        /// Constructor privado requerido para la materialización de Entity Framework Core.
        /// </summary>
        private Ticket() : base()
        {
        }

        /// <summary>
        /// Constructor para la creación de nuevos tickets.
        /// 
        /// Invariantes:
        /// - El inquilino debe ser válido (no Guid.Empty)
        /// - El título no puede estar vacío
        /// - La descripción original no puede estar vacía
        /// - El userId debe ser válido (el usuario solicitante)
        /// </summary>
        /// <param name="inquilinoId">ID del inquilino propietario del ticket</param>
        /// <param name="titulo">Título del incidente (máx 200 caracteres)</param>
        /// <param name="descripcionOriginal">Descripción del incidente (máx 5000 caracteres)</param>
        /// <param name="userId">ID del usuario que crea el ticket</param>
        /// <exception cref="ArgumentException">Si algún parámetro es inválido</exception>
        public Ticket(Guid inquilinoId, string titulo, string descripcionOriginal, Guid userId)
            : base(inquilinoId)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                throw new ArgumentException("El título del incidente es obligatorio.", nameof(titulo));

            if (titulo.Length > 200)
                throw new ArgumentException("El título no puede exceder 200 caracteres.", nameof(titulo));

            if (string.IsNullOrWhiteSpace(descripcionOriginal))
                throw new ArgumentException("La descripción original es obligatoria.", nameof(descripcionOriginal));

            if (descripcionOriginal.Length > 5000)
                throw new ArgumentException("La descripción no puede exceder 5000 caracteres.", nameof(descripcionOriginal));

            if (userId == Guid.Empty)
                throw new ArgumentException("El identificador del usuario solicitante es obligatorio.", nameof(userId));

            Titulo = titulo;
            DescripcionOriginal = descripcionOriginal;
            Estado = EstadoTicket.PendienteDeAnalisis;
            Prioridad = NivelPrioridad.NoDefinida;
            CreatedBy = userId;

            // Emitir evento de dominio para que la aplicación inicie el procesamiento asíncrono
            AddDomainEvent(new TicketCreadoEvent(Id, titulo, descripcionOriginal, userId));
        }

        /// <summary>
        /// Aplica la sanitización a la descripción del ticket.
        /// 
        /// La sanitización es el proceso de enmascarar datos sensibles (IPs, credenciales, PII).
        /// Se aplica después de que el servicio en Python procesa la descripción original.
        /// </summary>
        /// <param name="descripcionSanitizada">Descripción con datos sensibles enmascarados</param>
        /// <exception cref="ArgumentException">Si la descripción sanitizada está vacía</exception>
        /// <exception cref="InvalidOperationException">Si el ticket no está en estado pendiente de análisis</exception>
        public void AplicarSanitizacion(string descripcionSanitizada)
        {
            if (string.IsNullOrWhiteSpace(descripcionSanitizada))
                throw new ArgumentException("La descripción sanitizada no puede estar vacía.", nameof(descripcionSanitizada));

            if (descripcionSanitizada.Length > 5000)
                throw new ArgumentException("La descripción sanitizada no puede exceder 5000 caracteres.", nameof(descripcionSanitizada));

            if (!Estado.EstaEnAnalisis)
                throw new InvalidOperationException("Solo se puede aplicar sanitización a tickets pendientes de análisis.");

            DescripcionSanitizada = descripcionSanitizada;
            ModifiedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Clasifica el ticket utilizando análisis de inteligencia artificial.
        /// 
        /// Asigna una prioridad y categoría basado en el análisis de la descripción sanitizada.
        /// Transiciona el estado a "Clasificado".
        /// 
        /// Invariante: Solo se puede clasificar un ticket en estado "PendienteDeAnalisis"
        /// </summary>
        /// <param name="prioridad">Nivel de prioridad asignado por IA</param>
        /// <param name="categoriaId">ID de la categoría asignada</param>
        /// <exception cref="ArgumentException">Si prioridad o categoriaId son inválidos</exception>
        /// <exception cref="TicketInvalidStateTransitionException">Si el ticket no puede transicionar a Clasificado</exception>
        public void ClasificarPorIA(NivelPrioridad prioridad, Guid categoriaId)
        {
            if (prioridad == null)
                throw new ArgumentNullException(nameof(prioridad));

            if (!prioridad.EstaDefinida)
                throw new ArgumentException("La prioridad proporcionada debe estar definida.", nameof(prioridad));

            if (categoriaId == Guid.Empty)
                throw new ArgumentException("El identificador de la categoría es obligatorio.", nameof(categoriaId));

            if (!Estado.PuedeTransicionarA(EstadoTicket.Clasificado))
                throw new TicketInvalidStateTransitionException(Id, Estado, EstadoTicket.Clasificado);

            Prioridad = prioridad;
            CategoriaId = categoriaId;
            Estado = EstadoTicket.Clasificado;
            ModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new TicketClasificadoEvent(Id, prioridad, categoriaId));
        }

        /// <summary>
        /// Asigna un operador al ticket para que lo trabaje.
        /// 
        /// Transiciona el estado a "EnProgreso".
        /// 
        /// Invariante: No se puede asignar operador a un ticket resuelto.
        /// </summary>
        /// <param name="operadorId">ID del operador a asignar</param>
        /// <param name="userId">ID del usuario que realiza la asignación</param>
        /// <exception cref="ArgumentException">Si los IDs son inválidos</exception>
        /// <exception cref="TicketInvalidStateTransitionException">Si no puede transicionar a EnProgreso</exception>
        public void AsignarResponsable(Guid responsableId, Guid userId)
        {
            if (responsableId == Guid.Empty)
                throw new ArgumentException("El identificador del responsable es obligatorio.", nameof(responsableId));

            if (userId == Guid.Empty)
                throw new ArgumentException("El identificador del usuario que realiza la asignación es obligatorio.", nameof(userId));

            if (!Estado.PuedeTransicionarA(EstadoTicket.EnProgreso))
                throw new TicketInvalidStateTransitionException(Id, Estado, EstadoTicket.EnProgreso);

            ResponsableTecnologiaId = responsableId;
            FechaAsignacion = DateTime.UtcNow;
            Estado = EstadoTicket.EnProgreso;
            ModifiedBy = userId;
            ModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new ResponsableAsignadoEvent(Id, responsableId));
        }

        /// <summary>
        /// Marca el ticket como resuelto.
        /// 
        /// Transiciona el estado a "Resuelto" y registra la fecha de resolución.
        /// 
        /// Invariantes:
        /// - El ticket no puede estar ya resuelto
        /// - El ticket debe tener un operador asignado
        /// - El ticket debe poder transicionar a estado Resuelto
        /// </summary>
        /// <param name="userId">ID del usuario que resuelve el ticket</param>
        /// <exception cref="ArgumentException">Si userId es inválido</exception>
        /// <exception cref="TicketAlreadyResolvedException">Si el ticket ya está resuelto</exception>
        /// <exception cref="TicketMissingOperatorException">Si no hay operador asignado</exception>
        /// <exception cref="TicketInvalidStateTransitionException">Si no puede transicionar a Resuelto</exception>
        public void Resolver(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("El identificador del usuario que resuelve es obligatorio.", nameof(userId));

            if (Estado.EstaResuelto)
                throw new TicketAlreadyResolvedException(Id);

            if (ResponsableTecnologiaId == null || ResponsableTecnologiaId == Guid.Empty)
                throw new TicketMissingOperatorException(Id);

            if (!Estado.PuedeTransicionarA(EstadoTicket.Resuelto))
                throw new TicketInvalidStateTransitionException(Id, Estado, EstadoTicket.Resuelto);

            Estado = EstadoTicket.Resuelto;
            FechaResolucion = DateTime.UtcNow;
            ModifiedBy = userId;
            ModifiedAt = DateTime.UtcNow;

            // Emitir evento para notificar al creador del ticket
            AddDomainEvent(new TicketResueltoEvent(
                Id, 
                CreatedBy ?? Guid.Empty, 
                DateTime.UtcNow,
                CalcularTiempoDeResolucion()
            ));
        }

        /// <summary>
        /// Cancela un ticket que está en progreso.
        /// </summary>
        /// <param name="userId">ID del usuario que cancela</param>
        /// <exception cref="InvalidOperationException">Si no puede transicionar a Cancelado</exception>
        public void Cancelar(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("El identificador del usuario es obligatorio.", nameof(userId));

            if (!Estado.PuedeTransicionarA(EstadoTicket.Cancelado))
                throw new InvalidOperationException($"No se puede cancelar un ticket en estado {Estado.Nombre}.");

            Estado = EstadoTicket.Cancelado;
            ModifiedBy = userId;
            ModifiedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Rechaza un ticket que está pendiente o clasificado.
        /// </summary>
        /// <param name="userId">ID del usuario que rechaza</param>
        /// <param name="motivo">Motivo del rechazo (para auditoría)</param>
        public void Rechazar(Guid userId, string motivo = "")
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("El identificador del usuario es obligatorio.", nameof(userId));

            if (!Estado.PuedeTransicionarA(EstadoTicket.Rechazado))
                throw new InvalidOperationException($"No se puede rechazar un ticket en estado {Estado.Nombre}.");

            Estado = EstadoTicket.Rechazado;
            ModifiedBy = userId;
            ModifiedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Calcula el tiempo total de resolución desde creación hasta resolución.
        /// </summary>
        private TimeSpan CalcularTiempoDeResolucion()
        {
            if (!FechaResolucion.HasValue)
                return TimeSpan.Zero;

            return FechaResolucion.Value - CreatedAt;
        }

        /// <summary>
        /// Obtiene el tiempo total de resolución.
        /// </summary>
        public TimeSpan? ObtenerTiempoDeResolucion()
        {
            if (!FechaResolucion.HasValue)
                return null;

            return FechaResolucion.Value - CreatedAt;
        }

        /// <summary>
        /// Verifica si el ticket está atrasado (tiempo de resolución anormalmente alto).
        /// </summary>
        public bool EstaAtrasado => FechaAsignacion.HasValue && 
                                    DateTime.UtcNow.Subtract(FechaAsignacion.Value).TotalHours > 24;

        /// <summary>
        /// Verifica si el ticket requiere escalada (crítico y no resuelto).
        /// </summary>
        public bool RequiereEscalada => Prioridad.EsCritica && !Estado.EstaResuelto;
    }
}
