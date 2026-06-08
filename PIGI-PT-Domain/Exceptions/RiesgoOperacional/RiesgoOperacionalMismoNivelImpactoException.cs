namespace PIGI_PT_Domain.Exceptions.RiesgoOperacional
{
    /// <summary>
    /// Excepción lanzada cuando se intenta actualizar el nivel de impacto a uno igual al actual.
    /// </summary>
    public class RiesgoOperacionalMismoNivelImpactoException : RiesgoOperacionalDomainException
    {
        public RiesgoOperacionalMismoNivelImpactoException(Guid riesgoId, string detalles = "")
            : base(riesgoId,
                  $"El riesgo {riesgoId} ya tiene el nivel de impacto especificado. {detalles}",
                  "RIESGO_MISMO_NIVEL_IMPACTO")
        {
        }
    }
}
