using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PIGI_PT_WebApp.Models;

namespace PIGI_PT_WebApp.Services
{
    /// <summary>
    /// Servicio de autenticación real conectado a la API de PIGI-PT.
    /// Valida las credenciales a través del endpoint HTTP de la API y mantiene el estado del usuario en sesión.
    /// </summary>
    public class ApiAuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private Usuario? _currentUser;

        public ApiAuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Obtiene el usuario actual autenticado.
        /// Retorna un usuario invitado/vacío si no hay sesión activa.
        /// </summary>
        public Task<Usuario> GetCurrentUserAsync()
        {
            return Task.FromResult(_currentUser ?? new Usuario
            {
                Nombre = "Invitado",
                Rol = Rol.UsuarioGeneral,
                Email = ""
            });
        }

        public Task<int> GetUnreadNotificationsCountAsync()
        {
            return Task.FromResult(3);
        }

        /// <summary>
        /// Valida el correo y contraseña ingresados contra la API local de PIGI-PT.
        /// </summary>
        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/v1/auth/login", new { Email = email, Password = password });
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResult>();
                    if (result != null)
                    {
                        _currentUser = new Usuario
                        {
                            Id = result.UserId,
                            Nombre = result.FullName,
                            Email = result.Email,
                            Rol = ParseRol(result.Rol),
                            InquilinoId = result.InquilinoId
                        };
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                // Fallback silencioso en caso de desconexión o error
                return false;
            }
        }

        public Task LogoutAsync()
        {
            _currentUser = null;
            return Task.CompletedTask;
        }

        private Rol ParseRol(string rolName)
        {
            return rolName switch
            {
                "SuperAdmin" => Rol.SuperAdmin,
                "Admin" => Rol.Admin,
                "Operador" => Rol.Operador,
                _ => Rol.UsuarioGeneral
            };
        }

        private class LoginResult
        {
            public Guid UserId { get; set; }
            public Guid InquilinoId { get; set; }
            public string FullName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Rol { get; set; } = string.Empty;
        }
    }
}
