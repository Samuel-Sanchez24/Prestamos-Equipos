using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Prestamos.Domain.Entities;

namespace Prestamos.Persistence.Configurations;

public class ProrrogaConfiguration : IEntityTypeConfiguration<Prorroga>
{
    public void Configure(EntityTypeBuilder<Prorroga> builder)
    {
        builder.ToTable("Prorrogas", t =>
            t.HasCheckConstraint("CK_Prorrogas_Fechas", "[FechaFinSolicitada] > [FechaFinAnterior]"));
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Motivo).HasMaxLength(300).IsRequired();
        builder.Property(p => p.Estado).HasConversion<string>().HasMaxLength(20);
        builder.Property(p => p.MotivoRechazo).HasMaxLength(300);

        builder.HasOne(p => p.Solicitud)
               .WithMany(s => s.Prorrogas)
               .HasForeignKey(p => p.SolicitudId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.EvaluadaPor)
               .WithMany()
               .HasForeignKey(p => p.EvaluadaPorId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}