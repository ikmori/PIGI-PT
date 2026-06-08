# Exceptions (Excepciones de Dominio)

## Propósito
Esta carpeta contiene las **Excepciones de Dominio** - excepciones específicas que representan violaciones de invariantes y reglas de negocio del dominio.

## ¿Por Qué Excepciones de Dominio?

Las excepciones de dominio permiten:

1. **Claridad**: El código comunica exactamente qué salió mal
2. **Especificidad**: Diferentes excepciones para diferentes fallos
3. **Handling Granular**: Manejar cada tipo de error apropiadamente
4. **Logging/Auditoría**: Registrar qué invariantes se violaron
5. **API Responses**: Mapear a HTTP status codes específicos

## Jerarquía de Excepciones

```
System.Exception
└── DomainException (Clase Base)
	├── TicketDomainException
	│   ├── TicketInvalidStateTransitionException
	│   ├── TicketMissingOperatorException
	│   ├── TicketAlreadyResolvedException
	│   └── InvalidTicketStateException
	├── InquilinoDomainException
	│   ├── InquilinoAlreadySuspendedException
	│   └── InvalidInquilinoStateException
	├── UsuarioDomainException
	│   ├── DuplicateEmailException
	│   ├── InvalidUserRoleException
	│   └── UserAlreadyDeactivatedException
	├── RiesgoDomainException
	│   ├── InvalidRiskLevelException
	│   └── RiskServiceNotFound Exception
	└── CategoriaDomainException
		├── DuplicateCategoryNameException
		└── CategoryAlreadyInUseException
```

## Excepciones del Sistema PIGI-PT

### Clase Base

#### **DomainException**
```csharp
public abstract class DomainException : Exception
{
	public string Code { get; protected set; }
	public DateTime OccurredAt { get; }

	protected DomainException(string message, string code = "DOMAIN_ERROR")
		: base(message)
	{
		Code = code;
		OccurredAt = DateTime.UtcNow;
	}
}
```

### Ticket

#### **TicketDomainException**
Clase base para excepciones de Ticket.

```csharp
public class TicketDomainException : DomainException
{
	public Guid TicketId { get; }

	protected TicketDomainException(string message, Guid ticketId, string code)
		: base(message, code)
	{
		TicketId = ticketId;
	}
}
```

#### **TicketInvalidStateTransitionException**
Se lanza cuando se intenta una transición de estado no válida.

```csharp
public class TicketInvalidStateTransitionException : TicketDomainException
{
	public EstadoTicket CurrentState { get; }
	public EstadoTicket RequestedState { get; }

	public TicketInvalidStateTransitionException(
		Guid ticketId, 
		EstadoTicket currentState, 
		EstadoTicket requestedState)
		: base(
			$"El ticket {ticketId} no puede transicionar de {currentState} a {requestedState}.",
			ticketId,
			"TICKET_INVALID_STATE_TRANSITION")
	{
		CurrentState = currentState;
		RequestedState = requestedState;
	}
}
```

**Uso**:
```csharp
// En Ticket.cs
public void ClasificarPorIA(NivelPrioridad prioridad, Guid categoriaId)
{
	if (Estado != EstadoTicket.PendienteDeAnalisis)
		throw new TicketInvalidStateTransitionException(
			Id, Estado, EstadoTicket.Clasificado);

	// ...
}
```

#### **TicketMissingOperatorException**
Se lanza cuando se intenta resolver un ticket sin operador asignado.

```csharp
public class TicketMissingOperatorException : TicketDomainException
{
	public TicketMissingOperatorException(Guid ticketId)
		: base(
			$"El ticket {ticketId} no puede resolverse sin un operador asignado.",
			ticketId,
			"TICKET_MISSING_OPERATOR")
	{
	}
}
```

#### **TicketAlreadyResolvedException**
Se lanza cuando se intenta modificar un ticket ya resuelto.

```csharp
public class TicketAlreadyResolvedException : TicketDomainException
{
	public TicketAlreadyResolvedException(Guid ticketId)
		: base(
			$"El ticket {ticketId} ya ha sido resuelto y no puede modificarse.",
			ticketId,
			"TICKET_ALREADY_RESOLVED")
	{
	}
}
```

### Inquilino

#### **InquilinoDomainException**
Clase base para excepciones de Inquilino.

```csharp
public class InquilinoDomainException : DomainException
{
	public Guid InquilinoId { get; }

	protected InquilinoDomainException(string message, Guid inquilinoId, string code)
		: base(message, code)
	{
		InquilinoId = inquilinoId;
	}
}
```

#### **InquilinoAlreadySuspendedException**
Se lanza cuando se intenta suspender un inquilino ya suspendido.

```csharp
public class InquilinoAlreadySuspendedException : InquilinoDomainException
{
	public InquilinoAlreadySuspendedException(Guid inquilinoId)
		: base(
			$"El inquilino {inquilinoId} ya ha sido suspendido.",
			inquilinoId,
			"INQUILINO_ALREADY_SUSPENDED")
	{
	}
}
```

### Usuario

#### **UsuarioDomainException**
Clase base para excepciones de Usuario.

```csharp
public class UsuarioDomainException : DomainException
{
	public Guid UsuarioId { get; }
	public Guid InquilinoId { get; }

	protected UsuarioDomainException(
		string message, 
		Guid usuarioId, 
		Guid inquilinoId, 
		string code)
		: base(message, code)
	{
		UsuarioId = usuarioId;
		InquilinoId = inquilinoId;
	}
}
```

#### **DuplicateEmailException**
Se lanza cuando se intenta crear un usuario con email duplicado en el inquilino.

```csharp
public class DuplicateEmailException : UsuarioDomainException
{
	public string Email { get; }

	public DuplicateEmailException(string email, Guid inquilinoId)
		: base(
			$"Ya existe un usuario con el email {email} en este inquilino.",
			Guid.Empty,
			inquilinoId,
			"DUPLICATE_EMAIL")
	{
		Email = email;
	}
}
```

#### **InvalidUserRoleException**
Se lanza cuando se asigna un rol inválido.

```csharp
public class InvalidUserRoleException : UsuarioDomainException
{
	public string ProvidedRole { get; }

	public InvalidUserRoleException(string role, Guid usuarioId, Guid inquilinoId)
		: base(
			$"El rol '{role}' no es válido.",
			usuarioId,
			inquilinoId,
			"INVALID_USER_ROLE")
	{
		ProvidedRole = role;
	}
}
```

#### **UserAlreadyDeactivatedException**
Se lanza cuando se intenta deactivar un usuario ya deactivado.

```csharp
public class UserAlreadyDeactivatedException : UsuarioDomainException
{
	public UserAlreadyDeactivatedException(Guid usuarioId, Guid inquilinoId)
		: base(
			$"El usuario {usuarioId} ya ha sido desactivado.",
			usuarioId,
			inquilinoId,
			"USER_ALREADY_DEACTIVATED")
	{
	}
}
```

### RiesgoOperacional

#### **RiesgoDomainException**
Clase base para excepciones de RiesgoOperacional.

```csharp
public class RiesgoDomainException : DomainException
{
	public Guid RiesgoId { get; }

	protected RiesgoDomainException(string message, Guid riesgoId, string code)
		: base(message, code)
	{
		RiesgoId = riesgoId;
	}
}
```

#### **InvalidRiskLevelException**
Se lanza cuando se proporciona un nivel de impacto inválido.

```csharp
public class InvalidRiskLevelException : RiesgoDomainException
{
	public string ProvidedLevel { get; }

	public InvalidRiskLevelException(string level, Guid riesgoId)
		: base(
			$"El nivel de impacto '{level}' no es válido.",
			riesgoId,
			"INVALID_RISK_LEVEL")
	{
		ProvidedLevel = level;
	}
}
```

### Categoria

#### **CategoriaDomainException**
Clase base para excepciones de Categoria.

```csharp
public class CategoriaDomainException : DomainException
{
	public Guid CategoriaId { get; }

	protected CategoriaDomainException(string message, Guid categoriaId, string code)
		: base(message, code)
	{
		CategoriaId = categoriaId;
	}
}
```

#### **DuplicateCategoryNameException**
Se lanza cuando se intenta crear dos categorías con el mismo nombre en un inquilino.

```csharp
public class DuplicateCategoryNameException : CategoriaDomainException
{
	public string CategoryName { get; }

	public DuplicateCategoryNameException(string categoryName, Guid inquilinoId)
		: base(
			$"Ya existe una categoría llamada '{categoryName}' en este inquilino.",
			Guid.Empty,
			"DUPLICATE_CATEGORY_NAME")
	{
		CategoryName = categoryName;
	}
}
```

## Estructura de Carpetas

```
Exceptions/
├── DomainException.cs                          (Clase base)
├── Ticket/
│   ├── TicketDomainException.cs
│   ├── TicketInvalidStateTransitionException.cs
│   ├── TicketMissingOperatorException.cs
│   ├── TicketAlreadyResolvedException.cs
│   └── README.md
├── Inquilino/
│   ├── InquilinoDomainException.cs
│   ├── InquilinoAlreadySuspendedException.cs
│   └── README.md
├── Usuario/
│   ├── UsuarioDomainException.cs
│   ├── DuplicateEmailException.cs
│   ├── InvalidUserRoleException.cs
│   ├── UserAlreadyDeactivatedException.cs
│   └── README.md
├── RiesgoOperacional/
│   ├── RiesgoDomainException.cs
│   ├── InvalidRiskLevelException.cs
│   └── README.md
├── Categoria/
│   ├── CategoriaDomainException.cs
│   ├── DuplicateCategoryNameException.cs
│   └── README.md
└── README.md                  (Este archivo)
```

## Handling en Application Layer

Las excepciones de dominio se mapean a respuestas HTTP:

```csharp
// En API Middleware
public class ExceptionHandlingMiddleware
{
	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await _next(context);
		}
		catch (Exception ex)
		{
			await HandleExceptionAsync(context, ex);
		}
	}

	private static Task HandleExceptionAsync(HttpContext context, Exception exception)
	{
		context.Response.ContentType = "application/json";

		var response = new ErrorResponse();

		switch (exception)
		{
			case TicketInvalidStateTransitionException ex:
				context.Response.StatusCode = StatusCodes.Status400BadRequest;
				response.Code = ex.Code;
				response.Message = ex.Message;
				break;

			case TicketAlreadyResolvedException ex:
				context.Response.StatusCode = StatusCodes.Status409Conflict;
				response.Code = ex.Code;
				response.Message = ex.Message;
				break;

			case DomainException ex:
				context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
				response.Code = ex.Code;
				response.Message = ex.Message;
				break;

			default:
				context.Response.StatusCode = StatusCodes.Status500InternalServerError;
				response.Code = "INTERNAL_SERVER_ERROR";
				response.Message = "Ocurrió un error interno.";
				break;
		}

		return context.Response.WriteAsJsonAsync(response);
	}
}
```

## Mejores Prácticas

1. **Siempre hereda de DomainException** para excepciones de dominio
2. **Incluye contexto**: Guarda IDs relacionadas
3. **Usa codes únicos**: Facilita debugging y auditoría
4. **Documenta cuándo se lanza**: Añade comentarios XML
5. **No uses para control de flujo**: Las excepciones son para errores, no lógica

## Próximas Mejoras

- [ ] Crear resultado wrapper `Result<T>` para evitar excepciones en ciertos casos
- [ ] Implementar FluentValidation para validaciones más complejas
- [ ] Agregar traducción multiidioma de mensajes
- [ ] Crear correlationId para trazabilidad de errores
