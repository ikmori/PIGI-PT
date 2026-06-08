namespace PIGI_PT_Domain.Exceptions.Categoria
{
    /// <summary>
    /// Excepción lanzada cuando se intenta crear una categoría con un nombre que ya existe en el inquilino.
    /// </summary>
    public class CategoriaYaExistsConNombreException : CategoriaDomainException
    {
        public string Nombre { get; }

        public CategoriaYaExistsConNombreException(Guid categoriaId, string nombre, string detalles = "")
            : base(categoriaId,
                  $"Ya existe una categoría con el nombre '{nombre}'. {detalles}",
                  "CATEGORIA_NOMBRE_DUPLICADO")
        {
            Nombre = nombre;
        }
    }
}
