using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PIGI_PT_Application.Commands.Inquilino;
using PIGI_PT_Application.DTOs.Inquilino;
using PIGI_PT_Application.Queries.Inquilino;

namespace PIGI_PT_API.Controllers.V1
{
    /// <summary>
    /// Controller para la gestión de inquilinos (tenants).
    /// Un inquilino representa una organización/empresa dentro de la plataforma.
    /// Solo accesible por SuperAdmin (gestión global) y Admin (gestión propia).
    /// </summary>
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class InquilinosController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<InquilinosController> _logger;

        public InquilinosController(IMediator mediator, ILogger<InquilinosController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Lista todos los inquilinos. Filtra opcionalmente solo activos.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<InquilinoDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<InquilinoDto>>> GetInquilinos([FromQuery] bool soloActivos = true)
        {
            _logger.LogInformation("GET /api/v1/inquilinos - SoloActivos: {SoloActivos}", soloActivos);

            var query = new GetInquilinosQuery { SoloActivos = soloActivos };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene un inquilino por su ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(InquilinoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InquilinoDto>> GetInquilinoById([FromRoute] Guid id)
        {
            _logger.LogInformation("GET /api/v1/inquilinos/{Id}", id);

            try
            {
                var query = new GetInquilinoByIdQuery { InquilinoId = id };
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Registra un nuevo inquilino en la plataforma.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(InquilinoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<InquilinoDto>> CreateInquilino([FromBody] CreateInquilinoRequest request)
        {
            _logger.LogInformation("POST /api/v1/inquilinos - Nombre: {Nombre}", request.NombreComercial);

            try
            {
                var command = new CreateInquilinoCommand
                {
                    NombreComercial = request.NombreComercial,
                    DominioRed = request.DominioRed,
                    UserId = request.UserId
                };

                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetInquilinoById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Suspende un inquilino. Solo SuperAdmin.
        /// </summary>
        [HttpPut("{id:guid}/suspender")]
        [ProducesResponseType(typeof(InquilinoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<InquilinoDto>> SuspenderInquilino(
            [FromRoute] Guid id,
            [FromBody] SuspenderInquilinoRequest request)
        {
            _logger.LogInformation("PUT /api/v1/inquilinos/{Id}/suspender", id);

            try
            {
                var command = new SuspenderInquilinoCommand
                {
                    InquilinoId = id,
                    AdminId = request.AdminId,
                    Motivo = request.Motivo
                };
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
        }

        /// <summary>
        /// Reactiva un inquilino previamente suspendido. Solo SuperAdmin.
        /// </summary>
        [HttpPut("{id:guid}/reactivar")]
        [ProducesResponseType(typeof(InquilinoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<InquilinoDto>> ReactivarInquilino(
            [FromRoute] Guid id,
            [FromBody] ReactivarInquilinoRequest request)
        {
            _logger.LogInformation("PUT /api/v1/inquilinos/{Id}/reactivar", id);

            try
            {
                var command = new ReactivarInquilinoCommand
                {
                    InquilinoId = id,
                    AdminId = request.AdminId,
                    Motivo = request.Motivo
                };
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
        }

        /// <summary>
        /// Actualiza los permisos de IA para un inquilino.
        /// </summary>
        [HttpPut("{id:guid}/permisos-ia")]
        [ProducesResponseType(typeof(InquilinoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InquilinoDto>> ActualizarPermisosIA(
            [FromRoute] Guid id,
            [FromBody] ActualizarPermisosIARequest request)
        {
            _logger.LogInformation("PUT /api/v1/inquilinos/{Id}/permisos-ia - PermitirIA: {PermitirIA}", id, request.PermitirIA);

            try
            {
                var command = new ActualizarPermisosIACommand
                {
                    InquilinoId = id,
                    PermitirIA = request.PermitirIA,
                    AdminId = request.AdminId
                };
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
        }
    }

    // --- Request DTOs ---

    public class CreateInquilinoRequest
    {
        public string NombreComercial { get; set; } = string.Empty;
        public string DominioRed { get; set; } = string.Empty;
        public Guid UserId { get; set; }
    }

    public class SuspenderInquilinoRequest
    {
        public Guid AdminId { get; set; }
        public string Motivo { get; set; } = string.Empty;
    }

    public class ReactivarInquilinoRequest
    {
        public Guid AdminId { get; set; }
        public string Motivo { get; set; } = string.Empty;
    }

    public class ActualizarPermisosIARequest
    {
        public bool PermitirIA { get; set; }
        public Guid AdminId { get; set; }
    }
}
