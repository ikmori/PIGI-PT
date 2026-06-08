using PIGI_PT_Domain.Base;
using PIGI_PT_Domain.Exceptions.RiesgoOperacional;
using PIGI_PT_Domain.ValueObjects;

namespace PIGI_PT_Domain.Entities
{
    /// <summary>
    /// Agregado Raíz de RiesgoOperacional.
    /// Representa un riesgo o amenaza identificada en la operación de un inquilino.
    /// Gestiona:
    /// - Identificación del riesgo (servicio, amenaza)
    /// - Nivel de impacto (prioridad) del riesgo
    /// - Plan de mitigación
    /// - Auditoría de revisiones
    /// 
    /// Invariantes:
    /// - ServicioAfectado, DescripcionAmenaza, PlanDeMitigacion no pueden estar vacíos
    /// - NivelDeImpacto debe ser válido (usa ValueObject)
    /// - FechaUltimaRevision se actualiza con cada revisión
    /// </summary>
    public class RiesgoOperacional : InquilinoEntity
    {
        public string ServicioAfectado { get; private set; }
        public string DescripcionAmenaza { get; private set; }
        public NivelPrioridad NivelDeImpacto { get; private set; }
        public string PlanDeMitigacion { get; private set; }
        public DateTime FechaUltimaRevision { get; private set; }

        private RiesgoOperacional() : base()
        {
        }

        /// <summary>
        /// Constructor para crear un nuevo riesgo operacional.
        /// </summary>
        /// <param name="inquilinoId">ID del inquilino propietario</param>
        /// <param name="servicioAfectado">Nombre del servicio afectado (requerido)</param>
        /// <param name="descripcionAmenaza">Descripción de la amenaza (requerido)</param>
        /// <param name="nivelDeImpacto">Nivel de impacto (requerido)</param>
        /// <param name="planDeMitigacion">Plan para mitigar el riesgo (requerido)</param>
        /// <param name="userId">ID del usuario creador</param>
        /// <exception cref="ArgumentException">Si algún campo requerido está vacío</exception>
        /// <exception cref="ArgumentException">Si userId es Guid.Empty</exception>
        public RiesgoOperacional(Guid inquilinoId, string servicioAfectado, string descripcionAmenaza,
            NivelPrioridad nivelDeImpacto, string planDeMitigacion, Guid userId) : base(inquilinoId)
        {
            ValidarInvariantesConstruccion(servicioAfectado, descripcionAmenaza, nivelDeImpacto, planDeMitigacion, userId);

            ServicioAfectado = servicioAfectado;
            DescripcionAmenaza = descripcionAmenaza;
            NivelDeImpacto = nivelDeImpacto;
            PlanDeMitigacion = planDeMitigacion;
            FechaUltimaRevision = DateTime.UtcNow;
            CreatedBy = userId;
        }

        /// <summary>
        /// Actualiza el plan de mitigación del riesgo.
        /// </summary>
        /// <param name="nuevoPlan">Nuevo plan de mitigación (requerido)</param>
        /// <param name="userId">ID del usuario que actualiza</param>
        /// <exception cref="ArgumentException">Si nuevoPlan está vacío o userId es Guid.Empty</exception>
        public void ActualizarPlanDeMitigacion(string nuevoPlan, Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("El identificador del usuario modificador es obligatorio.", nameof(userId));

            if (string.IsNullOrWhiteSpace(nuevoPlan))
                throw new ArgumentException("El nuevo plan de mitigación no puede estar vacío.", nameof(nuevoPlan));

            PlanDeMitigacion = nuevoPlan;
            FechaUltimaRevision = DateTime.UtcNow;
            ModifiedBy = userId;
            ModifiedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Reevalúa el nivel de impacto del riesgo.
        /// </summary>
        /// <param name="nuevoImpacto">Nuevo nivel de impacto (requerido)</param>
        /// <param name="userId">ID del usuario que realiza la reevaluación</param>
        /// <exception cref="ArgumentNullException">Si nuevoImpacto es nulo</exception>
        /// <exception cref="ArgumentException">Si userId es Guid.Empty</exception>
        /// <exception cref="RiesgoOperacionalMismoNivelImpactoException">Si el nuevo nivel es igual al actual</exception>
        public void ReevaluarImpacto(NivelPrioridad nuevoImpacto, Guid userId)
        {
            if (nuevoImpacto == null)
                throw new ArgumentNullException(nameof(nuevoImpacto), "El nivel de impacto es obligatorio.");

            if (userId == Guid.Empty)
                throw new ArgumentException("El identificador del usuario modificador es obligatorio.", nameof(userId));

            if (NivelDeImpacto == nuevoImpacto)
                throw new RiesgoOperacionalMismoNivelImpactoException(
                    Id,
                    "El nuevo nivel de impacto es igual al actual.");

            NivelDeImpacto = nuevoImpacto;
            FechaUltimaRevision = DateTime.UtcNow;
            ModifiedBy = userId;
            ModifiedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Registra una revisión del riesgo operacional.
        /// Actualiza la fecha de última revisión sin cambiar otros datos.
        /// </summary>
        /// <param name="userId">ID del usuario auditor</param>
        /// <exception cref="ArgumentException">Si userId es Guid.Empty</exception>
        public void RegistrarRevision(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("El identificador del usuario auditor es obligatorio.", nameof(userId));

            FechaUltimaRevision = DateTime.UtcNow;
            ModifiedBy = userId;
            ModifiedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Obtiene los días transcurridos desde la última revisión.
        /// </summary>
        public int ObtenerDiasDesdeUltimaRevision()
        {
            return (int)DateTime.UtcNow.Subtract(FechaUltimaRevision).TotalDays;
        }

        /// <summary>
        /// Verifica si el riesgo requiere revisión urgente (más de 90 días sin revisar).
        /// </summary>
        public bool RequiereRevisionUrgente()
        {
            return ObtenerDiasDesdeUltimaRevision() > 90;
        }

        /// <summary>
        /// Método privado para validar invariantes en la construcción.
        /// </summary>
        private static void ValidarInvariantesConstruccion(string servicioAfectado, string descripcionAmenaza,
            NivelPrioridad nivelDeImpacto, string planDeMitigacion, Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("El identificador del usuario es obligatorio.", nameof(userId));

            if (string.IsNullOrWhiteSpace(servicioAfectado))
                throw new ArgumentException("El servicio afectado es obligatorio.", nameof(servicioAfectado));

            if (string.IsNullOrWhiteSpace(descripcionAmenaza))
                throw new ArgumentException("La descripción de la amenaza es obligatoria.", nameof(descripcionAmenaza));

            if (nivelDeImpacto == null)
                throw new ArgumentNullException(nameof(nivelDeImpacto), "El nivel de impacto es obligatorio.");

            if (string.IsNullOrWhiteSpace(planDeMitigacion))
                throw new ArgumentException("El plan de mitigación es obligatorio.", nameof(planDeMitigacion));
        }
    }
}
