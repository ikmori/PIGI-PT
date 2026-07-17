using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIGI_PT_Domain.Aggregates.Categoria;

namespace PIGI_PT_Infraestructure.Persistence.Configurations
{
    /// <summary>
    /// Configuración de Entity Framework Core para la entidad <see cref="Categoria"/>.
    /// Define el mapeo de propiedades, índice único por nombre dentro del inquilino
    /// y restricciones de la tabla Categorias.
    /// </summary>
    public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
    {
        public void Configure(EntityTypeBuilder<Categoria> builder)
        {
            // Tabla
            builder.ToTable("Categorias");

            // Primary Key
            builder.HasKey(c => c.Id);

            // --- Propiedades escalares ---

            builder.Property(c => c.NombreCategoria)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(c => c.Descripcion)
                .HasMaxLength(500);

            // --- Multi-tenant ---

            builder.HasOne<PIGI_PT_Domain.Aggregates.Inquilino.Inquilino>()
                .WithMany()
                .HasForeignKey(c => c.InquilinoId)
                .OnDelete(DeleteBehavior.Cascade);

            // --- Auditoría (heredadas de BaseEntity) ---

            builder.Property(c => c.CreatedAt).IsRequired();
            builder.Property(c => c.CreatedBy);
            builder.Property(c => c.ModifiedAt);
            builder.Property(c => c.ModifiedBy);
            builder.Property(c => c.IsActive).IsRequired();

            // --- Índices ---

            // Nombre de categoría único por inquilino
            builder.HasIndex(c => new { c.InquilinoId, c.NombreCategoria })
                .IsUnique()
                .HasDatabaseName("IX_Categorias_InquilinoId_NombreCategoria");

            builder.HasIndex(c => c.InquilinoId)
                .HasDatabaseName("IX_Categorias_InquilinoId");

            // --- Ignorar propiedades de dominio ---

            builder.Ignore(c => c.DomainEvents);
        }
    }
}
