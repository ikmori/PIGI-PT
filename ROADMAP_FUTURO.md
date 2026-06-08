# 🗺️ ROADMAP FUTURO - PIGI-PT

**Última actualización:** Enero 2024  
**Versión:** 1.0  
**Estado actual:** Fase 1 ✅ Completada | Fase 2 ⏳ Pendiente  

---

## 📍 ESTADO ACTUAL DEL PROYECTO

### ✅ Completado (Fase 1)
- Domain Layer con DDD
  - 5 Agregados completos (Ticket, Inquilino, Usuario, RiesgoOperacional, Categoria)
  - 4 Value Objects (EstadoTicket, EstadoInquilino, NivelPrioridad, Rol)
  - 12 Excepciones de dominio específicas
  - 10 Domain Events enriquecidos
  - Máquinas de estado validadas
  - Auditoría automática en todas las entidades

### ⏳ Pendiente (Fase 2 - Prioritario)
- Infrastructure Layer (EF Core, BD, Repositories)
- Application Layer (Commands, Queries, Handlers)
- Validation & Mapping (FluentValidation, AutoMapper)

### ⏳ Pendiente (Fase 3)
- API Layer (REST Controllers)
- Authentication & Authorization
- Hangfire Background Jobs

### ⏳ Pendiente (Fase 4)
- WebApp (Blazor WASM)
- Real-time features (SignalR)
- Analytics & Reporting

---

## 🚀 FASES DE DESARROLLO

## FASE 2: APPLICATION + INFRASTRUCTURE (ESTIMADO: 25-35 horas)

### 2.1 Infrastructure Layer - Database (Estimado: 4-5h)

**Objetivo:** Mapear entidades domain a base de datos relacional usando EF Core

#### 2.1.1 - EF Core DbContext
```
📁 PIGI-PT-Infraestructure/Persistence/
├── PigiPtDbContext.cs (crear)
├── Migrations/
│   ├── InitialCreate (ejecutar)
│   └── ... (futuras)
└── Configurations/
	├── TicketConfiguration.cs (crear)
	├── InquilinoConfiguration.cs (crear)
	├── UsuarioConfiguration.cs (crear)
	├── RiesgoOperacionalConfiguration.cs (crear)
	└── CategoriaConfiguration.cs (crear)
```

**Tareas:**
- [ ] Crear `PigiPtDbContext` con DbSets para los 5 agregados
- [ ] Implementar `IEntityTypeConfiguration<T>` para cada agregado
- [ ] Crear Value Object converters (Estado → string en BD)
- [ ] Crear primera migration: `dotnet ef migrations add InitialCreate`
- [ ] Ejecutar: `dotnet ef database update`
- [ ] Verificar schema en SQL Server

**Ejemplo:**
```csharp
// PigiPtDbContext.cs
public class PigiPtDbContext : DbContext
{
	public DbSet<Ticket> Tickets { get; set; }
	public DbSet<Inquilino> Inquilinos { get; set; }
	public DbSet<Usuario> Usuarios { get; set; }
	public DbSet<RiesgoOperacional> RiesgosOperacionales { get; set; }
	public DbSet<Categoria> Categorias { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(PigiPtDbContext).Assembly);
	}
}

// TicketConfiguration.cs
public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
	public void Configure(EntityTypeBuilder<Ticket> builder)
	{
		builder.HasKey(t => t.Id);

		builder.Property(t => t.Estado)
			.HasConversion(
				v => v.Valor,
				v => EstadoTicket.Create(v)
			);

		builder.HasIndex(t => new { t.InquilinoId, t.CreatedAt });
	}
}
```

### 2.2 Infrastructure Layer - Repositories (Estimado: 3-4h)

**Objetivo:** Implementar patrón Specification para queries complejas y abstracción de datos

#### 2.2.1 - Specification Pattern
```
📁 PIGI-PT-Domain/Specifications/
├── Specification.cs (crear base)
├── Ticket/
│   ├── ActiveTicketsSpec.cs (crear)
│   ├── TicketsByInquilinoIdSpec.cs (crear)
│   └── PaginatedTicketsSpec.cs (crear)
├── Inquilino/
│   └── ... (similar)
└── ... (otros)
```

#### 2.2.2 - Repositories Implementation
```
📁 PIGI-PT-Infraestructure/Persistence/Repositories/
├── BaseRepository.cs (crear)
├── TicketRepository.cs (crear)
├── InquilinoRepository.cs (crear)
├── UsuarioRepository.cs (crear)
├── RiesgoOperacionalRepository.cs (crear)
└── CategoriaRepository.cs (crear)
```

**Tareas:**
- [ ] Crear clase `Specification<T>` base con Criteria, Includes, OrderBy, Paging
- [ ] Crear `BaseRepository<T>` genérico con CRUD + GetBySpecification
- [ ] Crear 5 repositories específicas heredando de BaseRepository
- [ ] Implementar 3 specifications por agregado (al menos):
  - ByInquilinoId (multi-tenancy)
  - Active/Inactive (estado)
  - Paginated (paginación)
- [ ] Crear `IUnitOfWork` pattern para coordinar transacciones

### 2.3 Application Layer - DTOs & Validators (Estimado: 3-4h)

**Objetivo:** Mapear domain entities a Data Transfer Objects y validar comandos

#### 2.3.1 - DTOs
```
📁 PIGI-PT-Application/DTOs/
├── TicketDto.cs (crear)
├── InquilinoDto.cs (crear)
├── UsuarioDto.cs (crear)
├── RiesgoOperacionalDto.cs (crear)
└── CategoriaDto.cs (crear)
```

#### 2.3.2 - Validators
```
📁 PIGI-PT-Application/Validations/
├── Ticket/
│   ├── CreateTicketCommandValidator.cs (crear)
│   ├── ClassifyTicketCommandValidator.cs (crear)
│   └── ResolveTicketCommandValidator.cs (crear)
├── Inquilino/
│   ├── CreateInquilinoCommandValidator.cs (crear)
│   └── SuspendInquilinoCommandValidator.cs (crear)
└── ... (otros)
```

**Tareas:**
- [ ] Crear DTOs que NO exponen aggregates (flatten)
- [ ] Mapear Value Objects a propiedades simple (Estado.Valor → "Activo")
- [ ] Crear FluentValidation validators para cada comando
- [ ] Validar reglas: longitud máx, email format, etc.

### 2.4 Application Layer - MediatR Setup (Estimado: 2-3h)

**Objetivo:** Configurar MediatR para commands, queries y handlers

#### 2.4.1 - Commands
```
📁 PIGI-PT-Application/Commands/Ticket/
├── CreateTicket/
│   ├── CreateTicketCommand.cs (crear)
│   └── CreateTicketCommandHandler.cs (crear)
├── ClassifyTicket/
│   ├── ClassifyTicketCommand.cs (crear)
│   └── ClassifyTicketCommandHandler.cs (crear)
├── AssignTicket/
├── ResolveTicket/
└── CancelTicket/

(Similar para Inquilino, Usuario, etc.)
```

#### 2.4.2 - Queries
```
📁 PIGI-PT-Application/Queries/Ticket/
├── GetTicketById/
│   ├── GetTicketByIdQuery.cs (crear)
│   └── GetTicketByIdQueryHandler.cs (crear)
├── GetActiveTickets/
├── GetTicketsByInquilino/
└── GetTicketReport/

(Similar para otros agregados)
```

**Tareas:**
- [ ] Crear base interfaces: `ICommand<T>`, `IQuery<T>`
- [ ] Implementar 5+ commands por agregado
- [ ] Implementar 3+ queries por agregado
- [ ] Handlers que orquesten: Validate → Domain Logic → Persist → Return DTO
- [ ] Emitir eventos al persistir

### 2.5 Application Layer - AutoMapper & Services (Estimado: 2-3h)

**Objetivo:** Mapeo automático entre Entities ↔ DTOs

#### 2.5.1 - AutoMapper Profiles
```
📁 PIGI-PT-Application/Mappings/
├── MappingProfile.cs (crear)
│   ├── Ticket → TicketDto
│   ├── Inquilino → InquilinoDto
│   ├── Usuario → UsuarioDto
│   ├── EstadoTicket → string (Valor)
│   ├── NivelPrioridad → string (Valor)
│   └── Rol → RolDto
```

**Tareas:**
- [ ] Crear Profile con mapeos entity → DTO (flatten)
- [ ] Configurar ValueObject mapeos (solo Valor)
- [ ] Configurar reverse mappings para commands
- [ ] Testear mappings con unit tests

### 2.6 Application Layer - Event Handlers (Estimado: 3-4h)

**Objetivo:** Implementar handlers reactivos para Domain Events

#### 2.6.1 - Event Handlers
```
📁 PIGI-PT-Application/EventHandlers/
├── Ticket/
│   ├── TicketCreadoEventHandler.cs (crear)
│   │   → Enqueue Hangfire job de sanitización
│   ├── TicketClasificadoEventHandler.cs (crear)
│   │   → Notificar operador
│   ├── OperadorAsignadoEventHandler.cs (crear)
│   │   → Notificar usuario asignado
│   └── TicketResueltoEventHandler.cs (crear)
│       → Actualizar métricas
│
├── Inquilino/
│   ├── InquilinoRegistradoEventHandler.cs (crear)
│   ├── PrivacidadIAModificadaEventHandler.cs (crear)
│   │   → Validar consentimiento
│   └── InquilinoSuspendidoEventHandler.cs (crear)
│       → Revocar acceso
│
└── Usuario/
	├── UsuarioCreadoEventHandler.cs (crear)
	│   → Enviar email bienvenida
	├── RolModificadoEventHandler.cs (crear)
	│   → Auditar cambio
	└── UsuarioDesactivadoEventHandler.cs (crear)
		→ Revocar tokens JWT
```

**Tareas:**
- [ ] Implementar handler para cada Domain Event
- [ ] Enqueue Hangfire jobs desde handlers
- [ ] Enviar notificaciones (email, push)
- [ ] Actualizar auditoría

### 2.7 Dependency Injection & Configuration (Estimado: 2-3h)

**Objetivo:** Registrar servicios en contenedor DI

#### 2.7.1 - Program.cs Configuration
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

// DbContext
builder.Services.AddDbContext<PigiPtDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
```

**Tareas:**
- [ ] Registrar MediatR assembly
- [ ] Registrar todos los repositories
- [ ] Registrar todos los servicios
- [ ] Registrar validators
- [ ] Registrar AutoMapper
- [ ] Configurar DbContext connection string

---

## FASE 3: API LAYER (ESTIMADO: 15-20 horas)

### 3.1 REST Controllers

```
📁 PIGI-PT-API/Controllers/
├── TicketsController.cs
│   ├── POST /api/tickets (CreateTicket)
│   ├── GET /api/tickets/{id} (GetTicketById)
│   ├── PUT /api/tickets/{id}/classify (ClassifyTicket)
│   ├── PUT /api/tickets/{id}/assign (AssignTicket)
│   └── PUT /api/tickets/{id}/resolve (ResolveTicket)
│
├── InquilinosController.cs
│   ├── POST /api/inquilinos (CreateInquilino)
│   ├── GET /api/inquilinos/{id}
│   ├── PUT /api/inquilinos/{id}/suspend (SuspendInquilino)
│   ├── PUT /api/inquilinos/{id}/reactivate
│   └── PUT /api/inquilinos/{id}/ia-permissions (UpdateIAPermissions)
│
├── UsuariosController.cs
├── RiesgosController.cs
└── CategoriasController.cs
```

**Tareas:**
- [ ] Crear controllers thin (delegar a MediatR)
- [ ] Validar autorización (Claims)
- [ ] Retornar DTOs (nunca Domain entities)
- [ ] HTTP status codes correctos (201 Created, 204 No Content, etc.)
- [ ] Error handling consistente

### 3.2 Authentication & Authorization

- [ ] JWT token generation & validation
- [ ] Claims-based authorization
- [ ] Role-based access control (RBAC) basado en Rol VO
- [ ] Multi-tenancy header validation

### 3.3 API Documentation

- [ ] Swagger/OpenAPI configuration
- [ ] XML documentation en controllers
- [ ] API versioning (v1, v2, etc.)
- [ ] Rate limiting

---

## FASE 4: HANGFIRE & EXTERNAL SERVICES (ESTIMADO: 10-15 horas)

### 4.1 Hangfire Background Jobs

```
📁 PIGI-PT-Infraestructure/BackgroundJobs/
├── TicketSanitizationJob.cs
│   → Llamar Python FastAPI para sanitizar
├── TicketClassificationJob.cs
│   → Llamar Python FastAPI para clasificar
└── RiskReviewJob.cs
	→ Scheduled: Revisar riesgos >90 días sin revisar
```

**Tareas:**
- [ ] Configurar Hangfire en SQL Server
- [ ] Implementar job de sanitización (async call a Python)
- [ ] Implementar job de clasificación
- [ ] Implementar scheduled job para riesgos
- [ ] Dashboard de Hangfire para monitoreo

### 4.2 External Service Adapters

```
📁 PIGI-PT-Infraestructure/Services/
├── IAServiceAdapter.cs
│   → HTTP client a Python FastAPI
├── NotificationService.cs
│   → SendGrid para emails
├── AuthenticationService.cs
│   → JWT generation/validation
└── AuditService.cs
	→ Logging de cambios
```

---

## FASE 5: WEBSAPP - BLAZOR WASM (ESTIMADO: 30-40 horas)

### 5.1 Componentes Core

```
📁 PIGI-PT-WebApp/Components/
├── Layout/
│   ├── MainLayout.razor
│   ├── NavMenu.razor
│   └── AuthLayout.razor
│
├── Pages/
│   ├── Dashboard.razor (overview)
│   ├── Tickets/
│   │   ├── TicketsList.razor
│   │   ├── TicketDetail.razor
│   │   ├── CreateTicket.razor
│   │   └── ClassifyTicket.razor
│   ├── Inquilinos/
│   ├── Usuarios/
│   ├── Riesgos/
│   └── Admin/
│       ├── InquilinoManagement.razor
│       ├── UserManagement.razor
│       └── Configuration.razor
│
└── Shared/
	├── ConfirmDialog.razor
	├── LoadingSpinner.razor
	├── NotificationToast.razor
	└── ErrorBoundary.razor
```

### 5.2 Features

- [ ] Authentication flow (Login/Logout)
- [ ] Dashboard con KPIs (tickets abiertos, riesgos críticos, etc.)
- [ ] CRUD operations para todos los agregados
- [ ] Real-time updates con SignalR (opcional)
- [ ] Responsive design con Bootstrap
- [ ] Forms con validación
- [ ] Paging & filtering

### 5.3 State Management

- [ ] Cascading parameters para multi-tenancy
- [ ] Component state management
- [ ] API client service inyectado

---

## 📊 ROADMAP VISUAL

```
ENERO 2024
├── Fase 1: Domain ✅ COMPLETADA
│   ├── Agregados + Value Objects ✅
│   ├── Domain Events ✅
│   └── Excepciones ✅
│
├── Fase 2: Application + Infrastructure ⏳ SIGUIENTE
│   ├── 2.1: EF Core + Database (semana 1)
│   ├── 2.2: Repositories + Specifications (semana 1)
│   ├── 2.3: DTOs + Validators (semana 2)
│   ├── 2.4: MediatR Commands/Queries (semana 2)
│   ├── 2.5: AutoMapper (semana 2)
│   ├── 2.6: Event Handlers (semana 3)
│   └── 2.7: Dependency Injection (semana 3)
│
├── Fase 3: API Layer (semana 4-5)
│   ├── 3.1: REST Controllers
│   ├── 3.2: Authentication & Authorization
│   └── 3.3: API Documentation (Swagger)
│
├── Fase 4: Hangfire & External Services (semana 5-6)
│   ├── 4.1: Background Jobs
│   └── 4.2: Service Adapters
│
└── Fase 5: Blazor WebApp (semana 7-10)
	├── 5.1: Components Core
	├── 5.2: Features
	└── 5.3: State Management
```

---

## 🎯 HITOS CLAVE

| Hito | Fecha Target | Status |
|---|---|---|
| Domain completado | ✅ Enero 2024 | ✅ COMPLETADO |
| Infrastructure + Application | ⏳ Febrero 2024 | ⏳ PENDIENTE |
| API functional | ⏳ Febrero 2024 | ⏳ PENDIENTE |
| Background jobs | ⏳ Marzo 2024 | ⏳ PENDIENTE |
| WebApp MVP | ⏳ Marzo 2024 | ⏳ PENDIENTE |
| Production ready | ⏳ Abril 2024 | ⏳ PENDIENTE |

---

## 💡 DECISIONES ARQUITECTÓNICAS

### ✅ Adoptadas (Fase 1)
- Clean Architecture con DDD
- Value Objects para conceptos ricos
- Máquinas de estado validadas
- Domain Events reactivos
- Auditoría automática

### ⏳ Por adoptar (Fase 2+)
- CQRS (Commands ≠ Queries)
- Event Sourcing (opcional, Fase 5+)
- API Versioning (v1, v2)
- Real-time con SignalR (Fase 5, opcional)
- GraphQL (opcional, alternativa a REST)

---

## 🔐 CONSIDERACIONES DE SEGURIDAD

### ✅ Implementadas
- Multi-tenancy en Domain
- Auditoría de cambios
- Value Objects como type safety
- Excepciones específicas (no exponen internals)

### ⏳ Por implementar
- RBAC basado en Claims (Fase 3)
- JWT con refresh tokens (Fase 3)
- Input validation en API (Fase 3)
- Rate limiting (Fase 3)
- CORS configuration (Fase 3)
- Encryption at rest (Fase 4+)

---

## 📚 DOCUMENTACIÓN REQUERIDA

### ✅ Existentes
- `README.md` - Overview del proyecto
- `ARCHITECTURE_SUMMARY.md` - Arquitectura detallada
- `DOMAIN_COMPLETION_SUMMARY.md` - Cambios Fase 1
- `DOMAIN_QUICK_REFERENCE.md` - Referencia rápida
- `PHASE2_CHECKLIST.md` - Tareas Fase 2
- READMEs por capa (Domain, Application, Infrastructure, API, WebApp)

### ⏳ Por crear
- `INFRASTRUCTURE_GUIDE.md` (durante Fase 2)
- `API_DOCUMENTATION.md` (durante Fase 3)
- `DEPLOYMENT_GUIDE.md` (pre-producción)
- `CONTRIBUTING.md` (guía para contribuidores)
- `TROUBLESHOOTING.md` (problemas comunes)

---

## 🚀 CÓMO COMENZAR CON FASE 2

1. **Lee PHASE2_CHECKLIST.md**
   - Entiende estructura de cada paso
   - Revisa ejemplos de código

2. **Comienza con 2.1 - Database**
   - Crea DbContext
   - Implementa Configurations
   - Genera migrations

3. **Continúa con 2.2 - Repositories**
   - Implementa BaseRepository
   - Crea Specifications

4. **Luego 2.3-2.7 en orden**
   - DTOs + Validators
   - Commands/Queries
   - AutoMapper
   - Event Handlers
   - DI Configuration

5. **Valida todo**
   - Build sin errores
   - Unit tests para handlers
   - Integration tests con BD

---

## 📞 CONTATOS & RECURSOS

### Documentación de Referencia
- [Entity Framework Core Docs](https://docs.microsoft.com/ef/core/)
- [MediatR Docs](https://github.com/jbogard/MediatR)
- [FluentValidation](https://docs.fluentvalidation.net/)
- [AutoMapper](https://automapper.org/)
- [Hangfire](https://www.hangfire.io/)
- [Blazor Docs](https://docs.microsoft.com/aspnet/core/blazor/)

### Repository
- **GitHub:** https://github.com/ikmori/PIGI-PT
- **Branch:** DEV (desarrollo)
- **Branch:** main (producción)

---

## 📝 NOTAS FINALES

- **Fase 1 está 100% completa** - No tocar Domain excepto para bugs
- **Fase 2 es PRIORITARIA** - Sin ella, no hay forma de persistir datos
- **Build debe pasar siempre** - Comprometer código si falla
- **Tests desde el inicio** - No dejar para el final
- **Documentar cambios** - Actualizar READMEs según avances

---

**Documento:** `ROADMAP_FUTURO.md`  
**Versión:** 1.0  
**Creado:** Enero 2024  
**Status:** ✅ ACTUALIZADO  
**Próximo paso:** Comenzar Fase 2 cuando se tenga disponibilidad
