namespace PIGI_PT_Domain.Exceptions.Inquilino
{
    /// <summary>
    /// Excepción lanzada cuando se intenta reactivar un inquilino que ya está activo.
    /// </summary>
    public class InquilinoNoEstaActivoException : InquilinoDomainException
    {
        public InquilinoNoEstaActivoException(Guid inquilinoId, string detalles = "")
            : base(inquilinoId,
                  $"El inquilino {inquilinoId} no está suspendido. {detalles}",
                  "INQUILINO_NO_ESTA_INACTIVO")
        {
        }
    }
}
