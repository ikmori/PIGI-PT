using PIGI_PT_Domain.Base;

namespace PIGI_PT_Domain.ValueObjects
{
    /// <summary>
    /// Value Object que representa el rol de un usuario en el sistema.
    /// 
    /// Encapsula los roles disponibles y sus permisos asociados.
    /// Roles: SuperAdmin, Admin, Operador, UsuarioGeneral
    /// </summary>
    public class Rol : ValueObject
    {
        // Constantes para los valores
        public const int SUPER_ADMIN_VALUE = 1;
        public const int ADMIN_VALUE = 2;
        public const int OPERADOR_VALUE = 3;
        public const int USUARIO_GENERAL_VALUE = 4;

        // Instancias predefinidas
        public static readonly Rol SuperAdmin = new(SUPER_ADMIN_VALUE, "SuperAdmin");
        public static readonly Rol Admin = new(ADMIN_VALUE, "Admin");
        public static readonly Rol Operador = new(OPERADOR_VALUE, "Operador");
        public static readonly Rol UsuarioGeneral = new(USUARIO_GENERAL_VALUE, "Usuario General");

        /// <summary>
        /// Valor numérico del rol.
        /// </summary>
        public int Valor { get; private set; }

        /// <summary>
        /// Nombre legible del rol.
        /// </summary>
        public string Nombre { get; private set; }

        /// <summary>
        /// Constructor privado para EF Core.
        /// </summary>
        private Rol() { }

        /// <summary>
        /// Constructor interno.
        /// </summary>
        private Rol(int valor, string nombre)
        {
            Valor = valor;
            Nombre = nombre;
        }

        /// <summary>
        /// Factory method para crear un rol a partir de un valor numérico.
        /// </summary>
        public static Rol Create(int valor)
        {
            return valor switch
            {
                SUPER_ADMIN_VALUE => SuperAdmin,
                ADMIN_VALUE => Admin,
                OPERADOR_VALUE => Operador,
                USUARIO_GENERAL_VALUE => UsuarioGeneral,
                _ => throw new ArgumentException($"El valor de rol '{valor}' no es válido.", nameof(valor))
            };
        }

        /// <summary>
        /// Factory method para crear un rol a partir de un string.
        /// </summary>
        public static Rol DesdeString(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre del rol no puede estar vacío.", nameof(nombre));

            return nombre.ToLowerInvariant().Trim() switch
            {
                "superadmin" or "super admin" => SuperAdmin,
                "admin" => Admin,
                "operador" => Operador,
                "usuarioGeneral" or "usuario general" or "usuario" => UsuarioGeneral,
                _ => throw new ArgumentException($"El rol '{nombre}' no es reconocido.", nameof(nombre))
            };
        }

        /// <summary>
        /// Verifica si el rol tiene un permiso específico.
        /// </summary>
        /// <param name="permiso">Nombre del permiso (ej: "VER_TICKETS", "CREAR_USUARIOS")</param>
        /// <returns>true si el rol tiene el permiso</returns>
        public bool TienePermiso(string permiso)
        {
            if (string.IsNullOrWhiteSpace(permiso))
                return false;

            var permisos = ObtenerPermisos();
            return permisos.Contains(permiso.ToUpperInvariant());
        }

        /// <summary>
        /// Obtiene la lista de permisos del rol.
        /// </summary>
        public List<string> ObtenerPermisos()
        {
            return Valor switch
            {
                SUPER_ADMIN_VALUE => new()
                {
                    "VER_TICKETS",
                    "CREAR_TICKETS",
                    "CLASIFICAR_TICKETS",
                    "ASIGNAR_OPERADOR",
                    "RESOLVER_TICKETS",
                    "VER_USUARIOS",
                    "CREAR_USUARIOS",
                    "EDITAR_USUARIOS",
                    "CREAR_CATEGORIAS",
                    "EDITAR_CATEGORIAS",
                    "CREAR_RIESGOS",
                    "EDITAR_RIESGOS",
                    "SUSPENDER_INQUILINO",
                    "CAMBIAR_CONFIGURACION_IA",
                    "VER_REPORTES",
                    "EXPORTAR_DATOS"
                },

                ADMIN_VALUE => new()
                {
                    "VER_TICKETS",
                    "CREAR_TICKETS",
                    "CLASIFICAR_TICKETS",
                    "ASIGNAR_OPERADOR",
                    "RESOLVER_TICKETS",
                    "VER_USUARIOS",
                    "CREAR_USUARIOS",
                    "EDITAR_USUARIOS",
                    "CREAR_CATEGORIAS",
                    "EDITAR_CATEGORIAS",
                    "CREAR_RIESGOS",
                    "EDITAR_RIESGOS",
                    "CAMBIAR_CONFIGURACION_IA",
                    "VER_REPORTES"
                },

                OPERADOR_VALUE => new()
                {
                    "VER_TICKETS",
                    "CREAR_TICKETS",
                    "CLASIFICAR_TICKETS",
                    "ASIGNAR_OPERADOR",
                    "RESOLVER_TICKETS",
                    "VER_USUARIOS"
                },

                USUARIO_GENERAL_VALUE => new()
                {
                    "VER_TICKETS_PROPIOS",
                    "CREAR_TICKETS"
                },

                _ => new()
            };
        }

        /// <summary>
        /// Verifica si el rol tiene acceso a administración.
        /// </summary>
        public bool EsAdministrador => 
            Valor == SUPER_ADMIN_VALUE || 
            Valor == ADMIN_VALUE;

        /// <summary>
        /// Verifica si el rol es SuperAdmin.
        /// </summary>
        public bool EsSuperAdmin => Valor == SUPER_ADMIN_VALUE;

        /// <summary>
        /// Verifica si el rol puede clasificar tickets.
        /// </summary>
        public bool PuedeClasificarTickets => 
            Valor == SUPER_ADMIN_VALUE || 
            Valor == ADMIN_VALUE || 
            Valor == OPERADOR_VALUE;

        /// <summary>
        /// Verifica si el rol puede ver todos los tickets.
        /// </summary>
        public bool PuedeVerTodosLosTickets => 
            Valor == SUPER_ADMIN_VALUE || 
            Valor == ADMIN_VALUE || 
            Valor == OPERADOR_VALUE;

        /// <summary>
        /// Verifica si el rol puede crear usuarios.
        /// </summary>
        public bool PuedeCrearUsuarios => 
            Valor == SUPER_ADMIN_VALUE || 
            Valor == ADMIN_VALUE;

        /// <summary>
        /// Obtiene la jerarquía del rol (menor número = más permisos).
        /// </summary>
        public int ObtenerNivelJerarquia() => Valor;

        /// <summary>
        /// Verifica si este rol tiene más permisos que otro.
        /// </summary>
        public bool TieneMasPermisosQue(Rol otroRol)
        {
            if (otroRol == null)
                throw new ArgumentNullException(nameof(otroRol));

            // Menor número = más permisos en la jerarquía
            return Valor < otroRol.Valor;
        }

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
