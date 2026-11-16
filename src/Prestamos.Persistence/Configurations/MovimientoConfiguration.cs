using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Prestamos.Domain.Entities;

namespace Prestamos.Persistence.Configurations;

public class MovimientoConfiguration : IEntityTypeConfiguration<Movimiento>
{
    public void Configure(EntityTypeBuilder<Movimiento> builder)
    {
        builder.ToTable("Movimientos");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Entidad).HasMaxLength(20).IsRequired();
        builder.Property(m => m.EstadoAnterior).HasMaxLength(30);
        builder.Property(m => m.EstadoNuevo).HasMaxLength(30).IsRequired();
        builder.Property(m => m.Observacion).HasMaxLength(300);

        builder.HasIndex(m => new { m.Entidad, m.EntidadId });

        builder.HasOne(m => m.Usuario)
               .WithMany()
               .HasForeignKey(m => m.UsuarioId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}