using PIGI_PT_Domain.Base;
using PIGI_PT_Domain.Events.User;
using PIGI_PT_Domain.Exceptions.Usuario;
using PIGI_PT_Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PIGI_PT_Domain.Aggregates.Usuario
{
    /// <summary>
    /// Agregado Raíz de Usuario.
    /// Representa un usuario del sistema PIGI-PT dentro de un inquilino específico.
    /// Gestiona:
    /// - Identidad del usuario (nombre, email, nombre de usuario)
    /// - Credenciales (contraseña hasheada)
    /// - Rol con permisos granulares
    /// - Estado activo/inactivo
    /// - Categorías asignadas (para operadores: define su área de responsabilidad)
    /// 
    /// Invariantes:
    /// - Nombre, email, nombre de usuario no pueden estar vacíos
    /// - El rol debe ser válido
    /// - La contraseña siempre está hasheada
    /// - Solo usuarios activos pueden realizar acciones
    /// - Solo operadores pueden tener categorías asignadas
    /// </summary>
    public class Usuario : InquilinoEntity
    {
        public string FullName { get; private set; }
        public string Email { get; private set; }
        public string UserName { get; private set; }
        public string Password { get; private set; }  // Siempre hasheada
        public Rol Rol { get; private set; }

        private readonly List<Guid> _categoriasAsignadasIds = new();

        /// <summary>
        /// IDs de las categorías asignadas a este operador.
        /// Define el área de responsabilidad: un operador solo ve tickets
        /// clasificados en estas categorías.
        /// Solo aplica para usuarios con rol Operador.
        /// </summary>
        public IReadOnlyList<Guid> CategoriasAsignadasIds => _categoriasAsignadasIds.AsReadOnly();

        private Usuario() : base()
        {
        }

        /// <summary>
        /// Constructor para crear un nuevo usuario.
        /// Emite evento de creación.
        /// </summary>
        /// <param name="inquilinoId">ID del inquilino propietario del usuario</param>
        /// <param name="fullName">Nombre completo del usuario (requerido)</param>
        /// <param name="email">Email del usuario (requerido)</param>
        /// <param name="userName">Nombre de usuario único (requerido)</param>
        /// <param name="password">Hash de contraseña (requerido)</param>
        /// <param name="rol">Rol del usuario (requerido)</param>
        /// <exception cref="ArgumentException">Si algún campo requerido está vacío</exception>
        /// <exception cref="ArgumentNullException">Si rol es nulo</exception>
        public Usuario(Guid inquilinoId, string fullName, string email, string userName, string password, Rol rol)
            : base(inquilinoId)
        {
            ValidarInvariantesConstruccion(fullName, email, userName, password, rol);

            FullName = fullName;
            Email = email;
            UserName = userName;
            Password = password;
            Rol = rol;

            AddDomainEvent(new UsuarioCreadoEvent(Id, inquilinoId, fullName, email, userName, rol));
        }

        /// <summary>
        /// Asigna una categoría al operador, definiendo su área de responsabilidad.
        /// Solo usuarios con rol Operador pueden tener categorías asignadas.
        /// </summary>
        /// <param name="categoriaId">ID de la categoría a asignar (requerido)</param>
        /// <param name="modificadorId">ID del administrador que realiza la asignación</param>
        /// <exception cref="ArgumentException">Si categoriaId o modificadorId son vacíos</exception>
        /// <exception cref="InvalidOperationException">Si el usuario no es operador</exception>
        /// <exception cref="UsuarioInactivoException">Si el usuario está inactivo</exception>
        public void AsignarCategoria(Guid categoriaId, Guid modificadorId)
        {
            if (categoriaId == Guid.Empty)
                throw new ArgumentException("El identificador de la categoría es obligatorio.", nameof(categoriaId));

            if (modificadorId == Guid.Empty)
                throw new ArgumentException("El identificador del modificador es obligatorio.", nameof(modificadorId));

            if (!IsActive)
                throw new UsuarioInactivoException(Id, "No se puede asignar categoría a un usuario inactivo.");

            if (!EsOperador())
                throw new InvalidOperationException("Solo los operadores pueden tener categorías asignadas.");

            if (_categoriasAsignadasIds.Contains(categoriaId))
                return; // Ya está asignada, no duplicar

            _categoriasAsignadasIds.Add(categoriaId);
            ModifiedBy = modificadorId;
            ModifiedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Desasigna una categoría del operador.
        /// </summary>
        /// <param name="categoriaId">ID de la categoría a desasignar</param>
        /// <param name="modificadorId">ID del administrador que realiza la desasignación</param>
        /// <exception cref="ArgumentException">Si categoriaId o modificadorId son vacíos</exception>
        /// <exception cref="InvalidOperationException">Si la categoría no está asignada</exception>
        public void DesasignarCategoria(Guid categoriaId, Guid modificadorId)
        {
            if (categoriaId == Guid.Empty)
                throw new ArgumentException("El identificador de la categoría es obligatorio.", nameof(categoriaId));

            if (modificadorId == Guid.Empty)
                throw new ArgumentException("El identificador del modificador es obligatorio.", nameof(modificadorId));

            if (!_categoriasAsignadasIds.Contains(categoriaId))
                throw new InvalidOperationException($"La categoría '{categoriaId}' no está asignada a este operador.");

            _categoriasAsignadasIds.Remove(categoriaId);
            ModifiedBy = modificadorId;
            ModifiedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Verifica si el operador tiene asignada una categoría específica.
        /// </summary>
        public bool TieneCategoriaAsignada(Guid categoriaId)
        {
            return _categoriasAsignadasIds.Contains(categoriaId);
        }

        /// <summary>
        /// Verifica si el usuario es operador.
        /// </summary>
        public bool EsOperador() => Rol.Valor == Rol.OPERADOR_VALUE;

        /// <summary>
        /// Actualiza la contraseña del usuario.
        /// </summary>
        /// <param name="nuevoHash">Nuevo hash de contraseña (requerido)</param>
        /// <param name="modificadorId">ID del usuario que realiza la modificación</param>
        /// <exception cref="ArgumentException">Si el nuevo hash está vacío o modificadorId es vacío</exception>
        /// <exception cref="UsuarioInactivoException">Si el usuario está inactivo</exception>
        public void ActualizarContrasena(string nuevoHash, Guid modificadorId)
        {
            if (string.IsNullOrWhiteSpace(nuevoHash))
                throw new ArgumentException("La nueva contraseña no puede estar vacía.", nameof(nuevoHash));

            if (modificadorId == Guid.Empty)
                throw new ArgumentException("El identificador del modificador es obligatorio.", nameof(modificadorId));

            if (!IsActive)
                throw new UsuarioInactivoException(Id, "No se puede actualizar contraseña de un usuario inactivo.");

            Password = nuevoHash;
            ModifiedBy = modificadorId;
            ModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new ContrasenaActualizadaEvent(Id, InquilinoId, modificadorId));
        }

        /// <summary>
        /// Cambia el rol del usuario.
        /// </summary>
        /// <param name="nuevoRol">Nuevo rol (requerido)</param>
        /// <param name="modificadorId">ID del usuario que realiza la modificación</param>
        /// <exception cref="ArgumentNullException">Si el nuevo rol es nulo</exception>
        /// <exception cref="ArgumentException">Si modificadorId es vacío</exception>
        /// <exception cref="UsuarioInactivoException">Si el usuario está inactivo</exception>
        public void CambiarRol(Rol nuevoRol, Guid modificadorId)
        {
            if (nuevoRol == null)
                throw new ArgumentNullException(nameof(nuevoRol), "El rol es obligatorio.");

            if (modificadorId == Guid.Empty)
                throw new ArgumentException("El identificador del modificador es obligatorio.", nameof(modificadorId));

            if (!IsActive)
                throw new UsuarioInactivoException(Id, "No se puede cambiar rol de un usuario inactivo.");

            // Los Value Objects sobreescriben la igualdad
            if (Rol == nuevoRol)
                return;

            var rolAnterior = Rol;
            Rol = nuevoRol;

            // Si deja de ser operador, limpiar categorías asignadas
            if (nuevoRol.Valor != Rol.OPERADOR_VALUE)
                _categoriasAsignadasIds.Clear();

            ModifiedBy = modificadorId;
            ModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new RolModificadoEvent(Id, InquilinoId, rolAnterior, nuevoRol, modificadorId));
        }

        /// <summary>
        /// Desactiva el usuario.
        /// </summary>
        /// <param name="modificadorId">ID del usuario que realiza la modificación</param>
        /// <exception cref="ArgumentException">Si modificadorId es vacío</exception>
        /// <exception cref="UsuarioYaInactivoException">Si el usuario ya está inactivo</exception>
        public void DesactivarPerfil(Guid modificadorId)
        {
            if (modificadorId == Guid.Empty)
                throw new ArgumentException("El identificador del modificador es obligatorio.", nameof(modificadorId));

            if (!IsActive)
                throw new UsuarioYaInactivoException(Id, "El usuario ya está inactivo.");

            IsActive = false;
            ModifiedBy = modificadorId;
            ModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new UsuarioDesactivadoEvent(Id, InquilinoId, modificadorId));
        }

        /// <summary>
        /// Reactiva el usuario.
        /// </summary>
        /// <param name="modificadorId">ID del usuario que realiza la modificación</param>
        /// <exception cref="ArgumentException">Si modificadorId es vacío</exception>
        /// <exception cref="UsuarioYaActivoException">Si el usuario ya está activo</exception>
        public void ReactivarPerfil(Guid modificadorId)
        {
            if (modificadorId == Guid.Empty)
                throw new ArgumentException("El identificador del modificador es obligatorio.", nameof(modificadorId));

            if (IsActive)
                throw new UsuarioYaActivoException(Id, "El usuario ya está activo.");

            IsActive = true;
            ModifiedBy = modificadorId;
            ModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new UsuarioReactivadoEvent(Id, InquilinoId, modificadorId));
        }

        /// <summary>
        /// Verifica si el usuario tiene un permiso específico.
        /// </summary>
        public bool TienePermiso(string permiso)
        {
            return Rol.TienePermiso(permiso);
        }

        /// <summary>
        /// Verifica si el usuario es administrador.
        /// </summary>
        public bool EsAdministrador()
        {
            return Rol.EsAdministrador;
        }

        /// <summary>
        /// Método privado para validar invariantes en la construcción.
        /// </summary>
        private static void ValidarInvariantesConstruccion(string fullName, string email, string userName, string password, Rol rol)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("El nombre es obligatorio.", nameof(fullName));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("El email es obligatorio.", nameof(email));

            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("El nombre de usuario es obligatorio.", nameof(userName));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("La contraseña es obligatoria.", nameof(password));

            if (rol == null)
                throw new ArgumentNullException(nameof(rol), "El rol es obligatorio.");
        }
    }
}
