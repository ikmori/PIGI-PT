namespace PIGI_PT_Domain.Base
{
    /// <summary>
    /// Clase base para todos los Value Objects del dominio.
    /// 
    /// Un Value Object es un objeto que se compara por sus valores, no por su identidad.
    /// Los Value Objects son inmutables y encapsulan lógica relacionada a valores específicos del dominio.
    /// </summary>
    public abstract class ValueObject
    {
        /// <summary>
        /// Compara dos Value Objects por valor.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;

            var other = (ValueObject)obj;
            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        /// <summary>
        /// Genera un hash basado en los componentes de igualdad.
        /// </summary>
        public override int GetHashCode()
        {
            return GetEqualityComponents()
                .Select(x => x?.GetHashCode() ?? 0)
                .Aggregate((x, y) => x ^ y);
        }

        /// <summary>
        /// Operador de igualdad.
        /// </summary>
        public static bool operator ==(ValueObject left, ValueObject right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Operador de desigualdad.
        /// </summary>
        public static bool operator !=(ValueObject left, ValueObject right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Retorna los componentes que se usan para comparar dos Value Objects.
        /// Cada subclase debe implementar esto retornando los valores que definen su igualdad.
        /// </summary>
        protected abstract IEnumerable<object> GetEqualityComponents();
    }
}
