namespace PIGI_PT_Domain.Exceptions.Inquilino
{
    /// <summary>
    /// Excepción lanzada cuando se intenta realizar una operación que requiere un inquilino activo,
    /// pero el inquilino está suspendido.
    /// </summary>
    public class InquilinoSuspendidoException : InquilinoDomainException
    {
        public InquilinoSuspendidoException(Guid inquilinoId, string detalles = "")
            : base(inquilinoId,
                  $"El inquilino {inquilinoId} está suspendido. {detalles}",
                  "INQUILINO_SUSPENDIDO")
        {
        }
    }
}
