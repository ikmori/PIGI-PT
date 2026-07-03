using MediatR;
using Microsoft.AspNetCore.Mvc;
using PIGI_PT_Application.Commands.RiesgoOperacional;
using PIGI_PT_Application.DTOs.RiesgoOperacional;
using PIGI_PT_Application.Queries.RiesgoOperacional;

namespace PIGI_PT_API.Controllers.V1
{
    /// <summary>
    /// Controller para la gestión de riesgos operacionales.
    /// Permite registrar amenazas, reevaluar impacto y consultar riesgos por inquilino.
    /// Solo accesible por Admin y SuperAdmin.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class RiesgosOperacionalesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RiesgosOperacionalesController> _logger;

        public RiesgosOperacionalesController(IMediator mediator, ILogger<RiesgosOperacionalesController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Lista los riesgos operacionales de un inquilino.
        /// Opcionalmente filtra solo los que requieren revisión urgente.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<RiesgoOperacionalDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<RiesgoOperacionalDto>>> GetRiesgos(
            [FromQuery] Guid inquilinoId,
            [FromQuery] bool soloUrgentes = false)
        {
            if (inquilinoId == Guid.Empty)
                return BadRequest(new { error = "El parámetro inquilinoId es obligatorio." });

            _logger.LogInformation("GET /api/v1/riesgosoperacionales - InquilinoId: {InquilinoId}, SoloUrgentes: {SoloUrgentes}", inquilinoId, soloUrgentes);

            var query = new GetRiesgosByInquilinoQuery
            {
                InquilinoId = inquilinoId,
                SoloRevisionUrgente = soloUrgentes
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene un riesgo operacional por su ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(RiesgoOperacionalDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RiesgoOperacionalDto>> GetRiesgoById([FromRoute] Guid id)
        {
            _logger.LogInformation("GET /api/v1/riesgosoperacionales/{Id}", id);

            try
            {
                var query = new GetRiesgoByIdQuery { RiesgoId = id };
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Registra un nuevo riesgo operacional.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(RiesgoOperacionalDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RiesgoOperacionalDto>> CreateRiesgo([FromBody] CreateRiesgoRequest request)
        {
            _logger.LogInformation("POST /api/v1/riesgosoperacionales - Servicio: {Servicio}", request.ServicioAfectado);

            try
            {
                var command = new CreateRiesgoOperacionalCommand
                {
                    InquilinoId = request.InquilinoId,
                    ServicioAfectado = request.ServicioAfectado,
                    DescripcionAmenaza = request.DescripcionAmenaza,
                    NivelDeImpactoValor = request.NivelDeImpactoValor,
                    PlanDeMitigacion = request.PlanDeMitigacion,
                    UserId = request.UserId
                };

                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetRiesgoById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Reevalúa el nivel de impacto de un riesgo existente.
        /// </summary>
        [HttpPut("{id:guid}/reevaluar")]
        [ProducesResponseType(typeof(RiesgoOperacionalDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RiesgoOperacionalDto>> ReevaluarImpacto(
            [FromRoute] Guid id,
            [FromBody] ReevaluarImpactoRequest request)
        {
            _logger.LogInformation("PUT /api/v1/riesgosoperacionales/{Id}/reevaluar", id);

            try
            {
                var command = new ReevaluarImpactoRiesgoCommand
                {
                    RiesgoId = id,
                    NuevoImpactoValor = request.NuevoImpactoValor,
                    UserId = request.UserId
                };
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
            catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        }
    }

    // --- Request DTOs ---

    public class CreateRiesgoRequest
    {
        public Guid InquilinoId { get; set; }
        public string ServicioAfectado { get; set; } = string.Empty;
        public string DescripcionAmenaza { get; set; } = string.Empty;
        public int NivelDeImpactoValor { get; set; }
        public string PlanDeMitigacion { get; set; } = string.Empty;
        public Guid UserId { get; set; }
    }

    public class ReevaluarImpactoRequest
    {
        public int NuevoImpactoValor { get; set; }
        public Guid UserId { get; set; }
    }
}
