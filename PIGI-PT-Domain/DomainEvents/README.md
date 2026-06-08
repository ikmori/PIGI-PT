# Domain Events (Eventos de Dominio)

## Propósito
Esta carpeta contiene todos los **Eventos de Dominio** del sistema. Los eventos de dominio son la forma en que los agregados comunican cambios importantes al resto de la aplicación sin estar acoplados.

## ¿Qué es un Evento de Dominio?

Un evento de dominio:
- Representa algo que **sucedió** en el pasado en el dominio
- Se nombra en **tiempo pasado** (ej: `TicketCreadoEvent`, no `CrearTicketEvent`)
- Contiene la información mínima necesaria para que otros componentes reaccionen
- Se emite desde la raíz de un agregado
- Es inmutable una vez creado

## Beneficios

1. **Desacoplamiento**: Los agregados no conocen a los manejadores de eventos
2. **Auditabilidad**: Registro completo de cambios en el dominio
3. **Event Sourcing Ready**: Fácil de evolucionar hacia almacenamiento basado en eventos
4. **Escalabilidad**: Los eventos se pueden procesar de forma asíncrona
5. **Testing**: Fácil verificar qué eventos se emiten en un test

## Eventos del Sistema PIGI-PT

### Agregado: Ticket

#### 1. **TicketCreadoEvent**
Se emite cuando se crea un nuevo ticket.

```csharp
public class TicketCreadoEvent : DomainEvent
{
	public Guid TicketId { get; }
	public string Titulo { get; }
	public string DescripcionOriginal { get; }
	public Guid CreadorId { get; }
	public DateTime FechaCreacion { get; }
}
```

**Manejadores**:
- Encolador en Hangfire para sanitización asíncrona
- Notificación al inquilino de nuevo ticket

#### 2. **TicketClasificadoEvent**
Se emite cuando el sistema clasifica un ticket (después de análisis de IA).

```csharp
public class TicketClasificadoEvent : DomainEvent
{
	public Guid TicketId { get; }
	public NivelPrioridad Prioridad { get; }
	public Guid CategoriaId { get; }
	public DateTime FechaClasificacion { get; }
}
```

**Manejadores**:
- Notificación al operador asignado (si aplica)
- Registro en histórico

#### 3. **OperadorAsignadoEvent**
Se emite cuando se asigna un operador a un ticket.

```csharp
public class OperadorAsignadoEvent : DomainEvent
{
	public Guid TicketId { get; }
	public Guid OperadorId { get; }
	public DateTime FechaAsignacion { get; }
}
```

**Manejadores**:
- Notificación al operador vía email
- Actualización de dashboard del operador

#### 4. **TicketResueltoEvent**
Se emite cuando se marca un ticket como resuelto.

```csharp
public class TicketResueltoEvent : DomainEvent
{
	public Guid TicketId { get; }
	public Guid CreadorId { get; }
	public DateTime FechaResolucion { get; }
	public TimeSpan TiempoDeResolucion { get; }
}
```

**Manejadores**:
- Notificación al creador del ticket (por requimiento)
- Envío de encuesta de satisfacción
- Actualización de métricas

### Agregado: Inquilino

#### 1. **InquilinoRegistradoEvent**
Se emite cuando se registra un nuevo inquilino.

```csharp
public class InquilinoRegistradoEvent : DomainEvent
{
	public Guid InquilinoId { get; }
	public string NombreComercial { get; }
	public string DominioRed { get; }
	public DateTime FechaRegistro { get; }
}
```

**Manejadores**:
- Crear espacios de almacenamiento
- Enviar bienvenida
- Inicializar configuraciones por defecto

#### 2. **PrivacidadIAModificadaEvent**
Se emite cuando se cambia la configuración de IA de un inquilino.

```csharp
public class PrivacidadIAModificadaEvent : DomainEvent
{
	public Guid InquilinoId { get; }
	public bool PermitirIA { get; }
	public Guid ModificadorId { get; }
	public DateTime FechaModificacion { get; }
}
```

**Manejadores**:
- Auditoría de cambios de seguridad
- Notificación a administrador
- Actualización de políticas de seguridad

#### 3. **InquilinoSuspendidoEvent**
Se emite cuando se suspende un inquilino.

```csharp
public class InquilinoSuspendidoEvent : DomainEvent
{
	public Guid InquilinoId { get; }
	public Guid AdministradorId { get; }
	public DateTime FechaSuspension { get; }
	public string Motivo { get; }
}
```

**Manejadores**:
- Auditoría
- Notificación al inquilino
- Desactivación de todos los usuarios del inquilino

### Agregado: Usuario

#### 1. **UsuarioCreadoEvent**
Se emite cuando se crea un usuario.

```csharp
public class UsuarioCreadoEvent : DomainEvent
{
	public Guid UsuarioId { get; }
	public Guid InquilinoId { get; }
	public string Email { get; }
	public Rol Rol { get; }
	public DateTime FechaCreacion { get; }
}
```

**Manejadores**:
- Enviar email de bienvenida con instrucciones
- Auditoría

#### 2. **ContrasenaActualizadaEvent**
Se emite cuando se actualiza la contraseña.

```csharp
public class ContrasenaActualizadaEvent : DomainEvent
{
	public Guid UsuarioId { get; }
	public DateTime FechaActualizacion { get; }
}
```

**Manejadores**:
- Auditoría
- Invalidar sesiones activas

#### 3. **RolModificadoEvent**
Se emite cuando se cambia el rol de un usuario.

```csharp
public class RolModificadoEvent : DomainEvent
{
	public Guid UsuarioId { get; }
	public Rol RolAnterior { get; }
	public Rol RolNuevo { get; }
	public Guid ModificadorId { get; }
	public DateTime FechaModificacion { get; }
}
```

**Manejadores**:
- Auditoría
- Actualizar permisos en caché
- Notificar al usuario

#### 4. **DesactivarPerfilEvent**
Se emite cuando se desactiva un perfil de usuario.

```csharp
public class DesactivarPerfilEvent : DomainEvent
{
	public Guid UsuarioId { get; }
	public Guid DesactivadorId { get; }
	public DateTime FechaDesactivacion { get; }
}
```

**Manejadores**:
- Auditoría
- Invalidar todas las sesiones
- Notificar al usuario

### Agregado: RiesgoOperacional

#### 1. **RiesgoRegistradoEvent**
Se emite cuando se registra un nuevo riesgo operacional.

```csharp
public class RiesgoRegistradoEvent : DomainEvent
{
	public Guid RiesgoId { get; }
	public Guid InquilinoId { get; }
	public string ServicioAfectado { get; }
	public NivelPrioridad NivelDeImpacto { get; }
	public DateTime FechaRegistro { get; }
}
```

#### 2. **RiesgoCriterioModificadoEvent**
Se emite cuando se actualiza criterios de un riesgo.

```csharp
public class RiesgoCriterioModificadoEvent : DomainEvent
{
	public Guid RiesgoId { get; }
	public string CampoModificado { get; }
	public string ValorAnterior { get; }
	public string ValorNuevo { get; }
	public DateTime FechaModificacion { get; }
}
```

### Agregado: Categoria

#### 1. **CategoriaRegistradaEvent**
Se emite cuando se crea una categoría.

```csharp
public class CategoriaRegistradaEvent : DomainEvent
{
	public Guid CategoriaId { get; }
	public Guid InquilinoId { get; }
	public string NombreCategoria { get; }
	public DateTime FechaCreacion { get; }
}
```

#### 2. **CategoriaActualizadaEvent**
Se emite cuando se actualiza una categoría.

```csharp
public class CategoriaActualizadaEvent : DomainEvent
{
	public Guid CategoriaId { get; }
	public string NombreNuevo { get; }
	public string DescripcionNueva { get; }
	public DateTime FechaActualizacion { get; }
}
```

## Estructura de Carpetas

```
DomainEvents/
├── Ticket/
│   ├── TicketCreadoEvent.cs
│   ├── TicketClasificadoEvent.cs
│   ├── OperadorAsignadoEvent.cs
│   ├── TicketResueltoEvent.cs
│   └── README.md
├── Inquilino/
│   ├── InquilinoRegistradoEvent.cs
│   ├── PrivacidadIAModificadaEvent.cs
│   ├── InquilinoSuspendidoEvent.cs
│   └── README.md
├── Usuario/
│   ├── UsuarioCreadoEvent.cs
│   ├── ContrasenaActualizadaEvent.cs
│   ├── RolModificadoEvent.cs
│   ├── DesactivarPerfilEvent.cs
│   └── README.md
├── RiesgoOperacional/
│   ├── RiesgoRegistradoEvent.cs
│   ├── RiesgoCriterioModificadoEvent.cs
│   └── README.md
├── Categoria/
│   ├── CategoriaRegistradaEvent.cs
│   ├── CategoriaActualizadaEvent.cs
│   └── README.md
└── README.md                  (Este archivo)
```

## Patrón de Implementación

### Estructura Básica

```csharp
public class MiDomainEvent : DomainEvent
{
	public Guid EntidadId { get; }
	public DateTime Fecha { get; }
	public Guid UsuarioResponsable { get; }

	public MiDomainEvent(Guid entidadId, Guid usuarioResponsable)
	{
		EntidadId = entidadId;
		UsuarioResponsable = usuarioResponsable;
		Fecha = DateTime.UtcNow;
	}
}
```

## Flujo de Manejo de Eventos

```
1. Agregado crea instancia de evento
2. Agregado llama AddDomainEvent(evento)
3. Infraestructura persiste agregado en BD
4. Application extrae eventos del agregado
5. MediatR publica eventos (INotification)
6. Handlers de eventos reaccionan:
   - Algunos son síncronos (ej: cambiar cache)
   - Algunos son asíncronos (ej: enviar email vía Hangfire)
```

## Ventajas vs. Alternativas

### ❌ Sin Eventos de Dominio
```csharp
// Acoplamiento directo
public void CrearTicket(TicketDto dto)
{
	var ticket = new Ticket(...);
	_ticketRepository.Add(ticket);
	_emailService.SendNew(ticket);           // Acoplado
	_hangfireService.EnqueueSanitization();  // Acoplado
	_analyticsService.Track(ticket);         // Acoplado
}
```

### ✅ Con Eventos de Dominio
```csharp
// Desacoplado
public void CrearTicket(TicketDto dto)
{
	var ticket = new Ticket(...);  // Emite TicketCreadoEvent internamente
	_ticketRepository.Add(ticket);
}

// En otro lugar (Handler de evento)
public class TicketCreadoEventHandler : INotificationHandler<TicketCreadoEvent>
{
	public async Task Handle(TicketCreadoEvent notification, CancellationToken ct)
	{
		// Lógica de sanitización, email, analytics, etc.
	}
}
```

## Guía de Extensión

Cuando necesites emitir un nuevo evento:

1. Crea el archivo `MiDomainEvent.cs` en la carpeta del agregado
2. Hereda de `DomainEvent`
3. Define propiedades de solo lectura con la información mínima
4. Llama `AddDomainEvent()` en el método del agregado
5. Crea un handler en `Application/Events/Handlers/`
6. Registra el handler en el contenedor de DI
7. Documenta el evento aquí

## Debugging y Auditoría

Los eventos de dominio pueden capturarse para:

```csharp
// Auditoría completa
public class AuditEventHandler : INotificationHandler<DomainEvent>
{
	public async Task Handle(DomainEvent notification, CancellationToken ct)
	{
		await _auditRepository.LogAsync(new AuditLog
		{
			EventType = notification.GetType().Name,
			Data = JsonConvert.SerializeObject(notification),
			Timestamp = notification.DateOccurred
		});
	}
}
```

## Próximas Mejoras

- [ ] Implementar Event Sourcing (guardar todos los eventos)
- [ ] Agregar versionamiento de eventos para evolucionar contratos
- [ ] Crear snapshots para agregados con muchos eventos
- [ ] Implementar Dead Letter Queue para eventos que fallan
- [ ] Agregar correlationId para trazabilidad end-to-end
