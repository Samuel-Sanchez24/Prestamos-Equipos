using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Prestamos.Domain.Entities;

namespace Prestamos.Persistence.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Nombre).HasMaxLength(100).IsRequired();
        builder.Property(u => u.Documento).HasMaxLength(20).IsRequired();
        builder.Property(u => u.Correo).HasMaxLength(150).IsRequired();
        builder.Property(u => u.PasswordHash).HasMaxLength(500).IsRequired();
        builder.Property(u => u.Rol).HasConversion<string>().HasMaxLength(20);
        builder.Property(u => u.TipoSolicitante).HasConversion<string>().HasMaxLength(20);

        builder.HasIndex(u => u.Correo).IsUnique();
        builder.HasIndex(u => u.Documento).IsUnique();
    }
}