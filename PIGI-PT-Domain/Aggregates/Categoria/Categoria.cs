using PIGI_PT_Domain.Base;
using PIGI_PT_Domain.Exceptions.Categoria;

namespace PIGI_PT_Domain.Aggregates.Categoria
{
    /// <summary>
    /// Agregado Raíz de Categoria.
    /// Representa una categorización de tickets dentro de un inquilino.
    /// Gestiona:
    /// - Nombre y descripción de la categoría
    /// - Validación de campos obligatorios
    /// - Cambios auditados
    /// 
    /// Invariantes:
    /// - NombreCategoria no puede estar vacío
    /// </summary>
    public class Categoria : InquilinoEntity
    {
        public string NombreCategoria { get; private set; }
        public string Descripcion { get; private set; }

        private Categoria()
        {
        }

        /// <summary>
        /// Constructor para crear una nueva categoría.
        /// </summary>
        /// <param name="nombreCategoria">Nombre de la categoría (requerido)</param>
        /// <param name="descripcion">Descripción de la categoría (opcional)</param>
        /// <param name="inquilinoId">ID del inquilino propietario</param>
        /// <exception cref="ArgumentException">Si nombreCategoria está vacío</exception>
        public Categoria(string nombreCategoria, string descripcion, Guid inquilinoId) : base(inquilinoId)
        {
            if (string.IsNullOrWhiteSpace(nombreCategoria))
                throw new ArgumentException("El nombre de la categoría es obligatorio.", nameof(nombreCategoria));

            NombreCategoria = nombreCategoria;
            Descripcion = descripcion;
        }

        /// <summary>
        /// Actualiza el nombre y descripción de la categoría.
        /// </summary>
        /// <param name="nombreCategoria">Nuevo nombre (requerido)</param>
        /// <param name="descripcion">Nueva descripción (opcional)</param>
        /// <param name="administradorId">ID del administrador que realiza el cambio</param>
        /// <exception cref="ArgumentException">Si nombreCategoria está vacío o administradorId es Guid.Empty</exception>
        /// <exception cref="CategoriaYaExistsConNombreException">Si el nombre ya existe en el inquilino</exception>
        public void ActualizarCategoria(string nombreCategoria, string descripcion, Guid administradorId)
        {
            if (string.IsNullOrWhiteSpace(nombreCategoria))
                throw new ArgumentException("El nombre de la categoría es obligatorio.", nameof(nombreCategoria));

            if (administradorId == Guid.Empty)
                throw new ArgumentException("El identificador del administrador es obligatorio.", nameof(administradorId));

            // Si el nombre no cambió, no hacer nada
            if (NombreCategoria == nombreCategoria && Descripcion == descripcion)
                return;

            NombreCategoria = nombreCategoria;
            Descripcion = descripcion;
            ModifiedAt = DateTime.UtcNow;
            ModifiedBy = administradorId;
        }

        /// <summary>
        /// Actualiza solo el nombre de la categoría.
        /// </summary>
        public void ActualizarNombre(string nuevoNombre, Guid administradorId)
        {
            if (string.IsNullOrWhiteSpace(nuevoNombre))
                throw new ArgumentException("El nombre de la categoría es obligatorio.", nameof(nuevoNombre));

            if (administradorId == Guid.Empty)
                throw new ArgumentException("El identificador del administrador es obligatorio.", nameof(administradorId));

            if (NombreCategoria == nuevoNombre)
                return;

            NombreCategoria = nuevoNombre;
            ModifiedAt = DateTime.UtcNow;
            ModifiedBy = administradorId;
        }

        /// <summary>
        /// Actualiza solo la descripción de la categoría.
        /// </summary>
        public void ActualizarDescripcion(string nuevaDescripcion, Guid administradorId)
        {
            if (administradorId == Guid.Empty)
                throw new ArgumentException("El identificador del administrador es obligatorio.", nameof(administradorId));

            if (Descripcion == nuevaDescripcion)
                return;

            Descripcion = nuevaDescripcion;
            ModifiedAt = DateTime.UtcNow;
            ModifiedBy = administradorId;
        }

        /// <summary>
        /// Obtiene una representación legible de la categoría.
        /// </summary>
        public string ObtenerResumen()
        {
            return $"{NombreCategoria}" + (string.IsNullOrWhiteSpace(Descripcion) ? "" : $": {Descripcion}");
        }
    }
}
