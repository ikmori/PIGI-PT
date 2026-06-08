using PIGI_PT_Domain.Base;
using PIGI_PT_Domain.Events.Inquilino;
using PIGI_PT_Domain.Exceptions.Inquilino;
using PIGI_PT_Domain.ValueObjects;

namespace PIGI_PT_Domain.Aggregates.Inquilino
{
    /// <summary>
    /// Agregado Raíz de Inquilino.
    /// Representa un cliente o empresa que usa el sistema PIGI-PT.
    /// Gestiona:
    /// - Identidad del inquilino (nombre comercial, dominio)
    /// - Estado operacional (Activo/Suspendido)
    /// - Permisos de integración con IA respetando privacidad
    /// 
    /// Invariantes:
    /// - NombreComercial y DominioRed no pueden estar vacíos
    /// - El estado debe ser válido (Activo o Suspendido)
    /// - PermitirIA solo puede cambiar si el inquilino está activo
    /// - Las transiciones de estado deben ser válidas
    /// </summary>
    public class Inquilino : BaseEntity
    {
        public string NombreComercial { get; private set; }
        public string DominioRed { get; private set; }
        public bool PermitirIA { get; private set; }
        public EstadoInquilino Estado { get; private set; }

        private Inquilino() : base()
        {
        }

        /// <summary>
        /// Constructor para la creación de un nuevo inquilino.
        /// Por defecto: Estado=Activo, PermitirIA=false (por privacidad).
        /// Emite evento de creación.
        /// </summary>
        /// <param name="nombreComercial">Nombre comercial del inquilino (requerido)</param>
        /// <param name="dominioRed">Dominio de red del inquilino (requerido)</param>
        /// <param name="usuarioCreadorId">ID del usuario que crea el inquilino</param>
        /// <exception cref="ArgumentException">Si nombreComercial o dominioRed están vacíos</exception>
        /// <exception cref="ArgumentException">Si usuarioCreadorId es Guid.Empty</exception>
        public Inquilino(string nombreComercial, string dominioRed, Guid usuarioCreadorId) : base(generateId: true)
        {
            ValidarInvariantesConstruccion(nombreComercial, dominioRed, usuarioCreadorId);

            NombreComercial = nombreComercial;
            DominioRed = dominioRed;
            Estado = EstadoInquilino.Activo;
            PermitirIA = false; // Regla de negocio: privacidad por defecto
            CreatedBy = usuarioCreadorId;

            // Emitir evento de creación
            AddDomainEvent(new InquilinoRegistradoEvent(Id, nombreComercial, dominioRed));
        }

        /// <summary>
        /// Actualiza los permisos de integración con IA.
        /// Solo permite cambiar PermitirIA si el inquilino está activo.
        /// </summary>
        /// <param name="nuevoPermiso">Nuevo valor de PermitirIA</param>
        /// <param name="administradorId">ID del administrador que autoriza el cambio</param>
        /// <exception cref="InquilinoSuspendidoException">Si el inquilino está suspendido</exception>
        /// <exception cref="ArgumentException">Si administradorId es Guid.Empty</exception>
        public void ActualizarPermisosIA(bool nuevoPermiso, Guid administradorId)
        {
            if (administradorId == Guid.Empty)
                throw new ArgumentException("El identificador del administrador es obligatorio.", nameof(administradorId));

            if (Estado.EstaSuspendido)
                throw new InquilinoSuspendidoException(
                    Id,
                    "No es posible cambiar permisos de IA en un inquilino suspendido.");

            if (PermitirIA == nuevoPermiso)
                return; // No hay cambio, no emitir evento

            PermitirIA = nuevoPermiso;
            ModifiedAt = DateTime.UtcNow;
            ModifiedBy = administradorId;

            // Emitir evento enriquecido con contexto
            AddDomainEvent(new PrivacidadIAModificadaEvent(Id, nuevoPermiso, administradorId));
        }

        /// <summary>
        /// Suspende el servicio del inquilino.
        /// Transiciona el estado de Activo a Suspendido.
        /// </summary>
        /// <param name="administradorId">ID del administrador que suspende</param>
        /// <param name="motivo">Motivo de la suspensión (opcional)</param>
        /// <exception cref="InquilinoYaSuspendidoException">Si ya está suspendido</exception>
        /// <exception cref="ArgumentException">Si administradorId es Guid.Empty</exception>
        public void SuspenderServicio(Guid administradorId, string motivo = "Sin especificar")
        {
            if (administradorId == Guid.Empty)
                throw new ArgumentException("El identificador del administrador es obligatorio.", nameof(administradorId));

            if (Estado.EstaSuspendido)
                throw new InquilinoYaSuspendidoException(Id, "El inquilino ya está suspendido.");

            if (!Estado.PuedeTransicionarA(EstadoInquilino.Suspendido))
                throw new InquilinoInvalidStateTransitionException(
                    Id,
                    Estado.Valor,
                    EstadoInquilino.Suspendido.Valor,
                    "Transición de estado no permitida.");

            Estado = EstadoInquilino.Suspendido;
            ModifiedAt = DateTime.UtcNow;
            ModifiedBy = administradorId;

            // Emitir evento de suspensión
            AddDomainEvent(new InquilinoSuspendidoEvent(Id, administradorId, motivo));
        }

        /// <summary>
        /// Reactiva el servicio del inquilino.
        /// Transiciona el estado de Suspendido a Activo.
        /// </summary>
        /// <param name="administradorId">ID del administrador que reactiva</param>
        /// <param name="motivo">Motivo de la reactivación (opcional)</param>
        /// <exception cref="InquilinoNoEstaActivoException">Si no está suspendido</exception>
        /// <exception cref="ArgumentException">Si administradorId es Guid.Empty</exception>
        public void ReactivarServicio(Guid administradorId, string motivo = "Sin especificar")
        {
            if (administradorId == Guid.Empty)
                throw new ArgumentException("El identificador del administrador es obligatorio.", nameof(administradorId));

            if (Estado.EstaActivo)
                throw new InquilinoNoEstaActivoException(Id, "El inquilino ya está activo.");

            if (!Estado.PuedeTransicionarA(EstadoInquilino.Activo))
                throw new InquilinoInvalidStateTransitionException(
                    Id,
                    Estado.Valor,
                    EstadoInquilino.Activo.Valor,
                    "Transición de estado no permitida.");

            Estado = EstadoInquilino.Activo;
            ModifiedAt = DateTime.UtcNow;
            ModifiedBy = administradorId;

            // Emitir evento de reactivación
            AddDomainEvent(new InquilinoReactivadoEvent(Id, administradorId, motivo));
        }

        /// <summary>
        /// Actualiza el nombre comercial del inquilino.
        /// </summary>
        public void ActualizarNombreComercial(string nuevoNombre, Guid administradorId)
        {
            if (administradorId == Guid.Empty)
                throw new ArgumentException("El identificador del administrador es obligatorio.", nameof(administradorId));

            if (string.IsNullOrWhiteSpace(nuevoNombre))
                throw new ArgumentException("El nombre comercial no puede estar vacío.", nameof(nuevoNombre));

            if (NombreComercial == nuevoNombre)
                return; // No hay cambio

            NombreComercial = nuevoNombre;
            ModifiedAt = DateTime.UtcNow;
            ModifiedBy = administradorId;
        }

        /// <summary>
        /// Actualiza el dominio de red del inquilino.
        /// </summary>
        public void ActualizarDominioRed(string nuevoDominio, Guid administradorId)
        {
            if (administradorId == Guid.Empty)
                throw new ArgumentException("El identificador del administrador es obligatorio.", nameof(administradorId));

            if (string.IsNullOrWhiteSpace(nuevoDominio))
                throw new ArgumentException("El dominio de red no puede estar vacío.", nameof(nuevoDominio));

            if (DominioRed == nuevoDominio)
                return; // No hay cambio

            DominioRed = nuevoDominio;
            ModifiedAt = DateTime.UtcNow;
            ModifiedBy = administradorId;
        }

        /// <summary>
        /// Método privado para validar invariantes en la construcción.
        /// </summary>
        private static void ValidarInvariantesConstruccion(string nombreComercial, string dominioRed, Guid usuarioCreadorId)
        {
            if (string.IsNullOrWhiteSpace(nombreComercial))
                throw new ArgumentException("El nombre comercial es obligatorio.", nameof(nombreComercial));

            if (string.IsNullOrWhiteSpace(dominioRed))
                throw new ArgumentException("El dominio de red es obligatorio.", nameof(dominioRed));

            if (usuarioCreadorId == Guid.Empty)
                throw new ArgumentException("El identificador del usuario creador es obligatorio.", nameof(usuarioCreadorId));
        }
    }
}