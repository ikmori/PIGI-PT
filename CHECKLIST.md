# ✅ Checklist de Progreso PIGI-PT

Este documento detalla el progreso actual del proyecto en relación con el Roadmap establecido, marcando lo que ya ha sido completado y lo que aún falta por implementar.

## Fase 1: Dominio (Domain Layer) - 100% COMPLETADO
- [x] Implementación de **Clean Architecture** y **Domain-Driven Design (DDD)**.
- [x] Creación de los **5 Agregados Core** (Ticket, Inquilino, Usuario, RiesgoOperacional, Categoria).
- [x] Implementación de **Value Objects** (EstadoTicket, NivelPrioridad, EstadoInquilino, Rol).
- [x] Creación de **Excepciones Específicas del Dominio**.
- [x] Configuración de **Eventos de Dominio** (Domain Events) y auditoría (CreatedBy, ModifiedBy).

## Fase 2: Infraestructura y Aplicación - 100% COMPLETADO
- [x] Configuración de **Entity Framework Core** y `PigiPtDbContext`.
- [x] Mapeo de Entidades a Tablas y generación de Migraciones.
- [x] Implementación del patrón **Repository** y **Specifications** (`TicketsByRoleSpecs`, etc.).
- [x] Patrón CQRS con **MediatR** (Commands, Queries, Handlers).
- [x] Implementación de DTOs y Automapper.
- [x] Validaciones de entrada con **FluentValidation**.

## Fase 3: Capa de API y Seguridad - 100% COMPLETADO
- [x] Creación de Controladores REST (`TicketsController`, `UsuariosController`, etc.).
- [x] Integración del pipeline de MediatR en la API.
- [x] Documentación interactiva con **Swagger**.
- [x] Soporte Multi-Tenant por cabeceras/consultas (`inquilinoId`).
- [x] Configuración inicial de Roles y Permisos (RBAC).

## Fase 5: Aplicación Web Frontend (Blazor) - 90% COMPLETADO
- [x] Interfaz gráfica interactiva y moderna con Blazor y Bootstrap.
- [x] Layouts y menús dinámicos adaptados al Rol del usuario.
- [x] Dashboard de Resumen con métricas y KPIs básicos.
- [x] Vistas CRUD para Tickets, Usuarios y Categorías.
- [x] Flujos completos funcionales: Crear ticket, Asignar operador, Filtrar por categoría.
- [x] Corrección de visibilidad de tickets para los Operadores de Tecnología.
- [x] Retiro manual del estado "Cancelado/Cerrado" para mantener la integridad del flujo.
- [ ] **Pendiente**: Reemplazar la autenticación Mock (mock auth con el email `admin@empresa.com`) por la integración real de JWT Token e Identity completa entre Blazor y la API.

## Fase 4: Inteligencia Artificial y Background Jobs - 50% COMPLETADO
- [x] **Integración de IA (Gemini)**: Servicio nativo C# implementado (`GeminiAiService`) para clasificación automática de incidencias, reemplazando el requerimiento inicial de usar Python FastAPI.
- [x] Enmascaramiento y sanitización obligatoria de datos antes de enviarse a la IA.
- [x] **Pendiente**: Integrar **Hangfire** para ejecutar procesos en segundo plano (e.g., re-verificación de riesgos, limpiezas).
- [ ] **Pendiente**: Integración de notificaciones por Email (SendGrid / SMTP) tras la creación y asignación de tickets.

## Entorno y Despliegue - 10% COMPLETADO
- [x] Configuración para entorno de desarrollo local con `SQL Server LocalDB`.
- [x] Soporte para inicialización automática de DB (Seed de prueba).
- [ ] **Pendiente**: Configurar Cadenas de Conexión de Producción y Secretos (Key Vault).
- [ ] **Pendiente**: Desplegar API en **Azure App Service**.
- [ ] **Pendiente**: Desplegar WebApp en **Azure Static Web Apps** o App Service.
- [ ] **Pendiente**: Migrar Base de Datos a **Azure SQL Database**.
