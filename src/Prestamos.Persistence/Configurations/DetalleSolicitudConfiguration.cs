using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Prestamos.Domain.Entities;

namespace Prestamos.Persistence.Configurations;

public class DetalleSolicitudConfiguration : IEntityTypeConfiguration<DetalleSolicitud>
{
    public void Configure(EntityTypeBuilder<DetalleSolicitud> builder)
    {
        builder.ToTable("DetallesSolicitud");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.ObservacionDevolucion).HasMaxLength(300);

        builder.HasIndex(d => new { d.SolicitudId, d.RecursoId }).IsUnique();

        builder.HasOne(d => d.Solicitud)
               .WithMany(s => s.Detalles)
               .HasForeignKey(d => d.SolicitudId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Recurso)
               .WithMany(r => r.Detalles)
               .HasForeignKey(d => d.RecursoId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.RecibidoPor)
               .WithMany()
               .HasForeignKey(d => d.RecibidoPorId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}