# PIGI-PT: Plataforma Inteligente de Gestión y Predicción de Incidentes Tecnológicos

[![Build Status](https://img.shields.io/badge/build-passing-brightgreen)]()
[![Architecture](https://img.shields.io/badge/architecture-Clean%20Architecture%20%2B%20DDD-blue)]()
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)]()
[![License](https://img.shields.io/badge/license-Proprietary-red)]()

## 🎯 Descripción del Proyecto

**PIGI-PT** es una plataforma integral de gestión de incidentes tecnológicos con inteligencia artificial integrada. El sistema permite registrar, clasificar y gestionar tickets de incidentes operacionales, proporcionando sugerencias automáticas mediante análisis de IA mientras mantiene estrictos controles de privacidad y seguridad.

### Características Principales

- 🎟️ **Gestión de Tickets**: Registro, clasificación, asignación y resolución de incidentes
- 🤖 **IA Integrada**: Análisis automático de descripciones con sanitización obligatoria
- 📊 **Análisis Predictivo**: Identificación de patrones y tendencias
- 🔒 **Multi-Tenancy Seguro**: Aislamiento completo de datos por inquilino
- ⚙️ **Gestión de Riesgos**: Documentación de servicios críticos y planes de mitigación
- 👥 **Control de Acceso**: RBAC granular por rol
- 📱 **Interface Responsive**: Blazor WebAssembly con Bootstrap

## 🏗️ Arquitectura

El proyecto implementa **Clean Architecture** con **Domain-Driven Design (DDD)**:

```
┌─────────────────────────────────────────────────────────────┐
│                  PIGI-PT-WebApp (Blazor)                     │
│                   (Presentación - Frontend)                   │
└──────────────────────────┬──────────────────────────────────┘
						   │ HTTP/JSON
┌──────────────────────────▼──────────────────────────────────┐
│                   PIGI-PT-API (ASP.NET Core)                │
│                  (Presentación - REST API)                   │
├─────────────────────────────────────────────────────────────┤
│  Controllers │ Middleware │ Configuration │ Filters          │
└──────────────────────────┬──────────────────────────────────┘
						   │ MediatR
┌──────────────────────────▼──────────────────────────────────┐
│              PIGI-PT-Application (Orquestación)              │
├─────────────────────────────────────────────────────────────┤
│ Commands │ Queries │ DTOs │ Handlers │ Validators │ Mappers  │
│         Ports (Interfaces para Infrastructure)               │
└──────────────────────────┬──────────────────────────────────┘
						   │
		┌──────────────────┼──────────────────┐
		│                  │                  │
┌───────▼─────────┐ ┌─────▼──────────┐ ┌────▼────────┐
│ PIGI-PT-Domain  │ │ Infrastructure │ │  External   │
│ (Lógica Negocio)│ │ (Persistencia) │ │  Services   │
├─────────────────┤ ├────────────────┤ ├─────────────┤
│ • Aggregates    │ │ • Repositories │ │ • Python IA │
│ • Value Objects │ │ • DbContext    │ │ • Email     │
│ • Entities      │ │ • EF Core      │ │ • Hangfire  │
│ • Events        │ │ • Migrations   │ │ • Azure     │
│ • Specs         │ │ • Auth         │ │ • Cache     │
│ • Exceptions    │ │ • Logging      │ │             │
└─────────────────┘ └────────────────┘ └─────────────┘
```

### Capas Arquitectónicas

| Capa | Propósito | Tecnologías |
|------|-----------|------------|
| **Presentación (API)** | REST API con controladores | ASP.NET Core, Swagger, JWT |
| **Presentación (Web)** | Interfaz usuario interactiva | Blazor WASM, Bootstrap, SignalR |
| **Aplicación** | Orquestación de casos de uso | MediatR, CQRS, AutoMapper, FluentValidation |
| **Dominio** | Lógica y reglas de negocio | C#, DDD, Value Objects, Aggregate Roots |
| **Infraestructura** | Detalles técnicos | EF Core, Azure SQL, Python API, Hangfire |

## 📦 Estructura de Carpetas

```
PIGI-PT/
├── PIGI-PT-Domain/                    # Capa de Dominio
│   ├── Aggregates/                    # Raíces de agregados
│   ├── ValueObjects/                  # Objetos de valor ricos
│   ├── DomainEvents/                  # Eventos de dominio
│   ├── Specifications/                # Patrón Specification
│   ├── Exceptions/                    # Excepciones de dominio
│   └── Base/                          # Clases base
│
├── PIGI-PT-Application/               # Capa de Aplicación
│   ├── Commands/                      # Comandos (CQRS)
│   ├── Queries/                       # Consultas (CQRS)
│   ├── DTOs/                          # Data Transfer Objects
│   ├── Mappers/                       # AutoMapper profiles
│   ├── Ports/                         # Interfaces para Infra
│   ├── EventHandlers/                 # Manejadores de eventos
│   ├── Validations/                   # Validadores FluentValidation
│   └── Services/                      # Application Services
│
├── PIGI-PT-Infraestructure/           # Capa de Infraestructura
│   ├── Persistence/                   # EF Core, Repositories
│   ├── ExternalServices/              # Integraciones externas
│   ├── BackgroundJobs/                # Hangfire
│   ├── Auth/                          # Autenticación/Autorización
│   ├── Caching/                       # Cache (Redis/Memory)
│   └── Logging/                       # Observabilidad
│
├── PIGI-PT-API/                       # Capa Presentación (API)
│   ├── Controllers/                   # API Controllers V1, V2
│   ├── Middleware/                    # Custom middleware
│   └── Configuration/                 # Swagger, CORS, Auth
│
├── PIGI-PT-WebApp/                    # Capa Presentación (Blazor)
│   ├── Components/                    # Componentes reutilizables
│   ├── Pages/                         # Páginas principales
│   ├── Services/                      # HTTP clients, State
│   └── Models/                        # DTOs, ViewModels
│
└── README.md (Este archivo)
```

## 🚀 Inicio Rápido

### Prerequisitos

- .NET 10 SDK
- Visual Studio 2026 Community o superior
- Azure SQL Server (desarrollo local o cloud)
- Python 3.10+ (para servicio IA, opcional)

### Instalación

1. **Clonar repositorio**
   ```bash
   git clone https://github.com/ikmori/PIGI-PT.git
   cd PIGI-PT
   ```

2. **Restaurar dependencias**
   ```bash
   dotnet restore
   ```

3. **Configurar base de datos**
   ```bash
   # Actualizar appsettings.json con connection string
   cd PIGI-PT-Infraestructure
   dotnet ef database update
   ```

4. **Ejecutar API**
   ```bash
   cd PIGI-PT-API
   dotnet run
   ```

5. **Ejecutar WebApp** (en otra terminal)
   ```bash
   cd PIGI-PT-WebApp
   dotnet run
   ```

6. **Acceder a la aplicación**
   - API: https://localhost:5001/swagger
   - Web: https://localhost:5002

## 🔐 Seguridad

### Características de Seguridad

- **Multi-Tenancy**: Aislamiento de datos por inquilino con Row-Level Security
- **Authentication**: JWT Bearer token en cada request
- **Authorization**: RBAC con roles (SuperAdmin, Admin, Operador, UsuarioGeneral)
- **Sanitización**: Enmascaramiento obligatorio de datos sensibles antes de IA
- **Auditoría**: Registro de CreatedBy, ModifiedBy en todas las entidades
- **Privacidad**: Control granular de IA por inquilino

### Matriz de Permisos

| Acción | SuperAdmin | Admin | Operador | User |
|--------|-----------|-------|----------|------|
| Ver tickets | ✓ Todos | ✓ Todos | ✓ Todos | ✓ Propios |
| Crear tickets | ✓ | ✓ | ✓ | ✓ |
| Clasificar | ✓ | ✓ | ✓ | ✗ |
| Asignar operador | ✓ | ✓ | ✓ | ✗ |
| Crear usuarios | ✓ | ✓ | ✗ | ✗ |
| Configurar IA | ✓ | ✓ | ✗ | ✗ |
| Suspender inquilino | ✓ | ✗ | ✗ | ✗ |

## 📚 Documentación

Cada capa y componente principal tiene su propio README:

- [Domain Layer](./PIGI-PT-Domain/README.md) - Lógica de negocio
- [Application Layer](./PIGI-PT-Application/README.md) - Orquestación CQRS
- [Infrastructure Layer](./PIGI-PT-Infraestructure/README.md) - Persistencia e integraciones
- [API Layer](./PIGI-PT-API/README.md) - Endpoints REST
- [WebApp Layer](./PIGI-PT-WebApp/README.md) - Interfaz Blazor

Documentación detallada de arquitectura:
- [Architecture Summary](./ARCHITECTURE_SUMMARY.md) - Resumen ejecutivo

## 🧪 Testing

(En desarrollo - Fase 2)

```bash
# Tests unitarios del dominio
dotnet test PIGI-PT-Domain.Tests

# Tests de integración
dotnet test PIGI-PT-Integration.Tests

# Tests de la API
dotnet test PIGI-PT-API.Tests
```

## 📊 Ejemplos de Uso

### Crear un Ticket

```bash
POST /api/v1/tickets
Content-Type: application/json
Authorization: Bearer {token}

{
  "titulo": "No puedo acceder al sistema",
  "descripcion": "Desde hace 30 minutos no consigo autenticarme en la plataforma"
}
```

### Listar Tickets del Inquilino

```bash
GET /api/v1/tickets?estado=Clasificado&prioridad=Alta
Authorization: Bearer {token}
```

### Clasificar Ticket (por IA)

```bash
PUT /api/v1/tickets/{ticketId}/classify
Content-Type: application/json
Authorization: Bearer {token}

{
  "prioridad": "Alta",
  "categoriaId": "550e8400-e29b-41d4-a716-446655440000"
}
```

## 🔄 Flujo de Procesamiento de Tickets

```
1. Usuario crea ticket
   ↓
2. Sistema valida datos
   ↓
3. Ticket creado → TicketCreadoEvent
   ↓
4. Hangfire encola trabajo de sanitización
   ↓
5. Servicio Python sanitiza descripción
   ↓
6. Servicio Python llama IA (OpenAI/Google)
   ↓
7. IA retorna prioridad y categoría
   ↓
8. Backend clasifica ticket → TicketClasificadoEvent
   ↓
9. Operador asignado → Notificación
   ↓
10. Operador resuelve → TicketResueltoEvent
	↓
11. Usuario notificado de resolución
```

## 🐛 Resolución de Problemas

### Problema: Build fallido
```bash
# Limpiar y restaurar
dotnet clean
dotnet restore
dotnet build
```

### Problema: Migrations no aplicadas
```bash
cd PIGI-PT-Infraestructure
dotnet ef database update
```

### Problema: JWT inválido
- Verificar connection string en appsettings.json
- Verificar JWT secret en configuration
- Verificar token no expirado

## 🤝 Contribuciones

1. Crear rama de feature: `git checkout -b feature/mi-feature`
2. Commit cambios: `git commit -am 'Add feature'`
3. Push a rama: `git push origin feature/mi-feature`
4. Crear Pull Request

## 📝 Convenciones del Código

- **Naming**: PascalCase para clases, camelCase para variables
- **Documentación**: XML comments en métodos públicos
- **Excepciones**: DomainException para lógica de negocio
- **Tests**: Sufijo `.Tests`, naming `Should_ExpectedBehavior_When_Condition`
- **Commits**: `feat:`, `fix:`, `docs:`, `refactor:` prefixes

## 🗺️ Hoja de Ruta

### Fase 1 ✅ COMPLETADA
- [x] Estructuración de capas Clean Architecture
- [x] Implementación de DDD (Aggregates, Value Objects, Events)
- [x] Documentación completa
- [x] Value Objects refactorizados
- [x] Máquina de estados para Ticket
- [x] 5 agregados completamente refactorizados
- [x] Domain Events enriquecidos
- [x] Excepciones de dominio específicas

### Fase 2 ⏳ PRÓXIMA (Planificada)
- [ ] Infrastructure: EF Core DbContext & Migrations
- [ ] Infrastructure: Repositories & Specifications Pattern
- [ ] Application: DTOs & FluentValidation
- [ ] Application: MediatR Commands/Queries/Handlers
- [ ] Application: AutoMapper Profiles
- [ ] Application: Domain Event Handlers
- [ ] Application: Dependency Injection Configuration

### Fase 3 (Planificada)
- [ ] API: REST Controllers (CRUD)
- [ ] API: Authentication & Authorization (JWT)
- [ ] API: Swagger/OpenAPI Documentation

### Fase 4 (Planificada)
- [ ] Hangfire Background Jobs (Sanitización, Clasificación)
- [ ] External Service Adapters (Python IA, Email, etc.)
- [ ] Analytics & Metrics

### Fase 5 (Planificada)
- [ ] Blazor WebApp Components
- [ ] SignalR Real-time Updates
- [ ] Advanced Reporting & Analytics

---

**📖 Para detalles completos consulta [ROADMAP_FUTURO.md](./ROADMAP_FUTURO.md)** - Incluye:
- Timeline detallado con estimaciones de esfuerzo
- Ejemplos de código para cada fase
- Hitos clave y métricas de progreso
- Decisiones arquitectónicas pendientes
- Guía para comenzar Fase 2

## 📞 Contacto

- **Maintainer**: [Tu nombre/equipo]
- **Email**: [email]
- **Issues**: GitHub Issues

## 📄 Licencia

Proprietary - Todos los derechos reservados © 2024

---

**Última actualización**: Enero 2024
**Versión**: 1.0.0
**Estado**: En Desarrollo (Fase 2)
