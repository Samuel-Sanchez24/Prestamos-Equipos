using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Prestamos.Domain.Entities;

namespace Prestamos.Persistence.Configurations;

public class RecursoConfiguration : IEntityTypeConfiguration<Recurso>
{
    public void Configure(EntityTypeBuilder<Recurso> builder)
    {
        builder.ToTable("Recursos");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Codigo).HasMaxLength(20).IsRequired();
        builder.Property(r => r.Nombre).HasMaxLength(100).IsRequired();
        builder.Property(r => r.Descripcion).HasMaxLength(300);
        builder.Property(r => r.Ubicacion).HasMaxLength(100);
        builder.Property(r => r.EstadoOperativo).HasConversion<string>().HasMaxLength(20);

        builder.HasIndex(r => r.Codigo).IsUnique();

        builder.HasOne(r => r.Categoria)
               .WithMany(c => c.Recursos)
               .HasForeignKey(r => r.CategoriaId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}