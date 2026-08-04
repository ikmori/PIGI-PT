# 🔍 CONTEXTO GENERAL DEL PROYECTO: PIGI-PT

Este documento proporciona una guía de contexto detallada y exhaustiva para que una Inteligencia Artificial pueda comprender a fondo el diseño, la arquitectura, las entidades y los flujos de negocio del proyecto **PIGI-PT**.

---

## 🎯 1. Descripción del Proyecto

**PIGI-PT** (Plataforma Inteligente de Gestión y Predicción de Incidentes Tecnológicos) es una solución de nivel empresarial para la gestión y resolución de incidentes de tecnología. Sus pilares fundamentales son:
- **Aislamiento Multi-Inquilino (Multi-Tenancy)**: Los datos de diferentes organizaciones (inquilinos) se mantienen estrictamente aislados.
- **Inteligencia Artificial Integrada**: Los incidentes reportados son analizados automáticamente para predecir su prioridad y clasificar su categoría mediante modelos de IA.
- **Privacidad y Sanitización Obligatoria**: Antes de enviar cualquier descripción a la IA externa, se realiza un enmascaramiento riguroso de Datos de Identificación Personal (PII) como correos, direcciones IP y contraseñas.
- **Gestión de Riesgos**: Módulo para documentar los servicios críticos de infraestructura, evaluar sus amenazas y asociar planes de mitigación correspondientes.

---

## 🏗️ 2. Arquitectura de Software

El proyecto se rige por los principios de **Clean Architecture** combinados con **Domain-Driven Design (DDD)**. El flujo de dependencias siempre apunta hacia el centro (la capa de Dominio).

```
          ┌─────────────────────────────────────────────────────────────┐
          │                  PIGI-PT-WebApp (Blazor)                    │
          │                   (Presentación - Frontend)                 │
          └──────────────────────────┬──────────────────────────────────┘
                                     │ HTTP/JSON + JWT
          ┌──────────────────────────▼──────────────────────────────────┐
          │                   PIGI-PT-API (ASP.NET Core)                │
          │                  (Presentación - REST API)                  │
          ├─────────────────────────────────────────────────────────────┤
          │  Controllers │ Middleware │ Configuration │ Filters          │
          └──────────────────────────┬──────────────────────────────────┘
                                     │ MediatR (Commands/Queries)
          ┌──────────────────────────▼──────────────────────────────────┐
          │              PIGI-PT-Application (Orquestación)             │
          ├─────────────────────────────────────────────────────────────┤
          │ Commands │ Queries │ DTOs │ Handlers │ Validators │ Mappers  │
          │         Ports (Interfaces para Infrastructure)              │
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
          │ • Events        │ │ • Migrations   │ │ • Cache     │
          │ • Specs         │ │ • Auth         │ │             │
          │ • Exceptions    │ │ • Logging      │ │             │
          └─────────────────┘ └────────────────┘ └─────────────┘
```

### 📁 Estructura de Proyectos y Responsabilidades

1. **`PIGI-PT-Domain` (Núcleo)**: Contiene las reglas del negocio, entidades, objetos de valor (Value Objects), excepciones personalizadas, eventos de dominio y especificaciones. No depende de ninguna otra capa ni biblioteca técnica de persistencia.
2. **`PIGI-PT-Application` (Casos de Uso)**: Orquesta el flujo de datos. Define los Commands/Queries (CQRS) utilizando **MediatR**, validadores de entrada con **FluentValidation**, perfiles de transformación con **AutoMapper** e interfaces (Puertos) para los servicios de persistencia y externos.
3. **`PIGI-PT-Infraestructure` (Detalles Técnicos)**: Implementa los contratos definidos en la capa de aplicación. Incluye la persistencia de datos con **Entity Framework Core (EF Core)**, configuración del motor relacional **SQL Server/Azure SQL**, enrutamiento de trabajos en segundo plano con **Hangfire**, y adaptadores para el servicio de IA (FastAPI), envío de correos (SendGrid), caché y autenticación.
4. **`PIGI-PT-API` (Punto de Entrada)**: REST API de ASP.NET Core que expone controladores versionados (`v1`, `v2`), intercepta errores a través de Middlewares personalizados y asegura los endpoints utilizando JWT.
5. **`PIGI-PT-WebApp` (Interfaz de Usuario)**: Aplicación Blazor WebAssembly en modo interactivo global (`InteractiveServer`) estilizada con Bootstrap y componentes enriquecidos de **BootstrapBlazor**. Consume la API REST enviando tokens JWT.

---

## 🗃️ 3. Modelo de Dominio y Entidades (Aggregates)

Todas las entidades que pertenecen a un inquilino específico heredan de `InquilinoEntity` (la cual contiene el campo `InquilinoId`), garantizando el aislamiento de datos. La raíz de auditoría añade propiedades automáticas como `CreatedBy`, `CreatedAt`, `ModifiedBy` y `ModifiedAt`.

A continuación se detallan los 5 agregados del dominio:

### 1. Inquilino (`Inquilino.cs`)
*   **Responsabilidad**: Representa la organización cliente de la plataforma (Multi-tenancy).
*   **Propiedades clave**:
    *   `NombreComercial` (string, obligatorio)
    *   `DominioRed` (string, normalizado a minúsculas)
    *   `PermitirServicioIA` (bool - Control de privacidad por diseño)
    *   `Estado` (Value Object: `EstadoInquilino`)
*   **Invariantes y Reglas**:
    *   El nombre comercial es obligatorio.
    *   El servicio de IA está deshabilitado por defecto.
    *   Un inquilino no se elimina físicamente del sistema, solo se puede suspender.
*   **Máquina de Estados (`EstadoInquilino`)**:
    *   `Activo` $\leftrightarrow$ `Suspendido` (Transiciones explícitas validadas en el agregado mediante `SuspenderServicio()` y `ReactivarServicio()`).
*   **Eventos de Dominio**: `InquilinoRegistradoEvent`, `PrivacidadIAModificadaEvent`, `InquilinoSuspendidoEvent`, `InquilinoReactivadoEvent`.

### 2. Usuario (`Usuario.cs`)
*   **Responsabilidad**: Identidad y asignación de rol de un miembro de la plataforma.
*   **Propiedades clave**:
    *   `FullName` (string)
    *   `Email` (Value Object: `Email` con validación RFC 5322 y normalizado a minúsculas)
    *   `UserName` (string, único)
    *   `Password` (string, debe almacenarse como un hash seguro)
    *   `Rol` (Value Object: `Rol` que encapsula la matriz de permisos)
    *   `IsActive` (bool)
*   **Invariantes y Reglas**:
    *   Un usuario inactivo no puede iniciar sesión ni cambiar de rol.
    *   El correo debe tener una estructura sintácticamente correcta.
*   **Eventos de Dominio**: `UsuarioCreadoEvent`, `ContrasenaActualizadaEvent`, `RolModificadoEvent`, `UsuarioDesactivadoEvent`, `UsuarioReactivadoEvent`.

### 3. Categoria (`Categoria.cs`)
*   **Responsabilidad**: Categoriza los incidentes para simplificar la canalización (ej. "Infraestructura", "Seguridad", "Software").
*   **Propiedades clave**:
    *   `NombreCategoria` (string)
    *   `Descripcion` (string)
    *   `IsActive` (bool)
*   **Invariantes y Reglas**:
    *   El nombre de la categoría es obligatorio y exclusivo dentro del contexto de cada inquilino.
    *   Las categorías no se borran físicamente; solo se desactivan.
*   **Eventos de Dominio**: `CategoriaRegistradaEvent`, `CategoriaActualizadaEvent`, `CategoriaDesactivadaEvent`.

### 4. Ticket (`Ticket.cs`)
*   **Responsabilidad**: El elemento central del sistema. Modela el ciclo de vida completo de un incidente de TI.
*   **Propiedades clave**:
    *   `Titulo` (string)
    *   `DescripcionOriginal` (string)
    *   `DescripcionSanitizada` (Value Object: `DescripcionSanitizada`)
    *   `Prioridad` (Value Object: `NivelPrioridad`)
    *   `Estado` (Value Object: `EstadoTicket`)
    *   `OperadorAsignadoId` (Guid?, nulo hasta ser clasificado y asignado)
    *   `CategoriaId` (Guid?, nulo hasta ser clasificado)
    *   `Resolución` (string, nota de cierre)
    *   `FechaResolucion` (DateTime?)
*   **Invariantes y Reglas**:
    *   No se puede asignar un operador a un ticket resuelto, cancelado o rechazado.
    *   La clasificación por IA requiere que el ticket esté en estado `PendienteDeAnalisis`.
    *   Para poder ser `Resuelto`, el ticket debe tener un operador asignado previamente.
*   **Máquina de Estados (`EstadoTicket`)**:
    ```
    PendienteDeAnalisis (Creado) ──> Clasificado ──> EnProgreso ──> Resuelto
                                 └──> Rechazado  └──> Cancelado
    ```
*   **Eventos de Dominio**: `TicketCreadoEvent`, `TicketClasificadoEvent`, `OperadorAsignadoEvent`, `TicketResueltoEvent`.

### 5. RiesgoOperacional (`RiesgoOperacional.cs`)
*   **Responsabilidad**: Documenta amenazas asociadas a servicios críticos de infraestructura y su mitigación.
*   **Propiedades clave**:
    *   `ServicioAfectado` (string, ej. "Servidor de Base de Datos")
    *   `DescripcionAmenaza` (string)
    *   `NivelDeImpacto` (Value Object: `NivelPrioridad`)
    *   `PlanDeMitigacion` (string)
    *   `FechaUltimaRevision` (DateTime)
*   **Invariantes y Reglas**:
    *   El plan de mitigación y la descripción de la amenaza son obligatorios.
    *   La fecha de revisión debe actualizarse obligatoriamente en cada cambio.
*   **Eventos de Dominio**: `RiesgoRegistradoEvent`, `RiesgoCriterioModificadoEvent`, `RiesgoEliminadoEvent`.

---

## 💎 4. Value Objects (Objetos de Valor) Ricos

Para evitar la "obsesión por tipos primitivos", se diseñaron Value Objects inmutables con lógica de negocio y métodos de fábrica (`Create`):

1.  **`NivelPrioridad`**: Representa la severidad (`NoDefinida`, `Baja`, `Media`, `Alta`, `Critica`). Contiene métodos heurísticos como `EsMayorQue()` y `EsCritica()`.
2.  **`EstadoTicket`**: Define las transiciones estrictas del ciclo de vida del incidente mediante `PuedeTransicionarA()`.
3.  **`Rol`**: Encapsula los roles del sistema (`SuperAdmin` = 1, `Admin` = 2, `Operador` = 3, `UsuarioGeneral` = 4) y la matriz de permisos correspondiente.
4.  **`Email`**: Valida y normaliza direcciones de correo.
5.  **`DescripcionSanitizada`**: Asegura que el texto ha pasado por el pipeline de sanitización (remoción de PII) antes de su tratamiento por IA.

---

## ⚡ 5. Capa de Aplicación y Patrones Implementados

La capa de aplicación funciona bajo el principio de desacoplamiento tecnológico total:

### 1. Patrón CQRS con MediatR
Los casos de uso se dividen estrictamente en:
-   **Commands**: Operaciones que cambian el estado del sistema. No retornan datos complejos, solo DTOs descriptivos del objeto modificado.
    *   *Ejemplos*: `CreateTicketCommand`, `ClassifyTicketCommand`, `AssignOperatorCommand`, `ResolveTicketCommand`, `CreateUsuarioCommand`.
-   **Queries**: Operaciones de lectura optimizadas.
    *   *Ejemplos*: `GetTicketsQuery`, `GetTicketByIdQuery`, `GetInquilinosQuery`.
-   **Behaviors (Pipelines)**:
    *   `ValidationBehavior`: Ejecuta de manera transparente todos los validadores de FluentValidation asociados al comando entrante antes de procesarlo. Si se detecta un error de formato, lanza una excepción de validación interrumpiendo el flujo.

### 2. FluentValidation
Cada comando cuenta con un validador que describe sus reglas sintácticas básicas (longitud de caracteres, no nulos, formato de correo) para que el Handler solo reciba datos formalmente válidos.
*   *Ejemplo*: [CreateUsuarioCommandValidator.cs](file:///c:/PIGI-PT/PIGI-PT-Application/Validations/Usuario/CreateUsuarioCommandValidator.cs) define límites para `FullName`, valida el formato del `Email` y el rango numérico del `RolValor`.

### 3. Ports (Interfaces/Puertos)
Definen los contratos de salida que la capa de Infraestructura implementará:
-   `IUnitOfWork` / `IRepository<T>`: Contratos de acceso a datos.
-   `IIAService`: Contrato de comunicación para sanitizar y clasificar el ticket.
-   `IEmailService`, `INotificationService`, `IHangfireService`.

---

## 🔧 6. Capa de Infraestructura y Servicios Externos

Esta capa implementa el acceso físico a los datos y las integraciones con servicios web externos.

### 1. EF Core y Patrón Specification
-   **Mapeo de Base de Datos**: Las configuraciones en `Persistence/Configurations` mapean las entidades a las tablas físicas. Los Value Objects se persisten mapeándolos a tipos primitivos en la base de datos a través de **Value Converters**:
    ```csharp
    builder.Property(t => t.Estado)
        .HasConversion(v => v.Valor, v => EstadoTicket.Create(v));
    ```
-   **Specification Pattern**: Permite crear consultas reusables, fuertemente tipadas y encapsuladas (ej. `ActiveTicketsSpec`, `TicketsByInquilinoIdSpec`). El repositorio genérico `BaseRepository<T>` recibe la especificación y construye las cláusulas `Where` e `Include` dinámicamente antes de enviar la consulta al motor de base de datos.

### 2. Integración con IA mediante FastAPI (Python)
La plataforma interactúa con un microservicio de Inteligencia Artificial desarrollado en Python utilizando FastAPI. El adaptador [IAService.cs](file:///c:/PIGI-PT/PIGI-PT-Infraestructure/ExternalServices/AI/IAService.cs) maneja esta comunicación:
-   `SanitizeAsync`: Envía la descripción a la ruta `/sanitize` de FastAPI para eliminar datos confidenciales.
-   `ClassifyAsync`: Envía la descripción ya sanitizada a la ruta `/classify` para recibir el nivel de prioridad y la categoría recomendada.
-   **Mecanismo Fallback**: Si el servicio FastAPI de IA se encuentra inactivo, `IAService` cuenta con un fallback local basado en expresiones regulares para el enmascaramiento de PII y lógica heurística por palabras clave en C# para predecir la prioridad. Esto permite el desarrollo sin dependencias de red activas.

### 3. Trabajos en segundo plano con Hangfire
Para asegurar la responsividad del usuario, la llamada a la IA y el posterior enrutamiento se ejecutan de forma asíncrona fuera del ciclo de petición HTTP:
1. Al crear un ticket, se emite `TicketCreadoEvent`.
2. El manejador de eventos programa un Job asíncrono en **Hangfire**.
3. El Job de Hangfire ejecuta la sanitización mediante el adaptador de IA.
4. Con el texto sanitizado, se invoca a la clasificación de IA.
5. El ticket se actualiza en base de datos con los resultados y pasa al estado `Clasificado`.

---

## 🔒 7. Seguridad y Control de Acceso

La plataforma cuenta con un estricto diseño de seguridad estructurado de la siguiente forma:

### 1. Autenticación y Autorización JWT
*   El controlador [AuthController.cs](file:///c:/PIGI-PT/PIGI-PT-API/Controllers/V1/AuthController.cs) autentica credenciales de usuario y expide un token JWT firmado.
*   Los endpoints en la API se protegen utilizando el atributo `[Authorize]`.

### 2. Matriz de Permisos (RBAC)
Cada rol cuenta con permisos granulares incorporados dentro del Value Object `Rol`:
*   `SuperAdmin` (Nivel 1): Control global. Capacidad de administrar inquilinos globales y suspenderlos.
*   `Admin` (Nivel 2): Administrador local del inquilino. Puede crear categorías, agregar usuarios para su inquilino y reevaluar riesgos.
*   `Operador` (Nivel 3): Responsable de resolver incidentes. Puede modificar estados de tickets asignados y clasificar manualmente.
*   `UsuarioGeneral` (Nivel 4): El reportante clásico. Solo puede crear incidentes y listar sus propios tickets reportados.

**Matriz de Permisos en Código:**

| Permiso / Acción | SuperAdmin | Admin | Operador | UsuarioGeneral |
| :--- | :---: | :---: | :---: | :---: |
| `Ver todos los tickets` | ✓ | ✓ | ✓ | ✗ (Solo Propios) |
| `Crear tickets` | ✓ | ✓ | ✓ | ✓ |
| `Clasificar tickets` | ✓ | ✓ | ✓ | ✗ |
| `Asignar Operador` | ✓ | ✓ | ✓ | ✗ |
| `Gestionar Categorías` | ✓ | ✓ | ✗ | ✗ |
| `Crear/Modificar Usuarios` | ✓ | ✓ | ✗ | ✗ |
| `Suspender Inquilino` | ✓ | ✗ | ✗ | ✗ |
| `Registrar/Revisar Riesgos`| ✓ | ✓ | ✗ | ✗ |

---

## 📱 8. Frontend Blazor WebApp

La interfaz gráfica del usuario se ejecuta sobre **Blazor WebApp** con interactividad global del lado del servidor (`InteractiveServer`).

-   **Dashboard Personalizado por Rol**: Al iniciar sesión, la aplicación renderiza métricas y gráficos adaptados a las tareas de cada rol (ej. total de tickets pendientes de clasificar para operadores, KPIs de riesgo para administradores).
-   **Navegación Dinámica (`NavMenu`)**: Oculta o muestra secciones críticas de la barra lateral dependiendo de la identidad reclamada en el token del usuario.
-   **State Management (Manejo de Estado)**: Los contenedores de estado en memoria (`AppState`, `UserState` y `TenantState`) propagan cambios en toda la interfaz sin necesidad de recargar la página completa.

---

## 🔄 9. Flujo End-to-End del Procesamiento de un Ticket

A continuación se ilustra el ciclo de vida completo de un ticket de incidente:

```
[Usuario] 
   │
   ├─> 1. Envía formulario de creación (Título y Descripción)
   │
[REST API] 
   │
   ├─> 2. Valida la petición (Command Validator)
   │
[Domain / DB] 
   │
   ├─> 3. Se instancia Ticket en estado "PendienteDeAnalisis"
   ├─> 4. Se persiste en la BD y se emite "TicketCreadoEvent"
   │
[Application Event Handlers] 
   │
   ├─> 5. El Handler captura "TicketCreadoEvent"
   ├─> 6. Registra un background job en Hangfire
   │
[Hangfire Background Service]
   │
   ├─> 7. Ejecuta Job: Llama a "IAService.SanitizeAsync(descripcion)"
   │      └─> Enmascara Emails, IPs y Claves (PII)
   ├─> 8. Llama a "IAService.ClassifyAsync(descripcion_sanitizada)"
   │      └─> Retorna Categoría recomendada y Prioridad (Baja/Media/Alta/Crítica)
   ├─> 9. Ejecuta método de negocio "ticket.ClasificarPorIA(prioridad, categoriaId)"
   ├─> 10. Persiste el ticket y cambia el estado a "Clasificado"
   ├─> 11. Emite "TicketClasificadoEvent"
   │
[Notificaciones]
   │
   └─> 12. Notifica a los Operadores asignables del sistema
```

Una vez clasificado:
1. El Administrador o el Operador asigna un encargado (`ticket.AsignarOperador(operadorId)` $\rightarrow$ cambia el estado a `EnProgreso`).
2. El Operador trabaja en el incidente y, al finalizar, ejecuta la resolución (`ticket.Resolver(notaResolucion)` $\rightarrow$ cambia el estado final a `Resuelto`).
3. Se notifica al reportante y se actualizan los indicadores clave en el dashboard.
