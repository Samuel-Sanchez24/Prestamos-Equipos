using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Prestamos.Domain.Entities;

namespace Prestamos.Persistence.Configurations;

public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.ToTable("Categorias");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nombre).HasMaxLength(60).IsRequired();
        builder.HasIndex(c => c.Nombre).IsUnique();
    }
}