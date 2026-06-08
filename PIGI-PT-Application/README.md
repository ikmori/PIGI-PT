# Application Layer (Capa de Aplicación)

**Estado:** ⏳ FASE 2 - Pendiente de implementación  
**Última actualización:** Enero 2024  
**Dependencias:** Requiere PIGI-PT-Domain (✅ Completada) + PIGI-PT-Infrastructure (⏳ Pendiente)

## Propósito
La capa de **Application** (o Capa de Aplicación) es responsable de:

1. **Orquestación**: Coordinar la ejecución de casos de uso
2. **CQRS**: Implementar patrón Command Query Responsibility Segregation
3. **DTOs**: Transformar agregados en objetos transferibles
4. **Validación**: Aplicar reglas de validación (FluentValidation)
5. **Ports/Interfaces**: Definir contratos para servicios externos
6. **Casos de Uso**: Encapsular la lógica de negocio aplicativa

## 📋 Status de Implementación

| Componente | Estado | Notas |
|-----------|--------|-------|
| **Commands** | ⏳ Pendiente | Se crearán después de completar Infrastructure (2.1-2.2) |
| **Queries** | ⏳ Pendiente | Se crearán después de completar Infrastructure (2.1-2.2) |
| **DTOs** | ⏳ Pendiente | Se crearán en tarea 2.3 |
| **Validators** | ⏳ Pendiente | Se crearán con DTOs en tarea 2.3 |
| **AutoMapper Profiles** | ⏳ Pendiente | Se crearán en tarea 2.5 |
| **Event Handlers** | ⏳ Pendiente | Se crearán en tarea 2.6 |
| **DI Configuration** | ⏳ Pendiente | Se completará en tarea 2.7 |

## 🗺️ Guía para Fase 2

Para instrucciones detalladas sobre cómo implementar esta capa, consulta:
- **[ROADMAP_FUTURO.md](../ROADMAP_FUTURO.md#fase-2-application--infrastructure-estimado-25-35-horas)** - Desglose completo de tareas
- **[PHASE2_CHECKLIST.md](../PHASE2_CHECKLIST.md)** - Checklist práctico de implementación
- **[DOMAIN_QUICK_REFERENCE.md](../DOMAIN_QUICK_REFERENCE.md)** - Referencia rápida de agregados y eventos

## Arquitectura CQRS

### Command Query Responsibility Segregation

```
┌─ Commands (Modifican estado)       ┌─ Queries (Solo lectura)
│  • CreateTicketCommand             │  • GetTicketByIdQuery
│  • ClassifyTicketCommand           │  • GetTicketsByTenantQuery
│  • AssignOperatorCommand           │  • GetOperatorTicketsQuery
│  • ResolveTicketCommand            │  • GetCriticalTicketsQuery
│                                     │
├─> CommandHandler                   ├─> QueryHandler
│   (persiste en BD)                 │   (lee de BD)
│   (publica eventos)                │   (mapea DTOs)
│   (sin retorno o simple result)    │   (retorna datos)
```

## Estructura de Carpetas

```
PIGI-PT-Application/
├── Commands/                        # Acciones que modifican estado
│   ├── Ticket/
│   │   ├── CreateTicket/
│   │   │   ├── CreateTicketCommand.cs         # Definición
│   │   │   ├── CreateTicketCommandHandler.cs  # Lógica
│   │   │   ├── CreateTicketValidator.cs       # Validación
│   │   │   └── CreateTicketResponse.cs        # Respuesta
│   │   ├── ClassifyTicket/
│   │   │   └── ...
│   │   ├── AssignOperator/
│   │   │   └── ...
│   │   └── ResolveTicket/
│   │       └── ...
│   ├── Inquilino/
│   │   ├── CreateInquilino/
│   │   ├── UpdateIAPrivacy/
│   │   └── SuspendInquilino/
│   ├── Usuario/
│   │   ├── CreateUsuario/
│   │   ├── UpdatePassword/
│   │   ├── ChangeRole/
│   │   └── DeactivateUser/
│   └── README.md
│
├── Queries/                         # Consultas de solo lectura
│   ├── Ticket/
│   │   ├── GetTicketById/
│   │   │   ├── GetTicketByIdQuery.cs
│   │   │   ├── GetTicketByIdQueryHandler.cs
│   │   │   └── TicketDto.cs
│   │   ├── GetTicketsByTenant/
│   │   ├── GetOperatorTickets/
│   │   └── GetCriticalTickets/
│   ├── Usuario/
│   │   ├── GetActiveUsersByTenant/
│   │   └── GetUserByEmail/
│   ├── Categoria/
│   │   └── GetCategoriesByTenant/
│   └── README.md
│
├── DTOs/                            # Data Transfer Objects
│   ├── Ticket/
│   │   ├── CreateTicketDto.cs
│   │   ├── TicketDto.cs
│   │   ├── TicketDetailDto.cs
│   │   └── README.md
│   ├── Inquilino/
│   │   ├── CreateInquilinoDto.cs
│   │   ├── InquilinoDto.cs
│   │   └── README.md
│   ├── Usuario/
│   │   ├── CreateUsuarioDto.cs
│   │   ├── UsuarioDto.cs
│   │   └── README.md
│   └── README.md
│
├── Mappers/                         # Mapeo Entity ↔ DTO
│   ├── TicketMappingProfile.cs
│   ├── InquilinoMappingProfile.cs
│   ├── UsuarioMappingProfile.cs
│   └── README.md
│
├── Ports/                           # Interfaces para Infraestructura
│   ├── Repositories/
│   │   ├── ITicketRepository.cs
│   │   ├── IInquilinoRepository.cs
│   │   ├── IUsuarioRepository.cs
│   │   ├── IRepository.cs (genérico)
│   │   └── README.md
│   ├── Services/
│   │   ├── IIAService.cs            # Integración API Python/IA
│   │   ├── IEmailService.cs
│   │   ├── INotificationService.cs
│   │   ├── ISanitizationService.cs
│   │   └── README.md
│   ├── Infrastructure/
│   │   ├── IUnitOfWork.cs
│   │   ├── IHangfireService.cs
│   │   └── README.md
│   └── README.md
│
├── EventHandlers/                   # Manejadores de Eventos de Dominio
│   ├── Ticket/
│   │   ├── TicketCreadoEventHandler.cs
│   │   ├── TicketClasificadoEventHandler.cs
│   │   └── README.md
│   ├── Inquilino/
│   │   └── PrivacidadIAModificadaEventHandler.cs
│   ├── Usuario/
│   │   ├── UsuarioCreadoEventHandler.cs
│   │   └── README.md
│   └── README.md
│
├── Validations/                     # Validadores FluentValidation
│   ├── Ticket/
│   │   ├── CreateTicketValidator.cs
│   │   ├── ClassifyTicketValidator.cs
│   │   └── README.md
│   ├── Usuario/
│   │   ├── CreateUsuarioValidator.cs
│   │   └── README.md
│   └── README.md
│
├── Services/                        # Application Services (orquestadores)
│   ├── TicketApplicationService.cs
│   ├── InquilinoApplicationService.cs
│   ├── UsuarioApplicationService.cs
│   └── README.md
│
├── Common/                          # Utilidades compartidas
│   ├── Behaviors/
│   │   ├── ValidationBehavior.cs    # MediatR pipeline)
│   │   ├── LoggingBehavior.cs
│   │   └── README.md
│   ├── Exceptions/
│   │   ├── ApplicationException.cs
│   │   └── README.md
│   ├── Results/
│   │   ├── Result.cs                # Result pattern alternativo
│   │   └── README.md
│   └── README.md
│
├── Configuration/
│   ├── ApplicationDependencyInjection.cs
│   └── README.md
│
└── README.md                        # Este archivo
```

## Componentes Principales

### 1. Commands
Acciones que **modifican estado**. Siempre retornan algo o void.

**Ejemplo**:
```csharp
public class CreateTicketCommand : IRequest<TicketDto>
{
	public Guid InquilinoId { get; set; }
	public string Titulo { get; set; }
	public string Descripcion { get; set; }
	public Guid UserId { get; set; }
}

public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, TicketDto>
{
	private readonly ITicketRepository _repository;
	private readonly IMapper _mapper;

	public async Task<TicketDto> Handle(
		CreateTicketCommand request, 
		CancellationToken cancellationToken)
	{
		var ticket = new Ticket(
			request.InquilinoId,
			request.Titulo,
			request.Descripcion,
			request.UserId
		);

		await _repository.AddAsync(ticket);
		return _mapper.Map<TicketDto>(ticket);
	}
}
```

### 2. Queries
Acciones de **solo lectura**. No modifican estado.

**Ejemplo**:
```csharp
public class GetTicketByIdQuery : IRequest<TicketDto>
{
	public Guid TicketId { get; set; }
}

public class GetTicketByIdQueryHandler : IRequestHandler<GetTicketByIdQuery, TicketDto>
{
	private readonly ITicketRepository _repository;
	private readonly IMapper _mapper;

	public async Task<TicketDto> Handle(
		GetTicketByIdQuery request, 
		CancellationToken cancellationToken)
	{
		var ticket = await _repository.GetByIdAsync(request.TicketId);

		if (ticket == null)
			throw new ApplicationException($"Ticket {request.TicketId} no encontrado.");

		return _mapper.Map<TicketDto>(ticket);
	}
}
```

### 3. DTOs (Data Transfer Objects)
Objetos para transferir datos entre capas.

**Tipos**:
- **CreateXxxDto**: Input para crear entidades (desde HTTP body)
- **XxxDto**: Output representación simple de la entidad
- **XxxDetailDto**: Output representación completa con relaciones

**Ejemplo**:
```csharp
public class CreateTicketDto
{
	public string Titulo { get; set; }
	public string Descripcion { get; set; }
}

public class TicketDto
{
	public Guid Id { get; set; }
	public string Titulo { get; set; }
	public string DescripcionOriginal { get; set; }
	public string Estado { get; set; }
	public string Prioridad { get; set; }
	public DateTime CreatedAt { get; set; }
}
```

### 4. Mappers (AutoMapper)
Transforman entre Entities y DTOs.

**Ejemplo**:
```csharp
public class TicketMappingProfile : Profile
{
	public TicketMappingProfile()
	{
		// Ticket → TicketDto
		CreateMap<Ticket, TicketDto>()
			.ForMember(dest => dest.Estado, 
				opt => opt.MapFrom(src => src.Estado.ToString()))
			.ForMember(dest => dest.Prioridad, 
				opt => opt.MapFrom(src => src.Prioridad.ToString()));

		// CreateTicketDto → Ticket (en Application Service)
		CreateMap<CreateTicketDto, Ticket>()
			.ConstructUsing((src, ctx) => 
				new Ticket(
					ctx.Items["TenantId"] as Guid? ?? Guid.Empty,
					src.Titulo,
					src.Descripcion,
					ctx.Items["UserId"] as Guid? ?? Guid.Empty
				));
	}
}
```

### 5. Ports (Interfaces)
Contratos que definen qué servicios necesita la aplicación.

**Ejemplo**:
```csharp
// En Application/Ports/Repositories/ITicketRepository.cs
public interface ITicketRepository : IRepository<Ticket>
{
	Task<Ticket> GetByIdAsync(Guid id);
	Task<List<Ticket>> GetBySpecificationAsync(Specification<Ticket> spec);
	Task AddAsync(Ticket ticket);
	Task UpdateAsync(Ticket ticket);
	Task DeleteAsync(Guid id);
}

// La implementación está en Infrastructure
// (no en Application layer)
```

### 6. Validadores (FluentValidation)
Validan requests antes de procesarlos.

**Ejemplo**:
```csharp
public class CreateTicketValidator : AbstractValidator<CreateTicketCommand>
{
	public CreateTicketValidator()
	{
		RuleFor(x => x.Titulo)
			.NotEmpty().WithMessage("El título es obligatorio.")
			.MaximumLength(200).WithMessage("El título no puede exceder 200 caracteres.");

		RuleFor(x => x.Descripcion)
			.NotEmpty().WithMessage("La descripción es obligatoria.")
			.MinimumLength(10).WithMessage("La descripción debe tener al menos 10 caracteres.")
			.MaximumLength(5000).WithMessage("La descripción no puede exceder 5000 caracteres.");

		RuleFor(x => x.UserId)
			.NotEqual(Guid.Empty).WithMessage("El ID del usuario es obligatorio.");
	}
}
```

### 7. Event Handlers
Reaccionan a eventos de dominio.

**Ejemplo**:
```csharp
public class TicketCreadoEventHandler : INotificationHandler<TicketCreadoEvent>
{
	private readonly IHangfireService _hangfireService;

	public async Task Handle(TicketCreadoEvent notification, CancellationToken ct)
	{
		// Encolar sanitización en Hangfire
		_hangfireService.EnqueueSanitization(
			notification.TicketId,
			notification.DescripcionOriginal
		);
	}
}
```

### 8. MediatR Pipeline Behaviors
Interceptores para requests (ej: validación, logging).

**Ejemplo**:
```csharp
public class ValidationBehavior<TRequest, TResponse> 
	: IPipelineBehavior<TRequest, TResponse> 
	where TRequest : IRequest<TResponse>
{
	private readonly IValidator<TRequest> _validator;

	public async Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken ct)
	{
		var validation = await _validator.ValidateAsync(request, ct);

		if (!validation.IsValid)
			throw new ValidationException(validation.Errors);

		return await next();
	}
}
```

## Flujo Completo: Crear Ticket

```
1. API Controller recibe POST /tickets
   ↓
2. Controller deserializa JSON → CreateTicketCommand
   ↓
3. Controller envía: mediator.Send(command)
   ↓
4. MediatR pipeline ejecuta:
   a) ValidationBehavior valida
   b) LoggingBehavior registra request
   ↓
5. CreateTicketCommandHandler maneja comando:
   a) Crea instancia de Ticket (agregado)
   b) Ticket emite TicketCreadoEvent
   c) Repository.AddAsync(ticket)
   ↓
6. Infrastructure (Repository):
   a) Persiste ticket en BD
   b) Extrae eventos de ticket.DomainEvents
   c) MediatR publica TicketCreadoEvent
   ↓
7. MediatR ejecuta handlers de evento:
   a) TicketCreadoEventHandler encola en Hangfire
   b) AuditEventHandler registra en auditoría
   ↓
8. CommandHandler retorna TicketDto
   ↓
9. API Controller retorna HTTP 201 Created
```

## Configuración de DI (Dependency Injection)

```csharp
// En Program.cs o ApplicationDependencyInjection
public static IServiceCollection AddApplicationServices(
	this IServiceCollection services)
{
	// Registrar AutoMapper
	services.AddAutoMapper(typeof(TicketMappingProfile));

	// Registrar MediatR
	services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(
		typeof(CreateTicketCommand).Assembly));

	// Registrar Validators
	services.AddValidatorsFromAssembly(typeof(CreateTicketValidator).Assembly);

	// Registrar MediatR behaviors
	services.AddTransient(typeof(IPipelineBehavior<,>), 
		typeof(ValidationBehavior<,>));
	services.AddTransient(typeof(IPipelineBehavior<,>), 
		typeof(LoggingBehavior<,>));

	return services;
}
```

## Mejores Prácticas

1. **Thin Controllers**: Controllers solo deserializan, llaman MediatR, responden
2. **Rich Domains**: Lógica de negocio en Domain, no en Application
3. **Single Responsibility**: Cada handler hace una cosa
4. **Validación Temprana**: Validar en Command, no en Handler
5. **Errores como Excepciones**: Lanzar DomainException/ApplicationException
6. **DTOs Separados**: CreateXxxDto ≠ XxxDto
7. **No Persistencia Directa**: Application llama Repository, no EF Core

## Próximas Mejoras

- [ ] Implementar Result<T> pattern para evitar excepciones
- [ ] Agregar paging a queries
- [ ] Implementar filtrado dinámico
- [ ] Agregar sorting
- [ ] Crear query specifications con paging
- [ ] Implementar soft delete
- [ ] Agregar auditoría detallada de cambios
