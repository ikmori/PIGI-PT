# Presentation Layer: Web API (PIGI-PT-API)

**Estado:** ⏳ FASE 3 - Pendiente de implementación  
**Última actualización:** Enero 2024  
**Dependencias:** Requiere PIGI-PT-Application (⏳ Fase 2) + PIGI-PT-Infrastructure (⏳ Fase 2)

## Propósito
La capa de **Presentación - API** expone los casos de uso del sistema mediante endpoints REST.

## 📋 Status de Implementación

| Componente | Estado | Notas |
|-----------|--------|-------|
| **Controllers V1** | ⏳ Pendiente | Se crean en Fase 3 (tarea 3.1) |
| **Controllers V2** | ⏳ Pendiente | Planificado para Fase 4+ |
| **Middleware** | ⏳ Pendiente | Se crean en Fase 3 (tarea 3.1) |
| **Swagger/OpenAPI** | ⏳ Pendiente | Se configura en Fase 3 (tarea 3.3) |
| **Authentication/JWT** | ⏳ Pendiente | Se implementa en Fase 3 (tarea 3.2) |
| **Authorization/RBAC** | ⏳ Pendiente | Se implementa en Fase 3 (tarea 3.2) |

## Principios

1. **Thin Controllers**: Mínima lógica, solo delegación a MediatR
2. **Mapeo de Excepciones**: Domain exceptions → HTTP status codes
3. **DTOs en Request/Response**: Nunca devolver Entities
4. **Validación en Application**: Controllers no validan
5. **RESTful**: Seguir convenciones HTTP
6. **Versioning**: Preparar para v2 desde el inicio

## 🗺️ Guía para Fase 3

Para instrucciones detalladas, consulta:
- **[ROADMAP_FUTURO.md - Fase 3](../ROADMAP_FUTURO.md#fase-3-api-layer-estimado-15-20-horas)** - REST Controllers, Authentication, Documentation

## Estructura de Carpetas

```
PIGI-PT-API/
├── Controllers/
│   ├── V1/
│   │   ├── TicketsController.cs        # GET, POST, PUT
│   │   ├── InquilinosController.cs
│   │   ├── UsuariosController.cs
│   │   ├── CategoriasController.cs
│   │   ├── RiesgosOperacionalesController.cs
│   │   ├── AuthController.cs
│   │   └── HealthCheckController.cs
│   ├── V2/
│   │   └── README.md
│   └── README.md
│
├── Middleware/
│   ├── ExceptionHandlingMiddleware.cs  # Mapeo de excepciones
│   ├── TenantResolutionMiddleware.cs   # Extrae tenant del header
│   ├── LoggingMiddleware.cs
│   ├── AuthenticationMiddleware.cs
│   └── README.md
│
├── Configuration/
│   ├── ApiDependencyInjection.cs
│   ├── SwaggerConfiguration.cs         # OpenAPI/Swagger
│   ├── AuthenticationConfiguration.cs
│   ├── CorsConfiguration.cs
│   ├── HealthCheckConfiguration.cs
│   └── README.md
│
├── Extensions/
│   ├── ClaimsPrincipalExtensions.cs
│   ├── HttpContextExtensions.cs
│   └── README.md
│
├── Filters/
│   ├── ValidateModelStateFilter.cs
│   ├── ApiExceptionFilterAttribute.cs
│   └── README.md
│
├── Program.cs                          # Punto de entrada
├── appsettings.json
├── appsettings.Development.json
├── appsettings.Production.json
├── .http (REST Client file)
└── README.md                           (Este archivo)
```

## Estructura de un Controller

```csharp
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize] // Por defecto, requiere autenticación
public class TicketsController : ControllerBase
{
	private readonly IMediator _mediator;
	private readonly ILogger<TicketsController> _logger;

	public TicketsController(
		IMediator mediator,
		ILogger<TicketsController> logger)
	{
		_mediator = mediator;
		_logger = logger;
	}

	/// <summary>
	/// Obtener un ticket por ID
	/// </summary>
	/// <param name="ticketId">ID del ticket</param>
	/// <returns>Detalles del ticket</returns>
	[HttpGet("{ticketId}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<TicketDto>> GetTicketById(Guid ticketId)
	{
		var query = new GetTicketByIdQuery { TicketId = ticketId };
		var result = await _mediator.Send(query);
		return Ok(result);
	}

	/// <summary>
	/// Crear un nuevo ticket
	/// </summary>
	/// <param name="createDto">Datos del nuevo ticket</param>
	/// <returns>Ticket creado</returns>
	[HttpPost]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<TicketDto>> CreateTicket(
		[FromBody] CreateTicketDto createDto)
	{
		var tenantId = User.GetTenantId(); // Extraer del JWT
		var userId = User.GetUserId();     // Extraer del JWT

		var command = new CreateTicketCommand
		{
			InquilinoId = tenantId,
			Titulo = createDto.Titulo,
			Descripcion = createDto.Descripcion,
			UserId = userId
		};

		var result = await _mediator.Send(command);

		return CreatedAtAction(
			nameof(GetTicketById),
			new { ticketId = result.Id },
			result
		);
	}

	/// <summary>
	/// Clasificar un ticket (Admin/Operator)
	/// </summary>
	[HttpPut("{ticketId}/classify")]
	[Authorize(Roles = "Admin,Operador")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<TicketDto>> ClassifyTicket(
		Guid ticketId,
		[FromBody] ClassifyTicketDto classifyDto)
	{
		var userId = User.GetUserId();

		var command = new ClassifyTicketCommand
		{
			TicketId = ticketId,
			Prioridad = classifyDto.Prioridad,
			CategoriaId = classifyDto.CategoriaId,
			UserId = userId
		};

		var result = await _mediator.Send(command);
		return Ok(result);
	}

	/// <summary>
	/// Resolver un ticket
	/// </summary>
	[HttpPut("{ticketId}/resolve")]
	[Authorize(Roles = "Admin,Operador")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<ActionResult<TicketDto>> ResolveTicket(Guid ticketId)
	{
		var userId = User.GetUserId();

		var command = new ResolveTicketCommand
		{
			TicketId = ticketId,
			UserId = userId
		};

		var result = await _mediator.Send(command);
		return Ok(result);
	}
}
```

## Middleware Personalizado

### ExceptionHandlingMiddleware
Mapea excepciones de dominio a respuestas HTTP.

```csharp
public class ExceptionHandlingMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger<ExceptionHandlingMiddleware> _logger;

	public ExceptionHandlingMiddleware(RequestDelegate next, 
		ILogger<ExceptionHandlingMiddleware> logger)
	{
		_next = next;
		_logger = logger;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await _next(context);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Unhandled exception occurred");
			await HandleExceptionAsync(context, ex);
		}
	}

	private static Task HandleExceptionAsync(HttpContext context, Exception exception)
	{
		context.Response.ContentType = "application/json";

		var response = new ErrorResponse
		{
			TraceId = context.TraceIdentifier
		};

		switch (exception)
		{
			case TicketInvalidStateTransitionException ex:
				context.Response.StatusCode = StatusCodes.Status400BadRequest;
				response.Code = ex.Code;
				response.Message = ex.Message;
				break;

			case TicketAlreadyResolvedException ex:
				context.Response.StatusCode = StatusCodes.Status409Conflict;
				response.Code = ex.Code;
				response.Message = ex.Message;
				break;

			case DomainException ex:
				context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
				response.Code = ex.Code;
				response.Message = ex.Message;
				break;

			case ValidationException ex:
				context.Response.StatusCode = StatusCodes.Status400BadRequest;
				response.Code = "VALIDATION_ERROR";
				response.Message = "Errores de validación";
				response.Errors = ex.Errors
					.GroupBy(e => e.PropertyName)
					.ToDictionary(
						g => g.Key,
						g => g.Select(e => e.ErrorMessage).ToList()
					);
				break;

			default:
				context.Response.StatusCode = StatusCodes.Status500InternalServerError;
				response.Code = "INTERNAL_SERVER_ERROR";
				response.Message = "Ocurrió un error interno";
				break;
		}

		return context.Response.WriteAsJsonAsync(response);
	}
}
```

### TenantResolutionMiddleware
Extrae tenant del header y lo guarda en el contexto.

```csharp
public class TenantResolutionMiddleware
{
	private readonly RequestDelegate _next;

	public TenantResolutionMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		// Extraer tenant del header o JWT
		var tenantId = ExtractTenantId(context);

		if (tenantId == Guid.Empty)
		{
			context.Response.StatusCode = StatusCodes.Status400BadRequest;
			await context.Response.WriteAsJsonAsync(
				new { error = "Tenant ID is required" }
			);
			return;
		}

		// Almacenar en items para acceso en handlers
		context.Items["TenantId"] = tenantId;

		await _next(context);
	}

	private Guid ExtractTenantId(HttpContext context)
	{
		// Desde header
		if (context.Request.Headers.TryGetValue("X-Tenant-ID", out var headerValue))
		{
			if (Guid.TryParse(headerValue, out var tenantId))
				return tenantId;
		}

		// Desde JWT claim
		var claim = context.User?.FindFirst("tenant_id");
		if (claim != null && Guid.TryParse(claim.Value, out var jwtTenantId))
			return jwtTenantId;

		return Guid.Empty;
	}
}
```

## Program.cs

```csharp
var builder = WebApplication.CreateBuilder(args);

// Agregar servicios
builder.Services
	.AddApplicationServices()
	.AddInfrastructureServices(builder.Configuration)
	.AddApiServices(builder.Configuration);

// Configuración API
builder.Services.AddControllers();
builder.Services.AddApiVersioning(o =>
{
	o.AssumeDefaultVersionWhenUnspecified = true;
	o.DefaultApiVersion = new ApiVersion(1, 0);
	o.ReportApiVersions = true;
});
builder.Services.AddVersionedApiExplorer(o =>
{
	o.GroupNameFormat = "'v'VVV";
});

// Swagger
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "PIGI-PT API", Version = "v1.0" });
	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Type = SecuritySchemeType.Http,
		Scheme = "bearer",
		BearerFormat = "JWT"
	});
	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{ new OpenApiSecurityScheme { Reference = new OpenApiReference 
			{ Type = ReferenceType.SecurityScheme, Id = "Bearer" } 
		}, new string[] { } }
	});
});

// CORS
builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(builder =>
		builder
			.AllowAnyOrigin()
			.AllowAnyMethod()
			.AllowAnyHeader()
	);
});

// Autenticación JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.Authority = jwtSettings["Authority"];
		options.Audience = jwtSettings["Audience"];
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidIssuer = jwtSettings["Issuer"],
			ValidAudience = jwtSettings["Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(jwtSettings["Secret"])
			)
		};
	});

var app = builder.Build();

// Pipeline HTTP
if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseMiddleware<TenantResolutionMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
```

## Convenciones REST

| Método | Ruta | Acción | Status Code |
|--------|------|--------|------------|
| GET | /api/v1/tickets | Listar | 200 |
| GET | /api/v1/tickets/{id} | Obtener | 200 / 404 |
| POST | /api/v1/tickets | Crear | 201 / 400 |
| PUT | /api/v1/tickets/{id} | Actualizar completo | 200 / 404 |
| PATCH | /api/v1/tickets/{id} | Actualizar parcial | 200 / 404 |
| DELETE | /api/v1/tickets/{id} | Eliminar | 204 / 404 |

## Seguridad

1. **JWT Bearer**: Autenticación en cada request
2. **Autorización por Rol**: [Authorize(Roles = "Admin")]
3. **Tenant Isolation**: Cada usuario solo ve su tenant
4. **HTTPS**: Requerido en producción
5. **CORS**: Configurado con orígenes permitidos

## Documentación API

Ver `appsettings.json` para versión y metadata de Swagger.

## Próximas Mejoras

- [ ] Implementar Rate Limiting
- [ ] Agregar caching de responses
- [ ] Implementar paging en list endpoints
- [ ] Agregar filtering y sorting
- [ ] Implementar request compression
- [ ] Agregar monitoring/observability
