using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIGI_PT_Domain.Aggregates.RiesgoOperacional;
using PIGI_PT_Domain.ValueObjects;

namespace PIGI_PT_Infraestructure.Persistence.Configurations
{
    /// <summary>
    /// Configuración de Entity Framework Core para la entidad <see cref="RiesgoOperacional"/>.
    /// Define el mapeo de propiedades, conversión del Value Object NivelPrioridad (NivelDeImpacto),
    /// índices y restricciones de la tabla RiesgosOperacionales.
    /// </summary>
    public class RiesgoOperacionalConfiguration : IEntityTypeConfiguration<RiesgoOperacional>
    {
        public void Configure(EntityTypeBuilder<RiesgoOperacional> builder)
        {
            // Tabla
            builder.ToTable("RiesgosOperacionales");

            // Primary Key
            builder.HasKey(r => r.Id);

            // --- Propiedades escalares ---

            builder.Property(r => r.ServicioAfectado)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(r => r.DescripcionAmenaza)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(r => r.PlanDeMitigacion)
                .IsRequired()
                .HasMaxLength(5000);

            builder.Property(r => r.FechaUltimaRevision)
                .IsRequired();

            // --- Value Object Converter ---

            builder.Property(r => r.NivelDeImpacto)
                .HasConversion(
                    v => v.Valor,                   // To database: int
                    v => NivelPrioridad.Create(v)   // From database: NivelPrioridad
                )
                .IsRequired()
                .HasColumnName("NivelDeImpacto");

            // --- Multi-tenant ---

            builder.Property(r => r.InquilinoId).IsRequired();

            // --- Auditoría (heredadas de BaseEntity) ---

            builder.Property(r => r.CreatedAt).IsRequired();
            builder.Property(r => r.CreatedBy);
            builder.Property(r => r.ModifiedAt);
            builder.Property(r => r.ModifiedBy);
            builder.Property(r => r.IsActive).IsRequired();

            // --- Índices ---

            builder.HasIndex(r => r.InquilinoId)
                .HasDatabaseName("IX_RiesgosOperacionales_InquilinoId");

            builder.HasIndex(r => new { r.InquilinoId, r.NivelDeImpacto })
                .HasDatabaseName("IX_RiesgosOperacionales_InquilinoId_NivelDeImpacto");

            builder.HasIndex(r => r.FechaUltimaRevision)
                .HasDatabaseName("IX_RiesgosOperacionales_FechaUltimaRevision");

            // --- Ignorar propiedades de dominio ---

            builder.Ignore(r => r.DomainEvents);
        }
    }
}
