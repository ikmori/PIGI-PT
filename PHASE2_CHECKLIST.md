# ✅ CHECKLIST FASE 2 - APPLICATION + INFRASTRUCTURE

## 📋 RESUMEN ESTADO ACTUAL

**Fase 1 (Domain):** ✅ COMPLETADA  
**Fase 2 (Application + Infrastructure):** 🔄 EN PROGRESO (2A ✅ + 2B ✅)  
**Fase 3 (API + WebApp):** ⏳ PENDIENTE  

---

## 🔧 PHASE 2A: INFRASTRUCTURE LAYER

### Database & EF Core Configuration

- [x] **Crear PigiPtDbContext**
  ```csharp
  // PIGI-PT-Infraestructure/Persistence/PigiPtDbContext.cs
  public class PigiPtDbContext : DbContext
  {
	  public DbSet<Ticket> Tickets { get; set; }
	  public DbSet<Inquilino> Inquilinos { get; set; }
	  public DbSet<Usuario> Usuarios { get; set; }
	  public DbSet<RiesgoOperacional> RiesgosOperacionales { get; set; }
	  public DbSet<Categoria> Categorias { get; set; }

	  protected override void OnModelCreating(ModelBuilder modelBuilder)
	  {
		  // Aplicar todas las configuraciones
		  modelBuilder.ApplyConfigurationsFromAssembly(typeof(PigiPtDbContext).Assembly);

		  // Multi-tenancy query filter (opcional)
		  // modelBuilder.Entity<InquilinoEntity>()
		  //     .HasQueryFilter(e => e.InquilinoId == _currentTenantId);
	  }
  }
  ```

- [x] **Crear EntityTypeConfigurations**
  ```csharp
  // PIGI-PT-Infraestructure/Persistence/Configurations/
  // TicketConfiguration.cs
  public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
  {
	  public void Configure(EntityTypeBuilder<Ticket> builder)
	  {
		  builder.HasKey(t => t.Id);

		  // ValueObject converters
		  builder.Property(t => t.Estado)
			  .HasConversion(
				  v => v.Valor,
				  v => EstadoTicket.Create(v)
			  );

		  builder.Property(t => t.Prioridad)
			  .HasConversion(
				  v => v.Valor,
				  v => NivelPrioridad.Create(v)
			  );

		  // Índices
		  builder.HasIndex(t => new { t.InquilinoId, t.CreatedAt });
		  builder.HasIndex(t => t.Estado);
	  }
  }
  ```

  - [x] `InquilinoConfiguration.cs` - Mapeo de Inquilino
  - [x] `UsuarioConfiguration.cs` - Mapeo de Usuario con Rol converter
  - [x] `RiesgoOperacionalConfiguration.cs` - Mapeo de Riesgos
  - [x] `CategoriaConfiguration.cs` - Mapeo de Categoría
  - [x] `RegistroDeHistorialConfiguration.cs` - Mapeo de Historial

- [x] **Crear Value Object Converters**
  ```csharp
  // En cada Configuration:
  builder.Property(e => e.Rol)
	  .HasConversion(
		  v => v.Valor, // To database
		  v => Rol.Create(v) // From database
	  );
  ```

- [ ] **EF Core Migrations**
  ```bash
  # Desarrollo local con LocalDB (incluido en Visual Studio)
  dotnet ef migrations add InitialCreate -p PIGI-PT-Infraestructure
  dotnet ef database update
  ```
  > La base de datos `PigiPtDb` se creará automáticamente en LocalDB.
  > Para producción, se migrará a Azure SQL Database cambiando solo el connection string.

---

## 🔧 PHASE 2B: REPOSITORIES & SPECIFICATIONS

### BaseRepository & IRepository

- [x] **Crear Specification Pattern base** (ya existente en Fase 1)
  ```csharp
  // PIGI-PT-Domain/Specifications/Specification.cs
  public abstract class Specification<T> where T : BaseEntity
  {
	  public Expression<Func<T, bool>> Criteria { get; protected set; }
	  public List<Expression<Func<T, object>>> Includes { get; } = new();
	  public Expression<Func<T, object>> OrderBy { get; protected set; }
	  public bool IsPagingEnabled { get; protected set; }
	  public int PageSize { get; protected set; }
	  public int PageIndex { get; protected set; }

	  protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
	  {
		  Includes.Add(includeExpression);
	  }
  }
  ```

- [x] **Crear Specifications para cada agregado**
  ```csharp
  // PIGI-PT-Domain/Specifications/Ticket/
  public class TicketsByInquilinoIdSpec : Specification<Ticket>
  {
	  public TicketsByInquilinoIdSpec(Guid inquilinoId)
	  {
		  Criteria = t => t.InquilinoId == inquilinoId;
		  OrderBy = t => t.CreatedAt;
		  AddInclude(t => t.Asignado); // Si hay FK
	  }
  }

  public class ActiveTicketsSpec : Specification<Ticket>
  {
	  public ActiveTicketsSpec(Guid inquilinoId)
	  {
		  Criteria = t => t.InquilinoId == inquilinoId 
					  && t.Estado != EstadoTicket.Resuelto
					  && t.Estado != EstadoTicket.Cancelado;
		  OrderBy = t => t.Prioridad;
	  }
  }
  ```

  - [x] `TicketsByInquilinoIdSpec`
  - [x] `ActiveTicketsSpec`
  - [x] `PaginatedTicketsSpec`
  - [x] `InquilinoByDominioSpec`, `ActiveInquilinosSpec`
  - [x] `UsuarioByEmailSpec`, `UsuarioByUserNameSpec`, `UsuariosByInquilinoSpec`, `ActiveUsuariosByInquilinoSpec`
  - [x] `RiesgosByInquilinoSpec`, `RiesgosRequierenRevisionSpec`
  - [x] `CategoriasByInquilinoSpec`, `CategoriaByNombreSpec`

- [x] **Crear BaseRepository**
  ```csharp
  // PIGI-PT-Infraestructure/Persistence/Repositories/BaseRepository.cs
  public abstract class BaseRepository<T> : IRepository<T> where T : BaseEntity
  {
	  protected readonly PigiPtDbContext _context;

	  public async Task<T> GetByIdAsync(Guid id)
		  => await _context.Set<T>().FindAsync(id);

	  public async Task<List<T>> GetBySpecificationAsync(Specification<T> spec)
		  => await ApplySpecification(spec).ToListAsync();

	  public async Task AddAsync(T entity)
	  {
		  await _context.Set<T>().AddAsync(entity);
	  }

	  public async Task UpdateAsync(T entity)
	  {
		  _context.Set<T>().Update(entity);
		  await Task.CompletedTask;
	  }

	  private IQueryable<T> ApplySpecification(Specification<T> spec)
	  {
		  var query = _context.Set<T>().AsQueryable();

		  if (spec.Criteria != null)
			  query = query.Where(spec.Criteria);

		  query = spec.Includes.Aggregate(query,
			  (current, include) => current.Include(include));

		  if (spec.OrderBy != null)
			  query = query.OrderBy(spec.OrderBy);

		  if (spec.IsPagingEnabled)
			  query = query.Skip(spec.PageIndex * spec.PageSize)
						  .Take(spec.PageSize);

		  return query;
	  }
  }
  ```

- [x] **Crear Repository implementations**
  ```csharp
  // PIGI-PT-Infraestructure/Persistence/Repositories/TicketRepository.cs
  public class TicketRepository : BaseRepository<Ticket>, ITicketRepository
  {
	  public TicketRepository(PigiPtDbContext context) : base(context) { }

	  // Métodos específicos de Ticket si es necesario
  }
  ```

  - [x] `TicketRepository`
  - [x] `InquilinoRepository`
  - [x] `UsuarioRepository`
  - [x] `RiesgoOperacionalRepository`
  - [x] `CategoriaRepository`

- [x] **Crear IUnitOfWork** (interfaz ya existente + implementación creada)
  ```csharp
  // PIGI-PT-Infraestructure/Persistence/UnitOfWork.cs
  public interface IUnitOfWork : IDisposable
  {
	  ITicketRepository Tickets { get; }
	  IInquilinoRepository Inquilinos { get; }
	  IUsuarioRepository Usuarios { get; }
	  Task<int> SaveChangesAsync();
	  Task<int> SaveChangesAsync(CancellationToken ct);
  }

  public class UnitOfWork : IUnitOfWork
  {
	  private readonly PigiPtDbContext _context;

	  public ITicketRepository Tickets { get; }
	  public IInquilinoRepository Inquilinos { get; }
	  // ...

	  public async Task<int> SaveChangesAsync()
	  {
		  return await _context.SaveChangesAsync();
	  }
  }
  ```

---

## 🔧 PHASE 2C: PORTS & INTERFACES

### Application Ports (Interfaces)

- [ ] **Crear interfaces en Application/Ports**
  ```csharp
  // PIGI-PT-Application/Ports/Persistence/IRepository.cs
  public interface IRepository<T> where T : BaseEntity
  {
	  Task<T> GetByIdAsync(Guid id);
	  Task<List<T>> GetBySpecificationAsync(Specification<T> spec);
	  Task AddAsync(T entity);
	  Task UpdateAsync(T entity);
	  Task DeleteAsync(T entity);
  }

  // PIGI-PT-Application/Ports/Persistence/ITicketRepository.cs
  public interface ITicketRepository : IRepository<Ticket>
  {
	  Task<List<Ticket>> GetActiveByInquilinoAsync(Guid inquilinoId);
  }
  ```

  - [ ] `IRepository<T>`
  - [ ] `ITicketRepository`
  - [ ] `IInquilinoRepository`
  - [ ] `IUsuarioRepository`
  - [ ] `IRiesgoOperacionalRepository`
  - [ ] `ICategoriaRepository`

- [ ] **Crear Service Ports**
  ```csharp
  // PIGI-PT-Application/Ports/Services/IIAService.cs
  public interface IIAService
  {
	  Task<ClassificationResult> ClassifyAsync(string description, CancellationToken ct = default);
	  Task<string> SanitizeAsync(string description, CancellationToken ct = default);
  }

  // PIGI-PT-Application/Ports/Services/INotificationService.cs
  public interface INotificationService
  {
	  Task SendEmailAsync(string to, string subject, string body);
	  Task SendPushNotificationAsync(Guid userId, string message);
  }
  ```

  - [ ] `IIAService` - Sanitización & Clasificación
  - [ ] `INotificationService` - Email/Push
  - [ ] `IAuthenticationService` - JWT, validación
  - [ ] `IDateTimeProvider` - Para tests

---

## 🔧 PHASE 2D: APPLICATION LAYER - COMMANDS & QUERIES

### Commands

- [ ] **Crear Command base**
  ```csharp
  // PIGI-PT-Application/Commands/ICommand.cs
  public interface ICommand : IRequest<Unit> { }
  public interface ICommand<out TResponse> : IRequest<TResponse> { }
  ```

- [ ] **Crear Commands para Ticket**
  ```csharp
  // PIGI-PT-Application/Commands/Ticket/CreateTicket/
  public class CreateTicketCommand : ICommand<TicketDto>
  {
	  public Guid InquilinoId { get; set; }
	  public string Titulo { get; set; }
	  public string Descripcion { get; set; }
	  public Guid UserId { get; set; }
  }

  public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, TicketDto>
  {
	  private readonly ITicketRepository _repo;
	  private readonly IMapper _mapper;
	  private readonly IUnitOfWork _unitOfWork;

	  public async Task<TicketDto> Handle(CreateTicketCommand request, CancellationToken ct)
	  {
		  var ticket = new Ticket(
			  request.InquilinoId,
			  request.Titulo,
			  request.Descripcion,
			  request.UserId
		  );

		  await _repo.AddAsync(ticket);
		  await _unitOfWork.SaveChangesAsync(ct);

		  return _mapper.Map<TicketDto>(ticket);
	  }
  }
  ```

  - [ ] `CreateTicketCommand` & Handler
  - [ ] `ClassifyTicketCommand` & Handler
  - [ ] `AssignTicketCommand` & Handler
  - [ ] `ResolveTicketCommand` & Handler
  - [ ] Similar para otros agregados

### Queries

- [ ] **Crear Query base**
  ```csharp
  // PIGI-PT-Application/Queries/IQuery.cs
  public interface IQuery<out TResponse> : IRequest<TResponse> { }
  ```

- [ ] **Crear Queries para Ticket**
  ```csharp
  public class GetTicketByIdQuery : IQuery<TicketDto>
  {
	  public Guid TicketId { get; set; }
  }

  public class GetTicketByIdQueryHandler : IRequestHandler<GetTicketByIdQuery, TicketDto>
  {
	  private readonly ITicketRepository _repo;
	  private readonly IMapper _mapper;

	  public async Task<TicketDto> Handle(GetTicketByIdQuery request, CancellationToken ct)
	  {
		  var ticket = await _repo.GetByIdAsync(request.TicketId);
		  if (ticket == null)
			  throw new ApplicationException($"Ticket {request.TicketId} not found");

		  return _mapper.Map<TicketDto>(ticket);
	  }
  }
  ```

  - [ ] `GetTicketByIdQuery` & Handler
  - [ ] `GetTicketsByInquilinoQuery` & Handler
  - [ ] `GetActiveTicketsQuery` & Handler
  - [ ] Similar para otros agregados

---

## 🔧 PHASE 2E: VALIDATION & MAPPING

### FluentValidation

- [ ] **Crear Validators**
  ```csharp
  // PIGI-PT-Application/Validations/Ticket/
  public class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
  {
	  public CreateTicketCommandValidator()
	  {
		  RuleFor(x => x.Titulo)
			  .NotEmpty().WithMessage("Título es requerido")
			  .MaximumLength(200).WithMessage("Máximo 200 caracteres");

		  RuleFor(x => x.Descripcion)
			  .NotEmpty().WithMessage("Descripción es requerida")
			  .MinimumLength(10)
			  .MaximumLength(5000);

		  RuleFor(x => x.InquilinoId)
			  .NotEmpty().WithMessage("InquilinoId es requerido");
	  }
  }
  ```

  - [ ] `CreateTicketCommandValidator`
  - [ ] `CreateInquilinoCommandValidator`
  - [ ] `CreateUsuarioCommandValidator`
  - [ ] Similar para otros

### AutoMapper

- [ ] **Crear MappingProfile**
  ```csharp
  // PIGI-PT-Application/Mappings/MappingProfile.cs
  public class MappingProfile : Profile
  {
	  public MappingProfile()
	  {
		  // Ticket
		  CreateMap<Ticket, TicketDto>()
			  .ForMember(dest => dest.Estado, 
				  opt => opt.MapFrom(src => src.Estado.Valor))
			  .ForMember(dest => dest.Prioridad,
				  opt => opt.MapFrom(src => src.Prioridad.Valor));

		  CreateMap<CreateTicketCommand, Ticket>();

		  // ValueObjects
		  CreateMap<EstadoTicket, EstadoTicketDto>()
			  .ForMember(d => d.Valor, o => o.MapFrom(s => s.Valor));

		  CreateMap<NivelPrioridad, NivelPrioridadDto>()
			  .ForMember(d => d.Valor, o => o.MapFrom(s => s.Valor));

		  CreateMap<Rol, RolDto>()
			  .ForMember(d => d.Valor, o => o.MapFrom(s => s.Valor));
	  }
  }
  ```

  - [ ] Mappings para Ticket
  - [ ] Mappings para Inquilino
  - [ ] Mappings para Usuario
  - [ ] Mappings para RiesgoOperacional
  - [ ] Mappings para Categoria
  - [ ] Mappings para ValueObjects

---

## 🔧 PHASE 2F: EVENT HANDLERS

### Domain Event Handlers

- [ ] **Crear handlers para TicketCreadoEvent**
  ```csharp
  // PIGI-PT-Application/EventHandlers/Ticket/
  public class TicketCreadoEventHandler : INotificationHandler<TicketCreadoEvent>
  {
	  private readonly IBackgroundJobClient _backgroundJobClient;
	  private readonly ILogger<TicketCreadoEventHandler> _logger;

	  public async Task Handle(TicketCreadoEvent notification, CancellationToken ct)
	  {
		  _logger.LogInformation($"Ticket {notification.TicketId} creado");

		  // Enqueue Hangfire job para sanitización
		  _backgroundJobClient.Enqueue<ISanitizationService>(
			  s => s.SanitizeTicketAsync(
				  notification.TicketId,
				  notification.DescripcionOriginal
			  )
		  );

		  await Task.CompletedTask;
	  }
  }
  ```

  - [ ] `TicketCreadoEventHandler` → Enqueue sanitización Hangfire
  - [ ] `TicketClasificadoEventHandler` → Notificar operador
  - [ ] `OperadorAsignadoEventHandler` → Notificar asignado
  - [ ] `TicketResueltoEventHandler` → Actualizar métricas

- [ ] **Handlers para Inquilino**
  - [ ] `InquilinoRegistradoEventHandler`
  - [ ] `PrivacidadIAModificadaEventHandler` → Validar consentimiento
  - [ ] `InquilinoSuspendidoEventHandler` → Suspender acceso

- [ ] **Handlers para Usuario**
  - [ ] `UsuarioCreadoEventHandler` → Enviar email bienvenida
  - [ ] `RolModificadoEventHandler` → Auditar cambio
  - [ ] `UsuarioDesactivadoEventHandler` → Revoke tokens

---

## 🔧 PHASE 2G: DEPENDENCY INJECTION

- [ ] **Crear registro en Program.cs**
  ```csharp
  // PIGI-PT-API/Program.cs

  // MediatR
  builder.Services.AddMediatR(typeof(CreateTicketCommandHandler));

  // Repositories & UnitOfWork
  builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
  builder.Services.AddScoped<ITicketRepository, TicketRepository>();
  builder.Services.AddScoped<IInquilinoRepository, InquilinoRepository>();
  // ... etc

  // Services
  builder.Services.AddScoped<IIAService, IAServiceAdapter>();
  builder.Services.AddScoped<INotificationService, NotificationService>();

  // Validators
  builder.Services.AddFluentValidationAutoValidation();
  builder.Services.AddValidatorsFromAssembly(typeof(CreateTicketCommandValidator).Assembly);

  // AutoMapper
  builder.Services.AddAutoMapper(typeof(MappingProfile));

  // DbContext - Desarrollo local con LocalDB
  builder.Services.AddDbContext<PigiPtDbContext>(options =>
	  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
  );
  ```

  **Connection Strings por entorno:**
  ```json
  // appsettings.Development.json (LocalDB)
  {
    "ConnectionStrings": {
      "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=PigiPtDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
    }
  }

  // appsettings.Production.json (Azure SQL - cuando se contrate)
  {
    "ConnectionStrings": {
      "DefaultConnection": "Server=tcp:pigi-pt-server.database.windows.net,1433;Database=PigiPtDb;User ID=<user>;Password=<password>;Encrypt=True;TrustServerCertificate=False;"
    }
  }
  ```

---

## ✅ PHASE 2 CHECKLIST

### Infrastructure
- [x] PigiPtDbContext creado
- [x] EntityTypeConfigurations creadas (6: Ticket, Inquilino, Usuario, RiesgoOperacional, Categoria, RegistroDeHistorial)
- [x] Value Object Converters implementados
- [ ] Migrations ejecutadas
- [x] Repositories creados (5 + BaseRepository)
- [x] UnitOfWork implementado
- [x] Specification Pattern implementado (14 specs)

### Application
- [ ] Command base & Command handlers (10+)
- [ ] Query base & Query handlers (10+)
- [ ] FluentValidation validators (10+)
- [ ] AutoMapper MappingProfile
- [ ] Domain event handlers (10+)
- [ ] DTOs creados

### Dependency Injection
- [ ] MediatR registrado
- [ ] Repositories registrados
- [ ] Services registrados
- [ ] Validators registrados
- [ ] AutoMapper registrado
- [ ] DbContext registrado

### Testing
- [ ] Unit tests para handlers
- [ ] Integration tests con BD
- [ ] Specification pattern tests

---

## 📊 ESTIMACIÓN DE TIEMPO

| Tarea | Horas | Completado |
|---|---|---|
| EF Core & Migrations | 2-3 | ✅ |
| Repositories & Specifications | 3-4 | ✅ |
| Commands & Handlers | 4-6 | ⏳ |
| Queries & Handlers | 3-4 | ⏳ |
| Validation & Mapping | 2-3 | ⏳ |
| Event Handlers | 3-4 | ⏳ |
| Unit Tests | 4-6 | ⏳ |
| Integration Tests | 3-4 | ⏳ |
| **TOTAL** | **24-34** | ⏳ |

---

## 🚀 SIGUIENTES PASOS DESPUÉS DE PHASE 2

Una vez Phase 2 esté completa:

1. ✅ Domain completado
2. ✅ Application + Infrastructure completados
3. ⏳ **Phase 3: API Controllers**
   - REST endpoints para CRUD
   - Error handling middleware
   - API documentation (Swagger)
4. ⏳ **Phase 3b: Authentication & Authorization**
   - JWT tokens
   - Claims-based authorization
   - Role-based access control (RBAC)
5. ⏳ **Phase 3c: Hangfire Jobs**
   - Job para sanitización de tickets
   - Job para clasificación por IA
   - Scheduled jobs para revisión de riesgos
6. ⏳ **Phase 4: WebApp (Blazor)**
   - UI componentes
   - State management
   - Real-time updates

---

**Documento:** `PHASE2_CHECKLIST.md`  
**Versión:** 1.0  
**Status:** ⏳ PENDIENTE  
**Target Start:** Después de Fase 1 completada ✅
