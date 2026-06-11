using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIGI_PT_Domain.Aggregates.Inquilino;
using PIGI_PT_Domain.ValueObjects;

namespace PIGI_PT_Infraestructure.Persistence.Configurations
{
    /// <summary>
    /// Configuración de Entity Framework Core para la entidad <see cref="Inquilino"/>.
    /// Define el mapeo de propiedades, conversión del Value Object EstadoInquilino,
    /// índices únicos y restricciones de la tabla Inquilinos.
    /// </summary>
    public class InquilinoConfiguration : IEntityTypeConfiguration<Inquilino>
    {
        public void Configure(EntityTypeBuilder<Inquilino> builder)
        {
            // Tabla
            builder.ToTable("Inquilinos");

            // Primary Key
            builder.HasKey(i => i.Id);

            // --- Propiedades escalares ---

            builder.Property(i => i.NombreComercial)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(i => i.DominioRed)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(i => i.PermitirIA)
                .IsRequired()
                .HasDefaultValue(false);

            // --- Value Object Converter ---

            builder.Property(i => i.Estado)
                .HasConversion(
                    v => v.Valor,                   // To database: string
                    v => EstadoInquilino.Create(v)  // From database: EstadoInquilino
                )
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("Estado");

            // --- Auditoría (heredadas de BaseEntity) ---

            builder.Property(i => i.CreatedAt).IsRequired();
            builder.Property(i => i.CreatedBy);
            builder.Property(i => i.ModifiedAt);
            builder.Property(i => i.ModifiedBy);
            builder.Property(i => i.IsActive).IsRequired();

            // --- Índices ---

            builder.HasIndex(i => i.DominioRed)
                .IsUnique()
                .HasDatabaseName("IX_Inquilinos_DominioRed");

            builder.HasIndex(i => i.NombreComercial)
                .HasDatabaseName("IX_Inquilinos_NombreComercial");

            // --- Ignorar propiedades de dominio ---

            builder.Ignore(i => i.DomainEvents);
        }
    }
}
