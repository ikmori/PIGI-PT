using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIGI_PT_Domain.Aggregates.Ticket;
using PIGI_PT_Domain.ValueObjects;

namespace PIGI_PT_Infraestructure.Persistence.Configurations
{
    /// <summary>
    /// Configuración de Entity Framework Core para la entidad <see cref="Ticket"/>.
    /// Define el mapeo de propiedades, conversiones de Value Objects,
    /// índices compuestos y restricciones de la tabla Tickets.
    /// </summary>
    public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            // Tabla
            builder.ToTable("Tickets");

            // Primary Key
            builder.HasKey(t => t.Id);

            // --- Propiedades escalares ---

            builder.Property(t => t.Titulo)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.DescripcionOriginal)
                .IsRequired()
                .HasMaxLength(5000);

            builder.Property(t => t.DescripcionSanitizada)
                .HasMaxLength(5000);

            // --- Value Object Converters ---

            builder.Property(t => t.Estado)
                .HasConversion(
                    v => v.Valor,           // To database: int
                    v => EstadoTicket.Create(v) // From database: EstadoTicket
                )
                .IsRequired()
                .HasColumnName("Estado");

            builder.Property(t => t.Prioridad)
                .HasConversion(
                    v => v.Valor,               // To database: int
                    v => NivelPrioridad.Create(v)   // From database: NivelPrioridad
                )
                .IsRequired()
                .HasColumnName("Prioridad");

            // --- Foreign Keys ---

            builder.HasOne<PIGI_PT_Domain.Aggregates.Categoria.Categoria>()
                .WithMany()
                .HasForeignKey(t => t.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<PIGI_PT_Domain.Aggregates.Usuario.Usuario>()
                .WithMany()
                .HasForeignKey(t => t.ResponsableTecnologiaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<PIGI_PT_Domain.Aggregates.Inquilino.Inquilino>()
                .WithMany()
                .HasForeignKey(t => t.InquilinoId)
                .OnDelete(DeleteBehavior.Cascade);

            // --- Propiedades de fecha ---

            builder.Property(t => t.FechaResolucion);
            builder.Property(t => t.FechaAsignacion);

            // --- Auditoría (heredadas de BaseEntity) ---

            builder.Property(t => t.CreatedAt).IsRequired();
            builder.Property(t => t.CreatedBy);
            builder.Property(t => t.ModifiedAt);
            builder.Property(t => t.ModifiedBy);
            builder.Property(t => t.IsActive).IsRequired();

            // --- Índices ---

            builder.HasIndex(t => new { t.InquilinoId, t.CreatedAt })
                .HasDatabaseName("IX_Tickets_InquilinoId_CreatedAt");

            builder.HasIndex(t => t.Estado)
                .HasDatabaseName("IX_Tickets_Estado");

            builder.HasIndex(t => t.ResponsableTecnologiaId)
                .HasDatabaseName("IX_Tickets_ResponsableTecnologiaId");

            builder.HasIndex(t => t.CategoriaId)
                .HasDatabaseName("IX_Tickets_CategoriaId");

            // --- Ignorar propiedades calculadas y de dominio ---

            builder.Ignore(t => t.DomainEvents);
            builder.Ignore(t => t.EstaAtrasado);
            builder.Ignore(t => t.RequiereEscalada);
        }
    }
}
