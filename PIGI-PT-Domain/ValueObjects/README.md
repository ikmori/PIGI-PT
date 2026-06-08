# Value Objects

## Propósito
Esta carpeta contiene los **Value Objects** del dominio. A diferencia de las Entidades, los Value Objects:

- **No tienen identidad**: Se comparan por sus valores, no por su ID
- **Son inmutables**: Una vez creados, no se modifican
- **Encapsulan lógica**: Contienen validaciones y comportamientos relacionados con ese valor
- **Son reutilizables**: Pueden usarse en múltiples agregados
- **Facilitan testing**: Son fáciles de testear sin dependencias externas

## Value Objects del Sistema PIGI-PT

### 1. **NivelPrioridad** (`NivelPrioridad.cs`)
Refactorización del enum `NivelPrioridad` actual a un Value Object rico.

**Valores**:
- `NoDefinida` (0)
- `Baja` (1)
- `Media` (2)
- `Alta` (3)
- `Crítica` (4)

**Métodos**:
- `IsMayorQue(NivelPrioridad other)`: Compara niveles
- `DesdeString(string value)`: Factory method para crear desde string
- `AEscalaNumerica()`: Retorna valor numérico para cálculos

**Ventajas**:
```csharp
// Antes (con enum)
var prioridad = NivelPrioridad.Alta;
if (prioridad == NivelPrioridad.Alta) { }

// Después (con Value Object)
var prioridad = NivelPrioridad.Create(NivelPrioridadEnum.Alta);
if (prioridad.IsMayorQue(NivelPrioridad.Media)) { }
```

### 2. **EstadoTicket** (`EstadoTicket.cs`)
Refactorización del enum `EstadoTicket` actual a un Value Object que encapsula transiciones de estado válidas.

**Valores**:
- `PendienteDeAnalisis` (1)
- `Clasificado` (2)
- `EnProgreso` (3)
- `Resuelto` (4)
- `Cancelado` (5)
- `Rechazado` (6)

**Métodos**:
- `PuedeTransicionarA(EstadoTicket nuevoEstado)`: Valida transiciones permitidas
- `RequiereOperador()`: Retorna true si el estado requiere operador asignado
- `EsFinal()`: Retorna true si es un estado terminal

**Máquina de Estados**:
```
PendienteDeAnalisis → Clasificado → EnProgreso → Resuelto
				   ↘ Rechazado
```

### 3. **Rol** (`Rol.cs`)
Refactorización del enum `Rol` actual a un Value Object que encapsula permisos y niveles de acceso.

**Valores**:
- `SuperAdmin` (1) - Control total del sistema
- `Admin` (2) - Control de inquilino
- `Operador` (3) - Gestión de tickets
- `UsuarioGeneral` (4) - Crear y consultar sus propios tickets

**Métodos**:
- `TienePermiso(string permiso)`: Verifica si el rol tiene un permiso específico
- `Permisos()`: Retorna colección de permisos del rol
- `Puede(Accion accion)`: Valida si puede realizar una acción

**Matriz de Permisos**:
| Rol | Ver Tickets | Clasificar | Asignar Operador | Crear Categorías | Crear Usuarios | Suspender Inquilino |
|-----|-------------|-----------|------------------|-----------------|---------------|-------------------|
| SuperAdmin | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| Admin | ✓ | ✓ | ✓ | ✓ | ✓ | ✗ |
| Operador | ✓ | ✓ | ✓ | ✗ | ✗ | ✗ |
| UsuarioGeneral | Solo propios | ✗ | ✗ | ✗ | ✗ | ✗ |

### 4. **Email** (`Email.cs`)
Encapsula validación y formato de direcciones de correo.

**Responsabilidades**:
- Validar formato RFC 5322 (simplificado)
- Normalizar (lowercase)
- Comparación case-insensitive
- Construcción con factory method

```csharp
var email = Email.Create("Usuario@Example.COM");
// email.Value == "usuario@example.com"
```

### 5. **DescripcionSanitizada** (`DescripcionSanitizada.cs`)
Encapsula una descripción que ha sido validada y sanitizada.

**Responsabilidades**:
- Validar longitud
- Verificar que contiene tokens de enmascaramiento
- Garantizar que NO contiene datos sensibles

### 6. **ServicioCritico** (`ServicioCritico.cs`)
Value Object para representar un servicio crítico de infraestructura.

**Responsabilidades**:
- Validar que el nombre no está vacío
- Almacenar información de contacto para escaladas

## Estructura de Carpetas

```
ValueObjects/
├── NivelPrioridad.cs
├── EstadoTicket.cs
├── Rol.cs
├── Email.cs
├── DescripcionSanitizada.cs
├── ServicioCritico.cs
└── README.md                  (Este archivo)
```

## Patrón de Implementación

### Estructura Básica de un Value Object

```csharp
public class MiValueObject : ValueObject
{
	// Propiedades de solo lectura
	public string Valor { get; private set; }

	// Constructor privado para EF Core
	private MiValueObject() { }

	// Factory method para crear instancias
	public static MiValueObject Create(string valor)
	{
		if (string.IsNullOrWhiteSpace(valor))
			throw new DomainException("El valor es obligatorio.");

		return new MiValueObject { Valor = valor };
	}

	// Métodos de dominio
	public bool EsValido() => !string.IsNullOrWhiteSpace(Valor);

	// Comparación por valor (heredado de ValueObject)
	protected override IEnumerable<object> GetEqualityComponents()
	{
		yield return Valor;
	}
}
```

### Clase Base ValueObject

Ubicada en `Base/ValueObject.cs`, implementa:
- `Equals()` por comparación de valores
- `GetHashCode()` coherente con Equals
- `==` y `!=` operators

## Beneficios

1. **Type Safety**: No confundir prioridades con números
2. **Validación Centralizada**: Las reglas están en un solo lugar
3. **Comportamiento Anidado**: Los Value Objects pueden tener métodos de dominio
4. **Testabilidad**: Fáciles de testear sin mocks
5. **Documentación Viva**: El código es auto-documentante

## Ejemplos de Uso

```csharp
// En Aggregate Root (Ticket)
public class Ticket : InquilinoEntity
{
	public NivelPrioridad Prioridad { get; private set; }
	public EstadoTicket Estado { get; private set; }

	public void ClasificarPorIA(NivelPrioridad prioridad, Guid categoriaId)
	{
		if (!Estado.PuedeTransicionarA(EstadoTicket.Clasificado))
			throw new DomainException("No se puede clasificar desde este estado.");

		Prioridad = prioridad;
		Estado = EstadoTicket.Clasificado;
	}
}

// En Application Handler
public class CreateTicketCommandHandler
{
	public async Task<Result<TicketDto>> Handle(CreateTicketCommand request)
	{
		var email = Email.Create(request.EmailSolicitante);
		var ticket = new Ticket(
			request.InquilinoId,
			request.Titulo,
			request.Descripcion,
			request.UserId
		);

		await _ticketRepository.AddAsync(ticket);
		return TicketMapper.ToDto(ticket);
	}
}
```

## Guía de Extensión

Cuando necesites crear un nuevo Value Object:

1. Crea el archivo `NombreVO.cs` en esta carpeta
2. Hereda de `ValueObject` (clase base en `Base/`)
3. Define propiedades de solo lectura
4. Implementa factory method `Create()`
5. Implementa `GetEqualityComponents()`
6. Agrega métodos de dominio según sea necesario
7. Documenta los invariantes en comentarios XML
8. Añade tests unitarios en `Tests/Domain/ValueObjects/`

## Validación en EF Core

Para que EF Core persista correctamente los Value Objects, requieren Value Converters:

```csharp
// En DbContext
modelBuilder.Entity<Ticket>()
	.Property(t => t.Prioridad)
	.HasConversion(
		v => v.Valor,
		v => NivelPrioridad.DesdeString(v));
```

Ver `Infrastructure/Persistence/Configurations/` para implementación.
