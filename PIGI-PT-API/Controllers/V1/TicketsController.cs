using MediatR;
using Microsoft.AspNetCore.Mvc;
using PIGI_PT_Application.Commands.Ticket;
using PIGI_PT_Application.DTOs.Ticket;
using PIGI_PT_Application.Queries.Ticket;

namespace PIGI_PT_API.Controllers.V1
{
    /// <summary>
    /// Controller para la gestión de tickets de incidentes.
    /// Expone las operaciones CRUD y de ciclo de vida de tickets.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class TicketsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TicketsController> _logger;

        public TicketsController(IMediator mediator, ILogger<TicketsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Lista todos los tickets activos de un inquilino.
        /// Opcionalmente filtrar por estado.
        /// </summary>
        /// <param name="inquilinoId">ID del inquilino (tenant).</param>
        /// <param name="estado">Filtro opcional por estado: Pendiente, Clasificado, EnProgreso, Resuelto, Cancelado, Rechazado.</param>
        [HttpGet]
        [ProducesResponseType(typeof(List<TicketDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<TicketDto>>> GetTickets(
            [FromQuery] Guid inquilinoId,
            [FromQuery] string? estado = null)
        {
            if (inquilinoId == Guid.Empty)
                return BadRequest(new { error = "El parámetro inquilinoId es obligatorio." });

            _logger.LogInformation("GET /api/v1/tickets - InquilinoId: {InquilinoId}, Estado: {Estado}", inquilinoId, estado);

            var query = new GetTicketsQuery { InquilinoId = inquilinoId, Estado = estado };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene un ticket específico por su ID.
        /// </summary>
        /// <param name="id">ID único del ticket.</param>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(TicketDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TicketDto>> GetTicketById([FromRoute] Guid id)
        {
            _logger.LogInformation("GET /api/v1/tickets/{Id}", id);

            try
            {
                var query = new GetTicketByIdQuery { TicketId = id };
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo ticket de incidente.
        /// </summary>
        /// <param name="request">Datos del ticket a crear.</param>
        [HttpPost]
        [ProducesResponseType(typeof(TicketDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TicketDto>> CreateTicket([FromBody] CreateTicketRequest request)
        {
            _logger.LogInformation("POST /api/v1/tickets - Titulo: {Titulo}", request.Titulo);

            try
            {
                var command = new CreateTicketCommand
                {
                    InquilinoId = request.InquilinoId,
                    Titulo = request.Titulo,
                    Descripcion = request.Descripcion,
                    UserId = request.UserId
                };

                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetTicketById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Clasifica un ticket asignándole prioridad y categoría.
        /// Transiciona el estado de PendienteDeAnalisis → Clasificado.
        /// </summary>
        /// <param name="id">ID del ticket a clasificar.</param>
        /// <param name="request">Datos de clasificación (prioridad + categoría).</param>
        [HttpPut("{id:guid}/classify")]
        [ProducesResponseType(typeof(TicketDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<TicketDto>> ClassifyTicket(
            [FromRoute] Guid id,
            [FromBody] ClassifyTicketRequest request)
        {
            _logger.LogInformation("PUT /api/v1/tickets/{Id}/classify", id);

            try
            {
                var command = new ClassifyTicketCommand
                {
                    TicketId = id,
                    PrioridadValor = request.Prioridad,
                    CategoriaId = request.CategoriaId,
                    UserId = request.UserId
                };

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Resuelve un ticket. Requiere que el ticket tenga un operador asignado.
        /// Transiciona el estado de EnProgreso → Resuelto.
        /// </summary>
        /// <param name="id">ID del ticket a resolver.</param>
        /// <param name="userId">ID del usuario que resuelve el ticket.</param>
        [HttpPut("{id:guid}/resolve")]
        [ProducesResponseType(typeof(TicketDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<TicketDto>> ResolveTicket(
            [FromRoute] Guid id,
            [FromQuery] Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest(new { error = "El parámetro userId es obligatorio." });

            _logger.LogInformation("PUT /api/v1/tickets/{Id}/resolve - UserId: {UserId}", id, userId);

            try
            {
                var command = new ResolveTicketCommand { TicketId = id, UserId = userId };
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }
    }
}
