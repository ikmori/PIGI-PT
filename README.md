# PIGI-PT: Plataforma Inteligente de Gestión y Predicción de Incidentes Tecnológicos

[![Build Status](https://img.shields.io/badge/build-passing-brightgreen)]()
[![Architecture](https://img.shields.io/badge/architecture-Clean%20Architecture%20%2B%20DDD-blue)]()
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)]()
[![License](https://img.shields.io/badge/license-Proprietary-red)]()

## 🎯 Descripción del Proyecto

**PIGI-PT** es una plataforma integral de gestión de incidentes tecnológicos con inteligencia artificial integrada. El sistema permite registrar, clasificar y gestionar tickets de incidentes operacionales, proporcionando sugerencias automáticas mediante análisis de IA mientras mantiene estrictos controles de privacidad y seguridad.

### Características Principales

- 🎟️ **Gestión de Tickets**: Registro, clasificación, asignación y resolución de incidentes
- 🤖 **IA Integrada**: Análisis automático de descripciones con sanitización obligatoria y asignación de prioridades.
- 📊 **Análisis Predictivo**: Identificación de patrones y tendencias.
- 🔒 **Multi-Tenancy Seguro**: Aislamiento completo de datos por inquilino.
- 👥 **Control de Acceso**: RBAC granular por rol (Administrador, Operador, Usuario).
- 📱 **Interface Responsive**: Blazor WebAssembly con Bootstrap, diseñada para una experiencia ágil.

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
│ • Aggregates    │ │ • Repositories │ │ • Gemini IA │
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
| **Infraestructura** | Detalles técnicos | EF Core, SQL Server / Azure SQL, IA con Gemini |

## 📦 Estructura de Carpetas

```text
PIGI-PT/
├── PIGI-PT-Domain/                    # Capa de Dominio (Modelos ricos, DDD, Eventos)
├── PIGI-PT-Application/               # Capa de Aplicación (CQRS, MediatR, Mappers)
├── PIGI-PT-Infraestructure/           # Capa de Infraestructura (EF Core, Repositorios, External Services)
├── PIGI-PT-API/                       # Capa Presentación Backend (API REST)
├── PIGI-PT-WebApp/                    # Capa Presentación Frontend (Blazor Web)
├── docs/                              # Documentación general y antigua
└── README.md                          # Este archivo
```

## 🚀 Inicio Rápido

### Prerequisitos

- .NET 10 SDK
- SQL Server LocalDB o SQL Server Express (incluido con Visual Studio)
- Clave de API de Gemini (Google AI Studio) configurada en los appsettings

### Instalación y Ejecución

1. **Restaurar dependencias**
   ```bash
   dotnet restore
   ```

2. **Configurar base de datos (desarrollo local)**
   ```bash
   cd PIGI-PT-Infraestructure
   dotnet ef database update
   ```

3. **Ejecutar API**
   ```bash
   cd PIGI-PT-API
   dotnet run
   ```

4. **Ejecutar WebApp** (en otra terminal)
   ```bash
   cd PIGI-PT-WebApp
   dotnet run
   ```

5. **Acceder a la aplicación**
   - **Frontend (Web):** http://localhost:5001
   - **Backend (API Swagger):** http://localhost:5003/swagger

## 🔐 Seguridad

### Características de Seguridad

- **Multi-Tenancy**: Aislamiento de datos por inquilino con Row-Level Security.
- **Authentication**: JWT Bearer token en cada request.
- **Authorization**: RBAC con roles (Admin, Operador/Departamento Tecnológico, UsuarioGeneral).
- **Sanitización**: Enmascaramiento obligatorio de datos sensibles antes de enviarse a la IA.
- **Auditoría**: Registro de trazabilidad y autoría en todas las entidades.

### Matriz de Permisos

| Acción | Admin | Operador (Dep. Tecnología) | Usuario |
|--------|-------|----------|------|
| Ver tickets | ✓ Todos | ✓ Del Área / Asignados | ✓ Propios |
| Crear tickets | ✓ | ✓ | ✓ |
| Asignar / Tomar Ticket | ✓ | ✓ | ✗ |
| Resolver tickets | ✓ | ✓ | ✗ |
| Crear usuarios | ✓ | ✗ | ✗ |

## 🔄 Flujo de Procesamiento de Tickets

```text
1. Usuario crea ticket
   ↓
2. Sistema valida datos
   ↓
3. Ticket creado → TicketCreadoEvent
   ↓
4. Backend procesa directamente
   ↓
5. Servicio de Infraestructura (C#) sanitiza descripción
   ↓
6. Servicio Gemini (C#) llama IA
   ↓
7. IA retorna prioridad y sugiere área o categoría
   ↓
8. Backend clasifica ticket → TicketClasificadoEvent
   ↓
9. Operador toma el ticket o se asigna (En progreso)
   ↓
10. Operador resuelve → TicketResueltoEvent
```

## 📝 Convenciones del Código

- **Naming**: PascalCase para clases, camelCase para variables locales
- **Documentación**: XML comments en métodos y clases públicas
- **Excepciones**: Uso de domain exceptions específicas para la capa de negocio
- **Arquitectura**: Separación estricta de responsabilidades bajo los principios de Clean Architecture y DDD.

## 📄 Licencia

Proprietary - Todos los derechos reservados © 2026

---

**Última actualización**: Julio 2026
**Versión**: 1.0.0
**Estado**: Estable / MVP Funcional
