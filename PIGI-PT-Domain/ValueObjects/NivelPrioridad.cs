using PIGI_PT_Domain.Base;

namespace PIGI_PT_Domain.ValueObjects
{
    /// <summary>
    /// Value Object que representa el nivel de prioridad de un ticket.
    /// 
    /// Este Value Object refactoriza el enum anterior añadiendo:
    /// - Métodos de negocio (comparaciones, validaciones)
    /// - Factory methods para crear instancias
    /// - Comparación por valor automática
    /// </summary>
    public class NivelPrioridad : ValueObject
    {
        // Constantes para los valores
        public const int NO_DEFINIDA_VALUE = 0;
        public const int BAJA_VALUE = 1;
        public const int MEDIA_VALUE = 2;
        public const int ALTA_VALUE = 3;
        public const int CRITICA_VALUE = 4;

        // Instancias predefinidas (para evitar crear nuevas)
        public static readonly NivelPrioridad NoDefinida = new(NO_DEFINIDA_VALUE, "No Definida");
        public static readonly NivelPrioridad Baja = new(BAJA_VALUE, "Baja");
        public static readonly NivelPrioridad Media = new(MEDIA_VALUE, "Media");
        public static readonly NivelPrioridad Alta = new(ALTA_VALUE, "Alta");
        public static readonly NivelPrioridad Critica = new(CRITICA_VALUE, "Crítica");

        /// <summary>
        /// Valor numérico de la prioridad.
        /// </summary>
        public int Valor { get; private set; }

        /// <summary>
        /// Nombre legible de la prioridad.
        /// </summary>
        public string Nombre { get; private set; }

        /// <summary>
        /// Constructor privado para EF Core.
        /// </summary>
        private NivelPrioridad() { }

        /// <summary>
        /// Constructor interno.
        /// </summary>
        private NivelPrioridad(int valor, string nombre)
        {
            Valor = valor;
            Nombre = nombre;
        }

        /// <summary>
        /// Factory method para crear una prioridad a partir de un valor numérico.
        /// </summary>
        /// <param name="valor">Valor numérico (0-4)</param>
        /// <returns>Instancia de NivelPrioridad</returns>
        /// <exception cref="ArgumentException">Si el valor no es válido</exception>
        public static NivelPrioridad Create(int valor)
        {
            return valor switch
            {
                NO_DEFINIDA_VALUE => NoDefinida,
                BAJA_VALUE => Baja,
                MEDIA_VALUE => Media,
                ALTA_VALUE => Alta,
                CRITICA_VALUE => Critica,
                _ => throw new ArgumentException($"El valor de prioridad '{valor}' no es válido.", nameof(valor))
            };
        }

        /// <summary>
        /// Factory method para crear una prioridad a partir de un string.
        /// </summary>
        /// <param name="nombre">Nombre de la prioridad (case-insensitive)</param>
        /// <returns>Instancia de NivelPrioridad</returns>
        /// <exception cref="ArgumentException">Si el nombre no es válido</exception>
        public static NivelPrioridad DesdeString(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre de la prioridad no puede estar vacío.", nameof(nombre));

            return nombre.ToLowerInvariant().Trim() switch
            {
                "nodefinida" or "no definida" => NoDefinida,
                "baja" => Baja,
                "media" => Media,
                "alta" => Alta,
                "critica" or "crítica" => Critica,
                _ => throw new ArgumentException($"La prioridad '{nombre}' no es reconocida.", nameof(nombre))
            };
        }

        /// <summary>
        /// Verifica si esta prioridad es mayor que otra.
        /// </summary>
        /// <param name="otra">Prioridad a comparar</param>
        /// <returns>true si esta prioridad es mayor</returns>
        public bool EsMayorQue(NivelPrioridad otra)
        {
            if (otra == null)
                throw new ArgumentNullException(nameof(otra));

            return Valor > otra.Valor;
        }

        /// <summary>
        /// Verifica si esta prioridad es menor que otra.
        /// </summary>
        public bool EsMenorQue(NivelPrioridad otra)
        {
            if (otra == null)
                throw new ArgumentNullException(nameof(otra));

            return Valor < otra.Valor;
        }

        /// <summary>
        /// Verifica si esta prioridad es igual o mayor que otra.
        /// </summary>
        public bool EsIgualOMayorQue(NivelPrioridad otra)
        {
            if (otra == null)
                throw new ArgumentNullException(nameof(otra));

            return Valor >= otra.Valor;
        }

        /// <summary>
        /// Verifica si esta prioridad es igual o menor que otra.
        /// </summary>
        public bool EsIgualOMenorQue(NivelPrioridad otra)
        {
            if (otra == null)
                throw new ArgumentNullException(nameof(otra));

            return Valor <= otra.Valor;
        }

        /// <summary>
        /// Verifica si esta prioridad es crítica.
        /// </summary>
        public bool EsCritica => Valor == CRITICA_VALUE;

        /// <summary>
        /// Verifica si esta prioridad está definida (no es NoDefinida).
        /// </summary>
        public bool EstaDefinida => Valor != NO_DEFINIDA_VALUE;

        /// <summary>
        /// Retorna los componentes de igualdad para comparación de Value Objects.
        /// </summary>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Valor;
        }

        /// <summary>
        /// Representación en string.
        /// </summary>
        public override string ToString() => Nombre;
    }
}
