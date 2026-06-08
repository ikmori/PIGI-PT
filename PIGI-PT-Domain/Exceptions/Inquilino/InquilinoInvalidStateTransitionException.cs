namespace PIGI_PT_Domain.Exceptions.Inquilino
{
    /// <summary>
    /// Excepción lanzada cuando se intenta cambiar el estado de un inquilino a uno que no es válido.
    /// </summary>
    public class InquilinoInvalidStateTransitionException : InquilinoDomainException
    {
        public string EstadoActual { get; }
        public string EstadoDestino { get; }

        public InquilinoInvalidStateTransitionException(Guid inquilinoId, string estadoActual, string estadoDestino, string detalles = "")
            : base(inquilinoId,
                  $"No es posible transicionar el inquilino {inquilinoId} de '{estadoActual}' a '{estadoDestino}'. {detalles}",
                  "INQUILINO_INVALID_STATE_TRANSITION")
        {
            EstadoActual = estadoActual;
            EstadoDestino = estadoDestino;
        }
    }
}
