# Specifications (Patrón Specification)

## Propósito
Esta carpeta contiene las **Specifications** - implementación del patrón Specification Pattern para encapsular lógica de queries complejas de forma reutilizable, testeable y agnóstica a cómo se persisten los datos.

## ¿Qué es el Patrón Specification?

El patrón Specification permite:

1. **Encapsular criterios de búsqueda**: Evita tener 20 métodos diferentes en Repository
2. **Reutilización**: La misma especificación se usa en múltiples contextos
3. **Testabilidad**: Fácil de testear sin tocar la BD
4. **Mantenibilidad**: Cambios en queries centralizados
5. **Composición**: Combinar múltiples especificaciones

### Comparación

```csharp
// ❌ Sin Specification (anti-patrón)
public interface ITicketRepository
{
	List<Ticket> GetTicketsPorTenant(Guid tenantId);
	List<Ticket> GetTicketsPorTenantYPrioridad(Guid tenantId, NivelPrioridad prioridad);
	List<Ticket> GetTicketsPorTenantYEstado(Guid tenantId, EstadoTicket estado);
	List<Ticket> GetTicketsPorTenantYPrioridadYEstado(Guid tenantId, NivelPrioridad prioridad, EstadoTicket estado);
	// ... 10 más combinaciones
}

// ✅ Con Specification
public interface IRepository<T>
{
	List<T> GetBySpecification(Specification<T> spec);
}

repository.GetBySpecification(
	new TicketsByTenantAndPriorityAndStateSpecification(tenantId, prioridad, estado)
);
```

## Specifications del Sistema PIGI-PT

### Agregado: Ticket

#### 1. **TicketsByTenantSpecification**
Obtiene todos los tickets de un inquilino.

```csharp
public class TicketsByTenantSpecification : Specification<Ticket>
{
	public TicketsByTenantSpecification(Guid tenantId)
	{
		Query.Where(t => t.InquilinoId == tenantId);
	}
}
```

#### 2. **TicketsByTenantAndStateSpecification**
Obtiene tickets de un inquilino en un estado específico.

```csharp
public class TicketsByTenantAndStateSpecification : Specification<Ticket>
{
	public TicketsByTenantAndStateSpecification(Guid tenantId, EstadoTicket estado)
	{
		Query.Where(t => t.InquilinoId == tenantId && t.Estado == estado);
	}
}
```

#### 3. **TicketsByTenantAndPrioritySpecification**
Obtiene tickets de un inquilino con nivel de prioridad específico.

```csharp
public class TicketsByTenantAndPrioritySpecification : Specification<Ticket>
{
	public TicketsByTenantAndPrioritySpecification(Guid tenantId, NivelPrioridad prioridad)
	{
		Query.Where(t => t.InquilinoId == tenantId && t.Prioridad == prioridad);
		Query.OrderByDescending(t => t.Prioridad);
	}
}
```

#### 4. **OpenTicketsByOperatorSpecification**
Obtiene tickets abiertos asignados a un operador específico.

```csharp
public class OpenTicketsByOperatorSpecification : Specification<Ticket>
{
	public OpenTicketsByOperatorSpecification(Guid operatorId)
	{
		Query.Where(t => t.OperadorAsignadoId == operatorId 
					  && t.Estado != EstadoTicket.Resuelto);
		Query.OrderByDescending(t => t.Prioridad)
			 .ThenBy(t => t.CreatedAt);
	}
}
```

#### 5. **PendingAnalysisTicketsSpecification**
Obtiene tickets pendientes de análisis (para Hangfire).

```csharp
public class PendingAnalysisTicketsSpecification : Specification<Ticket>
{
	public PendingAnalysisTicketsSpecification(Guid? tenantId = null)
	{
		Query.Where(t => t.Estado == EstadoTicket.PendienteDeAnalisis);

		if (tenantId.HasValue)
			Query.Where(t => t.InquilinoId == tenantId.Value);
	}
}
```

#### 6. **TicketsCreatedByUserSpecification**
Obtiene tickets creados por un usuario específico.

```csharp
public class TicketsCreatedByUserSpecification : Specification<Ticket>
{
	public TicketsCreatedByUserSpecification(Guid userId)
	{
		Query.Where(t => t.CreatedBy == userId);
		Query.OrderByDescending(t => t.CreatedAt);
	}
}
```

#### 7. **PriorityCriticalTicketsSpecification**
Obtiene todos los tickets críticos (para alertas).

```csharp
public class PriorityCriticalTicketsSpecification : Specification<Ticket>
{
	public PriorityCriticalTicketsSpecification()
	{
		Query.Where(t => t.Prioridad == NivelPrioridad.Critica 
					  && t.Estado != EstadoTicket.Resuelto);
		Query.OrderBy(t => t.CreatedAt); // Más antiguos primero
	}
}
```

### Agregado: Usuario

#### 1. **ActiveUsersByTenantSpecification**
Obtiene usuarios activos de un inquilino.

```csharp
public class ActiveUsersByTenantSpecification : Specification<Usuario>
{
	public ActiveUsersByTenantSpecification(Guid tenantId)
	{
		Query.Where(u => u.InquilinoId == tenantId && u.IsActive);
	}
}
```

#### 2. **UsersByRoleSpecification**
Obtiene usuarios con un rol específico.

```csharp
public class UsersByRoleSpecification : Specification<Usuario>
{
	public UsersByRoleSpecification(Guid tenantId, Rol rol)
	{
		Query.Where(u => u.InquilinoId == tenantId && u.Rol == rol);
	}
}
```

#### 3. **UserByEmailSpecification**
Obtiene un usuario por email (búsqueda única).

```csharp
public class UserByEmailSpecification : Specification<Usuario>
{
	public UserByEmailSpecification(string email)
	{
		Query.Where(u => u.Email == email);
	}
}
```

### Agregado: RiesgoOperacional

#### 1. **OperationalRisksByTenantSpecification**
Obtiene riesgos operacionales de un inquilino.

```csharp
public class OperationalRisksByTenantSpecification : Specification<RiesgoOperacional>
{
	public OperationalRisksByTenantSpecification(Guid tenantId)
	{
		Query.Where(r => r.InquilinoId == tenantId && r.IsActive);
		Query.OrderByDescending(r => r.NivelDeImpacto);
	}
}
```

#### 2. **CriticalOperationalRisksSpecification**
Obtiene riesgos críticos (para alertas).

```csharp
public class CriticalOperationalRisksSpecification : Specification<RiesgoOperacional>
{
	public CriticalOperationalRisksSpecification()
	{
		Query.Where(r => r.NivelDeImpacto == NivelPrioridad.Critica && r.IsActive);
	}
}
```

### Agregado: Categoria

#### 1. **CategoriesByTenantSpecification**
Obtiene categorías activas de un inquilino.

```csharp
public class CategoriesByTenantSpecification : Specification<Categoria>
{
	public CategoriesByTenantSpecification(Guid tenantId)
	{
		Query.Where(c => c.InquilinoId == tenantId && c.IsActive);
	}
}
```

## Estructura de Carpetas

```
Specifications/
├── Base/
│   └── Specification.cs            (Clase base genérica)
├── Ticket/
│   ├── TicketsByTenantSpecification.cs
│   ├── TicketsByTenantAndStateSpecification.cs
│   ├── TicketsByTenantAndPrioritySpecification.cs
│   ├── OpenTicketsByOperatorSpecification.cs
│   ├── PendingAnalysisTicketsSpecification.cs
│   ├── TicketsCreatedByUserSpecification.cs
│   ├── PriorityCriticalTicketsSpecification.cs
│   └── README.md
├── Usuario/
│   ├── ActiveUsersByTenantSpecification.cs
│   ├── UsersByRoleSpecification.cs
│   ├── UserByEmailSpecification.cs
│   └── README.md
├── RiesgoOperacional/
│   ├── OperationalRisksByTenantSpecification.cs
│   ├── CriticalOperationalRisksSpecification.cs
│   └── README.md
├── Categoria/
│   ├── CategoriesByTenantSpecification.cs
│   └── README.md
└── README.md                  (Este archivo)
```

## Clase Base Specification

```csharp
public abstract class Specification<T> where T : BaseEntity
{
	public IQueryable<T> Query { get; }

	public List<string> Includes { get; } = new();
	public List<string> IncludeStrings { get; } = new();
	public Expression<Func<T, object>> OrderBy { get; set; }
	public Expression<Func<T, object>> OrderByDescending { get; set; }
	public int? Take { get; set; }
	public int? Skip { get; set; }
	public bool IsPagingEnabled { get; set; }

	protected Specification()
	{
		Query = new List<T>().AsQueryable();
	}

	protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
	{
		Includes.Add(includeExpression.ToString());
	}

	protected virtual void AddInclude(string includeString)
	{
		IncludeStrings.Add(includeString);
	}
}
```

## Implementación en EF Core

El Repository implementa la Specification:

```csharp
public class TicketRepository : ITicketRepository
{
	public async Task<List<Ticket>> GetBySpecificationAsync(
		Specification<Ticket> spec)
	{
		return await ApplySpecification(spec).ToListAsync();
	}

	private IQueryable<Ticket> ApplySpecification(Specification<Ticket> spec)
	{
		var query = _context.Tickets.AsQueryable();

		// Apply criteria
		if (spec.Query != null)
			query = query.Where(spec.Query);

		// Apply includes
		query = spec.Includes.Aggregate(query, 
			(current, include) => current.Include(include));

		// Apply ordering
		if (spec.OrderBy != null)
			query = query.OrderBy(spec.OrderBy);

		if (spec.OrderByDescending != null)
			query = query.OrderByDescending(spec.OrderByDescending);

		// Apply paging
		if (spec.IsPagingEnabled)
		{
			if (spec.Skip.HasValue)
				query = query.Skip(spec.Skip.Value);

			if (spec.Take.HasValue)
				query = query.Take(spec.Take.Value);
		}

		return query;
	}
}
```

## Ventajas

1. **DRY**: No repetir criteria en múltiples métodos
2. **Composable**: Especificaciones pueden combinarse
3. **Type-safe**: Compilación verificada
4. **Testeable**: Specifications pueden testearse sin BD
5. **Documentación**: El nombre de la clase documenta el query

## Ejemplos de Uso

```csharp
// En Application Handler
public class GetTicketsForOperatorQueryHandler 
	: IRequestHandler<GetTicketsForOperatorQuery, List<TicketDto>>
{
	private readonly ITicketRepository _repository;

	public async Task<List<TicketDto>> Handle(
		GetTicketsForOperatorQuery request, 
		CancellationToken ct)
	{
		var spec = new OpenTicketsByOperatorSpecification(request.OperatorId);
		var tickets = await _repository.GetBySpecificationAsync(spec);

		return tickets.Select(TicketMapper.ToDto).ToList();
	}
}

// Con Paging
public class GetPaginatedTicketsSpecification : Specification<Ticket>
{
	public GetPaginatedTicketsSpecification(Guid tenantId, int page, int pageSize)
	{
		Query.Where(t => t.InquilinoId == tenantId);
		ApplyPaging(page, pageSize);
	}

	private void ApplyPaging(int page, int pageSize)
	{
		Skip = (page - 1) * pageSize;
		Take = pageSize;
		IsPagingEnabled = true;
	}
}
```

## Guía de Extensión

Cuando necesites una query nueva:

1. Crea archivo `MiNombreSpecification.cs` en la carpeta del agregado
2. Hereda de `Specification<T>`
3. En el constructor, construye el Query usando LINQ
4. Nombra siguiendo patrón: `<Entidad>By<Criterio>Specification`
5. Úsalo en el Repository
6. Úsalo en el Application Handler
7. Documenta aquí

## Próximas Mejoras

- [ ] Agregar paging helper
- [ ] Implementar "Specification With Include" para evitar N+1
- [ ] Cachear resultados de especificaciones comunes
- [ ] Crear builders para especificaciones complejas
- [ ] Documentar performance de cada especificación
