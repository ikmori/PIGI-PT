using PIGI_PT_Domain.Base;

namespace PIGI_PT_Domain.ValueObjects
{
    /// <summary>
    /// Value Object que representa el estado de un ticket.
    /// 
    /// Encapsula la máquina de estados del ticket y valida transiciones permitidas.
    /// Estados posibles:
    /// - PendienteDeAnalisis: El ticket fue creado pero no ha sido procesado
    /// - Clasificado: El ticket ha sido analizado por IA
    /// - EnProgreso: Un operador está trabajando en el ticket
    /// - Resuelto: El ticket ha sido resuelto
    /// - Cancelado: El ticket fue cancelado por el usuario
    /// - Rechazado: El ticket fue rechazado por el sistema/operador
    /// </summary>
    public class EstadoTicket : ValueObject
    {
        // Constantes para los valores
        public const int PENDIENTE_DE_ANALISIS_VALUE = 1;
        public const int CLASIFICADO_VALUE = 2;
        public const int EN_PROGRESO_VALUE = 3;
        public const int RESUELTO_VALUE = 4;
        public const int CANCELADO_VALUE = 5;
        public const int RECHAZADO_VALUE = 6;

        // Instancias predefinidas
        public static readonly EstadoTicket PendienteDeAnalisis = 
            new(PENDIENTE_DE_ANALISIS_VALUE, "Pendiente de Análisis");
        public static readonly EstadoTicket Clasificado = 
            new(CLASIFICADO_VALUE, "Enviado al Área");
        public static readonly EstadoTicket EnProgreso = 
            new(EN_PROGRESO_VALUE, "En Progreso");
        public static readonly EstadoTicket Resuelto = 
            new(RESUELTO_VALUE, "Resuelto");
        public static readonly EstadoTicket Cancelado = 
            new(CANCELADO_VALUE, "Cancelado");
        public static readonly EstadoTicket Rechazado = 
            new(RECHAZADO_VALUE, "Rechazado");

        /// <summary>
        /// Valor numérico del estado.
        /// </summary>
        public int Valor { get; private set; }

        /// <summary>
        /// Nombre legible del estado.
        /// </summary>
        public string Nombre { get; private set; }

        /// <summary>
        /// Constructor privado para EF Core.
        /// </summary>
        private EstadoTicket() { }

        /// <summary>
        /// Constructor interno.
        /// </summary>
        private EstadoTicket(int valor, string nombre)
        {
            Valor = valor;
            Nombre = nombre;
        }

        /// <summary>
        /// Factory method para crear un estado a partir de un valor numérico.
        /// </summary>
        public static EstadoTicket Create(int valor)
        {
            return valor switch
            {
                PENDIENTE_DE_ANALISIS_VALUE => PendienteDeAnalisis,
                CLASIFICADO_VALUE => Clasificado,
                EN_PROGRESO_VALUE => EnProgreso,
                RESUELTO_VALUE => Resuelto,
                CANCELADO_VALUE => Cancelado,
                RECHAZADO_VALUE => Rechazado,
                _ => throw new ArgumentException($"El valor de estado '{valor}' no es válido.", nameof(valor))
            };
        }

        /// <summary>
        /// Factory method para crear un estado a partir de un string.
        /// </summary>
        public static EstadoTicket DesdeString(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre del estado no puede estar vacío.", nameof(nombre));

            return nombre.ToLowerInvariant().Trim() switch
            {
                "pendientedeanalisis" or "pendiente de análisis" or "pendiente" => PendienteDeAnalisis,
                "clasificado" or "enviadoalarea" or "enviado al área" or "enviado al area" => Clasificado,
                "enprogreso" or "en progreso" => EnProgreso,
                "resuelto" => Resuelto,
                "cancelado" => Cancelado,
                "rechazado" => Rechazado,
                _ => throw new ArgumentException($"El estado '{nombre}' no es reconocido.", nameof(nombre))
            };
        }

        /// <summary>
        /// Valida si es permitida una transición desde este estado a otro.
        /// 
        /// Transiciones permitidas:
        /// PendienteDeAnalisis → Clasificado | Rechazado | Cancelado
        /// Clasificado         → EnProgreso  | Rechazado | Cancelado
        /// EnProgreso          → Resuelto    | Cancelado
        /// Resuelto → (terminal, sin transiciones)
        /// Cancelado → (terminal, sin transiciones)
        /// Rechazado → (terminal, sin transiciones)
        /// </summary>
        /// <param name="nuevoEstado">Estado destino</param>
        /// <returns>true si la transición es válida</returns>
        public bool PuedeTransicionarA(EstadoTicket nuevoEstado)
        {
            if (nuevoEstado == null)
                throw new ArgumentNullException(nameof(nuevoEstado));

            // No permitir transición a sí mismo
            if (Valor == nuevoEstado.Valor)
                return false;

            // Definir máquina de estados
            return (Valor, nuevoEstado.Valor) switch
            {
                // Desde PendienteDeAnalisis
                (PENDIENTE_DE_ANALISIS_VALUE, CLASIFICADO_VALUE) => true,
                (PENDIENTE_DE_ANALISIS_VALUE, RECHAZADO_VALUE)   => true,
                (PENDIENTE_DE_ANALISIS_VALUE, CANCELADO_VALUE)   => true,  // usuario puede cancelar antes de que la IA procese

                // Desde Clasificado
                (CLASIFICADO_VALUE, EN_PROGRESO_VALUE) => true,
                (CLASIFICADO_VALUE, RECHAZADO_VALUE)   => true,
                (CLASIFICADO_VALUE, CANCELADO_VALUE)   => true,  // usuario puede cancelar antes de que se asigne operador

                // Desde EnProgreso
                (EN_PROGRESO_VALUE, RESUELTO_VALUE)  => true,
                (EN_PROGRESO_VALUE, CANCELADO_VALUE) => true,

                // Estados terminales no permiten transiciones
                (RESUELTO_VALUE,  _) => false,
                (CANCELADO_VALUE, _) => false,
                (RECHAZADO_VALUE, _) => false,

                // Cualquier otra combinación no permitida
                _ => false
            };
        }

        /// <summary>
        /// Obtiene todas las transiciones válidas desde este estado.
        /// </summary>
        public List<EstadoTicket> GetTransicionesValidas()
        {
            var transiciones = new List<EstadoTicket>();

            var posibles = new[] 
            { 
                PendienteDeAnalisis, Clasificado, EnProgreso, 
                Resuelto, Cancelado, Rechazado 
            };

            foreach (var posible in posibles)
            {
                if (PuedeTransicionarA(posible))
                    transiciones.Add(posible);
            }

            return transiciones;
        }

        /// <summary>
        /// Verifica si el estado es terminal (no permite transiciones).
        /// </summary>
        public bool EsFinal => 
            Valor == RESUELTO_VALUE || 
            Valor == CANCELADO_VALUE || 
            Valor == RECHAZADO_VALUE;

        /// <summary>
        /// Verifica si el estado es activo (no es cancelado ni rechazado).
        /// </summary>
        public bool EstaActivo => 
            Valor != CANCELADO_VALUE && 
            Valor != RECHAZADO_VALUE;

        /// <summary>
        /// Verifica si requiere operador asignado.
        /// </summary>
        public bool RequiereOperador => 
            Valor == EN_PROGRESO_VALUE || 
            Valor == RESUELTO_VALUE;

        /// <summary>
        /// Verifica si está en análisis.
        /// </summary>
        public bool EstaEnAnalisis => Valor == PENDIENTE_DE_ANALISIS_VALUE;

        /// <summary>
        /// Verifica si está en progreso.
        /// </summary>
        public bool EstaEnProgreso => Valor == EN_PROGRESO_VALUE;

        /// <summary>
        /// Verifica si está resuelto.
        /// </summary>
        public bool EstaResuelto => Valor == RESUELTO_VALUE;

        /// <summary>
        /// Retorna los componentes de igualdad.
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
