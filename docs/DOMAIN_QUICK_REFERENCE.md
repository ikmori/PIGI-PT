# 🔍 GUÍA DE REFERENCIA RÁPIDA - DOMAIN LAYER

## 📌 AGREGADOS Y SUS RESPONSABILIDADES

### Inquilino
```csharp
// Crear
var inquilino = new Inquilino(
	nombreComercial: "Empresa XYZ",
	dominioRed: "empresa.com",
	usuarioCreadorId: adminId
);

// Cambiar permisos IA
inquilino.ActualizarPermisosIA(nuevoPermiso: true, administradorId);

// Suspender/Reactivar
inquilino.SuspenderServicio(administradorId, motivo: "Falta de pago");
inquilino.ReactivarServicio(administradorId, motivo: "Deuda pagada");

// Verificar estado
if (inquilino.Estado.EstaActivo)
{
	// Hacer algo
}
```

### Usuario
```csharp
// Crear
var usuario = new Usuario(
	inquilinoId: tenantId,
	fullName: "Juan Pérez",
	email: "juan@example.com",
	userName: "juan.perez",
	password: hashPassword,
	rol: Rol.Admin
);

// Cambiar rol
usuario.CambiarRol(Rol.Operador, modificadorId);

// Cambiar contraseña
usuario.ActualizarContrasena(nuevoHash, modificadorId);

// Verificar permisos
if (usuario.TienePermiso("CREAR_TICKETS"))
{
	// Crear ticket
}

if (usuario.Rol.EsAdministrador)
{
	// Acceso admin
}
```

### RiesgoOperacional
```csharp
// Crear
var riesgo = new RiesgoOperacional(
	inquilinoId: tenantId,
	servicioAfectado: "Base de datos",
	descripcionAmenaza: "Pérdida de integridad",
	nivelDeImpacto: NivelPrioridad.Alta,
	planDeMitigacion: "Backup diario + redundancia",
	userId: userId
);

// Actualizar plan
riesgo.ActualizarPlanDeMitigacion("Backup cada 6 horas", userId);

// Reevaluar impacto
riesgo.ReevaluarImpacto(NivelPrioridad.Critica, userId);

// Marcar revisado
riesgo.RegistrarRevision(userId);

// Verificar si requiere revisión urgente
if (riesgo.RequiereRevisionUrgente())
{
	// Enviar notificación
}
```

### Categoria
```csharp
// Crear
var categoria = new Categoria(
	nombreCategoria: "Infraestructura",
	descripcion: "Problemas de hardware y redes",
	inquilinoId: tenantId
);

// Actualizar
categoria.ActualizarNombre("Infraestructura IT", adminId);
categoria.ActualizarDescripcion("Nueva descripción", adminId);

// Representación
string resumen = categoria.ObtenerResumen();
// Salida: "Infraestructura IT: Nueva descripción"
```

### Ticket
```csharp
// Crear (ya refactorizado en sesión anterior)
var ticket = new Ticket(
	inquilinoId: tenantId,
	titulo: "Login no funciona",
	descripcionOriginal: "Los usuarios no pueden iniciar sesión",
	userId: userId
);

// Aplicar sanitización
ticket.AplicarSanitizacion("usuarios no pueden iniciar sesión", userId);

// Clasificar por IA
ticket.ClasificarPorIA(NivelPrioridad.Alta, categoriaId);

// Asignar operador
ticket.AsignarOperador(operadorId, asignadorId);

// Resolver
ticket.Resolver(userId);

// Obtener métricas
int tiempoResolucion = ticket.ObtenerTiempoDeResolucion();
```

---

## 🎯 VALUE OBJECTS

### NivelPrioridad
```csharp
// Factory methods
var prioridad = NivelPrioridad.Create("Alta");
var prioridad = NivelPrioridad.Create(3);

// Instancias predefinidas
NivelPrioridad.NoDefinida
NivelPrioridad.Baja
NivelPrioridad.Media
NivelPrioridad.Alta
NivelPrioridad.Critica

// Métodos útiles
if (prioridad.EsMayorQue(NivelPrioridad.Media)) { }
if (prioridad.EsCritica()) { }

// Comparación (Value Object)
if (prioridad == NivelPrioridad.Alta) { }
```

### EstadoTicket
```csharp
// Estados disponibles
EstadoTicket.Creado
EstadoTicket.EnAnalisis
EstadoTicket.Clasificado
EstadoTicket.Asignado
EstadoTicket.Resuelto
EstadoTicket.Cancelado
EstadoTicket.Rechazado

// Validar transiciones
if (estadoActual.PuedeTransicionarA(EstadoTicket.Resuelto))
{
	// Hacer transición
}

// Helpers
if (estado.EstaEnAnalisis) { }
if (estado.EsFinal()) { }
```

### EstadoInquilino
```csharp
// Estados disponibles
EstadoInquilino.Activo
EstadoInquilino.Suspendido

// Validar transiciones
if (estado.PuedeTransicionarA(EstadoInquilino.Suspendido))
{
	// Suspender
}

// Helpers
if (estado.EstaActivo) { }
if (estado.EstaSuspendido) { }
```

### Rol
```csharp
// Instancias predefinidas
Rol.SuperAdmin
Rol.Admin
Rol.Operador
Rol.UsuarioGeneral

// Factory methods
var rol = Rol.Create(2); // Admin
var rol = Rol.DesdeString("Admin");

// Verificar permisos
if (rol.TienePermiso("CREAR_USUARIOS")) { }
if (rol.PuedeClasificarTickets) { }
if (rol.PuedeCrearUsuarios) { }

// Jerarquía
int nivel = rol.ObtenerNivelJerarquia(); // 1=SuperAdmin, 2=Admin, etc
if (rol.TieneMasPermisosQue(otroRol)) { }

// Helpers
if (rol.EsAdministrador) { }
if (rol.EsSuperAdmin) { }
```

---

## ⚠️ EXCEPCIONES A MANEJAR

```csharp
try
{
	inquilino.SuspenderServicio(adminId);
}
catch (InquilinoYaSuspendidoException ex)
{
	// El inquilino ya está suspendido
	Console.WriteLine($"Inquilino {ex.InquilinoId} ya suspendido");
}
catch (InquilinoInvalidStateTransitionException ex)
{
	// Transición no permitida
	Console.WriteLine($"No se puede ir de {ex.EstadoActual} a {ex.EstadoDestino}");
}
catch (ArgumentException ex)
{
	// Validación de parámetros
}

try
{
	usuario.CambiarRol(nuevoRol, modificadorId);
}
catch (UsuarioInactivoException)
{
	// Usuario inactivo
}
catch (UsuarioYaActivoException)
{
	// Usuario ya activo
}

try
{
	riesgo.ReevaluarImpacto(nuevoNivel, userId);
}
catch (RiesgoOperacionalMismoNivelImpactoException)
{
	// Mismo nivel, sin cambio
}
```

---

## 🔔 EVENTOS DOMAIN A ESCUCHAR

### Application/EventHandlers deben implementar handlers para:

```csharp
// Inquilino
INotificationHandler<InquilinoRegistradoEvent>
INotificationHandler<PrivacidadIAModificadaEvent> // Trigger: sanitización
INotificationHandler<InquilinoSuspendidoEvent>
INotificationHandler<InquilinoReactivadoEvent>

// Usuario
INotificationHandler<UsuarioCreadoEvent>
INotificationHandler<ContrasenaActualizadaEvent>
INotificationHandler<RolModificadoEvent>
INotificationHandler<UsuarioDesactivadoEvent>
INotificationHandler<UsuarioReactivadoEvent>

// Ticket
INotificationHandler<TicketCreadoEvent> // Trigger: Hangfire job (sanitización)
INotificationHandler<TicketClasificadoEvent> // Notificar operador
INotificationHandler<OperadorAsignadoEvent> // Notificar asignado
INotificationHandler<TicketResueltoEvent> // Actualizar métricas
```

---

## 📊 PATRONES IMPLEMENTADOS

### 1. **Domain-Driven Design**
- Agregados con lógica de negocio
- Value Objects para conceptos ricos
- Eventos de dominio reactivos

### 2. **Máquinas de Estado**
- `EstadoTicket`: 7 estados con transiciones validadas
- `EstadoInquilino`: 2 estados (Activo ↔ Suspendido)
- Previene operaciones inválidas

### 3. **Excepciones de Dominio**
- Heredan de `DomainException`
- Específicas por agregado
- Comunican invariantes rotos

### 4. **Auditoría Obligatoria**
- `CreatedBy`, `CreatedAt` en construcción
- `ModifiedBy`, `ModifiedAt` en mutaciones
- Rastreabilidad total

### 5. **Value Objects**
- Encapsulan lógica de negocio
- Comparación por valor
- Factory methods para validación

---

## 🧪 PATRONES DE TEST (Futura Fase)

```csharp
[TestClass]
public class InquilinoTests
{
	[TestMethod]
	public void SuspenderServicio_CuandoEstaActivo_Sucede()
	{
		// Arrange
		var inquilino = new Inquilino("Empresa", "empresa.com", adminId);

		// Act
		inquilino.SuspenderServicio(adminId, "Test");

		// Assert
		Assert.IsTrue(inquilino.Estado.EstaSuspendido);
		Assert.AreEqual(1, inquilino.DomainEvents.Count);
	}

	[TestMethod]
	[ExpectedException(typeof(InquilinoYaSuspendidoException))]
	public void SuspenderServicio_CuandoYaEstaSuspendido_Falla()
	{
		var inquilino = new Inquilino("Empresa", "empresa.com", adminId);
		inquilino.SuspenderServicio(adminId, "Test");

		// Act: Intentar suspender nuevamente
		inquilino.SuspenderServicio(adminId, "Test");

		// Assert: Debe lanzar excepción
	}
}
```

---

## 🚀 FLUJOS TÍPICOS

### Crear Ticket y Clasificar por IA
```csharp
// 1. Usuario crea ticket
var ticket = new Ticket(inquilinoId, titulo, descripción, userId);
repo.Add(ticket);
await unitOfWork.SaveAsync(); // Emite TicketCreadoEvent

// 2. Handler de TicketCreadoEvent enriquece el enqueue Hangfire
// Hangfire job ejecuta: sanitizar() + clasificar()

// 3. Callback de clasificación emite TicketClasificadoEvent
ticket.ClasificarPorIA(prioridad, categoriaId);
await repo.Update(ticket);
await unitOfWork.SaveAsync(); // Emite TicketClasificadoEvent

// 4. Handler de TicketClasificadoEvent notifica operador
```

### Cambiar Permisos IA de Inquilino
```csharp
// 1. Admin actualiza permisos
inquilino.ActualizarPermisosIA(permitir: true, adminId);
await repo.Update(inquilino);
await unitOfWork.SaveAsync(); // Emite PrivacidadIAModificadaEvent

// 2. Handler de PrivacidadIAModificadaEvent puede:
//    - Notificar cambio
//    - Loguear en auditoría
//    - Trigger: iniciar proceso de consentimiento
```

---

## 📚 REFERENCIAS

- Tickets: Ver `PIGI-PT-Domain/Entities/Ticket.cs`
- ValueObjects: Ver `PIGI-PT-Domain/ValueObjects/`
- Exceptions: Ver `PIGI-PT-Domain/Exceptions/`
- Events: Ver `PIGI-PT-Domain/Events/`
- Architecture: Ver `ARCHITECTURE_SUMMARY.md`

---

**Última actualización:** Enero 2024  
**Versión:** 1.0  
**Status:** ✅ COMPLETADO
