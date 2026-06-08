# Aggregates (Raíces de Agregados)

## Propósito
Esta carpeta contiene las **Aggregate Roots** (raíces de agregados) del sistema. Cada agregado es una unidad transaccional independiente que encapsula lógica de negocio compleja y garantiza la consistencia de sus datos internos.

## Principios DDD Aplicados

### ¿Qué es un Aggregate?
- **Raíz de Agregado (Aggregate Root)**: Entidad principal que controla el acceso a todas las entidades internas del agregado
- **Entidades Internas**: Objetos con identidad que solo pueden ser accedidos a través de la raíz
- **Value Objects**: Objetos sin identidad que representan valores del dominio
- **Consistencia Transaccional**: Todo el agregado es guardado/recuperado como una unidad

### Agregados del Sistema PIGI-PT

#### 1. **TicketAggregate** (`Ticket.cs`)
- **Raíz**: `Ticket`
- **Responsabilidad**: Gestionar el ciclo de vida completo de un incidente
- **Entidades Internas**: 
  - `Análisis` (futuro: información sobre análisis de IA aplicados)
  - `Historial` (futuro: registro de cambios)
- **Value Objects**: 
  - `EstadoTicket` (nuevo: refactorizado de enum)
  - `NivelPrioridad` (nuevo: refactorizado de enum)
- **Invariantes**:
  - No se puede asignar operador a un ticket resuelto
  - Solo se puede clasificar un ticket pendiente de análisis
  - Un ticket resuelto requiere operador asignado
- **Eventos de Dominio**: `TicketCreadoEvent`, `TicketClasificadoEvent`, `OperadorAsignadoEvent`, `TicketResueltoEvent`

#### 2. **InquilinoAggregate** (`Inquilino.cs`)
- **Raíz**: `Inquilino`
- **Responsabilidad**: Representar una organización dentro del sistema multi-tenancy
- **Entidades Internas**: (ninguna en la versión actual)
- **Value Objects**: 
  - Configuraciones de seguridad y privacidad
- **Invariantes**:
  - El nombre comercial es obligatorio y único
  - La integración con IA está deshabilitada por defecto (privacidad por diseño)
  - Un inquilino puede ser suspendido pero no eliminado
- **Eventos de Dominio**: `InquilinoRegistradoEvent`, `PrivacidadIAModificadaEvent`, `InquilinoSuspendidoEvent`

#### 3. **UsuarioAggregate** (`Usuario.cs`)
- **Raíz**: `Usuario`
- **Responsabilidad**: Gestionar identidades y permisos dentro de un inquilino
- **Entidades Internas**: (ninguna en la versión actual)
- **Value Objects**: 
  - `Rol` (nuevo: refactorizado de enum)
- **Invariantes**:
  - Email debe ser único dentro del inquilino
  - Contraseña debe almacenarse hasheada (nunca en texto plano)
  - Un usuario desactivado no puede acceder al sistema
- **Eventos de Dominio**: `UsuarioCreadoEvent`, `ContrasenaActualizadaEvent`, `RolModificadoEvent`, `DesactivarPerfilEvent`

#### 4. **RiesgoOperacionalAggregate** (`RiesgoOperacional.cs`)
- **Raíz**: `RiesgoOperacional`
- **Responsabilidad**: Documentar y gestionar riesgos de infraestructura
- **Entidades Internas**: (ninguna en la versión actual)
- **Value Objects**: 
  - `NivelDeImpacto` (Value Object para nivel de riesgo)
- **Invariantes**:
  - El servicio afectado es obligatorio
  - El plan de mitigación no puede estar vacío
  - La fecha de última revisión se actualiza siempre que se modifica
- **Eventos de Dominio**: `RiesgoRegistradoEvent`, `RiesgoCriterioModificadoEvent`, `RiesgoEliminadoEvent`

#### 5. **CategoriaAggregate** (`Categoria.cs`)
- **Raíz**: `Categoria`
- **Responsabilidad**: Clasificar tipos de incidentes dentro de un inquilino
- **Entidades Internas**: (ninguna)
- **Value Objects**: (ninguno especializado)
- **Invariantes**:
  - El nombre de categoría es obligatorio dentro del inquilino
  - Las categorías no pueden ser eliminadas, solo desactivadas
- **Eventos de Dominio**: `CategoriaRegistradaEvent`, `CategoriaActualizadaEvent`, `CategoriaDesactivadaEvent`

## Estructura de Carpetas

```
Aggregates/
├── Ticket/
│   ├── Ticket.cs                  (Raíz de agregado)
│   ├── RegistroDeHistorial.cs     (Entidad del historial del ticket)
│   └── TipoDeAccionHistorial.cs   (Enum de acciones de historial)
├── Inquilino/
│   └── Inquilino.cs               (Raíz de agregado)
├── Usuario/
│   └── Usuario.cs                 (Raíz de agregado)
├── RiesgoOperacional/
│   └── RiesgoOperacional.cs       (Raíz de agregado)
├── Categoria/
│   └── Categoria.cs               (Raíz de agregado)
└── README.md                      (Este archivo)
```

## Patrones Implementados

### 1. **Invariantes de Dominio**
Cada agregado valida sus invariantes en los métodos públicos. Si se viola una invariante, se lanza una `DomainException`.

### 2. **Constructores Privados**
Cada agregado tiene un constructor privado sin parámetros para EF Core. Los constructores públicos contienen la lógica de validación.

### 3. **Propiedades de Solo Lectura**
Una vez creado el agregado, sus propiedades se modifican a través de métodos públicos específicos, no directamente.

### 4. **Eventos de Dominio**
Al finalizar una operación importante, el agregado emite un evento de dominio que comunica el cambio al resto del sistema.

### 5. **Multi-Tenancy**
Todos los agregados (excepto Inquilino) heredan de `InquilinoEntity`, asegurando que todo dato está asociado a un inquilino específico.

## Relaciones Entre Agregados

```
Inquilino (Aggregate Root)
├── Usuarios (Aggregate Root - cada uno referencia InquilinoId)
├── Tickets (Aggregate Root - cada uno referencia InquilinoId)
├── Categorías (Aggregate Root - cada uno referencia InquilinoId)
└── RiesgosOperacionales (Aggregate Root - cada uno referencia InquilinoId)
```

**Nota**: Los agregados **no se referencian directamente** entre sí, solo comparten `InquilinoId`. Los JOIN y relaciones se manejan en la capa de Infraestructura.

## Guía de Extensión

Si necesitas agregar un nuevo Aggregate:

1. Crea una carpeta bajo `Aggregates/` con el nombre del agregado
2. Implementa la clase Aggregate Root heredando de `BaseEntity` o `InquilinoEntity`
3. Define invariantes claros en el constructor
4. Implementa métodos públicos para cada operación de negocio
5. Emite Domain Events en los puntos clave
6. Crea un `README.md` documentando el agregado
7. Define los repositorios en `Ports/Repositories/` dentro de Application

## Mejoras Futuras

- [ ] Implementar Specification Pattern para queries complejas
- [ ] Agregar métodos `ApplyDomainEvent(DomainEvent)` para Event Sourcing
- [ ] Crear Entities internas para agregar riqueza a los agregados
- [ ] Implementar validación de reglas de negocio más complejas
