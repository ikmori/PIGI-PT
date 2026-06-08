using PIGI_PT_Domain.Base;

namespace PIGI_PT_Domain.Exceptions.Categoria
{
    /// <summary>
    /// Base class para excepciones de dominio específicas del Agregado Categoria.
    /// </summary>
    public abstract class CategoriaDomainException : DomainException
    {
        public Guid CategoriaId { get; }

        protected CategoriaDomainException(Guid categoriaId, string message, string errorCode)
            : base(message, errorCode)
        {
            CategoriaId = categoriaId;
        }
    }
}
