# Infrastructure Layer (Capa de Infraestructura)

**Estado:** ⏳ FASE 2 - Pendiente de implementación  
**Última actualización:** Enero 2024  
**Dependencias:** Requiere PIGI-PT-Domain (✅ Completada)

## Propósito
La capa de **Infrastructure** es responsable de implementar los detalles técnicos definidos por los Ports de Application Layer:

1. **Persistence**: Implementar repositorios con EF Core
2. **External Services**: Integración con APIs externas (Python IA, Email, etc.)
3. **Data Access**: DbContext, migrations, query builders
4. **Configuration**: Hangfire, Redis, Azure SQL
5. **Adapters**: Convertir entre DTOs internos y APIs externas

## Principio Fundamental
**La Infrastructure NO debe contener lógica de negocio**. Solo técnica.

```
Application (Puertos)     Infrastructure (Adaptadores)
	↓                              ↓
ITicketRepository    →    TicketRepository (EF Core)
IIAService           →    IAService (HTTP client → Python)
IEmailService        →    EmailService (SMTP/SendGrid)
IHangfireService     →    HangfireService (Job scheduling)
```

## 📋 Status de Implementación

| Componente | Estado | Notas |
|-----------|--------|-------|
| **DbContext** | ⏳ Pendiente | Se crea en tarea 2.1 |
| **Configurations** | ⏳ Pendiente | Se crean en tarea 2.1 con ValueObject mappings |
| **Repositories** | ⏳ Pendiente | Se crean en tarea 2.2 |
| **Specifications** | ⏳ Pendiente | Se crean en tarea 2.2 |
| **UnitOfWork** | ⏳ Pendiente | Se crea en tarea 2.2 |
| **Migrations** | ⏳ Pendiente | Se generan en tarea 2.1 |
| **External Services** | ⏳ Pendiente | Se crean en Fase 4 |
| **Hangfire Setup** | ⏳ Pendiente | Se crea en Fase 4 |

## 🗺️ Guía para Fase 2

Para instrucciones detalladas sobre cómo implementar esta capa, consulta:
- **[ROADMAP_FUTURO.md](../ROADMAP_FUTURO.md#fase-2-application--infrastructure-estimado-25-35-horas)** - Desglose completo de tareas
  - **[2.1 - EF Core DbContext](../ROADMAP_FUTURO.md#21-infrastructure-layer---database-estimado-4-5h)** - Crear DbContext, Configurations, Value Object Converters
  - **[2.2 - Repositories & Specifications](../ROADMAP_FUTURO.md#22-infrastructure-layer---repositories-estimado-3-4h)** - Implementar patrón Specification
- **[PHASE2_CHECKLIST.md](../PHASE2_CHECKLIST.md)** - Checklist práctico con ejemplos de código

## Estructura de Carpetas

```
PIGI-PT-Infraestructure/
├── Persistence/                     # EF Core y acceso a datos
│   ├── DbContext/
│   │   ├── PigiPtDbContext.cs       # DbContext principal
│   │   ├── DesignTimeDbContextFactory.cs
│   │   └── README.md
│   ├── Repositories/
│   │   ├── BaseRepository.cs        # Implementación genérica
│   │   ├── TicketRepository.cs
│   │   ├── InquilinoRepository.cs
│   │   ├── UsuarioRepository.cs
│   │   ├── RiesgoOperacionalRepository.cs
│   │   ├── CategoriaRepository.cs
│   │   ├── UnitOfWork.cs            # Coordina transacciones
│   │   └── README.md
│   ├── Migrations/
│   │   ├── 20240101000000_InitialCreate.cs
│   │   ├── 20240102000000_AddDomainEvents.cs
│   │   └── README.md
│   ├── Configurations/
│   │   ├── TicketConfiguration.cs   # EF Core entity mapping
│   │   ├── InquilinoConfiguration.cs
│   │   ├── UsuarioConfiguration.cs
│   │   ├── RiesgoOperacionalConfiguration.cs
│   │   ├── CategoriaConfiguration.cs
│   │   └── README.md
│   └── README.md
│
├── ExternalServices/                # Integraciones externas
│   ├── AI/
│   │   ├── IAService.cs             # Implementación IIAService
│   │   ├── AIClient.cs              # HTTP client
│   │   ├── AIModels.cs              # Request/Response models
│   │   ├── SanitizationService.cs
│   │   └── README.md
│   ├── Email/
│   │   ├── EmailService.cs          # Implementación IEmailService
│   │   ├── EmailTemplates/
│   │   │   ├── TicketCreatedTemplate.html
│   │   │   ├── TicketResolvedTemplate.html
│   │   │   └── README.md
│   │   ├── EmailModels.cs
│   │   └── README.md
│   ├── Notifications/
│   │   ├── NotificationService.cs   # Implementación INotificationService
│   │   ├── NotificationHub.cs       # SignalR (opcional)
│   │   └── README.md
│   ├── Azure/
│   │   ├── AzureBlobService.cs      # Azure Blob Storage
│   │   ├── AzureKeyVaultService.cs  # Secrets management
│   │   └── README.md
│   └── README.md
│
├── BackgroundJobs/                  # Hangfire jobs
│   ├── Tickets/
│   │   ├── SanitizeTicketDescriptionJob.cs
│   │   ├── ClassifyTicketJob.cs
│   │   ├── SendTicketResolvedNotificationJob.cs
│   │   └── README.md
│   ├── Maintenance/
│   │   ├── CleanupOldLogsJob.cs
│   │   ├── GenerateReportsJob.cs
│   │   └── README.md
│   ├── HangfireService.cs
│   ├── HangfireConfiguration.cs
│   └── README.md
│
├── Auth/                            # Autenticación y autorización
│   ├── JwtTokenProvider.cs
│   ├── AuthenticationService.cs
│   ├── PermissionService.cs
│   └── README.md
│
├── Caching/                         # Caché (Redis/Memory)
│   ├── CacheService.cs
│   ├── CacheKeyBuilder.cs
│   ├── CacheConfiguration.cs
│   └── README.md
│
├── Logging/                         # Logging y observabilidad
│   ├── LoggerProvider.cs
│   ├── SerilogConfiguration.cs      # Serilog setup
│   ├── ApplicationInsightsService.cs
│   └── README.md
│
├── Common/                          # Utilidades compartidas
│   ├── DateTimeProvider.cs          # Inyectable DateTime
│   ├── GuidProvider.cs              # Inyectable Guid
│   ├── Constants.cs
│   ├── Extensions.cs
│   └── README.md
│
├── Configuration/
│   ├── InfrastructureDependencyInjection.cs
│   ├── AppSettings.cs               # Configuration models
│   └── README.md
│
└── README.md                        # Este archivo
```

## Componentes Principales

### 1. DbContext

```csharp
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
		modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

		// Multi-tenancy: Query filter global
		modelBuilder.Entity<Ticket>()
			.HasQueryFilter(t => t.InquilinoId == _currentTenantId);
		// ... más entities
	}
}
```

### 2. Repository Pattern

```csharp
public interface ITicketRepository : IRepository<Ticket>
{
	Task<Ticket> GetByIdAsync(Guid id);
	Task<List<Ticket>> GetBySpecificationAsync(Specification<Ticket> spec);
	Task AddAsync(Ticket ticket);
	Task UpdateAsync(Ticket ticket);
	Task DeleteAsync(Guid id);
}

public class TicketRepository : BaseRepository<Ticket>, ITicketRepository
{
	public TicketRepository(PigiPtDbContext context) : base(context) { }

	public async Task<Ticket> GetByIdAsync(Guid id)
	{
		return await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id);
	}

	public async Task<List<Ticket>> GetBySpecificationAsync(
		Specification<Ticket> spec)
	{
		return await ApplySpecification(spec).ToListAsync();
	}

	public async Task AddAsync(Ticket ticket)
	{
		await _context.Tickets.AddAsync(ticket);
	}

	public async Task UpdateAsync(Ticket ticket)
	{
		_context.Tickets.Update(ticket);
	}

	public async Task DeleteAsync(Guid id)
	{
		var ticket = await GetByIdAsync(id);
		if (ticket != null)
			_context.Tickets.Remove(ticket);
	}
}
```

### 3. Unit of Work

```csharp
public interface IUnitOfWork : IDisposable
{
	ITicketRepository Tickets { get; }
	IInquilinoRepository Inquilinos { get; }
	IUsuarioRepository Usuarios { get; }
	// ...más repositorios

	Task<int> SaveChangesAsync();
	Task<bool> BeginTransactionAsync();
	Task<bool> CommitTransactionAsync();
	Task<bool> RollbackTransactionAsync();
}

public class UnitOfWork : IUnitOfWork
{
	private readonly PigiPtDbContext _context;
	private ITicketRepository _ticketRepository;
	private IInquilinoRepository _inquilinoRepository;

	public ITicketRepository Tickets => 
		_ticketRepository ??= new TicketRepository(_context);

	public IInquilinoRepository Inquilinos => 
		_inquilinoRepository ??= new InquilinoRepository(_context);

	public async Task<int> SaveChangesAsync()
	{
		return await _context.SaveChangesAsync();
	}
}
```

### 4. External Service: AI Integration

```csharp
// Puerto (en Application)
public interface IIAService
{
	Task<ClassificationResult> ClassifyIncidentAsync(
		string sanitizedDescription);
	Task<string> SanitizeDescriptionAsync(string originalDescription);
}

// Adaptador (en Infrastructure)
public class IAService : IIAService
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<IAService> _logger;

	public IAService(HttpClient httpClient, ILogger<IAService> logger)
	{
		_httpClient = httpClient;
		_logger = logger;
	}

	public async Task<ClassificationResult> ClassifyIncidentAsync(
		string sanitizedDescription)
	{
		var request = new ClassifyRequest 
		{ 
			Description = sanitizedDescription 
		};

		var response = await _httpClient.PostAsJsonAsync(
			"api/v1/classify",
			request
		);

		if (!response.IsSuccessStatusCode)
			throw new ApplicationException("Failed to classify incident");

		return await response.Content.ReadAsAsync<ClassificationResult>();
	}

	public async Task<string> SanitizeDescriptionAsync(
		string originalDescription)
	{
		var request = new SanitizeRequest 
		{ 
			Description = originalDescription 
		};

		var response = await _httpClient.PostAsJsonAsync(
			"api/v1/sanitize",
			request
		);

		if (!response.IsSuccessStatusCode)
			throw new ApplicationException("Failed to sanitize description");

		var result = await response.Content.ReadAsAsync<SanitizeResponse>();
		return result.SanitizedDescription;
	}
}
```

### 5. Background Jobs: Hangfire

```csharp
public class SanitizeTicketDescriptionJob
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly IIAService _iaService;

	public async Task ExecuteAsync(Guid ticketId)
	{
		var ticket = await _unitOfWork.Tickets.GetByIdAsync(ticketId);
		if (ticket == null)
			return;

		try
		{
			// Sanitizar
			var sanitized = await _iaService.SanitizeDescriptionAsync(
				ticket.DescripcionOriginal
			);

			// Aplicar al ticket
			ticket.AplicarSanitizacion(sanitized);

			// Persistir
			await _unitOfWork.Tickets.UpdateAsync(ticket);
			await _unitOfWork.SaveChangesAsync();
		}
		catch (Exception ex)
		{
			// Hangfire reintenta automáticamente
			throw;
		}
	}
}

// En HangfireService.cs
public class HangfireService : IHangfireService
{
	private readonly IBackgroundJobClient _jobClient;

	public void EnqueueSanitization(Guid ticketId, string description)
	{
		BackgroundJob.Enqueue<SanitizeTicketDescriptionJob>(
			job => job.ExecuteAsync(ticketId)
		);
	}
}
```

### 6. Entity Configuration (EF Core Mapping)

```csharp
public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
	public void Configure(EntityTypeBuilder<Ticket> builder)
	{
		builder.HasKey(t => t.Id);

		// Multi-tenancy
		builder.HasIndex(t => new { t.InquilinoId, t.Id });

		// Propiedades básicas
		builder.Property(t => t.Titulo)
			.HasMaxLength(200)
			.IsRequired();

		builder.Property(t => t.DescripcionOriginal)
			.HasMaxLength(5000)
			.IsRequired();

		builder.Property(t => t.DescripcionSanitizada)
			.HasMaxLength(5000);

		// Value Objects (conversión)
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

		// Auditoría
		builder.Property(t => t.CreatedAt).IsRequired();
		builder.Property(t => t.CreatedBy);
		builder.Property(t => t.ModifiedAt);
		builder.Property(t => t.ModifiedBy);
		builder.Property(t => t.IsActive).HasDefaultValue(true);
	}
}
```

## Dependency Injection

```csharp
public static class InfrastructureDependencyInjection
{
	public static IServiceCollection AddInfrastructureServices(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		// DbContext
		services.AddDbContext<PigiPtDbContext>(options =>
			options.UseSqlServer(
				configuration.GetConnectionString("DefaultConnection"),
				b => b.MigrationsAssembly(typeof(PigiPtDbContext).Assembly.FullName)
			)
		);

		// Repositories
		services.AddScoped<ITicketRepository, TicketRepository>();
		services.AddScoped<IInquilinoRepository, InquilinoRepository>();
		services.AddScoped<IUsuarioRepository, UsuarioRepository>();
		services.AddScoped<IUnitOfWork, UnitOfWork>();

		// External Services
		services.AddScoped<IIAService, IAService>();
		services.AddScoped<IEmailService, EmailService>();
		services.AddScoped<IHangfireService, HangfireService>();

		// Hangfire
		services.AddHangfire(config => config
			.UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection"))
		);
		services.AddHangfireServer();

		// AutoMapper (si no está en Application)
		services.AddAutoMapper(typeof(InfrastructureDependencyInjection));

		return services;
	}
}

// En Program.cs
builder.Services
	.AddApplicationServices()
	.AddInfrastructureServices(builder.Configuration);
```

## Migrations

```csharp
// Crear migration
// dotnet ef migrations add CreateTicketTable

public partial class CreateTicketTable : Migration
{
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.CreateTable(
			name: "Tickets",
			columns: table => new
			{
				Id = table.Column<Guid>(nullable: false),
				InquilinoId = table.Column<Guid>(nullable: false),
				Titulo = table.Column<string>(maxLength: 200, nullable: false),
				DescripcionOriginal = table.Column<string>(maxLength: 5000, nullable: false),
				DescripcionSanitizada = table.Column<string>(maxLength: 5000, nullable: true),
				Estado = table.Column<int>(nullable: false),
				Prioridad = table.Column<int>(nullable: false),
				CreatedAt = table.Column<DateTime>(nullable: false),
				CreatedBy = table.Column<Guid>(nullable: true),
				ModifiedAt = table.Column<DateTime>(nullable: true),
				ModifiedBy = table.Column<Guid>(nullable: true),
				IsActive = table.Column<bool>(nullable: false, defaultValue: true)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_Tickets", x => x.Id);
				table.ForeignKey("FK_Tickets_Inquilinos", x => x.InquilinoId, 
					principalTable: "Inquilinos", principalColumn: "Id");
			}
		);

		migrationBuilder.CreateIndex(
			name: "IX_Tickets_InquilinoId",
			table: "Tickets",
			columns: new[] { "InquilinoId", "Id" }
		);
	}
}
```

## Mejores Prácticas

1. **Repositories son Thin**: No contienen lógica compleja
2. **Specifications en Domain**: Queries complejas se definen en Domain/Specifications
3. **No EF Core en Application**: Application usa Ports, no DbContext
4. **Events en Infrastructure**: Se publican al persistir
5. **Secrets en Key Vault**: Nunca en código
6. **Transactional Boundaries**: UnitOfWork coordina
7. **Async/Await**: Siempre async en I/O

## Próximas Mejoras

- [ ] Implementar query optimization con Includes
- [ ] Agregar query interceptors
- [ ] Implementar soft delete
- [ ] Agregar auditoría automática
- [ ] Implementar Change Tracking
- [ ] Crear seeding para datos iniciales
- [ ] Implementar snapshot loading
- [ ] Agregar performance monitoring
