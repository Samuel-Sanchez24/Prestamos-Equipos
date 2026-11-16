using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Prestamos.Domain.Entities;

namespace Prestamos.Persistence.Configurations;

public class SolicitudConfiguration : IEntityTypeConfiguration<Solicitud>
{
    public void Configure(EntityTypeBuilder<Solicitud> builder)
    {
        builder.ToTable("Solicitudes", t =>
            t.HasCheckConstraint("CK_Solicitudes_Fechas", "[FechaFin] > [FechaInicio]"));
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Motivo).HasMaxLength(300).IsRequired();
        builder.Property(s => s.Estado).HasConversion<string>().HasMaxLength(20);
        builder.Property(s => s.MotivoRechazo).HasMaxLength(300);

        builder.HasIndex(s => new { s.Estado, s.FechaInicio, s.FechaFin });

        builder.HasOne(s => s.Solicitante)
               .WithMany(u => u.Solicitudes)
               .HasForeignKey(s => s.SolicitanteId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.EvaluadaPor)
               .WithMany()
               .HasForeignKey(s => s.EvaluadaPorId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.EntregadaPor)
               .WithMany()
               .HasForeignKey(s => s.EntregadaPorId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}