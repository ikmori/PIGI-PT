using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIGI_PT_Domain.Aggregates.Usuario;
using PIGI_PT_Domain.ValueObjects;

namespace PIGI_PT_Infraestructure.Persistence.Configurations
{
    /// <summary>
    /// Configuración de Entity Framework Core para la entidad <see cref="Usuario"/>.
    /// Define el mapeo de propiedades, conversión del Value Object Rol,
    /// índices únicos por inquilino y restricciones de la tabla Usuarios.
    /// </summary>
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            // Tabla
            builder.ToTable("Usuarios");

            // Primary Key
            builder.HasKey(u => u.Id);

            // --- Propiedades escalares ---

            builder.Property(u => u.FullName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(320);

            builder.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Password)
                .IsRequired()
                .HasMaxLength(500);

            // --- Value Object Converter ---

            builder.Property(u => u.Rol)
                .HasConversion(
                    v => v.Valor,       // To database: int
                    v => Rol.Create(v)  // From database: Rol
                )
                .IsRequired()
                .HasColumnName("Rol");

            // --- Departamento Asignado (para operadores) ---
            builder.HasOne<PIGI_PT_Domain.Aggregates.Categoria.Categoria>()
                .WithMany()
                .HasForeignKey(u => u.DepartamentoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<PIGI_PT_Domain.Aggregates.Inquilino.Inquilino>()
                .WithMany()
                .HasForeignKey(u => u.InquilinoId)
                .OnDelete(DeleteBehavior.Cascade);

            // --- Multi-tenant ---

            builder.Property(u => u.InquilinoId).IsRequired();

            // --- Auditoría (heredadas de BaseEntity) ---

            builder.Property(u => u.CreatedAt).IsRequired();
            builder.Property(u => u.CreatedBy);
            builder.Property(u => u.ModifiedAt);
            builder.Property(u => u.ModifiedBy);
            builder.Property(u => u.IsActive).IsRequired();

            // --- Índices ---

            // Email único por inquilino
            builder.HasIndex(u => new { u.InquilinoId, u.Email })
                .IsUnique()
                .HasDatabaseName("IX_Usuarios_InquilinoId_Email");

            // UserName único por inquilino
            builder.HasIndex(u => new { u.InquilinoId, u.UserName })
                .IsUnique()
                .HasDatabaseName("IX_Usuarios_InquilinoId_UserName");

            builder.HasIndex(u => u.InquilinoId)
                .HasDatabaseName("IX_Usuarios_InquilinoId");

            // --- Ignorar propiedades de dominio ---

            builder.Ignore(u => u.DomainEvents);
        }
    }
}

