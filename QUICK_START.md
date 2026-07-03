# 📍 QUICK START - PRÓXIMOS PASOS

**Estado Actual:** Fase 1, 2, 3 y WebApp MVP ✅ Completadas | Fase 4 (Hangfire, Adapters y HTTP Integration) ⏳ Próxima  
**Token Budget:** Óptimo - Estructura de controladores e interfaz gráfica completamente integrada y funcional  
**Próximo Hito (Fase 4):** Integración de servicios en segundo plano y conexión HTTP real del frontend  

---

## 📖 DOCUMENTOS CLAVE (LEE EN ESTE ORDEN)

1. **[ROADMAP_FUTURO.md](ROADMAP_FUTURO.md)** ← EMPIEZA AQUÍ
   - Overview de todas las fases (actualizado a versión 2.0)
   - Desglose detallado del progreso actual e hitos futuros
   - Ejemplos de código

2. **[FASE3_API_IMPLEMENTACION.md](FASE3_API_IMPLEMENTACION.md)**
   - Resumen de la implementación de la API REST de incidentes, RBAC y ruteo interactivo
   - Endpoints REST de la pila e información del pipeline de desarrollo

3. **[PHASE2_CHECKLIST.md](PHASE2_CHECKLIST.md)**
   - Checklist de infraestructura, mapeo EF Core, migraciones aplicadas e inyección de dependencias

---

## 🚀 FASE 4: PASOS INICIALES

### Paso 1: Preparar Infrastructure Layer
```bash
cd PIGI-PT-Infraestructure

# Crear carpeta Persistence/DbContext
mkdir -p Persistence/DbContext
mkdir -p Persistence/Configurations
mkdir -p Persistence/Repositories
```

### Paso 1.5: Verificar conexión a base de datos local
El proyecto utiliza **SQL Server LocalDB** para desarrollo local (incluido con Visual Studio).
No se requiere instalación adicional.

```json
// appsettings.Development.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=PigiPtDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
  }
}
```

> ⚠️ **Producción:** Cuando se contrate Azure, el connection string se cambiará a Azure SQL Database en `appsettings.Production.json`. La arquitectura Clean Architecture permite este cambio sin modificar código.

### Paso 2: Crear PigiPtDbContext
Archivo: `PIGI-PT-Infraestructure/Persistence/DbContext/PigiPtDbContext.cs`

```csharp
public class PigiPtDbContext : DbContext
{
	public DbSet<Ticket> Tickets { get; set; }
	public DbSet<Inquilino> Inquilinos { get; set; }
	public DbSet<Usuario> Usuarios { get; set; }
	public DbSet<RiesgoOperacional> RiesgosOperacionales { get; set; }
	public DbSet<Categoria> Categorias { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(
			typeof(PigiPtDbContext).Assembly);
	}
}
```

### Paso 3: Crear Configurations
Ver ROADMAP_FUTURO.md Sección 2.1 para ejemplo completo de:
- `TicketConfiguration.cs`
- `InquilinoConfiguration.cs`
- Value Object converters

### Paso 4: Generate Migration
```bash
# Asegúrate de tener LocalDB disponible (viene con Visual Studio)
dotnet ef migrations add InitialCreate
dotnet ef database update
```

> La base de datos `PigiPtDb` se creará automáticamente en LocalDB.

---

## 📋 TODO LISTA RÁPIDA

### Semana 1 (Infraestructure)
- [ ] 2.1 - DbContext + Configurations + Migrations
- [ ] 2.2 - BaseRepository + Specifications

### Semana 2 (Application)
- [ ] 2.3 - DTOs + Validators
- [ ] 2.4 - Commands/Queries Handlers
- [ ] 2.5 - AutoMapper Profiles

### Semana 3 (Application + Config)
- [ ] 2.6 - Event Handlers
- [ ] 2.7 - Dependency Injection
- [ ] Build & Unit Tests

---

## 🎯 CRITERIOS DE ÉXITO FASE 2

✅ Build sin errores  
✅ Database migrations ejecutadas (LocalDB)  
✅ Repositories genéricas funcionando  
✅ Commands/Queries handlers listos  
✅ AutoMapper mappeos correctos  
✅ Event handlers recibiendo eventos  
✅ Tests unitarios >80% cobertura (recomendado)

---

## 💡 TIPS IMPORTANTES

### NO OLVIDES
- Build después de cada paso: `dotnet build`
- Commit después de completar cada subtarea
- Leer ROADMAP_FUTURO.md completo antes de empezar
- Mantener READMEs actualizados según avances

### EVITAR
- ❌ Modificar Domain (ya está completada)
- ❌ Saltar pasos intermedios
- ❌ Confundir Commands con Queries
- ❌ Poner lógica de negocio en Infrastructure

### RECORDAR
- Application = Orquestación (MediatR)
- Infrastructure = Persistencia (EF Core)
- Thin Controllers (API solo delega)
- Value Objects nunca retornar al cliente (serializar como string)

---

## 📞 REFERENCIAS RÁPIDAS

| Necesito... | Mira... |
|-----------|---------|
| Entender CQRS | ROADMAP_FUTURO.md > Fase 2.4 |
| Ver ejemplo DbContext | ROADMAP_FUTURO.md > Sección 2.1 |
| Ver ejemplo Repository | ROADMAP_FUTURO.md > Sección 2.2 |
| Ver ejemplo DTO | ROADMAP_FUTURO.md > Sección 2.3 |
| Ver ejemplo Command | ROADMAP_FUTURO.md > Sección 2.4 |
| Ver ejemplo Mapper | ROADMAP_FUTURO.md > Sección 2.5 |
| Ver ejemplo Event Handler | ROADMAP_FUTURO.md > Sección 2.6 |
| Ver DI Config | ROADMAP_FUTURO.md > Sección 2.7 |

---

## 🗺️ DESPUÉS DE FASE 2

Una vez completes Fase 2:
1. Fase 3: API Controllers (REST endpoints)
2. Fase 4: Hangfire + External Services
3. Fase 5: Blazor WebApp

Pero primero: **enfócate 100% en Fase 2**

---

## ☁️ ESTRATEGIA DE BASE DE DATOS

| Entorno | Motor | Connection String |
|---|---|---|
| **Desarrollo local** | SQL Server LocalDB | `(localdb)\MSSQLLocalDB` |
| **CI/CD (futuro)** | SQL Server Express | Configurable |
| **Producción (futuro)** | Azure SQL Database | `*.database.windows.net` |

> La migración a Azure SQL es transparente: solo cambia el connection string en `appsettings.Production.json`. No se requieren cambios de código gracias a la abstracción de EF Core.

---

**Último Update:** Junio 2026  
**Build Status:** ✅ Compilación correcta  
**Base de datos:** SQL Server LocalDB (desarrollo) → Azure SQL (producción)  
**Documentación:** ✅ Actualizada  
**Ready for:** Fase 2 Implementation
