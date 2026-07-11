using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PIGI_PT_Application.Commands.Categoria;
using PIGI_PT_Application.DTOs.Categoria;
using PIGI_PT_Application.Queries.Categoria;

namespace PIGI_PT_API.Controllers.V1
{
    /// <summary>
    /// Controller para la gestión de categorías de tickets.
    /// Las categorías definen áreas funcionales (Redes, Servidores, Software, etc.)
    /// y se usan para clasificar tickets y asignar operadores por área.
    /// Solo accesible por Admin y SuperAdmin.
    /// </summary>
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class CategoriasController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CategoriasController> _logger;

        public CategoriasController(IMediator mediator, ILogger<CategoriasController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Lista todas las categorías de un inquilino.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<CategoriaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<CategoriaDto>>> GetCategorias([FromQuery] Guid inquilinoId)
        {
            if (inquilinoId == Guid.Empty)
                return BadRequest(new { error = "El parámetro inquilinoId es obligatorio." });

            _logger.LogInformation("GET /api/v1/categorias - InquilinoId: {InquilinoId}", inquilinoId);

            var query = new GetCategoriasByInquilinoQuery { InquilinoId = inquilinoId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene una categoría por su ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(CategoriaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoriaDto>> GetCategoriaById([FromRoute] Guid id)
        {
            _logger.LogInformation("GET /api/v1/categorias/{Id}", id);

            try
            {
                var query = new GetCategoriaByIdQuery { CategoriaId = id };
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva categoría.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(CategoriaDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CategoriaDto>> CreateCategoria([FromBody] CreateCategoriaRequest request)
        {
            _logger.LogInformation("POST /api/v1/categorias - Nombre: {Nombre}", request.NombreCategoria);

            try
            {
                var command = new CreateCategoriaCommand
                {
                    NombreCategoria = request.NombreCategoria,
                    Descripcion = request.Descripcion,
                    InquilinoId = request.InquilinoId
                };

                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetCategoriaById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }

    // --- Request DTOs ---

    public class CreateCategoriaRequest
    {
        public string NombreCategoria { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public Guid InquilinoId { get; set; }
    }
}
