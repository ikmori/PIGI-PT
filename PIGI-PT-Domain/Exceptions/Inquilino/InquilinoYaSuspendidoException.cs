namespace PIGI_PT_Domain.Exceptions.Inquilino
{
    /// <summary>
    /// Excepción lanzada cuando se intenta suspender un inquilino que ya está suspendido.
    /// </summary>
    public class InquilinoYaSuspendidoException : InquilinoDomainException
    {
        public InquilinoYaSuspendidoException(Guid inquilinoId, string detalles = "")
            : base(inquilinoId,
                  $"El inquilino {inquilinoId} ya está suspendido. {detalles}",
                  "INQUILINO_YA_SUSPENDIDO")
        {
        }
    }
}
