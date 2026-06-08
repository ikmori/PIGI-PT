using PIGI_PT_Domain.Base;

namespace PIGI_PT_Domain.Events.Inquilino
{
    /// <summary>
    /// Evento de dominio emitido cuando un nuevo inquilino es registrado.
    /// Contiene información del inquilino para handlers que necesiten crear registros o notificaciones.
    /// </summary>
    public class InquilinoRegistradoEvent : DomainEvent
    {
        public Guid InquilinoId { get; }
        public string NombreComercial { get; }
        public string DominioRed { get; }

        public InquilinoRegistradoEvent(Guid inquilinoId, string nombreComercial, string dominioRed)
        {
            InquilinoId = inquilinoId;
            NombreComercial = nombreComercial;
            DominioRed = dominioRed;
        }
    }
}

