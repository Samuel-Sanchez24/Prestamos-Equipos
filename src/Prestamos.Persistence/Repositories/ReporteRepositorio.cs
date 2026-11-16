using Microsoft.EntityFrameworkCore;
using Prestamos.Application.Abstractions;
using Prestamos.Domain.Entities;
using Prestamos.Persistence.Context;

namespace Prestamos.Persistence.Repositories;

public class ReporteRepositorio : IReporteRepositorio
{
    private readonly PrestamosDbContext _db;

    public ReporteRepositorio(PrestamosDbContext db)
    {
        _db = db;
    }

    public Task<List<Solicitud>> ListarSolicitudesAsync(int? categoriaId, CancellationToken ct = default) =>
        _db.Solicitudes
           .AsNoTracking()
           .Include(s => s.Solicitante)
           .Include(s => s.EvaluadaPor)
           .Include(s => s.EntregadaPor)
           .Include(s => s.Detalles).ThenInclude(d => d.Recurso).ThenInclude(r => r.Categoria)
           .Include(s => s.Detalles).ThenInclude(d => d.RecibidoPor)
           .Where(s => categoriaId == null || s.Detalles.Any(d => d.Recurso.CategoriaId == categoriaId))
           .AsSplitQuery()
           .ToListAsync(ct);
}