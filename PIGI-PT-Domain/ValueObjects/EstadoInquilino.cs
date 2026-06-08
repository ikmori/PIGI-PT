using PIGI_PT_Domain.Base;
using System.Collections.Generic;

namespace PIGI_PT_Domain.ValueObjects
{
    /// <summary>
    /// Value Object que encapsula el estado de un inquilino.
    /// Estados permitidos: Activo, Suspendido.
    /// Proporciona métodos para transiciones válidas y consultas de estado.
    /// </summary>
    public class EstadoInquilino : ValueObject
    {
        public string Valor { get; }

        // Estados predefinidos
        public static readonly EstadoInquilino Activo = new EstadoInquilino("Activo");
        public static readonly EstadoInquilino Suspendido = new EstadoInquilino("Suspendido");

        private static readonly List<EstadoInquilino> AllStates = new()
        {
            Activo,
            Suspendido
        };

        private EstadoInquilino(string valor)
        {
            Valor = valor;
        }

        /// <summary>
        /// Factory method para crear un EstadoInquilino desde un string.
        /// Valida que el estado sea válido.
        /// </summary>
        public static EstadoInquilino Create(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ArgumentException("El estado no puede estar vacío.", nameof(valor));

            var estado = AllStates.FirstOrDefault(e => e.Valor.Equals(valor, StringComparison.OrdinalIgnoreCase));

            if (estado == null)
                throw new ArgumentException($"El estado '{valor}' no es válido. Estados permitidos: Activo, Suspendido.", nameof(valor));

            return estado;
        }

        /// <summary>
        /// Factory method para crear desde un int.
        /// 0 = Activo, 1 = Suspendido
        /// </summary>
        public static EstadoInquilino Create(int valor) => valor switch
        {
            0 => Activo,
            1 => Suspendido,
            _ => throw new ArgumentException($"El código de estado '{valor}' no es válido.", nameof(valor))
        };

        /// <summary>
        /// Verifica si es posible transicionar al estado destino.
        /// Reglas de negocio:
        /// - Desde Activo: puede ir a Suspendido
        /// - Desde Suspendido: puede ir a Activo
        /// </summary>
        public bool PuedeTransicionarA(EstadoInquilino estadoDestino)
        {
            if (estadoDestino == null)
                throw new ArgumentNullException(nameof(estadoDestino));

            return (this == Activo && estadoDestino == Suspendido) ||
                   (this == Suspendido && estadoDestino == Activo);
        }

        /// <summary>
        /// Obtiene la próxima transición válida (útil para operaciones automáticas).
        /// </summary>
        public EstadoInquilino ObtenerProximaTransicion()
        {
            return this == Activo ? Suspendido : Activo;
        }

        public bool EstaActivo => this == Activo;
        public bool EstaSuspendido => this == Suspendido;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Valor;
        }

        public override string ToString() => Valor;
    }
}
