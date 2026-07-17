# 📡 PIGI-PT API Layer — Implementación Fase 3.1

**Fecha de implementación:** Junio 26, 2026  
**Estado:** ✅ Implementado — Primeros endpoints REST funcionales  
**Requiere:** .NET 10 SDK + `dotnet run` desde `PIGI-PT-API/`

---

## 🎯 Qué se implementó en esta sesión

Esta sesión implementó la **pila completa mínima** para tener endpoints REST funcionales:
la conexión entre la capa API (controllers), la capa Application (CQRS con MediatR)
y la capa Infrastructure (EF Core + repositorios).

---

## 🗂️ Archivos creados

### Application Layer

| Archivo | Tipo | Propósito |
|---------|------|-----------|
| [`DTOs/Ticket/TicketDto.cs`](./PIGI-PT-Application/DTOs/Ticket/TicketDto.cs) | DTO | Respuesta de ticket hacia el cliente |
| [`DTOs/Ticket/CreateTicketRequest.cs`](./PIGI-PT-Application/DTOs/Ticket/CreateTicketRequest.cs) | DTO | Entrada para crear ticket |
| [`DTOs/Ticket/ClassifyTicketRequest.cs`](./PIGI-PT-Application/DTOs/Ticket/ClassifyTicketRequest.cs) | DTO | Entrada para clasificar ticket |
| [`Commands/Ticket/CreateTicketCommand.cs`](./PIGI-PT-Application/Commands/Ticket/CreateTicketCommand.cs) | Command+Handler | Crea un nuevo Ticket en el dominio |
| [`Commands/Ticket/ClassifyTicketCommand.cs`](./PIGI-PT-Application/Commands/Ticket/ClassifyTicketCommand.cs) | Command+Handler | Clasifica un ticket (prioridad + categoría) |
| [`Commands/Ticket/ResolveTicketCommand.cs`](./PIGI-PT-Application/Commands/Ticket/ResolveTicketCommand.cs) | Command+Handler | Resuelve un ticket |
| [`Commands/Ticket/TicketMapper.cs`](./PIGI-PT-Application/Commands/Ticket/TicketMapper.cs) | Mapper | Convierte `Ticket` (dominio) → `TicketDto` |
| [`Queries/Ticket/GetTicketsQuery.cs`](./PIGI-PT-Application/Queries/Ticket/GetTicketsQuery.cs) | Query+Handler | Lista tickets activos de un inquilino |
| [`Queries/Ticket/GetTicketByIdQuery.cs`](./PIGI-PT-Application/Queries/Ticket/GetTicketByIdQuery.cs) | Query+Handler | Obtiene un ticket por ID |

### Infrastructure Layer

| Archivo | Propósito |
|---------|-----------|
| [`Configuration/InfrastructureDependencyInjection.cs`](./PIGI-PT-Infraestructure/Configuration/InfrastructureDependencyInjection.cs) | Registra `DbContext`, repositorios y `UnitOfWork` en el contenedor de DI. Incluye fallback a **InMemory** si no hay connection string. |

### API Layer

| Archivo | Propósito |
|---------|-----------|
| [`Program.cs`](./PIGI-PT-API/Program.cs) | Pipeline completo: MediatR, Swagger, Infrastructure DI, CORS, Controllers |
| [`appsettings.json`](./PIGI-PT-API/appsettings.json) | Configuración base (logging, CORS) |
| [`appsettings.Development.json`](./PIGI-PT-API/appsettings.Development.json) | Config de desarrollo — `DefaultConnection` vacío = InMemory activo |
| [`Controllers/V1/TicketsController.cs`](./PIGI-PT-API/Controllers/V1/TicketsController.cs) | Controller con 5 endpoints REST para tickets |

---

## 🔗 Endpoints disponibles

> **Base URL:** `https://localhost:5001` (o el puerto asignado por launchSettings)  
> **Swagger UI:** `https://localhost:5001/swagger`  
> **Health check:** `GET /health`

### Tickets — `/api/v1/tickets`

| Método | Ruta | Descripción | Status codes |
|--------|------|-------------|-------------|
| `GET` | `/api/v1/tickets?inquilinoId={guid}` | Lista tickets activos. Filtro opcional: `&estado=Clasificado` | `200` |
| `GET` | `/api/v1/tickets/{id}` | Obtiene ticket por ID | `200 / 404` |
| `POST` | `/api/v1/tickets` | Crea nuevo ticket | `201 / 400` |
| `PUT` | `/api/v1/tickets/{id}/classify` | Clasifica ticket (prioridad + categoría) | `200 / 400 / 404 / 409` |
| `PUT` | `/api/v1/tickets/{id}/resolve?userId={guid}` | Resuelve ticket | `200 / 400 / 404 / 409` |

---

## 📋 Ejemplos de requests

### Crear un ticket
```http
POST /api/v1/tickets
Content-Type: application/json

{
  "inquilinoId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "titulo": "No puedo acceder al sistema",
  "descripcion": "Desde hace 30 minutos no consigo autenticarme en la plataforma.",
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa7"
}
```

### Listar tickets con filtro
```http
GET /api/v1/tickets?inquilinoId=3fa85f64-5717-4562-b3fc-2c963f66afa6&estado=Clasificado
```

### Clasificar un ticket
```http
PUT /api/v1/tickets/{id}/classify
Content-Type: application/json

{
  "prioridad": 3,
  "categoriaId": "3fa85f64-5717-4562-b3fc-2c963f66afa9",
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa7"
}
```
> **Valores de prioridad:** `1` = Baja, `2` = Media, `3` = Alta, `4` = Crítica

### Resolver un ticket
```http
PUT /api/v1/tickets/{id}/resolve?userId=3fa85f64-5717-4562-b3fc-2c963f66afa7
```

---

## 🏗️ Arquitectura de la pila implementada

```
HTTP Request
     │
     ▼
[TicketsController]          ← PIGI-PT-API/Controllers/V1/
     │  IMediator.Send(command/query)
     ▼
[MediatR Handler]            ← PIGI-PT-Application/Commands|Queries/Ticket/
     │  IUnitOfWork.Tickets
     ▼
[TicketRepository]           ← PIGI-PT-Infraestructure/Persistence/Repositories/
     │  EF Core DbSet<Ticket>
     ▼
[PigiPtDbContext]            ← PIGI-PT-Infraestructure/Persistence/DbContext/
     │  + Domain Events dispatch via MediatR
     ▼
[Database: InMemory o SQL]
```

### Patrón CQRS implementado

```
Commands (escritura):          Queries (lectura):
├── CreateTicketCommand        ├── GetTicketsQuery
├── ClassifyTicketCommand      └── GetTicketByIdQuery
└── ResolveTicketCommand
```

---

## ⚙️ Configuración de base de datos

### Opción A: InMemory (sin instalación, datos volátiles)
Dejar `DefaultConnection` vacío en `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": ""
  }
}
```
> Los datos se pierden al reiniciar la API. Ideal para desarrollo rápido.

### Opción B: SQL Server LocalDB (datos persistentes)
1. Configurar en `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PigiPtDb;Trusted_Connection=True;"
  }
}
```
2. Crear y aplicar migraciones:
```bash
cd PIGI-PT-Infraestructure
dotnet ef migrations add InitialCreate --startup-project ..\PIGI-PT-API
dotnet ef database update --startup-project ..\PIGI-PT-API
```

---

## 🚀 Cómo ejecutar la API

```bash
# Desde la raíz del proyecto
cd PIGI-PT-API
dotnet run
```

La API arranca en los puertos definidos en `Properties/launchSettings.json`:
- HTTP: `http://localhost:5003` (dev)
- HTTPS: `https://localhost:5001` (producción)

Swagger disponible en: `http://localhost:5003/swagger` (entorno Development)

---

## 🔄 Flujo completo de un ticket

```
POST /api/v1/tickets
  → Estado: PendienteDeAnalisis
  → Emite: TicketCreadoEvent
       ↓
PUT /api/v1/tickets/{id}/classify
  → Estado: Clasificado
  → Emite: TicketClasificadoEvent
       ↓
(futuro) PUT /api/v1/tickets/{id}/assign
  → Estado: EnProgreso
  → Emite: OperadorAsignadoEvent
       ↓
PUT /api/v1/tickets/{id}/resolve
  → Estado: Resuelto
  → Emite: TicketResueltoEvent
```

---

## 📦 Dependencias clave

| Paquete | Versión | Uso |
|---------|---------|-----|
| `MediatR` | 14.1.0 | CQRS — despacho de Commands y Queries |
| `Swashbuckle.AspNetCore` | 10.2.1 | Swagger UI + OpenAPI |
| `Microsoft.EntityFrameworkCore.SqlServer` | 10.0.8 | Persistencia con SQL Server |
| `Microsoft.EntityFrameworkCore.InMemory` | 10.0.8 | Base de datos en memoria para desarrollo |
| `Asp.Versioning.Mvc.ApiExplorer` | 10.0.0 | Versionado de API (v1, v2...) |

---

## ✅ Estado por capa

| Capa | Estado Actual | Completado / Detalles |
|------|---------------|-----------------------|
| **Domain** | ✅ Completo (Fase 1) | Lógica de negocio y entidades ricas |
| **Infrastructure** | ✅ DI registrada, repositorios y BD listos | Migraciones SQL aplicadas localmente, sembrado completo de usuarios |
| **Application** | ✅ DTOs, Commands, Queries, Validations | Commands y Queries para todos los agregados, incluyendo lógica de asignación y filtrados específicos |
| **API — Tickets** | ✅ Endpoints funcionales completos | CRUD completo, `/classify`, `/resolve`, `/assign` y filtrados avanzados |
| **API — Auth** | ✅ Login funcional | Autenticación real contra base de datos a través de `AuthController` |
| **API — Otros** | ✅ Controllers completados | Controllers de Inquilinos, Usuarios, Categorías y Riesgos Operacionales |
| **WebApp — UI** | ✅ Blazor MVP completado | Estructura RBAC, ruteo interactivo, NavMenu dinámico y dashboards personalizados |

---

## 🗺️ Próximos pasos (Fase 4 - Servicios Externos e Integración)

- [ ] Implementar autenticación JWT real en la API y proteger endpoints con `[Authorize]`
- [ ] Configurar Hangfire y programar los background jobs de sanitización y clasificación por IA
- [ ] Conectar la WebApp para que realice peticiones HTTP reales a la API (reemplazar mock data providers)
- [ ] Agregar validaciones con FluentValidation en los command handlers
- [ ] Configurar rate limiting y paginación en `GET /tickets`

---

*Actualizado y completado en Julio 2026*
