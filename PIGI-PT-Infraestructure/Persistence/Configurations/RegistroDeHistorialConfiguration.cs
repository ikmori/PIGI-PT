using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIGI_PT_Domain.Aggregates.Ticket;

namespace PIGI_PT_Infraestructure.Persistence.Configurations
{
    /// <summary>
    /// Configuración de Entity Framework Core para la entidad <see cref="RegistroDeHistorial"/>.
    /// Define el mapeo de propiedades, conversión de enum TipoDeAccionHistorial,
    /// índices y restricciones de la tabla RegistrosDeHistorial.
    /// </summary>
    public class RegistroDeHistorialConfiguration : IEntityTypeConfiguration<RegistroDeHistorial>
    {
        public void Configure(EntityTypeBuilder<RegistroDeHistorial> builder)
        {
            // Tabla
            builder.ToTable("RegistrosDeHistorial");

            // Primary Key
            builder.HasKey(r => r.Id);

            // --- Propiedades escalares ---

            builder.Property(r => r.TicketId)
                .IsRequired();

            builder.Property(r => r.TipoDeAccion)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(r => r.ValoresAnteriores)
                .HasMaxLength(5000);

            builder.Property(r => r.ValoresNuevos)
                .IsRequired()
                .HasMaxLength(5000);

            // --- Multi-tenant ---

            builder.Property(r => r.InquilinoId).IsRequired();

            // --- Auditoría (heredadas de BaseEntity) ---

            builder.Property(r => r.CreatedAt).IsRequired();
            builder.Property(r => r.CreatedBy);
            builder.Property(r => r.ModifiedAt);
            builder.Property(r => r.ModifiedBy);
            builder.Property(r => r.IsActive).IsRequired();

            // --- Índices ---

            builder.HasIndex(r => r.TicketId)
                .HasDatabaseName("IX_RegistrosDeHistorial_TicketId");

            builder.HasIndex(r => new { r.TicketId, r.CreatedAt })
                .HasDatabaseName("IX_RegistrosDeHistorial_TicketId_CreatedAt");

            builder.HasIndex(r => r.InquilinoId)
                .HasDatabaseName("IX_RegistrosDeHistorial_InquilinoId");

            // --- Ignorar propiedades de dominio ---

            builder.Ignore(r => r.DomainEvents);
        }
    }
}
