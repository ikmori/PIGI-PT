# Base (Clases Base del Dominio)

## Propósito
Esta carpeta contiene las **clases base** fundamentales que soportan toda la arquitectura DDD. Proporcionan funcionalidades comunes como gestión de IDs, auditoría, eventos de dominio y comparación de valores.

## Clases Base

### 1. **BaseEntity** (`BaseEntity.cs`)
Clase base para todas las entidades del dominio.

**Responsabilidades**:
- Proporcionar identidad única (GUID)
- Gestionar metadatos de auditoría (CreatedAt, CreatedBy, ModifiedAt, ModifiedBy)
- Mantener estado de activación (IsActive)
- Almacenar eventos de dominio pendientes de publicación

**Propiedades**:
```csharp
public abstract class BaseEntity
{
	public Guid Id { get; protected set; }
	public DateTime CreatedAt { get; protected set; }
	public Guid? CreatedBy { get; protected set; }
	public DateTime? ModifiedAt { get; protected set; }
	public Guid? ModifiedBy { get; protected set; }
	public bool IsActive { get; protected set; }
	public IReadOnlyCollection<DomainEvent> DomainEvents { get; }
}
```

**Métodos**:
- `AddDomainEvent(DomainEvent)`: Registra un evento para posterior publicación
- `ClearDomainEvents()`: Limpia eventos después de persistir

**Constructores**:
- `protected BaseEntity()`: Constructor sin parámetros para EF Core
- `protected BaseEntity(bool generateId)`: Genera ID, fecha y estado inicial

### 2. **InquilinoEntity** (`InquilinoEntity.cs`)
Clase base para todas las entidades que pertenecen a un inquilino (multi-tenancy).

**Responsabilidades**:
- Garantizar que cada entidad está asociada a un inquilino
- Facilitar filtrado automático de datos por tenant
- Encapsular la lógica de validación de tenant

**Propiedades**:
```csharp
public abstract class InquilinoEntity : BaseEntity
{
	public Guid InquilinoId { get; protected set; }
}
```

**Invariantes**:
- `InquilinoId` nunca puede ser `Guid.Empty`
- `InquilinoId` se asigna en el constructor y no puede cambiar

**Herencia**:
- Todas las entidades excepto `Inquilino` heredan de esta clase
- Entidades: Ticket, Usuario, RiesgoOperacional, Categoria

### 3. **ValueObject** (`ValueObject.cs`)
Clase base para todos los Value Objects del dominio.

**Responsabilidades**:
- Implementar comparación por valor (no por referencia)
- Garantizar que dos Value Objects son iguales si sus valores son iguales
- Proporcionar implementación correcta de `GetHashCode()` para uso en colecciones

**Métodos Abstractos**:
```csharp
public abstract class ValueObject
{
	public abstract override bool Equals(object obj);
	public abstract override int GetHashCode();
	protected abstract IEnumerable<object> GetEqualityComponents();
}
```

**Implementación Genérica**:
```csharp
public override bool Equals(object obj)
{
	if (obj == null || obj.GetType() != GetType())
		return false;

	var other = (ValueObject)obj;
	return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
}

public override int GetHashCode()
{
	return GetEqualityComponents()
		.Select(x => x?.GetHashCode() ?? 0)
		.Aggregate((x, y) => x ^ y);
}
```

**Ejemplos**:
```csharp
public class NivelPrioridad : ValueObject
{
	public int Valor { get; private set; }
	public string Nombre { get; private set; }

	protected override IEnumerable<object> GetEqualityComponents()
	{
		yield return Valor;
		yield return Nombre;
	}
}

// Uso
var p1 = NivelPrioridad.Create("Alta");
var p2 = NivelPrioridad.Create("Alta");
Assert.Equal(p1, p2); // ✓ True - comparación por valor
```

### 4. **DomainEvent** (`DomainEvent.cs`)
Clase base para todos los eventos de dominio.

**Responsabilidades**:
- Proporcionar metadatos comunes a todos los eventos
- Implementar `INotification` para integración con MediatR
- Registrar cuándo sucedió el evento

**Propiedades**:
```csharp
public abstract class DomainEvent : INotification
{
	public bool IsPublished { get; set; }
	public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
}
```

**Notas**:
- Los eventos son **inmutables** una vez creados
- `IsPublished` se actualiza después de que los handlers procesen el evento
- La fecha se captura automáticamente en la creación

## Estructura de Carpetas

```
Base/
├── BaseEntity.cs              (Clase base para entidades)
├── InquilinoEntity.cs         (Clase base para entidades multi-tenant)
├── ValueObject.cs             (Clase base para value objects)
├── DomainEvent.cs             (Clase base para eventos de dominio)
└── README.md                  (Este archivo)
```

## Patrones de Uso

### Crear una Nueva Entidad

```csharp
public class MiEntidad : InquilinoEntity
{
	public string Nombre { get; private set; }

	// Constructor privado para EF Core
	private MiEntidad() : base() { }

	// Constructor público para crear instancias
	public MiEntidad(Guid inquilinoId, string nombre) : base(inquilinoId)
	{
		if (string.IsNullOrWhiteSpace(nombre))
			throw new ArgumentException("El nombre es obligatorio.");

		Nombre = nombre;
	}

	// Métodos de dominio
	public void ActualizarNombre(string nuevoNombre, Guid modificadorId)
	{
		if (string.IsNullOrWhiteSpace(nuevoNombre))
			throw new ArgumentException("El nuevo nombre no puede estar vacío.");

		Nombre = nuevoNombre;
		ModifiedAt = DateTime.UtcNow;
		ModifiedBy = modificadorId;

		AddDomainEvent(new MiEntidadActualizadaEvent(Id, nuevoNombre));
	}
}
```

### Crear un Nuevo Value Object

```csharp
public class MiValor : ValueObject
{
	public string Valor { get; private set; }

	private MiValor() { }

	public static MiValor Create(string valor)
	{
		if (string.IsNullOrWhiteSpace(valor))
			throw new ArgumentException("El valor es obligatorio.");

		return new MiValor { Valor = valor };
	}

	protected override IEnumerable<object> GetEqualityComponents()
	{
		yield return Valor;
	}
}
```

### Emitir un Evento de Dominio

```csharp
public class MiEntidadCreada : DomainEvent
{
	public Guid EntidadId { get; }
	public string Nombre { get; }
	public Guid CreadorId { get; }

	public MiEntidadCreada(Guid entidadId, string nombre, Guid creadorId)
	{
		EntidadId = entidadId;
		Nombre = nombre;
		CreadorId = creadorId;
	}
}

// En la entidad
public MiEntidad(Guid inquilinoId, string nombre, Guid creadorId) 
	: base(inquilinoId)
{
	// ...validaciones...

	Nombre = nombre;
	CreatedBy = creadorId;

	AddDomainEvent(new MiEntidadCreada(Id, nombre, creadorId));
}
```

## Ciclo de Vida de una Entidad

```
1. Creación (Constructor Público)
   ├─ Validación de invariantes
   ├─ Asignación de propiedades
   ├─ Generación de ID y fecha
   └─ Emisión de evento (ej: MiEntidadCreadaEvent)

2. Modificación (Métodos Públicos)
   ├─ Validación de precondiciones
   ├─ Cambio de estado
   ├─ Actualización de auditoría (ModifiedAt, ModifiedBy)
   └─ Emisión de evento (ej: MiEntidadActualizadaEvent)

3. Persistencia
   ├─ Infraestructura save la entidad
   └─ Infraestructura extrae y publica eventos

4. Publicación de Eventos
   ├─ MediatR enruta cada evento a sus handlers
   ├─ Handlers pueden ser síncronos o asíncronos
   └─ IsPublished = true
```

## Multi-Tenancy

**Todas las entidades except Inquilino deben herdar de InquilinoEntity:**

```
BaseEntity
├── Inquilino (Raíz de agregado principal)
└── InquilinoEntity
	├── Ticket
	├── Usuario
	├── RiesgoOperacional
	└── Categoria
```

**Beneficios**:
- Aislamiento automático de datos por tenant
- Filtros aplicados consistentemente en repositorios
- Imposible "olvidar" el tenant en queries

**Aplicación en EF Core**:
```csharp
// En DbContext
modelBuilder.Entity<Ticket>()
	.HasQueryFilter(t => t.InquilinoId == _currentTenantId);
```

## Auditoría

Cada entidad rastrean:
- **CreatedAt**: Cuándo fue creada
- **CreatedBy**: Quién la creó (ID del usuario)
- **ModifiedAt**: Cuándo fue modificada por última vez
- **ModifiedBy**: Quién la modificó por última vez

**Uso**:
```csharp
var ticket = new Ticket(
	inquilinoId: Guid.NewGuid(),
	titulo: "Problema de acceso",
	descripcionOriginal: "No puedo acceder al sistema",
	userId: usuarioId
);

// ticket.CreatedAt = DateTime.UtcNow (automático)
// ticket.CreatedBy = usuarioId (asignado explícitamente)
// ticket.ModifiedAt = null (no ha sido modificado)
// ticket.ModifiedBy = null (no ha sido modificado)
```

## Eventos de Dominio

**Flujo**:
```
1. Agregado llama AddDomainEvent(evento)
2. Evento se almacena en _domainEvents List
3. Infraestructura persiste entidad
4. Infraestructura extrae eventos: entity.DomainEvents
5. Infraestructura publica eventos via MediatR
6. MediatR ejecuta handlers (síncronos y asíncronos)
7. Infraestructura llama entity.ClearDomainEvents()
```

**Ventajas**:
- Desacoplamiento total
- Trazabilidad completa
- Fácil testing
- Event Sourcing ready

## Próximas Mejoras

- [ ] Implementar `IComparable<BaseEntity>` para ordenamiento
- [ ] Agregar método `GetChanges()` para detectar cambios
- [ ] Implementar Soft Delete pattern
- [ ] Crear `AggregateRoot` explícita como subclase de BaseEntity
- [ ] Agregar validación con FluentValidation en constructores
