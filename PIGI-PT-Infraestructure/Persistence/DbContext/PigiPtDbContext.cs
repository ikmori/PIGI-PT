using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PIGI_PT_Domain.Aggregates.Categoria;
using PIGI_PT_Domain.Aggregates.Inquilino;
using MediatR;
using PIGI_PT_Domain.Aggregates.Ticket;
using PIGI_PT_Domain.Aggregates.Usuario;
using PIGI_PT_Domain.Base;

namespace PIGI_PT_Infraestructure.Persistence.DbContext
{
    /// <summary>
    /// Contexto de base de datos principal para PIGI-PT.
    /// Configura las entidades del dominio utilizando Entity Framework Core,
    /// gestiona la auditoría automática (CreatedAt, ModifiedAt) y
    /// despacha los eventos de dominio antes de guardar los cambios.
    /// </summary>
    public class PigiPtDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        private readonly IMediator? _mediator;

        /// <summary>
        /// Conjunto de datos para los tickets del sistema.
        /// </summary>
        public DbSet<Ticket> Tickets { get; set; } = null!;

        /// <summary>
        /// Conjunto de datos para los inquilinos (tenants) del sistema.
        /// </summary>
        public DbSet<Inquilino> Inquilinos { get; set; } = null!;

        /// <summary>
        /// Conjunto de datos para los usuarios del sistema.
        /// </summary>
        public DbSet<Usuario> Usuarios { get; set; } = null!;

        /// <summary>
        /// Conjunto de datos para las categorías de tickets.
        /// </summary>
        public DbSet<Categoria> Categorias { get; set; } = null!;

        /// <summary>
        /// Constructor para inyección de dependencias con opciones y MediatR.
        /// </summary>
        /// <param name="options">Opciones de configuración del DbContext.</param>
        /// <param name="mediator">Instancia de MediatR para despachar eventos de dominio.</param>
        public PigiPtDbContext(DbContextOptions<PigiPtDbContext> options, IMediator? mediator = null)
            : base(options)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Configura el modelo de entidades aplicando todas las configuraciones
        /// definidas en el ensamblado de infraestructura (IEntityTypeConfiguration).
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplica todas las IEntityTypeConfiguration<T> del ensamblado
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PigiPtDbContext).Assembly);
        }

        /// <summary>
        /// Guarda los cambios en la base de datos, despachando previamente
        /// los eventos de dominio registrados en las entidades modificadas.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <returns>Número de registros afectados.</returns>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Despachar eventos de dominio antes de persistir
            await DispatchDomainEventsAsync(cancellationToken);

            return await base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Recopila y publica todos los eventos de dominio pendientes
        /// en las entidades rastreadas por el Change Tracker.
        /// Limpia los eventos después de publicarlos para evitar duplicados.
        /// </summary>
        private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
        {
            if (_mediator == null)
                return;

            var domainEntities = ChangeTracker
                .Entries<BaseEntity>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            var domainEvents = domainEntities
                .SelectMany(e => e.DomainEvents)
                .ToList();

            // Limpiar eventos antes de publicar para evitar recursión
            domainEntities.ForEach(e => e.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }
        }
    }
}
