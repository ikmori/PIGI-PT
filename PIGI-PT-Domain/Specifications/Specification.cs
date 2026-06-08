using System.Linq.Expressions;
using PIGI_PT_Domain.Base;

namespace PIGI_PT_Domain.Specifications
{
    /// <summary>
    /// Clase base abstracta para implementar el patrón Specification.
    /// Encapsula los criterios de consulta LINQ para consultas de base de datos reutilizables y testeables.
    /// </summary>
    /// <typeparam name="T">Tipo de entidad que hereda de <see cref="BaseEntity"/>.</typeparam>
    public abstract class Specification<T> where T : BaseEntity
    {
        /// <summary>
        /// Expresión de criterio de filtro (cláusula WHERE).
        /// </summary>
        public Expression<Func<T, bool>>? Criteria { get; protected set; }

        /// <summary>
        /// Lista de expresiones de inclusión para relaciones y navegación eager loading (cláusulas INCLUDE).
        /// </summary>
        public List<Expression<Func<T, object>>> Includes { get; } = new();

        /// <summary>
        /// Expresión de ordenamiento ascendente (cláusula ORDER BY).
        /// </summary>
        public Expression<Func<T, object>>? OrderBy { get; protected set; }

        /// <summary>
        /// Expresión de ordenamiento descendente (cláusula ORDER BY DESC).
        /// </summary>
        public Expression<Func<T, object>>? OrderByDescending { get; protected set; }

        /// <summary>
        /// Indica si la paginación está habilitada en esta especificación.
        /// </summary>
        public bool IsPagingEnabled { get; protected set; }

        /// <summary>
        /// Tamaño de la página para la paginación.
        /// </summary>
        public int PageSize { get; protected set; }

        /// <summary>
        /// Índice de la página actual para la paginación (basado en 0).
        /// </summary>
        public int PageIndex { get; protected set; }

        /// <summary>
        /// Agrega una expresión de navegación de inclusión para cargar relaciones relacionadas.
        /// </summary>
        /// <param name="includeExpression">Expresión lambda que selecciona la propiedad de navegación.</param>
        protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }
    }
}
