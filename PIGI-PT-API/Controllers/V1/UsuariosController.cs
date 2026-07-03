using MediatR;
using Microsoft.AspNetCore.Mvc;
using PIGI_PT_Application.Commands.Usuario;
using PIGI_PT_Application.DTOs.Usuario;
using PIGI_PT_Application.Queries.Usuario;

namespace PIGI_PT_API.Controllers.V1
{
    /// <summary>
    /// Controller para la gestión de usuarios dentro de un inquilino.
    /// Solo accesible por Admin y SuperAdmin.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class UsuariosController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(IMediator mediator, ILogger<UsuariosController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Lista todos los usuarios de un inquilino.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<UsuarioDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<UsuarioDto>>> GetUsuarios(
            [FromQuery] Guid inquilinoId,
            [FromQuery] bool soloActivos = true)
        {
            if (inquilinoId == Guid.Empty)
                return BadRequest(new { error = "El parámetro inquilinoId es obligatorio." });

            _logger.LogInformation("GET /api/v1/usuarios - InquilinoId: {InquilinoId}", inquilinoId);

            var query = new GetUsuariosByInquilinoQuery
            {
                InquilinoId = inquilinoId,
                SoloActivos = soloActivos
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene un usuario por su ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UsuarioDto>> GetUsuarioById([FromRoute] Guid id)
        {
            _logger.LogInformation("GET /api/v1/usuarios/{Id}", id);

            try
            {
                var query = new GetUsuarioByIdQuery { UsuarioId = id };
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo usuario.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UsuarioDto>> CreateUsuario([FromBody] CreateUsuarioRequest request)
        {
            _logger.LogInformation("POST /api/v1/usuarios - FullName: {FullName}", request.FullName);

            try
            {
                var command = new CreateUsuarioCommand
                {
                    InquilinoId = request.InquilinoId,
                    FullName = request.FullName,
                    Email = request.Email,
                    UserName = request.UserName,
                    Password = request.Password,
                    RolValor = request.RolValor
                };

                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetUsuarioById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Desactiva un usuario (borrado lógico).
        /// </summary>
        [HttpPut("{id:guid}/desactivar")]
        [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<UsuarioDto>> DesactivarUsuario(
            [FromRoute] Guid id,
            [FromQuery] Guid modificadorId)
        {
            if (modificadorId == Guid.Empty)
                return BadRequest(new { error = "El parámetro modificadorId es obligatorio." });

            _logger.LogInformation("PUT /api/v1/usuarios/{Id}/desactivar", id);

            try
            {
                var command = new DesactivarUsuarioCommand { UsuarioId = id, ModificadorId = modificadorId };
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
        }

        /// <summary>
        /// Reactiva un usuario previamente desactivado.
        /// </summary>
        [HttpPut("{id:guid}/reactivar")]
        [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<UsuarioDto>> ReactivarUsuario(
            [FromRoute] Guid id,
            [FromQuery] Guid modificadorId)
        {
            if (modificadorId == Guid.Empty)
                return BadRequest(new { error = "El parámetro modificadorId es obligatorio." });

            _logger.LogInformation("PUT /api/v1/usuarios/{Id}/reactivar", id);

            try
            {
                var command = new ReactivarUsuarioCommand { UsuarioId = id, ModificadorId = modificadorId };
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
        }

        /// <summary>
        /// Cambia el rol de un usuario.
        /// </summary>
        [HttpPut("{id:guid}/cambiar-rol")]
        [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UsuarioDto>> CambiarRol(
            [FromRoute] Guid id,
            [FromBody] CambiarRolRequest request)
        {
            _logger.LogInformation("PUT /api/v1/usuarios/{Id}/cambiar-rol", id);

            try
            {
                var command = new CambiarRolUsuarioCommand
                {
                    UsuarioId = id,
                    NuevoRolValor = request.NuevoRolValor,
                    ModificadorId = request.ModificadorId
                };
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
            catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        }

        /// <summary>
        /// Asigna una categoría a un operador.
        /// </summary>
        [HttpPost("{id:guid}/categorias/{categoriaId:guid}")]
        [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UsuarioDto>> AsignarCategoria(
            [FromRoute] Guid id,
            [FromRoute] Guid categoriaId,
            [FromQuery] Guid adminId)
        {
            if (adminId == Guid.Empty)
                return BadRequest(new { error = "El parámetro adminId es obligatorio." });

            _logger.LogInformation("POST /api/v1/usuarios/{Id}/categorias/{CategoriaId}", id, categoriaId);

            try
            {
                var command = new AssignCategoriaToOperadorCommand
                {
                    OperadorId = id,
                    CategoriaId = categoriaId,
                    AdminId = adminId
                };
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
            catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        }

        /// <summary>
        /// Desasigna una categoría de un operador.
        /// </summary>
        [HttpDelete("{id:guid}/categorias/{categoriaId:guid}")]
        [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UsuarioDto>> DesasignarCategoria(
            [FromRoute] Guid id,
            [FromRoute] Guid categoriaId,
            [FromQuery] Guid adminId)
        {
            if (adminId == Guid.Empty)
                return BadRequest(new { error = "El parámetro adminId es obligatorio." });

            _logger.LogInformation("DELETE /api/v1/usuarios/{Id}/categorias/{CategoriaId}", id, categoriaId);

            try
            {
                var command = new RemoveCategoriaFromOperadorCommand
                {
                    OperadorId = id,
                    CategoriaId = categoriaId,
                    AdminId = adminId
                };
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
        }
    }

    // --- Request DTOs ---

    public class CreateUsuarioRequest
    {
        public Guid InquilinoId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int RolValor { get; set; }
    }

    public class CambiarRolRequest
    {
        public int NuevoRolValor { get; set; }
        public Guid ModificadorId { get; set; }
    }
}
