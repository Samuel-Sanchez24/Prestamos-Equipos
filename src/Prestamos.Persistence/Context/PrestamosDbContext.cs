using Microsoft.EntityFrameworkCore;
using Prestamos.Domain.Entities;

namespace Prestamos.Persistence.Context;

public class PrestamosDbContext : DbContext
{
    public PrestamosDbContext(DbContextOptions<PrestamosDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Recurso> Recursos => Set<Recurso>();
    public DbSet<Solicitud> Solicitudes => Set<Solicitud>();
    public DbSet<DetalleSolicitud> DetallesSolicitud => Set<DetalleSolicitud>();
    public DbSet<Prorroga> Prorrogas => Set<Prorroga>();
    public DbSet<Movimiento> Movimientos => Set<Movimiento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PrestamosDbContext).Assembly);
    }
}