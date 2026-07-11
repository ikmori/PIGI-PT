using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PIGI_PT_Application.Ports.Services;
using PIGI_PT_Infraestructure.Persistence.DbContext;

namespace PIGI_PT_API.Controllers.V1
{
    /// <summary>
    /// Controlador para gestionar la autenticación de usuarios.
    /// Para el avance funcional, valida credenciales directamente contra la base de datos de desarrollo.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly PigiPtDbContext _context;
        private readonly IAuthenticationService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            PigiPtDbContext context, 
            IAuthenticationService authService,
            ILogger<AuthController> logger)
        {
            _context = context;
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Valida las credenciales del usuario y retorna los detalles del perfil y de inquilino.
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            _logger.LogInformation("POST /api/v1/auth/login - Email: {Email}", request.Email);

            var user = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || user.Password != request.Password)
            {
                _logger.LogWarning("Autenticación fallida para: {Email}", request.Email);
                return Unauthorized(new { error = "Credenciales incorrectas o usuario no registrado." });
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("Intento de ingreso de usuario inactivo: {Email}", request.Email);
                return Unauthorized(new { error = "La cuenta de usuario está desactivada." });
            }

            _logger.LogInformation("Autenticación exitosa. Usuario: {Email}, Inquilino: {InquilinoId}", user.Email, user.InquilinoId);

            // Generar Token JWT Real
            var token = await _authService.GenerateTokenAsync(user.Id, user.UserName, user.Rol.Nombre);

            return Ok(new LoginResponse
            {
                UserId = user.Id,
                InquilinoId = user.InquilinoId,
                FullName = user.FullName,
                Email = user.Email,
                Rol = user.Rol.Nombre,
                Token = token
            });
        }
    }

    /// <summary>
    /// Modelo para la petición de inicio de sesión.
    /// </summary>
    public class LoginRequest
    {
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo electrónico inválido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// Modelo para la respuesta de inicio de sesión exitoso.
    /// </summary>
    public class LoginResponse
    {
        public Guid UserId { get; set; }
        public Guid InquilinoId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}

